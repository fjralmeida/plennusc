<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="employeeManagement.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.employeeManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/sweetalert2@11.7.12/dist/sweetalert2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11.7.12/dist/sweetalert2.all.min.js"></script>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet">

    <style>
        * {
            font-family: 'Poppins', sans-serif;
            box-sizing: border-box;
        }

        body, html {
            background-color: #f2f4f8;
            color: #333;
            font-size: 14px;
        }

        .titulo-gestao {
            font-size: 26px;
            font-weight: 600;
            margin-bottom: 30px;
            color: #413a3a;
            text-align: center;
        }

        .container-gestao {
            display: flex;
            gap: 12px;
            justify-content: center;
            flex-wrap: wrap;
            margin-bottom: 30px;
        }

        .btn-gestao {
            padding: 6px 14px;
            border: none;
            border-radius: 10px;
            font-size: 15px;
            font-weight: 500;
            color: white;
            cursor: pointer;
            transition: all 0.2s ease-in-out;
        }

        .btn-incluir {
            background-color: #4CB07A;
        }

        .btn-consultar {
            background-color: #83ceee;
        }

        .btn-desativar {
            background-color: #DC8689;
        }

        .btn-gestao:hover {
            transform: translateY(-1px);
            filter: brightness(0.95);
        }

        .form-panel {
            background: white;
            padding: 30px;
            border-radius: 18px;
            box-shadow: 0 3px 12px rgba(0,0,0,0.08);
            max-width: 1200px;
            margin: 0 auto 40px auto;
        }

        .section-block {
            margin-bottom: 40px;
        }

        .section-block h5 {
            font-size: 16px;
            font-weight: 600;
            color: #555;
            margin-bottom: 20px;
            border-left: 5px solid #c06ed4;
            padding-left: 12px;
        }

        .row.g-3 {
            display: flex;
            flex-wrap: wrap;
            gap: 20px;
        }

        .row.g-3 > .col-md-4,
        .row.g-3 > .col-md-6,
        .row.g-3 > .col-md-12 {
            flex: 1 1 calc(33% - 20px);
            min-width: 250px;
        }

        label {
            display: block;
            margin-bottom: 6px;
            font-weight: 500;
            color: #333;
        }

        input[type="text"],
        input[type="date"],
        input[type="email"],
        select,
        textarea {
            width: 100%;
            padding: 10px 12px;
            border: 1px solid #ccc;
            border-radius: 8px;
            background-color: #fff;
            transition: all 0.2s;
        }

        input:focus, select:focus, textarea:focus {
            border-color: #83ceee;
            outline: none;
            box-shadow: 0 0 0 3px rgba(131, 206, 238, 0.25);
        }

        .btn-success {
            background-color: #4CB07A;
            border: none;
            padding: 10px 24px;
            border-radius: 10px;
            font-weight: 500;
            font-size: 14px;
            color: white;
            cursor: pointer;
        }

        .btn-success:hover {
            background-color: #3e9f69;
        }

        .btn-secondary {
            background-color: #9E9E9E;
            color: white;
            padding: 10px 24px;
            border: none;
            border-radius: 8px;
            font-weight: 500;
            cursor: pointer;
        }

        .btn-secondary:hover {
            background-color: #757575;
        }

        .text-danger {
            color: #f44336;
            font-size: 13px;
        }

        .table-custom {
            width: 100%;
            border-collapse: collapse;
            border: 1px solid #ddd;
            background-color: white;
            border-radius: 8px;
            overflow: hidden;
        }

        .table-custom th, .table-custom td {
            padding: 12px 16px;
            text-align: left;
        }

        .table-custom th {
            background-color: #f4f7fb;
            color: #444;
            font-weight: 600;
        }

        .table-custom tr:nth-child(even) {
            background-color: #fafafa;
        }

        .titulo-cadastro {
            font-size: 20px;
            color: #4CB07A;
            font-weight: 600;
            margin-bottom: 24px;
            text-align: center;
            border-bottom: 2px solid #ddd;
            padding-bottom: 10px;
        }

        .btn-busca-nome{
             background-color: #83ceee;
             color: white;
             font-weight: 500;
             font-size: 13px;
             padding: 10px 14px;
             border-radius: 10px;
             border: none;
             cursor: pointer;
             transition: 0.2s;
        }
        .btn-busca-cpf {
            background-color: #c06ed4;
             color: white;
             font-weight: 500;
             font-size: 13px;
             padding: 10px 14px;
             border-radius: 10px;
             border: none;
             cursor: pointer;
             transition: 0.2s;
        }

        .btn-busca-nome:hover{
            background-color: #67b7da;
        }

        .btn-busca-cpf:hover {
            background-color: #c06ed4;
        }

        @media (max-width: 768px) {
            .row.g-3 > .col-md-4,
            .row.g-3 > .col-md-6,
            .row.g-3 > .col-md-12 {
                flex: 1 1 100%;
            }
        }
    </style>

