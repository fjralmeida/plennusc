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

            string secFetchDest = Request.Headers["Sec-Fetch-Dest"] ?? "";

            // Se veio de dentro do iframe, passa direto
            if (secFetchDest == "iframe") return;

            // Se é document (F5, link direto, nova aba) — redireciona pra shell
            string paginaAtual = Request.Url.PathAndQuery;
            Response.Redirect("/gestao?p=" + Uri.EscapeDataString(paginaAtual), true);
        }
    }
}