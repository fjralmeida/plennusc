<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="myDemandsProgress.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.myDemandsProgress" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet">
    <style>
        /* Mesmo CSS do myDemandsOpen */
        :root {
            --primary: #83ceee;
            --primary-hover: #0d62c9;
            --success: #4cb07a;
            --success-hover: #3b8b65;
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
        .container-main { max-width: 2206px; margin: 20px auto; padding: 0 16px; }
        .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 24px; flex-wrap: wrap; gap: 16px; }
        .page-title { display: flex; align-items: center; gap: 12px; font-size: 24px; font-weight: 500; color: var(--gray-800); margin: 0; }
        .title-icon { background: var(--primary); color: white; width: 44px; height: 44px; border-radius: 50%; display: flex; align-items: center; justify-content: center; }
        .btn-primary { background: var(--success); border: none; color: white; font-weight: 500; padding: 10px 20px; border-radius: 4px; transition: var(--transition); display: flex; align-items: center; gap: 8px; }
        .btn-primary:hover { background: var(--success-hover); box-shadow: 0 1px 3px rgba(0, 0, 0, 0.2); }
        .results-info { font-size: 14px; color: var(--gray-600); margin-bottom: 16px; padding: 8px 0; }
        .grid-container { background: white; border-radius: var(--border-radius); box-shadow: var(--shadow); overflow: hidden; }
        .custom-grid { width: 100%; border-collapse: collapse; }
        .custom-grid th { background: var(--gray-50); padding: 16px; text-align: left; font-weight: 500; color: var(--gray-700); border-bottom: 1px solid var(--gray-200); font-size: 14px; }
        .custom-grid td { padding: 16px; border-bottom: 1px solid var(--gray-200); vertical-align: middle; font-size: 14px; }
        .custom-grid tr:last-child td { border-bottom: none; }
        .custom-grid tr:hover { background: var(--gray-50); }
        .status-badge { display: inline-block; padding: 4px 10px; border-radius: 12px; font-size: 12px; font-weight: 500; }
        .btn-action { background: none; border: 1px solid var(--gray-300); color: var(--gray-700); padding: 6px 12px; border-radius: 4px; font-size: 13px; transition: var(--transition); display: inline-flex; align-items: center; gap: 4px; text-decoration: none; }
        .btn-action:hover { background: var(--gray-100); border-color: var(--gray-400); text-decoration: none; }
        .pagination-container { background: white; border-top: 1px solid var(--gray-200); display: flex; justify-content: center; }
        .custom-grid .pager table { margin: 0 auto; }
        .custom-grid .pager td { padding: 4px; border: none; }
        .custom-grid .pager a, .custom-grid .pager span { padding: 6px 12px; border: 1px solid var(--gray-300); border-radius: 4px; margin: 0 2px; text-decoration: none; color: var(--gray-700); display: inline-block; }
        .custom-grid .pager a:hover { background: var(--gray-100); }
        .custom-grid .pager span { background: var(--primary); color: white; border-color: var(--primary); }
        .prioridade-badge { padding: 4px 8px; border-radius: 12px; font-size: 12px; font-weight: 500; text-transform: uppercase; }
        .prioridade-29 { background-color: #e8f5e8; color: #2e7d32; border: 1px solid #c8e6c9; }
        .prioridade-30 { background-color: #e3f2fd; color: #1565c0; border: 1px solid #bbdefb; }
        .prioridade-31 { background-color: #fff3e0; color: #ef6c00; border: 1px solid #ffe0b2; }
        .prioridade-32 { background-color: #ffebee; color: #c62828; border: 1px solid #ffcdd2; }
        .prioridade-33 { background-color: #fce4ec; color: #ad1457; border: 1px solid #f8bbd0; font-weight: bold; }
        @media (max-width: 768px) {
            .page-header { flex-direction: column; align-items: flex-start; }
            .custom-grid { display: block; overflow-x: auto; }
            .btn-primary { width: 100%; justify-content: center; }
            .custom-grid th, .custom-grid td { padding: 12px 8px; }
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
                    <asp:BoundField DataField="CodDemanda" HeaderText="ID" />
                    <asp:BoundField DataField="Titulo" HeaderText="Título" />
                    <asp:BoundField DataField="Categoria" HeaderText="Categoria" />
                    <asp:BoundField DataField="Subtipo" HeaderText="Subtipo" />

                    <asp:TemplateField HeaderText="Prioridade">
                        <ItemTemplate>
                            <span class='prioridade-badge prioridade-<%# Eval("CodPrioridade") %>'>
                                <%# Eval("Prioridade") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class="status-badge"> 
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:BoundField DataField="Solicitante" HeaderText="Solicitante" />
                    <asp:BoundField DataField="DataSolicitacao" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}" />

                    <asp:TemplateField HeaderText="Ações">
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
