<%@ Page Title="" Language="C#" 
    MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" 
    AutoEventWireup="true" 
    CodeBehind="careerRegistration.aspx.cs" 
    Inherits="appWhatsapp.PlennuscGestao.Views.careerRegistration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Lista de profissões</title>

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
                Lista de Profissões
            </h1>
            <asp:Button ID="btnNovaProfissao" runat="server" CssClass="btn-primary"
                Text="Nova Profissão" OnClick="btnNovaProfissao_Click" />
        </div>

             <!-- Filtros -->
             <div class="filters-card">
                 <h3 class="filters-title">
                     <i class="bi bi-funnel"></i>
                     Filtros
                 </h3>
                 <div class="filter-section">

                     <div class="filter-item">
                         <label class="form-label">Nome</label>
                         <asp:TextBox ID="txtProfissao" runat="server" CssClass="form-control"
                             placeholder="Nome da profissão"></asp:TextBox>
                     </div>

                      <div class="btn-filter-container">
                         <asp:Button ID="btnFiltrar" runat="server" CssClass="btn-filter"
                             Text="Aplicar Filtros" OnClick="btnFiltrar_Click" />
                     </div>
                 </div>

             </div>

            <div class="grid-container">
                <asp:GridView ID="gvProfissao" runat="server" CssClass="custom-grid"
                    AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
                    OnPageIndexChanging="gvProfissao_PageIndexChanging"
                    OnRowCommand="gvProfissao_RowCommand"
                    OnRowDataBound="gvProfissao_RowDataBound"
                    EmptyDataText="Nenhuma profissão encontrada no cadastro.">

                <Columns>
                    <asp:BoundField DataField="CodigoProfissao" HeaderText="ID"
                        ItemStyle-CssClass="text-center col-codigoprofissao"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="Nome" HeaderText="Nome"
                        ItemStyle-CssClass="text-left col-nome"
                        HeaderStyle-CssClass="text-left" />
                </Columns>

                <PagerStyle HorizontalAlign="Center" CssClass="pagination-container" />
                <HeaderStyle CssClass="grid-header" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>
