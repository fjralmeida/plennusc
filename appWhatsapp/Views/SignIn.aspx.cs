using appWhatsapp.Data_Bd;
using appWhatsapp.SqlQueries;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.Views
{
    public partial class SignIn : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void ButtonSignIn_Click(object sender, EventArgs e)
        {
            string login = TextBoxEmail.Text.Trim();
            string senha = TextBoxPassword.Text;

            ItensPedIntegradoUtil util = new ItensPedIntegradoUtil();
            DataTable dtUser = util.ConsultaLoginUsuario(login, senha);

            if (dtUser.Rows.Count > 0)
            {
                // Usuário autenticado
                Session["UsuarioLogado"] = dtUser.Rows[0]["NomeUsuario"].ToString();
                Response.Redirect("Home.aspx");

            }
            else
            {
                //LabelErro.Text = "Usuário ou senha inválidos.";
            }
        } 
    }
}