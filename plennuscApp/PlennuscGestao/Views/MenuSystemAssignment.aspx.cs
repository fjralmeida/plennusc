using Plennusc.Core.Mappers.MappersGestao.helpers;
using Plennusc.Core.Models.ModelsGestao.modelsMenu;
using Plennusc.Core.Service.ServiceGestao.menu;
using Plennusc.Core.Service.ServiceGestao.PlatformSys;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class MenuSystemAssignment : System.Web.UI.Page
    {
        private MenuSystemService _service;

        protected void Page_Load(object sender, EventArgs e)
        {
            _service = new MenuSystemService();

            if (Session["CodUsuario"] == null)
            {
                Response.Redirect("~/SignIn.aspx");
                return;
            }

            if (!IsPostBack)
            {
                AplicarFiltros();
            }
        }

        private void AplicarFiltros()
        {
            try
            {
                var todosMenus = _service.ListarTodosMenusParaConfiguracao();
                if (todosMenus == null) return;

                int filtroSistema = Convert.ToInt32(ddlFiltroSistema.SelectedValue);

                // Aplicar filtros existentes
                List<MenuModel> menusFiltrados;

                if (filtroSistema == -1) // Apenas não vinculados
                {
                    menusFiltrados = todosMenus.Where(m => !_service.EstaVinculadoEmAlgumSistema(m.CodMenu)).ToList();
                }
                else if (filtroSistema > 0) // Sistema específico
                {
                    menusFiltrados = todosMenus.Where(m => _service.EstaVinculadoAoSistema(m.CodMenu, filtroSistema)).ToList();
                }
                else // Todos (valor 0)
                {
                    menusFiltrados = todosMenus.ToList();
                }

                // Aplicar filtro de busca se existir
                if (!string.IsNullOrEmpty(txtBuscarMenu.Text))
                {
                    string busca = txtBuscarMenu.Text.ToLower();
                    menusFiltrados = menusFiltrados.Where(m =>
                        (m.NomeDisplay?.ToLower().Contains(busca) == true) ||
                        (m.NomeMenu?.ToLower().Contains(busca) == true) ||
                        (m.NomeObjeto?.ToLower().Contains(busca) == true)).ToList();
                }

                // A NOVA LÓGICA: Separar menus não vinculados e ordenar
                var menusNaoVinculados = menusFiltrados
                    .Where(m => !_service.EstaVinculadoEmAlgumSistema(m.CodMenu))
                    .OrderBy(m => m.CodMenuPai ?? m.CodMenu)
                    .ThenBy(m => m.Conf_Ordem)
                    .ToList();

                var menusVinculados = menusFiltrados
                    .Where(m => _service.EstaVinculadoEmAlgumSistema(m.CodMenu))
                    .OrderBy(m => m.CodMenuPai ?? m.CodMenu)
                    .ThenBy(m => m.Conf_Ordem)
                    .ToList();

                // Juntar os menus: primeiro os não vinculados, depois os vinculados
                var menusOrdenados = new List<MenuModel>();
                menusOrdenados.AddRange(menusNaoVinculados);
                menusOrdenados.AddRange(menusVinculados);

                // Se não estamos no filtro "Apenas não vinculados" e há menus não vinculados,
                // vamos adicionar um separador visual
                if (filtroSistema != -1 && menusNaoVinculados.Any() && menusVinculados.Any())
                {
                    // Adicionar um item especial para servir como separador
                    // Mas como o Repeater não tem separador nativo, vamos adicionar uma propriedade
                    // para marcar no front-end
                    ViewState["MostrarSeparador"] = true;
                    ViewState["PosicaoSeparador"] = menusNaoVinculados.Count;
                }
                else
                {
                    ViewState["MostrarSeparador"] = false;
                }

                rptMenus.DataSource = menusOrdenados;
                rptMenus.DataBind();
                litTotalMenus.Text = $"{menusOrdenados.Count} menus";
                pnlNenhumResultado.Visible = menusOrdenados.Count == 0;
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro: {ex.Message}");
            }
        }

        protected void ddlFiltroSistema_SelectedIndexChanged(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        protected void txtBuscarMenu_TextChanged(object sender, EventArgs e)
        {
            AplicarFiltros();
        }

        protected void rptMenus_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var menu = (MenuModel)e.Item.DataItem;
                var hfCodMenu = (HiddenField)e.Item.FindControl("hfCodMenu");
                hfCodMenu.Value = menu.CodMenu.ToString();

                var cbGestao = (CheckBox)e.Item.FindControl("cbGestao");
                var cbFinance = (CheckBox)e.Item.FindControl("cbFinance");
                var cbMedic = (CheckBox)e.Item.FindControl("cbMedic");
                var cbOuvidoria = (CheckBox)e.Item.FindControl("cbOuvidoria");

                var sistemasVinculados = _service.GetSistemasVinculados(menu.CodMenu);

                cbGestao.Checked = sistemasVinculados.Contains(1);
                cbFinance.Checked = sistemasVinculados.Contains(2);
                cbMedic.Checked = sistemasVinculados.Contains(3);
                cbOuvidoria.Checked = sistemasVinculados.Contains(4);

                // ADICIONAR CLASSE PARA MENUS NÃO VINCULADOS
                bool estaVinculado = _service.EstaVinculadoEmAlgumSistema(menu.CodMenu);
                var menuItemRow = (HtmlGenericControl)e.Item.FindControl("menuItemRow") ??
                                 e.Item.FindControl("menu-item-row") as HtmlGenericControl;

                if (menuItemRow != null && !estaVinculado)
                {
                    // Por esta linha:
                    menuItemRow.Attributes["class"] = (menuItemRow.Attributes["class"] ?? "") + " menu-nao-vinculado";
                }

                // ADICIONAR SEPARADOR VISUAL
                bool mostrarSeparador = ViewState["MostrarSeparador"] != null && (bool)ViewState["MostrarSeparador"];
                int posicaoSeparador = ViewState["PosicaoSeparador"] != null ? (int)ViewState["PosicaoSeparador"] : -1;

                if (mostrarSeparador && e.Item.ItemIndex == posicaoSeparador)
                {
                    // Adicionar uma linha separadora antes do primeiro menu vinculado
                    var separador = new LiteralControl("<div class='separador-menus'><div class='separador-texto'>Menus Vinculados</div></div>");
                    e.Item.Controls.AddAt(0, separador);
                }
            }
        }

        protected void btnSalvarMenu_Click(object sender, EventArgs e)
        {
            try
            {
                var btn = (Button)sender;
                int codMenu = Convert.ToInt32(btn.CommandArgument);
                var item = (RepeaterItem)btn.NamingContainer;

                var cbGestao = (CheckBox)item.FindControl("cbGestao");
                var cbFinance = (CheckBox)item.FindControl("cbFinance");
                var cbMedic = (CheckBox)item.FindControl("cbMedic");
                var cbOuvidoria = (CheckBox)item.FindControl("cbOuvidoria");

                _service.SalvarConfiguracoesMenu(codMenu, cbGestao.Checked, cbFinance.Checked, cbMedic.Checked, cbOuvidoria.Checked);
                MostrarMensagemSucesso("Menu salvo!");
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro: {ex.Message}");
            }
        }

        protected void btnSalvarTodos_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (RepeaterItem item in rptMenus.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        var hfCodMenu = (HiddenField)item.FindControl("hfCodMenu");
                        var cbGestao = (CheckBox)item.FindControl("cbGestao");
                        var cbFinance = (CheckBox)item.FindControl("cbFinance");
                        var cbMedic = (CheckBox)item.FindControl("cbMedic");
                        var cbOuvidoria = (CheckBox)item.FindControl("cbOuvidoria");

                        int codMenu = Convert.ToInt32(hfCodMenu.Value);
                        _service.SalvarConfiguracoesMenu(codMenu, cbGestao.Checked, cbFinance.Checked, cbMedic.Checked, cbOuvidoria.Checked);
                    }
                }
                MostrarMensagemSucesso("Todos os menus salvos!");
                AplicarFiltros();
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro: {ex.Message}");
            }
        }

        private void MostrarMensagemSucesso(string mensagem)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "Sucesso",
                $"Swal.fire({{icon:'success',title:'Sucesso!',text:'{mensagem.Replace("'", "\\'")}'}});", true);
        }

        private void MostrarMensagemErro(string mensagem)
        {
            ScriptManager.RegisterStartupScript(this, GetType(), "Erro",
                $"Swal.fire({{icon:'error',title:'Erro!',text:'{mensagem.Replace("'", "\\'")}'}});", true);
        }
    }
}