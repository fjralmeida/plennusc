<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="myDemandsWaiting.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.myDemandsWaiting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

         <title>Aguardado aprovação</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet">
    <link href="../../Content/Css/projects/gestao/structuresCss/myDemandsWaiting.css" rel="stylesheet" />
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

        <!-- Filtros - NOVOS FILTROS -->
        <div class="filters-card">
            <h3 class="filters-title">
                <i class="bi bi-funnel"></i>
                Filtros
            </h3>
            <div class="filter-section">
                <!-- Prioridade -->
                <div class="filter-item">
                    <label class="form-label">Prioridade</label>
                    <asp:DropDownList ID="ddlPrioridade" runat="server" CssClass="form-select">
                    </asp:DropDownList>
                </div>

                <!-- Solicitante -->
                <div class="filter-item">
                    <label class="form-label">Solicitante</label>
                    <asp:TextBox ID="txtSolicitante" runat="server" CssClass="form-control" 
                        placeholder="Nome do solicitante"></asp:TextBox>
                </div>

                <!-- Botão Aplicar Filtros -->
                <div class="btn-filter-container">
                    <asp:Button ID="btnFiltrar" runat="server" CssClass="btn-filter"
                        Text="Aplicar Filtros" OnClick="btnFiltrar_Click" />
                </div>
            </div>
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