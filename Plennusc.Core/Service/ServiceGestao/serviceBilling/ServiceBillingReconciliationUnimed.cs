
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

        private string DeterminarTipoServico(string descricaoProduto)
        {
            if (string.IsNullOrEmpty(descricaoProduto))
                return "CONVENIO";

            if (descricaoProduto.IndexOf("ODONTO", StringComparison.OrdinalIgnoreCase) >= 0)
                return "ODONTO";

            if (descricaoProduto.IndexOf("AEROMEDICO", StringComparison.OrdinalIgnoreCase) >= 0
                || descricaoProduto.IndexOf("AÉREOMEDICO", StringComparison.OrdinalIgnoreCase) >= 0)
                return "AEROMEDICO";

            return "CONVENIO";
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

        public List<ItemRelatorioImportadoHapVida> ConferirComView(List<ItemRelatorioImportadoHapVida> itensImportados, int codigoGrupoContrato)
        {
            foreach (var item in itensImportados)
            {
                string carteirinhaTratada = TratarCredencial(item.Credencial);
                string tipoServico = DeterminarTipoServico(item.Plano); // Plano = Descricao do produto vinda do TXT

                string tipoView;
                string filtroDescricao;

                switch (tipoServico)
                {
                    case "ODONTO":
                        tipoView = "EVENTO ADICIONAL";
                        filtroDescricao = "ODONTO";
                        break;
                    case "AEROMEDICO":
                        tipoView = "EVENTO ADICIONAL";
                        filtroDescricao = "AEROMEDICO";
                        break;
                    default:
                        tipoView = "CONVÊNIO";
                        filtroDescricao = null;
                        break;
                }

                var resultado = _sql.BuscarDadosUnimedPorCarteirinha(carteirinhaTratada, item.MesAnoReferencia, codigoGrupoContrato, tipoView, filtroDescricao);

                if (resultado == null)
                {
                    item.ValorOperadoraView = null;
                    item.DiferencaValor = null;
                    item.StatusConferencia = "NAO_ENCONTRADO";
                    continue;
                }

                item.DataAdmissao = resultado.DataAdmissao;
                item.DataExclusao = resultado.DataExclusao;
                item.NomeMotivoExclusao = resultado.NomeMotivoExclusao;
                item.NomeTabelaPreco = resultado.NomeTabelaPreco;
                item.NomeGrupoPessoas = resultado.NomeGrupoPessoas;
                item.DescricaoGrupoFaturamento = resultado.DescricaoGrupoFaturamento;
                item.ValorOperadoraView = resultado.ValorOperadora;

                decimal diferenca = Math.Abs(item.Cobrado - resultado.ValorOperadora.Value);
                item.DiferencaValor = diferenca;

                if (diferenca == 0)
                    item.StatusConferencia = "OK";
                else if (diferenca <= TOLERANCIA_DIVERGENCIA)
                    item.StatusConferencia = "DIVERGENCIA_TOLERADA";
                else
                    item.StatusConferencia = "DIVERGENTE";
            }

            return itensImportados;
        }

        // Mesma lógica de tratamento usada no import — reaproveita se já existir, senão adiciona:
        private string TratarCredencial(string credencial)
        {
            if (string.IsNullOrEmpty(credencial)) return credencial;
            return credencial.Replace(".", "").Replace("-", "").Replace(" ", "").Trim();
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