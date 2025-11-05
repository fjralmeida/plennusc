<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="listDemand.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.listDemand" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Lista de demandas</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet">

    <link href="../../Content/Css/projects/gestao/structuresCss/listDemand.css" rel="stylesheet" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <!-- Header -->
        <div class="page-header">
            <h1 class="page-title">
                <span class="title-icon">
                    <i class="bi bi-list-ul me-2"></i>
                </span>
                Lista de Demandas
            </h1>
            <asp:Button ID="btnNovaDemanda" runat="server" CssClass="btn-primary"
                Text="Nova Demanda" OnClick="btnNovaDemanda_Click" />
        </div>

        <!-- Filtros -->
        <div class="filters-card">
            <h3 class="filters-title">
                <i class="bi bi-funnel"></i>
                Filtros
            </h3>
            <div class="filter-section">
                <!-- Dropdown de Visualização -->
                <div class="filter-item">
                    <label class="form-label">Visualização</label>
                    <asp:DropDownList ID="ddlVisibilidade" runat="server" CssClass="form-select" AutoPostBack="true" OnSelectedIndexChanged="ddlVisibilidade_Changed">
                        <asp:ListItem Value="S" Text="Meu Setor" Selected="True" />
                        <asp:ListItem Value="M" Text="Minhas Demandas" />
                    </asp:DropDownList>
                </div>

                <div class="filter-item">
                    <label class="form-label">Status</label>
                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>

                <div class="filter-item">
                    <label class="form-label">Categoria</label>
                    <asp:DropDownList ID="ddlCategoria" runat="server" CssClass="form-select"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlCategoria_SelectedIndexChanged">
                    </asp:DropDownList>
                </div>

               <div class="filter-item">
                    <label class="form-label">Prioridade</label>
                    <asp:DropDownList ID="ddlPrioridade" runat="server" CssClass="form-select"></asp:DropDownList>
                </div>

                <div class="filter-item">
                    <label class="form-label">Solicitante</label>
                    <asp:TextBox ID="txtSolicitante" runat="server" CssClass="form-control" placeholder="Nome do solicitante"></asp:TextBox>
                </div>

                <!-- Container especial para o botão alinhado -->
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
            <asp:GridView ID="gvDemandas" runat="server" CssClass="custom-grid"
                AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
                OnPageIndexChanging="gvDemandas_PageIndexChanging"
                OnRowCommand="gvDemandas_RowCommand"
                OnRowDataBound="gvDemandas_RowDataBound">

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
                           <span class='badge importancia-<%# Eval("Importancia").ToString().ToLower().Replace("é", "e").Replace("á", "a").Replace("í", "i").Replace("ê", "e").Replace("â", "a").Replace("ô", "o").Replace("û", "u").Replace("ç", "c") %>'>
                               <%# Eval("Importancia") %>
                           </span>
                       </ItemTemplate>
                   </asp:TemplateField>

                <asp:TemplateField HeaderText="Prioridade"
                    ItemStyle-CssClass="text-center col-prioridade" HeaderStyle-CssClass="text-center">
                    <ItemTemplate>
                        <span class='badge <%# string.IsNullOrEmpty(Eval("Prioridade")?.ToString()) ? 
                                              "prioridade-nao-definida" : 
                                              "prioridade-" + Eval("Prioridade").ToString().ToLower()
                                                  .Replace(" ", "-")
                                                  .Replace("com-prazo", "com-prazo")
                                                  .Replace("é", "e").Replace("á", "a").Replace("í", "i")
                                                  .Replace("ê", "e").Replace("â", "a").Replace("ô", "o")
                                                  .Replace("û", "u").Replace("ç", "c") %>'>
                            <%# string.IsNullOrEmpty(Eval("Prioridade")?.ToString()) ? "N/A" : Eval("Prioridade") %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Prazo" 
                    ItemStyle-CssClass="text-center col-prazo" HeaderStyle-CssClass="text-center">
                    <ItemTemplate>
                        <span class='badge badge-prazo <%# GetClassePrazo(Eval("DataPrazo")) %>'>
                            <%# !string.IsNullOrEmpty(Eval("DataPrazo")?.ToString()) && 
                                  Eval("DataPrazo") != DBNull.Value ? 
                                  Convert.ToDateTime(Eval("DataPrazo")).ToString("dd/MM/yyyy") : 
                                  "Sem Prazo" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>

                 

                    <asp:TemplateField HeaderText="Status"
                        ItemStyle-CssClass="text-center col-status" HeaderStyle-CssClass="text-center">
                        <ItemTemplate>
                            <span class='badge status-<%# Eval("Status").ToString().ToLower().Replace(" ", "-").Replace("ê", "e").Replace("é", "e").Replace("á", "a").Replace("í", "i").Replace("ó", "o").Replace("ú", "u").Replace("ã", "a").Replace("õ", "o").Replace("ç", "c").Replace("â", "a").Replace("ô", "o") %>'>
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>

                  <asp:TemplateField HeaderText="Aceite"
                    ItemStyle-CssClass="text-center col-aceite" HeaderStyle-CssClass="text-center">
                    <ItemTemplate>
                        <asp:Label ID="lblAceiteInfo" runat="server" CssClass="aceite-info"
                            Visible="false"> <%-- Inicia como false, controlamos no code behind --%>
                            <strong>Aceita</strong>
                            por: <%# Eval("NomePessoaExecucao") %><br/>
                            em <%# Eval("DataAceitacao", "{0:dd/MM/yyyy HH:mm}") %>
                        </asp:Label>

                        <asp:LinkButton ID="btnAceitar" runat="server" CssClass="btn-aceitar"
                            CommandName="Aceitar" CommandArgument='<%# Eval("CodDemanda") %>'
                            Visible="false"> <%-- Inicia como false, controlamos no code behind --%>
                            <i class="bi bi-check-circle"></i> Aceitar
                        </asp:LinkButton>
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
