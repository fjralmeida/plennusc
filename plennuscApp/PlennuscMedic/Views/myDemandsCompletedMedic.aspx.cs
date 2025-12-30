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
    public partial class myDemandsCompletedMedic : System.Web.UI.Page
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
                var demandas = _svc.GetDemandasConcluidasPorPessoa(CodPessoaAtual);

                if (demandas == null)
                {
                    gvDemandasConcluidas.DataSource = new List<DemandaInfo>();
                    gvDemandasConcluidas.DataBind();
                    lblResultados.Text = "Nenhuma demanda concluída encontrada.";
                    return;
                }

                // Aplica filtros
                var demandasFiltradas = AplicarFiltrosEmMemoria(demandas);

                // Debug
                System.Diagnostics.Debug.WriteLine($"Demandas concluídas encontradas: {demandasFiltradas.Count}");
                foreach (var d in demandasFiltradas.Take(5))
                {
                    System.Diagnostics.Debug.WriteLine($"Demanda {d.CodDemanda}: {d.Titulo}");
                }

                gvDemandasConcluidas.DataSource = demandasFiltradas;
                gvDemandasConcluidas.DataBind();
                lblResultados.Text = $"Total de demandas concluídas: {demandasFiltradas.Count}";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro no BindGrid: {ex.Message}");
                lblResultados.Text = "Erro ao carregar demandas concluídas.";
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
        protected void gvDemandasConcluidas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDemandasConcluidas.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvDemandasConcluidas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ver")
            {
                // ✅ CORREÇÃO AQUI - USA SESSION EM VEZ DE QUERYSTRING
                int codDemanda = Convert.ToInt32(e.CommandArgument);
                Session["CurrentDemandId"] = codDemanda;
                Response.Redirect($"detailDemandMedic.aspx?codDemanda={codDemanda}");
            }
        }
    }
}