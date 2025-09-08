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

        private void AjustarBotoes()
        {
            if (demandaAtual == null) demandaAtual = _service.ObterDemandaPorId(CodDemanda);
            if (demandaAtual == null) return;

            var permissoes = VerificarPermissoes();

            // Controla a visibilidade dos botões principais
            btnResolver.Visible = permissoes.PodeResolver;
            btnEncerrar.Visible = permissoes.PodeEncerrar;
            btnReabrir.Visible = permissoes.PodeReabrir;

            // CONTROLAR A VISIBILIDADE DO PAINEL DE RESPOSTA COM BASE NA PERMISSÃO
            pnlResposta.Visible = permissoes.PodeResponder;
        }

        private (bool PodeResponder, bool PodeResolver, bool PodeEncerrar, bool PodeReabrir) VerificarPermissoes()
        {
            // Recarrega demandaAtual caso esteja nulo
            if (demandaAtual == null) demandaAtual = _service.ObterDemandaPorId(CodDemanda);
            if (demandaAtual == null) return (false, false, false, false);

            var status = demandaAtual?.StatusNome?.ToUpperInvariant() ?? "";
            int codUsuario = CodPessoaAtual;

            // 1. É o Solicitante?
            bool ehSolicitante = (demandaAtual.CodPessoaSolicitacao == codUsuario);
            // 2. É o Aprovador/Destino? (A PESSOA PARA QUEM A DEMANDA FOI ENVIADA)
            bool ehAprovador = (demandaAtual.CodPessoaAprovacao == codUsuario);

            // REGRA PRINCIPAL: SÓ O APROVADOR (DESTINATÁRIO) PODE RESPONDER
            bool podeResponder = ehAprovador;

            // Regras para os botões de ação (baseadas no status e no papel)
            bool podeResolver = (status == "ABERTA") && ehAprovador;
            bool podeEncerrar = (status == "ABERTA") && ehAprovador;
            bool podeReabrir = (status == "RESOLVIDA" || status == "ENCERRADA") && (ehSolicitante || ehAprovador);

            return (podeResponder, podeResolver, podeEncerrar, podeReabrir);
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
        protected void btnResponder_Click(object sender, EventArgs e)
        {
            var texto = txtResposta.Text?.Trim();
            if (!string.IsNullOrEmpty(texto))
            {
                // salva como Acompanhamento (não cria tabela nova)
                _service.AdicionarAcompanhamento(CodDemanda, CodPessoaAtual, texto);

                // limpa campo e recarrega a lista de acompanhamentos
                txtResposta.Text = string.Empty;
                CarregarAcompanhamentos();
            }
        }

        protected void btnResolver_Click(object sender, EventArgs e)
        {
            // usa o método que atualiza status e grava histórico (passando codPessoa atual)
            _service.AtualizarStatusComHistorico(CodDemanda, "Resolvida", CodPessoaAtual);
            CarregarTudo();
        }

        protected void btnEncerrar_Click(object sender, EventArgs e)
        {
            _service.AtualizarStatusComHistorico(CodDemanda, "Encerrada", CodPessoaAtual);
            CarregarTudo();
        }

        protected void btnReabrir_Click(object sender, EventArgs e)
        {
            _service.AtualizarStatusComHistorico(CodDemanda, "Aberta", CodPessoaAtual);
            CarregarTudo();
        }
    }
}