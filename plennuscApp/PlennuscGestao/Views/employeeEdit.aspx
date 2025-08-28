<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="employeeEdit.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.employeeEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- jQuery -->
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
    
    <!-- jQuery Mask Plugin -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.16/jquery.mask.min.js"></script>

    <!-- Outras bibliotecas -->
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/sweetalert2@11.7.12/dist/sweetalert2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11.7.12/dist/sweetalert2.all.min.js"></script>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />

    <!-- Bootstrap 5 CSS -->
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet"/>

<!-- Bootstrap 5 JS (bundle = com Popper) -->
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>


    <title>Gestão de Funcionários</title>

    <style>
        * { font-family: 'Poppins', sans-serif; box-sizing: border-box; }
        body, html { background-color: #f2f4f8; color: #333; font-size: 14px; }

        .titulo-gestao { font-size: 26px; font-weight: 600; margin-bottom: 30px; color: #413a3a; text-align: center; }

        .container-gestao { display: flex; gap: 12px; justify-content: center; flex-wrap: wrap; margin-bottom: 30px; }
        .btn-gestao { padding: 6px 14px; border: none; border-radius: 10px; font-size: 15px; font-weight: 500; color: white; cursor: pointer; transition: all 0.2s ease-in-out; }
        .btn-incluir { background-color: #4CB07A; }
        .btn-consultar { background-color: #83ceee; }
        .btn-desativar { background-color: #DC8689; }
        .btn-gestao:hover { transform: translateY(-1px); filter: brightness(0.95); }
        .btn-gestao,.btn-gestao:link,.btn-gestao:visited,.btn-gestao:hover,.btn-gestao:active {text-decoration: none !important;display: inline-block; }

        .btn { border: none; padding: 10px 24px;border-radius: 10px;font-weight: 500; font-size: 14px;cursor: pointer;display: inline-block;text-decoration: none !important; /* remove sublinhado de <a class="btn"> */}


        .btn + .btn { margin-left: 8px; }
        .btn-login { background-color: #4CB07A; color: #fff;  box-shadow: 0 3px 8px rgba(76, 176, 122, 0.25);}
        .btn-login:hover { background-color: #3A9E68;}
        .btn-secondary { background-color: #ccc;  color: #333; }
        .btn-secondary:hover { background-color: #bbb; }

        .form-panel { background: white; padding: 30px; border-radius: 18px; box-shadow: 0 3px 12px rgba(0,0,0,0.08); max-width: 1900px; margin: 0 auto 40px auto; }

        .section-block { margin-bottom: 40px; }
        .section-block h5 { font-size: 16px; font-weight: 600; color: #353030; margin-bottom: 20px; border-left: 5px solid #c06ed4; padding-left: 12px; }

        .row.g-3 { display: flex; flex-wrap: wrap; gap: 20px; }
        .row.g-3 > .col-md-4, .row.g-3 > .col-md-6, .row.g-3 > .col-md-12 { flex: 1 1 calc(33% - 20px); min-width: 250px; }

        label { display: block; margin-bottom: 6px; font-weight: 500; color: #333; }

        input[type="text"], input[type="date"], input[type="email"], select, textarea {
            width: 100%; padding: 10px 12px; border: 1px solid #ccc; border-radius: 8px; background-color: #fff; transition: all 0.2s;
        }
        input:focus, select:focus, textarea:focus { border-color: #83ceee; outline: none; box-shadow: 0 0 0 3px rgba(131, 206, 238, 0.25); }

        .btn-success { background-color: #83ceee; border: none; padding: 10px 24px; border-radius: 10px; font-weight: 500; font-size: 14px; color: white; cursor: pointer; box-shadow: 0 3px 8px rgba(76, 176, 122, 0.25); }
        .btn-success:hover { background-color: #67b7da; }

        .btn-secondary { background-color: #ccc; color: #333; padding: 10px 24px; border: none; border-radius: 10px; font-weight: 500; cursor: pointer; }
        .btn-secondary:hover { background-color: #bbb; }

        .text-danger { color: #f44336; font-size: 13px; }

        .btn-editar { background-color: #4CB07A; color: #fff; border: none; padding: 6px 12px; font-size: 13px; border-radius: 50px; transition: background-color 0.3s; }
        .btn-editar:hover { background-color: #3A9E68 !important; }
        .bg-info { background-color: #4CB07A !important; margin: 10px; border-radius: 20px; }

        @media (max-width: 768px) {
            .modal-body .row.g-3 > .col-md-6 { flex: 1 1 100%; }
        }

      /* ===== MODAL VISUAL ===== */
#modalCriarLogin .modal-content {
  border: 0;
  border-radius: 16px;
  overflow: hidden;
}
#modalCriarLogin .modal-header {
  background: linear-gradient(90deg, #4CB07A, #3A9E68);
  color: #fff;
  border: 0;
}
#modalCriarLogin .modal-footer {
  background: #f9fafb;
  border: 0;
}

/* Campos modernos */
#modalCriarLogin .form-control,
#modalCriarLogin .form-select {
  border-radius: 10px;
  border: 1px solid #ddd;
  transition: all .2s;
}
#modalCriarLogin .form-control:focus,
#modalCriarLogin .form-select:focus {
  border-color: #4CB07A;
  box-shadow: 0 0 0 3px rgba(76,176,122,.25);
}

/* Botões */
#modalCriarLogin .btn-login {
  background-color: #4CB07A;
  border-radius: 10px;
  padding: 10px 22px;
  font-weight: 600;
  color: #fff;
  box-shadow: 0 3px 8px rgba(76,176,122,.25);
}
#modalCriarLogin .btn-login:hover { background-color: #3A9E68; }
#modalCriarLogin .btn-secondary {
  border-radius: 10px;
  padding: 10px 22px;
  font-weight: 500;
}

/* ===== GRID COM DIVISÓRIAS ===== */
#modalCriarLogin .form-grid {
  display: grid;
  grid-template-columns: 1fr;           /* mobile */
  border: 1px solid #e5e7eb;            /* borda externa */
  border-radius: 12px;
  overflow: hidden;
  background: #fff;
}
#modalCriarLogin .cell {
  padding: 14px 16px;
  border-bottom: 1px solid #eee;        /* linha horizontal */
}
#modalCriarLogin .cell:last-child {
  border-bottom: none;                   /* última célula da grade */
}

/* 2 colunas >=768px */
@media (min-width: 768px) {
  #modalCriarLogin .form-grid {
    grid-template-columns: repeat(2, minmax(0,1fr));
  }
  #modalCriarLogin .cell {
    border-right: 1px solid #eee;       /* linha vertical */
  }
  #modalCriarLogin .cell:nth-child(2n) {
    border-right: none;                  /* última coluna não tem borda à direita */
  }
  /* última linha em 2 colunas: remova a borda inferior */
  #modalCriarLogin .cell:nth-last-child(-n+2) {
    border-bottom: none;
  }
  #modalCriarLogin .span-3 {
    grid-column: 1 / -1;                 /* ocupa as 2 colunas */
    border-right: none;
    border-bottom: none;
  }
}

/* 3 colunas >=992px */
@media (min-width: 992px) {
  #modalCriarLogin .form-grid {
    grid-template-columns: repeat(3, minmax(0,1fr));
  }
  #modalCriarLogin .cell {
    border-right: 1px solid #eee;
  }
  #modalCriarLogin .cell:nth-child(3n) {
    border-right: none;                  /* última coluna */
  }
  /* última linha em 3 colunas: remova a borda inferior */
  #modalCriarLogin .cell:nth-last-child(-n+3) {
    border-bottom: none;
  }
  #modalCriarLogin .span-3 {
    grid-column: 1 / -1;                 /* ocupa as 3 colunas */
    border-right: none;
    border-bottom: none;
  }
}

/* ===== CheckBoxList em 2 colunas bem espaçado ===== */
#modalCriarLogin .check-grid {
  width: 100%;
  table-layout: fixed;                   /* cada coluna ocupa 50% */
  border-collapse: separate;
  border-spacing: 16px 8px;              /* HORIZONTAL, VERTICAL */
}
#modalCriarLogin .check-grid td {
  width: 50%;
  padding: 0;
  vertical-align: middle;
  white-space: normal;                   /* permite quebra */
}
#modalCriarLogin .check-grid input[type="checkbox"] {
  margin-right: 8px;
}

