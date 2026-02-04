using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace appWhatsapp.PlennuscGestao.Services
{
    public class DocxService
    {
        private static readonly Dictionary<string, string> CheckboxesMap = new Dictionary<string, string>
        {
            { "☐", "☒" },      // Checkbox vazio para marcado
            { "☑", "☒" },      // Checkbox marcado diferente (adicionar esta linha)
            { "□", "☒" },      // Quadrado vazio
            { "◻", "☒" },      // Quadrado branco
            { "○", "●" },      // Círculo vazio para preenchido
            { "[ ]", "[X]" },  // Checkbox entre colchetes
            { "( )", "(X)" },  // Checkbox entre parênteses
            { "(_)", "(X)" }   // Checkbox com underline
        };

        // MÉTODO PARA CRIAR RUN PROPERTIES SEM COR (para rótulos)
        private RunProperties CriarRunPropertiesNormal()
        {
            var runProperties = new RunProperties();
            runProperties.Append(new FontSize() { Val = "12" });
            runProperties.Append(new FontSizeComplexScript() { Val = "12" });
            return runProperties;
        }

        public int FillTitularBasico(
       string templatePath,
       string outputPath,
       Dictionary<string, string> dados)
        {
            File.Copy(templatePath, outputPath, true);
            int applied = 0;

            using (var doc = WordprocessingDocument.Open(outputPath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;

                var tables = body.Descendants<Table>().ToList();
                bool emSecaoTitular = false;

                foreach (var table in tables)
                {
                    var rows = table.Descendants<TableRow>().ToList();

                    for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
                    {
                        var cells = rows[rowIndex].Descendants<TableCell>().ToList();

                        // 1. VERIFICAR SE É LINHA DE SEÇÃO
                        bool linhaEhCabecalho = false;

                        for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
                        {
                            var cell = cells[cellIndex];
                            var cellTexts = cell.Descendants<Text>().ToList();

                            if (cellTexts.Count == 0) continue;

                            string cellContent = string.Join("", cellTexts.Select(t => t.Text));
                            string upperContent = cellContent.ToUpperInvariant();

                            // Se encontrar "DADOS DO TITULAR", ativa a seção
                            if (upperContent.Contains("DADOS") && upperContent.Contains("TITULAR"))
                            {
                                emSecaoTitular = true;
                                linhaEhCabecalho = true;
                                break;
                            }

                            // Se encontrar "DEPENDENTE", desativa a seção
                            if (upperContent.Contains("DEPENDENTE"))
                            {
                                emSecaoTitular = false;
                                linhaEhCabecalho = true;
                                break;
                            }
                        }

                        if (linhaEhCabecalho) continue;

                        // 2. SE NÃO ESTÁ NA SEÇÃO DO TITULAR, PULAR (exceto VIGÊNCIA)
                        if (!emSecaoTitular)
                        {
                            // Verificar se é linha de VIGÊNCIA com checkboxes
                            bool encontrouVigencia = false;
                            bool encontrouInicioVigencia = false; // NOVA VARIÁVEL

                            for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
                            {
                                var cell = cells[cellIndex];
                                var cellTexts = cell.Descendants<Text>().ToList();

                                if (cellTexts.Count == 0) continue;

                                string cellContent = string.Join("", cellTexts.Select(t => t.Text));
                                string upperContent = cellContent.ToUpperInvariant();

                                // Verificar se é INÍCIO DA VIGÊNCIA (campo de data)
                                if (upperContent.Contains("INÍCIO DA VIGÊNCIA") ||
                                    upperContent.Contains("INICIO DA VIGENCIA"))
                                {
                                    encontrouInicioVigencia = true;
                                    // Processar APENAS esta célula para o campo de data
                                    applied += ProcessarCampoVigencia(cell, cellTexts, cellContent, dados, "INICIO_VIGENCIA");
                                    break;
                                }

                                // Verificar se é linha de VIGÊNCIA com checkboxes
                                if (upperContent.Contains("VIGÊNCIA:") &&
                                    !upperContent.Contains("INÍCIO") &&
                                    !upperContent.Contains("INICIO"))
                                {
                                    encontrouVigencia = true;

                                    for (int i = cellIndex + 1; i < cells.Count && i <= cellIndex + 3; i++)
                                    {
                                        applied += MarcarCheckboxNaCelula(cells[i]);
                                    }
                                    break;
                                }
                            }

                            if (encontrouVigencia || encontrouInicioVigencia) continue;
                            else continue; // Pular linha se não for vigência
                        }

                        // 3. SE CHEGOU AQUI, ESTAMOS NA SEÇÃO DO TITULAR
                        // Verificar se é linha de VIGÊNCIA (dentro da seção)
                        bool encontrouVigenciaTitular = false;

                        for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
                        {
                            var cell = cells[cellIndex];
                            var cellTexts = cell.Descendants<Text>().ToList();

                            if (cellTexts.Count == 0) continue;

                            string cellContent = string.Join("", cellTexts.Select(t => t.Text));
                            string upperContent = cellContent.ToUpperInvariant();

                            if (upperContent.Contains("VIGÊNCIA:") &&
                                !upperContent.Contains("INÍCIO") &&
                                !upperContent.Contains("INICIO"))
                            {
                                encontrouVigenciaTitular = true;

                                for (int i = cellIndex + 1; i < cells.Count && i <= cellIndex + 3; i++)
                                {
                                    applied += MarcarCheckboxNaCelula(cells[i]);
                                }
                                break;
                            }
                        }

                        if (encontrouVigenciaTitular) continue;

                        // 4. PROCESSAR CÉLULAS NORMAIS DO TITULAR
                        for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
                        {
                            var cell = cells[cellIndex];
                            applied += ProcessarCelulaNormal(cell, dados, rowIndex, rows);
                        }
                    }
                }

                doc.MainDocumentPart.Document.Save();
            }

            return applied;
        }
        private int ProcessarCelulaNormal(
            TableCell cell,
            Dictionary<string, string> dados,
            int rowIndex,
            List<TableRow> rows)
        {
            var cellTexts = cell.Descendants<Text>().ToList();
            if (cellTexts.Count == 0) return 0;

            string cellContent = string.Join("", cellTexts.Select(t => t.Text));
            if (string.IsNullOrWhiteSpace(cellContent)) return 0;

            string upperContent = cellContent.ToUpperInvariant();
            int applied = 0;

            // INÍCIO DA VIGÊNCIA - procurar por "INÍCIO DA VIGÊNCIA"
            if (upperContent.Contains("INÍCIO"))
            {
                applied += ProcessarCampoVigencia(cell, cellTexts, cellContent, dados, "INICIO_VIGENCIA");
            }

            // NOME COMPLETO - procurar por "NOME COMPLETO" inteiro
            if (upperContent.Contains("NOME") && upperContent.Contains("COMPLETO"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados,
                    "NOME_COMPLETO", "NOME COMPLETO");
            }

            // CPF TITULAR - procurar por "CPF TITULAR" inteiro
            if (upperContent.Contains("CPF") && upperContent.Contains("TITULAR"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados,
                    "CPF_TITULAR", "CPF TITULAR");
            }

            // DATA NASCIMENTO - procurar por "DATA NASCIMENTO" inteiro
            if (upperContent.Contains("DATA") && upperContent.Contains("NASCIMENTO"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados,
                    "DATA_NASCIMENTO", "DATA NASCIMENTO");
            }

            // RG (sem ORGÃO) - procurar apenas por "RG"
            if (upperContent.Contains("RG") && !upperContent.Contains("ORGÃO"))
            {
                applied += ProcessarCampoSimples(cell, cellTexts, cellContent, dados, "RG");
            }

            // FILIAÇÃO - procurar por "FILIAÇÃO" (com acento) ou "FILIACAO" (sem acento)
            if (upperContent.Contains("FILIAÇÃO") || upperContent.Contains("FILIACAO"))
            {
                applied += ProcessarCampoFiliacao(cell, cellTexts, cellContent, dados);
            }

            // IDADE - procurar por "IDADE"
            if (upperContent.Contains("IDADE") && !upperContent.Contains("CIDADE"))
            {
                applied += ProcessarCampoIdade(cell, cellTexts, cellContent, dados);
            }

            // SEXO - procurar por "SEXO"
            if (upperContent.Contains("SEXO"))
            {
                applied += ProcessarCampoSexo(cell, cellTexts, cellContent, dados);
            }

            // ESTADO CIVIL - procurar por "ESTADO CIVIL"
            if (upperContent.Contains("ESTADO") && upperContent.Contains("CIVIL"))
            {
                applied += ProcessarCampoEstadoCivil(cell, cellTexts, cellContent, dados);
            }

            // ===== NOVOS CAMPOS =====

            // ORGÃO EXPEDIDOR
            if (upperContent.Contains("ORGÃO") && upperContent.Contains("EXPEDIDOR"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados,
                    "ORGAO_EXPEDIDOR", "ORGÃO EXPEDIDOR");
            }

            // CARTÃO DO SUS
            if (upperContent.Contains("CARTÃO") && upperContent.Contains("SUS"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados,
                    "CARTAO_SUS", "CARTÃO DO SUS");
            }

            // RESPONSÁVEL FINANCEIRO - APENAS se não contiver CPF
            // Verificar se contém RESPONSÁVEL FINANCEIRO mas NÃO contém CPF
            if (upperContent.Contains("RESPONSÁVEL") && upperContent.Contains("FINANCEIRO") && !upperContent.Contains("CPF"))
            {
                string patternResponsavel = @"RESPONSÁVEL\s+FINANCEIRO";
                Match match = Regex.Match(cellContent, patternResponsavel, RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados,
                        "RESPONSAVEL_FINANCEIRO", "RESPONSÁVEL FINANCEIRO");
                }
            }

            // CPF RESPONSÁVEL FINANCEIRO - verificar SEQUÊNCIA EXATA
            if (upperContent.Contains("CPF") && upperContent.Contains("RESPONSÁVEL") && upperContent.Contains("FINANCEIRO"))
            {
                // Verificar a ordem correta das palavras
                string patternCPFResponsavel = @"CPF\s+RESPONSÁVEL\s+FINANCEIRO";
                Match matchCPF = Regex.Match(cellContent, patternCPFResponsavel, RegexOptions.IgnoreCase);

                if (matchCPF.Success)
                {
                    applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados,
                        "CPF_RESPONSAVEL_FINANCEIRO", "CPF RESPONSÁVEL FINANCEIRO");

                    // IMPORTANTE: Retornar aqui para evitar processar o outro campo
                    return applied;
                }
            }


            // Nº DECLARAÇÃO NASCIDO VIVO
            if (upperContent.Contains("DECLARAÇÃO") && upperContent.Contains("NASCIDO") && upperContent.Contains("VIVO"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados,
                    "NUMERO_DECLARACAO_NASCIDO_VIVO", "Nº DECLARAÇÃO NASCIDO VIVO");
            }

            // ENDEREÇO
            if (upperContent.Contains("ENDEREÇO"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados,
                    "ENDERECO", "ENDEREÇO");
            }

            // NÚMERO
            if (upperContent.Contains("NÚMERO") && !upperContent.Contains("TELEFONE")) // Evitar conflito com "NÚMERO" de telefone
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados,
                    "NUMERO", "NÚMERO");
            }

            // COMPLEMENTO
            if (upperContent.Contains("COMPLEMENTO"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados,
                    "COMPLEMENTO", "COMPLEMENTO");
            }

            // CEP (se houver no template)
            if (upperContent.Contains("CEP"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados,
                    "CEP", "CEP");
            }

            // BAIRRO
            if (upperContent.Contains("BAIRRO"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados,
                    "BAIRRO", "BAIRRO");
            }

            // CIDADE
            if (upperContent.Contains("CIDADE"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados,
                    "CIDADE", "CIDADE");
            }

            // UF
            if (upperContent.Contains("UF"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados,
                    "UF", "UF");
            }

            // EMAIL
            if (upperContent.Contains("EMAIL"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados,
                    "EMAIL", "EMAIL");
            }

            // TELEFONE CELULAR
            if (upperContent.Contains("TELEFONE") && upperContent.Contains("CELULAR"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados,
                    "TELEFONE_CELULAR", "TELEFONE CELULAR");
            }

            // TELEFONE FIXO
            if (upperContent.Contains("TELEFONE") && upperContent.Contains("FIXO"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados,
                    "TELEFONE_FIXO", "TELEFONE FIXO");
            }

            return applied;
        }

        private int MarcarCheckboxNaCelula(TableCell cell)
        {
            var textos = cell.Descendants<Text>().ToList();
            if (textos.Count == 0) return 0;

            string conteudoOriginal = string.Join("", textos.Select(t => t.Text));

            // Verificar se já tem checkbox marcado
            if (conteudoOriginal.Contains("☒") || conteudoOriginal.Contains("✓") ||
                conteudoOriginal.Contains("●") || conteudoOriginal.Contains("[X]") ||
                conteudoOriginal.Contains("(X)"))
                return 0;

            bool foiAlterado = false;
            string novoConteudo = conteudoOriginal;

            // Tentar substituir checkboxes não marcados por marcados
            foreach (var checkbox in CheckboxesMap)
            {
                if (novoConteudo.Contains(checkbox.Key))
                {
                    novoConteudo = novoConteudo.Replace(checkbox.Key, checkbox.Value);
                    foiAlterado = true;
                }
            }

            // Se não encontrou nenhum checkbox, adicionar um no início
            if (!foiAlterado && !string.IsNullOrWhiteSpace(novoConteudo))
            {
                // Verificar se o conteúdo parece ser uma opção de dia
                string upperConteudo = novoConteudo.ToUpperInvariant();
                if (upperConteudo.Contains("DIA") || upperConteudo.Contains("(UM)") ||
                    upperConteudo.Contains("(DEZ)") || upperConteudo.Contains("(VINTE)"))
                {
                    novoConteudo = "☒ " + novoConteudo.TrimStart();
                    foiAlterado = true;
                }
            }

            if (!foiAlterado) return 0;

            // Limpar e atualizar
            for (int i = 0; i < textos.Count; i++)
            {
                textos[i].Text = "";
            }

            textos[0].Text = novoConteudo;
            return 1;
        }


        private int ProcessarCampoVigencia(
            TableCell cell,
            List<Text> textos,
            string conteudoCelula,
            Dictionary<string, string> dados,
            string campoKey)
        {
            if (!dados.ContainsKey(campoKey) || textos.Count == 0)
                return 0;

            string valor = dados[campoKey];
            string conteudoUpper = conteudoCelula.ToUpperInvariant();

            // Procurar por padrões relacionados a vigência
            if (conteudoUpper.Contains("VIGÊNCIA") || conteudoUpper.Contains("INÍCIO"))
            {
                // Padrão para encontrar "INÍCIO DA VIGÊNCIA:    /    /"
                string pattern = @"(IN[IÍ]CIO\s+DA\s+VIG[EÊ]NCIA\s*:?\s*)";
                Match match = Regex.Match(conteudoCelula, pattern, RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    int inicioPadrao = match.Index;
                    int fimPadrao = match.Index + match.Length;

                    // Extrair a parte antes do padrão
                    string parteAntes = conteudoCelula.Substring(0, fimPadrao);

                    // Limpar todos os textos
                    for (int i = 0; i < textos.Count; i++)
                    {
                        textos[i].Text = "";
                    }

                    // Criar nova estrutura: rótulo + espaço + valor em negrito
                    // Primeiro, vamos adicionar o rótulo
                    textos[0].Text = parteAntes.TrimEnd() + " ";

                    // Tentar adicionar o valor em negrito e fonte menor
                    var parentRun = textos[0].Parent as Run;
                    if (parentRun != null)
                    {
                        // Criar um novo Run para o valor em negrito
                        Run boldRun = new Run();

                        // Adicionar propriedades de negrito e tamanho de fonte menor
                        RunProperties runProperties = new RunProperties();
                        runProperties.Append(new Bold());
                        runProperties.Append(new FontSize() { Val = "12" });
                        runProperties.Append(new FontSizeComplexScript() { Val = "12" });
                        boldRun.AppendChild(runProperties);

                        // Adicionar o texto do valor
                        Text boldText = new Text(valor);
                        boldRun.AppendChild(boldText);

                        // Adicionar o Run com negrito após o Run original
                        var parentParagraph = parentRun.Parent as Paragraph;
                        if (parentParagraph != null)
                        {
                            parentParagraph.InsertAfter(boldRun, parentRun);
                        }
                        else
                        {
                            // Fallback: colocar tudo no mesmo texto
                            textos[0].Text = parteAntes.TrimEnd() + " " + valor;
                        }
                    }
                    else
                    {
                        // Fallback: adicionar tudo no mesmo texto
                        textos[0].Text = parteAntes.TrimEnd() + " " + valor;
                    }

                    return 1;
                }
            }

            return 0;
        }

        private int ProcessarCampoComRotuloCompleto(
            TableCell cell,
            List<Text> textos,
            string conteudoCelula,
            Dictionary<string, string> dados,
            string campoKey,
            string rotuloCompleto)
        {
            if (!dados.ContainsKey(campoKey) || textos.Count == 0)
                return 0;

            string valor = dados[campoKey];
            string conteudoUpper = conteudoCelula.ToUpperInvariant();
            string rotuloUpper = rotuloCompleto.ToUpperInvariant();

            // Remover espaços para busca (pois no documento pode estar sem espaços)
            string conteudoUpperSemEspacos = conteudoUpper.Replace(" ", "");
            string rotuloUpperSemEspacos = rotuloUpper.Replace(" ", "");

            // Encontrar a posição do rótulo completo (sem espaços) no conteúdo (sem espaços)
            int posRotulo = conteudoUpperSemEspacos.IndexOf(rotuloUpperSemEspacos);

            if (posRotulo >= 0)
            {
                // Usar regex para encontrar o padrão do rótulo (ignorando espaços e case)
                string pattern = Regex.Escape(rotuloCompleto)
                    .Replace("\\ ", "\\s*"); // Substituir espaços por \s* (qualquer quantidade de espaços)

                Match match = Regex.Match(conteudoCelula, pattern, RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    // Encontramos o rótulo completo no texto original
                    int inicioRotulo = match.Index;
                    int fimRotulo = match.Index + match.Length;

                    // Construir novo conteúdo: rótulo + quebra de linha + valor em negrito
                    string parteAntes = conteudoCelula.Substring(0, fimRotulo);

                    // Verificar se já tem algo após o rótulo
                    string resto = conteudoCelula.Substring(fimRotulo);

                    // Limpar todos os textos da célula
                    for (int i = 0; i < textos.Count; i++)
                    {
                        textos[i].Text = "";
                    }

                    // Criar nova estrutura com quebra de linha e negrito
                    // Primeiro, vamos adicionar o rótulo
                    textos[0].Text = parteAntes;

                    // Adicionar quebra de linha após o rótulo
                    // Para isso, precisamos criar um elemento Break no Run pai
                    var parentRun = textos[0].Parent as Run;
                    if (parentRun != null)
                    {
                        // Adicionar quebra de linha
                        parentRun.AppendChild(new Break());

                        // Criar um novo Run para o valor em negrito
                        Run boldRun = new Run();

                        // Adicionar propriedades de negrito E tamanho de fonte menor
                        RunProperties runProperties = new RunProperties();

                        // Negrito
                        runProperties.Append(new Bold());

                        // Tamanho de fonte menor (20 = 10 pontos, o padrão é 24 = 12 pontos)
                        runProperties.Append(new FontSize() { Val = "12" });
                        runProperties.Append(new FontSizeComplexScript() { Val = "12" });

                        boldRun.AppendChild(runProperties);

                        // Adicionar o texto do valor
                        Text boldText = new Text(valor);
                        boldRun.AppendChild(boldText);

                        // Adicionar o Run com negrito após o Run original
                        var parentParagraph = parentRun.Parent as Paragraph;
                        if (parentParagraph != null)
                        {
                            parentParagraph.InsertAfter(boldRun, parentRun);
                        }
                        else
                        {
                            // Se não encontrar parágrafo, apenas colocar o valor após quebra de linha
                            textos[0].Text = parteAntes + "\r\n" + valor;
                        }
                    }
                    else
                    {
                        // Fallback: adicionar quebra de linha simples
                        textos[0].Text = parteAntes + "\r\n" + valor;
                    }

                    return 1;
                }
            }

            return 0;
        }

        // NOVA FUNÇÃO ESPECÍFICA PARA FILIAÇÃO
        private int ProcessarCampoFiliacao(
            TableCell cell,
            List<Text> textos,
            string conteudoCelula,
            Dictionary<string, string> dados)
        {
            if (!dados.ContainsKey("FILIACAO") || textos.Count == 0)
                return 0;

            string valor = dados["FILIACAO"];
            string conteudoUpper = conteudoCelula.ToUpperInvariant();

            // Verificar se é realmente o campo FILIAÇÃO (não confundir com outros campos)
            // Procura por "FILIAÇÃO" ou "FILIACAO" isolado, não como parte de outra palavra
            string pattern = @"\b(FILIAÇÃO|FILIACAO)\b";
            Match match = Regex.Match(conteudoCelula, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                int inicioRotulo = match.Index;
                int fimRotulo = match.Index + match.Length;

                string parteAntes = conteudoCelula.Substring(0, fimRotulo);
                string resto = conteudoCelula.Substring(fimRotulo);

                // Limpar todos os textos
                for (int i = 0; i < textos.Count; i++)
                {
                    textos[i].Text = "";
                }

                // Adicionar rótulo
                textos[0].Text = parteAntes;

                // Tentar adicionar quebra de linha e negrito
                var parentRun = textos[0].Parent as Run;
                if (parentRun != null)
                {
                    // Adicionar quebra de linha
                    parentRun.AppendChild(new Break());

                    // Criar um novo Run para o valor em negrito
                    Run boldRun = new Run();

                    // Adicionar propriedades de negrito e tamanho de fonte menor
                    RunProperties runProperties = new RunProperties();
                    runProperties.Append(new Bold());
                    runProperties.Append(new FontSize() { Val = "12" });
                    runProperties.Append(new FontSizeComplexScript() { Val = "12" });
                    boldRun.AppendChild(runProperties);

                    // Adicionar o texto do valor
                    Text boldText = new Text(valor);
                    boldRun.AppendChild(boldText);

                    // Adicionar o Run com negrito após o Run original
                    var parentParagraph = parentRun.Parent as Paragraph;
                    if (parentParagraph != null)
                    {
                        parentParagraph.InsertAfter(boldRun, parentRun);
                    }
                    else
                    {
                        // Fallback
                        textos[0].Text = parteAntes + "\r\n" + valor;
                    }
                }
                else
                {
                    textos[0].Text = parteAntes + "\r\n" + valor;
                }

                return 1;
            }

            return 0;
        }

        // NOVA FUNÇÃO ESPECÍFICA PARA IDADE
        private int ProcessarCampoIdade(
            TableCell cell,
            List<Text> textos,
            string conteudoCelula,
            Dictionary<string, string> dados)
        {
            if (!dados.ContainsKey("IDADE") || textos.Count == 0)
                return 0;

            string valor = dados["IDADE"];
            string conteudoUpper = conteudoCelula.ToUpperInvariant();

            // Verificar se é realmente o campo IDADE (não confundir com CIDADE)
            // Procura por "IDADE" isolado, não como parte de outra palavra
            string pattern = @"\bIDADE\b";
            Match match = Regex.Match(conteudoCelula, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                int inicioRotulo = match.Index;
                int fimRotulo = match.Index + match.Length;

                string parteAntes = conteudoCelula.Substring(0, fimRotulo);
                string resto = conteudoCelula.Substring(fimRotulo);

                // Limpar todos os textos
                for (int i = 0; i < textos.Count; i++)
                {
                    textos[i].Text = "";
                }

                // Adicionar rótulo
                textos[0].Text = parteAntes;

                // Tentar adicionar quebra de linha e negrito
                var parentRun = textos[0].Parent as Run;
                if (parentRun != null)
                {
                    // Adicionar quebra de linha
                    parentRun.AppendChild(new Break());

                    // Criar um novo Run para o valor em negrito
                    Run boldRun = new Run();

                    // Adicionar propriedades de negrito e tamanho de fonte menor
                    RunProperties runProperties = new RunProperties();
                    runProperties.Append(new Bold());
                    runProperties.Append(new FontSize() { Val = "12" });
                    runProperties.Append(new FontSizeComplexScript() { Val = "12" });
                    boldRun.AppendChild(runProperties);

                    // Adicionar o texto do valor
                    Text boldText = new Text(valor);
                    boldRun.AppendChild(boldText);

                    // Adicionar o Run com negrito após o Run original
                    var parentParagraph = parentRun.Parent as Paragraph;
                    if (parentParagraph != null)
                    {
                        parentParagraph.InsertAfter(boldRun, parentRun);
                    }
                    else
                    {
                        // Fallback
                        textos[0].Text = parteAntes + "\r\n" + valor;
                    }
                }
                else
                {
                    textos[0].Text = parteAntes + "\r\n" + valor;
                }

                return 1;
            }

            return 0;
        }


        private int ProcessarCampoSimples(
            TableCell cell,
            List<Text> textos,
            string conteudoCelula,
            Dictionary<string, string> dados,
            string campoKey)
        {
            if (!dados.ContainsKey(campoKey) || textos.Count == 0)
                return 0;

            string valor = dados[campoKey];

            // Procurar por "RG" mas não "ORGÃO" (case insensitive)
            string pattern = @"\bRG\b(?![A-ZÀ-Ú]*ÃO)"; // RG que não seja seguido por palavra terminando em "ÃO"
            Match match = Regex.Match(conteudoCelula, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                int inicioRotulo = match.Index;
                int fimRotulo = match.Index + match.Length;

                string parteAntes = conteudoCelula.Substring(0, fimRotulo);
                string resto = conteudoCelula.Substring(fimRotulo);

                // Limpar todos os textos
                for (int i = 0; i < textos.Count; i++)
                {
                    textos[i].Text = "";
                }

                // Adicionar rótulo
                textos[0].Text = parteAntes;

                // Tentar adicionar quebra de linha e negrito
                var parentRun = textos[0].Parent as Run;
                if (parentRun != null)
                {
                    // Adicionar quebra de linha
                    parentRun.AppendChild(new Break());

                    // Criar um novo Run para o valor em negrito
                    Run boldRun = new Run();

                    // Adicionar propriedades de negrito e tamanho de fonte menor
                    RunProperties runProperties = new RunProperties();
                    runProperties.Append(new Bold());
                    runProperties.Append(new FontSize() { Val = "12" });
                    runProperties.Append(new FontSizeComplexScript() { Val = "12" });
                    boldRun.AppendChild(runProperties);

                    // Adicionar o texto do valor
                    Text boldText = new Text(valor);
                    boldRun.AppendChild(boldText);

                    // Adicionar o Run com negrito após o Run original
                    var parentParagraph = parentRun.Parent as Paragraph;
                    if (parentParagraph != null)
                    {
                        parentParagraph.InsertAfter(boldRun, parentRun);
                    }
                    else
                    {
                        // Fallback
                        textos[0].Text = parteAntes + "\r\n" + valor;
                    }
                }
                else
                {
                    textos[0].Text = parteAntes + "\r\n" + valor;
                }

                return 1;
            }

            return 0;
        }
        private int ProcessarCampoSexo(
      TableCell cell,
      List<Text> textos,
      string conteudoCelula,
      Dictionary<string, string> dados)
        {
            if (!dados.ContainsKey("SEXO") || textos.Count == 0)
                return 0;

            string valorSexo = dados["SEXO"].ToUpperInvariant(); // "M", "F" ou "OUTROS"

            // Juntar todo o conteúdo da célula
            string conteudoOriginal = string.Join("", textos.Select(t => t.Text));

            // Primeiro, NORMALIZAR o conteúdo para facilitar a manipulação
            string conteudoNormalizado = conteudoOriginal
                .Replace("SEXOF", "SEXO")  // Corrigir "SEXOF" para "SEXO"
                .Replace("SEX0", "SEXO")   // Corrigir possível erro de digitação (zero em vez de O)
                .Trim();

            // Construir o novo conteúdo baseado no valor
            string opcoes = "";

            // Determinar as opções marcadas
            switch (valorSexo)
            {
                case "M":
                    opcoes = "☒ M   ☐ F   ☐ OUTROS";
                    break;
                case "F":
                    opcoes = "☐ M   ☒ F   ☐ OUTROS";
                    break;
                case "OUTROS":
                    opcoes = "☐ M   ☐ F   ☒ OUTROS";
                    break;
                default:
                    return 0;
            }

            // Verificar se o conteúdo já está correto (com quebra de linha)
            string conteudoEsperado = "SEXO:\n" + opcoes;
            if (conteudoOriginal.Replace("\r\n", "\n").Trim() == conteudoEsperado.Replace("\r\n", "\n").Trim())
                return 0;

            // Limpar todos os textos da célula
            cell.RemoveAllChildren<Paragraph>();

            // Criar um novo parágrafo
            Paragraph newParagraph = new Paragraph();

            // 1. PRIMEIRO RUN: Rótulo "SEXO:" SEM NEGRITO
            Run runRotulo = new Run();
            RunProperties runPropertiesRotulo = new RunProperties();
            // Apenas tamanho padrão, SEM negrito
            runPropertiesRotulo.Append(new FontSize() { Val = "12" });
            runPropertiesRotulo.Append(new FontSizeComplexScript() { Val = "12" });
            runRotulo.AppendChild(runPropertiesRotulo);
            runRotulo.AppendChild(new Text("SEXO:"));

            // Adicionar quebra de linha após o rótulo
            runRotulo.AppendChild(new Break());

            // 2. SEGUNDO RUN: Opções dos checkboxes COM NEGRITO
            Run runOpcoes = new Run();
            RunProperties runPropertiesOpcoes = new RunProperties();

            // NEGRITO para as opções
            runPropertiesOpcoes.Append(new Bold());
            runPropertiesOpcoes.Append(new FontSize() { Val = "12" });
            runPropertiesOpcoes.Append(new FontSizeComplexScript() { Val = "12" });

            runOpcoes.AppendChild(runPropertiesOpcoes);
            runOpcoes.AppendChild(new Text(opcoes));

            // Adicionar os runs ao parágrafo
            newParagraph.AppendChild(runRotulo);
            newParagraph.AppendChild(runOpcoes);

            // Adicionar o parágrafo à célula
            cell.AppendChild(newParagraph);

            return 1;
        }

        // NOVA FUNÇÃO PARA ESTADO CIVIL
        private int ProcessarCampoEstadoCivil(
     TableCell cell,
     List<Text> textos,
     string conteudoCelula,
     Dictionary<string, string> dados)
        {
            if (!dados.ContainsKey("ESTADO_CIVIL") || textos.Count == 0)
                return 0;

            string valorEstadoCivil = dados["ESTADO_CIVIL"].ToUpperInvariant(); // "SOLTEIRO", "CASADO", "OUTROS"

            // Juntar todo o conteúdo da célula
            string conteudoOriginal = string.Join("", textos.Select(t => t.Text));
            string conteudoUpper = conteudoOriginal.ToUpperInvariant();

            // Verificar se a célula contém "ESTADO CIVIL"
            if (!conteudoUpper.Contains("ESTADO") || !conteudoUpper.Contains("CIVIL"))
                return 0;

            // Construir a string de checkboxes
            string opcoes = "";
            switch (valorEstadoCivil)
            {
                case "SOLTEIRO":
                    opcoes = "☒ SOLTEIRO   ☐ CASADO   ☐ OUTROS";
                    break;
                case "CASADO":
                    opcoes = "☐ SOLTEIRO   ☒ CASADO   ☐ OUTROS";
                    break;
                case "OUTROS":
                    opcoes = "☐ SOLTEIRO   ☐ CASADO   ☒ OUTROS";
                    break;
                default:
                    return 0; // Valor não reconhecido
            }

            // Verificar se o conteúdo já está correto (com quebra de linha)
            string conteudoEsperado = "ESTADO CIVIL\n" + opcoes;
            if (conteudoOriginal.Replace("\r\n", "\n").Trim() == conteudoEsperado.Replace("\r\n", "\n").Trim())
                return 0;

            // Limpar todos os textos da célula
            cell.RemoveAllChildren<Paragraph>();

            // Criar um novo parágrafo
            Paragraph newParagraph = new Paragraph();

            // 1. PRIMEIRO RUN: Rótulo "ESTADO CIVIL" SEM NEGRITO
            Run runRotulo = new Run();
            RunProperties runPropertiesRotulo = new RunProperties();
            // Apenas tamanho padrão, SEM negrito
            runPropertiesRotulo.Append(new FontSize() { Val = "12" });
            runPropertiesRotulo.Append(new FontSizeComplexScript() { Val = "12" });
            runRotulo.AppendChild(runPropertiesRotulo);
            runRotulo.AppendChild(new Text("ESTADO CIVIL"));

            // Adicionar quebra de linha após o rótulo
            runRotulo.AppendChild(new Break());

            // 2. SEGUNDO RUN: Opções dos checkboxes COM NEGRITO
            Run runOpcoes = new Run();
            RunProperties runPropertiesOpcoes = new RunProperties();

            // NEGRITO para as opções
            runPropertiesOpcoes.Append(new Bold());
            runPropertiesOpcoes.Append(new FontSize() { Val = "12" });
            runPropertiesOpcoes.Append(new FontSizeComplexScript() { Val = "12" });

            runOpcoes.AppendChild(runPropertiesOpcoes);
            runOpcoes.AppendChild(new Text(opcoes));

            // Adicionar os runs ao parágrafo
            newParagraph.AppendChild(runRotulo);
            newParagraph.AppendChild(runOpcoes);

            // Adicionar o parágrafo à célula
            cell.AppendChild(newParagraph);

            return 1;
        }

        // NOVA FUNÇÃO ESPECÍFICA PARA DEPENDENTES
        public int FillDependentes(
            string documentoPreenchidoPath,
            string outputPath,
            List<Dictionary<string, string>> listaDependentes)
        {
            // NÃO copia o arquivo - já existe
            int applied = 0;

            using (var doc = WordprocessingDocument.Open(documentoPreenchidoPath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;

                // 1. PROCURAR ESTRUTURA DE TABELA
                var tables = body.Descendants<Table>().ToList();

                // Controle de qual dependente estamos processando
                int dependenteAtual = -1;
                bool emSecaoDependente = false;
                bool encontrouPrimeiroDependente = false;

                foreach (var table in tables)
                {
                    var rows = table.Descendants<TableRow>().ToList();

                    for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
                    {
                        var cells = rows[rowIndex].Descendants<TableCell>().ToList();
                        bool linhaTemCabecalhoDependente = false;

                        // PRIMEIRO: Verificar se esta linha contém início de uma seção de dependente
                        for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
                        {
                            var cell = cells[cellIndex];
                            var cellTexts = cell.Descendants<Text>().ToList();

                            if (cellTexts.Count == 0) continue;

                            string cellContent = string.Join("", cellTexts.Select(t => t.Text));
                            if (string.IsNullOrWhiteSpace(cellContent)) continue;

                            string upperContent = cellContent.ToUpperInvariant();

                            // Verificar se é uma linha de DEPENDENTE
                            if (upperContent.Contains("DEPENDENTE"))
                            {
                                // Tentar extrair o número do dependente
                                Match match = Regex.Match(upperContent, @"DEPENDENTE\s+(\d+)");
                                if (match.Success)
                                {
                                    int numeroDependente = int.Parse(match.Groups[1].Value);

                                    // Verificar se temos dados para este dependente específico
                                    if (numeroDependente >= 1 && numeroDependente <= listaDependentes.Count)
                                    {
                                        dependenteAtual = numeroDependente - 1; // Índice 0-based
                                        emSecaoDependente = true;
                                        encontrouPrimeiroDependente = true;
                                        linhaTemCabecalhoDependente = true;

                                        // DEBUG: Mostrar qual dependente estamos processando
                                        Console.WriteLine($"Processando DEPENDENTE {numeroDependente}");

                                        // NÃO BREAK! Continue processando outras células nesta linha
                                        // porque "NOME COMPLETO" e "CPF" estão na MESMA LINHA!
                                    }
                                    else
                                    {
                                        // Não temos dados para este dependente, desativar a seção
                                        emSecaoDependente = false;
                                        linhaTemCabecalhoDependente = true;
                                    }
                                }
                                else if (listaDependentes.Count >= 1 && !encontrouPrimeiroDependente)
                                {
                                    // Se não tem número e é o primeiro dependente encontrado
                                    dependenteAtual = 0;
                                    emSecaoDependente = true;
                                    encontrouPrimeiroDependente = true;
                                    linhaTemCabecalhoDependente = true;
                                    Console.WriteLine($"Processando DEPENDENTE 1 (sem número)");

                                    // NÃO BREAK! Continue processando outras células nesta linha
                                }
                            }
                        }

                        // IMPORTANTE: NÃO PULAR A LINHA INTEIRA!
                        // Mesmo se tem "DEPENDENTE X" na linha, ainda temos que processar
                        // os outros campos que estão na MESMA LINHA (NOME COMPLETO, CPF, etc.)

                        // Se não estamos em uma seção de dependente, pular esta linha
                        if (!emSecaoDependente) continue;

                        // Se não temos dados para este dependente, continuar
                        if (dependenteAtual < 0 || dependenteAtual >= listaDependentes.Count)
                        {
                            emSecaoDependente = false; // Sair da seção se não temos dados
                            continue;
                        }

                        // Processar as células desta linha para o dependente atual
                        // Mesmo se a linha tem "DEPENDENTE X", processe as outras células!
                        for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
                        {
                            var cell = cells[cellIndex];
                            var dadosDependente = listaDependentes[dependenteAtual];
                            applied += ProcessarCelulaDependente(cell, dadosDependente, rowIndex, rows);
                        }
                    }
                }

                doc.MainDocumentPart.Document.Save();
            }

            return applied;
        }


        private int ProcessarCelulaDependente(
            TableCell cell,
            Dictionary<string, string> dadosDependente,
            int rowIndex,
            List<TableRow> rows)
        {
            var cellTexts = cell.Descendants<Text>().ToList();
            if (cellTexts.Count == 0) return 0;

            string cellContent = string.Join("", cellTexts.Select(t => t.Text));
            if (string.IsNullOrWhiteSpace(cellContent)) return 0;

            string upperContent = cellContent.ToUpperInvariant();
            int applied = 0;

            // NOME COMPLETO DO DEPENDENTE
            if (upperContent.Contains("NOME") && upperContent.Contains("COMPLETO"))
            {
                // Verificar se o dependente atual tem nome completo
                if (dadosDependente.ContainsKey("NOME_COMPLETO"))
                {
                    applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent,
                        dadosDependente, "NOME_COMPLETO", "NOME COMPLETO");
                }
            }

            // DATA NASCIMENTO DO DEPENDENTE
            if (upperContent.Contains("DATA") && upperContent.Contains("NASCIMENTO"))
            {
                if (dadosDependente.ContainsKey("DATA_NASCIMENTO"))
                {
                    applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent,
                        dadosDependente, "DATA_NASCIMENTO", "DATA NASCIMENTO");
                }
            }

            // RG DO DEPENDENTE
            if (upperContent.Contains("RG") && !upperContent.Contains("ORGÃO"))
            {
                if (dadosDependente.ContainsKey("RG"))
                {
                    applied += ProcessarCampoSimples(cell, cellTexts, cellContent,
                        dadosDependente, "RG");
                }
            }

            // ORGÃO EXPEDIDOR DO DEPENDENTE
            if (upperContent.Contains("ORGÃO") && upperContent.Contains("EXPEDIDOR"))
            {
                if (dadosDependente.ContainsKey("ORGAO_EXPEDIDOR"))
                {
                    applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent,
                        dadosDependente, "ORGAO_EXPEDIDOR", "ORGÃO EXPEDIDOR");
                }
            }

            // SEXO DO DEPENDENTE
            if (upperContent.Contains("SEXO"))
            {
                if (dadosDependente.ContainsKey("SEXO"))
                {
                    applied += ProcessarCampoSexo(cell, cellTexts, cellContent, dadosDependente);
                }
            }

            // ESTADO CIVIL DO DEPENDENTE
            if (upperContent.Contains("ESTADO") && upperContent.Contains("CIVIL"))
            {
                if (dadosDependente.ContainsKey("ESTADO_CIVIL"))
                {
                    applied += ProcessarCampoEstadoCivil(cell, cellTexts, cellContent, dadosDependente);
                }
            }

            // CARTÃO DO SUS DO DEPENDENTE
            if (upperContent.Contains("CARTÃO") && upperContent.Contains("SUS"))
            {
                if (dadosDependente.ContainsKey("CARTAO_SUS"))
                {
                    applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent,
                        dadosDependente, "CARTAO_SUS", "CARTÃO DO SUS");
                }
            }

            // Nº DECLARAÇÃO NASCIDO VIVO DO DEPENDENTE
            if (upperContent.Contains("DECLARAÇÃO") && upperContent.Contains("NASCIDO") && upperContent.Contains("VIVO"))
            {
                if (dadosDependente.ContainsKey("NUMERO_DECLARACAO_NASCIDO_VIVO"))
                {
                    applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent,
                        dadosDependente, "NUMERO_DECLARACAO_NASCIDO_VIVO", "Nº DECLARAÇÃO NASCIDO VIVO");
                }
            }

            // CPF DO DEPENDENTE
            if (upperContent.Contains("CPF") &&
                !upperContent.Contains("TITULAR") &&
                !upperContent.Contains("RESPONSÁVEL") &&
                !upperContent.Contains("FINANCEIRO"))
            {
                if (dadosDependente.ContainsKey("CPF"))
                {
                    applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent,
                        dadosDependente, "CPF", "CPF");
                }
            }

            // PARENTESCO DO DEPENDENTE
            if (upperContent.Contains("PARENTESCO"))
            {
                if (dadosDependente.ContainsKey("PARENTESCO"))
                {
                    applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent,
                        dadosDependente, "PARENTESCO", "PARENTESCO");
                }
            }

            // FILIAÇÃO DO DEPENDENTE
            if (upperContent.Contains("FILIAÇÃO") || upperContent.Contains("FILIACAO"))
            {
                if (dadosDependente.ContainsKey("FILIACAO"))
                {
                    applied += ProcessarCampoFiliacao(cell, cellTexts, cellContent, dadosDependente);
                }
            }

            // IDADE DO DEPENDENTE
            if (upperContent.Contains("IDADE") && !upperContent.Contains("CIDADE"))
            {
                if (dadosDependente.ContainsKey("IDADE"))
                {
                    applied += ProcessarCampoIdade(cell, cellTexts, cellContent, dadosDependente);
                }
            }

            // ENDEREÇO DO DEPENDENTE
            if (upperContent.Contains("ENDEREÇO"))
            {
                if (dadosDependente.ContainsKey("ENDERECO"))
                {
                    applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent,
                        dadosDependente, "ENDERECO", "ENDEREÇO");
                }
            }

            // BAIRRO DO DEPENDENTE
            if (upperContent.Contains("BAIRRO"))
            {
                if (dadosDependente.ContainsKey("BAIRRO"))
                {
                    applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent,
                        dadosDependente, "BAIRRO", "BAIRRO");
                }
            }

            // CIDADE DO DEPENDENTE
            if (upperContent.Contains("CIDADE"))
            {
                if (dadosDependente.ContainsKey("CIDADE"))
                {
                    applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent,
                        dadosDependente, "CIDADE", "CIDADE");
                }
            }

            // UF DO DEPENDENTE
            if (upperContent.Contains("UF"))
            {
                if (dadosDependente.ContainsKey("UF"))
                {
                    applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent,
                        dadosDependente, "UF", "UF");
                }
            }

            // EMAIL DO DEPENDENTE
            if (upperContent.Contains("EMAIL"))
            {
                if (dadosDependente.ContainsKey("EMAIL"))
                {
                    applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent,
                        dadosDependente, "EMAIL", "EMAIL");
                }
            }

            // TELEFONE CELULAR DO DEPENDENTE
            if (upperContent.Contains("TELEFONE") && upperContent.Contains("CELULAR"))
            {
                if (dadosDependente.ContainsKey("TELEFONE_CELULAR"))
                {
                    applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent,
                        dadosDependente, "TELEFONE_CELULAR", "TELEFONE CELULAR");
                }
            }

            // TELEFONE FIXO DO DEPENDENTE
            if (upperContent.Contains("TELEFONE") && upperContent.Contains("FIXO"))
            {
                if (dadosDependente.ContainsKey("TELEFONE_FIXO"))
                {
                    applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent,
                        dadosDependente, "TELEFONE_FIXO", "TELEFONE FIXO");
                }
            }

            return applied;
        }

        public int FillPlanoCoparticipacao(string documentoPreenchidoPath, string tipoCoparticipacao)
        {
            int applied = 0;
            bool planoEncontrado = false;

            using (var doc = WordprocessingDocument.Open(documentoPreenchidoPath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;

                // 1. ENCONTRAR A TABELA DO ADITIVO DE PRODUTO
                var tables = body.Descendants<Table>().ToList();

                foreach (var table in tables)
                {
                    var rows = table.Descendants<TableRow>().ToList();

                    for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
                    {
                        var cells = rows[rowIndex].Descendants<TableCell>().ToList();

                        // Verificar se esta linha contém o cabeçalho "ASSINALE ABAIXO O PLANO PRETENDIDO"
                        bool encontrouCabecalho = false;

                        for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
                        {
                            var cell = cells[cellIndex];
                            var cellTexts = cell.Descendants<Text>().ToList();

                            if (cellTexts.Count == 0) continue;

                            string cellContent = string.Join("", cellTexts.Select(t => t.Text));
                            string upperContent = cellContent.ToUpperInvariant();

                            // Se encontrar o cabeçalho, processar as próximas linhas (planos)
                            if (upperContent.Contains("ASSINALE ABAIXO O PLANO PRETENDIDO"))
                            {
                                encontrouCabecalho = true;
                                Console.WriteLine($"Encontrou cabeçalho do plano na linha {rowIndex}");

                                // Processar as próximas linhas (planos)
                                // Vamos começar da próxima linha (rowIndex + 1)
                                for (int i = rowIndex + 1; i < rows.Count && i <= rowIndex + 10; i++) // Até 10 linhas de planos (para garantir)
                                {
                                    var planoCells = rows[i].Descendants<TableCell>().ToList();

                                    // Verificar se é uma linha de plano (deve ter células suficientes)
                                    if (planoCells.Count >= 5)
                                    {
                                        // A última célula contém o tipo de coparticipação
                                        var ultimaCelula = planoCells[4];
                                        var ultimaCelulaTexts = ultimaCelula.Descendants<Text>().ToList();

                                        if (ultimaCelulaTexts.Count > 0)
                                        {
                                            string coparticipacaoTexto = string.Join("", ultimaCelulaTexts.Select(t => t.Text));
                                            string coparticipacaoUpper = coparticipacaoTexto.ToUpperInvariant().Trim();

                                            Console.WriteLine($"Texto da coparticipação na linha {i}: '{coparticipacaoUpper}'");
                                            Console.WriteLine($"Procurando por: '{tipoCoparticipacao.ToUpperInvariant()}'");

                                            // Verificar se esta linha corresponde ao tipo de coparticipação desejado
                                            // Vamos verificar de forma mais precisa
                                            bool corresponde = false;

                                            if (tipoCoparticipacao.ToUpperInvariant() == "PARCIAL")
                                            {
                                                corresponde = coparticipacaoUpper.Contains("PARCIAL") &&
                                                             !coparticipacaoUpper.Contains("SEM COPARTICIPAÇÃO") &&
                                                             !coparticipacaoUpper.Contains("TOTAL");
                                            }
                                            else if (tipoCoparticipacao.ToUpperInvariant() == "TOTAL")
                                            {
                                                corresponde = coparticipacaoUpper.Contains("TOTAL") &&
                                                             !coparticipacaoUpper.Contains("SEM COPARTICIPAÇÃO") &&
                                                             !coparticipacaoUpper.Contains("PARCIAL");
                                            }
                                            else if (tipoCoparticipacao.ToUpperInvariant() == "SEM COPARTICIPAÇÃO" ||
                                                     tipoCoparticipacao.ToUpperInvariant() == "SEM COPARTICIPACAO")
                                            {
                                                corresponde = coparticipacaoUpper.Contains("SEM COPARTICIPAÇÃO") ||
                                                              coparticipacaoUpper.Contains("SEM COPARTICIPACAO");
                                            }

                                            if (corresponde)
                                            {
                                                Console.WriteLine($"ENCONTRADO! Marcando checkbox na linha {i}");

                                                // Marcar o checkbox na primeira célula desta linha
                                                var primeiraCelula = planoCells[0];

                                                // TESTE EXPLÍCITO: Forçar marcação do checkbox
                                                applied += MarcarCheckboxExplicitamente(primeiraCelula);

                                                planoEncontrado = true;

                                                // Sair do loop após marcar o plano correto
                                                break;
                                            }
                                        }
                                    }
                                }

                                break;
                            }
                        }

                        if (encontrouCabecalho) break;
                    }
                }

                if (!planoEncontrado)
                {
                    Console.WriteLine($"AVISO: Não foi encontrado plano com coparticipação '{tipoCoparticipacao}'");
                }

                doc.MainDocumentPart.Document.Save();
            }

            return applied;
        }

        // NOVA FUNÇÃO PARA MARCAR CHECKBOX EXPLICITAMENTE
        // FUNÇÃO CORRIGIDA PARA MARCAR CHECKBOX MANTENDO FORMATAÇÃO
        private int MarcarCheckboxExplicitamente(TableCell cell)
        {
            int applied = 0;

            // Primeiro, procurar por qualquer texto que possa ser um checkbox
            var texts = cell.Descendants<Text>().ToList();

            // Se não tiver textos, adicionar um checkbox
            if (texts.Count == 0)
            {
                var paragraph = cell.Elements<Paragraph>().FirstOrDefault();
                if (paragraph != null)
                {
                    // Criar um novo run com o checkbox marcado
                    var run = new Run();
                    run.Append(new Text("☑"));
                    paragraph.Append(run);
                    applied++;
                    Console.WriteLine("Checkbox adicionado na célula vazia");
                }
                return applied;
            }

            // Procurar por caracteres de checkbox e substituir
            bool encontrouCheckbox = false;

            foreach (var text in texts)
            {
                string conteudo = text.Text;

                // Verificar se tem algum caractere de checkbox
                if (conteudo.Contains("☐") || conteudo.Contains("□") ||
                    conteudo.Contains("[ ]") || conteudo.Contains("( )") ||
                    conteudo.Trim() == "")
                {
                    // Substituir por checkbox marcado mantendo o restante do texto se houver
                    if (conteudo.Contains("☐"))
                    {
                        text.Text = conteudo.Replace("☐", "☑");
                        encontrouCheckbox = true;
                    }
                    else if (conteudo.Contains("□"))
                    {
                        text.Text = conteudo.Replace("□", "☑");
                        encontrouCheckbox = true;
                    }
                    else if (conteudo.Contains("[ ]"))
                    {
                        text.Text = conteudo.Replace("[ ]", "[✓]");
                        encontrouCheckbox = true;
                    }
                    else if (conteudo.Contains("( )"))
                    {
                        text.Text = conteudo.Replace("( )", "(✓)");
                        encontrouCheckbox = true;
                    }
                    else if (string.IsNullOrWhiteSpace(conteudo))
                    {
                        text.Text = "☑";
                        encontrouCheckbox = true;
                    }

                    if (encontrouCheckbox)
                    {
                        applied++;
                        Console.WriteLine($"Checkbox substituído: '{conteudo}' -> '{text.Text}'");
                        break;
                    }
                }
            }

            // Se não encontrou nenhum checkbox explícito, adicionar um no início
            if (!encontrouCheckbox && texts.Count > 0)
            {
                // Verificar se o primeiro texto tem conteúdo
                var primeiroTexto = texts[0];
                if (string.IsNullOrWhiteSpace(primeiroTexto.Text))
                {
                    primeiroTexto.Text = "☑";
                }
                else
                {
                    // Adicionar um checkbox antes do texto existente
                    primeiroTexto.Text = "☑ " + primeiroTexto.Text;
                }
                applied++;
                Console.WriteLine("Checkbox adicionado no início da célula");
            }

            return applied;
        }
    }
}