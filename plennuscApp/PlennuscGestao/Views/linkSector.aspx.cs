using Plennusc.Core.Service.ServiceGestao.vinculaDepartameto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class linkSector : System.Web.UI.Page
    {
        private linkSectorService _linkService = new linkSectorService();
        private departmentService _departamentoService = new departmentService();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarViews();
                CarregarSetores();
                pnlVincular.Visible = false;
            }
        }

        private void CarregarViews()
        {
            var views = _linkService.GetTodasViews();
            ddlView.DataSource = views;
            ddlView.DataTextField = "DescTipoEstrutura";
            ddlView.DataValueField = "CodTipoEstrutura";
            ddlView.DataBind();
        }

        private void CarregarSetores()
        {
            ddlSetor.Items.Clear();
            ddlSetor.Items.Add(new ListItem("-- Selecione --", "0"));
            var setores = _departamentoService.GetTodosDepartamentos();
            foreach (var setor in setores)
            {
                ddlSetor.Items.Add(new ListItem(setor.Nome, setor.CodDepartamento.ToString()));
            }
        }

        protected void ddlView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlView.SelectedValue))
            {
                int codTipoEstrutura = Convert.ToInt32(ddlView.SelectedValue);
                CarregarEstruturasDaView(codTipoEstrutura);
                lblViewSelecionada.Text = ddlView.SelectedItem.Text;
                pnlVincular.Visible = true;
            }
            else
            {
                pnlVincular.Visible = false;
            }
        }

        private void CarregarEstruturasDaView(int codTipoEstrutura)
        {
            var estruturas = _linkService.GetEstruturasPorView(codTipoEstrutura);
            gvEstruturas.DataSource = estruturas;
            gvEstruturas.DataBind();
        }

        protected void btnVincular_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(ddlView.SelectedValue))
                {
                    MostrarMensagem("Selecione uma View.", "warning");
                    return;
                }
                if (ddlSetor.SelectedValue == "0")
                {
                    MostrarMensagem("Selecione um setor.", "warning");
                    return;
                }

                int codTipoEstrutura = Convert.ToInt32(ddlView.SelectedValue);
                int codSetor = Convert.ToInt32(ddlSetor.SelectedValue);

                bool sucesso = _linkService.VincularSetorAoTipoEstrutura(codSetor, codTipoEstrutura);

                if (sucesso)
                    MostrarMensagem($"Setor vinculado a todas as estruturas da View '{lblViewSelecionada.Text}' com sucesso!", "success");
                else
                    MostrarMensagem("O setor já estava vinculado a todas as estruturas desta View.", "warning");
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro: {ex.Message}", "error");
            }
        }

        private void MostrarMensagem(string mensagem, string tipo = "success")
        {
            string script = $@"
                Swal.fire({{
                    toast: true,
                    position: 'top-end',
                    icon: '{tipo}',
                    title: '{mensagem.Replace("'", "\\'")}',
                    showConfirmButton: false,
                    timer: {(tipo == "error" || tipo == "warning" ? "4000" : "3000")},
                    timerProgressBar: true
                }});
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), $"Toast{tipo}", script, true);
        }
    }
}