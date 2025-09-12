<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="detailDemand.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.detailDemand" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet">
    <style>
        :root {
            --primary: #1a73e8;
            --primary-hover: #0d62c9;
            --success: #4cb07a;
            --success-hover: #3b8b65;
            --danger: #ea4335;
            --warning: #fbbc04;
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

        .container-main {
            max-width: 2206px;
            margin: 20px auto;
            padding: 0 16px;
        }

        /* Cabeçalho da Demanda */
        .demand-header {
            background: white;
            border-radius: var(--border-radius);
            box-shadow: var(--shadow);
            padding: 24px;
            margin-bottom: 24px;
        }

        .demand-title {
            font-size: 24px;
            font-weight: 600;
            color: var(--gray-800);
            margin-bottom: 12px;
            padding-bottom: 16px;
            border-bottom: 1px solid var(--gray-200);
        }

        .demand-description {
            font-size: 16px;
            color: var(--gray-700);
            line-height: 1.6;
            margin-bottom: 20px;
        }

        .demand-meta {
            display: flex;
            flex-wrap: wrap;
            gap: 20px;
            margin-bottom: 16px;
        }

        .meta-item {
            display: flex;
            flex-direction: column;
        }

        .meta-label {
            font-size: 12px;
            color: var(--gray-600);
            margin-bottom: 4px;
        }

        .meta-value {
            font-size: 14px;
            font-weight: 500;
            color: var(--gray-800);
        }

        .status-badge {
            display: inline-block;
            padding: 6px 12px;
            border-radius: 16px;
            font-size: 14px;
            font-weight: 500;
            margin-left: 12px;
        }

        .status-open {
            background-color: #e6f4ea;
            color: #137333;
        }

        .status-closed {
            background-color: #fce8e6;
            color: #c5221f;
        }

        .btn-close-demand {
            background: var(--danger);
            border: none;
            color: white;
            font-weight: 500;
            padding: 10px 20px;
            border-radius: 4px;
            transition: var(--transition);
            margin-top: 16px;
        }


        .btn-close-demand:hover {
            background: #c5221f;
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.2);
        }

        .btn-secondary {
            background: var(--gray-400) !important;
            border-color: var(--gray-400) !important;
            cursor: not-allowed !important;
        }
        /* Layout Principal */
        .main-layout {
            display: grid;
            grid-template-columns: 1fr 350px;
            gap: 24px;
        }

        @media (max-width: 992px) {
            .main-layout {
                grid-template-columns: 1fr;
            }
        }

        /* Acompanhamentos */
        .accompaniments-section {
            background: white;
            border-radius: var(--border-radius);
            box-shadow: var(--shadow);
            overflow: hidden;
        }

        .section-header {
            padding: 16px 24px;
            background: var(--gray-50);
            border-bottom: 1px solid var(--gray-200);
            font-size: 18px;
            font-weight: 500;
            color: var(--gray-800);
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .accompaniments-list {
            padding: 16px;
            max-height: 500px;
            overflow-y: auto;
        }

        .accompaniment-item {
            background: var(--gray-50);
            padding: 16px;
            border-radius: var(--border-radius);
            margin-bottom: 16px;
            border-left: 4px solid var(--primary);
        }

        .accompaniment-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 12px;
        }

        .accompaniment-author {
            font-weight: 500;
            color: var(--gray-800);
        }

        .accompaniment-date {
            font-size: 12px;
            color: var(--gray-600);
        }

        .accompaniment-content {
            color: var(--gray-700);
            line-height: 1.5;
        }

        /* Editor de Texto */
        .editor-section {
            background: white;
            border-radius: var(--border-radius);
            box-shadow: var(--shadow);
        }

        .editor-container {
            padding: 16px;
        }

        .editor-toolbar {
            display: flex;
            gap: 8px;
            margin-bottom: 12px;
            padding-bottom: 12px;
            border-bottom: 1px solid var(--gray-200);
            flex-wrap: wrap;
        }

        .toolbar-btn {
            background: none;
            border: 1px solid var(--gray-300);
            border-radius: 4px;
            padding: 6px 10px;
            cursor: pointer;
            color: var(--gray-700);
            transition: var(--transition);
        }

        .toolbar-btn:hover {
            background: var(--gray-100);
        }

        .toolbar-btn.active {
            background: var(--primary);
            color: white;
            border-color: var(--primary);
        }

        .editor-textarea {
            width: 100%;
            min-height: 150px;
            border: 1px solid var(--gray-300);
            border-radius: 4px;
            padding: 12px;
            font-family: 'Roboto', sans-serif;
            font-size: 14px;
            resize: vertical;
            margin-bottom: 16px;
        }

        .editor-textarea:focus {
            outline: none;
            border-color: var(--primary);
            box-shadow: 0 0 0 2px rgba(26, 115, 232, 0.2);
        }

        .btn-send {
            background: var(--success);
            border: none;
            color: white;
            font-weight: 500;
            padding: 10px 20px;
            border-radius: 4px;
            transition: var(--transition);
            width: 100%;
        }

        .btn-send:hover {
            background: var(--success-hover);
        }

        .btn-send:disabled {
            background: var(--gray-400);
            cursor: not-allowed;
        }

        /* Estilos para quando a demanda está fechada */
        .demand-closed .editor-toolbar {
            opacity: 0.5;
            pointer-events: none;
        }
        
        .demand-closed .editor-textarea {
            background-color: var(--gray-100);
            color: var(--gray-500);
            pointer-events: none;
        }
        
        .demand-closed .btn-send {
            background: var(--gray-400);
            cursor: not-allowed;
        }

        /* Histórico (Collapsible) */
        .history-section {
            background: white;
            border-radius: var(--border-radius);
            box-shadow: var(--shadow);
            margin-top: 24px;
        }

        .history-header {
            padding: 16px 24px;
            background: var(--gray-50);
            border-bottom: 1px solid var(--gray-200);
            font-size: 16px;
            font-weight: 500;
            color: var(--gray-700);
            display: flex;
            align-items: center;
            justify-content: space-between;
            cursor: pointer;
        }

        .history-content {
            padding: 16px;
            display: none;
        }

        .history-item {
            padding: 12px;
            border-left: 3px solid var(--gray-300);
            margin-bottom: 12px;
        }

        .history-user {
            font-weight: 500;
            color: var(--gray-800);
        }

        .history-date {
            font-size: 12px;
            color: var(--gray-600);
            margin-left: 8px;
        }

        .history-change {
            margin-top: 8px;
            font-size: 14px;
            color: var(--gray-700);
        }

        .history-visible .history-content {
            display: block;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
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
                    <span class="meta-value"><asp:Label ID="lblSolicitante" runat="server" /></span>
                </div>
                <div class="meta-item">
                    <span class="meta-label">Data</span>
                    <span class="meta-value"><asp:Label ID="lblDataSolicitacao" runat="server" /></span>
                </div>
            </div>
            
            <asp:Button ID="btnEncerrar" runat="server" CssClass="btn-close-demand" 
                        Text="✖ Encerrar Demanda" OnClick="btnEncerrar_Click" />
        </div>

        <div class="main-layout">
            <!-- Seção Principal - Acompanhamentos -->
            <div class="accompaniments-section">
                <div class="section-header">
                    <i class="bi bi-chat-text"></i>
                    Acompanhamentos
                </div>
                <div class="accompaniments-list">
                    <asp:Repeater ID="rptAcompanhamentos" runat="server">
                        <ItemTemplate>
                            <div class="accompaniment-item">
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

            <!-- Seção Lateral - Editor de Texto -->
            <div class="editor-section" id="editorSection" runat="server">
                <div class="section-header">
                    <i class="bi bi-pencil-square"></i>
                    Novo Acompanhamento
                </div>
                <div class="editor-container">
                    <div class="editor-toolbar">
                        <button type="button" class="toolbar-btn" data-command="bold" title="Negrito">
                            <i class="bi bi-type-bold"></i>
                        </button>
                        <button type="button" class="toolbar-btn" data-command="italic" title="Itálico">
                            <i class="bi bi-type-italic"></i>
                        </button>
                        <button type="button" class="toolbar-btn" data-command="underline" title="Sublinhado">
                            <i class="bi bi-type-underline"></i>
                        </button>
                        <select class="toolbar-btn" id="fontFamily" title="Fonte">
                            <option value="Arial">Arial</option>
                            <option value="Helvetica">Helvetica</option>
                            <option value="Times New Roman">Times New Roman</option>
                            <option value="Courier New">Courier New</option>
                            <option value="Verdana">Verdana</option>
                        </select>
                    </div>
                    <asp:TextBox ID="txtNovoAcompanhamento" runat="server" 
                                CssClass="editor-textarea" TextMode="MultiLine" 
                                placeholder="Digite seu acompanhamento..." Rows="6" />
                    <asp:Button ID="btnAdicionarAcompanhamento" runat="server" 
                                CssClass="btn-send" Text="Enviar Acompanhamento" 
                                OnClick="btnAdicionarAcompanhamento_Click" />
                </div>
            </div>
        </div>

        <!-- Histórico de Status (Collapsible) -->
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
    
    <script>
        function checkDemandStatus() {
            const statusBadge = document.getElementById('<%= lblStatusBadge.ClientID %>');
          const editorSection = document.getElementById('<%= editorSection.ClientID %>');
    
            if (statusBadge && statusBadge.textContent.includes('Fechada')) {
                if (editorSection) {
                    editorSection.classList.add('demand-closed');
                    const textarea = document.getElementById('<%= txtNovoAcompanhamento.ClientID %>');
                    if (textarea) {
                        textarea.disabled = true;
                        textarea.placeholder = "Demanda fechada - não é possível adicionar acompanhamentos";
                    }
            
                          const button = document.getElementById('<%= btnAdicionarAcompanhamento.ClientID %>');
                          if (button) {
                              button.disabled = true;
                              button.textContent = "Demanda Fechada";
                              button.classList.add("btn-secondary");
                          }
                      }
                  }
              }

        // Toggle do histórico
        function toggleHistory() {
            const section = document.getElementById('historySection');
            const icon = document.getElementById('historyIcon');

            section.classList.toggle('history-visible');
            icon.classList.toggle('bi-chevron-down');
            icon.classList.toggle('bi-chevron-up');
        }

        // Editor de texto simples
        document.querySelectorAll('.toolbar-btn').forEach(button => {
            button.addEventListener('click', function () {
                if (this.tagName === 'BUTTON') {
                    const command = this.dataset.command;
                    document.execCommand(command, false, null);
                    this.classList.toggle('active');
                }
            });
        });

        document.getElementById('fontFamily').addEventListener('change', function () {
            document.execCommand('fontName', false, this.value);
        });

        // Executar quando a página carregar
        document.addEventListener('DOMContentLoaded', function () {
            checkDemandStatus();
        });
    </script>
</asp:Content>