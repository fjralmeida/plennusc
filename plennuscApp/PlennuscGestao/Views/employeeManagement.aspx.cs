using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class employeeManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnIncluirUsuario_Click(object sender, EventArgs e)
        {
            PanelCadastro.Visible = true;
            lblTitGestao.Visible = false;
            btnConsultarUsuario.Visible = false;
            btnDesativarUsuario.Visible = false;
            btnIncluirUsuario.Visible = false;
        }

        protected void btnConsultarUsuario_Click(object sender, EventArgs e)
        {
            // Redirecionar para página de busca ou exibir grid
        }

        protected void btnDesativarUsuario_Click(object sender, EventArgs e)
        {
            // Abrir tela/modal de seleção para desativação
        }

        protected void btnSalvarUsuario_Click(object sender, EventArgs e)
        {

        }
    }
}