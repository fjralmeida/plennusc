using appWhatsapp.Models.Utils;
using Microsoft.Ajax.Utilities;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.settings;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class privacySettings : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Se não existir CodPessoa na sessão, redireciona para Login
                if (Session["CodPessoa"] == null)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "erroSessao",
                        "Swal.fire('Sessão Expirada', 'Por favor, faça login novamente.', 'warning').then(() => { window.location = 'login.aspx'; });",
                        true);
                }
                else
                {
                    // Se quiser, pode pré-carregar o login atual no campo:
                    int codPessoa = Convert.ToInt32(Session["CodPessoa"]);
                    PrivacySettings settings = new PrivacySettings();
                    DataTable dtLogin = settings.GetSettingsLogin(codPessoa);
                    if (dtLogin?.Rows.Count > 0)
                    {
                        txtLoginAtual.Text = dtLogin.Rows[0]["UsrNomeLogin"].ToString();
                    }                    
                }
            }
        }
        protected void btnAlterarLogin_Click(object sender, EventArgs e)
        {
            int codPessoa = Convert.ToInt32(Session["CodPessoa"]);

            PrivacySettings settingsConsultationLogin = new PrivacySettings();
            DataTable dtLogin = settingsConsultationLogin.GetSettingsLogin(codPessoa);
            if (dtLogin != null && dtLogin.Rows.Count > 0)
            {
                string oldLogin = dtLogin.Rows[0]["UsrNomeLogin"].ToString().Trim(); //login antigo
                int codAutenticaAcesso = Convert.ToInt32(dtLogin.Rows[0]["CodAutenticacaoAcesso"]);

                string currentLogin = txtLoginAtual.Text.Trim();//login atual
                string newLogin = txtNovoLogin.Text.Trim();//novo login

                if (string.IsNullOrWhiteSpace(currentLogin) || string.IsNullOrWhiteSpace(newLogin))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "camposVazios", "Swal.fire('Atenção', 'Preencha todos os campos.', 'warning');", true);
                    return;
                }

                if (currentLogin != oldLogin)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "loginIncorreto", "Swal.fire('Erro', 'O login atual informado não confere com o login registrado.', 'error');", true);
                    return;
                }

                // Atualiza no banco
                settingsConsultationLogin.UpdateUserLogin(codAutenticaAcesso, newLogin);

                ScriptManager.RegisterStartupScript(this, GetType(), "loginAlterado", "Swal.fire('Sucesso', 'Login alterado com sucesso.', 'success');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "erroConsulta", "Swal.fire('Erro', 'Não foi possível recuperar os dados do login.', 'error');", true);
            }
        }

        protected void btnAlterarSenha_Click(object sender, EventArgs e)
        {
            int userId = Convert.ToInt32(Session["CodPessoa"]); // CodPessoa = ID do usuário

            PrivacySettings passwordSettings = new PrivacySettings(); // Classe de consulta/atualização
            DataTable dtPassword = passwordSettings.GetSettingsPassword(userId); // Consulta senha atual do usuário

            if (dtPassword != null && dtPassword.Rows.Count > 0)
            {
                string currentPasswordHashDb = dtPassword.Rows[0]["UsrPasswd"].ToString().Trim(); // Senha atual vinda do banco 
                int authAccessId = Convert.ToInt32(dtPassword.Rows[0]["CodAutenticacaoAcesso"]); // ID de autenticação

                string currentPasswordInputHash = CriptografiaUtil.CalcularHashSHA512(txtSenhaAtual.Text); // Senha digitada

                if (currentPasswordHashDb != currentPasswordInputHash)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "wrongPassword", "Swal.fire('Erro', 'A senha atual informada não confere com a senha registrada.', 'error');", true);
                    return;
                }

                string currentPasswordInput = txtSenhaAtual.Text.Trim(); // Senha atual 
                string newPasswordInput = txtNovaSenha.Text.Trim(); // Nova senha
                string confirmNewPasswordInput = txtConfirmarSenha.Text.Trim(); // Confirmação da nova senha

                if (string.IsNullOrWhiteSpace(currentPasswordInput) || string.IsNullOrWhiteSpace(newPasswordInput))
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "emptyFields", "Swal.fire('Atenção', 'Preencha todos os campos.', 'warning');", true);
                    return;
                }

                if (newPasswordInput != confirmNewPasswordInput)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "passwordMismatch", "Swal.fire('Erro', 'As senhas não coincidem.', 'error');", true);
                    return;
                }

                string newPasswordHash = CriptografiaUtil.CalcularHashSHA512(newPasswordInput); // Criptografa nova senha
                passwordSettings.UpdateUserPassword(authAccessId, newPasswordHash); // Atualiza no banco

                ScriptManager.RegisterStartupScript(this, GetType(), "passwordUpdated", "Swal.fire('Sucesso', 'Senha alterada com sucesso.', 'success');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "queryError", "Swal.fire('Erro', 'Não foi possível recuperar os dados do usuário.', 'error');", true);
            }
        }
    }
}