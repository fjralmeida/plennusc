using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using Plennusc.Core.Models.ModelsGestao.modelsCardsImport;
using Plennusc.Core.Service.ServiceGestao.CardsImportService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using X = DocumentFormat.OpenXml.Spreadsheet;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class CardsImport : System.Web.UI.Page
    {
        private const string SESSION_KEY = "CardsImportResult";

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnImportar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            if (!fileUploadExcel.HasFile)
            {
                ExibirErro("Selecione um arquivo Excel.");
                return;
            }

            var connSettings = ConfigurationManager.ConnectionStrings["Alianca"];
            if (connSettings == null)
            {
                ExibirErro("Connection string 'Alianca' não encontrada.");
                return;
            }

            List<CardsImportResultModel> resultados;
            try
            {
                var service = new serviceCardsImport(connSettings.ConnectionString);
                resultados = service.ProcessarImportacao(fileUploadExcel.PostedFile.InputStream);
            }
            catch (Exception ex)
            {
                ExibirErro("Erro ao ler a planilha: " + ex.Message);
                return;
            }

            Session[SESSION_KEY] = resultados;

            gridTodos.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            gridTodos.PageIndex = 0;
            gridTodos.DataSource = resultados;
            gridTodos.DataBind();
            litCountTodos.Text = resultados.Count.ToString();

            var importados = resultados.Where(r => r.Sucesso).ToList();
            gridImportados.DataSource = importados;
            gridImportados.DataBind();
            litCountImportados.Text = importados.Count.ToString();

            var divergencias = resultados
                .Where(r => !r.Sucesso && r.Motivo != null &&
                            (r.Motivo.Contains("Data de Admissão") || r.Motivo.Contains("formato incorreto")))
                .ToList();
            gridDivergencia.DataSource = divergencias;
            gridDivergencia.DataBind();
            litCountDivergencia.Text = divergencias.Count.ToString();

            var naoEncontrados = resultados
                .Where(r => !r.Sucesso && r.Motivo != null && r.Motivo.Contains("não encontrado"))
                .ToList();
            gridNaoEncontrado.DataSource = naoEncontrados;
            gridNaoEncontrado.DataBind();
            litCountNaoEncontrado.Text = naoEncontrados.Count.ToString();

            pnlResultado.Visible = true;
        }

        protected void gridTodos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridTodos.PageIndex = e.NewPageIndex;
            BindGridTodos();
        }

        private void BindGridTodos()
        {
            var dados = Session[SESSION_KEY] as List<CardsImportResultModel>;
            if (dados == null)
            {
                ExibirErro("Os dados da importação expiraram. Realize uma nova importação.");
                pnlResultado.Visible = false;
                return;
            }

            if (ddlPageSize.Items.FindByValue(gridTodos.PageSize.ToString()) != null)
                ddlPageSize.SelectedValue = gridTodos.PageSize.ToString();

            gridTodos.DataSource = dados;
            gridTodos.DataBind();
        }

        protected void gridTodos_DataBound(object sender, EventArgs e)
        {
            var dados = Session[SESSION_KEY] as List<CardsImportResultModel>;
            int total = dados?.Count ?? 0;
            int pagina = gridTodos.PageIndex;
            int tamanho = gridTodos.PageSize;
            int inicio = total == 0 ? 0 : (pagina * tamanho) + 1;
            int fim = Math.Min(inicio + gridTodos.Rows.Count - 1, total);

            lblPagerInfo.Text = total == 0
                ? ""
                : $"<strong>{inicio} - {fim}</strong> de <strong>{total}</strong> itens";
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gridTodos.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            gridTodos.PageIndex = 0;
            BindGridTodos();
        }

        protected string GetStatusCss(object sucesso, object motivo)
        {
            if (sucesso != null && sucesso.Equals(true))
                return "status-ok";

            var motivoTexto = motivo?.ToString() ?? string.Empty;

            if (motivoTexto.Contains("Múltiplos registros") || motivoTexto.Contains("Erro no UPDATE") || motivoTexto.Contains("Erro ao gravar"))
                return "status-divergente";

            if (motivoTexto.Contains("não encontrado"))
                return "status-nao-encontrado";

            if (motivoTexto.Contains("Data de Admissão") || motivoTexto.Contains("formato incorreto"))
                return "status-divergencia-tolerada";

            return "status-divergente";
        }

        protected void gridTodos_RowDataBound(object sender, GridViewRowEventArgs e) { }

        private void ExibirErro(string mensagem)
        {
            var msg = mensagem.Replace("'", "\\'");
            ClientScript.RegisterStartupScript(this.GetType(), "erro", $"alert('{msg}');", true);
        }

        protected void btnExportarTodos_Click(object sender, EventArgs e)
        {
            var dados = ObterDadosDaSession();
            ExportarParaExcel(dados, "Carteirinhas_Todos");
        }

        protected void btnExportarImportados_Click(object sender, EventArgs e)
        {
            var dados = ObterDadosDaSession();
            if (dados == null) return;
            ExportarParaExcel(dados.Where(r => r.Sucesso).ToList(), "Carteirinhas_Importados");
        }

        protected void btnExportarDivergencia_Click(object sender, EventArgs e)
        {
            var dados = ObterDadosDaSession();
            if (dados == null) return;
            var filtrados = dados.Where(r => !r.Sucesso && r.Motivo != null &&
                                             (r.Motivo.Contains("Data de Admissão") || r.Motivo.Contains("formato incorreto"))).ToList();
            ExportarParaExcel(filtrados, "Carteirinhas_Divergencia");
        }

        protected void btnExportarNaoEncontrado_Click(object sender, EventArgs e)
        {
            var dados = ObterDadosDaSession();
            if (dados == null) return;
            var filtrados = dados.Where(r => !r.Sucesso && r.Motivo != null && r.Motivo.Contains("não encontrado")).ToList();
            ExportarParaExcel(filtrados, "Carteirinhas_CPF_Nao_Encontrado");
        }

        private List<CardsImportResultModel> ObterDadosDaSession()
        {
            var dados = Session[SESSION_KEY] as List<CardsImportResultModel>;
            if (dados == null || dados.Count == 0)
            {
                ExibirErro("Não há dados para exportar.");
                return null;
            }
            return dados;
        }

        private void ExportarParaExcel(List<CardsImportResultModel> itens, string nomeArquivoBase)
        {
            if (itens == null || itens.Count == 0)
            {
                ExibirErro("Não há dados para exportar.");
                return;
            }

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                using (var doc = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = doc.AddWorkbookPart();
                    workbookPart.Workbook = new X.Workbook();

                    var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    stylesPart.Stylesheet = CriarStylesheet();
                    stylesPart.Stylesheet.Save();

                    var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new X.SheetData();
                    worksheetPart.Worksheet = new X.Worksheet(sheetData);

                    var sheets = workbookPart.Workbook.AppendChild(new X.Sheets());
                    sheets.Append(new X.Sheet
                    {
                        Id = workbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Carteirinhas"
                    });

                    var cabecalhos = new[]
                    {
                        "Linha", "CPF", "Nome Associado", "Cód. Associado",
                        "Nº Carteirinha", "Data Admissão (Planilha)", "Data Admissão (PS1000)",
                        "Status", "Motivo"
                    };

                    var headerRow = new X.Row();
                    foreach (var cab in cabecalhos)
                        headerRow.Append(CriarCelulaTexto(cab, 1));
                    sheetData.Append(headerRow);

                    foreach (var item in itens)
                    {
                        var row = new X.Row();
                        row.Append(CriarCelulaTexto(item.LinhaCsv.ToString()));
                        row.Append(CriarCelulaTexto(item.Cpf ?? ""));
                        row.Append(CriarCelulaTexto(item.NomeAssociado ?? ""));
                        row.Append(CriarCelulaTexto(item.CodigoAssociado ?? ""));
                        row.Append(CriarCelulaTexto(item.NumeroCarteirinha ?? ""));
                        row.Append(CriarCelulaTexto(item.DataAdmissaoPlanilha ?? ""));
                        row.Append(CriarCelulaTexto(item.DataAdmissaoBanco ?? ""));
                        row.Append(CriarCelulaTexto(item.Sucesso ? "Importado" : "Não Importado"));
                        row.Append(CriarCelulaTexto(item.Motivo ?? ""));
                        sheetData.Append(row);
                    }

                    workbookPart.Workbook.Save();
                }

                bytes = stream.ToArray();
            }

            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition",
                $"attachment; filename=\"{nomeArquivoBase}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx\"");
            Response.AddHeader("Content-Length", bytes.Length.ToString());
            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

        private X.Stylesheet CriarStylesheet()
        {
            var fonts = new X.Fonts(
                new X.Font(new X.FontSize { Val = 11 }, new X.FontName { Val = "Calibri" }),
                new X.Font(new X.Bold(), new X.FontSize { Val = 11 }, new X.FontName { Val = "Calibri" })
            );

            var fills = new X.Fills(
                new X.Fill(new X.PatternFill { PatternType = X.PatternValues.None }),
                new X.Fill(new X.PatternFill { PatternType = X.PatternValues.Gray125 }),
                new X.Fill(new X.PatternFill
                {
                    PatternType = X.PatternValues.Solid,
                    ForegroundColor = new X.ForegroundColor { Rgb = new HexBinaryValue { Value = "E6E6E6" } },
                    BackgroundColor = new X.BackgroundColor { Indexed = 64 }
                })
            );

            var borders = new X.Borders(new X.Border());

            var cellFormats = new X.CellFormats(
                new X.CellFormat(),
                new X.CellFormat { FontId = 1, FillId = 2, ApplyFont = true, ApplyFill = true }
            );

            return new X.Stylesheet(fonts, fills, borders, cellFormats);
        }

        private X.Cell CriarCelulaTexto(string valor, uint estilo = 0)
        {
            return new X.Cell
            {
                DataType = X.CellValues.String,
                CellValue = new X.CellValue(valor ?? ""),
                StyleIndex = estilo
            };
        }
    }
}