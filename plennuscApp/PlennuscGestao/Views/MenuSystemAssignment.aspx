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
            <p class="page-subtitle">Defina a qual sistema cada menu pertence antes de vincular às empresas</p>
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
                <div class="row">
                    <div class="col-md-4">
                        <label class="form-label">Sistema</label>
                        <asp:DropDownList ID="ddlFiltroSistema" runat="server" CssClass="form-control" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlFiltroSistema_SelectedIndexChanged">
                            <asp:ListItem Text="Todos os sistemas" Value="0" />
                            <asp:ListItem Text="Gestão" Value="1" />
                            <asp:ListItem Text="Finance" Value="2" />
                            <asp:ListItem Text="Medic" Value="3" />
                            <asp:ListItem Text="Não definidos" Value="-1" />
                        </asp:DropDownList>
                    </div>
                    <div class="col-md-4">
                        <label class="form-label">Buscar menu</label>
                        <asp:TextBox ID="txtBuscarMenu" runat="server" CssClass="form-control" 
                            placeholder="Nome do menu..." AutoPostBack="true"
                            OnTextChanged="txtBuscarMenu_TextChanged"></asp:TextBox>
                    </div>
                </div>
            </div>
        </div>

        <!-- Grid de Menus -->
        <div class="main-card">
            <div class="card-header d-flex justify-content-between align-items-center">
                <h3 class="card-title">
                    <i class="bi bi-list-check"></i>
                    Menus Disponíveis
                    <small class="text-muted ms-2">
                        (<asp:Literal ID="litTotalMenus" runat="server" />)
                    </small>
                </h3>
                <asp:Button ID="btnSalvarTodos" runat="server" Text="Salvar Todas as Configurações" 
                    CssClass="btn btn-primary" OnClick="btnSalvarTodos_Click" />
            </div>
            <div class="card-body p-0">
                <asp:Repeater ID="rptMenus" runat="server" OnItemDataBound="rptMenus_ItemDataBound">
                    <HeaderTemplate>
                        <div class="menu-grid">
                            <div class="menu-item-row" style="background-color: #f8f9fa; font-weight: 500;">
                                <div class="row">
                                    <div class="col-1"><strong>Código</strong></div>
                                    <div class="col-4"><strong>Nome do Menu</strong></div>
                                    <div class="col-3"><strong>Objeto</strong></div>
                                    <div class="col-2"><strong>Sistema Atual</strong></div>
                                    <div class="col-2"><strong>Sistema Novo</strong></div>
                                </div>
                            </div>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <div class="menu-item-row">
                            <div class="row align-items-center">
                                <div class="col-1">
                                    <span class="menu-codigo"><%# Eval("CodMenu") %></span>
                                </div>
                                <div class="col-4">
                                    <div class="menu-nome"><%# Eval("NomeDisplay") %></div>
                                    <div class="menu-objeto"><%# Eval("NomeObjeto") %></div>
                                </div>
                                <div class="col-3">
                                    <div class="menu-objeto"><%# Eval("NomeObjeto") %></div>
                                </div>
                                <div class="col-2">
                                    <asp:Literal ID="litSistemaAtual" runat="server" />
                                </div>
                                <div class="col-2">
                                    <asp:DropDownList ID="ddlSistemaNovo" runat="server" CssClass="form-control form-control-sm">
                                        <asp:ListItem Text="-- Não definido --" Value="0" />
                                        <asp:ListItem Text="Gestão" Value="1" />
                                        <asp:ListItem Text="Finance" Value="2" />
                                        <asp:ListItem Text="Medic" Value="3" />
                                    </asp:DropDownList>
                                    <asp:HiddenField ID="hfCodMenu" runat="server" Value='<%# Eval("CodMenu") %>' />
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                    <FooterTemplate>
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
                
                <asp:Panel ID="pnlNenhumResultado" runat="server" Visible="false" CssClass="text-center py-5">
                    <i class="bi bi-search" style="font-size: 48px; color: #6c757d;"></i>
                    <h5 class="mt-3">Nenhum menu encontrado</h5>
                    <p class="text-muted">Tente ajustar os filtros de busca</p>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>