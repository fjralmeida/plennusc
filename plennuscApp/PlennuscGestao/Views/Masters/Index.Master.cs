using appWhatsapp.SqlQueries;
using appWhatsapp.ViewsApp;
using Plennusc.Core.Service.ServiceGestao;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.profile;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views.Masters
{
    public partial class Index : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // 1. VERIFICA SE ESTÁ LOGADO
                if (Session["CodUsuario"] == null && Session["CodUsuario"] == null)
                {
                    System.Diagnostics.Debug.WriteLine("REDIRECIONANDO PARA LOGIN - NENHUM CodUsuario NA SESSÃO");
                    Response.Redirect("~/ViewsApp/SignIn.aspx");
                    return;
                }

                // 2. VALIDA ACESSO À PÁGINA ATUAL (DINÂMICO)
                if (!ValidarAcessoPaginaAtual())
                {
                    Response.Redirect("~/ViewsApp/erro.aspx");
                    return;
                }

                // 3. APLICA FILTRO DINÂMICO NOS MENUS
                ApplyMenuFilteringDinamico();

                // 4. CONFIGURA USUÁRIO
                ConfigurarUsuario();

                // 5. VERIFICAÇÕES ESPECÍFICAS (gestor, admin)
                VerificarPermissoesEspecificas();
            }
        }

        private bool ValidarAcessoPaginaAtual()
        {
            try
            {
                if (Session["PermissoesPaginas"] == null)
                {
                    // Se não tem permissões carregadas, permite acesso (pode ser primeira vez)
                    return true;
                }

                var permissoesPaginas = Session["PermissoesPaginas"] as System.Collections.Generic.Dictionary<string, bool>;
                string paginaAtual = ObterNomePaginaAtual();

                // Se não encontrou a página nas permissões, nega acesso
                if (!permissoesPaginas.ContainsKey(paginaAtual))
                    return false;

                return permissoesPaginas[paginaAtual];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ValidarAcessoPaginaAtual: {ex.Message}");
                return false;
            }
        }

        private string ObterNomePaginaAtual()
        {
            string url = Request.Url.AbsolutePath;
            string[] partes = url.Split('/');
            string pagina = partes[partes.Length - 1];

            // Remove .aspx se existir
            if (pagina.EndsWith(".aspx"))
                pagina = pagina.Substring(0, pagina.Length - 5);

            return pagina;
        }

        private void ApplyMenuFilteringDinamico()
        {
            try
            {
                if (Session["EstruturaMenus"] == null)
                    return;

                DataTable dtMenus = Session["EstruturaMenus"] as DataTable;

                // PERCORRE TODOS OS CONTROLES DA PÁGINA E APLICA FILTRO DINÂMICO
                ApplyFilterRecursive(Page, dtMenus);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ApplyMenuFilteringDinamico: {ex.Message}");
            }
        }

        private void ApplyFilterRecursive(Control parent, DataTable dtMenus)
        {
            foreach (Control control in parent.Controls)
            {
                // SE É UM ITEM DE MENU (LI)
                if (control is HtmlGenericControl li && li.TagName == "li" && !string.IsNullOrEmpty(li.ID))
                {
                    string controlId = li.ID;

                    // PROCURA O MENU NA ESTRUTURA PELO ID DO CONTROLE
                    DataRow[] menuRows = dtMenus.Select($"NomeObjeto = '{controlId}' OR NomeDisplay LIKE '%{controlId}%'");

                    if (menuRows.Length > 0)
                    {
                        bool temAcesso = Convert.ToBoolean(menuRows[0]["TemAcesso"]);
                        li.Visible = temAcesso;
                    }
                    else
                    {
                        // SE NÃO ENCONTROU NA ESTRUTURA, TENTA PELO CONTEÚDO DO LINK
                        ApplyFilterByLinkContent(li, dtMenus);
                    }
                }

                // CONTINUA RECURSIVAMENTE
                if (control.HasControls())
                {
                    ApplyFilterRecursive(control, dtMenus);
                }
            }
        }

        private void ApplyFilterByLinkContent(HtmlGenericControl li, DataTable dtMenus)
        {
            // PROCURA LINKS DENTRO DO LI
            foreach (Control child in li.Controls)
            {
                if (child is HtmlAnchor anchor && !string.IsNullOrEmpty(anchor.HRef))
                {
                    string href = anchor.HRef.ToLower();

                    foreach (DataRow menuRow in dtMenus.Rows)
                    {
                        string nomeObjeto = menuRow["NomeObjeto"]?.ToString().ToLower();
                        string httpRouter = menuRow["HttpRouter"]?.ToString().ToLower();

                        bool match = (!string.IsNullOrEmpty(nomeObjeto) && href.Contains(nomeObjeto)) ||
                                    (!string.IsNullOrEmpty(httpRouter) && href.Contains(httpRouter));

                        if (match)
                        {
                            bool temAcesso = Convert.ToBoolean(menuRow["TemAcesso"]);
                            li.Visible = temAcesso;
                            return;
                        }
                    }
                }

                // PROCURA RECURSIVAMENTE EM SUBMENUS
                if (child is HtmlGenericControl childLi && childLi.HasControls())
                {
                    ApplyFilterByLinkContent(childLi, dtMenus);
                }
            }
        }

        private void ConfigurarUsuario()
        {
            lblUsuario.Text = Session["NomeUsuario"]?.ToString() ?? "Usuário";
            lblNomeSistema.Text = Session["NomeEmpresa"]?.ToString() ?? "Empresa";

            if (Session["CodSistema"] != null)
            {
                int codSistema = Convert.ToInt32(Session["CodSistema"]);
                CarregarInfoEmpresa(codSistema);
            }

            CarregarAvatarUsuario();
        }

        private void CarregarAvatarUsuario()
        {
            if (Session["CodUsuario"] != null)
            {
                int codUsuario = Convert.ToInt32(Session["CodUsuario"]);
                PessoaDAO pessoaDao = new PessoaDAO();
                DataRow pessoa = pessoaDao.ObterPessoaPorUsuario(codUsuario);

                string defaultAvatar = ResolveUrl("~/public/uploadgestao/images/imgDefultAvatar.jpg");
                string fotoUrl = defaultAvatar;

                if (pessoa != null)
                {
                    var foto = (pessoa["ImagemFoto"] ?? "").ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(foto))
                    {
                        fotoUrl = ResolveUrl("~/public/uploadgestao/images/" + foto);
                    }
                }

                imgAvatarUsuario.ImageUrl = fotoUrl;
                imgAvatarUsuario.AlternateText = "Avatar do Usuário";
                imgAvatarUsuario.Attributes["onerror"] = $"this.onerror=null;this.src='{defaultAvatar}';";

                imgAvatarUsuarioDropdown.ImageUrl = fotoUrl;
                imgAvatarUsuarioDropdown.AlternateText = "Avatar do Usuário";
                imgAvatarUsuarioDropdown.Attributes["onerror"] = $"this.onerror=null;this.src='{defaultAvatar}';";
            }
        }

        // MANTENHA SEUS MÉTODOS EXISTENTES AQUI (sem alterações)
        private void VerificarPermissoesEspecificas()
        {
            if (Session["CodPessoa"] != null && Session["CodDepartamento"] != null)
            {
                int codPessoa = Convert.ToInt32(Session["CodPessoa"]);
                int codSetor = Convert.ToInt32(Session["CodDepartamento"]);

                var demandaService = new DemandaService("Plennus");
                bool eGestor = demandaService.VerificarSeEGestor(codPessoa, codSetor);
                bool eAdministrador = demandaService.VerificarSeEAdministrador(codPessoa);

                Session["EGestor"] = eGestor;
                Session["EAdministrador"] = eAdministrador;

                ConfigurarMenuGestor(eGestor);
                ConfigurarMenuAdministrador(eAdministrador);
            }
        }

        private void ConfigurarMenuAdministrador(bool eAdministrador)
        {
            var menuEstruturas = FindControlRecursive(this, "liMenuEstruturas");
            if (menuEstruturas != null) menuEstruturas.Visible = eAdministrador;
        }

        private void ConfigurarMenuGestor(bool eGestor)
        {
            var linkAguardando = FindControl("linkAguardando") as HtmlGenericControl;
            if (linkAguardando != null) linkAguardando.Visible = eGestor;
        }

        private Control FindControlRecursive(Control root, string id)
        {
            if (root == null) return null;
            if (root.ID == id) return root;
            foreach (Control c in root.Controls)
            {
                Control t = FindControlRecursive(c, id);
                if (t != null) return t;
            }
            return null;
        }

        private void CarregarInfoEmpresa(int codSistema)
        {
            ItensPedIntegradoUtil util = new ItensPedIntegradoUtil();
            DataTable dtEmpresa = util.ConsultaInfoEmpresa(codSistema);

            if (dtEmpresa.Rows.Count > 0)
            {
                lblNomeSistema.Text = dtEmpresa.Rows[0]["NomeDisplay"].ToString();
            }
        }

        protected void LogoutUsuario(object sender, EventArgs e)
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/ViewsApp/SignIn.aspx", true);
        }
    }
}