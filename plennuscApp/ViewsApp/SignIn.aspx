<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="appWhatsapp.Views.SignIn" %>

<!DOCTYPE html>
<html lang="pt-BR">
<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Login - Plennus Connect</title>
    <link rel="icon" type="image/png" sizes="180x180" href="<%= ResolveUrl("~/Uploads/logo_Plennus.png") %>" />

    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com">

    <link href="https://fonts.googleapis.com/css2?family=Manrope:wght@300;400;500;600;700;800&display=swap"
        rel="stylesheet">

    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/Css/styleSignIn.css") %>" />

    <script src="https://unpkg.com/lucide@latest"></script>
</head>

    <body>

    <div class="background">
        <div class="circle circle-1"></div>
        <div class="circle circle-2"></div>
        <div class="grid"></div>
    </div>

    <main class="container">

        <!-- LADO ESQUERDO -->
        <section class="branding">
            <div class="branding-content">
                <div class="logo">
                   <!-- <div class="logo-icon">
                        <div class="hexagon"></div>
                    </div> -->
                    <img src="../Uploads/logo_plennus_cortado.png"/>
                    <div>
                        <h1>Plennus Connect</h1>
                        <span>ERP de Gestão Empresarial</span>
                    </div>
                </div>

                <h2>
                    O cérebro da sua gestão.
                </h2>

                <p>
                    Centralize processos, clientes, operadoras,
                    financeiro, contratos e indicadores
                    em uma única plataforma.
                </p>

                <div class="features">
                    <div class="feature">
                        <i data-lucide="building-2"></i>
                        <span>Gestão Empresarial</span>
                    </div>

                    <div class="feature">
                        <i data-lucide="calculator"></i>
                        <span>Gestão Financeira</span>
                    </div>

                    <div class="feature">
                        <i data-lucide="shield-plus"></i>
                        <span>Gestão Médica</span>
                    </div>

                    <div class="feature">
                        <i data-lucide="users"></i>
                        <span>Gestão de Ouvidoria</span>
                    </div>
                </div>

                <div class="company">
                    <div>
                        <small>
                            Plennus Connect é o núcleo operacional da empresa <strong>Vallor Benefícios</strong>.
                        </small>
                    </div>
                </div>

            </div>
        </section>

        <!-- LOGIN -->
            <section class="login-side">
                <div class="login-card">
                    <div class="login-header">
                        <!-- <div class="mini-logo">
                            <div class="mini-hexagon"></div>
                        </div> --> 

                        <img src="../Uploads/logo_plennus_sb.png" class="logo-login" ID="imgLogo"/>

                        <h2>Acesse sua conta</h2>

                        <p>
                            Faça login para acessar o
                            Plennus Connect.
                        </p>
                    </div>

                    <form id="form2" runat="server">

                        <div class="input-group">
                            <div class="input">
                                <i data-lucide="user"></i>
                                <asp:TextBox ID="TextBoxEmail" 
                                 runat="server"
                                 TextMode="SingleLine"
                                 inputmode="email" autocomplete="email"
                                 placeholder="Digite seu nome de usuário" />
                            </div>
                        </div>

                        <div class="input-group">
                            <div class="input">
                                <i data-lucide="lock"></i>
                                <asp:TextBox ID="TextBoxPassword" 
                                 runat="server" 
                                 TextMode="Password" 
                                 placeholder="Digite sua senha" />

                                <button
                                    type="button"
                                    id="togglePassword">
                                    <i data-lucide="eye"></i>
                                </button>
                            </div>
                        </div>

                       <div class="input-group">
                            <asp:DropDownList
                            ID="ddlSistema"
                            runat="server"
                            CssClass="dropdown">
                            </asp:DropDownList>
                       </div>

                       <asp:Button
                            ID="ButtonSignIn"
                            runat="server"
                            CssClass="login-button"
                            OnClick="ButtonSignIn_Click"
                            Text="Entrar">
    
                        </asp:Button>

                        <asp:Label ID="LabelErro" runat="server" ForeColor="Red" CssClass="error-message" Visible="false" />

                    </form>

                   <div class="version">
                        <hr />
                        <p>Versão 1.0.0</p>
                   </div>
                </div>
            </section>
    </main>

    <script>
        lucide.createIcons();

        const btn = document.getElementById("togglePassword");
        const password = document.getElementById("TextBoxPassword");

        btn.addEventListener("click", () => {

            if (password.type === "password") {

                password.type = "text";

                btn.innerHTML =
                    '<i data-lucide="eye-off"></i>';

            } else {

                password.type = "password";

                btn.innerHTML =
                    '<i data-lucide="eye"></i>';

            }

            lucide.createIcons();

                });
    </script>

</body>

</html>