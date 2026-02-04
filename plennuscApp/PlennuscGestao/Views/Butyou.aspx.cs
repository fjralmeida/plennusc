using appWhatsapp.PlennuscGestao.Services;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Table = DocumentFormat.OpenXml.Wordprocessing.Table;
using TableCell = DocumentFormat.OpenXml.Wordprocessing.TableCell;
using TableRow = DocumentFormat.OpenXml.Wordprocessing.TableRow;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class Butyou : System.Web.UI.Page
    {
        private DocxService _docxService = new DocxService();
        private ExcelService _excelService = new ExcelService();

        protected void Page_Load(object sender, EventArgs e)
        {
            //OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }

        protected void btnAnalisar_Click(object sender, EventArgs e)
        {
            //try
            //{
            //    string templatePath = Server.MapPath("~/public/uploadgestao/docs/youBut/MAIS VOCE - PE - DOCX.docx");
            //    //var list = _docxService.AnalyzeTemplate(templatePath);

            //    pnlCampos.Visible = true;
            //    litCampos.Text = $"<p>Total itens: {list.Count}</p><ul>";
            //    foreach (var item in list.Take(50))
            //        litCampos.Text += $"<li>{Server.HtmlEncode(item)}</li>";
            //    litCampos.Text += "</ul>";
            //}
            //catch (Exception ex)
            //{
            //    lblErro.Text = ex.Message;
            //    lblErro.Visible = true;
            //}
        }

        protected void btnTestarPreenchimento_Click(object sender, EventArgs e)
        {
            try
            {
                string templatePath = Server.MapPath("~/public/uploadgestao/docs/youBut/MAIS VOCE - PE - DOCX.docx");
                string tempDir = Server.MapPath("~/temp/");
                Directory.CreateDirectory(tempDir);
                string outputPath = Path.Combine(tempDir, $"PROPOSTA_PREENCHIDA_{DateTime.Now:yyyyMMddHHmmss}.docx");

                // DADOS COMPLETOS DO TITULAR
                var dadosTitular = new Dictionary<string, string>
                {               
                    // Vigência
                    { "INICIO_VIGENCIA", DateTime.Now.ToString("dd/MM/yyyy") },
 
                    // Opções de Vencimento - vamos marcar todas as 3 para começar
                    { "VENCIMENTO_DIA_01", "true" },
                    { "VENCIMENTO_DIA_10", "true" },
                    { "VENCIMENTO_DIA_20", "true" },

                    // Identificação
                    { "NOME_COMPLETO", "MARCOS ANTONIO SILVEIRA" },
                    { "CPF_TITULAR", "123.456.789-00" },
                    { "DATA_NASCIMENTO", "15/03/1985" },
                    { "RG", "12.345.678-9" },

                    // FILIAÇÃO E IDADE
                    { "FILIACAO", "JOÃO SILVEIRA E MARIA SILVEIRA" },
                    { "IDADE", "39" },

                    // SEXO
                    { "SEXO", "M" },

                    // ESTADO CIVIL
                    { "ESTADO_CIVIL", "CASADO" },

                    // Novos campos
                    { "ORGAO_EXPEDIDOR", "SSP/PE" },
                    { "CARTAO_SUS", "123 4567 8901 2345" },
                    { "NUMERO_DECLARACAO_NASCIDO_VIVO", "123456789" },
                    { "RESPONSAVEL_FINANCEIRO", "MARIA SILVEIRA" },
                    { "CPF_RESPONSAVEL_FINANCEIRO", "987.654.321-00" },

                    // Endereço
                    { "ENDERECO", "AVENIDA BOA VIAGEM" },
                    { "NUMERO", "1001" },
                    { "COMPLEMENTO", "APTO 502" },
                    { "CEP", "51020-100" },
                    { "BAIRRO", "BOA VIAGEM" },
                    { "CIDADE", "RECIFE" },
                    { "UF", "PE" },

                    // Contato
                    { "EMAIL", "marcos.silveira@email.com" },
                    { "TELEFONE_CELULAR", "(81) 99999-8888" },
                    { "TELEFONE_FIXO", "(81) 3333-4444" },

                    // NOVO: Tipo de coparticipação (PARCIAL, TOTAL ou SEM COPARTICIPAÇÃO)
                    { "PLANO_COPARTICIPACAO", "PARCIAL" }
                };

                // 2. DADOS DOS DEPENDENTES
                var dependentes = new List<Dictionary<string, string>>
                {
                    new Dictionary<string, string>
                    {
                        { "NOME_COMPLETO", "MARIA SILVEIRA" },
                        { "DATA_NASCIMENTO", "20/05/1990" },
                        { "SEXO", "F" },
                        { "ESTADO_CIVIL", "CASADO" },
                        { "CPF", "111.222.333-44" },
                        { "FILIACAO", "JOÃO SILVEIRA E MARIA SILVEIRA" },
                        { "RG", "98.765.432-1" },
                        { "PARENTESCO", "CÔNJUGE" },
                        { "ORGAO_EXPEDIDOR", "SSP/SP" },
                        { "CARTAO_SUS", "123 4567 8901 2345" },
                        { "NUMERO_DECLARACAO_NASCIDO_VIVO", "2023-123456" }
                    },
                    new Dictionary<string, string>
                    {
                        { "NOME_COMPLETO", "JOÃO SILVEIRA" },
                        { "DATA_NASCIMENTO", "15/08/2010" },
                        { "SEXO", "M" },
                        { "ESTADO_CIVIL", "SOLTEIRO" },
                        { "CPF", "555.666.777-88" },
                        { "FILIACAO", "JOÃO SILVEIRA E MARIA SILVEIRA" },
                        { "RG", "11.223.344-5" },
                        { "PARENTESCO", "FILHO" },
                        { "ORGAO_EXPEDIDOR", "SSP/SP" },
                        { "CARTAO_SUS", "987 6543 2109 8765" },
                        { "NUMERO_DECLARACAO_NASCIDO_VIVO", "2010-987654" }
                    }
                };

                // 1. PRIMEIRO: Processar o titular
                int appliedTitular = _docxService.FillTitularBasico(
                    templatePath,
                    outputPath,
                    dadosTitular);

                // AGUARDAR um pouco
                System.Threading.Thread.Sleep(500);
                GC.Collect();
                GC.WaitForPendingFinalizers();

                // 2. DEPOIS: Processar os dependentes
                int appliedDependentes = 0;

                try
                {
                    appliedDependentes = _docxService.FillDependentes(
                        outputPath,
                        outputPath,
                        dependentes);
                }
                catch (IOException ioEx)
                {
                    lblErro.Text = $"Erro ao acessar arquivo: {ioEx.Message}. Tentando alternativa...";
                    System.Threading.Thread.Sleep(1000);
                    appliedDependentes = _docxService.FillDependentes(outputPath, outputPath, dependentes);
                }

                // 3. AGORA: Processar o plano de coparticipação
                int appliedPlano = 0;

                // TESTE EXPLÍCITO: Vamos testar cada tipo separadamente
                // Escolha um dos três para testar:
                string[] tiposParaTestar = { "PARCIAL", "TOTAL", "SEM COPARTICIPAÇÃO" };
                string tipoCoparticipacao = "PARCIAL"; // Mude para testar outros: "TOTAL" ou "SEM COPARTICIPAÇÃO"

                try
                {
                    Console.WriteLine($"Testando plano com coparticipação: {tipoCoparticipacao}");
                    appliedPlano = _docxService.FillPlanoCoparticipacao(outputPath, tipoCoparticipacao);

                    if (appliedPlano == 0)
                    {
                        lblErro.Text += $"<br/>AVISO: Nenhum checkbox marcado para coparticipação '{tipoCoparticipacao}'";
                    }
                    else
                    {
                        lblErro.Text += $"<br/>SUCESSO: {appliedPlano} checkbox(s) marcado(s) para coparticipação '{tipoCoparticipacao}'";
                    }
                }
                catch (Exception ex)
                {
                    lblErro.Text += $"<br/>Erro ao processar plano: {ex.Message}";
                }

                int totalApplied = appliedTitular + appliedDependentes + appliedPlano;

                // DOWNLOAD DO ARQUIVO
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                Response.AddHeader("Content-Disposition", $"attachment; filename=PROPOSTA_PREENCHIDA.docx");
                Response.TransmitFile(outputPath);
                Response.End();
            }
            catch (Exception ex)
            {
                lblErro.Text = $"ERRO: {ex.Message}<br/><br/>Stack: {ex.StackTrace}";
                lblErro.Visible = true;
            }
        }
    }
}