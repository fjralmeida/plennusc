<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscMedic/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="structureTypeMedic.aspx.cs" Inherits="appWhatsapp.PlennuscMedic.Views.structureTypeMedic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Estruturas por Setor</title>
    <!-- Importação do Bootstrap Icons -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <style>
        :root {
            --primary: #83ceee;
            --primary-hover: #75bbd9;
            --success: #4cb07a;
            --success-hover: #3b8b65;
            --warning: #ffa726;
            --warning-hover: #f57c00;
            --danger: #f44336;
            --danger-hover: #d32f2f;
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
        }

        .container-main {
            max-width: 2206px;
            margin: 20px auto;
            padding: 0 16px;
        }

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
            display: flex;
            align-items: center;
            gap: 8px;
            cursor: pointer;
            transition: background 0.2s ease;
        }

        .btn-primary:hover {
            background: var(--success-hover);
        }

        .btn-secondary {
            background: var(--gray-300);
            border: none;
            color: var(--gray-700);
            font-weight: 500;
            padding: 10px 20px;
            border-radius: 4px;
            display: flex;
            align-items: center;
            gap: 8px;
            cursor: pointer;
            transition: background 0.2s ease;
        }

        .btn-secondary:hover {
            background: var(--gray-400);
        }

        .card {
            background: white;
            border-radius: var(--border-radius);
            box-shadow: var(--shadow);
            padding: 20px;
            margin-bottom: 24px;
        }

        .card h3 {
            margin-top: 0;
            margin-bottom: 16px;
            color: var(--gray-800);
            font-size: 18px;
            font-weight: 500;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .form-group {
            margin-bottom: 16px;
        }

        .form-label {
            display: block;
            font-size: 14px;
            font-weight: 500;
            color: var(--gray-700);
            margin-bottom: 8px;
            display: flex;
            align-items: center;
            gap: 6px;
        }

        .form-control {
            width: 100%;
            padding: 10px 12px;
            border: 1px solid var(--gray-300);
            border-radius: 4px;
            font-size: 14px;
            transition: border-color 0.2s ease;
        }

        .form-control:focus {
            outline: none;
            border-color: var(--primary);
            box-shadow: 0 0 0 2px rgba(131, 206, 238, 0.2);
        }

        .input-with-button-wrapper {
            position: relative;
            display: flex;
            align-items: stretch;
        }

        .input-with-button {
            flex: 1;
            display: flex;
        }

        .input-with-button .subtype-input {
            flex: 1;
            border: 1px solid var(--gray-300);
            border-radius: 4px 0 0 4px;
            padding: 8px 12px;
            font-size: 14px;
            transition: border-color 0.2s ease;
            border-right: none;
        }

        .input-with-button .subtype-input:focus {
            outline: none;
            border-color: var(--primary);
            box-shadow: none;
        }

        .input-with-button .subtype-input:focus + .btn-add-service-inside {
            border-color: var(--primary);
        }

        .btn-add-service-inside {
            background: var(--primary);
            color: white;
            border: 1px solid var(--gray-300);
            border-radius: 0 4px 4px 0;
            padding: 8px 16px;
            cursor: pointer;
            display: flex;
            align-items: center;
            gap: 6px;
            font-size: 14px;
            transition: all 0.2s ease;
            white-space: nowrap;
            height: auto;
        }

        .btn-add-service-inside:hover {
            background: var(--primary-hover);
        }

        .services-counter {
            font-size: 12px;
            color: var(--gray-500);
            margin-top: 4px;
            display: flex;
            align-items: center;
            gap: 4px;
        }

        .subtype-container {
            margin-top: 12px;
        }

        .subtype-item {
            display: flex;
            gap: 12px;
            align-items: center;
            margin-bottom: 8px;
            padding: 12px;
            background: var(--gray-50);
            border-radius: var(--border-radius);
            transition: all 0.2s ease;
            border-left: 3px solid var(--primary);
        }

        .subtype-item:hover {
            background: var(--gray-100);
        }

        .servico-text {
            flex: 1;
            padding: 8px 12px;
            background: white;
            border: 1px solid var(--gray-300);
            border-radius: 4px;
            font-size: 14px;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .btn-remove {
            background: var(--danger);
            color: white;
            border: none;
            border-radius: 4px;
            width: 32px;
            height: 32px;
            display: flex;
            align-items: center;
            justify-content: center;
            cursor: pointer;
            font-size: 14px;
            transition: all 0.2s ease;
        }

        .btn-remove:hover {
            background: var(--danger-hover);
        }

        .empty-state {
            text-align: center;
            padding: 20px;
            color: var(--gray-500);
            font-style: italic;
            background: var(--gray-50);
            border-radius: var(--border-radius);
            margin-bottom: 8px;
            display: flex;
            flex-direction: column;
            align-items: center;
            gap: 8px;
        }

        .form-actions {
            display: flex;
            gap: 12px;
            margin-top: 20px;
            padding-top: 16px;
            border-top: 1px solid var(--gray-200);
        }

        .service-item-container {
            max-height: 300px;
            overflow-y: auto;
            border: 1px solid var(--gray-200);
            border-radius: var(--border-radius);
            padding: 8px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <header class="page-header">
            <h1 class="page-title">
                <span class="title-icon">
                    <i class="bi bi-diagram-3"></i>
                </span>
                Estruturas por Setor
            </h1>
        </header>

        <!-- Seção de Seleção de Setor -->
        <div class="card">
            <h3><i class="bi bi-building"></i> Selecionar Setor</h3>
            <div class="form-group">
                <asp:DropDownList ID="ddlSetor" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlSetor_SelectedIndexChanged">
                    <asp:ListItem Text="Selecione um setor" Value="" />
                </asp:DropDownList>
            </div>
        </div>

        <asp:Panel ID="pnlEstruturas" runat="server" Visible="false">
            <!-- Formulário de Cadastro de Estrutura -->
            <div class="card">
                <h3><i class="bi bi-folder-plus"></i> Cadastrar Nova Categoria</h3>
                
                <div class="form-group">
                    <label class="form-label">
                        <i class="bi bi-tag"></i>
                        Nome da Categoria
                    </label>
                    <asp:TextBox ID="txtNomeEstrutura" runat="server" CssClass="form-control" placeholder="Ex: Tecnologia, Financeiro, etc." />
                </div>

                <!-- Tipos de Serviço -->
                <div class="form-group">
                    <label class="form-label">
                        <i class="bi bi-gear"></i>
                        Tipos de Serviço
                    </label>
                    
                    <!-- Input com botão DENTRO do campo -->
                    <div class="input-with-button-wrapper">
                        <div class="input-with-button">
                            <input type="text" class="subtype-input form-control" id="novoServicoInput" placeholder="Digite o nome do serviço..."/>
                            <button type="button" class="btn-add-service-inside" onclick="adicionarServico()" title="Adicionar serviço">
                                <i class="bi bi-plus-lg"></i>
                            </button>
                        </div>
                    </div>
                    
                    <span class="services-counter" id="subtypeCounter">
                        <i class="bi bi-list-check"></i>
                        0 serviço(s) adicionado(s)
                    </span>

                    <div class="service-item-container" id="subtypeContainer">
                        <div class="empty-state" id="emptyState">
                            <i class="bi bi-inbox"></i>
                            Nenhum serviço adicionado.
                        </div>
                    </div>

                    <!-- Hidden field para armazenar os subtipos -->
                    <asp:HiddenField ID="hdnSubtipos" runat="server" />
                </div>

                <div class="form-actions">
                    <asp:Button ID="btnSalvarEstrutura" runat="server" CssClass="btn-primary" Text="Salvar Categoria" OnClick="btnSalvarEstrutura_Click" OnClientClick="return validarFormulario();" />
                    <asp:Button ID="btnCancelar" runat="server" CssClass="btn-secondary" Text="Cancelar" OnClick="btnCancelar_Click" />
                </div>
            </div>
        </asp:Panel>
    </div>

    <script>
        let servicos = [];

        function adicionarServico() {
            const input = document.getElementById('novoServicoInput');
            const servico = input.value.trim();

            if (!servico) {
                alert('Informe o nome do serviço.');
                return;
            }

            servicos.push(servico);
            input.value = '';
            atualizarListaServicos();
            atualizarContador();
            input.focus();
        }

        function removerServico(index) {
            servicos.splice(index, 1);
            atualizarListaServicos();
            atualizarContador();
        }

        function atualizarListaServicos() {
            const container = document.getElementById('subtypeContainer');

            if (servicos.length === 0) {
                container.innerHTML = `
                    <div class="empty-state" id="emptyState">
                        <i class="bi bi-inbox"></i>
                        Nenhum serviço adicionado.
                    </div>`;
                return;
            }

            let html = '';
            servicos.forEach((servico, index) => {
                html += `
                    <div class="subtype-item">
                        <div class="servico-text">
                            <i class="bi bi-gear"></i>
                            ${servico}
                        </div>
                        <button type="button" class="btn-remove" onclick="removerServico(${index})" title="Remover serviço">
                            <i class="bi bi-trash"></i>
                        </button>
                    </div>
                `;
            });

            container.innerHTML = html;
        }

        function atualizarContador() {
            const counter = document.getElementById('subtypeCounter');
            counter.innerHTML = `<i class="bi bi-list-check"></i> ${servicos.length} serviço(s) adicionado(s)`;
        }

        function validarFormulario() {
            const nomeEstrutura = document.getElementById('<%= txtNomeEstrutura.ClientID %>').value.trim();
            if (!nomeEstrutura) {
                alert('Informe o nome da categoria.');
                return false;
            }

            if (servicos.length === 0) {
                alert('Adicione pelo menos um serviço.');
                return false;
            }

            // Salvar serviços no hidden field para o code-behind
            document.getElementById('<%= hdnSubtipos.ClientID %>').value = JSON.stringify(servicos);
            return true;
        }

        // Permitir adicionar com Enter
        document.addEventListener('DOMContentLoaded', function () {
            const input = document.getElementById('novoServicoInput');
            input.addEventListener('keypress', function (e) {
                if (e.key === 'Enter') {
                    adicionarServico();
                }
            });
            input.focus();
        });
    </script>
</asp:Content>