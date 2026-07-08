using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Plennusc.Core.Models.ModelsGestao.modelsBilling;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.billing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.serviceBilling
{
    public class ServiceBillingReconciliation
    {
        private readonly SqlBillingReconciliation _sql = new SqlBillingReconciliation();
        private readonly ServiceBillingReconciliationHapvida _hapvida = new ServiceBillingReconciliationHapvida();

        public List<OperadoraModel> ObterOperadoras()
        {
            return _sql.BuscarOperadoras();
        }

        public List<GrupoFaturamentoModel> ObterGruposFaturamento()
        {
            return _sql.BuscarGruposFaturamento();
        }


        #region TRATAMENTOS E CHAMADAS HAPVIDDA
        // Recebe o nome da operadora que já veio selecionado em tela (ddlOperadora.SelectedItem.Text)
        public List<ItemRelatorioImportadoHapVida> ProcessarRelatorioImportado(string nomeOperadora, Stream arquivo, string extensao)
        {
            if (nomeOperadora.IndexOf("HAPVIDA", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return _hapvida.LerRelatorio(arquivo, extensao);
            }

            throw new NotSupportedException($"Ainda não existe leitura implementada para a operadora '{nomeOperadora}'.");
        }

        public List<ItemRelatorioImportadoHapVida> ConferirComView(string nomeOperadora, List<ItemRelatorioImportadoHapVida> itensImportados, string tipoConferencia, int codigoGrupoContrato)
        {
            if (nomeOperadora.IndexOf("HAPVIDA", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return _hapvida.ConferirComView(itensImportados, tipoConferencia, codigoGrupoContrato);
            }
            throw new NotSupportedException($"Ainda não existe conferência implementada para a operadora '{nomeOperadora}'.");
        }
        #endregion


        public byte[] ExportarDivergentesExcel(List<ItemRelatorioImportadoHapVida> divergentes)
        {
            using (var stream = new MemoryStream())
            {
                using (var doc = SpreadsheetDocument.Create(stream, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = doc.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    sheets.Append(new Sheet
                    {
                        Id = workbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Divergentes"
                    });

                    // Cabeçalho
                    var headerRow = new Row();
                    string[] colunas = { "Empresa", "Beneficiário", "CPF", "Credencial", "Mês/Ano Referência",
                "Cobrado", "Valor Operadora", "Diferença", "Status" };
                    foreach (var col in colunas)
                        headerRow.Append(CriarCelulaTexto(col));
                    sheetData.Append(headerRow);

                    // Linhas
                    foreach (var item in divergentes)
                    {
                        var row = new Row();
                        row.Append(CriarCelulaTexto(item.Empresa));
                        row.Append(CriarCelulaTexto(item.Beneficiario));
                        row.Append(CriarCelulaTexto(item.Cpf));
                        row.Append(CriarCelulaTexto(item.Credencial));
                        row.Append(CriarCelulaTexto(item.MesAnoReferencia));
                        row.Append(CriarCelulaTexto(item.Cobrado.ToString("N2")));
                        row.Append(CriarCelulaTexto(item.ValorOperadoraView?.ToString("N2") ?? ""));
                        row.Append(CriarCelulaTexto(item.DiferencaValor?.ToString("N2") ?? ""));
                        row.Append(CriarCelulaTexto("Divergente"));
                        sheetData.Append(row);
                    }

                    workbookPart.Workbook.Save();
                }

                return stream.ToArray();
            }
        }

        private Cell CriarCelulaTexto(string valor)
        {
            return new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(valor ?? "")
            };
        }
    }
}
