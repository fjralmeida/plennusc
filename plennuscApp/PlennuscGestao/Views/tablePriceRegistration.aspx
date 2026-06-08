<%@ Page Title="" Language="C#" 
    MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" 
    AutoEventWireup="true" 
    CodeBehind="tablePriceRegistration.aspx.cs" 
    Inherits="appWhatsapp.PlennuscGestao.Views.tablePriceRegistration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Lista de Preços</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet">
    <link href="../../Content/Css/projects/gestao/structuresCss/listDemand.css" rel="stylesheet" />

    <style>
        tr.pagination-container a,
        tr.pagination-container span {
            display: inline-block !important;
            min-width: 36px !important;
            height: 36px !important;
            line-height: 36px !important;
            padding: 0 8px !important;
            margin: 0 4px !important;
            font-size: 14px !important;
            font-weight: 500 !important;
            text-align: center !important;
            text-decoration: none !important;
            border: 1px solid #dadce0 !important;
            border-radius: 6px !important;
            background-color: white !important;
            color: #3c4043 !important;
        }
        tr.pagination-container a:hover {
            background-color: #f1f3f4 !important;
            border-color: #bdc1c6 !important;
            transform: translateY(-1px) !important;
        }
        tr.pagination-container span {
            background-color: #4cb07a !important;
            border-color: #4cb07a !important;
            color: white !important;
        }
    </style>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <div class="container-main">

        <!-- Header -->
        <div class="page-header">
            <h1 class="page-title">
                <span class="title-icon">
                    <i class="bi bi-building me-2"></i>
                </span>
                Lista de Preços
            </h1>
            <asp:Button ID="btnNovoPreco" runat="server" CssClass="btn-primary"
                Text="Novo Preço" OnClick="btnNovoPreco_Click" />
        </div>

         <!-- Filtros -->
         <div class="filters-card">
             <h3 class="filters-title">
                 <i class="bi bi-funnel"></i>
                 Filtros
             </h3>
             <div class="filter-section">

                 <div class="filter-item">
                     <label class="form-label">Plano</label>
                     <asp:TextBox ID="txtNomePlanoComercial" runat="server" CssClass="form-control"
                         placeholder="Nome do plano"></asp:TextBox>
                 </div>

                  <div class="btn-filter-container">
                     <asp:Button ID="btnFiltrar" runat="server" CssClass="btn-filter"
                         Text="Aplicar Filtros" OnClick="btnFiltrar_Click" />
                 </div>
             </div>

         </div>

        <div class="grid-container">
            <asp:GridView ID="gvPrecos" runat="server" CssClass="custom-grid"
                AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
                OnPageIndexChanging="gvPrecos_PageIndexChanging"
                OnRowCommand="gvPrecos_RowCommand"
                OnRowDataBound="gvPrecos_RowDataBound"
                EmptyDataText="Nenhum preço encontrado no cadastro.">

            <Columns>
                <asp:BoundField DataField="CodigoTabelaPreco" HeaderText="ID"
                    ItemStyle-CssClass="text-center col-codigotabelapreco"
                    HeaderStyle-CssClass="text-center" />

                <asp:BoundField DataField="NomePlanoComercial" HeaderText="Nome Plano Comercial"
                    ItemStyle-CssClass="text-center col-nomeplanocomercial"
                    HeaderStyle-CssClass="text-center" />

                <asp:BoundField DataField="FaixaBeneficiarios" HeaderText="Faixa Beneficiários"
                    ItemStyle-CssClass="text-center col-faixabeneficiarios"
                    HeaderStyle-CssClass="text-center" />

                <asp:BoundField DataField="Faixa0" HeaderText="Faixa 0"
                    ItemStyle-CssClass="text-center col-faixa0"
                    HeaderStyle-CssClass="text-center" />

                <asp:BoundField DataField="Faixa1" HeaderText="Faixa 1"
                    ItemStyle-CssClass="text-center col-faixa1"
                    HeaderStyle-CssClass="text-center" />

                <asp:BoundField DataField="Faixa2" HeaderText="Faixa 2"
                    ItemStyle-CssClass="text-center col-faixa2"
                    HeaderStyle-CssClass="text-center" />

                <asp:BoundField DataField="Faixa3" HeaderText="Faixa 3"
                    ItemStyle-CssClass="text-center col-faixa3"
                    HeaderStyle-CssClass="text-center" />

                <asp:BoundField DataField="Faixa4" HeaderText="Faixa 4"
                    ItemStyle-CssClass="text-center col-faixa4"
                    HeaderStyle-CssClass="text-center" />

                <asp:BoundField DataField="Faixa5" HeaderText="Faixa 5"
                    ItemStyle-CssClass="text-center col-faixa5"
                    HeaderStyle-CssClass="text-center" />

                <asp:BoundField DataField="Faixa6" HeaderText="Faixa 6"
                    ItemStyle-CssClass="text-center col-faixa6"
                    HeaderStyle-CssClass="text-center" />

                <asp:BoundField DataField="Faixa7" HeaderText="Faixa 7"
                    ItemStyle-CssClass="text-center col-faixa7"
                    HeaderStyle-CssClass="text-center" />

                <asp:BoundField DataField="Faixa8" HeaderText="Faixa 8"
                    ItemStyle-CssClass="text-center col-faixa8"
                    HeaderStyle-CssClass="text-center" />

                <asp:BoundField DataField="Faixa9" HeaderText="Faixa 9"
                    ItemStyle-CssClass="text-center col-faixa9"
                    HeaderStyle-CssClass="text-center" />

                <asp:BoundField DataField="DataInicioVenda" HeaderText="Data Início Venda"
                    ItemStyle-CssClass="text-center col-datainiciovenda"
                    HeaderStyle-CssClass="text-center" />

                <asp:TemplateField HeaderText="Data Fim Venda"
                   ItemStyle-CssClass="text-center col-datafimvenda"
                       HeaderStyle-CssClass="text-center">
                    <ItemTemplate>
                        <%# Eval("DataFimVenda") == DBNull.Value || Eval("DataFimVenda") == null
                            ? "Indeterminado"
                            : ((DateTime)Eval("DataFimVenda")).ToString("dd/MM/yyyy HH:mm:ss") %>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Exibir Venda"
                    ItemStyle-CssClass="text-center col-exibirvenda"
                    HeaderStyle-CssClass="text-center">
                    <ItemTemplate>
                        <%# Eval("DataFimVenda") == DBNull.Value || Eval("DataFimVenda") == null
                            || (DateTime)Eval("DataFimVenda") > DateTime.Now
                            ? "Sim" : "Não" %>
                    </ItemTemplate>
                </asp:TemplateField>

            </Columns>

            <PagerStyle HorizontalAlign="Center" CssClass="pagination-container" />
            <HeaderStyle CssClass="grid-header" />
        </asp:GridView>
    </div>
</div>
</asp:Content>