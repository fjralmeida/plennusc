<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="registerStructures.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.registerStructures" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
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
            gap: 8px;
            align-items: center;
            margin-bottom: 12px;
        }

        .subtipo-input {
            flex: 1;
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

                <!-- ESTRUTURA PRINCIPAL - SÓ APARECE SE NÃO EXISTIR -->
                <asp:Panel ID="pnlEstruturaPrincipal" runat="server" Visible="false">
                    <div class="form-group">
                        <label class="form-label">Estrutura Principal *</label>
                        <asp:TextBox ID="txtEstruturaPrincipal" runat="server" CssClass="form-control" 
                            placeholder="Digite o nome da estrutura principal" MaxLength="100"></asp:TextBox>
                    </div>
                </asp:Panel>

                <!-- MENSAGEM SE JÁ EXISTIR ESTRUTURA PRINCIPAL -->
                <asp:Panel ID="pnlMensagemEstruturaExistente" runat="server" Visible="false">
                    <div class="alert alert-info">
                        <strong><i class="bi bi-info-circle"></i> Estrutura principal existente</strong>
                        <br />Já existe uma estrutura principal para esta View. Agora você pode adicionar subtipos abaixo.
                    </div>
                </asp:Panel>

              <!-- SUBTIPOS DINÂMICOS -->
                <div class="form-group">
                    <label class="form-label">Subtipos</label>
                    <div class="subtipos-container">
                        <div id="containerSubtipos">
                            <!-- Primeiro campo com botão + -->
                            <div class="subtipo-item">
                                <div class="subtipo-input">
                                    <input type="text" class="form-control" name="subtipo_1" 
                                        placeholder="Digite o nome do subtipo" maxlength="100" />
                                </div>
                                <div class="subtipo-actions">
                                    <button type="button" class="btn-add-small" onclick="adicionarSubtipo()" title="Adicionar outro subtipo">
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
                
                <!-- CAMPO HIDDEN PARA ARMAZENAR OS SUBTIPOS -->
                <asp:HiddenField ID="hdnSubtipos" runat="server" />
            </div>
        </div>
    </div>

    <script>
        let subtipoCount = 1; // Começa com 1 porque já tem um campo

        function adicionarSubtipo() {
            subtipoCount++;
            const container = document.getElementById('containerSubtipos');

            const subtipoItem = document.createElement('div');
            subtipoItem.className = 'subtipo-item';
            subtipoItem.innerHTML = `
        <div class="subtipo-input">
            <input type="text" class="form-control" name="subtipo_${subtipoCount}" 
                placeholder="Digite o nome do subtipo" maxlength="100" />
        </div>
        <div class="subtipo-actions">
            <button type="button" class="btn-add-small" onclick="adicionarSubtipo()" title="Adicionar outro subtipo">
                <i class="bi bi-plus-lg"></i>
            </button>
            <button type="button" class="btn-remove" onclick="removerSubtipo(this)" title="Remover subtipo">
                <i class="bi bi-dash-lg"></i>
            </button>
        </div>
    `;

            container.appendChild(subtipoItem);
        }

        function removerSubtipo(button) {
            const subtipoItem = button.closest('.subtipo-item');
            subtipoItem.remove();
        }
        function limparCamposSubtipos() {
            document.getElementById('containerSubtipos').innerHTML = '';
            subtipoCount = 0;
        }

        // Antes de enviar o formulário, coleta todos os subtipos e coloca no hidden field
        document.getElementById('<%= btnSalvarTudo.ClientID %>').addEventListener('click', function() {
            const inputs = document.querySelectorAll('#containerSubtipos input[type="text"]');
            const subtipos = [];
            
            inputs.forEach(input => {
                if (input.value.trim() !== '') {
                    subtipos.push(input.value.trim());
                }
            });
            
            document.getElementById('<%= hdnSubtipos.ClientID %>').value = JSON.stringify(subtipos);
        });

        // Adiciona um campo inicial se não houver nenhum
        document.addEventListener('DOMContentLoaded', function () {
            if (document.getElementById('containerSubtipos').children.length === 0) {
                adicionarSubtipo();
            }
        });
    </script>
</asp:Content>