using appWhatsapp.PlennuscGestao.Services;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml;
using Plennusc.Core.Models.ModelsGestao.modelsButYou;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.butYouQueries;
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
using System.IO.Compression;

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


        private Dictionary<string, string> ConverterAssociadoParaDicionario(DadosAssociadoCompleto associado)
        {
            return new Dictionary<string, string>
            {
                // Vigência (você pode ajustar conforme necessário)
                { "INICIO_VIGENCIA", DateTime.Now.ToString("dd/MM/yyyy") },
        
                // Opções de Vencimento
                { "VENCIMENTO_DIA_01", "true" },
                { "VENCIMENTO_DIA_10", "true" },
                { "VENCIMENTO_DIA_20", "true" },

                // Identificação
                { "NOME_COMPLETO", associado.NomeCompleto },
                { "CPF_TITULAR", associado.CpfTitular },
                { "DATA_NASCIMENTO", associado.DataNascimento },
                { "RG", associado.Rg },

                // FILIAÇÃO E IDADE (nota: Filiacao não está na sua classe, você precisa adicionar)
                { "FILIACAO", "" }, // Você precisa adicionar esta propriedade na classe
                { "IDADE", associado.Idade },

                // SEXO - o seu método ProcessarCampoSexo espera "M" ou "F"
                // Precisamos converter "MASCULINO" para "M" e "FEMININO" para "F"
                { "SEXO", associado.Sexo == "MASCULINO" ? "M" :
                          associado.Sexo == "FEMININO" ? "F" : "OUTROS" },

                // ESTADO CIVIL
                { "ESTADO_CIVIL", associado.EstadoCivil },

                // Novos campos
                { "ORGAO_EXPEDIDOR", associado.OrgaoExpedidor },
                { "CARTAO_SUS", associado.CartaoSus },
                { "NUMERO_DECLARACAO_NASCIDO_VIVO", associado.NumeroDeclaracaoNascidoVivo },

                // Responsável financeiro (assumindo que é o próprio titular)
                { "RESPONSAVEL_FINANCEIRO", associado.NomeCompleto },
                { "CPF_RESPONSAVEL_FINANCEIRO", associado.CpfTitular },

                // Endereço
                { "ENDERECO", associado.Endereco },
                { "NUMERO", associado.Numero },
                { "COMPLEMENTO", associado.Complemento },
                { "CEP", associado.Cep },
                { "BAIRRO", associado.Bairro },
                { "CIDADE", associado.Cidade },
                { "UF", associado.Uf },

                // Contato
                { "EMAIL", associado.Email },
                { "TELEFONE_CELULAR", associado.TelefoneCelular },
                { "TELEFONE_FIXO", "" }, // Não temos na consulta

                // Tipo de coparticipação (PARCIAL, TOTAL ou SEM COPARTICIPAÇÃO)
                { "PLANO_COPARTICIPACAO", "PARCIAL" }
            };
        }


        protected void btnTestarPreenchimento_Click(object sender, EventArgs e)
        {
            try
            {
                string templatePath = Server.MapPath("~/public/uploadgestao/docs/youBut/MAIS VOCE - PE - DOCX.docx");
                string tempDir = Server.MapPath("~/temp/");
                Directory.CreateDirectory(tempDir);
                string outputPath = Path.Combine(tempDir, $"PROPOSTA_PREENCHIDA_{DateTime.Now:yyyyMMddHHmmss}.docx");


                // 1
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
                    //{ "RESPONSAVEL_FINANCEIRO", "MARIA SILVEIRA" },
                    //{ "CPF_RESPONSAVEL_FINANCEIRO", "987.654.321-00" },

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
                        { "IDADE", "19" },
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

                //Processar o titular
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

                // 4. AGORA: Processar a tabela de valores (COMPOSIÇÃO DA CONTRAPRESTAÇÃO PECUNIÁRIA MENSAL)
                var composicaoContraprestacao = new Dictionary<string, string>
                {
                    // VALORES PARA A TABELA DE CONTRAPRESTAÇÃO
                    { "VALOR_TITULAR", "R$ 450,00" },
                    { "VALOR_DEPENDENTE_1", "R$ 360,00" },
                    { "VALOR_DEPENDENTE_2", "R$ 370,00" },
                    { "VALOR_DEPENDENTE_3", "R$ 380,00" },
                    { "VALOR_DEPENDENTE_4", "R$ 390,00" },
                    { "VALOR_DEPENDENTE_5", "R$ 320,00" }
                };

                int appliedValores = 0;

                try
                {
                    Console.WriteLine($"Processando tabela de valores...");
                    appliedValores = _docxService.FillTabelaValores(outputPath, composicaoContraprestacao);

                    if (appliedValores == 0)
                    {
                        lblErro.Text += $"<br/>AVISO: Nenhum valor preenchido na tabela de contraprestação";
                    }
                    else
                    {
                        lblErro.Text += $"<br/>SUCESSO: {appliedValores} valor(es) preenchido(s) na tabela de contraprestação";
                    }
                }
                catch (Exception ex)
                {
                    lblErro.Text += $"<br/>Erro ao processar tabela de valores: {ex.Message}";
                }


                // 5. AGORA: Calcular e preencher o total da Contraprestação
                int appliedTotal = 0;

                try
                {
                    Console.WriteLine($"Processando total da contraprestação...");

                    // DEBUG: Mostrar o que vamos calcular
                    var valores = new List<string>();
                    if (composicaoContraprestacao.ContainsKey("VALOR_TITULAR"))
                        valores.Add($"Titular: {composicaoContraprestacao["VALOR_TITULAR"]}");
                    for (int i = 1; i <= 5; i++)
                    {
                        string key = $"VALOR_DEPENDENTE_{i}";
                        if (composicaoContraprestacao.ContainsKey(key))
                            valores.Add($"Dep {i}: {composicaoContraprestacao[key]}");
                    }
                    Console.WriteLine($"Valores a somar: {string.Join(", ", valores)}");

                    appliedTotal = _docxService.FillTotalContraprestacao(outputPath, composicaoContraprestacao);

                    if (appliedTotal == 0)
                    {
                        lblErro.Text += $"<br/>AVISO: Total da contraprestação não preenchido - campo não encontrado no documento";
                    }
                    else
                    {
                        lblErro.Text += $"<br/>SUCESSO: Total da contraprestação calculado e preenchido";
                    }
                }
                catch (Exception ex)
                {
                    lblErro.Text += $"<br/>Erro ao processar total da contraprestação: {ex.Message}";
                }

                int totalApplied = appliedTitular + appliedDependentes + appliedPlano + appliedValores;

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

        protected void btnPreencherComDadosReais_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Buscar associados do banco
                var queriesService = new butYouQueriesAssociados(
                    System.Configuration.ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString);

                var associados = queriesService.BuscarAssociadosCompletos();

                if (associados == null || associados.Count == 0)
                {
                    lblErro.Text = "Nenhum associado encontrado no banco de dados.";
                    lblErro.Visible = true;
                    return;
                }

                lblMensagem.Text = $"Encontrados {associados.Count} associados. Processando...";
                lblMensagem.Visible = true;

                // 2. Para cada associado, criar um documento
                int documentosCriados = 0;
                string pastaDestino = Server.MapPath("~/public/uploadgestao/docs/dadosReaisYouBut/");

                // Criar pasta se não existir
                if (!Directory.Exists(pastaDestino))
                    Directory.CreateDirectory(pastaDestino);

                foreach (var associado in associados.Take(10)) // Limitar a 10 para teste
                {
                    try
                    {
                        string templatePath = Server.MapPath("~/public/uploadgestao/docs/youBut/MAIS VOCE - PE - DOCX.docx");
                        string outputPath = Path.Combine(pastaDestino,
                            $"PROPOSTA_{associado.CodigoAssociado}_{DateTime.Now:yyyyMMddHHmmss}.docx");

                        // Converter associado para dicionário
                        var dadosTitular = ConverterAssociadoParaDicionario(associado);

                        // 3. Processar o documento
                        int appliedTitular = _docxService.FillTitularBasico(
                            templatePath,
                            outputPath,
                            dadosTitular);

                        // AGUARDAR um pouco
                        System.Threading.Thread.Sleep(100);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                        // 4. Processar dependentes (vamos deixar vazio por enquanto)
                        var dependentes = new List<Dictionary<string, string>>();
                        int appliedDependentes = _docxService.FillDependentes(outputPath, outputPath, dependentes);

                        // 5. Processar plano de coparticipação
                        string tipoCoparticipacao = "PARCIAL"; // Pode ser ajustado
                        int appliedPlano = _docxService.FillPlanoCoparticipacao(outputPath, tipoCoparticipacao);

                        // 6. Processar tabela de valores (dados fictícios por enquanto)
                        var composicaoContraprestacao = new Dictionary<string, string>
                {
                    { "VALOR_TITULAR", "R$ 450,00" },
                    { "VALOR_DEPENDENTE_1", "R$ 360,00" },
                    { "VALOR_DEPENDENTE_2", "R$ 370,00" },
                    { "VALOR_DEPENDENTE_3", "R$ 380,00" },
                    { "VALOR_DEPENDENTE_4", "R$ 390,00" },
                    { "VALOR_DEPENDENTE_5", "R$ 320,00" }
                };

                        int appliedValores = _docxService.FillTabelaValores(outputPath, composicaoContraprestacao);

                        // 7. Calcular e preencher o total
                        int appliedTotal = _docxService.FillTotalContraprestacao(outputPath, composicaoContraprestacao);

                        documentosCriados++;

                        lblMensagem.Text += $"<br/>Documento criado para {associado.NomeCompleto}";

                    }
                    catch (Exception exAssociado)
                    {
                        lblErro.Text += $"<br/>Erro ao processar {associado.NomeCompleto}: {exAssociado.Message}";
                    }
                }

                lblMensagem.Text += $"<br/><br/>Processamento concluído! {documentosCriados} documentos criados na pasta {pastaDestino}";

                // 8. Criar um ZIP com todos os documentos
                string zipPath = Path.Combine(pastaDestino, $"PROPOSTAS_{DateTime.Now:yyyyMMddHHmmss}.zip");
                // Certifique-se de que o projeto tem referência ao assembly System.IO.Compression.FileSystem.
                // No Visual Studio, clique com o botão direito no projeto > Adicionar > Referência > Procure por "System.IO.Compression.FileSystem" e marque.

                // O código abaixo permanece igual, pois o erro é apenas de referência/using:
                System.IO.Compression.ZipFile.CreateFromDirectory(pastaDestino, zipPath);

                // 9. Oferecer download do ZIP
                Response.Clear();
                Response.ContentType = "application/zip";
                Response.AddHeader("Content-Disposition", $"attachment; filename=PROPOSTAS_COMPLETAS.zip");
                Response.TransmitFile(zipPath);
                Response.End();

            }
            catch (Exception ex)
            {
                lblErro.Text = $"ERRO GERAL: {ex.Message}<br/><br/>Stack: {ex.StackTrace}";
                lblErro.Visible = true;
            }
        }
    }
}