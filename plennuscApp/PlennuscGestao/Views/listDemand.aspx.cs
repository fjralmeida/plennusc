﻿using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Service.ServiceGestao;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class listDemand : System.Web.UI.Page
    {
        private readonly DemandaService _svc = new DemandaService("Plennus");

        private int CodPessoaAtual => Convert.ToInt32(Session["CodPessoa"] ?? 0);
        private int? CodSetorAtual => Session["CodDepartamento"] == null ? (int?)null : Convert.ToInt32(Session["CodDepartamento"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // DEIXE APENAS AS 2 OPÇÕES QUE EXISTEM NO ASPX
                ddlVisibilidade.Items.Clear();
                ddlVisibilidade.Items.Add(new ListItem("Meu Setor", "S"));
                ddlVisibilidade.Items.Add(new ListItem("Minhas Demandas", "M"));
                ddlVisibilidade.SelectedValue = "S"; // Default: Meu Setor

                ddlVisibilidade.SelectedValue = "T";

                CarregarFiltros();
                BindGrid();
            }
        }

        private void CarregarFiltros()
        {
            ddlStatus.DataSource = _svc.GetSituacoesDemanda();
            ddlStatus.DataValueField = "Value";
            ddlStatus.DataTextField = "Text";
            ddlStatus.DataBind();
            ddlStatus.Items.Insert(0, new ListItem("Todos", ""));

            ddlCategoria.DataSource = _svc.GetCategoriasDemanda();
            ddlCategoria.DataValueField = "Value";
            ddlCategoria.DataTextField = "Text";
            ddlCategoria.DataBind();
            ddlCategoria.Items.Insert(0, new ListItem("Todas", ""));
        }

        protected void ddlCategoria_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlCategoria.SelectedValue) &&
                int.TryParse(ddlCategoria.SelectedValue, out int categoriaId))
            {
                ddlSubtipo.DataSource = _svc.GetSubtiposDemanda(categoriaId);
                ddlSubtipo.DataValueField = "Value";
                ddlSubtipo.DataTextField = "Text";
                ddlSubtipo.DataBind();
            }
            else
            {
                ddlSubtipo.DataSource = null;
                ddlSubtipo.DataBind();
            }
            ddlSubtipo.Items.Insert(0, new ListItem("Todos", ""));
            BindGrid();
        }

        // NOVO MÉTODO: Evento do dropdown de visualização
        protected void ddlVisibilidade_Changed(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void btnFiltrar_Click(object sender, EventArgs e) => BindGrid();

        private void BindGrid()
        {
            var filtro = new DemandaFiltro
            {
                CodPessoa = CodPessoaAtual,
                CodSetor = CodSetorAtual,
                CodStatus = string.IsNullOrEmpty(ddlStatus.SelectedValue) ? (int?)null : int.Parse(ddlStatus.SelectedValue),
                CodCategoria = string.IsNullOrEmpty(ddlCategoria.SelectedValue) ? (int?)null : int.Parse(ddlCategoria.SelectedValue),
                CodSubtipo = string.IsNullOrEmpty(ddlSubtipo.SelectedValue) ? (int?)null : int.Parse(ddlSubtipo.SelectedValue),
                NomeSolicitante = txtSolicitante.Text.Trim(),
                Visibilidade = ddlVisibilidade.SelectedValue
            };

            System.Diagnostics.Debug.WriteLine("Filtro.CodSetor: " + filtro.CodSetor);
            System.Diagnostics.Debug.WriteLine("Filtro.Visibilidade: " + filtro.Visibilidade);

            var lista = _svc.ListarDemandas(filtro);
            gvDemandas.DataSource = lista;
            gvDemandas.DataBind();

            System.Diagnostics.Debug.WriteLine("Total de resultados: " + lista.Count);
            System.Diagnostics.Debug.WriteLine("=== FIM DEBUG ===");
        }

        protected void gvDemandas_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDemandas.PageIndex = e.NewPageIndex;
            BindGrid();
        }


        protected void gvDemandas_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblAceiteInfo = (Label)e.Row.FindControl("lblAceiteInfo");
                LinkButton btnAceitar = (LinkButton)e.Row.FindControl("btnAceitar");

                // Obter o DTO (não mais DataRowView)
                var dto = (DemandaListDto)e.Row.DataItem;

                // Configurar a visibilidade
                if (dto.CodPessoaExecucao.HasValue && dto.CodPessoaExecucao > 0)
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

        protected void gvDemandas_RowCommand(object sender, GridViewCommandEventArgs e)
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
                // Sua lógica existente para visualizar
                int codDemanda = Convert.ToInt32(e.CommandArgument);
                Response.Redirect($"detailDemand.aspx?codDemanda={codDemanda}");
            }
        }

        private void MostrarMensagem(string mensagem, string tipo)
        {
            string script = $@"showToast{(tipo == "success" ? "Sucesso" : "Erro")}('{mensagem.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Mensagem", script, true);
        }


        protected void btnNovaDemanda_Click(object sender, EventArgs e)
        {
            Response.Redirect("demand.aspx");
        }

    }
}