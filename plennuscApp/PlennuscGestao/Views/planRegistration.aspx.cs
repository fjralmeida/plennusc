using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Models.ModelsGestao.modelsPlan;
using Plennusc.Core.Service.ServiceGestao.planService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using PlanoFiltro = Plennusc.Core.Models.ModelsGestao.PlanoFiltro;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class planRegistration : System.Web.UI.Page
    {
        private PlanoService _svc;

        protected void Page_Load(object sender, EventArgs e)
        {
            string connPlennus = System.Configuration.ConfigurationManager.ConnectionStrings["Plennus"].ConnectionString;
            string connAlianca = System.Configuration.ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;
            _svc = new PlanoService(connPlennus, connAlianca);

            if (!IsPostBack)
            {
                BindGrid();
                VerificarPendentes();

                if (Session["NomeUsuario"] != null)
                    lblUsuarioConfirmacao.Text = Session["NomeUsuario"].ToString();
                else
                    lblUsuarioConfirmacao.Text = "Administrador";
            }

            // Sempre verifica pendentes (mantém o painel visível no postback)
            VerificarPendentes();

            // Atualiza contadores após qualquer postback
            ScriptManager.RegisterStartupScript(this, GetType(), "atualizarContadores",
                "setTimeout(function() { " +
                "var panelNovas = document.getElementById('panelNovas'); " +
                "var panelAlteracoes = document.getElementById('panelAlteracoes'); " +
                "if (panelNovas && panelNovas.classList.contains('active')) { atualizarContadorSelecionadas(); } " +
                "else if (panelAlteracoes && panelAlteracoes.classList.contains('active')) { atualizarContadorUpdate(); } " +
                "}, 200);", true);
        }

        // ============================================================
        //  VERIFICAR PENDENTES (banner e totais das abas)
        // ============================================================
        private void VerificarPendentes()
        {
            try
            {
                var pendentes = _svc.ListarPlanosPendentesAlianca();
                int qtd = pendentes?.Count ?? 0;

                if (qtd > 0)
                {
                    pnlAviso.Visible = true;
                    lblQtdPendentes.Text = qtd.ToString();
                }
                else
                {
                    pnlAviso.Visible = false;
                }

                lblQtdAbaNovas.Text = qtd.ToString();
                lblQtdAbaAlteracoes.Text = "0";
            }
            catch (Exception ex)
            {
                ExibirMensagem("Erro ao verificar pendentes: " + ex.Message, "danger");
            }
        }

        protected void btnVerPendentes_Click(object sender, EventArgs e)
        {
            try
            {
                var pendentes = _svc.ListarPlanosPendentesAlianca();
                if (pendentes == null || pendentes.Count == 0)
                {
                    ExibirMensagem("Nenhum plano pendente encontrado.", "info");
                    ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal", "abrirModalSync();", true);
                    return;
                }

                rptPendentes.DataSource = pendentes;
                rptPendentes.DataBind();

                lblQtdAbaNovas.Text = pendentes.Count.ToString();

                ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal",
                    "setTimeout(function(){ abrirModalSync(); }, 300);", true);

                VerificarPendentes();
            }
            catch (Exception ex)
            {
                ExibirMensagem("Erro ao carregar pendentes: " + ex.Message, "danger");
            }
        }

        protected void btnConfirmarSync_Click(object sender, EventArgs e)
        {
            try
            {
                List<PlanoPendenteAliancaModel> selecionados = new List<PlanoPendenteAliancaModel>();

                foreach (RepeaterItem item in rptPendentes.Items)
                {
                    CheckBox chk = item.FindControl("chkSelecionar") as CheckBox;
                    if (chk != null && chk.Checked)
                    {
                        var plano = new PlanoPendenteAliancaModel
                        {
                            CodigoPlano = Convert.ToInt32((item.FindControl("hdnCodigoPlano") as HiddenField).Value),
                            RegistroANS = (item.FindControl("hdnRegistroANS") as HiddenField).Value,
                            TipoContratacaoDescricao = (item.FindControl("hdnTipoContratacao") as HiddenField).Value,
                            CodigoAbrangencia = string.IsNullOrEmpty((item.FindControl("hdnCodigoAbrangencia") as HiddenField).Value)
                             ? (int?)null
                             : Convert.ToInt32((item.FindControl("hdnCodigoAbrangencia") as HiddenField).Value),
                            Coparticipacao = (item.FindControl("hdnCoparticipacao") as HiddenField).Value,
                            Segmentacao = (item.FindControl("hdnSegmentacao") as HiddenField).Value,
                            AcomodacaoDescricao = (item.FindControl("hdnAcomodacao") as HiddenField).Value,
                            Conf_Ativo = (item.FindControl("hdnConf_Ativo") as HiddenField).Value,
                            NomePlanoFamiliar = (item.FindControl("hdnNomePlano") as HiddenField).Value,
                            CodigoGrupoContrato = string.IsNullOrEmpty((item.FindControl("hdnCodigoGrupoContrato") as HiddenField).Value)
                             ? (int?)null
                             : Convert.ToInt32((item.FindControl("hdnCodigoGrupoContrato") as HiddenField).Value),
                            CnpjOperadora = (item.FindControl("hdnCnpjOperadora") as HiddenField).Value,
                            DecSau = (item.FindControl("ddlDecSau") as DropDownList).SelectedValue,
                            Promocional = (item.FindControl("ddlPromocional") as DropDownList).SelectedValue
                        };
                        selecionados.Add(plano);
                    }
                }

                if (selecionados.Count == 0)
                {
                    ExibirMensagem("Nenhum plano selecionado para importar.", "warning");
                    ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal", "abrirModalSync();", true);
                    return;
                }

                int importados = _svc.ImportarPlanos(selecionados, ObterCodUsuarioLogado());

                ExibirMensagem($"{importados} plano(s) importado(s) com sucesso!", "success");

                BindGrid();
                VerificarPendentes();

                ScriptManager.RegisterStartupScript(this, GetType(), "fecharModal", "$('#modalSyncPlanos').modal('hide');", true);
            }
            catch (Exception ex)
            {
                ExibirMensagem("Erro ao importar planos: " + ex.Message, "danger");
                ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal", "abrirModalSync();", true);
            }
        }

        // ============================================================
        //  BOTÃO DE ATUALIZAÇÃO (placeholder)
        // ============================================================
        protected void btnConfirmarUpdate_Click(object sender, EventArgs e)
        {
            ExibirMensagem("Funcionalidade de atualização em desenvolvimento.", "info");
            ScriptManager.RegisterStartupScript(this, GetType(), "abrirModal", "abrirModalSync();", true);
        }

        // ============================================================
        //  FILTROS
        // ============================================================
        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            var filtro = new PlanoFiltro
            {
                NomePlanoComercial = txtNomePlanoComercial.Text.Trim(),
                Segmentacao = txtSegmentacao.Text.Trim(),
                Abrangencia = txtAbrangencia.Text.Trim(),
                Coparticipacao = txtCoparticipacao.Text.Trim(),
            };

            var lista = _svc.ListarPlanos(filtro);
            gvPlanos.DataSource = lista;
            gvPlanos.DataBind();
        }

        protected void gvPlanos_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPlanos.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void gvPlanos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var item = e.Row.DataItem as PlanoListDto; // ← troque pelo nome real da sua classe
            if (item == null) return;

            bool isNovo = item.Informacoes_log_i.HasValue &&
                          item.Informacoes_log_i.Value > DateTime.Now.AddHours(-24);

            bool isAlterado = item.Informacoes_log_a.HasValue &&
                              item.Informacoes_log_a.Value > DateTime.Now.AddHours(-24) &&
                              item.Informacoes_log_i != item.Informacoes_log_a;

            if (isNovo)
            {
                e.Row.CssClass += " linha-inserida";
                e.Row.ToolTip = $"Novo registro — inserido em {item.Informacoes_log_i:dd/MM/yyyy HH:mm}";
            }
            else if (isAlterado)
            {
                e.Row.CssClass += " linha-atualizada";
                e.Row.ToolTip = $"Atualizado em {item.Informacoes_log_a:dd/MM/yyyy HH:mm}";
            }
        }

        protected void gvPlanos_DataBound(object sender, EventArgs e) { }

        // ============================================================
        //  REPEATER ITEM DATABOUND
        // ============================================================
        protected void rptPendentes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var plano = e.Item.DataItem as PlanoPendenteAliancaModel;
                if (plano != null)
                {
                    DropDownList ddlDec = e.Item.FindControl("ddlDecSau") as DropDownList;
                    DropDownList ddlProm = e.Item.FindControl("ddlPromocional") as DropDownList;

                    if (!string.IsNullOrEmpty(plano.DecSau))
                        ddlDec.SelectedValue = plano.DecSau;
                    if (!string.IsNullOrEmpty(plano.Promocional))
                        ddlProm.SelectedValue = plano.Promocional;
                }
            }
        }

        // ============================================================
        //  UTILITÁRIOS
        // ============================================================
        private void ExibirMensagem(string texto, string tipo)
        {
            pnlMensagem.Visible = true;
            lblMensagem.Text = texto;
            pnlMensagem.CssClass = "alert alert-" + tipo + " alert-dismissible mb-3";
        }

        private int ObterCodUsuarioLogado()
        {
            if (Session["CodUsuario"] != null)
                return Convert.ToInt32(Session["CodUsuario"]);
            return 1;
        }
    }
}