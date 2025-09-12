<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="demand.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.demand" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>


    <style>
        :root {
            --primary: #83ceee;
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

        /* Card principal */
        .demand-card {
            background: white;
            border-radius: var(--border-radius);
            box-shadow: var(--shadow);
            overflow: hidden;
            display: flex;
            flex-direction: column;
        }

        /* Cabeçalho */
        .demand-header {
            padding: 16px 24px;
            border-bottom: 1px solid var(--gray-200);
            display: flex;
            align-items: center;
            justify-content: space-between;
            background: white;
        }

        .demand-title {
            display: flex;
            align-items: center;
            gap: 12px;
            font-size: 18px;
            font-weight: 500;
            color: var(--gray-800);
            margin: 0;
        }

        .title-icon {
            background: var(--primary);
            color: white;
            width: 40px;
            height: 40px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .btn-save {
            background: var(--success);
            border: none;
            color: white;
            font-weight: 500;
            padding: 10px 24px;
            border-radius: 4px;
            transition: var(--transition);
            cursor: pointer;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .btn-save:hover {
            background: var(--success-hover);
            box-shadow: 0 1px 2px rgba(0, 0, 0, 0.3);
        }

        /* Corpo do formulário */
        .demand-body {
            padding: 24px;
            flex: 1;
        }

        .form-section {
            margin-bottom: 32px;
        }

        .section-title {
            font-size: 16px;
            font-weight: 500;
            color: var(--gray-700);
            margin-bottom: 16px;
            padding-bottom: 8px;
            border-bottom: 1px solid var(--gray-200);
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .section-icon {
            color: var(--gray-600);
        }

        /* Campos de formulário */
        .form-group {
            margin-bottom: 20px;
        }

        .form-label {
            display: block;
            font-size: 14px;
            font-weight: 500;
            color: var(--gray-700);
            margin-bottom: 8px;
        }

        .form-control, .form-select {
            width: 100%;
            padding: 12px 14px;
            border: 1px solid var(--gray-300);
            border-radius: 4px;
            font-size: 14px;
            transition: var(--transition);
            background: white;
        }

        .form-control:focus, .form-select:focus {
            outline: none;
            border-color: var(--primary);
            box-shadow: 0 0 0 2px rgba(26, 115, 232, 0.2);
        }

        textarea.form-control {
            min-height: 120px;
            resize: vertical;
        }

        .input-hint {
            font-size: 12px;
            color: var(--gray-600);
            margin-top: 6px;
        }

        .counter {
            font-size: 12px;
            color: var(--gray-500);
        }

        .row {
            display: flex;
            flex-wrap: wrap;
            margin: 0 -10px;
        }

        .col {
            flex: 1;
            padding: 0 10px;
            min-width: 250px;
            margin-bottom: 10px;
        }

        /* Rodapé com botão */
        .demand-footer {
            padding: 16px 24px;
            border-top: 1px solid var(--gray-200);
            background: white;
            display: flex;
            justify-content: flex-end;
        }

        /* Mensagens */
        .msg-area {
            border-radius: 4px;
            font-size: 14px;
        }

        .msg-success {
            background-color: #e6f4ea;
            color: #137333;
            border: 1px solid #b6e4c3;
        }

        .msg-error {
            background-color: #fce8e6;
            color: #c5221f;
            border: 1px solid #f5b5b2;
        }

        /* Responsividade */
        @media (max-width: 768px) {
            .demand-header {
                flex-direction: column;
                align-items: flex-start;
                gap: 16px;
            }
            
            .demand-footer {
                justify-content: stretch;
            }
            
            .btn-save {
                width: 100%;
                justify-content: center;
            }
            
            .col {
                flex: 100%;
            }
        }

        /* Toast (SweetAlert2) personalizado */
        .swal2-container{ z-index:3000 !important; }
        .swal2-container.swal2-top-end{ top:14px !important; right:14px !important; }
        .toast-warn{
            background:#fff !important; 
            color:#374151 !important;
            border:1px solid #e5e7eb !important;
            border-left:6px solid #dc3545 !important;
            border-radius:12px !important; 
            box-shadow:0 10px 24px rgba(0,0,0,.08) !important; 
            padding:10px 14px !important;
            width: 380px !important;
        }
       /* Toast de Sucesso */
        .toast-success {
            background: #fff !important; 
            color: #374151 !important;
            border: 1px solid #e5e7eb !important;
            border-left: 6px solid #28a745 !important;
            border-radius: 12px !important; 
            box-shadow: 0 10px 24px rgba(0,0,0,.08) !important; 
            padding: 10px 14px !important;
        }
        .toast-success .swal2-title { 
            font-size: 16px; 
            font-weight: 700; 
            margin: 0 0 4px; 
            color: #28a745;
        }
        .toast-success .swal2-html-container { 
            font-size: 14px; 
            margin: 0; 
            opacity: .95; 
            line-height: 1.4;
        }
        .toast-success .swal2-timer-progress-bar { 
            background: #28a745 !important; 
            height: 3px;
        }

        /* Toast de Alerta */
        .toast-warn {
            background: #fff !important; 
            color: #374151 !important;
            border: 1px solid #e5e7eb !important;
            border-left: 6px solid #ffc107 !important;
            border-radius: 12px !important; 
            box-shadow: 0 10px 24px rgba(0,0,0,.08) !important; 
            padding: 10px 14px !important;
        }
        .toast-warn .swal2-title { 
            font-size: 16px; 
            font-weight: 700; 
            margin: 0 0 4px; 
            color: #ffc107;
        }
                .demanda-critica-item {
            background-color: #fff3cd;
            border-color: #ffeaa7 !important;
        }
.demanda-critica-item {
    background-color: #fff3cd;
    border-color: #ffeaa7 !important;
    transition: all 0.2s ease;
}

.demanda-critica-item:hover {
    background-color: #ffecb5;
    transform: translateY(-1px);
}

.demanda-critica-item h6 {
    color: #856404;
    margin-bottom: 0.5rem;
    font-weight: 600;
}

.modal-header.bg-warning {
    background: linear-gradient(135deg, #ffc107 0%, #ff9800 100%) !important;
}

/* Animações suaves para o modal */
.modal.fade .modal-dialog {
    transform: translate(0, -50px);
    transition: transform 0.3s ease-out;
}

.modal.show .modal-dialog {
    transform: translate(0, 0);
}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <%--<asp:Panel ID="pnlDemandasCriticas" runat="server" Visible="false" CssClass="mt-4">
        <div class="card">
            <div class="card-header bg-warning text-white">
                <h5 class="mb-0">
                    <i class="bi bi-exclamation-triangle"></i>
                    Você possui demandas críticas em aberto
                </h5>
            </div>
            <div class="card-body">
                <p>Para criar uma nova demanda crítica, você precisa fechar ou alterar a situação de uma das demandas existentes:</p>
            
                <asp:Repeater ID="rptDemandasCriticas" runat="server" OnItemDataBound="rptDemandasCriticas_ItemDataBound">
                    <ItemTemplate>
                        <div class="demanda-critica-item mb-3 p-3 border rounded">
                            <div class="row">
                                <div class="col-md-8">
                                    <h6><%# Eval("Titulo") %></h6>
                                    <div class="text-muted small">
                                        Data: <%# Eval("DataDemanda", "{0:dd/MM/yyyy}") %> | 
                                        Situação: <span class="badge bg-info"><%# Eval("Situacao") %></span>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <asp:DropDownList ID="ddlSituacao" runat="server" CssClass="form-select form-select-sm">
                                        <asp:ListItem Text="-- Selecionar --" Value="" Selected="True" />
                                    </asp:DropDownList>
                                    <asp:Button ID="btnAlterarSituacao" runat="server" Text="Alterar Situação"
                                        CssClass="btn btn-primary btn-sm mt-2" 
                                        CommandArgument='<%# Eval("CodDemanda") %>'
                                        OnClick="btnAlterarSituacao_Click" />
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
            </asp:Repeater>
            </div>
        </div>
    </asp:Panel>--%>

    <!-- Modal para Demandas Críticas -->
<div class="modal fade" id="modalDemandasCriticas" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header bg-warning">
                <h5 class="modal-title text-white">
                    <i class="bi bi-exclamation-triangle"></i>
                    Você possui demandas críticas em aberto
                </h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
                <p>Para criar uma nova demanda crítica, você precisa fechar ou alterar a situação de uma das demandas existentes:</p>
                
                <asp:Repeater ID="rptDemandasCriticas" runat="server" OnItemDataBound="rptDemandasCriticas_ItemDataBound">
                    <ItemTemplate>
                        <div class="demanda-critica-item mb-3 p-3 border rounded">
                            <div class="row">
                                <div class="col-md-8">
                                    <h6><%# Eval("Titulo") %></h6>
                                    <div class="text-muted small">
                                        Data: <%# Eval("DataDemanda", "{0:dd/MM/yyyy}") %> | 
                                        Situação: <span class="badge bg-info"><%# Eval("Situacao") %></span>
                                    </div>
                                </div>
                                <div class="col-md-4">
                                    <asp:DropDownList ID="ddlSituacao" runat="server" CssClass="form-select form-select-sm">
                                        <asp:ListItem Text="-- Selecionar --" Value="" Selected="True" />
                                    </asp:DropDownList>
                                    <asp:Button ID="btnAlterarSituacao" runat="server" Text="Alterar Situação"
                                        CssClass="btn btn-primary btn-sm mt-2" 
                                        CommandArgument='<%# Eval("CodDemanda") %>'
                                        OnClick="btnAlterarSituacao_Click" />
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Fechar</button>
            </div>
        </div>
    </div>
</div>

    <div class="container-main">
        <div class="demand-card">
            <div class="demand-header">
                <h1 class="demand-title">
                    <span class="title-icon">
                        <i class="bi bi-clipboard-plus"></i>
                    </span>
                    Criar Nova Demanda
                </h1>
            </div>

            <div class="demand-body">
                <!-- Mensagens -->
                <asp:Label ID="lblMsg" runat="server" CssClass="msg-area d-block" EnableViewState="false" />

                <!-- Objetivo -->
                <div class="form-section">
                    <h3 class="section-title">
                        <i class="bi bi-bullseye section-icon"></i>
                        Objetivo
                    </h3>
                    <div class="row">
                        <div class="col">
                            <div class="form-group">
                                <label class="form-label">Título *</label>
                                <asp:TextBox ID="txtTitulo" runat="server" CssClass="form-control" MaxLength="100"
                                    placeholder="Ex: Correção de relatório financeiro" />
                                <div class="input-hint">
                                    <span class="counter" id="cntTitulo">0/100</span> caracteres
                                </div>
                            </div>
                        </div>
                       <asp:DropDownList ID="ddlPrioridade" runat="server" CssClass="form-select" 
                            AutoPostBack="true" OnSelectedIndexChanged="ddlPrioridade_SelectedIndexChanged">
                            <asp:ListItem Text="Selecione a prioridade" Value="" />
                        </asp:DropDownList>

                        <div class="col" id="divPrazo" runat="server" style="display: none;">
                            <div class="form-group">
                                <label class="form-label">Prazo *</label>
                                <input type="date" id="txtPrazo" runat="server" class="form-control" />
                            </div>
                        </div>

                        <asp:Label ID="Label1" runat="server" CssClass="text-danger d-block mb-3" />
                    </div>
                </div>

                <!-- Roteamento -->
                <div class="form-section">
                    <h3 class="section-title">
                        <i class="bi bi-signpost section-icon"></i>
                        Roteamento
                    </h3>
                    <div class="row">
                        <div class="col">
                            <div class="form-group">
                                <label class="form-label">Setor de Origem *</label>
                                <asp:DropDownList ID="ddlOrigem" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="Selecione o setor de origem" Value="" />
                                    <asp:ListItem Text="Financeiro" Value="1" />
                                    <asp:ListItem Text="RH" Value="2" />
                                    <asp:ListItem Text="TI" Value="3" />
                                    <asp:ListItem Text="Operações" Value="4" />
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col">
                            <div class="form-group">
                                <label class="form-label">Setor de Destino *</label>
                                <asp:DropDownList ID="ddlDestino" runat="server" CssClass="form-select" 
                                    AutoPostBack="true" 
                                    OnSelectedIndexChanged="ddlDestino_SelectedIndexChanged">
                                    <asp:ListItem Text="Selecione o setor de destino" Value="" />
                                    <asp:ListItem Text="Financeiro" Value="1" />
                                    <asp:ListItem Text="RH" Value="2" />
                                    <asp:ListItem Text="TI" Value="3" />
                                    <asp:ListItem Text="Operações" Value="4" />
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>

               <!-- Categoria -->
                <div class="form-section">
                    <h3 class="section-title">
                        <i class="bi bi-tag section-icon"></i>
                        Categoria
                    </h3>
                    <div class="row">
                        <div class="col">
                            <div class="form-group">
                                <label class="form-label">Tipo de Categoria *</label>
                                <asp:DropDownList ID="ddlTipoGrupo" runat="server" CssClass="form-select" 
                                    AutoPostBack="true" 
                                    OnSelectedIndexChanged="ddlTipoGrupo_SelectedIndexChanged">
                                    <asp:ListItem Text="Selecione uma categoria" Value="" />
                                    <asp:ListItem Text="Tecnologia - Suporte" Value="1" />
                                    <asp:ListItem Text="Financeiro - Relatórios" Value="2" />
                                    <asp:ListItem Text="RH - Documentação" Value="3" />
                                    <asp:ListItem Text="Operações - Processos" Value="4" />
                                </asp:DropDownList>
                                <div class="input-hint">Ex.: "Tecnologia - Suporte"</div>
                            </div>
                        </div>
                        <div class="col">
                            <div class="form-group">
                                <label class="form-label">Subtipo</label>
                                <asp:DropDownList ID="ddlTipoDetalhe" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="Selecione um subtipo" Value="" />
                                    <asp:ListItem Text="Criação Cx. Postal (email)" Value="1" />
                                    <asp:ListItem Text="Ajuste de permissões" Value="2" />
                                    <asp:ListItem Text="Correção de dados" Value="3" />
                                    <asp:ListItem Text="Atualização de sistema" Value="4" />
                                </asp:DropDownList>
                                <div class="input-hint">Ex.: "Criação Cx. Postal (email)"</div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Detalhes -->
                <div class="form-section">
                    <h3 class="section-title">
                        <i class="bi bi-text-paragraph section-icon"></i>
                        Detalhes
                    </h3>
                    <div class="form-group">
                        <label class="form-label">Descrição *</label>
                        <asp:TextBox ID="txtDescricao" runat="server" CssClass="form-control" TextMode="MultiLine"
                            placeholder="Descreva detalhadamente o que precisa ser feito, o contexto, os critérios de aceitação e qualquer informação relevante..." />
                        <div class="input-hint">
                            <span class="counter" id="cntDesc">0</span> caracteres
                        </div>
                    </div>
                </div>
            </div>
            
            <!-- Rodapé com botão alinhado à direita -->
            <div class="demand-footer">
                <asp:Button ID="btnSalvar" runat="server" Text="Criar Demanda" CssClass="btn-save" OnClick="btnSalvar_Click" />
            </div>
        </div>
    </div>

    <script>
        // Verificar se jQuery já está carregado, se não, carregar dinamicamente
        if (typeof jQuery === 'undefined') {
            console.log('jQuery não encontrado, carregando...');
            var script = document.createElement('script');
            script.src = 'https://code.jquery.com/jquery-3.6.0.min.js';
            script.onload = function () {
                console.log('jQuery carregado com sucesso');
                // Agora carregar Bootstrap que depende do jQuery
                var bootstrapScript = document.createElement('script');
                bootstrapScript.src = 'https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js';
                document.head.appendChild(bootstrapScript);
            };
            document.head.appendChild(script);
        } else {
            console.log('jQuery já está carregado');
        }
    </script>

    <script>
        // Contador de caracteres
        function setupCharacterCounters() {
            const titulo = document.getElementById('<%= txtTitulo.ClientID %>');
            const desc = document.getElementById('<%= txtDescricao.ClientID %>');
            const cntTitulo = document.getElementById('cntTitulo');
            const cntDesc = document.getElementById('cntDesc');

            function updateCounters() {
                if (titulo && cntTitulo) {
                    cntTitulo.textContent = (titulo.value || '').length;
                }
                if (desc && cntDesc) {
                    cntDesc.textContent = (desc.value || '').length;
                }
            }

            if (titulo) {
                titulo.addEventListener('input', updateCounters);
            }
            if (desc) {
                desc.addEventListener('input', updateCounters);
            }

            // Inicializar contadores
            updateCounters();
        }

        // Auto-ajuste da altura da textarea
        function autoResizeTextarea() {
            const textarea = document.getElementById('<%= txtDescricao.ClientID %>');
            if (textarea) {
                textarea.addEventListener('input', function () {
                    this.style.height = 'auto';
                    this.style.height = (this.scrollHeight) + 'px';
                });

                // Inicializar altura
                setTimeout(() => {
                    textarea.style.height = 'auto';
                    textarea.style.height = (textarea.scrollHeight) + 'px';
                }, 0);
            }
        }

        // Inicializar quando a página carregar
        document.addEventListener('DOMContentLoaded', function () {
            setupCharacterCounters();
            autoResizeTextarea();
        });
    </script>
</asp:Content>