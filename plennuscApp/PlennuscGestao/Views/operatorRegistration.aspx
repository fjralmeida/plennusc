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

<script type="text/javascript">
    // ============================================================
    //  FUNÇÕES PARA A ABA DE NOVAS (INSERÇÃO) - JÁ FUNCIONA
    // ============================================================

    function getCheckboxesLinhas() {
        var tabela = document.querySelector('.table-pendentes tbody');
        if (!tabela) return [];
        return tabela.querySelectorAll('input[type="checkbox"]:not(:disabled)');
    }

    function atualizarContadorSelecionadas() {
        var checks = getCheckboxesLinhas();
        if (checks.length === 0) {
            var span0 = document.getElementById('<%= lblQtdSelecionadas.ClientID %>');
            if (span0) span0.innerText = '0';

            var btnConfirmar0 = document.getElementById('<%= btnConfirmarSync.ClientID %>');
            if (btnConfirmar0) {
                btnConfirmar0.disabled = true;
                btnConfirmar0.style.opacity = '0.5';
                btnConfirmar0.style.cursor = 'not-allowed';
            }
            return;
        }

        var marcados = 0;
        for (var i = 0; i < checks.length; i++) {
            if (checks[i].checked) marcados++;
        }

        var span = document.getElementById('<%= lblQtdSelecionadas.ClientID %>');
        if (span) span.innerText = marcados;

        var chkTodos = document.getElementById('chkSelecionarTodos');
        if (chkTodos) {
            chkTodos.checked = (marcados === checks.length && checks.length > 0);
        }

        var btnConfirmar = document.getElementById('<%= btnConfirmarSync.ClientID %>');
        if (btnConfirmar) {
            btnConfirmar.disabled = (marcados === 0);
            btnConfirmar.style.opacity = (marcados === 0) ? '0.5' : '1';
            btnConfirmar.style.cursor = (marcados === 0) ? 'not-allowed' : 'pointer';
        }
    }

    function toggleSelecionarTodos() {
        var chkTodos = document.getElementById('chkSelecionarTodos');
        if (!chkTodos) return;

        var estado = chkTodos.checked;
        var checks = getCheckboxesLinhas();
        for (var i = 0; i < checks.length; i++) {
            checks[i].checked = estado;
        }
        atualizarContadorSelecionadas();
    }

    function confirmarImportacao() {
        var checks = getCheckboxesLinhas();
        var marcados = 0;
        for (var i = 0; i < checks.length; i++) {
            if (checks[i].checked) marcados++;
        }
        if (marcados === 0) {
            alert('Selecione pelo menos uma operadora para importar.');
            return false;
        }
        return confirm('Confirma a importação das ' + marcados + ' operadora(s) selecionada(s)?');
    }

    // ============================================================
    //  FUNÇÕES PARA A ABA DE ALTERAÇÕES (ATUALIZAÇÃO) - IGUAL À DE CIMA
    // ============================================================

    function getCheckboxesUpdate() {
        var tabela = document.querySelector('#panelAlteracoes .table-pendentes tbody');
        if (!tabela) return [];
        return tabela.querySelectorAll('input[type="checkbox"]:not(:disabled)');
    }

    function atualizarContadorUpdate() {
        var checks = getCheckboxesUpdate();
        if (checks.length === 0) {
            var span0 = document.getElementById('<%= lblQtdSelecionadasUpdate.ClientID %>');
            if (span0) span0.innerText = '0';

            var btnConfirmar0 = document.getElementById('<%= btnConfirmarUpdate.ClientID %>');
            if (btnConfirmar0) {
                btnConfirmar0.disabled = true;
                btnConfirmar0.style.opacity = '0.5';
                btnConfirmar0.style.cursor = 'not-allowed';
            }
            return;
        }

        var marcados = 0;
        for (var i = 0; i < checks.length; i++) {
            if (checks[i].checked) marcados++;
        }

        var span = document.getElementById('<%= lblQtdSelecionadasUpdate.ClientID %>');
        if (span) span.innerText = marcados;

        var chkTodos = document.getElementById('chkSelecionarTodosUpdate');
        if (chkTodos) {
            chkTodos.checked = (marcados === checks.length && checks.length > 0);
        }

        var btn = document.getElementById('<%= btnConfirmarUpdate.ClientID %>');
        if (btn) {
            btn.disabled = (marcados === 0);
            btn.style.opacity = (marcados === 0) ? '0.5' : '1';
            btn.style.cursor = (marcados === 0) ? 'not-allowed' : 'pointer';
        }
    }

    function toggleSelecionarTodosUpdate() {
        var chkTodos = document.getElementById('chkSelecionarTodosUpdate');
        if (!chkTodos) return;

        var estado = chkTodos.checked;
        var checks = getCheckboxesUpdate();
        for (var i = 0; i < checks.length; i++) {
            checks[i].checked = estado;
        }
        atualizarContadorUpdate();
    }

    function confirmarAtualizacaoUpdate() {
        var checks = getCheckboxesUpdate();
        var marcados = 0;
        for (var i = 0; i < checks.length; i++) {
            if (checks[i].checked) marcados++;
        }
        if (marcados === 0) {
            alert('Selecione pelo menos uma operadora para atualizar.');
            return false;
        }
        return confirm('Confirma a atualização das ' + marcados + ' operadora(s) selecionada(s)?');
    }

    // ============================================================
    //  FUNÇÕES GERAIS
    // ============================================================

    function trocarAbaSync(aba) {
        document.getElementById('tabBtnNovas').classList.remove('active');
        document.getElementById('tabBtnAlteracoes').classList.remove('active');
        document.getElementById('panelNovas').classList.remove('active');
        document.getElementById('panelAlteracoes').classList.remove('active');

        var btnNovas = document.getElementById('<%= btnConfirmarSync.ClientID %>');
        var btnUpdate = document.getElementById('<%= btnConfirmarUpdate.ClientID %>');

        if (aba === 'novas') {
            document.getElementById('tabBtnNovas').classList.add('active');
            document.getElementById('panelNovas').classList.add('active');
            if (btnNovas) btnNovas.style.display = 'inline-block';
            if (btnUpdate) btnUpdate.style.display = 'none';
            setTimeout(atualizarContadorSelecionadas, 50);
        } else {
            document.getElementById('tabBtnAlteracoes').classList.add('active');
            document.getElementById('panelAlteracoes').classList.add('active');
            if (btnNovas) btnNovas.style.display = 'none';
            if (btnUpdate) btnUpdate.style.display = 'inline-block';
            setTimeout(atualizarContadorUpdate, 50);
        }
    }

    function abrirModalSync() {
        var modalEl = document.getElementById('modalSyncOperadoras');
        if (!modalEl) return;
        var modal = new bootstrap.Modal(modalEl);
        modal.show();
        setTimeout(function () {
            var panelNovas = document.getElementById('panelNovas');
            var panelAlteracoes = document.getElementById('panelAlteracoes');
            if (panelNovas && panelNovas.classList.contains('active')) {
                atualizarContadorSelecionadas();
            } else if (panelAlteracoes && panelAlteracoes.classList.contains('active')) {
                atualizarContadorUpdate();
            } else {
                atualizarContadorSelecionadas(); // fallback
            }
        }, 300);
    }

    // ============================================================
    //  INICIALIZAÇÃO
    // ============================================================

    document.addEventListener('DOMContentLoaded', function () {
        // ---- Checkbox "Selecionar todos" para NOVAS ----
        var chkTodos = document.getElementById('chkSelecionarTodos');
        if (chkTodos) {
            chkTodos.addEventListener('change', toggleSelecionarTodos);
            chkTodos.addEventListener('click', toggleSelecionarTodos);
        }
        var tabela = document.querySelector('.table-pendentes tbody');
        if (tabela) {
            tabela.addEventListener('click', function (e) {
                if (e.target && e.target.type === 'checkbox' && !e.target.disabled) {
                    atualizarContadorSelecionadas();
                }
            });
        }

        // ---- Checkbox "Selecionar todos" para ALTERAÇÕES ----
        var chkTodosUpdate = document.getElementById('chkSelecionarTodosUpdate');
        if (chkTodosUpdate) {
            chkTodosUpdate.addEventListener('change', toggleSelecionarTodosUpdate);
            chkTodosUpdate.addEventListener('click', toggleSelecionarTodosUpdate);
        }
        var tabelaUpdate = document.querySelector('#panelAlteracoes .table-pendentes tbody');
        if (tabelaUpdate) {
            tabelaUpdate.addEventListener('click', function (e) {
                if (e.target && e.target.type === 'checkbox' && !e.target.disabled) {
                    atualizarContadorUpdate();
                }
            });
        }

        // ---- Inicializa contador conforme aba ativa ----
        var panelNovas = document.getElementById('panelNovas');
        var panelAlteracoes = document.getElementById('panelAlteracoes');
        if (panelNovas && panelNovas.classList.contains('active')) {
            atualizarContadorSelecionadas();
        } else if (panelAlteracoes && panelAlteracoes.classList.contains('active')) {
            atualizarContadorUpdate();
        }
    });

