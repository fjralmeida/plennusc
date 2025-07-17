using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PlennuscGestao.Views
{
    public partial class HomeGestao : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //lblNomeUsuario.Text = "Thales"; // Opcional
                lblTotalColaboradores.Text = "12"; // Exemplo fictício
                lblTotalDepartamentos.Text = "5";
                lblTotalEmpresas.Text = "3";
            }
        }
    }
}