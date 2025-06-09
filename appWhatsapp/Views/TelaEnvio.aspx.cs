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
        private void MontaDetalhesPedido()
        {
            ItensPedIntegradoUtil ItensAssoci = new ItensPedIntegradoUtil();
            GridAssociados.DataSource = ItensAssoci.ConsultaAssociados();
            GridAssociados.DataBind();
        }

        protected void btnUpload_Click(object sender, EventArgs e)
        {
            if (FileUpload1.HasFile && FileUpload1.PostedFile.ContentType == "application/pdf")
            {
                try
                {
                    string fileName = Path.GetFileName(FileUpload1.FileName);
                    string savePath = Server.MapPath("~/Uploads/") + fileName;

                    FileUpload1.SaveAs(savePath);

                    // Gera a URL do PDF
                    string fileUrl = Request.Url.GetLeftPart(UriPartial.Authority) + "/Uploads/" + fileName;

                    // Salva o link em ViewState para ser usado depois
                    ViewState["PdfUrl"] = fileUrl;

                    lblStatus.Text = "Arquivo enviado com sucesso! Link: " + fileUrl;
                }
                catch (Exception ex)
                {
                    lblStatus.Text = "Erro ao enviar o arquivo: " + ex.Message;
                }
            }
            else
            {
                lblStatus.Text = "Por favor, envie um arquivo PDF válido.";
            }
        }

        protected async void btnTestarApi_Click(object sender, EventArgs e)
        {
            if (ViewState["PdfUrl"] == null)
            {
                lblResultado.Text = "Você precisa enviar um PDF antes de testar a API.";
                return;
            }

            string pdfUrl = ViewState["PdfUrl"].ToString();

            var api = new WhatsappService();
            var telefones = new List<string>
                {
                    "553173069983",
                    //"553193046521",
                    //"553191927191",
                    //"553192150702",
                    //"553194768261"
                };

            string resultado = await api.ConexaoApiAsync(telefones, pdfUrl);
            lblResultado.Text = resultado.Replace("\n", "<br/>");
        }

    }
}