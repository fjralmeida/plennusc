<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscMedic/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="registerStructuresMedic.aspx.cs" Inherits="appWhatsapp.PlennuscMedic.Views.registerStructuresMedic" %>

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
            justify-content: end;
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
                        <small><i class="bi bi-lightbulb"></i><strong>Dica:</strong> Marque uma estrutura como "Principal" e defina a "Ordem" para organizar a exibição.</small>
                    </div>
                    <div class="subtipos-container">
                        <div id="containerSubtipos">
                            <!-- Primeiro campo com todos os dados -->
                            <div class="subtipo-item">
                                <div class="subtipo-input">
                                    <input type="text" class="form-control" name="subtipo_1"
                                        placeholder="Digite o nome da estrutura" maxlength="100" />
                                </div>
                                <div class="subtipo-default">
                                    <input type="checkbox" class="form-check-input-custom principal-checkbox"
                                        name="default_1" id="default_1" checked onchange="atualizarCheckboxesPrincipais(this)" />
                                    <label for="default_1">Principal</label>
                                </div>
                                <div class="subtipo-ordem">
                                    <input type="number" class="form-control" name="ordem_1"
                                        placeholder="Ordem" value="0" min="0" />
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
                        <strong><i class="bi bi-info-circle"></i>Estruturas existentes</strong>
                        <br />
                        Esta View já possui estruturas cadastradas. Você pode visualizá-las abaixo e adicionar novas.
                    </div>
                </asp:Panel>

                <!-- GRID COM ESTRUTURAS EXISTENTES -->
                <asp:Panel ID="pnlGridEstruturas" runat="server" Visible="false" class="mt-3">
                    <div class="card">
                        <div class="card-header">
                            <h5 class="card-title"><i class="bi bi-list-ul"></i>Estruturas Existentes</h5>
                        </div>
                        <div class="card-body">
                            <asp:GridView ID="gvEstruturas" runat="server" CssClass="table table-striped table-bordered"
                                AutoGenerateColumns="false" EmptyDataText="Nenhuma estrutura encontrada">
                                <Columns>
                                    <asp:BoundField DataField="DescEstrutura" HeaderText="Nome da Estrutura" />
                                    <asp:TemplateField HeaderText="Principal">
                                        <ItemTemplate>
                                            <%# Convert.ToBoolean(Eval("Conf_IsDefault")) ? "Sim" : "Não" %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField DataField="ValorPadrao" HeaderText="Ordem" />
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

            const subtipoItem = document.createElement('div');
            subtipoItem.className = 'subtipo-item';
            subtipoItem.innerHTML = `
        <div class="subtipo-input">
            <input type="text" class="form-control" name="subtipo_${subtipoCount}" 
                placeholder="Digite o nome da estrutura" maxlength="100" />
        </div>
        <div class="subtipo-default">
            <input type="checkbox" class="form-check-input-custom principal-checkbox" 
                name="default_${subtipoCount}" id="default_${subtipoCount}" 
                onchange="atualizarCheckboxesPrincipais(this)" />
            <label for="default_${subtipoCount}">Principal</label>
        </div>
        <div class="subtipo-ordem">
            <input type="number" class="form-control" name="ordem_${subtipoCount}" 
                placeholder="Ordem" value="0" min="0" />
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

        function atualizarCheckboxesPrincipais(checkboxClicado) {
            if (checkboxClicado.checked) {
                // Se este foi marcado, desmarca todos os outros
                const todosCheckboxes = document.querySelectorAll('.principal-checkbox');
                todosCheckboxes.forEach(checkbox => {
                    if (checkbox !== checkboxClicado) {
                        checkbox.checked = false;
                    }
                });
            }
        }

        function removerSubtipo(button) {
            const subtipoItem = button.closest('.subtipo-item');
            subtipoItem.remove();

            // Se o item removido era o principal, podemos marcar automaticamente o primeiro como principal
            const checkboxesMarcados = document.querySelectorAll('.principal-checkbox:checked');
            if (checkboxesMarcados.length === 0) {
                // Se não há nenhum marcado como principal, marca o primeiro
                const primeiroCheckbox = document.querySelector('.principal-checkbox');
                if (primeiroCheckbox) {
                    primeiroCheckbox.checked = true;
                }
            }
        }

        function limparCamposSubtipos() {
            document.getElementById('containerSubtipos').innerHTML = '';
            subtipoCount = 0;
            adicionarSubtipo(); // Adiciona um campo vazio após limpar
        }

        // Antes de enviar o formulário, coleta todos os dados dos subtipos
        document.getElementById('<%= btnSalvarTudo.ClientID %>').addEventListener('click', function () {
            const subtipos = [];

            // Percorre todos os itens de subtipo
            const subtipoItems = document.querySelectorAll('.subtipo-item');

            subtipoItems.forEach((item, index) => {
                const nome = item.querySelector('input[type="text"]').value.trim();
                const isDefault = item.querySelector('input[type="checkbox"]').checked;
                const ordem = item.querySelector('input[type="number"]').value;

                if (nome !== '') {
                    subtipos.push({
                        nome: nome,
                        isDefault: isDefault,
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

            // Marca o primeiro checkbox como principal por padrão
            const primeiroCheckbox = document.querySelector('.principal-checkbox');
            if (primeiroCheckbox) {
                primeiroCheckbox.checked = true;
            }
        });
    </script>


</asp:Content>
