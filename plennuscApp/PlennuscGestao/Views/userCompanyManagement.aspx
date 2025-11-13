<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="userCompanyManagement.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.userCompanyManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/Css/projects/gestao/structuresCss/user-company-management.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <div class="page-header">
            <h2 class="page-title">
                <div class="title-icon">
                    <i class="bi bi-people-fill"></i>
                </div>
                Gerenciar Empresas por Usuário
            </h2>
        </div>

        <div class="main-card">
            <div class="card-header">
                <h3 class="card-title">
                    <i class="bi bi-list-check"></i>
                    Usuários com Acesso ao Sistema
                </h3>
            </div>
            
            <div class="card-body">
                <div class="table-container">
                    <asp:GridView ID="gvUsuarios" runat="server" AutoGenerateColumns="false" CssClass="structure-grid"
                        DataKeyNames="CodPessoa,CodAutenticacaoAcesso" EmptyDataText="Nenhum usuário encontrado com acesso ao sistema.">
                        <Columns>
                            <asp:BoundField DataField="Nome" HeaderText="Nome" />
                            <asp:BoundField DataField="Sobrenome" HeaderText="Sobrenome" />
                            <asp:BoundField DataField="CPF" HeaderText="CPF" />
                            <asp:BoundField DataField="Email" HeaderText="E-mail" />
                            <asp:BoundField DataField="Login" HeaderText="Login" />
                            <asp:TemplateField HeaderText="Ações">
                                <ItemTemplate>
                                 <asp:LinkButton ID="btnVincular" runat="server" CssClass="btn-action"
    CommandName="Vincular"
    CommandArgument='<%# Eval("CodPessoa") + "," + Eval("CodAutenticacaoAcesso") %>'
    OnCommand="btnVincular_Command"
    ToolTip="Gerenciar Empresas">
    <i class="bi bi-building-gear"></i>
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
