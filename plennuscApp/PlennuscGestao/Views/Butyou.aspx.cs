using appWhatsapp.PlennuscGestao.Services;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using OfficeOpenXml;
using Plennusc.Core.Models.ModelsGestao.modelsButYou;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.butYouQueries;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
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
                { "SEXO", associado.Sexo == "M" ? "M" :
                          associado.Sexo == "F" ? "F" : "OUTROS" },

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
                { "SEXO", dependente.Sexo == "M" ? "M" :
                          dependente.Sexo == "F" ? "F" : "OUTROS" },
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
                string pastaDestino = @"\\Urano\vallorben_docs\DOC.PUBLICO\plennuc_ClickSing\";

                // Criar pasta se não existir
                if (!Directory.Exists(pastaDestino))
                    Directory.CreateDirectory(pastaDestino);

                // Limitar a 500 grupos para teste
                foreach (var grupo in gruposTitulares.Take(1400))
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

                        foreach (var dependente in grupo.Dependentes.Take(1400)) // Máximo 500 dependentes por formulário
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

                        //        // 7. Processar plano de coparticipação
                        //        string tipoCoparticipacao = "PARCIAL"; // Pode ser ajustado
                        //        int appliedPlano = _docxService.FillPlanoCoparticipacao(outputPath, tipoCoparticipacao);

                        // 8. Processar tabela de valores (dados fictícios por enquanto)
                        // Composição de contraprestação com valores REAIS do banco
                        var composicaoContraprestacao = new Dictionary<string, string>();

                        // Valor do titular (se existir)
                        if (grupo.Titular.ValorPlano.HasValue)
                            composicaoContraprestacao["VALOR_TITULAR"] = grupo.Titular.ValorPlano.Value.ToString("C2", new CultureInfo("pt-BR"));
                        else
                            composicaoContraprestacao["VALOR_TITULAR"] = "";

                        // Valores dos dependentes
                        int idx = 1;
                        foreach (var dep in grupo.Dependentes.Take(5)) // Máximo 5 dependentes (ajuste se necessário)
                        {
                            string chave = $"VALOR_DEPENDENTE_{idx}";
                            if (dep.ValorPlano.HasValue)
                                composicaoContraprestacao[chave] = dep.ValorPlano.Value.ToString("C2", new CultureInfo("pt-BR"));
                            else
                                composicaoContraprestacao[chave] = "";
                            idx++;
                        }

                        int appliedValores = _docxService.FillTabelaValores(outputPath, composicaoContraprestacao);

                        // 9. Calcular e preencher o total
                        int appliedTotal = _docxService.FillTotalContraprestacao(outputPath, composicaoContraprestacao);

                        documentosCriados++;

                        //        lblMensagem.Text += $"<br/>✓ Documento criado para {grupo.Titular.NomeCompleto} com {grupo.Dependentes.Count} dependente(s)";

                        //        // Log adicional para depuração
                        //        System.Diagnostics.Debug.WriteLine($"Documento criado: {outputPath}");

                    }
                    catch (Exception exTitular)
                    {
                        lblErro.Text += $"<br/>✗ Erro ao processar {grupo.Titular.NomeCompleto}: {exTitular.Message}";
                        lblErro.Visible = true;
                    }
                }

                // 10. Mostrar mensagem final SEM criar ZIP
                lblMensagem.Text += $"<br/><br/>✅ Processamento concluído! {documentosCriados} documentos criados na pasta: {pastaDestino}";

                // Opcional: Adicionar link para abrir a pasta
                if (documentosCriados > 0)
                {
                    lblMensagem.Text += $"<br/><a href='file:///{pastaDestino.Replace("\\", "/")}' target='_blank'>Abrir pasta de documentos</a>";
                }

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

        private string GerarNomeArquivo(DadosAssociadoCompleto titular, int quantidadeDependentes)
        {
            try
            {
                // 1. EMAIL - limpar e garantir
                string email = LimparCampo(titular.Email?.Trim(), "sememail", false);

                // 2. NOME COMPLETO - substituir espaços por UNDERLINE ÚNICO
                string nomeCompleto = FormatarNomeComUnderline(titular.NomeCompleto?.Trim());

                // 3. CPF - apenas números
                string cpf = LimparCpf(titular.CpfTitular);

                // 4. DATA - formato DDMMAAAA
                string dataNascimento = ConverterDataParaDDMMAAAA(titular.DataNascimento);

                // 5. MONTAR COM DOIS UNDERLINES entre campos
                string nomeArquivo = $"{email}__{nomeCompleto}__{cpf}__{dataNascimento}.docx";

                return SanitizarNomeArquivo(nomeArquivo);
            }
            catch (Exception ex)
            {
                string cpfBackup = LimparCpf(titular.CpfTitular) ?? "00000000000";
                return $"erro_geracao__{cpfBackup}__{DateTime.Now:HHmmss}.docx";
            }
        }

        // MÉTODO NOVO: Formatar nome com UNDERLINE ÚNICO entre palavras
        private string FormatarNomeComUnderline(string nomeCompleto)
        {
            if (string.IsNullOrWhiteSpace(nomeCompleto))
                return "sem_nome";

            // 1. Remover acentos
            nomeCompleto = RemoverAcentos(nomeCompleto);

            // 2. Substituir múltiplos espaços por espaço único
            nomeCompleto = System.Text.RegularExpressions.Regex.Replace(nomeCompleto, @"\s+", " ");

            // 3. Substituir espaços por UNDERLINE ÚNICO (_)
            nomeCompleto = nomeCompleto.Replace(' ', '_');

            // 4. Remover caracteres especiais (mantém letras, números e underline)
            nomeCompleto = new string(nomeCompleto.Where(c =>
                char.IsLetterOrDigit(c) || c == '_').ToArray());

            // 5. Remover underlines consecutivos (caso houvesse múltiplos espaços)
            while (nomeCompleto.Contains("__"))
            {
                nomeCompleto = nomeCompleto.Replace("__", "_");
            }

            // 6. Limitar tamanho
            if (nomeCompleto.Length > 100)
                nomeCompleto = nomeCompleto.Substring(0, 100);

            return nomeCompleto;
        }

        // MÉTODO NOVO: Limpar campo genérico
        private string LimparCampo(string valor, string valorPadrao, bool permitirCaracteresEspeciais = false)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return valorPadrao;

            valor = valor.Trim();

            if (!permitirCaracteresEspeciais)
            {
                // Para email, removemos apenas acentos
                valor = RemoverAcentos(valor);
            }

            return valor;
        }

        // Método já existente - ajustar se necessário
        private string RemoverAcentos(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return texto;

            string comAcentos = "áàâãäéèêëíìîïóòôõöúùûüçÁÀÂÃÄÉÈÊËÍÌÎÏÓÒÔÕÖÚÙÛÜÇñÑ";
            string semAcentos = "aaaaaeeeeiiiiooooouuuucAAAAAEEEEIIIIOOOOOUUUUCnN";

            for (int i = 0; i < comAcentos.Length; i++)
            {
                texto = texto.Replace(comAcentos[i], semAcentos[i]);
            }

            return texto;
        }

        private string LimparCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return "00000000000";

            // Extrair apenas números
            string cpfNumeros = new string(cpf.Where(char.IsDigit).ToArray());

            // Garantir 11 dígitos
            if (cpfNumeros.Length != 11)
            {
                if (cpfNumeros.Length < 11)
                    cpfNumeros = cpfNumeros.PadLeft(11, '0');
                else if (cpfNumeros.Length > 11)
                    cpfNumeros = cpfNumeros.Substring(0, 11);
            }

            return cpfNumeros;
        }

        private string ConverterDataParaDDMMAAAA(string dataNascimento)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dataNascimento))
                    return "00000000";

                DateTime data;

                // Tentar formatos comuns
                if (DateTime.TryParseExact(dataNascimento, "dd/MM/yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None, out data))
                {
                    return data.ToString("ddMMyyyy");
                }
                else if (DateTime.TryParse(dataNascimento, out data))
                {
                    return data.ToString("ddMMyyyy");
                }
                else
                {
                    // Extrair apenas números
                    string numeros = new string(dataNascimento.Where(char.IsDigit).ToArray());

                    if (numeros.Length == 8)
                        return numeros;

                    return "00000000";
                }
            }
            catch
            {
                return "00000000";
            }
        }

        private string SanitizarNomeArquivo(string nomeArquivo)
        {
            if (string.IsNullOrWhiteSpace(nomeArquivo))
                return "documento_sem_nome.docx";

            // Caracteres inválidos para nome de arquivo
            char[] caracteresInvalidos = Path.GetInvalidFileNameChars();

            // Remover caracteres inválidos (mas manter @ e . para email)
            foreach (char c in caracteresInvalidos)
            {
                if (c != '@' && c != '.') // Mantemos @ e . para email
                {
                    nomeArquivo = nomeArquivo.Replace(c, '_');
                }
            }

            // Garantir que temos DOIS underlines entre campos
            // Regex para garantir o padrão: campo1__campo2__campo3__campo4.docx
            nomeArquivo = System.Text.RegularExpressions.Regex.Replace(nomeArquivo, @"_{3,}", "__");

            // Remover underlines no início e fim
            nomeArquivo = nomeArquivo.Trim('_');

            // Limitar tamanho
            if (nomeArquivo.Length > 200)
            {
                string extensao = Path.GetExtension(nomeArquivo);
                string nomeSemExtensao = Path.GetFileNameWithoutExtension(nomeArquivo);

                if (nomeSemExtensao.Length > 195)
                    nomeSemExtensao = nomeSemExtensao.Substring(0, 195);

                nomeArquivo = nomeSemExtensao + extensao;
            }

            return nomeArquivo;
        }
    }
}