using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class priceTable : System.Web.UI.Page
    {
        private DataTable TabelaXls
        {
            get { return (DataTable)Session["TabelaXls"]; }
            set { Session["TabelaXls"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnLerXls_Click(object sender, EventArgs e)
        {
            lblResultado.Text = ""; // Limpa mensagem
            if (!fileUploadXls.HasFile)
            {
                lblResultado.Text = "<span style='color:red;'>Selecione um arquivo XLS ou XLSX!</span>";
                return;
            }

            // Valida extensão
            string ext = Path.GetExtension(fileUploadXls.FileName).ToLower();
            if (ext != ".xls" && ext != ".xlsx")
            {
                lblResultado.Text = "<span style='color:red;'>Arquivo inválido! Apenas XLS ou XLSX.</span>";
                return;
            }

            // Lê o arquivo
            try
            {
                using (var stream = fileUploadXls.FileContent)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true // primeira linha vira header
                        }
                    });

                    // Assume a primeira planilha
                    DataTable dt = result.Tables[0];
                    if (dt.Rows.Count == 0)
                    {
                        lblResultado.Text = "<span style='color:red;'>Arquivo vazio!</span>";
                        return;
                    }

                    TabelaXls = dt; // Salva na Session para usar depois
                    gridXsl.DataSource = dt;
                    gridXsl.DataBind();

                    btnEnviar.Enabled = true;
                    lblResultado.Text = "<span style='color:green;'>Arquivo carregado com sucesso!</span>";
                }
            }
            catch (Exception ex)
            {
                lblResultado.Text = "<span style='color:red;'>Erro ao ler arquivo: " + ex.Message + "</span>";
            }
        }
        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            if(TabelaXls == null)
            {
                lblResultado.Text = "<span style='color:red;'>Nenhum arquivo carregado!</span>";
                return;
            }

            int linhas = 0;
            foreach (DataRow row in TabelaXls.Rows)
            {
                linhas++;
            }

            lblResultado.Text = $"<span style='color:green;'>Processado {linhas} linhas!</span>";
            btnEnviar.Enabled = false;
            TabelaXls = null; 
            gridXsl.DataSource = null;
            gridXsl.DataBind();
        }
    }
}