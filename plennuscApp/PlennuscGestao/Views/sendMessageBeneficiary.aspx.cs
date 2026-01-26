using appWhatsapp.Models;
using appWhatsapp.Service;
using appWhatsapp.SqlQueries;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using Plennusc.Core.Models.ModelsGestao.modelsSendMessage;
using Plennusc.Core.SqlQueries.SqlQueriesGestao;
using PlennuscApp.PlennuscGestao.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LicenseContext = OfficeOpenXml.LicenseContext;


namespace PlennuscApp.PlennuscGestao.Views
{
    public partial class EnvioMensagemBeneficiario : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                CarregarOperadoras();
            }
        }

        private void CarregarOperadoras()
        {
            int codAutenticacao = Convert.ToInt32(Session["codUsuario"]);

            OperadoraUtil util = new OperadoraUtil();
            DataTable dtOperadora = util.consultaOperadora();

            if (dtOperadora != null && dtOperadora.Rows.Count > 0)
            {
                ddlOperadora.DataSource = dtOperadora;
                ddlOperadora.DataTextField = "DESCRICAO_GRUPO_CONTRATO";
                ddlOperadora.DataValueField = "CODIGO_GRUPO_CONTRATO";
                ddlOperadora.DataBind();

                // Adiciona a opção "Todas" no início
                ddlOperadora.Items.Insert(0, new ListItem("Todas", ""));
            }
        }
        protected void GridAssociados_PreRender(object sender, EventArgs e)
        {
            if (GridAssociados.HeaderRow != null)
            {
                GridAssociados.UseAccessibleHeader = true;
                GridAssociados.HeaderRow.TableSection = TableRowSection.TableHeader; // necessário pro sticky
            }
        }
        protected void GridAssociados_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridAssociados.PageIndex = e.NewPageIndex;
            CarregarGrid(); // ou refaça o filtro se tiver parâmetros
        }

        protected void ddlPageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            GridAssociados.PageSize = int.Parse(ddlPageSize.SelectedValue);
            GridAssociados.PageIndex = 0; // Volta para a primeira página
            CarregarGrid(); // ou refaça o filtro
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            GridAssociados.PageIndex = 0; // Reinicia para a primeira página
            CarregarGrid();
        }



        private void CarregarGrid()
        {
            var ItensAssoci = new ItensPedIntegradoUtil();

            int? operadoraSel = null;
            if (int.TryParse(ddlOperadora.SelectedValue, out int op))
            {
                operadoraSel = op;
            }

            string iniTexto = txtDataInicio.Value;
            string fimTexto = txtDataFim.Value;

            DateTime ini, fim;

            if (DateTime.TryParse(iniTexto, out ini) &&
                DateTime.TryParse(fimTexto, out fim))
            {
                if (ini > fim)
                {
                    var temp = ini; ini = fim; fim = temp;
                }

                GridAssociados.DataSource = ItensAssoci.ConsultaAssociados(ini, fim, operadoraSel);
                btnTestarApi.Enabled = true;
            }
            else
            {
                GridAssociados.DataSource = null;
            }

            GridAssociados.DataBind();
        }


        #region USAR PARA IMPORTAR PDF
        //protected void btnUpload_Click(object sender, EventArgs e)
        //{
        //    if (FileUpload1.HasFile && FileUpload1.PostedFile.ContentType == "application/pdf")
        //    {
        //        try
        //        {
        //            string fileName = Path.GetFileName(FileUpload1.FileName);
        //            string savePath = Server.MapPath("~/Uploads/") + fileName;

        //            FileUpload1.SaveAs(savePath);

        //            // Gera a URL do PDF
        //            string fileUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Uploads/" + fileName;

        //            // Salva o link em ViewState para ser usado depois
        //            ViewState["PdfUrl"] = fileUrl;

        //            lblStatus.Text = "Arquivo enviado com sucesso! Link: " + fileUrl;
        //        }
        //        catch (Exception ex)
        //        {
        //            lblStatus.Text = "Erro ao enviar o arquivo: " + ex.Message;
        //        }
        //    }
        //    else
        //    {
        //        lblStatus.Text = "Por favor, envie um arquivo PDF válido.";
        //    }
        //}

        //if (ViewState["PdfUrl"] == null)
        //{
        //    lblResultado.Text = "Você precisa enviar um PDF antes de testar a API.";
        //    return;
        //}
        #endregion
        protected async void btnTestarApi_Click(object sender, EventArgs e)
        {
            var mensagens = ObterLinhasSelecionadas();
            var api = new WhatsappService();
            string escolhaTemplate = hfTemplateEscolhido.Value;

            // Lista para armazenar resultados detalhados (para Excel)
            var resultadosDetalhados = new List<DetailedSubmissionResult>();

            int codAutenticacao = Convert.ToInt32(Session["codUsuario"]);
            StringBuilder resultadoFinal = new StringBuilder();

            foreach (var dados in mensagens)
            {
                // 1. Verificar disponibilidade REAL
                var disponibilidade = VerificarDisponibilidade(dados.Field8, dados.NotaFiscalUrl);

                // 2. Ajustar template baseado na disponibilidade REAL
                var templateAjustado = AjustarTemplateSimples(
                    escolhaTemplate,
                    disponibilidade.boletoDisponivel,
                    disponibilidade.notaFiscalDisponivel
                );

                // 3. Se não tem documento disponível, registrar e pular
                if (templateAjustado == "nenhum")
                {
                    resultadosDetalhados.Add(new DetailedSubmissionResult
                    {
                        CodigoAssociado = dados.CodigoAssociado,
                        Nome = dados.Field1,
                        Telefone = dados.Telefone,
                        TemplateEscolhido = escolhaTemplate,
                        TemplateAplicado = "NÃO ENVIADO",
                        BoletoDisponivel = disponibilidade.boletoDisponivel,
                        NotaFiscalDisponivel = disponibilidade.notaFiscalDisponivel,
                        BoletoEnviado = false,
                        NotaFiscalEnviada = false,
                        Status = "NÃO ENVIADO - SEM DOCUMENTOS DISPONÍVEIS",
                        Motivo = $"Boleto: {disponibilidade.boletoDisponivel}, Nota: {disponibilidade.notaFiscalDisponivel}"
                    });
                    continue;
                }

                // 4. Determinar data correta baseada no template ajustado
                string dataParaEnvio = "";
                switch (templateAjustado)
                {
                    case "Suspensao":
                        dataParaEnvio = txtDataSuspensao.Text;
                        break;
                    case "Definitivo":
                        dataParaEnvio = txtDataDefinitivo.Text;
                        break;
                    case "aVencer":
                        dataParaEnvio = txtDataVencer.Text;
                        break;
                }

                // Formatar data para dd/MM/yyyy
                if (DateTime.TryParse(dataParaEnvio, out DateTime dataFormat))
                {
                    dataParaEnvio = dataFormat.ToString("dd/MM/yyyy");
                }
                else
                {
                    // Se não conseguir formatar, usa a data original (já formatada)
                    dataParaEnvio = dados.Field4;
                }

                // 5. Enviar com template ajustado - NÃO MODIFICA OS CAMPOS ORIGINAIS
                List<string> telefones = new List<string> { dados.Telefone };
                string resultado = string.Empty;

                switch (templateAjustado)
                {
                    case "Suspensao":
                        resultado = await api.ConexaoApiSuspensao(
                            telefones,
                            dados.Field8, // pdfUrl (boleto) - ORIGINAL
                            dados.Field1, // nome - ORIGINAL
                            dados.Field3, // plano - ORIGINAL (NÃO MODIFICADO)
                            dataParaEnvio, // vencimento formatado
                            $"R$ {dados.Field7}", // valor formatado
                            templateAjustado, // template AJUSTADO
                            dados.CodigoAssociado,
                            codAutenticacao
                        );
                        break;

                    case "Definitivo":
                        resultado = await api.ConexaoApiNotaFiscal(
                            telefones,
                            dados.NotaFiscalUrl,
                            dados.Field1, // ORIGINAL
                            dados.Field3, // ORIGINAL
                            dataParaEnvio, // vencimento formatado
                            $"R$ {dados.Field7}",
                            templateAjustado, // template AJUSTADO
                            dados.CodigoAssociado,
                            codAutenticacao
                        );
                        break;

                    case "aVencer":
                        resultado = await api.ConexaoApiAVencer(
                            telefones,
                            dados.Field8, // ORIGINAL
                            dados.NotaFiscalUrl, // ORIGINAL
                            dados.Field1, // ORIGINAL
                            dados.Field3, // ORIGINAL
                            dataParaEnvio, // vencimento formatado
                            $"R$ {dados.Field7}" // valor formatado
                        );
                        break;
                }


                // 6. Registrar resultado detalhado
                resultadosDetalhados.Add(new DetailedSubmissionResult
                {
                    CodigoAssociado = dados.CodigoAssociado,
                    Nome = dados.Field1,
                    Telefone = dados.Telefone,
                    TemplateEscolhido = escolhaTemplate,
                    TemplateAplicado = templateAjustado,
                    BoletoDisponivel = disponibilidade.boletoDisponivel,
                    NotaFiscalDisponivel = disponibilidade.notaFiscalDisponivel,
                    BoletoEnviado = templateAjustado == "Suspensao" || templateAjustado == "aVencer",
                    NotaFiscalEnviada = templateAjustado == "Definitivo" || templateAjustado == "aVencer",
                    Status = resultado.Contains("✅") || resultado.Contains("Enviado") ? "ENVIADO" : "ERRO",
                    Motivo = resultado.Length > 200 ? resultado.Substring(0, 200) + "..." : resultado
                });

                resultadoFinal.AppendLine(resultado);
            }

            // 7. Gerar Excel com resultados detalhados
            GerarExcelResultados(resultadosDetalhados);

            // 8. Mostrar resultado resumido no modal
            string resultadoFinalTexto = resultadoFinal.ToString();
            string resultadoEscapado = HttpUtility.JavaScriptStringEncode(resultadoFinalTexto);

            ScriptManager.RegisterStartupScript(
                Page, Page.GetType(), "MostrarResultado",
                $"mostrarResultadoModal('{resultadoEscapado}');", true);
        }

        private (bool boletoDisponivel, bool notaFiscalDisponivel) VerificarDisponibilidade(string pdfUrl, string notaFiscalUrl)
        {
            // Verifica se as URLs são válidas e não estão vazias
            bool boletoOk = !string.IsNullOrWhiteSpace(pdfUrl) && pdfUrl != "0" && pdfUrl != "null";
            bool notaFiscalOk = !string.IsNullOrWhiteSpace(notaFiscalUrl) && notaFiscalUrl != "0" && notaFiscalUrl != "null";

            return (boletoOk, notaFiscalOk);
        }


        // MÉTODO SIMPLIFICADO - Só verifica disponibilidade
        private string AjustarTemplateSimples(
            string templateEscolhido,
            bool boletoDisponivel,
            bool notaFiscalDisponivel)
        {
            switch (templateEscolhido)
            {
                case "Suspensao":
                    return boletoDisponivel ? "Suspensao" : "nenhum";

                case "Definitivo":
                    return notaFiscalDisponivel ? "Definitivo" : "nenhum";

                case "aVencer":
                    if (boletoDisponivel && notaFiscalDisponivel)
                        return "aVencer";
                    else if (boletoDisponivel)
                        return "Suspensao";
                    else if (notaFiscalDisponivel)
                        return "Definitivo";
                    else
                        return "nenhum";

                default:
                    return "nenhum";
            }
        }



        // MÉTODO SIMPLIFICADO - Só verifica disponibilidade
        private string AjustarTemplate(
            string templateEscolhido,
            bool boletoDisponivel,
            bool notaFiscalDisponivel)
        {
            switch (templateEscolhido)
            {
                case "Suspensao":
                    return boletoDisponivel ? "Suspensao" : "nenhum";

                case "Definitivo":
                    return notaFiscalDisponivel ? "Definitivo" : "nenhum";

                case "aVencer":
                    if (boletoDisponivel && notaFiscalDisponivel)
                        return "aVencer";
                    else if (boletoDisponivel)
                        return "Suspensao";
                    else if (notaFiscalDisponivel)
                        return "Definitivo";
                    else
                        return "nenhum";

                default:
                    return "nenhum";
            }
        }
        protected List<DadosMensagem> ObterLinhasSelecionadas()
        {
            var lista = new List<DadosMensagem>();
            var notaFiscalUtil = new NotaFiscalUtil(); // Corrigido o nome da variável

            foreach (GridViewRow row in GridAssociados.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chkSelecionar");
                if (chk != null && chk.Checked)
                {
                    var lblCodigo = row.FindControl("lblCodigo") as Label;
                    string codigoAssociado = Convert.ToString(lblCodigo?.Text)?.Trim();

                    string telefone = "553173069983";
                    //string telefone = FormatTelefone(telefoneBruto);

                    if (string.IsNullOrEmpty(telefone))
                        continue; // Ignora se for inválido ou fixo

                    //string telefoneBruto = ((Label)row.FindControl("lblTelefone"))?.Text?.Trim();
                    //string telefone = FormatTelefone(telefoneBruto);

                    //if (string.IsNullOrEmpty(telefone))
                    //    continue; // Ignora se for inválido ou fixo

                    string nome = ((Label)row.FindControl("lblNome"))?.Text?.Trim();
                    string plano = ((Label)row.FindControl("lblPlano"))?.Text?.Trim();

                    // OBTÉM O REGISTRO ANTES DE USAR
                    string registro = ((Label)row.FindControl("lblRegistro"))?.Text?.Trim();
                    string valor = ((Label)row.FindControl("lblValor"))?.Text?.Trim();

                    int numeroRegistro;
                    bool registroValido = int.TryParse(registro, out numeroRegistro);

                    //string pdfUrl = $"https://portaldocliente.vallorbeneficios.com.br/ServidorAl2/boletos/boleto_itau_Vallor.php?numeroRegistro={registro}";

                    string pdfUrl = registro;
                    string notaFiscalUrl = null;

                    // Busca URL da nota fiscal apenas se o registro for válido
                    if (registroValido)
                    {
                        // AGORA USANDO O MÉTODO CORRETO
                        notaFiscalUrl = notaFiscalUtil.BuscarUrlNotaFiscal(numeroRegistro);
                    }

                    string vencimento = "";

                    if (hfTemplateEscolhido.Value == "Suspensao")
                    {
                        vencimento = txtDataSuspensao.Text;
                    }
                    else if (hfTemplateEscolhido.Value == "Definitivo")
                    {
                        vencimento = txtDataDefinitivo.Text;
                    }
                    //else if (hfTemplateEscolhido.Value == "DoisBoletos")
                    //{
                    //    vencimento = txtDataNovaOpcao.Text;
                    //}
                    else if (hfTemplateEscolhido.Value == "aVencer")
                    {
                        vencimento = txtDataVencer.Text;
                    }

                    string nomeMes = "";
                    if (DateTime.TryParse(vencimento, out DateTime dataVenc))
                    {
                        nomeMes = dataVenc.ToString("MMMM", new System.Globalization.CultureInfo("pt-BR"));
                        nomeMes = char.ToUpper(nomeMes[0]) + nomeMes.Substring(1);
                    }

                    // Formata data para dd/MM/yyyy
                    string dataFormatada = "";
                    if (DateTime.TryParse(vencimento, out DateTime dataFormat))
                    {
                        dataFormatada = dataFormat.ToString("dd/MM/yyyy");
                    }

                    // Formata valor para R$ X.XXX,XX
                    string valorFormatado = "R$ " + valor.Replace("R$", "").Trim();

                    lista.Add(new DadosMensagem
                    {
                        CodigoAssociado = codigoAssociado,
                        Telefone = telefone,
                        Field1 = nome,
                        Field2 = nomeMes,
                        Field3 = plano,
                        Field4 = dataFormatada,
                        Field7 = valor.Replace("R$", "").Trim().Replace(",", "."),
                        Field8 = pdfUrl,
                        NotaFiscalUrl = notaFiscalUrl
                    });
                }
            }

            return lista;
        }
        private string FormatTelefone(string telefone)
        {
            if (string.IsNullOrWhiteSpace(telefone))
                return null;

            // Remove tudo que não for número
            var clean = new string(telefone.Where(char.IsDigit).ToArray());

            // Ignora fixo (8 dígitos iniciando com 3)
            if (clean.Length == 8 && clean.StartsWith("3"))
                return null;

            // Caso tenha 9 dígitos (sem DDD), remover o 9 e adicionar DDD 31
            if (clean.Length == 9 && clean.StartsWith("9"))
            {
                return "55" + "31" + clean.Substring(1);
            }

            // Caso tenha 10 dígitos (DDD + número sem 9) — já está ok
            if (clean.Length == 10)
            {
                return "55" + clean;
            }

            // Caso tenha 11 dígitos (DDD + 9 + número), remover o 9
            if (clean.Length == 11 && clean[2] == '9')
            {
                return "55" + clean.Substring(0, 2) + clean.Substring(3);
            }

            // Se não bater com nenhum formato válido
            return null;
        }


        private void GerarExcelResultados(List<DetailedSubmissionResult> resultados)
            {
                try
                {
                    // Chama o serviço de Excel que você já tem pronto
                    var excelService = new ExcelReportService();
                    byte[] excelBytes = excelService.GerarExcelRelatorio(resultados);

                    // Salva na sessão
                    Session["ExcelBytes"] = excelBytes;
                    Session["ExcelFileName"] = $"Relatorio_Envio_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                }
                catch (Exception ex)
                {
                    // Se der erro com o Excel, usa fallback CSV
                    var excelService = new ExcelReportService();
                    byte[] csvBytes = excelService.GerarCSVResumo(resultados);

                    Session["ExcelBytes"] = csvBytes;
                    Session["ExcelFileName"] = $"Relatorio_Envio_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                }
            }

        protected void btnDownloadExcel_Click(object sender, EventArgs e)
        {
            byte[] fileBytes = Session["ExcelBytes"] as byte[];
            string fileName = Session["ExcelFileName"] as string;

            if (fileBytes != null && fileBytes.Length > 0)
            {
                Response.Clear();

                // Determinar content type baseado na extensão
                if (fileName.EndsWith(".xlsx"))
                {
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                }
                else
                {
                    Response.ContentType = "text/csv";
                }

                Response.AddHeader("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                Response.AddHeader("Content-Length", fileBytes.Length.ToString());
                Response.BinaryWrite(fileBytes);
                Response.Flush();

                // Limpar sessão
                Session.Remove("ExcelBytes");
                Session.Remove("ExcelFileName");
                Response.End();
            }
            else
            {
                string script = "alert('Nenhum relatório disponível para download.');";
                ScriptManager.RegisterStartupScript(this, GetType(), "NoDataAlert", script, true);
            }
        }

        protected void btnEscMens_Click(object sender, EventArgs e)
            {
                string escolherTemplete = hfTemplateEscolhido.Value;

                if (string.IsNullOrEmpty(escolherTemplete))
                {
                    lblResultado.Text = "Selecionar um templete";
                    return;
                }
                lblResultado.Text = $"Templete escolhido: {escolherTemplete}";
            }
    }
}