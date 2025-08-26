using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Service.ServiceGestao;
using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls; // para usar .Value no <input type="date" runat="server">

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class demand : Page
    {
        private readonly DemandaService _svc = new DemandaService("Plennus");

        private int CodPessoaAtual => Convert.ToInt32(Session["CodPessoa"] ?? 0);

        private int? CodDepartamentoAtual =>
            Session["CodDepartamento"] == null ? (int?)null : Convert.ToInt32(Session["CodDepartamento"]);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindAll();
        }

        private void BindAll()
        {
            // Departamentos
            ddlOrigem.DataSource = _svc.GetDepartamentos();
            ddlOrigem.DataValueField = "Value";
            ddlOrigem.DataTextField = "Text";
            ddlOrigem.DataBind();

            ddlDestino.DataSource = _svc.GetDepartamentos();
            ddlDestino.DataValueField = "Value";
            ddlDestino.DataTextField = "Text";
            ddlDestino.DataBind();

            // Prioridade
            ddlPrioridade.DataSource = _svc.GetPrioridades();
            ddlPrioridade.DataValueField = "Value";
            ddlPrioridade.DataTextField = "Text";
            ddlPrioridade.DataBind();

            // Aprovador
            ddlAprovador.DataSource = _svc.GetPessoasAtivas();
            ddlAprovador.DataValueField = "Value";
            ddlAprovador.DataTextField = "Text";
            ddlAprovador.DataBind();
            ddlAprovador.Items.Insert(0, new ListItem("(Deixar em branco)", ""));

            // Origem default pela sessão (se existir)
            if (CodDepartamentoAtual.HasValue)
            {
                var it = ddlOrigem.Items.FindByValue(CodDepartamentoAtual.Value.ToString());
                if (it != null) ddlOrigem.SelectedValue = it.Value;
            }

            // Categoria/Subtipo (sem filtro por setor)
            BindGrupos();
        }

        private void BindGrupos()
        {
            ddlTipoGrupo.DataSource = _svc.GetTiposDemandaGrupos(); // só grupos: CodTipoEstrutura=6 e CodEstruturaPai IS NULL
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

            ddlTipoDetalhe.DataSource = _svc.GetSubtiposDemanda(codGrupo); // filhos do grupo (CodEstruturaPai = codGrupo)
            ddlTipoDetalhe.DataValueField = "Value";
            ddlTipoDetalhe.DataTextField = "Text";
            ddlTipoDetalhe.DataBind();

            // opcional: deixa um “— selecionar —”
            if (ddlTipoDetalhe.Items.Count > 0)
                ddlTipoDetalhe.Items.Insert(0, new ListItem("— selecionar —", ""));
        }

        // wired no .aspx (AutoPostBack="true")
        protected void ddlDestino_SelectedIndexChanged(object sender, EventArgs e)
        {
            // não filtramos por setor; manter coerência recarregando grupos/subtipos
            BindGrupos();
        }

        // wired no .aspx (AutoPostBack="true")
        protected void ddlTipoGrupo_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindSubtiposDoGrupo();
        }

        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            lblMsg.CssClass = "text-danger d-block mb-3";
            lblMsg.Text = "";

            if (CodPessoaAtual == 0) { lblMsg.Text = "Sessão inválida."; return; }
            if (string.IsNullOrWhiteSpace(txtTitulo.Text)) { lblMsg.Text = "Informe o título."; return; }
            if (string.IsNullOrWhiteSpace(txtDescricao.Text)) { lblMsg.Text = "Informe a descrição."; return; }
            if (string.IsNullOrEmpty(ddlOrigem.SelectedValue) || string.IsNullOrEmpty(ddlDestino.SelectedValue))
            { lblMsg.Text = "Selecione origem e destino."; return; }
            if (string.IsNullOrEmpty(ddlTipoGrupo.SelectedValue))
            { lblMsg.Text = "Selecione a categoria."; return; }

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
                CodEstr_NivelPrioridade = int.Parse(ddlPrioridade.SelectedValue),
                Titulo = txtTitulo.Text.Trim(),
                TextoDemanda = txtDescricao.Text.Trim(),
                Conf_RequerAprovacao = chkAprova.Checked,
                CodPessoaAprovacao = string.IsNullOrWhiteSpace(ddlAprovador.SelectedValue)
                                        ? 0
                                        : int.Parse(ddlAprovador.SelectedValue),
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
                chkAprova.Checked = false;
                ddlAprovador.SelectedIndex = 0;
                txtPrazo.Value = "";

                // Recarrega categorias/subtipos (opcional)
                BindGrupos();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Erro ao salvar: " + ex.Message;
            }
        }
    }
}
