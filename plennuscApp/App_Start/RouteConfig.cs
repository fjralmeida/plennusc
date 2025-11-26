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
            // ✅ PADRÃO: Rotas curtas (sem "PlennuscGestao/Views/")

            // ✅ Estruturas
            routes.MapPageRoute("registerStructureType", "registerStructureType", "~/PlennuscGestao/Views/registerStructureType.aspx");
            routes.MapPageRoute("listStructureTypes", "listStructureTypes", "~/PlennuscGestao/Views/listStructureTypes.aspx");
            routes.MapPageRoute("registerStructures", "registerStructures", "~/PlennuscGestao/Views/registerStructures.aspx");
            routes.MapPageRoute("linkSector", "linkSector", "~/PlennuscGestao/Views/linkSector.aspx");

            // ✅ Usuários e Empresas
            routes.MapPageRoute("userCompanyManagement", "userCompanyManagement", "~/PlennuscGestao/Views/userCompanyManagement.aspx");
            routes.MapPageRoute("userCompanyRegistration", "userCompanyRegistration", "~/PlennuscGestao/Views/userCompanyRegistration.aspx");

            // ✅ Vincular Sistemas e Menus
            routes.MapPageRoute("userSystemMenuManagement", "userSystemMenuManagement", "~/PlennuscGestao/Views/userSystemMenuManagement.aspx");
            routes.MapPageRoute("companyMenuManagementSystem", "companyMenuManagementSystem", "~/PlennuscGestao/Views/companyMenuManagementSystem.aspx");

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
            routes.MapPageRoute("listPlatform", "listPlatform", "~/PlennuscGestao/Views/listPlatform.aspx");

            // ✅ Chatbot e Mensagens
            routes.MapPageRoute("sendMessageBeneficiary", "sendMessageBeneficiary", "~/PlennuscGestao/Views/sendMessageBeneficiary.aspx");
            routes.MapPageRoute("sendCompanyMessage", "sendCompanyMessage", "~/PlennuscGestao/Views/sendCompanyMessage.aspx");
            routes.MapPageRoute("FixedMessageSending", "FixedMessageSending", "~/PlennuscGestao/Views/FixedMessageSending.aspx");

            // ✅ Tabela de preços
            routes.MapPageRoute("priceTable", "priceTable", "~/PlennuscGestao/Views/priceTable.aspx");
            routes.MapPageRoute("updatePriceTable", "updatePriceTable", "~/PlennuscGestao/Views/updatePriceTable.aspx");

            // ✅ Gestão de Pessoas
            routes.MapPageRoute("employeeDepartment", "employeeDepartment", "~/PlennuscGestao/Views/employeeDepartment.aspx");
            routes.MapPageRoute("employeePosition", "employeePosition", "~/PlennuscGestao/Views/employeePosition.aspx");
            routes.MapPageRoute("employeeManagement", "employeeManagement", "~/PlennuscGestao/Views/employeeManagement.aspx");
            routes.MapPageRoute("employeeEdit", "employeeEdit", "~/PlennuscGestao/Views/employeeEdit.aspx");

            // ✅ Gestão Principal - ROTAS CURTAS
            routes.MapPageRoute("privacySettings", "privacySettings", "~/PlennuscGestao/Views/privacySettings.aspx");
            routes.MapPageRoute("profile", "profile", "~/PlennuscGestao/Views/profile.aspx");
            routes.MapPageRoute("homeManagement", "homeManagement", "~/PlennuscGestao/Views/homeManagement.aspx");

            // ✅ Adicionar esta rota - Vincular Empresas Usuário
            routes.MapPageRoute("vincularEmpresasUsuario", "vincularEmpresasUsuario", "~/PlennuscGestao/Views/userCompanyRegistration.aspx");

            // ✅ Rota padrão
            routes.MapPageRoute("default", "", "~/PlennuscGestao/Views/homeManagement.aspx");

            // ⚙️ Friendly URLs sem redirecionamento
            var settings = new FriendlyUrlSettings();
            settings.AutoRedirectMode = RedirectMode.Off;
            routes.EnableFriendlyUrls(settings);
        }
    }
}