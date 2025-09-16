using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Service.ServiceGestao;
using System;
using System.CodeDom; // para usar .Value no <input type="date" runat="server">
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

        // Adicione esta propriedade para armazenar as prioridades com limites
        private List<DemandaComLimite> PrioridadesComLimites;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindAll();
                // Esconder o campo de prazo inicialmente
                divPrazo.Style["display"] = "none";
            }
            else
            {
                // RECARREGAR as prioridades durante postback para que estejam disponíveis nos eventos
                PrioridadesComLimites = _svc.GetPrioridadesComLimites();
            }
        }

        private void BindAll()
        {
            // Departamentos - Filtrado pelo usuário logado (ORIGEM)
            ddlOrigem.DataSource = _svc.GetDepartamentoUsuario(CodPessoaAtual);
            ddlOrigem.DataValueField = "Value";
            ddlOrigem.DataTextField = "Text";
            ddlOrigem.DataBind();

            // Se houver apenas um departamento, selecione-o automaticamente
            if (ddlOrigem.Items.Count == 2) // 1 item + o "Selecione"
            {
                ddlOrigem.SelectedIndex = 1;
            }

            // Destino - Mantém todos os departamentos (sem filtro)
            ddlDestino.DataSource = _svc.GetDepartamentos();
            ddlDestino.DataValueField = "Value";
            ddlDestino.DataTextField = "Text";
            ddlDestino.DataBind();

            PrioridadesComLimites = _svc.GetPrioridadesComLimites();
            ddlPrioridade.DataSource = PrioridadesComLimites;
            ddlPrioridade.DataValueField = "Value";
            ddlPrioridade.DataTextField = "Text";
            ddlPrioridade.DataBind();

            // GARANTIR que o item vazio existe como primeiro item
            if (ddlPrioridade.Items.Count > 0 && ddlPrioridade.Items[0].Value != "")
            {
                ddlPrioridade.Items.Insert(0, new ListItem("Selecione a prioridade", ""));
            }
            // Origem default pela sessão (se existir)
            if (CodDepartamentoAtual.HasValue)
            {
                var it = ddlOrigem.Items.FindByValue(CodDepartamentoAtual.Value.ToString());
                if (it != null) ddlOrigem.SelectedValue = it.Value;
            }

            // Categoria/Subtipo (sem filtro por setor)
            BindGrupos();
        }

        protected void ddlPrioridade_SelectedIndexChanged(object sender, EventArgs e)
        {
            ValidarLimitePrioridade();
            try { upPrioridade.Update(); } catch { }
        }

        // Método para validar o limite de prioridade
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

                    // abre modal conforme prioridade
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

                // DataBind no repeater
                rptDemandas.DataSource = demandas;
                rptDemandas.DataBind();

                // Atualiza o UpdatePanel do modal para enviar o HTML atualizado ao cliente
                upModalDemandas.Update();

                // Mensagens dinâmicas
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

                // Script para abrir modal (será enviado junto com HTML atualizado)
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


        protected void rptDemandas_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DropDownList ddlNovaPrioridade = (DropDownList)e.Item.FindControl("ddlNovaPrioridade");

                if (ddlNovaPrioridade != null && PrioridadesComLimites != null)
                {
                    // Filtrar apenas prioridades NÃO críticas/altas (33 e 32)
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
                    // Mostrar toast de sucesso
                    MostrarMensagem("Demanda alterada com sucesso!", "success");

                    // Verificar se ainda existem demandas do tipo (crítica/alta) para o usuário
                    bool isCritica = litTituloModal.Text.Contains("CRÍTICAS");
                    List<DemandaCriticaInfo> demandasRemanescentes = isCritica
                        ? _svc.GetDemandasCriticasAbertas(CodPessoaAtual)
                        : _svc.GetDemandasAltasAbertas(CodPessoaAtual);

                    if (demandasRemanescentes != null && demandasRemanescentes.Any())
                    {
                        // Recarrega o modal com as demandas atualizadas (vai fazer DataBind e abrir o modal)
                        CarregarDemandasGenerica(isCritica);
                    }
                    else
                    {
                        // Se não houverem mais demandas, fechar modal e limpar backdrop (script robusto)
                        string fecharScript = @"
                            (function(){
                                try {
                                    var modalEl = document.getElementById('modalDemandas');
                                    if (modalEl) {
                                        // tenta usar Bootstrap 5 API
                                        if (window.bootstrap && typeof bootstrap.Modal !== 'undefined' && typeof bootstrap.Modal.getInstance === 'function') {
                                            var inst = bootstrap.Modal.getInstance(modalEl);
                                            if (inst) inst.hide();
                                        } else {
                                            // fallback jQuery se estiver disponível
                                            if (typeof jQuery !== 'undefined' && jQuery('#modalDemandas').length) {
                                                jQuery('#modalDemandas').modal('hide');
                                            }
                                        }
                                    }
                                } catch(e) { console.error('erro ao esconder modal:', e); }
                                // remove qualquer backdrop e a classe modal-open do body (com pequeno delay para segurança)
                                setTimeout(function(){
                                    try{
                                        document.querySelectorAll('.modal-backdrop').forEach(function(el){ el.parentNode && el.parentNode.removeChild(el); });
                                        document.body.classList.remove('modal-open');
                                    }catch(e){ console.error('erro cleanup backdrop:', e); }
                                }, 200);
                            })();
                            ";
                        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "FecharModalCleanup", fecharScript, true);
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
                    // Carregar situações de fechamento diretamente
                    var situacoesFechamento = _svc.GetSituacoesParaFechamento();
                    ddlSituacao.DataSource = situacoesFechamento;
                    ddlSituacao.DataValueField = "Value";
                    ddlSituacao.DataTextField = "Text";
                    ddlSituacao.DataBind();

                    // Adicionar item vazio no início
                    ddlSituacao.Items.Insert(0, new ListItem("-- Selecionar --", ""));
                }
            }
        }

        private void BindGrupos()
        {
            ddlTipoGrupo.DataSource = _svc.GetTiposDemandaGrupos();
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
            BindGrupos();
            try { upRoteamento.Update(); } catch { }
            try { upCategoria.Update(); } catch { }
        }

        protected void ddlTipoGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubtiposDoGrupo();
            try { upCategoria.Update(); } catch { }
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            // Validar limite de prioridade antes de tudo
            if (!ValidarLimitePrioridade())
                return;

            if (CodPessoaAtual == 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastSessao", "showToastErro('Sessão inválida.');", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTitulo.Text))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastTitulo", "showToastErro('Informe o título.');", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ScrollTitulo", "document.getElementById('" + txtTitulo.ClientID + "').scrollIntoView({ behavior: 'smooth', block: 'center' });", true);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescricao.Text))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastDescricao", "showToastErro('Informe a descrição.');", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ScrollDescricao", "document.getElementById('" + txtDescricao.ClientID + "').scrollIntoView({ behavior: 'smooth', block: 'center' });", true);
                return;
            }

            if (string.IsNullOrEmpty(ddlOrigem.SelectedValue) || string.IsNullOrEmpty(ddlDestino.SelectedValue))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastOrigemDestino", "showToastErro('Selecione origem e destino.');", true);
                return;
            }

            if (string.IsNullOrEmpty(ddlPrioridade.SelectedValue))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastPrioridade", "showToastErro('Selecione a prioridade.');", true);
                return;
            }

            if (string.IsNullOrEmpty(ddlTipoGrupo.SelectedValue))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastCategoria", "showToastErro('Selecione a categoria.');", true);
                return;
            }

            // Validação adicional para prioridades que exigem prazo
            int prioridadeSelecionada = Convert.ToInt32(ddlPrioridade.SelectedValue);
            if ((prioridadeSelecionada == 33 || prioridadeSelecionada == 32) && string.IsNullOrEmpty(txtPrazo.Value))
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastPrazo", "showToastErro('Para prioridades Alta ou Crítica, é necessário informar um prazo.');", true);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ScrollPrazo", "document.getElementById('" + txtPrazo.ClientID + "').scrollIntoView({ behavior: 'smooth', block: 'center' });", true);
                return;
            }

            // Decide o tipo a gravar: Subtipo (se selecionado) ou o próprio Grupo
            int codTipoDemanda;
            if (!string.IsNullOrEmpty(ddlTipoDetalhe.SelectedValue))
                codTipoDemanda = int.Parse(ddlTipoDetalhe.SelectedValue);
            else
                codTipoDemanda = int.Parse(ddlTipoGrupo.SelectedValue);

            // Prazo (input type="date" -> usar .Value)
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

            var dto = new DemandaCreate
            {
                CodPessoaSolicitacao = CodPessoaAtual,
                CodSetorOrigem = int.Parse(ddlOrigem.SelectedValue),
                CodSetorDestino = int.Parse(ddlDestino.SelectedValue),
                CodEstr_TipoDemanda = codTipoDemanda,
                CodEstr_NivelPrioridade = prioridadeSelecionada,
                Titulo = txtTitulo.Text.Trim(),
                TextoDemanda = txtDescricao.Text.Trim(),
                DataPrazoMaximo = prazo
            };

            try
            {
                int id = _svc.CriarDemanda(dto);

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

                            // PRIMEIRO salva o arquivo físico
                            string nomeArquivoSalvo = _svc.SalvarAnexoFisico(arquivo, id);

                            // DEPOIS lê o conteúdo para salvar no banco (se necessário)
                            byte[] conteudo;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                arquivo.InputStream.CopyTo(ms);
                                conteudo = ms.ToArray();
                            }

                            // Salvar anexo - passa o nome único que foi salvo fisicamente
                            _svc.SalvarAnexoDemanda(id, nomeArquivoSalvo, conteudo, arquivo.ContentType, CodPessoaAtual);
                            arquivosSalvos++;
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

                // Mensagem de sucesso
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastSucesso",
                    "showToastSucesso('Demanda criada com sucesso! (Código: " + id + ")');", true);

                // Limpa campos
                txtTitulo.Text = "";
                txtDescricao.Text = "";
                txtPrazo.Value = "";
                divPrazo.Style["display"] = "none";

                // Limpar arquivos selecionados
                fuAnexos.Attributes.Clear();
                ScriptManager.RegisterStartupScript(this, this.GetType(), "LimparArquivos",
                    "selectedFiles = []; updateFilePreview();", true);

                // Recarrega categorias/subtipos
                BindGrupos();
            }
            catch (Exception ex)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastErro",
                    "showToastErro('Erro ao salvar: " + ex.Message.Replace("'", "\\'") + "');", true);
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
                        MostrarMensagemSucesso("Situação da demanda alterada com sucesso!");

                        // Descobrir prioridade da demanda (32 = Alta, 33 = Crítica)
                        int prioridadeDemanda = _svc.ObterPrioridadeDemanda(codDemanda);

                        if (prioridadeDemanda == 33) // Crítica
                        {
                            CarregarDemandasGenerica(true);

                            if (!_svc.GetDemandasCriticasAbertas(CodPessoaAtual).Any())
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "FecharModalCriticas",
                                    "var m = bootstrap.Modal.getInstance(document.getElementById('modalDemandas')); if(m) m.hide();", true);
                            }
                        }
                        else if (prioridadeDemanda == 32) // Alta
                        {
                            CarregarDemandasGenerica(false);

                            if (!_svc.GetDemandasAltasAbertas(CodPessoaAtual).Any())
                            {
                                ScriptManager.RegisterStartupScript(this, this.GetType(), "FecharModalAltas",
                                    "var m = bootstrap.Modal.getInstance(document.getElementById('modalDemandas')); if(m) m.hide();", true);
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
                    MostrarMensagem($"Erro: {ex.Message}",  "error");
                }
            }
            else
            {
                MostrarMensagem("Selecione uma situação válida.",  "error");
            }
        }


        // Método para exibir mensagens com SweetAlert2
       private void MostrarMensagemSucesso(string mensagem)
       {
            string script = $@"
                Swal.fire({{
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Sucesso',
                    text: '{mensagem.Replace("'", "\\'")}',
                    showConfirmButton: false,
                    timer: 3000,
                    timerProgressBar: true
                }});
            ";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastSucesso", script, true);
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


        protected void btnRemoverArquivo_Click(object sender, EventArgs e)
        {

        }
    }
}