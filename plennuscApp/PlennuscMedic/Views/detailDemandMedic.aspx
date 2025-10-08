<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscMedic/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="detailDemandMedic.aspx.cs" Inherits="appWhatsapp.PlennuscMedic.Views.detailDemandMedic" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet">

    <style>
    :root {
        --primary: #6ebfe1;
        --primary-hover: #58c0eb;
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

      /* Cabeçalho da Demanda - CORREÇÃO DO TEXTO LONGO */
    .demand-header {
        background: white;
        border-radius: var(--border-radius);
        box-shadow: var(--shadow);
        padding: 32px;
        margin-bottom: 24px;
        border: 1px solid var(--gray-200);
        word-wrap: break-word;
        overflow-wrap: break-word;
    }

    .demand-title {
        font-size: 28px;
        font-weight: 700;
        color: var(--gray-900);
        margin-bottom: 8px;
        padding-bottom: 0;
        border-bottom: none;
        display: flex;
        align-items: center;
        justify-content: space-between;
        flex-wrap: wrap;
        gap: 16px;
        word-wrap: break-word;
        overflow-wrap: break-word;
    }

    .demand-description {
        background: var(--gray-50);
        border: 1px solid var(--gray-200);
        border-radius: var(--border-radius);
        padding: 20px;
        font-size: 16px;
        color: var(--gray-800);
        line-height: 1.6;
        margin-bottom: 24px;
        font-weight: 500;
        word-wrap: break-word;
        overflow-wrap: break-word;
        max-height: 200px; /* Altura máxima */
        overflow-y: auto; /* Scroll se necessário */
    }

            .demand-description:before {
                content: '"';
                font-size: 32px;
                color: var(--primary);
                position: absolute;
                top: 10px;
                left: 15px;
                font-family: Georgia, serif;
                opacity: 0.3;
            }

            .demand-description:after {
                content: '"';
                font-size: 32px;
                color: var(--primary);
                position: absolute;
                bottom: 10px;
                right: 15px;
                font-family: Georgia, serif;
                opacity: 0.3;
            }

            /* REMOVER HOVER DA DESCRIÇÃO */
            .demand-description:hover {
                border-color: var(--gray-200);
                box-shadow: none;
                transform: none;
            }
            /* REMOVER EFEITOS DESNECESSÁRIOS */
            .demand-description:hover,
            .meta-value:hover {
                transform: none;
                box-shadow: none;
            }

            /* REMOVER ASPAS DECORATIVAS */
            .demand-description:before,
            .demand-description:after {
                display: none;
            }


      /* Meta informações */
.demand-meta {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
    gap: 20px;
    margin-bottom: 0;
    padding: 0;
    background: transparent;
    border: none;
}

.meta-item {
    display: flex;
    flex-direction: column;
    padding: 0;
    background: transparent;
    border: none;
}

.meta-label {
    font-size: 12px;
    color: var(--gray-600);
    margin-bottom: 8px;
    font-weight: 600;
    text-transform: uppercase;
    letter-spacing: 0.5px;
}

.meta-value {
    font-size: 16px;
    font-weight: 500;
    color: var(--gray-800);
    padding: 12px 16px;
    background: var(--gray-50);
    border-radius: 6px;
    border: 1px solid var(--gray-200);
    word-wrap: break-word;
    overflow-wrap: break-word;
    min-height: 44px; /* Altura mínima consistente */
    display: flex;
    align-items: center;
}

        /* Status badge mais destacado */
        .status-badge {
            display: inline-block;
            padding: 8px 16px;
            border-radius: 6px;
            font-size: 14px;
            font-weight: 600;
            margin-left: 0;
            box-shadow: none;
            border: 1px solid;
        }

        .status-open {
            background: #e6f4ea;
            color: #137333;
            border-color: #b6e2c1;
        }

        .status-closed {
            background: #fce8e6;
            color: #c5221f;
            border-color: #f4b8b4;
        }
/* Container dos botões - ALINHAMENTO CORRETO */
.button-container {
    display: flex;
    justify-content: flex-end;
    gap: 12px;
    margin: 24px 0;
    padding: 0;
    width: 100%;
}

/* ESTILO GHOST PARA TODOS OS BOTÕES */
.btn-primary, .btn-close-demand, .btn-refuse {
    background: var(--gray-100);
    border: 1px solid var(--gray-300);
    color: var(--gray-700);
    font-weight: 500;
    padding: 10px 20px;
    border-radius: 6px;
    font-size: 14px;
    text-decoration: none;
    cursor: pointer;
    display: inline-flex;
    align-items: center;
    gap: 8px;
    transition: all 0.3s ease;
    height: 40px;
}

/* HOVER ESPECÍFICO PARA CADA BOTÃO */
.btn-primary:hover {
    background: var(--primary);
    border-color: var(--primary);
    color: white;
    transform: translateY(-1px);
    box-shadow: 0 2px 6px rgba(110, 191, 225, 0.2);
}

.btn-close-demand:hover {
    background: var(--success);
    border-color: var(--success);
    color: white;
    transform: translateY(-1px);
    box-shadow: 0 2px 6px rgba(76, 176, 122, 0.2);
}

.btn-refuse:hover {
    background: var(--danger);
    border-color: var(--danger);
    color: white;
    transform: translateY(-1px);
    box-shadow: 0 2px 6px rgba(234, 67, 53, 0.2);
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
/* Seção de Acompanhamentos - Estilo WhatsApp */
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
    flex-shrink: 0;
}

.accompaniments-list {
    padding: 16px;
    overflow-y: auto;
    flex: 1;
    display: flex;
    flex-direction: column;
    gap: 12px;
    background: #f0f2f5; /* Cinza azulado bem suave */
    background-image: url("data:image/svg+xml,%3Csvg width='80' height='80' viewBox='0 0 80 80' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='%2390caf9' fill-opacity='0.03'%3E%3Ccircle cx='40' cy='40' r='30'/%3E%3C/g%3E%3C/svg%3E");
}
/* Item de Acompanhamento Base */
.accompaniment-item {
    max-width: 70%;
    padding: 8px 12px;
    border-radius: 7.5px;
    position: relative;
    margin: 2px 0;
    word-wrap: break-word;
}

/* Minhas Mensagens - Lado Direito */
.accompaniment-item.my-message {
    align-self: flex-end;
    background: #dcf8c6; /* Verde claro do WhatsApp */
    margin-left: auto;
    border-bottom-right-radius: 0;
}

.accompaniment-item.my-message .accompaniment-header {
    justify-content: flex-end;
}

.accompaniment-item.my-message .accompaniment-author {
    color: #1a73e8;
    font-weight: 600;
}

.accompaniment-item.my-message .accompaniment-date {
    color: #666;
}

/* Mensagens de Outros - Lado Esquerdo */
.accompaniment-item.other-message {
    align-self: flex-start;
    background: white;
    border-bottom-left-radius: 0;
    box-shadow: 0 1px 0.5px rgba(0, 0, 0, 0.13);
}

.accompaniment-item.other-message .accompaniment-author {
    color: #1a73e8;
    font-weight: 600;
}

.accompaniment-item.other-message .accompaniment-date {
    color: #666;
}

/* Cabeçalho das Mensagens */
.accompaniment-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 4px;
    gap: 8px;
}

