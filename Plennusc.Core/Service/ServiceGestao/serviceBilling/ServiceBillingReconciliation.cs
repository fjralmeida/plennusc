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
        private const string OP_HAPVIDA = "HAPVIDA";
        private const string OP_UNIMED = "UNIMED";
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
                        Cpf = u.Cpf,
                        Credito = u.Credito,
                        Debito = u.Debito
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

            if (nomeOperadora.IndexOf("UNIMED", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                return _unimed.ConferirComView(itensImportados, codigoGrupoContrato);
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

        private class ColunaExport
        {
            public string Header { get; set; }
            public Func<ItemRelatorioImportadoHapVida, string> ObterValor { get; set; }
            public string[] OperadorasPermitidas { get; set; }
        }

        #region LOGICA DE EXPORTAÇÃO PARA EXCEL

        private List<ColunaExport> MontarDefinicaoColunas()
        {
            return new List<ColunaExport>
            {
                new ColunaExport { Header = "CPF / Carteirinha", ObterValor = i => i.Cpf ?? "" },
                new ColunaExport { Header = "Beneficiário", ObterValor = i => i.Beneficiario ?? "" },
                new ColunaExport { Header = "Nascimento", ObterValor = i => i.Nascimento?.ToString("dd/MM/yyyy") ?? "", OperadorasPermitidas = new[] { OP_HAPVIDA } },
                new ColunaExport { Header = "Parentesco", ObterValor = i => i.Parentesco ?? "", OperadorasPermitidas = new[] { OP_HAPVIDA } },
                new ColunaExport { Header = "Plano", ObterValor = i => i.Plano ?? "" },
                new ColunaExport { Header = "Mês/Ano Usado", ObterValor = i => i.MesAnoReferencia ?? "" },
                new ColunaExport { Header = "Valor Operadora", ObterValor = i => i.Cobrado.ToString("N2") },
                new ColunaExport { Header = "Valor Adicional", ObterValor = i => i.Adicional.ToString("N2"), OperadorasPermitidas = new[] { OP_HAPVIDA } },
                new ColunaExport { Header = "Crédito", ObterValor = i => i.Credito.ToString("N2"), OperadorasPermitidas = new[] { OP_UNIMED } },
                new ColunaExport { Header = "Débito", ObterValor = i => i.Debito.ToString("N2"), OperadorasPermitidas = new[] { OP_UNIMED } },
                new ColunaExport { Header = "Valor Cobrança", ObterValor = i => i.ValorOperadoraView?.ToString("N2") ?? "" },
                new ColunaExport { Header = "Diferença", ObterValor = i => i.DiferencaValor?.ToString("N2") ?? "" },
                new ColunaExport { Header = "Codigo Empresa", ObterValor = i => i.CodigoEmpresa?.ToString() ?? "", OperadorasPermitidas = new[] { OP_UNIMED } },
                new ColunaExport { Header = "Empresa", ObterValor = i => i.EmpresaUnimed ?? "", OperadorasPermitidas = new[] { OP_UNIMED } },
                new ColunaExport { Header = "Data Admissão", ObterValor = i => i.DataAdmissao?.ToString("dd/MM/yyyy") ?? "", OperadorasPermitidas = new[] { OP_UNIMED } },
                new ColunaExport { Header = "Data Exclusão", ObterValor = i => i.DataExclusao?.ToString("dd/MM/yyyy") ?? "" },
                new ColunaExport { Header = "Motivo Exclusão", ObterValor = i => i.NomeMotivoExclusao ?? "" },
                new ColunaExport { Header = "Tabela de Preço", ObterValor = i => i.NomeTabelaPreco ?? "", OperadorasPermitidas = new[] { OP_HAPVIDA } },
                new ColunaExport { Header = "Grupo de Pessoas", ObterValor = i => i.NomeGrupoPessoas ?? "", OperadorasPermitidas = new[] { OP_UNIMED } },
                new ColunaExport { Header = "Grupo de Faturamento", ObterValor = i => i.DescricaoGrupoFaturamento ?? "", OperadorasPermitidas = new[] { OP_HAPVIDA } },
                new ColunaExport { Header = "Status", ObterValor = i => TraduzirStatusExcel(i.StatusConferencia) },
            };
        }

        public byte[] ExportarConferenciaExcel(List<ItemRelatorioImportadoHapVida> itens, string codigoOperadora)
        {
            var colunasAplicaveis = MontarDefinicaoColunas()
                .Where(c => c.OperadorasPermitidas == null || c.OperadorasPermitidas.Contains(codigoOperadora))
                .ToList();

            using (var stream = new MemoryStream())
            {
                using (var doc = SpreadsheetDocument.Create(stream, DocumentFormat.OpenXml.SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = doc.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

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

                    var headerRow = new Row();
                    foreach (var coluna in colunasAplicaveis)
                        headerRow.Append(CriarCelulaTexto(coluna.Header, 5));
                    sheetData.Append(headerRow);

                    foreach (var item in itens)
                    {
                        uint estilo = ObterEstiloPorStatus(item.StatusConferencia);
                        var row = new Row();
                        foreach (var coluna in colunasAplicaveis)
                            row.Append(CriarCelulaTexto(coluna.ObterValor(item), estilo));
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
        public List<ItemInconsistenciaFaturamento> ObterInconsistenciasFaturamento(string mesAnoReferencia, int codigoGrupoContrato, List<int> codigosGrupoFaturamento)
        {
            return _sql.BuscarInconsistenciasFaturamento(mesAnoReferencia, codigoGrupoContrato, codigosGrupoFaturamento);
        }

        public void ConferirInconsistencias(List<ItemInconsistenciaFaturamento> itens)
        {
            _sql.ConferirInconsistencias(itens);
        }
        #endregion

    }
}
