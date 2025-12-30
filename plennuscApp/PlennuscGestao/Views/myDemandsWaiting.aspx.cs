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
    public partial class myDemandsWaiting : System.Web.UI.Page
    {
        private readonly DemandaService _svc = new DemandaService("Plennus");
        private int CodPessoaAtual => Convert.ToInt32(Session["CodPessoa"] ?? 0);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (CodPessoaAtual == 0)
                {
                    Response.Redirect("~/SignIn.aspx");
                    return;
                }

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
                var demandas = _svc.GetDemandasAguardandoAprovacaoPorGestor(CodPessoaAtual);

                if (demandas == null)
                {
                    gvDemandasAguardando.DataSource = new List<DemandaInfo>();
                    gvDemandasAguardando.DataBind();
                    lblResultados.Text = "Nenhuma demanda aguardando aprovação encontrada.";
                    return;
                }

                // Aplica filtros
                var demandasFiltradas = AplicarFiltrosEmMemoria(demandas);

                // Debug
                System.Diagnostics.Debug.WriteLine($"Demandas aguardando aprovação encontradas: {demandasFiltradas.Count}");
                foreach (var d in demandasFiltradas.Take(5))
                {
                    System.Diagnostics.Debug.WriteLine($"Demanda {d.CodDemanda}: {d.Titulo}");
                }

                gvDemandasAguardando.DataSource = demandasFiltradas;
                gvDemandasAguardando.DataBind();
                lblResultados.Text = $"Total de demandas aguardando aprovação: {demandasFiltradas.Count}";

                if (demandasFiltradas.Count == 0)
                {
                    lblResultados.Text += " - Nenhuma demanda encontrada para sua aprovação.";
                }
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
        protected void gvDemandasAguardando_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDemandasAguardando.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvDemandasAguardando_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Configurações específicas se necessário
                // Seu código atual aqui
            }
        }

        protected void gvDemandasAguardando_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Aprovar")
            {
                int codDemanda = Convert.ToInt32(e.CommandArgument);

                try
                {
                    if (_svc.AprovarDemanda(codDemanda, CodPessoaAtual))
                    {
                        MostrarMensagem("Demanda aprovada com sucesso! A demanda voltou para status 'Aberta'.", "success");
                        BindGrid(); // Recarregar o grid
                    }
                    else
                    {
                        MostrarMensagem("Erro ao aprovar a demanda. Verifique se você é o gestor responsável.", "error");
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensagem("Erro ao aprovar demanda: " + ex.Message, "error");
                }
            }
            else if (e.CommandName == "Ver")
            {
                // ✅ CORREÇÃO AQUI - USA SESSION EM VEZ DE QUERYSTRING
                int codDemanda = Convert.ToInt32(e.CommandArgument);
                Session["CurrentDemandId"] = codDemanda;
                Response.Redirect("~/detailDemand");
            }
        }

        // Métodos auxiliares para classes CSS - MANTIDOS
        protected string GetClassePrioridade(object prioridade)
        {
            if (prioridade == null || prioridade == DBNull.Value)
                return "prioridade-nao-definida";

            string prio = prioridade.ToString().ToLower()
                .Replace(" ", "-")
                .Replace("é", "e").Replace("á", "a").Replace("í", "i")
                .Replace("ê", "e").Replace("â", "a").Replace("ô", "o")
                .Replace("û", "u").Replace("ç", "c");

            return $"prioridade-{prio}";
        }

        protected string GetClasseImportancia(object importancia)
        {
            if (importancia == null || importancia == DBNull.Value)
                return "importancia-baixa";

            string imp = importancia.ToString().ToLower()
                .Replace("é", "e").Replace("á", "a").Replace("í", "i")
                .Replace("ê", "e").Replace("â", "a").Replace("ô", "o")
                .Replace("û", "u").Replace("ç", "c");

            return $"importancia-{imp}";
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

        private void MostrarMensagem(string mensagem, string tipo)
        {
            string script = $@"showToast{(tipo == "success" ? "Sucesso" : "Erro")}('{mensagem.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Mensagem", script, true);
        }
    }
}