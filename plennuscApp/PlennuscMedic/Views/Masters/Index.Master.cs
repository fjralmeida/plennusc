using appWhatsapp.SqlQueries;
using appWhatsapp.ViewsApp;
using Plennusc.Core.Service.ServiceGestao;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.profile;
using System;
using System.Collections.Generic;
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

            // 4. CONFIGURA USUÁRIO (SÓ NA PRIMEIRA VEZ)
            if (!IsPostBack)
            {
                ConfigurarUsuario();
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

                // ✅ FILTRA MENUS DE NÍVEL 1 (EXCLUINDO PÁGINAS QUE NÃO DEVEM APARECER)
                var menusNivel1 = dtMenus.Select("(CodMenuPai IS NULL OR CodMenuPai = 0) AND TemAcesso = 1", "Conf_Ordem ASC");

                foreach (DataRow menu in menusNivel1)
                {
                    // ✅ CRIA CADA MENU PRINCIPAL (EXCLUINDO PÁGINAS OCULTAS)
                    var liMenu = CriarItemMenu(menu, dtMenus);
                    if (liMenu.HasControls() || liMenu.InnerText != "") // Verifica se o menu não está vazio
                    {
                        ulPrincipal.Controls.Add(liMenu);
                    }
                }

                phMenuDinamico.Controls.Add(ulPrincipal);
            }
            catch (Exception ex)
            {
                // Log silencioso
                // phMenuDinamico.Controls.Add(new LiteralControl($"<li class='text-danger'>Erro: {ex.Message}</li>"));
            }
        }

        // ✅ CRIA UM ITEM DE MENU (RECURSIVO PARA SUBMENUS)
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

            // ✅ LISTA DE PÁGINAS QUE NÃO DEVEM APARECER NA BARRA LATERAL
            var paginasOcultas = new List<string>
            {
                // ✅ PÁGINAS PRINCIPAIS QUE SÃO ACESSADAS POR LINKS
                "homedoctor",           // Dashboard Medic (acessado por logo)
                "homemanagement",       // Home Management (do sistema Gestão)
                "profilemedic",         // Perfil (acessado pelo dropdown)
                "privacysettingsmedic", // Configurações (acessado pelo dropdown)
                
                // ✅ PÁGINAS DE DEMANDA QUE SÃO ACESSADAS POR LINKS INTERNOS
                "viewdemandbeforeacceptmedic", // Visualizar Demanda (chamada de listDemand)
                "detaildemandmedic",           // Detalhes Demanda (chamada de myDemands)
                
                // ✅ PÁGINAS DE GESTÃO QUE SÃO ACESSADAS POR LINKS INTERNOS
                "employeeedit",                // Editar Usuário (chamada de employeeManagement)
                "demand",                      // Criar Demanda (não existe no Medic, mas mantém padrão)
                "detaildemand"                 // Detalhes Demanda (não existe no Medic, mas mantém padrão)
            };

            // ✅ EXCLUI PÁGINAS QUE NÃO DEVEM APARECER
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
            string icone = ObterIconeMedic(menu);

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

            // ✅ LISTA DE PÁGINAS QUE NÃO DEVEM APARECER COMO SUBMENUS
            var paginasOcultas = new List<string>
            {
                // ✅ PÁGINAS QUE SÃO ACESSADAS APENAS POR LINKS INTERNOS
                "viewdemandbeforeacceptmedic",
                "detaildemandmedic",
                "employeeedit",
                "homedoctor",
                "homemanagement",
                "profilemedic",
                "privacysettingsmedic"
            };

            foreach (DataRow subMenu in subMenus)
            {
                string nomeObjetoSub = subMenu["NomeObjeto"].ToString().ToLower();

                // ✅ SÓ ADICIONA SE NÃO ESTÁ NA LISTA DE OCULTAS
                if (!paginasOcultas.Contains(nomeObjetoSub))
                {
                    var liSubMenu = CriarItemMenu(subMenu, todosMenus);
                    // Verifica se o submenu não está vazio
                    if (liSubMenu.HasControls() || liSubMenu.InnerText != "")
                    {
                        ulSubmenus.Controls.Add(liSubMenu);
                    }
                }
            }

            // ✅ SE NÃO HOUVER SUBMENUS VÁLIDOS, NÃO ADICIONA O CONTAINER DE SUBMENUS
            if (ulSubmenus.Controls.Count > 0)
            {
                collapseDiv.Controls.Add(ulSubmenus);
                container.Controls.Add(linkToggle);
                container.Controls.Add(collapseDiv);
            }
            else
            {
                // ✅ SE NÃO TEM SUBMENUS VÁLIDOS, CONVERTE EM MENU SIMPLES
                container.Controls.Add(CriarMenuSimples(menu));
            }

            return container;
        }

        // ✅ CRIA MENU SIMPLES (LINK DIRETO)
        private HtmlGenericControl CriarMenuSimples(DataRow menu)
        {
            string nomeDisplay = menu["NomeDisplay"].ToString();
            string nomeObjeto = menu["NomeObjeto"].ToString();
            string httpRouter = menu["HttpRouter"]?.ToString();
            string icone = ObterIconeMedic(menu);

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
            link.Attributes["class"] = "d-flex align-items-center";

            // ✅ ÍCONE E TEXTO
            var icon = new HtmlGenericControl("i");
            icon.Attributes["class"] = icone;
            link.Controls.Add(icon);

            var span = new HtmlGenericControl("span");
            span.Attributes["class"] = "label";
            span.InnerText = nomeDisplay;
            link.Controls.Add(span);

            return link;
        }

        // ✅ DEFINE ÍCONE BASEADO NO TIPO DE MENU - ESPECÍFICO PARA MEDIC
        private string ObterIconeMedic(DataRow menu)
        {
            string nomeObjeto = menu["NomeObjeto"].ToString().ToLower();
            string nomeDisplay = menu["NomeDisplay"].ToString().ToLower();
            string captionObjeto = menu["CaptionObjeto"]?.ToString().ToLower() ?? "";

            // ✅ MENUS DE NÍVEL 1
            if (nomeObjeto == "menudemandasmedic")
                return "bi bi-clipboard-check me-2"; // Ícone para Gestão de Demandas Medic

            if (nomeObjeto == "menuentrevista")
                return "bi bi-chat-left-dots me-2"; // Ícone para Entrevista

            // ✅ GESTÃO DE DEMANDAS MEDIC - NÍVEL 2
            if (nomeObjeto == "demandmedic")
                return "bi bi-plus-circle me-2"; // Criar Demanda Medic

            if (nomeObjeto == "listdemandmedic")
                return "bi bi-list-ul me-2"; // Listar Demandas Medic

            if (nomeObjeto == "menuminhasdemandasmedic")
                return "bi bi-person-lines-fill me-2"; // Minhas Demandas Medic

            if (nomeObjeto == "viewdemandbeforeacceptmedic")
                return "bi bi-eye me-2"; // Visualizar Demanda Medic

            if (nomeObjeto == "detaildemandmedic")
                return "bi bi-info-circle me-2"; // Detalhes Demanda Medic

            // ✅ ENTREVISTA - NÍVEL 2
            if (nomeObjeto == "interviewla")
                return "bi bi-robot me-2"; // Entrevista IA

            if (nomeObjeto == "medicalinterview")
                return "bi bi-heart-pulse me-2"; // Consulta Médica

            // ✅ MINHAS DEMANDAS MEDIC - NÍVEL 3
            if (nomeObjeto == "mydemandsopenmedic")
                return "bi bi-clock me-2"; // Em Aberto Medic

            if (nomeObjeto == "mydemandsprogressmedic")
                return "bi bi-arrow-right-circle me-2"; // Em Andamento Medic

            if (nomeObjeto == "mydemandswaitingmedic")
                return "bi bi-hourglass-split me-2"; // Aguardando Medic

            if (nomeObjeto == "mydemandsrefusedmedic")
                return "bi bi-x-circle me-2"; // Recusadas Medic

            if (nomeObjeto == "mydemandscompletedmedic")
                return "bi bi-check-circle me-2"; // Concluídas Medic

            // ✅ ÍCONE PADRÃO
            return "bi bi-circle me-2";
        }

        // ✅ VALIDA ACESSO À PÁGINA ATUAL
        private bool ValidarAcessoPaginaAtual()
        {
            try
            {
                if (Session["PermissoesPaginas"] == null)
                    return true;

                var permissoesPaginas = Session["PermissoesPaginas"] as Dictionary<string, bool>;
                string paginaAtual = ObterNomePaginaAtual();

                if (permissoesPaginas == null || !permissoesPaginas.ContainsKey(paginaAtual))
                    return false;

                return permissoesPaginas[paginaAtual];
            }
            catch (Exception)
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

        // ✅ CONFIGURA USUÁRIO - ADAPTADO PARA MEDIC
        private void ConfigurarUsuario()
        {
            lblUsuario.Text = Session["NomeUsuario"]?.ToString() ?? "Usuário";
            lblNomeSistema.Text = Session["NomeSistema"]?.ToString() ?? "Medic";

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

                // ✅ USA A MESMA PESSOA DAO DO GESTÃO
                PessoaDAO pessoaDao = new PessoaDAO();
                DataRow pessoa = pessoaDao.ObterPessoaPorUsuario(codUsuario);

                // ✅ CAMINHO ESPECÍFICO DO MEDIC
                string defaultAvatar = ResolveUrl("~/public/uploadmedic/images/imgDefultAvatar.jpg");
                string fotoUrl = defaultAvatar;

                if (pessoa != null)
                {
                    var foto = (pessoa["ImagemFoto"] ?? "").ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(foto))
                    {
                        fotoUrl = ResolveUrl("~/public/uploadmedic/images/" + foto);
                    }
                }

                // ✅ AVATAR DO TOPO
                imgAvatarUsuario.ImageUrl = fotoUrl;
                imgAvatarUsuario.AlternateText = "Avatar do Usuário";
                imgAvatarUsuario.Attributes["onerror"] = $"this.onerror=null;this.src='{defaultAvatar}';";

                // ✅ AVATAR DO DROPDOWN
                imgAvatarUsuarioDropdown.ImageUrl = fotoUrl;
                imgAvatarUsuarioDropdown.AlternateText = "Avatar do Usuário";
                imgAvatarUsuarioDropdown.Attributes["onerror"] = $"this.onerror=null;this.src='{defaultAvatar}';";
            }
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