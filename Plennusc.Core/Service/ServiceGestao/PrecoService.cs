using Plennusc.Core.Models.ModelsGestao;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao
{
    public class PrecoService
    {
        private readonly string _cs;

        public PrecoService(string connectionStringName = "Plennus")
        {
            _cs = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        private SqlConnection Open()
        {
            var c = new SqlConnection(_cs);
            c.Open();
            return c;
        }
        public List<PrecoListDto> ListarPrecos(PrecoFiltro filtro)
        {
            var lista = new List<PrecoListDto>();

            using (var con = Open())
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = con;

                var sql = new StringBuilder(@"
            SELECT
                tp.CodigoTabelaPreco,
                tp.CodigoPlano,
                tp.CodigoProduto,
                tp.FaixaBeneficiarios,
                tp.DataInicioVenda,
                tp.DataFimVenda,
                tp.Faixa0,
                tp.Faixa1,
                tp.Faixa2,
                tp.Faixa3,
                tp.Faixa4,
                tp.Faixa5,
                tp.Faixa6,
                tp.Faixa7,
                tp.Faixa8,
                tp.Faixa9,
                tp.Conf_ExibirVenda
            FROM dbo.API_Venda_TabelaPreco tp
            WHERE 1 = 1");

                if (filtro?.CodigoPlano > 0)
                {
                    sql.Append(" AND tp.CodigoPlano = @CodigoPlano");
                    cmd.Parameters.AddWithValue("@CodigoPlano", filtro.CodigoPlano);
                }

                if (filtro?.CodigoProduto > 0)
                {
                    sql.Append(" AND tp.CodigoProduto = @CodigoProduto");
                    cmd.Parameters.AddWithValue("@CodigoProduto", filtro.CodigoProduto);
                }

                sql.Append(" ORDER BY tp.CodigoTabelaPreco");
                cmd.CommandText = sql.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new PrecoListDto
                        {
                            CodigoTabelaPreco = rd.GetInt32(rd.GetOrdinal("CodigoTabelaPreco")),
                            CodigoPlano = rd.GetInt32(rd.GetOrdinal("CodigoPlano")),
                            CodigoProduto = rd.GetInt32(rd.GetOrdinal("CodigoProduto")),
                            FaixaBeneficiarios = rd.GetInt32(rd.GetOrdinal("FaixaBeneficiarios")),
                            DataInicioVenda = rd.GetDateTime(rd.GetOrdinal("DataInicioVenda")),
                            DataFimVenda = rd.IsDBNull(rd.GetOrdinal("DataFimVenda")) ? (DateTime?)null : rd.GetDateTime(rd.GetOrdinal("DataFimVenda")),
                            Faixa0 = rd.IsDBNull(rd.GetOrdinal("Faixa0")) ? (decimal?)null : rd.GetDecimal(rd.GetOrdinal("Faixa0")),
                            Faixa1 = rd.IsDBNull(rd.GetOrdinal("Faixa1")) ? (decimal?)null : rd.GetDecimal(rd.GetOrdinal("Faixa1")),
                            Faixa2 = rd.IsDBNull(rd.GetOrdinal("Faixa2")) ? (decimal?)null : rd.GetDecimal(rd.GetOrdinal("Faixa2")),
                            Faixa3 = rd.IsDBNull(rd.GetOrdinal("Faixa3")) ? (decimal?)null : rd.GetDecimal(rd.GetOrdinal("Faixa3")),
                            Faixa4 = rd.IsDBNull(rd.GetOrdinal("Faixa4")) ? (decimal?)null : rd.GetDecimal(rd.GetOrdinal("Faixa4")),
                            Faixa5 = rd.IsDBNull(rd.GetOrdinal("Faixa5")) ? (decimal?)null : rd.GetDecimal(rd.GetOrdinal("Faixa5")),
                            Faixa6 = rd.IsDBNull(rd.GetOrdinal("Faixa6")) ? (decimal?)null : rd.GetDecimal(rd.GetOrdinal("Faixa6")),
                            Faixa7 = rd.IsDBNull(rd.GetOrdinal("Faixa7")) ? (decimal?)null : rd.GetDecimal(rd.GetOrdinal("Faixa7")),
                            Faixa8 = rd.IsDBNull(rd.GetOrdinal("Faixa8")) ? (decimal?)null : rd.GetDecimal(rd.GetOrdinal("Faixa8")),
                            Faixa9 = rd.IsDBNull(rd.GetOrdinal("Faixa9")) ? (decimal?)null : rd.GetDecimal(rd.GetOrdinal("Faixa9")),
                            Conf_ExibirVenda = rd.GetBoolean(rd.GetOrdinal("Conf_ExibirVenda"))
                        });
                    }
                }
            }

            return lista;
        }
    }
}