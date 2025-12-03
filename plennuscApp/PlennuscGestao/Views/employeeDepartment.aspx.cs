using Plennusc.Core.Service.ServiceGestao.department;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.department;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class employeeDepartment : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarDepartamentos();
            }
        }

        protected void btnSalvarDepartamento_Click(object sender, EventArgs e)
        {
            try
            {
                // Criar instância do service
                var departmentService = new CreateDepartmentService();

                // Chamar o método para criar departamento
                var resultado = departmentService.CreateDepartment(
                    nome: txtNomeDepartamento.Text.Trim(),
                    numRamal: txtRamal.Text.Trim(),
                    emailGeral: txtEmail.Text.Trim(),
                    telefone: txtTelefone.Text.Trim()
                );

                if (resultado.Success)
                {
                    // Sucesso
                    LimparCampos();
                    CarregarDepartamentos();

                    // Fechar modal via JavaScript
                    ScriptManager.RegisterStartupScript(this, this.GetType(),
                        "FecharModal", "$('#modalNovoDepartamento').modal('hide');", true);

                    // Mostrar mensagem de sucesso
                    MostrarMensagemSucesso($"{resultado.Message} - ID: {resultado.DepartmentId}");
                }
                else
                {
                    // Erro
                    MostrarMensagemErro(resultado.Message);
                }
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro inesperado: {ex.Message}");
            }
        }

        private void CarregarDepartamentos()
        {
            var departmentService = new CreateDepartmentService();
            DataTable dt = departmentService.GetAllDepartments();

            if (dt != null && dt.Rows.Count > 0)
            {
                gvDepartments.DataSource = dt;
                gvDepartments.DataBind();
            }
            else
            {
                gvDepartments.DataSource = null;
                gvDepartments.DataBind();
            }
        }

        private void LimparCampos()
        {
            txtNomeDepartamento.Text = "";
            txtRamal.Text = "";
            txtEmail.Text = "";
            txtTelefone.Text = "";
        }

        private void MostrarMensagemSucesso(string mensagem)
        {
            string script = $@"
                Swal.fire({{
                    icon: 'success',
                    title: 'Sucesso!',
                    text: '{mensagem.Replace("'", "\\'")}',
                    confirmButtonText: 'OK',
                    customClass: {{ confirmButton: 'btn btn-success' }}
                }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "Sucesso", script, true);
        }

        private void MostrarMensagemErro(string mensagem)
        {
            string script = $@"
                Swal.fire({{
                    icon: 'error',
                    title: 'Erro!',
                    text: '{mensagem.Replace("'", "\\'")}',
                    confirmButtonText: 'Fechar'
                }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "Erro", script, true);
        }

        // Método adicional para mostrar mensagem de informação (opcional)
        private void MostrarMensagemInformacao(string mensagem)
        {
            string script = $@"
                Swal.fire({{
                    icon: 'info',
                    title: 'Informação',
                    text: '{mensagem.Replace("'", "\\'")}',
                    confirmButtonText: 'OK'
                }});";
            ScriptManager.RegisterStartupScript(this, GetType(), "Informacao", script, true);
        }
    }
}