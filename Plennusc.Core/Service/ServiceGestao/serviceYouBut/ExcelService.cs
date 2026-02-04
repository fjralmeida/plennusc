using OfficeOpenXml;
using Plennusc.Core.Models.ModelsGestao.modelsButYou;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace appWhatsapp.PlennuscGestao.Services
{
    public class ExcelService
    {
        public Dictionary<string, string> ReadFirstRowAsMap(string excelPath)
        {
            var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!File.Exists(excelPath)) return map;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var p = new ExcelPackage(new FileInfo(excelPath)))
            {
                var ws = p.Workbook.Worksheets.FirstOrDefault();
                if (ws == null) return map;
                int colCount = ws.Dimension.End.Column;
                int rowCount = ws.Dimension.End.Row;
                if (rowCount < 2) return map;

                for (int c = 1; c <= colCount; c++)
                {
                    var header = ws.Cells[1, c].Text?.Trim();
                    var val = ws.Cells[2, c].Text?.Trim() ?? "";
                    if (!string.IsNullOrEmpty(header))
                        map[header] = val;
                }
            }
            return map;
        }

        // exemplo: ler todas linhas em ProposalModel (básico)
        public List<ProposalModel> ReadAllProposals(string excelPath)
        {
            var list = new List<ProposalModel>();
            if (!File.Exists(excelPath)) return list;

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var p = new ExcelPackage(new FileInfo(excelPath)))
            {
                var ws = p.Workbook.Worksheets.FirstOrDefault();
                if (ws == null) return list;
                int colCount = ws.Dimension.End.Column;
                int rowCount = ws.Dimension.End.Row;
                if (rowCount < 2) return list;

                var headers = new List<string>();
                for (int c = 1; c <= colCount; c++)
                    headers.Add(ws.Cells[1, c].Text?.Trim());

                for (int r = 2; r <= rowCount; r++)
                {
                    var model = new ProposalModel();
                    for (int c = 1; c <= colCount; c++)
                    {
                        var key = headers[c - 1];
                        var val = ws.Cells[r, c].Text?.Trim();

                        // mapear colunas comuns (ajuste conforme seu Excel)
                        if (string.Equals(key, "NOME_COMPLETO", StringComparison.OrdinalIgnoreCase))
                            model.NomeTitular = val;
                        else if (string.Equals(key, "CPF_TITULAR", StringComparison.OrdinalIgnoreCase))
                            model.CpfTitular = val;
                        else if (string.Equals(key, "ENDERECO", StringComparison.OrdinalIgnoreCase))
                            model.Endereco = val;
                        else if (string.Equals(key, "EMAIL", StringComparison.OrdinalIgnoreCase))
                            model.Email = val;
                        // acrescente outros mapeamentos conforme necessário
                    }
                    list.Add(model);
                }
            }
            return list;
        }
    }
}
