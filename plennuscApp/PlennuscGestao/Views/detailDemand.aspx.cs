using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Models.ModelsGestao.modelsAnnex;
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
        private int CodDemanda
        {
            get
            {
                return Convert.ToInt32(Session["CurrentDemandId"] ?? "0");
            }
        }
        private int CodPessoaAtual => Convert.ToInt32(Session["CodPessoa"] ?? 0);
        private bool DemandaFechada => (demandaAtual?.StatusCodigo == 23); // 23 = Status "Concluída"

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarTudo();
                ConfigurarFormularioAcompanhamento();
                AtualizarListaAnexos();
            }
        }


        private List<temporaryAnnex> AnexosSelecionados
        {
            get
            {
                if (ViewState["AnexosSelecionados"] == null)
                    ViewState["AnexosSelecionados"] = new List<temporaryAnnex>();
                return (List<temporaryAnnex>)ViewState["AnexosSelecionados"];
            }
            set { ViewState["AnexosSelecionados"] = value; }
        }

        private void CarregarTudo()
        {
            CarregarDemanda();
            CarregarHistorico();
            CarregarAcompanhamentos();
            AjustarBotoes();
            CarregarAnexos();
            CarregarStatusAcompanhamento();
        }

        private DemandaDto demandaAtual;

        private void CarregarStatusAcompanhamento()
        {
            ddlStatusAcompanhamento.DataSource = _service.GetStatusDemandaFiltrados();
            ddlStatusAcompanhamento.DataValueField = "Value";
            ddlStatusAcompanhamento.DataTextField = "Text";
            ddlStatusAcompanhamento.DataBind();

            // Define o status atual como selecionado
            var demanda = _service.ObterDemandaDetalhesPorId(CodDemanda);
            if (demanda != null && demanda.StatusCodigo.HasValue)
            {
                string statusAtual = demanda.StatusCodigo.Value.ToString();
                ddlStatusAcompanhamento.SelectedValue = demanda.StatusCodigo.Value.ToString();
                hdnStatusOriginal.Value = statusAtual;
            }
        }
        private void CarregarDemanda()
        {
            demandaAtual = _service.ObterDemandaPorId(CodDemanda);
            if (demandaAtual != null)
            {
                lblTitulo.Text = demandaAtual.Titulo;
                lblTexto.Text = demandaAtual.TextoDemanda;

                // ✅ CORREÇÃO: Usar o StatusNome que vem do banco
                if (!string.IsNullOrEmpty(demandaAtual.StatusNome))
                {
                    lblStatusBadge.Text = demandaAtual.StatusNome;

                    // ✅ CORREÇÃO: Aplicar classe CSS baseada no status real
                    if (demandaAtual.StatusNome.ToLower().Contains("fechada") ||
                        demandaAtual.StatusNome.ToLower().Contains("concluída") ||
                        demandaAtual.StatusNome.ToLower().Contains("concluida"))
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
                    // Fallback caso não tenha status
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

            btnRecusar.Visible = true;
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

        // MÉTODOS PARA ANEXOS
        // MÉTODOS PARA ANEXOS
        protected void rptAnexos_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Remover")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                if (index >= 0 && index < AnexosSelecionados.Count)
                {
                    AnexosSelecionados.RemoveAt(index);
                    AtualizarListaAnexos();
                    MostrarMensagem("Arquivo removido com sucesso!", "success");
                }
            }
        }

        protected void btnAdicionarAnexos_Click(object sender, EventArgs e)
        {
            if (fuAnexos.HasFiles)
            {
                foreach (HttpPostedFile arquivo in fuAnexos.PostedFiles)
                {
                    // Validação de tamanho
                    if (arquivo.ContentLength > 10 * 1024 * 1024) // 10MB
                    {
                        MostrarMensagem($"O arquivo {arquivo.FileName} excede o limite de 10MB.", "error");
                        continue;
                    }

                    // Validação de tipo
                    string extensao = Path.GetExtension(arquivo.FileName).ToLower();
                    string[] extensoesPermitidas = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".gif" };

                    if (!extensoesPermitidas.Contains(extensao))
                    {
                        MostrarMensagem($"Tipo de arquivo não permitido: {arquivo.FileName}", "error");
                        continue;
                    }

                    // Adiciona à lista temporária
                    var anexo = new temporaryAnnex
                    {
                        FileName = arquivo.FileName,
                        Size = arquivo.ContentLength,
                        SizeFormatted = FormatFileSize(arquivo.ContentLength),
                        ContentType = arquivo.ContentType,
                        Index = AnexosSelecionados.Count
                    };

                    // Lê o conteúdo do arquivo
                    using (MemoryStream ms = new MemoryStream())
                    {
                        arquivo.InputStream.CopyTo(ms);
                        anexo.Content = ms.ToArray();
                    }

                    AnexosSelecionados.Add(anexo);
                }

                // Atualiza a lista
                AtualizarListaAnexos();

                // Limpa o FileUpload
                fuAnexos.Attributes.Clear();

                MostrarMensagem($"{fuAnexos.PostedFiles.Count} arquivo(s) adicionado(s) com sucesso!", "success");
            }
            else
            {
                MostrarMensagem("Selecione pelo menos um arquivo para adicionar.", "info");
            }
        }

        private void AtualizarListaAnexos()
        {
            // CORREÇÃO: Use o Repeater correto - você tem Repeater1 no HTML
            Repeater1.DataSource = AnexosSelecionados;
            Repeater1.DataBind();

            // Mostra/oculta o container baseado na quantidade
            if (AnexosSelecionados.Count > 0)
            {
                Repeater1.Visible = true;
            }
            else
            {
                Repeater1.Visible = false;
            }
        }

        // Método para formatar tamanho do arquivo
        private string FormatFileSize(long bytes)
        {
            if (bytes == 0) return "0 Bytes";
            string[] sizes = { "Bytes", "KB", "MB", "GB" };
            int i = (int)Math.Floor(Math.Log(bytes) / Math.Log(1024));
            return Math.Round(bytes / Math.Pow(1024, i), 2) + " " + sizes[i];
        }

        // Método auxiliar para salvar arquivo do byte array
        private string SalvarAnexoFisicoTemp(string fileName, byte[] content, int codDemanda)
        {
            try
            {
                string pastaAnexos = HttpContext.Current.Server.MapPath("~/public/uploadgestao/docs/");

                if (!Directory.Exists(pastaAnexos))
                {
                    Directory.CreateDirectory(pastaAnexos);
                }

                string nomeUnico = $"{codDemanda}_{DateTime.Now:yyyyMMddHHmmss}_{Path.GetFileName(fileName)}";
                string caminhoCompleto = Path.Combine(pastaAnexos, nomeUnico);

                // Salva o conteúdo do byte array no arquivo
                File.WriteAllBytes(caminhoCompleto, content);

                return nomeUnico;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao salvar arquivo {fileName}: {ex.Message}");
            }
        }

        protected void btnAdicionarAcompanhamento_Click(object sender, EventArgs e)
        {
            // Verificar se a demanda está fechada
            if (DemandaFechada)
            {
                MostrarMensagem("Esta demanda está fechada. Não é possível adicionar acompanhamentos.", "error");
                return;
            }

            var texto = txtNovoAcompanhamento.Text?.Trim();

            // CORREÇÃO: Verificar se o dropdown tem valor selecionado
            if (string.IsNullOrEmpty(ddlStatusAcompanhamento.SelectedValue))
            {
                MostrarMensagem("Selecione um status para o acompanhamento.", "error");
                return;
            }

            int novoStatus = int.Parse(ddlStatusAcompanhamento.SelectedValue);

            if (!string.IsNullOrEmpty(texto))
            {
                try
                {
                    int idAcompanhamento = _service.InserirAcompanhamento(
                        CodDemanda,
                        CodPessoaAtual,
                        texto,
                        novoStatus);

                    // SALVAR ANEXOS DA LISTA TEMPORÁRIA (usando sua estrutura)
                    if (AnexosSelecionados.Count > 0)
                    {
                        int arquivosSalvos = 0;
                        int arquivosComErro = 0;

                        foreach (var anexo in AnexosSelecionados)
                        {
                            try
                            {
                                // Salva o arquivo fisicamente
                                string nomeArquivoSalvo = SalvarAnexoFisicoTemp(anexo.FileName, anexo.Content, CodDemanda);

                                // USA SEU MÉTODO EXISTENTE - só passa o nome do arquivo
                                _service.SalvarAnexoAcompanhamento(CodDemanda, idAcompanhamento, nomeArquivoSalvo);

                                arquivosSalvos++;
                            }
                            catch (Exception exArquivo)
                            {
                                MostrarMensagem($"Erro ao processar {anexo.FileName}: {exArquivo.Message}", "error");
                                arquivosComErro++;
                            }
                        }

                        //// Feedback sobre os anexos processados
                        //if (arquivosSalvos > 0)
                        //{
                        //    MostrarMensagem($"{arquivosSalvos} arquivo(s) anexado(s) com sucesso!", "success");
                        //}

                        //if (arquivosComErro > 0)
                        //{
                        //    MostrarMensagem($"{arquivosComErro} arquivo(s) não puderam ser anexados.", "warning");
                        //}

                        // LIMPA A LISTA DE ANEXOS APÓS SALVAR
                        AnexosSelecionados.Clear();
                        AtualizarListaAnexos();
                    }

                    hdnStatusOriginal.Value = novoStatus.ToString();

                    CarregarAnexos();

                    // Mensagem de sucesso do acompanhamento
                    MostrarMensagem("Acompanhamento adicionado com sucesso!", "success");

                    // LIMPA OS CAMPOS
                    txtNovoAcompanhamento.Text = string.Empty;

                    // Limpar FileUpload
                    fuAnexos.Attributes.Clear();

                    // RECARREGA OS DADOS
                    CarregarAcompanhamentos();
                    CarregarDemanda();
                }
                catch (Exception ex)
                {
                    MostrarMensagem($"Erro ao adicionar acompanhamento: {ex.Message}", "error");
                }
            }
            else
            {
                MostrarMensagem("Digite o texto do acompanhamento.", "error");
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
            try
            {
                int codDemanda = Convert.ToInt32(Session["CurrentDemandId"] ?? "0");
                int codPessoaAtual = Convert.ToInt32(Session["CodPessoa"]);

                // Verificar se a demanda existe
                var demanda = _service.GetDemandaPorCodigo(codDemanda);

                if (demanda == null)
                {
                    MostrarMensagem("Demanda não encontrada.", "error");
                    return;
                }

                // Verificar se o usuário atual é o executor da demanda
                if (demanda.CodPessoaExecucao != codPessoaAtual)
                {
                    MostrarMensagem("Você não tem permissão para recusar esta demanda.", "error");
                    return;
                }

                // Verificar se a demanda já não está recusada pelo status
                if (demanda.Status.ToLower().Contains("recusada"))
                {
                    MostrarMensagem("Esta demanda já está recusada.", "error");
                    return;
                }

                // Recusar a demanda
                _service.RecusarDemanda(codDemanda, codPessoaAtual, demanda.CodEstr_SituacaoDemanda);

                MostrarMensagem("Demanda recusada com sucesso!", "success");

                CarregarDemanda();
            }
            catch (Exception ex)
            {
                MostrarMensagem("Erro ao recusar demanda: " + ex.Message, "error");
            }
        }

        protected bool IsMyMessage(int codPessoaAcompanhamento)
        {
            return codPessoaAcompanhamento == CodPessoaAtual;
        }
    }
}