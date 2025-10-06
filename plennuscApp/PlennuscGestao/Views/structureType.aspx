<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="structureType.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.structureType" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Estruturas por Setor</title>
    <style>
        :root {
            --primary: #83ceee;
            --primary-hover: #0d62c9;
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

        .input-with-button {
            display: flex;
            gap: 8px;
            align-items: center;
        }

        .input-with-button .subtype-input {
            flex: 1;
        }

        .services-counter {
            font-size: 12px;
            color: var(--gray-500);
            margin-top: 4px;
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
        }

        .subtype-item:hover {
            background: var(--gray-100);
        }

        .subtype-input {
            border: 1px solid var(--gray-300);
            border-radius: 4px;
            padding: 8px 12px;
            font-size: 14px;
            transition: border-color 0.2s ease;
        }

        .subtype-input:focus {
            outline: none;
            border-color: var(--primary);
            box-shadow: 0 0 0 2px rgba(131, 206, 238, 0.2);
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
            font-size: 16px;
            transition: all 0.2s ease;
        }

        .btn-remove:hover {
            background: var(--danger-hover);
            transform: scale(1.05);
        }

        .btn-add-service {
            background: var(--primary);
            color: white;
            border: none;
            border-radius: 4px;
            padding: 8px 16px;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 6px;
            font-size: 14px;
            transition: all 0.2s ease;
            white-space: nowrap;
            height: 38px;
        }

        .btn-add-service:hover {
            background: var(--primary-hover);
            transform: translateY(-1px);
        }

        .empty-state {
            text-align: center;
            padding: 20px;
            color: var(--gray-500);
            font-style: italic;
            background: var(--gray-50);
            border-radius: var(--border-radius);
            margin-bottom: 8px;
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
                    <i class="fas fa-sitemap"></i>
                </span>
                Estruturas por Setor
            </h1>
        </header>

        <!-- Seção de Seleção de Setor -->
        <div class="card">
            <h3>Selecionar Setor</h3>
            <div class="form-group">
                <label class="form-label">Setor de Destino</label>
                <asp:DropDownList ID="ddlSetor" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlSetor_SelectedIndexChanged">
                    <asp:ListItem Text="Selecione um setor" Value="" />
                </asp:DropDownList>
            </div>
        </div>

        <asp:Panel ID="pnlEstruturas" runat="server" Visible="false">
            <!-- Formulário de Cadastro de Estrutura -->
            <div class="card">
                <h3>Cadastrar Nova Categoria</h3>
                
                <div class="form-group">
                    <label class="form-label">Nome da Categoria</label>
                    <asp:TextBox ID="txtNomeEstrutura" runat="server" CssClass="form-control" placeholder="Ex: Tecnologia, Financeiro, etc." />
                </div>

                <!-- Tipos de Serviço -->
                <div class="form-group">
                    <label class="form-label">Tipos de Serviço</label>
                    
                    <!-- Input com botão ao lado -->
                    <div class="input-with-button">
                        <input type="text" class="subtype-input form-control" id="novoServicoInput"/>
                        <button type="button" class="btn-add-service" onclick="adicionarServico()">
                            <i class="fas fa-plus"></i>
                            Adicionar Serviço
                        </button>
                    </div>
                    
                    <span class="services-counter" id="subtypeCounter">0 serviço(s) adicionado(s)</span>

                    <div class="service-item-container" id="subtypeContainer">
                        <div class="empty-state" id="emptyState">
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
                container.innerHTML = '<div class="empty-state" id="emptyState">Nenhum serviço adicionado.</div>';
                return;
            }

            let html = '';
            servicos.forEach((servico, index) => {
                html += `
                    <div class="subtype-item">
                        <span class="subtype-input" style="background: white; border: 1px solid var(--gray-300);">${servico}</span>
                        <button type="button" class="btn-remove" onclick="removerServico(${index})" title="Remover serviço">
                            <i class="fas fa-times"></i>
                        </button>
                    </div>
                `;
            });

            container.innerHTML = html;
        }

        function atualizarContador() {
            const counter = document.getElementById('subtypeCounter');
            counter.textContent = `${servicos.length} serviço(s) adicionado(s)`;
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
        });
    </script>
</asp:Content>
