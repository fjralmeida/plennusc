<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="detailDemand.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.detailDemand" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

        <title>Detalhes da demanda</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet">

    <link href="../../Content/Css/projects/gestao/structuresCss/detailDemand.css" rel="stylesheet" />

<script type="text/javascript">
    (function () {
        'use strict';

        // Helpers
        function safeById(id) {
            try { return document.getElementById(id); } catch (e) { return null; }
        }
        function safeQuery(selector) {
            try { return document.querySelector(selector); } catch (e) { return null; }
        }
        function safeAddEvent(el, evt, fn) {
            if (!el) return;
            el.addEventListener(evt, fn);
        }

        // -------------------------
        // CONFIGURA CONTROLE BOTÕES
        // -------------------------
        function configurarControleBotoes() {
            var ddlStatus = safeById('<%= ddlStatusAcompanhamento.ClientID %>');
            var hdnStatusOriginal = safeById('<%= hdnStatusOriginal.ClientID %>');

            var btnSolicitarAprovacao = safeById('<%= btnSolicitarAprovacao.ClientID %>');
            var btnRecusar = safeById('<%= btnRecusar.ClientID %>');
            var btnEncerrar = safeById('<%= btnEncerrar.ClientID %>');
            var btnAdicionarAcompanhamento = safeById('<%= btnAdicionarAcompanhamento.ClientID %>');

            if (!ddlStatus || !hdnStatusOriginal) return;

            function atualizarBotoes() {
                var statusAtual = ddlStatus.value;
                var statusOriginal = hdnStatusOriginal.value || '';
                var textoStatusAtual = (ddlStatus.options[ddlStatus.selectedIndex] || {}).text || '';

                var habilitarSolicitar = (statusAtual !== statusOriginal) && textoStatusAtual === "Aguardando Aprovação";
                var habilitarRecusar = (statusAtual !== statusOriginal) && textoStatusAtual === "Recusada";
                var habilitarConcluir = (statusAtual !== statusOriginal) && textoStatusAtual === "Concluída";
                var desabilitarAcompanhamento = textoStatusAtual === "Aguardando Aprovação";

                function aplicar(btn, habilitar) {
                    if (!btn) return;
                    btn.disabled = !habilitar;
                    btn.style.opacity = habilitar ? '1' : '0.5';
                    btn.style.pointerEvents = habilitar ? 'auto' : 'none';
                }

                aplicar(btnSolicitarAprovacao, habilitarSolicitar);
                aplicar(btnRecusar, habilitarRecusar);
                aplicar(btnEncerrar, habilitarConcluir);

                if (btnAdicionarAcompanhamento) {
                    btnAdicionarAcompanhamento.disabled = desabilitarAcompanhamento;
                    btnAdicionarAcompanhamento.style.opacity = desabilitarAcompanhamento ? '0.5' : '1';
                    btnAdicionarAcompanhamento.style.pointerEvents = desabilitarAcompanhamento ? 'none' : 'auto';
                    btnAdicionarAcompanhamento.title = desabilitarAcompanhamento
                        ? "Status aguardando aprovação - acompanhamento bloqueado"
                        : "Enviar Acompanhamento";
                }
            }

            // inicial
            atualizarBotoes();

            // change
            safeAddEvent(ddlStatus, 'change', function () {
                try { atualizarBotoes(); } catch (e) { console.error(e); }
            });
        }

        // -------------------------
        // CHECK DEMAND STATUS
        // -------------------------
        function checkDemandStatus() {
            try {
                var statusBadge = safeById('<%= lblStatusBadge.ClientID %>');
                var editorSection = safeById('<%= editorSection.ClientID %>');
                if (!statusBadge || !editorSection) return;

                if ((statusBadge.textContent || '').includes('Fechada')) {
                    editorSection.classList.add('demand-closed');

                    var textarea = safeById('<%= txtNovoAcompanhamento.ClientID %>');
                    if (textarea) {
                        textarea.disabled = true;
                        textarea.placeholder = "Demanda fechada - não é possível adicionar acompanhamentos";
                    }

                    var button = safeById('<%= btnAdicionarAcompanhamento.ClientID %>');
                    if (button) {
                        button.disabled = true;
                        button.textContent = "Demanda Fechada";
                        button.classList.add("btn-secondary");
                    }

                    var fileUploadArea = safeQuery('.file-upload-area');
                    if (fileUploadArea) {
                        fileUploadArea.style.opacity = '0.5';
                        fileUploadArea.style.pointerEvents = 'none';
                    }
                }
            } catch (err) {
                console.error('checkDemandStatus error:', err);
            }
        }

        // -------------------------
        // TOGGLE HISTORY
        // -------------------------
        function toggleHistory() {
            var section = safeById('historySection');
            var icon = safeById('historyIcon');
            if (!section || !icon) return;
            section.classList.toggle('history-visible');
            icon.classList.toggle('bi-chevron-down');
            icon.classList.toggle('bi-chevron-up');
        }

        // -------------------------
        // TOOLBAR (delegation)
        // -------------------------
        function setupToolbarDelegation() {
            document.addEventListener('click', function (e) {
                var btn = e.target.closest && e.target.closest('.toolbar-btn');
                if (!btn) return;
                if (btn.tagName !== 'BUTTON' && btn.tagName !== 'A') return;

                var command = btn.getAttribute('data-command');
                if (!command) return;

                try {
                    document.execCommand(command, false, null);
                    btn.classList.toggle('active');
                } catch (ex) {
                    console.error('toolbar command failed:', command, ex);
                }
            });

            // font family change (safe)
            var fontSel = safeById('fontFamily');
            if (fontSel) {
                safeAddEvent(fontSel, 'change', function () {
                    try { document.execCommand('fontName', false, fontSel.value); } catch (e) { console.error(e); }
                });
            }
        }

        // -------------------------
        // Inicialização quando DOM pronto
        // -------------------------
        function init() {
            try {
                configurarControleBotoes();
                checkDemandStatus();
                setupToolbarDelegation();

                // Configura o toggle do histórico
                var historyHeader = safeQuery('.history-header');
                if (historyHeader) {
                    historyHeader.removeAttribute('onclick');
                    safeAddEvent(historyHeader, 'click', function () { toggleHistory(); });
                }
            } catch (e) {
                console.error('Erro na inicialização:', e);
            }
        }

        // Inicialização
        if (document.readyState === 'complete' || document.readyState === 'interactive') {
            setTimeout(init, 20);
        } else {
            document.addEventListener('DOMContentLoaded', init);
        }

        // Expor funções para uso global
        window.toggleHistory = toggleHistory;
        window.showToastSucesso = function (message) {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'success',
                title: message,
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true
            });
        };
        window.showToastErro = function (message) {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'error',
                title: message,
                showConfirmButton: false,
                timer: 4000,
                timerProgressBar: true
            });
        };
        window.showToastAviso = function (message) {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'warning',
                title: message,
                showConfirmButton: false,
                timer: 4000,
                timerProgressBar: true
            });
        };

    })();
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="toast-container" id="globalToastContainer" runat="server" style="display: none"></div>

    <div class="container-main">
        <!-- Cabeçalho da Demanda -->
        <div class="demand-header">
            <h1 class="demand-title">
                <asp:Label ID="lblTitulo" runat="server"></asp:Label>
                <asp:Label ID="lblStatusBadge" runat="server" CssClass="status-badge" />
            </h1>

            <div class="demand-description">
                <asp:Label ID="lblTexto" runat="server"></asp:Label>
            </div>

            <div class="demand-meta">
                <div class="meta-item">
                    <span class="meta-label">Solicitante</span>
                    <span class="meta-value">
                        <asp:Label ID="lblSolicitante" runat="server" /></span>
                </div>
                <div class="meta-item">
                    <span class="meta-label">Data</span>
                    <span class="meta-value">
                        <asp:Label ID="lblDataSolicitacao" runat="server" /></span>
                </div>
              <%--  <div class="meta-item">
                    <span class="meta-label">Status Atual</span>
                    <span class="meta-value">
                        <asp:Label ID="lblStatusAtual" runat="server" CssClass="current-status-value" />
                    </span>
                </div>--%>
            </div>
        </div>

        <asp:HiddenField ID="hdnStatusOriginal" runat="server" />

        <!-- Seção de Anexos EXISTENTES -->
        <div class="attachments-section">
            <div class="section-header">
                <i class="bi bi-paperclip"></i>
                Anexos da Demanda
            </div>
            <div class="attachments-list">
                <asp:Repeater ID="rptAnexos" runat="server">
                    <ItemTemplate>
                        <div class="attachment-item">
                            <div class="attachment-info">
                                <i class="bi bi-file-earmark attachment-icon"></i>
                                <div class="attachment-details">
                                    <div class="attachment-name"><%# Eval("NomeArquivo") %></div>
                                    <div class="attachment-meta">
                                        <%# Eval("DataEnvio", "{0:dd/MM/yyyy HH:mm}") %>
                                        • <%# Eval("TamanhoFormatado") %>
                                        • Enviado por: <strong><%# Eval("NomeUsuarioUpload") %></strong>
                                    </div>
                                </div>
                            </div>
                            <a href='<%# Eval("CaminhoDownload") %>' target="_blank" class="btn-download">
                                <i class="bi bi-download"></i>Download
                            </a>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Label ID="lblSemAnexos" runat="server" Text="Nenhum anexo encontrado."
                    CssClass="no-attachments" />
            </div>
        </div>

        <!-- Controle de Status (Agora sozinho) -->
        <div class="status-control-single">
            <div class="status-selector-single">
                <label class="status-label-single">
                    <i class="bi bi-arrow-repeat"></i>
                    Alterar Status:
                </label>
                <asp:DropDownList ID="ddlStatusAcompanhamento" runat="server" CssClass="status-dropdown-single">
                </asp:DropDownList>
            </div>
        </div>

        <div class="main-layout">
            <!-- Seção Principal - NOVO ACOMPANHAMENTO -->
            <div class="editor-section" id="editorSection" runat="server">
                <div class="section-header">
                    <i class="bi bi-pencil-square"></i>
                    Novo Acompanhamento
                </div>
                <div class="editor-container">
                    <asp:TextBox ID="txtNovoAcompanhamento" runat="server"
                        CssClass="editor-textarea" TextMode="MultiLine"
                        placeholder="Digite seu acompanhamento..." Rows="8" />

                    <!-- SEÇÃO DE UPLOAD DE ANEXOS PARA O ACOMPANHAMENTO -->
                    <div class="mb-4">
                        <h3 class="section-title"><i class="bi bi-paperclip"></i>Anexos</h3>
                        <div class="form-card">
                            <label class="form-label">Adicionar arquivos (Máx. 10MB cada)</label>

                            <div class="file-upload-area">
                                <i class="bi bi-cloud-arrow-up text-muted mb-2"></i>
                                <p class="mb-2 small text-muted">Selecione os arquivos e clique em Adicionar</p>
            
                                <!-- FILEUPLOAD VISÍVEL E SIMPLES -->
                                <asp:FileUpload ID="fuAnexos" runat="server" CssClass="form-control mb-2" AllowMultiple="true" />
            
                                <!-- BOTÃO para adicionar arquivos -->
                                <asp:Button ID="btnAdicionarAnexos" runat="server" Text="Adicionar Arquivos à Lista" 
                                    CssClass="btn btn-primary btn-sm" OnClick="btnAdicionarAnexos_Click" />
                            </div>

                            <div class="input-hint">
                                Formatos permitidos: PDF, Word, Excel, imagens (JPG, PNG, GIF)
                            </div>
   
                            <asp:Repeater ID="Repeater1" runat="server" OnItemCommand="rptAnexos_ItemCommand">
                                <HeaderTemplate>
                                    <div class="file-preview-container mt-3">
                                        <h6 class="small text-muted">Arquivos selecionados:</h6>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div class="file-preview-item">
                                        <div class="file-preview-info">
                                            <i class="bi bi-file-earmark"></i>
                                            <span class="file-preview-name"><%# Eval("FileName") %></span>
                                            <span class="file-preview-size">(<%# Eval("SizeFormatted") %>)</span>
                                        </div>
                                        <asp:LinkButton ID="btnRemoverAnexo" runat="server" 
                                            CommandName="Remover" 
                                            CommandArgument='<%# Eval("Index") %>'
                                            CssClass="file-preview-remove"
                                            OnClientClick="return confirm('Remover este arquivo?');">
                                            <i class="bi bi-x-circle"></i>
                                        </asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </div>
                                </FooterTemplate>
                            </asp:Repeater>
                        </div>
                    </div>


                    <asp:Button ID="btnAdicionarAcompanhamento" runat="server"
                        CssClass="btn-send" Text="Enviar Acompanhamento"
                        OnClick="btnAdicionarAcompanhamento_Click" />
                </div>
            </div>

        <!-- Seção Lateral - ACOMPANHAMENTOS EXISTENTES -->
   <!-- Coluna da Direita (Acompanhamentos + Histórico) -->
    <div class="side-sections-container">
        <!-- Seção de Acompanhamentos -->
        <div class="accompaniments-section">
            <div class="section-header">
                <i class="bi bi-chat-text"></i>
                Acompanhamentos
            </div>
            <div class="accompaniments-list">
                <asp:Repeater ID="rptAcompanhamentos" runat="server">
                    <ItemTemplate>
                        <div class="accompaniment-item <%# IsMyMessage(Convert.ToInt32(Eval("CodPessoaAcompanhamento"))) ? "my-message" : "other-message" %>">
                            <div class="accompaniment-header">
                                <span class="accompaniment-author"><%# Eval("Autor") %></span>
                                <span class="accompaniment-date"><%# Eval("DataAcompanhamento", "{0:dd/MM/yyyy HH:mm}") %></span>
                            </div>
                            <div class="accompaniment-content">
                                <%# Eval("TextoAcompanhamento") %>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- Histórico de Status (Agora ao lado dos acompanhamentos) -->
        <div class="history-section" id="historySection">
            <div class="history-header" onclick="toggleHistory()">
                <span>
                    <i class="bi bi-clock-history"></i>
                    Histórico de Alterações de Status
                </span>
                <i class="bi bi-chevron-down" id="historyIcon"></i>
            </div>
            <div class="history-content">
                <asp:Repeater ID="rptHistorico" runat="server">
                    <ItemTemplate>
                        <div class="history-item">
                            <div>
                                <span class="history-user"><%# Eval("Usuario") %></span>
                                <span class="history-date"><%# Eval("DataAlteracao", "{0:dd/MM/yyyy HH:mm}") %></span>
                            </div>
                            <div class="history-change">
                                <%# Eval("SituacaoAnterior") %> → <%# Eval("SituacaoAtual") %>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
    
    </div>
        <!-- Botões de Ação - AGORA NO FINAL LADO DIREITO -->
    <div class="final-actions-container">
        <div class="final-buttons-group">
            <asp:LinkButton ID="btnRecusar" runat="server" CssClass="btn-final btn-refuse"
                OnClick="btnRecusar_Click" Visible="false">
                <i class="bi bi-x-circle"></i> Recusar
            </asp:LinkButton>

            <asp:LinkButton ID="btnSolicitarAprovacao" runat="server" CssClass="btn-final btn-primary"
                OnClick="btnSolicitarAprovacao_Click" Visible="false">
                <i class="bi bi-check-circle"></i> Solicitar Aprovação
            </asp:LinkButton>

            <asp:LinkButton ID="btnEncerrar" runat="server" CssClass="btn-final btn-close-demand"
                OnClick="btnEncerrar_Click" Visible="false">
                <i class="bi bi-check-lg"></i> Concluir
            </asp:LinkButton>
        </div>
    </div>
  </div> 
</asp:Content>
