using Plennusc.Core.Service.ServiceGestao.CIDsService;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class ICDImport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnImportar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            if (!fileUploadExcel.HasFile)
            {
                ExibirErro("Selecione um arquivo Excel.");
                return;
            }

            if (!DateTime.TryParse(txtVigencia.Text, out var vigencia))
            {
                ExibirErro("Vigência inválida.");
                return;
            }

            var connSettings = ConfigurationManager.ConnectionStrings["Alianca"];
            if (connSettings == null)
            {
                ExibirErro("Connection string 'Alianca' não encontrada no web.config.");
                return;
            }

            var service = new serviceCIDs(connSettings.ConnectionString);

            var resultados = service.ProcessarImportacao(fileUploadExcel.PostedFile.InputStream, vigencia);

            var importados = resultados.Where(r => r.Sucesso).ToList();
            var naoImportados = resultados.Where(r => !r.Sucesso).ToList();

            gridImportados.DataSource = importados;
            gridImportados.DataBind();

            gridNaoImportados.DataSource = naoImportados;
            gridNaoImportados.DataBind();

            pnlResultado.Visible = true;
        }

        private void ExibirErro(string mensagem)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "erro",
                $"alert('{mensagem}');", true);
        }
    }
}