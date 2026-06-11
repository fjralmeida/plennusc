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
    public partial class accessoriesRegistration : System.Web.UI.Page
    {
        private readonly AcessorioService _svc = new AcessorioService("Plennus");

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
            var filtro = new AcessorioFiltro
            {
                NomeAcessorio = txtAcessorio.Text.Trim(),
                ValorAcessorio = txtValor.Text.Trim()
            };

            var lista = _svc.ListarAcessorios(filtro);
            gvAcessorios.DataSource = lista;
            gvAcessorios.DataBind();
        }

        protected void gvAcessorios_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAcessorios.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvAcessorios_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ver")
            {
                int codAcessorio = Convert.ToInt32(e.CommandArgument);
                Session["CurrentAcessorioId"] = codAcessorio;
                Response.Redirect("~/viewAcessorio");
            }
        }

        protected void btnNovoAcessorio_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/novoAcessorio");
        }

        protected void gvAcessorios_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }
    }
}