using Plennusc.Core.Models.ModelsGestao.modelsCardsImport;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.dataCardsImport;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Linq;

namespace Plennusc.Core.Service.ServiceGestao.CardsImportService
{
    public class serviceCardsImport
    {
        private readonly CardsImportData _data;

        public serviceCardsImport(string connectionString)
        {
            _data = new CardsImportData(connectionString);
        }

        public List<CardsImportResultModel> ProcessarImportacao(Stream excelStream)
        {
            var linhas = LerExcel(excelStream);
            var resultados = new List<CardsImportResultModel>();

            using (var conn = _data.AbrirConexao())
            {
                foreach (var linha in linhas)
                {
                    var item = new CardsImportResultModel
                    {
                        LinhaCsv = linha.LinhaCsv,
                        Cpf = linha.NumeroCpf,
                        NumeroCarteirinha = linha.NumeroCarteirinha,
                        DataAdmissaoPlanilha = linha.DataAdmissao
                    };

                    if (string.IsNullOrWhiteSpace(linha.NumeroCpf))
                    {
                        item.Sucesso = false;
                        item.Motivo = "CPF não informado na planilha.";
                        resultados.Add(item);
                        continue;
                    }

                    if (linha.NumeroCpf.Length != 11)
                    {
                        item.Sucesso = false;
                        item.Motivo = $"CPF inválido. Possui {linha.NumeroCpf.Length} dígito(s). O CPF deve ter 11 dígitos.";
                        resultados.Add(item);
                        continue;
                    }

                    if (!TryParseData(linha.DataAdmissao, out DateTime dataAdmissaoPlanilha))
                    {
                        item.Sucesso = false;
                        item.Motivo = $"Data de Admissão inválida ou em formato incorreto: '{linha.DataAdmissao}'. Use dd/MM/yyyy.";
                        resultados.Add(item);
                        continue;
                    }

                    if (!TryParseData(linha.DataEmissaoCarteirinha, out DateTime dataEmissaoCarteirinha))
                    {
                        item.Sucesso = false;
                        item.Motivo = $"Data de Emissão inválida: '{linha.DataEmissaoCarteirinha}'. Use dd/MM/yyyy.";
                        resultados.Add(item);
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(linha.NumeroCarteirinha))
                    {
                        item.Sucesso = false;
                        item.Motivo = "Número da Carteirinha não informado.";
                        resultados.Add(item);
                        continue;
                    }

                    var todosDoCpf = _data.BuscarTodosPorCpf(conn, linha.NumeroCpf);

                    if (todosDoCpf == null || todosDoCpf.Count == 0)
                    {
                        item.Sucesso = false;
                        item.Motivo = "CPF não encontrado na PS1000.";
                        resultados.Add(item);
                        continue;
                    }

                    var ativos = todosDoCpf.Where(a => a.DataExclusao == null).ToList();

                    if (ativos.Count == 0)
                    {
                        item.Sucesso = false;
                        item.Motivo = "CPF não encontrado na PS1000.";
                        resultados.Add(item);
                        continue;
                    }

                    if (ativos.Count > 1)
                    {
                        item.Sucesso = false;
                        item.Motivo = "Erro: Múltiplos registros ativos (DATA_EXCLUSAO nula) encontrados para este CPF.";
                        resultados.Add(item);
                        continue;
                    }

                    var associado = ativos.First();

                    item.CodigoAssociado = associado.CodigoAssociado;
                    item.NomeAssociado = associado.Nome;
                    item.DataAdmissaoBanco = associado.DataAdmissao?.ToString("dd/MM/yyyy");

                    if (!associado.DataAdmissao.HasValue || associado.DataAdmissao.Value.Date != dataAdmissaoPlanilha.Date)
                    {
                        item.Sucesso = false;
                        item.Motivo = $"Data de Admissão não confere. Planilha: {dataAdmissaoPlanilha:dd/MM/yyyy} | PS1000: {associado.DataAdmissao:dd/MM/yyyy}.";
                        resultados.Add(item);
                        continue;
                    }

                    try
                    {
                        _data.AtualizarCarteirinhaPs1000(conn, associado.CodigoAssociado, linha.NumeroCarteirinha, dataEmissaoCarteirinha);
                        _data.AtualizarObservacaoPs1007(conn, associado.CodigoAssociado, linha.ObservacaoCadastro);

                        item.Sucesso = true;
                        item.Motivo = ""; // Sucesso não retorna motivo
                    }
                    catch (Exception ex)
                    {
                        item.Sucesso = false;
                        item.Motivo = ex.Message;
                    }

                    resultados.Add(item);
                }
            }

            return resultados;
        }

        private bool TryParseData(string valor, out DateTime data)
        {
            data = default;
            if (string.IsNullOrWhiteSpace(valor)) return false;
            valor = valor.Trim();

            if (double.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out var serial))
            {
                try { data = DateTime.FromOADate(serial); return true; } catch { }
            }

