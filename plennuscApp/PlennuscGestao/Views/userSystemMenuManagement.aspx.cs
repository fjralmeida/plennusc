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
        private List<int> _sistemasEmpresasSelecionados = new List<int>(); // ADICIONAR ESTA LINHA

        protected void Page_Load(object sender, EventArgs e)
        {
            _service = new userSystemMenuManagementService();

            if (!IsPostBack)
            {
                // Carregamento inicial
                CarregarUsuarios();
            }
            else
            {
                // EM TODOS OS POSTBACKS, recrie os controles dinâmicos
                if (ddlUsuarios.SelectedValue != "")
                {
                    int codAutenticacao = Convert.ToInt32(ddlUsuarios.SelectedValue);
                    CriarMultiplasSecoesMenus(codAutenticacao);
                }
            }
        }

        private void CarregarUsuarios()
        {
            try
            {
                var usuarios = _service.ObterUsuarios();
                ddlUsuarios.DataSource = usuarios;
                ddlUsuarios.DataTextField = "NomeCompleto";
                ddlUsuarios.DataValueField = "CodAutenticacaoAcesso";
                ddlUsuarios.DataBind();
                ddlUsuarios.Items.Insert(0, new ListItem("Selecione um usuário", ""));
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro ao carregar usuários: {ex.Message}");
            }
        }

        protected void ddlUsuarios_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlUsuarios.SelectedValue == "")
            {
                chkSistemaEmpresas.Items.Clear();
                pnlMultiplosMenus.Visible = false; // MUDAR para pnlMultiplosMenus
                idCheck.Visible = true;
                //btnSalvarVinculos.Visible = true;

                return;
            }

            try
            {
                int codAutenticacao = Convert.ToInt32(ddlUsuarios.SelectedValue);
                CarregarSistemaEmpresas(codAutenticacao);
                pnlMultiplosMenus.Visible = false; // MUDAR para pnlMultiplosMenus
                idCheck.Visible = true;
                //btnSalvarVinculos.Visible = true;
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro ao carregar sistemas: {ex.Message}");
            }
        }

        private void CarregarSistemaEmpresas(int codAutenticacao)
        {
            var sistemaEmpresas = _service.ObterSistemaEmpresas(codAutenticacao);
            chkSistemaEmpresas.Items.Clear();

            // ✅ CORREÇÃO: REMOVER DUPLICATAS
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

            // ✅ ADICIONA APENAS OS ÚNICOS
            foreach (var item in sistemasUnicos.Values)
            {
                chkSistemaEmpresas.Items.Add(item);
            }
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
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro ao carregar menus: {ex.Message}");
            }
        }

        private void CriarMultiplasSecoesMenus(int codAutenticacao)
        {
            // Limpa o container anterior
            pnlMultiplosMenus.Controls.Clear();
            _sistemasEmpresasSelecionados.Clear();

            // Para cada Sistema×Empresa selecionado
            foreach (ListItem item in chkSistemaEmpresas.Items)
            {
                if (item.Selected)
                {
                    int codSistemaEmpresa = Convert.ToInt32(item.Value);
                    _sistemasEmpresasSelecionados.Add(codSistemaEmpresa);

                    // Cria uma seção para este Sistema×Empresa
                    CriarSecaoMenu(codSistemaEmpresa, item.Text, codAutenticacao);
                }
            }

            pnlMultiplosMenus.Visible = _sistemasEmpresasSelecionados.Count > 0;
        }

        private void CriarSecaoMenu(int codSistemaEmpresa, string displaySistemaEmpresa, int codAutenticacao)
        {
            // Cria o container da seção
            var containerSecao = new Panel();
            containerSecao.CssClass = "row mb-4 menu-section";
            containerSecao.ID = $"secMenu_{codSistemaEmpresa}";

            // Cria o título da seção
            var tituloSecao = new Label();
            tituloSecao.Text = $"Menus para: {displaySistemaEmpresa}";
            tituloSecao.CssClass = "h5 section-title";

            var divTitulo = new Panel();
            divTitulo.CssClass = "col-12";
            divTitulo.Controls.Add(tituloSecao);

            // Cria o container dos checkboxes
            var divCheckboxes = new Panel();
            divCheckboxes.CssClass = "col-12";

            var divScroll = new Panel();
            divScroll.Style.Add("max-height", "400px");
            divScroll.Style.Add("overflow-y", "auto");
            divScroll.CssClass = "menu-checklist";

            // Cria o CheckBoxList para os menus
            var chkMenus = new CheckBoxList();
            chkMenus.ID = $"chkMenus_{codSistemaEmpresa}";
            chkMenus.CssClass = "form-check";

            var menus = _service.ObterMenusPorSistemaEmpresa(codSistemaEmpresa, codAutenticacao);

            // ORDENAR OS MENUS HIERARQUICAMENTE
            var menusOrdenados = OrdenarMenusHierarquicamente(menus);

            // Adiciona os itens com hierarquia visual
            foreach (var menu in menusOrdenados)
            {
                var prefix = new string('─', (menu.Conf_Nivel - 1) * 2);
                var listItem = new ListItem($"{prefix} {menu.NomeDisplay}", menu.CodSistemaEmpresaMenu.ToString());
                listItem.Selected = menu.MenuJaVinculado;

                listItem.Attributes["class"] = $"menu-item level-{menu.Conf_Nivel}";
                chkMenus.Items.Add(listItem);
            }

            // Monta a hierarquia
            divScroll.Controls.Add(chkMenus);
            divCheckboxes.Controls.Add(divScroll);

            containerSecao.Controls.Add(divTitulo);
            containerSecao.Controls.Add(divCheckboxes);

            // Adiciona uma linha separadora (exceto para o primeiro)
            if (pnlMultiplosMenus.Controls.Count > 0)
            {
                var separador = new Panel();
                separador.CssClass = "col-12";
                separador.Style.Add("border-top", "2px solid #e8eaed");
                separador.Style.Add("margin-top", "20px");
                separador.Style.Add("padding-top", "20px");
                pnlMultiplosMenus.Controls.Add(separador);
            }

            pnlMultiplosMenus.Controls.Add(containerSecao);
        }

        // MÉTODO DE ORDENAÇÃO HIERÁRQUICA COMPLETO
       private List<UsuarioSistemaEmpresaMenu> OrdenarMenusHierarquicamente(List<UsuarioSistemaEmpresaMenu> menus)
        {
            if (menus == null || !menus.Any())
                return new List<UsuarioSistemaEmpresaMenu>();

            var resultado = new List<UsuarioSistemaEmpresaMenu>();
    
            // 1. Buscar menus de nível 1 dinamicamente (onde CodMenuPai é NULL ou 0)
            var menusNivel1 = menus.Where(m => m.CodMenuPai == null || m.CodMenuPai == 0)
                                  .OrderBy(m => m.Conf_Ordem)
                                  .ThenBy(m => m.NomeDisplay)
                                  .ToList();

            foreach (var menuNivel1 in menusNivel1)
            {
                // Adiciona o menu nível 1
                resultado.Add(menuNivel1);
        
                // 2. Buscar filhos dinamicamente baseado no CodMenuPai real
                AdicionarFilhosRecursivamente(menuNivel1.CodMenu, menus, resultado);
            }
    
            return resultado;
        }

