using Plennusc.Core.Models.ModelsGestao.modelsOperator;
using Plennusc.Core.Service.ServiceGestao.planiumApi;       // OperadoraService
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

// ─── IMPORTANTE ───────────────────────────────────────────────────────────────
// NÃO adicione: using Plennusc.Core.Models.ModelsGestao.modelsOperator
// Esse namespace tinha uma cópia antiga de OperadoraListDto que causava
// a ambiguidade. Delete o arquivo antigo de modelsOperator com essas classes.
// ─────────────────────────────────────────────────────────────────────────────

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class operatorRegistration : System.Web.UI.Page
    {
        private readonly OperadoraService _svc = new OperadoraService("Plennus", "Alianca");

        private const string SESSION_PENDENTES = "OperadorasPendentes";

        // ─────────────────────────────────────────────────────────────────────
        // Page_Load
        // ─────────────────────────────────────────────────────────────────────

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                VerificarPendentes();
                BindGrid();
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Verifica pendentes → controla banner
        // ─────────────────────────────────────────────────────────────────────

        private void VerificarPendentes()
        {
            try
            {
                var pendentes = _svc.BuscarOperadorasPendentes();

                if (pendentes != null && pendentes.Count > 0)
                {
                    Session[SESSION_PENDENTES] = pendentes;
                    pnlAviso.Visible = true;
                    lblQtdPendentes.Text = pendentes.Count.ToString();
                }
                else
                {
                    pnlAviso.Visible = false;
                    Session.Remove(SESSION_PENDENTES);
                }
            }
            catch (Exception ex)
            {
                pnlAviso.Visible = false;
                MostrarAlerta("Não foi possível verificar pendentes de sincronização: " + ex.Message, "warning");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Botão "Ver pendentes" → popula repeater (com checkbox) e abre modal
        // Está dentro de um UpdatePanel — não recarrega a página inteira.
        // ─────────────────────────────────────────────────────────────────────

        protected void btnVerPendentes_Click(object sender, EventArgs e)
        {
            var pendentes = Session[SESSION_PENDENTES] as List<OperadoraSyncDto>;

            if (pendentes == null || pendentes.Count == 0)
            {
                VerificarPendentes();
                pendentes = Session[SESSION_PENDENTES] as List<OperadoraSyncDto>;
            }

            if (pendentes != null && pendentes.Count > 0)
            {
                int codPessoa = ObterCodPessoa();
                string nomePessoa = _svc.BuscarNomePessoa(codPessoa);

                lblUsuarioConfirmacao.Text = string.IsNullOrWhiteSpace(nomePessoa)
                    ? $"CodPessoa {codPessoa}"
                    : nomePessoa;

                // Todas vêm marcadas por padrão — usuário desmarca o que não quer
                rptPendentes.DataSource = pendentes;
                rptPendentes.DataBind();

                lblQtdSelecionadas.Text = pendentes.Count.ToString();

                ClientScript.RegisterStartupScript(
                 this.GetType(), "AbrirModalSync",
                 @"document.addEventListener('DOMContentLoaded', function(){ 
                    var modalEl = document.getElementById('modalSyncOperadoras');
                    var modal = new bootstrap.Modal(modalEl);
                    modal.show();
                    setTimeout(function(){ atualizarContadorSelecionadas(); }, 200);
                });",
                 true);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Confirmar sincronização → grava SÓ as operadoras com checkbox marcado
        // ─────────────────────────────────────────────────────────────────────

        protected void btnConfirmarSync_Click(object sender, EventArgs e)
        {
            var todasPendentes = Session[SESSION_PENDENTES] as List<OperadoraSyncDto>;

            if (todasPendentes == null || todasPendentes.Count == 0)
            {
                MostrarAlerta("Nenhuma operadora pendente encontrada.", "warning");
                return;
            }

            // Lê os CodigoGrupo marcados no repeater (cada linha tem um checkbox + hidden)
            var codigosSelecionados = ObterCodigosSelecionadosNoRepeater();

            if (codigosSelecionados.Count == 0)
            {
                MostrarAlerta("Selecione ao menos uma operadora para importar.", "warning");

                // Reabre o modal já que o usuário não selecionou nada
                ClientScript.RegisterStartupScript(
                  this.GetType(), "ReabrirModalSync",
                  "document.addEventListener('DOMContentLoaded', function(){ new bootstrap.Modal(document.getElementById('modalSyncOperadoras')).show(); });",
                  true);
                return;
            }

            var selecionadas = todasPendentes
                .Where(p => codigosSelecionados.Contains(p.CodigoGrupo))
                .ToList();

            try
            {
                int codPessoa = ObterCodPessoa();

                _svc.ConfirmarSincronizacao(selecionadas, codPessoa);

                // Remove da lista de pendentes só as que foram importadas;
                // o que ficou de fora continua pendente para uma próxima confirmação
                var restantes = todasPendentes
                    .Where(p => !codigosSelecionados.Contains(p.CodigoGrupo))
                    .ToList();

                if (restantes.Count > 0)
                {
                    Session[SESSION_PENDENTES] = restantes;
                    pnlAviso.Visible = true;
                    lblQtdPendentes.Text = restantes.Count.ToString();
                }
                else
                {
                    Session.Remove(SESSION_PENDENTES);
                    pnlAviso.Visible = false;
                }

                MostrarAlerta($"{selecionadas.Count} operadora(s) sincronizada(s) com sucesso!", "success");

                ClientScript.RegisterStartupScript(
                    this.GetType(), "FecharModal",
                    "document.addEventListener('DOMContentLoaded', function(){ var m = bootstrap.Modal.getInstance(document.getElementById('modalSyncOperadoras')); if(m) m.hide(); });",
                    true);

                BindGrid();
            }
            catch (Exception ex)
            {
                MostrarAlerta("Erro ao sincronizar: " + ex.Message, "danger");
            }
        }

        /// <summary>
        /// Percorre o repeater e retorna os CodigoGrupo cujas checkboxes
        /// foram mantidas marcadas pelo usuário no momento do postback.
        /// </summary>
        private HashSet<int> ObterCodigosSelecionadosNoRepeater()
        {
            var selecionados = new HashSet<int>();

            foreach (RepeaterItem item in rptPendentes.Items)
            {
                var chk = item.FindControl("chkSelecionar") as CheckBox;
                var hidden = item.FindControl("hdnCodigoGrupo") as HiddenField;

                if (chk != null && chk.Checked && hidden != null
                    && int.TryParse(hidden.Value, out int codigoGrupo))
                {
                    selecionados.Add(codigoGrupo);
                }
            }

            return selecionados;
        }

        /// <summary>
        /// Chamado via __doPostBack a partir do JS (checkbox "selecionar todos"
        /// e contador de selecionados são feitos no client-side; aqui é só leitura
        /// no postback de confirmação — não precisa de handler de evento extra).
        /// </summary>

        // ─────────────────────────────────────────────────────────────────────
        // Grid
        // ─────────────────────────────────────────────────────────────────────

        protected void btnFiltrar_Click(object sender, EventArgs e) => BindGrid();

        private void BindGrid()
        {
            var filtro = new OperadoraFiltro
            {
                NomeOperadora = txtOperadora.Text.Trim(),
                RegistroANS = txtRegistroAns.Text.Trim(),
                Numero_CNPJ = txtCnpj.Text.Trim()
            };

            var lista = _svc.ListarOperadoras(filtro);
            gvOperadoras.DataSource = lista;
            gvOperadoras.DataBind();
        }

        protected void gvOperadoras_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvOperadoras.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvOperadoras_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Ver")
            {
                Session["CurrentOperadoraId"] = Convert.ToInt32(e.CommandArgument);
                Response.Redirect("~/viewOperadora");
            }
        }

        protected void gvOperadoras_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var item = e.Row.DataItem as OperadoraListDto;
            if (item != null && item.IsRegistroNovo)
            {
                e.Row.Style["background-color"] = "#FFE0B2";
                e.Row.Style["font-weight"] = "500";
                e.Row.ToolTip = $"Novo registro — inserido em {item.Informacoes_log_i:dd/MM/yyyy HH:mm}";
            }
        }

        protected void btnNovaOperadora_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/novaOperadora");
        }

        // ─────────────────────────────────────────────────────────────────────
        // Utilitários
        // ─────────────────────────────────────────────────────────────────────

        private int ObterCodPessoa()
        {
            return Session["CodPessoa"] != null
                ? Convert.ToInt32(Session["CodPessoa"])
                : 1;
        }

        private void MostrarAlerta(string mensagem, string tipo = "info")
        {
            pnlMensagem.Visible = true;
            lblMensagem.Text = mensagem;
            pnlMensagem.CssClass = $"alert alert-{tipo} alert-dismissible mb-3";
        }
    }
}