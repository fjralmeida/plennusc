using Plennusc.Core.Service.ServiceGestao.serviceBilling;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class billingReconciliation : System.Web.UI.Page
    {
        private readonly ServiceBillingReconciliation _service = new ServiceBillingReconciliation();

        // Chaves de Session usadas para carregar o contexto da conferência nas próximas etapas
        private const string SESSION_OPERADORA = "BillingReconciliation_CodigoOperadora";
        private const string SESSION_GRUPOS_FATURAMENTO = "BillingReconciliation_CodigosGrupoFaturamento";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarOperadoras();
                CarregarGruposFaturamento();
            }
        }

        private void CarregarOperadoras()
        {
            var operadoras = _service.ObterOperadoras();
            ddlOperadora.DataSource = operadoras;
            ddlOperadora.DataTextField = "NomeOperadora";
            ddlOperadora.DataValueField = "CodigoGrupoContrato";
            ddlOperadora.DataBind();
            ddlOperadora.Items.Insert(0, new ListItem("Selecione...", ""));
        }

        private void CarregarGruposFaturamento()
        {
            var grupos = _service.ObterGruposFaturamento();
            cblGrupoFaturamento.DataSource = grupos;
            cblGrupoFaturamento.DataTextField = "DescricaoGrupoFaturamento";
            cblGrupoFaturamento.DataValueField = "CodigoGrupoFaturamento";
            cblGrupoFaturamento.DataBind();
        }

        protected void btnImportar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlOperadora.SelectedValue))
            {
                ExibirMensagem("Selecione uma operadora antes de importar.", erro: true);
                return;
            }

            if (!fileRelatorio.HasFile)
            {
                ExibirMensagem("Selecione um arquivo para importar.", erro: true);
                return;
            }

            string extensao = System.IO.Path.GetExtension(fileRelatorio.FileName).ToLower();
            var extensoesPermitidas = new[] { ".csv", ".xlsx", ".xls", ".docx" };

            if (!extensoesPermitidas.Contains(extensao))
            {
                ExibirMensagem("Formato inválido. Envie um arquivo .csv, .xlsx, .xls ou .docx.", erro: true);
                return;
            }

            int codigoOperadora = Convert.ToInt32(ddlOperadora.SelectedValue);
            string nomeOperadora = ddlOperadora.SelectedItem.Text;

            List<int> codigosGrupoFaturamento = cblGrupoFaturamento.Items
                .Cast<ListItem>()
                .Where(item => item.Selected)
                .Select(item => Convert.ToInt32(item.Value))
                .ToList();

            Session[SESSION_OPERADORA] = codigoOperadora;
            Session[SESSION_GRUPOS_FATURAMENTO] = codigosGrupoFaturamento;

            try
            {
                using (var streamArquivo = fileRelatorio.PostedFile.InputStream)
                {
                    var itensImportados = _service.ProcessarRelatorioImportado(nomeOperadora, streamArquivo, extensao);

                    Session["BillingReconciliation_ItensImportados"] = itensImportados;

                    // Popula o grid de pré-visualização
                    gridPreview.DataSource = itensImportados;
                    gridPreview.DataBind();
                    divPreview.Attributes.Remove("class");
                    divPreview.Attributes.Add("class", "form-group"); // remove "hidden"

                    ExibirMensagem($"Arquivo '{fileRelatorio.FileName}' importado com sucesso. {itensImportados.Count} registro(s) encontrado(s).", erro: false);
                }
            }
            catch (Exception ex)
            {
                ExibirMensagem("Erro ao processar o arquivo: " + ex.Message, erro: true);
            }
        }

        private void ExibirMensagem(string mensagem, bool erro)
        {
            lblMensagemImportacao.Text = mensagem;
            lblMensagemImportacao.CssClass = "msg-importacao " + (erro ? "erro" : "sucesso");
        }
    }
}