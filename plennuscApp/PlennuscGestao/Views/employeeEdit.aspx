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

    <link href="../../Content/Css/projects/gestao/structuresCss/employee-Edit.css" rel="stylesheet" />

    <title>Gestão de Funcionários</title>

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

<script>
    (function () {
        // pega um nome amigável pro campo (usa <label> mais próximo)
        function labelAmigavel(input) {
            if (!input) return 'Campo obrigatório';
            const wrap = input.closest('label');
            if (wrap) {
                const txt = Array.from(wrap.childNodes)
                    .filter(n => n.nodeType === Node.TEXT_NODE)
                    .map(n => n.textContent).join(' ').trim();
                if (txt) return txt.replace('*', '').trim();
            }
            const bloco = input.closest('.col-md-4, .col-md-6, .col-md-12, .cell, .form-group') || input.parentElement;
            if (bloco) {
                const lbl = bloco.querySelector('label');
                if (lbl && lbl.textContent) return lbl.textContent.replace('*', '').trim();
            }
            return input.placeholder || input.name || input.id || 'Campo obrigatório';
        }

        // coleta inválidos dos validators ASP.NET (depois de Page_ClientValidate)
        function invalidosAspNet(group) {
            const itens = [];
            if (!window.Page_Validators) return itens;
            Page_Validators.forEach(v => {
                if (v.validationGroup !== group) return;
                const el = document.getElementById(v.controltovalidate);
                if (el && v.isvalid === false) {
                    el.classList.add('is-invalid');
                    itens.push({ el, name: labelAmigavel(el) });
                }
            });
            return itens;
        }

        // coleta inválidos HTML5 (email, date, required nativo etc.)
        function invalidosHtml5(form) {
            const itens = [];
            if (!form) return itens;
            form.querySelectorAll(':invalid').forEach(el => {
                el.classList.add('is-invalid');
                itens.push({ el, name: labelAmigavel(el) });
            });
            return itens;
        }

        // mostra toast no topo-direita (SweetAlert2)
        function toastTopRight(titulo, texto) {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'warning',
                iconColor: '#dc3545',
                title: 'Faltou preencher',
                text: 'Data de Nascimento, Perfil Pessoa',
                showConfirmButton: false,
                timer: 4500,
                timerProgressBar: true,
                customClass: { popup: 'toast-warn' }
            });
        }

        // anexa validação + toast no botão
        function attach(btnId, group) {
            const btn = document.getElementById(btnId);
            if (!btn) return;
            const form = btn.form || document.querySelector('form');

            btn.addEventListener('click', function (ev) {
                // valida client-side do WebForms
                if (typeof (Page_ClientValidate) === 'function') Page_ClientValidate(group);
                const validoAsp = (typeof (Page_IsValid) === 'undefined') ? true : Page_IsValid;
                const validoHtml = form ? form.checkValidity() : true;

                if (validoAsp && validoHtml) {
                    // tudo certo -> deixa postback acontecer
                    return true;
                }

                // bloqueia e avisa no topo
                ev.preventDefault(); ev.stopPropagation();

                const faltas = [];
                if (!validoAsp) faltas.push(...invalidosAspNet(group));
                if (!validoHtml) faltas.push(...invalidosHtml5(form));

                // dedup por elemento + nomes
                const vistos = new Set(), unicos = [];
                for (const f of faltas) { if (!vistos.has(f.el)) { vistos.add(f.el); unicos.push(f); } }
                const nomes = [...new Set(unicos.map(x => x.name))];

                const titulo = nomes.length === 1 ? 'Preencha o campo obrigatório' : 'Faltou preencher';
                const texto = nomes.length <= 3 ? nomes.join(', ') : (nomes.slice(0, 3).join(', ') + '…');

                toastTopRight(titulo, texto);

                // foca no primeiro inválido
                if (unicos.length) setTimeout(() => unicos[0].el.focus(), 0);

                return false;
            }, { capture: true });
        }

        document.addEventListener('DOMContentLoaded', function () {
            attach('<%= btnSalvarUsuario.ClientID %>', 'Cadastro');
    // se quiser o mesmo comportamento no "Criar Login", descomente:
    // attach('<%= btnCriarLogin.ClientID %>', 'Acesso');
  });
    })();
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
                        <label>Email Coorporativo *</label>
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
        
                      <!-- NOVO CAMPO: EMPRESA -->
                      <div class="col-md-12">
                          <label>Empresa *</label>
                          <asp:DropDownList ID="ddlEmpresa" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                              <asp:ListItem Text="Selecione a empresa" Value="" />
                          </asp:DropDownList>
                          <asp:RequiredFieldValidator ID="rfvEmpresa" runat="server" ControlToValidate="ddlEmpresa" 
                              InitialValue="" ErrorMessage="Selecione a empresa" CssClass="text-danger" 
                              Display="Dynamic" ValidationGroup="Cadastro" />
                      </div>

                      <div class="col-md-12">
                          <label>Perfil Pessoa *</label>
                          <asp:DropDownList ID="ddlPerfilPessoa" runat="server" CssClass="form-control" AppendDataBoundItems="true">
                          </asp:DropDownList>
                          <asp:RequiredFieldValidator ID="rfvPerfilPessoa" runat="server" ControlToValidate="ddlPerfilPessoa" 
                              InitialValue="" ErrorMessage="Campo obrigatório" CssClass="text-danger" 
                              Display="Dynamic" ValidationGroup="Cadastro" />
                      </div>

                      <div class="col-md-6">
                          <label>Cargo *</label>
                          <asp:DropDownList ID="ddlCargo" runat="server" CssClass="form-control">
                              <asp:ListItem Text="Selecione" Value="" />
                          </asp:DropDownList>
                          <asp:RequiredFieldValidator ID="rfvCargo" runat="server" ControlToValidate="ddlCargo" 
                              InitialValue="" ErrorMessage="Campo obrigatório" CssClass="text-danger" 
                              Display="Dynamic" ValidationGroup="Cadastro" />
                      </div>
        
                      <div class="col-md-6">
                          <label>Departamento *</label>
                          <asp:DropDownList ID="ddlDepartamento" runat="server" CssClass="form-control">
                          </asp:DropDownList>
                          <asp:RequiredFieldValidator ID="rfvDepartamento" runat="server" ControlToValidate="ddlDepartamento" 
                              InitialValue="" ErrorMessage="Campo obrigatório" CssClass="text-danger" 
                              Display="Dynamic" ValidationGroup="Cadastro" />
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

                <a href="employeeManagement?acao=consultar" class="btn btn-secondary">Cancelar</a>
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
                        <label class="form-label">Empresa</label>
                        <asp:TextBox ID="txtLoginEmpresa" runat="server" CssClass="form-control" ReadOnly="true" />
                        <asp:HiddenField ID="hfCodEmpresa" runat="server" />
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
