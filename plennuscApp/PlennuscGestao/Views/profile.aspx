<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="profile.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /* Container geral */
        .card-container {
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.03);
            padding: 24px;
            margin: 20px;
        }

        /* Título */
        .card-header h4 {
            color: #4CB07A;
            font-size: 20px;
            margin: 0;
        }

        /* Botão Editar */
        #btnEditar {
            font-weight: 600;
            background: #4CB07A;
            border-color: #4CB07A;
            color: white;
            padding: 8px 18px;
            border-radius: 25px;
            transition: 0.2s;
        }

        #btnEditar:hover {
            background-color: #3a9e69;
            border-color: #3a9e69;
        }

        /* Foto de perfil */
        #imgFotoPerfil {
            width: 130px;
            height: 130px;
            object-fit: cover;
            border-radius: 50%;
            border: 3px solid #4CB07A;
            margin-bottom: 8px;
        }

        /* Botão alterar foto */
        #btnAlterarFoto {
            font-size: 12px;
            padding: 6px 14px;
            border-radius: 8px;
        }

        /* Labels */
        .form-label {
            font-weight: 600;
            color: #444;
            font-size: 13px;
            margin-bottom: 4px;
        }

        /* Campos */
        .form-control {
            font-size: 13px;
            padding: 8px 10px;
            border-radius: 8px;
            border: 1px solid #ced4da;
            background-color: #f9f9f9;
            transition: border 0.2s;
        }

        .form-control:focus {
            background-color: #fff;
            border-color: #4CB07A;
            box-shadow: 0 0 0 2px rgba(76, 176, 122, 0.2);
        }

        /* Campos somente leitura */
        input[readonly].form-control {
            background-color: #f0f0f0;
            color: #555;
            cursor: not-allowed;
        }

        /* Espaçamento geral */
        .card-container .row {
            margin-top: 10px;
        }

        .card-container .col-md-6,
        .card-container .col-md-3 {
            margin-bottom: 16px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <div class="container" style="max-width: 960px; margin: auto;">
        <div class="card-container">
            <div class="card-header">
                <h4 class="fw-bold mb-0" style="color: #4CB07A;">Perfil do Usuário</h4>
                <asp:Button ID="btnEditar" runat="server" CssClass="btn btn-info btn-pill" Text="Editar Perfil" />
            </div>

            <div class="row">
                <!-- Foto -->
               <div class="col-md-4 text-center">
                    <asp:Image ID="imgFotoPerfil" runat="server" CssClass="rounded-circle" 
                        Style="width: 140px; height: 140px; object-fit: cover; border: 2px solid #e0e0e0;" />
                    <br />
                    <asp:Button ID="btnAlterarFoto" runat="server" CssClass="btn btn-outline-secondary mt-2 btn-sm" Text="Alterar Foto" OnClick="btnAlterarFoto_Click" />
                    <asp:FileUpload ID="fuFoto" runat="server" CssClass="form-control form-control-sm mt-2" />
                </div>

                <!-- Dados pessoais -->
                <div class="col-md-8">
                    <div class="row g-3">
                        <div class="col-md-6">
                            <label class="form-label">Nome</label>
                            <asp:TextBox ID="txtNome" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Sobrenome</label>
                            <asp:TextBox ID="txtSobrenome" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>

                        <div class="col-md-6">
                            <label class="form-label">Apelido</label>
                            <asp:TextBox ID="txtApelido" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">Sexo</label>
                            <asp:TextBox ID="txtSexo" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="col-md-3">
                            <label class="form-label">Data de Nascimento</label>
                            <asp:TextBox ID="txtDataNasc" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>

                        <div class="col-md-6">
                            <label class="form-label">CPF</label>
                            <asp:TextBox ID="txtCpf" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">E-mail</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>

                        <div class="col-md-6">
                            <label class="form-label">Telefone</label>
                            <asp:TextBox ID="txtTelefone1" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                        <div class="col-md-6">
                            <label class="form-label">Cargo</label>
                            <asp:TextBox ID="txtCargo" runat="server" CssClass="form-control" ReadOnly="true" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
