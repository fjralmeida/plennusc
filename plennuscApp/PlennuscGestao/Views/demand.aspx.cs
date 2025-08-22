using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class demand : System.Web.UI.Page
    {
        private readonly DemandaService _svc = new DemandaService("Plennus");

        private int CodPessoaAtual
            => Convert.ToInt32(Session["CodPessoa"] ?? 0);
        
        private int? CodDepartamentoAtual
            => Session["CodDepartamento"] == null ? (int?)null : Convert.ToInt32(Session["CodDepartamento"]);

        protected void Page_Load(object sender, EventArgs e)
        {
           if(!IsPostBack)
           {
                BindCombos();
            }
        }

        private void BindCombos()
        {
            ddlOrigem.DataSource = _svc.GetDepartamentos();
            ddlOrigem.DataValueField = "Value";
            ddlOrigem.DataTextField = "Text";
            ddlOrigem.DataBind();

            ddlDestino.DataSource = _svc.GetDepartamentos();
            ddlDestino.DataValueField = "Value";
            ddlDestino.DataTextField = "Text";
            ddlDestino.DataBind();

            ddlPrioridade.DataSource = _svc.GetPrioridades();   // agora por CodTipoEstrutura = 7
            ddlPrioridade.DataValueField = "Value";
            ddlPrioridade.DataTextField = "Text";
            ddlPrioridade.DataBind();

            ddlTipo.DataSource = _svc.GetTiposDemanda();         // agora por CodTipoEstrutura = 6 (Pai/Filho)
            ddlTipo.DataValueField = "Value";
            ddlTipo.DataTextField = "Text";
            ddlTipo.DataBind();

            ddlAprovador.DataSource = _svc.GetPessoasAtivas();
            ddlAprovador.DataValueField = "Value";
            ddlAprovador.DataTextField = "Text";
            ddlAprovador.DataBind();
            ddlAprovador.Items.Insert(0, new ListItem("(Deixar em branco)", ""));

            // Pré-seleciona ORIGEM pelo departamento da Sessão (se existir)
            if (CodDepartamentoAtual.HasValue)
            {
                var it = ddlOrigem.Items.FindByValue(CodDepartamentoAtual.Value.ToString());
                if (it != null) ddlOrigem.SelectedValue = it.Value;
            }
        }


        protected void btnSalvar_Click(object sender, EventArgs e)
        {
            lblMsg.CssClass = "text-danger d-block mb-3";
            lblMsg.Text = "";

            if (CodPessoaAtual == 0) { lblMsg.Text = "Sessão inválida."; return; }
            if (string.IsNullOrWhiteSpace(txtTitulo.Text)) { lblMsg.Text = "Informe o título."; return; }
            if (string.IsNullOrWhiteSpace(txtDescricao.Text)) { lblMsg.Text = "Informe a descrição."; return; }

            DateTime? prazo = null;
            if (!string.IsNullOrWhiteSpace(txtPrazo.Text))
            {
                if (DateTime.TryParse(txtPrazo.Text, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d))
                    prazo = d;
            }

            var dto = new DemandaCreate
            {
                CodPessoaSolicitacao = CodPessoaAtual,
                CodSetorOrigem = int.Parse(ddlOrigem.SelectedValue),
                CodSetorDestino = int.Parse(ddlDestino.SelectedValue),
                CodEstr_TipoDemanda = int.Parse(ddlTipo.SelectedValue),
                CodEstr_NivelPrioridade = int.Parse(ddlPrioridade.SelectedValue),
                Titulo = txtTitulo.Text.Trim(),
                TextoDemanda = txtDescricao.Text.Trim(),
                Conf_RequerAprovacao = chkAprova.Checked,
                CodPessoaAprovacao = string.IsNullOrWhiteSpace(ddlAprovador.SelectedValue) ? 0 : int.Parse(ddlAprovador.SelectedValue),
                DataPrazoMaximo = prazo
            };

            try
            {
                int id = _svc.CriarDemanda(dto);
                lblMsg.CssClass = "text-success d-block mb-3";
                lblMsg.Text = $"Demanda criada (CodDemanda: {id}).";

                // limpa campos
                txtTitulo.Text = "";
                txtDescricao.Text = "";
                chkAprova.Checked = false;
                ddlAprovador.SelectedIndex = 0;
                txtPrazo.Text = "";
            }
            catch (Exception ex)
            {
                lblMsg.Text = "Erro ao salvar: " + ex.Message;
            }
        }
    }
}