using Plennusc.Core.Service.ServiceGestao.company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class userCompanyManagement : System.Web.UI.Page
    {
        private UserCompanyService _userCompanyService;
        protected void Page_Load(object sender, EventArgs e)
        {
            _userCompanyService = new UserCompanyService();
            if (Session["CodUsuario"] == null)
            {
                Response.Redirect("~/SignIn.aspx");
                return;
            }
            CarregarUsuarios();
        }

        private void CarregarUsuarios()
        {
            try
            {
                var usuarios = _userCompanyService.ListarUsuariosComAcesso();
                gvUsuarios.DataSource = usuarios;
                gvUsuarios.DataBind();
            }
            catch (Exception ex)
            {

            }
        }

        protected void btnVincular_Command(object sender, CommandEventArgs e)
        {
            string[] args = e.CommandArgument.ToString().Split(',');
            if (args.Length == 2)
            {
                Session["Vinculacao_CodPessoa"] = args[0];
                Session["Vinculacao_CodAutenticacao"] = args[1];

                // Redireciona via rota amigável, sem parâmetros na URL
                string url = GetRouteUrl("vincularEmpresasUsuario", null);
                Response.Redirect(url, false);
            }
        }
    }
}