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
                pnlEstruturas.Visible = false;
            }
        }

        private void CarregarViews()
        {
            try
            {
                var views = _linkService.GetTodasViews();
                ddlView.DataSource = views;
                ddlView.DataTextField = "DescTipoEstrutura";
                ddlView.DataValueField = "CodTipoEstrutura";
                ddlView.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao carregar views: {ex.Message}", "error");
            }
        }

        protected void ddlView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlView.SelectedValue))
            {
                int codTipoEstrutura = Convert.ToInt32(ddlView.SelectedValue);
                CarregarEstruturasDaView(codTipoEstrutura);

                lblViewSelecionada.Text = ddlView.SelectedItem.Text;
                pnlEstruturas.Visible = true;
            }
            else
            {
                pnlEstruturas.Visible = false;
            }
        }

        private void CarregarEstruturasDaView(int codTipoEstrutura)
        {
            try
            {
                var estruturas = _linkService.GetEstruturasPorView(codTipoEstrutura);
                gvEstruturas.DataSource = estruturas;
                gvEstruturas.DataBind();
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao carregar estruturas: {ex.Message}", "error");
            }
        }

        protected void gvEstruturas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlSetor = (DropDownList)e.Row.FindControl("ddlSetor");
                ddlSetor.Items.Clear();
                ddlSetor.Items.Add(new ListItem("-- Selecione --", "0"));

                var setores = _departamentoService.GetTodosDepartamentos();
                foreach (var setor in setores)
                {
                    ddlSetor.Items.Add(new ListItem(setor.Nome, setor.CodDepartamento.ToString()));
                }
            }
        }

        // REMOVA o método btnRemoverSetor_Click completamente por enquanto

        protected void btnVincularSetor_Click(object sender, EventArgs e)
        {
            try
            {
                Button btn = (Button)sender;
                GridViewRow row = (GridViewRow)btn.NamingContainer;

                // PEGA O CÓDIGO DIRETO DA LABEL
                Label lblCodEstrutura = (Label)row.FindControl("lblCodEstrutura");
                int codEstrutura = Convert.ToInt32(lblCodEstrutura.Text);

                DropDownList ddlSetor = (DropDownList)row.FindControl("ddlSetor");

                if (ddlSetor.SelectedValue == "0")
                {
                    MostrarMensagem("Selecione um setor para vincular.", "warning");
                    return;
                }

                int codSetor = Convert.ToInt32(ddlSetor.SelectedValue);
                bool sucesso = _linkService.VincularSetor(codSetor, codEstrutura);

                if (sucesso)
                {
                    MostrarMensagem("Setor vinculado com sucesso!", "success");
                    // Recarrega o grid se necessário
                }
                else
                {
                    MostrarMensagem("Este setor já está vinculado.", "warning");
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro: {ex.Message}", "error");
            }
        }

        //protected void btnRemoverSetor_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        LinkButton btn = (LinkButton)sender;
        //        int codSetorTipoDemanda = Convert.ToInt32(btn.CommandArgument);

        //        bool sucesso = _linkService.DesvincularSetor(codSetorTipoDemanda);

        //        if (sucesso)
        //        {
        //            MostrarMensagem("Setor desvinculado com sucesso!", "success");
        //            // Recarrega o grid para atualizar os vínculos
        //            if (!string.IsNullOrEmpty(ddlView.SelectedValue))
        //            {
        //                int codTipoEstrutura = Convert.ToInt32(ddlView.SelectedValue);
        //                CarregarEstruturasDaView(codTipoEstrutura);
        //            }
        //        }
        //        else
        //        {
        //            MostrarMensagem("Erro ao desvincular setor.", "error");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MostrarMensagem($"Erro: {ex.Message}", "error");
        //    }
        //}

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