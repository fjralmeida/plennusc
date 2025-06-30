<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="profile.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
    .perfil-card {
      background: #fff;
      border-radius: 1rem;
      padding: 2rem;
      box-shadow: 0 2px 12px rgba(131, 206, 238, 0.25); /* leve azul claro */
    }

        .perfil-titulo {
      font-weight: bold;
      font-size: 1.5rem;
      color: #4CB07A; /* título verde */
      margin-bottom: 1.5rem;
    }
    .avatar-img
    { width:140px;
      height:140px; border-radius:50%;
      object-fit:cover;
      border:2px groove#c06ed4;
      transition:.3s; }
   
      .avatar-img:hover {
      transform: scale(1.05);
      box-shadow: 0 0 0 4px #DC8689; /* rosa hover */
    }

   .avatar-img:hover {
   transform: scale(1.05);
   box-shadow: 0 0 0 4px #dc8689; /* rosa */
 }

 .file-upload {
   position: absolute;
   top: 0; left: 0;
   width: 100%; height: 100%;
   opacity: 0; cursor: pointer;
 }

 .upload-label {
   display: block;
   margin-top: 0.5rem;
   font-size: 0.875rem;
   color: #666;
 }


 .form-control:focus {
   border-color: #83ceee;
   box-shadow: 0 0 0 0.2rem rgba(131, 206, 238, 0.4);
 }

 .btn-primary {
   background-color: #4CB07A;
   border-color: #4CB07A;
 }

 .btn-primary:hover {
   background-color: #3e9b68;
   border-color: #3e9b68;
 }

  </style>

    <script type="text/javascript">
      function previewFoto() {
        var fileInput = document.getElementById("fuFoto");
        var imgPreview = document.getElementById("imgFotoPerfil");

        if (fileInput.files && fileInput.files[0]) {
          var reader = new FileReader();

          reader.onload = function (e) {
            imgPreview.src = e.target.result;
          };

          reader.readAsDataURL(fileInput.files[0]);
        }
      }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <div class="container py-4">
    <div class="perfil-card mx-auto">
      <div class="row align-items-center g-4">
        <div class="col-md-4 text-center position-relative">
        <asp:Image ID="imgFotoPerfil" runat="server" CssClass="avatar-img" ClientIDMode="Static" />
<asp:FileUpload ID="fuFoto" runat="server" CssClass="file-upload" ClientIDMode="Static" onchange="previewFoto()" />
          <label for="<%= fuFoto.ClientID %>" class="upload-label">Clique para trocar foto</label>
          <asp:Button ID="btnAlterarFoto" runat="server" CssClass="btn btn-success mt-3" Text="Salvar Foto" OnClick="btnAlterarFoto_Click1" />
        </div>

        <div class="col-md-8">
          <h4 class="perfil-titulo">Perfil do Usuário</h4>

          <!-- DADOS PESSOAIS -->
          <h5 class="mt-3 mb-2 text-muted fw-bold">Dados Pessoais</h5>
          <div class="row g-3">
            <div class="col-md-6">
              <label class="form-label" for="txtNome">Nome</label>
              <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" ReadOnly="true" />
            </div>
            <div class="col-md-6">
              <label class="form-label" for="txtSobrenome">Sobrenome</label>
              <asp:TextBox ID="txtSobrenome" runat="server" CssClass="form-control" ReadOnly="true" />
            </div>
            <div class="col-md-4">
              <label class="form-label" for="txtApelido">Apelido</label>
              <asp:TextBox ID="txtApelido" runat="server" CssClass="form-control" ReadOnly="true" />
            </div>
            <div class="col-md-4">
              <label class="form-label" for="txtSexo">Sexo</label>
              <asp:TextBox ID="txtSexo" runat="server" CssClass="form-control" ReadOnly="true" />
            </div>
            <div class="col-md-4">
              <label class="form-label" for="txtDataNasc">Data de Nascimento</label>
              <asp:TextBox ID="txtDataNasc" runat="server" CssClass="form-control" ReadOnly="true" />
            </div>
          </div>

          <!-- CONTATO & CARGO -->
          <h5 class="mt-4 mb-2 text-muted fw-bold" >Contato & Cargo</h5>
          <div class="row g-3">
            <div class="col-md-6">
              <label class="form-label" for="txtEmail">E‑mail</label>
              <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" ReadOnly="true" />
            </div>
            <div class="col-md-6">
              <label class="form-label" for="txtTelefone1">Telefone</label>
              <asp:TextBox ID="txtTelefone1" runat="server" CssClass="form-control" ReadOnly="true" />
            </div>
            <div class="col-md-6">
              <label class="form-label" for="txtCpf">CPF</label>
              <asp:TextBox ID="txtCpf" runat="server" CssClass="form-control" ReadOnly="true" />
            </div>
            <div class="col-md-6">
              <label class="form-label" for="txtCargo">Cargo</label>
              <asp:TextBox ID="txtCargo" runat="server" CssClass="form-control" ReadOnly="true" />
            </div>
          </div>

        </div>
      </div>
    </div>
  </div>
</asp:Content>

