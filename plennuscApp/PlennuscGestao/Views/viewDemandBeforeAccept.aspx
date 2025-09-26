<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="viewDemandBeforeAccept.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.viewDemandBeforeAccept" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <style>
        :root {
            --primary: #6ebfe1;
            --primary-hover: #5ca8c7;
            --success: #4cb07a;
            --success-hover: #3b8b65;
            --gray-400: #6c757d;
            --gray-500: #5a6268;
            --gray-100: #f8f9fa;
            --gray-200: #e8eaed;
            --gray-300: #f1f3f4;
            --text-primary: #202124;
            --text-secondary: #5f6368;
        }

        .container-main {
            max-width: 1200px;
            margin: 20px auto;
            padding: 0 16px;
        }

        .demand-header {
            background: white;
            border-radius: 8px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.1);
            padding: 30px;
            margin-bottom: 24px;
        }

        .demand-title {
            font-size: 24px;
            font-weight: 600;
            color: var(--text-primary);
            margin-bottom: 20px;
            padding-bottom: 15px;
            border-bottom: 2px solid var(--gray-300);
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .status-badge {
            padding: 4px 12px;
            border-radius: 12px;
            font-size: 12px;
            font-weight: 600;
        }

        .status-open {
            background: #e7f4e4;
            color: #2e7d32;
        }

        .demand-details-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }

        .detail-group {
            background: var(--gray-100);
            padding: 15px;
            border-radius: 6px;
            border-left: 4px solid var(--primary);
        }

        .detail-label {
            font-size: 12px;
            color: var(--text-secondary);
            font-weight: 600;
            text-transform: uppercase;
            margin-bottom: 5px;
            display: block;
        }

        .detail-value {
            font-size: 16px;
            color: var(--text-primary);
            font-weight: 500;
        }

        .demand-description {
            background: var(--gray-100);
            border: 1px solid var(--gray-200);
            border-radius: 8px;
            padding: 20px;
            margin-bottom: 30px;
            line-height: 1.6;
        }

        .description-label {
            font-size: 14px;
            color: var(--text-secondary);
            font-weight: 600;
            margin-bottom: 10px;
        }

        .description-text {
            font-size: 15px;
            color: var(--text-primary);
            white-space: pre-wrap;
        }

        .attachments-section {
            background: white;
            border-radius: 8px;
            box-shadow: 0 1px 3px rgba(0,0,0,0.1);
            padding: 20px;
            margin-bottom: 24px;
        }

        .attachments-title {
            font-size: 18px;
            font-weight: 600;
            color: var(--text-primary);
            margin-bottom: 15px;
            padding-bottom: 10px;
            border-bottom: 1px solid var(--gray-300);
        }

        .attachment-item {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 10px 0;
            border-bottom: 1px solid var(--gray-200);
        }

        .attachment-info {
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .attachment-icon {
            width: 24px;
            height: 24px;
            background-color: var(--gray-300);
            border-radius: 4px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 12px;
            color: var(--text-secondary);
        }

        .attachment-name {
            font-size: 14px;
            color: var(--text-primary);
        }

        .attachment-details {
            font-size: 12px;
            color: var(--text-secondary);
        }

        .btn-download {
            background: var(--primary);
            border: none;
            color: white;
            font-weight: 500;
            padding: 6px 12px;
            border-radius: 4px;
            font-size: 12px;
            text-decoration: none;
            cursor: pointer;
            transition: all 0.3s ease;
        }

        .btn-download:hover {
            background: var(--primary-hover);
        }

        .button-container {
            display: flex;
            justify-content: center;
            gap: 20px;
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid var(--gray-200);
        }

        .btn-accept {
            background: var(--success);
            border: 1px solid var(--success-hover);
            color: white;
            font-weight: 600;
            padding: 12px 24px;
            border-radius: 6px;
            font-size: 14px;
            text-decoration: none;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            transition: all 0.3s ease;
        }

        .btn-accept:hover {
            background: var(--success-hover);
            transform: translateY(-1px);
        }

        .btn-back {
            background: var(--gray-400);
            border: 1px solid var(--gray-500);
            color: white;
            font-weight: 500;
            padding: 12px 24px;
            border-radius: 6px;
            font-size: 14px;
            text-decoration: none;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            transition: all 0.3s ease;
        }

        .btn-back:hover {
            background: var(--gray-500);
        }

        .section-header {
            font-size: 18px;
            font-weight: 600;
            color: var(--text-primary);
            margin-bottom: 15px;
            padding-bottom: 10px;
            border-bottom: 1px solid var(--gray-300);
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .meta-item {
            background: var(--gray-100);
            padding: 15px;
            border-radius: 6px;
            border-left: 4px solid var(--primary);
            margin-bottom: 10px;
        }

        .meta-label {
            font-size: 12px;
            color: var(--text-secondary);
            font-weight: 600;
            text-transform: uppercase;
            margin-bottom: 5px;
            display: block;
        }

        .meta-value {
            font-size: 16px;
            color: var(--text-primary);
            font-weight: 500;
        }

        .no-attachments {
            text-align: center;
            color: var(--text-secondary);
            font-style: italic;
            padding: 20px;
        }

        @media (max-width: 768px) {
            .demand-details-grid {
                grid-template-columns: 1fr;
            }
            
            .button-container {
                flex-direction: column;
                align-items: center;
            }
            
            .btn-accept, .btn-back {
                width: 100%;
                max-width: 300px;
                justify-content: center;
            }
            
            .demand-title {
                flex-direction: column;
                align-items: flex-start;
                gap: 10px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <div class="demand-header">
            <h1 class="demand-title">
                <asp:Label ID="lblTitulo" runat="server"></asp:Label>
                <asp:Label ID="lblStatusBadge" runat="server" CssClass="status-badge status-open" Text="Aberta" />
            </h1>

            <!-- Detalhes da Demanda -->
            <div class="demand-details-grid">
                <div class="detail-group">
                    <span class="detail-label">ID da Demanda</span>
                    <span class="detail-value">
                        <asp:Label ID="lblCodDemanda" runat="server" />
                    </span>
                </div>
                <div class="detail-group">
                    <span class="detail-label">Solicitante</span>
                    <span class="detail-value">
                        <asp:Label ID="lblSolicitante" runat="server" />
                    </span>
                </div>
                <div class="detail-group">
                    <span class="detail-label">Data de Abertura</span>
                    <span class="detail-value">
                        <asp:Label ID="lblDataSolicitacao" runat="server" />
                    </span>
                </div>
                <div class="detail-group">
                    <span class="detail-label">Categoria</span>
                    <span class="detail-value">
                        <asp:Label ID="lblCategoria" runat="server" />
                    </span>
                </div>
                <div class="detail-group">
                    <span class="detail-label">Prioridade</span>
                    <span class="detail-value">
                        <asp:Label ID="lblPrioridade" runat="server" />
                    </span>
                </div>
                <div class="detail-group">
                    <span class="detail-label">Importância</span>
                    <span class="detail-value">
                        <asp:Label ID="lblImportancia" runat="server" />
                    </span>
                </div>
                <div class="detail-group">
                    <span class="detail-label">Prazo Máximo</span>
                    <span class="detail-value">
                        <asp:Label ID="lblPrazo" runat="server" />
                    </span>
                </div>
            </div>

            <!-- Descrição da Demanda -->
            <div class="demand-description">
                <div class="description-label">Descrição</div>
                <div class="description-text">
                    <asp:Label ID="lblTexto" runat="server"></asp:Label>
                </div>
            </div>

            <!-- Anexos -->
            <div class="attachments-section">
                <div class="section-header">
                    <i class="bi bi-paperclip"></i> Anexos da Demanda
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
                                            <span><%# Eval("TamanhoFormatado") %></span>
                                            <span>•</span>
                                            <span>Enviado em <%# Eval("DataEnvio", "{0:dd/MM/yyyy HH:mm}") %></span>
                                        </div>
                                    </div>
                                </div>
                                <a href='<%# Eval("CaminhoDownload") %>' class="btn-download" download>
                                    <i class="bi bi-download"></i> Baixar
                                </a>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                    <asp:Label ID="lblSemAnexos" runat="server" Text="Nenhum anexo encontrado." 
                        CssClass="no-attachments" Visible="false" />
                </div>
            </div>

            <!-- Botões de Ação -->
            <div class="button-container">
                <asp:Button ID="btnVoltar" runat="server" CssClass="btn-back"
                    Text="Voltar para Lista" OnClick="btnVoltar_Click" />
                
                <asp:Button ID="btnAceitarDemanda" runat="server" CssClass="btn-accept"
                    Text="Aceitar esta Demanda" OnClick="btnAceitarDemanda_Click" />
            </div>
        </div>
    </div>
</asp:Content>