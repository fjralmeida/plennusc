using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Plennusc.Core.Service.ServiceGestao.serviceYouBut
{
    public class EnterpriseDocx
    {
        private static readonly Dictionary<string, string> CheckboxesMap = new Dictionary<string, string>
        {
            { "☐", "☒" },
            { "☑", "☒" },
            { "□", "☒" },
            { "◻", "☒" },
            { "○", "●" },
            { "[ ]", "[X]" },
            { "( )", "(X)" },
            { "(_)", "(X)" }
        };

        // ==================== MÉTODOS PÚBLICOS ====================

        public int FillTitularBasico(
          string templatePath,
          string outputPath,
          Dictionary<string, string> dados,
          int indiceBloco = 0)
        {
            if (templatePath != outputPath && !File.Exists(outputPath))
                File.Copy(templatePath, outputPath, true);

            int applied = 0;

            using (var doc = WordprocessingDocument.Open(outputPath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;
                var tables = body.Descendants<Table>().ToList();

                int blocoAtual = -1;
                bool emSecaoTitular = false;
                bool processandoBloco = false;

                foreach (var table in tables)
                {
                    var rows = table.Descendants<TableRow>().ToList();

                    for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
                    {
                        var cells = rows[rowIndex].Descendants<TableCell>().ToList();

                        // Detectar início de novo bloco
                        for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
                        {
                            var cell = cells[cellIndex];
                            var cellTexts = cell.Descendants<Text>().ToList();
                            if (cellTexts.Count == 0) continue;

                            string cellContent = string.Join("", cellTexts.Select(t => t.Text));
                            string upperContent = cellContent.ToUpperInvariant();

                            if (upperContent.Contains("BENEFICIÁRIO TITULAR"))
                            {
                                blocoAtual++;
                                emSecaoTitular = true;
                                processandoBloco = (blocoAtual == indiceBloco);
                                break;
                            }
                        }

                        // Permite processar linhas antes do primeiro bloco apenas quando indiceBloco == 0
                        if (!processandoBloco && !(blocoAtual == -1 && indiceBloco == 0))
                            continue;

                        // Verificar se é linha de cabeçalho de seção
                        bool linhaEhCabecalho = false;
                        for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
                        {
                            var cell = cells[cellIndex];
                            var cellTexts = cell.Descendants<Text>().ToList();
                            if (cellTexts.Count == 0) continue;

                            string cellContent = string.Join("", cellTexts.Select(t => t.Text));
                            string upperContent = cellContent.ToUpperInvariant();

                            if (upperContent.Contains("DADOS") && upperContent.Contains("TITULAR"))
                            {
                                emSecaoTitular = true;
                                linhaEhCabecalho = true;
                                break;
                            }

                            if (upperContent.Contains("DEPENDENTE"))
                            {
                                emSecaoTitular = false;
                                linhaEhCabecalho = true;
                                break;
                            }
                        }

                        if (linhaEhCabecalho) continue;

                        // Fora da seção do titular – processa vigência apenas antes do primeiro bloco
                        if (!emSecaoTitular)
                        {
                            // Só processa vigência se estivermos antes do primeiro bloco (blocoAtual == -1)
                            if (blocoAtual == -1)
                            {
                                bool encontrouVigencia = false;
                                bool encontrouInicioVigencia = false;

                                for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
                                {
                                    var cell = cells[cellIndex];
                                    var cellTexts = cell.Descendants<Text>().ToList();
                                    if (cellTexts.Count == 0) continue;

                                    string cellContent = string.Join("", cellTexts.Select(t => t.Text));
                                    string upperContent = cellContent.ToUpperInvariant();

                                    if (upperContent.Contains("INÍCIO DA VIGÊNCIA") ||
                                        upperContent.Contains("INICIO DA VIGENCIA"))
                                    {
                                        encontrouInicioVigencia = true;
                                        applied += ProcessarCampoVigencia(cell, cellTexts, cellContent, dados, "INICIO_VIGENCIA");
                                        break;
                                    }

                                    if (upperContent.Contains("VIGÊNCIA:") &&
                                        !upperContent.Contains("INÍCIO") &&
                                        !upperContent.Contains("INICIO"))
                                    {
                                        encontrouVigencia = true;
                                        string[] chaves = { "VENCIMENTO_DIA_01", "VENCIMENTO_DIA_10", "VENCIMENTO_DIA_20" };
                                        for (int j = 0; j < 3; j++)
                                        {
                                            int celulaIndex = cellIndex + 1 + j;
                                            if (celulaIndex < cells.Count &&
                                                dados.ContainsKey(chaves[j]) &&
                                                dados[chaves[j]] == "true")
                                            {
                                                applied += MarcarCheckboxNaCelula(cells[celulaIndex]);
                                            }
                                        }
                                        break;
                                    }
                                }

                                if (encontrouVigencia || encontrouInicioVigencia) continue;
                            }

                            continue; // pula linha se não for vigência
                        }

                        // Dentro da seção do titular – vigência interna (se houver)
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

                        // Processar células normais do titular
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
        public int FillDependentes(
            string templatePath,
            string outputPath,
            List<Dictionary<string, string>> dependentes,
            int indiceBloco = 0)
        {
            if (templatePath != outputPath && !File.Exists(outputPath))
                File.Copy(templatePath, outputPath, true);

            int applied = 0;

            using (var doc = WordprocessingDocument.Open(outputPath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;
                var tables = body.Descendants<Table>().ToList();

                int blocoAtual = -1;
                bool processandoBloco = false;
                bool emSecaoDependentes = false;
                int dependenteIndex = -1; // -1 significa que ainda não começamos o primeiro dependente

                foreach (var table in tables)
                {
                    var rows = table.Descendants<TableRow>().ToList();

                    for (int rowIndex = 0; rowIndex < rows.Count; rowIndex++)
                    {
                        var cells = rows[rowIndex].Descendants<TableCell>().ToList();

                        // Detectar início de bloco de titular
                        for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
                        {
                            var cell = cells[cellIndex];
                            var cellTexts = cell.Descendants<Text>().ToList();
                            if (cellTexts.Count == 0) continue;

                            string cellContent = string.Join("", cellTexts.Select(t => t.Text));
                            string upperContent = cellContent.ToUpperInvariant();

                            if (upperContent.Contains("BENEFICIÁRIO TITULAR"))
                            {
                                blocoAtual++;
                                processandoBloco = (blocoAtual == indiceBloco);
                                emSecaoDependentes = false;
                                dependenteIndex = -1;
                                break;
                            }
                        }

                        if (!processandoBloco) continue;

                        // Dentro do bloco correto, verificar se entramos na seção de dependentes
                        for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
                        {
                            var cell = cells[cellIndex];
                            var cellTexts = cell.Descendants<Text>().ToList();
                            if (cellTexts.Count == 0) continue;

                            string cellContent = string.Join("", cellTexts.Select(t => t.Text));
                            string upperContent = cellContent.ToUpperInvariant();

                            if (upperContent.Contains("BENEFICIÁRIOS DEPENDENTES"))
                            {
                                emSecaoDependentes = true;
                                dependenteIndex = -1;
                                break;
                            }
                        }

                        if (!emSecaoDependentes) continue;

                        // Agora estamos na seção de dependentes: percorrer todas as células
                        for (int cellIndex = 0; cellIndex < cells.Count; cellIndex++)
                        {
                            var cell = cells[cellIndex];
                            var cellTexts = cell.Descendants<Text>().ToList();
                            if (cellTexts.Count == 0) continue;

                            string cellContent = string.Join("", cellTexts.Select(t => t.Text));
                            string upperContent = cellContent.ToUpperInvariant();

                            // Verifica se é um campo que indica início de novo dependente
                            if (upperContent.Contains("NOME") && upperContent.Contains("COMPLETO"))
                            {
                                // Se ainda não começamos, vamos para o primeiro dependente
                                if (dependenteIndex == -1)
                                {
                                    if (dependentes.Count > 0)
                                        dependenteIndex = 0;
                                    else
                                        dependenteIndex = dependentes.Count; // sem dependentes, índice inválido
                                }
                                else if (dependenteIndex < dependentes.Count - 1)
                                {
                                    // Avança para o próximo dependente
                                    dependenteIndex++;
                                }
                                else
                                {
                                    // Já processamos todos os dependentes, definimos índice como inválido para ignorar o resto
                                    dependenteIndex = dependentes.Count; // ou um valor fora do intervalo
                                }
                            }

                            // Se não temos um dependente ativo, não processamos
                            if (dependenteIndex < 0 || dependenteIndex >= dependentes.Count)
                                continue;

                            // Processa a célula com o dependente atual
                            var dadosDep = dependentes[dependenteIndex];
                            applied += ProcessarCampoGenerico(cell, cellTexts, cellContent, dadosDep, upperContent);
                        }
                    }
                }

                doc.MainDocumentPart.Document.Save();
            }

            return applied;
        }
        // Função auxiliar que chama os métodos de campo apropriados com base no conteúdo da célula
        private int ProcessarCampoGenerico(TableCell cell, List<Text> cellTexts, string cellContent, Dictionary<string, string> dadosDep, string upperContent)
        {
            int applied = 0;

            if (upperContent.Contains("NOME COMPLETO"))
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosDep, "NOME_COMPLETO", "NOME COMPLETO");

            else if (upperContent.Contains("CPF"))
                applied += ProcessarCampoCpf(cell, cellTexts, cellContent, dadosDep, "CPF");

            //else if (upperContent.Contains("FILIAÇÃO") || upperContent.Contains("FILIACAO"))
            //    applied += ProcessarCampoFiliacao(cell, cellTexts, cellContent, dadosDep);
            else if (upperContent.Contains("DATA NASCIMENTO"))
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosDep, "DATA_NASCIMENTO", "DATA NASCIMENTO");
            else if (upperContent.Contains("IDADE"))
                applied += ProcessarCampoIdade(cell, cellTexts, cellContent, dadosDep);
            else if (upperContent.Contains("SEXO"))
                applied += ProcessarCampoSexo(cell, cellTexts, cellContent, dadosDep);
            else if (upperContent.Contains("ESTADO CIVIL"))
                applied += ProcessarCampoEstadoCivil(cell, cellTexts, cellContent, dadosDep);
            else if (upperContent.Contains("PARENTESCO"))
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosDep, "PARENTESCO", "PARENTESCO");
            else if (upperContent.Contains("RG") && !upperContent.Contains("ORGÃO"))
                applied += ProcessarCampoSimples(cell, cellTexts, cellContent, dadosDep, "RG");
            else if (upperContent.Contains("ORGÃO EXPEDIDOR"))
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosDep, "ORGAO_EXPEDIDOR", "ORGÃO EXPEDIDOR");
            else if (upperContent.Contains("CARTÃO DO SUS"))
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosDep, "CARTAO_SUS", "CARTÃO DO SUS");
            else if (upperContent.Contains("DECLARAÇÃO NASCIDO VIVO"))
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosDep, "NUMERO_DECLARACAO_NASCIDO_VIVO", "Nº DECLARAÇÃO NASCIDO VIVO");

            return applied;
        }

        public int FillDadosEmpresa(string docPath, Dictionary<string, string> dadosEmpresa)
        {
            int applied = 0;

            using (var doc = WordprocessingDocument.Open(docPath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;
                var tables = body.Elements<Table>().ToList();

                foreach (var table in tables)
                {
                    var rows = table.Elements<TableRow>().ToList();

                    for (int r = 0; r < rows.Count; r++)
                    {
                        var cells = rows[r].Elements<TableCell>().ToList();

                        for (int c = 0; c < cells.Count; c++)
                        {
                            applied += ProcessarCelulaEmpresa(cells[c], dadosEmpresa, r, rows);
                        }
                    }
                }

                doc.MainDocumentPart.Document.Save();
            }

            return applied;
        }

        public void DuplicarPaginaBeneficiario(string documentoPath, string templatePath)
        {
            // 1. Extrair o bloco modelo (segundo bloco) do template original
            List<OpenXmlElement> blocoModelo;
            using (WordprocessingDocument templateDoc = WordprocessingDocument.Open(templatePath, false))
            {
                var templateBody = templateDoc.MainDocumentPart.Document.Body;
                var templateElementos = templateBody.Elements().ToList();

                var indicesModelo = new List<int>();
                for (int i = 0; i < templateElementos.Count; i++)
                {
                    if (templateElementos[i].InnerText.ToUpperInvariant().Contains("BENEFICIÁRIO TITULAR"))
                        indicesModelo.Add(i);
                }
                if (indicesModelo.Count < 2)
                    throw new Exception("O template não contém dois blocos de beneficiário.");

                int inicioModelo = indicesModelo[1]; // segundo bloco
                int fimModelo = templateElementos.Count;

                // O bloco termina antes do próximo "BENEFICIÁRIO TITULAR" ou antes do "RESUMO"
                for (int i = inicioModelo + 1; i < templateElementos.Count; i++)
                {
                    if (templateElementos[i].InnerText.ToUpperInvariant().Contains("BENEFICIÁRIO TITULAR"))
                    {
                        fimModelo = i;
                        break;
                    }
                }

                blocoModelo = templateElementos.Skip(inicioModelo).Take(fimModelo - inicioModelo).ToList();
            }

            // 2. Inserir o clone no documento de saída, **ANTES** do resumo
            using (WordprocessingDocument doc = WordprocessingDocument.Open(documentoPath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;
                var elementos = body.Elements().ToList();

                // Encontrar o índice do resumo
                int indiceResumo = -1;
                for (int i = 0; i < elementos.Count; i++)
                {
                    if (elementos[i].InnerText.ToUpperInvariant().Contains("RESUMO DAS CARACTERÍSTICAS"))
                    {
                        indiceResumo = i;
                        break;
                    }
                }

                if (indiceResumo == -1)
                    throw new Exception("Não foi possível localizar o resumo no documento.");

                // O elemento de referência é o último elemento antes do resumo
                var refElemento = elementos[indiceResumo - 1];

                // Inserir os clones do modelo antes do resumo
                foreach (var elem in blocoModelo)
                {
                    var clone = elem.CloneNode(true);
                    body.InsertAfter(clone, refElemento);
                    refElemento = clone; // atualiza a referência para o próximo clone
                }

                doc.MainDocumentPart.Document.Save();
            }
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

            if (upperContent.Contains("INÍCIO"))
            {
                applied += ProcessarCampoVigencia(cell, cellTexts, cellContent, dados, "INICIO_VIGENCIA");
            }
            else if (upperContent.Contains("NOME") && upperContent.Contains("COMPLETO"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados, "NOME_COMPLETO", "NOME COMPLETO");
            }

            else if (upperContent.Contains("CPF"))
                applied += ProcessarCampoCpf(cell, cellTexts, cellContent, dados, "CPF_TITULAR");

            else if (upperContent.Contains("DATA") && upperContent.Contains("NASCIMENTO"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados, "DATA_NASCIMENTO", "DATA NASCIMENTO");
            }
            else if (upperContent.Contains("RG") && !upperContent.Contains("ORGÃO"))
            {
                applied += ProcessarCampoSimples(cell, cellTexts, cellContent, dados, "RG");
            }
            //else if (upperContent.Contains("FILIAÇÃO") || upperContent.Contains("FILIACAO"))
            //{
            //    applied += ProcessarCampoFiliacao(cell, cellTexts, cellContent, dados);
            //}
            else if (upperContent.Contains("IDADE") && !upperContent.Contains("CIDADE"))
            {
                applied += ProcessarCampoIdade(cell, cellTexts, cellContent, dados);
            }
            else if (upperContent.Contains("SEXO"))
            {
                applied += ProcessarCampoSexo(cell, cellTexts, cellContent, dados);
            }
            else if (upperContent.Contains("ESTADO") && upperContent.Contains("CIVIL"))
            {
                applied += ProcessarCampoEstadoCivil(cell, cellTexts, cellContent, dados);
            }
            else if (upperContent.Contains("ORGÃO") && upperContent.Contains("EXPEDIDOR"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados, "ORGAO_EXPEDIDOR", "ORGÃO EXPEDIDOR");
            }
            else if (upperContent.Contains("CARTÃO") && upperContent.Contains("SUS"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados, "CARTAO_SUS", "CARTÃO DO SUS");
            }
            else if (upperContent.Contains("DECLARAÇÃO") && upperContent.Contains("NASCIDO") && upperContent.Contains("VIVO"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados, "NUMERO_DECLARACAO_NASCIDO_VIVO", "Nº DECLARAÇÃO NASCIDO VIVO");
            }
            else if (upperContent.Contains("ENDEREÇO"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados, "ENDERECO", "ENDEREÇO");
            }
            else if (upperContent.Contains("NÚMERO") && !upperContent.Contains("TELEFONE"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados, "NUMERO", "NÚMERO");
            }
            else if (upperContent.Contains("COMPLEMENTO"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados, "COMPLEMENTO", "COMPLEMENTO");
            }
            else if (upperContent.Contains("CEP"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados, "CEP", "CEP");
            }
            else if (upperContent.Contains("BAIRRO"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados, "BAIRRO", "BAIRRO");
            }
            else if (upperContent.Contains("CIDADE"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados, "CIDADE", "CIDADE");
            }
            //else if (upperContent.Contains("UF"))
            //{
            //    applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados, "UF", "UF");
            //}
            else if (upperContent.Contains("E-MAIL"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados, "EMAIL", "E-MAIL");
            }
            else if (upperContent.Contains("TELEFONE") && upperContent.Contains("CELULAR"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados, "TELEFONE_CELULAR", "TELEFONE CELULAR");
            }
            else if (upperContent.Contains("TELEFONE") && upperContent.Contains("FIXO"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dados, "TELEFONE_FIXO", "TELEFONE FIXO");
            }

            return applied;
        }

        private int ProcessarCelulaEmpresa(
            TableCell cell,
            Dictionary<string, string> dadosEmpresa,
            int rowIndex,
            List<TableRow> rows)
        {
            var cellTexts = cell.Descendants<Text>().ToList();
            if (cellTexts.Count == 0) return 0;

            string cellContent = string.Join("", cellTexts.Select(t => t.Text));
            if (string.IsNullOrWhiteSpace(cellContent)) return 0;

            string upperContent = cellContent.ToUpperInvariant();
            int applied = 0;

            if (upperContent.Contains("RAZÃO") && upperContent.Contains("SOCIAL"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosEmpresa, "RAZAO_SOCIAL", "RAZÃO SOCIAL");
            }
            else if (upperContent.Contains("NOME") && upperContent.Contains("FANTASIA"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosEmpresa, "NOME_FANTASIA", "NOME FANTASIA");
            }
            else if (upperContent.Contains("ENDEREÇO") && upperContent.Contains("EMPRESA"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosEmpresa, "ENDERECO_EMPRESA", "ENDEREÇO DA EMPRESA");
            }
            else if (upperContent.Contains("BAIRRO") && !upperContent.Contains("NOME") && !upperContent.Contains("ENDERECO"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosEmpresa, "BAIRRO_EMPRESA", "BAIRRO");
            }
            else if (upperContent.Contains("CEP"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosEmpresa, "CEP_EMPRESA", "CEP");
            }
            else if (upperContent.Contains("ESTADO") && !upperContent.Contains("CIVIL"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosEmpresa, "ESTADO_EMPRESA", "ESTADO");
            }
            else if (upperContent.Contains("CIDADE"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosEmpresa, "CIDADE_EMPRESA", "CIDADE");
            }
            else if (upperContent.Trim() == "CNPJ" || upperContent.Trim() == "CNPJ:")
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosEmpresa, "CNPJ", "CNPJ");
            }
            //else if (upperContent.Contains("DATA") && upperContent.Contains("INSCRIÇÃO") && upperContent.Contains("CNPJ"))
            //{
            //    applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosEmpresa, "DATA_INSCRICAO_CNPJ", "DATA DE INSCRIÇÃO DO CNPJ");
            //}
            else if (upperContent.Contains("NÚMERO") && upperContent.Contains("INSCRIÇÃO") && upperContent.Contains("MUNICIPAL"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosEmpresa, "INSCRICAO_MUNICIPAL", "NÚMERO DA INSCRIÇÃO MUNICIPAL");
            }
            else if (upperContent.Contains("NÚMERO") && upperContent.Contains("INSCRIÇÃO") && upperContent.Contains("ESTADUAL"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosEmpresa, "INSCRICAO_ESTADUAL", "NÚMERO DA INSCRIÇÃO ESTADUAL");
            }
            else if (upperContent.Contains("E-MAIL") || (upperContent.Contains("EMAIL")))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosEmpresa, "EMAIL", "E-MAIL");
            }
            else if (upperContent.Contains("TELEFONE") && !upperContent.Contains("CELULAR") && !upperContent.Contains("FIXO"))
            {
                applied += ProcessarCampoComRotuloCompleto(cell, cellTexts, cellContent, dadosEmpresa, "TELEFONE_EMPRESA", "TELEFONE");
            }

            return applied;
        }


        private int ProcessarCampoCpf(
       TableCell cell,
       List<Text> textos,
       string conteudoCelula,
       Dictionary<string, string> dadosCpf,
       string campoKey)
        {
            if (!dadosCpf.ContainsKey(campoKey) || textos.Count == 0)
                return 0;

            string valor = dadosCpf[campoKey];

            if (string.IsNullOrWhiteSpace(valor))
                return 0;


            // Tenta encontrar "CPF" ou "CPF TITULAR" no texto
            int indexRotulo = -1;
            string rotuloEncontrado = "";

            if (campoKey == "CPF_TITULAR")
            {
                indexRotulo = conteudoCelula.IndexOf("CPF TITULAR", StringComparison.OrdinalIgnoreCase);
                if (indexRotulo >= 0) rotuloEncontrado = "CPF TITULAR";
            }

            if (indexRotulo < 0)
            {
                indexRotulo = conteudoCelula.IndexOf("CPF", StringComparison.OrdinalIgnoreCase);
                if (indexRotulo >= 0) rotuloEncontrado = "CPF";
            }

            // Se não achou, tenta com regex para pegar "CPF" mesmo com espaços
            if (indexRotulo < 0)
            {
                Match match = Regex.Match(conteudoCelula, @"C\s*P\s*F", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    indexRotulo = match.Index;
                    rotuloEncontrado = match.Value;
                }
            }

            if (indexRotulo < 0) return 0;

            int fimRotulo = indexRotulo + rotuloEncontrado.Length;
            string parteAntes = conteudoCelula.Substring(0, fimRotulo);

            // Limpa todos os textos
            for (int i = 0; i < textos.Count; i++)
                textos[i].Text = "";

            // Coloca o rótulo no primeiro texto
            textos[0].Text = parteAntes;

            var parentRun = textos[0].Parent as Run;
            if (parentRun != null)
            {
                // Adiciona quebra de linha após o rótulo
                parentRun.AppendChild(new Break());

                // Cria um novo Run com o valor em azul
                Run boldRun = new Run();
                RunProperties runProperties = new RunProperties();
                runProperties.Append(new FontSize() { Val = "14" }); // Tamanho igual aos outros
                runProperties.Append(new FontSizeComplexScript() { Val = "14" });
                runProperties.Append(new Color() { Val = "0000FF" });
                boldRun.AppendChild(runProperties);
                boldRun.AppendChild(new Text(valor));

                var parentParagraph = parentRun.Parent as Paragraph;
                if (parentParagraph != null)
                {
                    parentParagraph.InsertAfter(boldRun, parentRun);
                }
                else
                {
                    // Fallback: se não achar o parágrafo, coloca na mesma linha com espaço
                    textos[0].Text = parteAntes + " " + valor;
                }
            }
            else
            {
                // Fallback: se não achar o Run, coloca na mesma linha
                textos[0].Text = parteAntes + " " + valor;
            }

            return 1;
        }

        private int MarcarCheckboxNaCelula(TableCell cell)
        {
            var paragraph = cell.Descendants<Paragraph>().FirstOrDefault();
            if (paragraph == null) return 0;

            var runs = paragraph.Descendants<Run>().ToList();
            if (runs.Count == 0) return 0;

            string textoCompleto = string.Join("", runs.SelectMany(r => r.Descendants<Text>()).Select(t => t.Text));
            bool jaMarcado = textoCompleto.Contains("☒") || textoCompleto.Contains("✓") ||
                             textoCompleto.Contains("●") || textoCompleto.Contains("[X]") ||
                             textoCompleto.Contains("(X)");

            if (jaMarcado) return 0;

            bool foiAlterado = false;

            foreach (var checkbox in CheckboxesMap)
            {
                if (textoCompleto.Contains(checkbox.Key))
                {
                    foreach (var run in runs)
                    {
                        foreach (var text in run.Descendants<Text>().ToList())
                        {
                            if (text.Text.Contains(checkbox.Key))
                            {
                                text.Text = text.Text.Replace(checkbox.Key, checkbox.Value);
                            }
                        }
                    }
                    foiAlterado = true;
                    break;
                }
            }

            if (!foiAlterado)
            {
                string upper = textoCompleto.ToUpperInvariant();
                if (upper.Contains("DIA") || upper.Contains("(UM)") ||
                    upper.Contains("(DEZ)") || upper.Contains("(VINTE)"))
                {
                    Run novoRun = new Run();
                    RunProperties props = new RunProperties();
                    props.Append(new FontSize() { Val = "16" });
                    props.Append(new FontSizeComplexScript() { Val = "16" });
                    props.Append(new Color() { Val = "0000FF" });
                    novoRun.AppendChild(props);
                    novoRun.Append(new Text("☒ "));

                    paragraph.InsertBefore(novoRun, runs.First());
                    foiAlterado = true;
                }
            }

            if (!foiAlterado) return 0;

            foreach (var run in paragraph.Descendants<Run>())
            {
                RunProperties props = run.RunProperties;
                if (props == null)
                {
                    props = new RunProperties();
                    run.PrependChild(props);
                }

                var fontSize = props.Descendants<FontSize>().FirstOrDefault();
                if (fontSize == null)
                {
                    fontSize = new FontSize() { Val = "18" };
                    props.Append(fontSize);
                }
                else fontSize.Val = "18";

                var fontSizeCs = props.Descendants<FontSizeComplexScript>().FirstOrDefault();
                if (fontSizeCs == null)
                {
                    fontSizeCs = new FontSizeComplexScript() { Val = "18" };
                    props.Append(fontSizeCs);
                }
                else fontSizeCs.Val = "18";

                var color = props.Descendants<Color>().FirstOrDefault();
                if (color == null)
                {
                    color = new Color() { Val = "0000FF" };
                    props.Append(color);
                }
                else color.Val = "0000FF";
            }

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

            if (string.IsNullOrWhiteSpace(valor))
                return 0;

            string conteudoUpper = conteudoCelula.ToUpperInvariant();

            if (conteudoUpper.Contains("VIGÊNCIA") || conteudoUpper.Contains("INÍCIO"))
            {
                string pattern = @"(IN[IÍ]CIO\s+DA\s+VIG[EÊ]NCIA\s*:?\s*)";
                Match match = Regex.Match(conteudoCelula, pattern, RegexOptions.IgnoreCase);

                if (match.Success)
                {
                    int fimPadrao = match.Index + match.Length;
                    string parteAntes = conteudoCelula.Substring(0, fimPadrao);

                    for (int i = 0; i < textos.Count; i++)
                        textos[i].Text = "";

                    textos[0].Text = parteAntes.TrimEnd() + " ";

                    var parentRun = textos[0].Parent as Run;
                    if (parentRun != null)
                    {
                        Run boldRun = new Run();
                        RunProperties runProperties = new RunProperties();
                        runProperties.Append(new FontSize() { Val = "16" });
                        runProperties.Append(new FontSizeComplexScript() { Val = "16" });
                        runProperties.Append(new Color() { Val = "0000FF" });
                        boldRun.AppendChild(runProperties);
                        boldRun.AppendChild(new Text(valor));

                        var parentParagraph = parentRun.Parent as Paragraph;
                        if (parentParagraph != null)
                        {
                            parentParagraph.InsertAfter(boldRun, parentRun);
                        }
                        else
                        {
                            textos[0].Text = parteAntes.TrimEnd() + " " + valor;
                        }
                    }
                    else
                    {
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
            // Se o valor for vazio ou só espaços, não faz nada (evita quebra de linha vazia)
            if (string.IsNullOrWhiteSpace(valor))
                return 0;

            string pattern = Regex.Escape(rotuloCompleto).Replace("\\ ", "\\s*");
            Match match = Regex.Match(conteudoCelula, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                int fimRotulo = match.Index + match.Length;
                string parteAntes = conteudoCelula.Substring(0, fimRotulo);

                for (int i = 0; i < textos.Count; i++)
                    textos[i].Text = "";

                textos[0].Text = parteAntes;

                var parentRun = textos[0].Parent as Run;
                if (parentRun != null)
                {
                    parentRun.AppendChild(new Break());

                    Run boldRun = new Run();
                    RunProperties runProperties = new RunProperties();
                    runProperties.Append(new FontSize() { Val = "12" });
                    runProperties.Append(new FontSizeComplexScript() { Val = "12" });
                    runProperties.Append(new Color() { Val = "0000FF" });
                    boldRun.AppendChild(runProperties);
                    boldRun.AppendChild(new Text(valor));

                    var parentParagraph = parentRun.Parent as Paragraph;
                    if (parentParagraph != null)
                    {
                        parentParagraph.InsertAfter(boldRun, parentRun);
                    }
                    else
                    {
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
        //    private int ProcessarCampoFiliacao(
        //TableCell cell,
        //List<Text> textos,
        //string conteudoCelula,
        //Dictionary<string, string> dados)
        //    {
        //        if (!dados.ContainsKey("FILIACAO") || textos.Count == 0)
        //            return 0;

        //        string valor = dados["FILIACAO"];
        //        string pattern = @"\b(FILIAÇÃO|FILIACAO)\b";
        //        Match match = Regex.Match(conteudoCelula, pattern, RegexOptions.IgnoreCase);

        //        if (match.Success)
        //        {
        //            int fimRotulo = match.Index + match.Length;
        //            string parteAntes = conteudoCelula.Substring(0, fimRotulo);

        //            for (int i = 0; i < textos.Count; i++)
        //                textos[i].Text = "";

        //            textos[0].Text = parteAntes;

        //            var parentRun = textos[0].Parent as Run;
        //            if (parentRun != null)
        //            {
        //                // ADICIONA QUEBRA DE LINHA ANTES DO VALOR
        //                parentRun.AppendChild(new Break());

        //                Run boldRun = new Run();
        //                RunProperties runProperties = new RunProperties();
        //                runProperties.Append(new FontSize() { Val = "12" });
        //                runProperties.Append(new FontSizeComplexScript() { Val = "12" });
        //                runProperties.Append(new Color() { Val = "0000FF" });
        //                boldRun.AppendChild(runProperties);
        //                boldRun.AppendChild(new Text(valor));

        //                var parentParagraph = parentRun.Parent as Paragraph;
        //                if (parentParagraph != null)
        //                {
        //                    parentParagraph.InsertAfter(boldRun, parentRun);
        //                }
        //                else
        //                {
        //                    textos[0].Text = parteAntes + "\r\n" + valor;
        //                }
        //            }
        //            else
        //            {
        //                textos[0].Text = parteAntes + "\r\n" + valor;
        //            }

        //            return 1;
        //        }

        //        return 0;
        //    }

        private int ProcessarCampoIdade(
            TableCell cell,
            List<Text> textos,
            string conteudoCelula,
            Dictionary<string, string> dados)
        {
            if (!dados.ContainsKey("IDADE") || textos.Count == 0)
                return 0;

            string valor = dados["IDADE"];

            if (string.IsNullOrWhiteSpace(valor))
                return 0;

            string pattern = @"\bIDADE\b";
            Match match = Regex.Match(conteudoCelula, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                int fimRotulo = match.Index + match.Length;
                string parteAntes = conteudoCelula.Substring(0, fimRotulo);

                for (int i = 0; i < textos.Count; i++)
                    textos[i].Text = "";

                textos[0].Text = parteAntes;

                var parentRun = textos[0].Parent as Run;
                if (parentRun != null)
                {
                    parentRun.AppendChild(new Break());

                    Run boldRun = new Run();
                    RunProperties runProperties = new RunProperties();
                    runProperties.Append(new FontSize() { Val = "16" });
                    runProperties.Append(new FontSizeComplexScript() { Val = "16" });
                    runProperties.Append(new Color() { Val = "0000FF" });
                    boldRun.AppendChild(runProperties);
                    boldRun.AppendChild(new Text(valor));

                    var parentParagraph = parentRun.Parent as Paragraph;
                    if (parentParagraph != null)
                    {
                        parentParagraph.InsertAfter(boldRun, parentRun);
                    }
                    else
                    {
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

            if (string.IsNullOrWhiteSpace(valor))
                return 0;

            string pattern = @"\bRG\b(?![A-ZÀ-Ú]*ÃO)";
            Match match = Regex.Match(conteudoCelula, pattern, RegexOptions.IgnoreCase);

            if (match.Success)
            {
                int fimRotulo = match.Index + match.Length;
                string parteAntes = conteudoCelula.Substring(0, fimRotulo);

                for (int i = 0; i < textos.Count; i++)
                    textos[i].Text = "";

                textos[0].Text = parteAntes;

                var parentRun = textos[0].Parent as Run;
                if (parentRun != null)
                {
                    parentRun.AppendChild(new Break());

                    Run boldRun = new Run();
                    RunProperties runProperties = new RunProperties();
                    runProperties.Append(new FontSize() { Val = "16" });
                    runProperties.Append(new FontSizeComplexScript() { Val = "16" });
                    runProperties.Append(new Color() { Val = "0000FF" });
                    boldRun.AppendChild(runProperties);
                    boldRun.AppendChild(new Text(valor));

                    var parentParagraph = parentRun.Parent as Paragraph;
                    if (parentParagraph != null)
                    {
                        parentParagraph.InsertAfter(boldRun, parentRun);
                    }
                    else
                    {
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

            string valorSexo = dados["SEXO"].ToUpperInvariant();
            string conteudoOriginal = string.Join("", textos.Select(t => t.Text));

            string opcoes = "";
            if (valorSexo == "M")
            {
                opcoes = "☒ M   ☐ F   ☐ OUTROS";
            }
            else if (valorSexo == "F")
            {
                opcoes = "☐ M   ☒ F   ☐ OUTROS";
            }
            else
            {
                opcoes = "☐ M   ☐ F   ☒ OUTROS";
            }

            string conteudoEsperado = "SEXO:\n" + opcoes;
            if (conteudoOriginal.Replace("\r\n", "\n").Trim() == conteudoEsperado.Replace("\r\n", "\n").Trim())
                return 0;

            cell.RemoveAllChildren<Paragraph>();

            Paragraph newParagraph = new Paragraph();

            Run runRotulo = new Run();
            RunProperties runPropertiesRotulo = new RunProperties();
            runPropertiesRotulo.Append(new FontSize() { Val = "12" });
            runPropertiesRotulo.Append(new FontSizeComplexScript() { Val = "12" });
            runRotulo.AppendChild(runPropertiesRotulo);
            runRotulo.AppendChild(new Text("SEXO:"));
            runRotulo.AppendChild(new Break());

            Run runOpcoes = new Run();
            RunProperties runPropertiesOpcoes = new RunProperties();
            runPropertiesOpcoes.Append(new FontSize() { Val = "16" });
            runPropertiesOpcoes.Append(new FontSizeComplexScript() { Val = "16" });
            runPropertiesOpcoes.Append(new Color() { Val = "0000FF" });
            runOpcoes.AppendChild(runPropertiesOpcoes);
            runOpcoes.AppendChild(new Text(opcoes));

            newParagraph.AppendChild(runRotulo);
            newParagraph.AppendChild(runOpcoes);
            cell.AppendChild(newParagraph);

            return 1;
        }

        private int ProcessarCampoEstadoCivil(
            TableCell cell,
            List<Text> textos,
            string conteudoCelula,
            Dictionary<string, string> dados)
        {
            if (!dados.ContainsKey("ESTADO_CIVIL") || textos.Count == 0)
                return 0;

            string valorEstadoCivil = dados["ESTADO_CIVIL"].ToUpperInvariant();
            string conteudoOriginal = string.Join("", textos.Select(t => t.Text));
            string conteudoUpper = conteudoOriginal.ToUpperInvariant();

            if (!conteudoUpper.Contains("ESTADO") || !conteudoUpper.Contains("CIVIL"))
                return 0;

            string opcoes = "";
            if (valorEstadoCivil == "SOLTEIRO")
            {
                opcoes = "☒ SOLTEIRO   ☐ CASADO   ☐ OUTROS";
            }
            else if (valorEstadoCivil == "CASADO")
            {
                opcoes = "☐ SOLTEIRO   ☒ CASADO   ☐ OUTROS";
            }
            else
            {
                opcoes = "☐ SOLTEIRO   ☐ CASADO   ☒ OUTROS";
            }

            string conteudoEsperado = "ESTADO CIVIL\n" + opcoes;
            if (conteudoOriginal.Replace("\r\n", "\n").Trim() == conteudoEsperado.Replace("\r\n", "\n").Trim())
                return 0;

            cell.RemoveAllChildren<Paragraph>();

            Paragraph newParagraph = new Paragraph();

            Run runRotulo = new Run();
            RunProperties runPropertiesRotulo = new RunProperties();
            runPropertiesRotulo.Append(new FontSize() { Val = "16" });
            runPropertiesRotulo.Append(new FontSizeComplexScript() { Val = "16" });
            runRotulo.AppendChild(runPropertiesRotulo);
            runRotulo.AppendChild(new Text("ESTADO CIVIL"));
            runRotulo.AppendChild(new Break());

            Run runOpcoes = new Run();
            RunProperties runPropertiesOpcoes = new RunProperties();
            runPropertiesOpcoes.Append(new FontSize() { Val = "16" });
            runPropertiesOpcoes.Append(new FontSizeComplexScript() { Val = "16" });
            runPropertiesOpcoes.Append(new Color() { Val = "0000FF" });
            runOpcoes.AppendChild(runPropertiesOpcoes);
            runOpcoes.AppendChild(new Text(opcoes));

            newParagraph.AppendChild(runRotulo);
            newParagraph.AppendChild(runOpcoes);
            cell.AppendChild(newParagraph);

            return 1;
        }

        public int FillTabelaValoresPorBloco(string documentoPath, Dictionary<string, string> valores, int indiceBloco)
        {
            int applied = 0;
            using (var doc = WordprocessingDocument.Open(documentoPath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;
                var tables = body.Descendants<Table>().ToList();

                int blocoAtual = -1;
                bool dentroBloco = false;
                bool tabelaPreenchida = false;

                foreach (var table in tables)
                {
                    var rows = table.Descendants<TableRow>().ToList();

                    // Primeiro, verifica se esta tabela contém o cabeçalho de valores
                    int linhaCabecalho = -1;
                    for (int r = 0; r < rows.Count; r++)
                    {
                        var cells = rows[r].Descendants<TableCell>().ToList();
                        if (cells.Count < 2) continue;

                        bool temTitular = false;
                        bool temDependente = false;
                        foreach (var cell in cells)
                        {
                            string texto = string.Join("", cell.Descendants<Text>().Select(t => t.Text)).ToUpper();
                            if (texto.Contains("TITULAR") && texto.Contains("(R")) temTitular = true;
                            if (texto.Contains("DEPENDENTE") && texto.Contains("(R")) temDependente = true;
                        }
                        if (temTitular && temDependente)
                        {
                            linhaCabecalho = r;
                            break;
                        }
                    }

                    // Se não é a tabela de valores, verifica se há marcador de bloco
                    if (linhaCabecalho == -1)
                    {
                        for (int r = 0; r < rows.Count; r++)
                        {
                            var cells = rows[r].Descendants<TableCell>().ToList();
                            foreach (var cell in cells)
                            {
                                string texto = string.Join("", cell.Descendants<Text>().Select(t => t.Text)).ToUpper();
                                if (texto.Contains("BENEFICIÁRIO TITULAR"))
                                {
                                    blocoAtual++;
                                    dentroBloco = (blocoAtual == indiceBloco);
                                    tabelaPreenchida = false; // novo bloco, ainda não preenchemos a tabela
                                    break;
                                }
                            }
                        }
                        continue;
                    }

                    // Estamos na tabela de valores
                    if (dentroBloco && !tabelaPreenchida)
                    {
                        if (linhaCabecalho + 1 < rows.Count)
                        {
                            var linhaValores = rows[linhaCabecalho + 1];
                            var celulasValores = linhaValores.Descendants<TableCell>().ToList();

                            for (int idx = 0; idx < celulasValores.Count; idx++)
                            {
                                string chave = idx == 0 ? "VALOR_TITULAR" : $"VALOR_DEPENDENTE_{idx}";
                                if (valores.ContainsKey(chave))
                                {
                                    var celula = celulasValores[idx];
                                    celula.RemoveAllChildren<Paragraph>();

                                    Paragraph p = new Paragraph();
                                    Run r = new Run();
                                    RunProperties rp = new RunProperties();
                                    rp.Append(new Color() { Val = "0000FF" });
                                    rp.Append(new FontSize() { Val = "12" });
                                    r.RunProperties = rp;
                                    r.AppendChild(new Text(valores[chave]));
                                    p.AppendChild(r);
                                    celula.AppendChild(p);
                                    applied++;
                                }
                            }
                            tabelaPreenchida = true;
                        }
                    }
                }
                doc.MainDocumentPart.Document.Save();
            }
            return applied;
        }

        public int FillTotalContraprestacaoPorBloco(string documentoPath, Dictionary<string, string> valores, int indiceBloco)
        {
            int applied = 0;
            using (var doc = WordprocessingDocument.Open(documentoPath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;
                var allParagraphs = body.Descendants<Paragraph>().ToList();

                int blocoAtual = -1;
                bool dentroBloco = false;
                bool totalPreenchido = false;

                for (int i = 0; i < allParagraphs.Count; i++)
                {
                    var p = allParagraphs[i];
                    string texto = string.Join("", p.Descendants<Text>().Select(t => t.Text));

                    // Detectar início de bloco
                    if (texto.Contains("BENEFICIÁRIO TITULAR"))
                    {
                        blocoAtual++;
                        dentroBloco = (blocoAtual == indiceBloco);
                        totalPreenchido = false;
                        continue;
                    }

                    if (!dentroBloco || totalPreenchido) continue;

                    // Procurar o texto longo que antecede o campo de total
                    if (texto.Contains("taxa de administração") && texto.Contains("mensalidades desta proposta"))
                    {
                        if (i + 1 < allParagraphs.Count)
                        {
                            Paragraph campo = allParagraphs[i + 1];
                            campo.RemoveAllChildren<Paragraph>();

                            // Calcular total
                            decimal total = 0;
                            decimal ExtrairValor(string txt)
                            {
                                if (string.IsNullOrWhiteSpace(txt)) return 0;
                                string limpo = txt.Replace("R$", "").Replace(" ", "").Replace(",", ".");
                                decimal.TryParse(limpo, System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture, out decimal val);
                                return val;
                            }

                            if (valores.ContainsKey("VALOR_TITULAR"))
                                total += ExtrairValor(valores["VALOR_TITULAR"]);
                            for (int j = 1; j <= 5; j++)
                            {
                                string key = $"VALOR_DEPENDENTE_{j}";
                                if (valores.ContainsKey(key))
                                    total += ExtrairValor(valores[key]);
                            }

                            string totalFormatado = $"R$ {total.ToString("N2").Replace(".", ",")}";

                            Run runValor = new Run();
                            RunProperties rp = new RunProperties();
                            rp.Append(new Color() { Val = "0000FF" });
                            rp.Append(new FontSize() { Val = "12" });
                            runValor.RunProperties = rp;
                            runValor.AppendChild(new Text(totalFormatado));
                            campo.AppendChild(runValor);

                            totalPreenchido = true;
                            applied++;
                        }
                    }
                }
                doc.MainDocumentPart.Document.Save();
            }
            return applied;
        }
        public int FillTermoAutorizacao(string docPath, string nomeServidor, string assinatura)
        {
            int applied = 0;
            using (var doc = WordprocessingDocument.Open(docPath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;
                var paragraphs = body.Descendants<Paragraph>().ToList();

                // 1. Preencher o nome do servidor (campo depois de "Eu,") com formatação azul
                int indiceTermo = -1;
                for (int i = 0; i < paragraphs.Count; i++)
                {
                    var para = paragraphs[i];
                    string textoParagrafo = para.InnerText;
                    if (textoParagrafo.Contains("Eu,") && textoParagrafo.Contains("servidor(a)"))
                    {
                        indiceTermo = i;
                        // Concatena todo o texto do parágrafo
                        var runs = para.Descendants<Run>().ToList();
                        string fullText = string.Join("", runs.SelectMany(r => r.Descendants<Text>()).Select(t => t.Text));
                        if (fullText.Contains("Eu,  ,"))
                        {
                            // Divide o texto em três partes: antes do nome, nome, depois do nome
                            string antes = "Eu, ";
                            string depois = "," + fullText.Substring(fullText.IndexOf("Eu,  ,") + "Eu,  ,".Length);

                            para.RemoveAllChildren();

                            // Parte fixa "Eu, " (normal)
                            Run runAntes = new Run();
                            runAntes.AppendChild(new Text(antes));
                            para.AppendChild(runAntes);

                            // Nome em azul
                            Run runNome = new Run();
                            RunProperties rpNome = new RunProperties();
                            rpNome.Append(new Color() { Val = "0000FF" });
                            rpNome.Append(new FontSize() { Val = "18" });
                            runNome.RunProperties = rpNome;
                            runNome.AppendChild(new Text(nomeServidor));
                            para.AppendChild(runNome);

                            // Resto do texto (vírgula e o que vem depois) normal
                            Run runDepois = new Run();
                            runDepois.AppendChild(new Text(depois));
                            para.AppendChild(runDepois);

                            applied++;
                        }
                        break;
                    }
                }

                // 2. Preencher a Assinatura no termo com formatação azul
                if (indiceTermo != -1)
                {
                    for (int i = indiceTermo + 1; i < paragraphs.Count; i++)
                    {
                        var para = paragraphs[i];
                        string texto = para.InnerText.Trim();
                        if (texto.Equals("Assinatura", StringComparison.OrdinalIgnoreCase) ||
                            texto.Equals("Assinatura:", StringComparison.OrdinalIgnoreCase))
                        {
                            para.RemoveAllChildren();

                            // Texto "Assinatura: " normal
                            Run runLabel = new Run();
                            runLabel.AppendChild(new Text("Assinatura: "));
                            para.AppendChild(runLabel);

                            // Nome da assinatura em azul
                            Run runAss = new Run();
                            RunProperties rpAss = new RunProperties();
                            rpAss.Append(new Color() { Val = "0000FF" });
                            rpAss.Append(new FontSize() { Val = "18" });
                            runAss.RunProperties = rpAss;
                            runAss.AppendChild(new Text(assinatura));
                            para.AppendChild(runAss);

                            applied++;
                            break;
                        }
                    }
                }

                doc.MainDocumentPart.Document.Save();
            }
            return applied;
        }
    }
}