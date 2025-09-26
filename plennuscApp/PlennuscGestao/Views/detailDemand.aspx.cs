using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Service.ServiceGestao;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class detailDemand : System.Web.UI.Page
    {
        private readonly DemandaService _service = new DemandaService("Plennus");
        private int CodDemanda => Convert.ToInt32(Request.QueryString["codDemanda"]);
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
            CarregarAnexos();
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
            bool ehExecutor = demandaAtual.CodPessoaExecucao.HasValue && demandaAtual.CodPessoaExecucao.Value == codUsuario;
            bool jaTemAprovador = demandaAtual.CodPessoaAprovacao.HasValue && demandaAtual.CodPessoaAprovacao.Value > 0;

            // Status que consideram a demanda como "fechada"
            bool demandaFechada = (statusCodigo == 22 || statusCodigo == 23 || statusCodigo == 65); // Finalizada, Concluída ou Aguardando Aprovação

            //// Só mostra botão de encerrar para quem abriu E se status for "Em andamento"
            //bool podeEncerrar = ehSolicitante && (statusCodigo == 18);

            // Só aparece o botão de encerrar para quem abriu a demanda E se não estiver concluída
            bool podeEncerrar = (demandaAtual.CodPessoaSolicitacao == CodPessoaAtual)
                               && (demandaAtual.StatusCodigo != 23);
            btnEncerrar.Visible = podeEncerrar;

            // Mostrar o botão de "Solicitar Aprovação" para quem é executor da demanda,
            // e apenas se: não tem aprovador, não está fechada E status é "Em andamento" (18)
            bool podeSolicitarAprovacao = ehExecutor && !jaTemAprovador && !demandaFechada && (statusCodigo == 18);
            btnSolicitarAprovacao.Visible = podeSolicitarAprovacao;

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
                    "showToastErro('Esta demanda está fechada. Não é possível adicionar acompanhamentos.');", true);
                return;
            }

            var texto = txtNovoAcompanhamento.Text?.Trim();
            if (!string.IsNullOrEmpty(texto))
            {
                try
                {
                    // SALVA O ACOMPANHAMENTO E OBTÉM O ID
                    int idAcompanhamento = _service.InserirAcompanhamento(CodDemanda, CodPessoaAtual, texto);

                    // Salvar anexos se houver
                    if (fuAnexos.HasFiles)
                    {
                        int arquivosSalvos = 0;
                        int arquivosComErro = 0;

                        foreach (HttpPostedFile arquivo in fuAnexos.PostedFiles)
                        {
                            try
                            {
                                // Validar tamanho (10MB máximo)
                                if (arquivo.ContentLength > 10 * 1024 * 1024)
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastArquivoTamanho",
                                        "showToastErro('O arquivo " + arquivo.FileName.Replace("'", "\\'") + " excede o limite de 10MB.');", true);
                                    arquivosComErro++;
                                    continue;
                                }

                                // Validar extensão
                                string extensao = Path.GetExtension(arquivo.FileName).ToLower();
                                string[] extensoesPermitidas = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".gif" };

                                if (!extensoesPermitidas.Contains(extensao))
                                {
                                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastArquivoTipo",
                                        "showToastErro('Tipo de arquivo não permitido: " + arquivo.FileName.Replace("'", "\\'") + "');", true);
                                    arquivosComErro++;
                                    continue;
                                }

                                // Salva o arquivo físico (MÉTODO QUE JÁ EXISTE)
                                string nomeArquivoSalvo = _service.SalvarAnexoFisico(arquivo, CodDemanda);

                                // Lê o conteúdo para salvar no banco
                                byte[] conteudo;
                                using (MemoryStream ms = new MemoryStream())
                                {
                                    arquivo.InputStream.CopyTo(ms);
                                    conteudo = ms.ToArray();
                                }

                                // Dentro do loop dos arquivos, depois de salvar o arquivo físico:
                                _service.SalvarAnexoAcompanhamento(CodDemanda, idAcompanhamento, nomeArquivoSalvo);
                            }
                            catch (Exception exArquivo)
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastArquivoErro",
                                    "showToastErro('Erro ao processar " + arquivo.FileName.Replace("'", "\\'") + ": " + exArquivo.Message.Replace("'", "\\'") + "');", true);
                                arquivosComErro++;
                            }
                        }

                        // Feedback sobre os anexos processados
                        if (arquivosSalvos > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastArquivosSucesso",
                                "showToastSucesso('" + arquivosSalvos + " arquivo(s) anexado(s) com sucesso!');", true);
                        }

                        if (arquivosComErro > 0)
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastArquivosErro",
                                "showToastAviso('" + arquivosComErro + " arquivo(s) não puderam ser anexados.');", true);
                        }
                    }

                    CarregarAnexos();

                    // Mensagem de sucesso do acompanhamento
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastSucesso",
                        "showToastSucesso('Acompanhamento adicionado com sucesso!');", true);

                    // LIMPA OS CAMPOS
                    txtNovoAcompanhamento.Text = string.Empty;

                    // Limpar arquivos selecionados
                    fuAnexos.Attributes.Clear();
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "LimparArquivos",
                        "selectedFiles = []; updateFilePreview();", true);

                    // RECARREGA OS DADOS
                    CarregarAcompanhamentos();

                }
                catch (Exception ex)
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastErro",
                        "showToastErro('Erro ao adicionar acompanhamento: " + ex.Message.Replace("'", "\\'") + "');", true);
                }
            }
        }

        protected void btnEncerrar_Click(object sender, EventArgs e)
        {
            // MUDA STATUS PARA "CONCLUÍDA" (23) - DEMANDA FINALIZADA
            _service.AtualizarStatusComHistorico(CodDemanda, 23, CodPessoaAtual);
            Response.Redirect("listDemand.aspx");
        }

        private void CarregarAnexos()
        {
            if (CodDemanda > 0)
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
                    rptAnexos.Visible = true;
                }
                else
                {
                    rptAnexos.Visible = false;
                    lblSemAnexos.Visible = true;
                }
            }
            else
            {
                rptAnexos.Visible = false;
                lblSemAnexos.Visible = true;
            }
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

        protected void btnSolicitarAprovacao_Click(object sender, EventArgs e)
        {
            try
            {
                int gestorId;
                var ok = _service.SolicitarAprovacaoDemanda(CodDemanda, CodPessoaAtual, out gestorId);
                if (ok)
                {
                    if (gestorId > 0)
                        MostrarMensagem($"Solicitação enviada para o gestor (ID {gestorId}).", "success");
                    else
                        MostrarMensagem("Solicitação enviada.", "success");

                    // Recarrega demanda e UI
                    demandaAtual = _service.ObterDemandaPorId(CodDemanda);
                    CarregarDemanda();
                    AjustarBotoes();
                }
                else
                {
                    MostrarMensagem("Não foi possível localizar um gestor para este setor.", "error");
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro ao solicitar aprovação: " + ex.Message, "error");
            }
        }

        private void MostrarMensagem(string mensagem, string tipo)
        {
            string functionName = tipo == "success" ? "showToastSucesso" : "showToastErro";
            string script = $@"{functionName}('{mensagem.Replace("'", "\\'")}');";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "MensagemToast", script, true);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "MensagemToast", script, true);
        }

        protected void btnRecusar_Click(object sender, EventArgs e)
        {

        }
    }
}