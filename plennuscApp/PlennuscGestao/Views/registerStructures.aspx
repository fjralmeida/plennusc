<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="registerStructures.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.registerStructures" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <style>
        :root {
            --primary: #83ceee;
            --primary-hover: #0d62c9;
            --success: #4cb07a;
            --success-hover: #3b8b65;
            --warning: #ffa726;
            --warning-hover: #f57c00;
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

        /* Header */
        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 24px;
            flex-wrap: wrap;
            gap: 16px;
        }

        .page-title {
            display: flex;
            align-items: center;
            gap: 12px;
            font-size: 24px;
            font-weight: 500;
            color: var(--gray-800);
            margin: 0;
        }

        .title-icon {
            background: var(--primary);
            color: white;
            width: 44px;
            height: 44px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .btn-primary {
            background: var(--success);
            border: none;
            color: white;
            font-weight: 500;
            padding: 10px 20px;
            border-radius: 4px;
            transition: var(--transition);
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .btn-primary:hover {
            background: var(--success-hover);
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.2);
        }

        /* Card Principal */
        .main-card {
            background: white;
            border-radius: var(--border-radius);
            box-shadow: var(--shadow);
            margin-bottom: 24px;
            overflow: hidden;
        }

        .card-header {
            background: var(--gray-50);
            padding: 20px 24px;
            border-bottom: 1px solid var(--gray-200);
        }

        .card-title {
            font-size: 18px;
            font-weight: 500;
            color: var(--gray-800);
            margin: 0;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .card-body {
            padding: 24px;
        }

        /* Formulário */
        .form-group {
            margin-bottom: 20px;
        }

        .form-label {
            display: block;
            font-size: 14px;
            font-weight: 500;
            color: var(--gray-700);
            margin-bottom: 8px;
            white-space: nowrap;
        }

        .form-control, .form-select {
            width: 100%;
            padding: 10px 12px;
            border: 1px solid var(--gray-300);
            border-radius: 4px;
            font-size: 14px;
            transition: var(--transition);
            background: white;
            height: 40px;
            box-sizing: border-box;
        }

        .form-control:focus, .form-select:focus {
            outline: none;
            border-color: var(--primary);
            box-shadow: 0 0 0 2px rgba(26, 115, 232, 0.2);
        }

        /* Alertas */
        .alert {
            padding: 16px;
            border-radius: 4px;
            margin-bottom: 20px;
            border-left: 4px solid;
        }

        .alert-info {
            background: #e3f2fd;
            border-color: #2196f3;
            color: #1565c0;
        }

        /* Subtipos Dinâmicos */
        .subtipos-container {
            border: 1px solid var(--gray-200);
            border-radius: 4px;
            padding: 16px;
            background: var(--gray-50);
        }

        /* Ações dos subtipos */
        .subtipo-actions {
            display: flex;
            gap: 4px;
        }

        .btn-add-small {
            background: var(--success);
            border: none;
            color: white;
            width: 32px;
            height: 32px;
            border-radius: 4px;
            display: flex;
            align-items: center;
            justify-content: center;
            cursor: pointer;
            transition: var(--transition);
        }

        .btn-add-small:hover {
            background: var(--success-hover);
            transform: scale(1.05);
        }

        .subtipo-item {
            display: flex;
            gap: 12px;
            align-items: center;
            margin-bottom: 12px;
            padding: 8px;
            background: white;
            border-radius: 4px;
            border: 1px solid var(--gray-200);
        }

        .subtipo-input {
            flex: 1;
        }

        .subtipo-default {
    display: flex;
    align-items: center;
    gap: 6px;
    min-width: 100px;
}

.subtipo-default label {
    font-size: 12px;
    font-weight: 500;
    color: var(--gray-700);
    white-space: nowrap;
}

.subtipo-ordem {
    width: 80px;
}

.subtipo-actions {
    display: flex;
    gap: 4px;
}

/* Checkbox customizado para os subtipos */
.subtipo-default .form-check-input-custom {
    width: 16px !important;
    height: 16px !important;
    margin: 0 !important;
    appearance: none !important;
    -webkit-appearance: none !important;
    -moz-appearance: none !important;
    background: white !important;
    border: 2px solid var(--gray-400) !important;
    border-radius: 3px !important;
    cursor: pointer !important;
    position: relative !important;
}

.subtipo-default .form-check-input-custom:checked {
    background: var(--success) !important;
    border-color: var(--success) !important;
}

.subtipo-default .form-check-input-custom:checked::after {
    content: "✓" !important;
    color: white !important;
    font-size: 10px !important;
    font-weight: bold !important;
    position: absolute !important;
    top: 50% !important;
    left: 50% !important;
    transform: translate(-50%, -50%) !important;
}

        .btn-remove {
            background: #f44336;
            border: none;
            color: white;
            width: 32px;
            height: 32px;
            border-radius: 4px;
            display: flex;
            align-items: center;
            justify-content: center;
            cursor: pointer;
            transition: var(--transition);
        }

        .btn-remove:hover {
            background: #d32f2f;
        }

        .btn-add {
            background: var(--success);
            border: none;
            color: white;
            font-weight: 500;
            padding: 10px 16px;
            border-radius: 4px;
            transition: var(--transition);
            display: inline-flex;
            align-items: center;
            gap: 6px;
            cursor: pointer;
        }

        .btn-add:hover {
            background: var(--success-hover);
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.2);
        }

        /* Botões de Ação */
        .action-buttons {
            display: flex;
            justify-content:end;
            gap: 12px;
            margin-top: 24px;
            padding-top: 20px;
            border-top: 1px solid var(--gray-200);
        }

        .btn-save {
            background: var(--success);
            border: none;
            color: white;
            font-weight: 500;
            padding: 12px 24px;
            border-radius: 4px;
            transition: var(--transition);
            display: flex;
            align-items: center;
            gap: 8px;
            cursor: pointer;
        }

        .btn-save:hover {
            background: var(--success-hover);
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.2);
        }

        /* Mensagens */
        .message {
            padding: 12px 16px;
            border-radius: 4px;
            margin-top: 16px;
            font-size: 14px;
        }

        .message-success {
            background: #e8f5e9;
            border: 1px solid #c8e6c9;
            color: #2e7d32;
        }

        .message-error {
            background: #ffebee;
            border: 1px solid #ffcdd2;
            color: #c62828;
        }

        .message-info {
            background: #e3f2fd;
            border: 1px solid #bbdefb;
            color: #1565c0;
        }

    /* ESTILOS PARA O GRID DE ESTRUTURAS EXISTENTES */
