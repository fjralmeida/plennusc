using Plennusc.Core.Service.ServiceGestao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class myDemands : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["CodPessoa"] == null)
                {
                    Response.Redirect("~/SignIn.aspx");
                    return;
                }
            }
            BindGrid();
        }

        private void BindGrid()
        {
            int codPessoa = Convert.ToInt32(Session["CodPessoa"]);

            try
            {
                var svc = new DemandaService("Plennus");
                var dt = svc.GetDemandasParaListagem(codPessoa);
                gvMinhasDemandas.DataSource = dt;
                gvMinhasDemandas.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro ao carregar demandas: " + ex.Message, "error");
            }
        }
        private void MostrarMensagem(string mensagem, string tipo)
        {
            string script = $@"showToast{(tipo == "success" ? "Sucesso" : "Erro")}('{mensagem.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Mensagem", script, true);
        }
    }
}