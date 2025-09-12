using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Service.ServiceGestao;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class detailDemand : System.Web.UI.Page
    {
        private readonly DemandaService _service = new DemandaService("Plennus");
        private int CodDemanda => Convert.ToInt32(Request.QueryString["id"]);
        private int CodPessoaAtual => Convert.ToInt32(Session["CodPessoa"] ?? 0);
        private bool DemandaFechada => (demandaAtual?.StatusCodigo == 23); // 23 = Status "Concluída"

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarTudo();
                ConfigurarFormularioAcompanhamento();
            }
        }

        private void CarregarTudo()
        {
            CarregarDemanda();
            CarregarHistorico();
            CarregarAcompanhamentos();
            AjustarBotoes();
        }

        private DemandaDto demandaAtual;

        private void CarregarDemanda()
        {
            demandaAtual = _service.ObterDemandaPorId(CodDemanda);
            if (demandaAtual != null)
            {
                lblTitulo.Text = demandaAtual.Titulo;
                lblTexto.Text = demandaAtual.TextoDemanda;

                // Configurar o badge de status
                if (DemandaFechada)
                {
                    lblStatusBadge.Text = "Fechada";
                    lblStatusBadge.CssClass = "status-badge status-closed";
                }
                else
                {
                    lblStatusBadge.Text = "Em andamento";
                    lblStatusBadge.CssClass = "status-badge status-open";
                }

                lblSolicitante.Text = demandaAtual.Solicitante;
                lblDataSolicitacao.Text = demandaAtual.DataSolicitacao?.ToString("dd/MM/yyyy") ?? string.Empty;
            }
        }

        private void CarregarHistorico()
        {
            var historico = _service.ObterHistorico(CodDemanda);
            rptHistorico.DataSource = historico;
            rptHistorico.DataBind();
        }

        private void CarregarAcompanhamentos()
        {
            var acs = _service.ObterAcompanhamentos(CodDemanda);
            rptAcompanhamentos.DataSource = acs;
            rptAcompanhamentos.DataBind();
        }

        private void AjustarBotoes()
        {
            if (demandaAtual == null) demandaAtual = _service.ObterDemandaPorId(CodDemanda);
            if (demandaAtual == null) return;

            int statusCodigo = demandaAtual.StatusCodigo ?? 0;
            int codUsuario = CodPessoaAtual;

            bool ehSolicitante = (demandaAtual.CodPessoaSolicitacao == codUsuario);

            // Só mostra botão de encerrar para quem abriu E se status for "Em andamento"
            bool podeEncerrar = ehSolicitante && (statusCodigo == 18);

            btnEncerrar.Visible = podeEncerrar;
        }

        private void ConfigurarFormularioAcompanhamento()
        {
            if (DemandaFechada)
            {
                // Desabilitar apenas os controles que existem
                txtNovoAcompanhamento.Enabled = false;
                txtNovoAcompanhamento.Attributes["placeholder"] = "Demanda fechada - não é possível adicionar acompanhamentos";
                btnAdicionarAcompanhamento.Enabled = false;
                btnAdicionarAcompanhamento.CssClass = "btn btn-secondary w-100";
                btnAdicionarAcompanhamento.Text = "Demanda Fechada";
            }
        }

        protected void btnAdicionarAcompanhamento_Click(object sender, EventArgs e)
        {
            // Verificar se a demanda está fechada
            if (DemandaFechada)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "demandaFechada",
                    "alert('Esta demanda está fechada. Não é possível adicionar acompanhamentos.');", true);
                return;
            }

            var texto = txtNovoAcompanhamento.Text?.Trim();
            if (!string.IsNullOrEmpty(texto))
            {
                // SALVA O ACOMPANHAMENTO
                _service.AdicionarAcompanhamento(CodDemanda, CodPessoaAtual, texto);

                // LIMPA O CAMPO E RECARREGA
                txtNovoAcompanhamento.Text = string.Empty;
                CarregarAcompanhamentos(); // Atualiza a lista
            }
        }

        protected void btnEncerrar_Click(object sender, EventArgs e)
        {
            // MUDA STATUS PARA "CONCLUÍDA" (23) - DEMANDA FINALIZADA
            _service.AtualizarStatusComHistorico(CodDemanda, 23, CodPessoaAtual);
            Response.Redirect("listDemand.aspx");
        }
    }
}