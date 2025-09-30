﻿<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="myDemandsProgress.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.myDemandsProgress" %>
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
            background: var(--primary);
            color: white;
            width: 44px;
            height: 44px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .btn-primary {
            background: var(--success);
            border: none;
            color: white;
            font-weight: 500;
            padding: 10px 20px;
            border-radius: 4px;
            transition: var(--transition);
            display: flex;
            align-items: center;
            gap: 8px;
        }

            .btn-primary:hover {
                background: var(--success-hover);
                box-shadow: 0 1px 3px rgba(0, 0, 0, 0.2);
            }

        /* Filtros */
        .filters-card {
            background: white;
            border-radius: var(--border-radius);
            box-shadow: var(--shadow);
            margin-bottom: 24px;
            padding: 20px;
        }

        .filters-title {
            font-size: 16px;
            font-weight: 500;
            color: var(--gray-700);
            margin-bottom: 16px;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .filter-section {
            display: grid;
            grid-template-columns: repeat(6, 1fr);
            gap: 12px;
            align-items: end;
        }

        .filter-item {
            display: flex;
            flex-direction: column;
        }

        .form-label {
            display: block;
            font-size: 14px;
            font-weight: 500;
            color: var(--gray-700);
            margin-bottom: 8px;
            white-space: nowrap;
        }

        .form-control, .form-select {
            width: 100%;
            padding: 10px 12px;
            border: 1px solid var(--gray-300);
            border-radius: 4px;
            font-size: 14px;
            transition: var(--transition);
            background: white;
            height: 40px;
            box-sizing: border-box;
        }

            .form-control:focus, .form-select:focus {
                outline: none;
                border-color: var(--primary);
                box-shadow: 0 0 0 2px rgba(26, 115, 232, 0.2);
            }

        .btn-filter-container {
            display: flex;
            flex-direction: column;
            justify-content: flex-end;
            height: 100%;
        }

        .btn-filter {
            background: var(--success);
            border: none;
            color: white;
            font-weight: 500;
            padding: 10px 16px;
            border-radius: 4px;
            transition: var(--transition);
            height: 40px;
            display: flex;
            align-items: center;
            justify-content: center;
            width: 100%;
            box-sizing: border-box;
            margin-top: 28px;
        }

            .btn-filter:hover {
                background: var(--success-hover);
                box-shadow: 0 1px 3px rgba(0, 0, 0, 0.2);
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
.status-concluída, .status-concluida, .status-concluido, .status-concluído {
    background: #fff3e0 !important;
    color: #ef6c00 !important;
    border: 1px solid #ffe0b2 !important;
}

/* 🔴 VERMELHO - ALTO/ATRASADO/URGENTE */
.importancia-alta, .importancia-alto,
.prioridade-alta, .prioridade-alto,
.prazo-atrasado, .prioridade-com-prazo { 
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

/* FALLBACK PARA VALORES DESCONHECIDOS */
.badge:not([class*="status-"]):not([class*="prioridade-"]):not([class*="importancia-"]):not([class*="prazo-"]) {
    background: #f1f3f4 !important;
    color: #5f6368 !important;
    border: 1px solid #dadce0 !important;
}

        /* Botão de Aceite */
        .btn-aceitar {
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

            .btn-aceitar:hover {
                background: var(--success-hover);
                transform: translateY(-1px);
                box-shadow: 0 2px 6px rgba(0,0,0,0.15);
                text-decoration: none;
                color: white;
            }

            .btn-aceitar i {
                font-size: 14px;
            }

        /* Informação de Aceite */
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


            .aprovação-info {
    font-size: 10px;
    color: #2e7d32;
    line-height: 1.2;
}

.aprovação-info strong {
    display: block;
    color: #1b5e20;
    font-size: 10px;
    font-weight: 600;
    margin-bottom: 2px;
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
        }

            .btn-action:hover {
                background: var(--primary);
                border-color: var(--primary);
                color: white;
                transform: translateY(-1px);
                text-decoration: none;
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

        .col-subtipo {
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

        .col-data {
            width: 100px;
        }

        .col-aceite {
            width: 140px;
        }

        .col-acoes {
            width: 100px;
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

        .col-data-abertura {
            width: 120px;
        }

        .col-prazo {
            width: 100px;
        }

        .text-muted {
            color: var(--gray-500) !important;
            font-style: italic;
            font-size: 12px;
        }

      /* Informação de Aprovação - AGORA IGUAL AO ACEITE */
.aprovacao-info {
    font-size: 10px;
    color: var(--gray-700);
    line-height: 1.2;
    text-align: center;
    padding: 6px 4px;
    background: #f8f9fa;
    border-radius: 4px;
    border-left: 3px solid var(--primary); /* Azul para aprovação */
    display: block;
    max-width: 140px;
    word-wrap: break-word;
    white-space: normal;
    margin-bottom: 4px;
}

.aprovacao-info strong {
    display: block;
    color: var(--gray-800);
    font-size: 10px;
    font-weight: 600;
    margin-bottom: 2px;
}

        /* Responsividade */
        @media (max-width: 1024px) {
            .filter-section {
                grid-template-columns: repeat(3, 1fr);
            }

            .container-main {
                padding: 0 12px;
            }
        }

        @media (max-width: 768px) {
            .page-header {
                flex-direction: column;
                align-items: flex-start;
            }

            .filter-section {
                grid-template-columns: 1fr;
            }

            .btn-primary {
                width: 100%;
                justify-content: center;
            }

            .custom-grid th,
            .custom-grid td {
                padding: 12px 8px;
            }

            .btn-filter {
                margin-top: 0;
            }

            .badge {
                min-width: 70px;
                padding: 4px 8px;
                font-size: 10px;
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
                    <i class="bi bi-hourglass-split"></i>
                </span>
                Minhas Demandas em Andamento
            </h1>
            <p class="text-muted">Aqui estão as demandas que você aceitou e estão em andamento</p>
        </div>

        <!-- Resultados -->
        <div class="results-info">
            <asp:Label ID="lblResultados" runat="server"></asp:Label>
        </div>

        <div class="grid-container">
      <asp:GridView ID="gvMinhasDemandas" runat="server" CssClass="custom-grid"
    AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
    OnPageIndexChanging="gvMinhasDemandas_PageIndexChanging"
    OnRowCommand="gvMinhasDemandas_RowCommand"
    OnRowDataBound="gvMinhasDemandas_RowDataBound"
    EmptyDataText="Nenhuma demanda em andamento encontrada.">

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

         <asp:TemplateField HeaderText="Importância"
             ItemStyle-CssClass="text-center col-importancia" HeaderStyle-CssClass="text-center">
             <ItemTemplate>
                 <span class='badge importancia-<%# Eval("Importancia").ToString().ToLower()
                     .Replace("é", "e").Replace("á", "a").Replace("í", "i")
                     .Replace("ê", "e").Replace("â", "a").Replace("ô", "o")
                     .Replace("û", "u").Replace("ç", "c") %>'>
                     <%# Eval("Importancia") %>
                 </span>
             </ItemTemplate>
         </asp:TemplateField>

       <asp:TemplateField HeaderText="Prioridade"
            ItemStyle-CssClass="text-center col-prioridade" HeaderStyle-CssClass="text-center">
            <ItemTemplate>
                <span class='badge prioridade-<%# Eval("Prioridade").ToString().ToLower()
                    .Replace("é", "e").Replace("á", "a").Replace("í", "i")
                    .Replace("ê", "e").Replace("â", "a").Replace("ô", "o")
                    .Replace("û", "u").Replace("ç", "c")
                    .Replace(" ", "-") %>'> 
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


        <asp:TemplateField HeaderText="Status"
            ItemStyle-CssClass="text-center col-status" HeaderStyle-CssClass="text-center">
            <ItemTemplate>
                <span class='badge status-<%# Eval("Status").ToString().ToLower()
                    .Replace(" ", "-").Replace("ê", "e").Replace("é", "e").Replace("á", "a")
                    .Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Replace("ã", "a")
                    .Replace("õ", "o").Replace("ç", "c").Replace("â", "a").Replace("ô", "o") %>'>
                    <%# Eval("Status") %>
                </span>
            </ItemTemplate>
        </asp:TemplateField>

   <asp:TemplateField HeaderText="Situação"
    ItemStyle-CssClass="text-center col-aceite" HeaderStyle-CssClass="text-center">
    <ItemTemplate>
        <!-- Informação de Aceite -->
        <asp:Label ID="lblAceiteInfo" runat="server" CssClass="aceite-info"
            Visible='<%# Eval("CodPessoaExecucao") != null && Convert.ToInt32(Eval("CodPessoaExecucao")) > 0 %>'>
            <strong>Aceita</strong>
            por: <%# Eval("NomePessoaExecucao") %><br/>
            em <%# Eval("DataAceitacao", "{0:dd/MM/yyyy HH:mm}") %>
        </asp:Label>

        <!-- Informação de Aprovação -->
        <asp:Label ID="lblAprovacaoInfo" runat="server" CssClass="aprovacao-info"
            Visible='<%# Eval("CodPessoaAprovacao") != null && Convert.ToInt32(Eval("CodPessoaAprovacao")) > 0 %>'>
            <strong>Aprovada</strong>
            por: <%# Eval("NomePessoaAprovacao") %><br/>
            em <%# Eval("DataAprovacao", "{0:dd/MM/yyyy HH:mm}") %>
        </asp:Label>

        <!-- Botão Aceitar (só aparece para executores quando não aceitaram ainda) -->
        <asp:LinkButton ID="btnAceitar" runat="server" CssClass="btn-aceitar"
            CommandName="Aceitar" CommandArgument='<%# Eval("CodDemanda") %>'
            Visible='<%# Eval("PapelUsuario").ToString() == "Executor" && 
                       (Eval("CodPessoaExecucao") == null || Convert.ToInt32(Eval("CodPessoaExecucao")) == 0) %>'>
            <i class="bi bi-check-circle"></i> Aceitar
        </asp:LinkButton>
    </ItemTemplate>
</asp:TemplateField>

        <asp:TemplateField HeaderText="Ações"
            ItemStyle-CssClass="text-center col-acoes" HeaderStyle-CssClass="text-center">
            <ItemTemplate>
                <asp:LinkButton ID="lnkVer" runat="server" CssClass="btn-action"
                    CommandName="Ver" CommandArgument='<%# Eval("CodDemanda") %>'
                    ToolTip="Visualizar demanda">
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

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            colorStatusBadges();
        });

        function colorStatusBadges() {
            const badges = document.querySelectorAll('.status-badge');
            badges.forEach(badge => {
                const statusText = badge.textContent.trim().toLowerCase();
                badge.style.backgroundColor = '';
                badge.style.color = '';

                if (statusText.includes('aberta')) {
                    badge.style.backgroundColor = '#e6f4ea';
                    badge.style.color = '#137333';
                } else if (statusText.includes('andamento')) {
                    badge.style.backgroundColor = '#e8f0fe';
                    badge.style.color = '#1a73e8';
                } else if (statusText.includes('concluída') || statusText.includes('concluida')) {
                    badge.style.backgroundColor = '#fef7e0';
                    badge.style.color = '#f9ab00';
                } else if (statusText.includes('fechada')) {
                    badge.style.backgroundColor = '#fce8e6';
                    badge.style.color = '#c5221f';
                } else {
                    badge.style.backgroundColor = '#f1f3f4';
                    badge.style.color = '#5f6368';
                }
            });
        }

        const observer = new MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                if (mutation.addedNodes.length) {
                    colorStatusBadges();
                }
            });
        });

        window.addEventListener('load', function () {
            const gridContainer = document.querySelector('.grid-container');
            if (gridContainer) {
                observer.observe(gridContainer, { childList: true, subtree: true });
            }
        });
    </script>
</asp:Content>
