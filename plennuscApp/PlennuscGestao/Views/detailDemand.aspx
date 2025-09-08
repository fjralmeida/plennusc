<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="detailDemand.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.detailDemand" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .demand-header { background:#fff; padding:20px; border-radius:12px; box-shadow:0 2px 6px rgba(0,0,0,0.08); margin-bottom:20px;}
        .demand-messages { max-height:450px; overflow-y:auto; border:1px solid #e0e0e0; border-radius:12px; padding:15px; margin-bottom:20px; background:#fafafa;}
        .msg { display:inline-block; max-width:70%; padding:10px 15px; margin-bottom:10px; border-radius:20px; font-size:14px; line-height:1.4; position:relative; clear:both; }
        .msg.self { background:#d1f0ff; color:#055160; float:right; border-bottom-right-radius:5px; }
        .msg.other { background:#f1f1f1; color:#333; float:left; border-bottom-left-radius:5px; }
        .msg small { display:block; font-size:12px; margin-top:4px; opacity:0.7; }
        .demand-reply { background:#fff; border-radius:12px; padding:15px; box-shadow:0 2px 6px rgba(0,0,0,0.08); }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container py-3">

       <!-- Cabeçalho da Demanda -->
<div class="card shadow-sm mb-4 demand-header">
    <div class="card-body">
        <h3 class="card-title">
            <asp:Label ID="lblTitulo" runat="server" CssClass="mb-0"></asp:Label>
        </h3>
        <p class="card-text">
            <asp:Label ID="lblTexto" runat="server"></asp:Label>
        </p>
        <asp:Label ID="lblStatus" runat="server" CssClass="badge bg-info"></asp:Label>
        <p class="text-muted mt-2">
            Solicitante: <asp:Label ID="lblSolicitante" runat="server" /> |
            Data: <asp:Label ID="lblDataSolicitacao" runat="server" />
        </p>
    </div>
</div>


        <!-- Histórico de status (DemandaHistorico) -->
        <div class="card shadow-sm mb-4">
            <div class="card-header"><strong>📜 Histórico de Status</strong></div>
            <div class="card-body" style="max-height:300px; overflow-y:auto;">
                <asp:Repeater ID="rptHistorico" runat="server">
                    <ItemTemplate>
                        <div class="mb-3">
                            <div>
                                <strong><%# Eval("Usuario") %></strong>
                                <small class="text-muted"> - <%# Eval("DataAlteracao", "{0:dd/MM/yyyy HH:mm}") %></small>
                            </div>
                            <div class="p-2 bg-light rounded">
                                <%# Eval("SituacaoAnterior") %> → <%# Eval("SituacaoAtual") %>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- Acompanhamentos (textos de resposta salvos em DemandaAcompanhamento) -->
        <div class="card shadow-sm mb-4">
            <div class="card-header"><strong>📨 Acompanhamentos</strong></div>
            <div class="card-body" style="max-height:300px; overflow-y:auto;">
                <asp:Repeater ID="rptAcompanhamentos" runat="server">
                    <ItemTemplate>
                        <div class="mb-3">
                            <div>
                                <strong><%# Eval("Autor") %></strong>
                                <small class="text-muted"> - <%# Eval("DataAcompanhamento", "{0:dd/MM/yyyy HH:mm}") %></small>
                            </div>
                            <div class="p-2 bg-light rounded">
                                <%# Eval("TextoAcompanhamento") %>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>

        <!-- Ações -->
        <div class="d-flex gap-2 mb-4">
            <asp:Button ID="btnResolver" runat="server" CssClass="btn btn-success" Text="✔ Resolver" OnClick="btnResolver_Click" />
            <asp:Button ID="btnEncerrar" runat="server" CssClass="btn btn-danger" Text="✖ Encerrar" OnClick="btnEncerrar_Click" />
            <asp:Button ID="btnReabrir" runat="server" CssClass="btn btn-warning" Text="🔄 Reabrir" OnClick="btnReabrir_Click" />
        </div>

        <!-- Resposta / Acompanhamento (painel referenciado pelo code-behind) -->
        <asp:Panel ID="pnlResposta" runat="server" CssClass="card shadow-sm">
            <div class="card-body demand-reply">
                <asp:TextBox ID="txtResposta" runat="server" CssClass="form-control mb-3" TextMode="MultiLine" Rows="4" placeholder="Digite sua resposta..."></asp:TextBox>
                <asp:Button ID="btnResponder" runat="server" CssClass="btn btn-primary" Text="Responder" OnClick="btnResponder_Click" />
            </div>
        </asp:Panel>

    </div>
</asp:Content>