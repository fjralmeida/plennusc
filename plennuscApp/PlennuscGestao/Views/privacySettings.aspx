<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="privacySettings.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.privacySettings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Configurações de privacidade</title>

    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <style>
        * {
            font-family: 'Poppins', sans-serif;
            box-sizing: border-box;
        }

        body {
            background-color: #f2f4f8;
            font-size: 13px;
            color: #333;
        }
        .titulo-pagina {
            font-size: 26px;
            font-weight: 600;
            color: #333;
            text-align: center;
            margin-bottom: 30px;
            padding-top: 8px;
            position: relative;
        }

        .titulo-pagina::after {
            content: "";
            width: 80px;
            height: 4px;
            background-color: #4CB07A;
            display: block;
            margin: 0.5rem auto 0 auto;
            border-radius: 2px;
        }

        .grid-columns {
            display: flex;
            flex-wrap: wrap;
            gap: 32px;
            justify-content: center;
        }

        .card {
            background-color: white;
            padding: 30px;
            border-radius: 12px;
            flex: 1;
            min-width: 340px;
            max-width: 480px;
        }

        .card-title {
            font-size: 18px;
            font-weight: 600;
            margin-bottom: 20px;
            display: flex;
            align-items: center;
        }

        .card-title i {
            margin-right: 10px;
            font-size: 18px;
        }

        .login-title {
            color: #83ceee;
        }

        .senha-title {
            color: #4CB07A;
        }

        .form-label {
            margin-top: 10px;
            font-weight: 500;
        }

        .privacy-settings-page .form-control {
            width: 100%;
            padding: 10px;
            margin-top: 4px;
            border-radius: 8px;
            border: 1px solid #ccc;
            font-size: 13px;
        }

        .btn-salvar {
            width: 100%;
            margin-top: 20px;
            padding: 12px;
            font-weight: 500;
            font-size: 14px;
            border-radius: 25px;
            border: none;
            cursor: pointer;
            transition: 0.3s;
        }

        .btn-login {
            background-color: #83ceee;
            color: white;
        }

        .btn-login:hover {
            background-color: #6ec1e3;
        }

        .btn-senha {
            background-color: #4CB07A;
            color: white;
        }

        .btn-senha:hover {
            background-color: #3a9e65;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="privacy-settings-page">
   <div class="px-4 pt-4">
        <h2 class="titulo-pagina">
            <i class="fa-solid fa-user-shield me-2" style="color:#4CB07A;"></i>
            Configurações & Privacidade
        </h2>

        <div class="grid-columns">
            <!-- BLOCO ALTERAR LOGIN -->
            <div class="card">
                <div class="card-title login-title">
                    <i class="fa-solid fa-user"></i> Alterar Login
                </div>

                <label class="form-label" for="txtLoginAtual">Login atual</label>
                <asp:TextBox ID="txtLoginAtual" runat="server" CssClass="form-control" />

                <label class="form-label" for="txtNovoLogin">Novo login</label>
                <asp:TextBox ID="txtNovoLogin" runat="server" CssClass="form-control" />

                <asp:Button ID="btnAlterarLogin" runat="server" Text="Salvar novo login" CssClass="btn-salvar btn-login" OnClick="btnAlterarLogin_Click" />
            </div>

            <!-- BLOCO ALTERAR SENHA -->
            <div class="card">
                <div class="card-title senha-title">
                    <i class="fa-solid fa-key"></i> Alterar Senha
                </div>

                <label class="form-label" for="txtSenhaAtual">Senha atual</label>
                <asp:TextBox ID="txtSenhaAtual" runat="server" CssClass="form-control" TextMode="Password" />

                <label class="form-label" for="txtNovaSenha">Nova senha</label>
                <asp:TextBox ID="txtNovaSenha" runat="server" CssClass="form-control" TextMode="Password" />

                <label class="form-label" for="txtConfirmarSenha">Confirmar nova senha</label>
                <asp:TextBox ID="txtConfirmarSenha" runat="server" CssClass="form-control" TextMode="Password" />

                <asp:Button ID="btnAlterarSenha" runat="server" Text="Salvar nova senha" CssClass="btn-salvar btn-senha" OnClick="btnAlterarSenha_Click" />
            </div>
        </div>
    </div>
        </div>
</asp:Content>
