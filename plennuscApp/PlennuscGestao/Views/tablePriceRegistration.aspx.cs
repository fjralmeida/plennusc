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
    public partial class tablePriceRegistration : System.Web.UI.Page
    {
        private readonly PrecoService _svc = new PrecoService("Plennus");

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
            var filtro = new PrecoFiltro
            {
                NomePlanoComercial = txtNomePlanoComercial.Text.Trim()
            };

            var lista = _svc.ListarPrecos(filtro);
            gvPrecos.DataSource = lista;
            gvPrecos.DataBind();
        }

        protected void gvPrecos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPrecos.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvPrecos_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ver")
            {
                int codPlano = Convert.ToInt32(e.CommandArgument);
                Session["CurrentPlanoId"] = codPlano;
                Response.Redirect("~/viewPlano");
            }
        }

        protected void btnNovoPreco_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/novoPlano");
        }

        protected void gvPrecos_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }
    }
}