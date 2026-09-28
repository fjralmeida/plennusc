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

        // Nomes normalizados dos itens faturados que vêm no CSV da Aurora
        private const string ITEM_MENSALIDADE = "MENSALIDADE";
        private const string ITEM_TRANSPORTE_AEROMEDICO = "AEROMEDICO";   // CSV: "TRANSPORTE AEROMÉDICO"  | View: contém "AEROMEDICO"
        private const string ITEM_PLANO_ODONTOLOGICO = "ODONTO";           // CSV: "PLANO ODONTOLÓGICO"      | View: contém "ODONTO"

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
                        string itemFaturado = LimparCampo(campos[10]); // NOVO - coluna "ITEM"
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
                            TipoItemFaturado = itemFaturado, // NOVO
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

                    // Decide QUAL consulta fazer com base no item faturado do CSV da Aurora
                    ResultadoViewConferencia resultado = BuscarResultadoPorItem(
                        item.TipoItemFaturado,
                        cpfTratado,
                        item.MesAnoReferencia,
                        codigoGrupoContrato);

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

        /// <summary>
        /// Roteia a busca na VW_RELATORIO_CONFERENCIA com base no item faturado do CSV.
        /// 
        /// Regra de negócio (Aurora):
        ///   - "MENSALIDADE"             -> CONVÊNIO
        ///   - "TRANSPORTE AEROMÉDICO"   -> EVENTO ADICIONAL filtrando DESCRICAO LIKE '%AEROM%', ou seja, não tem problema se vier com É ou com E no AEROMÉDICO
        ///   - "PLANO ODONTOLÓGICO"      -> EVENTO ADICIONAL filtrando DESCRICAO LIKE '%ODONTO%'
        /// 
        /// A normalização (remover acento, uppercase) garante que tanto a variação do CSV
        /// quanto a da view casem corretamente.
        /// </summary>
        private ResultadoViewConferencia BuscarResultadoPorItem(
            string tipoItemFaturado,
            string cpfTratado,
            string mesAnoReferencia,
            int codigoGrupoContrato)
        {
            string itemNormalizado = NormalizarTexto(tipoItemFaturado);

            // MENSALIDADE (ou vazio/desconhecido): busca CONVÊNIO
            if (string.IsNullOrEmpty(itemNormalizado) ||
                itemNormalizado.Contains(ITEM_MENSALIDADE))
            {
                return _sql.BuscarDadosConvenioPorCpf(cpfTratado, mesAnoReferencia);
            }

            // TRANSPORTE AEROMÉDICO: EVENTO ADICIONAL com DESCRICAO contendo "AEROM"
            if (itemNormalizado.Contains(ITEM_TRANSPORTE_AEROMEDICO))
            {
                return _sql.BuscarDadosEventoAdicionalPorCpf(
                    cpfTratado, mesAnoReferencia, codigoGrupoContrato, "AEROM");
            }

            // PLANO ODONTOLÓGICO: EVENTO ADICIONAL com DESCRICAO contendo "ODONTO"
            if (itemNormalizado.Contains(ITEM_PLANO_ODONTOLOGICO))
            {
                return _sql.BuscarDadosEventoAdicionalPorCpf(
                    cpfTratado, mesAnoReferencia, codigoGrupoContrato, "ODONTO");
            }

            // Fallback: trata como CONVÊNIO
            return _sql.BuscarDadosConvenioPorCpf(cpfTratado, mesAnoReferencia);
        }

        /// <summary>
        /// Normaliza texto para comparação: remove acentos, remove espaços extras e coloca em maiúsculas.
        /// Ex: "TRANSPORTE AEROMÉDICO" -> "TRANSPORTE AEROMEDICO"
        /// </summary>
        private string NormalizarTexto(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            // Remove acentos via decomposição Unicode (FormD) e filtragem de marcas
            var normalizado = texto.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();

            foreach (var c in normalizado)
            {
                var categoria = CharUnicodeInfo.GetUnicodeCategory(c);
                if (categoria != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }

            return sb.ToString().ToUpperInvariant().Trim();
        }
    }
}