private void AdicionarFilhosRecursivamente(int codMenuPai, List<UsuarioSistemaEmpresaMenu> todosMenus, List<UsuarioSistemaEmpresaMenu> resultado)
{
    // Buscar filhos deste menu pai específico
    var filhos = todosMenus.Where(m => m.CodMenuPai == codMenuPai)
                          .OrderBy(m => m.Conf_Ordem)
                          .ThenBy(m => m.NomeDisplay)
                          .ToList();

    foreach (var filho in filhos)
    {
        // Adiciona o filho à lista resultado
        resultado.Add(filho);
        
        // Buscar netos recursivamente (nível +1)
        AdicionarFilhosRecursivamente(filho.CodMenu, todosMenus, resultado);
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
                // 1. Primeiro, processa apenas os Sistemas×Empresas SELECIONADOS
                foreach (ListItem sistemaEmpresa in chkSistemaEmpresas.Items)
                {
                    int codSistemaEmpresa = Convert.ToInt32(sistemaEmpresa.Value);

                    if (sistemaEmpresa.Selected)
                    {
                        // Vincular usuário ao sistema×empresa
                        _service.VincularUsuarioSistemaEmpresa(codSistemaEmpresa, codAutenticacao);

                        // Vincular os menus específicos para este sistema×empresa
                        VincularMenusUsuario(codSistemaEmpresa, codAutenticacao);
                    }
                }

                // 2. AGORA, processa os NÃO SELECIONADOS para desvincular
                foreach (ListItem sistemaEmpresa in chkSistemaEmpresas.Items)
                {
                    int codSistemaEmpresa = Convert.ToInt32(sistemaEmpresa.Value);

                    if (!sistemaEmpresa.Selected)
                    {
                        // Desvincular usuário do sistema×empresa (e todos seus menus)
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
            // Encontra o CheckBoxList específico para este Sistema×Empresa
            var chkMenus = pnlMultiplosMenus.FindControl($"chkMenus_{codSistemaEmpresa}") as CheckBoxList;

            if (chkMenus != null)
            {
                // Primeiro remove todos os vínculos de menu para este usuário neste sistema×empresa
                _service.DesvincularTodosMenusUsuario(codSistemaEmpresa, codAutenticacao);

                // Depois adiciona os selecionados
                foreach (ListItem menu in chkMenus.Items)
                {
                    if (menu.Selected)
                    {
                        // AQUI ESTÁ O PROBLEMA - Use o código do menu, não do sistema×empresa
                        int codMenu = Convert.ToInt32(menu.Value);
                        _service.VincularMenuUsuario(codSistemaEmpresa, codAutenticacao, codMenu);
                    }
                }
            }
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

        // REMOVER ESTE MÉTODO DUPLICADO QUE ESTÁ CAUSANDO ERROS:
        // private void CarregarMenusSistemaEmpresa(int codSistemaEmpresa, int codAutenticacao)
        // {
        //     // Este método foi substituído pelo CriarSecaoMenu
        // }
    }
}