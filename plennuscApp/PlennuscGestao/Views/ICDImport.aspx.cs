using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using Plennusc.Core.Models.ModelsGestao.modelsCIDs;
using Plennusc.Core.Service.ServiceGestao.CIDsService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
// Alias para evitar ambiguidade com System.Web.UI.WebControls (Font, FontSize, Border, Color, Row, etc.)
using X = DocumentFormat.OpenXml.Spreadsheet;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class ICDImport : System.Web.UI.Page
    {
        private const string SESSION_KEY = "ICDResult";

        protected void Page_Load(object sender, EventArgs e)
        {
            // Nada especial aqui. Paginação nativa não precisa de recriação manual.
        }

        protected void btnImportar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            if (!fileUploadExcel.HasFile)
            {
                ExibirErro("Selecione um arquivo Excel.");
                return;
            }
            if (!DateTime.TryParse(txtVigencia.Text, out var vigencia))
            {
                ExibirErro("Vigência inválida.");
                return;
            }

            var connSettings = ConfigurationManager.ConnectionStrings["Alianca"];
            if (connSettings == null)
            {
                ExibirErro("Connection string 'Alianca' não encontrada.");
                return;
            }

            var service = new serviceCIDs(connSettings.ConnectionString);
            var resultados = service.ProcessarImportacao(fileUploadExcel.PostedFile.InputStream, vigencia);

            // Guarda na SESSION (não usa ViewState para a lista)
            Session[SESSION_KEY] = resultados;

            // Aplica o tamanho de página atual (vindo do DropDown)
            gridTodos.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            gridTodos.PageIndex = 0;
            gridTodos.DataSource = resultados;
            gridTodos.DataBind();

            litCountTodos.Text = resultados.Count.ToString();

            var importados = resultados.Where(r => r.Sucesso).ToList();
            gridImportados.DataSource = importados;
            gridImportados.DataBind();
            litCountImportados.Text = importados.Count.ToString();

            var jaCadastrados = resultados.Where(r => !r.Sucesso && r.Motivo != null && r.Motivo.Contains("Já cadastrado")).ToList();
            gridJaCadastrados.DataSource = jaCadastrados;
            gridJaCadastrados.DataBind();
            litCountJaCadastrados.Text = jaCadastrados.Count.ToString();

            var divergencia = resultados.Where(r => !r.Sucesso && r.Motivo != null && r.Motivo.Contains("Data de admissão")).ToList();
            gridDivergencia.DataSource = divergencia;
            gridDivergencia.DataBind();
            litCountDivergencia.Text = divergencia.Count.ToString();

            // === ALTERAÇÃO: casa com a mensagem real gerada pelo serviceCIDs ("CID 'X' NÃO CADASTRADO.") ===
            var cidInvalido = resultados.Where(r => !r.Sucesso && r.Motivo != null && r.Motivo.Contains("NÃO CADASTRADO")).ToList();
            gridCidInvalido.DataSource = cidInvalido;
            gridCidInvalido.DataBind();
            litCountCidInvalido.Text = cidInvalido.Count.ToString();

            var naoEncontrado = resultados.Where(r => !r.Sucesso && r.Motivo != null && r.Motivo.Contains("não encontrado na PS1000")).ToList();
            gridNaoEncontrado.DataSource = naoEncontrado;
            gridNaoEncontrado.DataBind();
            litCountNaoEncontrado.Text = naoEncontrado.Count.ToString();

            pnlResultado.Visible = true;
        }

        // ============================================================
        //  PAGINAÇÃO NATIVA DO GRIDVIEW
        // ============================================================

        protected void gridTodos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridTodos.PageIndex = e.NewPageIndex;
            BindGridTodos();
        }

        private void BindGridTodos()
        {
            var dados = Session[SESSION_KEY] as System.Collections.IList;
            if (dados == null)
            {
                ExibirErro("Os dados da importação expiraram. Realize uma nova importação.");
                pnlResultado.Visible = false;
                return;
            }

            // Sincroniza o DropDown com o PageSize atual (caso seja alterado por código)
            if (ddlPageSize.Items.FindByValue(gridTodos.PageSize.ToString()) != null)
                ddlPageSize.SelectedValue = gridTodos.PageSize.ToString();

            gridTodos.DataSource = dados;
            gridTodos.DataBind();
        }

        protected void gridTodos_DataBound(object sender, EventArgs e)
        {
            var dados = Session[SESSION_KEY] as System.Collections.IList;
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
            // Altera o PageSize da grid
            gridTodos.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            // Volta para a primeira página (opcional, mas recomendado)
            gridTodos.PageIndex = 0;
            // Rebind com os dados da SESSION
            BindGridTodos();
        }

        // ============================================================
        //  AUXILIARES
        // ============================================================

        protected string GetStatusCss(object sucesso, object motivo)
        {
            if (sucesso != null && sucesso.Equals(true))
                return "status-ok";

            var motivoTexto = motivo?.ToString() ?? string.Empty;
            if (motivoTexto.Contains("Já cadastrado"))
                return "status-nao-encontrado";
            if (motivoTexto.Contains("Data de admissão"))
                return "status-divergencia-tolerada";
            // === ALTERAÇÃO: casa com a mensagem real ("CID 'X' NÃO CADASTRADO.") ===
            if (motivoTexto.Contains("NÃO CADASTRADO"))
                return "status-divergente";
            if (motivoTexto.Contains("não encontrado na PS1000"))
                return "status-nao-encontrado";
            return "status-divergente";
        }

        protected void gridTodos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Não pinta linha
        }

        private void ExibirErro(string mensagem)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "erro", $"alert('{mensagem}');", true);
        }


        //  EXPORTAÇÃO PARA EXCEL (um botão por aba)

        protected void btnExportarTodos_Click(object sender, EventArgs e)
        {
            var dados = ObterDadosDaSession();
            ExportarParaExcel(dados, "ICD_Todos");
        }

        protected void btnExportarImportados_Click(object sender, EventArgs e)
        {
            var dados = ObterDadosDaSession();
            if (dados == null) return;

            var filtrados = dados.Where(r => r.Sucesso).ToList();
            ExportarParaExcel(filtrados, "ICD_Importados");
        }

        protected void btnExportarJaCadastrados_Click(object sender, EventArgs e)
        {
            var dados = ObterDadosDaSession();
            if (dados == null) return;

            var filtrados = dados.Where(r => !r.Sucesso && r.Motivo != null && r.Motivo.Contains("Já cadastrado")).ToList();
            ExportarParaExcel(filtrados, "ICD_Ja_Cadastrados");
        }

        protected void btnExportarDivergencia_Click(object sender, EventArgs e)
        {
            var dados = ObterDadosDaSession();
            if (dados == null) return;

            var filtrados = dados.Where(r => !r.Sucesso && r.Motivo != null && r.Motivo.Contains("Data de admissão")).ToList();
            ExportarParaExcel(filtrados, "ICD_Vigencia_Divergente");
        }

        protected void btnExportarCidInvalido_Click(object sender, EventArgs e)
        {
            var dados = ObterDadosDaSession();
            if (dados == null) return;

            var filtrados = dados.Where(r => !r.Sucesso && r.Motivo != null && r.Motivo.Contains("NÃO CADASTRADO")).ToList();
            ExportarParaExcel(filtrados, "ICD_CID_Invalido");
        }

        protected void btnExportarNaoEncontrado_Click(object sender, EventArgs e)
        {
            var dados = ObterDadosDaSession();
            if (dados == null) return;

            var filtrados = dados.Where(r => !r.Sucesso && r.Motivo != null && r.Motivo.Contains("não encontrado na PS1000")).ToList();
            ExportarParaExcel(filtrados, "ICD_CPF_Nao_Encontrado");
        }

        /// <summary>
        /// Recupera a lista completa da sessão. Se não houver, exibe alerta e retorna null.
        /// </summary>
        private List<CIDsImportResultModel> ObterDadosDaSession()
        {
            var dados = Session[SESSION_KEY] as List<CIDsImportResultModel>;
            if (dados == null || dados.Count == 0)
            {
                ExibirErro("Não há dados para exportar.");
                return null;
            }
            return dados;
        }

        /// <summary>
        /// Exporta a lista para um arquivo .xlsx real (DocumentFormat.OpenXml), sem prompts de importação.
        /// Usa a mesma biblioteca já utilizada no billingReconciliation — sem conflito de versão.
        /// O alias "X" evita ambiguidade com System.Web.UI.WebControls (Font, FontSize, Border, etc.).
        /// </summary>
        private void ExportarParaExcel(List<CIDsImportResultModel> itens, string nomeArquivoBase)
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

                    // Stylesheet (negrito no cabeçalho + fundo cinza)
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
                        Name = "Importação CID"
                    });

                    // Cabeçalhos
                    var cabecalhos = new[] { "CPF", "Titular", "Beneficiário", "CID", "Cód. Associado", "Status", "Motivo" };
                    var headerRow = new X.Row();
                    foreach (var cab in cabecalhos)
                        headerRow.Append(CriarCelulaTexto(cab, 1)); // estilo 1 = cabeçalho
                    sheetData.Append(headerRow);

                    // Dados
                    foreach (var item in itens)
                    {
                        var row = new X.Row();
                        row.Append(CriarCelulaTexto(item.Cpf ?? ""));
                        row.Append(CriarCelulaTexto(item.Titular ?? ""));
                        row.Append(CriarCelulaTexto(item.Beneficiario ?? ""));
                        row.Append(CriarCelulaTexto(item.Cid ?? ""));
                        row.Append(CriarCelulaTexto(item.CodigoAssociado ?? ""));
                        row.Append(CriarCelulaTexto(item.Sucesso ? "Importado" : "Não Importado"));
                        row.Append(CriarCelulaTexto(item.Motivo ?? ""));
                        sheetData.Append(row);
                    }

                    workbookPart.Workbook.Save();
                }

                bytes = stream.ToArray();
            }

            // === LIMPA TUDO ANTES DE ESCREVER ===
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

        //  HELPERS DE ESTILO / CÉLULA (DocumentFormat.OpenXml)

        /// <summary>
        /// Cria o Stylesheet mínimo: fonte padrão + fonte negrito (cabeçalho) + fill cinza (cabeçalho).
        /// Usa o alias X.* para evitar ambiguidade com System.Web.UI.WebControls.
        /// </summary>
        private X.Stylesheet CriarStylesheet()
        {
            var fonts = new X.Fonts(
                new X.Font(new X.FontSize { Val = 11 }, new X.FontName { Val = "Calibri" }),   // 0 - padrão
                new X.Font(new X.Bold(), new X.FontSize { Val = 11 }, new X.FontName { Val = "Calibri" }) // 1 - negrito
            );

            var fills = new X.Fills(
                new X.Fill(new X.PatternFill { PatternType = X.PatternValues.None }),           // 0
                new X.Fill(new X.PatternFill { PatternType = X.PatternValues.Gray125 }),        // 1
                new X.Fill(new X.PatternFill                                                     // 2 - cinza cabeçalho
                {
                    PatternType = X.PatternValues.Solid,
                    ForegroundColor = new X.ForegroundColor { Rgb = new HexBinaryValue { Value = "E6E6E6" } },
                    BackgroundColor = new X.BackgroundColor { Indexed = 64 }
                })
            );

            var borders = new X.Borders(new X.Border());

            var cellFormats = new X.CellFormats(
                new X.CellFormat(),                                                              // 0 - padrão
                new X.CellFormat { FontId = 1, FillId = 2, ApplyFont = true, ApplyFill = true }  // 1 - cabeçalho
            );

            return new X.Stylesheet(fonts, fills, borders, cellFormats);
        }

        /// <summary>
        /// Cria uma célula do tipo texto, opcionalmente com estilo.
        /// </summary>
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