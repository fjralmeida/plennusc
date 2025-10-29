using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Service.ServiceGestao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
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

                // ✅ CORREÇÃO: Atualizar o status badge
                if (!string.IsNullOrEmpty(demanda.StatusNome))
                {
                    lblStatusBadge.Text = demanda.StatusNome;

                    // Aplicar classe CSS baseada no status
                    if (demanda.StatusNome.ToLower().Contains("fechada") ||
                        demanda.StatusNome.ToLower().Contains("concluída") ||
                        demanda.StatusNome.ToLower().Contains("concluida"))
                    {
                        lblStatusBadge.CssClass = "status-badge status-closed";
                    }
                    else
                    {
                        lblStatusBadge.CssClass = "status-badge status-open";
                    }
                }
                else
                {
                    // Fallback
                    lblStatusBadge.Text = "Aberta";
                    lblStatusBadge.CssClass = "status-badge status-open";
                }

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

            var attachmentsSection = (HtmlGenericControl)FindControl("attachmentsSection");

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

                // MOSTRAR a seção de anexos
                if (attachmentsSection != null)
                    attachmentsSection.Visible = true;
            }
            else
            {
                rptAnexos.Visible = false;
                lblSemAnexos.Visible = true;

                // OCULTAR a seção de anexos
                if (attachmentsSection != null)
                    attachmentsSection.Visible = false;
            }
        }

        private void VerificarPermissaoAceitar(DemandaDetalhesDto demanda)
        {
            if (demanda.StatusCodigo == 17) // Status "Aberta"
            {
                btnAceitarDemanda.Visible = true;
                btnAceitarDemanda.CssClass = "btn-accept";
            }
            if (demanda.CodPessoaSolicitacao == CodPessoaAtual)
            {
                btnAceitarDemanda.Visible = false;
            }
            else
            {
                btnAceitarDemanda.Visible = false;
            }
        }

        protected void btnAceitarDemanda_Click1(object sender, EventArgs e)
        {
            try
            {
                // Aceitar a demanda
                bool aceitou = _service.AceitarDemanda(CodDemanda, CodPessoaAtual);

                if (aceitou)
                {
                    // Redirecionar para a página de detalhes completa
                    Response.Redirect($"detailDemand.aspx?codDemanda={CodDemanda}", false);
                    Context.ApplicationInstance.CompleteRequest(); // Opcional
                    return; // Para a execução
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