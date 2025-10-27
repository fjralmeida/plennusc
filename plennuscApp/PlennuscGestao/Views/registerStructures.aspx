﻿<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="registerStructures.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.registerStructures" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>

    <link href="../../Content/Css/projects/gestao/structuresCss/register-Structures.css" rel="stylesheet" />
    <script src="../../Content/Css/js/projects/gestaoJs/structuresJs/registerStructures.js"></script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <!-- Header da Página -->
        <div class="page-header">
            <div class="page-title">
                <div class="title-icon">
                   <i class="bi bi-building-add me-2"></i>
                </div>
                Cadastro de Estrutura
            </div>
        </div>

        <!-- Card Principal -->
        <div class="main-card">
            <div class="card-header">
                <h2 class="card-title">
                    <i class="bi bi-pencil-square"></i>
                    Dados da Estrutura
                </h2>
            </div>
            
            <div class="card-body">

                <!-- COMBO COM AS VIEWS -->
                <div class="form-group">
                    <label class="form-label">View *</label>
                    <asp:DropDownList ID="ddlView" runat="server" CssClass="form-control form-select" AutoPostBack="true"
                        OnSelectedIndexChanged="ddlView_SelectedIndexChanged">
                        <asp:ListItem Text="Selecione uma View" Value=""></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <!-- SUBTIPOS DINÂMICOS - SEMPRE VISÍVEL -->
                <div class="form-group mt-3">
                    <label class="form-label">Adicionar Novas Estruturas</label>
                    <div class="alert alert-warning">
                        <small><i class="bi bi-lightbulb"></i> <strong>Dica:</strong> Defina a "Ordem" para organizar a exibição.</small>
                    </div>
                    <div class="subtipos-container">
                        <div id="containerSubtipos">
                            <!-- Primeiro campo com todos os dados -->
                            <div class="subtipo-item">
                                <div class="subtipo-input">
                                    <input type="text" class="form-control" name="subtipo_1" 
                                        placeholder="Digite o nome da estrutura" maxlength="100" />
                                </div>
                                <div class="subtipo-ordem">
                                    <input type="number" class="form-control campo-ordem" name="ordem_1" 
                                        placeholder="Ordem" value="1" min="1" 
                                        onchange="reordenarAutomaticamente(this)" />
                                </div>
                                <div class="subtipo-actions">
                                    <button type="button" class="btn-add-small" onclick="adicionarSubtipo()" title="Adicionar outra estrutura">
                                        <i class="bi bi-plus-lg"></i>
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- BOTÕES DE AÇÃO -->
                <div class="action-buttons">
                    <asp:Button ID="btnSalvarTudo" runat="server" Text="Salvar Tudo" 
                        CssClass="btn-save" OnClick="btnSalvarTudo_Click" />
                </div>

                <!-- MENSAGENS -->
                <asp:Panel ID="pnlMensagem" runat="server" Visible="false">
                    <div class="message" id="divMensagem" runat="server">
                        <asp:Label ID="lblMensagem" runat="server"></asp:Label>
                    </div>
                </asp:Panel>

                <!-- MENSAGEM SE JÁ EXISTIR ESTRUTURAS -->
                <asp:Panel ID="pnlMensagemEstruturaExistente" runat="server" Visible="false">
                    <div class="alert alert-info">
                        <strong><i class="bi bi-info-circle"></i> Estruturas existentes</strong>
                        <br />Esta View já possui estruturas cadastradas. Você pode visualizá-las abaixo e adicionar novas.
                    </div>
                </asp:Panel>

                <!-- GRID COM ESTRUTURAS EXISTENTES -->
                <asp:Panel ID="pnlGridEstruturas" runat="server" Visible="false" class="mt-3">
                    <div class="grid-card">
                        <div class="grid-card-header">
                            <h5 class="grid-card-title"><i class="bi bi-list-ul"></i> Estruturas Existentes</h5>
                        </div>
                        <div class="grid-card-body">
                            <asp:GridView ID="gvEstruturas" runat="server" CssClass="table table-striped table-bordered" 
                                AutoGenerateColumns="false" EmptyDataText="Nenhuma estrutura encontrada"
                                OnRowCommand="gvEstruturas_RowCommand" OnRowDataBound="gvEstruturas_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="DescEstrutura" HeaderText="Nome da Estrutura" 
                                        ItemStyle-CssClass="column-text" HeaderStyle-CssClass="column-text" />
                                    <asp:BoundField DataField="ValorPadrao" HeaderText="Ordem" 
                                        ItemStyle-CssClass="column-number" HeaderStyle-CssClass="column-number" />
                                    <asp:TemplateField HeaderText="Ações" 
                                        ItemStyle-CssClass="column-actions" HeaderStyle-CssClass="column-actions">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="btnExcluir" runat="server" CssClass="btn btn-danger btn-sm"
                                                CommandName="Excluir" CommandArgument='<%# Eval("CodEstrutura") %>'
                                                OnClientClick='<%# "return confirm(\"Tem certeza que deseja excluir a estrutura \\\"" + Eval("DescEstrutura") + "\\\"?\");" %>'
                                                ToolTip="Excluir estrutura">
                                                <i class="bi bi-trash"></i> Excluir
                                            </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </asp:Panel>

                <!-- CAMPO HIDDEN PARA ARMAZENAR OS SUBTIPOS -->
                <asp:HiddenField ID="hdnSubtipos" runat="server" />
            </div>
        </div>
    </div>
</asp:Content>