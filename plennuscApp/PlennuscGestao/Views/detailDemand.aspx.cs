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
    public partial class detailDemand : System.Web.UI.Page
    {
        private readonly DemandaService _service = new DemandaService("Plennus");

        private int CodDemanda => Convert.ToInt32(Request.QueryString["id"]);
        private int CodPessoaAtual => Convert.ToInt32(Session["CodPessoa"] ?? 0);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarTudo();
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
                lblStatus.Text = demandaAtual.StatusNome;
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
            bool ehDestinatario = (demandaAtual.CodPessoaAprovacao == codUsuario);

            // REGRAS:
            bool podeResponder = ehDestinatario && (statusCodigo == 17); // Aberta
            bool podeEncerrar = ehSolicitante && (statusCodigo == 18);   // Em andamento
            bool podeReabrir = ehSolicitante && (statusCodigo == 18);    // Em andamento

            btnEncerrar.Visible = podeEncerrar;
            btnReabrir.Visible = podeReabrir;
            pnlResposta.Visible = podeResponder;
            pnlReabertura.Visible = false; // Inicialmente escondido
        }

        protected void btnResponder_Click(object sender, EventArgs e)
        {
            var texto = txtResposta.Text?.Trim();
            if (!string.IsNullOrEmpty(texto))
            {
                // SALVA A RESPOSTA
                _service.AdicionarAcompanhamento(CodDemanda, CodPessoaAtual, texto);

                // MUDA STATUS PARA "EM ANDAMENTO" (18) - VOLTA PARA QUEM ABRIU
                _service.AtualizarStatusComHistorico(CodDemanda, 18, CodPessoaAtual);

                txtResposta.Text = string.Empty;
                Response.Redirect("listDemand.aspx");
            }
        }

        protected void btnEncerrar_Click(object sender, EventArgs e)
        {
            // MUDA STATUS PARA "CONCLUÍDA" (23)
            _service.AtualizarStatusComHistorico(CodDemanda, 23, CodPessoaAtual);
            Response.Redirect("listDemand.aspx");
        }

        protected void btnReabrir_Click(object sender, EventArgs e)
        {
            // Mostra o painel de reabertura em vez de fazer direto
            pnlReabertura.Visible = true;
            btnReabrir.Visible = false; // Esconde o botão original
        }

        // NOVO MÉTODO: Confirmar reabertura com mensagem
        protected void btnConfirmarReabertura_Click(object sender, EventArgs e)
        {
            var motivo = txtMotivoReabertura.Text?.Trim();

            if (!string.IsNullOrEmpty(motivo))
            {
                // SALVA O MOTIVO DA REABERTURA COMO ACOMPANHAMENTO
                _service.AdicionarAcompanhamento(CodDemanda, CodPessoaAtual, "Motivo da reabertura: " + motivo);

                // MUDA STATUS PARA "ABERTA" (17) - VOLTA PARA O DESTINATÁRIO
                _service.AtualizarStatusComHistorico(CodDemanda, 17, CodPessoaAtual);

                Response.Redirect("listDemand.aspx");
            }
            else
            {
                // Mostrar mensagem de erro se não preencheu o motivo
                ClientScript.RegisterStartupScript(this.GetType(), "alert", "alert('Por favor, digite o motivo da reabertura.');", true);
            }
        }
    }
}