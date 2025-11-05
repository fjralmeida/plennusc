<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="viewDemandBeforeAccept.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.viewDemandBeforeAccept" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    
    <title>Visualizar demandas</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">

    <link href="../../Content/Css/projects/gestao/structuresCss/view-Demand-Before-Accept.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <div class="demand-header">
            <h1 class="demand-title">
                <asp:Label ID="lblTitulo" runat="server"></asp:Label>
                 <asp:Label ID="lblStatusBadge" runat="server" CssClass="status-badge" />
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
          <div class="attachments-section" id="attachmentsSection" runat="server">
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