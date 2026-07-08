using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace appWhatsapp
{
    public class FrameRedirectModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.EndRequest += OnEndRequest;
        }

        private void OnEndRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            var response = app.Response;
            var request = app.Request;

            if (response.StatusCode != 301 && response.StatusCode != 302) return;
            if (request.QueryString["frame"] != "1") return;

            string location = response.RedirectLocation;
            if (string.IsNullOrEmpty(location)) return;
            if (location.Contains("frame=1")) return;

            // ✅ Só intercepta URLs dentro da gestão (rotas amigáveis)
            // Ignora tudo que vai para ViewsApp, SignIn, erro, ou caminhos físicos .aspx
            if (location.Contains(".aspx")) return;
            if (location.Contains("ViewsApp")) return;
            if (location.Contains("SignIn")) return;
            if (location.Contains("signin")) return;
            if (location.Contains("erro")) return;

            string sep = location.Contains("?") ? "&" : "?";
            response.RedirectLocation = location + sep + "frame=1";
        }

        public void Dispose() { }
    }
}