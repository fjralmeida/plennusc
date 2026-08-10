using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Plennusc.Core.Models.ModelsGestao.modelsMigration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Plennusc.Core.Service.ServiceGestao.serviceMigration
{
    public class DocxServiceSetCemg
    {
        public string GerarDocumento(string templatePath, string outputPath, DadosPropostaSetCemg dados)
        {
            File.Copy(templatePath, outputPath, true);
            var log = new StringBuilder();

            using (var doc = WordprocessingDocument.Open(outputPath, true))
            {
                var body = doc.MainDocumentPart.Document.Body;

                // 1. Preencher campos de texto via placeholders {{TOKEN}}
                PreencherCamposTexto(body, dados, log);

                // 2. Marcar vigências e vencimentos (só na tabela 1)
                MarcarVigencias(body, dados.Vigencias, log);
                MarcarVencimentos(body, dados.Vencimentos, log);

                // 3. Marcar produtos (podem ser vários, separados por ; no import)
                MarcarProdutos(body, dados.Produtos, log);

                // 4. Marcar mensalidades (pareadas por posição com produtos)
                MarcarMensalidades(body, dados.Mensalidades, log);

                // 5. Marcar opcionais
                MarcarOpcionais(body, dados.Aeromedico, dados.Odontologia, log);

                doc.MainDocumentPart.Document.Save();
            }

            return log.ToString();
        }

        // ─── PREENCHER CAMPOS DE TEXTO VIA PLACEHOLDER {{TOKEN}} ───
        // Muito mais seguro que buscar o rótulo em texto livre: o token só
        // existe no campo que deve ser preenchido, nunca colide com texto
        // descritivo (ex.: "CNPJ" que aparece no parágrafo de abertura).
        private void PreencherCamposTexto(Body body, DadosPropostaSetCemg dados, StringBuilder log)
        {
            var mapeamento = new Dictionary<string, string>
            {
                { "{{PROPOSTA}}", dados.Proposta },
                { "{{RAZ_O_SOCIAL}}", dados.RazaoSocial },
                { "{{NOME_FANTASIA}}", dados.NomeFantasia },
                { "{{CNPJ}}", dados.Cnpj },
                { "{{INSCRI__O_ESTADUAL}}", dados.InscricaoEstadual },
                { "{{INSCRI__O_MUNICIPAL}}", dados.InscricaoMunicipal },
                { "{{LOGRADOURO}}", dados.Logradouro },
                { "{{N_MERO}}", dados.Numero },
                { "{{COMPLEMENTO}}", dados.Complemento },
                { "{{BAIRRO}}", dados.Bairro },
                { "{{MUNIC_PIO_UF}}", dados.MunicipioUf },
                { "{{CEP}}", dados.Cep },
                { "{{EMAIL}}", dados.Email },
                { "{{TELEFONE}}", dados.Telefone },
                { "{{NOME_DO_RESPONS_VEL}}", dados.NomeResponsavel },
                { "{{TELEFONE_DO_RESPONS_VEL}}", dados.TelefoneResponsavel },
                { "{{CARGO}}", dados.Cargo },
                { "{{EMAIL_DO_RESPONS_VEL}}", dados.EmailResponsavel },
            };

            // Percorre Run por Run (não Paragraph inteiro) e faz igualdade
            // exata de texto — nunca "Contains", pra não confundir
            // "{{EMAIL}}" com "{{EMAIL_DO_RESPONS_VEL}}", por exemplo.
            var runs = body.Descendants<Run>().ToList();

            foreach (var kv in mapeamento)
            {
                string token = kv.Key;
                string valor = kv.Value ?? "";
                bool encontrado = false;

                foreach (var run in runs)
                {
                    var textEl = run.Descendants<Text>().FirstOrDefault(t => t.Text == token);
                    if (textEl == null) continue;

                    textEl.Text = valor;
                    textEl.Space = SpaceProcessingModeValues.Preserve;

                    RunProperties rp = run.RunProperties ?? new RunProperties();
                    // remove cor anterior (se houver) e força azul
                    foreach (var c in rp.Elements<Color>().ToList()) c.Remove();
                    rp.Append(new Color() { Val = "0000FF" });
                    run.RunProperties = rp;

                    encontrado = true;
                    break; // token é único no documento
                }

                log.AppendLine($"[TEXTO] '{token}' => {(encontrado ? "OK" : "NÃO ENCONTRADO")}");
            }
        }

        // ─── VIGÊNCIA / VENCIMENTO ───
        // No template novo, cada opção (01/11/21, 10/15/25) é um parágrafo
        // próprio "☐ NN" dentro da célula — e tudo isso está só na
        // primeira tabela do documento (cabeçalho "VIGÊNCIA:"/"VENCIMENTO:").
        private void MarcarVigencias(Body body, List<string> vigencias, StringBuilder log)
        {
            var tabela = LocalizarTabelaVigenciaVencimento(body);
            foreach (var vigencia in vigencias ?? new List<string>())
            {
                string dia = ExtrairDia(vigencia);
                bool ok = MarcarParagrafoPorNumeroNaTabela(tabela, dia, coluna: 0);
                log.AppendLine($"[VIGÊNCIA] dia '{dia}' => {(ok ? "MARCADO" : "NÃO ENCONTRADO")}");
            }
        }

        private void MarcarVencimentos(Body body, List<string> vencimentos, StringBuilder log)
        {
            var tabela = LocalizarTabelaVigenciaVencimento(body);
            foreach (var vencimento in vencimentos ?? new List<string>())
            {
                string dia = ExtrairDia(vencimento);
                bool ok = MarcarParagrafoPorNumeroNaTabela(tabela, dia, coluna: 1);
                log.AppendLine($"[VENCIMENTO] dia '{dia}' => {(ok ? "MARCADO" : "NÃO ENCONTRADO")}");
            }
        }

        private Table LocalizarTabelaVigenciaVencimento(Body body)
        {
            return body.Descendants<Table>().FirstOrDefault(t =>
            {
                var header = t.Descendants<TableRow>().FirstOrDefault();
                if (header == null) return false;
                string textoHeader = string.Concat(header.Descendants<Text>().Select(x => x.Text)).ToUpperInvariant();
                return textoHeader.Contains("VIGÊNCIA") && textoHeader.Contains("VENCIMENTO");
            });
        }

        private string ExtrairDia(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return "";
            var partes = data.Trim().Split('/');
            return partes.Length > 0 ? partes[0].Trim() : data.Trim();
        }

        // Busca, na coluna indicada (0 = vigência, 1 = vencimento) da
        // segunda linha da tabela, o parágrafo cujo texto (sem o ☐) é
        // exatamente igual ao número, e marca o checkbox daquele parágrafo.
        private bool MarcarParagrafoPorNumeroNaTabela(Table tabela, string numero, int coluna)
        {
            if (tabela == null) return false;
            var linhaDados = tabela.Descendants<TableRow>().Skip(1).FirstOrDefault();
            if (linhaDados == null) return false;

            var celulas = linhaDados.Descendants<TableCell>().ToList();
            if (coluna >= celulas.Count) return false;

            var paragrafos = celulas[coluna].Descendants<Paragraph>().ToList();
            foreach (var p in paragrafos)
            {
                string texto = string.Concat(p.Descendants<Text>().Select(t => t.Text))
                    .Replace("☐", "").Replace("☒", "").Trim();
                if (texto == numero.Trim())
                {
                    return MarcarCheckboxNoParagrafo(p) > 0;
                }
            }
            return false;
        }

        // ─── PRODUTOS (tabela "4. CARACTERÍSTICAS GERAIS") ───
        // Layout real: cells[0] = ☐ + nome do produto | cells[1] = REDE |
        // cells[2] = COBERTURA | cells[3] = REGISTRO-ANS
        private void MarcarProdutos(Body body, List<string> produtos, StringBuilder log)
        {
            foreach (var produto in produtos ?? new List<string>())
            {
                bool ok = MarcarLinhaTabelaProduto(body, produto);
                log.AppendLine($"[PRODUTO] '{produto}' => {(ok ? "OK" : "NÃO ENCONTRADO")}");
            }
        }

        private bool MarcarLinhaTabelaProduto(Body body, string produtoCsv)
        {
            string produtoUpper = (produtoCsv ?? "").ToUpperInvariant();
            var tables = body.Descendants<Table>().ToList();
            foreach (var table in tables)
            {
                foreach (var row in table.Descendants<TableRow>())
                {
                    var cells = row.Descendants<TableCell>().ToList();
                    if (cells.Count < 4) continue;

                    string textoProduto = string.Concat(cells[0].Descendants<Text>().Select(t => t.Text))
                        .Replace("☐", "").Replace("☒", "").ToUpperInvariant().Trim();
                    string textoCobertura = string.Concat(cells[2].Descendants<Text>().Select(t => t.Text))
                        .ToUpperInvariant().Trim();

                    if (string.IsNullOrEmpty(textoProduto) || string.IsNullOrEmpty(textoCobertura)) continue;

                    // precisa bater produto E cobertura, pra diferenciar
                    // "UNIPART FLEX ENFERMARIA" de "UNIPART FLEX APARTAMENTO"
                    if (produtoUpper.Contains(textoProduto) && produtoUpper.Contains(textoCobertura))
                    {
                        var paragrafo = cells[0].Descendants<Paragraph>().FirstOrDefault();
                        return MarcarCheckboxNoParagrafo(paragrafo) > 0;
                    }
                }
            }
            return false;
        }

        // ─── MENSALIDADES (tabela "5.1 VALOR DA MENSALIDADE...") ───
        // cells[0] = ☐ + descrição (ex: "5.1.1 CONTRATAÇÃO UNITÁRIA UNIFÁCIL") | cells[1] = valor
        private void MarcarMensalidades(Body body, List<string> mensalidades, StringBuilder log)
        {
            foreach (var mensalidade in mensalidades ?? new List<string>())
            {
                bool ok = MarcarLinhaTabelaPorDescricao(body, mensalidade);
                log.AppendLine($"[MENSALIDADE] '{mensalidade}' => {(ok ? "OK" : "NÃO ENCONTRADO")}");
            }
        }

        private bool MarcarLinhaTabelaPorDescricao(Body body, string descricaoCsv)
        {
            string descricaoUpper = (descricaoCsv ?? "").ToUpperInvariant().Trim();
            var tables = body.Descendants<Table>().ToList();
            foreach (var table in tables)
            {
                foreach (var row in table.Descendants<TableRow>())
                {
                    var cells = row.Descendants<TableCell>().ToList();
                    if (cells.Count < 2) continue;

                    string textoDescricao = string.Concat(cells[0].Descendants<Text>().Select(t => t.Text))
                        .Replace("☐", "").Replace("☒", "").ToUpperInvariant().Trim();
                    if (string.IsNullOrEmpty(textoDescricao)) continue;

                    if (descricaoUpper.Contains(textoDescricao) || textoDescricao.Contains(descricaoUpper))
                    {
                        var paragrafo = cells[0].Descendants<Paragraph>().FirstOrDefault();
                        return MarcarCheckboxNoParagrafo(paragrafo) > 0;
                    }
                }
            }
            return false;
        }

        // ─── OPCIONAIS (tabela "6. PRODUTOS E SERVIÇOS OPCIONAIS") ───
        private void MarcarOpcionais(Body body, string aeromedico, string odontologia, StringBuilder log)
        {
            if (EhSim(aeromedico))
            {
                bool ok = MarcarLinhaTabelaPorTipo(body, "AEROMÉDICO");
                log.AppendLine($"[AEROMÉDICO] => {(ok ? "OK" : "NÃO ENCONTRADO")}");
            }
            if (EhSim(odontologia))
            {
                bool ok = MarcarLinhaTabelaPorTipo(body, "ODONTOLOGIA");
                log.AppendLine($"[ODONTOLOGIA] => {(ok ? "OK" : "NÃO ENCONTRADO")}");
            }
        }

        private bool EhSim(string valor) => string.Equals(valor?.Trim(), "SIM", StringComparison.OrdinalIgnoreCase);

        private bool MarcarLinhaTabelaPorTipo(Body body, string tipoUpper)
        {
            var tables = body.Descendants<Table>().ToList();
            foreach (var table in tables)
            {
                foreach (var row in table.Descendants<TableRow>())
                {
                    var cells = row.Descendants<TableCell>().ToList();
                    if (cells.Count < 2) continue;

                    string textoTipo = string.Concat(cells[0].Descendants<Text>().Select(t => t.Text))
                        .Replace("☐", "").Replace("☒", "").ToUpperInvariant().Trim();
                    if (textoTipo.Contains(tipoUpper))
                    {
                        var paragrafo = cells[0].Descendants<Paragraph>().FirstOrDefault();
                        return MarcarCheckboxNoParagrafo(paragrafo) > 0;
                    }
                }
            }
            return false;
        }

        // ─── HELPER ÚNICO DE CHECKBOX ───
        // Todos os checkboxes do template são o caractere real "☐" dentro
        // de um Text run do parágrafo — então a troca é sempre ☐ -> ☒.
        private int MarcarCheckboxNoParagrafo(Paragraph paragrafo)
        {
            if (paragrafo == null) return 0;

            foreach (var text in paragrafo.Descendants<Text>())
            {
                if (text.Text.Contains("☐"))
                {
                    text.Text = text.Text.Replace("☐", "☒");

                    var parentRun = text.Parent as Run;
                    if (parentRun != null)
                    {
                        RunProperties rp = parentRun.RunProperties ?? new RunProperties();
                        if (!rp.Elements<Color>().Any())
                            rp.Append(new Color() { Val = "0000FF" });
                        parentRun.RunProperties = rp;
                    }
                    return 1;
                }
            }
            return 0;
        }
    }
}