.accompaniment-author {
    font-size: 12.5px;
    font-weight: 500;
}

.accompaniment-date {
    font-size: 10.5px;
    opacity: 0.8;
}

/* Conteúdo das Mensagens */
.accompaniment-content {
    font-size: 13.5px;
    line-height: 1.4;
    word-wrap: break-word;
    color: #303030;
}

/* Seta das mensagens - Estilo WhatsApp */
.accompaniment-item.my-message::after {
    content: '';
    position: absolute;
    bottom: 0;
    right: -8px;
    width: 0;
    height: 0;
    border-left: 8px solid #dcf8c6;
    border-top: 8px solid transparent;
    border-bottom: 8px solid transparent;
}

.accompaniment-item.other-message::before {
    content: '';
    position: absolute;
    bottom: 0;
    left: -8px;
    width: 0;
    height: 0;
    border-right: 8px solid white;
    border-top: 8px solid transparent;
    border-bottom: 8px solid transparent;
}

/* Scrollbar personalizada */
.accompaniments-list::-webkit-scrollbar {
    width: 6px;
}

.accompaniments-list::-webkit-scrollbar-track {
    background: transparent;
}

.accompaniments-list::-webkit-scrollbar-thumb {
    background: #cccccc;
    border-radius: 3px;
}

.accompaniments-list::-webkit-scrollbar-thumb:hover {
    background: #aaaaaa;
}
  .side-sections-container {
        display: flex;
        flex-direction: column;
        gap: 24px;
        height: fit-content;
    }

    /* Ajuste na altura da seção de acompanhamentos */
    .accompaniments-section {
        background: white;
        border-radius: var(--border-radius);
        box-shadow: var(--shadow);
        overflow: hidden;
        height: 600px; /* Reduzida para caber junto com o histórico */
        display: flex;
        flex-direction: column;
    }

