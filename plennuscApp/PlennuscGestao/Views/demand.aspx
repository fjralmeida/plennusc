<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="demand.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.demand" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <div class="container-fluid p-3">
    <div class="card shadow-sm">
      <div class="card-header d-flex justify-content-between align-items-center">
        <h5 class="mb-0">Criar Demanda</h5>
        <asp:Button ID="btnSalvar" runat="server" CssClass="btn btn-success" Text="Salvar" OnClick="btnSalvar_Click" />
      </div>

      <div class="card-body">
        <asp:Label ID="lblMsg" runat="server" CssClass="text-danger d-block mb-3"></asp:Label>

        <div class="row g-3">
          <div class="col-md-6">
            <label class="form-label">Título *</label>
            <asp:TextBox ID="txtTitulo" runat="server" CssClass="form-control" MaxLength="100" />
          </div>

          <div class="col-md-3">
            <label class="form-label">Prioridade *</label>
            <asp:DropDownList ID="ddlPrioridade" runat="server" CssClass="form-select" />
          </div>

          <div class="col-md-3">
            <label class="form-label">Prazo</label>
            <asp:TextBox ID="txtPrazo" runat="server" CssClass="form-control" TextMode="Date" />
          </div>

          <div class="col-md-6">
            <label class="form-label">Setor de Origem *</label>
            <asp:DropDownList ID="ddlOrigem" runat="server" CssClass="form-select" />
          </div>

          <div class="col-md-6">
            <label class="form-label">Setor de Destino *</label>
            <asp:DropDownList ID="ddlDestino" runat="server" CssClass="form-select" />
          </div>

          <div class="col-md-6">
            <label class="form-label">Tipo de Demanda *</label>
            <asp:DropDownList ID="ddlTipo" runat="server" CssClass="form-select" />
          </div>

          <div class="col-md-6">
            <div class="form-check mt-4">
              <asp:CheckBox ID="chkAprova" runat="server" CssClass="form-check-input" />
              <label class="form-check-label ms-2">Requer aprovação</label>
            </div>
            <div class="mt-2">
              <label class="form-label">Aprovador (opcional)</label>
              <asp:DropDownList ID="ddlAprovador" runat="server" CssClass="form-select" />
            </div>
          </div>

          <div class="col-12">
            <label class="form-label">Descrição *</label>
            <asp:TextBox ID="txtDescricao" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="6" />
          </div>
        </div>
      </div>
    </div>
  </div>

</asp:Content>
