using Plennusc.Core.Service.ServiceGestao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscMedic.Views
{
    public partial class myDemandsOpenMedic : System.Web.UI.Page
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
                var demandas = _svc.GetDemandasEmAbertoPorPessoa(CodPessoaAtual);


                if (demandas != null && demandas.Count > 0)
                {
                    foreach (var d in demandas)
                    {
                        System.Diagnostics.Debug.WriteLine($"Demanda {d.CodDemanda}: Status={d.Status}, Executor={d.CodPessoaExecucao}, Titulo={d.Titulo}");
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("NENHUMA demanda retornada pela consulta!");
                }

                gvDemandasAberto.DataSource = demandas;
                gvDemandasAberto.DataBind();
                lblResultados.Text = $"Total de demandas: {demandas?.Count ?? 0}";
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro ao carregar demandas: " + ex.Message, "error");
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
                Response.Redirect($"~/PlennuscMedic/Views/detailDemandMedic.aspx?codDemanda={codDemanda}");
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