/* Responsividade */
@media (max-width: 768px) {
    
    .accompaniment-item {
        max-width: 85%;
    }
    
    .accompaniment-header {
        flex-direction: column;
        align-items: flex-start;
        gap: 2px;
    }
    
    .accompaniment-item.my-message .accompaniment-header {
        align-items: flex-end;
    }
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
             min-height: 600px;
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
            min-height: 200px;
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
                margin-top: 0; /* Remove a margem superior anterior */
                flex: 1;
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
        max-height: 300px;
        overflow-y: auto;
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

        /* Estilos para a seção de upload de anexos */
        .attachment-upload-section {
            background: var(--gray-50);
            border: 1px solid var(--gray-200);
            border-radius: var(--border-radius);
            padding: 20px;
            margin: 16px 0;
        }

        .upload-header {
            display: flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 16px;
            font-weight: 500;
            color: var(--gray-700);
        }

        .file-counter {
            font-size: 12px;
            color: var(--gray-500);
            margin-left: auto;
        }

        .file-upload-area {
            border: 2px dashed var(--gray-300);
            border-radius: var(--border-radius);
            padding: 32px 16px;
            text-align: center;
            background: white;
            transition: var(--transition);
            cursor: pointer;
            margin-bottom: 12px;
        }

            .file-upload-area:hover {
                border-color: var(--primary);
                background: var(--gray-50);
            }

            .file-upload-area.dragover {
                border-color: var(--primary);
                background: rgba(26, 115, 232, 0.05);
            }

        .upload-content {
            display: flex;
            flex-direction: column;
            align-items: center;
            gap: 8px;
        }

        .upload-icon {
            font-size: 32px;
            color: var(--gray-400);
        }

        .upload-title {
            font-weight: 500;
            color: var(--gray-700);
            margin: 0;
        }

        .upload-subtitle {
            color: var(--gray-500);
            margin: 0;
            font-size: 14px;
        }

        .btn-upload {
            background: var(--primary);
            color: white;
            border: none;
            padding: 8px 16px;
            border-radius: 4px;
            font-size: 14px;
            font-weight: 500;
            cursor: pointer;
            transition: var(--transition);
            display: inline-flex;
            align-items: center;
            gap: 6px;
        }

            .btn-upload:hover {
                background: var(--primary-hover);
                transform: translateY(-1px);
            }

        .upload-info {
            margin-top: 12px;
        }

        .input-hint {
            font-size: 12px;
            color: var(--gray-500);
            display: flex;
            align-items: center;
            gap: 6px;
        }

        /* Preview dos arquivos */
        .file-preview-container {
            margin-top: 16px;
            background: white;
            border: 1px solid var(--gray-200);
            border-radius: var(--border-radius);
            overflow: hidden;
        }

        .preview-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 12px 16px;
            background: var(--gray-50);
            border-bottom: 1px solid var(--gray-200);
        }

            .preview-header h6 {
                margin: 0;
                font-weight: 500;
                color: var(--gray-700);
            }

        .btn-clear {
            background: none;
            border: 1px solid var(--gray-300);
            color: var(--gray-600);
            padding: 4px 8px;
            border-radius: 4px;
            font-size: 12px;
            cursor: pointer;
            transition: var(--transition);
            display: flex;
            align-items: center;
            gap: 4px;
        }

            .btn-clear:hover {
                background: var(--gray-100);
                color: var(--danger);
            }

        .file-preview-list {
            max-height: 200px;
            overflow-y: auto;
            padding: 8px;
        }

        .file-preview-item {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 8px 12px;
            background: var(--gray-50);
            border-radius: 4px;
            margin-bottom: 4px;
            border: 1px solid var(--gray-200);
        }

        .file-preview-info {
            display: flex;
            align-items: center;
            gap: 8px;
            flex: 1;
        }

        .file-preview-name {
            font-size: 14px;
            color: var(--gray-700);
            font-weight: 500;
        }

        .file-preview-size {
            font-size: 12px;
            color: var(--gray-500);
        }

        .file-preview-remove {
            background: none;
            border: none;
            color: var(--gray-500);
            cursor: pointer;
            padding: 4px;
            border-radius: 2px;
            transition: var(--transition);
        }

            .file-preview-remove:hover {
                color: var(--danger);
                background: var(--gray-100);
            }

        /* Estilos para a seção de anexos */
        .attachments-section {
            background: white;
            border-radius: var(--border-radius);
            box-shadow: var(--shadow);
            margin-bottom: 24px;
            overflow: hidden;
        }

        .attachments-list {
            padding: 16px;
        }

        .attachment-item {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 12px;
            background: var(--gray-50);
            border-radius: 6px;
            margin-bottom: 8px;
            border: 1px solid var(--gray-200);
            transition: var(--transition);
        }

            .attachment-item:hover {
                background: var(--gray-100);
                box-shadow: 0 1px 3px rgba(0, 0, 0, 0.1);
            }

        .attachment-info {
            display: flex;
            align-items: center;
            gap: 10px;
            flex: 1;
        }

        .attachment-icon {
            font-size: 20px;
            color: var(--gray-600);
            flex-shrink: 0;
        }

        .attachment-details {
            overflow: hidden;
        }

        .attachment-name {
            font-weight: 500;
            color: var(--gray-800);
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
            max-width: 300px;
        }

        .attachment-meta strong {
            color: #137333;
            font-weight: 600;
        }

        .attachment-meta {
            font-size: 12px;
            color: var(--gray-600);
            display: flex;
            gap: 8px;
            flex-wrap: wrap;
            align-items: center;
        }

        .btn-download {
            background: var(--primary);
            color: white;
            padding: 6px 12px;
            border-radius: 4px;
            text-decoration: none;
            font-size: 14px;
            transition: var(--transition);
            display: flex;
            align-items: center;
            gap: 4px;
            white-space: nowrap;
        }

            .btn-download:hover {
                background: var(--primary-hover);
                color: white;
            }

        .no-attachments {
            padding: 24px;
            text-align: center;
            color: var(--gray-500);
            font-style: italic;
        }

        .toast-container {
            position: fixed;
            right: 20px;
            top: 20px;
            z-index: 99999;
        }

        .toast {
            min-width: 260px;
            max-width: 380px;
            padding: 12px 16px;
            border-radius: 8px;
            box-shadow: 0 6px 18px rgba(0,0,0,0.12);
            color: #fff;
            margin-bottom: 10px;
            font-family: Roboto, Arial, sans-serif;
            opacity: 0;
            transform: translateY(-10px);
            transition: all .25s ease;
        }

            .toast.show {
                opacity: 1;
                transform: translateY(0);
            }

            .toast.success {
                background: #28a745;
            }

            .toast.error {
                background: #dc3545;
            }

            .toast .title {
                font-weight: 600;
                margin-bottom: 4px;
            }

            .toast .msg {
                font-size: 13px;
            }
