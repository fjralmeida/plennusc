using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using Microsoft.AspNet.FriendlyUrls;

namespace appWhatsapp
{
    public static class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //// Primeiro registra as rotas personalizadas
            //routes.MapPageRoute("LoginRoute", "login", "~/Views/SignIn.aspx");
            //routes.MapPageRoute("EnvioMensagemRoute", "mensagem", "~/Views/EnvioMensagemBeneficiario.aspx");
            //routes.MapPageRoute("TelaEnvioRoute", "telaenvio", "~/Views/TelaEnvio.aspx");
            //routes.MapPageRoute(
            //    "HomeRoute",
            //    "home",
            //    "~/Views/Home.aspx"
            //);

            // Depois habilita FriendlyUrls para o resto do site
            var settings = new FriendlyUrlSettings();
            settings.AutoRedirectMode = RedirectMode.Permanent;
            routes.EnableFriendlyUrls(settings);

        }
    }
}
