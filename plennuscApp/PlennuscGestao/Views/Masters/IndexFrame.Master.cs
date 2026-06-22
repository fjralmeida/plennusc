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
            if (Request.QueryString["frame"] == "1") return;

            string paginaAtual = Request.Url.PathAndQuery;
            Response.Redirect("/gestao?p=" + Uri.EscapeDataString(paginaAtual), true);
        }
    }
}