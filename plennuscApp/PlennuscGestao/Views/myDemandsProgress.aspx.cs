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
                BindGrid();
            }
        }

        private void BindGrid()
        {
            // Usa direto o método que você vai criar no Service
            var lista = _svc.GetDemandasEmAndamentoPorPessoa(CodPessoaAtual);

            gvMinhasDemandas.DataSource = lista;
            gvMinhasDemandas.DataBind();
        }

        protected void gvMinhasDemandas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ver")
            {
                int codDemanda = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"detailDemand.aspx?codDemanda={codDemanda}");
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
    }
}