using appWhatsapp.PlennuscGestao.Services;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using Plennusc.Core.Models.ModelsGestao.modelsButYou;
using Plennusc.Core.Service.ServiceGestao.serviceYouBut;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.butYouQueries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class enterpriseButYou : System.Web.UI.Page
    {
        // USE APENAS EnterpriseDocx, NADA de DocxService
        private EnterpriseDocx _enterpriseDocx = new EnterpriseDocx();
        private DocxService _docxService = new DocxService();
        private ExcelService _excelService = new ExcelService();

        protected void Page_Load(object sender, EventArgs e)
        {
            //OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
        }
        protected void propostaEmpresa_Click(object sender, EventArgs e)
        {
            try
            {
                // ==================== DADOS FICTÍCIOS ====================
                var dadosEmpresa = new Dictionary<string, string>
        {
            { "RAZAO_SOCIAL", "EMPRESA MODELO LTDA" },
            { "NOME_FANTASIA", "MODELO SAÚDE" },
            { "ENDERECO_EMPRESA", "AVENIDA PAULISTA, 1000" },
            { "BAIRRO_EMPRESA", "BELA VISTA" },
            { "CEP_EMPRESA", "01310-100" },
            { "ESTADO_EMPRESA", "SP" },
            { "CIDADE_EMPRESA", "SÃO PAULO" },
            { "CNPJ", "12.345.678/0001-90" },
            { "DATA_INSCRICAO_CNPJ", "01/01/2020" },
            { "INSCRICAO_MUNICIPAL", "123456" },
            { "INSCRICAO_ESTADUAL", "987654" },
            { "EMAIL_EMPRESA", "contato@modelo.com.br" },
            { "TELEFONE_EMPRESA", "(11) 3333-4444" }
        };

                // Titular 1 (João + 2 dependentes)
                var dadosTitular1 = new Dictionary<string, string>
        {
            { "NOME_COMPLETO", "JOÃO DA SILVA" },
            { "CPF_TITULAR", "111.222.333-44" },
            { "DATA_NASCIMENTO", "10/05/1980" },
            { "RG", "12.345.678-9" },
            { "FILIACAO", "PAI: JOSÉ, MÃE: MARIA" },
            { "IDADE", "45" },
            { "SEXO", "M" },
            { "ESTADO_CIVIL", "CASADO" },
            { "ORGAO_EXPEDIDOR", "SSP/SP" },
            { "CARTAO_SUS", "123456789012345" },
            { "ENDERECO", "RUA A, 100" },
            { "NUMERO", "100" },
            { "COMPLEMENTO", "APTO 1" },
            { "CEP", "01010-010" },
            { "BAIRRO", "CENTRO" },
            { "CIDADE", "SÃO PAULO" },
            { "UF", "SP" },
            { "EMAIL", "joao@email.com" },
            { "TELEFONE_CELULAR", "(11) 99999-8888" }
        };

                var dependentes1 = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string>
            {
                { "NOME_COMPLETO", "MARIA DA SILVA" },
                { "DATA_NASCIMENTO", "15/08/1982" },
                { "SEXO", "F" },
                { "ESTADO_CIVIL", "CASADO" },
                { "IDADE", "43" },
                { "CPF", "222.333.444-55" },
                { "FILIACAO", "PAI: ANTÔNIO, MÃE: JOANA" },
                { "RG", "98.765.432-1" },
                { "PARENTESCO", "CÔNJUGE" }
            },
            new Dictionary<string, string>
            {
                { "NOME_COMPLETO", "PEDRO DA SILVA" },
                { "DATA_NASCIMENTO", "10/01/2010" },
                { "SEXO", "M" },
                { "ESTADO_CIVIL", "SOLTEIRO" },
                { "IDADE", "15" },
                { "CPF", "333.444.555-66" },
                { "FILIACAO", "PAI: JOÃO, MÃE: MARIA" },
                { "RG", "11.223.344-5" },
                { "PARENTESCO", "FILHO" }
            }
        };

                // Titular 2 (Maria + 1 dependente)
                var dadosTitular2 = new Dictionary<string, string>
        {
            { "NOME_COMPLETO", "MARIA APARECIDA OLIVEIRA" },
            { "CPF_TITULAR", "444.555.666-77" },
            { "DATA_NASCIMENTO", "25/08/1985" },
            { "RG", "55.666.777-8" },
            { "FILIACAO", "PAI: JOÃO, MÃE: TEREZA" },
            { "IDADE", "40" },
            { "SEXO", "F" },
            { "ESTADO_CIVIL", "DIVORCIADA" },
            { "ORGAO_EXPEDIDOR", "SSP/SP" },
            { "CARTAO_SUS", "999888777666555" },
            { "ENDERECO", "RUA C, 300" },
            { "NUMERO", "300" },
            { "COMPLEMENTO", "" },
            { "CEP", "03030-030" },
            { "BAIRRO", "MOOCA" },
            { "CIDADE", "SÃO PAULO" },
            { "UF", "SP" },
            { "EMAIL", "maria@email.com" },
            { "TELEFONE_CELULAR", "(11) 98888-7777" }
        };

                var dependentes2 = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string>
            {
                { "NOME_COMPLETO", "ANA OLIVEIRA" },
                { "DATA_NASCIMENTO", "12/03/2015" },
                { "SEXO", "F" },
                { "ESTADO_CIVIL", "SOLTEIRO" },
                { "IDADE", "10" },
                { "CPF", "555.666.777-88" },
                { "FILIACAO", "PAI: CARLOS, MÃE: MARIA" },
                { "RG", "66.777.888-9" },
                { "PARENTESCO", "FILHA" }
            }
        };

                // Titular 3 (Pedro + 3 dependentes)
                var dadosTitular3 = new Dictionary<string, string>
        {
            { "NOME_COMPLETO", "PEDRO HENRIQUE SANTOS" },
            { "CPF_TITULAR", "666.777.888-99" },
            { "DATA_NASCIMENTO", "02/02/1970" },
            { "RG", "77.888.999-0" },
            { "FILIACAO", "PAI: ANTÔNIO, MÃE: LUZIA" },
            { "IDADE", "55" },
            { "SEXO", "M" },
            { "ESTADO_CIVIL", "CASADO" },
            { "ORGAO_EXPEDIDOR", "SSP/SP" },
            { "CARTAO_SUS", "888777666555444" },
            { "ENDERECO", "RUA D, 400" },
            { "NUMERO", "400" },
            { "COMPLEMENTO", "CASA 10" },
            { "CEP", "04040-040" },
            { "BAIRRO", "SAÚDE" },
            { "CIDADE", "SÃO PAULO" },
            { "UF", "SP" },
            { "EMAIL", "pedro@email.com" },
            { "TELEFONE_CELULAR", "(11) 96666-5555" }
        };

                var dependentes3 = new List<Dictionary<string, string>>
        {
            new Dictionary<string, string>
            {
                { "NOME_COMPLETO", "LUCAS SANTOS" },
                { "DATA_NASCIMENTO", "10/10/2000" },
                { "SEXO", "M" },
                { "ESTADO_CIVIL", "SOLTEIRO" },
                { "IDADE", "25" },
                { "CPF", "777.888.999-00" },
                { "FILIACAO", "PAI: PEDRO, MÃE: JOANA" },
                { "RG", "88.999.000-1" },
                { "PARENTESCO", "FILHO" }
            },
            new Dictionary<string, string>
            {
                { "NOME_COMPLETO", "BEATRIZ SANTOS" },
                { "DATA_NASCIMENTO", "05/05/2005" },
                { "SEXO", "F" },
                { "ESTADO_CIVIL", "SOLTEIRO" },
                { "IDADE", "20" },
                { "CPF", "888.999.000-11" },
                { "FILIACAO", "PAI: PEDRO, MÃE: JOANA" },
                { "RG", "99.000.111-2" },
                { "PARENTESCO", "FILHA" }
            },
            new Dictionary<string, string>
            {
                { "NOME_COMPLETO", "GABRIEL SANTOS" },
                { "DATA_NASCIMENTO", "20/12/2018" },
                { "SEXO", "M" },
                { "ESTADO_CIVIL", "SOLTEIRO" },
                { "IDADE", "7" },
                { "CPF", "999.000.111-22" },
                { "FILIACAO", "PAI: PEDRO, MÃE: JOANA" },
                { "RG", "00.111.222-3" },
                { "PARENTESCO", "FILHO" }
            }
        };

                // Lista de titulares com seus dependentes
                var titulares = new List<Tuple<Dictionary<string, string>, List<Dictionary<string, string>>>>
        {
            Tuple.Create(dadosTitular1, dependentes1),
            Tuple.Create(dadosTitular2, dependentes2),
            Tuple.Create(dadosTitular3, dependentes3)
        };

                // ==================== CRIAÇÃO DO DOCUMENTO ====================
                string templatePath = Server.MapPath("~/public/uploadgestao/docs/youBut/Proposta_MVCS_E_PME_VSF_PARCIAL.docx");
                string tempDir = Server.MapPath("~/temp/");
                Directory.CreateDirectory(tempDir);
                string outputPath = Path.Combine(tempDir, $"PROPOSTA_EMPRESA_{DateTime.Now:yyyyMMddHHmmss}.docx");

                // Faz uma única cópia do template
                File.Copy(templatePath, outputPath, true);

                // ==================== PREENCHER DADOS DA EMPRESA ====================
                _enterpriseDocx.FillDadosEmpresa(outputPath, dadosEmpresa);

                // Aguarda e limpa
                System.Threading.Thread.Sleep(100);
                GC.Collect();
                GC.WaitForPendingFinalizers();

                // ==================== PROCESSAR CADA TITULAR ====================
                for (int i = 0; i < titulares.Count; i++)
                {
                    var dadosTitular = titulares[i].Item1;
                    var dependentes = titulares[i].Item2;

                    if (i == 0)
                    {
                        // Primeiro titular → bloco 0
                        _enterpriseDocx.FillTitularBasico(outputPath, outputPath, dadosTitular, 0);
                        Thread.Sleep(100); GC.Collect(); GC.WaitForPendingFinalizers();

                        if (dependentes.Count > 0)
                        {
                            _enterpriseDocx.FillDependentes(outputPath, outputPath, dependentes, 0); // ← índice 0
                            Thread.Sleep(100); GC.Collect(); GC.WaitForPendingFinalizers();
                        }
                    }
                    else if (i == 1)
                    {
                        // Segundo titular → bloco 1
                        _enterpriseDocx.FillTitularBasico(outputPath, outputPath, dadosTitular, 1);
                        Thread.Sleep(100); GC.Collect(); GC.WaitForPendingFinalizers();

                        if (dependentes.Count > 0)
                        {
                            _enterpriseDocx.FillDependentes(outputPath, outputPath, dependentes, 1);
                            Thread.Sleep(100); GC.Collect(); GC.WaitForPendingFinalizers();
                        }
                    }
                    else
                    {
                        // Terceiro em diante: duplica a página 2 e preenche o novo bloco
                        _enterpriseDocx.DuplicarPaginaBeneficiario(outputPath);
                        Thread.Sleep(100); GC.Collect(); GC.WaitForPendingFinalizers();

                        // Após duplicar, o novo bloco terá índice i (pois agora há i+1 blocos)
                        _enterpriseDocx.FillTitularBasico(outputPath, outputPath, dadosTitular, i);
                        Thread.Sleep(100); GC.Collect(); GC.WaitForPendingFinalizers();

                        if (dependentes.Count > 0)
                        {
                            _enterpriseDocx.FillDependentes(outputPath, outputPath, dependentes, i);
                            Thread.Sleep(100); GC.Collect(); GC.WaitForPendingFinalizers();
                        }
                    }
                }

                // ==================== DOWNLOAD ====================
                Response.Clear();
                Response.ContentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                Response.AddHeader("Content-Disposition", $"attachment; filename=PROPOSTA_EMPRESA_{DateTime.Now:yyyyMMdd}.docx");
                Response.TransmitFile(outputPath);
                Response.End();
            }
            catch (Exception ex)
            {
                lblErro.Text = $"ERRO: {ex.Message}<br/>{ex.StackTrace}";
                lblErro.Visible = true;
            }
        }
    }
}