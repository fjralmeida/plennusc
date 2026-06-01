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
            if (IsPostBack) return;

            if (Request.QueryString["frame"] != "1")
            {
                string paginaAtual = Request.Url.PathAndQuery;
                // Evita loop: se já está dentro do /gestao não redireciona
                if (!Request.Url.AbsolutePath.StartsWith("/gestao"))
                {
                    Response.Redirect("/gestao?p=" + Uri.EscapeDataString(paginaAtual), true);
                }
            }
        }
    }
}