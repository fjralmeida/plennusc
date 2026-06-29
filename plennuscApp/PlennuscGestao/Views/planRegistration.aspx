<%@ Page Title="Lista de Planos" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" AutoEventWireup="true" CodeBehind="planRegistration.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.planRegistration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Lista de planos</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet" />
    <link href="../../Content/Css/projects/gestao/structuresCss/PlanRegistration/planRegistratrion.css" rel="stylesheet" />

    <script type="text/javascript">
        // ============================================================
        //  FUNÇÕES PARA A ABA DE NOVAS (INSERÇÃO) - IGUAL À OPERADORA
        // ============================================================

        function getCheckboxesLinhas() {
            var tabela = document.querySelector('#panelNovas .table-pendentes tbody');
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
                alert('Selecione pelo menos um plano para importar.');
                return false;
            }
            return confirm('Confirma a importação de ' + marcados + ' plano(s)?');
        }

        // ============================================================
        //  FUNÇÕES PARA A ABA DE ALTERAÇÕES (ATUALIZAÇÃO)
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
                alert('Selecione pelo menos um plano para atualizar.');
                return false;
            }
            return confirm('Confirma a atualização de ' + marcados + ' plano(s)?');
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
            var modalEl = document.getElementById('modalSyncPlanos');
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
                    atualizarContadorSelecionadas();
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
            var tabela = document.querySelector('#panelNovas .table-pendentes tbody');
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

        // Reatualizar após postback assíncrono (UpdatePanel)
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            var panelNovas = document.getElementById('panelNovas');
            var panelAlteracoes = document.getElementById('panelAlteracoes');
            if (panelNovas && panelNovas.classList.contains('active')) {
                atualizarContadorSelecionadas();
            } else if (panelAlteracoes && panelAlteracoes.classList.contains('active')) {
                atualizarContadorUpdate();
            }
            // Reatach eventos para os novos checkboxes carregados via UpdatePanel
            var chkTodos = document.getElementById('chkSelecionarTodos');
            if (chkTodos) {
                chkTodos.onchange = toggleSelecionarTodos;
                chkTodos.onclick = toggleSelecionarTodos;
            }
            var tabela = document.querySelector('#panelNovas .table-pendentes tbody');
            if (tabela) {
                tabela.onclick = function (e) {
                    if (e.target && e.target.type === 'checkbox' && !e.target.disabled) {
                        atualizarContadorSelecionadas();
                    }
                };
            }
            var chkTodosUpdate = document.getElementById('chkSelecionarTodosUpdate');
            if (chkTodosUpdate) {
                chkTodosUpdate.onchange = toggleSelecionarTodosUpdate;
                chkTodosUpdate.onclick = toggleSelecionarTodosUpdate;
            }
            var tabelaUpdate = document.querySelector('#panelAlteracoes .table-pendentes tbody');
            if (tabelaUpdate) {
                tabelaUpdate.onclick = function (e) {
                    if (e.target && e.target.type === 'checkbox' && !e.target.disabled) {
                        atualizarContadorUpdate();
                    }
                };
            }
        });

    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePartialRendering="true" />

    <div class="container-main">

        <%-- ── Mensagem de feedback ── --%>
        <asp:Panel ID="pnlMensagem" runat="server" Visible="false" CssClass="alert alert-info alert-dismissible mb-3">
            <asp:Label ID="lblMensagem" runat="server"></asp:Label>
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        </asp:Panel>

        <%-- ── Banner de pendentes ── --%>
        <asp:Panel ID="pnlAviso" runat="server" Visible="false" CssClass="banner-sync">
            <i class="bi bi-exclamation-triangle-fill"></i>
            <span class="msg">
                Há <strong><asp:Label ID="lblQtdPendentes" runat="server"></asp:Label></strong>
                plano(s) pendente(s) de confirmação.
            </span>
