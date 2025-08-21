using ExcelDataReader;
using Plennusc.Core.Mappers.MappersGestao;
using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.price;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class updatePriceTable : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var url = ResolveUrl("~/public/uploadgestao/docs/ModeloDeAtualizaçãoTabelaPreços%20-%20PS1032.xlsx");
                lnkModeloXls.NavigateUrl = url;
                lnkModeloXls.Target = ""; // garante que não abre nova aba
                lnkModeloXls.Attributes["download"] = "Modelo_Tabela_Preco_PS1032 - Atualização.xlsx"; // força download
            }
        }

        private DataTable TabelaXls
        {
            get { return (DataTable)Session["TabelaXls"]; }
            set { Session["TabelaXls"] = value; }
        }
        protected void gridXsl_PreRender(object sender, EventArgs e)
        {
            if (gridXsl.HeaderRow != null)
            {
                gridXsl.UseAccessibleHeader = true;
                gridXsl.HeaderRow.TableSection = TableRowSection.TableHeader;
            }
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

                int atualizados = 0;
                int linhaPlanilha = 0;

                foreach (DataRow row in TabelaXls.Rows)
                {
                    linhaPlanilha++;
                    var item = InsertionIntoTheMapperPriceTable.FromDataRow(row);

                    var afetadas = tablePrice.updatePriceTable(item);

                    if (afetadas == 0)
                    {
                        // guarda info para o modal
                        Session["UpdateErroItem"] = item;
                        Session["UpdateErroLinha"] = linhaPlanilha;

                        // preenche o conteúdo do modal nesta mesma resposta
                        PreencherModalErro();

                        // mostra o botão "Ver linha com erro"
                        btnMostrarErro.Visible = true;

                        lblResultado.Text = $"<span style='color:red;'>Linha {linhaPlanilha}: registro não encontrado com as chaves informadas. Clique em <b>Ver linha com erro</b> para detalhes.</span>";
                        return; // para no primeiro erro
                    }

                    atualizados++;
                }

                lblResultado.Text = $"<span style='color:green;'>{atualizados} registro(s) atualizado(s) na PS1032.</span>";
                btnMostrarErro.Visible = false; // garante oculto se não houve erro
            }
            catch (Exception ex)
            {
                lblResultado.Text = "<span style='color:red;'>Erro ao enviar dados: " + ex.Message + "</span>";
            }
        }

        private void PreencherModalErro()
        {
            var item = Session["UpdateErroItem"] as DataInsertionPriceTableMessage;
            var linhaObj = Session["UpdateErroLinha"];
            int linha = (linhaObj is int i) ? i : 0;

            if (item == null) return;

            litErroLinha.Text = $@"
            <div class='small'>
                <div><b>Linha:</b> {linha} <b>Com Erro:</b></div>
                <hr/>
                <div><b>NUMERO_REGISTRO:</b> {item.NUMERO_REGISTRO}</div>
                <div><b>CODIGO_PLANO:</b> {item.CODIGO_PLANO}</div>
                <div><b>CODIGO_TABELA_PRECO:</b> {item.CODIGO_TABELA_PRECO}</div>
                <div><b>CODIGO_GRUPO_CONTRATO:</b> {item.CODIGO_GRUPO_CONTRATO}</div>
                <hr/>
                <div><b>VALOR_PLANO:</b> {item.VALOR_PLANO:N2}</div>
                <div><b>VALOR_NET:</b> {item.VALOR_NET:N2}</div>
                <div><b>TIPO_RELACAO_DEPENDENCIA:</b> {item.TIPO_RELACAO_DEPENDENCIA}</div>
                <div><b>NOME_TABELA:</b> {item.NOME_TABELA}</div>
                div><b>TIPO_CONTRATO_ESTIPULADO:</b> {item.TIPO_CONTRATO_ESTIPULADO}</div>
            </div>";
        }
    }
}