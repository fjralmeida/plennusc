using ClosedXML.Excel;
using Plennusc.Core.Models.ModelsGestao.modelsCIDs;
using Plennusc.Core.Service.ServiceGestao.CIDsService;
using Plennusc.Core.Service.ServiceGestao.serviceBilling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Globalization;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class activityReport : System.Web.UI.Page
    {
        private readonly activityReportService _service = new activityReportService();

        // Usado só para manter os dados entre postbacks de paginação do GridView
        private List<ActivityReportModel> DadosRelatorio
        {
            get => Session["ActivityReportResult"] as List<ActivityReportModel>;
            set => Session["ActivityReportResult"] = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarOperadoras();
                DadosRelatorio = null; // limpa resultado antigo ao entrar na página
            }
        }

        private void CarregarOperadoras()
        {
            var operadoras = _service.ObterOperadoras();
            ddlOperadora.DataSource = operadoras;
            ddlOperadora.DataTextField = "NomeOperadora";
            ddlOperadora.DataValueField = "CodigoGrupoContrato";
            ddlOperadora.DataBind();
            ddlOperadora.Items.Insert(0, new ListItem("Selecione...", ""));
        }

        protected void btnRelatorioMovimentacao_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlOperadora.SelectedValue))
            {
                ExibirErro("Selecione uma operadora.");
                return;
            }

            if (!DateTime.TryParse(txtVigencia.Text, out var vigencia))
            {
                ExibirErro("Vigência inválida.");
                return;
            }

            int codigoGrupoContrato = int.Parse(ddlOperadora.SelectedValue);

            List<ActivityReportModel> dados = _service.GerarRelatorio(codigoGrupoContrato, vigencia);

            if (dados == null || dados.Count == 0)
            {
                divResultado.Visible = false;
                ExibirErro("Nenhum registro encontrado para essa operadora/vigência.");
                return;
            }

            foreach (var item in dados)
            {
                // Se o Parentesco for titular, vazio
                if (string.Equals(item.Parentesco?.Trim(), "TITULAR", StringComparison.OrdinalIgnoreCase))
                    item.Parentesco = string.Empty;

                // Endereço: o que estiver depois do traço vai para Complemento
                if (!string.IsNullOrWhiteSpace(item.Endereco))
                {
                    var enderecoOriginal = item.Endereco;
                    var indiceTraco = enderecoOriginal.IndexOf('-');

                    if (indiceTraco >= 0)
                    {
                        item.Endereco = enderecoOriginal.Substring(0, indiceTraco).Trim();
                        item.Complemento = enderecoOriginal.Substring(indiceTraco + 1).Trim();
                    }
                    else
                    {
                        item.Endereco = enderecoOriginal.Trim();
                        item.Complemento = string.Empty; // sem traço, complemento fica vazio
                    }
                }
                else
                {
                    item.Complemento = string.Empty;
                }
            }

            DadosRelatorio = dados;
            gvRelatorio.PageIndex = 0;
            VincularGrid();
        }

        protected void gvRelatorio_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvRelatorio.PageIndex = e.NewPageIndex;
            VincularGrid();
        }

        private void VincularGrid()
        {
            var dados = DadosRelatorio;
            if (dados == null || dados.Count == 0)
            {
                divResultado.Visible = false;
                return;
            }

            lblMensagem.Visible = false;
            divResultado.Visible = true;
            litTotalRegistros.Text = dados.Count.ToString();
            gvRelatorio.DataSource = dados;
            gvRelatorio.DataBind();
        }

        protected void btnExportarExcel_Click(object sender, EventArgs e)
        {
            var dados = DadosRelatorio;
            if (dados == null || dados.Count == 0)
            {
                ExibirErro("Não há dados para exportar.");
                return;
            }

            var sb = new StringBuilder();

            sb.Append("<html xmlns:x='urn:schemas-microsoft-com:office:excel'>");
            sb.Append("<head><meta charset='UTF-8'/>");
            sb.Append("<!--[if gte mso 9]><xml><x:ExcelWorkbook><x:ExcelWorksheets><x:ExcelWorksheet>");
            sb.Append("<x:Name>Relatório</x:Name><x:WorksheetOptions><x:DisplayGridlines/></x:WorksheetOptions>");
            sb.Append("</x:ExcelWorksheet></x:ExcelWorksheets></x:ExcelWorkbook></xml><![endif]-->");
            sb.Append("<style>td { mso-number-format:'\\@'; } th { background:#DCDCDC; font-weight:bold; }</style>");
            sb.Append("</head><body><table border='1'>");

            // Cabeçalho
            string[] cabecalhos = {
                "Modalidade", "Plano", "Nome Tabela Preço", "Entidade", "Titular", "Nome", "Tipo",
                "Parentesco", "Estado Civil", "Sexo", "Data Nasc.", "CPF", "CNS",
                "Telefone", "Endereço", "Complemento", "Bairro", "CEP", "Cidade",
                "Estado", "Dt. Vigência", "Email", "Filiação 1", "Filiação 2",
                "Nomenclatura da Carência", "CID"
            };

            sb.Append("<tr>");
            foreach (var c in cabecalhos)
                sb.Append("<th>").Append(HttpUtility.HtmlEncode(c)).Append("</th>");
            sb.Append("</tr>");

            // Linhas
            foreach (var item in dados)
            {
                sb.Append("<tr>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Modalidade)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Plano)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.NomeTabelaPreco)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Entidade)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Titular)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Nome)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Tipo)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Parentesco)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.EstadoCivil)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Sexo)).Append("</td>");
                sb.Append("<td>").Append(item.DataNascimento.HasValue ? item.DataNascimento.Value.ToString("dd/MM/yyyy") : "").Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Cpf)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Cns)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Telefone)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Endereco)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Complemento)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Bairro)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Cep)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Cidade)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Estado)).Append("</td>");
                sb.Append("<td>").Append(item.DtVigencia.HasValue ? item.DtVigencia.Value.ToString("dd/MM/yyyy") : "").Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Email)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Filiacao1)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Filiacao2)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.NomenclaturaCarencia)).Append("</td>");
                sb.Append("<td>").Append(HttpUtility.HtmlEncode(item.Cid)).Append("</td>");
                sb.Append("</tr>");
            }

            sb.Append("</table></body></html>");

            Response.Clear();
            Response.Charset = "UTF-8";
            Response.ContentEncoding = Encoding.UTF8;
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("content-disposition",
                $"attachment;filename=RelatorioAtividade_{DateTime.Now:yyyyMMdd_HHmm}.xls");

            Response.Write(sb.ToString());
            Response.End();
        }

        private void ExibirErro(string mensagem)
        {
            lblMensagem.Text = mensagem;
            lblMensagem.Visible = true;
        }
    }
}