using Microsoft.Ajax.Utilities;
using Plennusc.Core.Models.ModelsGestao.modelsUser;
using Plennusc.Core.Service.ServiceGestao.usuario;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class userSystemMenuManagement : System.Web.UI.Page
    {
        private userSystemMenuManagementService _service;
        private List<int> _sistemasEmpresasSelecionados = new List<int>();

        protected void Page_Load(object sender, EventArgs e)
        {
            _service = new userSystemMenuManagementService();

            if (!IsPostBack)
            {
                // Carrega todos os usuários inicialmente
                CarregarUsuarios();
            }
            else
            {
                // Só executa a lógica se houver um usuário selecionado E NÃO for o item "Selecione"
                if (ddlUsuarios.SelectedValue != "" && ddlUsuarios.SelectedIndex > 0)
                {
                    int codAutenticacao = Convert.ToInt32(ddlUsuarios.SelectedValue);
                    CriarMultiplasSecoesMenus(codAutenticacao);
                }
            }

            // Mantém o texto do filtro após postback
            if (!string.IsNullOrEmpty(FiltroAtual))
            {
                txtFiltroUsuario.Text = FiltroAtual;
            }
        }
        private string FiltroAtual
        {
            get { return ViewState["FiltroUsuario"] as string ?? ""; }
            set { ViewState["FiltroUsuario"] = value; }
        }

        // Modifique o método CarregarUsuarios para aceitar filtro
        private void CarregarUsuarios(string filtro = "")
        {
            try
            {
                var usuarios = _service.ObterUsuarios();

                // Aplica filtro APENAS pelo NomeCompleto
                if (!string.IsNullOrWhiteSpace(filtro))
                {
                    usuarios = usuarios
                        .Where(u => u.NomeCompleto.IndexOf(filtro, StringComparison.OrdinalIgnoreCase) >= 0)
                        .ToList();
                }

                // Limpa seleção atual ANTES de recarregar
                ddlUsuarios.SelectedIndex = -1;
                ddlUsuarios.ClearSelection();

                ddlUsuarios.DataSource = usuarios;
                ddlUsuarios.DataTextField = "NomeCompleto";
                ddlUsuarios.DataValueField = "CodAutenticacaoAcesso";
                ddlUsuarios.DataBind();

                // SEMPRE adiciona o item de seleção no início
                ddlUsuarios.Items.Insert(0, new ListItem("Selecione um usuário", ""));

                // Garante que NENHUM item está selecionado inicialmente
                ddlUsuarios.SelectedIndex = 0;

                // Atualiza informação
                litInfoUsuarios.Text = usuarios.Count == 0
                    ? $"Nenhum usuário encontrado com '{filtro}'"
                    : $"{usuarios.Count} usuários disponíveis" +
                      (string.IsNullOrEmpty(filtro) ? "" : $" filtrados por '{filtro}'");
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro ao carregar usuários: {ex.Message}");
            }
        }

        protected void ddlUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlUsuarios.SelectedIndex == 0 || ddlUsuarios.SelectedValue == "")
            {
                chkSistemaEmpresas.Items.Clear();
                pnlMultiplosMenus.Visible = false;
                idCheck.Visible = true;
                btnSalvarVinculos.Enabled = false;
                return;
            }

            try
            {
                int codAutenticacao = Convert.ToInt32(ddlUsuarios.SelectedValue);
                CarregarSistemaEmpresas(codAutenticacao);

                //Carregar menus automaticamente para sistemas já vinculados
                CarregarMenusAutomaticamente(codAutenticacao);

                pnlMultiplosMenus.Visible = true;
                idCheck.Visible = true;

                // HABILITA SE HOUVER MENUS
                btnSalvarVinculos.Enabled = _sistemasEmpresasSelecionados.Count > 0;
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro ao carregar sistemas: {ex.Message}");
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            string filtro = txtFiltroUsuario.Text.Trim();
            FiltroAtual = filtro;

            // Limpa seleção antes de filtrar
            ddlUsuarios.SelectedIndex = -1;
            ddlUsuarios.ClearSelection();

            // Recarrega usuários com filtro
            CarregarUsuarios(filtro);

            // Limpa os painéis (pois não tem seleção após filtrar)
            chkSistemaEmpresas.Items.Clear();
            pnlMultiplosMenus.Visible = false;
            idCheck.Visible = true;
            btnSalvarVinculos.Enabled = false;
        }

        protected void btnLimparFiltro_Click(object sender, EventArgs e)
        {
            txtFiltroUsuario.Text = "";
            FiltroAtual = "";

            // LIMPA a seleção antes de recarregar
            ddlUsuarios.ClearSelection();
            ddlUsuarios.SelectedValue = "";

            // Recarrega todos os usuários
            CarregarUsuarios();

            // NÃO chama o evento de seleção, pois limpou
            chkSistemaEmpresas.Items.Clear();
            pnlMultiplosMenus.Visible = false;
            idCheck.Visible = false;
            btnSalvarVinculos.Enabled = false;
        }

        // NOVO MÉTODO: Carregar menus automaticamente para sistemas já vinculados
        private void CarregarMenusAutomaticamente(int codAutenticacao)
        {
            pnlMultiplosMenus.Controls.Clear();
            _sistemasEmpresasSelecionados.Clear();

            // Verifica quais sistemas×empresas já estão vinculados ao usuário
            foreach (ListItem item in chkSistemaEmpresas.Items)
            {
                if (item.Selected) 
                {
                    int codSistemaEmpresa = Convert.ToInt32(item.Value);
                    _sistemasEmpresasSelecionados.Add(codSistemaEmpresa);
                    CriarSecaoMenu(codSistemaEmpresa, item.Text, codAutenticacao);
                }
            }

            pnlMultiplosMenus.Visible = _sistemasEmpresasSelecionados.Count > 0;
        }

        private void CarregarSistemaEmpresas(int codAutenticacao)
        {
            var sistemaEmpresas = _service.ObterSistemaEmpresas(codAutenticacao);
            chkSistemaEmpresas.Items.Clear();

            var sistemasUnicos = new Dictionary<string, ListItem>();

            foreach (var se in sistemaEmpresas)
            {
                string chaveUnica = $"{se.CodSistemaEmpresa}-{se.SistemaEmpresaDisplay}";

                if (!sistemasUnicos.ContainsKey(chaveUnica))
                {
                    var listItem = new ListItem(se.SistemaEmpresaDisplay, se.CodSistemaEmpresa.ToString());
                    listItem.Selected = se.JaVinculado;
                    sistemasUnicos.Add(chaveUnica, listItem);
                }
            }

            foreach (var item in sistemasUnicos.Values)
            {
                chkSistemaEmpresas.Items.Add(item);
            }

            // Garantir que o evento SelectedIndexChanged seja disparado para atualizar a tela
            chkSistemaEmpresas.AutoPostBack = true;
        }

        protected void chkSistemaEmpresas_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (chkSistemaEmpresas.Items.Count == 0)
            {
                pnlMultiplosMenus.Visible = false;
                return;
            }

            try
            {
                int codAutenticacao = Convert.ToInt32(ddlUsuarios.SelectedValue);
                CriarMultiplasSecoesMenus(codAutenticacao);

                // HABILITA/DESABILITA BASEADO NOS MENUS
                btnSalvarVinculos.Enabled = _sistemasEmpresasSelecionados.Count > 0;
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro ao carregar menus: {ex.Message}");
            }
        }

        private void CriarMultiplasSecoesMenus(int codAutenticacao)
        {
            pnlMultiplosMenus.Controls.Clear();
            _sistemasEmpresasSelecionados.Clear();

            foreach (ListItem item in chkSistemaEmpresas.Items)
            {
                if (item.Selected)
                {
                    int codSistemaEmpresa = Convert.ToInt32(item.Value);
                    _sistemasEmpresasSelecionados.Add(codSistemaEmpresa);
                    CriarSecaoMenu(codSistemaEmpresa, item.Text, codAutenticacao);
                }
            }

            pnlMultiplosMenus.Visible = _sistemasEmpresasSelecionados.Count > 0;
        }

        private void CriarSecaoMenu(int codSistemaEmpresa, string displaySistemaEmpresa, int codAutenticacao)
        {
            var containerSecao = new Panel();
            containerSecao.CssClass = "row mb-4 menu-section";
            containerSecao.ID = $"secMenu_{codSistemaEmpresa}";

            var tituloSecao = new Label();
            tituloSecao.Text = $"Menus para: {displaySistemaEmpresa}";
            tituloSecao.CssClass = "h5 section-title";

            var divTitulo = new Panel();
            divTitulo.CssClass = "col-12";
            divTitulo.Controls.Add(tituloSecao);

            var divModulos = new Panel();
            divModulos.CssClass = "col-12 modulos-container";

            var menus = _service.ObterMenusPorSistemaEmpresa(codSistemaEmpresa, codAutenticacao);
            var modulos = OrganizarMenusPorModulos(menus);

            foreach (var modulo in modulos)
            {
                var cardModulo = new Panel();
                cardModulo.CssClass = "modulo-card";

                // Body do módulo
                var bodyModulo = new Panel();
                bodyModulo.CssClass = "modulo-body";

                // Adiciona TODOS os menus do módulo (incluindo o principal)
                foreach (var menuItem in modulo.Value)
                {
                    var itemMenu = CriarItemMenu(menuItem, codSistemaEmpresa);
                    bodyModulo.Controls.Add(itemMenu);
                }

                cardModulo.Controls.Add(bodyModulo);
                divModulos.Controls.Add(cardModulo);
            }

            containerSecao.Controls.Add(divTitulo);
            containerSecao.Controls.Add(divModulos);
            pnlMultiplosMenus.Controls.Add(containerSecao);
        }

        private Panel CriarItemMenu(UsuarioSistemaEmpresaMenu menu, int codSistemaEmpresa)
        {
            var panelItem = new Panel();

            // DEFINE CSS CLASS BASEADO NO NÍVEL
            switch (menu.Conf_Nivel)
            {
                case 1:
                    panelItem.CssClass = "menu-item-nivel1";
                    break;
                case 2:
                    panelItem.CssClass = "menu-item-nivel2";
                    break;
                case 3:
                    panelItem.CssClass = "menu-item-nivel3";
                    break;
                default:
                    panelItem.CssClass = "menu-item";
                    break;
            }

            var checkbox = new CheckBox();

            // ID ÚNICO: inclui CodMenu E CodSistemaEmpresa
            checkbox.ID = $"chkMenu_{menu.CodMenu}_{codSistemaEmpresa}";

            // O problema é aqui! Se você tem o mesmo menu em múltiplos sistemas,
            // o ID está sendo duplicado. Vamos garantir que seja único:
            checkbox.ID = $"chkMenu_{menu.CodMenu}_{codSistemaEmpresa}_{menu.CodSistemaEmpresaMenu}";

            checkbox.Text = menu.NomeDisplay;
            checkbox.Checked = menu.MenuJaVinculado;
            checkbox.CssClass = "menu-checkbox";

            panelItem.Controls.Add(checkbox);
            return panelItem;
        }

        private Dictionary<string, List<UsuarioSistemaEmpresaMenu>> OrganizarMenusPorModulos(List<UsuarioSistemaEmpresaMenu> menus)
        {
            var modulos = new Dictionary<string, List<UsuarioSistemaEmpresaMenu>>();

            var menusNivel1 = menus.Where(m => m.Conf_Nivel == 1).OrderBy(m => m.Conf_Ordem).ToList();

            foreach (var modulo in menusNivel1)
            {
                string nomeModulo = modulo.NomeDisplay;
                var listaModulo = new List<UsuarioSistemaEmpresaMenu>();

                // ADICIONA O PRÓPRIO MÓDULO (NÍVEL 1) À LISTA
                listaModulo.Add(modulo);

                // ADICIONA OS SUBMENUS RECURSIVAMENTE
                AdicionarDescendentesRecursivamente(modulo.CodMenu, menus, listaModulo);

                modulos[nomeModulo] = listaModulo;
            }

            return modulos;
        }


        private void AdicionarDescendentesRecursivamente(int codMenuPai, List<UsuarioSistemaEmpresaMenu> todosMenus, List<UsuarioSistemaEmpresaMenu> resultado)
        {
            var filhos = todosMenus.Where(m => m.CodMenuPai == codMenuPai)
                                  .OrderBy(m => m.Conf_Ordem)
                                  .ToList();

            foreach (var filho in filhos)
            {
                resultado.Add(filho);
                AdicionarDescendentesRecursivamente(filho.CodMenu, todosMenus, resultado);
            }
        }

        protected void btnSalvarVinculos_Click(object sender, EventArgs e)
        {
            if (ddlUsuarios.SelectedValue == "")
            {
                MostrarMensagemErro("Selecione um usuário para continuar.");
                return;
            }

            int codAutenticacao = Convert.ToInt32(ddlUsuarios.SelectedValue);

            try
            {
                // 1. Processa Sistemas×Empresas SELECIONADOS
                foreach (ListItem sistemaEmpresa in chkSistemaEmpresas.Items)
                {
                    int codSistemaEmpresa = Convert.ToInt32(sistemaEmpresa.Value);

                    if (sistemaEmpresa.Selected)
                    {
                        _service.VincularUsuarioSistemaEmpresa(codSistemaEmpresa, codAutenticacao);
                        VincularMenusUsuario(codSistemaEmpresa, codAutenticacao);
                    }
                }

                // 2. Processa Sistemas×Empresas NÃO SELECIONADOS
                foreach (ListItem sistemaEmpresa in chkSistemaEmpresas.Items)
                {
                    int codSistemaEmpresa = Convert.ToInt32(sistemaEmpresa.Value);

                    if (!sistemaEmpresa.Selected)
                    {
                        _service.DesvincularUsuarioSistemaEmpresa(codSistemaEmpresa, codAutenticacao);
                    }
                }

                MostrarMensagemSucesso("Vínculos salvos com sucesso!");

                // Recarregar dados
                int codAutenticacaoAtual = Convert.ToInt32(ddlUsuarios.SelectedValue);
                CarregarSistemaEmpresas(codAutenticacaoAtual);
                CriarMultiplasSecoesMenus(codAutenticacaoAtual);
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro ao salvar vínculos: {ex.Message}");
            }
        }

        private void VincularMenusUsuario(int codSistemaEmpresa, int codAutenticacao)
        {
            _service.DesvincularTodosMenusUsuario(codSistemaEmpresa, codAutenticacao);

            var containerSecao = pnlMultiplosMenus.FindControl($"secMenu_{codSistemaEmpresa}");
            if (containerSecao != null)
            {
                var checkboxes = BuscarCheckboxesRecursivamente(containerSecao);

                foreach (CheckBox checkbox in checkboxes)
                {
                    if (checkbox.Checked)
                    {
                        string[] partes = checkbox.ID.Split('_');
                        if (partes.Length >= 2 && int.TryParse(partes[1], out int codMenu))
                        {
                            _service.VincularMenuUsuario(codSistemaEmpresa, codAutenticacao, codMenu);
                        }
                    }
                }
            }
        }

        private List<CheckBox> BuscarCheckboxesRecursivamente(Control container)
        {
            var checkboxes = new List<CheckBox>();

            foreach (Control control in container.Controls)
            {
                if (control is CheckBox checkbox)
                {
                    checkboxes.Add(checkbox);
                }
                else
                {
                    checkboxes.AddRange(BuscarCheckboxesRecursivamente(control));
                }
            }

            return checkboxes;
        }

        private void MostrarMensagemSucesso(string mensagem)
        {
            string script = $@"
                Swal.fire({{
                    icon: 'success',
                    title: 'Sucesso!',
                    text: '{mensagem.Replace("'", "\\'")}',
                    confirmButtonText: 'OK',
                    customClass: {{ confirmButton: 'btn btn-success' }}
                }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "Sucesso", script, true);
        }

        private void MostrarMensagemErro(string mensagem)
        {
            string script = $@"
                Swal.fire({{
                    icon: 'error',
                    title: 'Erro!',
                    text: '{mensagem.Replace("'", "\\'")}',
                    confirmButtonText: 'Fechar'
                }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "Erro", script, true);
        }
    }
}