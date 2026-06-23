using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Service.ServiceGestao.planiumApi;
using Plennusc.Core.Service.ServiceGestao.department;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class planRegistration : System.Web.UI.Page
    {
        private readonly PlanoService _svc = new PlanoService("Plennus");
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e) => BindGrid();

        private void BindGrid()
        {
            var filtro = new PlanoFiltro
            {
                NomePlanoComercial = txtNomePlanoComercial.Text.Trim(),
                Segmentacao = txtSegmentacao.Text.Trim(),
                Abrangencia = txtAbrangencia.Text.Trim(),
                Coparticipacao = txtCoparticipacao.Text.Trim(),
            };

            var listaPlanos = _svc.ListarPlanos(filtro);
            int total = listaPlanos.Count;
            ViewState["TotalRegistros"] = total;

            gvPlanos.DataSource = listaPlanos;
            gvPlanos.DataBind();
        }

        protected void GvPlanos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPlanos.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvPlanos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ver")
            {
                int codPlano = Convert.ToInt32(e.CommandArgument);
                Session["CurrentPlanoId"] = codPlano;
                Response.Redirect("~/viewPlano");
            }
        }

        protected void btnNovoPlano_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/novoPlano");
        }

        protected void gvPlanos_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        protected void gvPlanos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
        }

        protected void gvPlanos_DataBound(object sender, EventArgs e)
        {
            GridViewRow pagerRow = gvPlanos.BottomPagerRow;
            if (pagerRow == null) return;

            // Atualiza a contagem
            Label lblInfo = (Label)pagerRow.FindControl("lblPagerInfo");
            if (lblInfo != null)
            {
                int totalGeral = ObterTotalRegistros();
                int paginaAtual = gvPlanos.PageIndex;
                int tamanhoPagina = gvPlanos.PageSize;
                int inicio = (paginaAtual * tamanhoPagina) + 1;
                int fim = Math.Min(inicio + gvPlanos.Rows.Count - 1, totalGeral);

                lblInfo.Text = $"<strong>{inicio} - {fim}</strong> de <strong>{totalGeral}</strong> itens";
            }

            // Recria os botões (usando o método comum)
            RecriarBotoesPaginacao();
        }

        private void RecriarBotoesPaginacao()
        {
            GridViewRow pagerRow = gvPlanos.BottomPagerRow;
            if (pagerRow == null) return;

            PlaceHolder ph = (PlaceHolder)pagerRow.FindControl("phPagerButtons");
            if (ph == null) return;

            ph.Controls.Clear();

            int totalGeral = ObterTotalRegistros();
            if (totalGeral == 0) return;

            int totalPaginas = (int)Math.Ceiling((double)totalGeral / gvPlanos.PageSize);
            int currentPage = gvPlanos.PageIndex;

            // Função local para adicionar botão
            void AddBtn(string text, int pageIndex, bool disabled)
            {
                LinkButton btn = new LinkButton();
                btn.Text = text;
                btn.CssClass = "pager-button" + (pageIndex == currentPage ? " active" : "");
                btn.CommandName = "Page";
                btn.CommandArgument = pageIndex.ToString();
                btn.Enabled = !disabled;
                if (disabled) btn.CssClass += " disabled";
                ph.Controls.Add(btn);
            }

            AddBtn("«", 0, currentPage == 0);
            AddBtn("‹", currentPage - 1, currentPage == 0);

            int start = Math.Max(0, currentPage - 3);
            int end = Math.Min(totalPaginas - 1, currentPage + 3);
            for (int i = start; i <= end; i++)
            {
                AddBtn((i + 1).ToString(), i, false);
            }

            AddBtn("›", currentPage + 1, currentPage >= totalPaginas - 1);
            AddBtn("»", totalPaginas - 1, currentPage >= totalPaginas - 1);
        }

        private int ObterTotalRegistros()
        {
            if (ViewState["TotalRegistros"] != null)
                return (int)ViewState["TotalRegistros"];
            return 0;
        }

        protected void btnVerPendentes_Click(object sender, EventArgs e)
        {

        }

        private void MostrarAlerta(string mensagem, string tipo = "info")
        {
            pnlMensagem.Visible = true;
            lblMensagem.Text = mensagem;
            pnlMensagem.CssClass = $"alert alert-{tipo} alert-dismissible mb-3";
        }
    }
}
    