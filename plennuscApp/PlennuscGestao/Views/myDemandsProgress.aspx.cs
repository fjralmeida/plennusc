using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Service.ServiceGestao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class myDemandsProgress : System.Web.UI.Page
    {
        private readonly DemandaService _svc = new DemandaService("Plennus");
        private int CodPessoaAtual => Convert.ToInt32(Session["CodPessoa"] ?? 0);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarFiltros();
                BindGrid();
            }
        }

        private void CarregarFiltros()
        {
            try
            {
                // Carrega prioridades
                ddlPrioridade.DataSource = _svc.GetPrioridadesDemanda();
                ddlPrioridade.DataValueField = "Value";
                ddlPrioridade.DataTextField = "Text";
                ddlPrioridade.DataBind();
                ddlPrioridade.Items.Insert(0, new ListItem("Todas", ""));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar filtros: {ex.Message}");
            }
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            try
            {
                // Pega as demandas em andamento do usuário
                var demandas = _svc.GetDemandasEmAndamentoPorPessoa(CodPessoaAtual);

                if (demandas == null)
                {
                    gvMinhasDemandas.DataSource = new List<DemandaInfo>();
                    gvMinhasDemandas.DataBind();
                    lblResultados.Text = "Nenhuma demanda em andamento encontrada.";
                    return;
                }

                // Aplica filtros
                var demandasFiltradas = AplicarFiltrosEmMemoria(demandas);

                // Debug
                System.Diagnostics.Debug.WriteLine($"Demandas em andamento encontradas: {demandasFiltradas.Count}");
                foreach (var d in demandasFiltradas.Take(5))
                {
                    System.Diagnostics.Debug.WriteLine($"Demanda {d.CodDemanda}: {d.Titulo}");
                }

                gvMinhasDemandas.DataSource = demandasFiltradas;
                gvMinhasDemandas.DataBind();
                lblResultados.Text = $"Total de demandas em andamento: {demandasFiltradas.Count}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no BindGrid: {ex.Message}");
                MostrarMensagem("Erro ao carregar demandas: " + ex.Message, "error");
            }
        }

        private List<DemandaInfo> AplicarFiltrosEmMemoria(List<DemandaInfo> demandas)
        {
            if (demandas == null) return new List<DemandaInfo>();

            var filtradas = demandas.AsEnumerable();

            // Filtro por Prioridade
            if (!string.IsNullOrEmpty(ddlPrioridade.SelectedValue) &&
                ddlPrioridade.SelectedValue != "" &&
                int.TryParse(ddlPrioridade.SelectedValue, out int prioridadeId))
            {
                var prioridadeSelecionada = ddlPrioridade.SelectedItem.Text;
                System.Diagnostics.Debug.WriteLine($"Filtrando por prioridade: {prioridadeSelecionada}");
                filtradas = filtradas.Where(d =>
                    !string.IsNullOrEmpty(d.Prioridade) &&
                    d.Prioridade.Equals(prioridadeSelecionada, StringComparison.OrdinalIgnoreCase));
            }

            // Filtro por Solicitante
            if (!string.IsNullOrWhiteSpace(txtSolicitante.Text))
            {
                System.Diagnostics.Debug.WriteLine($"Filtrando por solicitante: {txtSolicitante.Text}");
                filtradas = filtradas.Where(d =>
                    !string.IsNullOrEmpty(d.Solicitante) &&
                    d.Solicitante.IndexOf(txtSolicitante.Text, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            var resultado = filtradas.ToList();
            System.Diagnostics.Debug.WriteLine($"Demandas após filtro: {resultado.Count}");
            return resultado;
        }

        // MANTENHA TODOS OS MÉTODOS EXISTENTES SEM ALTERAÇÕES
        protected void gvMinhasDemandas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ver")
            {
                // ✅ PADRÃO PARA ESCONDER ID
                int codDemanda = Convert.ToInt32(e.CommandArgument);
                Session["CurrentDemandId"] = codDemanda;
                Response.Redirect("~/detailDemand");
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