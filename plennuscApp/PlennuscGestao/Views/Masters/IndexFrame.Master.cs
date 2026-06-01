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

            // Aceita se tem frame=1
            if (Request.QueryString["frame"] == "1") return;

            // Aceita qualquer redirect interno (mesmo domínio/host)
            string refererHost = Request.UrlReferrer?.Host ?? "";
            string currentHost = Request.Url.Host;
            if (!string.IsNullOrEmpty(refererHost) && refererHost == currentHost) return;

            // F5 ou acesso direto — redireciona para o shell passando a página atual
            string paginaAtual = Request.Url.PathAndQuery;

            // Remove ?frame=1 se já existir para não duplicar
            paginaAtual = System.Text.RegularExpressions.Regex.Replace(
                paginaAtual, @"[?&]frame=1", "");

            Response.Redirect("/gestao?p=" + Uri.EscapeDataString(paginaAtual), true);
        }
    }
}