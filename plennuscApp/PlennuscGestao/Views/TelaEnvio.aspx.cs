
//using appWhatsapp.SqlQueries;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Threading.Tasks;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System.Text;
//using appWhatsapp.Service;
//using System.IO;
//using Microsoft.Win32;
//using appWhatsapp.Models;

//namespace appWhatsapp.Controller
//{
//    public partial class TelaEnvio : System.Web.UI.Page
//    {
//        protected void Page_Load(object sender, EventArgs e)
//        {
//            if (!Page.IsPostBack)
//            {
//                //MontaDetalhesPedido();
//            }
//        }
//        //private void MontaDetalhesPedido(DateTime? ini = null, DateTime? fim = null)
//        //{
//        //    ItensPedIntegradoUtil ItensAssoci = new ItensPedIntegradoUtil();
//        //    GridAssociados.DataSource = ItensAssoci.ConsultaAssociados(ini, fim);
//        //    GridAssociados.DataBind();
//        //}

//        protected void btnFiltrar_Click(object sender, EventArgs e)
//        {
//            var ItensAssoci = new ItensPedIntegradoUtil();

//            string iniTexto = txtDataInicio.Value;
//            string fimTexto = txtDataFim.Value;

//            DateTime ini, fim;

//            if (DateTime.TryParse(iniTexto, out ini) &&
//                DateTime.TryParse(fimTexto, out fim))
//            {
//                if (ini > fim)
//                {
//                    var temp = ini; ini = fim; fim = temp;
//                }

//                GridAssociados.DataSource = ItensAssoci.ConsultaAssociados(ini, fim);
//            }
//            else
//            {
//                // Filtro incompleto: não consulta nada
//                GridAssociados.DataSource = null;
//            }

//            GridAssociados.DataBind();
//        }

//        #region USAR PARA IMPORTAR PDF
//        //protected void btnUpload_Click(object sender, EventArgs e)
//        //{
//        //    if (FileUpload1.HasFile && FileUpload1.PostedFile.ContentType == "application/pdf")
//        //    {
//        //        try
//        //        {
//        //            string fileName = Path.GetFileName(FileUpload1.FileName);
//        //            string savePath = Server.MapPath("~/Uploads/") + fileName;

//        //            FileUpload1.SaveAs(savePath);

//        //            // Gera a URL do PDF
//        //            string fileUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Uploads/" + fileName;

//        //            // Salva o link em ViewState para ser usado depois
//        //            ViewState["PdfUrl"] = fileUrl;

//        //            lblStatus.Text = "Arquivo enviado com sucesso! Link: " + fileUrl;
//        //        }
//        //        catch (Exception ex)
//        //        {
//        //            lblStatus.Text = "Erro ao enviar o arquivo: " + ex.Message;
//        //        }
//        //    }
//        //    else
//        //    {
//        //        lblStatus.Text = "Por favor, envie um arquivo PDF válido.";
//        //    }
//        //}

//        //if (ViewState["PdfUrl"] == null)
//        //{
//        //    lblResultado.Text = "Você precisa enviar um PDF antes de testar a API.";
//        //    return;
//        //}
//        #endregion
//        protected async void btnTestarApi_Click(object sender, EventArgs e)
//        {
//            var mensagens = ObterLinhasSelecionadas();

//            var api = new WhatsappService();
//            var resultadoFinal = new StringBuilder();

//            foreach (var dados in mensagens)
//            {
//                string resultado = await api.ConexaoApiSuspensao(
//                    new List<string> { dados.Telefone }, 
//                    dados.Field8,                        
//                    dados.Field1,
//                    dados.Field2,
//                    dados.Field3,
//                    dados.Field4,
//                    dados.Field5,
//                    dados.Field6,
//                    dados.Field7
//                );

//                resultadoFinal.AppendLine(resultado);
//            }

//            lblResultado.Text = resultadoFinal.ToString().Replace("\n", "<br/>");
//        }
//        protected List<DadosMensagem> ObterLinhasSelecionadas()
//        {
//            var lista = new List<DadosMensagem>();

//            foreach (GridViewRow row in GridAssociados.Rows)
//            {
//                CheckBox chk = (CheckBox)row.FindControl("chkSelecionar");
//                if (chk != null && chk.Checked)
//                {
//                    string telefone = "553173069983";
//                    //string telefone = FormatTelefone(telefoneBruto);

//                    if (string.IsNullOrEmpty(telefone))
//                        continue; // Ignora se for inválido ou fixo

//                    string nome = ((Label)row.FindControl("lblNome"))?.Text?.Trim();
//                    string plano = ((Label)row.FindControl("lblPlano"))?.Text?.Trim();
//                    string operadora = ((Label)row.FindControl("lblOperadora"))?.Text?.Trim();
//                    string vencimento = ((Label)row.FindControl("lblVencimento"))?.Text?.Trim();
//                    string valor = ((Label)row.FindControl("lblValor"))?.Text?.Trim();
//                    string registro = ((Label)row.FindControl("lblRegistro"))?.Text?.Trim();

//                    string pdfUrl = $"https://portaldocliente.vallorbeneficios.com.br/ServidorAl2/boletos/boleto_itau_Vallor.php?numeroRegistro={registro}";

//                    lista.Add(new DadosMensagem
//                    {
//                        Telefone = telefone, // Já vem formatado com 5531...
//                        Field1 = nome,
//                        Field2 = plano,
//                        Field3 = operadora,
//                        Field4 = vencimento,
//                        Field5 = nome,
//                        Field6 = vencimento,
//                        Field7 = valor.Replace("R$", "").Trim().Replace(",", "."),
//                        Field8 = pdfUrl
//                    });
//                }
//            }

//            return lista;
//        }
//        private string FormatTelefone(string telefone)
//        {
//            if (string.IsNullOrWhiteSpace(telefone))
//                return null;

//            // Remove tudo que não for número
//            var clean = new string(telefone.Where(char.IsDigit).ToArray());

//            // Ignora fixo (8 dígitos iniciando com 3)
//            if (clean.Length == 8 && clean.StartsWith("3"))
//                return null;

//            // Caso tenha 9 dígitos (sem DDD), remover o 9 e adicionar DDD 31
//            if (clean.Length == 9 && clean.StartsWith("9"))
//            {
//                return "55" + "31" + clean.Substring(1); 
//            }

//            // Caso tenha 10 dígitos (DDD + número sem 9) — já está ok
//            if (clean.Length == 10)
//            {
//                return "55" + clean;
//            }

//            // Caso tenha 11 dígitos (DDD + 9 + número), remover o 9
//            if (clean.Length == 11 && clean[2] == '9')
//            {
//                return "55" + clean.Substring(0, 2) + clean.Substring(3);
//            }

//            // Se não bater com nenhum formato válido
//            return null;
//        }



//    }
//}
