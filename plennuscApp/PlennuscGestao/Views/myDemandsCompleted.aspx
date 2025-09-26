<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="myDemandsCompleted.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.myDemandsCompleted" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

        <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <style>
        .container-main {
            max-width: 2206px;
            margin: 20px auto;
            padding: 0 16px;
        }

        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 24px;
            flex-wrap: wrap;
            gap: 16px;
        }

        .page-title {
            display: flex;
            align-items: center;
            gap: 12px;
            font-size: 24px;
            font-weight: 500;
            margin: 0;
        }

        .title-icon {
            background: #4cb07a;
            color: white;
            width: 44px;
            height: 44px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .grid-container {
            background: white;
            border-radius: 8px;
            box-shadow: 0 1px 2px 0 rgba(60, 64, 67, 0.3), 0 1px 3px 1px rgba(60, 64, 67, 0.15);
            overflow: hidden;
            overflow-x: auto;
        }

        .custom-grid {
            width: 100%;
            border-collapse: collapse;
            min-width: 1200px;
        }

        .custom-grid th {
            background: #f8f9fa;
            padding: 16px 12px;
            text-align: left;
            font-weight: 600;
            color: #5f6368;
            border-bottom: 2px solid #dadce0;
            font-size: 13px;
            text-transform: uppercase;
            white-space: nowrap;
        }

        .custom-grid td {
            padding: 14px 12px;
            border-bottom: 1px solid #e8eaed;
            vertical-align: middle;
            font-size: 14px;
        }

        .custom-grid tr:hover {
            background: #f8f9fa;
        }

        .badge {
            display: inline-block;
            padding: 6px 12px;
            border-radius: 16px;
            font-size: 11px;
            font-weight: 600;
            text-transform: uppercase;
            min-width: 80px;
            white-space: nowrap;
        }

        .status-concluida {
            background: #e8f5e9 !important;
            color: #2e7d32 !important;
            border: 1px solid #c8e6c9 !important;
        }

        .btn-action {
            background: white;
            border: 1px solid #dadce0;
            color: #5f6368;
            padding: 6px 12px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 500;
            display: inline-flex;
            align-items: center;
            gap: 4px;
            text-decoration: none;
            white-space: nowrap;
        }

        .btn-action:hover {
            background: #83ceee;
            border-color: #83ceee;
            color: white;
            text-decoration: none;
        }

        .text-center { text-align: center !important; }
        .text-left { text-align: left !important; }

        .col-id { width: 80px; }
        .col-titulo { width: 250px; }
        .col-solicitante { width: 180px; }
        .col-data-abertura { width: 120px; }
        .col-categoria { width: 150px; }
        .col-prioridade { width: 100px; }
        .col-prazo { width: 100px; }
        .col-importancia { width: 100px; }
        .col-status { width: 120px; }
        .col-aceite { width: 140px; }
        .col-acoes { width: 100px; }

        .text-muted {
            color: #9aa0a6 !important;
            font-style: italic;
            font-size: 12px;
        }

        @media (max-width: 768px) {
            .page-header { flex-direction: column; align-items: flex-start; }
            .custom-grid th, .custom-grid td { padding: 12px 8px; }
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">  <div class="container-main">
        <!-- Header -->
        <div class="page-header">
            <h1 class="page-title">
                <span class="title-icon"> 
                    <i class="bi bi-check-circle"></i>
                </span>
                Minhas Demandas Concluídas
            </h1>
            <p class="text-muted">Aqui estão as demandas que você concluiu ou que foram concluídas para você</p>
        </div>

        <!-- Resultados -->
        <div class="results-info">
            <asp:Label ID="lblResultados" runat="server"></asp:Label>
        </div>

        <div class="grid-container">
            <asp:GridView ID="gvDemandasConcluidas" runat="server" CssClass="custom-grid"
                AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
                OnPageIndexChanging="gvDemandasConcluidas_PageIndexChanging"
                OnRowCommand="gvDemandasConcluidas_RowCommand"
                EmptyDataText="Nenhuma demanda concluída encontrada.">

                <Columns>
                    <asp:BoundField DataField="CodDemanda" HeaderText="ID"
                        ItemStyle-CssClass="text-center col-id" HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="Titulo" HeaderText="Título"
                        ItemStyle-CssClass="text-left col-titulo" HeaderStyle-CssClass="text-left" />

                    <asp:BoundField DataField="Solicitante" HeaderText="Solicitante"
                        ItemStyle-CssClass="text-left col-solicitante" HeaderStyle-CssClass="text-left" />

                    <asp:BoundField DataField="DataSolicitacao" HeaderText="Data de Abertura" DataFormatString="{0:dd/MM/yyyy}"
                        ItemStyle-CssClass="text-center col-data-abertura" HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="Categoria" HeaderText="Categoria"
                        ItemStyle-CssClass="text-left col-categoria" HeaderStyle-CssClass="text-left" />

                    <asp:TemplateField HeaderText="Prioridade"
                        ItemStyle-CssClass="text-center col-prioridade" HeaderStyle-CssClass="text-center">
                        <ItemTemplate>
                            <span class='badge prioridade-<%# Eval("Prioridade").ToString().ToLower().Replace(" ", "-") %>'>
                                <%# Eval("Prioridade") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Prazo"
                        ItemStyle-CssClass="text-center col-prazo" HeaderStyle-CssClass="text-center">
                        <ItemTemplate>
                            <%# !string.IsNullOrEmpty(Eval("DataPrazo")?.ToString()) && 
                                Eval("DataPrazo") != DBNull.Value ? 
                                Convert.ToDateTime(Eval("DataPrazo")).ToString("dd/MM/yyyy") : 
                                "<span class='text-muted'>N/A</span>" %>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Importância"
                        ItemStyle-CssClass="text-center col-importancia" HeaderStyle-CssClass="text-center">
                        <ItemTemplate>
                            <span class='badge importancia-<%# Eval("Importancia").ToString().ToLower().Replace(" ", "-") %>'>
                                <%# Eval("Importancia") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Status"
                        ItemStyle-CssClass="text-center col-status" HeaderStyle-CssClass="text-center">
                        <ItemTemplate>
                            <span class='badge status-concluida'>
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Executor"
                        ItemStyle-CssClass="text-center col-aceite" HeaderStyle-CssClass="text-center">
                        <ItemTemplate>
                            <asp:Label ID="lblExecutorInfo" runat="server" CssClass="aceite-info"
                                Visible='<%# Eval("CodPessoaExecucao") != null && Convert.ToInt32(Eval("CodPessoaExecucao")) > 0 %>'>
                                <strong>Executada por:</strong>
                                <%# Eval("NomePessoaExecucao") %>
                            </asp:Label>
                            <asp:Label ID="lblSemExecutor" runat="server" CssClass="text-muted"
                                Visible='<%# Eval("CodPessoaExecucao") == null || Convert.ToInt32(Eval("CodPessoaExecucao")) == 0 %>'>
                                Sem executor
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Ações"
                        ItemStyle-CssClass="text-center col-acoes" HeaderStyle-CssClass="text-center">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkVer" runat="server" CssClass="btn-action"
                                CommandName="Ver" CommandArgument='<%# Eval("CodDemanda") %>'>
                                <i class="bi bi-eye"></i> Ver
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>

                <PagerStyle CssClass="pagination-container" />
                <HeaderStyle CssClass="grid-header" />
            </asp:GridView>
        </div>
    </div>
</asp:Content>