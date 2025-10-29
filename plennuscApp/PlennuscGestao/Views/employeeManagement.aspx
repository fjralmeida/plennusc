<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="employeeManagement.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.employeeManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Gestão de Funcionários</title>

    <!-- jQuery e plugins -->
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.16/jquery.mask.min.js"></script>

    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/sweetalert2@11.7.12/dist/sweetalert2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11.7.12/dist/sweetalert2.all.min.js"></script>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />

    <link href="../../Content/Css/projects/gestao/structuresCss/employee-Management.css" rel="stylesheet" />

    <!-- ============================================
         INLINE: tudo que depende de ClientID do servidor
         Coloquei aqui as funções que precisam dos controles ASP.NET
         ============================================ -->
    <script type="text/javascript">
        (function () {
            // ---------- HELPERS que usam ClientIDs do servidor ----------
            var selectors = {
                hfCodPessoaInativa: '#<%= hfCodPessoaInativa.ClientID %>',
                lblNomeUsuarioInativa: '#lblNomeUsuarioInativa', // span estático no markup
                modalInativarId: 'modalInativarUsuario',

                txtDocCPF: '#<%= txtDocCPF.ClientID %>',
                txtBuscaCPF: '#<%= txtBuscaCPF.ClientID %>',
                txtTelefone1: '#<%= txtTelefone1.ClientID %>',
                txtTelefone2: '#<%= txtTelefone2.ClientID %>',
                txtTelefone3: '#<%= txtTelefone3.ClientID %>',

                panelCadastroId: '<%= PanelCadastro.ClientID %>'
            };

            // ---------- ABRE MODAL DE INATIVAÇÃO ----------
            window.abrirModalInativarBtn = function (btn) {
                try {
                    if (!btn) return;
                    var codPessoa = btn.getAttribute('data-codpessoa');
                    var nome = btn.getAttribute('data-nome');

                    // Preenche HiddenField (server control)
                    if ($(selectors.hfCodPessoaInativa).length) {
                        $(selectors.hfCodPessoaInativa).val(codPessoa);
                    }

                    // Preenche label do modal (span estático)
                    if ($(selectors.lblNomeUsuarioInativa).length) {
                        $(selectors.lblNomeUsuarioInativa).text(nome);
                    }

                    // Mostra modal Bootstrap
                    var modalEl = document.getElementById(selectors.modalInativarId);
                    if (modalEl) {
                        var m = new bootstrap.Modal(modalEl);
                        m.show();
                    }
                } catch (err) {
                    console.error('abrirModalInativarBtn error:', err);
                }
            };

            // ---------- APLICA MÁSCARAS (jQuery Mask) ----------
            window.aplicarMascaras = function () {
                try {
                    var $cpf = $(selectors.txtDocCPF || '');
                    var $cpfBusca = $(selectors.txtBuscaCPF || '');

                    if ($cpf.length) $cpf.unmask().mask('000.000.000-00', { reverse: true });
                    if ($cpfBusca.length) $cpfBusca.unmask().mask('000.000.000-00', { reverse: true });

                    function phoneBehavior(val) {
                        var nums = val.replace(/\D/g, '').slice(0, 11);
                        return nums.length === 11 ? '(00) 00000-0000' : '(00) 0000-00009';
                    }
                    var phoneOptions = {
                        onKeyPress: function (val, e, field, options) {
                            field.mask(phoneBehavior.apply({}, arguments), options);
                        }
                    };

                    var $tel1 = $(selectors.txtTelefone1 || '');
                    var $tel2 = $(selectors.txtTelefone2 || '');
                    var $tel3 = $(selectors.txtTelefone3 || '');

                    if ($tel1.length) $tel1.attr('maxlength', 15).unmask().mask(phoneBehavior, phoneOptions);
                    if ($tel2.length) $tel2.attr('maxlength', 15).unmask().mask(phoneBehavior, phoneOptions);
                    if ($tel3.length) $tel3.attr('maxlength', 15).unmask().mask(phoneBehavior, phoneOptions);
                } catch (err) {
                    console.error('aplicarMascaras error:', err);
                }
            };

            // ---------- TOAST DE ERRO PARA VALIDAÇÃO ----------
            window.showToastErroObrigatorio = function () {
                try {
                    Swal.fire({
                        toast: true,
                        position: 'top-end',
                        icon: 'error',
                        title: 'Preencha todos os campos obrigatórios.',
                        showConfirmButton: false,
                        timer: 3000,
                        timerProgressBar: true
                    });
                } catch (err) {
                    console.error('showToastErroObrigatorio error:', err);
                }
            };

            // ---------- ROLAR PARA O PRIMEIRO CAMPO INVÁLIDO DO ValidationGroup ----------
            window.rolarParaPrimeiroCampoInvalido = function (validationGroup) {
                try {
                    var validators = window.Page_Validators || [];
                    for (var i = 0; i < validators.length; i++) {
                        var v = validators[i];
                        if (v.validationGroup === validationGroup && !v.isvalid) {
                            var campo = document.getElementById(v.controltovalidate);
                            if (campo) {
                                campo.scrollIntoView({ behavior: 'smooth', block: 'center' });
                                campo.focus();
                                break;
                            }
                        }
                    }
                } catch (err) {
                    console.error('rolarParaPrimeiroCampoInvalido error:', err);
                }
            };

            // ---------- BINDs e inicializações ----------
            $(document).ready(function () {
                // aplica máscaras no load
                try { window.aplicarMascaras(); } catch (e) { console.error(e); }

                // se PanelCadastro estiver visível no carregamento, reaplica máscaras (caso o Panel seja mostrado via server)
                try {
                    var panel = document.getElementById(selectors.panelCadastroId);
                    if (panel && $(panel).is(':visible')) {
                        window.aplicarMascaras();
                    }
                } catch (e) { /*silent*/ }
            });

            // Reaplica máscaras após postback parcial (UpdatePanel) se estiver presente
            if (typeof (Sys) !== "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager) {
                try {
                    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                        try { window.aplicarMascaras(); } catch (e) { console.error(e); }
                    });
                } catch (e) { /*silent*/ }
            }

        })();
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container py-4">

        <asp:Panel ID="PanelCadastro" runat="server" CssClass="form-panel mt-4" Visible="false">
            <h4 class="titulo-cadastro">Cadastro de Novo Colaborador</h4>

            <!-- DADOS PESSOAIS -->
            <div class="section-block bg-white-section">
                <h5>Dados Pessoais</h5>
                <div class="row g-3">
                    <div class="col-md-6">
                        <label>Nome *</label>
                        <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvNome" runat="server" ControlToValidate="txtNome" ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" ValidationGroup="Cadastro" />
                    </div>
                    <div class="col-md-6">
                        <label>Sobrenome *</label>
                        <asp:TextBox ID="txtSobrenome" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvSobrenome" runat="server" ControlToValidate="txtSobrenome" ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" ValidationGroup="Cadastro" />
                    </div>
                    <div class="col-md-4">
                        <label>Apelido</label>
                        <asp:TextBox ID="txtApelido" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label>Sexo *</label>
                        <asp:DropDownList ID="ddlSexo" runat="server" CssClass="form-control">
                            <asp:ListItem Value="">Selecione</asp:ListItem>
                            <asp:ListItem Value="M">Masculino</asp:ListItem>
                            <asp:ListItem Value="F">Feminino</asp:ListItem>
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvSexo" runat="server" ControlToValidate="ddlSexo" InitialValue="" ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" ValidationGroup="Cadastro" />
                    </div>
                    <div class="col-md-4">
                        <label>Data de Nascimento *</label>
                        <asp:TextBox ID="txtDataNasc" runat="server" CssClass="form-control" TextMode="Date" />
                        <asp:RequiredFieldValidator ID="rfvDataNasc" runat="server" ControlToValidate="txtDataNasc" ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" ValidationGroup="Cadastro" />
                    </div>
                </div>
            </div>

            <!-- DOCUMENTOS -->
            <div class="section-block bg-gray-section">
                <h5>Documentos</h5>
                <div class="row g-3">
                    <div class="col-md-6">
                        <label>CPF *<asp:TextBox ID="txtDocCPF" runat="server" CssClass="form-control" MaxLength="11" placeHolder="Insira apenas números" />
                        </label>

                    </div>
                    <div class="col-md-6">
                        <label>
                            RG *
                        <asp:TextBox ID="txtDocRG" runat="server" CssClass="form-control" />
                            <asp:RequiredFieldValidator ID="rfvRG" runat="server" ControlToValidate="txtDocRG" ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" ValidationGroup="Cadastro" />
                        </label>
                    </div>
                </div>
            </div>

            <!-- DADOS ELEITORAIS -->
            <div class="section-block bg-white-section">
                <h5>Dados Eleitorais</h5>
                <div class="row g-3">
                    <div class="col-md-4">
                        <label>Título de Eleitor</label>
                        <asp:TextBox ID="txtTitulo" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label>Zona</label>
                        <asp:TextBox ID="txtZona" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label>Seção</label>
                        <asp:TextBox ID="txtSecao" runat="server" CssClass="form-control" />
                    </div>
                </div>
            </div>

            <!-- DADOS TRABALHISTAS -->
            <div class="section-block bg-gray-section">
                <h5>Dados Trabalhistas</h5>
                <div class="row g-3">
                    <div class="col-md-4">
                        <label>CTPS</label>
                        <asp:TextBox ID="txtCTPS" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label>Série</label>
                        <asp:TextBox ID="txtCTPSSerie" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label>UF</label>
                        <asp:TextBox ID="txtCTPSUf" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-6">
                        <label>PIS</label>
                        <asp:TextBox ID="txtPis" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-6">
                        <label>Matrícula</label>
                        <asp:TextBox ID="txtMatricula" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-6">
                        <label>Admissão *</label>
                        <asp:TextBox ID="txtDataAdmissao" runat="server" CssClass="form-control" TextMode="Date" />
                        <asp:RequiredFieldValidator ID="rfvAdmissao" runat="server" ControlToValidate="txtDataAdmissao" ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" ValidationGroup="Cadastro" />
                    </div>
                    <%--  <div class="col-md-6"  id="linhaDemissao" runat="server">
                        <label>Demissão</label>
                        <asp:TextBox ID="txtDataDemissao" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>--%>
                </div>
            </div>

            <!-- FILIAÇÃO -->
            <div class="section-block bg-white-section">
                <h5>Filiação</h5>
                <div class="row g-3">
                    <div class="col-md-6">
                        <label>Nome da Filiação 1</label>
                        <asp:TextBox ID="txtFiliacao1" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-6">
                        <label>Nome da Filiação 2</label>
                        <asp:TextBox ID="txtFiliacao2" runat="server" CssClass="form-control" />
                    </div>
                </div>
            </div>

            <!-- CONTATO -->
            <div class="section-block bg-gray-section">
                <h5>Contato</h5>
                <div class="row g-3">
                    <div class="col-md-4">
                        <label>Telefone 1 *</label>
                        <asp:TextBox ID="txtTelefone1" runat="server" CssClass="form-control" MaxLength="15" />
                        <asp:RequiredFieldValidator ID="rfvTel1" runat="server" ControlToValidate="txtTelefone1"
                            ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" ValidationGroup="Cadastro" />
                    </div>
                    <div class="col-md-4">
                        <label>Telefone 2</label>
                        <asp:TextBox ID="txtTelefone2" runat="server" CssClass="form-control" MaxLength="15" />
                    </div>
                    <div class="col-md-4">
                        <label>Telefone 3</label>
                        <asp:TextBox ID="txtTelefone3" runat="server" CssClass="form-control" MaxLength="15" />
                    </div>
                    <div class="col-md-6">
                        <label>Email *</label>
                        <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
                        <asp:RequiredFieldValidator ID="rfvEmail" runat="server" ControlToValidate="txtEmail" ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" ValidationGroup="Cadastro" />
                    </div>
                    <div class="col-md-6">
                        <label>Email Alternativo</label>
                        <asp:TextBox ID="txtEmailAlt" runat="server" CssClass="form-control" TextMode="Email" />
                    </div>
                </div>
            </div>

            <!-- CARGO E DEPARTAMENTO -->
            <div class="section-block bg-white-section">
                <h5>Cargo e Departamento</h5>
                <div class="row g-3">

                    <div class="col-md-12">
                        <label>Perfil Pessoa *</label>
                        <asp:DropDownList ID="ddlPerfilPessoa" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvPerfilPessoa" runat="server" ControlToValidate="ddlPerfilPessoa" InitialValue="" ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" ValidationGroup="Cadastro" />
                    </div>

                    <div class="col-md-6">
                        <label>Cargo *</label>
                        <asp:DropDownList ID="ddlCargo" runat="server" CssClass="form-control">
                            <asp:ListItem Text="Selecione" Value="" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvCargo" runat="server" ControlToValidate="ddlCargo" InitialValue="" ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" ValidationGroup="Cadastro" />
                    </div>
                    <div class="col-md-6">
                        <label>Departamento *</label>
                        <asp:DropDownList ID="ddlDepartamento" runat="server" CssClass="form-control">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvDepartamento" runat="server" ControlToValidate="ddlDepartamento" InitialValue="" ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" ValidationGroup="Cadastro" />
                    </div>
                </div>
            </div>

            <!-- CONFIGURAÇÕES -->
            <div class="section-block bg-gray-section">
                <h5>Configurações</h5>
                <div class="row px-2">
                    <div class="col-md-3">
                        <label class="d-flex align-items-center gap-2">
                            <asp:CheckBox ID="chkCriaContaAD" runat="server" />
                            Criar conta no AD
                        </label>
                    </div>
                    <div class="col-md-3">
                        <label class="d-flex align-items-center gap-2">
                            <asp:CheckBox ID="chkCadastraPonto" runat="server" />
                            Cadastrar no ponto
                        </label>
                    </div>
                    <div class="col-md-3">
                        <label class="d-flex align-items-center gap-2">
                            <asp:CheckBox ID="chkAtivo" runat="server" Checked="true" />
                            Ativo *
                        </label>
                    </div>
                    <div class="col-md-3">
                        <label class="d-flex align-items-center gap-2">
                            <asp:CheckBox ID="chkPermiteAcesso" runat="server" />
                            Permite Acesso
                        </label>
                    </div>
                </div>
            </div>

            <!-- OBSERVAÇÕES -->
            <div class="section-block bg-white-section">
                <h5>Observações</h5>
                <asp:TextBox ID="txtObservacao" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
            </div>

            <!-- SALVAR -->
            <div class="text-center mt-3">
                <asp:Button ID="btnSalvarUsuario" runat="server" Text="Salvar Usuário"
                    CssClass="btn btn-success"
                    ValidationGroup="Cadastro"
                    OnClick="btnSalvarUsuario_Click"
                    OnClientClick="if (!Page_ClientValidate('Cadastro')) { showToastErroObrigatorio(); rolarParaPrimeiroCampoInvalido('Cadastro'); return false; }" />


            </div>
        </asp:Panel>

        <asp:Panel ID="PanelConsulta" runat="server" CssClass="form-panel mt-4" Visible="false">
            <!-- Título principal -->
            <h4 class="titulo-cadastro">Consultar Usuário</h4>

            <div class="filters-block">
                <h5 class="filters-title">Filtros</h5>

                <div class="row g-3 align-items-end">
                    <div class="col-lg-4 col-md-6">
                        <label class="form-label">Nome</label>
                        <asp:TextBox ID="txtBuscaNome" runat="server" CssClass="form-control" placeholder="Digite o nome" />
                    </div>

                    <div class="col-lg-4 col-md-6">
                        <label class="form-label">CPF</label>
                        <asp:TextBox ID="txtBuscaCPF" runat="server" CssClass="form-control" placeholder="Somente números" MaxLength="11" />
                    </div>

                    <div class="col-lg-4">
                        <label class="form-label">Departamento</label>
                        <asp:TextBox ID="TxtBuscaDepartamento" runat="server" CssClass="form-control" placeholder="Informe o departamento" />
                    </div>

                    <!-- barra de botões dentro de coluna -->
                    <div class="col-12">
                        <div class="filters-btnbar">
                            <asp:Button ID="btnBuscarPorNome" runat="server"
                                Text="Buscar Nome" CssClass="btn btn-filter btn-filter-primary"
                                OnClick="btnBuscarPorNome_Click" />
                            <asp:Button ID="btnBuscarPorCPF" runat="server"
                                Text="Buscar CPF" CssClass="btn btn-filter"
                                OnClick="btnBuscarPorCPF_Click" />
                            <asp:Button ID="btnBuscarDepartamento" runat="server"
                                Text="Buscar Depto." CssClass="btn btn-filter"
                                OnClick="btnBuscarDepartamento_Click" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- 📋 RESULTADOS -->
            <asp:Panel ID="PanelResultado" runat="server" CssClass="section-block" Visible="false">
                <h5>Resultados</h5>
                <div class="table-responsive">
                    <asp:GridView
                        ID="gvUsuarios"
                        runat="server"
                        CssClass="table table-modern"
                        AutoGenerateColumns="False"
                        GridLines="None"
                        ShowHeaderWhenEmpty="False"
                        EmptyDataText="Nenhum usuário encontrado."
                        DataKeyNames="CodPessoa,CodDepartamento,CodCargo"
                        OnRowDataBound="gvUsuarios_RowDataBound">

                        <HeaderStyle CssClass="table-custom-header" />
                        <Columns>
                            <asp:BoundField DataField="CodPessoa" HeaderText="CodPessoa" />
                            <asp:BoundField DataField="NomeCompleto" HeaderText="Nome">
                                <ItemStyle CssClass="col-nome-nowrap" />
                            </asp:BoundField>
                            <asp:BoundField DataField="CPF" HeaderText="CPF" />
                            <asp:BoundField DataField="Email" HeaderText="Email" />
                            <asp:BoundField DataField="Telefone1" HeaderText="Telefone" />

                            <asp:BoundField DataField="NomeDepartamento" HeaderText="Departamento" />
                            <asp:BoundField DataField="NomeCargo" HeaderText="Cargo" />

                            <asp:BoundField DataField="Conf_Ativo" HeaderText="Ativo">
                                <ItemStyle Width="80px" />
                            </asp:BoundField>

                            <asp:TemplateField HeaderText="Editar">
                                <ItemStyle Width="90px" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:PlaceHolder ID="phEditar" runat="server">
                                        <a class="btn-editar" href='<%# "employeeEdit.aspx?id=" + Eval("CodPessoa") %>'>
                                            <i class="fa-solid fa-pen-to-square"></i>
                                        </a>
                                    </asp:PlaceHolder>
                                </ItemTemplate>
                            </asp:TemplateField>

                            <asp:TemplateField HeaderText="Inativar">
                                <ItemStyle Width="110px" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:PlaceHolder ID="phInativar" runat="server">
                                        <button type="button" class="btn-inativar"
                                            data-codpessoa='<%# Eval("CodPessoa") %>'
                                            data-nome='<%# System.Web.HttpUtility.HtmlEncode(Eval("NomeCompleto")) %>'
                                            onclick="abrirModalInativarBtn(this)">
                                            <i class="fa-solid fa-user-slash"></i>
                                        </button>
                                    </asp:PlaceHolder>
                                </ItemTemplate>
                            </asp:TemplateField>

                        </Columns>
                    </asp:GridView>
                </div>

                <!-- Modal de Inativação -->
                <div class="modal fade" id="modalInativarUsuario" tabindex="-1" aria-labelledby="modalInativarUsuarioLabel" aria-hidden="true">
                    <div class="modal-dialog modal-md modal-dialog-centered">
                        <div class="modal-content rounded-4 shadow">
                            <div class="modal-header bg-danger text-white">
                                <h5 class="modal-title" id="modalInativarUsuarioLabel">
                                    <i class="fa-solid fa-user-slash me-2"></i>Inativar Usuário
                                </h5>
                                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Fechar"></button>
                            </div>

                            <div class="modal-body">
                                <asp:HiddenField ID="hfCodPessoaInativa" runat="server" />
                                <p><strong>Tem certeza que deseja inativar o usuário:</strong> <span id="lblNomeUsuarioInativa" style="color: #d9534f;"></span>?</p>
                                <label class="form-label mt-3">Motivo da Inativação</label>
                                <asp:TextBox ID="txtMotivoInativacao" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3" />
                            </div>

                            <div class="modal-footer bg-light rounded-bottom-4">
                                <asp:Button ID="btnConfirmarInativar" runat="server"
                                    CssClass="btn btn-danger btn-pill"
                                    Text="Confirmar Inativação"
                                    OnClick="btnConfirmarInativar_Click" />
                                <button type="button" class="btn btn-secondary btn-pill" data-bs-dismiss="modal">
                                    <i class="fa-solid fa-xmark me-1"></i>Cancelar
                                </button>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </asp:Panel>
    </div>
</asp:Content>
