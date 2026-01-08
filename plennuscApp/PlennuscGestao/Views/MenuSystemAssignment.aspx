<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="MenuSystemAssignment.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.MenuSystemAssignment" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="../../Content/Css/projects/gestao/structuresCss/menu/Munu-System-Assignment.css" rel="stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <div class="page-header">
            <h2 class="page-title">
                <div class="title-icon">
                    <i class="bi bi-link-45deg"></i>
                </div>
                Configurar Sistema dos Menus
            </h2>
            <p class="page-subtitle">Marque TODOS os sistemas onde cada menu deve estar disponível (configuração global)</p>
        </div>

        <!-- Filtros -->
        <div class="main-card mb-4">
            <div class="card-header">
                <h3 class="card-title">
                    <i class="bi bi-funnel"></i>
                    Filtros
                </h3>
            </div>
            <div class="card-body">
                <div class="filter-row">
                    <div class="filter-group">
                        <label class="form-label">Status</label>
                        <asp:DropDownList ID="ddlFiltroSistema" runat="server" CssClass="form-control" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlFiltroSistema_SelectedIndexChanged">
                            <asp:ListItem Text="Todos" Value="0" />
                            <asp:ListItem Text="Apenas não vinculados" Value="-1" />
                            <asp:ListItem Text="Gestão" Value="1" />
                            <asp:ListItem Text="Finance" Value="2" />
                            <asp:ListItem Text="Medic" Value="3" />
                        </asp:DropDownList>
                    </div>
                <div class="filter-group">
    <label class="form-label">Buscar menu</label>
    <div id="buscaMenuPaginaAtual" class="search-container">
        <asp:TextBox ID="txtBuscarMenu" runat="server" CssClass="form-control" 
            placeholder="Digite nome ou objeto do menu..." AutoPostBack="true"
            OnTextChanged="txtBuscarMenu_TextChanged"></asp:TextBox>
        <span class="search-icon">
            <i class="bi bi-search"></i>
        </span>
    </div>
</div>
                </div>
            </div>
        </div>

        <!-- Grid de Menus -->
        <div class="main-card">
            <div class="card-header">
                <div>
                    <h3 class="card-title">
                        <i class="bi bi-list-check"></i>
                        Menus Disponíveis
                        <small class="text-muted ms-2">
                            (<asp:Literal ID="litTotalMenus" runat="server" /> menus)
                        </small>
                    </h3>
                    <p class="text-muted mb-0">Marque as caixas para definir em quais sistemas cada menu estará disponível</p>
                </div>
                <asp:Button ID="btnSalvarTodos" runat="server" Text="Salvar Todas as Configurações" 
                    CssClass="btn btn-salvar-todos" OnClick="btnSalvarTodos_Click" />
            </div>
            
            <div class="card-body">
                <!-- Cabeçalho da Grid -->
                <div class="grid-header">
                    <div class="row">
                        <div class="text-center">Código</div>
                        <div>Nome do Menu</div>
                        <div>Objeto</div>
                        <div class="text-center">Gestão</div>
                        <div class="text-center">Finance</div>
                        <div class="text-center">Medic</div>
                        <div class="text-center">Ouvidoria</div>
                        <div class="text-center">Ações</div>
                    </div>
                </div>

                <!-- Grid de Menus -->
                <asp:Repeater ID="rptMenus" runat="server" OnItemDataBound="rptMenus_ItemDataBound">
                    <ItemTemplate>
                        <div class="menu-item-row">
                            <div class="row">
                                <div class="text-center">
                                    <span class="menu-codigo"><%# Eval("CodMenu") %></span>
                                </div>
                                <div>
                                    <div class="menu-nome"><%# Eval("NomeDisplay") %></div>
                                    <div class="menu-objeto"><%# Eval("NomeObjeto") %></div>
                                </div>
                                <div>
                                    <div class="menu-objeto"><%# Eval("NomeObjeto") %></div>
                                </div>
                                <div class="system-checkbox-container">
                                    <asp:CheckBox ID="cbGestao" runat="server" CssClass="system-checkbox" data-system="1" />
                                </div>
                                <div class="system-checkbox-container">
                                    <asp:CheckBox ID="cbFinance" runat="server" CssClass="system-checkbox" data-system="2" />
                                </div>
                                <div class="system-checkbox-container">
                                    <asp:CheckBox ID="cbMedic" runat="server" CssClass="system-checkbox" data-system="3" />
                                </div>
                                <div class="system-checkbox-container">
                                    <asp:CheckBox ID="cbOuvidoria" runat="server" CssClass="system-checkbox" data-system="4" />
                                </div>
                                <div class="action-buttons">
                                    <asp:Button ID="btnSalvarMenu" runat="server" Text="Salvar" CssClass="btn btn-primary btn-sm" 
                                        CommandArgument='<%# Eval("CodMenu") %>' OnClick="btnSalvarMenu_Click" />
                                    <asp:HiddenField ID="hfCodMenu" runat="server" Value='<%# Eval("CodMenu") %>' />
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <!-- Sem resultados -->
                <asp:Panel ID="pnlNenhumResultado" runat="server" Visible="false" CssClass="no-results">
                    <div class="no-results-icon">
                        <i class="bi bi-search"></i>
                    </div>
                    <h5>Nenhum menu encontrado</h5>
                    <p class="text-muted">Tente ajustar os filtros de busca</p>
                </asp:Panel>
            </div>
        </div>

        <!-- Aviso informativo -->
        <div class="aviso-info">
            <div class="aviso-content">
                
                <div>
                    <i class="bi bi-info-circle"></i>
                    <strong>Nota:</strong> Esta configuração é global. Os menus configurados aqui estarão disponíveis para todas as empresas.
                    Após configurar aqui, você poderá selecionar quais menus serão ativados para cada empresa específica.
                </div>
            </div>
        </div>
    </div>
</asp:Content>