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
                <div class="form-group">
                    <label for="txtBusca" class="form-label">Buscar (Nome ou CPF)</label>
                    <asp:TextBox ID="txtBusca" runat="server" CssClass="form-control" placeholder="Digite o nome ou CPF..." />
                </div>
                <div class="form-group" style="max-width:none; flex:0; align-self:flex-end; display:flex; gap:8px;">
                    <asp:Button ID="btnPesquisar" runat="server" Text="Pesquisar" CssClass="btn btn-success" OnClick="btnPesquisar_Click" />
                </div>
            </div>
            <asp:Label ID="lblMensagemPesquisa" runat="server" CssClass="msg-importacao" />
        </div>

        <!-- CARD 2: RESULTADO -->
        <div class="filters-card hidden" id="divResultado" runat="server">
            <div style="display: flex; justify-content: space-between; align-items: center; margin-bottom: 16px; flex-wrap: wrap; gap: 8px;">
                <div class="filters-title" style="margin-bottom: 0;">
                    <i class="bi bi-exclamation-triangle"></i> Pendentes de Conferência
                </div>
                <div style="display: flex; align-items: center; gap: 12px; flex-wrap: wrap;">
                    <span style="display:flex; align-items:center; gap:6px; font-size:13px; color:#475569;">
                        <label for="ddlPageSize" style="margin:0; font-weight:500;">Registros por página:</label>
                        <asp:DropDownList ID="ddlPageSize" runat="server" CssClass="form-control" 
                            Style="width:70px; height:32px; padding:2px 6px; font-size:13px;"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
                            <asp:ListItem Value="10" Selected="True">10</asp:ListItem>
                            <asp:ListItem Value="20">20</asp:ListItem>
                            <asp:ListItem Value="50">50</asp:ListItem>
                            <asp:ListItem Value="100">100</asp:ListItem>
                        </asp:DropDownList>
                    </span>
                    <asp:Button ID="btnConferirSelecionados" runat="server" Text=" Conferir Selecionados" CssClass="btn btn-success" OnClick="btnConferirSelecionados_Click" />
                    <asp:Button ID="btnExportar" runat="server" Text="Exportar Excel" CssClass="btn btn-primary" OnClick="btnExportar_Click" style="white-space:nowrap;" />
                </div>
            </div>

            <div class="grid-container" style="margin-top:0;">
                <asp:GridView ID="gridPreview" runat="server" CssClass="custom-grid" AutoGenerateColumns="false"
                    EmptyDataText="Nenhum registro encontrado." GridLines="None"
                    AllowPaging="True" PageSize="10"
                    OnPageIndexChanging="gridPreview_PageIndexChanging"
                    OnDataBound="gridPreview_DataBound"
                    OnRowDataBound="gridPreview_RowDataBound">
                    <Columns>
                        <asp:TemplateField ItemStyle-CssClass="col-check" HeaderStyle-CssClass="col-check">
                            <HeaderTemplate>
                                <input type="checkbox" id="chkSelecionarTodos" onclick="marcarTodos(this)" />
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelecionar" runat="server" CssClass="chk-linha-pendente" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="NumeroRegistro" HeaderText="Número Registro" ItemStyle-CssClass="col-curta" />
                        <asp:BoundField DataField="NumeroCpf" HeaderText="CPF" ItemStyle-CssClass="col-curta" />
                        <asp:BoundField DataField="NomeDoAssociado" HeaderText="Nome" ItemStyle-CssClass="col-nome" />
                        <asp:BoundField DataField="MesAnoReferencia" HeaderText="Mês/Ano" ItemStyle-CssClass="col-curta" />
                        <asp:BoundField DataField="ValorConvenio" HeaderText="Valor Operadora (Aliança)" DataFormatString="{0:N2}" ItemStyle-CssClass="col-curta" />
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

        function marcarTodos(checkbox) {
            var grid = document.getElementById('<%= gridPreview.ClientID %>');
            if (!grid) return;
            var checkboxes = grid.querySelectorAll('input[type="checkbox"]');
            for (var i = 0; i < checkboxes.length; i++) {
                if (checkboxes[i] !== checkbox) {
                    checkboxes[i].checked = checkbox.checked;
                }
            }
        }
    </script>
</asp:Content>