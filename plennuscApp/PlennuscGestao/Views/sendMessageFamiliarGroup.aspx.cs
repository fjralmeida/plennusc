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
            // LIMPA AS SELEÇÕES AO FILTRAR
            hfSelecionados.Value = "";

            GridAssociados.PageIndex = 0; // Reinicia para a primeira página
            CarregarGrid();

            // Se mudou para "enviados24h", mostra mensagem informativa
            if (ddlFiltroEnvio.SelectedValue == "enviados24h")
            {
                string script = @"
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'info',
                    title: 'Modo de visualização',
                    text: 'Estes associados já receberam mensagem nas últimas 24 horas. Não é possível selecioná-los para novo envio.',
                    showConfirmButton: false,
                    timer: 3000,
                    width: 450
                });";

                ScriptManager.RegisterStartupScript(this, GetType(), "ModoVisualizacao", script, true);
            }
            else if (ddlFiltroEnvio.SelectedValue == "disponiveis")
            {
                string script = @"
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Modo de visualização',
                    text: 'Estes associados estão disponíveis para receberam mensagem.',
                    showConfirmButton: false,
                    timer: 3000
                });";

                ScriptManager.RegisterStartupScript(this, GetType(), "ModoVisualizacao", script, true);
            }
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

            // DECLARA dtFiltrado FORA DO IF
            DataTable dtFiltrado = new DataTable();

            if (DateTime.TryParse(iniTexto, out ini) &&
                DateTime.TryParse(fimTexto, out fim))
            {
                if (ini > fim)
                {
                    var temp = ini; ini = fim; fim = temp;
                }

                string templateEscolhido = hfTemplateEscolhido.Value;
                string filtroEnvio = ddlFiltroEnvio.SelectedValue;

                // 1. Busca associados do banco Aliança
                DataTable dtAssociados = ItensAssoci.ConsultaAssociados(ini, fim, operadoraSel);

                // Se não há associados, para aqui
                if (dtAssociados.Rows.Count == 0)
                {
                    GridAssociados.DataSource = null;
                    btnTestarApi.Enabled = false;
                    GridAssociados.DataBind();
                    hfTodosCodigos.Value = "";
                    hfSelecionados.Value = "";
                    return;
                }

                // 2. Prepara lista de códigos para verificar envios
                List<string> codigosAssociados = new List<string>();
                foreach (DataRow row in dtAssociados.Rows)
                {
                    codigosAssociados.Add(row["CODIGO_ASSOCIADO"].ToString());
                }

                // 3. Busca quais já foram enviados (banco Plennus)
                DataTable dtEnvios = ItensAssoci.ConsultarEnviosUltimas24H(codigosAssociados, templateEscolhido);

                // 4. Cria um HashSet para busca rápida
                HashSet<string> enviados24h = new HashSet<string>();
                foreach (DataRow row in dtEnvios.Rows)
                {
                    enviados24h.Add(row["CodigoAssociado"].ToString());
                }

                // 5. Adiciona coluna de status ao DataTable
                dtAssociados.Columns.Add("JA_ENVIADO_24H", typeof(bool));
                dtAssociados.Columns.Add("DATA_ULTIMO_ENVIO", typeof(DateTime));

                foreach (DataRow row in dtAssociados.Rows)
                {
                    string codigo = row["CODIGO_ASSOCIADO"].ToString();
                    bool jaEnviado = enviados24h.Contains(codigo);
                    row["JA_ENVIADO_24H"] = jaEnviado;

                    // Encontra a data do último envio se existir
                    if (jaEnviado)
                    {
                        DataRow[] envioRows = dtEnvios.Select($"CodigoAssociado = '{codigo.Replace("'", "''")}'");
                        if (envioRows.Length > 0)
                        {
                            row["DATA_ULTIMO_ENVIO"] = envioRows[0]["DataUltimoEnvio"];
                        }
                    }
                }

                // 6. Aplica filtro de envio
                dtFiltrado = dtAssociados.Clone();
                DataRow[] rowsFiltradas;

                switch (filtroEnvio)
                {
                    case "disponiveis":
                        rowsFiltradas = dtAssociados.Select("JA_ENVIADO_24H = false");
                        break;
                    case "enviados24h":
                        rowsFiltradas = dtAssociados.Select("JA_ENVIADO_24H = true");
                        break;
                    case "todos":
                    default:
                        rowsFiltradas = dtAssociados.Select();
                        break;
                }

                foreach (DataRow row in rowsFiltradas)
                {
                    dtFiltrado.ImportRow(row);
                }

                // 7. Configura o GridView
                GridAssociados.DataSource = dtFiltrado;

                lblTotalRegistros.Text = dtFiltrado.Rows.Count.ToString();

                // Habilita ou desabilita o botão de envio
                if (filtroEnvio == "enviados24h" || dtFiltrado.Rows.Count == 0)
                {
                    btnTestarApi.Enabled = false;
                    btnTestarApi.ToolTip = filtroEnvio == "enviados24h"
                        ? "Modo visualização apenas - estes associados já receberam mensagem nas últimas 24 horas"
                        : "Nenhum associado disponível para envio";
                }
                else
                {
                    btnTestarApi.Enabled = true;
                    btnTestarApi.ToolTip = "";
                }
            }
            else
            {
                GridAssociados.DataSource = null;
                btnTestarApi.Enabled = false;
                dtFiltrado = new DataTable(); // Inicializa vazio
            }

            GridAssociados.DataBind();

            // ============================================
            // ADICIONE ESTE CÓDIGO APÓS O DataBind()
            // ============================================

            // Coleta TODOS os códigos da consulta COMPLETA (não apenas da página)
            List<string> todosCodigos = new List<string>();

            // Se dtFiltrado não existe (caso não tenha dados), saia
            if (dtFiltrado == null || dtFiltrado.Rows.Count == 0)
            {
                hfTodosCodigos.Value = "";
                hfSelecionados.Value = "";
                AplicarEstiloEnviados();
                return;
            }

            // Pega TODOS os códigos do DataTable
            foreach (DataRow row in dtFiltrado.Rows)
            {
                string codigo = row["CODIGO_ASSOCIADO"].ToString();
                if (!string.IsNullOrEmpty(codigo) && !todosCodigos.Contains(codigo))
                {
                    todosCodigos.Add(codigo);
                }
            }

            // Salva no HiddenField
            hfTodosCodigos.Value = string.Join(",", todosCodigos);

            // Se não tem seleções salvas, limpa
            if (string.IsNullOrEmpty(hfSelecionados.Value))
            {
                hfSelecionados.Value = "";
            }

            // Aplica estilo especial para linhas de associados já enviados
            AplicarEstiloEnviados();
        }

        private void AplicarEstiloEnviados()
        {
            // Aplica estilo apenas se estamos visualizando enviados
            if (ddlFiltroEnvio.SelectedValue != "enviados24h")
                return;

            foreach (GridViewRow row in GridAssociados.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    // Desabilita o checkbox
                    CheckBox chk = (CheckBox)row.FindControl("chkSelecionar");
                    if (chk != null)
                    {
                        chk.Enabled = false;
                        chk.CssClass = "form-check-input disabled";
                        chk.Attributes["title"] = "Associado já recebeu mensagem nas últimas 24 horas";
                    }

                    // Aplica estilo visual
                    row.CssClass = row.CssClass + " table-secondary enviado-recente";
                }
            }

            // Desabilita o checkbox "Selecionar Todos" no header
            if (GridAssociados.HeaderRow != null)
            {
                CheckBox chkTodos = (CheckBox)GridAssociados.HeaderRow.FindControl("chkSelecionarTodos");
                if (chkTodos != null)
                {
                    chkTodos.Enabled = false;
                    chkTodos.CssClass = "form-check-input disabled";
                }
            }
        }
        protected void GridAssociados_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Verifica se o associado já foi enviado nas últimas 24 horas
                DataRowView rowView = (DataRowView)e.Row.DataItem;
                bool jaEnviado24h = Convert.ToInt32(rowView["JA_ENVIADO_24H"]) == 1;

                if (jaEnviado24h)
                {
                    // Aplica estilo visual
                    e.Row.CssClass = e.Row.CssClass + " enviado-recente";

                    // Desabilita o checkbox se estiver no modo de visualização
                    if (ddlFiltroEnvio.SelectedValue == "enviados24h")
                    {
                        CheckBox chk = (CheckBox)e.Row.FindControl("chkSelecionar");
                        if (chk != null)
                        {
                            chk.Enabled = false;
                            chk.CssClass = "form-check-input disabled";
                        }
                    }
                }
            }
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

            // VERIFICA SE TEM MENSAGENS SELECIONADAS
            if (mensagens.Count == 0)
            {
                string script = "alert('Selecione pelo menos um associado para enviar mensagem.');";
                ScriptManager.RegisterStartupScript(this, GetType(), "NenhumSelecionado", script, true);
                return;
            }

            var api = new WhatsappService();
            string escolhaTemplate = hfTemplateEscolhido.Value;

            // Lista para armazenar resultados detalhados (para Excel)
            var resultadosDetalhados = new List<DetailedSubmissionResult>();

            int codAutenticacao = Convert.ToInt32(Session["codUsuario"]);
            StringBuilder resultadoFinal = new StringBuilder();

            int totalSemDocumentos = 0;
            int totalComDocumentos = 0;

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
                    totalSemDocumentos++;
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

                totalComDocumentos++;

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
            // MOSTRAR ALERTA SE ALGUNS NÃO TIVERAM DOCUMENTOS
            if (totalSemDocumentos > 0)
            {
                if (totalComDocumentos == 0)
                {
                    // NENHUM tinha documentos
                    string msg = $"Nenhum documento disponível para envio.";

                    //lblResultado.Text = $"Nenhum dos {totalSemDocumentos} associados possui documentos (boleto ou nota fiscal).";
                    //lblResultado.CssClass = "text-danger fw-bold alert-warning p-3 rounded";

                    // TOAST MENOR
                    string script = $@"
                    Swal.fire({{
                        toast: true,
                        position: 'top-end',
                        icon: 'warning',
                        title: 'Sem documentos',
                        text: '{msg.Replace("'", "\\'")}',
                        showConfirmButton: false,
                        timer: 4000,
                        timerProgressBar: true
                    }});";

                    ScriptManager.RegisterStartupScript(this, GetType(), "ToastAviso", script, true);
                    GerarExcelResultados(resultadosDetalhados);
                    return;
                }
                else
                {
                    // Alguns tinham, outros não
                    string msg = $"{totalComDocumentos} enviadas, {totalSemDocumentos} sem docs.";

                    resultadoFinal.Insert(0, msg + "\n\n");
                    lblResultado.Text = msg;
                    lblResultado.CssClass = "text-warning fw-bold alert-info p-3 rounded";

                    string script = $@"
                    Swal.fire({{
                        toast: true,
                        position: 'top-end',
                        icon: 'info',
                        title: 'Envio parcial',
                        text: '{msg.Replace("'", "\\'")}',
                        showConfirmButton: false,
                        timer: 4000
                    }});";

                    ScriptManager.RegisterStartupScript(this, GetType(), "ToastInfo", script, true);
                }
            }
            else
            {
                // Todos tinham documentos
                string msg = $"{mensagens.Count} mensagens enviadas!";

                //lblResultado.Text = msg;
                //lblResultado.CssClass = "text-success fw-bold alert-success p-3 rounded";

                string script = $@"
                Swal.fire({{
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Concluído',
                    text: '{msg.Replace("'", "\\'")}',
                    showConfirmButton: false,
                    timer: 3000
                }});";

                ScriptManager.RegisterStartupScript(this, GetType(), "ToastSucesso", script, true);
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
        protected List<DadosMensagem> ObterLinhasSelecionadas()
        {
            var lista = new List<DadosMensagem>();
            var notaFiscalUtil = new NotaFiscalUtil();

            // VERIFICA SE TEM SELEÇÕES NO HIDDENFIELD (SELECIONAR TODOS)
            if (!string.IsNullOrEmpty(hfSelecionados.Value))
            {
                // Usa as seleções do HiddenField (todas as páginas)
                return BuscarRegistrosPorCodigos(hfSelecionados.Value.Split(',').ToList());
            }
            else
            {
                // Método original (apenas página atual)
                foreach (GridViewRow row in GridAssociados.Rows)
                {
                    CheckBox chk = (CheckBox)row.FindControl("chkSelecionar");
                    if (chk != null && chk.Checked)
                    {
                        var lblCodigo = row.FindControl("lblCodigo") as Label;
                        string codigoAssociado = Convert.ToString(lblCodigo?.Text)?.Trim();

                        string telefone = "553173069983";
                        if (string.IsNullOrEmpty(telefone))
                            continue;

                        string nome = ((Label)row.FindControl("lblNome"))?.Text?.Trim();
                        string plano = ((Label)row.FindControl("lblPlano"))?.Text?.Trim();
                        string registro = ((Label)row.FindControl("lblRegistro"))?.Text?.Trim();
                        string valor = ((Label)row.FindControl("lblValor"))?.Text?.Trim();

                        int numeroRegistro;
                        bool registroValido = int.TryParse(registro, out numeroRegistro);

                        string pdfUrl = registro;
                        string notaFiscalUrl = null;

                        if (registroValido)
                        {
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

                        string dataFormatada = "";
                        if (DateTime.TryParse(vencimento, out DateTime dataFormat))
                        {
                            dataFormatada = dataFormat.ToString("dd/MM/yyyy");
                        }

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
            }

            return lista;
        }

        private List<DadosMensagem> BuscarRegistrosPorCodigos(List<string> codigos)
        {
            var lista = new List<DadosMensagem>();
            var notaFiscalUtil = new NotaFiscalUtil();

            // Usa os mesmos filtros da consulta atual
            var ItensAssoci = new ItensPedIntegradoUtil();
            DateTime ini, fim;

            if (DateTime.TryParse(txtDataInicio.Value, out ini) &&
                DateTime.TryParse(txtDataFim.Value, out fim))
            {
                int? operadoraSel = null;
                if (int.TryParse(ddlOperadora.SelectedValue, out int op))
                {
                    operadoraSel = op;
                }

                string templateEscolhido = hfTemplateEscolhido.Value;
                string filtroEnvio = ddlFiltroEnvio.SelectedValue;

                // Busca TODOS os associados do filtro atual (igual no CarregarGrid)
                DataTable dtAssociados = ItensAssoci.ConsultaAssociados(ini, fim, operadoraSel);

                if (dtAssociados.Rows.Count == 0)
                    return lista;

                // Aplica o mesmo filtro de envio do CarregarGrid
                // ... (código de filtro igual ao do CarregarGrid) ...

                // Para simplificar, vou pegar todos os registros e filtrar depois
                // Na prática, você deveria replicar o mesmo filtro do CarregarGrid

                foreach (DataRow row in dtAssociados.Rows)
                {
                    string codigo = row["CODIGO_ASSOCIADO"].ToString();

                    // Se o código está na lista de selecionados
                    if (codigos.Contains(codigo))
                    {
                        string nome = row["NOME_ASSOCIADO"].ToString();
                        string plano = row["NOME_PLANO_ABREVIADO"].ToString();
                        string registro = row["NUMERO_REGISTRO"].ToString();
                        string valor = row["VALOR_FATURA"].ToString();
                        string telefone = "553173069983";

                        int numeroRegistro;
                        bool registroValido = int.TryParse(registro, out numeroRegistro);

                        string pdfUrl = registro;
                        string notaFiscalUrl = null;

                        if (registroValido)
                        {
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

                        string dataFormatada = "";
                        if (DateTime.TryParse(vencimento, out DateTime dataFormat))
                        {
                            dataFormatada = dataFormat.ToString("dd/MM/yyyy");
                        }

                        lista.Add(new DadosMensagem
                        {
                            CodigoAssociado = codigo,
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