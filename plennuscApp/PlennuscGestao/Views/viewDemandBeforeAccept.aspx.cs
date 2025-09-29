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
    public partial class viewDemandBeforeAccept : System.Web.UI.Page
    {
        private readonly DemandaService _service = new DemandaService("Plennus");
        private int CodDemanda => Convert.ToInt32(Request.QueryString["codDemanda"]);
        private int CodPessoaAtual => Convert.ToInt32(Session["CodPessoa"] ?? 0);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (CodDemanda > 0)
                {
                    CarregarDemanda();
                    CarregarAnexos();
                }
                else
                {
                    Response.Redirect("listDemand.aspx");
                }
            }
        }

        private void CarregarDemanda()
        {
            var demanda = _service.ObterDemandaDetalhesPorId(CodDemanda);
            if (demanda != null)
            {
                // Preencher os dados básicos
                lblCodDemanda.Text = demanda.CodDemanda.ToString();
                lblTitulo.Text = demanda.Titulo;
                lblTexto.Text = demanda.TextoDemanda;
                lblSolicitante.Text = demanda.Solicitante;
                lblDataSolicitacao.Text = demanda.DataSolicitacao?.ToString("dd/MM/yyyy HH:mm") ?? "N/A";

                // NOVOS CAMPOS
                lblCategoria.Text = demanda.Categoria ?? "-";
                lblPrioridade.Text = demanda.Prioridade ?? "-";
                lblImportancia.Text = demanda.Importancia ?? "N/A";
                lblPrazo.Text = demanda.DataPrazo ?? "-";

                // Verificar se pode aceitar
                VerificarPermissaoAceitar(demanda);
            }
        }

        private void CarregarAnexos()
        {
            var anexos = _service.GetAnexosDemanda(CodDemanda);
            if (anexos != null && anexos.Any())
            {
                rptAnexos.DataSource = anexos.Select(a => new
                {
                    NomeArquivo = a.NomeArquivo,
                    DataEnvio = a.DataEnvio,
                    TamanhoFormatado = FormatTamanhoArquivo(a.TamanhoBytes),
                    CaminhoDownload = $"/public/uploadgestao/docs/{a.NomeArquivo}",
                    NomeUsuarioUpload = a.NomeUsuarioUpload
                }).ToList();

                rptAnexos.DataBind();
                lblSemAnexos.Visible = false;
            }
            else
            {
                rptAnexos.Visible = false;
                lblSemAnexos.Visible = true;
            }
        }

        private void VerificarPermissaoAceitar(DemandaDetalhesDto demanda)
        {
            // Mantém a mesma lógica, mas agora recebe DemandaDetalhesDto
            if (demanda.StatusCodigo == 1) // Status "Aberta"
            {
                btnAceitarDemanda.Enabled = true;
                btnAceitarDemanda.CssClass = "btn-accept";
            }
            else
            {
                btnAceitarDemanda.Enabled = false;
                btnAceitarDemanda.CssClass = "btn-accept btn-secondary";
            }
        }

        protected void btnAceitarDemanda_Click(object sender, EventArgs e)
        {
            try
            {
                // Aceitar a demanda
                bool aceitou = _service.AceitarDemanda(CodDemanda, CodPessoaAtual);

                if (aceitou)
                {
                    // Redirecionar para a página de detalhes completa
                    Response.Redirect($"detailDemand.aspx?codDemanda={CodDemanda}");
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, GetType(), "ErroAceitar",
                        "showToastErro('Erro ao aceitar a demanda. Tente novamente.');", true);
                }
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, GetType(), "ErroAceitar",
                    $"showToastErro('Erro: {ex.Message}');", true);
            }
        }

        protected void btnVoltar_Click(object sender, EventArgs e)
        {
            Response.Redirect("listDemand.aspx");
        }

        private string FormatTamanhoArquivo(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}