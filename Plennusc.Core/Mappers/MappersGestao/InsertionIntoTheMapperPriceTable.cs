using Plennusc.Core.Models.ModelsGestao;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Mappers.MappersGestao
{
    public class InsertionIntoTheMapperPriceTable
    {
        private static readonly CultureInfo PtBr = new CultureInfo("pt-BR");

        private static int ToInt(object v)
        {
            if (v == null || v == DBNull.Value) return 0;
            if (v is int i) return i;
            if (v is long l) return (int)l;
            if (v is double d) return (int)Math.Truncate(d);

            var s = Convert.ToString(v)?.Trim();
            if (string.IsNullOrWhiteSpace(s)) return 0;

            if (int.TryParse(s, NumberStyles.Any, PtBr, out var a)) return a;
            if (int.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var b)) return b;
            return 0;
        }

        private static decimal ToDec(object v)
        {
            if (v == null || v == DBNull.Value) return 0m;
            if (v is decimal dec) return dec;
            if (v is double dbl) return (decimal)dbl;

            var s = Convert.ToString(v)?.Trim();
            if (string.IsNullOrWhiteSpace(s)) return 0m;

            if (decimal.TryParse(s, NumberStyles.Any, PtBr, out var a)) return a;

            s = s.Replace(".", "").Replace(",", ".");
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var b)) return b;

            return 0m;
        }
        // 🔸 Força 2 casas decimais (padrão dinheiro)
        private static decimal ToMoney2(object v)
            => Math.Round(ToDec(v), 2, MidpointRounding.AwayFromZero);
        private static string ToStr(object v) => v == null || v == DBNull.Value ? null : v.ToString().Trim();

        public static DataInsertionPriceTableMessage FromDataRow(DataRow r)
        {
            return new DataInsertionPriceTableMessage
            {
                NUMERO_REGISTRO = ToInt(r["NUMERO_REGISTRO"]),
                CODIGO_PLANO = ToInt(r["CODIGO_PLANO"]),
                CODIGO_TABELA_PRECO = ToInt(r["CODIGO_TABELA_PRECO"]),
                IDADE_MINIMA = ToInt(r["IDADE_MINIMA"]),
                IDADE_MAXIMA = ToInt(r["IDADE_MAXIMA"]),
                VALOR_PLANO = ToMoney2(r["VALOR_PLANO"]),
                TIPO_RELACAO_DEPENDENCIA = ToStr(r["TIPO_RELACAO_DEPENDENCIA"]),
                CODIGO_GRUPO_CONTRATO = ToInt(r["CODIGO_GRUPO_CONTRATO"]),
                NOME_TABELA = ToStr(r["NOME_TABELA"]),
                VALOR_NET = ToMoney2(r["VALOR_NET"]),
            };
        }
    }
}
