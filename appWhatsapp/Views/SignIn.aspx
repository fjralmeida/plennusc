<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="appWhatsapp.Views.SignIn" %>

<!DOCTYPE html>
<html lang="en">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Vallor</title>

    <%-- Correto para Web Forms --%>
    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/Css/styleSignIn.css") %>" />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;600&display=swap" rel="stylesheet" />
</head>

<body>
  <form id="form1" runat="server">
    <div class="container">
        <div class="card">
            <div class="left">
                <img src="../Content/Img/imgLogin.jpeg" alt="Ilustração de Autenticação" />
            </div>
            <div class="right">
                <div class="logo">
                    <asp:Image ID="imgLogo" runat="server" AlternateText="Logo da Empresa" />
                </div>
      <%--          <h1><asp:Label ID="lblNomeEmpresa" runat="server" Text="Minha Empresa"></asp:Label></h1>--%>

              
                <p class="subtitle">Acesse sua conta</p>

                <div class="divider">ou use seu e-mail</div>

                <label>Endereço de E-mail</label>
                <asp:TextBox ID="TextBoxEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="nome@exemplo.com" />

                <label>Senha</label>
                <asp:TextBox ID="TextBoxPassword" runat="server" CssClass="form-control" TextMode="Password" placeholder="Digite sua senha" />

                <asp:Button ID="ButtonSignIn" runat="server" CssClass="signin" Text="Entrar" OnClick="ButtonSignIn_Click" />

                <asp:Label ID="LabelErro" runat="server" ForeColor="Red" CssClass="error-message" Visible="false" />
            </div>
        </div>
    </div>
  </form>
</body>
</html>
