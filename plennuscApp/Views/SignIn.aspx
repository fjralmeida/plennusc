<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="appWhatsapp.Views.SignIn" %>

<!DOCTYPE html>
<html lang="pt-br">
<head runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    <title>Vallor</title>

    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/Css/styleSignIn.css") %>" />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;600&display=swap" rel="stylesheet" />
</head>

<body>
    <!-- IMAGEM DE FUNDO NO FRONT -->
    <img src="../Uploads/Fundo_Login.jpg" 
         style="position:fixed; top:0; left:0; width:100%; height:100%; object-fit:cover; z-index:-1;" 
         alt="Fundo Login" />

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