.grid-container {
    margin-top: 20px;
}

.grid-card {
    background: white;
    border-radius: var(--border-radius);
    box-shadow: var(--shadow);
    overflow: hidden;
}

.grid-card-header {
    background: var(--gray-50);
    padding: 16px 20px;
    border-bottom: 1px solid var(--gray-200);
}

.grid-card-title {
    font-size: 16px;
    font-weight: 500;
    color: var(--gray-800);
    margin: 0;
    display: flex;
    align-items: center;
    gap: 8px;
}

.grid-card-body {
    padding: 0;
    /* REMOVI O overflow-x: auto QUE CAUSAVA SCROLL */
}

/* ESTILOS DA GRIDVIEW */
.table {
    width: 100%;
    border-collapse: collapse;
    margin: 0;
    font-size: 14px;
    table-layout: fixed; /* ADICIONEI PARA EVITAR SCROLL */
}

.table th {
    background: var(--gray-50);
    color: var(--gray-700);
    font-weight: 600;
    padding: 12px 16px;
    text-align: left;
    border-bottom: 2px solid var(--gray-200);
    white-space: nowrap;
    overflow: hidden;
    text-overflow: ellipsis;
}

.table td {
    padding: 12px 16px;
    border-bottom: 1px solid var(--gray-200);
    vertical-align: middle;
    color: var(--gray-800);
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
}

.table tbody tr {
    transition: var(--transition);
}

.table tbody tr:hover {
    background: var(--gray-50);
}

.table-striped tbody tr:nth-child(odd) {
    background: var(--gray-100);
}

.table-striped tbody tr:nth-child(odd):hover {
    background: var(--gray-200);
}

.table-bordered {
    border: 1px solid var(--gray-200);
}

.table-bordered th,
.table-bordered td {
    border: 1px solid var(--gray-200);
}

/* BOTÕES DO GRID */
.btn {
    display: inline-flex;
    align-items: center;
    gap: 4px;
    padding: 6px 12px;
    font-size: 12px;
    font-weight: 500;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    transition: var(--transition);
    text-decoration: none;
    white-space: nowrap;
}

.btn-sm {
    padding: 4px 8px;
    font-size: 11px;
}

.btn-danger {
    background: #dc3545;
    color: white;
}

