<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="privacySettings.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.privacySettings" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Configurações de privacidade</title>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <link href="../../Content/Css/projects/gestao/structuresCss/privacySettings.css" rel="stylesheet" />
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
