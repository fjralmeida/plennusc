using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Service.ServiceGestao.planiumApi;
using Plennusc.Core.Service.ServiceGestao.department;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class commercializationRegistration : System.Web.UI.Page
    {
        private readonly ComercializacaoService _svc = new ComercializacaoService("Plennus");

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
            var filtro = new ComercializacaoFiltro
            {
                SiglaEstado = txtEstado.Text.Trim(),
                NomeCidade = txtCidade.Text.Trim()
            };

            var lista = _svc.ListarComercializacao(filtro);
            gvComercializacao.DataSource = lista;
            gvComercializacao.DataBind();
        }

        protected void gvComercializacao_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvComercializacao.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvComercializacao_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ver")
            {
                int codComercializacao = Convert.ToInt32(e.CommandArgument);
                Session["CurrentComercializacaoId"] = codComercializacao;
                Response.Redirect("~/viewComercializacao");
            }
        }

        protected void btnNovaComercializacao_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/novaComercializacao");
        }

        protected void gvComercializacao_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }
    }
}