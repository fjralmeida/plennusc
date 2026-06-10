using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Service.ServiceGestao.planiumApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class careerRegistration : System.Web.UI.Page
    {
        private readonly ProfissaoService _svc = new ProfissaoService("Plennus");

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
            var filtro = new ProfissaoFiltro
            {
                NomeProfissao = txtProfissao.Text.Trim(),
            };

            var lista = _svc.ListarProfissao(filtro);
            gvProfissao.DataSource = lista;
            gvProfissao.DataBind();
        }

        protected void gvProfissao_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProfissao.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvProfissao_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ver")
            {
                int codProfissao = Convert.ToInt32(e.CommandArgument);
                Session["CurrentProfissaoId"] = codProfissao;
                Response.Redirect("~/viewProfissao");
            }
        }

        protected void btnNovaProfissao_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/novaProfissao");
        }

        protected void gvProfissao_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }
    }
}