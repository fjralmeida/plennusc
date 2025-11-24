using appWhatsapp.SqlQueries;
using appWhatsapp.ViewsApp;
using Plennusc.Core.Service.ServiceGestao;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.profile;
using System;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscMedic.Views.Masters 
{
    public partial class Index : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblUsuario.Text = Session["NomeUsuario"]?.ToString() ?? "Usuário";
                lblNomeSistema.Text = Session["NomeEmpresa"]?.ToString() ?? "Empresa";

                if (Session["CodSistema"] != null)
                {
                    int codSistema = Convert.ToInt32(Session["CodSistema"]);
                    CarregarInfoEmpresa(codSistema);
                }
                if (Session["CodPessoa"] == null)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "erroSessao",
                        "Swal.fire('Sessão Expirada', 'Por favor, faça login novamente.', 'warning').then(() => { window.location = 'login.aspx'; });",
                        true);
                }

                // VERIFICA SE É GESTOR E CONFIGURA O MENU
                VerificarPermissaoMenu();
                VerificarPermissaoEstruturas();

                int codUsuario = Convert.ToInt32(Session["CodUsuario"]); // CORRIGIDO: "CodUsuario" em vez de "codUsuario"
                PessoaDAO pessoaDao = new PessoaDAO();
                DataRow pessoa = pessoaDao.ObterPessoaPorUsuario(codUsuario);

                if (pessoa != null)
                {
                    var foto = (pessoa["ImagemFoto"] ?? "").ToString().Trim();

                    var defaultAvatar = ResolveUrl("~/public/uploadmedic/images/imgDefultAvatar.jpg");
                    var fotoUrl = string.IsNullOrWhiteSpace(foto)
                        ? defaultAvatar
                        : ResolveUrl("~/public/uploadmedic/images/" + foto);

                    imgAvatarUsuario.ImageUrl = fotoUrl;
                    imgAvatarUsuario.AlternateText = "Avatar do Usuário";
                    imgAvatarUsuario.Attributes["onerror"] = $"this.onerror=null;this.src='{defaultAvatar}';";

                    imgAvatarUsuarioDropdown.ImageUrl = fotoUrl;
                    imgAvatarUsuarioDropdown.AlternateText = "Avatar do Usuário";
                    imgAvatarUsuarioDropdown.Attributes["onerror"] = $"this.onerror=null;this.src='{defaultAvatar}';";
                }
                else
                {
                    var defaultAvatar = ResolveUrl("~/public/uploadmedic/images/imgDefultAvatar.jpg");
                    imgAvatarUsuario.ImageUrl = defaultAvatar;
                    imgAvatarUsuario.Attributes["onerror"] = $"this.onerror=null;this.src='{defaultAvatar}';";

                    imgAvatarUsuarioDropdown.ImageUrl = defaultAvatar;
                    imgAvatarUsuarioDropdown.Attributes["onerror"] = $"this.onerror=null;this.src='{defaultAvatar}';";
                }
            }
        }

        private void VerificarPermissaoMenu()
        {
            if (Session["CodPessoa"] != null && Session["CodDepartamento"] != null)
            {
                int codPessoa = Convert.ToInt32(Session["CodPessoa"]);
                int codSetor = Convert.ToInt32(Session["CodDepartamento"]);

                var demandaService = new DemandaService("Plennus");
                bool eGestor = demandaService.VerificarSeEGestor(codPessoa, codSetor);

                Session["EGestor"] = eGestor;
                ConfigurarMenuGestor(eGestor);
            }
        }

        private void ConfigurarMenuGestor(bool eGestor)
        {
            Control menuItem = FindControlRecursive(Page.Master, "menuAguardandoAprovacao");

            if (menuItem != null)
            {
                menuItem.Visible = eGestor;
            }
            else
            {
                Control menuContainer = FindControlRecursive(Page.Master, "subMenuMinhasDemandas");
                if (menuContainer != null)
                {
                    foreach (Control control in menuContainer.Controls)
                    {
                        if (control is HtmlGenericControl li)
                        {
                            var link = li.FindControl("linkAguardando") as HtmlAnchor;
                            if (link != null && link.HRef.Contains("myDemandsWaitingMedic.aspx"))
                            {
                                li.Visible = eGestor;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private Control FindControlRecursive(Control root, string id)
        {
            if (root == null) return null;

            if (root.ID == id)
                return root;
            foreach (Control c in root.Controls)
            {
                Control t = FindControlRecursive(c, id);
                if (t != null)
                    return t;
            }
            return null;
        }

        private void CarregarInfoEmpresa(int codSistema)
        {
            ItensPedIntegradoUtil util = new ItensPedIntegradoUtil();
            DataTable dtEmpresa = util.ConsultaInfoEmpresa(codSistema);

            if (dtEmpresa.Rows.Count > 0)
            {
                //imgLogo.ImageUrl = ResolveUrl("~/Uploads/" + dtEmpresa.Rows[0]["Conf_Logo"].ToString());
                //lblNomeSistema.Text = dtEmpresa.Rows[0]["NomeDisplay"].ToString();
            }
        }

        private void VerificarPermissaoEstruturas()
        {
            if (Session["CodPessoa"] != null)
            {
                int codPessoa = Convert.ToInt32(Session["CodPessoa"]);

                var demandaService = new DemandaService("Plennus");
                bool eAdministrador = demandaService.VerificarSeEAdministrador(codPessoa);

                Session["EAdministrador"] = eAdministrador;
                ConfigurarMenuEstruturas(eAdministrador);
            }
        }

        private void ConfigurarMenuEstruturas(bool eAdministrador)
        {
            Control menuItem = FindControlRecursive(this, "liMenuEstruturas");

            if (menuItem != null)
            {
                menuItem.Visible = eAdministrador;
            }
        }

        protected void LogoutUsuario(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            string baseUrl = Request.Url.Host.Contains("localhost")
                ? "https://localhost:44332"
                : "http://plennuschomo.vallorbeneficios.com.br";
            Response.Redirect($"{baseUrl}/ViewsApp/SignIn", true);
        }
    }
}