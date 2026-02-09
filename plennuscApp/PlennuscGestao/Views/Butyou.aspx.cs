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

        private Dictionary<string, string> ConverterDependenteParaDicionario(DadosAssociadoCompleto dependente)
        {
            // Determinar parentesco baseado na idade
            string parentesco = DeterminarParentesco(dependente);

            return new Dictionary<string, string>
            {
                { "NOME_COMPLETO", dependente.NomeCompleto },
                { "DATA_NASCIMENTO", dependente.DataNascimento },
                { "SEXO", dependente.Sexo == "MASCULINO" ? "M" :
                          dependente.Sexo == "FEMININO" ? "F" : "OUTROS" },
                { "ESTADO_CIVIL", dependente.EstadoCivil },
                { "IDADE", dependente.Idade },
                { "CPF", dependente.CpfTitular },
                { "FILIACAO", dependente.Filiacao },
                { "RG", dependente.Rg },
                { "PARENTESCO", parentesco },
                { "ORGAO_EXPEDIDOR", dependente.OrgaoExpedidor },
                { "CARTAO_SUS", dependente.CartaoSus },
                { "NUMERO_DECLARACAO_NASCIDO_VIVO", dependente.NumeroDeclaracaoNascidoVivo },
                // Campos de endereço do dependente (pode ser o mesmo do titular)
                { "ENDERECO", dependente.Endereco },
                { "NUMERO", dependente.Numero },
                { "COMPLEMENTO", dependente.Complemento },
                { "CEP", dependente.Cep },
                { "BAIRRO", dependente.Bairro },
                { "CIDADE", dependente.Cidade },
                { "UF", dependente.Uf },
                { "EMAIL", dependente.Email },
                { "TELEFONE_CELULAR", dependente.TelefoneCelular },
                { "TELEFONE_FIXO", "" } // Se não tiver
            };
        }

        private string DeterminarParentesco(DadosAssociadoCompleto dependente)
        {
            // Lógica para determinar parentesco baseado em idade e estado civil
            if (int.TryParse(dependente.Idade, out int idade))
            {
                // Se for criança/ adolescente, provavelmente é FILHO
                if (idade <= 18)
                    return "FILHO";

                // Se for adulto e casado, pode ser CÔNJUGE
                if (dependente.EstadoCivil == "CASADO" && idade >= 18)
                    return "CÔNJUGE";

                // Outros casos
                if (idade > 60)
                    return "PAI/MAE";
            }

            return "OUTROS";
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
                // 1. Buscar todos os associados (titulares e dependentes)
                var queriesService = new butYouQueriesAssociados(
                    System.Configuration.ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString);

                var todosAssociados = queriesService.BuscarAssociadosCompletos();

                if (todosAssociados == null || todosAssociados.Count == 0)
                {
                    lblErro.Text = "Nenhum associado encontrado no banco de dados.";
                    lblErro.Visible = true;
                    return;
                }

                // 2. Agrupar titulares com seus dependentes
                var gruposTitulares = AgruparTitularesComDependentes(todosAssociados);

                lblMensagem.Text = $"Encontrados {gruposTitulares.Count} titulares (com dependentes). Processando...";
                lblMensagem.Visible = true;

                // 3. Para cada grupo (titular + dependentes), criar um documento
                int documentosCriados = 0;
                string pastaDestino = Server.MapPath("~/public/uploadgestao/docs/dadosReaisYouBut/");

                // Criar pasta se não existir
                if (!Directory.Exists(pastaDestino))
                    Directory.CreateDirectory(pastaDestino);

                // Limitar a 5 grupos para teste
                foreach (var grupo in gruposTitulares.Take(500))
                {
                    try
                    {
                        string templatePath = Server.MapPath("~/public/uploadgestao/docs/youBut/MAIS VOCE - PE - DOCX.docx");

                        // Nome do arquivo
                        string nomeArquivo = GerarNomeArquivo(grupo.Titular, grupo.Dependentes.Count);

                        string outputPath = Path.Combine(pastaDestino, nomeArquivo);

                        // Converter titular para dicionário
                        var dadosTitular = ConverterAssociadoParaDicionario(grupo.Titular);

                        // 4. Converter dependentes para lista de dicionários
                        var listaDependentes = new List<Dictionary<string, string>>();
                        int dependenteNumero = 1;

                        foreach (var dependente in grupo.Dependentes.Take(500)) // Máximo 5 dependentes por formulário
                        {
                            var dictDependente = ConverterDependenteParaDicionario(dependente);
                            // Adicionar número do dependente (DEPENDENTE 1, DEPENDENTE 2, etc.)
                            dictDependente.Add("NUMERO_DEPENDENTE", dependenteNumero.ToString());
                            listaDependentes.Add(dictDependente);
                            dependenteNumero++;
                        }

                        // 5. Processar o documento com o titular
                        int appliedTitular = _docxService.FillTitularBasico(
                            templatePath,
                            outputPath,
                            dadosTitular);

                        // AGUARDAR um pouco
                        System.Threading.Thread.Sleep(100);
                        GC.Collect();
                        GC.WaitForPendingFinalizers();

                        // 6. Processar os dependentes
                        int appliedDependentes = 0;
                        if (listaDependentes.Count > 0)
                        {
                            appliedDependentes = _docxService.FillDependentes(outputPath, outputPath, listaDependentes);
                        }

                        // 7. Processar plano de coparticipação
                        string tipoCoparticipacao = "PARCIAL"; // Pode ser ajustado
                        int appliedPlano = _docxService.FillPlanoCoparticipacao(outputPath, tipoCoparticipacao);

                        // 8. Processar tabela de valores (dados fictícios por enquanto)
                        var composicaoContraprestacao = new Dictionary<string, string>
                        {
                            { "VALOR_TITULAR", "R$ 450,00" },
                            { "VALOR_DEPENDENTE_1", listaDependentes.Count >= 1 ? "R$ 360,00" : "" },
                            { "VALOR_DEPENDENTE_2", listaDependentes.Count >= 2 ? "R$ 370,00" : "" },
                            { "VALOR_DEPENDENTE_3", listaDependentes.Count >= 3 ? "R$ 380,00" : "" },
                            { "VALOR_DEPENDENTE_4", listaDependentes.Count >= 4 ? "R$ 390,00" : "" },
                            { "VALOR_DEPENDENTE_5", listaDependentes.Count >= 5 ? "R$ 320,00" : "" }
                        };

                        int appliedValores = _docxService.FillTabelaValores(outputPath, composicaoContraprestacao);

                        // 9. Calcular e preencher o total
                        int appliedTotal = _docxService.FillTotalContraprestacao(outputPath, composicaoContraprestacao);

                        documentosCriados++;

                        lblMensagem.Text += $"<br/>✓ Documento criado para {grupo.Titular.NomeCompleto} com {grupo.Dependentes.Count} dependente(s)";

                    }
                    catch (Exception exTitular)
                    {
                        lblErro.Text += $"<br/>✗ Erro ao processar {grupo.Titular.NomeCompleto}: {exTitular.Message}";
                    }
                }

                lblMensagem.Text += $"<br/><br/>✅ Processamento concluído! {documentosCriados} documentos criados na pasta: {pastaDestino}";

                // 10. Criar um ZIP com todos os documentos
                string zipPath = Path.Combine(pastaDestino, $"PROPOSTAS_{DateTime.Now:yyyyMMddHHmmss}.zip");

                // Usando o método alternativo que já criamos
                CriarZipComArquivos(pastaDestino, zipPath);

                // 11. Oferecer download do ZIP
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

        private List<GrupoTitularDependentes> AgruparTitularesComDependentes(List<DadosAssociadoCompleto> todosAssociados)
        {
            var grupos = new List<GrupoTitularDependentes>();

            // 1. Primeiro, separar titulares
            var titulares = todosAssociados
                .Where(a => a.TipoAssociado == "T")
                .ToList();

            // 2. Para cada titular, criar um grupo
            foreach (var titular in titulares)
            {
                var grupo = new GrupoTitularDependentes
                {
                    Titular = titular
                };

                // 3. Buscar dependentes deste titular
                grupo.Dependentes = todosAssociados
                    .Where(a => a.TipoAssociado == "D" && a.CodigoTitular == titular.CodigoAssociado)
                    .ToList();

                grupos.Add(grupo);
            }

            return grupos;
        }

        private void CriarZipComArquivos(string pastaOrigem, string arquivoZipDestino)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    // Adiciona todos os arquivos da pasta ao ZIP
                    foreach (var file in Directory.GetFiles(pastaOrigem))
                    {
                        var entryName = Path.GetFileName(file);
                        var entry = archive.CreateEntry(entryName);

                        using (var entryStream = entry.Open())
                        using (var fileStream = File.OpenRead(file))
                        {
                            fileStream.CopyTo(entryStream);
                        }
                    }
                }

                // Salva o ZIP no disco
                using (var fileStream = new FileStream(arquivoZipDestino, FileMode.Create))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }
            }
        }

        private string GerarNomeArquivo(DadosAssociadoCompleto titular, int quantidadeDependentes)
        {
            // 1. Nome do titular (primeiro nome + sobrenome)
            string nomeTitular = FormatacaoNomeTitular(titular.NomeCompleto);

            // 2. CPF (apenas números)
            string cpfNumeros = ExtrairApenasNumeros(titular.CpfTitular);

            // 3. Indicador de dependentes
            string indicadorDependentes = quantidadeDependentes > 0 ? "C_DEP" : "S_DEP";

            // 4. Data/hora (opcional, para evitar conflitos)
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            // Montar nome do arquivo
            string nomeArquivo = $"{nomeTitular}_{cpfNumeros}_{indicadorDependentes}_{timestamp}.docx";

            // Limpar caracteres inválidos
            return RemoveCaracteresInvalidos(nomeArquivo);
        }

        private string FormatacaoNomeTitular(string nomeCompleto)
        {
            if (string.IsNullOrWhiteSpace(nomeCompleto))
                return "SEM_NOME";

            // Remover acentos e caracteres especiais
            nomeCompleto = RemoverAcentos(nomeCompleto);

            // Pegar apenas o primeiro e último nome (ou os 2 primeiros se nome composto)
            var partes = nomeCompleto.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (partes.Length >= 2)
            {
                // Primeiro nome + último sobrenome
                return $"{partes[0]}_{partes[partes.Length - 1]}";
            }
            else if (partes.Length == 1)
            {
                return partes[0];
            }

            return "SEM_NOME";
        }

        private string RemoverAcentos(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            string comAcentos = "áàâãäéèêëíìîïóòôõöúùûüçÁÀÂÃÄÉÈÊËÍÌÎÏÓÒÔÕÖÚÙÛÜÇ";
            string semAcentos = "aaaaaeeeeiiiiooooouuuucAAAAAEEEEIIIIOOOOOUUUUC";

            for (int i = 0; i < comAcentos.Length; i++)
            {
                texto = texto.Replace(comAcentos[i], semAcentos[i]);
            }

            return texto;
        }

        private string ExtrairApenasNumeros(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return "00000000000";

            // Extrair apenas números
            return new string(texto.Where(char.IsDigit).ToArray());
        }

        private string RemoveCaracteresInvalidos(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return "arquivo_sem_nome.docx";

            var invalidChars = Path.GetInvalidFileNameChars();

            // Substituir caracteres inválidos por underscore
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c, '_');
            }

            // Remover múltiplos underscores consecutivos
            while (fileName.Contains("__"))
            {
                fileName = fileName.Replace("__", "_");
            }

            // Remover underscores no início ou fim
            fileName = fileName.Trim('_');

            // Garantir que não exceda o limite de caracteres (255 é seguro para maioria dos sistemas)
            if (fileName.Length > 100)
            {
                fileName = fileName.Substring(0, 100);
            }

            return fileName;
        }
    }
}