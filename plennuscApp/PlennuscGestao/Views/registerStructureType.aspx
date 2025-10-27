<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="registerStructureType.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.registerStructureType" %>
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

        /* Checkbox CUSTOMIZADO - SEM BOOTSTRAP */
        .form-check {
            display: flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 20px;
            padding-left: 0.5em;
        }

        .form-check-input-custom:checked {
            background: var(--success) !important;
            border-color: var(--success) !important;
        }

        .form-check-input-custom:checked::after {
            content: "✓" !important;
            color: white !important;
            font-size: 12px !important;
            font-weight: bold !important;
            position: absolute !important;
            top: 50% !important;
            left: 50% !important;
            transform: translate(-50%, -50%) !important;
        }

        .form-check-input-custom:focus {
            border-color: var(--primary) !important;
            outline: none !important;
        }

            .form-check-label {
            font-size: 14px;
            font-weight: 500;
            color: var(--gray-700);
            cursor: pointer;
        }

        /* Info da View */
        .view-info {
            background: #e3f2fd;
            border: 1px solid #bbdefb;
            border-radius: 4px;
            padding: 16px;
            margin-bottom: 20px;
        }

        .view-label {
            font-size: 12px;
            font-weight: 600;
            color: #1565c0;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            margin-bottom: 4px;
        }

        .view-name {
            font-size: 16px;
            font-weight: 600;
            color: var(--gray-800);
            font-family: 'Courier New', monospace;
        }

        /* Botões */
        .action-buttons {
            display: flex;
            justify-content:flex-end;
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

        .bi-tags::before {
    content: "\f5b2";
    width: 13px;
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
                     <i class="bi bi-tags me-2"></i>
                </div>
                Cadastro de Tipo Estrutura
            </div>
        </div>

        <!-- Card Principal -->
        <div class="main-card">
            <div class="card-header">
                <h2 class="card-title">
                    <i class="bi bi-pencil-square"></i>
                    Dados do Tipo Estrutura
                </h2>
            </div>
            
            <div class="card-body">
                <!-- DESCRIÇÃO -->
                <div class="form-group">
                    <label class="form-label">Descrição *</label>
                    <asp:TextBox ID="txtDescricao" runat="server" CssClass="form-control" 
                        placeholder="Ex: PERFIL PESSOA, TIPO EMPRESA, etc." MaxLength="100"></asp:TextBox>
                </div>

               <!-- EDITÁVEL -->
                <div class="form-check">
                    <asp:CheckBox ID="chkEditavel" runat="server" CssClass="form-check-input-custom" />
                    <label class="form-check-label" for="<%= chkEditavel.ClientID %>">Editável</label>
                </div>
                <!-- INFO DA VIEW -->
                <div class="view-info">
                    <div class="view-label">View que será criada</div>
                    <div class="view-name" id="lblViewNome" runat="server">VW_</div>
                </div>

                <!-- BOTÃO SALVAR -->
                <div class="action-buttons">
                    <asp:Button ID="btnSalvar" runat="server" Text="Salvar e Criar View" 
                        CssClass="btn-save" OnClick="btnSalvar_Click" />
                </div>

                <!-- MENSAGENS -->
                <asp:Panel ID="pnlMensagem" runat="server" Visible="false">
                    <div class="message" id="divMensagem" runat="server">
                        <asp:Label ID="lblMensagem" runat="server"></asp:Label>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>

    <script>
        // Atualiza nome da view automaticamente
        function atualizarViewNome() {
            var descricao = document.getElementById('<%= txtDescricao.ClientID %>').value;
            var lblView = document.getElementById('<%= lblViewNome.ClientID %>');

            if (descricao) {
                lblView.innerText = 'VW_' + descricao.toUpperCase().replace(/ /g, '_');
            } else {
                lblView.innerText = 'VW_';
            }
        }

        document.getElementById('<%= txtDescricao.ClientID %>').addEventListener('input', atualizarViewNome);

        // Executa uma vez ao carregar a página
        document.addEventListener('DOMContentLoaded', function () {
            atualizarViewNome();
        });
    </script>
</asp:Content>