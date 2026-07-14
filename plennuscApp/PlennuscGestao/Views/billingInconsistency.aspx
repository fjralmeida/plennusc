<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" AutoEventWireup="true" CodeBehind="billingInconsistency.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.billingInconsistency" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/Css/projects/gestao/structuresCss/billing/billingInconsistency.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <div class="page-header">
            <h1 class="page-title">
                <span class="title-icon"><i class="bi bi-file-earmark-check"></i></span>
                Inconsistências de Faturamento
            </h1>
        </div>

        <!-- CARD 1: FILTROS -->
<div class="filters-card" id="divImportacao" runat="server">
    <div class="filters-title"><i class="bi bi-search"></i> Filtros de Pesquisa</div>
    <div class="form-row">
        <div class="form-group">
            <label for="ddlOperadora" class="form-label">Operadora *</label>
            <asp:DropDownList ID="ddlOperadora" runat="server" CssClass="form-control" />
        </div>
        <div class="form-group">
            <label for="txtMesAnoReferencia" class="form-label">Mês/Ano Referência *</label>
            <asp:TextBox ID="txtMesAnoReferencia" runat="server" CssClass="form-control"
                MaxLength="7" placeholder="MM/AAAA" onkeyup="mascararMesAno(this)" />
        </div>
        <!-- 🔥 CAMPO DE BUSCA POR NOME/CPF -->
        <div class="form-group">
            <label for="txtBusca" class="form-label">Buscar (Nome ou CPF)</label>
            <asp:TextBox ID="txtBusca" runat="server" CssClass="form-control" placeholder="Digite o nome ou CPF..." />
        </div>
        <div class="form-group" style="max-width:none; flex:0; align-self:flex-end;">
            <asp:Button ID="btnPesquisar" runat="server" Text="Pesquisar" CssClass="btn btn-success" OnClick="btnPesquisar_Click" />
        </div>
    </div>
    <asp:Label ID="lblMensagemPesquisa" runat="server" CssClass="msg-importacao" />
</div>

        <!-- CARD 2: RESULTADO -->
        <div class="filters-card hidden" id="divResultado" runat="server">
            <div class="filters-title"><i class="bi bi-exclamation-triangle"></i> Pendentes de Conferência</div>

            <div class="grid-container" style="margin-top:16px;">
<asp:GridView ID="gridPreview" runat="server" CssClass="custom-grid" AutoGenerateColumns="false"
    EmptyDataText="Nenhum registro encontrado." GridLines="None"
    AllowPaging="True" PageSize="10"
    OnPageIndexChanging="gridPreview_PageIndexChanging"
    OnDataBound="gridPreview_DataBound">
    <Columns>
        <asp:BoundField DataField="NumeroCpf" HeaderText="CPF" ItemStyle-CssClass="col-curta" />
        <asp:BoundField DataField="NomeDoAssociado" HeaderText="Nome" ItemStyle-CssClass="col-nome" />
        <asp:BoundField DataField="MesAnoReferencia" HeaderText="Mês/Ano" ItemStyle-CssClass="col-curta" />
        <asp:BoundField DataField="ValorConvenio" HeaderText="Valor Convênio" DataFormatString="{0:N2}" ItemStyle-CssClass="col-curta" />
        <asp:BoundField DataField="ValorAdicional" HeaderText="Valor Adicional" DataFormatString="{0:N2}" ItemStyle-CssClass="col-curta" />
        <asp:BoundField DataField="ValorFatura" HeaderText="Valor Fatura" DataFormatString="{0:N2}" ItemStyle-CssClass="col-curta" />
        <asp:BoundField DataField="DataAdmissao" HeaderText="Data Admissão" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="col-curta" />
        <asp:BoundField DataField="DataExclusao" HeaderText="Data Exclusão" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="col-curta" />
    </Columns>
    <PagerTemplate>
        <div class="pager-custom">
            <span class="pager-info">
                <asp:Label ID="lblPagerInfo" runat="server" Text=""></asp:Label>
            </span>
            <span class="pager-buttons">
                <asp:PlaceHolder ID="phPagerButtons" runat="server"></asp:PlaceHolder>
            </span>
        </div>
    </PagerTemplate>
</asp:GridView>
            </div>
        </div>
    </div>

    <script>
        function mascararMesAno(input) {
            var valor = input.value.replace(/\D/g, '');
            if (valor.length > 6) valor = valor.substring(0, 6);
            if (valor.length >= 3) valor = valor.substring(0, 2) + '/' + valor.substring(2);
            input.value = valor;
        }
    </script>
</asp:Content>