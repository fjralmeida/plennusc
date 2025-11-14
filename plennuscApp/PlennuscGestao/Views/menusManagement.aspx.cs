using Plennusc.Core.Models.ModelsGestao.modelsMenu;
using Plennusc.Core.Service.ServiceGestao.menu;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.DynamicData;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class menusManagement : System.Web.UI.Page
    {
        private menusManagementService _menuService;

        protected void Page_Load(object sender, EventArgs e)
        {
            _menuService = new menusManagementService();

            if (Session["CodUsuario"] == null)
            {
                Response.Redirect("~/SignIn.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarMenusPai();
                CarregarGridMenus();
                LimparFormulario();
            }
        }

        private void CarregarMenusPai()
        {
            try
            {
                var menusPai = _menuService.ListarMenusPrincipais();
                ddlMenuPai.DataSource = menusPai;
                ddlMenuPai.DataTextField = "NomeDisplay";
                ddlMenuPai.DataValueField = "CodMenu";
                ddlMenuPai.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao carregar menus pai: {ex.Message}", false);
            }
        }

        private void CarregarGridMenus()
        {
            try
            {
                var menus = _menuService.ListarTodosMenus();
                gvMenus.DataSource = menus;
                gvMenus.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao carregar menus: {ex.Message}", false);
            }
        }
        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            if (!ValidarFormulario())
                return;

            try
            {
                var menu = new menusManagementModels
                {
                    NomeMenu = txtNomeMenu.Text.Trim(),
                    NomeDisplay = txtNomeDisplay.Text.Trim(),
                    NomeObjeto = txtNomeObjeto.Text.Trim(),
                    CaptionObjeto = txtCaptionObjeto.Text.Trim(),
                    HttpRouter = txtHttpRouter.Text.Trim(),
                    CodMenuPai = string.IsNullOrEmpty(ddlMenuPai.SelectedValue) ? (int?)null : Convert.ToInt32(ddlMenuPai.SelectedValue),
                    Conf_Ordem = Convert.ToInt32(txtOrdem.Text),
                    Conf_Nivel = Convert.ToInt32(txtNivel.Text),
                    Conf_Habilitado = chkHabilitado.Checked
                };

                bool sucesso;
                string mensagem;

                if (string.IsNullOrEmpty(hfCodMenuEdicao.Value))
                {
                    // Novo menu
                    sucesso = _menuService.InserirMenu(menu);
                    mensagem = sucesso ? "Menu cadastrado com sucesso!" : "Erro ao cadastrar menu.";
                }
                else
                {
                    // Edição
                    menu.CodMenu = Convert.ToInt32(hfCodMenuEdicao.Value);
                    sucesso = _menuService.AtualizarMenu(menu);
                    mensagem = sucesso ? "Menu atualizado com sucesso!" : "Erro ao atualizar menu.";
                }

                if (sucesso)
                {
                    LimparFormulario();
                    CarregarGridMenus();
                    CarregarMenusPai();
                }

                MostrarMensagem(mensagem, sucesso);
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao salvar menu: {ex.Message}", false);
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            LimparFormulario();
        }

        protected void gvMenus_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Editar")
            {
                EditarMenu(Convert.ToInt32(e.CommandArgument));
            }
            else if (e.CommandName == "Excluir")
            {
                ExcluirMenu(Convert.ToInt32(e.CommandArgument));
            }
        }

        protected void gvMenus_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            // Aplicar estilos específicos se necessário
        }

        public string GetIconByNivel(object nivelObj)
        {
            if (nivelObj == null) return "bi bi-folder";

            int nivel = Convert.ToInt32(nivelObj);

            if (nivel == 1) return "bi bi-folder";
            if (nivel == 2) return "bi bi-folder2";
            if (nivel == 3) return "bi bi-file-earmark";

            return "bi bi-folder";
        }

        private void EditarMenu(int codMenu)
        {
            try
            {
                var menu = _menuService.ObterMenuPorCodigo(codMenu);
                if (menu != null)
                {
                    txtNomeMenu.Text = menu.NomeMenu;
                    txtNomeDisplay.Text = menu.NomeDisplay;
                    txtNomeObjeto.Text = menu.NomeObjeto;
                    txtCaptionObjeto.Text = menu.CaptionObjeto;
                    txtHttpRouter.Text = menu.HttpRouter;

                    if (menu.CodMenuPai.HasValue)
                        ddlMenuPai.SelectedValue = menu.CodMenuPai.Value.ToString();
                    else
                        ddlMenuPai.SelectedIndex = 0;

                    txtOrdem.Text = menu.Conf_Ordem.ToString();
                    txtNivel.Text = menu.Conf_Nivel.ToString();
                    chkHabilitado.Checked = menu.Conf_Habilitado;

                    hfCodMenuEdicao.Value = menu.CodMenu.ToString();
                    lblFormTitle.Text = "Editar Menu";
                    btnCancelar.Visible = true;

                    // Scroll para o formulário
                    ScriptManager.RegisterStartupScript(this, GetType(), "scrollToForm",
                        "document.querySelector('.menu-form').scrollIntoView({ behavior: 'smooth' });", true);
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao carregar menu para edição: {ex.Message}", false);
            }
        }

        private void ExcluirMenu(int codMenu)
        {
            try
            {
                bool sucesso = _menuService.ExcluirMenu(codMenu);
                string mensagem = sucesso ? "Menu excluído com sucesso!" : "Erro ao excluir menu.";

                if (sucesso)
                {
                    CarregarGridMenus();
                    CarregarMenusPai();
                }

                MostrarMensagem(mensagem, sucesso);
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao excluir menu: {ex.Message}", false);
            }
        }

        private bool ValidarFormulario()
        {
            if (string.IsNullOrWhiteSpace(txtNomeMenu.Text))
            {
                MostrarMensagem("Nome do menu é obrigatório.", false);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNomeDisplay.Text))
            {
                MostrarMensagem("Nome para display é obrigatório.", false);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNomeObjeto.Text))
            {
                MostrarMensagem("Nome do objeto é obrigatório.", false);
                return false;
            }

            return true;
        }

        private void LimparFormulario()
        {
            txtNomeMenu.Text = "";
            txtNomeDisplay.Text = "";
            txtNomeObjeto.Text = "";
            txtCaptionObjeto.Text = "";
            txtHttpRouter.Text = "";
            ddlMenuPai.SelectedIndex = 0;
            txtOrdem.Text = "0";
            txtNivel.Text = "1";
            chkHabilitado.Checked = true;
            hfCodMenuEdicao.Value = "";
            lblFormTitle.Text = "Cadastrar Novo Menu";
            btnCancelar.Visible = false;
        }

        private void MostrarMensagem(string mensagem, bool sucesso)
        {
            // Implementar sua lógica de exibição de mensagens
            // Pode ser um Label, Toast, ou Alert
            ScriptManager.RegisterStartupScript(this, GetType(), "showMessage",
                $"alert('{mensagem}');", true);
        }
    }
}