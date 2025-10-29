<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="registerStructures.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.registerStructures" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
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
                    <label class="form-label">View *</label>
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
                    <asp:Button ID="btnSalvarTudo" runat="server" Text="Salvar Tudo"
                        CssClass="btn-save" OnClick="btnSalvarTudo_Click" />
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
                                            <asp:LinkButton ID="btnExcluir" runat="server"
                                                CssClass="btn btn-danger btn-sm"
                                                CommandName="Excluir"
                                                CommandArgument='<%# Eval("CodEstrutura") %>'
                                                OnClientClick='<%# "return confirm(\"Excluir a estrutura \\\"" + Eval("DescEstrutura") + "\\\"?\");" %>'>
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
            </div>
        </div>
    </div>
</asp:Content>