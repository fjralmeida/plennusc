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
                    DataTable dt = settings.GetSettingsLogin(codPessoa);
                    if (dt?.Rows.Count > 0)
                    {
                        txtLoginAtual.Text = dt.Rows[0]["UsrNomeLogin"].ToString();
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
                string oldLogin = dtLogin.Rows[0]["UsrNomeLogin"].ToString().Trim();
                string currentLogin = txtLoginAtual.Text.Trim();
                string newLogin = txtNovoLogin.Text.Trim();

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

                // Aqui colocaremos a função de atualização (depois)
                // Ex: settingsConsultationLogin.AtualizarLogin(novoLogin);

                ScriptManager.RegisterStartupScript(this, GetType(), "loginAlterado", "Swal.fire('Sucesso', 'Login alterado com sucesso.', 'success');", true);
            }
            else
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "erroConsulta", "Swal.fire('Erro', 'Não foi possível recuperar os dados do login.', 'error');", true);
            }
        }

        protected void btnAlterarSenha_Click(object sender, EventArgs e)
        {
            PrivacySettings settingsConsultationPassword = new PrivacySettings();
            DataTable dtPassword = settingsConsultationPassword.GetSettingsPassword();

            if(dtPassword != null && dtPassword.Rows.Count > 0) 
            {


                string senhaAtual = txtSenhaAtual.Text.Trim();
                string novaSenha = txtNovaSenha.Text.Trim();
                string confirmar = txtConfirmarSenha.Text.Trim();

                if (novaSenha != confirmar)
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "erroSenha", "Swal.fire('Erro', 'As senhas não coincidem.', 'error');", true);
                    return;
                }

                // TODO: Verificar senha atual no banco e atualizar
                ScriptManager.RegisterStartupScript(this, GetType(), "sucessoSenha", "Swal.fire('Sucesso', 'Senha alterada com sucesso.', 'success');", true);
            }
        }

    }
}