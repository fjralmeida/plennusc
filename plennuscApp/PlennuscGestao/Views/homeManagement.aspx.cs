using Plennusc.Core.Models.ModelsGestao.ModelsHome;
using Plennusc.Core.Service.ServiceGestao.home;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.department;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.position;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.profile;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace PlennuscGestao.Views
{
    public partial class HomeGestao : System.Web.UI.Page
    {
        public homeManagementModels DashboardData { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AplicarPermissoesNosCards();

                BuscarUsuarios();
                BuscarDepartamentos();
                CarregarCargos();
                CarregarWidgetsDemandas();
            }
        }

        private void AplicarPermissoesNosCards()
        {
            ConfigurarCard(divCardColaboradores, "employeeManagement",
        "window.location.href='/employeeManagement?acao=consultar&frame=1';");

            ConfigurarCard(divCardDepartamentos, "employeeDepartment",
                "window.location.href='/employeeDepartment?frame=1';");

            ConfigurarCard(divCardCargos, "employeePosition",
                "window.location.href='/employeePosition?frame=1';");

            ConfigurarCard(divCardDemanda, "demand",
                "window.location.href='/demand?frame=1';");
        }

        private void ConfigurarCard(HtmlGenericControl div, string nomeObjeto, string onclickUrl)
        {
            bool temAcesso = UsuarioTemAcessoAoMenu(nomeObjeto);

            if (temAcesso)
            {
                div.Attributes["onclick"] = onclickUrl;
            }
            else
            {
                div.Attributes["class"] += " dashboard-card-disabled";
                div.Attributes["title"] = "Você não tem permissão para acessar esta área";
            }
        }

        private bool UsuarioTemAcessoAoMenu(string nomeObjeto)
        {
            var dtMenus = Session["EstruturaMenus"] as DataTable;
            if (dtMenus == null) return false;

            var linhas = dtMenus.Select($"NomeObjeto = '{nomeObjeto}' AND TemAcesso = 1");
            return linhas.Length > 0;
        }

        private void CarregarWidgetsDemandas()
        {
            try
            {
                // 🔥 PEGA O CÓDIGO DA PESSOA LOGADA (ajuste conforme sua session)
                int codPessoaLogada = ObterCodPessoaLogada();

                string connectionString = WebConfigurationManager.ConnectionStrings["Plennus"].ConnectionString;
                var service = new HomeManagementService(connectionString);
                DashboardData = service.CarregarWidgetsDemandas(codPessoaLogada);
            }
            catch (Exception ex)
            {
                DashboardData = new homeManagementModels();
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar widgets: {ex.Message}");
            }
        }

        private int ObterCodPessoaLogada()
        {
            // Por enquanto, vou assumir que você tem isso na session:
            if (Session["CodPessoa"] != null)
                return Convert.ToInt32(Session["CodPessoa"]);

            // Fallback - substitua pelo seu método real
            return 3; // Leonardo (exemplo)
        }

        private void BuscarUsuarios()
        {
            PessoaDAO profile = new PessoaDAO();
            DataTable dt = profile.GetTotalUsuarios();
            if (dt != null && dt.Rows.Count > 0)
                lblTotalColaboradores.Text = dt.Rows[0]["Total"].ToString();
        }

        private void BuscarDepartamentos()
        {
            EmpDepartment profile = new EmpDepartment();
            DataTable dt = profile.GetTotalDepartamentos();
            if (dt != null && dt.Rows.Count > 0)
                lblTotalDepartamentos.Text = dt.Rows[0]["Total"].ToString();
        }

        private void CarregarCargos()
        {
            EmpPosition profile = new EmpPosition();
            DataTable dt = profile.GetTotalCargos();
            if (dt != null && dt.Rows.Count > 0)
                lblTotalCargos.Text = dt.Rows[0]["Total"].ToString();
        }
    }
}