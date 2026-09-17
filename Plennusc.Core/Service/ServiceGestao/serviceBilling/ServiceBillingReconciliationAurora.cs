using Plennusc.Core.Models.ModelsGestao.modelsBilling;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.billing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Plennusc.Core.Service.ServiceGestao.serviceBilling
{
    public class ServiceBillingReconciliationAurora
    {
        private const decimal TOLERANCIA_DIVERGENCIA = 0.10m;
        private readonly SqlBillingReconciliation _sql = new SqlBillingReconciliation();

        // ===================== LEITURA DO RELATÓRIO =====================

        public List<ItemRelatorioImportadoHapVida> LerRelatorio(Stream arquivo, string extensao)
        {
            if (arquivo == null || arquivo.Length == 0)
                throw new ArgumentException("O arquivo está vazio ou inválido.");

            extensao = (extensao ?? string.Empty).ToLowerInvariant();

            // O arquivo da Aurora vem como CSV (delimitador ';'), mesmo quando exportado com extensão .xls
            if (extensao == ".csv" || extensao == ".xls" || extensao == ".xlsx")
                return LerRelatorioCsv(arquivo);

            throw new NotSupportedException($"Extensão '{extensao}' não suportada para Aurora. Use .csv ou .xls.");
        }

        private List<ItemRelatorioImportadoHapVida> LerRelatorioCsv(Stream arquivo)
        {
            var itens = new List<ItemRelatorioImportadoHapVida>();

            using (var reader = new StreamReader(arquivo, Encoding.GetEncoding("ISO-8859-1")))
            {
                bool cabecalhoEncontrado = false;
                int numeroLinha = 0;

                string linha;
                while ((linha = reader.ReadLine()) != null)
                {
                    numeroLinha++;

                    if (string.IsNullOrWhiteSpace(linha))
                        continue;

                    // Detectar o cabeçalho (primeira linha que contém "MATRICULA" e "CARTEIRINHA")
                    if (!cabecalhoEncontrado)
                    {
                        if (linha.IndexOf("MATRICULA", StringComparison.OrdinalIgnoreCase) >= 0 &&
                            linha.IndexOf("CARTEIRINHA", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            cabecalhoEncontrado = true;
                        }
                        continue;
                    }

                    // Cada linha de dados vem no formato: "valor";"valor";"valor";...
                    var campos = linha.Split(';');

                    // Blindagem: precisa ter pelo menos os campos essenciais (índice 11 = VALOR)
                    const int MIN_CAMPOS_NECESSARIOS = 12;
                    if (campos.Length < MIN_CAMPOS_NECESSARIOS)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            $"Linha {numeroLinha} ignorada: apenas {campos.Length} campos (esperado >= {MIN_CAMPOS_NECESSARIOS}).");
                        continue;
                    }

                    try
                    {
                        string matricula = LimparCampo(campos[0]);
                        string carteirinha = LimparCampo(campos[1]);
                        string nome = LimparCampo(campos[3]);
                        string cpfBruto = LimparCampo(campos[4]);
                        string plano = LimparCampo(campos[7]);
                        string valorBruto = LimparCampo(campos[11]);

                        if (string.IsNullOrWhiteSpace(cpfBruto) || string.IsNullOrWhiteSpace(valorBruto))
                            continue;

                        // CPF: remove não-dígitos e preenche com zero à esquerda até 11 dígitos
                        // (o Excel/export costuma cortar o zero inicial)
                        string cpf = LimparCpf(cpfBruto);
                        if (cpf.Length < 11)
                            cpf = cpf.PadLeft(11, '0');

                        if (cpf.Length != 11)
                            continue;

                        if (!TryConverterValorMonetario(valorBruto, out decimal valor))
                            continue;

                        var item = new ItemRelatorioImportadoHapVida
                        {
                            Cpf = cpf,
                            Beneficiario = nome,
                            Matricula = matricula,
                            Credencial = carteirinha,
                            Plano = plano,
                            Cobrado = valor,
                            StatusConferencia = "PENDENTE"
                        };

                        itens.Add(item);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Linha {numeroLinha} ignorada por erro: {ex.Message}");
                    }
                }
            }

            return itens;
        }

        // ===================== MÉTODOS AUXILIARES =====================

        private string LimparCampo(string campo)
        {
            if (string.IsNullOrWhiteSpace(campo))
                return string.Empty;

            // Remove aspas duplas envolvendo o campo, se houver
            return campo.Trim().Trim('"').Trim();
        }

        private string LimparCpf(string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return string.Empty;
            return Regex.Replace(cpf, @"[^\d]", "");
        }

        private bool TryConverterValorMonetario(string valorTexto, out decimal valor)
        {
            valor = 0;
            if (string.IsNullOrWhiteSpace(valorTexto))
                return false;

            // O arquivo da Aurora traz o valor já com ponto decimal (ex: "223.36")
            var texto = valorTexto.Trim().Replace(" ", "");

            // Tenta primeiro com InvariantCulture (ponto como decimal)
            if (decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out valor))
                return true;

            // Fallback pt-BR (vírgula como decimal, ponto como milhar)
            if (decimal.TryParse(valorTexto, NumberStyles.Any, new CultureInfo("pt-BR"), out valor))
                return true;

            return false;
        }

        // ===================== CONFERÊNCIA =====================

        public List<ItemRelatorioImportadoHapVida> ConferirComView(
            List<ItemRelatorioImportadoHapVida> itensImportados,
            string tipoConferencia,
            int codigoGrupoContrato)
        {
            if (itensImportados == null || itensImportados.Count == 0)
                return itensImportados;

            foreach (var item in itensImportados)
            {
                try
                {
                    string cpfTratado = LimparCpf(item.Cpf);

                    if (string.IsNullOrEmpty(cpfTratado) || cpfTratado.Length != 11)
                    {
                        item.StatusConferencia = "NAO_ENCONTRADO";
                        item.DiferencaValor = null;
                        continue;
                    }

                    ResultadoViewConferencia resultado;

                    // Aurora não possui odontológico separado; sempre consulta CONVÊNIO
                    if (tipoConferencia == "EVENTO_ADICIONAL")
                    {
                        resultado = _sql.BuscarDadosOdontologicoPorCpf(cpfTratado, item.MesAnoReferencia, codigoGrupoContrato);
                    }
                    else
                    {
                        resultado = _sql.BuscarDadosConvenioPorCpf(cpfTratado, item.MesAnoReferencia);
                    }

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
                    item.CodigoEmpresa = resultado.CodigoEmpresa;
                    item.Empresa = resultado.Empresa;

                    decimal diferenca = Math.Abs(item.Cobrado - resultado.ValorOperadora.Value);
                    item.DiferencaValor = diferenca;

                    if (diferenca == 0)
                        item.StatusConferencia = "OK";
                    else if (diferenca <= TOLERANCIA_DIVERGENCIA)
                        item.StatusConferencia = "DIVERGENCIA_TOLERADA";
                    else
                        item.StatusConferencia = "DIVERGENTE";
                }
                catch (Exception ex)
                {
                    item.StatusConferencia = "NAO_ENCONTRADO";
                    item.DiferencaValor = null;
                    System.Diagnostics.Debug.WriteLine($"Erro na conferência do CPF {item.Cpf}: {ex.Message}");
                }
            }

            return itensImportados;
        }
    }
}