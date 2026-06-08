using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Service.ServiceGestao.planiumApi;
using Plennusc.Core.Service.ServiceGestao.department;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class operatorRegistration : System.Web.UI.Page
    {
        private readonly OperadoraService _svc = new OperadoraService("Plennus");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e) => BindGrid();

        private void BindGrid()
        {
            var filtro = new OperadoraFiltro
            {
                NomeOperadora = txtOperadora.Text.Trim(),
                RegistroANS = txtRegistroAns.Text.Trim(),
                Numero_CNPJ = txtCnpj.Text.Trim()
            };

            var lista = _svc.ListarOperadoras(filtro);
            gvOperadoras.DataSource = lista;
            gvOperadoras.DataBind();
        }

        protected void gvOperadoras_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvOperadoras.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvOperadoras_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ver")
            {
                int codOperadora = Convert.ToInt32(e.CommandArgument);
                Session["CurrentOperadoraId"] = codOperadora;
                Response.Redirect("~/viewOperadora");
            }
        }

        protected void btnNovaOperadora_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/novaOperadora");
        }

        protected void gvOperadoras_RowDataBound(object sender, GridViewRowEventArgs e)
        {

        }

        /* Criar a função dos botões de editar e excluir 
        protected void btnEditar_Click(object sender, EventArgs e)
        {
            try
            {
                // Use LinkButton ao invés de Button
                LinkButton btn = (LinkButton)sender;
                int operatorId = Convert.ToInt32(btn.CommandArgument);

                var criarNovaOperadora = new CreateDepartmentService();
                var departamento = service.GetDepartmentById(operatorId);

                if (departamento != null)
                {
                    // Preencher os campos do modal
                    txtNomeDepartamento.Text = departamento["Nome"].ToString();
                    txtRamal.Text = departamento["NumRamal"] != null ? departamento["NumRamal"].ToString() : "";
                    txtTelefone.Text = departamento["Telefone"] != null ? departamento["Telefone"].ToString() : "";
                    txtEmail.Text = departamento["EmailGeral"] != null ? departamento["EmailGeral"].ToString() : "";

                    // Colocar o ID no CommandArgument do botão salvar
                    btnSalvarDepartamento.CommandArgument = operatorId.ToString();

                    // Abrir o modal via JavaScript
                    string script = @"
                $('#modalNovoDepartamentoLabel').html('<i class=""fas fa-edit me-2""></i>Editar Departamento');
                $('#btnSalvarDepartamento').text('Atualizar Departamento');
                var modal = new bootstrap.Modal(document.getElementById('modalNovoDepartamento'));
                modal.show();
            ";

                    ScriptManager.RegisterStartupScript(this, GetType(), "AbrirModalEdicao", script, true);
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao carregar departamento: {ex.Message}", "error");
            }
        }
        */

    }
}