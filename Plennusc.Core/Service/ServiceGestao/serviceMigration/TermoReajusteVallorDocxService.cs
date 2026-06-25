using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Plennusc.Core.Models.ModelsGestao.modelsMigration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Plennusc.Core.Service.ServiceGestao.serviceMigration
{
      public class TermoReajusteVallorDocxService
    {
        // ── Símbolo de checkbox marcado / desmarcado ──────────────────────
        // O Word usa o caractere Unicode ☑ (U+2611) ou ☐ (U+2610).
        // Aqui usamos o símbolo "☑" para marcar e deixamos o template original para desmarcado.
        private const string CheckboxMarcado    = "☑";
        private const string CheckboxDesmarcado = "☐";
 
        // ── Mapeamento NOME_PRODUTO → texto que aparece na célula da tabela ──
        // Ajuste as chaves conforme os nomes que vierem no seu CSV.
        private static readonly Dictionary<string, string> MapProdutoParaPlano =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "NOSSO PLANO CE + ODONTO SC GM 1", "NOSSO PLANO CE + ODONTO SC GM 1" },
                { "NOSSO PLANO CE + ODONTO SC GM 2", "NOSSO PLANO CE + ODONTO SC GM 2" },
                { "PLENO XXVI",                       "PLENO XXVI"  },
                { "PLENO XXVII",                      "PLENO XXVII" },
                // Aliases curtos (caso o CSV venha abreviado)
                { "GM1",       "NOSSO PLANO CE + ODONTO SC GM 1" },
                { "GM2",       "NOSSO PLANO CE + ODONTO SC GM 2" },
                { "PLENO 26",  "PLENO XXVI"  },
                { "PLENO 27",  "PLENO XXVII" },
            };
 
        // ─────────────────────────────────────────────────────────────────
        /// <summary>
        /// Ponto de entrada principal.
        /// </summary>
        public void GerarDocumento(
            string templatePath,
            string outputPath,
            TermoReajusteVallor dados)
        {
            File.Copy(templatePath, outputPath, true);
 
            using (var doc = WordprocessingDocument.Open(outputPath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;

                SubstituirPlaceholders(body, dados);
                MarcarCheckboxPlano(body, dados.NomeProduto);
                PreencherTabelaComposicao(body, dados);
                //PreencherAssinatura(body, dados.Nome);
 
                doc.MainDocumentPart.Document.Save();
            }
        }

        private void SubstituirPlaceholders(Body body, TermoReajusteVallor dados)
        {
            var replacements = new Dictionary<string, string>
                {
                    { "{NOME}",     dados.Nome        },
                    { "{CPF}",      dados.Cpf         },
                    { "{PRODUTO}",  dados.NomeProduto },
                };

            foreach (var para in body.Descendants<Paragraph>())
            {
                string textoCompleto = string.Join("", para.Descendants<Text>().Select(t => t.Text));

                bool temPlaceholder = replacements.Keys.Any(k => textoCompleto.Contains(k));
                if (!temPlaceholder) continue;

                var paraProps = para.ParagraphProperties != null
                    ? (ParagraphProperties)para.ParagraphProperties.CloneNode(true)
                    : null;

                // Quebra o texto em segmentos: normal e substituído
                // Ex: "Eu, {NOME} ,inscrito" → ["Eu, ", "AMANDA", " ,inscrito"]
                var pattern = string.Join("|", replacements.Keys.Select(k => Regex.Escape(k)));
                var partes = Regex.Split(textoCompleto, $"({pattern})");

                para.RemoveAllChildren();
                if (paraProps != null) para.AppendChild(paraProps);

                foreach (var parte in partes)
                {
                    if (string.IsNullOrEmpty(parte)) continue;

                    var run = new Run();

                    if (replacements.TryGetValue(parte, out string valor))
                    {
                        // É um placeholder — azul
                        var rp = new RunProperties();
                        rp.Append(new Color { Val = "0070C0" });
                        run.RunProperties = rp;
                        run.AppendChild(new Text(valor) { Space = SpaceProcessingModeValues.Preserve });
                    }
                    else
                    {
                        // Texto normal — sem cor
                        run.AppendChild(new Text(parte) { Space = SpaceProcessingModeValues.Preserve });
                    }

                    para.AppendChild(run);
                }
            }
        }


        // ═══════════════════════════════════════════════════════════════════
        // 1. PARÁGRAFOS DE TEXTO CORRIDO
        //    "Eu, _______, inscrito(a) no CPF sob o nº _______, declaro...
        //     ciente e de acordo com o reajuste da mensalidade do produto _______"
        // ═══════════════════════════════════════════════════════════════════
        //private void PreencherParagrafosTexto(Body body, TermoReajusteVallor dados)
        //{
        //    var paragrafos = body.Descendants<Paragraph>().ToList();

        //    foreach (var para in paragrafos)
        //    {
        //        string texto = TextoParagrafo(para);
        //        string upper = texto.ToUpperInvariant();

        //        // Parágrafo único: "Eu, ___ inscrito(a) no CPF sob o nº ___ ... produto ___"
        //        if (upper.Contains("INSCRITO") && upper.Contains("CPF") && upper.Contains("PRODUTO"))
        //        {
        //            SubstituirBrancosNoParagrafo(para, new[]
        //            {
        //        dados.Nome,
        //        dados.Cpf,
        //        dados.NomeProduto
        //    });
        //            continue;
        //        }

        //        // Caso o template tenha parágrafos separados (fallback)
        //        if (upper.Contains("INSCRITO") && upper.Contains("CPF"))
        //        {
        //            SubstituirBrancosNoParagrafo(para, new[]
        //            {
        //        dados.Nome,
        //        dados.Cpf
        //    });
        //            continue;
        //        }

        //        if (upper.Contains("MENSALIDADE") && upper.Contains("PRODUTO"))
        //        {
        //            SubstituirBrancosNoParagrafo(para, new[]
        //            {
        //        dados.NomeProduto
        //    });
        //            continue;
        //        }
        //    }
        //}

        /// <summary>
        /// Encontra os campos em branco (sequências de "_____" ou runs vazios após
        /// pontuação típica de formulário) e substitui pelos valores fornecidos na ordem.
        /// </summary>
        //private void SubstituirBrancosNoParagrafo(Paragraph para, string[] valores)
        //{
        //    // Coleta todos os Text nodes do parágrafo em ordem
        //    var allTexts = para.Descendants<Run>()
        //                       .SelectMany(r => r.Descendants<Text>()
        //                                         .Select(t => new { Run = r, Text = t }))
        //                       .ToList();

        //    // Concatena tudo para encontrar os ___ via regex
        //    string textoCompleto = string.Join("", allTexts.Select(x => x.Text.Text));
        //    var matches = Regex.Matches(textoCompleto, @"_{2,}");

        //    if (matches.Count == 0) return;

        //    int slot = 0;
        //    foreach (Match m in matches)
        //    {
        //        if (slot >= valores.Length) break;

        //        // Descobre qual Text node contém o início do match
        //        int pos = 0;
        //        foreach (var item in allTexts)
        //        {
        //            int len = item.Text.Text.Length;
        //            if (pos + len > m.Index)
        //            {
        //                // Este é o Text node do underscore — substitui só ele
        //                // e limpa os outros Text nodes do mesmo "campo" se houver
        //                item.Text.Text = valores[slot];
        //                item.Text.Space = SpaceProcessingModeValues.Preserve;

        //                // Aplica cor azul no Run pai
        //                var run = item.Run;
        //                if (run.RunProperties == null)
        //                    run.RunProperties = new RunProperties();

        //                // Remove cor existente antes de adicionar
        //                run.RunProperties.RemoveAllChildren<Color>();
        //                run.RunProperties.Append(new Color { Val = "0070C0" });

        //                // Limpa outros Text nodes do mesmo run que fazem parte do campo
        //                foreach (var outro in allTexts
        //                    .Where(x => x.Run == run && x.Text != item.Text))
        //                {
        //                    int outroPos = textoCompleto.IndexOf(outro.Text.Text,
        //                                    StringComparison.Ordinal);
        //                    if (outroPos >= m.Index && outroPos < m.Index + m.Length)
        //                        outro.Text.Text = "";
        //                }

        //                slot++;
        //                break;
        //            }
        //            pos += len;
        //        }
        //    }
        //}

        // ═══════════════════════════════════════════════════════════════════
        // 2. MARCAR CHECKBOX DO PLANO NA TABELA "ASSINALE ABAIXO O PLANO PRETENDIDO"
        // ═══════════════════════════════════════════════════════════════════
        private void MarcarCheckboxPlano(Body body, string nomeProduto)
        {
            if (string.IsNullOrWhiteSpace(nomeProduto)) return;

            var mapaPlanos = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "NOSSO PLANO CE + ODONTO SC GM 1", "51A" },
        { "NOSSO PLANO CE + ODONTO SC GM 2", "52B" },
        { "PLENO XXVI",                      "53C" },
        { "PLENO XXVII",                     "54D" },
    };

            if (!mapaPlanos.TryGetValue(nomeProduto.Trim(), out string idAlvo)) return;

            foreach (var cell in body.Descendants<TableCell>())
            {
                string textoCelula = string.Join("", cell.Descendants<Text>().Select(t => t.Text));

                if (textoCelula.Contains(idAlvo))
                {
                    cell.RemoveAllChildren<Paragraph>();
                    var para = new Paragraph();

                    var runCheck = new Run();
                    var rpCheck = new RunProperties();
                    rpCheck.Append(new Color { Val = "0070C0" });
                    runCheck.RunProperties = rpCheck;
                    runCheck.AppendChild(new Text("☑ ") { Space = SpaceProcessingModeValues.Preserve });
                    para.AppendChild(runCheck);

                    var runTexto = new Run();
                    runTexto.AppendChild(new Text(nomeProduto) { Space = SpaceProcessingModeValues.Preserve });
                    para.AppendChild(runTexto);

                    cell.AppendChild(para);
                    return;
                }
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // 3. PREENCHER TABELA DE COMPOSIÇÃO DA CONTRAPRESTAÇÃO
        //    Titular (R$) | Dependente 1 (R$) | ... | Dependente N (R$)
        //    Suporta até 9 dependentes (duas linhas de tabela).
        // ═══════════════════════════════════════════════════════════════════
        private void PreencherTabelaComposicao(Body body, TermoReajusteVallor dados)
        {
            // Montar lista de valores: [0]=titular, [1..9]=dependentes
            var valores = new List<string>();
            valores.Add(dados.Valor ?? "");
            foreach (var dep in dados.Dependentes)
                valores.Add(dep.Valor ?? "");
 
            var tables = body.Descendants<Table>().ToList();
 
            foreach (var table in tables)
            {
                var rows = table.Descendants<TableRow>().ToList();
                bool tabelaCorreta = false;
 
                // Identificar tabela pela linha de cabeçalho
                foreach (var row in rows)
                {
                    string textoRow = TextoRow(row).ToUpperInvariant();
                    if (textoRow.Contains("COMPOSIÇÃO") && textoRow.Contains("CONTRAPRESTAÇÃO"))
                    {
                        tabelaCorreta = true;
                        break;
                    }
                    // Cabeçalho pode estar na linha de células também
                    if (textoRow.Contains("TITULAR") && textoRow.Contains("DEPENDENTE"))
                    {
                        tabelaCorreta = true;
                        break;
                    }
                }
 
                if (!tabelaCorreta) continue;
 
                // Percorrer linhas de dados (não-cabeçalho)
                int slotValor = 0;
 
                foreach (var row in rows)
                {
                    string textoRow = TextoRow(row).ToUpperInvariant();
 
                    // Pular linha de título e linha de cabeçalho das colunas
                    if (textoRow.Contains("COMPOSIÇÃO") || textoRow.Contains("CONTRAPRESTAÇÃO"))
                        continue;
                    if (textoRow.Contains("TITULAR (R$)") || textoRow.Contains("DEPENDENTE 1"))
                        continue; // linha de cabeçalho das células de valor
 
                    // Linha de dados: células vazias que receberão os valores
                    var cells = row.Descendants<TableCell>().ToList();
                    if (cells.Count == 0) continue;
 
                    // Verificar se é linha de cabeçalho (tem "TITULAR" ou "DEPENDENTE")
                    bool ehCabecalho = cells.Any(c =>
                    {
                        string tc = TextoCelula(c).ToUpperInvariant();
                        return tc.Contains("TITULAR") || tc.Contains("DEPENDENTE");
                    });
 
                    if (ehCabecalho)
                    {
                        // A linha ABAIXO deste cabeçalho é a linha de valores
                        // Não faz nada aqui; vamos pegar a linha seguinte
                        continue;
                    }
 
                    // Linha de valores: preencher cada célula com o valor correspondente
                    bool preencheuAlgo = false;
                    foreach (var cell in cells)
                    {
                        if (slotValor >= valores.Count) break;
 
                        string conteudoAtual = TextoCelula(cell).Trim();
 
                        // Só preencher células que estejam vazias ou com placeholder
                        if (string.IsNullOrWhiteSpace(conteudoAtual)
                            || conteudoAtual == "_"
                            || conteudoAtual.All(c => c == '_'))
                        {
                            EscreverValorNaCelula(cell, valores[slotValor]);
                            slotValor++;
                            preencheuAlgo = true;
                        }
                    }
 
                    // Se não preencheu nada mas as células existem, tenta forçar
                    if (!preencheuAlgo && slotValor < valores.Count)
                    {
                        foreach (var cell in cells)
                        {
                            if (slotValor >= valores.Count) break;
                            EscreverValorNaCelula(cell, valores[slotValor]);
                            slotValor++;
                        }
                    }
 
                    if (slotValor >= valores.Count) break;
                }
 
                break; // Processou a tabela correta
            }
        }
 
        private void EscreverValorNaCelula(TableCell cell, string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return;
 
            cell.RemoveAllChildren<Paragraph>();
 
            var para = new Paragraph();
            var ppr  = new ParagraphProperties();
            ppr.Append(new Justification { Val = JustificationValues.Center });
            para.AppendChild(ppr);
 
            var run = new Run();
            var rp  = new RunProperties();
            rp.Append(new Color { Val = "0070C0" });
            rp.Append(new Bold());
            rp.Append(new FontSize { Val = "18" });
            run.RunProperties = rp;
            run.AppendChild(new Text(valor) { Space = SpaceProcessingModeValues.Preserve });
 
            para.AppendChild(run);
            cell.AppendChild(para);
        }
 
        // ═══════════════════════════════════════════════════════════════════
        // 4. ASSINATURA: preencher o nome do titular abaixo da linha
        //    "Assinatura Titular / Responsável Financeiro"
        // ═══════════════════════════════════════════════════════════════════
        //private void PreencherAssinatura(Body body, string nomeCompleto)
        //{
        //    if (string.IsNullOrWhiteSpace(nomeCompleto)) return;
 
        //    var paragrafos = body.Descendants<Paragraph>().ToList();
 
        //    for (int i = 0; i < paragrafos.Count; i++)
        //    {
        //        string texto = TextoParagrafo(paragrafos[i]).ToUpperInvariant();
 
        //        // Localizar o parágrafo com "ASSINATURA TITULAR"
        //        if (texto.Contains("ASSINATURA") && texto.Contains("TITULAR"))
        //        {
        //            // O campo para o nome é o parágrafo ANTERIOR (linha de _____)
        //            // ou o parágrafo com underscores antes deste
        //            for (int j = i - 1; j >= Math.Max(0, i - 5); j--)
        //            {
        //                string textoAnterior = TextoParagrafo(paragrafos[j]);
        //                if (textoAnterior.Contains("_") || string.IsNullOrWhiteSpace(textoAnterior))
        //                {
        //                    // Inserir o nome aqui
        //                    InserirTextoNoParagrafo(paragrafos[j], nomeCompleto);
        //                    return;
        //                }
        //            }
 
        //            // Fallback: inserir após a linha "Assinatura Titular"
        //            if (i + 1 < paragrafos.Count)
        //                InserirTextoNoParagrafo(paragrafos[i + 1], nomeCompleto);
 
        //            return;
        //        }
        //    }
        //}
 
        private void InserirTextoNoParagrafo(Paragraph para, string valor)
        {
            var paraProps = para.ParagraphProperties != null
                ? (ParagraphProperties)para.ParagraphProperties.CloneNode(true)
                : null;
 
            para.RemoveAllChildren();
            if (paraProps != null) para.AppendChild(paraProps);
 
            var run = new Run();
            var rp  = new RunProperties();
            rp.Append(new Color { Val = "0070C0" });
            rp.Append(new Bold());
            run.RunProperties = rp;
            run.AppendChild(new Text(valor) { Space = SpaceProcessingModeValues.Preserve });
            para.AppendChild(run);
        }
 
        // ── Helpers de texto ─────────────────────────────────────────────
        private string TextoParagrafo(Paragraph p)
            => string.Join("", p.Descendants<Text>().Select(t => t.Text));
 
        private string TextoRow(TableRow row)
            => string.Join(" ", row.Descendants<Text>().Select(t => t.Text));
 
        private string TextoCelula(TableCell cell)
            => string.Join("", cell.Descendants<Text>().Select(t => t.Text));
    }
}
 