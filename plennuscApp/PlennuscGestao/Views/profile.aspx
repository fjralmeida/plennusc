<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="profile.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.profile" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Perfil Gestão</title>
    <!-- jQuery -->
    <script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />

    <link href="../../Content/Css/projects/gestao/structuresCss/profile.css" rel="stylesheet" />
    <script src="../../Content/Css/js/projects/gestaoJs/structuresJs/profile.js"></script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container py-4">
        <div class="avatar-wrapper">
            <asp:Image ID="imgFotoPerfil" runat="server" CssClass="avatar-img" ClientIDMode="Static" />
            <asp:FileUpload ID="fuFoto" runat="server" CssClass="file-upload" ClientIDMode="Static" onchange="previewFoto()" />
            <label class="upload-label" for="fuFoto">Clique na imagem para trocar</label>
            <asp:Button ID="btnAlterarFoto" runat="server" CssClass="btn-salvar-foto mt-2" Text="Salvar Foto" OnClick="btnAlterarFoto_Click1" />


            <!-- Label de erro -->
            <asp:Label ID="lblErro" runat="server" CssClass="erro-upload-foto" Visible="false" />
        </div>

        <div class="w-100">
            <asp:Label ID="lblUsuario" runat="server" CssClass="perfil-titulo"></asp:Label>
        </div>
        <div class="section-block bg-white-section">
            <h5 class="mb-3 text-muted fw-bold">Dados Pessoais</h5>

            <!-- Linha 1: Nome, Sobrenome, Apelido -->
            <div class="row g-2">
                <div class="col-md-4">
                    <label for="txtNome" class="form-label">Nome</label>
                    <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-4">
                    <label for="txtSobrenome" class="form-label">Sobrenome</label>
                    <asp:TextBox ID="txtSobrenome" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-4">
                    <label for="txtApelido" class="form-label">Apelido</label>
                    <asp:TextBox ID="txtApelido" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
            </div>

            <!-- Linha 2: Sexo, Data de Nascimento -->
            <div class="row g-2 mt-2">
                <div class="col-md-6">
                    <label for="txtSexo" class="form-label">Sexo</label>
                    <asp:TextBox ID="txtSexo" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-6">
                    <label for="txtDataNasc" class="form-label">Data de Nascimento</label>
                    <asp:TextBox ID="txtDataNasc" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
            </div>
        </div>


        <div class="section-block bg-gray-section">
            <h5 class="mb-3 text-muted fw-bold">Documentos Pessoais</h5>
            <div class="row g-3">
                <div class="col-md-6">
                    <label for="txtDocCPF" class="form-label">CPF</label>
                    <asp:TextBox ID="txtDocCPF" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-6">
                    <label for="txtDocRG" class="form-label">RG</label>
                    <asp:TextBox ID="txtDocRG" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
            </div>
        </div>

        <div class="section-block bg-white-section">
            <h5 class="mb-3 text-muted fw-bold">Dados Eleitorais</h5>
            <div class="row row-eleitorais">
                <div class="col-md-4">
                    <label for="txtTitulo" class="form-label">Título de Eleitor</label>
                    <asp:TextBox ID="txtTitulo" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-4">
                    <label for="txtZona" class="form-label">Zona</label>
                    <asp:TextBox ID="txtZona" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-4">
                    <label for="txtSecao" class="form-label">Seção</label>
                    <asp:TextBox ID="txtSecao" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
            </div>
        </div>

        <div class="section-block bg-gray-section">
            <h5 class="mb-3 text-muted fw-bold">Dados Trabalhistas</h5>
            <div class="row g-3">
                <div class="col-md-4">
                    <label for="txtCTPS" class="form-label">CTPS</label>
                    <asp:TextBox ID="txtCTPS" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-4">
                    <label for="txtCTPSSerie" class="form-label">Série</label>
                    <asp:TextBox ID="txtCTPSSerie" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-4">
                    <label for="txtCTPSUf" class="form-label">UF</label>
                    <asp:TextBox ID="txtCTPSUf" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-6">
                    <label for="txtPis" class="form-label">PIS</label>
                    <asp:TextBox ID="txtPis" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-6">
                    <label for="txtMatricula" class="form-label">Matrícula</label>
                    <asp:TextBox ID="txtMatricula" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-6">
                    <label for="txtDataAdmissao" class="form-label">Admissão</label>
                    <asp:TextBox ID="txtDataAdmissao" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-6">
                    <label for="txtDataDemissao" class="form-label">Demissão</label>
                    <asp:TextBox ID="txtDataDemissao" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
            </div>
        </div>

        <!-- FILIAÇÃO -->
        <div class="section-block bg-white-section">
            <h5 class="mt-4 mb-2 text-muted fw-bold">Filiação</h5>
            <div class="row g-3">
                <div class="col-md-6">
                    <label for="txtFiliacao1" class="form-label">Nome da Filiação 1</label>
                    <asp:TextBox ID="txtFiliacao1" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-6">
                    <label for="txtFiliacao2" class="form-label">Nome da Filiação 2</label>
                    <asp:TextBox ID="txtFiliacao2" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
            </div>
        </div>

        <!-- CONTATO & CARGO -->
        <div class="section-block bg-gray-section">
            <h5 class="mt-4 mb-2 text-muted fw-bold">Contato & Cargo</h5>
            <div class="row g-3">
                <div class="col-md-6">
                    <label for="txtEmail" class="form-label">E‑mail</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-6">
                    <label for="txtEmailAlt" class="form-label">E‑mail Alternativo</label>
                    <asp:TextBox ID="txtEmailAlt" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-4">
                    <label for="txtTelefone1" class="form-label">Telefone 1</label>
                    <asp:TextBox ID="txtTelefone1" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-4">
                    <label for="txtTelefone2" class="form-label">Telefone 2</label>
                    <asp:TextBox ID="txtTelefone2" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-4">
                    <label for="txtTelefone3" class="form-label">Telefone 3</label>
                    <asp:TextBox ID="txtTelefone3" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-6">
                    <label for="txtCargo" class="form-label">Cargo</label>
                    <asp:TextBox ID="txtCargo" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
                <div class="col-md-6">
                    <label for="txtDepartamento" class="form-label">Departamento</label>
                    <asp:TextBox ID="txtDepartamento" runat="server" CssClass="form-control" ReadOnly="true" />
                </div>
            </div>
            <asp:Label ID="txtCodPessoa" Visible="false" runat="server" class="form-label">Departamento</asp:Label>
        </div>
    </div>
</asp:Content>

