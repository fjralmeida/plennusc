using Plennusc.Core.Models.ModelsGestao;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.planiumApi
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
                        pre.CodigoTabelaPreco,
                        pla.NomePlanoComercial, 
                        pre.FaixaBeneficiarios,
                        pre.DataInicioVenda,
                        pre.DataFimVenda,
                        pre.Faixa0,
                        pre.Faixa1,
                        pre.Faixa2,
                        pre.Faixa3,
                        pre.Faixa4,
                        pre.Faixa5,
                        pre.Faixa6,
                        pre.Faixa7,
                        pre.Faixa8,
                        pre.Faixa9,
                        pre.Conf_ExibirVenda
                FROM dbo.API_Venda_TabelaPreco pre
                INNER JOIN dbo.API_Venda_Plano pla 
                ON pre.CodigoPlano = pla.CodigoPlano
                WHERE (1=1)"
                );

                if (!string.IsNullOrWhiteSpace(filtro?.NomePlanoComercial))
                {
                    sql.Append(" AND pla.NomePlanoComercial LIKE @NomePlanoComercial");
                    cmd.Parameters.AddWithValue("@NomePlanoComercial", "%" + filtro.NomePlanoComercial.Trim() + "%");
                }

                sql.Append(" ORDER BY pre.CodigoTabelaPreco");
                cmd.CommandText = sql.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new PrecoListDto
                        {
                            CodigoTabelaPreco = rd.GetInt32(rd.GetOrdinal("CodigoTabelaPreco")),
                            NomePlanoComercial = rd.IsDBNull(rd.GetOrdinal("NomePlanoComercial")) ? null : rd.GetString(rd.GetOrdinal("NomePlanoComercial")),
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