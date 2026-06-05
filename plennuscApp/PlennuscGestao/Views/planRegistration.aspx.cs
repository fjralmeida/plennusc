using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Service.ServiceGestao;
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

            var lista = _svc.ListarPlanos(filtro);
            gvPlanos.DataSource = lista;
            gvPlanos.DataBind();
        }

        protected void gvPlanos_PageIndexChanging(object sender, GridViewPageEventArgs e)
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

    }
}
    