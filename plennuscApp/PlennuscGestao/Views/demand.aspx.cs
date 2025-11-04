using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Models.ModelsGestao.modelsAnnex;
using Plennusc.Core.Service.ServiceGestao;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class demand : Page
    {
        private readonly DemandaService _svc = new DemandaService("Plennus");

        private int CodPessoaAtual => Convert.ToInt32(Session["CodPessoa"] ?? 0);

        private int? CodDepartamentoAtual =>
            Session["CodDepartamento"] == null ? (int?)null : Convert.ToInt32(Session["CodDepartamento"]);

        private List<DemandaComLimite> PrioridadesComLimites;

        // Lista para armazenar os anexos temporários
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblNomeUser.Text = Session["NomeUsuario"]?.ToString() ?? "Usuário não identificado";
                BindAll();

                if (!string.IsNullOrEmpty(ddlOrigem.SelectedItem.Text))
                {
                    lblSetorUsuario.Text = ddlOrigem.SelectedItem.Text;
                }

                divPrazo.Style["display"] = "none";
                CarregarNiveisImportancia();
                AtualizarListaAnexos();
            }
            else
            {
                PrioridadesComLimites = _svc.GetPrioridadesComLimites();
                AtualizarListaAnexos(); // ATUALIZA A CADA POSTBACK
            }
        }

        private void BindAll()
        {
            ddlOrigem.DataSource = _svc.GetDepartamentoUsuario(CodPessoaAtual);
            ddlOrigem.DataValueField = "Value";
            ddlOrigem.DataTextField = "Text";
            ddlOrigem.DataBind();

            if (ddlOrigem.Items.Count == 2)
            {
                ddlOrigem.SelectedIndex = 1;
            }

            ddlDestino.DataSource = _svc.GetDepartamentos();
            ddlDestino.DataValueField = "Value";
            ddlDestino.DataTextField = "Text";
            ddlDestino.DataBind();

            PrioridadesComLimites = _svc.GetPrioridadesComLimites();
            ddlPrioridade.DataSource = PrioridadesComLimites;
            ddlPrioridade.DataValueField = "Value";
            ddlPrioridade.DataTextField = "Text";
            ddlPrioridade.DataBind();

            if (ddlPrioridade.Items.Count > 0 && ddlPrioridade.Items[0].Value != "")
            {
                ddlPrioridade.Items.Insert(0, new ListItem("Selecione a prioridade", ""));
            }

            if (CodDepartamentoAtual.HasValue)
            {
                var it = ddlOrigem.Items.FindByValue(CodDepartamentoAtual.Value.ToString());
                if (it != null) ddlOrigem.SelectedValue = it.Value;
            }

            BindGrupos();
        }

        protected void ddlPrioridade_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidarLimitePrioridade();
            try { upPrioridade.Update(); } catch { }
        }

        private bool ValidarLimitePrioridade()
        {
            if (string.IsNullOrEmpty(ddlPrioridade.SelectedValue) || PrioridadesComLimites == null)
                return true;

            if (!int.TryParse(ddlPrioridade.SelectedValue, out int prioridadeSelecionada))
                return true;

            var prioridade = PrioridadesComLimites.FirstOrDefault(p => (int)p.Value == prioridadeSelecionada);

            if (prioridade != null && prioridade.Limite < 999)
            {
                int demandasExistentes = _svc.ContarDemandasPorPrioridade(CodPessoaAtual, prioridadeSelecionada);

                if (demandasExistentes >= prioridade.Limite)
                {
                    MostrarMensagem($"Você já atingiu o limite máximo de {prioridade.Limite} demanda(s) com prioridade '{prioridade.Text}'.", "warning");

                    if (prioridadeSelecionada == 33)
                    {
                        CarregarDemandasGenerica(isCritica: true);
                    }
                    else if (prioridadeSelecionada == 32)
                    {
                        CarregarDemandasGenerica(isCritica: false);
                    }

                    ddlPrioridade.ClearSelection();
                    divPrazo.Style["display"] = "none";
                    return false;
                }

                if (prioridadeSelecionada == 33 || prioridadeSelecionada == 32)
                {
                    divPrazo.Style["display"] = "block";
                    txtPrazo.Attributes["required"] = "required";
                }
                else
                {
                    divPrazo.Style["display"] = "none";
                    txtPrazo.Attributes.Remove("required");
                }
            }
            else
            {
                divPrazo.Style["display"] = "none";
                txtPrazo.Attributes.Remove("required");
            }

            return true;
        }

        private void CarregarDemandasGenerica(bool isCritica)
        {
            try
            {
                List<DemandaCriticaInfo> demandas;
                if (isCritica)
                    demandas = _svc.GetDemandasCriticasAbertas(CodPessoaAtual);
                else
                    demandas = _svc.GetDemandasAltasAbertas(CodPessoaAtual);

                if (demandas == null || !demandas.Any())
                {
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(),
                        "console.log('CarregarDemandasGenerica: lista vazia');", true);
                    return;
                }

                rptDemandas.DataSource = demandas;
                rptDemandas.DataBind();
                upModalDemandas.Update();

                if (isCritica)
                {
                    litTituloModal.Text = "<i class='bi bi-exclamation-triangle'></i> Você possui demandas CRÍTICAS em aberto";
                    litTextoModal.Text = "<p>Para criar uma nova demanda CRÍTICA, você precisa fechar ou alterar a situação de uma das demandas existentes:</p>";
                }
                else
                {
                    litTituloModal.Text = "<i class='bi bi-exclamation-triangle'></i> Você possui demandas ALTAS em aberto";
                    litTextoModal.Text = "<p>Para criar uma nova demanda ALTA, você precisa fechar ou alterar a situação de uma das demandas existentes:</p>";
                }

                string key = isCritica ? "AbrirModalCriticas" : "AbrirModalAltas";
                string script = @"
            (function() {
                try {
                    var modalEl = document.getElementById('modalDemandas');
                    if (!modalEl) return;
                    if (window.bootstrap && typeof bootstrap.Modal !== 'undefined' && typeof bootstrap.Modal.getOrCreateInstance === 'function') {
                        var inst = bootstrap.Modal.getOrCreateInstance(modalEl);
                        inst.show();
                        return;
                    }
                    if (window.bootstrap && typeof bootstrap.Modal !== 'undefined') {
                        var m = new bootstrap.Modal(modalEl);
                        m.show();
                        return;
                    }
                    var btn = document.getElementById('btnAbrirModalHidden');
                    if (btn) btn.click();
                } catch(e) {
                    console.error('abrir modal erro', e);
                }
            })();
        ";

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao carregar demandas: {ex.Message}", "warning");
            }
        }

        private void CarregarNiveisImportancia()
        {
            try
            {
                var importancias = _svc.GetNiveisImportancia();
                ddlImportancia.DataSource = importancias;
                ddlImportancia.DataValueField = "Value";
                ddlImportancia.DataTextField = "Text";
                ddlImportancia.DataBind();

                if (ddlImportancia.Items.Count > 0)
                    ddlImportancia.Items.Insert(0, new ListItem("Selecione a importância (opcional)", ""));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar importâncias: {ex.Message}");
            }
        }

        protected void rptDemandas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DropDownList ddlNovaPrioridade = (DropDownList)e.Item.FindControl("ddlNovaPrioridade");

                if (ddlNovaPrioridade != null && PrioridadesComLimites != null)
                {
                    var prioridadesPermitidas = PrioridadesComLimites
                        .Where(p => p.Value != 33 && p.Value != 32)
                        .ToList();

                    ddlNovaPrioridade.DataSource = prioridadesPermitidas;
                    ddlNovaPrioridade.DataValueField = "Value";
                    ddlNovaPrioridade.DataTextField = "Text";
                    ddlNovaPrioridade.DataBind();

                    ddlNovaPrioridade.Items.Insert(0, new ListItem("Selecionar nova prioridade", ""));
                }
            }
        }

        protected void btnAlterarPrioridade_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            RepeaterItem item = (RepeaterItem)btn.NamingContainer;
            DropDownList ddlNovaPrioridade = (DropDownList)item.FindControl("ddlNovaPrioridade");

            if (string.IsNullOrEmpty(ddlNovaPrioridade.SelectedValue))
            {
                MostrarMensagem("Selecione uma nova prioridade.", "info");
                return;
            }

            int codDemanda = Convert.ToInt32(btn.CommandArgument);
            int novaPrioridade = Convert.ToInt32(ddlNovaPrioridade.SelectedValue);

            try
            {
                bool sucesso = _svc.AtualizarPrioridadeDemanda(codDemanda, novaPrioridade);

                if (sucesso)
                {
                    MostrarMensagem("Demanda alterada com sucesso!", "success");

                    bool isCritica = litTituloModal.Text.Contains("CRÍTICAS");
                    List<DemandaCriticaInfo> demandasRemanescentes = isCritica
                        ? _svc.GetDemandasCriticasAbertas(CodPessoaAtual)
                        : _svc.GetDemandasAltasAbertas(CodPessoaAtual);

                    if (demandasRemanescentes != null && demandasRemanescentes.Any())
                    {
                        CarregarDemandasGenerica(isCritica);
                    }
                    else
                    {
                        string fecharScript = @"
                            (function(){
                                try {
                                    var modalEl = document.getElementById('modalDemandas');
                                    if (modalEl) {
                                        if (window.bootstrap && typeof bootstrap.Modal !== 'undefined') {
                                            var inst = bootstrap.Modal.getInstance(modalEl);
                                            if (inst) {
                                                inst.hide();
                                            } else {
                                                modalEl.style.display = 'none';
                                            }
                                        } 
                                        else {
                                            modalEl.style.display = 'none';
                                            modalEl.classList.remove('show');
                                        }
                                    }
                                    
                                    var backdrops = document.querySelectorAll('.modal-backdrop');
                                    backdrops.forEach(function(backdrop) {
                                        backdrop.remove();
                                    });
                                    
                                    document.body.classList.remove('modal-open');
                                    document.body.style.overflow = 'auto';
                                    document.body.style.paddingRight = '';
                                    
                                } catch(e) { 
                                    console.error('Erro ao fechar modal:', e);
                                    document.querySelectorAll('.modal-backdrop').forEach(el => el.remove());
                                    document.querySelectorAll('.modal').forEach(el => el.style.display = 'none');
                                    document.body.classList.remove('modal-open');
                                }
                            })();
                        ";
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "FecharModalScript", fecharScript, true);
                    }
                }
                else
                {
                    MostrarMensagem("Erro ao alterar a prioridade da demanda.", "warning");
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro: {ex.Message}", "error");
            }
        }

        protected void rptDemandasCriticas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DropDownList ddlSituacao = (DropDownList)e.Item.FindControl("ddlSituacao");

                if (ddlSituacao != null)
                {
                    var situacoesFechamento = _svc.GetSituacoesParaFechamento();
                    ddlSituacao.DataSource = situacoesFechamento;
                    ddlSituacao.DataValueField = "Value";
                    ddlSituacao.DataTextField = "Text";
                    ddlSituacao.DataBind();
                    ddlSituacao.Items.Insert(0, new ListItem("-- Selecionar --", ""));
                }
            }
        }

        private void BindGrupos()
        {
            int? codSetorDestino = null;

            if (!string.IsNullOrEmpty(ddlDestino.SelectedValue) && ddlDestino.SelectedValue != "")
            {
                codSetorDestino = int.Parse(ddlDestino.SelectedValue);
            }

            ddlTipoGrupo.DataSource = _svc.GetTiposDemandaGrupos(codSetorDestino);
            ddlTipoGrupo.DataValueField = "Value";
            ddlTipoGrupo.DataTextField = "Text";
            ddlTipoGrupo.DataBind();

            if (ddlTipoGrupo.Items.Count > 0)
                ddlTipoGrupo.SelectedIndex = 0;

            BindSubtiposDoGrupo();
        }

        private void BindSubtiposDoGrupo()
        {
            ddlTipoDetalhe.Items.Clear();

            if (!int.TryParse(ddlTipoGrupo.SelectedValue, out var codGrupo) || codGrupo <= 0)
                return;

            ddlTipoDetalhe.DataSource = _svc.GetSubtiposDemanda(codGrupo);
            ddlTipoDetalhe.DataValueField = "Value";
            ddlTipoDetalhe.DataTextField = "Text";
            ddlTipoDetalhe.DataBind();

            if (ddlTipoDetalhe.Items.Count > 0)
                ddlTipoDetalhe.Items.Insert(0, new ListItem("— selecionar —", ""));
        }

        protected void ddlDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlDestino.SelectedValue))
            {
                int codSetor = Convert.ToInt32(ddlDestino.SelectedValue);
                CarregarCategoriasPorSetor(codSetor);
            }
            else
            {
                ddlTipoGrupo.Items.Clear();
                ddlTipoGrupo.Items.Add(new ListItem("Selecione uma categoria", ""));
                ddlTipoDetalhe.Items.Clear();
                ddlTipoDetalhe.Items.Add(new ListItem("Selecione um subtipo", ""));
            }

            try { upRoteamento.Update(); } catch { }
            try { upCategoria.Update(); } catch { }
        }

        private void CarregarCategoriasPorSetor(int codSetor)
        {
            try
            {
                var categorias = _svc.GetCategoriasPorSetor(codSetor);

                ddlTipoGrupo.DataSource = categorias;
                ddlTipoGrupo.DataValueField = "CodEstrutura";
                ddlTipoGrupo.DataTextField = "DescEstrutura";
                ddlTipoGrupo.DataBind();

                ddlTipoDetalhe.Items.Clear();
                ddlTipoDetalhe.Items.Add(new ListItem("Selecione um subtipo", ""));

                if (categorias.Count == 0)
                {
                    MostrarMensagem("Nenhuma categoria encontrada para este setor.", "info");
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao carregar categorias: {ex.Message}", "error");
            }
        }

        protected void ddlTipoGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlTipoGrupo.SelectedValue))
            {
                int codEstruturaPai = Convert.ToInt32(ddlTipoGrupo.SelectedValue);
                CarregarSubtiposDaCategoria(codEstruturaPai);
            }
            else
            {
                ddlTipoDetalhe.Items.Clear();
                ddlTipoDetalhe.Items.Add(new ListItem("Selecione um subtipo", ""));
            }

            try { upCategoria.Update(); } catch { }
        }

        private void CarregarSubtiposDaCategoria(int codEstruturaPai)
        {
            try
            {
                var subtipos = _svc.GetSubtiposPorCategoria(codEstruturaPai);

                ddlTipoDetalhe.DataSource = subtipos;
                ddlTipoDetalhe.DataValueField = "CodEstrutura";
                ddlTipoDetalhe.DataTextField = "DescEstrutura";
                ddlTipoDetalhe.DataBind();

                if (subtipos.Count == 0)
                {
                    ddlTipoDetalhe.Items.Insert(0, new ListItem("— Nenhum subtipo disponível —", ""));
                }
                else
                {
                    ddlTipoDetalhe.Items.Insert(0, new ListItem("— selecionar —", ""));
                }
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao carregar subtipos: {ex.Message}", "error");
            }
        }

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
        }

        private void AtualizarListaAnexos()
        {
            rptAnexos.DataSource = AnexosSelecionados;
            rptAnexos.DataBind();

            // Mostra/oculta o container baseado na quantidade
            if (AnexosSelecionados.Count > 0)
            {
                rptAnexos.Visible = true;
            }
            else
            {
                rptAnexos.Visible = false;
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

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            if (!ValidarLimitePrioridade())
                return;

            if (CodPessoaAtual == 0)
            {
                MostrarMensagem("Sessão inválida.", "error");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTitulo.Text))
            {
                MostrarMensagem("Informe o título.", "error");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ScrollTitulo",
                    $"document.getElementById('{txtTitulo.ClientID}').scrollIntoView({{ behavior: 'smooth', block: 'center' }});", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescricao.Text))
            {
                MostrarMensagem("Informe a descrição.", "error");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ScrollDescricao",
                    $"document.getElementById('{txtDescricao.ClientID}').scrollIntoView({{ behavior: 'smooth', block: 'center' }});", true);
                return;
            }

            if (string.IsNullOrEmpty(ddlPrioridade.SelectedValue))
            {
                MostrarMensagem("Selecione a prioridade.", "error");
                return;
            }

            if (string.IsNullOrEmpty(ddlTipoGrupo.SelectedValue))
            {
                MostrarMensagem("Selecione a categoria.", "error");
                return;
            }

            int prioridadeSelecionada = Convert.ToInt32(ddlPrioridade.SelectedValue);
            if ((prioridadeSelecionada == 33 || prioridadeSelecionada == 32) && string.IsNullOrEmpty(txtPrazo.Value))
            {
                MostrarMensagem("Para prioridades Alta ou Crítica, é necessário informar um prazo.", "error");
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ScrollPrazo",
                    $"document.getElementById('{txtPrazo.ClientID}').scrollIntoView({{ behavior: 'smooth', block: 'center' }});", true);
                return;
            }

            int codTipoDemanda;
            if (!string.IsNullOrEmpty(ddlTipoDetalhe.SelectedValue))
                codTipoDemanda = int.Parse(ddlTipoDetalhe.SelectedValue);
            else
                codTipoDemanda = int.Parse(ddlTipoGrupo.SelectedValue);

            DateTime? prazo = null;
            var rawPrazo = (txtPrazo.Value ?? "").Trim();
            if (!string.IsNullOrEmpty(rawPrazo))
            {
                if (DateTime.TryParseExact(rawPrazo,
                                           new[] { "yyyy-MM-dd", "dd/MM/yyyy" },
                                           CultureInfo.InvariantCulture,
                                           DateTimeStyles.None,
                                           out var d))
                {
                    prazo = d;
                }
            }

            int? codImportancia = null;
            if (!string.IsNullOrEmpty(ddlImportancia.SelectedValue))
            {
                codImportancia = Convert.ToInt32(ddlImportancia.SelectedValue);
            }

            var dto = new DemandaCreate
            {
                CodPessoaSolicitacao = CodPessoaAtual,
                CodSetorOrigem = int.Parse(ddlOrigem.SelectedValue),
                CodSetorDestino = int.Parse(ddlDestino.SelectedValue),
                CodEstr_TipoDemanda = codTipoDemanda,
                CodEstr_NivelPrioridade = prioridadeSelecionada,
                Titulo = txtTitulo.Text.Trim(),
                TextoDemanda = txtDescricao.Text.Trim(),
                DataPrazoMaximo = prazo,
                CodEstr_NivelImportancia = codImportancia
            };

            try
            {
                int id = _svc.CriarDemanda(dto);

                // Salva os anexos da lista temporária
                if (AnexosSelecionados.Count > 0)
                {
                    int arquivosSalvos = 0;
                    int arquivosComErro = 0;

                    foreach (var anexo in AnexosSelecionados)
                    {
                        try
                        {
                            // Usa o método auxiliar
                            string nomeArquivoSalvo = SalvarAnexoFisicoTemp(anexo.FileName, anexo.Content, id);
                            _svc.SalvarAnexoDemanda(id, nomeArquivoSalvo, anexo.Content, anexo.ContentType, CodPessoaAtual);
                            arquivosSalvos++;
                        }
                        catch (Exception exArquivo)
                        {
                            MostrarMensagem($"Erro ao processar {anexo.FileName}: {exArquivo.Message}", "error");
                            arquivosComErro++;
                        }
                    }

                    if (arquivosComErro > 0)
                    {
                        MostrarMensagem($"{arquivosComErro} arquivo(s) não puderam ser anexados.", "warning");
                    }

                    // Limpa a lista de anexos após salvar
                    AnexosSelecionados.Clear();
                    AtualizarListaAnexos();
                }

                MostrarMensagem($"Demanda criada com sucesso! (Código: {id})", "success");

                // Limpa os campos
                txtTitulo.Text = "";
                txtDescricao.Text = "";
                txtPrazo.Value = "";
                divPrazo.Style["display"] = "none";

            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao salvar: {ex.Message}", "error");
            }
        }

        protected void btnAlterarSituacao_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            RepeaterItem item = (RepeaterItem)btn.NamingContainer;
            DropDownList ddlSituacao = (DropDownList)item.FindControl("ddlSituacao");
            int codDemanda = Convert.ToInt32(btn.CommandArgument);

            if (ddlSituacao != null && !string.IsNullOrEmpty(ddlSituacao.SelectedValue))
            {
                int novaSituacao = Convert.ToInt32(ddlSituacao.SelectedValue);

                try
                {
                    bool sucesso = _svc.AlterarSituacaoDemanda(codDemanda, novaSituacao, CodPessoaAtual);

                    if (sucesso)
                    {
                        MostrarMensagem("Situação da demanda alterada com sucesso!", "success");

                        int prioridadeDemanda = _svc.ObterPrioridadeDemanda(codDemanda);

                        if (prioridadeDemanda == 33)
                        {
                            CarregarDemandasGenerica(true);

                            if (!_svc.GetDemandasCriticasAbertas(CodPessoaAtual).Any())
                            {
                                string fecharScript = @"
                                    try {
                                        var modal = document.getElementById('modalDemandas');
                                        if (modal) {
                                            if (window.bootstrap && bootstrap.Modal) {
                                                var instance = bootstrap.Modal.getInstance(modal);
                                                if (instance) instance.hide();
                                            } else {
                                                modal.style.display = 'none';
                                            }
                                        }
                                        document.querySelectorAll('.modal-backdrop').forEach(el => el.remove());
                                        document.body.classList.remove('modal-open');
                                    } catch(e) { console.error(e); }
                                ";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "FecharModalSituacao", fecharScript, true);
                            }
                        }
                        else if (prioridadeDemanda == 32)
                        {
                            CarregarDemandasGenerica(false);

                            if (!_svc.GetDemandasAltasAbertas(CodPessoaAtual).Any())
                            {
                                string fecharScript = @"
                                    try {
                                        var modal = document.getElementById('modalDemandas');
                                        if (modal) {
                                            if (window.bootstrap && bootstrap.Modal) {
                                                var instance = bootstrap.Modal.getInstance(modal);
                                                if (instance) instance.hide();
                                            } else {
                                                modal.style.display = 'none';
                                            }
                                        }
                                        document.querySelectorAll('.modal-backdrop').forEach(el => el.remove());
                                        document.body.classList.remove('modal-open');
                                    } catch(e) { console.error(e); }
                                ";
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "FecharModalSituacaoAlta", fecharScript, true);
                            }
                        }
                    }
                    else
                    {
                        MostrarMensagem("Erro ao alterar a situação da demanda.", "error");
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensagem($"Erro: {ex.Message}", "error");
                }
            }
            else
            {
                MostrarMensagem("Selecione uma situação válida.", "error");
            }
        }

        private void MostrarMensagem(string mensagem, string tipo = "success")
        {
            string titulo;
            switch (tipo.ToLower())
            {
                case "success":
                    titulo = "Sucesso";
                    break;
                case "error":
                    titulo = "Erro";
                    break;
                case "warning":
                    titulo = "Atenção";
                    break;
                case "info":
                    titulo = "Informação";
                    break;
                default:
                    titulo = "Mensagem";
                    break;
            }

            string script = $@"
                Swal.fire({{
                    toast: true,
                    position: 'top-end',
                    icon: '{tipo}',
                    title: '{titulo}',
                    text: '{mensagem.Replace("'", "\\'")}',
                    showConfirmButton: false,
                    timer: 4000,
                    timerProgressBar: true
                }});
            ";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastMsg", script, true);
        }
    }
}