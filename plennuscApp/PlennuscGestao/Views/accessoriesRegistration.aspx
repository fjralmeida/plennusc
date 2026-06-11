<%@ Page Title="" Language="C#" 
    MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" 
    AutoEventWireup="true" 
    CodeBehind="accessoriesRegistration.aspx.cs" 
    Inherits="appWhatsapp.PlennuscGestao.Views.accessoriesRegistration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Lista de acessórios</title>

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
                Lista de Acessórios
            </h1>
            <asp:Button ID="btnNovoAcessorio" runat="server" CssClass="btn-primary"
                Text="Novo Acessório" OnClick="btnNovoAcessorio_Click" />
        </div>

        <!-- Filtros -->
        <div class="filters-card">
            <h3 class="filters-title">
                <i class="bi bi-funnel"></i>
                Filtros
            </h3>
            <div class="filter-section">

                <div class="filter-item">
                    <label class="form-label">Acessório</label>
                    <asp:TextBox ID="txtAcessorio" runat="server" CssClass="form-control"
                        placeholder="Nome do acessório"></asp:TextBox>
                </div>

                <div class="filter-item">
                    <label class="form-label">Valor</label>
                    <asp:TextBox ID="txtValor" runat="server" CssClass="form-control"
                        placeholder="Valor do acessório"></asp:TextBox>
                </div>

                <div class="btn-filter-container">
                    <asp:Button ID="btnFiltrar" runat="server" CssClass="btn-filter"
                        Text="Aplicar Filtros" OnClick="btnFiltrar_Click" />
                </div>

            </div>
        </div>

        <div class="grid-container">
            <asp:GridView ID="gvAcessorios" runat="server" CssClass="custom-grid"
                AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
                OnPageIndexChanging="gvAcessorios_PageIndexChanging"
                OnRowCommand="gvAcessorios_RowCommand"
                OnRowDataBound="gvAcessorios_RowDataBound"
                EmptyDataText="Nenhum acessório encontrado no cadastro.">

                <Columns>
                    <asp:BoundField DataField="CodigoAcessorio" HeaderText="ID"
                        ItemStyle-CssClass="text-center col-id"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="NomeAcessorio" HeaderText="Nome Acessório"
                        ItemStyle-CssClass="text-left col-nomeacessorio"
                        HeaderStyle-CssClass="text-left" />

                    <asp:BoundField DataField="ValorAcessorio" HeaderText="Valor Acessório"
                        ItemStyle-CssClass="text-left col-valoracessorio"
                        HeaderStyle-CssClass="text-left" />

                    <asp:BoundField DataField="ModoAcessorio" HeaderText="Modo Acessório"
                        ItemStyle-CssClass="text-left col-modoacessorio"
                        HeaderStyle-CssClass="text-left" />

                    <asp:BoundField DataField="Sincor" HeaderText="Sincor"
                        ItemStyle-CssClass="text-left col-sincor"
                        HeaderStyle-CssClass="text-left" />

                    <asp:TemplateField HeaderText="Status"
                        ItemStyle-CssClass="text-center col-confativo"
                        HeaderStyle-CssClass="text-center">
                        <ItemTemplate>
                                <%# ((bool)Eval("Conf_Ativo")) ? "Ativo" : "Inativo" %>
                        </ItemTemplate>
                    </asp:TemplateField>

                     <asp:TemplateField HeaderText="Exibir"
                         ItemStyle-CssClass="text-center col-confexibir"
                         HeaderStyle-CssClass="text-center">
                         <ItemTemplate>
                                 <%# ((bool)Eval("Conf_Exibir")) ? "Sim" : "Não" %>
                         </ItemTemplate>
                     </asp:TemplateField>

                </Columns>

                <PagerStyle HorizontalAlign="Center" CssClass="pagination-container" />
                <HeaderStyle CssClass="grid-header" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
