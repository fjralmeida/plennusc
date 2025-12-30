
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
    public partial class myDemandsOpen : System.Web.UI.Page
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
                //// Carrega categorias
                //ddlCategoria.DataSource = _svc.GetCategoriasDemanda();
                //ddlCategoria.DataValueField = "Value";
                //ddlCategoria.DataTextField = "Text";
                //ddlCategoria.DataBind();
                //ddlCategoria.Items.Insert(0, new ListItem("Todas", ""));

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

        protected void ddlCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            try
            {
                // Obtém todas as demandas em aberto da pessoa
                var demandas = _svc.GetDemandasEmAbertoPorPessoa(CodPessoaAtual);

                if (demandas == null)
                {
                    gvDemandasAberto.DataSource = new List<DemandaInfo>();
                    gvDemandasAberto.DataBind();
                    lblResultados.Text = "Nenhuma demanda em aberto encontrada.";
                    return;
                }

                // Aplica filtros
                var demandasFiltradas = AplicarFiltrosEmMemoria(demandas);

                // Debug
                System.Diagnostics.Debug.WriteLine($"Demandas encontradas: {demandasFiltradas.Count}");
                foreach (var d in demandasFiltradas.Take(5))
                {
                    System.Diagnostics.Debug.WriteLine($"Demanda {d.CodDemanda}: {d.Titulo}");
                }

                gvDemandasAberto.DataSource = demandasFiltradas;
                gvDemandasAberto.DataBind();
                lblResultados.Text = $"Total de demandas: {demandasFiltradas.Count}";
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

            //// Filtro por Categoria
            //if (!string.IsNullOrEmpty(ddlCategoria.SelectedValue) &&
            //    ddlCategoria.SelectedValue != "" &&
            //    int.TryParse(ddlCategoria.SelectedValue, out int categoriaId))
            //{
            //    var categoriaSelecionada = ddlCategoria.SelectedItem.Text;
            //    System.Diagnostics.Debug.WriteLine($"Filtrando por categoria: {categoriaSelecionada}");
            //    filtradas = filtradas.Where(d =>
            //        !string.IsNullOrEmpty(d.Categoria) &&
            //        d.Categoria.Equals(categoriaSelecionada, StringComparison.OrdinalIgnoreCase));
            //}

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

        // MANTENHA TODOS OS MÉTODOS ORIGINAIS ABAIXO (SEM ALTERAÇÕES)
        protected void gvDemandasAberto_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDemandasAberto.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvDemandasAberto_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblAceiteInfo = (Label)e.Row.FindControl("lblAceiteInfo");
                LinkButton btnAceitar = (LinkButton)e.Row.FindControl("btnAceitar");

                var dto = e.Row.DataItem as dynamic;

                if (dto.CodPessoaExecucao != null && dto.CodPessoaExecucao > 0)
                {
                    if (lblAceiteInfo != null) lblAceiteInfo.Visible = true;
                    if (btnAceitar != null) btnAceitar.Visible = false;
                }
                else
                {
                    if (lblAceiteInfo != null) lblAceiteInfo.Visible = false;
                    if (btnAceitar != null) btnAceitar.Visible = true;
                }
            }
        }

        protected void gvDemandasAberto_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Aceitar")
            {
                int codDemanda = Convert.ToInt32(e.CommandArgument);
                int codPessoa = Convert.ToInt32(Session["CodPessoa"]);

                var svc = new DemandaService("Plennus");
                if (svc.AceitarDemanda(codDemanda, codPessoa))
                {
                    MostrarMensagem("Demanda aceita com sucesso!", "success");
                    BindGrid();
                }
                else
                {
                    MostrarMensagem("Erro ao aceitar a demanda.", "error");
                }
            }
            else if (e.CommandName == "Ver")
            {
                int codDemanda = Convert.ToInt32(e.CommandArgument);
                Session["CurrentDemandId"] = codDemanda;
                Response.Redirect("~/detailDemand");
            }
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