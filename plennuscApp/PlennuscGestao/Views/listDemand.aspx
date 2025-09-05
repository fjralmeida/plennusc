<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="listDemand.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.listDemand" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <div class="container-fluid py-3">

    <!-- Header -->
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h2 class="mb-0">📋 Demandas</h2>
      <asp:Button ID="btnNovaDemanda" runat="server" CssClass="btn btn-primary"
                  Text="+ Nova Demanda" OnClick="btnNovaDemanda_Click" />
    </div>

    <!-- Filtros -->
    <div class="card mb-3 shadow-sm">
      <div class="card-body row g-3 align-items-end">

        <div class="col-md-3">
          <label for="ddlVisibilidade" class="form-label">Visibilidade</label>
          <asp:DropDownList ID="ddlVisibilidade" runat="server" CssClass="form-select">
            <asp:ListItem Value="T" Text="Todas"></asp:ListItem>
            <asp:ListItem Value="M" Text="Minhas"></asp:ListItem>
            <asp:ListItem Value="S" Text="Do meu setor"></asp:ListItem>
          </asp:DropDownList>
        </div>

        <div class="col-md-3">
          <label for="ddlStatus" class="form-label">Status</label>
          <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select"></asp:DropDownList>
        </div>

        <div class="col-md-3">
          <label for="ddlCategoria" class="form-label">Categoria</label>
          <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-select"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlCategoria_SelectedIndexChanged"></asp:DropDownList>
        </div>

        <div class="col-md-3">
          <label for="ddlSubtipo" class="form-label">Subtipo</label>
          <asp:DropDownList ID="ddlSubtipo" runat="server" CssClass="form-select"></asp:DropDownList>
        </div>

        <div class="col-md-3">
          <label for="txtSolicitante" class="form-label">Solicitante</label>
          <asp:TextBox ID="txtSolicitante" runat="server" CssClass="form-control"></asp:TextBox>
        </div>

        <div class="col-md-2">
          <asp:Button ID="btnFiltrar" runat="server" CssClass="btn btn-secondary w-100"
                      Text="Filtrar" OnClick="btnFiltrar_Click" />
        </div>
      </div>
    </div>

    <!-- Grid -->
    <asp:GridView ID="gvDemandas" runat="server" CssClass="table table-hover table-striped"
                  AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
                  OnPageIndexChanging="gvDemandas_PageIndexChanging"
                  OnRowCommand="gvDemandas_RowCommand">

      <Columns>
        <asp:BoundField DataField="CodDemanda" HeaderText="#" />
        <asp:BoundField DataField="Titulo" HeaderText="Título" />
        <asp:BoundField DataField="Categoria" HeaderText="Categoria" />
        <asp:BoundField DataField="Subtipo" HeaderText="Subtipo" />
        <asp:BoundField DataField="Status" HeaderText="Status" />
        <asp:BoundField DataField="Solicitante" HeaderText="Solicitante" />
        <asp:BoundField DataField="DataSolicitacao" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}" />
        <asp:TemplateField HeaderText="Ações">
          <ItemTemplate>
            <asp:LinkButton ID="lnkVer" runat="server" CssClass="btn btn-sm btn-outline-primary"
                            CommandName="Ver" CommandArgument='<%# Eval("CodDemanda") %>'>👁 Ver</asp:LinkButton>
          </ItemTemplate>
        </asp:TemplateField>
      </Columns>
    </asp:GridView>

  </div>
</asp:Content>
