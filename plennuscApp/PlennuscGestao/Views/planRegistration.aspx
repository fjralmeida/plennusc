<%@ Page Title="" Language="C#" 
    MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master"
    AutoEventWireup="true" 
    CodeBehind="planRegistration.aspx.cs" 
    Inherits="appWhatsapp.PlennuscGestao.Views.planRegistration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Lista de planos</title>

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
                Lista de Planos
            </h1>
            <asp:Button ID="btnNovoPlano" runat="server" CssClass="btn-primary"
                Text="Novo Plano" OnClick="btnNovoPlano_Click" />
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
                        placeholder="Nome do Plano"></asp:TextBox>
                </div>

                <div class="filter-item">
                    <label class="form-label">Segmentação</label>
                    <asp:TextBox ID="txtSegmentacao" runat="server" CssClass="form-control"
                        placeholder="A, H, O, OB"></asp:TextBox>
                </div>

                <div class="filter-item">
                    <label class="form-label">Abrangência</label>
                    <asp:TextBox ID="txtAbrangencia" runat="server" CssClass="form-control"
                        placeholder="Nacional, Estadual, Municipal..."></asp:TextBox>
                </div>

               <div class="filter-item">
                    <label class="form-label">Coparticipação</label>
                    <asp:TextBox ID="txtCoparticipacao" runat="server" CssClass="form-control"
                        placeholder="Com ou sem"></asp:TextBox>
                </div>

                <div class="btn-filter-container">
                    <asp:Button ID="btnFiltrar" runat="server" CssClass="btn-filter"
                        Text="Aplicar Filtros" OnClick="btnFiltrar_Click" />
                </div>

            </div>

        </div>

        <div class="grid-container">
            <asp:GridView ID="gvPlanos" runat="server" CssClass="custom-grid"
                AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
                OnPageIndexChanging="gvPlanos_PageIndexChanging"
                OnRowCommand="gvPlanos_RowCommand"
                OnRowDataBound="gvPlanos_RowDataBound"
                EmptyDataText="Nenhum plano encontrado no cadastro.">

                <Columns>
                    <asp:BoundField DataField="CodigoPlano" HeaderText="ID"
                        ItemStyle-CssClass="text-center col-codigoplano"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="CodigoProduto" HeaderText="Código Produto"
                        ItemStyle-CssClass="text-center col-codigoproduto"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="Num_CNPJ_Operadora" HeaderText="CNPJ Operadora"
                        ItemStyle-CssClass="text-left col-cnpj"
                        HeaderStyle-CssClass="text-left" />

                    <asp:BoundField DataField="TipoContratacao" HeaderText="Tipo Contratação"
                        ItemStyle-CssClass="text-center col-tipocontratacao"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="NomePlanoComercial" HeaderText="Nome Plano"
                        ItemStyle-CssClass="text-left col-nomecomercial"
                        HeaderStyle-CssClass="text-left" />

                    <asp:BoundField DataField="Segmentacao" HeaderText="Segmentação"
                        ItemStyle-CssClass="text-center col-segmentacao"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="Abrangencia" HeaderText="Abrangência"
                        ItemStyle-CssClass="text-center col-abrangencia"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="Coparticipacao" HeaderText="Coparticipação"
                        ItemStyle-CssClass="text-center col-coparticipacao"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="Acomodacao" HeaderText="Acomodação"
                       ItemStyle-CssClass="text-center col-acomodacao"
                       HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="DecSau" HeaderText="Declaração de Saúde"
                       ItemStyle-CssClass="text-center col-decsau"
                       HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="Promocional" HeaderText="Promocional"
                       ItemStyle-CssClass="text-center col-promocional"
                       HeaderStyle-CssClass="text-center" />

                    <asp:TemplateField HeaderText="Ativo ou Inativo"
                        ItemStyle-CssClass="text-center col-confativo"
                        HeaderStyle-CssClass="text-center">
                        <ItemTemplate>
                                <%# ((bool)Eval("Conf_Ativo")) ? "Ativo" : "Inativo" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>


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
