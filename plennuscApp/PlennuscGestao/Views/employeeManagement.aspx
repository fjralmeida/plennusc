<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="employeeManagement.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.employeeManagement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.16/jquery.mask.min.js"></script>
<link href="https://cdn.jsdelivr.net/npm/sweetalert2@11.7.12/dist/sweetalert2.min.css" rel="stylesheet" />
<script src="https://cdn.jsdelivr.net/npm/sweetalert2@11.7.12/dist/sweetalert2.all.min.js"></script>

<script type="text/javascript">
    function aplicarMascaras() {
        // CPF
        $('#<%= txtDocCPF.ClientID %>').mask('000.000.000-00', { reverse: true });

        // Telefones (Celular ou Fixo dinâmico)
        aplicarMascaraTelefone('#<%= txtTelefone1.ClientID %>');
        aplicarMascaraTelefone('#<%= txtTelefone2.ClientID %>');
        aplicarMascaraTelefone('#<%= txtTelefone3.ClientID %>');
    }

    function aplicarMascaraTelefone(selector) {
        var $campo = $(selector);
        $campo.mask('(00) 00000-0000');

        $campo.on('blur', function () {
            var val = $campo.val().replace(/\D/g, '');
            if (val.length <= 10) {
                $campo.mask('(00) 0000-0000');
            } else {
                $campo.mask('(00) 00000-0000');
            }
        });
    }

    // Aplica as máscaras quando o painel estiver visível
    Sys.Application.add_load(function () {
        if ($('#<%= PanelCadastro.ClientID %>').is(':visible')) {
            aplicarMascaras();
        }
    });