/* Flags lado a lado */
#modalCriarLogin .flags {
  display: flex;
  align-items: center;
  gap: 24px;
  flex-wrap: wrap;
}
#modalCriarLogin .flag-line {
  display: inline-flex;
  align-items: center;
  gap: 6px;
  font-weight: 500;
}


        .btn-inativar { background-color: #DC8689; color: white; border: none; padding: 6px 12px; font-size: 13px; border-radius: 50px; transition: background-color 0.3s; }
        .btn-inativar:hover { background-color: #c95b60; }
    </style>

   <script>
       function showModalCriarLogin() {
           var el = document.getElementById('modalCriarLogin');
           if (!el) return;

           // Bootstrap 5
           if (window.bootstrap && bootstrap.Modal) {
               var m = bootstrap.Modal.getOrCreateInstance(el);
               m.show();
               return;
           }
           // Bootstrap 4
           if (window.jQuery && $('#modalCriarLogin').modal) {
               $('#modalCriarLogin').modal('show');
               return;
           }
           // fallback tosco (só pra debug se nada carregou)
           el.classList.add('show');
           el.style.display = 'block';
           el.removeAttribute('aria-hidden');
           document.body.classList.add('modal-open');
       }
   </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <div class="container py-4">
        <h2 class="titulo-gestao" runat="server" id="lblTitGestao">Editar Colaborador</h2>

        <!-- (Opcional) barra de ações / voltar -->
      <%--  <div class="container-gestao">
            <a href="employeeManagement.aspx" class="btn-gestao btn-consultar"> Voltar</a>
        </div>--%>

        <!-- Hidden com o ID da pessoa -->
        <asp:HiddenField ID="hfCodPessoa" runat="server" />

        <!-- Painel de edição (mesmo layout do cadastro) -->
        <asp:Panel ID="PanelCadastro" runat="server" CssClass="form-panel mt-4" Visible="true">
            <h4 class="titulo-cadastro">Dados do Colaborador</h4>

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
                        <label>CPF *<asp:TextBox ID="txtDocCPF" runat="server" CssClass="form-control" MaxLength="14" placeHolder="000.000.000-00" /> </label>
                    </div>
                    <div class="col-md-6">
                        <label>RG *
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
                    <%-- <div class="col-md-6" id="linhaDemissao" runat="server">
                        <label>Demissão</label>
                        <asp:TextBox ID="txtDataDemissao" runat="server" CssClass="form-control" TextMode="Date" />
                    </div> --%>
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
                        <asp:RequiredFieldValidator ID="rfvTel1" runat="server" ControlToValidate="txtTelefone1" ErrorMessage="Campo obrigatório" CssClass="text-danger" Display="Dynamic" ValidationGroup="Cadastro" />
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
                        <asp:DropDownList ID="ddlPerfilPessoa" runat="server" CssClass="form-control" AppendDataBoundItems="true"></asp:DropDownList>
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
                        <asp:DropDownList ID="ddlDepartamento" runat="server" CssClass="form-control"></asp:DropDownList>
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
                            <asp:CheckBox ID="chkCriaContaAD" runat="server" /> Criar conta no AD
                        </label>
                    </div>
                    <div class="col-md-3">
                        <label class="d-flex align-items-center gap-2">
                            <asp:CheckBox ID="chkCadastraPonto" runat="server" /> Cadastrar no ponto
                        </label>
                    </div>
                    <div class="col-md-3">
                        <label class="d-flex align-items-center gap-2">
                            <asp:CheckBox ID="chkAtivo" runat="server" Checked="true" /> Ativo *
                        </label>
                    </div>
                    <div class="col-md-3">
                        <label class="d-flex align-items-center gap-2">
                            <asp:CheckBox ID="chkPermiteAcesso" runat="server" /> Permite Acesso
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
                <asp:Button ID="btnSalvarUsuario" runat="server" 
                    Text="Salvar Alterações"
                    CssClass="btn btn-success"
                    ValidationGroup="Cadastro"
                    OnClick="btnSalvarUsuario_Click" />

                <asp:Button ID="btnCriarLogin" runat="server"  
                    Text="Criar Login"
                    CssClass="btn btn-login"
                    ValidationGroup="Acesso"
                    OnClick="btnCriarLogin_Click" />

                <a href="employeeManagement.aspx" class="btn btn-secondary">Cancelar</a>
            </div>
        </asp:Panel>

         <!-- MODAL: Criar Login -->
<div class="modal fade" id="modalCriarLogin" tabindex="-1" aria-labelledby="modalCriarLoginLabel" aria-hidden="true">
  <div class="modal-dialog modal-lg modal-dialog-centered">
    <div class="modal-content rounded-4 shadow">

      <div class="modal-header">
        <h5 class="modal-title" id="modalCriarLoginLabel">
          <i class="fa-solid fa-user-plus me-2"></i>Criar Login do Colaborador
        </h5>
        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Fechar"></button>
      </div>

      <div class="modal-body">
        <asp:HiddenField ID="hfCodPessoaLogin" runat="server" />

        <!-- GRADE COM DIVISÓRIAS -->
        <div class="form-grid">

          <!-- Linha 1 -->
          <div class="cell">
            <label class="form-label">Nome</label>
            <asp:TextBox ID="txtLoginNome" runat="server" CssClass="form-control" ReadOnly="true" />
          </div>

          <div class="cell">
            <label class="form-label">Sobrenome</label>
            <asp:TextBox ID="txtLoginSobrenome" runat="server" CssClass="form-control" ReadOnly="true" />
          </div>

          <div class="cell">
            <label class="form-label">E-mail</label>
            <asp:TextBox ID="txtLoginEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="nome@empresa.com" />
          </div>

          <!-- Linha 2 -->
          <div class="cell">
            <label class="form-label">Usuário (login)</label>
            <asp:TextBox ID="txtLoginUsuario" runat="server" CssClass="form-control" placeholder="ex.: nome.sobrenome" />
            <small class="form-text text-muted">Ex.: nome.sobrenome</small>
          </div>

          <div class="cell">
            <label class="form-label">Perfil de Acesso</label>
            <asp:DropDownList ID="ddlPerfilUsuario" runat="server" CssClass="form-select">
              <asp:ListItem Value="">Selecione</asp:ListItem>
              <asp:ListItem Value="1">Administrador</asp:ListItem>
              <asp:ListItem Value="2">Gestor</asp:ListItem>
              <asp:ListItem Value="3">Diretor</asp:ListItem>
              <asp:ListItem Value="4">Colaborador</asp:ListItem>
            </asp:DropDownList>
          </div>

          <div class="cell">
            <label class="form-label">Sistemas com acesso</label>
            <asp:CheckBoxList ID="cblSistemas" runat="server"
              RepeatDirection="Horizontal" RepeatColumns="2" RepeatLayout="Table"
              CssClass="check-grid">
            </asp:CheckBoxList>
            <small class="text-muted d-block mt-1">Marque os sistemas que este usuário poderá acessar.</small>
          </div>

          <!-- Linha 3 (span em toda largura) -->
          <div class="cell span-3">
            <div class="flags">
              <label class="flag-line">
                <asp:CheckBox ID="chkLoginAtivo" runat="server" Checked="true" /> Ativo
              </label>
              <label class="flag-line">
                <asp:CheckBox ID="chkLoginPermiteAcesso" runat="server" Checked="true" /> Permite acesso
              </label>
            </div>
          </div>

        </div>
        <!-- /form-grid -->
      </div>

      <div class="modal-footer">
        <asp:Button ID="btnConfirmarCriarLogin" runat="server"
          CssClass="btn btn-login" Text="Criar login"
          OnClick="btnConfirmarCriarLogin_Click" />
        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
      </div>

    </div>
  </div>
</div>



    </div>
</asp:Content>
