<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SignIn.aspx.cs" Inherits="appWhatsapp.Views.SignIn" %>

<!DOCTYPE html>
<html lang="pt-BR">

<head runat="server">
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Login - Plennus Connect</title>
    <link rel="preconnect" href="https://fonts.googleapis.com">
    <link rel="preconnect" href="https://fonts.gstatic.com">
    <link
        href="https://fonts.googleapis.com/css2?family=Fraunces:opsz,wght@9..144,400;9..144,500;9..144,600&family=Inter:wght@400;500;600;700&display=swap"
        rel="stylesheet">
    <link rel="stylesheet" href="<%= ResolveUrl("~/Content/Css/styleSignIn.css") %>" />
</head>

<body>

    <div class="layout">

        <!-- ============ Esquerda ============ -->
        <div class="brand">
            <div class="brand-top">
                <div class="logo-mark">
                    <span class="pulse-ring"></span>
                    <svg width="24" height="24" viewBox="0 0 24 24" fill="none">
                        <path
                            d="M12 3C9.5 3 7.6 5 7.6 7.5V11.2C7.6 11.9 7.3 12.6 6.8 13.1L5.6 14.4C4.9 15.1 5.4 16.3 6.4 16.3H17.6C18.6 16.3 19.1 15.1 18.4 14.4L17.2 13.1C16.7 12.6 16.4 11.9 16.4 11.2V7.5C16.4 5 14.5 3 12 3Z"
                            stroke="#52C99A" stroke-width="1.6" stroke-linejoin="round" fill="rgba(31,169,122,0.14)" />
                        <path d="M9.8 19C10.2 19.9 11 20.5 12 20.5C13 20.5 13.8 19.9 14.2 19" stroke="#1FA97A"
                            stroke-width="1.6" stroke-linecap="round" />
                </div>
                <p class="brand-name">Plennus <span>Connect</span></p>
            </div>

            <div class="brand-mid">
                <div class="eyebrow">ERP de Gestão Empresarial</div>
                <h1 class="headline">O cérebro da sua <br><em>gestão.</em></h1>
                <p class="sub">
                    Centralize processos, clientes, operadoras,
                    financeiro, contratos e indicadores
                    em uma única plataforma.
                </p>
            </div>

            <div>
                <div class="ecg-wrap">
                    <svg viewBox="0 0 400 48" preserveAspectRatio="none">
                        <path class="ecg-line" d="M0,24 L60,24 L78,24 L88,6 L98,42 L108,16 L118,24 L140,24
                                     L160,24 L178,24 L188,6 L198,42 L208,16 L218,24 L240,24
                                     L260,24 L278,24 L288,6 L298,42 L308,16 L318,24 L400,24" />
                    </svg>
                </div>
                <div class="brand-bottom">
                    <span>Plennus Connect · ERP de Gestão Empresarial</span>
                    <span><strong>+100</strong> já utilizando o sistema</span>
                </div>
            </div>
        </div>

        <!-- ============ Direita ============ -->
        <div class="panel">
            <div class="form-card">
                <div class="form-eyebrow">Acesso restrito</div>
                <h2 class="form-title">Entrar no painel</h2>
                <p class="form-desc">Use suas credenciais de equipe para acessar.</p>

                <!-- FORMULÁRIO -->
                <form id="form2" runat="server">
                    <div class="form-group">

                        <!-- Campo de Usuário -->
                        <div class="field">
                            <div class="input-shell">
                                <span class="input-icon">
                                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none">
                                        <path d="M12 12a4.5 4.5 0 1 0 0-9 4.5 4.5 0 0 0 0 9Z" stroke="currentColor"
                                            stroke-width="1.6" />
                                        <path d="M4 20.5c1.4-3.6 4.6-5.5 8-5.5s6.6 1.9 8 5.5" stroke="currentColor"
                                            stroke-width="1.6" stroke-linecap="round" />
                                    </svg>
                                </span>
                                <asp:TextBox ID="TextBoxEmail" 
                                    runat="server"
                                    TextMode="SingleLine"
                                    inputmode="email" 
                                    autocomplete="email"
                                    placeholder="Seu usuário"
                                    CssClass="asp-textbox" />
                            </div>
                        </div>

                        <!-- Campo de Senha -->
                        <div class="field">
                            <div class="input-shell">
                                <span class="input-icon">
                                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none">
                                        <rect x="5" y="11" width="14" height="9" rx="2" stroke="currentColor"
                                            stroke-width="1.6" />
                                        <path d="M8 11V8a4 4 0 1 1 8 0v3" stroke="currentColor" stroke-width="1.6"
                                            stroke-linecap="round" />
                                    </svg>
                                </span>
                                <asp:TextBox ID="TextBoxPassword" 
                                    runat="server" 
                                    TextMode="Password" 
                                    placeholder="Sua senha"
                                    CssClass="asp-textbox" />

                                <button type="button" class="toggle-pw" aria-label="Mostrar senha"
                                    onclick="togglePassword()">
                                    <svg width="18" height="18" viewBox="0 0 24 24" fill="none">
                                        <path d="M1.5 12S5 5 12 5s10.5 7 10.5 7-3.5 7-10.5 7S1.5 12 1.5 12Z"
                                            stroke="currentColor" stroke-width="1.6" />
                                        <circle cx="12" cy="12" r="3" stroke="currentColor" stroke-width="1.6" />
                                    </svg>
                                </button>
                            </div>
                        </div>

                        <!-- Dropdown -->
                        <div class="field">
                            <asp:DropDownList
                                ID="ddlSistema"
                                runat="server"
                                CssClass="dropdown">
                            </asp:DropDownList>
                        </div>

                        <!-- Botão de Login -->
                        <asp:Button
                            ID="ButtonSignIn"
                            runat="server"
                            CssClass="submit-btn"
                            OnClick="ButtonSignIn_Click"
                            Text="Acessar"
                           />

                        <!-- Label de Erro -->
                        <asp:Label 
                            ID="LabelErro" 
                            runat="server" 
                            ForeColor="Red" 
                            CssClass="error-message" 
                            Visible="false" />

                    </div>
                </form>

                <p class="form-footer">Dúvidas de acesso? Fale com o administrador do sistema.</p>
            </div>
        </div>

    </div>

    <script type="text/javascript">
        function togglePassword() {
            var senhaInput = document.getElementById('<%= TextBoxPassword.ClientID %>');
            var btn = document.querySelector('.toggle-pw');
            
            if (senhaInput.type === 'password') {
                senhaInput.type = 'text';
                btn.setAttribute('aria-label', 'Ocultar senha');
                btn.innerHTML = '<svg width="18" height="18" viewBox="0 0 24 24" fill="none">' +
                    '<path d="M1.5 12S5 5 12 5s10.5 7 10.5 7-3.5 7-10.5 7S1.5 12 1.5 12Z" stroke="currentColor" stroke-width="1.6" />' +
                    '<path d="M3 3L21 21" stroke="currentColor" stroke-width="1.6" stroke-linecap="round" />' +
                    '<circle cx="12" cy="12" r="3" stroke="currentColor" stroke-width="1.6" />' +
                    '</svg>';
            } else {
                senhaInput.type = 'password';
                btn.setAttribute('aria-label', 'Mostrar senha');
                btn.innerHTML = '<svg width="18" height="18" viewBox="0 0 24 24" fill="none">' +
                    '<path d="M1.5 12S5 5 12 5s10.5 7 10.5 7-3.5 7-10.5 7S1.5 12 1.5 12Z" stroke="currentColor" stroke-width="1.6" />' +
                    '<circle cx="12" cy="12" r="3" stroke="currentColor" stroke-width="1.6" />' +
                    '</svg>';
            }
        }
    </script>

</body>

</html>