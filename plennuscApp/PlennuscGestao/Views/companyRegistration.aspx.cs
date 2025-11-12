using Plennusc.Core.Models.ModelsGestao.modelsCompany;
using Plennusc.Core.Service.ServiceGestao.company;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class companyRegistration : System.Web.UI.Page
    {
        private CompanyService _companyService;
        protected void Page_Load(object sender, EventArgs e)
        {
            _companyService = new CompanyService();

            if (!IsPostBack)
            {
                if (Session["CodUsuario"] == null)
                {
                    Response.Redirect("~/SignIn.aspx");
                    return;
                }
            }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsValid) return;

                var company = new CompanyModel
                {
                    RazaoSocial = txtRazaoSocial.Text.Trim(),
                    NomeFantasia = txtNomeFantasia.Text.Trim(),
                    CNPJ = txtCNPJ.Text.Trim(),
                    LiberaAcesso = chkLiberaAcesso.Checked,
                    Ativo = chkAtivo.Checked,
                };

                int codEmpresa = _companyService.SalvarEmpresa(company);

                if (codEmpresa > 0)
                {
                    MostrarMensagemSucesso("Empresa cadastrada com sucesso!");

                    // Limpar formulário após sucesso
                    LimparFormulario();
                }
                else
                {
                    MostrarMensagem("Erro ao cadastrar empresa. Tente novamente.", "error");
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem(ex.Message, "error");
            }
        }
        protected void cvCNPJ_ServerValidate(object source, ServerValidateEventArgs args)
        {
            try
            {
                string cnpj = txtCNPJ.Text.Trim();
                args.IsValid = _companyService.ValidarCNPJ(cnpj);

                if (!args.IsValid)
                {
                    MostrarMensagem("CNPJ inválido. Verifique os dígitos.", "warning");
                }
            }
            catch
            {
                args.IsValid = false;
            }
        }

        private void MostrarMensagemSucesso(string mensagem)
        {
            string script = $@"
                Swal.fire({{
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Sucesso',
                    text: '{mensagem.Replace("'", "\\'")}',
                    showConfirmButton: false,
                    timer: 3000,
                    timerProgressBar: true
                }});
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastSucesso", script, true);
        }

        private void MostrarMensagem(string mensagem, string tipo = "success")
        {
            string titulo;
            switch (tipo.ToLower())
            {
                case "success":
                    titulo = "Sucesso";
                    break;
                case "error":
                    titulo = "Erro";
                    break;
                case "warning":
                    titulo = "Atenção";
                    break;
                case "info":
                    titulo = "Informação";
                    break;
                default:
                    titulo = "Mensagem";
                    break;
            }

            string script = $@"
                Swal.fire({{
                    toast: true,
                    position: 'top-end',
                    icon: '{tipo}',
                    title: '{titulo}',
                    text: '{mensagem.Replace("'", "\\'")}',
                    showConfirmButton: false,
                    timer: 4000,
                    timerProgressBar: true
                }});
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastMsg", script, true);
        }

        private void LimparFormulario()
        {
            txtRazaoSocial.Text = "";
            txtNomeFantasia.Text = "";
            txtCNPJ.Text = "";
            chkLiberaAcesso.Checked = true;
            chkAtivo.Checked = true;
        }
    }
}