using Plennusc.Core.Models.ModelsGestao.modelsBilling;
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

        #region CHAMADA DIRETA DE OPERADORAS ESPECIFICAS
        //private void CarregarOperadoras()
        //{
        //    var operadoras = _service.ObterOperadoras();
        //    ddlOperadora.DataSource = operadoras;
        //    ddlOperadora.DataTextField = "NomeOperadora";
        //    ddlOperadora.DataValueField = "CodigoGrupoContrato";
        //    ddlOperadora.DataBind();
        //    ddlOperadora.Items.Insert(0, new ListItem("Selecione...", ""));
        //}
        #endregion

        private void CarregarOperadoras()
        {
            var operadora = _service.ObterOperadoras();

            var lista = new List<OperadoraModel>();
            if (operadora != null)
                lista.Add(operadora);

            ddlOperadora.DataSource = lista;
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

            // ===== Validação do Mês/Ano Referência =====
            string mesAnoReferencia = txtMesAnoReferencia.Text.Trim();

            if (string.IsNullOrEmpty(mesAnoReferencia))
            {
                ExibirMensagem("Informe o Mês/Ano Referência antes de importar.", erro: true);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(mesAnoReferencia, @"^(0[1-9]|1[0-2])\/\d{4}$"))
            {
                ExibirMensagem("Mês/Ano Referência inválido. Use o formato MM/AAAA.", erro: true);
                return;
            }
            // =============================================

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
            Session["BillingReconciliation_MesAnoReferencia"] = mesAnoReferencia; // guardado pra usar depois

            try
            {
                using (var streamArquivo = fileRelatorio.PostedFile.InputStream)
                {
                    var itensImportados = _service.ProcessarRelatorioImportado(nomeOperadora, streamArquivo, extensao);

                    // Preenche o mês/ano em cada item importado
                    foreach (var item in itensImportados)
                    {
                        item.MesAnoReferencia = mesAnoReferencia;
                    }

                    Session["BillingReconciliation_ItensImportados"] = itensImportados;

                    gridPreview.DataSource = itensImportados;
                    gridPreview.DataBind();
                    divPreview.Attributes.Remove("class");
                    divPreview.Attributes.Add("class", "form-group");

                    ExibirMensagem($"Arquivo '{fileRelatorio.FileName}' importado com sucesso. {itensImportados.Count} registro(s) encontrado(s).", erro: false);
                }
            }
            catch (Exception ex)
            {
                ExibirMensagem("Erro ao processar o arquivo: " + ex.Message, erro: true);
            }
        }

        protected void gridPreview_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            var item = e.Row.DataItem as ItemRelatorioImportadoHapVida;
            if (item == null || string.IsNullOrEmpty(item.StatusConferencia)) return;

            switch (item.StatusConferencia)
            {
                case "OK": e.Row.CssClass = "linha-ok"; break;
                case "DIVERGENCIA_TOLERADA": e.Row.CssClass = "linha-divergencia-tolerada"; break;
                case "DIVERGENTE": e.Row.CssClass = "linha-divergente"; break;
                case "NAO_ENCONTRADO": e.Row.CssClass = "linha-nao-encontrado"; break;
            }
        }


        protected string TraduzirStatus(string status)
        {
            switch (status)
            {
                case "OK": return "OK";
                case "DIVERGENCIA_TOLERADA": return "OK (dif. 10 centavos)";
                case "DIVERGENTE": return "Divergente";
                case "NAO_ENCONTRADO": return "Não encontrado";
                default: return status;
            }
        }

        protected void btnConferir_Click(object sender, EventArgs e)
        {
            var itensImportados = Session["BillingReconciliation_ItensImportados"] as List<ItemRelatorioImportadoHapVida>;

            if (itensImportados == null || itensImportados.Count == 0)
            {
                lblMensagemConferencia.Text = "Nenhum item importado para conferir. Importe o relatório novamente.";
                lblMensagemConferencia.CssClass = "msg-importacao erro";
                return;
            }

            string nomeOperadora = ddlOperadora.SelectedItem.Text;
            int codigoGrupoContrato = Convert.ToInt32(ddlOperadora.SelectedValue);
            string tipoConferencia = rblTipoConferencia.SelectedValue; // "CONVENIO" ou "EVENTO_ADICIONAL"

            try
            {
                var itensConferidos = _service.ConferirComView(nomeOperadora, itensImportados, tipoConferencia, codigoGrupoContrato);
                Session["BillingReconciliation_ItensImportados"] = itensConferidos;

                gridPreview.DataSource = itensConferidos;
                gridPreview.DataBind();

                int divergentes = itensConferidos.Count(i => i.StatusConferencia == "DIVERGENTE");
                int naoEncontrados = itensConferidos.Count(i => i.StatusConferencia == "NAO_ENCONTRADO");
                int ok = itensConferidos.Count(i => i.StatusConferencia == "OK" || i.StatusConferencia == "DIVERGENCIA_TOLERADA");

                lblMensagemConferencia.Text = $"Conferência concluída: {ok} OK, {divergentes} divergente(s), {naoEncontrados} não encontrado(s).";
                lblMensagemConferencia.CssClass = "msg-importacao " + (divergentes > 0 || naoEncontrados > 0 ? "erro" : "sucesso");
            }
            catch (Exception ex)
            {
                lblMensagemConferencia.Text = "Erro ao conferir: " + ex.Message;
                lblMensagemConferencia.CssClass = "msg-importacao erro";
            }
        }

        protected void btnExportarDivergentes_Click(object sender, EventArgs e)
        {
            var itens = Session["BillingReconciliation_ItensImportados"] as List<ItemRelatorioImportadoHapVida>;

            if (itens == null || itens.Count == 0)
            {
                lblMensagemConferencia.Text = "Nenhum item conferido para exportar.";
                lblMensagemConferencia.CssClass = "msg-importacao erro";
                return;
            }

            byte[] arquivo = _service.ExportarConferenciaExcel(itens);

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", $"attachment; filename=Conferencia_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            Response.BinaryWrite(arquivo);
            Response.End();
        }

        private void ExibirMensagem(string mensagem, bool erro)
        {
            lblMensagemImportacao.Text = mensagem;
            lblMensagemImportacao.CssClass = "msg-importacao " + (erro ? "erro" : "sucesso");
        }
    }
}