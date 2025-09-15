﻿<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="demand.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.demand" %>

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
.file-item {
    border: 1px solid #dee2e6;
    border-radius: 4px;
    padding: 8px 12px;
    margin-bottom: 5px;
    background-color: #f8f9fa;
}

.file-item:hover {
    background-color: #e9ecef;
}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

   

<div class="modal fade" id="modalDemandas" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-lg">
        <div class="modal-content">
            <div class="modal-header bg-warning">
                <h5 class="modal-title text-white">
                    <asp:Literal ID="litTituloModal" runat="server"></asp:Literal>
                </h5>
                <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>

            <div class="modal-body">
                <asp:Literal ID="litTextoModal" runat="server"></asp:Literal>

             <asp:Repeater ID="rptDemandas" runat="server" OnItemDataBound="rptDemandas_ItemDataBound">
    <ItemTemplate>
        <div class="demanda-critica-item mb-3 p-3 border rounded">
            <div class="row">
                <div class="col-md-8">
                    <h6><%# Eval("Titulo") %></h6>
                    <div class="text-muted small">
                        Data: <%# Eval("DataDemanda", "{0:dd/MM/yyyy}") %> |
                        Prioridade: <span class="badge bg-info"><%# Eval("Prioridade") %></span>
                    </div>
                </div>
                <div class="col-md-4">
                    <asp:DropDownList ID="ddlNovaPrioridade" runat="server" CssClass="form-select form-select-sm">
                        <asp:ListItem Text="-- Selecionar nova prioridade --" Value="" />
                    </asp:DropDownList>
                    <asp:Button ID="btnAlterarPrioridade" runat="server" Text="Alterar Prioridade"
                        CssClass="btn btn-primary btn-sm mt-2"
                        CommandArgument='<%# Eval("CodDemanda") %>'
                        OnClick="btnAlterarPrioridade_Click" />
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

<!-- Botão oculto fallback (dispara modal via data-bs) -->
<button id="btnAbrirModalHidden" type="button" class="d-none" data-bs-toggle="modal" data-bs-target="#modalDemandas"></button>



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

               <!-- Anexos -->
                <div class="form-section">
                    <h3 class="section-title">
                        <i class="bi bi-paperclip section-icon"></i>
                        Anexos
                    </h3>
                    <div class="form-group">
                        <label class="form-label">Adicionar arquivos (Máx. 10MB cada)</label>
        
                        <!-- FileUpload ASP.NET -->
                        <asp:FileUpload ID="fuAnexos" runat="server" CssClass="form-control" AllowMultiple="true" />
                        <div class="input-hint">
                            Formatos permitidos: PDF, Word, Excel, imagens (JPG, PNG, GIF)
                        </div>
        
                        <!-- Lista de arquivos selecionados -->
                        <asp:Panel ID="pnlArquivosSelecionados" runat="server" CssClass="mt-3" Visible="false">
                            <h6>Arquivos selecionados:</h6>
                            <asp:Repeater ID="rptArquivos" runat="server">
                                <ItemTemplate>
                                    <div class="file-item d-flex justify-content-between align-items-center p-2 mb-2 bg-light rounded">
                                        <div>
                                            <i class="bi bi-file-earmark"></i>
                                            <asp:Label ID="lblNomeArquivo" runat="server" Text='<%# Eval("Nome") %>' CssClass="ms-2" />
                                            <small class="text-muted ms-2">(<%# Eval("Tamanho") %>)</small>
                                        </div>
                                        <asp:LinkButton ID="btnRemoverArquivo" runat="server" CssClass="text-danger" 
                                            CommandArgument='<%# Container.ItemIndex %>' OnClick="btnRemoverArquivo_Click">
                                            <i class="bi bi-x-circle"></i>
                                        </asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </asp:Panel>
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

    <script>
        // Debug: Verificar se Bootstrap está carregado
        console.log('Bootstrap carregado:', typeof bootstrap !== 'undefined');
        console.log('Modal element exists:', document.getElementById('modalDemandasCriticas') !== null);

        // Função global para testar abertura do modal manualmente
        function testarModal() {
            const modalElement = document.getElementById('modalDemandasCriticas');
            if (modalElement && typeof bootstrap !== 'undefined') {
                const modal = new bootstrap.Modal(modalElement);
                modal.show();
                console.log('Modal aberto manualmente');
            } else {
                console.error('Não foi possível abrir o modal');
            }
        }
    </script>
</asp:Content>