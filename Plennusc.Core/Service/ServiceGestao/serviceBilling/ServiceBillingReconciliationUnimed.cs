
using Plennusc.Core.Models.ModelsGestao.modelsBilling;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.billing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.serviceBilling
{
    public class ServiceBillingReconciliationUnimed
    {
        private const decimal TOLERANCIA_DIVERGENCIA = 0.10m;
        private readonly SqlBillingReconciliation _sql = new SqlBillingReconciliation();

        public List<ItemRelatorioImportadoUnimed> LerRelatorioUnimed(Stream arquivo, string extensao)
        {
            switch (extensao)
            {
                case ".txt":
                    return LerRelatorioTXT(arquivo);

                case ".csv":
                    throw new NotSupportedException("Leitura de CSV para Unimed ainda não implementada.");

                case ".xlsx":
                case ".xls":
                    throw new NotSupportedException("Leitura de Excel para Unimed ainda não implementada.");

                case ".docx":
                    throw new NotSupportedException("Leitura de Word para Unimed ainda não implementada.");

                default:
                    throw new NotSupportedException($"Extensão '{extensao}' não suportada para Unimed.");
            }
        }

        private List<ItemRelatorioImportadoUnimed> LerRelatorioTXT(Stream arquivo)
        {
            var itens = new List<ItemRelatorioImportadoUnimed>();

            using (var reader = new StreamReader(arquivo, Encoding.GetEncoding("ISO-8859-1")))
            {
                string linha;
                while ((linha = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(linha))
                        continue;

                    var campos = linha.Split(';');

                    // Header (H) e trailer (9) ignorados por enquanto
                    if (campos[0] != "1")
                        continue;

                    // Linha fora do padrão esperado -> ignora com segurança
                    if (campos.Length < 20)
                        continue;

                    string carteirinha = campos[1].Trim();
                    string nomeBeneficiario = campos[2].Trim();
                    string nomeTitular = campos[3].Trim();
                    string descricaoProduto = campos[18].Trim();
                    string valorBruto = campos[19].Trim();

                    decimal valor = ConverterValor(valorBruto);

                    // CPF só vem na última linha do grupo familiar
                    string cpf = carteirinha;

                    var item = new ItemRelatorioImportadoUnimed
                    {
                        Credencial = carteirinha,
                        NomeBeneficiario = nomeBeneficiario,
                        NomeTitular = nomeTitular,
                        Descricao = descricaoProduto,
                        ValorOperadora = valor,
                        Cpf = string.IsNullOrWhiteSpace(cpf) ? null : cpf
                    };

                    itens.Add(item);
                }
            }

            return itens;
        }

        private decimal ConverterValor(string valorBruto)
        {
            if (string.IsNullOrWhiteSpace(valorBruto))
                return 0;

            // Remove tudo que não for dígito (defesa contra espaços/caracteres soltos)
            var digitos = new string(valorBruto.Where(char.IsDigit).ToArray());

            if (digitos.Length == 0)
                return 0;

            // Os 2 últimos dígitos são sempre os centavos
            if (!decimal.TryParse(digitos, NumberStyles.None, CultureInfo.InvariantCulture, out decimal valorInteiro))
                return 0;

            return valorInteiro / 100m;
        }
    }
}