/* Controle de Status Individual */
.status-control-single {
    display: flex;
    justify-content: flex-start;
    margin: 24px 0;
    padding: 0;
}

.status-selector-single {
    display: flex;
    align-items: center;
    gap: 12px;
    padding: 16px 20px;
    background: white;
    border-radius: var(--border-radius);
    box-shadow: var(--shadow);
    border: 1px solid var(--gray-200);
}

.status-label-single {
    font-size: 14px;
    font-weight: 600;
    color: var(--gray-700);
    margin: 0;
    display: flex;
    align-items: center;
    gap: 8px;
    white-space: nowrap;
}

.status-dropdown-single {
    padding: 10px 16px;
    border: 1px solid var(--gray-300);
    border-radius: 6px;
    background: white;
    font-size: 14px;
    color: var(--gray-800);
    min-width: 200px;
    transition: var(--transition);
    cursor: pointer;
}

.status-dropdown-single:focus {
    outline: none;
    border-color: var(--primary);
    box-shadow: 0 0 0 2px rgba(110, 191, 225, 0.2);
}

/* Container Final de Ações - LADO DIREITO */
.final-actions-container {
    display: flex;
    justify-content: flex-end;
    margin: 32px 0 24px 0;
    padding: 0;
}

.final-buttons-group {
    display: flex;
    gap: 12px;
    flex-wrap: wrap;
    justify-content: flex-end;
}

/* Botões Finais - Estilo Ghost Padronizado */
.btn-final {
    background: var(--gray-100);
    border: 1px solid var(--gray-300);
    color: var(--gray-700);
    font-weight: 500;
    padding: 12px 20px;
    border-radius: 6px;
    font-size: 14px;
    text-decoration: none;
    cursor: pointer;
    display: inline-flex;
    align-items: center;
    gap: 8px;
    transition: all 0.3s ease;
    height: 44px;
    min-width: 140px;
    justify-content: center;
}

/* Hover específico para cada botão final */
.btn-final.btn-primary:hover {
    background: var(--primary);
    border-color: var(--primary);
    color: white;
    transform: translateY(-1px);
    box-shadow: 0 2px 6px rgba(110, 191, 225, 0.2);
}

.btn-final.btn-close-demand:hover {
    background: var(--success);
    border-color: var(--success);
    color: white;
    transform: translateY(-1px);
    box-shadow: 0 2px 6px rgba(76, 176, 122, 0.2);
}

