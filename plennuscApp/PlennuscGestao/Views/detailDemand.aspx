<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="detailDemand.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.detailDemand" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet">

    <link href="../../Content/Css/projects/gestao/structuresCss/detailDemand.css" rel="stylesheet" />

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
</asp:Content>
