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
    public partial class entityRegistration : System.Web.UI.Page
    {
        private readonly EntidadeService _svc = new EntidadeService("Plennus");

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
            var filtro = new EntidadeFiltro
            {
                NomeEntidade = txtEntidade.Text.Trim(),
                Numero_CNPJ = txtCnpj.Text.Trim()
            };

            var lista = _svc.ListarEntidade(filtro);
            gvEntidade.DataSource = lista;
            gvEntidade.DataBind();
        }

        protected void gvEntidade_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEntidade.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvEntidade_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ver")
            {
                int codEntidade = Convert.ToInt32(e.CommandArgument);
                Session["CurrentEntidadeId"] = codEntidade;
                Response.Redirect("~/viewEntidade");
            }
        }

        protected void btnNovaEntidade_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/novaEntidade");
        }

        protected void gvEntidade_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }
    }
}