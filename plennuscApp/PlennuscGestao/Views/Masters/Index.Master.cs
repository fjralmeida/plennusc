using appWhatsapp.SqlQueries;
using appWhatsapp.ViewsApp;
using Plennusc.Core.Service.ServiceGestao;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.profile;
using System;
using System.Collections.Generic;
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
            // 1. VERIFICA SE ESTÁ LOGADO (SEM ISPOSTBACK)
            if (Session["CodUsuario"] == null)
            {
                Response.Redirect("~/ViewsApp/SignIn.aspx");
                return;
            }

            // 2. VALIDA ACESSO À PÁGINA ATUAL (SEM ISPOSTBACK)
                if (!ValidarAcessoPaginaAtual())
            {
                Response.Redirect("~/ViewsApp/erro.aspx");
                return;
            }

            // 3. ✅ GERA MENU DINÂMICO DO BANCO (SEM ISPOSTBACK)
            GerarMenuDinamico();

            // 5. CONFIGURA USUÁRIO (SÓ NA PRIMEIRA VEZ)
            if (!IsPostBack)
            {
                ConfigurarUsuario();
                VerificarPermissoesEspecificas();
            }
        }

        // ✅ MÉTODO PRINCIPAL - GERA MENU COMPLETO DO BANCO
        private void GerarMenuDinamico()
        {
            try
            {
                if (Session["EstruturaMenus"] == null)
                    return;

                DataTable dtMenus = Session["EstruturaMenus"] as DataTable;
                phMenuDinamico.Controls.Clear();

                // ✅ CRIA CONTAINER PRINCIPAL
                var ulPrincipal = new HtmlGenericControl("ul");
                ulPrincipal.Attributes["class"] = "list-unstyled";

                // ✅ FILTRA MENUS DE NÍVEL 1 (EXCLUINDO HOME, PROFILE E PRIVACY)
                var menusNivel1 = dtMenus.Select("(CodMenuPai IS NULL OR CodMenuPai = 0) AND TemAcesso = 1 AND NomeObjeto NOT IN ('homeManagement', 'profile', 'privacySettings')", "Conf_Ordem ASC");

                foreach (DataRow menu in menusNivel1)
                {
                    // ✅ CRIA CADA MENU PRINCIPAL
                    var liMenu = CriarItemMenu(menu, dtMenus);
                    ulPrincipal.Controls.Add(liMenu);
                }

                phMenuDinamico.Controls.Add(ulPrincipal);
            }
            catch (Exception ex)
            {
                // Log silencioso
            }
        }

        // ✅ CRIA UM ITEM DE MENU (RECURSIVO PARA SUBMENUS) - CORRIGIDO
        private HtmlGenericControl CriarItemMenu(DataRow menu, DataTable todosMenus)
        {
            int codMenu = Convert.ToInt32(menu["CodMenu"]);
            string nomeDisplay = menu["NomeDisplay"].ToString();
            string nomeObjeto = menu["NomeObjeto"].ToString();
            string httpRouter = menu["HttpRouter"]?.ToString();
            bool temAcesso = Convert.ToBoolean(menu["TemAcesso"]);

            // ✅ SÓ CRIA SE TEM ACESSO
            if (!temAcesso)
                return new HtmlGenericControl("div"); // Retorna div vazia

            // ✅ EXCLUI PÁGINAS QUE NÃO DEVEM APARECER NA BARRA LATERAL
            var paginasOcultas = new List<string>
            {
                "employeeedit",
                "viewdemandbeforeaccept",
                "detaildemand",
                "usercompanyregistration"
            };

            if (paginasOcultas.Contains(nomeObjeto.ToLower()))
                return new HtmlGenericControl("div"); // Retorna div vazia

            // ✅ CRIA O LI
            var li = new HtmlGenericControl("li");
            li.Attributes["class"] = "mb-2";
            li.ID = $"liMenu_{codMenu}";

            // ✅ VERIFICA SE TEM SUBMENUS
            var subMenus = todosMenus.Select($"CodMenuPai = {codMenu} AND TemAcesso = 1", "Conf_Ordem ASC");
            bool temSubMenus = subMenus.Length > 0;

            if (temSubMenus)
            {
                // ✅ MENU COM SUBMENUS (COLLAPSE)
                li.Controls.Add(CriarMenuComSubmenus(menu, subMenus, todosMenus));
            }
            else
            {
                // ✅ MENU SIMPLES (LINK DIRETO)
                li.Controls.Add(CriarMenuSimples(menu));
            }

            return li;
        }

        // ✅ CRIA MENU COM SUBMENUS (COLLAPSE BOOTSTRAP) 
        private HtmlGenericControl CriarMenuComSubmenus(DataRow menu, DataRow[] subMenus, DataTable todosMenus)
        {
            int codMenu = Convert.ToInt32(menu["CodMenu"]);
            string nomeDisplay = menu["NomeDisplay"].ToString();
            string icone = ObterIcone(menu);

            var container = new HtmlGenericControl("div");

            // ✅ LINK TOGGLE (MENU PRINCIPAL)
            var linkToggle = new HtmlGenericControl("a");
            linkToggle.Attributes["class"] = "d-flex justify-content-between align-items-center menu-head";
            linkToggle.Attributes["data-bs-toggle"] = "collapse";
            linkToggle.Attributes["href"] = $"#subMenu_{codMenu}";
            linkToggle.Attributes["role"] = "button";
            linkToggle.Attributes["aria-expanded"] = "false";
            linkToggle.Attributes["aria-controls"] = $"subMenu_{codMenu}";
            linkToggle.Attributes["title"] = menu["CaptionObjeto"]?.ToString() ?? nomeDisplay;

            // ✅ ÍCONE E TEXTO
            var span = new HtmlGenericControl("span");
            var icon = new HtmlGenericControl("i");
            icon.Attributes["class"] = icone;
            span.Controls.Add(icon);
            span.Controls.Add(new LiteralControl { Text = $"<strong class='label'>{nomeDisplay}</strong>" });

            // ✅ ÍCONE TOGGLE
            var toggleIcon = new HtmlGenericControl("i");
            toggleIcon.Attributes["class"] = "bi bi-chevron-right toggle-icon";

            linkToggle.Controls.Add(span);
            linkToggle.Controls.Add(toggleIcon);

            // ✅ CONTAINER SUBMENUS (COLLAPSE)
            var collapseDiv = new HtmlGenericControl("div");
            collapseDiv.ID = $"subMenu_{codMenu}";
            collapseDiv.Attributes["class"] = "collapse ms-3";

            var ulSubmenus = new HtmlGenericControl("ul");
            ulSubmenus.Attributes["class"] = "list-unstyled ps-3 submenu";

            // ✅ ADICIONA SUBMENUS (FILTRANDO OS QUE NÃO DEVEM APARECER)
            var paginasOcultas = new List<string>
            {
                "employeeedit",
                "viewdemandbeforeaccept",
                "detaildemand",
                "usercompanyregistration"
            };

            foreach (DataRow subMenu in subMenus)
            {
                string nomeObjetoSub = subMenu["NomeObjeto"].ToString().ToLower();

                // ✅ SÓ ADICIONA SE NÃO ESTÁ NA LISTA DE OCULTAS
                if (!paginasOcultas.Contains(nomeObjetoSub))
                {
                    var liSubMenu = CriarItemMenu(subMenu, todosMenus);
                    ulSubmenus.Controls.Add(liSubMenu);
                }
            }

            collapseDiv.Controls.Add(ulSubmenus);
            container.Controls.Add(linkToggle);
            container.Controls.Add(collapseDiv);

            return container;
        }


        // ✅ CRIA MENU SIMPLES (LINK DIRETO)
        private HtmlGenericControl CriarMenuSimples(DataRow menu)
        {
            string nomeDisplay = menu["NomeDisplay"].ToString();
            string nomeObjeto = menu["NomeObjeto"].ToString();
            string httpRouter = menu["HttpRouter"]?.ToString();
            string icone = ObterIcone(menu);

            var link = new HtmlGenericControl("a");

            // ✅ PADRÃO CORRETO: Usa ResolveUrl para garantir o caminho absoluto
            if (!string.IsNullOrEmpty(httpRouter))
            {
                link.Attributes["href"] = httpRouter;
            }
            else
            {
                // ✅ CORREÇÃO: Usa ResolveUrl para criar URL absoluta
                link.Attributes["href"] = ResolveUrl($"~/{nomeObjeto}");
            }

            link.Attributes["title"] = menu["CaptionObjeto"]?.ToString() ?? nomeDisplay;
            link.Attributes["class"] = "d-flex align-items-center"; // ✅ ADICIONA FLEX PARA ALINHAR ÍCONE E TEXTO

            // ✅ ÍCONE E TEXTO - CORRIGIDO
            var icon = new HtmlGenericControl("i");
            icon.Attributes["class"] = icone;
            link.Controls.Add(icon);

            var span = new HtmlGenericControl("span");
            span.Attributes["class"] = "label";
            span.InnerText = nomeDisplay;
            link.Controls.Add(span);

            return link;
        }

        // ✅ DEFINE ÍCONE BASEADO NO TIPO DE MENU - CORRIGIDO SEM REPETIÇÕES
        private string ObterIcone(DataRow menu)
        {
            string nomeObjeto = menu["NomeObjeto"].ToString().ToLower();
            string nomeDisplay = menu["NomeDisplay"].ToString().ToLower();
            string captionObjeto = menu["CaptionObjeto"]?.ToString().ToLower() ?? "";

            // ✅ MENU (dentro de parametrização) - PRIMEIRO PARA EVITAR CONFLITOS
            if (nomeObjeto.Contains("menumenu") || (nomeDisplay.Contains("menu") && captionObjeto.Contains("menu")))
                return "bi bi-menu-button me-2";

            if (nomeDisplay.Contains("cadastro empresa") || nomeObjeto.Contains("companyregistration"))
                return "bi bi-building-add me-2";

            if (nomeDisplay.Contains("sistemas x empresa"))
                return "bi bi-link-45deg me-2";

            // ✅ PARAMETRIZAÇÃO - SISTEMAS
            if (nomeObjeto.Contains("menusistemas"))
                return "bi bi-window-stack me-2";

            if (nomeDisplay.Contains("listar sistema"))
                return "bi bi-list-check me-2";

            if (nomeDisplay.Contains("sistema x menu"))
                return "bi bi-link-45deg me-2";

            // ✅ PREÇOS
            if (nomeObjeto.Contains("menuprecos") || nomeDisplay.Contains("preço") || nomeDisplay.Contains("precos"))
                return "bi bi-currency-dollar me-2";

            if (nomeDisplay.Contains("nova tabela"))
                return "bi bi-file-earmark-spreadsheet me-2";

            if (nomeDisplay.Contains("atualizar valor"))
                return "bi bi-arrow-repeat me-2";

            // ✅ DEMANDA
            if (nomeObjeto.Contains("menudemandas"))
                return "bi bi-envelope me-2";

            if (nomeDisplay.Contains("criar demanda"))
                return "bi bi-plus-circle me-2";

            if (nomeDisplay.Contains("listar demanda"))
                return "bi bi-list-ul me-2";

            if (nomeDisplay.Contains("minhas demanda"))
                return "bi bi-inboxes me-2";

            // ✅ MINHAS DEMANDAS - SUBITENS
            if (nomeDisplay.Contains("em aberto"))
                return "bi bi-folder2-open me-2";

            if (nomeDisplay.Contains("em andamento"))
                return "bi bi-hourglass-split me-2";

            if (nomeDisplay.Contains("aguardando"))
                return "bi bi-clock-history me-2";

            if (nomeDisplay.Contains("recusada"))
                return "bi bi-x-circle me-2";

            if (nomeDisplay.Contains("concluída"))
                return "bi bi-check-circle me-2";

            // ✅ PESSOAS
            if (nomeObjeto.Contains("menupessoas") || nomeDisplay.Contains("pessoas"))
                return "bi bi-person-badge me-2";

            if (nomeDisplay.Contains("incluir usuário") || nomeDisplay.Contains("incluir usuario"))
                return "bi bi-person-plus me-2";

            if (nomeDisplay.Contains("consultar usuário") || nomeDisplay.Contains("consultar usuario"))
                return "bi bi-search me-2";

            if (nomeDisplay.Contains("vincular empresa") || nomeObjeto.Contains("usercompanymanagement"))
                return "bi bi-building-check me-2";

            if (nomeDisplay.Contains("departamento") || nomeObjeto.Contains("employeedepartment"))
                return "bi bi-diagram-3 me-2";

            if (nomeDisplay.Contains("cargo") || nomeObjeto.Contains("employeeposition"))
                return "bi bi-briefcase me-2";

            // ✅ PARAMETRIZAÇÃO (menu principal)
            if (nomeObjeto.Contains("menuparametrizacao") || nomeDisplay.Contains("parametrização"))
                return "bi bi-gear me-2";

            // ✅ ESTRUTURAS
            if (nomeObjeto.Contains("menuestruturas") || nomeDisplay.Contains("estruturas"))
                return "bi bi-diagram-3 me-2";

            if (nomeDisplay.Contains("tipos de estrutura") || nomeObjeto.Contains("registerstructuretype"))
                return "bi bi-tags me-2";

            if (nomeDisplay.Contains("lista de estrutura") || nomeObjeto.Contains("liststructuretypes"))
                return "bi bi-list-ul me-2";

            if (nomeDisplay.Contains("cadastrar estrutura") || nomeObjeto.Contains("registerstructures"))
                return "bi bi-building-add me-2";

            if (nomeDisplay.Contains("vincular setor") || nomeObjeto.Contains("linksector"))
                return "bi bi-shuffle me-2";

            // ✅ CHATBOT
            if (nomeObjeto.Contains("menuchatbot") || nomeDisplay.Contains("chatbot"))
                return "bi bi-robot me-2";

            if (nomeDisplay.Contains("beneficiário") || nomeObjeto.Contains("sendmessagebeneficiary"))
                return "bi bi-person-lines-fill me-2";

            if (nomeDisplay.Contains("pme") || nomeObjeto.Contains("sendcompanymessage"))
                return "bi bi-building me-2";

            if (nomeDisplay.Contains("msg fixa") || nomeObjeto.Contains("fixedmessagesending"))
                return "bi bi-pin-angle me-2";

            // ✅ OUTROS
            if (nomeObjeto.Contains("home") || nomeDisplay.Contains("inicio"))
                return "bi bi-house me-2";

            if (nomeObjeto.Contains("profile") || nomeDisplay.Contains("perfil"))
                return "bi bi-person me-2";

            if (nomeObjeto.Contains("privacy") || nomeDisplay.Contains("privacidade"))
                return "bi bi-shield-lock me-2";

            // ✅ ÍCONE PADRÃO
            return "bi bi-circle me-2";
        }

        // ✅ MÉTODOS EXISTENTES (MANTIDOS)
        private bool ValidarAcessoPaginaAtual()
        {
            try
            {
                if (Session["PermissoesPaginas"] == null)
                    return true;

                var permissoesPaginas = Session["PermissoesPaginas"] as System.Collections.Generic.Dictionary<string, bool>;
                string paginaAtual = ObterNomePaginaAtual();

                if (!permissoesPaginas.ContainsKey(paginaAtual))
                    return false;

                return permissoesPaginas[paginaAtual];
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        private string ObterNomePaginaAtual()
        {
            string url = Request.Url.AbsolutePath;
            string[] partes = url.Split('/');
            string pagina = partes[partes.Length - 1];

            if (pagina.EndsWith(".aspx"))
                pagina = pagina.Substring(0, pagina.Length - 5);

            return pagina;
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