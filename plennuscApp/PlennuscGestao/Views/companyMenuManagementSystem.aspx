<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="companyMenuManagementSystem.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.companyMenuManagementSystem" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/Css/projects/gestao/structuresCss/sistema/Company-Menu-Management-System.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <div class="page-header">
            <h2 class="page-title">
                <div class="title-icon">
                    <i class="bi bi-link"></i>
                </div>
                Vincular Menus aos Sistemas × Empresa
            </h2>
        </div>

        <!-- Card de Seleção -->
        <div class="main-card">
            <div class="card-header">
                <h3 class="card-title">
                    <i class="bi bi-building-gear"></i>
                    Selecionar Empresa e Sistema
                </h3>
                 
                      <asp:Button ID="btnSalvar" runat="server" Text="Salvar Vínculos" CssClass="btn btn-primary" 
                          OnClick="btnSalvar_Click" />
                   <%--   <asp:Button ID="btnLimpar" runat="server" Text="Limpar Seleção" CssClass="btn btn-secondary" 
                          OnClick="btnLimpar_Click" />--%>
            </div>
            <div class="card-body">
                <div class="row">
                    <div class="col-md-6">
                        <div class="mb-3">
                            <label class="form-label">Empresa</label>
                            <asp:DropDownList ID="ddlEmpresa" runat="server" CssClass="form-control" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlEmpresa_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div class="mb-3">
                            <label class="form-label">Sistema</label>
                            <asp:DropDownList ID="ddlSistema" runat="server" CssClass="form-control" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlSistema_SelectedIndexChanged">
                            </asp:DropDownList>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Card de Menus Agrupados por Módulo -->
        <asp:Panel ID="pnlMenus" runat="server" Visible="false">
            <div class="main-card mt-4">
                <div class="card-header">
                    <h3 class="card-title">
                        <i class="bi bi-list-check"></i>
                        Menus Disponíveis por Módulo
                        <small class="text-muted ms-2">
                            (<asp:Literal ID="litInfoSistema" runat="server" />)
                        </small>
                    </h3>
                </div>
                <div class="card-body">
                 <%--   <div class="mb-3">
                        <asp:CheckBox ID="chkSelectAll" runat="server" AutoPostBack="true" 
                            OnCheckedChanged="chkSelectAll_CheckedChanged" Text=" Selecionar Todos" />
                    </div>--%>
                    
                    <div class="modules-container" id="modulesContainer" runat="server">
                        <!-- Os módulos serão gerados dinamicamente no code-behind -->
                    </div>
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>