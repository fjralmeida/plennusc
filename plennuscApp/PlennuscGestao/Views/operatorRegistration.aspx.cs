using Plennusc.Core.Models.ModelsGestao.modelsOperator;
using Plennusc.Core.Service.ServiceGestao.planiumApi;       // OperadoraService
using System;
using System.Collections.Generic;
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
        // Botão "Ver pendentes" → popula repeater e abre modal
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

                rptPendentes.DataSource = pendentes;
                rptPendentes.DataBind();

                // Força o script a executar após o render completo
                string script = @"
                    setTimeout(function() {
                        var modalEl = document.getElementById('modalSyncOperadoras');
                        if (modalEl) {
                            var modal = new bootstrap.Modal(modalEl);
                            modal.show();
                        } else {
                            console.error('Modal não encontrado');
                        }
                    }, 100);
                ";
                ScriptManager.RegisterStartupScript(this, GetType(), "AbrirModalSync", script, true);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // Confirmar sincronização → INSERT + Informacoes_log_i + CodPessoa
        // ─────────────────────────────────────────────────────────────────────

        protected void btnConfirmarSync_Click(object sender, EventArgs e)
        {
            var pendentes = Session[SESSION_PENDENTES] as List<OperadoraSyncDto>;

            if (pendentes == null || pendentes.Count == 0)
            {
                MostrarAlerta("Nenhuma operadora pendente encontrada.", "warning");
                return;
            }

            try
            {
                int codPessoa = ObterCodPessoa();

                _svc.ConfirmarSincronizacao(pendentes, codPessoa);

                Session.Remove(SESSION_PENDENTES);
                pnlAviso.Visible = false;

                MostrarAlerta($"{pendentes.Count} operadora(s) sincronizada(s) com sucesso!", "success");

                ScriptManager.RegisterStartupScript(
                    this, GetType(), "FecharModal",
                    "bootstrap.Modal.getInstance(document.getElementById('modalSyncOperadoras'))?.hide();",
                    true);

                BindGrid();
            }
            catch (Exception ex)
            {
                MostrarAlerta("Erro ao sincronizar: " + ex.Message, "danger");
            }
        }

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

        /// <summary>
        /// Pinta linha em laranja claro se inserida nas últimas 24h via Informacoes_log_i.
        /// </summary>
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

        /// <summary>
        /// CodPessoa do usuário logado — ajuste a chave conforme sua autenticação.
        /// </summary>
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