.btn-danger:hover {
    background: #c82333;
    box-shadow: 0 1px 3px rgba(220, 53, 69, 0.3);
}

/* ESTADOS DA GRID */
.empty-data {
    text-align: center;
    padding: 40px 20px;
    color: var(--gray-500);
    font-style: italic;
}

.empty-data i {
    font-size: 24px;
    margin-bottom: 8px;
    display: block;
}

/* BADGES E STATUS */
.badge {
    display: inline-block;
    padding: 4px 8px;
    font-size: 11px;
    font-weight: 600;
    border-radius: 12px;
    text-transform: uppercase;
}

.badge-success {
    background: #d4edda;
    color: #155724;
}

.badge-secondary {
    background: #e2e3e5;
    color: #383d41;
}

/* LARGURAS DAS COLUNAS PARA EVITAR SCROLL */
.column-text {
    width: 60%; /* Nome da Estrutura ocupa mais espaço */
    min-width: 200px;
}

.column-boolean {
    width: 15%; /* Principal ocupa menos espaço */
    min-width: 80px;
    text-align: center;
}

.column-number {
    width: 10%; /* Ordem ocupa pouco espaço */
    min-width: 60px;
    text-align: center;
}

.column-actions {
    width: 15%; /* Ações ocupa espaço moderado */
    min-width: 100px;
    text-align: center;
}

/* RESPONSIVIDADE DO GRID - AGORA SEM SCROLL */
@media (max-width: 768px) {
    .grid-card-body {
        font-size: 12px;
    }
    
    .table th,
    .table td {
        padding: 8px 12px;
    }
    
    .btn {
        padding: 4px 6px;
        font-size: 10px;
    }
    
    .btn i {
        font-size: 10px;
    }
    
    /* EM MOBILE, PERMITE QUEBRAR TEXTO */
    .table td {
        white-space: normal;
        word-wrap: break-word;
    }
}

