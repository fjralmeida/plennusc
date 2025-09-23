
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
                    Response.Redirect("~/Login.aspx");
                    return;
                }

                BindGrid();
            }
        }

        private void BindGrid()
        {
            try
            {
                var demandas = _svc.GetDemandasEmAbertoPorPessoa(CodPessoaAtual);

                // DEBUG - Adicione estas linhas
                System.Diagnostics.Debug.WriteLine($"Usuário: {CodPessoaAtual}");
                System.Diagnostics.Debug.WriteLine($"Demandas encontradas: {demandas?.Count ?? 0}");

                if (demandas != null)
                {
                    foreach (var d in demandas)
                    {
                        System.Diagnostics.Debug.WriteLine($"Demanda {d.CodDemanda} - Executor: {d.CodPessoaExecucao} - Prazo: {d.DataPrazo}");
                    }
                }

                gvDemandasAberto.DataSource = demandas;
                gvDemandasAberto.DataBind();

                // Atualizar label de resultados
                lblResultados.Text = $"Total de demandas: {demandas?.Count ?? 0}";
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro ao carregar demandas: " + ex.Message, "error");
                System.Diagnostics.Debug.WriteLine($"ERRO: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack: {ex.StackTrace}");
            }
        }


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

                // Obter o DTO
                var dto = e.Row.DataItem as dynamic;

                // Configurar a visibilidade
                if (dto.CodPessoaExecucao != null && dto.CodPessoaExecucao > 0)
                {
                    lblAceiteInfo.Visible = true;
                    btnAceitar.Visible = false;
                }
                else
                {
                    lblAceiteInfo.Visible = false;
                    btnAceitar.Visible = true;
                }
            }
        }

        protected void gvDemandasAberto_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Aceitar")
            {
                int codDemanda = Convert.ToInt32(e.CommandArgument);
                int codPessoa = Convert.ToInt32(Session["CodPessoa"]);

                // Registrar o aceite da demanda
                var svc = new DemandaService("Plennus");
                if (svc.AceitarDemanda(codDemanda, codPessoa))
                {
                    MostrarMensagem("Demanda aceita com sucesso!", "success");
                    BindGrid(); // Recarregar o grid
                }
                else
                {
                    MostrarMensagem("Erro ao aceitar a demanda.", "error");
                }
            }
            else if (e.CommandName == "Ver")
            {
                int codDemanda = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"~/PlennuscGestao/Views/detailDemand.aspx?codDemanda={codDemanda}");
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
