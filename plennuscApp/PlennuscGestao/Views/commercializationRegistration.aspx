<%@ Page Title="" Language="C#" 
    MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" 
    AutoEventWireup="true" 
    CodeBehind="commercializationRegistration.aspx.cs" 
    Inherits="appWhatsapp.PlennuscGestao.Views.commercializationRegistration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Lista de comercialização</title>

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
                Lista de Comercialização
            </h1>
            <asp:Button ID="btnNovaComercializacao" runat="server" CssClass="btn-primary"
                Text="Nova Comercialização" OnClick="btnNovaComercializacao_Click" />
        </div>

             <!-- Filtros -->
             <div class="filters-card">
                 <h3 class="filters-title">
                     <i class="bi bi-funnel"></i>
                     Filtros
                 </h3>
                 <div class="filter-section">

                     <div class="filter-item">
                         <label class="form-label">Estado</label>
                         <asp:TextBox ID="txtEstado" runat="server" CssClass="form-control"
                             placeholder="Nome do estado"></asp:TextBox>
                     </div>

                     <div class="filter-item">
                         <label class="form-label">Cidade</label>
                         <asp:TextBox ID="txtCidade" runat="server" CssClass="form-control"
                             placeholder="Nome da cidade"></asp:TextBox>
                     </div>

                      <div class="btn-filter-container">
                         <asp:Button ID="btnFiltrar" runat="server" CssClass="btn-filter"
                             Text="Aplicar Filtros" OnClick="btnFiltrar_Click" />
                     </div>
                 </div>

             </div>

            <div class="grid-container">
                <asp:GridView ID="gvComercializacao" runat="server" CssClass="custom-grid"
                    AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
                    OnPageIndexChanging="gvComercializacao_PageIndexChanging"
                    OnRowCommand="gvComercializacao_RowCommand"
                    OnRowDataBound="gvComercializacao_RowDataBound"
                    EmptyDataText="Nenhum local de comercialização encontrado no cadastro.">

                <Columns>
                    <asp:BoundField DataField="CodigoComercializacaoMunicipio" HeaderText="ID"
                        ItemStyle-CssClass="text-center col-codigocomercializacaomunicipio"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="CodigoCidade" HeaderText="Código IBGE Cidade"
                        ItemStyle-CssClass="text-center col-codigocidade"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="NomeEstado" HeaderText="Sigla Estado"
                        ItemStyle-CssClass="text-center col-nomeestado"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="NomePlano" HeaderText="Nome Plano"
                        ItemStyle-CssClass="text-center col-nomeplano"
                        HeaderStyle-CssClass="text-center" />

                     <asp:BoundField DataField="NomeCidade" HeaderText="Nome Cidade"
                         ItemStyle-CssClass="text-center col-nomecidade"
                         HeaderStyle-CssClass="text-center" />

                     <asp:TemplateField HeaderText="Ativo ou Inativo"
                         ItemStyle-CssClass="text-center col-confativo"
                         HeaderStyle-CssClass="text-center">
                         <ItemTemplate>
                                 <%# ((bool)Eval("Conf_Ativo")) ? "Ativo" : "Inativo" %>
                             </span>
                         </ItemTemplate>
                     </asp:TemplateField>

                    <asp:TemplateField HeaderText="Exibir"
                       ItemStyle-CssClass="text-center col-exibir"
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