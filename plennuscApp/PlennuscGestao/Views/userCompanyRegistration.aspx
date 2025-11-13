<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="userCompanyRegistration.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.userCompanyRegistration" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<style>
    /* USER COMPANY REGISTRATION - CSS CORRETO */
    :root {
        --primary: #83ceee;
        --primary-hover: #0d62c9;
        --success: #4cb07a;
        --success-hover: #3b8b65;
        --danger: #e74c3c;
        --danger-hover: #c0392b;
        --gray-50: #f8f9fa;
        --gray-100: #f1f3f4;
        --gray-200: #e8eaed;
        --gray-300: #dadce0;
        --gray-400: #bdc1c6;
        --gray-500: #9aa0a6;
        --gray-600: #80868b;
        --gray-700: #5f6368;
        --gray-800: #3c4043;
        --border-radius: 8px;
        --shadow: 0 1px 2px 0 rgba(60, 64, 67, 0.3), 0 1px 3px 1px rgba(60, 64, 67, 0.15);
        --transition: all 0.3s ease;
    }

    /* CONTAINER PRINCIPAL */
    .container-main {
        padding: 20px;
        max-width: 2600px;
        margin: 0 auto;
    }

    /* HEADER */
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
        font-weight: 600;
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

    /* BOTÃO VOLTAR */
    .btn {
        display: inline-flex;
        align-items: center;
        gap: 8px;
        padding: 10px 16px;
        border-radius: var(--border-radius);
        text-decoration: none;
        font-weight: 500;
        font-size: 14px;
        transition: var(--transition);
        border: none;
        cursor: pointer;
    }

    .btn-primary {
        background: var(--primary);
        color: white;
    }

        .btn-primary:hover {
            background: var(--primary-hover);
            color: white;
            text-decoration: none;
        }

    /* CARD PRINCIPAL */
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
        font-weight: 600;
        color: var(--gray-800);
        margin: 0;
        display: flex;
        align-items: center;
        gap: 8px;
    }

    .card-body {
        padding: 24px;
    }

    /* SEÇÕES DO FORMULÁRIO */
    .form-section {
        margin-bottom: 24px;
    }

    .section-title {
        font-size: 16px;
        font-weight: 600;
        color: var(--gray-800);
        margin-bottom: 12px;
        padding-bottom: 8px;
        border-bottom: 1px solid var(--gray-200);
    }

    .form-section p {
        color: var(--gray-600);
        margin-bottom: 16px;
        font-size: 14px;
    }

    /* CHECKBOXLIST - SIMPLES, UM DO LADO DO OUTRO COMO NA IMAGEM */
    .companies-list {
        margin-top: 16px;
    }

    /* RESETA COMPLETAMENTE O CHECKBOXLIST */
    .companies-checkbox-list {
        display: block !important;
        border: none !important;
        width: 100% !important;
    }

        .companies-checkbox-list table {
            width: 100% !important;
            border: none !important;
            border-collapse: collapse !important;
            display: flex !important;
            flex-direction: row !important; /* LINHA HORIZONTAL */
            gap: 32px !important; /* ESPAÇAMENTO ENTRE OS CHECKBOXES */
            align-items: center !important;
            flex-wrap: wrap !important;
        }

        .companies-checkbox-list tr {
            display: flex !important;
            margin: 0 !important;
            flex: 0 0 auto !important;
        }

        .companies-checkbox-list td {
            display: flex !important;
            border: none !important;
            padding: 0 !important;
            align-items: center !important;
        }

        /* CHECKBOXES SIMPLES - ESTILO PADRÃO MAS MELHORADO */
        .companies-checkbox-list input[type="checkbox"] {
            display: inline-block !important;
            width: 18px !important;
            height: 18px !important;
            margin: 0 8px 0 0 !important;
            cursor: pointer !important;
        }

        /* LABELS SIMPLES - SEM CARDS, SEM BACKGROUND */
        .companies-checkbox-list label {
            display: flex !important;
            align-items: center !important;
            padding: 8px 0 !important;
            cursor: pointer !important;
            font-size: 14px !important;
            color: var(--gray-700) !important;
            margin: 0 !important;
            white-space: nowrap !important;
        }

        .companies-checkbox-list input[type="checkbox"]:checked + label {
            color: var(--success) !important;
            font-weight: 600 !important;
        }

    /* BOTÕES DE AÇÃO */
    .action-buttons {
        display: flex;
        justify-content: flex-end;
        gap: 12px;
        margin-top: 24px;
        padding-top: 20px;
        border-top: 1px solid var(--gray-200);
        clear: both;
    }

    .btn-save {
        background: var(--success);
        border: none;
        color: white;
        font-weight: 500;
        padding: 12px 24px;
        border-radius: 4px;
        font-size: 14px;
        cursor: pointer;
        display: inline-flex;
        align-items: center;
        gap: 8px;
        transition: var(--transition);
        min-width: 160px;
        justify-content: center;
        text-decoration: none;
    }

        .btn-save:hover {
            background: var(--success-hover);
        }

    /* RESPONSIVIDADE */
    @media (max-width: 768px) {
        .container-main {
            padding: 12px;
        }

        .page-header {
            flex-direction: column;
            align-items: flex-start;
        }

        .card-body {
            padding: 16px;
        }

        .companies-checkbox-list table {
            flex-direction: column !important;
            align-items: flex-start !important;
            gap: 16px !important;
        }

        .companies-checkbox-list label {
            white-space: normal !important;
        }

        .action-buttons {
            flex-direction: column;
        }

        .btn-save {
            width: 100%;
        }

        .btn {
            width: 100%;
            justify-content: center;
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

    .main-card {
        animation: fadeIn 0.3s ease-out;
    }
</style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <div class="page-header">
            <h2 class="page-title">
                <div class="title-icon">
                    <i class="bi bi-building-gear"></i>
                </div>
                Vincular Empresas ao Usuário
            </h2>
            <a href="userCompanyManagement.aspx" class="btn btn-primary">
                <i class="bi bi-arrow-left"></i>Voltar
            </a>
        </div>

        <asp:HiddenField ID="hfCodPessoa" runat="server" />
        <asp:HiddenField ID="hfCodAutenticacao" runat="server" />

        <div class="main-card">
            <div class="card-header">
                <h3 class="card-title">
                    <i class="bi bi-person-check"></i>
                    Usuário:
                    <asp:Literal ID="litUsuario" runat="server" />
                </h3>
            </div>

            <div class="card-body">
                <div class="form-section">
                    <h4 class="section-title">Selecionar Empresas</h4>
                    <p>Marque as empresas que este usuário poderá acessar:</p>

                    <div class="companies-list">
                        <asp:CheckBoxList ID="cblEmpresas" runat="server" CssClass="companies-checkbox-list">
                        </asp:CheckBoxList>
                    </div>
                </div>

                <div class="action-buttons">
                    <asp:Button ID="btnSalvar" runat="server" Text="Salvar Vinculações" CssClass="btn-save"
                        OnClick="btnSalvar_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>
