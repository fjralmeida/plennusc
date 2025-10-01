using Plennusc.Core.Service.ServiceGestao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class myDemandsRefused : System.Web.UI.Page
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
                var demandas = _svc.GetDemandasRecusadasPorPessoa(CodPessoaAtual);

                gvDemandasRecusadas.DataSource = demandas;
                gvDemandasRecusadas.DataBind();
                lblResultados.Text = $"Total de {demandas.Count} demanda(s) recusada(s)";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERRO: {ex.Message}");
                lblResultados.Text = "Erro ao carregar demandas recusadas.";
            }
        }

        protected void gvDemandasRecusadas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDemandasRecusadas.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvDemandasRecusadas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ver")
            {
                int codDemanda = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"detailDemand.aspx?codDemanda={codDemanda}");
            }
        }
    }
}