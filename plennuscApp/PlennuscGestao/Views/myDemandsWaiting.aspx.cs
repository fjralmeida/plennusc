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

                BindGrid();
            }
        }

        private void BindGrid()
        {
            try
            {
                var demandas = _svc.GetDemandasAguardandoAprovacaoPorGestor(CodPessoaAtual);



                gvDemandasAguardando.DataSource = demandas;
                gvDemandasAguardando.DataBind();
                lblResultados.Text = $"Total de demandas aguardando aprovação: {demandas?.Count ?? 0}";

                if (demandas == null || demandas.Count == 0)
                {
                    lblResultados.Text += " - Nenhuma demanda encontrada para sua aprovação.";
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro ao carregar demandas: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"ERRO: {ex.Message}");
            }
        }

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

        // Métodos auxiliares para classes CSS
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