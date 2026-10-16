using Plennusc.Core.Models.ModelsGestao.modelsBilling;
using Plennusc.Core.Service.ServiceGestao.serviceBilling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class billingReconciliation : System.Web.UI.Page
    {
        private readonly ServiceBillingReconciliation _service = new ServiceBillingReconciliation();

        private const string SESSION_OPERADORA = "BillingReconciliation_CodigoOperadora";
        private const string SESSION_GRUPOS_FATURAMENTO = "BillingReconciliation_CodigosGrupoFaturamento";
        private const string SESSION_ITENS_IMPORTADOS = "BillingReconciliation_ItensImportados";
        private const string SESSION_MES_ANO = "BillingReconciliation_MesAnoReferencia";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarOperadoras();
                CarregarGruposFaturamento();
                ConfigurarRadioButtonList();
            }
        }

        #region CARREGAMENTO DE DADOS BÁSICOS

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

        /// <summary>
        /// Garante que os dois itens estejam sempre disponíveis no RadioButtonList
        /// </summary>
        private void ConfigurarRadioButtonList()
        {
            if (rblTipoConferencia.Items.FindByValue("CONVENIO") == null)
                rblTipoConferencia.Items.Insert(0, new ListItem("Convênio", "CONVENIO"));

            if (rblTipoConferencia.Items.FindByValue("EVENTO_ADICIONAL") == null)
                rblTipoConferencia.Items.Add(new ListItem("Odontológico (Evento Adicional)", "EVENTO_ADICIONAL"));

            rblTipoConferencia.SelectedValue = "CONVENIO";
        }

        #endregion

        #region IMPORTAÇÃO DO RELATÓRIO

        protected void btnImportar_Click(object sender, EventArgs e)
        {
            if (!ValidarCamposImportacao(out string mensagemErro))
            {
                ExibirMensagem(mensagemErro, erro: true);
                return;
            }

            string nomeOperadora = ddlOperadora.SelectedItem.Text;
            int codigoOperadora = Convert.ToInt32(ddlOperadora.SelectedValue);
            string mesAnoReferencia = txtMesAnoReferencia.Text.Trim();
            string extensao = System.IO.Path.GetExtension(fileRelatorio.FileName).ToLower();

            var codigosGrupoFaturamento = cblGrupoFaturamento.Items
                .Cast<ListItem>()
                .Where(item => item.Selected)
                .Select(item => Convert.ToInt32(item.Value))
                .ToList();

            Session[SESSION_OPERADORA] = codigoOperadora;
            Session[SESSION_GRUPOS_FATURAMENTO] = codigosGrupoFaturamento;
            Session[SESSION_MES_ANO] = mesAnoReferencia;

            try
            {
                using (var streamArquivo = fileRelatorio.PostedFile.InputStream)
                {
                    var itensImportados = _service.ProcessarRelatorioImportado(nomeOperadora, streamArquivo, extensao);

                    foreach (var item in itensImportados)
                    {
                        item.MesAnoReferencia = mesAnoReferencia;
                    }

                    Session[SESSION_ITENS_IMPORTADOS] = itensImportados;

                    // ===== DEFINIÇÕES DE COMPORTAMENTO POR OPERADORA =====
                    bool ehCasLorena = IsCasLorena(nomeOperadora);
                    bool ehHapvida = !ehCasLorena && IsHapvida(nomeOperadora);
                    bool ehUniaoMedica = !ehCasLorena && IsUniaoMedica(nomeOperadora);
                    bool ehUnimed = !ehCasLorena && IsUnimed(nomeOperadora);
                    bool ehAurora = !ehCasLorena && IsAurora(nomeOperadora);

                    // 1. Painel de Tipo de Conferência: visível APENAS para Hapvida
                    pnlTipoConferencia.Visible = ehHapvida;

                    // 2. Operadoras que NÃO usam tipo de conferência (forçam CONVENIO e removem odontológico):
                    //    União Médica, Aurora e CAS Lorena
                    bool operadoraSemTipoConferencia = ehUniaoMedica || ehAurora || ehCasLorena;

                    if (operadoraSemTipoConferencia)
                    {
                        var itemOdonto = rblTipoConferencia.Items.FindByValue("EVENTO_ADICIONAL");
                        if (itemOdonto != null)
                            rblTipoConferencia.Items.Remove(itemOdonto);

                        var itemConvenio = rblTipoConferencia.Items.FindByValue("CONVENIO");
                        if (itemConvenio == null)
                            rblTipoConferencia.Items.Insert(0, new ListItem("Convênio", "CONVENIO"));
                        rblTipoConferencia.SelectedValue = "CONVENIO";
                    }
                    else
                    {
                        if (rblTipoConferencia.Items.FindByValue("EVENTO_ADICIONAL") == null)
                            rblTipoConferencia.Items.Add(new ListItem("Odontológico (Evento Adicional)", "EVENTO_ADICIONAL"));
                    }

                    // 3. Ajustar colunas do grid
                    AjustarColunasGridPorOperadora(nomeOperadora);

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

        private bool ValidarCamposImportacao(out string mensagemErro)
        {
            mensagemErro = null;

            if (string.IsNullOrEmpty(ddlOperadora.SelectedValue))
            {
                mensagemErro = "Selecione uma operadora antes de importar.";
                return false;
            }

            string mesAnoReferencia = txtMesAnoReferencia.Text.Trim();
            if (string.IsNullOrEmpty(mesAnoReferencia))
            {
                mensagemErro = "Informe o Mês/Ano Referência antes de importar.";
                return false;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(mesAnoReferencia, @"^(0[1-9]|1[0-2])\/\d{4}$"))
            {
                mensagemErro = "Mês/Ano Referência inválido. Use o formato MM/AAAA.";
                return false;
            }

            if (!fileRelatorio.HasFile)
            {
                mensagemErro = "Selecione um arquivo para importar.";
                return false;
            }

            string extensao = System.IO.Path.GetExtension(fileRelatorio.FileName).ToLower();
            var extensoesPermitidas = new[] { ".csv", ".xlsx", ".xls", ".docx", ".txt" };
            if (!extensoesPermitidas.Contains(extensao))
            {
                mensagemErro = "Formato inválido. Envie um arquivo .csv, .xlsx, .xls ou .docx.";
                return false;
            }

            return true;
        }

        #endregion

        #region AJUSTES DO GRID

        private void AjustarColunasGridPorOperadora(string nomeOperadora)
        {
            bool ehCasLorena = IsCasLorena(nomeOperadora);
            bool ehHapvida = !ehCasLorena && IsHapvida(nomeOperadora);
            bool ehUniaoMedica = !ehCasLorena && IsUniaoMedica(nomeOperadora);
            bool ehUnimed = !ehCasLorena && IsUnimed(nomeOperadora);
            bool ehAurora = !ehCasLorena && IsAurora(nomeOperadora);

            foreach (DataControlField coluna in gridPreview.Columns)
            {
                if (coluna is BoundField boundField)
                {
                    string dataField = boundField.DataField;

                    if (dataField == "Nascimento")
                        coluna.Visible = ehHapvida || ehUniaoMedica;
                    else if (dataField == "Adicional" || dataField == "NomeTabelaPreco" || dataField == "DescricaoGrupoFaturamento")
                        coluna.Visible = ehHapvida || ehUniaoMedica || ehCasLorena;
                    else if (dataField == "Parentesco")
                        coluna.Visible = ehHapvida;
                    else if (dataField == "Empresa")
                        coluna.Visible = ehHapvida || ehCasLorena;
                    else if (dataField == "Plano")
                        coluna.Visible = !ehUniaoMedica;
                    else if (dataField == "Credito" || dataField == "Debito" ||
                             dataField == "CodigoEmpresa" || dataField == "EmpresaUnimed")
                        coluna.Visible = ehUnimed;
                }
            }
        }

        protected void gridPreview_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            var item = e.Row.DataItem as ItemRelatorioImportadoHapVida;
            if (item == null || string.IsNullOrEmpty(item.StatusConferencia)) return;

            switch (item.StatusConferencia)
            {
                case "OK":
                    e.Row.CssClass = "linha-ok";
                    break;
                case "DIVERGENCIA_TOLERADA":
                    e.Row.CssClass = "linha-divergencia-tolerada";
                    break;
                case "DIVERGENTE":
                    e.Row.CssClass = "linha-divergente";
                    break;
                case "NAO_ENCONTRADO":
                    e.Row.CssClass = "linha-nao-encontrado";
                    break;
                case "CARTEIRINHA_NAO_ENCONTRADA":
                    e.Row.CssClass = "linha-carteirinha-nao-encontrada";
                    break;
            }
        }

        #endregion

        #region CONFERÊNCIA

        protected void btnConferir_Click(object sender, EventArgs e)
        {
            var itensImportados = Session[SESSION_ITENS_IMPORTADOS] as List<ItemRelatorioImportadoHapVida>;

            if (itensImportados == null || itensImportados.Count == 0)
            {
                lblMensagemConferencia.Text = "Nenhum item importado para conferir. Importe o relatório novamente.";
                lblMensagemConferencia.CssClass = "msg-importacao erro";
                return;
            }

            string nomeOperadora = ddlOperadora.SelectedItem.Text;
            int codigoGrupoContrato = Convert.ToInt32(ddlOperadora.SelectedValue);
            string tipoConferencia = rblTipoConferencia.SelectedValue;

            try
            {
                var itensConferidos = _service.ConferirComView(nomeOperadora, itensImportados, tipoConferencia, codigoGrupoContrato);
                Session[SESSION_ITENS_IMPORTADOS] = itensConferidos;

                // Reaplica a visibilidade das colunas no postback (o estado das colunas não persiste)
                AjustarColunasGridPorOperadora(nomeOperadora);

                gridPreview.DataSource = itensConferidos;
                gridPreview.DataBind();

                _service.ConferirFaturamento(itensConferidos);

                int ok = itensConferidos.Count(i => i.StatusConferencia == "OK" || i.StatusConferencia == "DIVERGENCIA_TOLERADA");
                int divergentes = itensConferidos.Count(i => i.StatusConferencia == "DIVERGENTE");
                int naoEncontrados = itensConferidos.Count(i => i.StatusConferencia == "NAO_ENCONTRADO");

                lblMensagemConferencia.Text = $"Conferência concluída: {ok} OK, {divergentes} divergente(s), {naoEncontrados} não encontrado(s).";
                lblMensagemConferencia.CssClass = "msg-importacao " + (divergentes > 0 || naoEncontrados > 0 ? "erro" : "sucesso");
            }
            catch (Exception ex)
            {
                lblMensagemConferencia.Text = "Erro ao conferir: " + ex.Message;
                lblMensagemConferencia.CssClass = "msg-importacao erro";
            }
        }

        #endregion

        #region EXPORTAÇÃO

        protected void btnExportarDivergentes_Click(object sender, EventArgs e)
        {
            var itens = Session[SESSION_ITENS_IMPORTADOS] as List<ItemRelatorioImportadoHapVida>;

            if (itens == null || itens.Count == 0)
            {
                lblMensagemConferencia.Text = "Nenhum item conferido para exportar.";
                lblMensagemConferencia.CssClass = "msg-importacao erro";
                return;
            }

            string nomeOperadora = ddlOperadora.SelectedItem.Text;
            string codigoOperadora = DeterminarCodigoOperadora(nomeOperadora);

            if (string.IsNullOrEmpty(codigoOperadora))
            {
                lblMensagemConferencia.Text = "Operadora não reconhecida para exportação.";
                lblMensagemConferencia.CssClass = "msg-importacao erro";
                return;
            }

            byte[] arquivo = _service.ExportarConferenciaExcel(itens, codigoOperadora);

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", $"attachment; filename=Conferencia_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
            Response.BinaryWrite(arquivo);
            Response.End();
        }

        #endregion

        #region MÉTODOS AUXILIARES

        private string DeterminarCodigoOperadora(string nomeOperadora)
        {
            if (IsCasLorena(nomeOperadora))
                return "CAS_LORENA";

            if (IsHapvida(nomeOperadora))
                return "HAPVIDA";

            if (IsUnimed(nomeOperadora))
                return "UNIMED";

            if (IsUniaoMedica(nomeOperadora))
                return "UNIAO_MEDICA";

            if (IsAurora(nomeOperadora))
                return "AURORA";

            return null;
        }

        private bool IsHapvida(string nome)
        {
            return nome.IndexOf("HAPVIDA", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool IsUnimed(string nome)
        {
            return nome.IndexOf("UNIMED", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool IsUniaoMedica(string nome)
        {
            return nome.IndexOf("UNIÃO MÉDICA", StringComparison.OrdinalIgnoreCase) >= 0 ||
                   nome.IndexOf("UNIAO MEDICA", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool IsAurora(string nome)
        {
            return nome.IndexOf("AURORA", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private bool IsCasLorena(string nome)
        {
            return ServiceBillingReconciliation.IsCasLorena(nome);
        }

        private void ExibirMensagem(string mensagem, bool erro)
        {
            lblMensagemImportacao.Text = mensagem;
            lblMensagemImportacao.CssClass = "msg-importacao " + (erro ? "erro" : "sucesso");
        }

        public string TraduzirStatus(string status)
        {
            switch (status)
            {
                case "OK":
                    return "OK";
                case "DIVERGENCIA_TOLERADA":
                    return "OK (dif. 10 centavos)";
                case "DIVERGENTE":
                    return "Divergente";
                case "NAO_ENCONTRADO":
                    return "Não encontrado";
                case "CARTEIRINHA_NAO_ENCONTRADA":
                    return "Carteirinha não encontrada";
                default:
                    return status;
            }
        }

        #endregion
    }
}