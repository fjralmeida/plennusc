using Plennusc.Core.Models.ModelsGestao.modelsCIDs;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.dataCIDs;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Plennusc.Core.Service.ServiceGestao.CIDsService
{
    public class serviceCIDs
    {
        private readonly CIDsData _data;

        public serviceCIDs(string connectionString)
        {
            _data = new CIDsData(connectionString);
        }

        public List<CIDsImportResultModel> ProcessarImportacao(Stream excelStream, DateTime vigenciaObrigatoria)
        {
            var linhas = LerExcel(excelStream);
            var resultados = new List<CIDsImportResultModel>();

            using (var conn = _data.AbrirConexao())
            {
                foreach (var linha in linhas)
                {
                    if (string.IsNullOrWhiteSpace(linha.Cid))
                        continue;

                    var item = new CIDsImportResultModel
                    {
                        LinhaCsv = linha.LinhaCsv,
                        Cpf = linha.Cpf,
                        Titular = linha.Titular,
                        Beneficiario = linha.Beneficiario,
                        Cid = linha.Cid
                    };

                    if (string.IsNullOrWhiteSpace(linha.Cpf))
                    {
                        item.Sucesso = false;
                        item.Motivo = "CPF não informado na planilha.";
                        resultados.Add(item);
                        continue;
                    }

                    var associado = _data.BuscarAssociadoPorCpf(conn, linha.Cpf);

                    if (associado == null)
                    {
                        item.Sucesso = false;
                        item.Motivo = "CPF não encontrado na PS1000.";
                        resultados.Add(item);
                        continue;
                    }

                    item.CodigoAssociado = associado.CodigoAssociado;

                    if (!associado.DataAdmissao.HasValue ||
                        associado.DataAdmissao.Value.Date != vigenciaObrigatoria.Date)
                    {
                        item.Sucesso = false;
                        item.Motivo = $"Data de admissão ({associado.DataAdmissao:dd/MM/yyyy}) não confere com a vigência informada ({vigenciaObrigatoria:dd/MM/yyyy}).";
                        resultados.Add(item);
                        continue;
                    }

                    if (_data.JaExisteRegistro(conn, associado.CodigoAssociado, linha.Cid))
                    {
                        item.Sucesso = false;
                        item.Motivo = "Já cadastrado no sistema (PS1009).";
                        resultados.Add(item);
                        continue;
                    }

                    try
                    {
                        var registro = new CIDsRegistroInsertModel
                        {
                            CodigoAssociado = associado.CodigoAssociado,
                            CodigoCid = linha.Cid,
                            DataTermino = linha.Vigencia,
                            ReferenciaImportacao = "IMPORT_XLSX_" + DateTime.Now.ToString("yyyyMMddHHmmss"),
                            InformacoesLogI = $"Importado em {DateTime.Now:dd/MM/yyyy HH:mm:ss} - CPF: {linha.Cpf}",
                            InformacoesLogA = null,
                            IdInstanciaProcesso = null
                        };

                        _data.InserirRegistro(conn, registro);

                        item.Sucesso = true;
                        item.Motivo = "Importado com sucesso.";
                    }
                    catch (Exception ex)
                    {
                        item.Sucesso = false;
                        item.Motivo = "Erro ao inserir: " + ex.Message;
                    }

                    resultados.Add(item);
                }
            }

            return resultados;
        }

        // =================================================================
        // LEITURA DE .XLSX SEM NENHUMA DEPENDÊNCIA EXTERNA
        // Usa apenas System.IO.Compression e System.Xml (nativos do .NET Framework)
        // Um .xlsx é um .zip contendo XMLs internos:
        //   - xl/sharedStrings.xml  -> tabela de strings compartilhadas
        //   - xl/worksheets/sheet1.xml -> dados da primeira planilha
        // =================================================================
        private List<CIDsCsvRowModel> LerExcel(Stream excelStream)
        {
            var linhas = new List<CIDsCsvRowModel>();

            // ZipArchive precisa de um stream com posição no início e, de preferência, seekable
            var memoryStream = new MemoryStream();
            excelStream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
            {
                var sharedStrings = LerSharedStrings(archive);

                var sheetEntry = archive.GetEntry("xl/worksheets/sheet1.xml");
                if (sheetEntry == null)
                    throw new InvalidOperationException("Não foi possível localizar a planilha (sheet1.xml) dentro do arquivo Excel.");

                using (var sheetStream = sheetEntry.Open())
                {
                    var doc = XDocument.Load(sheetStream);
                    XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

                    var rows = doc.Descendants(ns + "row").ToList();

                    // Assume primeira linha = cabeçalho
                    bool primeiraLinha = true;
                    int linhaAtual = 0;

                    foreach (var row in rows)
                    {
                        linhaAtual++;

                        if (primeiraLinha)
                        {
                            primeiraLinha = false;
                            continue; // pula cabeçalho
                        }

                        var valoresPorColuna = new Dictionary<int, string>();

                        foreach (var cell in row.Elements(ns + "c"))
                        {
                            var refAttr = cell.Attribute("r")?.Value; // ex: "A2", "J2"
                            if (string.IsNullOrEmpty(refAttr))
                                continue;

                            int colIndex = ColunaParaIndice(refAttr);
                            string valor = LerValorCelula(cell, sharedStrings, ns);

                            valoresPorColuna[colIndex] = valor;
                        }

                        if (valoresPorColuna.Count == 0)
                            continue;

                        var linha = new CIDsCsvRowModel
                        {
                            LinhaCsv = linhaAtual,
                            Operadora = ObterValor(valoresPorColuna, 0),
                            Vigencia = ParseData(ObterValor(valoresPorColuna, 1)),
                            Titular = ObterValor(valoresPorColuna, 2),
                            Beneficiario = ObterValor(valoresPorColuna, 3),
                            Cpf = LimparCpf(ObterValor(valoresPorColuna, 4)),
                            Proposta = ObterValor(valoresPorColuna, 5),
                            Data = ParseData(ObterValor(valoresPorColuna, 6)),
                            Horario = ObterValor(valoresPorColuna, 7),
                            DoencaOuLesaoPreexistente = ObterValor(valoresPorColuna, 8),
                            Cid = ObterValor(valoresPorColuna, 9),
                            Video = ObterValor(valoresPorColuna, 10),
                            Enfermeiro = ObterValor(valoresPorColuna, 11),
                            ParecerTecnico = ObterValor(valoresPorColuna, 12),
                            Observacao = ObterValor(valoresPorColuna, 13),
                            Pendencias = ObterValor(valoresPorColuna, 14)
                        };

                        // Ignora linha se todos os campos relevantes estiverem vazios
                        if (string.IsNullOrWhiteSpace(linha.Cpf) && string.IsNullOrWhiteSpace(linha.Cid) && string.IsNullOrWhiteSpace(linha.Titular))
                            continue;

                        linhas.Add(linha);
                    }
                }
            }

            return linhas;
        }

        private Dictionary<int, string> LerSharedStrings(ZipArchive archive)
        {
            var dict = new Dictionary<int, string>();

            var entry = archive.GetEntry("xl/sharedStrings.xml");
            if (entry == null)
                return dict; // planilha pode não ter strings compartilhadas (só números/datas)

            using (var stream = entry.Open())
            {
                var doc = XDocument.Load(stream);
                XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";

                int index = 0;
                foreach (var si in doc.Descendants(ns + "si"))
                {
                    // Pode ter texto simples <t> ou texto rico <r><t>
                    var texto = string.Concat(si.Descendants(ns + "t").Select(t => t.Value));
                    dict[index] = texto;
                    index++;
                }
            }

            return dict;
        }

        private string LerValorCelula(XElement cell, Dictionary<int, string> sharedStrings, XNamespace ns)
        {
            var tipo = cell.Attribute("t")?.Value; // "s" = shared string, "str" = fórmula string, ausente = número/data
            var valorElemento = cell.Element(ns + "v");

            if (valorElemento == null)
            {
                // Pode ser texto inline <is><t>...</t></is>
                var inlineStr = cell.Element(ns + "is")?.Element(ns + "t")?.Value;
                return inlineStr ?? string.Empty;
            }

            var valorBruto = valorElemento.Value;

            if (tipo == "s")
            {
                if (int.TryParse(valorBruto, out var idx) && sharedStrings.TryGetValue(idx, out var texto))
                    return texto;
                return string.Empty;
            }

            return valorBruto;
        }

        /// <summary>
        /// Converte referência de célula tipo "A2", "J15" no índice numérico da coluna (0-based).
        /// </summary>
        private int ColunaParaIndice(string cellRef)
        {
            int indice = 0;
            foreach (char c in cellRef)
            {
                if (!char.IsLetter(c))
                    break;

                indice = indice * 26 + (char.ToUpper(c) - 'A' + 1);
            }
            return indice - 1; // 0-based
        }

        private string ObterValor(Dictionary<int, string> valores, int coluna)
        {
            return valores.TryGetValue(coluna, out var v) ? v.Trim() : string.Empty;
        }

        private DateTime? ParseData(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return null;

            // Excel guarda datas como número serial (dias desde 30/12/1899)
            if (double.TryParse(valor, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var serial))
            {
                try
                {
                    return DateTime.FromOADate(serial);
                }
                catch
                {
                    // não era serial válido, continua tentando como texto
                }
            }

            if (DateTime.TryParse(valor, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var data))
                return data;

            if (DateTime.TryParseExact(valor.Trim(), new[] { "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss", "yyyy-MM-dd" },
                System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out data))
                return data;

            return null;
        }

        private string LimparCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return string.Empty;
            return new string(cpf.Where(char.IsDigit).ToArray());
        }
    }
}