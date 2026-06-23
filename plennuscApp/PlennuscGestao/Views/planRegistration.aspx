<%@ Page Title="" Language="C#"
    MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master"
    AutoEventWireup="true"
    CodeBehind="planRegistration.aspx.cs"
    Inherits="appWhatsapp.PlennuscGestao.Views.planRegistration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Lista de planos</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet">
    <link href="../../Content/Css/projects/gestao/structuresCss/PlanRegistration/planRegistratrion.css" rel="stylesheet" />.

        <script type="text/javascript">
        // ============================================================
        //  FUNÇÕES PARA A ABA DE NOVAS (INSERÇÃO) - JÁ FUNCIONA
        // ============================================================

        function getCheckboxesLinhas() {
            var tabela = document.querySelector('.table-pendentes tbody');
            if (!tabela) return [];
            return tabela.querySelectorAll('input[type="checkbox"]:not(:disabled)');
        }

       <%-- function atualizarContadorSelecionadas() {
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
        }--%>

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
            return confirm('Confirma a importação das ' + marcados + ' plano(s) selecionada(s)?');
        }

        // ============================================================
        //  FUNÇÕES PARA A ABA DE ALTERAÇÕES (ATUALIZAÇÃO) - IGUAL À DE CIMA
        // ============================================================

        function getCheckboxesUpdate() {
            var tabela = document.querySelector('#panelAlteracoes .table-pendentes tbody');
            if (!tabela) return [];
            return tabela.querySelectorAll('input[type="checkbox"]:not(:disabled)');
        }

      <%--  function atualizarContadorUpdate() {
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
        }--%>

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

      <%--  function trocarAbaSync(aba) {
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
        }--%>

        function abrirModalSync() {
            var modalEl = document.getElementById('modalSyncPlano');
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
              <span class="msg">Há <strong>
                  <asp:Label ID="lblQtdPendentes" runat="server"></asp:Label>
                  planos(s)
              </strong>no sistema de origem pendente(s) de confirmação.
              </span>
              <asp:Button ID="btnVerPendentes" runat="server"
                  CssClass="btn-ver-pendentes"
                  Text="Ver pendentes"
                  OnClick="btnVerPendentes_Click" />
          </asp:Panel>

        <!-- Filtros -->
        <div class="filters-card">
            <h3 class="filters-title">
                <i class="bi bi-funnel"></i>
                Filtros
            </h3>
            <div class="filter-section">

                <div class="filter-item">
                    <label class="form-label">Plano</label>
                    <asp:TextBox ID="txtNomePlanoComercial" runat="server" CssClass="form-control"
                        placeholder="Nome do Plano"></asp:TextBox>
                </div>

                <div class="filter-item">
                    <label class="form-label">Segmentação</label>
                    <asp:TextBox ID="txtSegmentacao" runat="server" CssClass="form-control"
                        placeholder="A, H, O, OB"></asp:TextBox>
                </div>

                <div class="filter-item">
                    <label class="form-label">Abrangência</label>
                    <asp:TextBox ID="txtAbrangencia" runat="server" CssClass="form-control"
                        placeholder="Nacional, Estadual, Municipal..."></asp:TextBox>
                </div>

                <div class="filter-item">
                    <label class="form-label">Coparticipação</label>
                    <asp:TextBox ID="txtCoparticipacao" runat="server" CssClass="form-control"
                        placeholder="Com ou sem"></asp:TextBox>
                </div>

                <div class="btn-filter-container">
                    <asp:Button ID="btnFiltrar" runat="server" CssClass="btn-filter"
                        Text="Aplicar Filtros" OnClick="btnFiltrar_Click" />
                </div>

            </div>

        </div>

        <div class="grid-container">
            <asp:GridView ID="gvPlanos" runat="server" CssClass="custom-grid"
                AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
                OnPageIndexChanging="gvPlanos_PageIndexChanging"
                OnRowCommand="gvPlanos_RowCommand"
                OnDataBound="gvPlanos_DataBound"
                OnRowDataBound="gvPlanos_RowDataBound"
                EmptyDataText="Nenhum plano encontrado no cadastro.">

                <Columns>
                    <asp:BoundField DataField="CodigoPlano" HeaderText="ID"
                        ItemStyle-CssClass="text-center col-codigoplano"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="CodigoProduto" HeaderText="Código Produto"
                        ItemStyle-CssClass="text-center col-codigoproduto"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="Num_CNPJ_Operadora" HeaderText="CNPJ Operadora"
                        ItemStyle-CssClass="text-left col-cnpj"
                        HeaderStyle-CssClass="text-left" />

                    <asp:BoundField DataField="TipoContratacao" HeaderText="Tipo Contratação"
                        ItemStyle-CssClass="text-center col-tipocontratacao"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="NomePlanoComercial" HeaderText="Nome Plano"
                        ItemStyle-CssClass="text-left col-nomecomercial"
                        HeaderStyle-CssClass="text-left" />

                    <asp:BoundField DataField="Segmentacao" HeaderText="Segmentação"
                        ItemStyle-CssClass="text-center col-segmentacao"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="Abrangencia" HeaderText="Abrangência"
                        ItemStyle-CssClass="text-center col-abrangencia"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="Coparticipacao" HeaderText="Coparticipação"
                        ItemStyle-CssClass="text-center col-coparticipacao"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="Acomodacao" HeaderText="Acomodação"
                        ItemStyle-CssClass="text-center col-acomodacao"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="DecSau" HeaderText="Declaração de Saúde"
                        ItemStyle-CssClass="text-center col-decsau"
                        HeaderStyle-CssClass="text-center" />

                    <asp:BoundField DataField="Promocional" HeaderText="Promocional"
                        ItemStyle-CssClass="text-center col-promocional"
                        HeaderStyle-CssClass="text-center" />

                    <asp:TemplateField HeaderText="Ativo ou Inativo"
                        ItemStyle-CssClass="text-center col-confativo"
                        HeaderStyle-CssClass="text-center">
                        <ItemTemplate>
                            <%# ((bool)Eval("Conf_Ativo")) ? "Ativo" : "Inativo" %>
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