</script>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">

        <%-- ── Mensagem de feedback ── --%>
        <asp:Panel ID="pnlMensagem" runat="server" Visible="false" CssClass="alert alert-info alert-dismissible mb-3">
            <asp:Label ID="lblMensagem" runat="server"></asp:Label>
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </asp:Panel>

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

        <%-- ═══════════════════════════════════════════════════════════
             MODAL DE SINCRONIZAÇÃO
        ═══════════════════════════════════════════════════════════ --%>
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

    <div class="modal-usuario-info">
        <i class="bi bi-person-check me-1"></i>
        Confirmação será registrada por:
        <strong><asp:Label ID="lblUsuarioConfirmacao" runat="server"></asp:Label></strong>
        em <strong><%: DateTime.Now.ToString("dd/MM/yyyy HH:mm") %></strong>
    </div>

    <div class="tabs-sync">
        <button type="button" class="tab-sync-btn active" id="tabBtnNovas" onclick="trocarAbaSync('novas')">
            <i class="bi bi-plus-circle"></i>
            Novas
            <span class="tab-sync-count"><asp:Label ID="lblQtdAbaNovas" runat="server">0</asp:Label></span>
        </button>
        <button type="button" class="tab-sync-btn tab-alteracoes" id="tabBtnAlteracoes" onclick="trocarAbaSync('alteracoes')">
            <i class="bi bi-arrow-repeat"></i>
            Alterações
            <span class="tab-sync-count"><asp:Label ID="lblQtdAbaAlteracoes" runat="server">0</asp:Label></span>
        </button>
    </div>

    <%-- ═══ PAINEL: NOVAS ═══ --%>
    <div class="tab-sync-panel active" id="panelNovas">

        <p class="text-muted mb-3" style="font-size:13px;">
            As operadoras abaixo existem no sistema de origem (Aliança) mas
            ainda <strong>não foram importadas</strong> para esta base.
        </p>

        <div class="barra-selecao">
            <label style="display:flex; align-items:center; gap:6px; cursor:pointer; margin:0;">
                <input type="checkbox" id="chkSelecionarTodos" checked
                       style="width:16px; height:16px; cursor:pointer;" />
                Selecionar todos
            </label>
            <span>
                <strong><asp:Label ID="lblQtdSelecionadas" runat="server">0</asp:Label></strong>
                operadora(s) selecionada(s)
            </span>
        </div>

        <table class="table table-bordered table-hover table-pendentes">
            <thead>
                <tr>
                    <th class="col-check"></th>
                    <th>CNPJ</th>
                    <th>Registro ANS</th>
                    <th>Razão Social</th>
                    <th>Nome Comercial</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rptPendentes" runat="server">
                    <ItemTemplate>
                        <tr class='<%# (bool)Eval("AnsValido") ? "" : "linha-ans-invalido" %>'>
                            <td class="col-check">
                                <asp:HiddenField ID="hdnCodigoGrupo" runat="server" Value='<%# Eval("CodigoGrupo") %>' />
                                <asp:CheckBox ID="chkSelecionar" runat="server"
                                    Checked='<%# (bool)Eval("AnsValido") %>'
                                    Enabled='<%# (bool)Eval("AnsValido") %>'
                                    CssClass="chk-linha-pendente" />
                            </td>
                            <td><%# Eval("Numero_CNPJ") %></td>
                            <td>
                                <%# Eval("RegistroANS") ?? "—" %>
                                <%# (bool)Eval("AnsValido") ? "" : "<i class='bi bi-exclamation-triangle-fill text-danger ms-1' title='Registro ANS inválido'></i>" %>
                            </td>
                            <td><%# Eval("RazaoSocial") %></td>
                            <td>
                                <%# Eval("NomeComercial") %>
                                <span class="badge-novo ms-1">NOVO</span>
                                <%# (bool)Eval("AnsValido")
                                    ? ""
                                    : "<div class='text-danger' style='font-size:11px; margin-top:4px;'><i class='bi bi-exclamation-circle'></i> Registro ANS inválido — corrija na origem antes de importar</div>" %>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>

    </div>

    <%-- ═══ PAINEL: ALTERAÇÕES ═══ --%>
    <div class="tab-sync-panel" id="panelAlteracoes">

        <p class="text-muted mb-3" style="font-size:13px;">
            As operadoras abaixo já existem nesta base, mas o sistema de origem (Aliança)
            possui valores diferentes. Marque as que deseja atualizar.
        </p>

        <div class="barra-selecao">
            <label style="display:flex; align-items:center; gap:6px; cursor:pointer; margin:0;">
                <input type="checkbox" id="chkSelecionarTodosUpdate" checked
                       style="width:16px; height:16px; cursor:pointer;" />
                Selecionar todos
            </label>
            <span>
                <strong><asp:Label ID="lblQtdSelecionadasUpdate" runat="server">0</asp:Label></strong>
                operadora(s) selecionada(s)
            </span>
        </div>

        <table class="table table-bordered table-hover table-pendentes">
            <thead>
                <tr>
                    <th class="col-check"></th>
                    <th>CNPJ</th>
                    <th>Registro ANS</th>
                    <th>Razão Social</th>
                    <th>Nome Comercial</th>
                </tr>
            </thead>
            <tbody>
                <asp:Repeater ID="rptAlteracoesUpdate" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td class="col-check">
                                <asp:HiddenField ID="hdnCnpjUpdate" runat="server" Value='<%# Eval("Numero_CNPJ") %>' />
                                <asp:CheckBox ID="chkSelecionarUpdate" runat="server" Checked="true" CssClass="chk-linha-update" />
                            </td>
                            <td><%# Eval("Numero_CNPJ") %></td>
                            <td>
                                <%# (bool)Eval("DivergeAns")
                                    ? "<span class='valor-antigo'>" + (Eval("RegistroANS_Atual") ?? "—") + "</span><span class='seta-mudanca'>→</span><span class='valor-novo'>" + Eval("RegistroANS_Novo") + "</span>"
                                    : (Eval("RegistroANS_Atual") ?? "—").ToString() %>
                            </td>
                            <td>
                                <%# (bool)Eval("DivergeRazaoSocial")
                                    ? "<span class='valor-antigo'>" + Eval("RazaoSocial_Atual") + "</span><span class='seta-mudanca'>→</span><span class='valor-novo'>" + Eval("RazaoSocial_Novo") + "</span>"
                                    : Eval("RazaoSocial_Atual") %>
                            </td>
                            <td>
                                <%# (bool)Eval("DivergeNomeComercial")
                                    ? "<span class='valor-antigo'>" + Eval("NomeComercial_Atual") + "</span><span class='seta-mudanca'>→</span><span class='valor-novo'>" + Eval("NomeComercial_Novo") + "</span>"
                                    : Eval("NomeComercial_Atual") %>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </tbody>
        </table>

    </div>

</div>

                   <div class="modal-footer">
    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>

    <asp:Button ID="btnConfirmarSync" runat="server"
        CssClass="btn btn-success"
        Text="✔ Confirmar e importar selecionadas"
        OnClick="btnConfirmarSync_Click"
        OnClientClick="return confirmarImportacao();" />

    <asp:Button ID="btnConfirmarUpdate" runat="server"
        CssClass="btn btn-primary"
        Text="✔ Confirmar e atualizar selecionadas"
        OnClick="btnConfirmarUpdate_Click"
        OnClientClick="return confirmarAtualizacaoUpdate();"
        Style="display:none;" />
</div>

                </div>
            </div>
        </div>

        <%-- ── Header ── --%>
        <div class="page-header">
            <h1 class="page-title">
                <span class="title-icon">
                    <i class="bi bi-building me-2"></i>
                </span>
                Cadastro de Operadora
            </h1>
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

   <div class="legenda-status">
    <span class="legenda-item">
        <span class="legenda-cor laranja"></span> Inserida nas últimas 24h
    </span>
    <span class="legenda-item">
        <span class="legenda-cor azul"></span> Atualizada recentemente
    </span>
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

</asp:Content>