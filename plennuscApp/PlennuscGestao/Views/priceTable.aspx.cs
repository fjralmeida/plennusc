using ExcelDataReader;
using Plennusc.Core.Mappers.MappersGestao;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.import;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.price;
using System;
using System.Collections.Generic;
using System.Data;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class priceTable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        private DataTable TabelaXls
        {
            get { return (DataTable)Session["TabelaXls"]; }
            set { Session["TabelaXls"] = value; }
        }

        protected void btnLerXls_Click(object sender, EventArgs e)
        {
            lblResultado.Text = "";
            if (!fileUploadXls.HasFile)
            {
                lblResultado.Text = "<span style='color:red;'>Selecione um arquivo XLS ou XLSX!</span>";
                return;
            }

            string ext = Path.GetExtension(fileUploadXls.FileName).ToLower();
            if (ext != ".xls" && ext != ".xlsx")
            {
                lblResultado.Text = "<span style='color:red;'>Arquivo inválido! Apenas XLS ou XLSX.</span>";
                return;
            }

            try
            {
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

                using (var stream = fileUploadXls.FileContent)
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = _ => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true, // primeira linha como cabeçalho
                            FilterColumn = (rowReader, colIndex) => true // não ignora colunas vazias
                        }
                    });

                    DataTable dt = result.Tables[0];

                    // 🔹 Ajuste para preservar exatamente o nome vindo do Excel (com trim)
                    foreach (DataColumn col in dt.Columns)
                    {
                        col.ColumnName = col.ColumnName.Trim();
                    }

                    // 🔹 Debug opcional para conferir nomes de colunas
                    // lblResultado.Text = "Colunas encontradas: " + string.Join(", ", dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName));

                    TabelaXls = dt;

                    gridXsl.AutoGenerateColumns = false; // vamos usar os BoundFields definidos no ASPX
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
            var tablePrice = new TablePrice();

            try
            {
                if (TabelaXls == null || TabelaXls.Rows.Count == 0)
                {
                    lblResultado.Text = "<span style='color:red;'>Carregue o XLS antes de enviar.</span>";
                    return;
                }

                int inseridos = 0;

                foreach (DataRow row in TabelaXls.Rows)
                {
                    var item = TablePriceMapper.FromDataRow(row);
                    tablePrice.InserirPriceTable(item);
                    inseridos++;
                }

                lblResultado.Text = $"<span style='color:green;'>{inseridos} registro(s) inserido(s) na PS1032.</span>";

            }
            catch (Exception ex)
            {
                lblResultado.Text = "<span style='color:red;'>Erro ao enviar dados: " + ex.Message + "</span>";
            }
        }
    }
}