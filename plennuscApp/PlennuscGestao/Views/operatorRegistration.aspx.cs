using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Service.ServiceGestao;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class operatorRegistration : System.Web.UI.Page
    {
        private readonly OperadoraService _svc = new OperadoraService("Plennus");

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
            var filtro = new OperadoraFiltro
            {
                NomeOperadora = txtOperadora.Text.Trim(),
                RegistroANS = txtRegistroAns.Text.Trim(),
                Numero_CNPJ = txtCnpj.Text.Trim()
            };

            var lista = _svc.ListarOperadoras(filtro);
            gvOperadoras.DataSource = lista;
            gvOperadoras.DataBind();
        }

        protected void gvOperadoras_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvOperadoras.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvOperadoras_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ver")
            {
                int codOperadora = Convert.ToInt32(e.CommandArgument);
                Session["CurrentOperadoraId"] = codOperadora;
                Response.Redirect("~/viewOperadora");
            }
        }

        protected void btnNovaOperadora_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/novaOperadora");
        }

        protected void gvOperadoras_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // extensão futura
        }
    }
}