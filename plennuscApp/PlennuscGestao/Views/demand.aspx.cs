using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Service.ServiceGestao;
using System;
using System.CodeDom; // para usar .Value no <input type="date" runat="server">
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
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

            int prioridadeSelecionada;
            if (!int.TryParse(ddlPrioridade.SelectedValue, out prioridadeSelecionada))
                return true;

            var prioridade = PrioridadesComLimites.FirstOrDefault(p => (int)p.Value == prioridadeSelecionada);

            if (prioridade != null && prioridade.Limite < 999)
            {
                int demandasExistentes = _svc.ContarDemandasPorPrioridade(CodPessoaAtual, prioridadeSelecionada);

                if (demandasExistentes >= prioridade.Limite)
                {
                    MostrarMensagemErro($"Você já atingiu o limite máximo de {prioridade.Limite} demanda(s) com prioridade '{prioridade.Text}'.");

                    // SE FOR PRIORIDADE CRÍTICA, MOSTRAR MODAL COM DEMANDAS EXISTENTES
                    if (prioridadeSelecionada == 33)
                    {
                        CarregarDemandasCriticas(); // Isso agora abre o modal automaticamente
                    }

                    // Resetar a seleção
                    ddlPrioridade.ClearSelection();
                    divPrazo.Style["display"] = "none";
                    return false;
                }

                // Mostrar campo de prazo para prioridades críticas e altas
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

        private void CarregarDemandasCriticas()
        {
            try
            {
                var demandasCriticas = _svc.GetDemandasCriticasAbertas(CodPessoaAtual);

                if (demandasCriticas.Any())
                {
                    rptDemandasCriticas.DataSource = demandasCriticas;
                    rptDemandasCriticas.DataBind();

                    // Script que funciona com ou sem jQuery
                    string script = @"
                function openModal() {
                    var modalElement = document.getElementById('modalDemandasCriticas');
                    if (modalElement) {
                        // Tentar com Bootstrap 5 primeiro
                        if (typeof bootstrap !== 'undefined') {
                            var modal = new bootstrap.Modal(modalElement);
                            modal.show();
                        } 
                        // Fallback para jQuery se disponível
                        else if (typeof $ !== 'undefined') {
                            $('#modalDemandasCriticas').modal('show');
                        }
                        // Fallback direto para CSS
                        else {
                            modalElement.style.display = 'block';
                            modalElement.classList.add('show');
                        }
                    }
                }
                
                // Esperar um pouco antes de abrir
                setTimeout(openModal, 100);
            ";

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "AbrirModal",
                        script, true);
                }
            }
            catch (Exception ex)
            {
                MostrarMensagemErro($"Erro ao carregar demandas críticas: {ex.Message}");
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
                lblMsg.CssClass = "text-success d-block mb-3";
                lblMsg.Text = $"Demanda criada (CodDemanda: {id}).";

                // Limpa campos
                txtTitulo.Text = "";
                txtDescricao.Text = "";
                txtPrazo.Value = "";
                divPrazo.Style["display"] = "none";

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

                        // Recarregar a lista e manter modal aberto
                        CarregarDemandasCriticas();

                        // Se não há mais demandas críticas, fechar modal
                        if (!_svc.GetDemandasCriticasAbertas(CodPessoaAtual).Any())
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "FecharModal",
                                "$('#modalDemandasCriticas').modal('hide');", true);
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
                    iconColor: '#28a745',
                    title: 'Sucesso',
                    text: '{mensagem.Replace("'", "\\'")}',
                    showConfirmButton: false,
                    timer: 3000,
                    timerProgressBar: true,
                    customClass: {{ popup: 'toast-success' }}
                }});
            ";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastSucesso", script, true);
        }

        // Método para exibir mensagens de erro (atualizado)
        private void MostrarMensagemErro(string mensagem, bool isError = true)
        {
            string script = $@"
                Swal.fire({{
                    toast: true,
                    position: 'top-end',
                    icon: '{(isError ? "error" : "warning")}',
                    iconColor: '{(isError ? "#dc3545" : "#ffc107")}',
                    title: '{(isError ? "Erro" : "Atenção")}',
                    text: '{mensagem.Replace("'", "\\'")}',
                    showConfirmButton: false,
                    timer: 5000,
                    timerProgressBar: true,
                    customClass: {{ popup: 'toast-warn' }}
                }});
            ";

            ScriptManager.RegisterStartupScript(this, this.GetType(), "ToastNotificacao", script, true);
        }
    }
}