            string[] formatos = { "dd/MM/yyyy", "dd/MM/yyyy HH:mm:ss", "dd/MM/yy", "yyyy-MM-dd" };
            if (DateTime.TryParseExact(valor, formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out data))
                return true;

            return DateTime.TryParse(valor, new CultureInfo("pt-BR"), DateTimeStyles.None, out data);
        }

        private List<CardsImportCsvRowModel> LerExcel(Stream excelStream)
        {
            var linhas = new List<CardsImportCsvRowModel>();
            var memoryStream = new MemoryStream();
            excelStream.CopyTo(memoryStream);
            memoryStream.Position = 0;

            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
            {
                var sharedStrings = LerSharedStrings(archive);
                var sheetEntry = archive.GetEntry("xl/worksheets/sheet1.xml");

                if (sheetEntry == null) throw new InvalidOperationException("Planilha (sheet1.xml) não encontrada.");

                using (var sheetStream = sheetEntry.Open())
                {
                    var doc = XDocument.Load(sheetStream);
                    XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
                    var rows = doc.Descendants(ns + "row").ToList();

                    bool primeiraLinha = true;
                    int linhaAtual = 0;

                    foreach (var row in rows)
                    {
                        linhaAtual++;
                        if (primeiraLinha) { primeiraLinha = false; continue; }

                        var valores = new Dictionary<int, string>();
                        foreach (var cell in row.Elements(ns + "c"))
                        {
                            var refAttr = cell.Attribute("r")?.Value;
                            if (string.IsNullOrEmpty(refAttr)) continue;
                            valores[ColunaParaIndice(refAttr)] = LerValorCelula(cell, sharedStrings, ns);
                        }

                        if (valores.Count == 0) continue;

                        var linhaModel = new CardsImportCsvRowModel
                        {
                            LinhaCsv = linhaAtual,
                            NumeroCpf = LimparCpf(ObterValor(valores, 0)),
                            DataAdmissao = FormatarDataExcel(ObterValor(valores, 1)),
                            NumeroCarteirinha = ObterValor(valores, 2),
                            DataEmissaoCarteirinha = FormatarDataExcel(ObterValor(valores, 3)),
                            ObservacaoCadastro = ObterValor(valores, 4)
                        };

                        if (string.IsNullOrWhiteSpace(linhaModel.NumeroCpf) && string.IsNullOrWhiteSpace(linhaModel.NumeroCarteirinha))
                            continue;

                        linhas.Add(linhaModel);
                    }
                }
            }
            return linhas;
        }

        private Dictionary<int, string> LerSharedStrings(ZipArchive archive)
        {
            var dict = new Dictionary<int, string>();
            var entry = archive.GetEntry("xl/sharedStrings.xml");
            if (entry == null) return dict;

            using (var stream = entry.Open())
            {
                var doc = XDocument.Load(stream);
                XNamespace ns = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
                int index = 0;
                foreach (var si in doc.Descendants(ns + "si"))
                {
                    dict[index] = string.Concat(si.Descendants(ns + "t").Select(t => t.Value));
                    index++;
                }
            }
            return dict;
        }

        private string LerValorCelula(XElement cell, Dictionary<int, string> sharedStrings, XNamespace ns)
        {
            var tipo = cell.Attribute("t")?.Value;
            var valorElemento = cell.Element(ns + "v");

            if (valorElemento == null)
                return cell.Element(ns + "is")?.Element(ns + "t")?.Value ?? string.Empty;

            var valorBruto = valorElemento.Value;

            if (tipo == "s" && int.TryParse(valorBruto, out var idx) && sharedStrings.TryGetValue(idx, out var texto))
                return texto;

            return valorBruto;
        }

        private int ColunaParaIndice(string cellRef)
        {
            int indice = 0;
            foreach (char c in cellRef)
            {
                if (!char.IsLetter(c)) break;
                indice = indice * 26 + (char.ToUpper(c) - 'A' + 1);
            }
            return indice - 1;
        }

        private string ObterValor(Dictionary<int, string> valores, int coluna)
        {
            return valores.TryGetValue(coluna, out var v) ? v.Trim() : string.Empty;
        }

        private string LimparCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return string.Empty;

            var numeros = new string(cpf.Where(char.IsDigit).ToArray());

            if (numeros.Length > 0 && numeros.Length < 11)
            {
                numeros = numeros.PadLeft(11, '0');
            }

            return numeros;
        }

        private string FormatarDataExcel(string valorOriginal)
        {
            if (string.IsNullOrWhiteSpace(valorOriginal))
                return valorOriginal;

            // Se for um número (serial do Excel), converte para data
            if (double.TryParse(valorOriginal, NumberStyles.Any, CultureInfo.InvariantCulture, out var serial))
            {
                try
                {
                    return DateTime.FromOADate(serial).ToString("dd/MM/yyyy");
                }
                catch { }
            }

            return valorOriginal;
        }
    }
}