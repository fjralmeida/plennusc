using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using Plennusc.Core.Models.ModelsGestao.modelsTaxasAssociativas;
using Plennusc.Core.Service.ServiceGestao.serviceTaxasAssociativas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using X = DocumentFormat.OpenXml.Spreadsheet;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class demaisEntidadesTaxaAssociativa : System.Web.UI.Page
    {
        private readonly ServiceTaxasAssociativas _service = new ServiceTaxasAssociativas();

        private const string SESSION_TAXAS_DEMAIS_ENTIDADES = "TaxasDemaisEntidades_Dados";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarEntidades();
            }
        }

        // CARREGAR CHECKBOX LIST DE ENTIDADES

        private void CarregarEntidades()
        {
            try
            {
                var entidades = _service.ObterEntidades();

                cblEntidades.Items.Clear();

                foreach (var ent in entidades)
                {
                    cblEntidades.Items.Add(new ListItem(ent.NomeEvento, ent.CodigoEvento.ToString()));
                }
            }
            catch (Exception ex)
            {
                ExibirMensagem("Erro ao carregar entidades: " + ex.Message, erro: true);
            }
        }

        // PROCESSAR

        protected void btnProcessar_Click(object sender, EventArgs e)
        {
            var codigosEvento = cblEntidades.Items
                .Cast<ListItem>()
                .Where(i => i.Selected)
                .Select(i => Convert.ToInt32(i.Value))
                .ToList();

            if (codigosEvento.Count == 0)
            {
                ExibirMensagem("Selecione pelo menos uma entidade.", erro: true);
                return;
            }

            if (!DateTime.TryParse(txtDataInicio.Text, out var dataInicio) ||
                !DateTime.TryParse(txtDataFim.Text, out var dataFim))
            {
                ExibirMensagem("Informe corretamente as datas de início e fim.", erro: true);
                return;
            }

            if (dataFim.Date < dataInicio.Date)
            {
                ExibirMensagem("A data final não pode ser menor que a data inicial.", erro: true);
                return;
            }

            try
            {
                var dados = _service.ObterTaxasDemaisEntidades(codigosEvento, dataInicio, dataFim);

                Session[SESSION_TAXAS_DEMAIS_ENTIDADES] = dados;

                gvResultado.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
                gvResultado.PageIndex = 0;
                gvResultado.DataSource = dados;
                gvResultado.DataBind();

                litTotalRegistros.Text = dados.Count.ToString();
                litTotalValor.Text = dados.Sum(x => x.ValorEvento).ToString("N2");
                divResultado.Visible = true;

                if (dados.Count == 0)
                    ExibirMensagem("Nenhum registro encontrado para o período informado.", erro: false);
                else
                    ExibirMensagem($"{dados.Count} registro(s) encontrado(s).", erro: false);
            }
            catch (Exception ex)
            {
                ExibirMensagem("Erro ao processar: " + ex.Message, erro: true);
            }
        }

        // PAGINAÇÃO NATIVA DO GRIDVIEW

        protected void gvResultado_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvResultado.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvResultado.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            gvResultado.PageIndex = 0;
            BindGrid();
        }

        protected void gvResultado_DataBound(object sender, EventArgs e)
        {
            var dados = Session[SESSION_TAXAS_DEMAIS_ENTIDADES] as List<TaxaAssociativaDemaisEntidadesModel>;
            int total = dados?.Count ?? 0;
            int pagina = gvResultado.PageIndex;
            int tamanho = gvResultado.PageSize;
            int inicio = total == 0 ? 0 : (pagina * tamanho) + 1;
            int fim = Math.Min(inicio + gvResultado.Rows.Count - 1, total);

            lblPagerInfo.Text = total == 0
                ? ""
                : $"<strong>{inicio} - {fim}</strong> de <strong>{total}</strong> itens";
        }

        private void BindGrid()
        {
            var dados = Session[SESSION_TAXAS_DEMAIS_ENTIDADES] as List<TaxaAssociativaDemaisEntidadesModel>;
            if (dados == null)
            {
                divResultado.Visible = false;
                return;
            }

            // Sincroniza o DropDown com o PageSize atual
            if (ddlPageSize.Items.FindByValue(gvResultado.PageSize.ToString()) != null)
                ddlPageSize.SelectedValue = gvResultado.PageSize.ToString();

            gvResultado.DataSource = dados;
            gvResultado.DataBind();
        }

        // EXPORTAR EXCEL

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            var dados = Session[SESSION_TAXAS_DEMAIS_ENTIDADES] as List<TaxaAssociativaDemaisEntidadesModel>;

            if (dados == null || dados.Count == 0)
            {
                ExibirMensagem("Não há dados para exportar.", erro: true);
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
                        Name = "Demais Entidades"
                    });

                    // Cabeçalho
                    var cabecalhos = new[]
                    {
                        "Nº Registro", "Beneficiário", "Tipo",
                        "Data Pagamento", "Valor Taxa"
                    };

                    var headerRow = new X.Row();
                    foreach (var cab in cabecalhos)
                        headerRow.Append(CriarCelulaTexto(cab, 1));
                    sheetData.Append(headerRow);

                    // Dados
                    foreach (var item in dados)
                    {
                        var row = new X.Row();
                        row.Append(CriarCelulaTexto(item.NumeroRegistroPs1020.ToString()));
                        row.Append(CriarCelulaTexto(item.NomeBeneficiario ?? ""));
                        row.Append(CriarCelulaTexto(item.TipoContratacaoAns ?? ""));
                        row.Append(CriarCelulaTexto(item.DataPagamento?.ToString("dd/MM/yyyy") ?? ""));
                        row.Append(CriarCelulaTexto(item.ValorEvento.ToString("N2")));
                        sheetData.Append(row);
                    }

                    // Linha de total (negrito)
                    decimal somaTotal = dados.Sum(x => x.ValorEvento);

                    var totalRow = new X.Row();
                    totalRow.Append(CriarCelulaTexto(""));
                    totalRow.Append(CriarCelulaTexto(""));
                    totalRow.Append(CriarCelulaTexto(""));
                    totalRow.Append(CriarCelulaTexto("Valor Total", 1));
                    totalRow.Append(CriarCelulaTexto(somaTotal.ToString("N2"), 1));
                    sheetData.Append(totalRow);

                    workbookPart.Workbook.Save();
                }

                bytes = stream.ToArray();
            }

            Response.Clear();
            Response.ClearHeaders();
            Response.ClearContent();
            Response.Buffer = true;
            Response.Charset = "";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition",
                $"attachment; filename=\"TaxasDemaisEntidades_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx\"");
            Response.AddHeader("Content-Length", bytes.Length.ToString());

            Response.BinaryWrite(bytes);
            Response.Flush();
            Response.SuppressContent = true;
            HttpContext.Current.ApplicationInstance.CompleteRequest();
        }

       
        // HELPERS DE ESTILO / CÉLULA

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
                    ForegroundColor = new X.ForegroundColor { Rgb = new DocumentFormat.OpenXml.HexBinaryValue { Value = "E6E6E6" } },
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

        // AUXILIAR

        private void ExibirMensagem(string mensagem, bool erro)
        {
            lblMensagem.Text = mensagem;
            lblMensagem.CssClass = "msg-importacao " + (erro ? "erro" : "sucesso");
        }
    }
}