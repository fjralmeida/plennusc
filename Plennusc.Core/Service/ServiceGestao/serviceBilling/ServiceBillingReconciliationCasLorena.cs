using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Plennusc.Core.Models.ModelsGestao.modelsBilling;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.billing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Plennusc.Core.Service.ServiceGestao.serviceBilling
{
    public class ServiceBillingReconciliationCasLorena
    {
        private readonly SqlBillingReconciliation _sql = new SqlBillingReconciliation();
        private const decimal TOLERANCIA = 0.10m;

        #region LEITURA

        /// <summary>
        /// Lê somente as linhas do tipo PLANO. Linhas TARIFA são ignoradas.
        /// Cobrado = Valor Operadora (nunca Valor Apurado).
        /// </summary>
        public List<ItemRelatorioImportadoHapVida> LerRelatorio(Stream arquivo, string extensao)
        {
            List<string[]> linhas;
            extensao = (extensao ?? "").ToLower();

            if (extensao == ".xlsx")
                linhas = LerXlsx(arquivo);
            else if (extensao == ".csv" || extensao == ".txt")
                linhas = LerTexto(arquivo);
            else
                throw new NotSupportedException("Para CAS Lorena envie um arquivo .xlsx, .csv ou .txt.");

            if (linhas.Count < 2)
                throw new ArgumentException("O arquivo não possui dados.");

            int idxHeader = linhas.FindIndex(l => l.Any(c => Normalizar(c) == "TITULAR"));
            if (idxHeader < 0)
                throw new ArgumentException("Cabeçalho não encontrado (coluna TITULAR).");

            var header = linhas[idxHeader].Select(Normalizar).ToArray();

            int iCodigo = Array.IndexOf(header, "CODIGO");
            int iTipo = Array.IndexOf(header, "TIPO");
            int iNome = Array.IndexOf(header, "NOME");
            int iCpf = iNome + 1; // coluna do CPF não possui título no layout
            int iDescricao = Array.FindIndex(header, h => h.StartsWith("DESCRICAO"));
            int iValorOperadora = Array.FindIndex(header, h => h == "VALOR OPERADORA");
            int iEmpresa = Array.IndexOf(header, "EMPRESA");

            if (iTipo < 0 || iNome < 0 || iValorOperadora < 0)
                throw new ArgumentException("Layout inválido: colunas TIPO, NOME e Valor Operadora são obrigatórias.");

            var itens = new List<ItemRelatorioImportadoHapVida>();

            foreach (var linha in linhas.Skip(idxHeader + 1))
            {
                // Somente PLANO (tarifa é ignorada)
                string tipo = Normalizar(Obter(linha, iTipo));
                if (tipo != "PLANO") continue;

                string cpf = SomenteDigitos(Obter(linha, iCpf));
                string codigo = Obter(linha, iCodigo).Trim();
                if (string.IsNullOrEmpty(cpf) && string.IsNullOrEmpty(codigo)) continue;

                itens.Add(new ItemRelatorioImportadoHapVida
                {
                    Cpf = cpf,
                    Credencial = codigo,
                    Beneficiario = Obter(linha, iNome).Trim(),
                    Empresa = Obter(linha, iEmpresa).Trim(),
                    Plano = Obter(linha, iDescricao).Trim(),
                    Cobrado = ParseValor(Obter(linha, iValorOperadora)),
                    Adicional = 0m
                });
            }

            return itens;
        }

        private static string Obter(string[] linha, int idx)
        {
            return (idx >= 0 && idx < linha.Length && linha[idx] != null) ? linha[idx] : "";
        }

        private List<string[]> LerTexto(Stream arquivo)
        {
            var resultado = new List<string[]>();
            using (var reader = new StreamReader(arquivo, Encoding.GetEncoding("ISO-8859-1")))
            {
                string linha;
                while ((linha = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(linha)) continue;
                    char sep = linha.Contains('\t') ? '\t' : ';';
                    resultado.Add(linha.Split(sep));
                }
            }
            return resultado;
        }

        private List<string[]> LerXlsx(Stream arquivo)
        {
            var resultado = new List<string[]>();

            using (var doc = SpreadsheetDocument.Open(arquivo, false))
            {
                var wbPart = doc.WorkbookPart;
                var sheet = wbPart.Workbook.Descendants<Sheet>().First();
                var wsPart = (WorksheetPart)wbPart.GetPartById(sheet.Id);
                var sst = wbPart.SharedStringTablePart?.SharedStringTable;

                foreach (var row in wsPart.Worksheet.Descendants<Row>())
                {
                    var valores = new List<string>();
                    foreach (var cell in row.Elements<Cell>())
                    {
                        int col = IndiceColuna(cell.CellReference?.Value);
                        while (valores.Count < col) valores.Add("");

                        string texto = cell.CellValue?.Text ?? cell.InnerText ?? "";
                        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString && sst != null)
                            texto = sst.ElementAt(int.Parse(texto)).InnerText;

                        valores.Add(texto);
                    }
                    if (valores.Any(v => !string.IsNullOrWhiteSpace(v)))
                        resultado.Add(valores.ToArray());
                }
            }
            return resultado;
        }

        private static int IndiceColuna(string cellRef)
        {
            if (string.IsNullOrEmpty(cellRef)) return 0;
            int idx = 0;
            foreach (char c in cellRef)
            {
                if (!char.IsLetter(c)) break;
                idx = idx * 26 + (char.ToUpper(c) - 'A' + 1);
            }
            return idx - 1;
        }

        private static decimal ParseValor(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto)) return 0m;

            texto = texto.Replace("R$", "").Replace(" ", "").Replace("\u00A0", "").Trim();

            if (texto.Contains(","))
                texto = texto.Replace(".", "").Replace(",", ".");

            decimal v;
            return decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out v) ? v : 0m;
        }

        private static string SomenteDigitos(string v)
        {
            return string.IsNullOrWhiteSpace(v) ? "" : new string(v.Where(char.IsDigit).ToArray());
        }

        private static string Normalizar(string v)
        {
            if (string.IsNullOrWhiteSpace(v)) return "";
            var formD = v.Trim().ToUpperInvariant().Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (char c in formD)
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            return Regex.Replace(sb.ToString(), @"\s+", " ");
        }

        #endregion

        #region CONFERÊNCIA

        /// <summary>
        /// Confere o Valor Operadora (Plano) contra o CONVÊNIO da view.
        /// </summary>
        public List<ItemRelatorioImportadoHapVida> ConferirComView(
            List<ItemRelatorioImportadoHapVida> itens,
            string tipoConferencia,
            int codigoGrupoContrato)
        {
            foreach (var item in itens)
            {
                if (string.IsNullOrEmpty(item.Cpf) || string.IsNullOrEmpty(item.MesAnoReferencia))
                {
                    item.StatusConferencia = "NAO_ENCONTRADO";
                    continue;
                }

                var convenio = _sql.BuscarDadosConvenioPorCpfEGrupo(item.Cpf, item.MesAnoReferencia, codigoGrupoContrato);

                if (convenio == null || !convenio.ValorOperadora.HasValue)
                {
                    item.StatusConferencia = "NAO_ENCONTRADO";
                    continue;
                }

                item.DataAdmissao = convenio.DataAdmissao;
                item.DataExclusao = convenio.DataExclusao;
                item.NomeMotivoExclusao = convenio.NomeMotivoExclusao;
                item.NomeTabelaPreco = convenio.NomeTabelaPreco;
                item.NomeGrupoPessoas = convenio.NomeGrupoPessoas;
                item.DescricaoGrupoFaturamento = convenio.DescricaoGrupoFaturamento;

                decimal valorView = convenio.ValorOperadora.Value;
                decimal diferenca = item.Cobrado - valorView;
                decimal abs = Math.Abs(diferenca);

                item.ValorOperadoraView = valorView;
                item.DiferencaValor = diferenca;

                if (abs == 0m) item.StatusConferencia = "OK";
                else if (abs <= TOLERANCIA) item.StatusConferencia = "DIVERGENCIA_TOLERADA";
                else item.StatusConferencia = "DIVERGENTE";
            }

            return itens;
        }

        #endregion
    }
}