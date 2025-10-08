using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Service.ServiceGestao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscMedic.Views
{
    public partial class myDemandsProgressMedic : System.Web.UI.Page
    {
        private readonly DemandaService _svc = new DemandaService("Plennus");

        private int CodPessoaAtual => Convert.ToInt32(Session["CodPessoa"] ?? 0);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid()
        {
            try
            {
                // Pega as demandas em andamento do usuário
                var demandas = _svc.GetDemandasEmAndamentoPorPessoa(CodPessoaAtual);

                if (demandas != null)
                {
                    foreach (var d in demandas)
                    {
                        System.Diagnostics.Debug.WriteLine($"Demanda {d.CodDemanda} - Executor: {d.CodPessoaExecucao}");
                    }
                }

                gvMinhasDemandas.DataSource = demandas;
                gvMinhasDemandas.DataBind();

                // Atualiza label ou contador de resultados se existir
                lblResultados.Text = $"Total de demandas em andamento: {demandas?.Count ?? 0}";
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro ao carregar demandas em andamento: " + ex.Message, "error");
            }
        }

        protected void gvMinhasDemandas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ver")
            {
                int codDemanda = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"detailDemandMedic.aspx?codDemanda={codDemanda}");
            }
        }

        protected void gvMinhasDemandas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var dto = (DemandaInfo)e.Row.DataItem;
                var lblStatus = e.Row.FindControl("lblStatus") as Label;

                if (lblStatus != null)
                {
                    if (dto.Status != null && dto.Status.ToLower().Contains("conclu"))
                    {
                        lblStatus.CssClass = "status-badge status-closed";
                    }
                    else
                    {
                        lblStatus.CssClass = "status-badge status-open";
                    }

                }
            }
        }

        protected void gvMinhasDemandas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvMinhasDemandas.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        private void MostrarMensagem(string mensagem, string tipo)
        {
            string script = $@"showToast{(tipo == "success" ? "Sucesso" : "Erro")}('{mensagem.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Mensagem", script, true);
        }
        protected string GetClassePrazo(object dataPrazo)
        {
            if (dataPrazo == null || dataPrazo == DBNull.Value)
                return "prazo-sem-data";

            DateTime prazo = Convert.ToDateTime(dataPrazo);
            DateTime hoje = DateTime.Today;

            if (prazo < hoje)
                return "prazo-atrasado";
            else if (prazo == hoje)
                return "prazo-hoje";
            else if (prazo <= hoje.AddDays(3))
                return "prazo-proximo";
            else
                return "prazo-dentro-prazo";
        }
    }
}