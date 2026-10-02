using Plennusc.Core.Service.ServiceGestao.CIDsService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class setCemgImport : System.Web.UI.Page
    {
        private readonly setCemgImportService _service = new setCemgImportService();
        private readonly setCemgExportService _export = new setCemgExportService();

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btnImportar_Click(object sender, EventArgs e)
        {
            pnlErros.Visible = false;
            pnlLinhas.Visible = false;

            if (!fuPlanilha.HasFile)
            {
                lblMensagem.Text = "Selecione a planilha de carga.";
                return;
            }

            var extensao = Path.GetExtension(fuPlanilha.FileName).ToLowerInvariant();
            if (extensao != ".xlsm" && extensao != ".xlsx")
            {
                lblMensagem.Text = "Formato inválido. Envie um arquivo .xlsm ou .xlsx.";
                return;
            }

            try
            {
                var resultado = _service.Ler(fuPlanilha.PostedFile.InputStream);

                // Plano digitado (vazio = "PREENCHER MANUALMENTE" no layout)
                var codPlano = (txtCodPlano.Text ?? string.Empty).Trim();
                var codAns = (txtCodAnsPlano.Text ?? string.Empty).Trim();

                setCemgPlano plano = null;
                if (codPlano.Length > 0 || codAns.Length > 0)
                {
                    plano = new setCemgPlano
                    {
                        CodPlano = codPlano.Length > 0 ? codPlano : setCemgExportService.PreencherManualmente,
                        CodAnsPlano = codAns.Length > 0 ? codAns : setCemgExportService.PreencherManualmente
                    };
                }

                var saida = _export.Transformar(resultado.Linhas, resultado.CnpjEmpresa, plano);
                Session["CemgLayout"] = saida.Linhas;
                foreach (var a in saida.Avisos) resultado.Erros.Add(a);

                if (plano != null && codPlano.Length == 0)
                    resultado.Erros.Add(new Plennusc.Core.Models.ModelsGestao.modelsCIDs.setCemgImportErro { LinhaExcel = 0, Campo = "COD_PLANO", Mensagem = "Código do plano não informado. " + setCemgExportService.PreencherManualmente + "." });
                if (plano != null && codAns.Length == 0)
                    resultado.Erros.Add(new Plennusc.Core.Models.ModelsGestao.modelsCIDs.setCemgImportErro { LinhaExcel = 0, Campo = "COD_ANS_PLANO", Mensagem = "Código ANS do plano não informado. " + setCemgExportService.PreencherManualmente + "." });

                lblMensagem.Text = resultado.Linhas.Count + " linha(s) lida(s), "
                                 + resultado.Erros.Count + " inconsistência(s)/aviso(s). "
                                 + "CNPJ da empresa: " + (string.IsNullOrEmpty(resultado.CnpjEmpresa) ? "não encontrado" : resultado.CnpjEmpresa) + ".";

                gvLinhas.DataSource = resultado.Linhas;
                gvLinhas.DataBind();
                pnlLinhas.Visible = resultado.Linhas.Count > 0;

                gvErros.DataSource = resultado.Erros;
                gvErros.DataBind();
                pnlErros.Visible = resultado.Erros.Count > 0;
            }
            catch (Exception ex)
            {
                lblMensagem.Text = "Não foi possível ler a planilha: " + ex.Message;
            }
        }

        protected void btnExportar_Click(object sender, EventArgs e)
        {
            var linhas = Session["CemgLayout"] as List<string[]>;
            if (linhas == null || linhas.Count == 0)
            {
                lblMensagem.Text = "Importe a planilha antes de exportar.";
                return;
            }

            var bytes = _export.GerarXlsx(linhas);

            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("Content-Disposition", "attachment; filename=layout_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".xlsx");
            Response.BinaryWrite(bytes);
            Response.End();
        }
    }
}
