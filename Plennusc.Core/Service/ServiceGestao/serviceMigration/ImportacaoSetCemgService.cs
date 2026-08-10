using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Plennusc.Core.Models.ModelsGestao.modelsMigration; // ajuste para seu DTO

namespace Plennusc.Core.Service.ServiceGestao.serviceMigration
{
    public class ImportacaoSetCemgService
    {
        private static readonly Dictionary<string, string> MapColunas = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "VIGENCIA",                 "VigenciaRaw" },
            { "VENCIMENTO",               "VencimentoRaw" },
            { "PROPOSTA",                 "Proposta" },
            { "RAZÃO SOCIAL",             "RazaoSocial" },
            { "NOME FANTASIA",            "NomeFantasia" },
            { "CNPJ",                     "Cnpj" },
            { "INSCRIÇÃO ESTADUAL",       "InscricaoEstadual" },
            { "INSCRIÇÃO MUNICIPAL",      "InscricaoMunicipal" },
            { "LOGRADOURO",               "Logradouro" },
            { "NÚMERO",                   "Numero" },
            { "COMPLEMENTO",              "Complemento" },
            { "BAIRRO",                   "Bairro" },
            { "MUNICÍPIO/UF",             "MunicipioUf" },
            { "CEP",                      "Cep" },
            { "EMAIL",                    "Email" },
            { "TELEFONE",                 "Telefone" },
            { "NOME DO RESPONSÁVEL",      "NomeResponsavel" },
            { "TELEFONE DO RESPONSÁVEL",  "TelefoneResponsavel" },
            { "CARGO",                    "Cargo" },
            { "EMAIL DO RESPONSÁVEL",     "EmailResponsavel" },
            { "PRODUTO",                  "ProdutoRaw" },
            { "MESALIDADE",               "MensalidadeRaw" },
            { "MODALIDADE",               "Modalidade" },
            { "AEROMÉDICO",               "Aeromedico" },
            { "ODONTOLOGIA",              "Odontologia" },
        };

        public List<DadosPropostaSetCemg> ImportarCsv(string caminhoArquivo, char separador = ';')
        {
            if (!File.Exists(caminhoArquivo))
                throw new FileNotFoundException($"Arquivo não encontrado: {caminhoArquivo}");

            // Lê com detecção de BOM (UTF-8 com BOM ou sem)
            var linhas = File.ReadAllLines(caminhoArquivo, Encoding.UTF8);
            if (linhas.Length < 2)
                throw new Exception("CSV vazio ou sem dados.");

            // Detecta separador automaticamente: se o cabeçalho tiver ';' usa ';', senão usa ','
            var cabecalho = linhas[0].Split(separador);
            // Se o número de colunas for pequeno, pode ser que o separador seja outro
            if (cabecalho.Length < 3)
            {
                // Tenta com vírgula
                separador = separador == ';' ? ',' : ';';
                cabecalho = linhas[0].Split(separador);
            }

            var mapa = MapearColunasCsv(cabecalho);

            var lista = new List<DadosPropostaSetCemg>();

            for (int i = 1; i < linhas.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(linhas[i])) continue;

                var colunas = SplitCsvLinha(linhas[i], separador);

                // Função auxiliar para extrair valor
                string Get(string prop)
                {
                    if (mapa.TryGetValue(prop, out int idx) && idx >= 0 && idx < colunas.Length)
                        return colunas[idx]?.Trim() ?? "";
                    return "";
                }

                // Pula linhas sem razão social
                if (string.IsNullOrWhiteSpace(Get("RazaoSocial")))
                    continue;

                var dto = new DadosPropostaSetCemg
                {
                    VigenciaRaw = Get("VigenciaRaw"),
                    VencimentoRaw = Get("VencimentoRaw"),
                    Proposta = Get("Proposta"),
                    RazaoSocial = Get("RazaoSocial"),
                    NomeFantasia = Get("NomeFantasia"),
                    Cnpj = Get("Cnpj"),
                    InscricaoEstadual = Get("InscricaoEstadual"),
                    InscricaoMunicipal = Get("InscricaoMunicipal"),
                    Logradouro = Get("Logradouro"),
                    Numero = Get("Numero"),
                    Complemento = Get("Complemento"),
                    Bairro = Get("Bairro"),
                    MunicipioUf = Get("MunicipioUf"),
                    Cep = Get("Cep"),
                    Email = Get("Email"),
                    Telefone = Get("Telefone"),
                    NomeResponsavel = Get("NomeResponsavel"),
                    TelefoneResponsavel = Get("TelefoneResponsavel"),
                    Cargo = Get("Cargo"),
                    EmailResponsavel = Get("EmailResponsavel"),
                    ProdutoRaw = Get("ProdutoRaw"),
                    MensalidadeRaw = Get("MensalidadeRaw"),
                    Modalidade = Get("Modalidade"),
                    Aeromedico = Get("Aeromedico"),
                    Odontologia = Get("Odontologia"),
                };

                // Quebra campos multivalorados
                dto.Vigencias = SplitMultivalor(dto.VigenciaRaw);
                dto.Vencimentos = SplitMultivalor(dto.VencimentoRaw);
                dto.Produtos = SplitMultivalor(dto.ProdutoRaw);
                dto.Mensalidades = SplitMultivalor(dto.MensalidadeRaw);

                lista.Add(dto);
            }

            return lista;
        }

        private List<string> SplitMultivalor(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return new List<string>();
            return valor.Split(';')
                        .Select(v => v.Trim())
                        .Where(v => !string.IsNullOrWhiteSpace(v))
                        .ToList();
        }

        private Dictionary<string, int> MapearColunasCsv(string[] cabecalho)
        {
            var mapa = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < cabecalho.Length; i++)
            {
                string header = cabecalho[i].Trim().Trim('"');
                if (string.IsNullOrEmpty(header)) continue;

                // Tenta encontrar no dicionário de mapeamento
                foreach (var kv in MapColunas)
                {
                    // Comparação case-insensitive e remove acentos? Pode simplificar:
                    if (string.Equals(header, kv.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        mapa[kv.Value] = i;
                        break;
                    }
                }
            }
            return mapa;
        }

        /// <summary>
        /// Divide uma linha CSV respeitando aspas (campo pode conter o separador)
        /// </summary>
        private string[] SplitCsvLinha(string linha, char separador)
        {
            if (string.IsNullOrEmpty(linha)) return new string[0];

            var resultado = new List<string>();
            bool dentroAspas = false;
            var campo = new StringBuilder();

            for (int i = 0; i < linha.Length; i++)
            {
                char c = linha[i];

                if (c == '"')
                {
                    // Se duas aspas seguidas, é escape ("" -> ")
                    if (i + 1 < linha.Length && linha[i + 1] == '"')
                    {
                        campo.Append('"');
                        i++; // pula a segunda aspa
                    }
                    else
                    {
                        dentroAspas = !dentroAspas;
                    }
                }
                else if (c == separador && !dentroAspas)
                {
                    resultado.Add(campo.ToString().Trim());
                    campo.Clear();
                }
                else
                {
                    campo.Append(c);
                }
            }

            // Último campo
            resultado.Add(campo.ToString().Trim());
            return resultado.ToArray();
        }
    }
}