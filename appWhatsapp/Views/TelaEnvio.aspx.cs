using appWhatsapp.SqlQueries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using appWhatsapp.Service;
using System.IO;

namespace appWhatsapp.Controller
{
    public partial class TelaEnvio : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                MontaDetalhesPedido();
            }
        }
        private void MontaDetalhesPedido(DateTime? ini = null, DateTime? fim = null)
        {
            ItensPedIntegradoUtil ItensAssoci = new ItensPedIntegradoUtil();
            GridAssociados.DataSource = ItensAssoci.ConsultaAssociados(ini, fim);
            GridAssociados.DataBind();
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            var ItensAssoci = new ItensPedIntegradoUtil();

            // Lê os dois campos
            string iniTexto = txtDataInicio.Value;
            string fimTexto = txtDataFim.Value;

            DateTime ini, fim;

            // Ambos preenchidos?
            if (DateTime.TryParse(iniTexto, out ini) &&
                DateTime.TryParse(fimTexto, out fim))
            {
                // Garante ini <= fim
                if (ini > fim)
                {
                    // exibe mensagem ou apenas troca as datas
                    var temp = ini; ini = fim; fim = temp;
                }

                GridAssociados.DataSource = ItensAssoci.ConsultaAssociados(ini, fim);
            }
            else
            {
                // Sem filtro ou filtro incompleto → lista tudo
                GridAssociados.DataSource = ItensAssoci.ConsultaAssociados();
            }

            GridAssociados.DataBind();
        }

        protected void btnSelecionar_Click(object sender, EventArgs e)
        {
            List<string> codigosSelecionados = new List<string>();

            foreach (GridViewRow row in GridAssociados.Rows)
            {
                CheckBox chk = (CheckBox)row.FindControl("chkSelecionar");

                if (chk != null && chk.Checked)
                {
                    // Aqui você pode pegar qualquer campo da linha (ex: código do associado)
                    string codigo = ((Label)row.Cells[1].FindControl("Label1"))?.Text; // Ou use Eval("CODIGO_ASSOCIADO") de outra forma
                    codigosSelecionados.Add(codigo);
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
          
            int codRegistro = 0;

            string pdfUrl = $"https://portaldocliente.vallorbeneficios.com.br/ServidorAl2/boletos/boleto_itau_Vallor.php?numeroRegistro={codRegistro}";

            var api = new WhatsappService();
            var telefones = new List<string>
                {
                    "553173069983"
                    //"553193046521",
                    //"553191927191",
                    //"553192150702",
                    //"553194768261",
                    //"553188466155",
                    //"553195778272",
                    //"553171043323",
                    //"553194308813",
                    //"553183312484",
                    //"553175402922",
                    //"553198114194"
                };

            string resultado = await api.ConexaoApiAsync(telefones, pdfUrl);
            lblResultado.Text = resultado.Replace("\n", "<br/>");
        }

    }
}