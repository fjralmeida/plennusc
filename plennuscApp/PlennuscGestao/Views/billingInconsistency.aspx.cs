using Plennusc.Core.Models.ModelsGestao.modelsBilling;
using Plennusc.Core.Service.ServiceGestao.serviceBilling;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class billingInconsistency : System.Web.UI.Page
    {
        private readonly ServiceBillingReconciliation _service = new ServiceBillingReconciliation();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CarregarOperadoras();
            }
            else
            {
                RecriarBotoesPaginacaoGridPreview();
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

        protected void btnPesquisar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlOperadora.SelectedValue))
            {
                ExibirMensagem("Selecione uma operadora antes de pesquisar.", erro: true);
                return;
            }

            string mesAnoReferencia = txtMesAnoReferencia.Text.Trim();

            if (string.IsNullOrEmpty(mesAnoReferencia))
            {
                ExibirMensagem("Informe o Mês/Ano Referência antes de pesquisar.", erro: true);
                return;
            }

            if (!System.Text.RegularExpressions.Regex.IsMatch(mesAnoReferencia, @"^(0[1-9]|1[0-2])\/\d{4}$"))
            {
                ExibirMensagem("Mês/Ano Referência inválido. Use o formato MM/AAAA.", erro: true);
                return;
            }


            int codigoGrupoContrato = Convert.ToInt32(ddlOperadora.SelectedValue);

            try
            {
                var itens = _service.ObterInconsistenciasFaturamento(mesAnoReferencia, codigoGrupoContrato);

                // 🔥 FILTRO POR NOME OU CPF
                string busca = txtBusca.Text.Trim();
                if (!string.IsNullOrEmpty(busca))
                {
                    itens = itens.Where(x =>
                        x.NomeDoAssociado != null && x.NomeDoAssociado.ToLower().Contains(busca.ToLower()) ||
                        x.NumeroCpf != null && x.NumeroCpf.Contains(busca)
                    ).ToList();
                }

                Session["BillingInconsistency_Itens"] = itens;

                gridPreview.DataSource = itens;
                gridPreview.DataBind();

                divResultado.Attributes.Remove("class");
                divResultado.Attributes.Add("class", "filters-card");

                if (itens.Count == 0)
                    ExibirMensagem("Nenhuma inconsistência encontrada.", erro: false);
                else
                    ExibirMensagem($"{itens.Count} registro(s) pendente(s) de conferência encontrado(s).", aviso: true);
            }
            catch (Exception ex)
            {
                ExibirMensagem("Erro ao pesquisar: " + ex.Message, erro: true);
            }
        }

        protected void gridPreview_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gridPreview.PageIndex = e.NewPageIndex;

            // 👇 CHAVE E TIPO CORRETOS
            var itens = Session["BillingInconsistency_Itens"] as List<ItemInconsistenciaFaturamento>;
            if (itens != null)
            {
                gridPreview.DataSource = itens;
                gridPreview.DataBind();
            }
        }

        protected void gridPreview_DataBound(object sender, EventArgs e)
        {
            // 👇 CHAVE E TIPO CORRETOS
            var itens = Session["BillingInconsistency_Itens"] as List<ItemInconsistenciaFaturamento>;
            ViewState["TotalRegistrosGridPreview"] = itens?.Count ?? 0;

            GridViewRow pagerRow = gridPreview.BottomPagerRow;
            if (pagerRow == null) return;

            Label lblInfo = (Label)pagerRow.FindControl("lblPagerInfo");
            if (lblInfo != null)
            {
                int totalGeral = ObterTotalRegistrosGridPreview();
                int paginaAtual = gridPreview.PageIndex;
                int tamanhoPagina = gridPreview.PageSize;
                int inicio = totalGeral == 0 ? 0 : (paginaAtual * tamanhoPagina) + 1;
                int fim = Math.Min(inicio + gridPreview.Rows.Count - 1, totalGeral);

                lblInfo.Text = $"<strong>{inicio} - {fim}</strong> de <strong>{totalGeral}</strong> itens";
            }

            RecriarBotoesPaginacaoGridPreview();
        }

        private void RecriarBotoesPaginacaoGridPreview()
        {
            GridViewRow pagerRow = gridPreview.BottomPagerRow;
            if (pagerRow == null) return;

            PlaceHolder ph = (PlaceHolder)pagerRow.FindControl("phPagerButtons");
            if (ph == null) return;

            ph.Controls.Clear();

            int totalGeral = ObterTotalRegistrosGridPreview();
            if (totalGeral == 0) return;

            int totalPaginas = (int)Math.Ceiling((double)totalGeral / gridPreview.PageSize);
            int currentPage = gridPreview.PageIndex;

            void AddBtn(string text, int pageIndex, bool disabled)
            {
                LinkButton btn = new LinkButton();
                btn.Text = text;
                btn.CssClass = "pager-button" + (pageIndex == currentPage ? " active" : "");
                btn.CommandName = "Page";
                btn.CommandArgument = pageIndex.ToString();
                btn.Click += (s, ev) =>
                {
                    gridPreview.PageIndex = pageIndex;
                    // 👇 CHAVE E TIPO CORRETOS
                    var itens = Session["BillingInconsistency_Itens"] as List<ItemInconsistenciaFaturamento>;
                    if (itens != null)
                    {
                        gridPreview.DataSource = itens;
                        gridPreview.DataBind();
                    }
                };
                btn.Enabled = !disabled;
                if (disabled) btn.CssClass += " disabled";
                ph.Controls.Add(btn);
            }

            AddBtn("«", 0, currentPage == 0);
            AddBtn("‹", currentPage - 1, currentPage == 0);

            int start = Math.Max(0, currentPage - 3);
            int end = Math.Min(totalPaginas - 1, currentPage + 3);
            for (int i = start; i <= end; i++)
                AddBtn((i + 1).ToString(), i, false);

            AddBtn("›", currentPage + 1, currentPage >= totalPaginas - 1);
            AddBtn("»", totalPaginas - 1, currentPage >= totalPaginas - 1);
        }

        private int ObterTotalRegistrosGridPreview()
        {
            if (ViewState["TotalRegistrosGridPreview"] != null)
                return (int)ViewState["TotalRegistrosGridPreview"];
            return 0;
        }

        protected string TraduzirStatus(string status)
        {
            return string.IsNullOrEmpty(status) ? "Pendente" : status;
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            // Recupera a lista completa da sessão (já filtrada)
            var itens = Session["BillingInconsistency_Itens"] as List<ItemInconsistenciaFaturamento>;

            if (itens == null || itens.Count == 0)
            {
                ExibirMensagem("Não há dados para exportar.", erro: true);
                return;
            }

            // Gera o Excel e faz o download
            ExportarParaExcel(itens);
        }

        private void ExportarParaExcel(List<ItemInconsistenciaFaturamento> itens)
        {
            // Cria um DataTable com as colunas desejadas
            DataTable dt = new DataTable();
            dt.Columns.Add("CPF", typeof(string));
            dt.Columns.Add("Nome", typeof(string));
            dt.Columns.Add("Mês/Ano", typeof(string));
            dt.Columns.Add("Valor Convênio", typeof(decimal));
            dt.Columns.Add("Valor Adicional", typeof(decimal));
            dt.Columns.Add("Valor Fatura", typeof(decimal));
            dt.Columns.Add("Data Admissão", typeof(string));
            dt.Columns.Add("Data Exclusão", typeof(string));

            foreach (var item in itens)
            {
                dt.Rows.Add(
                    item.NumeroCpf,
                    item.NomeDoAssociado,
                    item.MesAnoReferencia,
                    item.ValorConvenio,
                    item.ValorAdicional,
                    item.ValorFatura,
                    item.DataAdmissao?.ToString("dd/MM/yyyy"),
                    item.DataExclusao?.ToString("dd/MM/yyyy")
                );
            }

            // Configura a resposta HTTP para download do Excel
            Response.Clear();
            Response.ContentType = "application/vnd.ms-excel";
            Response.AddHeader("Content-Disposition", $"attachment; filename=Inconsistencias_{DateTime.Now:yyyyMMdd_HHmmss}.xls");

            // Gera o HTML da tabela (Excel reconhece)
            using (StringWriter sw = new StringWriter())
            using (HtmlTextWriter htw = new HtmlTextWriter(sw))
            {
                // Cria a tabela HTML
                htw.Write("<table border='1' cellpadding='3' cellspacing='0'>");

                // Cabeçalho
                htw.Write("<tr>");
                foreach (DataColumn col in dt.Columns)
                {
                    htw.Write($"<th style='font-weight:bold;background:#f0f0f0;'>{col.ColumnName}</th>");
                }
                htw.Write("</tr>");

                // Dados
                foreach (DataRow row in dt.Rows)
                {
                    htw.Write("<tr>");
                    foreach (DataColumn col in dt.Columns)
                    {
                        htw.Write($"<td>{row[col]}</td>");
                    }
                    htw.Write("</tr>");
                }
                htw.Write("</table>");

                Response.Write(sw.ToString());
                Response.End();
            }
        }

        private void ExibirMensagem(string mensagem, bool erro = false, bool aviso = false)
        {
            lblMensagemPesquisa.Text = mensagem;
            string classe = "msg-importacao";
            if (erro) classe += " erro";
            else if (aviso) classe += " aviso";
            else classe += " sucesso";
            lblMensagemPesquisa.CssClass = classe;
        }
    }
}