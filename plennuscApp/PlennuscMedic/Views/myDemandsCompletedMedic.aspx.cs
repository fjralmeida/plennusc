﻿using Plennusc.Core.Service.ServiceGestao;
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
                BindGrid();
            }
        }

        private void BindGrid()
        {
            try
            {
                var demandas = _svc.GetDemandasConcluidasPorPessoa(CodPessoaAtual);

                gvDemandasConcluidas.DataSource = demandas;
                gvDemandasConcluidas.DataBind();
            }
            catch (Exception ex)
            {

            }
        }

        protected void gvDemandasConcluidas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDemandasConcluidas.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvDemandasConcluidas_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ver")
            {
                int codDemanda = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"detailDemandMedic.aspx?codDemanda={codDemanda}");
            }
        }
    }
}