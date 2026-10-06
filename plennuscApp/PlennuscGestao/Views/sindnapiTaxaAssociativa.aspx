<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" AutoEventWireup="true" CodeBehind="sindnapiTaxaAssociativa.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.sindnapiTaxaAssociativa" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/Css/projects/gestao/structuresCss/taxasAssociativas/taxasAssociativas.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div id="overlayCarregando" class="overlay-carregando">
        <div class="overlay-carregando-card">
            <div class="overlay-spinner"></div>
            <p class="overlay-texto">Processando, aguarde...</p>
        </div>
    </div>

    <div class="container-main">

        <div class="page-header">
            <h1 class="page-title">
                <span class="title-icon"><i class="bi bi-people-fill"></i></span>
                Taxas Associativas - SINDNAPI
            </h1>
        </div>

        <!-- CARD 1: FILTROS -->
        <div class="filters-card">
            <div class="filters-title"><i class="bi bi-search"></i>Filtros de Pesquisa</div>

            <div class="form-row">
                <div class="form-group">
                    <label for="txtEntidade" class="form-label">Entidade</label>
                    <asp:TextBox ID="txtEntidade" runat="server" CssClass="form-control"
                        Text="SINDNAPI" ReadOnly="true" />
                </div>

                <div class="form-group">
                    <label for="txtDataInicio" class="form-label">Data Pagamento - De *</label>
                    <asp:TextBox ID="txtDataInicio" runat="server" TextMode="Date" CssClass="form-control" />
                </div>

                <div class="form-group">
                    <label for="txtDataFim" class="form-label">Data Pagamento - Até *</label>
                    <asp:TextBox ID="txtDataFim" runat="server" TextMode="Date" CssClass="form-control" />
                </div>

                <div class="form-group" style="max-width: none; flex: 0;">
                    <asp:Button ID="btnProcessar" runat="server" Text="Processar"
                        CssClass="btn btn-primary"
                        OnClientClick="return mostrarOverlayCarregando();"
                        OnClick="btnProcessar_Click" />
                </div>
            </div>

            <asp:Label ID="lblMensagem" runat="server" CssClass="msg-importacao" />
        </div>

        <!-- CARD 2: RESULTADO -->
        <div class="filters-card" id="divResultado" runat="server" visible="false">
            <div class="filters-title"><i class="bi bi-table"></i>Resultado</div>

            <div class="grid-container">

                <div class="grid-toolbar">
                    <div class="grid-toolbar-left">
                        <label for="ddlPageSize">Registros por página:</label>
                        <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged"
                            CssClass="form-control ddl-pagesize">
                            <asp:ListItem Text="5" Value="5" />
                            <asp:ListItem Text="10" Value="10" Selected="True" />
                            <asp:ListItem Text="20" Value="20" />
                            <asp:ListItem Text="50" Value="50" />
                            <asp:ListItem Text="100" Value="100" />
                        </asp:DropDownList>
                    </div>
                   <div class="grid-toolbar-right">
                        <span class="resultado-total">
                            <strong><asp:Literal ID="litTotalRegistros" runat="server" /></strong> registro(s) encontrado(s)
                            &nbsp;|&nbsp;
                            Valor Total: <strong><asp:Literal ID="litTotalValor" runat="server" /></strong>
                        </span>
                        <asp:Button ID="btnExportarExcel" runat="server" Text="Exportar Excel"
                            CssClass="btn btn-success" OnClick="btnExportarExcel_Click" />
                    </div>
                </div>

                <asp:GridView ID="gvResultado" runat="server" AutoGenerateColumns="false"
                    CssClass="custom-grid" GridLines="None"
                    EmptyDataText="Nenhum registro encontrado."
                    AllowPaging="True" PageSize="10"
                    OnPageIndexChanging="gvResultado_PageIndexChanging"
                    OnDataBound="gvResultado_DataBound">
                    <PagerSettings Mode="NumericFirstLast"
                        FirstPageText="«" LastPageText="»"
                        PageButtonCount="7"
                        Position="Bottom" />
                    <PagerStyle CssClass="pager-custom" />
                    <Columns>
                        <asp:BoundField DataField="Cpf" HeaderText="CPF" ItemStyle-CssClass="col-curta" />
                        <asp:BoundField DataField="NomeBeneficiario" HeaderText="Beneficiário" ItemStyle-CssClass="col-nome" />
                        <asp:BoundField DataField="DataNascimento" HeaderText="Data Nascimento"
                            DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="col-curta" />
                        <asp:BoundField DataField="DataPagamento" HeaderText="Data Pagamento"
                            DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="col-curta" />
                        <asp:BoundField DataField="ValorEvento" HeaderText="Valor Taxa"
                            DataFormatString="{0:N2}" ItemStyle-CssClass="col-curta" />                    
                    </Columns>
                </asp:GridView>

                <asp:Label ID="lblPagerInfo" runat="server" CssClass="pager-info" />
            </div>
        </div>
    </div>


    <script>
        function mostrarOverlayCarregando() {
            document.getElementById('overlayCarregando').style.display = 'flex';
            document.body.style.overflow = 'hidden';

            setTimeout(function () {
                var botoes = document.querySelectorAll('button, input[type="submit"]');
                botoes.forEach(function (b) { b.disabled = true; });
            }, 0);

            return true;
        }
    </script>

</asp:Content>
