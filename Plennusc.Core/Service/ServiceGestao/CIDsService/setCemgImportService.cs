using Plennusc.Core.Models.ModelsGestao.modelsCIDs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Plennusc.Core.Service.ServiceGestao.CIDsService
{
    public class setCemgImportService
    {
        private const string NomeAba = "Planilha Padrao";
        private const int LinhaCabecalho = 2;
        private const int PrimeiraLinhaDados = 3;
        private const int TotalColunas = 50;      // A..AX
        private const int ColunasLayout = 49;     // A..AW

        private static readonly int[] Tamanhos =
        {
            7, 7, 17, 17, 9, 2, 8, 100, 1, 1, 1, 8, 11, 2, 100, 20, 8, 60, 9, 25,
            30, 40, 2, 15, 15, 60, 6, 4, 4, 4, 4, 4, 4, 6, 6, 4, 17, 8, 17, 50, 11,
            15, 3, 3, 9, 1, 8, 4, 100
        };

        private static readonly Dictionary<int, int> DigitosFixos = new Dictionary<int, int>
        {
            { 1, 7 }, { 7, 8 }, { 12, 8 }, { 13, 11 }, { 14, 2 }, { 17, 8 }, { 38, 8 }, { 47, 8 }
        };

        private static readonly string[] TiposMovimentacao =
            { "IC", "ID", "AD", "EC", "IR", "VC", "IM", "EM", "ER" };

        private static readonly XNamespace NsMain = "http://schemas.openxmlformats.org/spreadsheetml/2006/main";
        private static readonly XNamespace NsRel = "http://schemas.openxmlformats.org/officeDocument/2006/relationships";
        private static readonly XNamespace NsPkgRel = "http://schemas.openxmlformats.org/package/2006/relationships";

        public setCemgImportResultado Ler(Stream arquivo)
        {
            var resultado = new setCemgImportResultado();

            using (var zip = new ZipArchive(arquivo, ZipArchiveMode.Read, true))
            {
                var caminhoAba = LocalizarAba(zip, NomeAba);
                if (caminhoAba == null)
                {
                    resultado.Erros.Add(new setCemgImportErro
                    {
                        LinhaExcel = 0,
                        Campo = "Arquivo",
                        Mensagem = "Aba '" + NomeAba + "' não encontrada. Verifique se é a planilha padrão de carga."
                    });
                    return resultado;
                }

                var textos = LerTextosCompartilhados(zip);
                var estiloData = LerEstilosDeData(zip);
                var celulas = LerCelulas(zip, caminhoAba, textos, estiloData); // linha -> (coluna -> valor)

                if (celulas.Count == 0) return resultado;

                var cabecalhos = new string[TotalColunas];
                for (int c = 1; c <= TotalColunas; c++)
                    cabecalhos[c - 1] = Obter(celulas, LinhaCabecalho, c);

                int ultimaLinha = celulas.Keys.Max();

                for (int l = PrimeiraLinhaDados; l <= ultimaLinha; l++)
                {
                    if (!celulas.ContainsKey(l)) continue;

                    var v = new string[TotalColunas];
                    for (int c = 1; c <= TotalColunas; c++)
                        v[c - 1] = Normalizar(c, Obter(celulas, l, c));

                    if (v.Skip(1).All(string.IsNullOrEmpty)) continue;

                    Validar(l, v, cabecalhos, resultado);
                    resultado.Linhas.Add(setCemgImportModels.De(l, v));
                }
            }

            return resultado;
        }

        // ---------------------------------------------------------------- leitura do xlsx (sem libs externas)

        private static string Obter(Dictionary<int, Dictionary<int, string>> cel, int linha, int coluna)
        {
            Dictionary<int, string> row;
            string valor;
            if (cel.TryGetValue(linha, out row) && row.TryGetValue(coluna, out valor)) return valor;
            return string.Empty;
        }

        private static XDocument AbrirXml(ZipArchive zip, string caminho)
        {
            var entrada = zip.GetEntry(caminho.TrimStart('/'));
            if (entrada == null) return null;
            using (var s = entrada.Open())
                return XDocument.Load(s);
        }

        private static string LocalizarAba(ZipArchive zip, string nomeAba)
        {
            var wb = AbrirXml(zip, "xl/workbook.xml");
            var rels = AbrirXml(zip, "xl/_rels/workbook.xml.rels");
            if (wb == null || rels == null) return null;

            var sheet = wb.Descendants(NsMain + "sheet")
                .FirstOrDefault(s => string.Equals((string)s.Attribute("name"), nomeAba, StringComparison.OrdinalIgnoreCase));
            if (sheet == null) return null;

            var rid = (string)sheet.Attribute(NsRel + "id");
            var rel = rels.Descendants(NsPkgRel + "Relationship").FirstOrDefault(r => (string)r.Attribute("Id") == rid);
            if (rel == null) return null;

            var alvo = (string)rel.Attribute("Target");
            return alvo.StartsWith("/") ? alvo.TrimStart('/') : "xl/" + alvo;
        }

        private static List<string> LerTextosCompartilhados(ZipArchive zip)
        {
            var lista = new List<string>();
            var doc = AbrirXml(zip, "xl/sharedStrings.xml");
            if (doc == null) return lista;

            foreach (var si in doc.Descendants(NsMain + "si"))
            {
                // concatena <t> (inclusive rich text), ignorando fonética (rPh)
                var sb = new StringBuilder();
                foreach (var t in si.Descendants(NsMain + "t"))
                {
                    if (t.Ancestors(NsMain + "rPh").Any()) continue;
                    sb.Append(t.Value);
                }
                lista.Add(sb.ToString());
            }
            return lista;
        }

        // índice do estilo (s="") -> true se for formato de data
        private static bool[] LerEstilosDeData(ZipArchive zip)
        {
            var doc = AbrirXml(zip, "xl/styles.xml");
            if (doc == null) return new bool[0];

            var formatosCustom = new Dictionary<int, string>();
            foreach (var nf in doc.Descendants(NsMain + "numFmt"))
                formatosCustom[(int)nf.Attribute("numFmtId")] = (string)nf.Attribute("formatCode");

            var cellXfs = doc.Descendants(NsMain + "cellXfs").FirstOrDefault();
            if (cellXfs == null) return new bool[0];

            return cellXfs.Elements(NsMain + "xf").Select(xf =>
            {
                int id = (int?)xf.Attribute("numFmtId") ?? 0;
                string codigo;
                if (formatosCustom.TryGetValue(id, out codigo)) return CodigoEhData(codigo);
                return (id >= 14 && id <= 22) || (id >= 27 && id <= 36) || (id >= 45 && id <= 47) || (id >= 50 && id <= 58);
            }).ToArray();
        }

        private static bool CodigoEhData(string codigo)
        {
            if (string.IsNullOrEmpty(codigo)) return false;
            var limpo = Regex.Replace(codigo, "\"[^\"]*\"|\\[[^\\]]*\\]|\\\\.", string.Empty);
            return Regex.IsMatch(limpo, "[dmyhs]", RegexOptions.IgnoreCase);
        }

        private static Dictionary<int, Dictionary<int, string>> LerCelulas(
            ZipArchive zip, string caminho, List<string> textos, bool[] estiloData)
        {
            var resultado = new Dictionary<int, Dictionary<int, string>>();
            var doc = AbrirXml(zip, caminho);
            if (doc == null) return resultado;

            foreach (var c in doc.Descendants(NsMain + "c"))
            {
                var r = (string)c.Attribute("r");
                if (string.IsNullOrEmpty(r)) continue;

                int linha, coluna;
                if (!SepararReferencia(r, out linha, out coluna)) continue;
                if (coluna > TotalColunas) continue;

                var valor = LerValor(c, textos, estiloData);
                if (string.IsNullOrEmpty(valor)) continue;

                Dictionary<int, string> row;
                if (!resultado.TryGetValue(linha, out row))
                {
                    row = new Dictionary<int, string>();
                    resultado[linha] = row;
                }
                row[coluna] = valor;
            }

            return resultado;
        }

        private static string LerValor(XElement c, List<string> textos, bool[] estiloData)
        {
            var tipo = (string)c.Attribute("t");
            var v = (string)c.Element(NsMain + "v");

            if (tipo == "inlineStr")
            {
                var is_ = c.Element(NsMain + "is");
                return is_ == null ? string.Empty
                    : string.Concat(is_.Descendants(NsMain + "t").Where(t => !t.Ancestors(NsMain + "rPh").Any()).Select(t => t.Value)).Trim();
            }

            if (v == null) return string.Empty;

            if (tipo == "s")
            {
                int idx;
                return int.TryParse(v, out idx) && idx >= 0 && idx < textos.Count ? textos[idx].Trim() : string.Empty;
            }
            if (tipo == "str") return v.Trim();
            if (tipo == "b") return v;
            if (tipo == "e") return string.Empty;

            // número (t="n" ou ausente)
            double num;
            if (!double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out num))
                return v.Trim();

            int estilo = (int?)c.Attribute("s") ?? 0;
            if (estilo >= 0 && estilo < estiloData.Length && estiloData[estilo])
                return DateTime.FromOADate(num).ToString("ddMMyyyy", CultureInfo.InvariantCulture);

            return num.ToString("0.##", CultureInfo.InvariantCulture);
        }

        // "AX12" -> linha 12, coluna 50
        private static bool SepararReferencia(string referencia, out int linha, out int coluna)
        {
            linha = 0; coluna = 0;
            int i = 0;
            while (i < referencia.Length && char.IsLetter(referencia[i]))
            {
                coluna = coluna * 26 + (char.ToUpperInvariant(referencia[i]) - 'A' + 1);
                i++;
            }
            return i > 0 && int.TryParse(referencia.Substring(i), out linha);
        }

        // ---------------------------------------------------------------- normalização / validação (inalterados)

        private static string Normalizar(int coluna, string valor)
        {
            if (string.IsNullOrEmpty(valor)) return string.Empty;

            int digitos;
            if (DigitosFixos.TryGetValue(coluna, out digitos)
                && valor.Length < digitos
                && valor.All(char.IsDigit))
            {
                return valor.PadLeft(digitos, '0');
            }

            return valor;
        }

        private static void Validar(int linha, string[] v, string[] cabecalhos, setCemgImportResultado resultado)
        {
            var tipoMov = v[5];

            if (!TiposMovimentacao.Contains(tipoMov))
                Add(resultado, linha, cabecalhos[5], "Tipo de movimentação vazio ou inválido: '" + tipoMov + "'.");

            if ((tipoMov == "IC" || tipoMov == "ID") && string.IsNullOrEmpty(v[7]))
                Add(resultado, linha, cabecalhos[7], "Nome do cliente é obrigatório para inclusão.");

            for (int c = 0; c < ColunasLayout; c++)
            {
                if (v[c].Length > Tamanhos[c])
                    Add(resultado, linha, cabecalhos[c],
                        "Valor com " + v[c].Length + " caracteres excede o limite de " + Tamanhos[c] + ".");
            }
        }

        private static void Add(setCemgImportResultado r, int linha, string campo, string msg)
        {
            r.Erros.Add(new setCemgImportErro { LinhaExcel = linha, Campo = campo, Mensagem = msg });
        }
    }
}