<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="myDemandsCompleted.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.myDemandsCompleted" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

         <title>Demandas concluídas</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="../../Content/Css/projects/gestao/structuresCss/myDemandsCompleted.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">  
    <div class="container-main">
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

                    <asp:TemplateField HeaderText="Importância"
                        ItemStyle-CssClass="text-center col-importancia" HeaderStyle-CssClass="text-center">
                        <ItemTemplate>
                            <span class='badge importancia-<%# Eval("Importancia").ToString().ToLower().Replace(" ", "-") %>'>
                                <%# Eval("Importancia") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

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