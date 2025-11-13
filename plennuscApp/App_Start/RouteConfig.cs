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
            // ✅ Estruturas
            routes.MapPageRoute("registerStructureType", "registerStructureType", "~/PlennuscGestao/Views/registerStructureType.aspx");
            routes.MapPageRoute("listStructureTypes", "listStructureTypes", "~/PlennuscGestao/Views/listStructureTypes.aspx");
            routes.MapPageRoute("registerStructures", "registerStructures", "~/PlennuscGestao/Views/registerStructures.aspx");
            routes.MapPageRoute("linkSector", "linkSector", "~/PlennuscGestao/Views/linkSector.aspx");

            // ✅ Usuários e Empresas
            routes.MapPageRoute("userCompanyManagement", "userCompanyManagement", "~/PlennuscGestao/Views/userCompanyManagement.aspx");
            routes.MapPageRoute( "vincularEmpresasUsuario", "userCompanyRegistration", "~/PlennuscGestao/Views/userCompanyRegistration.aspx");

            // ✅ Demandas
            routes.MapPageRoute("detailDemand", "detailDemand", "~/PlennuscGestao/Views/detailDemand.aspx");
            routes.MapPageRoute("viewDemandBeforeAccept", "viewDemandBeforeAccept", "~/PlennuscGestao/Views/viewDemandBeforeAccept.aspx");
            routes.MapPageRoute("demand", "demand", "~/PlennuscGestao/Views/demand.aspx");
            routes.MapPageRoute("listDemand", "listDemand", "~/PlennuscGestao/Views/listDemand.aspx");
            routes.MapPageRoute("myDemandsOpen", "myDemandsOpen", "~/PlennuscGestao/Views/myDemandsOpen.aspx");
            routes.MapPageRoute("myDemandsProgress", "myDemandsProgress", "~/PlennuscGestao/Views/myDemandsProgress.aspx");
            routes.MapPageRoute("myDemandsWaiting", "myDemandsWaiting", "~/PlennuscGestao/Views/myDemandsWaiting.aspx");
            routes.MapPageRoute("myDemandsRefused", "myDemandsRefused", "~/PlennuscGestao/Views/myDemandsRefused.aspx");
            routes.MapPageRoute("myDemandsCompleted", "myDemandsCompleted", "~/PlennuscGestao/Views/myDemandsCompleted.aspx");

            // ✅ Empresa e Parametrização
            routes.MapPageRoute("companyRegistration", "companyRegistration", "~/PlennuscGestao/Views/companyRegistration.aspx");

            // ✅ Chatbot e Mensagens
            routes.MapPageRoute("sendMessageBeneficiary", "sendMessageBeneficiary", "~/PlennuscGestao/Views/sendMessageBeneficiary.aspx");
            routes.MapPageRoute("sendCompanyMessage", "sendCompanyMessage", "~/PlennuscGestao/Views/sendCompanyMessage.aspx");
            routes.MapPageRoute("FixedMessageSending", "FixedMessageSending", "~/PlennuscGestao/Views/FixedMessageSending.aspx");

            // ✅ Tabela de preços
            routes.MapPageRoute("priceTable", "priceTable", "~/PlennuscGestao/Views/priceTable.aspx");
            routes.MapPageRoute("updatePriceTable", "updatePriceTable", "~/PlennuscGestao/Views/updatePriceTable.aspx");

            // ✅ Outros possíveis cadastros ou configurações
            routes.MapPageRoute("employeeDepartment", "employeeDepartment", "~/PlennuscGestao/Views/employeeDepartment.aspx");
            routes.MapPageRoute("employeePosition", "employeePosition", "~/PlennuscGestao/Views/employeePosition.aspx");
            routes.MapPageRoute("employeeManagement", "employeeManagement", "~/PlennuscGestao/Views/employeeManagement.aspx");
            routes.MapPageRoute("listPlatform", "listPlatform", "~/PlennuscGestao/Views/listPlatform.aspx");

            // ⚙️ Friendly URLs sem redirecionamento
            var settings = new FriendlyUrlSettings();
            settings.AutoRedirectMode = RedirectMode.Off;
            routes.EnableFriendlyUrls(settings);
        }
    }
}