/* ANIMAÇÕES */
@keyframes fadeIn {
    from {
        opacity: 0;
        transform: translateY(10px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

.grid-container {
    animation: fadeIn 0.3s ease-out;
}

/* MENSAGEM DE ESTRUTURAS EXISTENTES */
.alert-grid {
    margin-bottom: 16px;
    border-radius: 6px;
    border-left: 4px solid #17a2b8;
}

.alert-grid strong {
    display: flex;
    align-items: center;
    gap: 6px;
    margin-bottom: 4px;
}

/* HOVER EFFECTS MELHORADOS */
.table tbody tr {
    border-left: 3px solid transparent;
    transition: all 0.2s ease;
}

.table tbody tr:hover {
    border-left: 3px solid var(--primary);
    background: #f8fbff;
}

        /* Responsividade */
        @media (max-width: 768px) {
            .page-header {
                flex-direction: column;
                align-items: flex-start;
            }

            .card-body {
                padding: 16px;
            }

            .action-buttons {
                flex-direction: column;
            }

            .btn-save {
                width: 100%;
                justify-content: center;
            }
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <!-- Header da Página -->
        <div class="page-header">
            <div class="page-title">
                <div class="title-icon">
                    <i class="bi bi-diagram-3"></i>
                </div>
                Cadastro de Estrutura
            </div>
        </div>

        <!-- Card Principal -->
        <div class="main-card">
            <div class="card-header">
                <h2 class="card-title">
                    <i class="bi bi-pencil-square"></i>
                    Dados da Estrutura
                </h2>
            </div>
            
            <div class="card-body">

                <!-- COMBO COM AS VIEWS -->
                <div class="form-group">
                    <label class="form-label">View *</label>
                    <asp:DropDownList ID="ddlView" runat="server" CssClass="form-control form-select" AutoPostBack="true"
                        OnSelectedIndexChanged="ddlView_SelectedIndexChanged">
                        <asp:ListItem Text="Selecione uma View" Value=""></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <!-- SUBTIPOS DINÂMICOS - SEMPRE VISÍVEL -->
                <div class="form-group mt-3">
                    <label class="form-label">Adicionar Novas Estruturas</label>
                    <div class="alert alert-warning">
                        <small><i class="bi bi-lightbulb"></i> <strong>Dica:</strong> Defina a "Ordem" para organizar a exibição.</small>
                    </div>
                    <div class="subtipos-container">
                        <div id="containerSubtipos">
                            <!-- Primeiro campo com todos os dados -->
                            <div class="subtipo-item">
                                <div class="subtipo-input">
                                    <input type="text" class="form-control" name="subtipo_1" 
                                        placeholder="Digite o nome da estrutura" maxlength="100" />
                                </div>
                                <div class="subtipo-ordem">
                                    <input type="number" class="form-control campo-ordem" name="ordem_1" 
                                        placeholder="Ordem" value="1" min="1" 
                                        onchange="reordenarAutomaticamente(this)" />
                                </div>
                                <div class="subtipo-actions">
                                    <button type="button" class="btn-add-small" onclick="adicionarSubtipo()" title="Adicionar outra estrutura">
                                        <i class="bi bi-plus-lg"></i>
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- BOTÕES DE AÇÃO -->
                <div class="action-buttons">
                    <asp:Button ID="btnSalvarTudo" runat="server" Text="Salvar Tudo" 
                        CssClass="btn-save" OnClick="btnSalvarTudo_Click" />
                </div>

                <!-- MENSAGENS -->
                <asp:Panel ID="pnlMensagem" runat="server" Visible="false">
                    <div class="message" id="divMensagem" runat="server">
                        <asp:Label ID="lblMensagem" runat="server"></asp:Label>
                    </div>
                </asp:Panel>

                <!-- MENSAGEM SE JÁ EXISTIR ESTRUTURAS -->
                <asp:Panel ID="pnlMensagemEstruturaExistente" runat="server" Visible="false">
                    <div class="alert alert-info">
                        <strong><i class="bi bi-info-circle"></i> Estruturas existentes</strong>
                        <br />Esta View já possui estruturas cadastradas. Você pode visualizá-las abaixo e adicionar novas.
                    </div>
                </asp:Panel>

                <!-- GRID COM ESTRUTURAS EXISTENTES -->
                <asp:Panel ID="pnlGridEstruturas" runat="server" Visible="false" class="mt-3">
                    <div class="grid-card">
                        <div class="grid-card-header">
                            <h5 class="grid-card-title"><i class="bi bi-list-ul"></i> Estruturas Existentes</h5>
                        </div>
                        <div class="grid-card-body">
                            <asp:GridView ID="gvEstruturas" runat="server" CssClass="table table-striped table-bordered" 
                                AutoGenerateColumns="false" EmptyDataText="Nenhuma estrutura encontrada"
                                OnRowCommand="gvEstruturas_RowCommand" OnRowDataBound="gvEstruturas_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="DescEstrutura" HeaderText="Nome da Estrutura" 
                                        ItemStyle-CssClass="column-text" HeaderStyle-CssClass="column-text" />
                                    <asp:BoundField DataField="ValorPadrao" HeaderText="Ordem" 
                                        ItemStyle-CssClass="column-number" HeaderStyle-CssClass="column-number" />
                                    <asp:TemplateField HeaderText="Ações" 
                                        ItemStyle-CssClass="column-actions" HeaderStyle-CssClass="column-actions">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnExcluir" runat="server" CssClass="btn btn-danger btn-sm"
                                                CommandName="Excluir" CommandArgument='<%# Eval("CodEstrutura") %>'
                                                OnClientClick='<%# "return confirm(\"Tem certeza que deseja excluir a estrutura \\\"" + Eval("DescEstrutura") + "\\\"?\");" %>'
                                                ToolTip="Excluir estrutura">
                                                <i class="bi bi-trash"></i> Excluir
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </asp:Panel>

                <!-- CAMPO HIDDEN PARA ARMAZENAR OS SUBTIPOS -->
                <asp:HiddenField ID="hdnSubtipos" runat="server" />
            </div>
        </div>
    </div>

    <script>
        let subtipoCount = 1;

        function adicionarSubtipo() {
            subtipoCount++;
            const container = document.getElementById('containerSubtipos');

            // Pega a próxima ordem disponível (maior ordem atual + 1)
            const proximaOrdem = obterProximaOrdem();

            const subtipoItem = document.createElement('div');
            subtipoItem.className = 'subtipo-item';
            subtipoItem.innerHTML = `
                <div class="subtipo-input">
                    <input type="text" class="form-control" name="subtipo_${subtipoCount}" 
                        placeholder="Digite o nome da estrutura" maxlength="100" />
                </div>
                <div class="subtipo-ordem">
                    <input type="number" class="form-control campo-ordem" name="ordem_${subtipoCount}" 
                        placeholder="Ordem" value="${proximaOrdem}" min="1" 
                        onchange="reordenarAutomaticamente(this)" />
                </div>
                <div class="subtipo-actions">
                    <button type="button" class="btn-add-small" onclick="adicionarSubtipo()" title="Adicionar outra estrutura">
                        <i class="bi bi-plus-lg"></i>
                    </button>
                    <button type="button" class="btn-remove" onclick="removerSubtipo(this)" title="Remover estrutura">
                        <i class="bi bi-dash-lg"></i>
                    </button>
                </div>
            `;

            container.appendChild(subtipoItem);
        }

        // Função para obter a próxima ordem disponível
        function obterProximaOrdem() {
            const camposOrdem = document.querySelectorAll('.campo-ordem');
            let maiorOrdem = 0;

            camposOrdem.forEach(campo => {
                const ordem = parseInt(campo.value) || 0;
                if (ordem > maiorOrdem) {
                    maiorOrdem = ordem;
                }
            });

            return maiorOrdem + 1;
        }

        // Função principal de reordenação automática
        function reordenarAutomaticamente(campoAlterado) {
            const novaOrdem = parseInt(campoAlterado.value);
            const ordemAntiga = parseInt(campoAlterado.getAttribute('data-ordem-anterior')) || parseInt(campoAlterado.value);

            if (isNaN(novaOrdem) || novaOrdem < 1) {
                campoAlterado.value = ordemAntiga;
                return;
            }

            const todosCamposOrdem = document.querySelectorAll('.campo-ordem');
            let campoParaTrocar = null;

            // Encontra o campo que tem a ordem para a qual estamos mudando
            todosCamposOrdem.forEach(campo => {
                if (campo !== campoAlterado && parseInt(campo.value) === novaOrdem) {
                    campoParaTrocar = campo;
                }
            });

            // Se encontrou um campo com a mesma ordem, faz a troca
            if (campoParaTrocar) {
                campoParaTrocar.value = ordemAntiga;
                campoParaTrocar.setAttribute('data-ordem-anterior', ordemAntiga);
            }

            // Atualiza a ordem anterior para o novo valor
            campoAlterado.setAttribute('data-ordem-anterior', novaOrdem);
        }

        function removerSubtipo(button) {
            const subtipoItem = button.closest('.subtipo-item');
            const ordemRemovida = parseInt(subtipoItem.querySelector('.campo-ordem').value);

            subtipoItem.remove();

            // Reorganiza as ordens dos itens restantes
            reorganizarOrdensAposRemocao(ordemRemovida);
        }

        // Reorganiza as ordens quando um item é removido
        function reorganizarOrdensAposRemocao(ordemRemovida) {
            const camposOrdem = document.querySelectorAll('.campo-ordem');
            const ordens = [];

            // Coleta todas as ordens atuais
            camposOrdem.forEach(campo => {
                ordens.push(parseInt(campo.value));
            });

            // Ordena as ordens
            ordens.sort((a, b) => a - b);

            // Reatribui as ordens sequencialmente
            camposOrdem.forEach((campo, index) => {
                campo.value = index + 1;
                campo.setAttribute('data-ordem-anterior', index + 1);
            });
        }

        function limparCamposSubtipos() {
            document.getElementById('containerSubtipos').innerHTML = '';
            subtipoCount = 0;
            adicionarSubtipo();
        }

        // Antes de enviar o formulário, coleta todos os dados dos subtipos
        document.getElementById('<%= btnSalvarTudo.ClientID %>').addEventListener('click', function () {
            const subtipos = [];
            const subtipoItems = document.querySelectorAll('.subtipo-item');

            subtipoItems.forEach((item, index) => {
                const nome = item.querySelector('input[type="text"]').value.trim();
                const ordem = item.querySelector('.campo-ordem').value;

                if (nome !== '') {
                    subtipos.push({
                        nome: nome,
                        ordem: parseInt(ordem) || 0
                    });
                }
            });

            document.getElementById('<%= hdnSubtipos.ClientID %>').value = JSON.stringify(subtipos);
        });

        // Adiciona um campo inicial se não houver nenhum
        document.addEventListener('DOMContentLoaded', function () {
            if (document.getElementById('containerSubtipos').children.length === 0) {
                adicionarSubtipo();
            }

            // Inicializa os data-atributes de ordem anterior
            const camposOrdem = document.querySelectorAll('.campo-ordem');
            camposOrdem.forEach(campo => {
                campo.setAttribute('data-ordem-anterior', campo.value);
            });
        });
    </script>
</asp:Content>