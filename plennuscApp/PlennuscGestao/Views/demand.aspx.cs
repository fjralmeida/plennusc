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
                    MostrarMensagemErro($"Você já atingiu o limite máximo de {prioridade.Limite} demanda(s) com prioridade '{prioridade.Text}'.");

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
                    // opcional: log no cliente
                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), Guid.NewGuid().ToString(),
                        "console.log('CarregarDemandasGenerica: lista vazia');", true);
                    return;
                }

                // DataBind no repeater único
                rptDemandas.DataSource = demandas;
                rptDemandas.DataBind();

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

                // Script robusto com getOrCreateInstance + fallback por botão
                string key = isCritica ? "AbrirModalCriticas" : "AbrirModalAltas";
                string script = @"
                (function() {
                    console.log('>>> Iniciando abertura de modal (genérico)');
                    function abrirInstancia() {
                        try {
                            var modalEl = document.getElementById('modalDemandas');
                            if (!modalEl) { console.error('modalDemandas não encontrado'); return false; }
                            if (window.bootstrap && typeof bootstrap.Modal !== 'undefined' && typeof bootstrap.Modal.getOrCreateInstance === 'function') {
                                var inst = bootstrap.Modal.getOrCreateInstance(modalEl);
                                inst.show();
                                console.log('modalDemandas: show() chamado via getOrCreateInstance');
                                return true;
                            }
                            // fallback: criar nova instância se getOrCreate não existir
                            if (window.bootstrap && typeof bootstrap.Modal !== 'undefined') {
                                var m = new bootstrap.Modal(modalEl);
                                m.show();
                                console.log('modalDemandas: show() chamado via new Modal() fallback');
                                return true;
                            }
                            return false;
                        } catch(e) {
                            console.error('erro abrirInstancia:', e);
                            return false;
                        }
                    }
                    function abrirPorBotao() {
                        try {
                            var btn = document.getElementById('btnAbrirModalHidden');
                            if (btn) { btn.click(); console.log('modalDemandas: aberto por botão fallback'); return true; }
                            console.error('botão fallback não encontrado');
                            return false;
                        } catch(e) {
                            console.error('erro abrirPorBotao:', e);
                            return false;
                        }
                    }
                    setTimeout(function() {
                        var ok = abrirInstancia();
                        if (!ok) abrirPorBotao();
                    }, 350);
                })();
            ";

                ScriptManager.RegisterStartupScript(Page, Page.GetType(), key, script, true);
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro ao carregar demandas: {ex.Message}");
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
                MostrarMensagemErro("Selecione uma nova prioridade.");
                return;
            }

            int codDemanda = Convert.ToInt32(btn.CommandArgument);
            int novaPrioridade = Convert.ToInt32(ddlNovaPrioridade.SelectedValue);

            try
            {
                // Método para atualizar a prioridade da demanda
                bool sucesso = _svc.AtualizarPrioridadeDemanda(codDemanda, novaPrioridade);

                if (sucesso)
                {
                    // Recarregar o modal com as demandas atualizadas
                    bool isCritica = litTituloModal.Text.Contains("CRÍTICAS");
                    CarregarDemandasGenerica(isCritica);

                    ScriptManager.RegisterStartupScript(Page, Page.GetType(), "AtualizarModal",
                        "console.log('Prioridade alterada com sucesso!');", true);
                }
                else
                {
                    MostrarMensagemErro("Erro ao alterar a prioridade da demanda.");
                }
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro: {ex.Message}");
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
        }

        protected void ddlTipoGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubtiposDoGrupo();
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            lblMsg.CssClass = "text-danger d-block mb-3";
            lblMsg.Text = "";

            // Validar limite de prioridade antes de tudo
            if (!ValidarLimitePrioridade())
                return;

            if (CodPessoaAtual == 0) { lblMsg.Text = "Sessão inválida."; return; }
            if (string.IsNullOrWhiteSpace(txtTitulo.Text)) { lblMsg.Text = "Informe o título."; return; }
            if (string.IsNullOrWhiteSpace(txtDescricao.Text)) { lblMsg.Text = "Informe a descrição."; return; }
            if (string.IsNullOrEmpty(ddlOrigem.SelectedValue) || string.IsNullOrEmpty(ddlDestino.SelectedValue))
            { lblMsg.Text = "Selecione origem e destino."; return; }
            if (string.IsNullOrEmpty(ddlPrioridade.SelectedValue))
            { lblMsg.Text = "Selecione a prioridade."; return; }
            if (string.IsNullOrEmpty(ddlTipoGrupo.SelectedValue))
            { lblMsg.Text = "Selecione a categoria."; return; }

            // Validação adicional para prioridades que exigem prazo
            int prioridadeSelecionada = Convert.ToInt32(ddlPrioridade.SelectedValue);
            if ((prioridadeSelecionada == 33 || prioridadeSelecionada == 32) && string.IsNullOrEmpty(txtPrazo.Value))
            {
                lblMsg.Text = "Para prioridades Alta ou Crítica, é necessário informar um prazo.";
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
                    foreach (HttpPostedFile arquivo in fuAnexos.PostedFiles)
                    {
                        // Validar tamanho (10MB máximo)
                        if (arquivo.ContentLength > 10 * 1024 * 1024)
                        {
                            MostrarMensagemErro($"O arquivo {arquivo.FileName} excede o limite de 10MB.");
                            continue;
                        }

                        // Validar extensão
                        string extensao = Path.GetExtension(arquivo.FileName).ToLower();
                        string[] extensoesPermitidas = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".jpg", ".jpeg", ".png", ".gif" };

                        if (!extensoesPermitidas.Contains(extensao))
                        {
                            MostrarMensagemErro($"Tipo de arquivo não permitido: {arquivo.FileName}");
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
                    }
                }

                lblMsg.CssClass = "text-success d-block mb-3";
                lblMsg.Text = $"Demanda criada (CodDemanda: {id}).";

                // Limpa campos
                txtTitulo.Text = "";
                txtDescricao.Text = "";
                txtPrazo.Value = "";
                divPrazo.Style["display"] = "none";
                fuAnexos.Attributes.Clear(); // Limpa os arquivos selecionados

                // Recarrega categorias/subtipos
                BindGrupos();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Erro ao salvar: " + ex.Message;
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
                        MostrarMensagemErro("Erro ao alterar a situação da demanda.");
                    }
                }
                catch (Exception ex)
                {
                    MostrarMensagemErro($"Erro: {ex.Message}");
                }
            }
            else
            {
                MostrarMensagemErro("Selecione uma situação válida.");
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

        private void MostrarMensagemErro(string mensagem, bool isError = true)
        {
            string iconType = isError ? "error" : "warning";
            string title = isError ? "Erro" : "Atenção";

            string script = $@"
                Swal.fire({{
                    toast: true,
                    position: 'top-end',
                    icon: '{iconType}',
                    title: '{title}',
                    text: '{mensagem.Replace("'", "\\'")}',
                    showConfirmButton: false,
                    timer: 5000,
                    timerProgressBar: true
                }});
            ";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastErro", script, true);
        }

        protected void btnRemoverArquivo_Click(object sender, EventArgs e)
        {

        }
    }
}