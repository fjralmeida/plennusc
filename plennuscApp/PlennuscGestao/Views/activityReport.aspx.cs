using Plennusc.Core.Service.ServiceGestao.CIDsService;
using Plennusc.Core.Service.ServiceGestao.serviceBilling;
using Plennusc.Core.Models.ModelsGestao.modelsCIDs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class activityReport : System.Web.UI.Page
    {
        private readonly activityReportService _service = new activityReportService();

        // Usado só para manter os dados entre postbacks de paginação do GridView
        private List<ActivityReportModel> DadosRelatorio
        {
            get => Session["ActivityReportResult"] as List<ActivityReportModel>;
            set => Session["ActivityReportResult"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarOperadoras();
                DadosRelatorio = null; // limpa resultado antigo ao entrar na página
            }
        }

        private void CarregarOperadoras()
        {
            var operadoras = _service.ObterOperadoras();
            ddlOperadora.DataSource = operadoras;
            ddlOperadora.DataTextField = "NomeOperadora";
            ddlOperadora.DataValueField = "CodigoGrupoContrato";
            ddlOperadora.DataBind();
            ddlOperadora.Items.Insert(0, new ListItem("Selecione...", ""));
        }

        protected void btnRelatorioMovimentacao_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlOperadora.SelectedValue))
            {
                ExibirErro("Selecione uma operadora.");
                return;
            }

            if (!DateTime.TryParse(txtVigencia.Text, out var vigencia))
            {
                ExibirErro("Vigência inválida.");
                return;
            }

            int codigoGrupoContrato = int.Parse(ddlOperadora.SelectedValue);

            List<ActivityReportModel> dados = _service.GerarRelatorio(codigoGrupoContrato, vigencia);

            if (dados == null || dados.Count == 0)
            {
                divResultado.Visible = false;
                ExibirErro("Nenhum registro encontrado para essa operadora/vigência.");
                return;
            }

            DadosRelatorio = dados;
            gvRelatorio.PageIndex = 0;
            VincularGrid();
        }

        protected void gvRelatorio_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvRelatorio.PageIndex = e.NewPageIndex;
            VincularGrid();
        }

        private void VincularGrid()
        {
            var dados = DadosRelatorio;
            if (dados == null || dados.Count == 0)
            {
                divResultado.Visible = false;
                return;
            }

            lblMensagem.Visible = false;
            divResultado.Visible = true;
            litTotalRegistros.Text = dados.Count.ToString();
            gvRelatorio.DataSource = dados;
            gvRelatorio.DataBind();
        }

        private void ExibirErro(string mensagem)
        {
            lblMensagem.Text = mensagem;
            lblMensagem.Visible = true;
        }
    }
}