<asp:Button ID="btnSincronizar" runat="server"
    CssClass="btn-ver-pendentes"
    Text="Ver pendentes"
    OnClick="btnVerPendentes_Click" />
        </asp:Panel>

        <%-- ═══════════════════════════════════════════════════════════
             MODAL DE SINCRONIZAÇÃO (BOOTSTRAP)
        ═══════════════════════════════════════════════════════════ --%>
        <div class="modal fade" id="modalSyncPlanos" tabindex="-1"
            aria-labelledby="modalSyncLabel" aria-hidden="true">
            <div class="modal-dialog modal-xl modal-dialog-scrollable">
                <div class="modal-content">

                    <div class="modal-header modal-sync-header">
                        <h5 class="modal-title" id="modalSyncLabel">
                            <i class="bi bi-arrow-repeat me-2"></i>
                            Planos pendentes de sincronização
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
                                <span class="tab-sync-count">
                                    <asp:Label ID="lblQtdAbaNovas" runat="server">0</asp:Label>
                                </span>
                            </button>
                            <button type="button" class="tab-sync-btn tab-alteracoes" id="tabBtnAlteracoes" onclick="trocarAbaSync('alteracoes')">
                                <i class="bi bi-arrow-repeat"></i>
                                Alterações
                                <span class="tab-sync-count">
                                    <asp:Label ID="lblQtdAbaAlteracoes" runat="server">0</asp:Label>
                                </span>
                            </button>
                        </div>

                        <%-- ═══ PAINEL: NOVAS ═══ --%>
                        <div class="tab-sync-panel active" id="panelNovas">

                            <p class="text-muted mb-3" style="font-size: 13px;">
                                Os planos abaixo existem no sistema de origem (Aliança) mas
                                ainda <strong>não foram importados</strong> para esta base.
                            </p>

                            <div class="barra-selecao">
                                <label style="display: flex; align-items: center; gap: 6px; cursor: pointer; margin: 0;">
                                    <input type="checkbox" id="chkSelecionarTodos" checked
                                        style="width: 16px; height: 16px; cursor: pointer;" />
                                    Selecionar todos
                                </label>
                                <span>
                                    <strong>
                                        <asp:Label ID="lblQtdSelecionadas" runat="server">0</asp:Label>
                                    </strong>
                                    plano(s) selecionado(s)
                                </span>
                            </div>

                            <table class="table table-bordered table-hover table-pendentes">
                                <thead>
                                    <tr>
                                        <th class="col-check"></th>
                                        <th>Código</th>
                                        <th>Nome do Plano</th>
                                        <th>ANS</th>
                                        <th>Contratação</th>
                                        <th>Acomodação</th>
                                        <th>DecSau</th>
                                        <th>Promocional</th>
                                        <th>Operadora</th>

                                    </tr>
                                </thead>
                                <tbody>
                                    <asp:Repeater ID="rptPendentes" runat="server" OnItemDataBound="rptPendentes_ItemDataBound">
                                        <ItemTemplate>
                                            <tr>
                                                <td class="col-check">
                                                    <asp:HiddenField ID="hdnCodigoPlano" runat="server" Value='<%# Eval("CodigoPlano") %>' />
                                                    <asp:HiddenField ID="hdnRegistroANS" runat="server" Value='<%# Eval("RegistroANS") %>' />
                                                    <asp:HiddenField ID="hdnTipoContratacao" runat="server" Value='<%# Eval("TipoContratacaoDescricao") %>' />
                                                    <asp:HiddenField ID="hdnCodigoAbrangencia" runat="server" Value='<%# Eval("CodigoAbrangencia") %>' />
                                                    <asp:HiddenField ID="hdnCoparticipacao" runat="server" Value='<%# Eval("Coparticipacao") %>' />
                                                    <asp:HiddenField ID="hdnSegmentacao" runat="server" Value='<%# Eval("Segmentacao") %>' />
                                                    <asp:HiddenField ID="hdnAcomodacao" runat="server" Value='<%# Eval("AcomodacaoDescricao") %>' />
                                                    <asp:HiddenField ID="hdnConf_Ativo" runat="server" Value='<%# Eval("Conf_Ativo") %>' />
                                                    <asp:HiddenField ID="hdnNomePlano" runat="server" Value='<%# Eval("NomePlanoFamiliar") %>' />
                                                    <asp:HiddenField ID="hdnCodigoGrupoContrato" runat="server" Value='<%# Eval("CodigoGrupoContrato") %>' />
                                                    <asp:HiddenField ID="hdnNomeOperadora" runat="server" Value='<%# Eval("NomeOperadora") %>' />
                                                    <asp:HiddenField ID="hdnCnpjOperadora" runat="server" Value='<%# Eval("CnpjOperadora") %>' />
                                                    <asp:CheckBox ID="chkSelecionar" runat="server"
                                                        Checked="true"
                                                        CssClass="chk-linha-pendente" />
                                                </td>
                                                <td><%# Eval("CodigoPlano") %></td>
                                                <td>
                                                    <%# Eval("NomePlanoFamiliar") %>
                                                    <span class="badge-novo ms-1">NOVO</span>
                                                </td>
                                                <td><%# Eval("RegistroANS") %></td>
                                                <td><%# Eval("TipoContratacaoDescricao") %></td>
                                                <td><%# Eval("AcomodacaoDescricao") %></td>
                                               <td>
                                                    <asp:DropDownList ID="ddlDecSau" runat="server" 
                                                        CssClass="grid-dropdown grid-dropdown-dec">
                                                        <asp:ListItem Text="S" Value="S" Selected="True" />
                                                        <asp:ListItem Text="N" Value="N" />
                                                    </asp:DropDownList>
                                                </td>
                                                <td>
                                                    <asp:DropDownList ID="ddlPromocional" runat="server" 
                                                        CssClass="grid-dropdown grid-dropdown-promo">
                                                        <asp:ListItem Text="N" Value="N" Selected="True" />
                                                        <asp:ListItem Text="S" Value="S" />
                                                    </asp:DropDownList>
                                                </td>
                                                <td><%# Eval("NomeOperadora") %></td>
                                            </tr>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </tbody>
                            </table>
                        </div>

                        <%-- ═══ PAINEL: ALTERAÇÕES ═══ --%>
                        <div class="tab-sync-panel" id="panelAlteracoes">

                            <p class="text-muted mb-3" style="font-size: 13px;">
                                Os planos abaixo já existem nesta base, mas o sistema de origem (Aliança)
                                possui valores diferentes. Marque os que deseja atualizar.
                            </p>

                            <div class="barra-selecao">
                                <label style="display: flex; align-items: center; gap: 6px; cursor: pointer; margin: 0;">
                                    <input type="checkbox" id="chkSelecionarTodosUpdate" checked
                                        style="width: 16px; height: 16px; cursor: pointer;" />
                                    Selecionar todos
                                </label>
                                <span>
                                    <strong>
                                        <asp:Label ID="lblQtdSelecionadasUpdate" runat="server">0</asp:Label>
                                    </strong>
                                    plano(s) selecionado(s)
                                </span>
                            </div>

                            <table class="table table-bordered table-hover table-pendentes">
                                <thead>
                                    <tr>
                                        <th class="col-check"></th>
                                        <th>Código</th>
                                        <th>Plano</th>
                                        <th>Campo</th>
                                        <th>Valor Antigo</th>
                                        <th>Valor Novo</th>
                                    </tr>
                                </thead>
                                <tbody>
                                    <tr>
                                        <td colspan="6" class="text-center text-muted py-3">
                                            <i class="bi bi-inbox"></i> Nenhuma alteração pendente no momento.
                                        </td>
                                    </tr>
                                </tbody>
                            </table>

                        </div>

                    </div>

                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>

                        <asp:Button ID="btnConfirmarSync" runat="server"
                            CssClass="btn btn-success"
                            Text="✔ Confirmar e importar selecionados"
                            OnClick="btnConfirmarSync_Click"
                            OnClientClick="return confirmarImportacao();" />

                        <asp:Button ID="btnConfirmarUpdate" runat="server"
                            CssClass="btn btn-primary"
                            Text="✔ Confirmar e atualizar selecionados"
                            OnClick="btnConfirmarUpdate_Click"
                            OnClientClick="return confirmarAtualizacaoUpdate();"
                            Style="display: none;" />
                    </div>

                </div>
            </div>
        </div>

        <%-- ── Header ── --%>
        <div class="page-header">
            <h1 class="page-title">
                <span class="title-icon">
                    <i class="bi bi-card-list me-2"></i>
                </span>
                Cadastro de Planos
            </h1>
        </div>

        <%-- ── Filtros ── --%>
        <div class="filters-card">
            <div class="filters-header">
                <h3 class="filters-title">
                    <i class="bi bi-funnel"></i>
                    Filtros
                </h3>
            </div>
            <div class="filter-section">

                <div class="filter-item">
                    <label class="form-label">Plano comercial</label>
                    <asp:TextBox ID="txtNomePlanoComercial" runat="server" CssClass="form-control"
                        placeholder="Buscar..."></asp:TextBox>
                </div>

                <div class="filter-item">
                    <label class="form-label">Segmentação</label>
                    <asp:TextBox ID="txtSegmentacao" runat="server" CssClass="form-control"
                        placeholder="Ex: AHCO"></asp:TextBox>
                </div>

                <div class="filter-item">
                    <label class="form-label">Abrangência</label>
                    <asp:TextBox ID="txtAbrangencia" runat="server" CssClass="form-control"
                        placeholder="Ex: E"></asp:TextBox>
                </div>

                <div class="filter-item">
                    <label class="form-label">Coparticipação</label>
                    <asp:TextBox ID="txtCoparticipacao" runat="server" CssClass="form-control"
                        placeholder="Ex: C"></asp:TextBox>
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
                    <span class="legenda-cor laranja"></span>Inserido nas últimas 24h
                </span>
                <span class="legenda-item">
                    <span class="legenda-cor azul"></span>Atualizado recentemente
                </span>
            </div>

            <asp:GridView ID="gvPlanos" runat="server" CssClass="custom-grid"
                AutoGenerateColumns="False" AllowPaging="True" PageSize="20"
                OnRowDataBound="gvPlanos_RowDataBound"
                OnDataBound="gvPlanos_DataBound"
                OnPageIndexChanging="gvPlanos_PageIndexChanging"
                EmptyDataText="Nenhum plano encontrado no cadastro.">

                <Columns>
                    <asp:BoundField DataField="CodigoPlano" HeaderText="Código"
                        ItemStyle-CssClass="text-center col-id"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="NomePlanoComercial" HeaderText="Plano Comercial"
                        ItemStyle-CssClass="text-left col-plano"
                        HeaderStyle-CssClass="text-left" />

                    <asp:BoundField DataField="Segmentacao" HeaderText="Segmentação"
                        ItemStyle-CssClass="text-left col-segmentacao"
                        HeaderStyle-CssClass="text-left" />

                    <asp:BoundField DataField="Abrangencia" HeaderText="Abrangência"
                        ItemStyle-CssClass="text-left col-abrangencia"
                        HeaderStyle-CssClass="text-left" />

                    <asp:BoundField DataField="Coparticipacao" HeaderText="Coparticipação"
                        ItemStyle-CssClass="text-left col-coparticipacao"
                        HeaderStyle-CssClass="text-left" />

                    <asp:BoundField DataField="Acomodacao" HeaderText="Acomodação"
                        ItemStyle-CssClass="text-left col-acomodacao"
                        HeaderStyle-CssClass="text-left" />

                    <asp:BoundField DataField="DecSau" HeaderText="DecSau"
                        ItemStyle-CssClass="text-center col-decsau"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="Promocional" HeaderText="Promocional"
                        ItemStyle-CssClass="text-center col-promocional"
                        HeaderStyle-CssClass="text-center" />

                    <asp:TemplateField HeaderText="Status"
                        ItemStyle-CssClass="text-center col-status"
                        HeaderStyle-CssClass="text-center">
                        <ItemTemplate>
                            <span class='status-badge <%# Convert.ToString(Eval("Conf_Ativo")) == "S" ? "status-active" : "status-inactive" %>'>
                                <%# Convert.ToString(Eval("Conf_Ativo")) == "S" ? "Ativo" : "Inativo" %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>

                <PagerTemplate>
                    <div class="pager-custom">
                        <span class="pager-info">
                            <asp:Label ID="lblPagerInfo" runat="server" Text=""></asp:Label>
                        </span>
                        <span class="pager-buttons">
                            <asp:PlaceHolder ID="phPagerButtons" runat="server"></asp:PlaceHolder>
                        </span>
                    </div>
                </PagerTemplate>
                <HeaderStyle CssClass="grid-header" />
            </asp:GridView>
        </div>

    </div>
</asp:Content>