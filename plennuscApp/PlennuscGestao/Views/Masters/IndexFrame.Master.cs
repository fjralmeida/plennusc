using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views.Masters
{
    public partial class IndexFrame : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Se não veio do iframe (acessou direto), redireciona para a shell
            if (!IsPostBack && Request.Headers["Sec-Fetch-Dest"] != "iframe")
            {
                string paginaAtual = Request.Url.PathAndQuery;
                Response.Redirect("/gestao?p=" + Uri.EscapeDataString(paginaAtual));
                return;
            }
        }
    }
}