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
                var service = new CreateDepartmentService();
                var resultado = service.CreateDepartment(
                    txtNomeDepartamento.Text.Trim(),
                    txtRamal.Text.Trim(),
                    txtEmail.Text.Trim(),
                    txtTelefone.Text.Trim()
                );

                if (resultado.Success)
                {
                    LimparCampos();
                    CarregarDepartamentos();

                    // SÓ ISSO - Chamar uma função que fecha o modal E mostra a mensagem
                    MostrarMensagemComFecharModal(resultado.Message, "success");
                }
                else
                {
                    string tipo = resultado.Message.Contains("já existe") ||
                                  resultado.Message.Contains("inválido") ||
                                  resultado.Message.Contains("obrigatório")
                        ? "warning" : "error";

                    MostrarMensagem(resultado.Message, tipo);
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro: {ex.Message}", "error");
            }
        }

        protected void btnExcluir_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                int departmentId = Convert.ToInt32(btn.CommandArgument);

                var service = new CreateDepartmentService();
                var resultado = service.DeleteDepartment(departmentId);

                if (resultado.Success)
                {
                    CarregarDepartamentos();
                    MostrarMensagem(resultado.Message, "success");
                }
                else
                {
                    string tipo = resultado.Message.Contains("não encontrado") ||
                                  resultado.Message.Contains("não pode excluir") ||
                                  resultado.Message.Contains("vinculado")
                        ? "warning" : "error";

                    MostrarMensagem(resultado.Message, tipo);
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro: {ex.Message}", "error");
            }
        }

        private void CarregarDepartamentos()
        {
            try
            {
                var service = new CreateDepartmentService();
                DataTable dt = service.GetAllDepartments();

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
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao carregar departamentos: {ex.Message}", "error");
            }
        }

        private void LimparCampos()
        {
            txtNomeDepartamento.Text = "";
            txtRamal.Text = "";
            txtEmail.Text = "";
            txtTelefone.Text = "";
        }

        private void MostrarMensagem(string mensagem, string tipo = "success")
        {
            string script;

            if (tipo == "success" || tipo == "info" || tipo == "warning")
            {
                script = $@"
                    const Toast = Swal.mixin({{
                        toast: true,
                        position: 'top-end',
                        showConfirmButton: false,
                        timer: 3000,
                        timerProgressBar: true,
                        didOpen: (toast) => {{
                            toast.addEventListener('mouseenter', Swal.stopTimer);
                            toast.addEventListener('mouseleave', Swal.resumeTimer);
                        }}
                    }});
                    
                    Toast.fire({{
                        icon: '{tipo}',
                        title: '{mensagem.Replace("'", "\\'")}'
                    }});
                ";
            }
            else if (tipo == "error")
            {
                script = $@"
                    Swal.fire({{
                        icon: 'error',
                        title: 'Erro!',
                        text: '{mensagem.Replace("'", "\\'")}',
                        confirmButtonText: 'OK',
                        confirmButtonColor: '#d33'
                    }});
                ";
            }
            else
            {
                script = $@"
                    Swal.fire({{
                        icon: '{tipo}',
                        title: '{mensagem.Replace("'", "\\'")}',
                        showConfirmButton: true,
                        confirmButtonText: 'OK'
                    }});
                ";
            }

            ScriptManager.RegisterStartupScript(this, GetType(), "Mensagem_" + Guid.NewGuid(), script, true);
        }

        // NOVA FUNÇÃO - Fecha modal E mostra mensagem
        private void MostrarMensagemComFecharModal(string mensagem, string tipo = "success")
        {
            string script = $@"
                // Fecha o modal
                var modalElement = document.getElementById('modalNovoDepartamento');
                var modal = bootstrap.Modal.getInstance(modalElement);
                if (modal) {{
                    modal.hide();
                }}
                
                // Mostra a mensagem APÓS fechar o modal
                setTimeout(function() {{
                    const Toast = Swal.mixin({{
                        toast: true,
                        position: 'top-end',
                        showConfirmButton: false,
                        timer: 3000,
                        timerProgressBar: true,
                        didOpen: (toast) => {{
                            toast.addEventListener('mouseenter', Swal.stopTimer);
                            toast.addEventListener('mouseleave', Swal.resumeTimer);
                        }}
                    }});
                    
                    Toast.fire({{
                        icon: '{tipo}',
                        title: '{mensagem.Replace("'", "\\'")}'
                    }});
                }}, 300); // Pequeno delay para garantir que o modal fechou
            ";

            ScriptManager.RegisterStartupScript(this, GetType(), "MensagemModal_" + Guid.NewGuid(), script, true);
        }
    }
}