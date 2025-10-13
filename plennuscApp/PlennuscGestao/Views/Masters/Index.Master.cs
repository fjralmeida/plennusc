using appWhatsapp.SqlQueries;
using Plennusc.Core.Service.ServiceGestao;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.profile;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace PlennuscGestao.Views.Masters
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
                VerificarPermissoesMenu();
                VerificarPermissaoEstruturas();

                int codUsuario = Convert.ToInt32(Session["codUsuario"]);
                PessoaDAO pessoaDao = new PessoaDAO();
                DataRow pessoa = pessoaDao.ObterPessoaPorUsuario(codUsuario);

                if (pessoa != null)
                {
                    var foto = (pessoa["ImagemFoto"] ?? "").ToString().Trim();

                    var defaultAvatar = ResolveUrl("~/public/uploadgestao/images/imgDefultAvatar.jpg"); // <-- seu ícone padrão
                    var fotoUrl = string.IsNullOrWhiteSpace(foto)
                        ? defaultAvatar
                        : ResolveUrl("~/public/uploadgestao/images/" + foto);

                    imgAvatarUsuario.ImageUrl = fotoUrl;
                    imgAvatarUsuario.AlternateText = "Avatar do Usuário";
                    imgAvatarUsuario.Attributes["onerror"] = $"this.onerror=null;this.src='{defaultAvatar}';";

                    imgAvatarUsuarioDropdown.ImageUrl = fotoUrl;
                    imgAvatarUsuarioDropdown.AlternateText = "Avatar do Usuário";
                    imgAvatarUsuarioDropdown.Attributes["onerror"] = $"this.onerror=null;this.src='{defaultAvatar}';";
                }
                else
                {
                    var defaultAvatar = ResolveUrl("~/public/uploadgestao/images/imgDefultAvatar.jpg");
                    imgAvatarUsuario.ImageUrl = defaultAvatar;
                    imgAvatarUsuario.Attributes["onerror"] = $"this.onerror=null;this.src='{defaultAvatar}';";

                    imgAvatarUsuarioDropdown.ImageUrl = defaultAvatar;
                    imgAvatarUsuarioDropdown.Attributes["onerror"] = $"this.onerror=null;this.src='{defaultAvatar}';";
                }
            }
        }

        private void VerificarPermissoesMenu()
        {
            if (Session["CodPessoa"] != null && Session["CodDepartamento"] != null)
            {
                int codPessoa = Convert.ToInt32(Session["CodPessoa"]);
                int codSetor = Convert.ToInt32(Session["CodDepartamento"]);

                var demandaService = new DemandaService("Plennus");

                // Verifica ambas as permissões usando o service
                bool eGestor = demandaService.VerificarSeEGestor(codPessoa, codSetor);
                bool eAdministrador = demandaService.VerificarSeEAdministrador(codPessoa);

                // Armazena na sessão
                Session["EGestor"] = eGestor;
                Session["EAdministrador"] = eAdministrador;

                // Configura os menus
                ConfigurarMenuGestor(eGestor);
                ConfigurarMenuAdministrador(eAdministrador);
            }
        }

        private void ConfigurarMenuAdministrador(bool eAdministrador)
        {
            // Encontra o item do menu "Estruturas"
            Control menuEstruturas = FindControlRecursive(Page.Master, "liMenuEstruturas");

            if (menuEstruturas != null)
            {
                menuEstruturas.Visible = eAdministrador;
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
                            if (link != null && link.HRef.Contains("myDemandsWaiting.aspx"))
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
            // Encontra o controle específico do menu "Estruturas"
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

            string baseUrl;

            if (Request.Url.Host.Contains("localhost"))
            {
                // Ambiente local — endereço do PlennuscApp local
                baseUrl = "https://localhost:44332";
            }
            else
            {
                // Ambiente de produção — endereço do PlennuscApp no servidor
                baseUrl = "http://plennuschomo.vallorbeneficios.com.br";
            }

            string redirectUrl = $"{baseUrl}/ViewsApp/SignIn";
            Response.Redirect(redirectUrl, true);
        }
    }
}