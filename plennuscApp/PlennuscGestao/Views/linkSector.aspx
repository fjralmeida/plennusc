<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" AutoEventWireup="true" CodeBehind="linkSector.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.linkSector" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
          <title>Vincular Setores às Estruturas</title>

    <link href="../../Content/Css/projects/gestao/structuresCss/link-Sector.css" rel="stylesheet" />
    <script src="../../Content/Css/js/projects/gestaoJs/structuresJs/linkSector.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <div class="page-header">
            <div class="page-title">
                <div class="title-icon"><i class="bi bi-link-45deg"></i></div>
                Vincular Setores às Estruturas
            </div>
        </div>

        <div class="main-card">
            <div class="card-header">
                <h2 class="card-title"><i class="bi bi-diagram-3"></i> Gerenciar Vínculos</h2>
            </div>
            
            <div class="card-body">
                <div class="form-group">
                    <label class="form-label">Selecione uma View *</label>
                    <asp:DropDownList ID="ddlView" runat="server" CssClass="form-control form-select" AutoPostBack="true"
                        OnSelectedIndexChanged="ddlView_SelectedIndexChanged">
                        <asp:ListItem Text="Selecione uma View" Value=""></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <asp:Panel ID="pnlVincular" runat="server" Visible="false" CssClass="mt-4">
                    <div class="card">
                        <div class="card-header">
                            <h5 class="card-title"><i class="bi bi-link"></i> Vincular Setor à View: 
                                <asp:Label ID="lblViewSelecionada" runat="server" Font-Bold="true" />
                            </h5>
                        </div>
                        <div class="card-body">
                            <div class="form-group">
                                <label class="form-label">Setor</label>
                                <asp:DropDownList ID="ddlSetor" runat="server" CssClass="form-control form-select">
                                    <asp:ListItem Text="-- Selecione --" Value="0" />
                                </asp:DropDownList>
                            </div>
                            <asp:Button ID="btnVincular" runat="server" Text="Vincular Setor" 
                                CssClass="btn btn-success mt-2" OnClick="btnVincular_Click" />
                        </div>
                    </div>

                    <div class="card mt-3">
                        <div class="card-header"><h5 class="card-title"><i class="bi bi-list-ul"></i> Estruturas nesta View</h5></div>
                        <div class="card-body">
                            <asp:GridView ID="gvEstruturas" runat="server" CssClass="table table-striped table-bordered" 
                                AutoGenerateColumns="false" EmptyDataText="Nenhuma estrutura encontrada">
                                <Columns>
                                    <asp:BoundField DataField="DescEstrutura" HeaderText="Nome da Estrutura" />
                                    <asp:BoundField DataField="ValorPadrao" HeaderText="Ordem" />
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>
</asp:Content>