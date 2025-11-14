using Plennusc.Core.Models.ModelsGestao.modelsMenu;
using Plennusc.Core.Service.ServiceGestao.PlatformSys;
using System;
using System.Collections.Generic;
using System.Linq;
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
                MostrarMensagem($"Erro ao carregar empresas: {ex.Message}", false);
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
                MostrarMensagem($"Erro ao carregar sistemas: {ex.Message}", false);
            }
        }

        protected void ddlSistema_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSistema.SelectedValue != "")
            {
                int codSistemaEmpresa = Convert.ToInt32(ddlSistema.SelectedValue);
                CarregarMenusHierarquicos(codSistemaEmpresa);
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

        private void CarregarMenusHierarquicos(int codSistemaEmpresa)
        {
            try
            {
                var menus = _service.ListarMenusParaVincular(codSistemaEmpresa);
                chkMenus.Items.Clear();

                // Ordem fixa dos pais que você pediu
                var ordemDesejada = new[] { "Pessoas", "Chatbot", "Preços", "Parametrização", "Demanda" };

                // pegar apenas os menus raiz (pais) — considere CodMenuPai NULL ou 0 como raiz
                var menusPais = menus
                    .Where(m => m.CodMenuPai == null || m.CodMenuPai == 0)
                    .ToList();

                var orderedParents = new List<MenuModel>();

                // adiciona na ordem desejada quando encontrar correspondência por NomeDisplay (case-insensitive)
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

                // adiciona os que sobraram (ordenados por Conf_Ordem)
                orderedParents.AddRange(menusPais.OrderBy(m => m.Conf_Ordem));

                // adiciona cada pai e seus filhos recursivamente (todos abertos)
                foreach (var menuPai in orderedParents)
                {
                    AdicionarMenuItem(menuPai, 0, menuPai.Vinculado);
                    AdicionarFilhosRecursivamente(menus, menuPai.CodMenu, 1);
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao carregar menus: {ex.Message}", false);
            }
        }

        private void AdicionarFilhosRecursivamente(List<MenuModel> todosMenus, int codMenuPai, int nivelAtual)
        {
            // buscar filhos diretos ordenados
            var filhos = todosMenus
                .Where(m => m.CodMenuPai == codMenuPai)
                .OrderBy(m => m.Conf_Ordem)
                .ToList();

            foreach (var filho in filhos)
            {
                AdicionarMenuItem(filho, nivelAtual, filho.Vinculado);
                // recursão para netos
                AdicionarFilhosRecursivamente(todosMenus, filho.CodMenu, nivelAtual + 1);
            }
        }

        private void AdicionarMenuItem(MenuModel menu, int nivel, bool vinculado)
        {
            string textoLimpo = menu.NomeDisplay;

            ListItem item = new ListItem();
            item.Value = menu.CodMenu.ToString();
            item.Selected = vinculado;
            item.Attributes["data-level"] = nivel.ToString();

            if (nivel == 0)
            {
                // PAI – clicável
                item.Text = $"<span class='menu-parent' data-id='{menu.CodMenu}'>📁 {textoLimpo}</span>";
                item.Attributes["class"] = "menu-parent-row";
            }
            else
            {
                // FILHO – inicia escondido
                item.Text = $"<span style='margin-left:{nivel * 20}px' class='menu-child'>{textoLimpo}</span>";
                item.Attributes["class"] = $"menu-child-row child-of-{menu.CodMenuPai}";
                item.Attributes["style"] = "display:none;";
            }

            chkMenus.Items.Add(item);
        }

        private string GetIconByLevel(int nivel)
        {
            switch (nivel)
            {
                case 0: return "bi bi-folder";
                case 1: return "bi bi-folder2";
                case 2: return "bi bi-file-earmark";
                case 3: return "bi bi-file-earmark";
                default: return "bi bi-folder";
            }
        }

        protected void chkSelectAll_CheckedChanged(object sender, EventArgs e)
        {
            foreach (ListItem item in chkMenus.Items)
            {
                item.Selected = chkSelectAll.Checked;
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            if (ddlSistema.SelectedValue == "")
            {
                MostrarMensagem("Selecione um sistema e empresa para continuar.", false);
                return;
            }

            int codSistemaEmpresa = Convert.ToInt32(ddlSistema.SelectedValue);

            try
            {
                _service.DesvincularTodosMenusSistemaEmpresa(codSistemaEmpresa);

                int count = 0;
                foreach (ListItem item in chkMenus.Items)
                {
                    if (item.Selected)
                    {
                        int codMenu = Convert.ToInt32(item.Value);
                        _service.VincularMenuSistemaEmpresa(codSistemaEmpresa, codMenu);
                        count++;
                    }
                }

                MostrarMensagem($"{count} menus vinculados com sucesso ao sistema!", true);
                CarregarMenusHierarquicos(codSistemaEmpresa);
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao salvar vínculos: {ex.Message}", false);
            }
        }

        protected void btnLimpar_Click(object sender, EventArgs e)
        {
            foreach (ListItem item in chkMenus.Items)
            {
                item.Selected = false;
            }
            chkSelectAll.Checked = false;
        }

        private void MostrarMensagem(string mensagem, bool sucesso)
        {
            string script = $@"alert('{mensagem.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, GetType(), "showMessage", script, true);
        }
    }
}