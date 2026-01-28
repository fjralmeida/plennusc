using Plennusc.Core.Service.ServiceGestao.sendMessage;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class viewShippingHistory : System.Web.UI.Page
    {
        protected historicalShippingService _service;

        protected void Page_Load(object sender, EventArgs e)
        {
            _service = new historicalShippingService();

            if (!IsPostBack)
            {
                ConfigurarDatasPadrao();
                CarregarGrid();
            }
        }

        private void ConfigurarDatasPadrao()
        {
            txtDataInicio.Text = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
            txtDataFim.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }

        private void CarregarGrid()
        {
            try
            {
                // Obtém parâmetros
                DateTime? dataInicio = ObterData(txtDataInicio.Text);
                DateTime? dataFim = ObterData(txtDataFim.Text, true);
                string codigo = txtCodigoAssociado.Text.Trim();
                string telefone = txtTelefone.Text.Trim();
                string status = ddlStatus.SelectedValue;
                string template = ddlTemplate.SelectedValue;

                // Busca dados
                var dt = _service.BuscarHistorico(
                    dataInicio, dataFim, codigo, null, telefone,
                    status, template, 1, 30);

                GridHistorico.DataSource = dt;
                GridHistorico.DataBind();

                // Atualiza contador
                int totalRegistros = _service.ContarTotalRegistros(
                    dataInicio, dataFim, codigo, null, telefone, status, template);

                litTotalRegistros.Text = totalRegistros.ToString("N0");
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao carregar dados: {ex.Message}", "error");
            }
        }

        private DateTime? ObterData(string textoData, bool fimDoDia = false)
        {
            if (DateTime.TryParse(textoData, out DateTime data))
            {
                if (fimDoDia)
                    return data.Date.AddDays(1).AddSeconds(-1);
                return data.Date;
            }
            return null;
        }

        protected void btnFiltrar_Click(object sender, EventArgs e)
        {
            CarregarGrid();
        }

        protected void btnLimpar_Click(object sender, EventArgs e)
        {
            ConfigurarDatasPadrao();
            txtCodigoAssociado.Text = "";
            txtTelefone.Text = "";
            ddlStatus.SelectedIndex = 0;
            ddlTemplate.SelectedIndex = 0;

            CarregarGrid();
        }

        protected void GridHistorico_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridHistorico.PageIndex = e.NewPageIndex;
            CarregarGrid();
        }

        // Métodos para formatar badges
        public string GetBadgeTemplate(string template)
        {
            // Retorna classe mais sutil
            switch (template)
            {
                case "Suspensao": return "badge badge-info";
                case "Definitivo": return "badge badge-primary";
                case "aVencer": return "badge badge-secondary";
                default: return "badge badge-secondary";
            }
        }

        public string GetBadgeStatus(string status)
        {
            // Badges mais sutis para status
            switch (status)
            {
                case "ENVIADO": return "badge badge-success";
                case "FALHOU": return "badge badge-danger";
                case "SEM_DOCUMENTOS": return "badge badge-warning";
                default: return "badge badge-secondary";
            }
        }

        public string GetDescricaoTemplate(string template)
        {
            switch (template)
            {
                case "Suspensao": return "Boleto";
                case "Definitivo": return "Nota Fiscal";
                case "aVencer": return "Boleto + Nota";
                default: return template;
            }
        }

        // MÉTODO PARA EXPORTAR COMO EXCEL REAL (HTML)
        protected void btnExportar_Click(object sender, EventArgs e)
        {
            try
            {
                // Busca os dados
                DateTime? dataInicio = ObterData(txtDataInicio.Text);
                DateTime? dataFim = ObterData(txtDataFim.Text, true);
                string codigo = txtCodigoAssociado.Text.Trim();
                string telefone = txtTelefone.Text.Trim();
                string status = ddlStatus.SelectedValue == "TODOS" ? null : ddlStatus.SelectedValue;
                string template = ddlTemplate.SelectedValue == "TODOS" ? null : ddlTemplate.SelectedValue;

                DataTable dt = _service.BuscarHistorico(
                    dataInicio, dataFim, codigo, null, telefone,
                    status, template, 1, int.MaxValue);

                // Cria um GridView temporário
                GridView gv = new GridView();
                gv.DataSource = dt;
                gv.DataBind();

                // Remove as colunas que não queremos
                gv.Columns.Clear();

                // Adiciona apenas as colunas que aparecem no Grid
                gv.AutoGenerateColumns = false;

                // Coluna Data/Hora
                BoundField colData = new BoundField();
                colData.HeaderText = "Data/Hora";
                colData.DataField = "DataEnvio";
                colData.DataFormatString = "{0:dd/MM/yyyy HH:mm}";
                gv.Columns.Add(colData);

                // Coluna Código
                BoundField colCodigo = new BoundField();
                colCodigo.HeaderText = "Código";
                colCodigo.DataField = "CodigoAssociado";
                gv.Columns.Add(colCodigo);

                // NOVA COLUNA: Nome do Associado
                BoundField colNome = new BoundField();
                colNome.HeaderText = "Associado";
                colNome.DataField = "NomeAssociado";
                gv.Columns.Add(colNome);

                // Coluna Telefone
                BoundField colTelefone = new BoundField();
                colTelefone.HeaderText = "Telefone";
                colTelefone.DataField = "NumTelefoneDestino";
                gv.Columns.Add(colTelefone);

                // Coluna Template (com formatação)
                TemplateField colTemplate = new TemplateField();
                colTemplate.HeaderText = "Template";
                colTemplate.ItemTemplate = new GridViewTemplate(DataControlRowType.DataRow, "Mensagem", GetDescricaoTemplate);
                gv.Columns.Add(colTemplate);

                // Coluna Status
                BoundField colStatus = new BoundField();
                colStatus.HeaderText = "Status";
                colStatus.DataField = "StatusEnvio";
                gv.Columns.Add(colStatus);

                gv.DataBind();

                // Exporta
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition",
                    $"attachment;filename=historico_envios_{DateTime.Now:yyyyMMdd_HHmmss}.xls");
                Response.Charset = "";
                Response.ContentType = "application/vnd.ms-excel";

                StringWriter sw = new StringWriter();
                HtmlTextWriter hw = new HtmlTextWriter(sw);

                gv.RenderControl(hw);

                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            catch (Exception ex)
            {
                MostrarMensagem($"Erro ao exportar: {ex.Message}", "error");
            }
        }

        // Classe auxiliar para o TemplateField
        public class GridViewTemplate : ITemplate
        {
            private DataControlRowType _templateType;
            private string _columnName;
            private Func<string, string> _formatFunction;

            public GridViewTemplate(DataControlRowType type, string colname, Func<string, string> formatFunc)
            {
                _templateType = type;
                _columnName = colname;
                _formatFunction = formatFunc;
            }

            public void InstantiateIn(Control container)
            {
                switch (_templateType)
                {
                    case DataControlRowType.DataRow:
                        Label lbl = new Label();
                        lbl.DataBinding += new EventHandler(this.lbl_DataBinding);
                        container.Controls.Add(lbl);
                        break;
                }
            }

            private void lbl_DataBinding(object sender, EventArgs e)
            {
                Label lbl = (Label)sender;
                GridViewRow row = (GridViewRow)lbl.NamingContainer;
                string data = DataBinder.Eval(row.DataItem, _columnName).ToString();
                lbl.Text = _formatFunction(data);
            }
        }

        private void MostrarMensagem(string mensagem, string tipo = "success")
        {
            string script = $@"
                Swal.fire({{
                    toast: true,
                    position: 'top-end',
                    icon: '{tipo}',
                    text: '{mensagem.Replace("'", "\\'")}',
                    showConfirmButton: false,
                    timer: 4000,
                    width: 350
                }});";

            ScriptManager.RegisterStartupScript(this, GetType(), "Mensagem", script, true);
        }
    }
}