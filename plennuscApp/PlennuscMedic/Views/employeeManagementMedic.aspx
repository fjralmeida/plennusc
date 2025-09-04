<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscMedic/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="employeeManagementMedic.aspx.cs" Inherits="appWhatsapp.PlennuscMedic.Views.employeeManagementMedic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<!-- CSS do Bootstrap -->
<link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />

<!-- jQuery -->
<script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

<!-- Bootstrap JS -->
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>

<!-- jQuery Mask Plugin -->
<script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.16/jquery.mask.min.js"></script>

<!-- FontAwesome -->
<link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />

<!-- SweetAlert2 -->
<link href="https://cdn.jsdelivr.net/npm/sweetalert2@11.7.12/dist/sweetalert2.min.css" rel="stylesheet" />
<script src="https://cdn.jsdelivr.net/npm/sweetalert2@11.7.12/dist/sweetalert2.all.min.js"></script>


 <title>Gestão de Funcionários</title>

<style>

* { font-family: 'Poppins', sans-serif; box-sizing: border-box; }
html, body { background:#f2f4f8; color:#333; font-size:14px; }

/* ====== Títulos / barras ====== */
.titulo-gestao { font-size:26px; font-weight:600; margin-bottom:30px; color:#413a3a; text-align:center; }
.titulo-cadastro {
  font-size:20px; color:#4CB07A; font-weight:600; margin-bottom:24px;
  text-align:center; border-bottom:2px solid #ddd; padding-bottom:10px;
}

/* ====== Painéis / seções ====== */
.form-panel { background:#fff; padding:30px; border-radius:18px; box-shadow:0 3px 12px rgba(0,0,0,.08); max-width:1900px; margin:0 auto 40px; }
.section-block { margin-bottom:40px; }
.section-block h5 {
  border-left:5px solid #c06ed4; padding-left:12px; 
  font-size:16px; font-weight:600; color:#353030;  margin-top:20px;
  border-left:5px solid #c06ed4; padding-left:12px; 
}

/* ====== Grid flex simplificado ====== */
.row.g-3 { display:flex; flex-wrap:wrap; gap:20px; }
.row.g-3 > .col-md-4, .row.g-3 > .col-md-6, .row.g-3 > .col-md-12 {
  flex:1 1 calc(33% - 20px); min-width:250px;
}

/* ====== Formulários ====== */
label { display:block; margin-bottom:6px; font-weight:500; color:#333; }
input[type="text"], input[type="date"], input[type="email"], select, textarea {
  width:100%; padding:10px 12px; border:1px solid #ccc; border-radius:8px; background:#fff; transition:all .2s;
}
input:focus, select:focus, textarea:focus {
  border-color:#83ceee; outline:none; box-shadow:0 0 0 3px rgba(131,206,238,.25);
}

/* Campos somente leitura (área de auditoria) */
.input-readonly { background:#f4f6f9 !important; color:#6b7280; }

/* Deixa checkbox verdinho sem precisar mudar markup */
input[type="checkbox"] { accent-color:#4CB07A; }

/* ====== Botões topo ====== */
.container-gestao { display:flex; gap:12px; justify-content:center; flex-wrap:wrap; margin-bottom:30px; }
.btn-gestao {
  padding:6px 14px; border:none; border-radius:10px; font-size:15px; font-weight:500; color:#fff;
  cursor:pointer; transition:all .2s ease-in-out;
}
.btn-gestao:hover { transform:translateY(-1px); filter:brightness(.95); }
.btn-incluir   { background:#4CB07A; }
.btn-consultar { background:#83ceee; }
.btn-desativar { background:#DC8689; }

/* ====== Botões gerais ====== */
.btn-success {
  background:#83ceee; border:none; padding:10px 24px; border-radius:10px; font-weight:500; font-size:14px; color:#fff;
  cursor:pointer; box-shadow:0 3px 8px rgba(76,176,122,.25);
}
.btn-success:hover { background:#67b7da; }

.btn-secondary {
  background:#ccc; color:#333; padding:10px 24px; border:none; border-radius:10px; font-weight:500; cursor:pointer;
}
.btn-secondary:hover { background:#bbb; }

/* Remover sublinhado de anchors usados como botão */
.btn, a.btn, a.btn:link, a.btn:visited, a.btn:hover, a.btn:active { text-decoration:none !important; display:inline-block; }

/* ====== Tabela de resultados ====== */
.table-responsive { width:100%; overflow-x:auto; -webkit-overflow-scrolling:touch; }
.table-custom { width:100%; border-collapse:collapse; border:1px solid #ddd; background:#fff; border-radius:8px; overflow:hidden; }
.table-custom th, .table-custom td { padding:12px 16px; text-align:left; }
.table-custom th { background:#f4f7fb; color:#444; font-weight:600; }
.table-custom tr:nth-child(even) { background:#fafafa; }

/* ====== Ações na grid ====== */
.btn-editar {
  background:#4CB07A; color:#fff; border:none; padding:6px 12px; font-size:13px; border-radius:50px; transition:background-color .3s;
}
.btn-editar:hover { background:#3A9E68 !important; }
.btn-inativar {
  background:#DC8689; color:#fff; border:none; padding:6px 12px; font-size:13px; border-radius:50px; transition:background-color .3s;
}
.btn-inativar:hover { background:#c95b60; }
.bg-info { background:#4CB07A !important; margin:10px; border-radius:20px; }

/* ====== Modal (cores iguais ao resto) ====== */
.modal-header { background:#4CB07A; border-top-left-radius:10px; border-top-right-radius:10px; padding:16px 24px; display:flex; align-items:center; justify-content:space-between; }
.modal-title { font-size:16px; font-weight:600; color:#fff; display:flex; align-items:center; gap:8px; margin:0; }
.modal-title i { font-size:16px; }
.btn-close { filter:brightness(0) invert(1); opacity:.85; transition:.2s; }
.btn-close:hover { opacity:1; }
.modal-body { padding:24px; background:#fff; }
.modal-body .form-control {
  background:#fff; border:1px solid #ccc; border-radius:10px; padding:10px 14px; font-size:14px; color:#333; height:42px;
  box-shadow:0 1px 2px rgba(0,0,0,.05); transition:border-color .3s, box-shadow .3s;
}
.modal-body .form-control:focus { border-color:#4CB07A; box-shadow:0 0 0 3px rgba(76,176,122,.25); }
.modal-body label.form-label { font-weight:500; color:#444; font-size:13px; margin-bottom:4px; }
.modal-footer { background:#f5f7fa; padding:18px 24px; border-bottom-left-radius:14px; border-bottom-right-radius:14px; display:flex; justify-content:flex-end; gap:10px; }

/* ====== Utilidades ====== */
.text-danger { color:#f44336; font-size:13px; }

/* ====== Responsividade ====== */
@media (max-width: 768px) {
  .modal-body .row.g-3 > .col-md-6 { flex:1 1 100%; }
  .row.g-3 > .col-md-4, .row.g-3 > .col-md-6, .row.g-3 > .col-md-12 { flex:1 1 100%; }
}

/*CAMPO DE FILTROS*/
/* =======================
   Tokens / base
======================= */
:root{
  --brand:#4CB07A;
  --accent:#83ceee;
  --ink:#1f2937;
  --muted:#6b7280;
  --panel:#ffffff;
  --panel-border:#e5e7eb;
  --shadow:0 6px 14px rgba(0,0,0,.06);
  --radius:14px;
}

/* Painel dos filtros (cart) */
.filters-block{
  background:var(--panel);
  border:1px solid var(--panel-border);
  border-radius:var(--radius);
  padding:22px 24px;
  box-shadow:var(--shadow);
}

/* Título interno "Filtros" */
.filters-block .filters-title{
  display:flex; align-items:center; gap:10px;
  font-weight:600; color:var(--ink); margin:0 0 14px 0;
}
.filters-block .filters-title::before{
  content:""; width:6px; height:18px; border-radius:3px; background:var(--brand);
}

/* Inputs */
.filters-block .form-label{ font-weight:500; color:#444; }
.filters-block .form-control{
  height:44px;
  border:1px solid #dfe3ea;
  border-radius:10px;
  box-shadow:0 1px 2px rgba(0,0,0,.03);
}
.filters-block .form-control:focus{
  border-color:var(--accent);
  box-shadow:0 0 0 3px rgba(131,206,238,.25);
  outline:0;
}

/* barra dos botões dos filtros */
.filters-btnbar{
  display:flex;
  gap:12px;
  align-items:center;
  margin-top:10px;
justify-content: space-around;
}

/* botão “flat”, minimalista */
.btn-filter{
  appearance:none;
  background:#fff;
  color:#2b3543;
  border:1px solid #E7ECF2;
  padding:10px 18px;
  height:44px;          /* altura consistente */
  min-width:140px;      /* todos com largura boa */
  border-radius:12px;   /* cantos suaves */
  font-weight:500;
  letter-spacing:.2px;
  box-shadow:0 1px 2px rgba(16,24,40,.06);
  transition:background .2s,border-color .2s,box-shadow .2s,transform .06s;
}

.btn-filter:hover{
  background:#fbfdff;
  border-color:#cbd5e1;
  box-shadow:0 4px 12px rgba(16,24,40,.08);
}

.btn-filter:active{ 
  transform:translateY(1px);
  box-shadow:0 2px 6px rgba(16,24,40,.12);
}

.btn-filter:focus-visible{
  outline:none;
  box-shadow:0 0 0 4px rgba(131,206,238,.25); /* foco acessível */
}

/* variante destaque (verde) – deixe só 1 ou use em todos se quiser */
.btn-filter-primary{
  background:var(--brand, #4CB07A);
  border-color:var(--brand, #4CB07A);
  color:#fff;
  box-shadow:0 6px 14px rgba(76,176,122,.20);
}
.btn-filter-primary:hover{
  background:#3A9E68;
  border-color:#3A9E68;
  box-shadow:0 10px 18px rgba(76,176,122,.22);
}

/* responsivo: empilha e deixa largura total no celular */
@media (max-width:768px){
  .filters-btnbar .btn{ width:100%; }
}

/* =======================
   Grid moderno
======================= */
./* 🔓 Aumenta a largura do container principal desta página */
.container.py-4{
  max-width: 1600px !important;   /* ajuste p/ 1500, 1700 etc. */
}

/* Em telas bem grandes dá mais folga ainda (opcional) */
@media (min-width: 1800px){
  .container.py-4{ max-width: 1720px !important; }
}

/* 🧱 Painel ocupa toda a largura do container */
.form-panel{ width: 100%; }

/* 🧭 Grid mais largo (sem espremer colunas) */
.table-modern{
  width: 100%;
  background: var(--panel);
  border: 1px solid var(--panel-border);
  border-radius: var(--radius);
  overflow: hidden;              /* cantos arredondados no header */
  box-shadow: var(--shadow);
  font-size: .96rem;
  min-width: 1400px;             /* base do grid — ajuste se quiser */
}

/* Cabeçalho e células */
.table-modern thead th{
  background: #f6f8fb;
  color: #4b5563;
  font-weight: 600;
  padding: 16px 18px;
  border-bottom: 1px solid var(--panel-border);
}
.table-modern td{
  padding: 16px 18px;
  border-top: 1px solid var(--panel-border);
  vertical-align: middle;
}
.table-modern tbody tr:hover{ background:#f9fdfb; }

/* Nome NÃO quebra linha (email pode quebrar) */
.table-modern th,
.table-modern td{ white-space: nowrap; }
.table-modern td:nth-child(5){ white-space: normal; }  /* E-mail */

/* Botão editar com área maior */
.table-modern .btn-editar{
  background: var(--brand); color: #fff; border: none;
  padding: 8px 12px; border-radius: 999px;
}
.table-modern .btn-editar:hover{ background:#3A9E68; }

/* Respiro acima do grid */
#<%= PanelResultado.ClientID %>{ margin-top: 16px; }

/* Responsivo */
@media (max-width: 992px){
  .filters-block{ padding: 16px; }
  .table-modern{ font-size: .92rem; }
}

</style>

<script type="text/javascript">
    // ❌ ABRE MODAL DE INATIVAR
    function abrirModalInativar(codPessoa, nome) {
        // Preenche o hidden field e o label
        $('#<%= hfCodPessoaInativa.ClientID %>').val(codPessoa);
       document.getElementById("lblNomeUsuarioInativa").innerText = nome;

       // Bootstrap 5 modal (sem depender de jQuery antigo)
       const myModal = new bootstrap.Modal(document.getElementById('modalInativarUsuario'));
       myModal.show();
   }

    // ✅ FUNÇÃO ÚNICA DE MÁSCARAS
    function aplicarMascaras() {
        const campoCPF = $('#<%= txtDocCPF.ClientID %>');
        const campoRG = $('#<%= txtDocRG.ClientID %>');
        const campoBuscaCPF = $('#<%= txtBuscaCPF.ClientID %>');

        if (campoCPF.length)      campoCPF.unmask().mask('000.000.000-00', { reverse: true });
        if (campoRG.length)       campoRG.unmask().mask('00.000.000-0');
        if (campoBuscaCPF.length) campoBuscaCPF.unmask().mask('000.000.000-00', { reverse: true });

        const phoneBehavior = function (val) {
            const nums = val.replace(/\D/g, '').slice(0, 11);
            return nums.length === 11 ? '(00) 00000-0000' : '(00) 0000-00009';
        };
        const phoneOptions = {
            onKeyPress: function (val, e, field, options) {
                field.mask(phoneBehavior.apply({}, arguments), options);
            }
        };

        const tel1 = $('#<%= txtTelefone1.ClientID %>');
        const tel2 = $('#<%= txtTelefone2.ClientID %>');
        const tel3 = $('#<%= txtTelefone3.ClientID %>');

        if (tel1.length) tel1.attr('maxlength', 15).unmask().mask(phoneBehavior, phoneOptions);
        if (tel2.length) tel2.attr('maxlength', 15).unmask().mask(phoneBehavior, phoneOptions);
        if (tel3.length) tel3.attr('maxlength', 15).unmask().mask(phoneBehavior, phoneOptions);
    }

    // ✅ APLICA MÁSCARAS QUANDO A PÁGINA CARREGA
    $(document).ready(function () {
        aplicarMascaras();
    });

    // ✅ (Opcional) Se usar UpdatePanel, re-aplica após postback parcial
    if (typeof (Sys) !== "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager) {
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            aplicarMascaras();
        });
    }
</script>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

        <div class="container py-4">
        <h2 class="titulo-gestao" runat="server" id="lblTitGestao">Gestão de Colaboradores</h2>

       <%-- <div class="container-gestao">
            <asp:Button ID="btnIncluirUsuario" runat="server" Text="Incluir Usuário" CssClass="btn-gestao btn-incluir" OnClick="btnIncluirUsuario_Click" />
            <asp:Button ID="btnConsultarUsuario" runat="server" Text="Consultar Usuário" CssClass="btn-gestao btn-consultar" OnClick="btnConsultarUsuario_Click" />
         <asp:Button ID="btnDesativarUsuario" runat="server" Text="Desativar Usuário" CssClass="btn-gestao btn-desativar" OnClick="btnDesativarUsuario_Click" />
        </div>--%>

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
                        <label>CPF *<asp:TextBox ID="txtDocCPF" runat="server" CssClass="form-control" MaxLength="11" placeHolder="Insira apenas números" /> </label>

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
                <asp:Button ID="btnSalvarUsuario" runat="server" Text="Salvar Usuário" CssClass="btn btn-success"
                    ValidationGroup="Cadastro" OnClick="btnSalvarUsuario_Click" />
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
                          <a class="btn-editar" href='<%# "employeeEditMedic.aspx?id=" + Eval("CodPessoa") %>'>
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
                onclick='abrirModalInativar(
                    <%# Eval("CodPessoa") %>,
                    "<%# System.Web.HttpUtility.JavaScriptStringEncode(Eval("NomeCompleto") as string, true) %>"
                )'>
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


    <script type="text/javascript">
        Sys.Application.add_load(function () {
            if ($('#<%= PanelCadastro.ClientID %>').is(':visible')) {
                aplicarMascaras();
            }
        });
    </script>

    </div>

</asp:Content>
