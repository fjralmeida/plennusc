using appWhatsapp.SqlQueries;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.Views
{
    public partial class Index : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtBusca.Text = "";

                lblUsuario.Text = Session["NomeUsuario"]?.ToString() ?? "Usuário";
                lblNomeSistema.Text = Session["NomeEmpresa"]?.ToString() ?? "Empresa";

                // Se tiver o CodEmpresa na sessão, carrega a logo correta
                if (Session["CodEmpresa"] != null)
                {
                    int codEmpresa = Convert.ToInt32(Session["CodEmpresa"]);

                    CarregarInfoEmpresa(codEmpresa);
                }
            }
        }

        private void CarregarInfoEmpresa(int codEmpresa)
        {
            ItensPedIntegradoUtil util = new ItensPedIntegradoUtil();
            DataTable dtEmpresa = util.ConsultaInfoEmpresa(codEmpresa);

            if (dtEmpresa.Rows.Count > 0)
            {
                imgLogo.ImageUrl = ResolveUrl("~/Uploads/" + dtEmpresa.Rows[0]["Conf_Logo"].ToString());
                lblNomeSistema.Text = dtEmpresa.Rows[0]["NomeFantasia"].ToString();
            }
        }
    }
}