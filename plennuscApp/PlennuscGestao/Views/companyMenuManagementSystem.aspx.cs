using Plennusc.Core.Models.ModelsGestao.modelsMenu;
using Plennusc.Core.Service.ServiceGestao.PlatformSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class companyMenuManagementSystem : System.Web.UI.Page
    {
        private companyMenuManagementSystemService _service;

        protected void Page_Load(object sender, EventArgs e)
        {
            _service = new companyMenuManagementSystemService();

            if (Session["CodUsuario"] == null)
            {
                Response.Redirect("~/SignIn.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarEmpresas();
                pnlMenus.Visible = false;
            }
            else
            {
                // RECRIA OS CONTROLES NO POSTBACK
                if (ddlSistema.SelectedValue != "")
                {
                    int codSistemaEmpresa = Convert.ToInt32(ddlSistema.SelectedValue);
                    CarregarMenusAgrupados(codSistemaEmpresa);
                }
            }
        }

        private void CarregarEmpresas()
        {
            try
            {
                var empresas = _service.ListarEmpresasAtivas();
                ddlEmpresa.DataSource = empresas;
                ddlEmpresa.DataTextField = "NomeFantasia";
                ddlEmpresa.DataValueField = "CodEmpresa";
                ddlEmpresa.DataBind();
                ddlEmpresa.Items.Insert(0, new ListItem("-- Selecione uma Empresa --", ""));
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro ao carregar empresas: {ex.Message}");
            }
        }

        protected void ddlEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEmpresa.SelectedValue != "")
            {
                int codEmpresa = Convert.ToInt32(ddlEmpresa.SelectedValue);
                CarregarSistemas(codEmpresa);
            }
            else
            {
                ddlSistema.Items.Clear();
                ddlSistema.Items.Insert(0, new ListItem("-- Selecione um Sistema --", ""));
                pnlMenus.Visible = false;
            }
        }

        private void CarregarSistemas(int codEmpresa)
        {
            try
            {
                var sistemas = _service.ListarSistemasPorEmpresa(codEmpresa);
                ddlSistema.DataSource = sistemas;
                ddlSistema.DataTextField = "NomeDisplay";
                ddlSistema.DataValueField = "CodSistemaEmpresa";
                ddlSistema.DataBind();
                ddlSistema.Items.Insert(0, new ListItem("-- Selecione um Sistema --", ""));
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro ao carregar sistemas: {ex.Message}"          );
            }
        }

        protected void ddlSistema_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSistema.SelectedValue != "")
            {
                int codSistemaEmpresa = Convert.ToInt32(ddlSistema.SelectedValue);
                CarregarMenusAgrupados(codSistemaEmpresa);
                pnlMenus.Visible = true;

                var empresaNome = ddlEmpresa.SelectedItem.Text;
                var sistemaNome = ddlSistema.SelectedItem.Text;
                litInfoSistema.Text = $"{empresaNome} - {sistemaNome}";
            }
            else
            {
                pnlMenus.Visible = false;
            }
        }

        private void CarregarMenusAgrupados(int codSistemaEmpresa)
        {
            try
            {
                var todosMenus = _service.ListarMenusParaVincular(codSistemaEmpresa);
                modulesContainer.Controls.Clear();

                // Ordem fixa dos módulos principais
                var ordemDesejada = new[] { "Pessoas", "Chatbot", "Preços", "Parametrização", "Demanda" };

                // Pegar apenas os menus raiz (pais) - nível 1
                var menusPais = todosMenus
                    .Where(m => m.CodMenuPai == null || m.CodMenuPai == 0)
                    .ToList();

                var orderedParents = new List<MenuModel>();

                // Ordenar conforme a ordem desejada
                foreach (var nome in ordemDesejada)
                {
                    var encontrado = menusPais.FirstOrDefault(m =>
                        string.Equals((m.NomeDisplay ?? m.NomeMenu), nome, StringComparison.OrdinalIgnoreCase));
                    if (encontrado != null)
                    {
                        orderedParents.Add(encontrado);
                        menusPais.Remove(encontrado);
                    }
                }

                // Adicionar os que sobraram
                orderedParents.AddRange(menusPais.OrderBy(m => m.Conf_Ordem));

                // Criar módulos dinamicamente
                foreach (var moduloPai in orderedParents)
                {
                    CriarModulo(moduloPai, todosMenus);
                }
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro ao carregar menus: {ex.Message}");
            }
        }

        private void CriarModulo(MenuModel moduloPai, List<MenuModel> todosMenus)
        {
            // Criar o container do módulo
            var moduleCard = new Panel();
            moduleCard.Attributes["class"] = "module-card";

            // Cabeçalho do módulo - AGORA COM CHECKBOX
            var moduleHeader = new Panel();
            moduleHeader.Attributes["class"] = "module-header";

            // CHECKBOX do módulo principal
            var chkModulo = new CheckBox
            {
                ID = $"chkModulo_{moduloPai.CodMenu}",
                Text = $"📁 {moduloPai.NomeDisplay ?? moduloPai.NomeMenu}",
                Checked = moduloPai.Vinculado,
                CssClass = "module-main-checkbox"
            };
            moduleHeader.Controls.Add(chkModulo);

            moduleCard.Controls.Add(moduleHeader);

            // Corpo do módulo com os checkboxes dos filhos
            var moduleBody = new Panel();
            moduleBody.Attributes["class"] = "module-body";

            // Buscar filhos diretos deste módulo pai
            var filhosDiretos = todosMenus
                .Where(m => m.CodMenuPai == moduloPai.CodMenu)
                .OrderBy(m => m.Conf_Ordem)
                .ToList();

            foreach (var menuFilho in filhosDiretos)
            {
                AdicionarMenuItem(moduleBody, menuFilho, false);

                // Buscar netos (nível 3)
                var netos = todosMenus
                    .Where(m => m.CodMenuPai == menuFilho.CodMenu)
                    .OrderBy(m => m.Conf_Ordem)
                    .ToList();

                foreach (var neto in netos)
                {
                    AdicionarMenuItem(moduleBody, neto, true);
                }
            }

            moduleCard.Controls.Add(moduleBody);
            modulesContainer.Controls.Add(moduleCard);
        }

        private void AdicionarMenuItem(Panel container, MenuModel menu, bool isChild)
        {
            var menuItemContainer = new Panel();
            menuItemContainer.Attributes["class"] = isChild ? "menu-item-container menu-child" : "menu-item-container";

            var checkbox = new CheckBox
            {
                ID = $"chkMenu_{menu.CodMenu}",
                Text = menu.NomeDisplay ?? menu.NomeMenu,
                Checked = menu.Vinculado,
                CssClass = "menu-checkbox"
            };

            menuItemContainer.Controls.Add(checkbox);
            container.Controls.Add(menuItemContainer);
        }

        //protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        //{
        //    SetAllCheckboxes(chkSelectAll.Checked);
        //}

        //private void SetAllCheckboxes(bool isChecked)
        //{
        //    SetCheckboxesRecursive(modulesContainer, isChecked);
        //}

        //private void SetCheckboxesRecursive(Control parent, bool isChecked)
        //{
        //    foreach (Control control in parent.Controls)
        //    {
        //        if (control is CheckBox checkbox)
        //        {
        //            checkbox.Checked = isChecked;
        //        }
        //        else
        //        {
        //            SetCheckboxesRecursive(control, isChecked);
        //        }
        //    }
        //}

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlSistema.SelectedValue))
            {
                MostrarMensagemErro("Selecione um sistema e empresa para continuar.");
                return;
            }

            int codSistemaEmpresa = Convert.ToInt32(ddlSistema.SelectedValue);

            try
            {
                // 1. LER CHECKBOXES DO FORM
                List<int> menusSelecionados = ColetarMenusDoForm();
                menusSelecionados = menusSelecionados.Distinct().ToList();

                if (!menusSelecionados.Contains(37))
                    menusSelecionados.Add(37);

                _service.DesvincularTodosMenusSistemaEmpresa(codSistemaEmpresa);

                foreach (int codMenu in menusSelecionados)
                {
                    _service.VincularMenuSistemaEmpresa(codSistemaEmpresa, codMenu);
                }

                MostrarMensagemSucesso("Menus salvos com sucesso!");

                // 4. RECARREGAR INTERFACE
                CarregarMenusAgrupados(codSistemaEmpresa);
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro ao salvar vínculos: {ex.Message}");
            }
        }



        // MÉTODO QUE FUNCIONA - LÊ DIRETO DO REQUEST.FORM
        private List<int> ColetarMenusDoForm()
        {
            var menusSelecionados = new List<int>();
            var regex = new Regex(@"chk(?:Menu|Modulo)_(\d+)", RegexOptions.IgnoreCase);

            foreach (string key in Request.Form.AllKeys)
            {
                if (string.IsNullOrEmpty(key)) continue;

                var m = regex.Match(key);
                if (!m.Success) continue;

                // valor do checkbox (quando marcado costuma ser "on" ou "true")
                var val = Request.Form[key];
                if (string.IsNullOrEmpty(val)) continue;

                if (val.Equals("on", StringComparison.OrdinalIgnoreCase) ||
                    val.Equals("true", StringComparison.OrdinalIgnoreCase))
                {
                    if (int.TryParse(m.Groups[1].Value, out int codMenu))
                        menusSelecionados.Add(codMenu);
                }
            }

            return menusSelecionados;
        }
        protected void btnLimpar_Click(object sender, EventArgs e)
        {
            //chkSelectAll.Checked = false;
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