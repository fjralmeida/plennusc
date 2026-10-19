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
using A = DocumentFormat.OpenXml.Drawing;
using X = DocumentFormat.OpenXml.Spreadsheet;
using Xdr = DocumentFormat.OpenXml.Drawing.Spreadsheet;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class demaisEntidadesTaxaAssociativa : System.Web.UI.Page
    {
        private readonly ServiceTaxasAssociativas _service = new ServiceTaxasAssociativas();

        private const string SESSION_TAXAS_DEMAIS_ENTIDADES = "TaxasDemaisEntidades_Dados";

        private string CaminhoLogoVallor
        {
            get
            {
                return Server.MapPath("~/Uploads/logoVallor.png");
            }
        }

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
                    var nomeExibicao = RemoverPrefixoEntidade(ent.NomeEvento);

                    cblEntidades.Items.Add(new ListItem(nomeExibicao, ent.CodigoEvento.ToString()));
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
                divResultado.Visible = true;

                // CABEÇALHO TIMBRADO: Exibe todos os nomes selecionados
                var selecionados = cblEntidades.Items.Cast<ListItem>().Where(i => i.Selected).ToList();

                if (selecionados.Count == 1)
                {
                    litEntidade.Text = RemoverPrefixoEntidade(selecionados[0].Text);
                }
                else
                {
                    // Junta todos os nomes com quebra de linha na tela HTML
                    litEntidade.Text = string.Join("<br />", selecionados.Select(i => RemoverPrefixoEntidade(i.Text)));
                }

                litVigencia.Text = ObterVigenciaMesAno(dados);
                litTotalRelatorio.Text = $"R$ {dados.Sum(x => x.ValorEvento):N2}";
                litDataRelatorio.Text = DateTime.Now.ToString("dd/MM/yyyy");

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

            // NOME PARA O EXCEL: Exibe todos os nomes selecionados
            var selecionados = cblEntidades.Items.Cast<ListItem>().Where(i => i.Selected).ToList();
            string nomeEntidade;

            if (selecionados.Count == 1)
            {
                nomeEntidade = RemoverPrefixoEntidade(selecionados[0].Text);
            }
            else
            {
                // Junta todos os nomes com " | | " (conforme solicitado para o Excel)
                nomeEntidade = string.Join(" | | ", selecionados.Select(i => RemoverPrefixoEntidade(i.Text)));
            }

            string vigencia = ObterVigenciaMesAno(dados);
            decimal somaTotal = dados.Sum(x => x.ValorEvento);

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                using (var doc = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = doc.AddWorkbookPart();
                    workbookPart.Workbook = new X.Workbook();

                    // STYLESHEET
                    var stylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
                    stylesPart.Stylesheet = CriarStylesheet();
                    stylesPart.Stylesheet.Save();

                    // WORKSHEET
                    var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new X.SheetData();

                    //  Worksheet precisa ter: SheetData, MergeCells, Columns (nessa ordem)
                    worksheetPart.Worksheet = new X.Worksheet();

                    // Larguras das colunas
                    var columns = new X.Columns(
                        new X.Column { Min = 1, Max = 1, Width = 20, CustomWidth = true },  // A - Nº Registro / Nome Entidade (aumentei um pouco)
                        new X.Column { Min = 2, Max = 2, Width = 30, CustomWidth = true },  // B - Beneficiário
                        new X.Column { Min = 3, Max = 3, Width = 15, CustomWidth = true },  // C - Tipo
                        new X.Column { Min = 4, Max = 4, Width = 18, CustomWidth = true },  // D - Data Pagamento
                        new X.Column { Min = 5, Max = 5, Width = 15, CustomWidth = true }   // E - Valor Taxa
                    );
                    worksheetPart.Worksheet.AppendChild(columns);

                    // SheetData
                    worksheetPart.Worksheet.AppendChild(sheetData);

                    // MERGE CELLS
                    var mergeCells = new X.MergeCells(
                        new X.MergeCell { Reference = "A1:D1" },
                        new X.MergeCell { Reference = "A2:D2" },
                        new X.MergeCell { Reference = "A3:D3" },
                        new X.MergeCell { Reference = "B5:C5" }
                    );
                    worksheetPart.Worksheet.AppendChild(mergeCells);

                    // Aba
                    var sheets = workbookPart.Workbook.AppendChild(new X.Sheets());
                    sheets.Append(new X.Sheet
                    {
                        Id = workbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Taxas Associativas"
                    });

                    // FOLHA DE ROSTO
                    var linha1 = new X.Row();
                    linha1.Append(CriarCelulaTexto("VALLOR BENEFÍCIOS", 10, "A1"));
                    linha1.Append(CriarCelulaTexto("", 10, "B1"));
                    linha1.Append(CriarCelulaTexto("", 10, "C1"));
                    linha1.Append(CriarCelulaTexto("", 10, "D1"));
                    linha1.Append(CriarCelulaTexto(DateTime.Now.ToString("dd/MM/yyyy"), 11, "E1"));
                    sheetData.Append(linha1);

                    var linha2 = new X.Row();
                    linha2.Append(CriarCelulaTexto("Av. Assis Chateaubriand, 288 - Floresta - BH/MG", 12, "A2"));
                    linha2.Append(CriarCelulaTexto("", 12, "B2"));
                    linha2.Append(CriarCelulaTexto("", 12, "C2"));
                    linha2.Append(CriarCelulaTexto("", 12, "D2"));
                    linha2.Append(CriarCelulaTexto("", 12, "E2"));
                    sheetData.Append(linha2);

                    var linha3 = new X.Row();
                    linha3.Append(CriarCelulaTexto("comissoes@vallorbeneficios.com.br", 12, "A3"));
                    linha3.Append(CriarCelulaTexto("", 12, "B3"));
                    linha3.Append(CriarCelulaTexto("", 12, "C3"));
                    linha3.Append(CriarCelulaTexto("", 12, "D3"));
                    linha3.Append(CriarCelulaTexto("", 12, "E3"));
                    sheetData.Append(linha3);

                    var linha4 = new X.Row();
                    for (int i = 1; i <= 5; i++)
                        linha4.Append(CriarCelulaTexto("", 14, $"{LetraColuna(i)}4"));
                    sheetData.Append(linha4);

                    // Linha 5: faixa azul (usando estilo 15 para aceitar quebra de texto)
                    var linha5 = new X.Row();
                    linha5.Append(CriarCelulaTexto(nomeEntidade, 15, "A5"));
                    linha5.Append(CriarCelulaTexto("RELATÓRIO TAXAS ASSOCIATIVAS", 13, "B5"));
                    linha5.Append(CriarCelulaTexto("", 13, "C5"));
                    linha5.Append(CriarCelulaTexto($"Vig. {vigencia}", 13, "D5"));
                    linha5.Append(CriarCelulaTexto($"TOTAL: R$ {somaTotal:N2}", 13, "E5"));
                    sheetData.Append(linha5);

                    // CABEÇALHO DA TABELA (linha 6)
                    var cabecalhos = new[] { "Nº Registro", "Beneficiário", "Tipo", "Data Pagamento", "Valor Taxa" };
                    var headerRow = new X.Row();
                    for (int i = 0; i < cabecalhos.Length; i++)
                    {
                        headerRow.Append(CriarCelulaTexto(cabecalhos[i], 1, $"{LetraColuna(i + 1)}6"));
                    }
                    sheetData.Append(headerRow);

                    // DADOS (a partir da linha 7)
                    int linha = 7;
                    foreach (var item in dados)
                    {
                        var row = new X.Row();
                        row.Append(CriarCelulaTexto(item.NumeroRegistroPs1020.ToString(), 0, $"A{linha}"));
                        row.Append(CriarCelulaTexto(item.NomeBeneficiario ?? "", 0, $"B{linha}"));
                        row.Append(CriarCelulaTexto(item.TipoContratacaoAns ?? "", 0, $"C{linha}"));
                        row.Append(CriarCelulaTexto(item.DataPagamento?.ToString("dd/MM/yyyy") ?? "", 0, $"D{linha}"));
                        row.Append(CriarCelulaTexto(item.ValorEvento.ToString("N2"), 0, $"E{linha}"));
                        sheetData.Append(row);
                        linha++;
                    }

                    // LINHA DE TOTAL
                    var totalRow = new X.Row();
                    totalRow.Append(CriarCelulaTexto("", 0, $"A{linha}"));
                    totalRow.Append(CriarCelulaTexto("", 0, $"B{linha}"));
                    totalRow.Append(CriarCelulaTexto("", 0, $"C{linha}"));
                    totalRow.Append(CriarCelulaTexto("Valor Total", 1, $"D{linha}"));
                    totalRow.Append(CriarCelulaTexto(somaTotal.ToString("N2"), 1, $"E{linha}"));
                    sheetData.Append(totalRow);

                    // INSERIR O LOGO DA VALLOR
                    InserirLogoVallor(worksheetPart, CaminhoLogoVallor);

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

        // HELPERS

        private string LetraColuna(int indice)
        {
            return ((char)('A' + indice - 1)).ToString();
        }

        private string ObterVigenciaMesAno(List<TaxaAssociativaDemaisEntidadesModel> dados)
        {
            var menorData = dados
                .Where(d => d.DataPagamento.HasValue)
                .Select(d => d.DataPagamento.Value)
                .OrderBy(d => d)
                .FirstOrDefault();

            if (menorData == default(DateTime))
                return string.Empty;

            return menorData.ToString("MM.yyyy");
        }

        private string RemoverPrefixoEntidade(string nomeEntidade)
        {
            if (string.IsNullOrWhiteSpace(nomeEntidade))
                return string.Empty;

            var nome = nomeEntidade.Trim();

            if (nome.StartsWith("TX ASSOC ", StringComparison.OrdinalIgnoreCase))
                nome = nome.Substring("TX ASSOC ".Length);

            return nome.Trim().ToUpper();
        }

        // INSERIR LOGO NO EXCEL

        private void InserirLogoVallor(WorksheetPart worksheetPart, string caminhoLogo)
        {
            // Código idêntico ao seu original...
            if (!File.Exists(caminhoLogo))
                throw new FileNotFoundException($"Logo da Vallor não encontrado em: {caminhoLogo}");

            var drawingPart = worksheetPart.AddNewPart<DrawingsPart>();
            string drawingPartId = worksheetPart.GetIdOfPart(drawingPart);

            drawingPart.WorksheetDrawing = new Xdr.WorksheetDrawing();

            var imagePart = drawingPart.AddImagePart(ImagePartType.Png);
            using (var fs = new FileStream(caminhoLogo, FileMode.Open, FileAccess.Read))
            {
                imagePart.FeedData(fs);
            }

            string imagePartId = drawingPart.GetIdOfPart(imagePart);

            uint drawingObjectId = 1;

            var oneCellAnchor = new Xdr.OneCellAnchor(
                new Xdr.FromMarker(
                    new Xdr.ColumnId("2"),
                    new Xdr.ColumnOffset("0"),
                    new Xdr.RowId("0"),
                    new Xdr.RowOffset("0")
                ),
                new Xdr.Extent { Cx = 2000000, Cy = 900000 },
                new Xdr.Picture(
                    new Xdr.NonVisualPictureProperties(
                        new Xdr.NonVisualDrawingProperties { Id = drawingObjectId, Name = "Logo Vallor" },
                        new Xdr.NonVisualPictureDrawingProperties(new A.PictureLocks { NoChangeAspect = true })
                    ),
                    new Xdr.BlipFill(
                        new A.Blip { Embed = imagePartId },
                        new A.Stretch(new A.FillRectangle())
                    ),
                    new Xdr.ShapeProperties(
                        new A.Transform2D(
                            new A.Offset { X = 0, Y = 0 },
                            new A.Extents { Cx = 2000000, Cy = 900000 }
                        ),
                        new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle }
                    )
                )
            );

            drawingPart.WorksheetDrawing.Append(oneCellAnchor);
            drawingPart.WorksheetDrawing.Save();

            if (!worksheetPart.Worksheet.Elements<X.Drawing>().Any())
            {
                worksheetPart.Worksheet.AppendChild(new X.Drawing { Id = drawingPartId });
            }
        }


        // HELPERS DE ESTILO / CÉLULA

        private X.Border CriarBordaFina(string corArgb)
        {
            return new X.Border(
                new X.LeftBorder(new X.Color { Rgb = corArgb }) { Style = X.BorderStyleValues.Thin },
                new X.RightBorder(new X.Color { Rgb = corArgb }) { Style = X.BorderStyleValues.Thin },
                new X.TopBorder(new X.Color { Rgb = corArgb }) { Style = X.BorderStyleValues.Thin },
                new X.BottomBorder(new X.Color { Rgb = corArgb }) { Style = X.BorderStyleValues.Thin },
                new X.DiagonalBorder()
            );
        }

        private X.Stylesheet CriarStylesheet()
        {
            var fonts = new X.Fonts(
                new X.Font(new X.FontSize { Val = 11 }, new X.FontName { Val = "Calibri" }),
                new X.Font(new X.Bold(), new X.FontSize { Val = 11 }, new X.FontName { Val = "Calibri" }),
                new X.Font(new X.Bold(), new X.FontSize { Val = 12 }, new X.FontName { Val = "Calibri" }),
                new X.Font(new X.FontSize { Val = 10 }, new X.FontName { Val = "Calibri" }),
                new X.Font(new X.Bold(), new X.FontSize { Val = 12 }, new X.FontName { Val = "Calibri" })
                {
                    Color = new X.Color { Rgb = new DocumentFormat.OpenXml.HexBinaryValue { Value = "FFFFFF" } }
                }
            );

            var fills = new X.Fills(
                new X.Fill(new X.PatternFill { PatternType = X.PatternValues.None }),
                new X.Fill(new X.PatternFill { PatternType = X.PatternValues.Gray125 }),
                new X.Fill(new X.PatternFill
                {
                    PatternType = X.PatternValues.Solid,
                    ForegroundColor = new X.ForegroundColor { Rgb = new DocumentFormat.OpenXml.HexBinaryValue { Value = "E6E6E6" } },
                    BackgroundColor = new X.BackgroundColor { Indexed = 64 }
                }),
                new X.Fill(new X.PatternFill
                {
                    PatternType = X.PatternValues.Solid,
                    ForegroundColor = new X.ForegroundColor { Rgb = new DocumentFormat.OpenXml.HexBinaryValue { Value = "1F3B73" } },
                    BackgroundColor = new X.BackgroundColor { Indexed = 64 }
                })
            );

            var borders = new X.Borders(
                new X.Border(),
                CriarBordaFina("FFFFFFFF")
            );

            var cellFormats = new X.CellFormats(
                new X.CellFormat(),
                new X.CellFormat
                {
                    FontId = 1,
                    FillId = 2,
                    BorderId = 1,
                    ApplyFont = true,
                    ApplyFill = true,
                    ApplyBorder = true
                },
                new X.CellFormat(), new X.CellFormat(), new X.CellFormat(),
                new X.CellFormat(), new X.CellFormat(), new X.CellFormat(),
                new X.CellFormat(), new X.CellFormat(),
                new X.CellFormat
                {
                    FontId = 2,
                    FillId = 4,
                    BorderId = 1,
                    ApplyFont = true,
                    ApplyFill = true,
                    ApplyBorder = true
                },
                new X.CellFormat
                {
                    FontId = 0,
                    FillId = 4,
                    BorderId = 1,
                    ApplyFont = true,
                    ApplyFill = true,
                    ApplyBorder = true,
                    Alignment = new X.Alignment { Horizontal = X.HorizontalAlignmentValues.Right }
                },
                new X.CellFormat
                {
                    FontId = 3,
                    FillId = 4,
                    BorderId = 1,
                    ApplyFont = true,
                    ApplyFill = true,
                    ApplyBorder = true
                },
                new X.CellFormat
                {
                    FontId = 4,
                    FillId = 3,
                    BorderId = 1,
                    ApplyFont = true,
                    ApplyFill = true,
                    ApplyBorder = true,
                    Alignment = new X.Alignment
                    {
                        Horizontal = X.HorizontalAlignmentValues.Center,
                        Vertical = X.VerticalAlignmentValues.Center
                    }
                },
                new X.CellFormat { BorderId = 1, ApplyBorder = true },
                // 15 - Faixa azul com quebra de texto (Para a célula A5)
                new X.CellFormat
                {
                    FontId = 4,
                    FillId = 3,
                    BorderId = 1,
                    ApplyFont = true,
                    ApplyFill = true,
                    ApplyBorder = true,
                    Alignment = new X.Alignment
                    {
                        Horizontal = X.HorizontalAlignmentValues.Left,
                        Vertical = X.VerticalAlignmentValues.Center,
                        WrapText = true
                    }
                }
            );

            return new X.Stylesheet(fonts, fills, borders, cellFormats);
        }

        private X.Cell CriarCelulaTexto(string valor, uint estilo = 0, string referencia = null)
        {
            var cell = new X.Cell
            {
                DataType = X.CellValues.String,
                CellValue = new X.CellValue(valor ?? ""),
                StyleIndex = estilo
            };

            if (!string.IsNullOrEmpty(referencia))
                cell.CellReference = referencia;

            return cell;
        }

        // AUXILIAR

        private void ExibirMensagem(string mensagem, bool erro)
        {
            lblMensagem.Text = mensagem;
            lblMensagem.CssClass = "msg-importacao " + (erro ? "erro" : "sucesso");
        }
    }
}