using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Plennusc.Core.Models.ModelsGestao.modelsBilling;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.billing;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Plennusc.Core.Service.ServiceGestao.serviceBilling
{
    public class ServiceBillingReconciliationUniaoMedica
    {
        private const decimal TOLERANCIA_DIVERGENCIA = 0.10m;
        private readonly SqlBillingReconciliation _sql = new SqlBillingReconciliation();

        // ===================== LEITURA DO RELATÓRIO =====================

        public List<ItemRelatorioImportadoHapVida> LerRelatorio(Stream arquivo, string extensao)
        {
            if (arquivo == null || arquivo.Length == 0)
                throw new ArgumentException("O arquivo está vazio ou inválido.");

            extensao = (extensao ?? string.Empty).ToLowerInvariant();

            if (extensao == ".xlsx" || extensao == ".xls")
                return LerRelatorioExcel(arquivo);

            throw new NotSupportedException($"Extensão '{extensao}' não suportada para União Médica. Use .xlsx ou .xls.");
        }

        private List<ItemRelatorioImportadoHapVida> LerRelatorioExcel(Stream arquivo)
        {
            var itens = new List<ItemRelatorioImportadoHapVida>();

            using (var doc = SpreadsheetDocument.Open(arquivo, false))
            {
                var workbookPart = doc.WorkbookPart;
                if (workbookPart == null)
                    throw new InvalidOperationException("O arquivo Excel não contém um Workbook válido.");

                var worksheetPart = workbookPart.WorksheetParts.FirstOrDefault();
                if (worksheetPart == null)
                    throw new InvalidOperationException("O arquivo Excel não contém uma planilha.");

                var sheetData = worksheetPart.Worksheet.Elements<SheetData>().FirstOrDefault();
                if (sheetData == null)
                    throw new InvalidOperationException("A planilha não contém dados.");

                int colunaCpf = -1;
                int colunaMensalidade = -1;
                int colunaUsuario = -1;
                int colunaNascimento = -1; // NOVO
                bool cabecalhoEncontrado = false;

                foreach (var row in sheetData.Elements<Row>())
                {
                    var celulas = row.Elements<Cell>().ToList();
                    if (!celulas.Any())
                        continue;

                    var primeiroValor = ObterValorCelula(celulas.FirstOrDefault(), workbookPart);
                    if (string.IsNullOrWhiteSpace(primeiroValor))
                        continue;

                    // Pular linhas de cabeçalho do relatório (ex: "LOCACAO", "Titulares", "Total", etc.)
                    if (EhLinhaDeCabecalhoInvalida(primeiroValor))
                        continue;

                    // Detectar cabeçalho com os nomes das colunas
                    if (!cabecalhoEncontrado)
                    {
                        for (int i = 0; i < celulas.Count; i++)
                        {
                            var valor = ObterValorCelula(celulas[i], workbookPart);
                            if (string.IsNullOrEmpty(valor))
                                continue;

                            var valorLimpo = valor.Trim();
                            if (valorLimpo.Equals("CPF", StringComparison.OrdinalIgnoreCase))
                                colunaCpf = i;
                            else if (valorLimpo.Equals("Mensalidade", StringComparison.OrdinalIgnoreCase))
                                colunaMensalidade = i;
                            else if (valorLimpo.Equals("Usuario", StringComparison.OrdinalIgnoreCase) ||
                                     valorLimpo.Equals("Usuário", StringComparison.OrdinalIgnoreCase))
                                colunaUsuario = i;
                            else if (valorLimpo.Equals("Nascimento", StringComparison.OrdinalIgnoreCase) ||
                                     valorLimpo.Equals("Data Nascimento", StringComparison.OrdinalIgnoreCase))
                                colunaNascimento = i;
                        }

                        if (colunaCpf >= 0 && colunaMensalidade >= 0)
                        {
                            cabecalhoEncontrado = true;
                        }
                        continue;
                    }

                    // Se não encontrou cabeçalho, não processa
                    if (!cabecalhoEncontrado)
                        continue;

                    // Pular linhas que contêm palavras como "Total" (resumo)
                    if (celulas.Any(c =>
                    {
                        var val = ObterValorCelula(c, workbookPart);
                        return !string.IsNullOrEmpty(val) &&
                               (val.IndexOf("Total", StringComparison.OrdinalIgnoreCase) >= 0 ||
                                val.IndexOf("TOTAL", StringComparison.OrdinalIgnoreCase) >= 0);
                    }))
                        continue;

                    try
                    {
                        var cpf = colunaCpf < celulas.Count ? ObterValorCelula(celulas[colunaCpf], workbookPart) : null;
                        var mensalidade = colunaMensalidade < celulas.Count ? ObterValorCelula(celulas[colunaMensalidade], workbookPart) : null;
                        var usuario = colunaUsuario >= 0 && colunaUsuario < celulas.Count ? ObterValorCelula(celulas[colunaUsuario], workbookPart) : null;
                        var nascimento = colunaNascimento >= 0 && colunaNascimento < celulas.Count ? ObterValorCelula(celulas[colunaNascimento], workbookPart) : null;

                        if (string.IsNullOrWhiteSpace(cpf) || string.IsNullOrWhiteSpace(mensalidade))
                            continue;

                        cpf = LimparCpf(cpf);
                        if (cpf.Length != 11)
                            continue;

                        if (!TryConverterValorMonetario(mensalidade, out decimal valor))
                            continue;

                        // Converter data de nascimento
                        DateTime? dataNascimento = ConverterDataNascimento(nascimento);

                        var item = new ItemRelatorioImportadoHapVida
                        {
                            Cpf = cpf,
                            Beneficiario = usuario,
                            Cobrado = valor,
                            Nascimento = dataNascimento, // Preenche a data
                            StatusConferencia = "PENDENTE"
                        };

                        itens.Add(item);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Erro ao processar linha: {ex.Message}");
                    }
                }
            }

            return itens;
        }

        // ===================== MÉTODOS AUXILIARES =====================

        private string ObterValorCelula(Cell cell, WorkbookPart workbookPart)
        {
            if (cell == null || cell.CellValue == null)
                return null;

            if (cell.DataType != null && cell.DataType == CellValues.SharedString)
            {
                var sharedStringPart = workbookPart.SharedStringTablePart;
                if (sharedStringPart != null)
                {
                    var index = int.Parse(cell.CellValue.Text);
                    return sharedStringPart.SharedStringTable
                        .Elements<SharedStringItem>()
                        .ElementAt(index)
                        .InnerText;
                }
                return null;
            }

            return cell.CellValue.Text;
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

            var texto = valorTexto.Trim().Replace(" ", "");
            texto = texto.Replace(",", ".");

            if (decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out valor))
                return true;

            if (decimal.TryParse(valorTexto, NumberStyles.Any, new CultureInfo("pt-BR"), out valor))
                return true;

            return false;
        }

        private bool EhLinhaDeCabecalhoInvalida(string texto)
        {
            if (string.IsNullOrEmpty(texto))
                return false;

            var invalidos = new[]
            {
                "LOCACAO",
                "Titulares",
                "Dependentes",
                "Agregados",
                "Total",
                "TOTAL",
                "IMPOSTO",
                "ISS",
                "FATURA",
                "Vencimento"
            };

            return invalidos.Any(i => texto.IndexOf(i, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private DateTime? ConverterDataNascimento(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;

            // Remove aspas duplas se houver
            valor = valor.Trim('"');

            // Tenta formatos comuns
            if (DateTime.TryParseExact(valor, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt1))
                return dt1;

            if (DateTime.TryParseExact(valor, "dd/MM/yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dt2))
                return dt2;

            if (DateTime.TryParse(valor, new CultureInfo("pt-BR"), DateTimeStyles.None, out DateTime dt3))
                return dt3;

            return null;
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