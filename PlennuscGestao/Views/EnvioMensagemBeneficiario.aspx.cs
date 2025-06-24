using appWhatsapp.Models;
using appWhatsapp.Service;
using appWhatsapp.SqlQueries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PlennuscGestao.Views
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
            if(int.TryParse(ddlOperadora.SelectedValue, out int op))
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

            var resultadoFinal = new StringBuilder();

            foreach (var dados in mensagens)
            {
                List<string> telefones = new List<string> { dados.Telefone };
                string resultado = string.Empty;

                switch (escolhaTemplate)
                {
                    case "Suspensao":
                        resultado = await api.ConexaoApiSuspensao(
                            telefones,
                            dados.Field8,
                            dados.Field1,
                            dados.Field2,
                            dados.Field3,
                            dados.Field4
                        );
                        break;

                    case "Definitivo":
                        resultado = await api.ConexaoApiDefinitivo(
                            telefones,
                            dados.Field8,
                            dados.Field1,
                            dados.Field2,
                            dados.Field3,
                            dados.Field4
                        );
                        break;

                    case "DoisBoletos":
                        resultado = await api.ConexaoApiDoisBoletos(
                            telefones,
                            dados.Field1, // substitua se tiver um campo específico
                            dados.Field1,
                            dados.Field2,
                            dados.Field3,
                            dados.Field4,
                            dados.Field5,
                            dados.Field6,
                            dados.Field7
                        );
                        break;

                    default:
                        resultado = "Template não selecionado corretamente.";
                        break;
                }

                resultadoFinal.AppendLine(resultado);
            }
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "MostrarResultado", $"mostrarResultadoModal(`{resultadoFinal}`);", true);

            //lblResultado.Text = resultadoFinal.ToString().Replace("\n", "<br/>");
        }
        protected List<DadosMensagem> ObterLinhasSelecionadas()
        {
            var lista = new List<DadosMensagem>();

            foreach (GridViewRow row in GridAssociados.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chkSelecionar");
                if (chk != null && chk.Checked)
                {
                    //string telefone = "553173069983";
                    ////string telefone = FormatTelefone(telefoneBruto);

                    //if (string.IsNullOrEmpty(telefone))
                    //    continue; // Ignora se for inválido ou fixo

                    string telefoneBruto = ((Label)row.FindControl("lblTelefone"))?.Text?.Trim();
                    string telefone = FormatTelefone(telefoneBruto);

                    if (string.IsNullOrEmpty(telefone))
                        continue; // Ignora se for inválido ou fixo

                    string nome = ((Label)row.FindControl("lblNome"))?.Text?.Trim();
                    string plano = ((Label)row.FindControl("lblPlano"))?.Text?.Trim();
                    string operadora = ((Label)row.FindControl("lblOperadora"))?.Text?.Trim();
                    string vencimento = ((Label)row.FindControl("lblVencimento"))?.Text?.Trim();
                    string vencimentoMes = vencimento;

                    string nomeMes = ""; // Declara fora pra poder usar depois

                    if (DateTime.TryParse(vencimentoMes, out DateTime dataVenc))
                    {
                        nomeMes = dataVenc.ToString("MMMM", new System.Globalization.CultureInfo("pt-BR"));
                        nomeMes = char.ToUpper(nomeMes[0]) + nomeMes.Substring(1); // Primeira letra maiúscula, opcional
                    }

                    //string valor = ((Label)row.FindControl("lblValor"))?.Text?.Trim();
                    string registro = ((Label)row.FindControl("lblRegistro"))?.Text?.Trim();

                    string pdfUrl = $"https://portaldocliente.vallorbeneficios.com.br/ServidorAl2/boletos/boleto_itau_Vallor.php?numeroRegistro={registro}";

                    lista.Add(new DadosMensagem
                    {
                        Telefone = telefone, // Já vem formatado com 5531...
                        Field1 = nome,
                        Field2 = nomeMes,
                        Field3 = plano,
                        Field4 = vencimento,
                        //Field2 = operadora,
                        //Field5 = nome,
                        //Field6 = vencimento,
                        //Field7 = valor.Replace("R$", "").Trim().Replace(",", "."),
                        Field8 = pdfUrl
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