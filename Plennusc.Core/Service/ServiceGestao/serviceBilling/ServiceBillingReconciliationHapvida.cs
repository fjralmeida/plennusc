using Plennusc.Core.Models.ModelsGestao.modelsBilling;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.serviceBilling
{
    public class ServiceBillingReconciliationHapvida
    {
        private const string MARCADOR_INICIO = "empresa";
        private const string MARCADOR_FIM = "Historico de Beneficiarios de Empresa com Vigencia Retroativa";

        public List<ItemRelatorioImportadoHapVida> LerRelatorio(Stream arquivo, string extensao)
        {
            switch (extensao)
            {
                case ".csv":
                    return LerRelatorioCsv(arquivo);

                case ".xlsx":
                case ".xls":
                    throw new NotSupportedException("Leitura de Excel para Hapvida ainda não implementada.");

                case ".docx":
                    throw new NotSupportedException("Leitura de Word para Hapvida ainda não implementada.");

                default:
                    throw new NotSupportedException($"Extensão '{extensao}' não suportada para Hapvida.");
            }
        }

        private List<ItemRelatorioImportadoHapVida> LerRelatorioCsv(Stream arquivo)
        {
            var itens = new List<ItemRelatorioImportadoHapVida>();

            using (var reader = new StreamReader(arquivo, Encoding.GetEncoding("ISO-8859-1")))
            {
                bool dentroDoBloco = false;
                string[] colunas = null;
                char delimitador = '\t'; // valor padrão, será ajustado ao encontrar o cabeçalho

                string linha;
                while ((linha = reader.ReadLine()) != null)
                {
                    linha = linha.Trim('"');

                    if (string.IsNullOrWhiteSpace(linha))
                        continue;

                    if (!dentroDoBloco)
                    {
                        if (linha.TrimStart().StartsWith(MARCADOR_INICIO, StringComparison.OrdinalIgnoreCase))
                        {
                            delimitador = DetectarDelimitador(linha);
                            colunas = linha.Split(delimitador);
                            dentroDoBloco = true;
                        }
                        continue;
                    }

                    if (linha.StartsWith(MARCADOR_FIM, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    var valores = linha.Split(delimitador);

                    if (valores.Length < colunas.Length)
                        continue;

                    itens.Add(MapearItem(valores));
                }
            }

            return itens;
        }

        // Verifica qual separador realmente divide a linha de cabeçalho em mais de uma coluna.
        // Prioriza TAB (formato original visto) e cai para ';' se TAB não separar nada.
        private char DetectarDelimitador(string linhaCabecalho)
        {
            if (linhaCabecalho.Split('\t').Length > 1)
                return '\t';

            if (linhaCabecalho.Split(';').Length > 1)
                return ';';

            // fallback: vírgula, caso apareça uma variação futura
            return ',';
        }

        private ItemRelatorioImportadoHapVida MapearItem(string[] v)
        {
            return new ItemRelatorioImportadoHapVida
            {
                Empresa = ObterTexto(v, 0),
                Unidade = ObterTexto(v, 1),
                NomeUnidade = ObterTexto(v, 2),
                Credencial = ObterTexto(v, 3),
                Matricula = ObterTexto(v, 4),
                Cpf = ObterTexto(v, 5),
                Beneficiario = ObterTexto(v, 6),
                NomeMae = ObterTexto(v, 7),
                Nascimento = ObterData(v, 8),
                Inicio = ObterData(v, 9),
                Idade = ObterInteiro(v, 10),
                Parentesco = ObterTexto(v, 11),
                Plano = ObterTexto(v, 12),
                Ac = ObterTexto(v, 13),
                Mensalidade = ObterDecimal(v, 14),
                Adicional = ObterDecimal(v, 15),
                TaxaAdesao = ObterDecimal(v, 16),
                Desconto = ObterDecimal(v, 17),
                Cobrado = ObterDecimal(v, 18)
            };
        }

        private string ObterTexto(string[] v, int indice)
        {
            if (indice >= v.Length) return null;
            return v[indice].Trim();
        }

        private DateTime? ObterData(string[] v, int indice)
        {
            var texto = ObterTexto(v, indice);
            if (string.IsNullOrEmpty(texto)) return null;

            if (DateTime.TryParseExact(texto, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var data))
                return data;

            return null;
        }

        private int? ObterInteiro(string[] v, int indice)
        {
            var texto = ObterTexto(v, indice);
            if (string.IsNullOrEmpty(texto)) return null;

            if (int.TryParse(texto, out var numero))
                return numero;

            return null;
        }

        private decimal ObterDecimal(string[] v, int indice)
        {
            var texto = ObterTexto(v, indice);
            if (string.IsNullOrEmpty(texto)) return 0;

            if (decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out var valor))
                return valor;

            return 0;
        }
    }
}