</script>

    <style>
        .titulo-gestao {
            font-weight: 600;
            font-size: 1.5rem;
            color: #4CB07A;
            margin-bottom: 1.5rem;
            text-align: center;
        }

        .container-gestao {
            display: flex;
            justify-content: center;
            flex-wrap: wrap;
            gap: 0.75rem;
        }

        .btn-gestao {
            padding: 0.5rem 1.2rem;
            font-size: 0.95rem;
            border-radius: 8px;
            border: none;
            color: white;
            cursor: pointer;
            transition: transform 0.2s ease, box-shadow 0.2s ease;
            box-shadow: 0 2px 6px rgba(0, 0, 0, 0.08);
            display: flex;
            align-items: center;
            gap: 0.5rem;
        }

            .btn-gestao:hover {
                transform: translateY(-2px);
                box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
            }

        .btn-incluir {
            background-color: #4CB07A;
        }

            .btn-incluir:hover {
                background-color: #3c9a66;
            }

        .btn-consultar {
            background-color: #83CEEE;
        }

            .btn-consultar:hover {
                background-color: #63b6da;
            }

        .btn-desativar {
            background-color: #DC8689;
        }

            .btn-desativar:hover {
                background-color: #c96c6f;
            }

        @media (max-width: 600px) {
            .btn-gestao {
                width: 100%;
                justify-content: center;
            }

            .container-gestao {
                flex-direction: column;
                align-items: center;
            }
        }

        /*PANEL E ESTILOS NOS CAMPOS PARA SEREM EXIBIDOS NO MOMENTO DE INCLUIR USUARIO*/
        .form-panel {
            max-width: 1000px;
            margin: 0 auto;
        }

        /* Seções em blocos alternando cores */
        .section-block {
            background: #ffffff;
            padding: 1.5rem 2rem;
            margin-bottom: 1.5rem;
            border-radius: 12px;
            box-shadow: 0 2px 8px rgba(0, 0, 0, 0.03);
        }

            .section-block:nth-child(even) {
                background: #f7fafd;
            }

            /* Títulos com cor verde padrão */
            .section-block h5 {
                font-size: 1.1rem;
                font-weight: 600;
                color: #4CB07A;
                margin-bottom: 1rem;
                border-left: 5px solid #83CEEE;
                padding-left: 0.6rem;
            }

        /* Campos em grid com espaçamento responsivo */
        .row.g-3 {
            display: flex;
            flex-wrap: wrap;
            gap: 1rem;
        }

            .row.g-3 > .col-md-6,
            .row.g-3 > .col-md-4 {
                flex: 1 1 47%;
                min-width: 260px;
            }

        .titulo-cadastro {
            font-size: 1.1rem;
            font-weight: 500;
            text-align: center;
            color: #444;
            margin: 1rem auto 2rem auto;
            padding-bottom: 0.3rem;
            border-bottom: 2px solid #c06ed4; /* linha azul para destacar */
            max-width: 350px;
        }

        label {
            font-weight: 600;
            margin-bottom: 0.3rem;
            display: block;
            color: #333;
        }

        /* Inputs com foco azul e detalhes suaves */
        input[type="text"],
        input[type="date"],
        input[type="email"],
        select,
        textarea,
        .form-control {
            width: 100%;
            padding: 0.5rem 0.75rem;
            border: 1px solid #ced4da;
            border-radius: 6px;
            font-size: 0.95rem;
            transition: border-color 0.3s ease-in-out;
        }

            input:focus,
            select:focus,
            textarea:focus {
                border-color: #83CEEE;
                outline: none;
                box-shadow: 0 0 0 2px rgba(131, 206, 238, 0.2);
            }

        /* Validação com vermelho suave */
        .text-danger {
            color: #DC8689;
            font-size: 0.85rem;
            font-weight: 500;
        }

        /* Botão Salvar com verde */
        .btn-success {
            background-color: #4CB07A;
            border: none;
            padding: 0.6rem 1.5rem;
            font-size: 1rem;
            border-radius: 8px;
            color: white;
            transition: all 0.3s ease;
        }

            .btn-success:hover {
                background-color: #3b9562;
            }

        /* Botões secundários com roxo padrão */
        .btn-secondary {
            background-color: #c06ed4;
            color: white;
            border: none;
            padding: 0.5rem 1.2rem;
            font-size: 0.95rem;
            border-radius: 6px;
        }

            .btn-secondary:hover {
                background-color: #a44cba;
            }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container py-4">
        <h2 class="titulo-gestao" runat="server" id="lblTitGestao">Gestão de Colaboradores</h2>

        <div class="container-gestao">
            <asp:Button ID="btnIncluirUsuario" runat="server" Text="➕ Incluir Usuário" CssClass="btn-gestao btn-incluir" OnClick="btnIncluirUsuario_Click" />
            <asp:Button ID="btnConsultarUsuario" runat="server" Text="🔍 Consultar Usuário" CssClass="btn-gestao btn-consultar" OnClick="btnConsultarUsuario_Click" />
            <asp:Button ID="btnDesativarUsuario" runat="server" Text="🚫 Desativar Usuário" CssClass="btn-gestao btn-desativar" OnClick="btnDesativarUsuario_Click" />
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
                    <asp:TextBox ID="txtDocCPF" runat="server" CssClass="form-control" MaxLength="14" />
                    <asp:RequiredFieldValidator ID="rfvCPF" runat="server" ControlToValidate="txtDocCPF" ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" ValidationGroup="Cadastro" />
                    <asp:RegularExpressionValidator ID="revCPF" runat="server" ControlToValidate="txtDocCPF" 
                        ValidationExpression="^\d{3}\.\d{3}\.\d{3}-\d{2}$" ErrorMessage="CPF inválido" CssClass="text-danger" Display="Dynamic" ValidationGroup="Cadastro" />
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
                              <asp:ListItem Text="Selecione" Value="" />
                          </asp:DropDownList>
                          <asp:RequiredFieldValidator ID="rfvPerfilPessoa" runat="server" ControlToValidate="ddlPerfilPessoa" InitialValue="" ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" ValidationGroup="Cadastro" />
                      </div>

                    <div class="col-md-6">
                        <label>Cargo *</label>
                        <asp:DropDownList ID="ddlCargo" runat="server" CssClass="form-control">
                            <asp:ListItem Text= "Selecione" Value="" />
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
                <asp:Button ID="btnSalvarUsuario" runat="server" Text="💾 Salvar Usuário" CssClass="btn btn-success"
                    ValidationGroup="Cadastro" OnClick="btnSalvarUsuario_Click" />
            </div>
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
