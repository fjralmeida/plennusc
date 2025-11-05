<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="registerStructures.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.registerStructures" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

          <title>Cadastro de estrutura</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet">
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <link href="../../Content/Css/projects/gestao/structuresCss/register-Structures.css" rel="stylesheet" />

    <script type="text/javascript">
        let subtipoCount = 1;

        function adicionarSubtipo() {
            subtipoCount++;
            const container = document.getElementById('containerSubtipos');
            const proximaOrdem = obterProximaOrdem();

            const subtipoItem = document.createElement('div');
            subtipoItem.className = 'subtipo-item';

            subtipoItem.innerHTML = `
                <div class="subtipo-input">
                    <input type="text" class="form-control" name="subtipo_${subtipoCount}"
                        placeholder="Digite o nome da estrutura" maxlength="100" />
                </div>
                <div class="subtipo-ordem">
                    <input type="number" class="form-control campo-ordem" name="ordem_${subtipoCount}"
                        placeholder="Ordem" value="${proximaOrdem}" min="1"
                        onchange="reordenarAutomaticamente(this)" />
                </div>
                <div class="subtipo-actions">
                    <button type="button" class="btn-add-small" onclick="adicionarSubtipo()" title="Adicionar outra estrutura">
                        <i class="bi bi-plus-lg"></i>
                    </button>
                    <button type="button" class="btn-remove" onclick="removerSubtipo(this)" title="Remover estrutura">
                        <i class="bi bi-dash-lg"></i>
                    </button>
                </div>`;
            container.appendChild(subtipoItem);
        }

        function obterProximaOrdem() {
            const camposOrdem = document.querySelectorAll('.campo-ordem');
            let maior = 0;
            camposOrdem.forEach(c => { const n = parseInt(c.value) || 0; if (n > maior) maior = n; });
            return maior + 1;
        }

        function reordenarAutomaticamente(campo) {
            const nova = parseInt(campo.value);
            const antiga = parseInt(campo.getAttribute('data-ordem-anterior')) || nova;
            if (isNaN(nova) || nova < 1) { campo.value = antiga; return; }

            const todos = document.querySelectorAll('.campo-ordem');
            let trocar = null;
            todos.forEach(c => { if (c !== campo && parseInt(c.value) === nova) trocar = c; });
            if (trocar) {
                trocar.value = antiga;
                trocar.setAttribute('data-ordem-anterior', antiga);
            }
            campo.setAttribute('data-ordem-anterior', nova);
        }

        function removerSubtipo(btn) {
            const item = btn.closest('.subtipo-item');
            const ordem = parseInt(item.querySelector('.campo-ordem').value);
            item.remove();
            reorganizarOrdens(ordem);
        }

        function reorganizarOrdens() {
            const campos = document.querySelectorAll('.campo-ordem');
            campos.forEach((campo, i) => {
                campo.value = i + 1;
                campo.setAttribute('data-ordem-anterior', i + 1);
            });
        }

        function limparCamposSubtipos() {
            document.getElementById('containerSubtipos').innerHTML = '';
            subtipoCount = 0;
            adicionarSubtipo();
        }

        function prepararDadosParaSalvar() {
            const subtipos = [];
            document.querySelectorAll('.subtipo-item').forEach((item, index) => {
                const nome = item.querySelector('input[type="text"]').value.trim();
                const ordem = item.querySelector('.campo-ordem').value;
                if (nome !== '') subtipos.push({ nome, ordem: parseInt(ordem) || (index + 1) });
            });

            const hdn = document.getElementById('<%= hdnSubtipos.ClientID %>');
            hdn.value = JSON.stringify(subtipos);

            if (subtipos.length === 0) {
                Swal.fire({ icon: 'warning', title: 'Atenção', text: 'Adicione pelo menos uma estrutura antes de salvar.' });
                return false;
            }
            return true;
        }

        // Função para abrir o modal de edição
        function abrirModalEdicao(codEstrutura, descEstrutura, valorPadrao, memoEstrutura, infoEstrutura, isDefault) {
            // Preenche os campos do modal
            document.getElementById('<%= txtDescEstruturaEditar.ClientID %>').value = descEstrutura;
            document.getElementById('<%= txtValorPadraoEditar.ClientID %>').value = valorPadrao;
            document.getElementById('<%= txtMemoEstruturaEditar.ClientID %>').value = memoEstrutura || '';
            document.getElementById('<%= txtInfoEstruturaEditar.ClientID %>').value = infoEstrutura || '';
            
            // Guarda o código da estrutura
            document.getElementById('<%= hdnCodEstruturaEditar.ClientID %>').value = codEstrutura;
            
            // Abre o modal
            var modal = new bootstrap.Modal(document.getElementById('modalEditarEstrutura'));
            modal.show();
        }

        document.addEventListener('DOMContentLoaded', function () {
            if (document.getElementById('containerSubtipos').children.length === 0)
                adicionarSubtipo();

            document.querySelectorAll('.campo-ordem').forEach(c => {
                c.setAttribute('data-ordem-anterior', c.value);
            });

            document.getElementById('<%= btnSalvarTudo.ClientID %>').addEventListener('click', prepararDadosParaSalvar);
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <div class="page-header">
            <div class="page-title">
                <div class="title-icon"><i class="bi bi-building-add me-2"></i></div>
                Cadastro de Estrutura
            </div>
        </div>

        <div class="main-card">
            <div class="card-header">
                <h2 class="card-title"><i class="bi bi-pencil-square"></i> Dados da Estrutura</h2>
            </div>

            <div class="card-body">
                <div class="form-group">
                    <label class="form-label">Tipo Estrtura</label>
                    <asp:DropDownList ID="ddlView" runat="server" CssClass="form-control form-select"
                        AutoPostBack="true" OnSelectedIndexChanged="ddlView_SelectedIndexChanged">
                        <asp:ListItem Text="Selecione uma View" Value=""></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="form-group mt-3">
                    <label class="form-label">Adicionar Novas Estruturas</label>
                    <div class="alert alert-warning">
                        <small><i class="bi bi-lightbulb"></i> <strong>Dica:</strong> Defina a "Ordem" para organizar a exibição.</small>
                    </div>
                    <div class="subtipos-container">
                        <div id="containerSubtipos"></div>
                    </div>
                </div>

              <div class="action-buttons">
                    <asp:Button ID="btnSalvarTudo" runat="server" Text="Salvar"
                        CssClass="btn-save" OnClick="btnSalvarTudo_Click" 
                        OnClientClick="return prepararDadosParaSalvar();" />
                </div>

                <asp:Panel ID="pnlMensagem" runat="server" Visible="false">
                    <div class="message" id="divMensagem" runat="server">
                        <asp:Label ID="lblMensagem" runat="server"></asp:Label>
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlMensagemEstruturaExistente" runat="server" Visible="false">
                    <div class="alert alert-info">
                        <strong><i class="bi bi-info-circle"></i> Estruturas existentes</strong>
                        <br />Esta View já possui estruturas cadastradas. Você pode visualizá-las abaixo e adicionar novas.
                    </div>
                </asp:Panel>

                <asp:Panel ID="pnlGridEstruturas" runat="server" Visible="false" class="mt-3">
                    <div class="grid-card">
                        <div class="grid-card-header">
                            <h5 class="grid-card-title"><i class="bi bi-list-ul"></i> Estruturas Existentes</h5>
                        </div>
                        <div class="grid-card-body">
                            <asp:GridView ID="gvEstruturas" runat="server"
                                CssClass="table table-striped table-bordered"
                                AutoGenerateColumns="false" EmptyDataText="Nenhuma estrutura encontrada"
                                OnRowCommand="gvEstruturas_RowCommand" OnRowDataBound="gvEstruturas_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="DescEstrutura" HeaderText="Nome da Estrutura" />
                                    <asp:BoundField DataField="ValorPadrao" HeaderText="Ordem" />
                                    <asp:TemplateField HeaderText="Ações">
                                        <ItemTemplate>
                                            <button type="button" class="btn btn-primary btn-sm me-1"
                                                onclick="abrirModalEdicao(
                                                    '<%# Eval("CodEstrutura") %>',
                                                    '<%# Eval("DescEstrutura").ToString().Replace("'", "\\'") %>',
                                                    '<%# Eval("ValorPadrao") %>',
                                                    '<%# (Eval("MemoEstrutura") ?? "").ToString().Replace("'", "\\'") %>',
                                                    '<%# (Eval("InfoEstrutura") ?? "").ToString().Replace("'", "\\'") %>',
                                                    '<%# Eval("Conf_IsDefault") %>'
                                                )"
                                                title="Editar estrutura">
                                                <i class="bi bi-pencil"></i> Editar
                                            </button>
                                            <asp:LinkButton ID="btnExcluir" runat="server"
                                                CssClass="btn btn-danger btn-sm"
                                                CommandName="Excluir"
                                                CommandArgument='<%# Eval("CodEstrutura") %>'
                                                OnClientClick='<%# "return confirm(\"Excluir a estrutura \\\"" + Eval("DescEstrutura") + "\\\"?\");" %>'
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

                <asp:HiddenField ID="hdnSubtipos" runat="server" />
                <asp:HiddenField ID="hdnCodEstruturaEditar" runat="server" />
            </div>
        </div>

        <!-- Modal de Edição VERDADEIRO -->
        <div class="modal fade" id="modalEditarEstrutura" tabindex="-1" aria-labelledby="modalEditarEstruturaLabel" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content">
                    <div class="modal-header bg-primary text-white">
                        <h5 class="modal-title" id="modalEditarEstruturaLabel">
                            <i class="bi bi-pencil-square me-2"></i>Editar Estrutura
                        </h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <div class="mb-3">
                            <label for="txtDescEstruturaEditar" class="form-label">Nome da Estrutura *</label>
                            <asp:TextBox ID="txtDescEstruturaEditar" runat="server" CssClass="form-control" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvDescEstruturaEditar" runat="server" 
                                ControlToValidate="txtDescEstruturaEditar" ErrorMessage="Nome da estrutura é obrigatório"
                                CssClass="text-danger small" Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                        <div class="mb-3">
                            <label for="txtValorPadraoEditar" class="form-label">Ordem *</label>
                            <asp:TextBox ID="txtValorPadraoEditar" runat="server" CssClass="form-control" 
                                TextMode="Number" min="0" max="999"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rfvValorPadraoEditar" runat="server" 
                                ControlToValidate="txtValorPadraoEditar" ErrorMessage="Ordem é obrigatória"
                                CssClass="text-danger small" Display="Dynamic"></asp:RequiredFieldValidator>
                        </div>
                        <div class="mb-3">
                            <label for="txtMemoEstruturaEditar" class="form-label">Descrição</label>
                            <asp:TextBox ID="txtMemoEstruturaEditar" runat="server" CssClass="form-control" 
                                TextMode="MultiLine" Rows="3" MaxLength="500" placeholder="Descrição opcional da estrutura"></asp:TextBox>
                        </div>
                        <div class="mb-3">
                            <label for="txtInfoEstruturaEditar" class="form-label">Informações Adicionais</label>
                            <asp:TextBox ID="txtInfoEstruturaEditar" runat="server" CssClass="form-control" 
                                TextMode="MultiLine" Rows="2" MaxLength="200" placeholder="Informações adicionais opcionais"></asp:TextBox>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                        <asp:Button ID="btnSalvarEdicao" runat="server" Text="Salvar Alterações" 
                            CssClass="btn btn-primary" OnClick="btnSalvarEdicao_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>