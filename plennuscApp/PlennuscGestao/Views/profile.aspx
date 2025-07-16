<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="profile.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.profile" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <!-- jQuery -->
<script src="https://code.jquery.com/jquery-3.7.1.min.js"></script>

<!-- SweetAlert2 -->
<script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>


    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />
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

        .perfil-titulo {
            font-size: 24px;
            font-weight: 600;
            color: #4CB07A;
            text-align: center;
            margin: 2rem auto 1.5rem auto;
            position: relative;
            max-width: 100%;
            display: block;
        }

            .perfil-titulo::after {
                content: "";
                width: 60px;
                height: 3px;
                background-color: #83CEEE;
                display: block;
                margin: 0.5rem auto 0 auto;
                border-radius: 2px;
            }





        .avatar-wrapper {
            display: flex;
            flex-direction: column;
            align-items: center;
            margin-bottom: 2rem;
        }

        .avatar-img {
            width: 120px;
            height: 120px;
            border-radius: 50%;
            object-fit: cover;
            border: 2px solid #4CB07A;
            transition: 0.3s;
            cursor: pointer;
            box-shadow: 0 3px 10px rgba(0, 0, 0, 0.08);
        }

            .avatar-img:hover {
                transform: scale(1.05);
            }

        .upload-label {
            font-size: 0.875rem;
            color: #555;
            margin-top: 0.5rem;
        }

        .file-upload {
            display: none;
        }

        .btn-salvar-foto {
            background-color: #c06ed4;
            color: white;
            font-size: 0.875rem;
            padding: 0.45rem 1rem;
            border: none;
            border-radius: 10px;
            margin-top: 0.75rem;
            transition: 0.2s;
        }

            .btn-salvar-foto:hover {
                background-color: #a650b1;
            }

        .form-control:focus {
            border-color: #83CEEE;
            box-shadow: 0 0 0 3px rgba(131, 206, 238, 0.25);
        }

        .section-block {
            background: white;
            padding: 30px;
            border-radius: 18px;
            box-shadow: 0 3px 12px rgba(0,0,0,0.08);
            margin-bottom: 40px;
        }

        .bg-gray-section {
            background-color: #f4f6f9;
        }

        h5 {
            font-size: 16px;
            font-weight: 600;
            color: #444;
            border-left: 4px solid #83CEEE;
            padding-left: 12px;
            margin-bottom: 20px;
        }

        .erro-upload-foto {
            color: #DC8689;
            font-size: 0.875rem;
            text-align: center;
            margin-top: 0.5rem;
        }

        .row.g-3 {
            display: flex;
            flex-wrap: wrap;
            gap: 20px;
        }

         .col-md-4 {
            flex: 1 1 calc(33.33% - 20px);
            min-width: 250px;
        }

        .col-md-6 {
            flex: 1 1 calc(50% - 20px);
            min-width: 250px;
        }


        @media (max-width: 768px) {
            .col-md-4, .col-md-6 {
                flex: 1 1 100%;
            }
        }
    </style>
    <script>
        document.addEventListener("DOMContentLoaded", function () {
            const img = document.getElementById("imgFotoPerfil");
            const file = document.getElementById("fuFoto");
            if (img && file) {
                img.addEventListener("click", function () {
                    file.click();
                });
            }
        });
        function previewFoto() {
            const fileInput = document.getElementById("fuFoto");
            const imgPreview = document.getElementById("imgFotoPerfil");
            if (fileInput.files && fileInput.files[0]) {
                const reader = new FileReader();
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
            <div class="row g-3">
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

