<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="menusManagement.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.menusManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="../../Content/Css/projects/gestao/structuresCss/menu/Menus-Management.css" rel="stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <div class="container-main menu-management-container">
        <div class="page-header">
            <h2 class="page-title">
                <div class="title-icon">
                    <i class="bi bi-list-check"></i>
                </div>
                Gerenciar Menus do Sistema
            </h2>
        </div>

        <!-- Card de Cadastro/Edição -->
        <div class="main-card menu-form">
            <div class="card-header">
                <h3 class="card-title">
                    <i class="bi bi-plus-circle"></i>
                    <asp:Label ID="lblFormTitle" runat="server" Text="Cadastrar Novo Menu"></asp:Label>
                </h3>
            </div>
            
            <div class="card-body">
                <div class="row">
                    <!-- Coluna 1 -->
                    <div class="col-md-6">
                        <div class="mb-3">
                            <label class="form-label">Nome do Menu *</label>
                            <asp:TextBox ID="txtNomeMenu" runat="server" CssClass="form-control" MaxLength="60" 
                                placeholder="Ex: Gestão de Pessoas" />
                            <asp:RequiredFieldValidator ID="rfvNomeMenu" runat="server" ControlToValidate="txtNomeMenu"
                                ErrorMessage="Nome do menu é obrigatório" CssClass="text-danger small" Display="Dynamic" />
                        </div>
                        
                        <div class="mb-3">
                            <label class="form-label">Nome para Display *</label>
                            <asp:TextBox ID="txtNomeDisplay" runat="server" CssClass="form-control" MaxLength="40" 
                                placeholder="Ex: Pessoas" />
                            <asp:RequiredFieldValidator ID="rfvNomeDisplay" runat="server" ControlToValidate="txtNomeDisplay"
                                ErrorMessage="Nome para display é obrigatório" CssClass="text-danger small" Display="Dynamic" />
                        </div>
                        
                        <div class="mb-3">
                            <label class="form-label">Nome do Objeto *</label>
                            <asp:TextBox ID="txtNomeObjeto" runat="server" CssClass="form-control" MaxLength="60" 
                                placeholder="Ex: MenuPessoas" />
                            <asp:RequiredFieldValidator ID="rfvNomeObjeto" runat="server" ControlToValidate="txtNomeObjeto"
                                ErrorMessage="Nome do objeto é obrigatório" CssClass="text-danger small" Display="Dynamic" />
                        </div>
                    </div>
                    
                    <!-- Coluna 2 -->
                    <div class="col-md-6">
                        <div class="mb-3">
                            <label class="form-label">Caption do Objeto *</label>
                            <asp:TextBox ID="txtCaptionObjeto" runat="server" CssClass="form-control" MaxLength="40" 
                                placeholder="Ex: Gestão de Pessoas" />
                            <asp:RequiredFieldValidator ID="rfvCaptionObjeto" runat="server" ControlToValidate="txtCaptionObjeto"
                                ErrorMessage="Caption do objeto é obrigatório" CssClass="text-danger small" Display="Dynamic" />
                        </div>
                        
                        <div class="mb-3">
                            <label class="form-label">Rota (HttpRouter)</label>
                            <asp:TextBox ID="txtHttpRouter" runat="server" CssClass="form-control" MaxLength="200" 
                                placeholder="Ex: /gestao/pessoas" />
                        </div>
                        
                        <div class="mb-3">
                            <label class="form-label">Menu Pai</label>
                            <asp:DropDownList ID="ddlMenuPai" runat="server" CssClass="form-control">
                                <asp:ListItem Value="">-- Menu Principal (Nível 1) --</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
                
                <div class="row">
                    <div class="col-md-4">
                        <div class="mb-3">
                            <label class="form-label">Ordem *</label>
                            <asp:TextBox ID="txtOrdem" runat="server" CssClass="form-control" TextMode="Number" 
                                min="0" max="999" Text="0" />
                            <asp:RequiredFieldValidator ID="rfvOrdem" runat="server" ControlToValidate="txtOrdem"
                                ErrorMessage="Ordem é obrigatória" CssClass="text-danger small" Display="Dynamic" />
                        </div>
                    </div>
                    
                    <div class="col-md-4">
                        <div class="mb-3">
                            <label class="form-label">Nível *</label>
                            <asp:TextBox ID="txtNivel" runat="server" CssClass="form-control" TextMode="Number" 
                                min="1" max="3" Text="1" />
                            <asp:RequiredFieldValidator ID="rfvNivel" runat="server" ControlToValidate="txtNivel"
                                ErrorMessage="Nível é obrigatório" CssClass="text-danger small" Display="Dynamic" />
                            <small class="form-text text-muted">1=Principal, 2=Submenu, 3=Item</small>
                        </div>
                    </div>
                    
                    <div class="col-md-4">
                        <div class="mb-3">
                            <label class="form-label">Status</label>
                            <div class="form-check mt-2">
                                <asp:CheckBox ID="chkHabilitado" runat="server" CssClass="form-check-input" Checked="true" />
                                <label class="form-check-label" for="<%= chkHabilitado.ClientID %>">
                                    Menu Habilitado
                                </label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            
            <div class="card-footer">
                <asp:Button ID="btnSalvar" runat="server" Text="Salvar Menu" CssClass="btn btn-primary" 
                    OnClick="btnSalvar_Click" />
                <asp:Button ID="btnCancelar" runat="server" Text="Cancelar" CssClass="btn btn-secondary" 
                    OnClick="btnCancelar_Click" Visible="false" />
                <asp:HiddenField ID="hfCodMenuEdicao" runat="server" Value="" />
            </div>
        </div>

        <!-- Card de Listagem -->
        <div class="main-card menu-grid">
            <div class="card-header">
                <h3 class="card-title">
                    <i class="bi bi-list-ul"></i>
                    Menus Cadastrados
                </h3>
            </div>
            
            <div class="card-body">
                <div class="table-container">
                    <asp:GridView ID="gvMenus" runat="server" AutoGenerateColumns="false" CssClass="structure-grid"
                        DataKeyNames="CodMenu" EmptyDataText="Nenhum menu cadastrado."
                        OnRowCommand="gvMenus_RowCommand" OnRowDataBound="gvMenus_RowDataBound">
                        <Columns>
                            <asp:BoundField DataField="CodMenu" HeaderText="Código" ItemStyle-Width="80px" />
                            <asp:TemplateField HeaderText="Menu">
                                <ItemTemplate>
                                    <div class='<%# "hierarchy-level-" + Eval("Conf_Nivel") %>'>
                                       <i class='<%# "bi bi-folder" %> me-2'></i>
                                        <%# Eval("NomeDisplay") %>
                                        <small class="text-muted d-block"><%# Eval("HttpRouter") %></small>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="NomeMenu" HeaderText="Nome Interno" />
                            <asp:BoundField DataField="Conf_Ordem" HeaderText="Ordem" ItemStyle-Width="80px" />
                            <asp:BoundField DataField="Conf_Nivel" HeaderText="Nível" ItemStyle-Width="80px" />
                            <asp:TemplateField HeaderText="Status" ItemStyle-Width="100px">
                                <ItemTemplate>
                                    <span class='badge <%# (bool)Eval("Conf_Habilitado") ? "bg-success" : "bg-secondary" %>'>
                                        <%# (bool)Eval("Conf_Habilitado") ? "Ativo" : "Inativo" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Ações" ItemStyle-Width="120px" ItemStyle-CssClass="actions-column">
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn-action text-primary"
                                        CommandName="Editar" CommandArgument='<%# Eval("CodMenu") %>'
                                        ToolTip="Editar Menu">
                                        <i class="bi bi-pencil"></i>
                                    </asp:LinkButton>
                                    
                                    <asp:LinkButton ID="btnExcluir" runat="server" CssClass="btn-action text-danger"
                                        CommandName="Excluir" CommandArgument='<%# Eval("CodMenu") %>'
                                        ToolTip="Excluir Menu" 
                                        OnClientClick="return confirm('Tem certeza que deseja excluir este menu?');">
                                        <i class="bi bi-trash"></i>
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
