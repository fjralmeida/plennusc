using OfficeOpenXml;
using OfficeOpenXml.Style;
using Plennusc.Core.Models.ModelsGestao.modelsSendMessage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace PlennuscApp.PlennuscGestao.Services
{
    public class ExcelReportService
    {
        public byte[] GerarExcelRelatorio(List<DetailedSubmissionResult> resultados)
        {
            // Configurar licença EPPlus 8
            ExcelPackage.License.SetNonCommercialOrganization("PlennuscApp");

            using (var package = new ExcelPackage())
            {
                // PLANILHA 1: RESUMO
                var wsResumo = package.Workbook.Worksheets.Add("Resumo");
                ConfigurarCabecalhoResumo(wsResumo);
                PreencherDadosResumo(wsResumo, resultados);

                // PLANILHA 2: DETALHES
                var wsDetalhes = package.Workbook.Worksheets.Add("Detalhes Envio");
                ConfigurarCabecalhoDetalhes(wsDetalhes);
                PreencherDetalhesEnvio(wsDetalhes, resultados);

                return package.GetAsByteArray();
            }
        }

        private void ConfigurarCabecalhoResumo(ExcelWorksheet worksheet)
        {
            string[] headers = {
                "Código", "Nome", "Telefone", "Template Escolhido",
                "Template Aplicado", "Boleto Disponível", "Nota Fiscal Disponível",
                "Boleto Enviado", "Nota Fiscal Enviada", "Status", "Motivo Resumido"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cells[1, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(79, 129, 189));
                cell.Style.Font.Color.SetColor(Color.White);
                cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            }
        }

        private void PreencherDadosResumo(ExcelWorksheet worksheet, List<DetailedSubmissionResult> resultados)
        {
            int row = 2;
            foreach (var resultado in resultados)
            {
                // Coluna 1: Código (formato texto)
                worksheet.Cells[row, 1].Value = resultado.CodigoAssociado;
                worksheet.Cells[row, 1].Style.Numberformat.Format = "@";

                // Coluna 2: Nome
                worksheet.Cells[row, 2].Value = resultado.Nome;

                // Coluna 3: Telefone (formato texto)
                worksheet.Cells[row, 3].Value = resultado.Telefone;
                worksheet.Cells[row, 3].Style.Numberformat.Format = "@";

                // Colunas 4 e 5: Templates
                worksheet.Cells[row, 4].Value = resultado.TemplateEscolhido;
                worksheet.Cells[row, 5].Value = resultado.TemplateAplicado;

                // Colunas 6 a 9: Disponibilidade e envio
                worksheet.Cells[row, 6].Value = resultado.BoletoDisponivel ? "SIM" : "NÃO";
                worksheet.Cells[row, 7].Value = resultado.NotaFiscalDisponivel ? "SIM" : "NÃO";
                worksheet.Cells[row, 8].Value = resultado.BoletoEnviado ? "SIM" : "NÃO";
                worksheet.Cells[row, 9].Value = resultado.NotaFiscalEnviada ? "SIM" : "NÃO";

                // Coluna 10: Status
                var statusCell = worksheet.Cells[row, 10];
                statusCell.Value = resultado.Status;

                if (resultado.Status.Contains("ENVIADO"))
                {
                    statusCell.Style.Font.Color.SetColor(Color.Green);
                    statusCell.Style.Font.Bold = true;
                }
                else if (resultado.Status.Contains("ERRO") || resultado.Status.Contains("NÃO ENVIADO"))
                {
                    statusCell.Style.Font.Color.SetColor(Color.Red);
                    statusCell.Style.Font.Bold = true;
                }

                // Coluna 11: Motivo Resumido
                string motivoResumido = ExtrairPrimeiraLinhaMotivo(resultado.Motivo);
                worksheet.Cells[row, 11].Value = motivoResumido;

                row++;
            }

            // Ajustar largura das colunas
            worksheet.Column(1).Width = 15;
            worksheet.Column(2).Width = 30;
            worksheet.Column(3).Width = 15;
            worksheet.Column(11).Width = 50;

            // Adicionar bordas
            var dataRange = worksheet.Cells[1, 1, row - 1, 11];
            dataRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            dataRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        }

        private void ConfigurarCabecalhoDetalhes(ExcelWorksheet worksheet)
        {
            string[] headers = {
                "Código", "Nome", "Telefone", "Evento", "Data/Hora", "Status"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = worksheet.Cells[1, i + 1];
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                cell.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(42, 157, 143));
                cell.Style.Font.Color.SetColor(Color.White);
            }
        }

        private void PreencherDetalhesEnvio(ExcelWorksheet worksheet, List<DetailedSubmissionResult> resultados)
        {
            int row = 2;
            foreach (var resultado in resultados)
            {
                var detalhes = ExtrairLinhasDetalhadas(resultado.Motivo);

                if (detalhes.Count > 0)
                {
                    foreach (var detalhe in detalhes)
                    {
                        worksheet.Cells[row, 1].Value = resultado.CodigoAssociado;
                        worksheet.Cells[row, 1].Style.Numberformat.Format = "@";

                        worksheet.Cells[row, 2].Value = resultado.Nome;

                        worksheet.Cells[row, 3].Value = resultado.Telefone;
                        worksheet.Cells[row, 3].Style.Numberformat.Format = "@";

                        worksheet.Cells[row, 4].Value = detalhe.Evento;
                        worksheet.Cells[row, 5].Value = detalhe.DataHora;
                        worksheet.Cells[row, 6].Value = resultado.Status;

                        // Colorir evento
                        var eventoCell = worksheet.Cells[row, 4];
                        if (detalhe.Evento.Contains("✅")) eventoCell.Style.Font.Color.SetColor(Color.Green);
                        if (detalhe.Evento.Contains("📥")) eventoCell.Style.Font.Color.SetColor(Color.Blue);
                        if (detalhe.Evento.Contains("❌")) eventoCell.Style.Font.Color.SetColor(Color.Red);
                        if (detalhe.Evento.Contains("📞")) eventoCell.Style.Font.Color.SetColor(Color.DarkOrange);

                        row++;
                    }
                }
                else
                {
                    worksheet.Cells[row, 1].Value = resultado.CodigoAssociado;
                    worksheet.Cells[row, 1].Style.Numberformat.Format = "@";
                    worksheet.Cells[row, 2].Value = resultado.Nome;
                    worksheet.Cells[row, 3].Value = resultado.Telefone;
                    worksheet.Cells[row, 3].Style.Numberformat.Format = "@";
                    worksheet.Cells[row, 4].Value = "Sem detalhes disponíveis";
                    row++;
                }
            }

            // Ajustar largura
            worksheet.Column(1).Width = 15;
            worksheet.Column(2).Width = 30;
            worksheet.Column(3).Width = 15;
            worksheet.Column(4).Width = 20;
            worksheet.Column(5).Width = 25;
            worksheet.Column(6).Width = 15;
        }

        private string ExtrairPrimeiraLinhaMotivo(string motivoCompleto)
        {
            if (string.IsNullOrEmpty(motivoCompleto))
                return "Sem informações";

            var linhas = motivoCompleto.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return linhas.Length > 0 ? linhas[0] : "Sem informações";
        }

        private List<(string Evento, string DataHora)> ExtrairLinhasDetalhadas(string motivoCompleto)
        {
            var detalhes = new List<(string, string)>();

            if (string.IsNullOrEmpty(motivoCompleto))
                return detalhes;

            var linhas = motivoCompleto.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var linha in linhas)
            {
                var linhaLimpa = linha.Trim();

                if (linhaLimpa.StartsWith("📞"))
                    continue;

                if (linhaLimpa.Contains("✅") || linhaLimpa.Contains("📥") || linhaLimpa.Contains("❌"))
                {
                    int doisPontosIndex = linhaLimpa.IndexOf(':');
                    if (doisPontosIndex > 0)
                    {
                        string evento = linhaLimpa.Substring(0, doisPontosIndex).Trim();
                        string dataHora = linhaLimpa.Substring(doisPontosIndex + 1).Trim();

                        detalhes.Add((evento, dataHora));
                    }
                }
            }

            return detalhes;
        }

        public byte[] GerarCSVResumo(List<DetailedSubmissionResult> resultados)
        {
            using (MemoryStream ms = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(ms, System.Text.Encoding.UTF8))
            {
                sw.WriteLine("Código;Nome;Telefone;Template Escolhido;Template Aplicado;Boleto Disponível;Nota Fiscal Disponível;Boleto Enviado;Nota Fiscal Enviada;Status;Motivo");

                foreach (var resultado in resultados)
                {
                    sw.WriteLine($"{resultado.CodigoAssociado};{resultado.Nome};{resultado.Telefone};{resultado.TemplateEscolhido};{resultado.TemplateAplicado};{resultado.BoletoDisponivel};{resultado.NotaFiscalDisponivel};{resultado.BoletoEnviado};{resultado.NotaFiscalEnviada};{resultado.Status};{resultado.Motivo}");
                }

                sw.Flush();
                return ms.ToArray();
            }
        }
    }
}