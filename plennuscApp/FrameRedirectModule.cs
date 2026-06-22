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

            // Só intercepta redirects (301/302)
            if (response.StatusCode != 301 && response.StatusCode != 302) return;

            // Só intercepta se veio de dentro do iframe (tem frame=1)
            if (request.QueryString["frame"] != "1") return;

            // Pega a URL de destino do redirect
            string location = response.RedirectLocation;
            if (string.IsNullOrEmpty(location)) return;

            // Já tem frame=1? Não duplica
            if (location.Contains("frame=1")) return;

            // Adiciona frame=1 na URL de destino
            string sep = location.Contains("?") ? "&" : "?";
            response.RedirectLocation = location + sep + "frame=1";
        }

        public void Dispose() { }
    }
}