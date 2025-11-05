<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="myDemandsProgress.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.myDemandsProgress" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

     <title>Demandas em andamento</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet">

    <link href="../../Content/Css/projects/gestao/structuresCss/myDemandsProgress.css" rel="stylesheet" />

    <script src="../../Content/Css/js/projects/gestaoJs/structuresJs/myDemandsProgress.js"></script>

    <script type="text/javascript">

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
</asp:Content>
