using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Service.ServiceGestao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class listDemand : System.Web.UI.Page
    {
        private readonly DemandaService _svc = new DemandaService("Plennus");

        private int CodPessoaAtual => Convert.ToInt32(Session["CodPessoa"] ?? 0);
        private int? CodSetorAtual => Session["CodDepartamento"] == null ? (int?)null : Convert.ToInt32(Session["CodDepartamento"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarFiltros();
                BindGrid();
            }
        }

        private void CarregarFiltros()
        {
            ddlStatus.DataSource = _svc.GetSituacoesDemanda();
            ddlStatus.DataValueField = "Value";
            ddlStatus.DataTextField = "Text";
            ddlStatus.DataBind();
            ddlStatus.Items.Insert(0, new ListItem("Todos", ""));

            ddlCategoria.DataSource = _svc.GetCategoriasDemanda();
            ddlCategoria.DataValueField = "Value";
            ddlCategoria.DataTextField = "Text";
            ddlCategoria.DataBind();
            ddlCategoria.Items.Insert(0, new ListItem("Todas", ""));
        }

        protected void ddlCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlSubtipo.DataSource = _svc.GetSubtiposDemanda(int.Parse(ddlCategoria.SelectedValue));
            ddlSubtipo.DataValueField = "Value";
            ddlSubtipo.DataTextField = "Text";
            ddlSubtipo.DataBind();
            ddlSubtipo.Items.Insert(0, new ListItem("Todos", ""));
        }

        protected void btnFiltrar_Click(object sender, EventArgs e) => BindGrid();

        private void BindGrid()
        {
            var filtro = new DemandaFiltro
            {
                CodPessoa = CodPessoaAtual,
                CodSetor = CodSetorAtual,
                CodStatus = string.IsNullOrEmpty(ddlStatus.SelectedValue) ? (int?)null : int.Parse(ddlStatus.SelectedValue),
                CodCategoria = string.IsNullOrEmpty(ddlCategoria.SelectedValue) ? (int?)null : int.Parse(ddlCategoria.SelectedValue),
                CodSubtipo = string.IsNullOrEmpty(ddlSubtipo.SelectedValue) ? (int?)null : int.Parse(ddlSubtipo.SelectedValue),
                NomeSolicitante = txtSolicitante.Text.Trim(),
                Visibilidade = "M" 
            };


            var lista = _svc.ListarDemandas(filtro);
            gvDemandas.DataSource = lista;
            gvDemandas.DataBind();
        }

        protected void gvDemandas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDemandas.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvDemandas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ver")
                Response.Redirect("detailDemand.aspx?id=" + e.CommandArgument);
        }

        protected void btnNovaDemanda_Click(object sender, EventArgs e)
        {

        }
    }
}