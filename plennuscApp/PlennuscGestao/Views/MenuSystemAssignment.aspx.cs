using Plennusc.Core.Mappers.MappersGestao.helpers;
using Plennusc.Core.Models.ModelsGestao.modelsMenu;
using Plennusc.Core.Service.ServiceGestao.PlatformSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class MenuSystemAssignment : System.Web.UI.Page
    {
        private companyMenuManagementSystemService _service;
        private MenuSistemaConfig _config;
        private List<MenuModel> _todosMenus;

        protected void Page_Load(object sender, EventArgs e)
        {
            _service = new companyMenuManagementSystemService();
            _config = new MenuSistemaConfig();

            if (Session["CodUsuario"] == null)
            {
                Response.Redirect("~/SignIn.aspx");
                return;
            }

            if (!IsPostBack)
            {
                CarregarTodosMenus();
                AplicarFiltros();
            }
        }

        private void CarregarTodosMenus()
        {
            try
            {
                _todosMenus = _service.ListarTodosMenusParaConfiguracao();
                ViewState["TodosMenus"] = _todosMenus;
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro ao carregar menus: {ex.Message}");
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

        private void AplicarFiltros()
        {
            if (ViewState["TodosMenus"] != null)
            {
                _todosMenus = (List<MenuModel>)ViewState["TodosMenus"];
            }
            else
            {
                CarregarTodosMenus();
            }

            if (_todosMenus == null) return;

            // Aplicar filtro por sistema
            var menusFiltrados = _todosMenus.AsEnumerable();

            int filtroSistema = Convert.ToInt32(ddlFiltroSistema.SelectedValue);
            if (filtroSistema == -1) // Não definidos
            {
                menusFiltrados = menusFiltrados.Where(m =>
                    _config.GetSistemaDoMenu(m.CodMenu) == null ||
                    _config.GetSistemaDoMenu(m.CodMenu) == 0);
            }
            else if (filtroSistema > 0) // Sistema específico
            {
                menusFiltrados = menusFiltrados.Where(m =>
                    _config.GetSistemaDoMenu(m.CodMenu) == filtroSistema);
            }

            // Aplicar filtro por texto
            if (!string.IsNullOrEmpty(txtBuscarMenu.Text))
            {
                string busca = txtBuscarMenu.Text.ToLower();
                menusFiltrados = menusFiltrados.Where(m =>
                    (m.NomeDisplay?.ToLower().Contains(busca) == true) ||
                    (m.NomeMenu?.ToLower().Contains(busca) == true) ||
                    (m.NomeObjeto?.ToLower().Contains(busca) == true));
            }

            var listaFinal = menusFiltrados.ToList();

            rptMenus.DataSource = listaFinal;
            rptMenus.DataBind();

            litTotalMenus.Text = $"{listaFinal.Count} menus";
            pnlNenhumResultado.Visible = listaFinal.Count == 0;
        }

        protected void rptMenus_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                MenuModel menu = (MenuModel)e.Item.DataItem;

                HiddenField hfCodMenu = (HiddenField)e.Item.FindControl("hfCodMenu");
                hfCodMenu.Value = menu.CodMenu.ToString();

                Literal litSistemaAtual = (Literal)e.Item.FindControl("litSistemaAtual");
                DropDownList ddlSistemaNovo = (DropDownList)e.Item.FindControl("ddlSistemaNovo");

                // Obter sistema atual
                int? sistemaAtual = _config.GetSistemaDoMenu(menu.CodMenu);

                // Configurar literal com badge
                string sistemaTexto = "Não definido";
                string classeCss = "sistema-0";

                if (sistemaAtual.HasValue && sistemaAtual.Value > 0)
                {
                    switch (sistemaAtual.Value)
                    {
                        case 1: sistemaTexto = "Gestão"; classeCss = "sistema-1"; break;
                        case 2: sistemaTexto = "Finance"; classeCss = "sistema-2"; break;
                        case 3: sistemaTexto = "Medic"; classeCss = "sistema-3"; break;
                    }
                }

                litSistemaAtual.Text = $"<span class='sistema-badge {classeCss}'>{sistemaTexto}</span>";

                // Configurar dropdown com valor atual
                if (sistemaAtual.HasValue)
                {
                    ddlSistemaNovo.SelectedValue = sistemaAtual.Value.ToString();
                }
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
                        HiddenField hfCodMenu = (HiddenField)item.FindControl("hfCodMenu");
                        DropDownList ddlSistemaNovo = (DropDownList)item.FindControl("ddlSistemaNovo");

                        int codMenu = Convert.ToInt32(hfCodMenu.Value);
                        int novoSistema = Convert.ToInt32(ddlSistemaNovo.SelectedValue);

                        if (novoSistema == 0)
                        {
                            _config.RemoveConfig(codMenu);
                        }
                        else
                        {
                            _config.SetSistemaDoMenu(codMenu, novoSistema);
                        }
                    }
                }

                MostrarMensagemSucesso("Configurações salvas com sucesso!");

                // Recarregar para mostrar as alterações
                AplicarFiltros();
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro ao salvar configurações: {ex.Message}");
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
                    confirmButtonText: 'Fechar',
                    customClass: {{ confirmButton: 'btn btn-danger' }}
                }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "Erro", script, true);
        }
    }
}