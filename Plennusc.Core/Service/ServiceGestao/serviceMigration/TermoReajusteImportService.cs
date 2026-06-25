using Plennusc.Core.Models.ModelsGestao.modelsMigration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.serviceMigration
{
    public class TermoReajusteImportService
    {
        // ── Mapeamento de cabeçalhos aceitos → propriedade interna ────────
        private static readonly Dictionary<string, string> MapCabecalho =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "NOME",          "Nome"        },
                { "NOME COMPLETO", "Nome"        },
                { "CPF",           "Cpf"         },
                { "EMAIL",         "Email"       },
                { "E-MAIL",        "Email"       },
                { "NOME_PRODUTO",  "NomeProduto" },
                { "PRODUTO",       "NomeProduto" },
                { "PLANO",         "NomeProduto" },
                { "VALOR",         "Valor"       },
                { "DATA",          "Data"        },
                { "TIPO ASSOCIADO", "Tipo" },
                { "TIPO",           "Tipo" },
            };

        // ─────────────────────────────────────────────────────────────────
        public List<TermoReajusteVallor> ImportarCsv(string caminhoArquivo)
        {
            if (!File.Exists(caminhoArquivo))
                throw new FileNotFoundException($"Arquivo não encontrado: {caminhoArquivo}");

            // Tentar UTF-8; fallback para Latin-1 (Windows-1252)
            string[] linhas;
            try { linhas = File.ReadAllLines(caminhoArquivo, Encoding.UTF8); }
            catch { linhas = File.ReadAllLines(caminhoArquivo, Encoding.GetEncoding(1252)); }

            if (linhas.Length < 2)
                throw new Exception("CSV vazio ou sem linhas de dados.");

            // ── Auto-detectar separador ───────────────────────────────────
            char sep = DetectarSeparador(linhas[0]);

            // ── Mapear colunas do cabeçalho ───────────────────────────────
            var cabecalho = SplitLinha(linhas[0], sep);
            var mapa = MapearColunas(cabecalho);   // propriedade → índice

            // ── Ler todas as linhas brutas ────────────────────────────────
            var linhasBrutas = new List<Dictionary<string, string>>();
            for (int i = 1; i < linhas.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(linhas[i])) continue;
                var cols = SplitLinha(linhas[i], sep);
                var row = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var kvp in mapa)
                    row[kvp.Key] = kvp.Value < cols.Length ? cols[kvp.Value].Trim() : "";
                linhasBrutas.Add(row);
            }

            // ── Agrupar titular + dependentes ────────────────────────────
            return AgruparTitulares(linhasBrutas);
        }

        // ─────────────────────────────────────────────────────────────────
        private List<TermoReajusteVallor> AgruparTitulares(
     List<Dictionary<string, string>> linhas)
        {
            var resultado = new List<TermoReajusteVallor>();
            TermoReajusteVallor titularAtual = null;
            string cpfTitularAtual = null;

            foreach (var row in linhas)
            {
                string nome = Valor(row, "Nome");
                string cpf = LimparCpf(Valor(row, "Cpf"));
                string email = Valor(row, "Email");
                string prod = Valor(row, "NomeProduto");
                string val = FormatarValor(Valor(row, "Valor"));
                string data = Valor(row, "Data");
                string tipo = Valor(row, "Tipo").ToUpperInvariant().Trim();

                bool ehTitular;
                bool ehDependente;

                if (!string.IsNullOrWhiteSpace(tipo))
                {
                    // Usa coluna TIPO ASSOCIADO quando disponível
                    ehTitular = tipo == "T" || tipo == "TITULAR";
                    ehDependente = tipo == "D" || tipo == "DEPENDENTE";
                }
                else
                {
                    // Fallback: decide pelo CPF
                    ehTitular = !string.IsNullOrWhiteSpace(cpf) && cpf != cpfTitularAtual;
                    ehDependente = !ehTitular && !string.IsNullOrWhiteSpace(nome);
                }

                if (ehTitular)
                {
                    if (titularAtual != null)
                        resultado.Add(titularAtual);

                    titularAtual = new TermoReajusteVallor
                    {
                        Nome = nome,
                        Cpf = cpf,
                        Email = email,
                        NomeProduto = prod,
                        Valor = val,
                        Data = data,
                    };
                    cpfTitularAtual = cpf;
                }
                else if (ehDependente && titularAtual != null)
                {
                    string cpfDep = string.IsNullOrWhiteSpace(cpf) ? cpfTitularAtual : cpf;

                    titularAtual.Dependentes.Add(new TermoReajusteDependente
                    {
                        Nome = nome,
                        Cpf = cpfDep,
                        Email = email,
                        NomeProduto = prod,
                        Valor = val,
                        Data = data,
                    });
                }
            }

            if (titularAtual != null)
                resultado.Add(titularAtual);

            return resultado;
        }

        // ── Helpers ───────────────────────────────────────────────────────
        private char DetectarSeparador(string cabecalho)
        {
            int pontoVirgulas = cabecalho.Count(c => c == ';');
            int virgulas = cabecalho.Count(c => c == ',');
            return pontoVirgulas >= virgulas ? ';' : ',';
        }

        private string[] SplitLinha(string linha, char sep)
        {
            // Suporte a campos entre aspas
            var campos = new List<string>();
            bool dentroAspas = false;
            var atual = new StringBuilder();

            foreach (char c in linha)
            {
                if (c == '"') { dentroAspas = !dentroAspas; continue; }
                if (c == sep && !dentroAspas)
                {
                    campos.Add(atual.ToString().Trim());
                    atual.Clear();
                }
                else atual.Append(c);
            }
            campos.Add(atual.ToString().Trim());
            return campos.ToArray();
        }

        private Dictionary<string, int> MapearColunas(string[] cabecalho)
        {
            var mapa = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < cabecalho.Length; i++)
            {
                string h = cabecalho[i].Trim().Trim('"');
                if (MapCabecalho.TryGetValue(h, out string prop))
                    mapa[prop] = i;
            }
            return mapa;
        }

        private string Valor(Dictionary<string, string> row, string chave)
            => row.TryGetValue(chave, out string v) ? v ?? "" : "";

        private string LimparCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return "";
            // Manter apenas dígitos e pontuação original
            return cpf.Trim().Trim('"');
        }

        /// <summary>
        /// Garante que o valor venha no formato "R$ 1.234,56".
        /// Aceita entradas como "1234.56", "R$ 1.234,56", "1.234,56".
        /// </summary>
        private string FormatarValor(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "";
            if (raw.TrimStart().StartsWith("R$")) return raw.Trim();

            // Tentar converter número puro para moeda pt-BR
            string limpo = raw.Replace("R$", "").Replace(" ", "").Trim();
            // Troca vírgula decimal por ponto (caso venha 1.234,56)
            if (limpo.Contains(",") && limpo.Contains("."))
                limpo = limpo.Replace(".", "").Replace(",", ".");
            else if (limpo.Contains(","))
                limpo = limpo.Replace(",", ".");

            if (decimal.TryParse(limpo,
                    System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture,
                    out decimal d))
                return d.ToString("C2", new System.Globalization.CultureInfo("pt-BR"));

            return raw.Trim(); // devolve como veio se não conseguiu parsear
        }
    }
}