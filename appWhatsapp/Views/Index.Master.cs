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
                // Busca (opcional: preencher automaticamente)
                txtBusca.Text = "";

                // Usuário logado (de Session)
                lblUsuario.Text = Session["UsuarioLogado"]?.ToString() ?? "Usuário";

                ItensPedIntegradoUtil util = new ItensPedIntegradoUtil();
                DataTable dtUser = util.ConsultaInfoPerfil();

                if (dtUser.Rows.Count > 0)
                {
                    lblNomeSistema.Text = dtUser.Rows[0]["Nome"]?.ToString();
                    imgLogo.ImageUrl = ResolveUrl("~" + dtUser.Rows[0]["Conf_Simbolo"].ToString());

                }
            }
        }
    }
}