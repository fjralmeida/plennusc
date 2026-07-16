using Plennusc.Core.Models.ModelsGestao.modelsBilling;
using Plennusc.Core.Service.ServiceGestao.serviceBilling;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class billingInconsistency : System.Web.UI.Page
    {
        private readonly ServiceBillingReconciliation _service = new ServiceBillingReconciliation();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarOperadoras();
                // Restaura o PageSize salvo em ViewState
                if (ViewState["PageSize"] != null)
                {
                    gridPreview.PageSize = (int)ViewState["PageSize"];
                    ddlPageSize.SelectedValue = gridPreview.PageSize.ToString();
                }
            }
            else
            {
                RecriarBotoesPaginacaoGridPreview();
            }
        }

        private void CarregarOperadoras()
        {
            var operadoras = _service.ObterOperadoras();
            ddlOperadora.DataSource = operadoras;
            ddlOperadora.DataTextField = "NomeOperadora";
            ddlOperadora.DataValueField = "CodigoGrupoContrato";
            ddlOperadora.DataBind();
            ddlOperadora.Items.Insert(0, new ListItem("Selecione...", ""));
        }

        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlOperadora.SelectedValue))
            {
                ExibirMensagem("Selecione uma operadora.", erro: true);
                return;
            }

            string mesAnoReferencia = txtMesAnoReferencia.Text.Trim();
            if (string.IsNullOrEmpty(mesAnoReferencia))
            {
                ExibirMensagem("Informe o Mês/Ano Referência.", erro: true);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(mesAnoReferencia, @"^(0[1-9]|1[0-2])\/\d{4}$"))
            {
                ExibirMensagem("Formato inválido. Use MM/AAAA.", erro: true);
                return;
            }

            int codigoGrupoContrato = Convert.ToInt32(ddlOperadora.SelectedValue);

            try
            {
                var itens = _service.ObterInconsistenciasFaturamento(mesAnoReferencia, codigoGrupoContrato);

                // Filtro por nome ou CPF
                string busca = txtBusca.Text.Trim();
                if (!string.IsNullOrEmpty(busca))
                {
                    itens = itens.Where(x =>
                        (x.NomeDoAssociado != null && x.NomeDoAssociado.ToLower().Contains(busca.ToLower())) ||
                        (x.NumeroCpf != null && x.NumeroCpf.Contains(busca))
                    ).ToList();
                }

                Session["BillingInconsistency_Itens"] = itens;

                // Aplica o PageSize atual
                gridPreview.PageSize = Convert.ToInt32(ddlPageSize.SelectedValue);
                ViewState["PageSize"] = gridPreview.PageSize;

                gridPreview.DataSource = itens;
                gridPreview.DataBind();

                divResultado.Attributes.Remove("class");
                divResultado.Attributes.Add("class", "filters-card");

                if (itens.Count == 0)
                    ExibirMensagem("Nenhuma inconsistência encontrada.", erro: false);
                else
                    ExibirMensagem($"{itens.Count} registro(s) pendente(s) de conferência encontrado(s).", aviso: true);
            }
            catch (Exception ex)
            {
                ExibirMensagem("Erro ao pesquisar: " + ex.Message, erro: true);
            }
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            int newSize = Convert.ToInt32(ddlPageSize.SelectedValue);
            gridPreview.PageSize = newSize;
            ViewState["PageSize"] = newSize;
            gridPreview.PageIndex = 0;

            var itens = Session["BillingInconsistency_Itens"] as List<ItemInconsistenciaFaturamento>;
            if (itens != null)
            {
                gridPreview.DataSource = itens;
                gridPreview.DataBind();
            }
        }

        protected void gridPreview_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridPreview.PageIndex = e.NewPageIndex;
            var itens = Session["BillingInconsistency_Itens"] as List<ItemInconsistenciaFaturamento>;
            if (itens != null)
            {
                gridPreview.DataSource = itens;
                gridPreview.DataBind();
            }
        }

        protected void gridPreview_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Opcional: associar dado ao checkbox, se necessário
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Exemplo: CheckBox chk = (CheckBox)e.Row.FindControl("chkSelecionar");
                // chk.Attributes["data-id"] = DataBinder.Eval(e.Row.DataItem, "NumeroRegistro").ToString();
            }
        }

        protected void gridPreview_DataBound(object sender, EventArgs e)
        {
            // Sincroniza o dropdown com o PageSize atual
            if (ViewState["PageSize"] != null)
            {
                int pageSize = (int)ViewState["PageSize"];
                gridPreview.PageSize = pageSize;
                ddlPageSize.SelectedValue = pageSize.ToString();
            }
            else
            {
                ddlPageSize.SelectedValue = gridPreview.PageSize.ToString();
            }

            var itens = Session["BillingInconsistency_Itens"] as List<ItemInconsistenciaFaturamento>;
            ViewState["TotalRegistrosGridPreview"] = itens?.Count ?? 0;

            GridViewRow pagerRow = gridPreview.BottomPagerRow;
            if (pagerRow == null) return;

            Label lblInfo = (Label)pagerRow.FindControl("lblPagerInfo");
            if (lblInfo != null)
            {
                int totalGeral = ObterTotalRegistrosGridPreview();
                int paginaAtual = gridPreview.PageIndex;
                int tamanhoPagina = gridPreview.PageSize;
                int inicio = totalGeral == 0 ? 0 : (paginaAtual * tamanhoPagina) + 1;
                int fim = Math.Min(inicio + gridPreview.Rows.Count - 1, totalGeral);
                lblInfo.Text = $"<strong>{inicio} - {fim}</strong> de <strong>{totalGeral}</strong> itens";
            }

            RecriarBotoesPaginacaoGridPreview();
        }

        private void RecriarBotoesPaginacaoGridPreview()
        {
            GridViewRow pagerRow = gridPreview.BottomPagerRow;
            if (pagerRow == null) return;

            PlaceHolder ph = (PlaceHolder)pagerRow.FindControl("phPagerButtons");
            if (ph == null) return;

            ph.Controls.Clear();

            int totalGeral = ObterTotalRegistrosGridPreview();
            if (totalGeral == 0) return;

            int totalPaginas = (int)Math.Ceiling((double)totalGeral / gridPreview.PageSize);
            int currentPage = gridPreview.PageIndex;

            void AddBtn(string text, int pageIndex, bool disabled)
            {
                LinkButton btn = new LinkButton();
                btn.Text = text;
                btn.CssClass = "pager-button" + (pageIndex == currentPage ? " active" : "");
                btn.CommandName = "Page";
                btn.CommandArgument = pageIndex.ToString();
                btn.Click += (s, ev) =>
                {
                    gridPreview.PageIndex = pageIndex;
                    var itens = Session["BillingInconsistency_Itens"] as List<ItemInconsistenciaFaturamento>;
                    if (itens != null)
                    {
                        gridPreview.DataSource = itens;
                        gridPreview.DataBind();
                    }
                };
                btn.Enabled = !disabled;
                if (disabled) btn.CssClass += " disabled";
                ph.Controls.Add(btn);
            }

            AddBtn("«", 0, currentPage == 0);
            AddBtn("‹", currentPage - 1, currentPage == 0);

            int start = Math.Max(0, currentPage - 3);
            int end = Math.Min(totalPaginas - 1, currentPage + 3);
            for (int i = start; i <= end; i++)
                AddBtn((i + 1).ToString(), i, false);

            AddBtn("›", currentPage + 1, currentPage >= totalPaginas - 1);
            AddBtn("»", totalPaginas - 1, currentPage >= totalPaginas - 1);
        }

        private int ObterTotalRegistrosGridPreview()
        {
            return ViewState["TotalRegistrosGridPreview"] as int? ?? 0;
        }

        private void ExibirMensagem(string mensagem, bool erro = false, bool aviso = false)
        {
            lblMensagemPesquisa.Text = mensagem;
            string classe = "msg-importacao";
            if (erro) classe += " erro";
            else if (aviso) classe += " aviso";
            else classe += " sucesso";
            lblMensagemPesquisa.CssClass = classe;
        }

        // 🔥 EXPORTAÇÃO PARA EXCEL
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            var itens = Session["BillingInconsistency_Itens"] as List<ItemInconsistenciaFaturamento>;

            if (itens == null || itens.Count == 0)
            {
                ExibirMensagem("Não há dados para exportar.", erro: true);
                return;
            }

            ExportarParaExcel(itens);
        }

        private void ExportarParaExcel(List<ItemInconsistenciaFaturamento> itens)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("CPF", typeof(string));
            dt.Columns.Add("Nome", typeof(string));
            dt.Columns.Add("Mês/Ano", typeof(string));
            dt.Columns.Add("Valor Convênio", typeof(decimal));
            dt.Columns.Add("Valor Adicional", typeof(decimal));
            dt.Columns.Add("Valor Fatura", typeof(decimal));
            dt.Columns.Add("Data Admissão", typeof(string));
            dt.Columns.Add("Data Exclusão", typeof(string));

            foreach (var item in itens)
            {
                dt.Rows.Add(
                    item.NumeroCpf,
                    item.NomeDoAssociado,
                    item.MesAnoReferencia,
                    item.ValorConvenio,
                    item.ValorAdicional,
                    item.ValorFatura,
                    item.DataAdmissao?.ToString("dd/MM/yyyy"),
                    item.DataExclusao?.ToString("dd/MM/yyyy")
                );
            }

            Response.Clear();
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", $"attachment; filename=Inconsistencias_{DateTime.Now:yyyyMMdd_HHmmss}.xls");

            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            using (HtmlTextWriter htw = new HtmlTextWriter(sw))
            {
                htw.Write(@"
                <html xmlns:o='urn:schemas-microsoft-com:office:office' 
                      xmlns:x='urn:schemas-microsoft-com:office:excel' 
                      xmlns='http://www.w3.org/TR/REC-html40'>
                <head>
                <!--[if gte mso 9]>
                <xml>
                <x:ExcelWorkbook>
                <x:ExcelWorksheets>
                <x:ExcelWorksheet>
                <x:Name>Inconsistencias</x:Name>
                <x:WorksheetOptions>
                <x:DisplayGridlines/>
                </x:WorksheetOptions>
                </x:ExcelWorksheet>
                </x:ExcelWorksheets>
                </x:ExcelWorkbook>
                </xml>
                <![endif]-->
                <style>
                    body { font-family: Calibri, Arial, sans-serif; }
                    table { border-collapse: collapse; width: 100%; }
                    th, td { 
                        border: 1px solid #999; 
                        padding: 6px 8px; 
                        font-size: 12px;
                        vertical-align: middle;
                        text-align: left;
                    }
                    th { 
                        background-color: #e6e6e6; 
                        font-weight: bold; 
                        text-align: center;
                    }
                    .numero { text-align: right; }
                    .data { text-align: center; }
                    .texto { text-align: left; }
                </style>
                </head>
                <body>");

                htw.Write("<table>");
                htw.Write("<tr>");
                foreach (DataColumn col in dt.Columns)
                {
                    htw.Write($"<th>{col.ColumnName}</th>");
                }
                htw.Write("</tr>");

                foreach (DataRow row in dt.Rows)
                {
                    htw.Write("<tr>");
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        string classe = "texto";
                        if (i == 0) classe = "texto";
                        else if (i == 1) classe = "texto";
                        else if (i == 2) classe = "data";
                        else if (i >= 3 && i <= 5) classe = "numero";
                        else if (i >= 6) classe = "data";

                        object valor = row[i];
                        string conteudo = valor != DBNull.Value && valor != null ? valor.ToString() : "";

                        if (i >= 3 && i <= 5 && valor != DBNull.Value && valor != null)
                        {
                            decimal dec = Convert.ToDecimal(valor);
                            conteudo = dec.ToString("N2");
                        }

                        htw.Write($"<td class='{classe}'>{conteudo}</td>");
                    }
                    htw.Write("</tr>");
                }

                htw.Write("</table>");
                htw.Write("</body></html>");

                Response.Write(sw.ToString());
                Response.End();
            }
        }

        protected void btnConferirSelecionados_Click(object sender, EventArgs e)
        {
            var selecionados = ObterItensSelecionados();

            if (selecionados.Count == 0)
            {
                ExibirMensagem("Selecione pelo menos um registro para conferir.", erro: true);
                return;
            }

            try
            {
                // Atualiza DATA_CONFERENCIA_FATUR no banco
                _service.ConferirInconsistencias(selecionados);

                ExibirMensagem($"{selecionados.Count} registro(s) conferido(s) com sucesso.", erro: false);

                // Recarrega a lista removendo os conferidos (ou refaz a pesquisa)
                RecarregarLista();

                // Rebinda o grid
                var itens = Session["BillingInconsistency_Itens"] as List<ItemInconsistenciaFaturamento>;
                if (itens != null)
                {
                    gridPreview.DataSource = itens;
                    gridPreview.DataBind();
                }

                // Se não houver mais itens, esconde o card de resultado
                if (itens == null || itens.Count == 0)
                {
                    divResultado.Attributes.Add("class", "filters-card hidden");
                    ExibirMensagem("Todos os registros foram conferidos.", erro: false);
                }
            }
            catch (Exception ex)
            {
                ExibirMensagem("Erro ao conferir: " + ex.Message, erro: true);
            }
        }

        private List<ItemInconsistenciaFaturamento> ObterItensSelecionados()
        {
            var selecionados = new List<ItemInconsistenciaFaturamento>();
            var itens = Session["BillingInconsistency_Itens"] as List<ItemInconsistenciaFaturamento>;

            if (itens == null) return selecionados;

            foreach (GridViewRow row in gridPreview.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chk = (CheckBox)row.FindControl("chkSelecionar");
                    if (chk != null && chk.Checked)
                    {
                        int rowIndex = row.RowIndex;
                        int dataIndex = gridPreview.PageIndex * gridPreview.PageSize + rowIndex;
                        if (dataIndex < itens.Count)
                        {
                            selecionados.Add(itens[dataIndex]);
                        }
                    }
                }
            }
            return selecionados;
        }

        private void RecarregarLista()
        {
            // Refaz a pesquisa com os mesmos filtros
            btnPesquisar_Click(null, null);
        }
    }
}
