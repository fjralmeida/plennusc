<%@ Page Title="" Language="C#"
    MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master"
    AutoEventWireup="true"
    CodeBehind="operatorRegistration.aspx.cs"
    Inherits="appWhatsapp.PlennuscGestao.Views.operatorRegistration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Lista de operadoras</title>

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
                Lista de Operadoras
            </h1>
            <asp:Button ID="btnNovaOperadora" runat="server" CssClass="btn-primary"
                Text="Nova Operadora" OnClick="btnNovaOperadora_Click" />
        </div>

        <!-- Filtros -->
        <div class="filters-card">
            <h3 class="filters-title">
                <i class="bi bi-funnel"></i>
                Filtros
            </h3>
            <div class="filter-section">

                <div class="filter-item">
                    <label class="form-label">Operadora</label>
                    <asp:TextBox ID="txtOperadora" runat="server" CssClass="form-control"
                        placeholder="Nome da operadora"></asp:TextBox>
                </div>

                <div class="filter-item">
                    <label class="form-label">Registro ANS</label>
                    <asp:TextBox ID="txtRegistroAns" runat="server" CssClass="form-control"
                        placeholder="Registro ANS"></asp:TextBox>
                </div>

                <div class="filter-item">
                    <label class="form-label">CNPJ</label>
                    <asp:TextBox ID="txtCnpj" runat="server" CssClass="form-control"
                        placeholder="CNPJ"></asp:TextBox>
                </div>

                <div class="btn-filter-container">
                    <asp:Button ID="btnFiltrar" runat="server" CssClass="btn-filter"
                        Text="Aplicar Filtros" OnClick="btnFiltrar_Click" />
                </div>

            </div>
        </div>

        <div class="grid-container">
            <asp:GridView ID="gvOperadoras" runat="server" CssClass="custom-grid"
                AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
                OnPageIndexChanging="gvOperadoras_PageIndexChanging"
                OnRowCommand="gvOperadoras_RowCommand"
                OnRowDataBound="gvOperadoras_RowDataBound"
                EmptyDataText="Nenhuma operadora encontrada no cadastro.">

                <Columns>
                    <asp:BoundField DataField="CodigoOperadora" HeaderText="ID"
                        ItemStyle-CssClass="text-center col-id"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="RegistroANS" HeaderText="Registro ANS"
                        ItemStyle-CssClass="text-left col-registroans"
                        HeaderStyle-CssClass="text-left" />

                    <asp:BoundField DataField="Numero_CNPJ" HeaderText="CNPJ"
                        ItemStyle-CssClass="text-left col-cnpj"
                        HeaderStyle-CssClass="text-left" />

                    <asp:BoundField DataField="RazaoSocial" HeaderText="Razão Social"
                        ItemStyle-CssClass="text-left col-razaosocial"
                        HeaderStyle-CssClass="text-left" />

                    <asp:BoundField DataField="NomeComercial" HeaderText="Nome Comercial"
                        ItemStyle-CssClass="text-left col-nomecomercial"
                        HeaderStyle-CssClass="text-left" />


                      <%-- Criar os botões de editar e excluir 
                    
                    <asp:TemplateField HeaderText="Ações" ItemStyle-CssClass="col-acoes">
                        <ItemTemplate>
                            <asp:LinkButton ID="btnEditar" runat="server"
                                CssClass="btn btn-sm btn-outline-primary btn-editar me-2"
                                CommandArgument='<%# Eval("CodOperadora") %>'
                                OnClick="btnEditar_Click"
                                ToolTip="Editar">
                                <i class="fas fa-edit"></i>
                            </asp:LinkButton>
                            <asp:Button ID="btnExcluir" runat="server" Text="Excluir" 
                                CssClass="btn btn-sm btn-outline-danger btn-excluir-departamento" 
                                CommandArgument='<%# Eval("CodOperadora") %>'
                                OnClick="btnExcluir_Click"
                                OnClientClick="return confirm('Tem certeza que deseja excluir este departamento?');" />
                        </ItemTemplate>
                    </asp:TemplateField> --%>

                </Columns>

                <PagerStyle HorizontalAlign="Center" CssClass="pagination-container" />
                <HeaderStyle CssClass="grid-header" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>