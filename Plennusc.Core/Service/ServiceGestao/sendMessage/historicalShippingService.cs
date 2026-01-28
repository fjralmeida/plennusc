using OfficeOpenXml;
using OfficeOpenXml.Style;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.sendMessage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.sendMessage
{
    public class historicalShippingService
    {
        private readonly HistoricoEnvioQuery _query;

        public historicalShippingService()
        {
            _query = new HistoricoEnvioQuery();
        }

        public DataTable BuscarHistorico(
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            string codigoAssociado = null,
            string nomeAssociado = null,
            string telefone = null,
            string status = null,
            string template = null,
            int pagina = 1,
            int registrosPorPagina = 50)
        {
            return _query.ConsultarHistoricoEnvios(
                dataInicio, dataFim, codigoAssociado, nomeAssociado,
                telefone, status, template, pagina, registrosPorPagina);
        }

        public int ContarTotalRegistros(
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            string codigoAssociado = null,
            string nomeAssociado = null,
            string telefone = null,
            string status = null,
            string template = null)
        {
            return _query.ContarTotalRegistros(
                dataInicio, dataFim, codigoAssociado, nomeAssociado,
                telefone, status, template);
        }

        public byte[] GerarExcelHistorico(
           DateTime? dataInicio = null,
           DateTime? dataFim = null,
           string codigoAssociado = null,
           string nomeAssociado = null,
           string telefone = null,
           string status = null,
           string template = null)
        {
            // Busca todos os registros sem paginação
            var dt = _query.ConsultarHistoricoEnvios(
                dataInicio, dataFim, codigoAssociado, nomeAssociado,
                telefone, status, template, 1, int.MaxValue);

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Histórico de Envios");

                // Cabeçalhos - AGORA COM NOME
                string[] cabecalhos = {
                    "Data/Hora", "Código Associado", "Nome Associado", "Telefone",
                    "Template", "Status", "Usuário", "ID Resposta API", "Log API"
                };

                for (int i = 0; i < cabecalhos.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = cabecalhos[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                    worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                }

                // Dados
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var row = dt.Rows[i];
                    int excelRow = i + 2;

                    worksheet.Cells[excelRow, 1].Value = row["DataEnvio"];
                    worksheet.Cells[excelRow, 1].Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";

                    worksheet.Cells[excelRow, 2].Value = row["CodigoAssociado"];
                    worksheet.Cells[excelRow, 3].Value = row["NomeAssociado"]; // Nome do associado
                    worksheet.Cells[excelRow, 4].Value = row["NumTelefoneDestino"];
                    worksheet.Cells[excelRow, 5].Value = GetDescricaoTemplate(row["Mensagem"]?.ToString());
                    worksheet.Cells[excelRow, 6].Value = row["StatusEnvio"];
                    worksheet.Cells[excelRow, 7].Value = row["CodUsuarioEnvio"]; // Usuário (código)
                    worksheet.Cells[excelRow, 8].Value = row["ID_RESPOSTA_API"];
                    worksheet.Cells[excelRow, 9].Value = row["STATUS_API_JSON"];
                }

                // Ajusta largura
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }

        private string GetDescricaoTemplate(string template)
        {
            // CORREÇÃO: SWITCH TRADICIONAL PARA C# 7.3
            string descricao;
            switch (template)
            {
                case "Suspensao":
                    descricao = "Boleto";
                    break;
                case "Definitivo":
                    descricao = "Nota Fiscal";
                    break;
                case "aVencer":
                    descricao = "Boleto + Nota Fiscal";
                    break;
                default:
                    descricao = template;
                    break;
            }
            return descricao;
        }

        public Dictionary<string, string> GetTemplatesDisponiveis()
        {
            return new Dictionary<string, string>
            {
                { "TODOS", "Todos" },
                { "Suspensao", "Boleto" },
                { "Definitivo", "Nota Fiscal" },
                { "aVencer", "Boleto + Nota Fiscal" }
            };
        }

        public Dictionary<string, string> GetStatusDisponiveis()
        {
            return new Dictionary<string, string>
            {
                { "TODOS", "Todos" },
                { "ENVIADO", "Enviado" },
                { "FALHOU", "Falhou" },
                { "SEM_DOCUMENTOS", "Sem documentos" }
            };
        }
    }
}