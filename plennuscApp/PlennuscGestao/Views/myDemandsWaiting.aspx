<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="myDemandsWaiting.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.myDemandsWaiting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet">
    <style>
        :root {
            --primary: #83ceee;
            --primary-hover: #0d62c9;
            --success: #4cb07a;
            --success-hover: #3b8b65;
            --warning: #ffa726;
            --warning-hover: #f57c00;
            --gray-50: #f8f9fa;
            --gray-100: #f1f3f4;
            --gray-200: #e8eaed;
            --gray-300: #dadce0;
            --gray-400: #bdc1c6;
            --gray-500: #9aa0a6;
            --gray-600: #80868b;
            --gray-700: #5f6368;
            --gray-800: #3c4043;
            --gray-900: #202124;
            --border-radius: 8px;
            --shadow: 0 1px 2px 0 rgba(60, 64, 67, 0.3), 0 1px 3px 1px rgba(60, 64, 67, 0.15);
            --transition: all 0.2s ease-in-out;
        }

        body {
            background: var(--gray-100);
            font-family: 'Roboto', sans-serif;
            color: var(--gray-800);
            line-height: 1.5;
        }

        .container-main {
            max-width: 2206px;
            margin: 20px auto;
            padding: 0 16px;
        }

        /* Header */
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
            color: var(--gray-800);
            margin: 0;
        }

        .title-icon {
            background: var(--warning);
            color: white;
            width: 44px;
            height: 44px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        /* Resultados */
        .results-info {
            font-size: 14px;
            color: var(--gray-600);
            margin-bottom: 16px;
            padding: 8px 0;
        }

        /* Tabela */
        .grid-container {
            background: white;
            border-radius: var(--border-radius);
            box-shadow: var(--shadow);
            overflow: hidden;
            overflow-x: auto;
        }

        .custom-grid {
            width: 100%;
            border-collapse: collapse;
            min-width: 1200px;
        }

            .custom-grid th {
                background: var(--gray-50);
                padding: 16px 12px;
                text-align: left;
                font-weight: 600;
                color: var(--gray-700);
                border-bottom: 2px solid var(--gray-300);
                font-size: 13px;
                text-transform: uppercase;
                letter-spacing: 0.5px;
                white-space: nowrap;
            }

            .custom-grid td {
                padding: 14px 12px;
                border-bottom: 1px solid var(--gray-200);
                vertical-align: middle;
                font-size: 14px;
                line-height: 1.4;
            }

            .custom-grid tr:last-child td {
                border-bottom: none;
            }

            .custom-grid tr:hover {
                background: var(--gray-50);
                transition: var(--transition);
            }

        /* BADGES - PADRÃO CONSISTENTE */
        .badge {
            display: inline-block;
            padding: 6px 12px;
            border-radius: 16px;
            font-size: 11px;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: 0.3px;
            text-align: center;
            min-width: 80px;
            white-space: nowrap;
        }

        /* 🟢 VERDE - BAIXO/NORMAL/DENTRO DO PRAZO */
        .importancia-baixa, .importancia-baixo,
        .prioridade-normal,
        .prioridade-com-prazo,
        .prazo-dentro-prazo,
        .status-aberta, .status-aberto {
            background: #e8f5e9 !important;
            color: #2e7d32 !important;
            border: 1px solid #c8e6c9 !important;
        }

        /* 🔵 AZUL - BAIXA PRIORIDADE/EM ANDAMENTO */
        .prioridade-baixa, .prioridade-baixo,
        .status-em-andamento, .status-andamento, .status-em_andamento,
        .prazo-proximo {
            background: #e3f2fd !important;
            color: #1565c0 !important;
            border: 1px solid #bbdefb !important;
        }

        /* 🟠 LARANJA - MÉDIA/PROXIMIDADE */
        .importancia-media, .importancia-média, .importancia-medio, .importancia-médio,
        .prioridade-media, .prioridade-média, .prioridade-medio, .prioridade-médio,
        .prazo-hoje,
        .status-concluída, .status-concluida, .status-concluido, .status-concluído,
        .status-aguardando-aprovacao {
            background: #fff3e0 !important;
            color: #ef6c00 !important;
            border: 1px solid #ffe0b2 !important;
        }

        /* 🔴 VERMELHO - ALTO/ATRASADO/URGENTE */
        .importancia-alta, .importancia-alto,
        .prioridade-alta, .prioridade-alto,
        .prazo-atrasado {
            background: #ffebee !important;
            color: #c62828 !important;
            border: 1px solid #ffcdd2 !important;
        }

        /* 🟣 ROXO - CRÍTICO/MUITO URGENTE */
        .importancia-critica, .importancia-crítica, .importancia-critico, .importancia-crítico,
        .prioridade-critica, .prioridade-crítica, .prioridade-critico, .prioridade-crítico {
            background: #f3e5f5 !important;
            color: #7b1fa2 !important;
            border: 1px solid #e1bee7 !important;
            font-weight: 700 !important;
        }

        /* ⚪ CINZA - SEM DEFINIÇÃO/FECHADO */
        .prazo-sem-data,
        .status-fechada, .status-fechado,
        .prioridade-nao-definida {
            background: #f5f5f5 !important;
            color: #616161 !important;
            border: 1px solid #e0e0e0 !important;
            font-style: italic !important;
        }

        /* Botão de Aprovar */
        .btn-aprovar {
            background: var(--success);
            border: none;
            color: white;
            font-weight: 600;
            padding: 8px 16px;
            border-radius: 6px;
            font-size: 12px;
            transition: var(--transition);
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 6px;
            text-decoration: none;
            cursor: pointer;
            min-width: 120px;
            white-space: nowrap;
            box-shadow: 0 1px 3px rgba(0,0,0,0.1);
        }

            .btn-aprovar:hover {
                background: var(--success-hover);
                transform: translateY(-1px);
                box-shadow: 0 2px 6px rgba(0,0,0,0.15);
                text-decoration: none;
                color: white;
            }

            .btn-aprovar i {
                font-size: 14px;
            }

        /* Botões de ação */
        .btn-action {
            background: white;
            border: 1px solid var(--gray-300);
            color: var(--gray-700);
            padding: 6px 12px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 500;
            transition: var(--transition);
            display: inline-flex;
            align-items: center;
            gap: 4px;
            text-decoration: none;
            white-space: nowrap;
            margin: 2px;
        }

            .btn-action:hover {
                background: var(--primary);
                border-color: var(--primary);
                color: white;
                transform: translateY(-1px);
                text-decoration: none;
            }

        /* Informação de Executor */
        .aceite-info {
            font-size: 10px;
            color: var(--gray-700);
            line-height: 1.2;
            text-align: center;
            padding: 6px 4px;
            background: #f8f9fa;
            border-radius: 4px;
            border-left: 3px solid var(--success);
            display: block;
            max-width: 140px;
            word-wrap: break-word;
            white-space: normal;
        }

            .aceite-info strong {
                display: block;
                color: var(--gray-800);
                font-size: 10px;
                font-weight: 600;
                margin-bottom: 2px;
            }

        /* Alinhamentos */
        .text-center {
            text-align: center !important;
        }

        .text-left {
            text-align: left !important;
        }

        .text-right {
            text-align: right !important;
        }

        /* Colunas específicas */
        .col-id {
            width: 80px;
        }

        .col-titulo {
            width: 250px;
            min-width: 200px;
        }

        .col-categoria {
            width: 150px;
        }

        .col-importancia {
            width: 100px;
        }

        .col-prioridade {
            width: 100px;
        }

        .col-status {
            width: 120px;
        }

        .col-solicitante {
            width: 180px;
        }

        .col-data-abertura {
            width: 120px;
        }

        .col-prazo {
            width: 100px;
        }

        .col-aceite {
            width: 140px;
        }

        .col-acoes {
            width: 200px;
            min-width: 180px;
        }

        /* Paginação */
        .pagination-container {
            background: white;
            border-top: 1px solid var(--gray-200);
            display: flex;
            justify-content: center;
            padding: 16px;
        }

        .custom-grid .pager table {
            margin: 0 auto;
        }

        .custom-grid .pager td {
            padding: 4px;
            border: none;
        }

        .custom-grid .pager a,
        .custom-grid .pager span {
            padding: 8px 12px;
            border: 1px solid var(--gray-300);
            border-radius: 4px;
            margin: 0 2px;
            text-decoration: none;
            color: var(--gray-700);
            display: inline-block;
            font-size: 13px;
        }

            .custom-grid .pager a:hover {
                background: var(--gray-100);
            }

        .custom-grid .pager span {
            background: var(--primary);
            color: white;
            border-color: var(--primary);
        }

        .text-muted {
            color: var(--gray-500) !important;
            font-style: italic;
            font-size: 12px;
        }

        /* Responsividade */
        @media (max-width: 1024px) {
            .container-main {
                padding: 0 12px;
            }
        }

        @media (max-width: 768px) {
            .page-header {
                flex-direction: column;
                align-items: flex-start;
            }

            .custom-grid th,
            .custom-grid td {
                padding: 12px 8px;
            }

            .badge {
                min-width: 70px;
                padding: 4px 8px;
                font-size: 10px;
            }

            .col-acoes {
                width: 150px;
                min-width: 140px;
            }

            .btn-aprovar, .btn-action {
                min-width: 100px;
                padding: 6px 10px;
                font-size: 11px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-main">
        <!-- Header -->
        <div class="page-header">
            <h1 class="page-title">
                <span class="title-icon"> 
                    <i class="bi bi-clock-history"></i>
                </span>
                Demandas Aguardando Minha Aprovação
            </h1>
            <p class="text-muted">Aprove ou visualize as demandas que estão aguardando sua análise</p>
        </div>

        <!-- Resultados -->
        <div class="results-info">
            <asp:Label ID="lblResultados" runat="server"></asp:Label>
        </div>

        <div class="grid-container">
            <asp:GridView ID="gvDemandasAguardando" runat="server" CssClass="custom-grid"
                AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
                OnPageIndexChanging="gvDemandasAguardando_PageIndexChanging"
                OnRowCommand="gvDemandasAguardando_RowCommand"
                OnRowDataBound="gvDemandasAguardando_RowDataBound"
                EmptyDataText="Nenhuma demanda aguardando sua aprovação.">

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
                            <span class='badge <%# GetClassePrioridade(Eval("Prioridade")) %>'>
                                <%# string.IsNullOrEmpty(Eval("Prioridade")?.ToString()) ? "N/A" : Eval("Prioridade") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Prazo" 
                        ItemStyle-CssClass="text-center col-prazo" HeaderStyle-CssClass="text-center">
                        <ItemTemplate>
                            <span class='badge <%# GetClassePrazo(Eval("DataPrazo")) %>'>
                                <%# !string.IsNullOrEmpty(Eval("DataPrazo")?.ToString()) && 
                                      Eval("DataPrazo") != DBNull.Value ? 
                                      Convert.ToDateTime(Eval("DataPrazo")).ToString("dd/MM/yyyy") : 
                                      "N/A" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Importância"
                        ItemStyle-CssClass="text-center col-importancia" HeaderStyle-CssClass="text-center">
                        <ItemTemplate>
                            <span class='badge <%# GetClasseImportancia(Eval("Importancia")) %>'>
                                <%# Eval("Importancia") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Status"
                        ItemStyle-CssClass="text-center col-status" HeaderStyle-CssClass="text-center">
                        <ItemTemplate>
                            <span class='badge status-aguardando-aprovacao'>
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
                            <asp:LinkButton ID="lnkAprovar" runat="server" CssClass="btn-aprovar"
                                CommandName="Aprovar" CommandArgument='<%# Eval("CodDemanda") %>'
                                OnClientClick="return confirm('Tem certeza que deseja aprovar esta demanda?');">
                                <i class="bi bi-check-circle"></i> Aprovar
                            </asp:LinkButton>
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