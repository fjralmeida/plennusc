<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="linkSector.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.linkSector" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
          <title>Vincular Setores às Estruturas</title>

    <link href="../../Content/Css/projects/gestao/structuresCss/link-Sector.css" rel="stylesheet" />
    <script src="../../Content/Css/js/projects/gestaoJs/structuresJs/linkSector.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <div class="page-header">
            <div class="page-title">
                <div class="title-icon">
                    <i class="bi bi-link-45deg"></i>
                </div>
                Vincular Setores às Estruturas
            </div>
        </div>

        <div class="main-card">
            <div class="card-header">
                <h2 class="card-title">
                    <i class="bi bi-diagram-3"></i>
                    Gerenciar Vínculos
                </h2>
            </div>
            
            <div class="card-body">
                <!-- COMBO COM AS VIEWS -->
                <div class="form-group">
                    <label class="form-label">Selecione uma View *</label>
                    <asp:DropDownList ID="ddlView" runat="server" CssClass="form-control form-select" AutoPostBack="true"
                        OnSelectedIndexChanged="ddlView_SelectedIndexChanged">
                        <asp:ListItem Text="Selecione uma View" Value=""></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <!-- GRID COM ESTRUTURAS DA VIEW SELECIONADA -->
                <asp:Panel ID="pnlEstruturas" runat="server" Visible="false" class="mt-4">
                    <div class="card">
                        <div class="card-header">
                            <h5 class="card-title">
                                <i class="bi bi-list-ul"></i>
                                Estruturas da View: <asp:Label ID="lblViewSelecionada" runat="server" Text=""></asp:Label>
                            </h5>
                        </div>
                        <div class="card-body">
                            <asp:GridView ID="gvEstruturas" runat="server" CssClass="table table-striped table-bordered" 
                                AutoGenerateColumns="false" EmptyDataText="Nenhuma estrutura encontrada para esta View"
                                OnRowDataBound="gvEstruturas_RowDataBound">
                                <Columns>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCodEstrutura" runat="server" Text='<%# Eval("CodEstrutura") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="DescEstrutura" HeaderText="Nome da Estrutura" 
                                        ItemStyle-CssClass="column-text" HeaderStyle-CssClass="column-text" />
                                    
                                    <asp:BoundField DataField="ValorPadrao" HeaderText="Ordem" 
                                        ItemStyle-CssClass="column-number" HeaderStyle-CssClass="column-number" />
                                   
                                    <asp:TemplateField HeaderText="Setores Vinculados" 
                                        ItemStyle-CssClass="column-setores" HeaderStyle-CssClass="column-setores">
                                        <ItemTemplate>
                                            <small>Vincule um setor ao lado →</small>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Vincular Novo Setor" 
                                        ItemStyle-CssClass="column-vincular" HeaderStyle-CssClass="column-vincular">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlSetor" runat="server" CssClass="form-control form-select">
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Ações" 
                                        ItemStyle-CssClass="column-actions" HeaderStyle-CssClass="column-actions">
                                        <ItemTemplate>
                                            <asp:Button ID="btnVincularSetor" runat="server" CssClass="btn btn-success btn-sm"
                                                Text="Vincular" OnClick="btnVincularSetor_Click" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div class="empty-data">
                                        <i class="bi bi-inbox"></i>
                                        Nenhuma estrutura encontrada para esta View
                                    </div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>