.btn-final.btn-refuse:hover {
    background: var(--danger);
    border-color: var(--danger);
    color: white;
    transform: translateY(-1px);
    box-shadow: 0 2px 6px rgba(234, 67, 53, 0.2);
}

/* Status atual no cabeçalho */
.current-status-value {
    font-weight: 600;
    color: var(--primary);
}

/* Responsividade */
@media (max-width: 768px) {
    .status-control-single {
        justify-content: center;
    }
    
    .status-selector-single {
        flex-direction: column;
        align-items: stretch;
        text-align: center;
        gap: 8px;
    }
    
    .status-dropdown-single {
        min-width: 100%;
    }
    
    .final-actions-container {
        justify-content: center;
    }
    
    .final-buttons-group {
        justify-content: center;
        width: 100%;
    }
}

@media (max-width: 576px) {
    .final-buttons-group {
        flex-direction: column;
        width: 100%;
    }
    
    .btn-final {
        width: 100%;
        min-width: auto;
    }
}
             .btn-primary:disabled,
    .btn-refuse:disabled,
    .btn-close-demand:disabled {
        opacity: 0.5 !important;
        pointer-events: none !important;
        cursor: not-allowed !important;
    }

        /* Responsividade para anexos */
        @media (max-width: 768px) {
            .attachment-item {
                flex-direction: column;
                align-items: flex-start;
                gap: 12px;
            }

            .btn-download {
                align-self: flex-end;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="toast-container" id="globalToastContainer" runat="server" style="display: none"></div>

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
                    <span class="meta-value">
                        <asp:Label ID="lblSolicitante" runat="server" /></span>
                </div>
                <div class="meta-item">
                    <span class="meta-label">Data</span>
                    <span class="meta-value">
                        <asp:Label ID="lblDataSolicitacao" runat="server" /></span>
                </div>
              <%--  <div class="meta-item">
                    <span class="meta-label">Status Atual</span>
                    <span class="meta-value">
                        <asp:Label ID="lblStatusAtual" runat="server" CssClass="current-status-value" />
                    </span>
                </div>--%>
            </div>
        </div>

        <asp:HiddenField ID="hdnStatusOriginal" runat="server" />

        <!-- Seção de Anexos EXISTENTES -->
        <div class="attachments-section">
            <div class="section-header">
                <i class="bi bi-paperclip"></i>
                Anexos da Demanda
            </div>
            <div class="attachments-list">
                <asp:Repeater ID="rptAnexos" runat="server">
                    <ItemTemplate>
                        <div class="attachment-item">
                            <div class="attachment-info">
                                <i class="bi bi-file-earmark attachment-icon"></i>
                                <div class="attachment-details">
                                    <div class="attachment-name"><%# Eval("NomeArquivo") %></div>
                                    <div class="attachment-meta">
                                        <%# Eval("DataEnvio", "{0:dd/MM/yyyy HH:mm}") %>
                                        • <%# Eval("TamanhoFormatado") %>
                                        • Enviado por: <strong><%# Eval("NomeUsuarioUpload") %></strong>
                                    </div>
                                </div>
                            </div>
                            <a href='<%# Eval("CaminhoDownload") %>' target="_blank" class="btn-download">
                                <i class="bi bi-download"></i>Download
                            </a>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Label ID="lblSemAnexos" runat="server" Text="Nenhum anexo encontrado."
                    CssClass="no-attachments" />
            </div>
        </div>

        <!-- Controle de Status (Agora sozinho) -->
        <div class="status-control-single">
            <div class="status-selector-single">
                <label class="status-label-single">
                    <i class="bi bi-arrow-repeat"></i>
                    Alterar Status:
                </label>
                <asp:DropDownList ID="ddlStatusAcompanhamento" runat="server" CssClass="status-dropdown-single">
                </asp:DropDownList>
            </div>
        </div>

        <div class="main-layout">
            <!-- Seção Principal - NOVO ACOMPANHAMENTO -->
            <div class="editor-section" id="editorSection" runat="server">
                <div class="section-header">
                    <i class="bi bi-pencil-square"></i>
                    Novo Acompanhamento
                </div>
                <div class="editor-container">
                    <asp:TextBox ID="txtNovoAcompanhamento" runat="server"
                        CssClass="editor-textarea" TextMode="MultiLine"
                        placeholder="Digite seu acompanhamento..." Rows="8" />

                    <!-- SEÇÃO DE UPLOAD DE ANEXOS PARA O ACOMPANHAMENTO -->
                    <div class="attachment-upload-section">
                        <div class="upload-header">
                            <i class="bi bi-paperclip"></i>
                            <span>Anexar arquivos</span>
                            <span class="file-counter" id="fileCounter">0 arquivos selecionados</span>
                        </div>

                        <div class="file-upload-area" id="fileUploadArea">
                            <div class="upload-content">
                                <i class="bi bi-cloud-arrow-up upload-icon"></i>
                                <p class="upload-title">Arraste arquivos aqui</p>
                                <p class="upload-subtitle">ou</p>
                                <button type="button" class="btn-upload"
                                    onclick="document.getElementById('<%= fuAnexos.ClientID %>').click()">
                                    <i class="bi bi-folder2-open"></i>Selecionar Arquivos
                                </button>
                            </div>
                            <asp:FileUpload ID="fuAnexos" runat="server" CssClass="d-none" AllowMultiple="true"
                                onchange="handleFileSelection(this);" />
                        </div>

                        <div class="upload-info">
                            <div class="input-hint">
                                <i class="bi bi-info-circle"></i>
                                Formatos: PDF, Word, Excel, imagens (JPG, PNG, GIF) • Máx. 10MB cada
                            </div>
                        </div>

                        <!-- Preview dos Arquivos -->
                        <div id="filePreviewContainer" class="file-preview-container" style="display: none;">
                            <div class="preview-header">
                                <h6>Arquivos selecionados</h6>
                                <button type="button" class="btn-clear" onclick="limparAnexos()">
                                    <i class="bi bi-x-circle"></i>Limpar todos
                                </button>
                            </div>
                            <div id="filePreviewList" class="file-preview-list"></div>
                        </div>
                    </div>

                    <asp:Button ID="btnAdicionarAcompanhamento" runat="server"
                        CssClass="btn-send" Text="Enviar Acompanhamento"
                        OnClick="btnAdicionarAcompanhamento_Click" />
                </div>
            </div>

        <!-- Seção Lateral - ACOMPANHAMENTOS EXISTENTES -->
   <!-- Coluna da Direita (Acompanhamentos + Histórico) -->
    <div class="side-sections-container">
        <!-- Seção de Acompanhamentos -->
        <div class="accompaniments-section">
            <div class="section-header">
                <i class="bi bi-chat-text"></i>
                Acompanhamentos
            </div>
            <div class="accompaniments-list">
                <asp:Repeater ID="rptAcompanhamentos" runat="server">
                    <ItemTemplate>
                        <div class="accompaniment-item <%# IsMyMessage(Convert.ToInt32(Eval("CodPessoaAcompanhamento"))) ? "my-message" : "other-message" %>">
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

        <!-- Histórico de Status (Agora ao lado dos acompanhamentos) -->
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
    
    </div>
        <!-- Botões de Ação - AGORA NO FINAL LADO DIREITO -->
    <div class="final-actions-container">
        <div class="final-buttons-group">
            <asp:LinkButton ID="btnRecusar" runat="server" CssClass="btn-final btn-refuse"
                OnClick="btnRecusar_Click" Visible="false">
                <i class="bi bi-x-circle"></i> Recusar
            </asp:LinkButton>

            <asp:LinkButton ID="btnSolicitarAprovacao" runat="server" CssClass="btn-final btn-primary"
                OnClick="btnSolicitarAprovacao_Click" Visible="false">
                <i class="bi bi-check-circle"></i> Solicitar Aprovação
            </asp:LinkButton>

            <asp:LinkButton ID="btnEncerrar" runat="server" CssClass="btn-final btn-close-demand"
                OnClick="btnEncerrar_Click" Visible="false">
                <i class="bi bi-check-lg"></i> Concluir
            </asp:LinkButton>
        </div>
    </div>
        </div>
<script>
    function configurarControleBotoes() {
        const ddlStatus = document.getElementById('<%= ddlStatusAcompanhamento.ClientID %>');
        const hdnStatusOriginal = document.getElementById('<%= hdnStatusOriginal.ClientID %>');

        const btnSolicitarAprovacao = document.getElementById('<%= btnSolicitarAprovacao.ClientID %>');
        const btnRecusar = document.getElementById('<%= btnRecusar.ClientID %>');
        const btnEncerrar = document.getElementById('<%= btnEncerrar.ClientID %>');
        const btnAdicionarAcompanhamento = document.getElementById('<%= btnAdicionarAcompanhamento.ClientID %>'); // 🔥 NOVO

        if (ddlStatus && hdnStatusOriginal) {
            ddlStatus.addEventListener('change', function () {
                const statusAtual = ddlStatus.value;
                const statusOriginal = hdnStatusOriginal.value;
                const textoStatusAtual = ddlStatus.options[ddlStatus.selectedIndex].text;

                // Habilita cada botão apenas para seu status específico
                const habilitarSolicitar = (statusAtual !== statusOriginal) && textoStatusAtual === "Aguardando Aprovação";
                const habilitarRecusar = (statusAtual !== statusOriginal) && textoStatusAtual === "Recusada";
                const habilitarConcluir = (statusAtual !== statusOriginal) && textoStatusAtual === "Concluída";

                // 🔥 NOVO: Desabilita botão de acompanhamento para "Aguardando Aprovação"
                const desabilitarAcompanhamento = textoStatusAtual === "Aguardando Aprovação";

                if (btnSolicitarAprovacao) {
                    btnSolicitarAprovacao.disabled = !habilitarSolicitar;
                    btnSolicitarAprovacao.style.opacity = habilitarSolicitar ? '1' : '0.5';
                    btnSolicitarAprovacao.style.pointerEvents = habilitarSolicitar ? 'auto' : 'none';
                }

                if (btnRecusar) {
                    btnRecusar.disabled = !habilitarRecusar;
                    btnRecusar.style.opacity = habilitarRecusar ? '1' : '0.5';
                    btnRecusar.style.pointerEvents = habilitarRecusar ? 'auto' : 'none';
                }

                if (btnEncerrar) {
                    btnEncerrar.disabled = !habilitarConcluir;
                    btnEncerrar.style.opacity = habilitarConcluir ? '1' : '0.5';
                    btnEncerrar.style.pointerEvents = habilitarConcluir ? 'auto' : 'none';
                }

                // 🔥 NOVO: Controla botão de acompanhamento
                if (btnAdicionarAcompanhamento) {
                    btnAdicionarAcompanhamento.disabled = desabilitarAcompanhamento;
                    btnAdicionarAcompanhamento.style.opacity = desabilitarAcompanhamento ? '0.5' : '1';
                    btnAdicionarAcompanhamento.style.pointerEvents = desabilitarAcompanhamento ? 'none' : 'auto';
                    btnAdicionarAcompanhamento.title = desabilitarAcompanhamento
                        ? "Status aguardando aprovação - acompanhamento bloqueado"
                        : "Enviar Acompanhamento";
                }
            });

            // Configuração inicial - desabilita todos os botões
            if (btnSolicitarAprovacao) {
                btnSolicitarAprovacao.disabled = true;
                btnSolicitarAprovacao.style.opacity = '0.5';
                btnSolicitarAprovacao.style.pointerEvents = 'none';
            }

            if (btnRecusar) {
                btnRecusar.disabled = true;
                btnRecusar.style.opacity = '0.5';
                btnRecusar.style.pointerEvents = 'none';
            }

            if (btnEncerrar) {
                btnEncerrar.disabled = true;
                btnEncerrar.style.opacity = '0.5';
                btnEncerrar.style.pointerEvents = 'none';
            }

            // 🔥 NOVO: Configuração inicial do botão de acompanhamento
            if (btnAdicionarAcompanhamento) {
                const textoStatusAtual = ddlStatus.options[ddlStatus.selectedIndex].text;
                const desabilitarAcompanhamento = textoStatusAtual === "Aguardando Aprovação";

                btnAdicionarAcompanhamento.disabled = desabilitarAcompanhamento;
                btnAdicionarAcompanhamento.style.opacity = desabilitarAcompanhamento ? '0.5' : '1';
                btnAdicionarAcompanhamento.style.pointerEvents = desabilitarAcompanhamento ? 'none' : 'auto';
                btnAdicionarAcompanhamento.title = desabilitarAcompanhamento
                    ? "Status aguardando aprovação - acompanhamento bloqueado"
                    : "Enviar Acompanhamento";
            }
        }
    }

    // Executa quando a página carrega
    document.addEventListener('DOMContentLoaded', function () {
        configurarControleBotoes();
    });
</script>

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

                    // Desabilitar upload de arquivos também
                    const fileUploadArea = document.querySelector('.file-upload-area');
                    if (fileUploadArea) {
                        fileUploadArea.style.opacity = '0.5';
                        fileUploadArea.style.pointerEvents = 'none';
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
            setupDragAndDrop();
        });
    </script>

    <script>
        function createToast(message, type, title) {
            var container = document.getElementById('globalToastContainer');
            if (!container) return;
            container.style.display = 'block';

            var toast = document.createElement('div');
            toast.className = 'toast ' + type;
            toast.innerHTML = (title ? '<div class=\"title\">' + title + '</div>' : '') +
                '<div class=\"msg\">' + message + '</div>';
            container.appendChild(toast);

            // show
            setTimeout(function () { toast.classList.add('show'); }, 10);

            // remove after 5s
            setTimeout(function () {
                toast.classList.remove('show');
                setTimeout(function () { try { container.removeChild(toast); } catch (e) { } }, 300);
            }, 5000);
        }

        function showToastSucesso(message) {
            createToast(message, 'success', 'Sucesso');
        }
        function showToastErro(message) {
            createToast(message, 'error', 'Erro');
        }
        function showToastAviso(message) {
            createToast(message, 'warning', 'Aviso');
        }
    </script>

    <script>
        // Variável global para armazenar os arquivos selecionados
        let selectedFiles = [];

        // Função para lidar com a seleção de arquivos
        function handleFileSelection(fileInput) {
            if (fileInput.files.length > 0) {
                selectedFiles = Array.from(fileInput.files);
                updateFilePreview();

                // Mostrar toast de sucesso
                showToastSucesso(selectedFiles.length + ' arquivo(s) selecionado(s) com sucesso!');
            }
        }

        // Função para atualizar o preview dos arquivos
        function updateFilePreview() {
            const container = document.getElementById('filePreviewContainer');
            const list = document.getElementById('filePreviewList');
            const counter = document.getElementById('fileCounter');

            if (selectedFiles.length === 0) {
                container.style.display = 'none';
                list.innerHTML = '';
                counter.textContent = '0 arquivos selecionados';
                return;
            }

            container.style.display = 'block';
            list.innerHTML = '';
            counter.textContent = selectedFiles.length + ' arquivo(s) selecionado(s)';

            selectedFiles.forEach((file, index) => {
                const fileItem = document.createElement('div');
                fileItem.className = 'file-preview-item';
                fileItem.innerHTML = `
            <div class="file-preview-info">
                <i class="bi bi-file-earmark"></i>
                <span class="file-preview-name">${file.name}</span>
                <span class="file-preview-size">(${formatFileSize(file.size)})</span>
            </div>
            <button type="button" class="file-preview-remove" onclick="removeFile(${index})">
                <i class="bi bi-x-circle"></i>
            </button>
        `;
                list.appendChild(fileItem);
            });
        }


        // Função para formatar o tamanho do arquivo
        function formatFileSize(bytes) {
            if (bytes === 0) return '0 Bytes';
            const k = 1024;
            const sizes = ['Bytes', 'KB', 'MB', 'GB'];
            const i = Math.floor(Math.log(bytes) / Math.log(k));
            return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
        }

        // Função para remover arquivo da seleção
        function removeFile(index) {
            selectedFiles.splice(index, 1);
            updateFilePreview();

            // Atualizar o FileUpload ASP.NET
            const fileInput = document.getElementById('<%= fuAnexos.ClientID %>');
            const dataTransfer = new DataTransfer();

            selectedFiles.forEach(file => {
                dataTransfer.items.add(file);
            });

            fileInput.files = dataTransfer.files;
        }

        // Função para arrastar e soltar arquivos (atualizada)
        function setupDragAndDrop() {
            const dropArea = document.getElementById('fileUploadArea');
            if (!dropArea) return;

            ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
                dropArea.addEventListener(eventName, preventDefaults, false);
            });

            function preventDefaults(e) {
                e.preventDefault();
                e.stopPropagation();
            }

            ['dragenter', 'dragover'].forEach(eventName => {
                dropArea.addEventListener(eventName, function () {
                    dropArea.classList.add('dragover');
                }, false);
            });

            ['dragleave', 'drop'].forEach(eventName => {
                dropArea.addEventListener(eventName, function () {
                    dropArea.classList.remove('dragover');
                }, false);
            });

            dropArea.addEventListener('drop', handleDrop, false);

            function handleDrop(e) {
                const dt = e.dataTransfer;
                const files = dt.files;

                if (files.length > 0) {
                    const fileInput = document.getElementById('<%= fuAnexos.ClientID %>');

                    // Combinar arquivos existentes com novos
                    const allFiles = [...selectedFiles, ...Array.from(files)];
                    selectedFiles = allFiles;

                    // Atualizar o FileUpload ASP.NET
                    const dataTransfer = new DataTransfer();
                    selectedFiles.forEach(file => {
                        dataTransfer.items.add(file);
                    });

                    fileInput.files = dataTransfer.files;
                    updateFilePreview();

                    showToastSucesso(files.length + ' arquivo(s) adicionado(s)!');
                }
            }
        }
        // Função para limpar os anexos (chamada após o envio)
        function limparAnexos() {
            selectedFiles = [];
            updateFilePreview();

            const fileInput = document.getElementById('<%= fuAnexos.ClientID %>');
            fileInput.value = '';
        }
    </script>  
</asp:Content>

