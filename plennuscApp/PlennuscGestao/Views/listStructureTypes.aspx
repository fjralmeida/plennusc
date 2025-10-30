<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="listStructureTypes.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.listStructureTypes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="../../Content/Css/projects/gestao/structuresCss/list-structure-types.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <!-- Header da Página -->
        <div class="page-header">
            <div class="page-title">
                <div class="title-icon">
                    <i class="bi bi-list-ul me-2"></i>
                </div>
                Lista de Tipos de Estrutura
            </div>
            <div class="page-actions">
                <asp:Button ID="btnNovo" runat="server" Text="Novo Tipo Estrutura" CssClass="btn-new" OnClick="btnNovo_Click" />
            </div>
        </div>

        <!-- Card Principal -->
        <div class="main-card">
            <div class="card-header">
                <h2 class="card-title">
                    <i class="bi bi-table"></i>
                    Tipos de Estrutura Cadastrados
                </h2>
            </div>

            <div class="card-body">
                <!-- Filtros -->
                <div class="filters-section">
                    <div class="filter-group">
                        <label>Filtrar por:</label>
                        <asp:TextBox ID="txtFiltro" runat="server" CssClass="form-control filter-input" 
                            placeholder="Digite para filtrar..." AutoPostBack="true" OnTextChanged="txtFiltro_TextChanged"></asp:TextBox>
                    </div>
                </div>

                <!-- GridView -->
                <div class="table-container">
                    <asp:GridView ID="gvTiposEstrutura" runat="server" AutoGenerateColumns="False" CssClass="structure-grid"
                        AllowPaging="True" PageSize="15" OnPageIndexChanging="gvTiposEstrutura_PageIndexChanging"
                        EmptyDataText="Nenhum tipo de estrutura encontrado." DataKeyNames="CodTipoEstrutura">
                        <Columns>

                            <asp:BoundField DataField="CodTipoEstrutura" HeaderText="Código" SortExpression="CodTipoEstrutura">
                                <HeaderStyle Width="80px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:BoundField>

                            <asp:BoundField DataField="DescTipoEstrutura" HeaderText="Tipo Estrutura" SortExpression="DescTipoEstrutura">
                                <HeaderStyle Width="200px" />
                            </asp:BoundField>

                            <asp:BoundField DataField="NomeView" HeaderText="View" SortExpression="NomeView">
                                <HeaderStyle Width="200px" />
                            </asp:BoundField>

                            <asp:TemplateField HeaderText="Editável" SortExpression="Editavel">
                                <HeaderStyle Width="100px" />
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <span class='badge <%# (bool)Eval("Editavel") ? "badge-success" : "badge-secondary" %>'>
                                        <%# (bool)Eval("Editavel") ? "Sim" : "Não" %>
                                    </span>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:BoundField DataField="Definicao" HeaderText="Definição" SortExpression="Definicao">
                                <HeaderStyle Width="250px" />
                            </asp:BoundField>

                            <asp:BoundField DataField="Utilizacao" HeaderText="Utilização" SortExpression="Utilizacao">
                                <HeaderStyle Width="250px" />
                            </asp:BoundField>

                            <asp:BoundField DataField="Informacoes_Log_I" HeaderText="Criado em" SortExpression="Informacoes_Log_I" 
                                DataFormatString="{0:dd/MM/yyyy HH:mm}">
                                <HeaderStyle Width="150px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:BoundField>

                            <asp:BoundField DataField="Informacoes_Log_A" HeaderText="Alterado em" SortExpression="Informacoes_Log_A" 
                                DataFormatString="{0:dd/MM/yyyy HH:mm}" NullDisplayText="--">
                                <HeaderStyle Width="150px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </asp:BoundField>

                            <asp:TemplateField HeaderText="Ações">
                                <HeaderStyle Width="120px" />
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <div class="action-buttons">
                                        <asp:LinkButton ID="btnEditar" runat="server" CssClass="btn-action btn-edit" 
                                            CommandArgument='<%# Eval("CodTipoEstrutura") %>' OnClick="btnEditar_Click"
                                            ToolTip="Editar Tipo Estrutura">
                                            <i class="bi bi-pencil"></i>
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnExcluir" runat="server" CssClass="btn-action btn-delete" 
                                            CommandArgument='<%# Eval("CodTipoEstrutura") %>' OnClick="btnExcluir_Click"
                                            ToolTip="Excluir Tipo Estrutura">
                                            <i class="bi bi-trash"></i>
                                        </asp:LinkButton>
                                    </div>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <PagerStyle CssClass="grid-pager" />
                        <HeaderStyle CssClass="grid-header" />
                        <RowStyle CssClass="grid-row" />
                        <AlternatingRowStyle CssClass="grid-alternating-row" />
                    </asp:GridView>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal de Confirmação de Exclusão -->
    <div class="modal-overlay" id="modalExclusao" style="display: none;">
        <div class="modal-container">
            <div class="modal-header">
                <h3 class="modal-title">
                    <i class="bi bi-exclamation-triangle text-warning"></i>
                    Confirmar Exclusão
                </h3>
                <button type="button" class="modal-close" onclick="fecharModal()">
                    <i class="bi bi-x"></i>
                </button>
            </div>
            <div class="modal-body">
                <p>Tem certeza que deseja excluir o tipo de estrutura?</p>
                <div class="exclusao-info">
                    <strong>Código:</strong> <span id="modalCodigo"></span><br>
                    <strong>Tipo Estrutura:</strong> <span id="modalDescricao"></span><br>
                    <strong>View:</strong> <span id="modalView"></span>
                </div>
                <div class="modal-alert">
                    <i class="bi bi-info-circle"></i>
                    <strong>Atenção:</strong> Esta ação não pode ser desfeita!
                </div>
            </div>
            <div class="modal-footer">
                <asp:Button ID="btnCancelarExclusao" runat="server" Text="Cancelar" CssClass="btn btn-secondary" OnClientClick="fecharModal(); return false;" />
                <asp:Button ID="btnConfirmarExclusao" runat="server" Text="Confirmar Exclusão" CssClass="btn btn-danger" OnClick="btnConfirmarExclusao_Click" />
            </div>
        </div>
    </div>

    <!-- Hidden fields para armazenar dados do item a ser excluído -->
    <asp:HiddenField ID="hdnCodigoExclusao" runat="server" />
    <asp:HiddenField ID="hdnDescricaoExclusao" runat="server" />
    <asp:HiddenField ID="hdnViewExclusao" runat="server" />

    <script type="text/javascript">
        // Função para abrir o modal de exclusão
        function abrirModalExclusao(codigo, descricao, view) {
            document.getElementById('modalCodigo').innerText = codigo;
            document.getElementById('modalDescricao').innerText = descricao;
            document.getElementById('modalView').innerText = view;
            
            // Preenche os hidden fields
            document.getElementById('<%= hdnCodigoExclusao.ClientID %>').value = codigo;
            document.getElementById('<%= hdnDescricaoExclusao.ClientID %>').value = descricao;
            document.getElementById('<%= hdnViewExclusao.ClientID %>').value = view;

            // Exibe o modal
            document.getElementById('modalExclusao').style.display = 'flex';
        }

        // Função para fechar o modal
        function fecharModal() {
            document.getElementById('modalExclusao').style.display = 'none';
        }

        // Fecha o modal ao clicar fora dele
        document.getElementById('modalExclusao').addEventListener('click', function (e) {
            if (e.target === this) {
                fecharModal();
            }
        });
    </script>
</asp:Content>