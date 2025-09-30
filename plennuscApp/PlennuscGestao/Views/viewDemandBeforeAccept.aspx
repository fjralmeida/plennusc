<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="viewDemandBeforeAccept.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.viewDemandBeforeAccept" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
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

        .container-main {
            max-width: 2206px;
            margin: 20px auto;
            padding: 0 16px;
        }

        .demand-header {
            background: white;
            border-radius: var(--border-radius);
            box-shadow: var(--shadow);
            padding: 32px;
            margin-bottom: 24px;
            border: 1px solid var(--gray-200);
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
        }

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

        .demand-details-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 20px;
            margin-bottom: 30px;
        }

        .detail-group {
            background: var(--gray-50);
            padding: 15px;
            border-radius: 6px;
            border-left: 4px solid var(--primary);
        }

        .detail-label {
            font-size: 12px;
            color: var(--gray-600);
            font-weight: 600;
            text-transform: uppercase;
            margin-bottom: 5px;
            display: block;
        }

        .detail-value {
            font-size: 16px;
            color: var(--gray-800);
            font-weight: 500;
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
        }

        .description-label {
            font-size: 14px;
            color: var(--gray-600);
            font-weight: 600;
            margin-bottom: 10px;
        }

        .description-text {
            font-size: 15px;
            color: var(--gray-800);
            line-height: 1.6;
            word-wrap: break-word;
            white-space: normal;
        }

        .attachments-section {
            background: white;
            border-radius: var(--border-radius);
            box-shadow: var(--shadow);
            padding: 20px;
            margin-bottom: 24px;
            border: 1px solid var(--gray-200);
        }

        .section-header {
            font-size: 18px;
            font-weight: 600;
            color: var(--gray-800);
            margin-bottom: 15px;
            padding-bottom: 10px;
            border-bottom: 1px solid var(--gray-300);
            display: flex;
            align-items: center;
            gap: 8px;
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
            flex: 1;
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
            color: var(--gray-700);
        }

        .attachment-name {
            font-size: 14px;
            color: var(--gray-800);
            font-weight: 500;
        }

        .attachment-meta {
            font-size: 12px;
            color: var(--gray-600);
            display: flex;
            gap: 5px;
            align-items: center;
        }

        .btn-download {
            background: var(--primary);
            border: none;
            color: white;
            font-weight: 500;
            padding: 8px 16px;
            border-radius: 4px;
            font-size: 14px;
            text-decoration: none;
            cursor: pointer;
            transition: var(--transition);
            display: inline-flex;
            align-items: center;
            gap: 5px;
        }

        .btn-download:hover {
            background: var(--primary-hover);
            text-decoration: none;
            color: white;
        }

        .button-container {
            display: flex;
            justify-content: flex-end;
            gap: 12px;
            margin-top: 30px;
            padding-top: 20px;
            border-top: 1px solid var(--gray-200);
        }

        .btn-accept {
            background: var(--gray-100);
            border: 1px solid var(--gray-300);
            color: var(--gray-700);
            font-weight: 500;
            padding: 10px 20px;
            border-radius: 6px;
            font-size: 14px;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            transition: var(--transition);
            height: 40px;
        }

        .btn-accept:hover {
            background: var(--success);
            border-color: var(--success);
            color: white;
            transform: translateY(-1px);
            box-shadow: 0 2px 6px rgba(76, 176, 122, 0.2);
        }

        .btn-back {
            background: var(--gray-100);
            border: 1px solid var(--gray-300);
            color: var(--gray-700);
            font-weight: 500;
            padding: 10px 20px;
            border-radius: 6px;
            font-size: 14px;
            cursor: pointer;
            display: inline-flex;
            align-items: center;
            gap: 8px;
            transition: var(--transition);
            height: 40px;
        }

        .btn-back:hover {
            background: var(--primary);
            border-color: var(--primary);
            color: white;
            transform: translateY(-1px);
            box-shadow: 0 2px 6px rgba(110, 191, 225, 0.2);
        }

        .no-attachments {
            text-align: center;
            color: var(--gray-600);
            font-style: italic;
            padding: 20px;
            display: block;
        }

        @media (max-width: 768px) {
            .demand-details-grid {
                grid-template-columns: 1fr;
            }
            
            .button-container {
                flex-direction: column;
            }
            
            .btn-accept, .btn-back {
                width: 100%;
                justify-content: center;
            }
            
            .demand-title {
                flex-direction: column;
                align-items: flex-start;
                gap: 10px;
            }
            
            .attachment-item {
                flex-direction: column;
                align-items: flex-start;
                gap: 10px;
            }
            
            .btn-download {
                align-self: flex-end;
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
                <span class="description-label">Descrição</span>
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
                                    <i class="bi bi-file-earmark"></i>
                                    <div>
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
                    Text="Aceitar esta Demanda" OnClick="btnAceitarDemanda_Click1" />
            </div>
        </div>
    </div>
</asp:Content>