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
using DocumentFormat.OpenXml;
using X = DocumentFormat.OpenXml.Spreadsheet;


namespace Plennusc.Core.Service.ServiceGestao.serviceBilling
{
    public class ServiceBillingReconciliation
    {
        private readonly SqlBillingReconciliation _sql = new SqlBillingReconciliation();
        private readonly ServiceBillingReconciliationHapvida _hapvida = new ServiceBillingReconciliationHapvida();
        private readonly ServiceBillingReconciliationUnimed _unimed = new ServiceBillingReconciliationUnimed();

        #region CHAMADA DE SQL PARA OBTER TODAS AS OPERADORAS E GRUPOS DE FATURAMENTO
        public List<OperadoraModel> ObterOperadoras()
        {
            return _sql.BuscarOperadoras();
        }
        #endregion
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

            if (nomeOperadora.IndexOf("UNIMED", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return _unimed.LerRelatorioUnimed(arquivo, extensao)
                    .Select(u => new ItemRelatorioImportadoHapVida
                    {
                        Credencial = u.Credencial,
                        Beneficiario = u.NomeBeneficiario,
                        Plano = u.Descricao,
                        Cobrado = u.ValorOperadora,
                        Cpf = u.Cpf
                    })
                    .ToList();
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

        public void ConferirFaturamento(List<ItemRelatorioImportadoHapVida> itensConferidos)
        {
            var itensParaAtualizar = itensConferidos
                .Where(i => i.StatusConferencia == "OK" || i.StatusConferencia == "DIVERGENCIA_TOLERADA")
                .ToList();

            if (itensParaAtualizar.Count == 0)
                return;

            _sql.ConferirFaturamento(itensParaAtualizar);
        }

        #region LOGICA DE EXPORTAÇÃO PARA EXCEL
        public byte[] ExportarConferenciaExcel(List<ItemRelatorioImportadoHapVida> itens)
        {
            using (var stream = new MemoryStream())
            {
                using (var doc = SpreadsheetDocument.Create(stream, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = doc.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    // Estilos
                    var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    stylesPart.Stylesheet = CriarStylesheet();
                    stylesPart.Stylesheet.Save();

                    var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    sheets.Append(new Sheet
                    {
                        Id = workbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Conferência"
                    });

                    // Cabeçalho com TODAS as colunas do grid
                    var headerRow = new Row();
                    string[] colunas = {
                        "CPF",
                        "Beneficiário",
                        "Nascimento",
                        "Parentesco",
                        "Plano",
                        "Mês/Ano Usado",
                        "Cobrado",
                        "Valor Operadora",
                        "Diferença",
                        "Data Admissão",
                        "Data Exclusão",
                        "Motivo Exclusão",
                        "Tabela de Preço",
                        "Grupo de Pessoas",
                        "Grupo de Faturamento",
                        "Status"
                    };
                    foreach (var col in colunas)
                        headerRow.Append(CriarCelulaTexto(col, 5)); // estilo 5 = cabeçalho
                    sheetData.Append(headerRow);

                    // Linhas — TODOS os itens
                    foreach (var item in itens)
                    {
                        uint estilo = ObterEstiloPorStatus(item.StatusConferencia);

                        var row = new Row();
                        row.Append(CriarCelulaTexto(item.Cpf ?? "", estilo));
                        row.Append(CriarCelulaTexto(item.Beneficiario ?? "", estilo));
                        row.Append(CriarCelulaTexto(item.Nascimento?.ToString("dd/MM/yyyy") ?? "", estilo));
                        row.Append(CriarCelulaTexto(item.Parentesco ?? "", estilo));
                        row.Append(CriarCelulaTexto(item.Plano ?? "", estilo));
                        row.Append(CriarCelulaTexto(item.MesAnoReferencia ?? "", estilo));
                        row.Append(CriarCelulaTexto(item.Cobrado.ToString("N2"), estilo));
                        row.Append(CriarCelulaTexto(item.ValorOperadoraView?.ToString("N2") ?? "", estilo));
                        row.Append(CriarCelulaTexto(item.DiferencaValor?.ToString("N2") ?? "", estilo));
                        row.Append(CriarCelulaTexto(item.DataAdmissao?.ToString("dd/MM/yyyy") ?? "", estilo));
                        row.Append(CriarCelulaTexto(item.DataExclusao?.ToString("dd/MM/yyyy") ?? "", estilo));
                        row.Append(CriarCelulaTexto(item.NomeMotivoExclusao ?? "", estilo));
                        row.Append(CriarCelulaTexto(item.NomeTabelaPreco ?? "", estilo));
                        row.Append(CriarCelulaTexto(item.NomeGrupoPessoas ?? "", estilo));
                        row.Append(CriarCelulaTexto(item.DescricaoGrupoFaturamento ?? "", estilo));
                        row.Append(CriarCelulaTexto(TraduzirStatusExcel(item.StatusConferencia), estilo));
                        sheetData.Append(row);
                    }

                    workbookPart.Workbook.Save();
                }
                return stream.ToArray();
            }
        }

        private string TraduzirStatusExcel(string status)
        {
            switch (status)
            {
                case "OK": return "OK";
                case "DIVERGENCIA_TOLERADA": return "OK (dif. até 10 centavos)";
                case "DIVERGENTE": return "Divergente";
                case "NAO_ENCONTRADO": return "Não encontrado";
                case "CARTEIRINHA_NAO_ENCONTRADA": return "Carteirinha não encontrada";
                default: return status ?? "";
            }
        }


        // Mapeia o status pro índice de estilo (cor) criado no CriarStylesheet
        private uint ObterEstiloPorStatus(string status)
        {
            switch (status)
            {
                case "OK": return 1;                    // verde
                case "DIVERGENCIA_TOLERADA": return 2;   // amarelo
                case "DIVERGENTE": return 3;             // vermelho
                case "CARTEIRINHA_NAO_ENCONTRADA": return 6; // laranja
                case "NAO_ENCONTRADO": return 4;         // cinza
                default: return 0;                       // sem cor
            }
        }

        private X.Stylesheet CriarStylesheet()
        {
            var fills = new X.Fills(
                 new X.Fill(new X.PatternFill { PatternType = X.PatternValues.None }),
                 new X.Fill(new X.PatternFill { PatternType = X.PatternValues.Gray125 }),
                 CriarFillSolido("C8E6C9"), // OK
                 CriarFillSolido("FFE0B2"), // tolerada
                 CriarFillSolido("FFCDD2"), // divergente
                 CriarFillSolido("E0E0E0"), // não encontrado
                 CriarFillSolido("D1C4E9")  // carteirinha não encontrada
             );
            var fonts = new X.Fonts(
                new X.Font(new X.FontSize { Val = 11 }, new X.FontName { Val = "Calibri" }),
                new X.Font(new X.Bold(), new X.FontSize { Val = 11 }, new X.FontName { Val = "Calibri" })
            );
            var borders = new X.Borders(new X.Border());
            var cellFormats = new X.CellFormats(
                 new X.CellFormat(),                                    // 0
                 new X.CellFormat { FillId = 2, FontId = 0, ApplyFill = true }, // 1 - OK
                 new X.CellFormat { FillId = 3, FontId = 0, ApplyFill = true }, // 2 - tolerada
                 new X.CellFormat { FillId = 4, FontId = 0, ApplyFill = true }, // 3 - divergente
                 new X.CellFormat { FillId = 5, FontId = 0, ApplyFill = true }, // 4 - não encontrado
                 new X.CellFormat { FontId = 1, ApplyFont = true },             // 5 - negrito (cabeçalho) — VOLTOU pro lugar original
                 new X.CellFormat { FillId = 6, FontId = 0, ApplyFill = true }  // 6 - carteirinha não encontrada — vai pro final
             );
            return new X.Stylesheet(fonts, fills, borders, cellFormats);
        }

        private X.Fill CriarFillSolido(string corHex)
        {
            return new X.Fill(new X.PatternFill
            {
                PatternType = X.PatternValues.Solid,
                ForegroundColor = new X.ForegroundColor { Rgb = new HexBinaryValue { Value = corHex } },
                BackgroundColor = new X.BackgroundColor { Indexed = 64 }
            });
        }

        private Cell CriarCelulaTexto(string valor, uint estilo = 0)
        {
            return new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(valor ?? ""),
                StyleIndex = estilo
            };
        }



        #endregion

        #region LOGICA DE INCONSISTÊNCIAS DE FATURAMENTO
        public List<ItemInconsistenciaFaturamento> ObterInconsistenciasFaturamento(string mesAnoReferencia, int codigoGrupoContrato)
        {
            return _sql.BuscarInconsistenciasFaturamento(mesAnoReferencia, codigoGrupoContrato);
        }

        public void ConferirInconsistencias(List<ItemInconsistenciaFaturamento> itens)
        {
            _sql.ConferirInconsistencias(itens);
        }
        #endregion

    }
}
