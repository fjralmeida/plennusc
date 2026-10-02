using ClosedXML.Excel;
using Plennusc.Core.Models.ModelsGestao.modelsCIDs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.CIDsService
{
    /// <summary>Plano que será gravado no layout final (COD_PLANO / COD_ANS_PLANO).</summary>
    public class setCemgPlano
    {
        public string Nome { get; set; }          // texto exibido no filtro
        public string CodPlano { get; set; }      // COD_PLANO
        public string CodAnsPlano { get; set; }   // COD_ANS_PLANO
    }

    /// <summary>
    /// Transforma as linhas lidas da planilha de carga (setCemgImportModels)
    /// no layout de 101 colunas e gera o .xlsx (sem bibliotecas externas).
    /// </summary>
    public class setCemgExportService
    {
        // ------------------------------------------------------------ configuração

        private const string FormatoData = "dd/MM/yyyy";   // use "ddMMyyyy" se o destino não quiser barras
        public const string PreencherManualmente = "PREENCHER MANUALMENTE";

        // Valores fixos
        private const string CodVend = "12091127000142";
        private const string CodSuperv = "12091127000142";
        private const string CodProfissao = "59587393600";
        private const string CnpjOperadora = "16513178000176";
        private const string CodAnsOperadora = "343889";
        private const string NomeOperadora = "UNIMED";
        private const string NomeVendedor = "VENDA INTERNA - ALEXANDRE ANTONIO SA SOARES";
        private const string CpfVendedor = "59587393600";
        private const string CodForma = "BU";
        private const string DiaVenc = "10";
        private const string TipoContrato = "PJ";
        private const string NomeEntidade = "SETCEMG";
        private const string CnpjEntidade = "17433780000166";
        private const string TipoMovimentacaoFixo = "I";

        // Campos da empresa que a operadora não disponibiliza no layout.
        private static readonly string[] CamposManuais =
        {
            "RAZAO_SOCIAL", "NOME_FANTASIA", "ENDERECO_EMPRESA", "BAIRRO_EMPRESA", "CEP_EMPRESA",
            "ESTADO_EMPRESA", "CIDADE_EMPRESA", "CNPJ", "DATA_INSC_CNPJ", "NUMERO_INSC_MUNICIPAL",
            "NUMERO_INSC_ESTADUAL", "EMAIL_EMPRESA", "TELEFONE_EMPRESA"
        };

        // ------------------------------------------------------------ planos (filtro da tela)

        // Preencha com os planos. Depois isso pode vir da PS1030 (basta trocar esta lista por uma consulta).
        // Exemplo:
        // new setCemgPlano { Nome = "UNIPART ENFERMARIA", CodPlano = "363", CodAnsPlano = "475303168" },
        public static readonly List<setCemgPlano> Planos = new List<setCemgPlano>
        {
        };

        // ------------------------------------------------------------ DE/PARA

        // Estado civil (coluna K)
        private static readonly Dictionary<string, string> DeParaEstadoCivil =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "C", "Casado(a)" },
            { "S", "Solteiro(a)" },
            { "D", "Outros" },
            { "V", "Outros" },
            { "A", "Outros" }
        };

        // Parentesco (coluna J). Titular fica vazio.
        private static readonly Dictionary<string, string> DeParaParentesco =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "T", "" },
            { "C", "2" },
            { "F", "1" },
            { "P", "3" },
            { "S", "11" },
            { "I", "8" },
            { "A", "10" }
        };

        // Prefixo do logradouro (primeira palavra da coluna R) -> tipo por extenso.
        private static readonly Dictionary<string, string> TiposLogradouro =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "R", "RUA" }, { "RUA", "RUA" },
            { "AV", "AVENIDA" }, { "AVENIDA", "AVENIDA" },
            { "AL", "ALAMEDA" }, { "ALAMEDA", "ALAMEDA" },
            { "TV", "TRAVESSA" }, { "TRAV", "TRAVESSA" }, { "TRAVESSA", "TRAVESSA" },
            { "PC", "PRACA" }, { "PCA", "PRACA" }, { "PRACA", "PRACA" },
            { "ROD", "RODOVIA" }, { "RODOVIA", "RODOVIA" },
            { "EST", "ESTRADA" }, { "ESTRADA", "ESTRADA" },
            { "BC", "BECO" }, { "BECO", "BECO" },
            { "LRG", "LARGO" }, { "LARGO", "LARGO" }
        };

        // ------------------------------------------------------------ layout de saída (ordem das colunas)

        public static readonly string[] Cabecalhos =
        {
            "NOME", "ID", "ORG_EXP", "CPF", "PIS", "CNS",
            "DT_NASC", "NATURALIDADE", "SEXO", "EST_CIVIL", "TIPO_LOGRADOURO", "ENDERECO",
            "BAIRRO", "CIDADE", "UF", "CEP", "DDD_TEL", "TEL",
            "DDD_TEL_2", "TEL_2", "DDD_CEL", "CELULAR", "COD_PLANO", "DT_INCL",
            "DT_VIGENCIA", "COD_EMPRESA", "COD_UNID", "MAT", "ADMISSÃO", "NOME_MAE",
            "NOME_PAI", "EMAIL", "COD_RESP", "PARENT", "NUMERO_VIDAS", "UNIVER",
            "NR_DEC_NASC_VIVO", "AGREGADO", "DEF_INVALIDO", "COD_LOTACAO", "TIPO_MOVIMENTACAO", "DATA_EXCLUSAO",
            "MOTIVO_EXCLUSAO", "COD_OUTRO", "COD_GSEG", "COD_VEND", "COD_PROP", "OBS",
            "OBS_TEC", "MOSTRA_LIB", "COD_FORMA", "DIA_VENC", "COD_TABCOM", "CARGO",
            "RESPONSAVEL", "DT_CASAMENTO", "UF_ORGAO", "COD_GRP", "COD_SUPERV", "COD_PROFISSAO",
            "CNPJ_OPERADORA", "COD_ANS_OPERADORA", "COD_ANS_PLANO", "NOME_OPERADORA", "IDADE", "RESPF_MÃE",
            "RESPF_NASCIMENTO", "RESPF_EMAIL", "RESPF_IDADE", "RESPF_SEXO", "RESPF_ESTADOCIVIL", "RESPF_RG",
            "RESPF_ORG_EXP", "RESPF_CNS", "NOME_VENDEDOR", "CPF_VENDEDOR", "EMAIL_VENDEDOR", "TELEFONE_VENDEDOR",
            "MENSALIDADE_TIT", "MENSALIDADE_DEP", "ACESSÓRIOS", "LINK_PROPOSA", "TP_PROPOSTA", "RAZAO_SOCIAL",
            "NOME_FANTASIA", "ENDERECO_EMPRESA", "BAIRRO_EMPRESA", "CEP_EMPRESA", "ESTADO_EMPRESA", "CIDADE_EMPRESA",
            "CNPJ", "DATA_INSC_CNPJ", "NUMERO_INSC_MUNICIPAL", "NUMERO_INSC_ESTADUAL", "EMAIL_EMPRESA", "TELEFONE_EMPRESA",
            "TIPO_CONTRATO", "STATUS", "NOME_ENTIDADE", "CNPJ_ENTIDADE", "DT_LIBERACAO_DOCUMENTAL"

        };

        // ------------------------------------------------------------ transformação

        public setCemgExportResultado Transformar(IEnumerable<setCemgImportModels> linhas, string cnpjEmpresa, setCemgPlano plano)
        {
            var resultado = new setCemgExportResultado();
            string cpfTitularAtual = string.Empty;

            // Avisos gerais (uma vez só, não por linha)
            AvisoGeral(resultado, "Campos da empresa",
                PreencherManualmente + " (operadora não disponibiliza esta informação em layout): " + string.Join(", ", CamposManuais) + ".");

            if (string.IsNullOrEmpty(cnpjEmpresa))
                AvisoGeral(resultado, "COD_PROP", "CNPJ da empresa não encontrado no cabeçalho da planilha. " + PreencherManualmente + ".");

            if (plano == null)
                AvisoGeral(resultado, "COD_PLANO / COD_ANS_PLANO", "Nenhum plano selecionado no filtro. " + PreencherManualmente + ".");

            foreach (var l in linhas)
            {
                var isTitular = string.Equals(l.Parentesco, "T", StringComparison.OrdinalIgnoreCase);
                if (isTitular) cpfTitularAtual = l.Cpf ?? string.Empty;

                if (l.TipoMovimentacao != "IC" && l.TipoMovimentacao != "ID")
                    Aviso(resultado, l, "Tp Mov", "Movimentação '" + l.TipoMovimentacao + "' exportada como 'I' (layout fixo).");

                if (!isTitular && string.IsNullOrEmpty(cpfTitularAtual))
                    Aviso(resultado, l, "Parent", "Dependente sem titular anterior na planilha: COD_RESP ficou vazio.");

                var r = new Dictionary<string, string>();

                r["NOME"] = l.NomeCliente;
                r["CPF"] = l.Cpf;
                r["DT_NASC"] = FormatarData(l.DataNascimento);
                r["SEXO"] = l.Sexo;
                r["EST_CIVIL"] = DePara(DeParaEstadoCivil, l.EstadoCivil, l, "Est Civ", resultado);

                string tipoLog, nomeLog;
                SepararLogradouro(l.Logradouro, out tipoLog, out nomeLog);
                r["TIPO_LOGRADOURO"] = tipoLog;
                r["ENDERECO"] = MontarEndereco(nomeLog, l.Numero, l.Complemento);

                r["BAIRRO"] = l.Bairro;
                r["CIDADE"] = l.Cidade;
                r["UF"] = l.Uf;
                r["CEP"] = l.Cep;

                string ddd, cel;
                SepararTelefone(l.Celular, out ddd, out cel);
                r["DDD_CEL"] = ddd;
                r["CELULAR"] = cel;

                r["COD_PLANO"] = plano != null ? plano.CodPlano : PreencherManualmente;
                r["COD_ANS_PLANO"] = plano != null ? plano.CodAnsPlano : PreencherManualmente;

                r["DT_INCL"] = FormatarData(l.DataVigencia);
                r["DT_VIGENCIA"] = FormatarData(l.DataVigencia);
                r["NOME_MAE"] = l.NomeMae;
                r["EMAIL"] = l.Email;
                r["COD_RESP"] = isTitular ? string.Empty : cpfTitularAtual;
                r["PARENT"] = DePara(DeParaParentesco, l.Parentesco, l, "Parent", resultado);
                r["NR_DEC_NASC_VIVO"] = l.DeclaracaoNascidoVivo;

                r["TIPO_MOVIMENTACAO"] = TipoMovimentacaoFixo;
                r["COD_VEND"] = CodVend;
                r["COD_PROP"] = string.IsNullOrEmpty(cnpjEmpresa) ? PreencherManualmente : cnpjEmpresa;
                r["COD_FORMA"] = CodForma;
                r["DIA_VENC"] = DiaVenc;
                r["COD_SUPERV"] = CodSuperv;
                r["COD_PROFISSAO"] = CodProfissao;
                r["CNPJ_OPERADORA"] = CnpjOperadora;
                r["COD_ANS_OPERADORA"] = CodAnsOperadora;
                r["NOME_OPERADORA"] = NomeOperadora;
                r["NOME_VENDEDOR"] = NomeVendedor;
                r["CPF_VENDEDOR"] = CpfVendedor;
                r["TIPO_CONTRATO"] = TipoContrato;
                r["NOME_ENTIDADE"] = NomeEntidade;
                r["CNPJ_ENTIDADE"] = CnpjEntidade;

                foreach (var campo in CamposManuais)
                    r[campo] = PreencherManualmente;

                resultado.Linhas.Add(Cabecalhos.Select(h =>
                {
                    string x;
                    return r.TryGetValue(h, out x) ? (x ?? string.Empty) : string.Empty;
                }).ToArray());
            }

            return resultado;
        }

        // ------------------------------------------------------------ geração do xlsx (sem ClosedXML)

        public byte[] GerarXlsx(IList<string[]> linhas)
        {
            using (var ms = new MemoryStream())
            {
                using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
                {
                    const string hdr = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>";

                    EscreverEntrada(zip, "[Content_Types].xml", hdr +
                        "<Types xmlns=\"http://schemas.openxmlformats.org/package/2006/content-types\">" +
                        "<Default Extension=\"rels\" ContentType=\"application/vnd.openxmlformats-package.relationships+xml\"/>" +
                        "<Default Extension=\"xml\" ContentType=\"application/xml\"/>" +
                        "<Override PartName=\"/xl/workbook.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml\"/>" +
                        "<Override PartName=\"/xl/worksheets/sheet1.xml\" ContentType=\"application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml\"/>" +
                        "</Types>");

                    EscreverEntrada(zip, "_rels/.rels", hdr +
                        "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                        "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument\" Target=\"xl/workbook.xml\"/>" +
                        "</Relationships>");

                    EscreverEntrada(zip, "xl/workbook.xml", hdr +
                        "<workbook xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\" xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\">" +
                        "<sheets><sheet name=\"Layout\" sheetId=\"1\" r:id=\"rId1\"/></sheets></workbook>");

                    EscreverEntrada(zip, "xl/_rels/workbook.xml.rels", hdr +
                        "<Relationships xmlns=\"http://schemas.openxmlformats.org/package/2006/relationships\">" +
                        "<Relationship Id=\"rId1\" Type=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet\" Target=\"worksheets/sheet1.xml\"/>" +
                        "</Relationships>");

                    var sb = new StringBuilder();
                    sb.Append(hdr);
                    sb.Append("<worksheet xmlns=\"http://schemas.openxmlformats.org/spreadsheetml/2006/main\"><sheetData>");

                    AppendLinha(sb, 1, Cabecalhos);
                    for (int i = 0; i < linhas.Count; i++)
                        AppendLinha(sb, i + 2, linhas[i]);

                    sb.Append("</sheetData></worksheet>");
                    EscreverEntrada(zip, "xl/worksheets/sheet1.xml", sb.ToString());
                }
                return ms.ToArray();
            }
        }

        private static void EscreverEntrada(ZipArchive zip, string nome, string conteudo)
        {
            var entrada = zip.CreateEntry(nome);
            var bytes = new UTF8Encoding(false).GetBytes(conteudo);
            using (var s = entrada.Open()) s.Write(bytes, 0, bytes.Length);
        }

        private static void AppendLinha(StringBuilder sb, int numero, string[] valores)
        {
            sb.Append("<row r=\"").Append(numero).Append("\">");
            for (int c = 0; c < valores.Length; c++)
            {
                var v = valores[c];
                if (string.IsNullOrEmpty(v)) continue;

                sb.Append("<c r=\"").Append(NomeColuna(c)).Append(numero).Append("\" t=\"inlineStr\"><is><t xml:space=\"preserve\">");
                foreach (var ch in v)
                {
                    if (ch == '&') sb.Append("&amp;");
                    else if (ch == '<') sb.Append("&lt;");
                    else if (ch == '>') sb.Append("&gt;");
                    else if (ch < 0x20 && ch != '\t' && ch != '\n' && ch != '\r') continue;
                    else sb.Append(ch);
                }
                sb.Append("</t></is></c>");
            }
            sb.Append("</row>");
        }

        private static string NomeColuna(int indice)
        {
            string s = string.Empty;
            int n = indice + 1;
            while (n > 0)
            {
                int m = (n - 1) % 26;
                s = (char)('A' + m) + s;
                n = (n - m - 1) / 26;
            }
            return s;
        }

        // ------------------------------------------------------------ auxiliares

        private static string FormatarData(string ddMMyyyy)
        {
            if (string.IsNullOrEmpty(ddMMyyyy)) return string.Empty;
            DateTime d;
            if (DateTime.TryParseExact(ddMMyyyy, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d))
                return d.ToString(FormatoData, CultureInfo.InvariantCulture);
            return ddMMyyyy;
        }

        // Nunca descarta dado: se não houver de/para, mantém o valor original e avisa.
        private static string DePara(Dictionary<string, string> mapa, string valor, setCemgImportModels l,
                                     string campo, setCemgExportResultado res)
        {
            if (string.IsNullOrEmpty(valor)) return string.Empty;
            string destino;
            if (mapa.TryGetValue(valor, out destino)) return destino;
            Aviso(res, l, campo, "Sem de/para para o valor '" + valor + "'. Mantido o valor original.");
            return valor;
        }

        // "R CARAJAS" -> tipo "RUA", nome "CARAJAS". Se o prefixo não for conhecido, tipo fica vazio e o nome fica completo.
        private static void SepararLogradouro(string valor, out string tipo, out string nome)
        {
            tipo = string.Empty;
            nome = valor ?? string.Empty;
            if (string.IsNullOrWhiteSpace(valor)) return;

            var partes = valor.Trim().Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            string tipoCompleto;
            if (partes.Length == 2 && TiposLogradouro.TryGetValue(partes[0].TrimEnd('.'), out tipoCompleto))
            {
                tipo = tipoCompleto;
                nome = partes[1].Trim();
            }
        }

        // Padrão do exemplo: "NOME DA RUA, 560 - AP 802"
        private static string MontarEndereco(string nome, string numero, string complemento)
        {
            var s = nome ?? string.Empty;
            if (!string.IsNullOrEmpty(numero)) s += ", " + numero;
            if (!string.IsNullOrEmpty(complemento)) s += " - " + complemento;
            return s;
        }

        // "31971362716" -> DDD "31", número "971362716"
        private static void SepararTelefone(string valor, out string ddd, out string numero)
        {
            ddd = string.Empty;
            numero = string.Empty;
            if (string.IsNullOrEmpty(valor)) return;

            var d = new string(valor.Where(char.IsDigit).ToArray());
            if (d.Length >= 10) { ddd = d.Substring(0, 2); numero = d.Substring(2); }
            else numero = d;
        }

        private static void Aviso(setCemgExportResultado res, setCemgImportModels l, string campo, string msg)
        {
            res.Avisos.Add(new setCemgImportErro { LinhaExcel = l.LinhaExcel, Campo = campo, Mensagem = msg });
        }

        private static void AvisoGeral(setCemgExportResultado res, string campo, string msg)
        {
            res.Avisos.Add(new setCemgImportErro { LinhaExcel = 0, Campo = campo, Mensagem = msg });
        }
    }

    public class setCemgExportResultado
    {
        public List<string[]> Linhas { get; } = new List<string[]>();
        public List<setCemgImportErro> Avisos { get; } = new List<setCemgImportErro>();
    }
}
