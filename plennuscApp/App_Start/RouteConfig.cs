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
            // ✅ ROTAS PARA ESTRUTURAS
            routes.MapPageRoute(
                "RegisterStructureTypeRoute",
                "estruturas/tipos/cadastrar",
                "~/PlennuscGestao/Views/registerStructureType.aspx"
            );

            routes.MapPageRoute(
                "ListStructureTypesRoute",
                "estruturas/tipos/listar",
                "~/PlennuscGestao/Views/listStructureTypes.aspx"
            );

            routes.MapPageRoute(
                "RegisterStructuresRoute",
                "estruturas/cadastrar",
                "~/PlennuscGestao/Views/registerStructures.aspx"
            );

            routes.MapPageRoute(
                "LinkSectorRoute",
                "estruturas/vincular-setor",
                "~/PlennuscGestao/Views/linkSector.aspx"
            );

            // ✅ ROTAS PARA DEMANDAS (que já tínhamos)
            routes.MapPageRoute(
                "DetailDemandRoute",
                "detailDemand",
                "~/PlennuscGestao/Views/detailDemand.aspx"
            );

            routes.MapPageRoute(
                "ViewDemandBeforeAcceptRoute",
                "viewDemandBeforeAccept",
                "~/PlennuscGestao/Views/viewDemandBeforeAccept.aspx"
            );

            routes.MapPageRoute(
                "DemandRoute",
                "demand",
                "~/PlennuscGestao/Views/demand.aspx"
            );

            routes.MapPageRoute(
                "ListDemandRoute",
                "listDemand",
                "~/PlennuscGestao/Views/listDemand.aspx"
            );

            // ✅ OUTRAS ROTAS DE DEMANDAS
            routes.MapPageRoute(
                "MyDemandsOpenRoute",
                "myDemandsOpen",
                "~/PlennuscGestao/Views/myDemandsOpen.aspx"
            );

            routes.MapPageRoute(
                "MyDemandsProgressRoute",
                "myDemandsProgress",
                "~/PlennuscGestao/Views/myDemandsProgress.aspx"
            );

            routes.MapPageRoute(
                "MyDemandsWaitingRoute",
                "myDemandsWaiting",
                "~/PlennuscGestao/Views/myDemandsWaiting.aspx"
            );

            routes.MapPageRoute(
                "MyDemandsRefusedRoute",
                "myDemandsRefused",
                "~/PlennuscGestao/Views/myDemandsRefused.aspx"
            );

            routes.MapPageRoute(
                "MyDemandsCompletedRoute",
                "myDemandsCompleted",
                "~/PlennuscGestao/Views/myDemandsCompleted.aspx"
            );

            // Topicos de empresa
            routes.MapPageRoute(
                "RegisterCompanyRoute",
                "parametrizacao/empresa/cadastrar",
                "~/PlennuscGestao/Views/companyRegistration.aspx"
            );

            var settings = new FriendlyUrlSettings();
            settings.AutoRedirectMode = RedirectMode.Permanent;
            routes.EnableFriendlyUrls(settings);
        }
    }
}
