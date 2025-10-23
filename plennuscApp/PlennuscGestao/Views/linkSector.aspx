<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="linkSector.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.linkSector" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <title>Vincular Setores às Estruturas</title>
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
                                    <asp:BoundField DataField="DescEstrutura" HeaderText="Nome da Estrutura" />
                                    <asp:BoundField DataField="ValorPadrao" HeaderText="Ordem" />
                                   
                                    <asp:TemplateField HeaderText="Setores Vinculados">
                                        <ItemTemplate>
                                            <small>Vincule um setor ao lado →</small>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Vincular Novo Setor">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlSetor" runat="server" CssClass="form-control form-select">
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                      <asp:TemplateField HeaderText="Ações">
                                    <ItemTemplate>
                                        <asp:Button ID="btnVincularSetor" runat="server" CssClass="btn btn-success btn-sm"
                                            Text="Vincular" OnClick="btnVincularSetor_Click" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>
