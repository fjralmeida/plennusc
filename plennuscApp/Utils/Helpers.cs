using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace appWhatsapp.Utils
{
    public class Helpers
    {
        protected string ObterCelula(ExcelWorksheet sheet, int row, Dictionary<string, int> mapa, string prop)
        {
            if (mapa.TryGetValue(prop, out int col))
                return sheet.Cells[row, col].Text?.Trim() ?? "";
            return "";
        }

        protected string ObterCelula(ExcelWorksheet sheet, int row, int col)
            => sheet.Cells[row, col].Text?.Trim() ?? "";

        protected string ObterValorCsv(string[] colunas, Dictionary<string, int> mapa, string prop)
        {
            if (mapa.TryGetValue(prop, out int idx) && idx < colunas.Length)
                return colunas[idx]?.Trim() ?? "";
            return "";
        }

        protected string[] SplitCsvLinha(string linha, char sep)
        {
            var campos = new List<string>();
            bool dentroAspas = false;
            var atual = new System.Text.StringBuilder();

            foreach (char c in linha)
            {
                if (c == '"') { dentroAspas = !dentroAspas; continue; }
                if (c == sep && !dentroAspas) { campos.Add(atual.ToString()); atual.Clear(); continue; }
                atual.Append(c);
            }

            campos.Add(atual.ToString());
            return campos.ToArray();
        }
    }
}