<script type="text/javascript">
    function abrirModalEdicao(codPessoa, nome, cpf, rg, email, telefone, cargo) {
        $('#<%= hfCodPessoa.ClientID %>').val(codPessoa);
        $('#<%= txtModalNome.ClientID %>').val(nome);
        $('#<%= txtModalCPF.ClientID %>').val(cpf);
        $('#<%= txtModalRG.ClientID %>').val(rg);
        $('#<%= txtModalEmail.ClientID %>').val(email);
        $('#<%= txtModalTelefone.ClientID %>').val(telefone);
        $('#<%= txtModalCargo.ClientID %>').val(cargo);

        var modal = new bootstrap.Modal(document.getElementById('modalEditarUsuario'));
        modal.show();
    }
</script>

</asp:Content>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container py-4">
        <h2 class="titulo-gestao" runat="server" id="lblTitGestao">Gestão de Colaboradores</h2>

        <div class="container-gestao">
            <asp:Button ID="btnIncluirUsuario" runat="server" Text="Incluir Usuário" CssClass="btn-gestao btn-incluir" OnClick="btnIncluirUsuario_Click" />
            <asp:Button ID="btnConsultarUsuario" runat="server" Text="Consultar Usuário" CssClass="btn-gestao btn-consultar" OnClick="btnConsultarUsuario_Click" />
            <asp:Button ID="btnDesativarUsuario" runat="server" Text="Desativar Usuário" CssClass="btn-gestao btn-desativar" OnClick="btnDesativarUsuario_Click" />
        </div>

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
                        <label>CPF *</label>
                        <asp:TextBox ID="txtDocCPF" runat="server" CssClass="form-control" MaxLength="11" placeHolder="Insira apenas números" />          
                    </div>
                    <div class="col-md-6">
                        <label>RG *</label>
                        <asp:TextBox ID="txtDocRG" runat="server" CssClass="form-control" />
                        <asp:RequiredFieldValidator ID="rfvRG" runat="server" ControlToValidate="txtDocRG" ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" ValidationGroup="Cadastro" />
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
                    <div class="col-md-6">
                        <label>Demissão</label>
                        <asp:TextBox ID="txtDataDemissao" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>
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
                <div class="row g-3">

                    <div class="col-md-4">
                        <label>
                            <asp:CheckBox ID="chkCriaContaAD" runat="server" />
                            Criar conta no AD</label>
                    </div>
                    <div class="col-md-4">
                        <label>
                            <asp:CheckBox ID="chkCadastraPonto" runat="server" />
                            Cadastrar no ponto</label>
                    </div>
                    <div class="col-md-4">
                        <label>
                            <asp:CheckBox ID="chkAtivo" runat="server" Checked="true" />
                            Ativo *</label>
                    </div>
                    <div class="col-md-4">
                        <label>
                            <asp:CheckBox ID="chkPermiteAcesso" runat="server" />
                            Permite Acesso</label>
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
                <asp:Button ID="btnSalvarUsuario" runat="server" Text="Salvar Usuário" CssClass="btn btn-success"
                    ValidationGroup="Cadastro" OnClick="btnSalvarUsuario_Click" />
            </div>
        </asp:Panel>

        <asp:Panel ID="PanelConsulta" runat="server" CssClass="form-panel mt-4" Visible="false">
            <!-- Título principal -->
            <h4 class="titulo-cadastro">Consultar Usuário</h4>

            <!-- 🔎 BLOCO DE BUSCA -->
            <div class="section-block">
                <h5>Buscar por Nome ou CPF</h5>

                <!-- BLOCO DE NOME -->
                <div class="row g-3 align-items-end mb-3">
                    <div class="col-md-6">
                        <label>Buscar por Nome</label>
                        <asp:TextBox ID="txtBuscaNome" runat="server" CssClass="form-control" placeholder="Digite o nome" />
                    </div>
                    <div class="col-md-3">
                        <asp:Button ID="btnBuscarPorNome" runat="server" Text="Buscar por Nome" CssClass="btn btn-busca-nome w-100" OnClick="btnBuscarPorNome_Click" />
                    </div>
                </div>

                <!-- BLOCO DE CPF -->
                <div class="row g-3 align-items-end">
                    <div class="col-md-6">
                        <label>Buscar por CPF</label>
                        <asp:TextBox ID="txtBuscaCPF" runat="server" CssClass="form-control" placeholder="Digite o CPF (somente números)" MaxLength="11" />
                    </div>
                    <div class="col-md-3">
                        <asp:Button ID="btnBuscarPorCPF" runat="server" Text="Buscar por CPF" CssClass="btn btn-busca-cpf w-100" OnClick="btnBuscarPorCPF_Click" />
                    </div>
                </div>
            </div>

            <!-- 📋 RESULTADOS -->
            <asp:Panel ID="PanelResultado" runat="server" CssClass="section-block" Visible="false">
                <h5>Resultados</h5>
                <asp:GridView 
                    ID="gvUsuarios" 
                    runat="server" 
                    CssClass="table-custom" 
                    AutoGenerateColumns="False" 
                    GridLines="None" 
                    ShowHeaderWhenEmpty="False" 
                    EmptyDataText="Nenhum usuário encontrado.">
            
                    <HeaderStyle CssClass="table-custom-header" />
                    <Columns>
                        <asp:BoundField DataField="CodPessoa" HeaderText="CodPessoa" />
                        <asp:BoundField DataField="NomeCompleto" HeaderText="Nome" />
                        <asp:BoundField DataField="CPF" HeaderText="CPF" />
                        <asp:BoundField DataField="RG" HeaderText="RG" />
                        <asp:BoundField DataField="Email" HeaderText="Email" />
                        <asp:BoundField DataField="Telefone1" HeaderText="Telefone" />
                        <asp:BoundField DataField="Cargo" HeaderText="Cargo" />
                       <asp:TemplateField HeaderText="Editar">
                            <ItemTemplate>
                                <button type="button" class="btn btn-info btn-sm btn-pill"
                                    onclick='abrirModalEdicao(
                                        <%# Eval("CodPessoa") %>,
                                        "<%# Eval("NomeCompleto").ToString().Replace("\"", "\\\"") %>",
                                        "<%# Eval("CPF").ToString() %>",
                                        "<%# Eval("RG").ToString() %>",
                                        "<%# Eval("Email").ToString() %>",
                                        "<%# Eval("Telefone1").ToString() %>",
                                        "<%# Eval("Cargo").ToString().Replace("\"", "\\\"") %>"
                                    )'>
                                    <i class="fa-solid fa-pen-to-square"></i>
                                </button>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>

                <!-- Modal Edição -->
                    <div class="modal fade" id="modalEditarUsuario" tabindex="-1" aria-labelledby="modalEditarUsuarioLabel" aria-hidden="true">
                        <div class="modal-dialog modal-lg modal-dialog-centered">
                            <div class="modal-content rounded-4 shadow">
                                <div class="modal-header bg-info text-white">
                                    <h5 class="modal-title" id="modalEditarUsuarioLabel">
                                        <i class="fa-solid fa-user-pen me-2"></i>Editar Usuário
                                    </h5>
                                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Fechar"></button>
                                </div>

                                <div class="modal-body">
                                    <!-- Campos do formulário -->
                                    <asp:HiddenField ID="hfCodPessoa" runat="server" />

                                    <div class="row g-3">
                                        <div class="col-md-6">
                                            <label class="form-label">Nome Completo</label>
                                            <asp:TextBox ID="txtModalNome" runat="server" CssClass="form-control" />
                                        </div>
                                        <div class="col-md-6">
                                            <label class="form-label">CPF</label>
                                            <asp:TextBox ID="txtModalCPF" runat="server" CssClass="form-control" />
                                        </div>
                                        <div class="col-md-6">
                                            <label class="form-label">RG</label>
                                            <asp:TextBox ID="txtModalRG" runat="server" CssClass="form-control" />
                                        </div>
                                        <div class="col-md-6">
                                            <label class="form-label">E-mail</label>
                                            <asp:TextBox ID="txtModalEmail" runat="server" CssClass="form-control" />
                                        </div>
                                        <div class="col-md-6">
                                            <label class="form-label">Telefone</label>
                                            <asp:TextBox ID="txtModalTelefone" runat="server" CssClass="form-control" />
                                        </div>
                                        <div class="col-md-6">
                                            <label class="form-label">Cargo</label>
                                            <asp:TextBox ID="txtModalCargo" runat="server" CssClass="form-control" />
                                        </div>
                                    </div>
                                </div>

                                <div class="modal-footer bg-light rounded-bottom-4">
                                    <asp:Button ID="btnSaveUser" runat="server"
                                        CssClass="btn btn-success btn-pill"
                                        Text="Salvar Alterações"
                                        OnClick="btnSaveUser_Click" />

                                    <button type="button" class="btn btn-secondary btn-pill" data-bs-dismiss="modal">
                                        <i class="fa-solid fa-xmark me-1"></i>Fechar
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
            </asp:Panel>
        </asp:Panel>

    </div>


    <script type="text/javascript">
        Sys.Application.add_load(function () {
            if ($('#<%= PanelCadastro.ClientID %>').is(':visible')) {
                aplicarMascaras();
            }
        });
    </script>
</asp:Content>
