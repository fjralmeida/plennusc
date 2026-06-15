<%@ Page Title="" Language="C#"
    MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master"
    AutoEventWireup="true"
    CodeBehind="operatorRegistration.aspx.cs"
    Inherits="appWhatsapp.PlennuscGestao.Views.operatorRegistration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Lista de operadoras</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet">
    <link href="../../Content/Css/projects/gestao/structuresCss/RegistrationOperador/operatorRegistration.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">

        <%-- ── Mensagem de feedback ── --%>
        <asp:Panel ID="pnlMensagem" runat="server" Visible="false" CssClass="alert alert-info alert-dismissible mb-3">
            <asp:Label ID="lblMensagem" runat="server"></asp:Label>
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </asp:Panel>

        <%-- ══════════════════════════════════════════════════════════════════
             BANNER — visível apenas quando há operadoras pendentes
        ══════════════════════════════════════════════════════════════════ --%>
        <asp:Panel ID="pnlAviso" runat="server" Visible="false" CssClass="banner-sync">
            <i class="bi bi-exclamation-triangle-fill"></i>
            <span class="msg">
                Há <strong>
                    <asp:Label ID="lblQtdPendentes" runat="server"></asp:Label>
                    operadora(s)
                </strong> no sistema de origem pendente(s) de confirmação.
            </span>
            <asp:Button ID="btnVerPendentes" runat="server"
                CssClass="btn-ver-pendentes"
                Text="Ver pendentes"
                OnClick="btnVerPendentes_Click" />
        </asp:Panel>

        <%-- ── Header ── --%>
        <div class="page-header">
            <h1 class="page-title">
                <span class="title-icon">
                    <i class="bi bi-building me-2"></i>
                </span>
                Cadastro de Operadora
            </h1>
            <asp:Button ID="btnNovaOperadora" runat="server" CssClass="btn-primary"
                Text="Nova Operadora" OnClick="btnNovaOperadora_Click" />
        </div>

        <%-- ── Filtros ── --%>
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

        <%-- ── Grid ── --%>
        <div class="grid-container">

            <%-- Legenda visual --%>
            <div class="legenda-novo mb-2">
                <span></span> Operadora inserida nas últimas 24h
            </div>

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

                    <asp:BoundField DataField="RegistroAns" HeaderText="Registro ANS"
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
                </Columns>

                <PagerStyle HorizontalAlign="Center" CssClass="pagination-container" />
                <HeaderStyle CssClass="grid-header" />
            </asp:GridView>
        </div>

    </div>

    <%-- ══════════════════════════════════════════════════════════════════
         MODAL DE SINCRONIZAÇÃO
    ══════════════════════════════════════════════════════════════════ --%>
    <div class="modal fade" id="modalSyncOperadoras" tabindex="-1"
         aria-labelledby="modalSyncLabel" aria-hidden="true">
        <div class="modal-dialog modal-xl modal-dialog-scrollable">
            <div class="modal-content">

                <div class="modal-header modal-sync-header">
                    <h5 class="modal-title" id="modalSyncLabel">
                        <i class="bi bi-arrow-repeat me-2"></i>
                        Operadoras pendentes de sincronização
                    </h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Fechar"></button>
                </div>

                <div class="modal-body">

                    <%-- Info do usuário que irá confirmar --%>
                    <div class="modal-usuario-info">
                        <i class="bi bi-person-check me-1"></i>
                        Confirmação será registrada por:
                        <strong><asp:Label ID="lblUsuarioConfirmacao" runat="server"></asp:Label></strong>
                        em <strong><%: DateTime.Now.ToString("dd/MM/yyyy HH:mm") %></strong>
                    </div>

                    <p class="text-muted mb-3" style="font-size:13px;">
                        As operadoras abaixo existem no sistema de origem (Aliança) mas
                        ainda <strong>não foram importadas</strong> para esta base.
                        Ao confirmar, elas serão gravadas com a data/hora atual em
                        <code>Informacoes_log_i</code> e ficarão destacadas em laranja por 24h.
                    </p>

                    <table class="table table-bordered table-hover table-pendentes">
                        <thead>
                            <tr>
                                <th style="width:40px">#</th>
                                <th>CNPJ</th>
                                <th>Registro ANS</th>
                                <th>Razão Social</th>
                                <th>Nome Comercial</th>
                            </tr>
                        </thead>
                        <tbody>
                            <asp:Repeater ID="rptPendentes" runat="server">
                                <ItemTemplate>
                                    <tr>
                                        <td class="text-center text-muted"><%# Container.ItemIndex + 1 %></td>
                                        <td><%# Eval("Numero_CNPJ") %></td>
                                        <td><%# Eval("RegistroANS") %></td>
                                        <td><%# Eval("RazaoSocial") %></td>
                                        <td>
                                            <%# Eval("NomeComercial") %>
                                            <span class="badge-novo ms-1">NOVO</span>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tbody>
                    </table>

                </div>

                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary"
                            data-bs-dismiss="modal">Cancelar</button>

                    <asp:Button ID="btnConfirmarSync" runat="server"
                        CssClass="btn btn-success"
                        Text="✔ Confirmar e importar"
                        OnClick="btnConfirmarSync_Click"
                        OnClientClick="return confirm('Confirma a importação de todas as operadoras listadas?');" />
                </div>

            </div>
        </div>
    </div>

</asp:Content>

