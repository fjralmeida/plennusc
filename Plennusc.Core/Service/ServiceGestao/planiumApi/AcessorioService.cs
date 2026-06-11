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
    public class AcessorioService
    {
        private readonly string _cs;

        public AcessorioService(string connectionStringName = "Plennus")
        {
            _cs = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        private SqlConnection Open()
        {
            var c = new SqlConnection(_cs);
            c.Open();
            return c;
        }

        public List<AcessorioListDto> ListarAcessorios(AcessorioFiltro filtro)
        {
            var lista = new List<AcessorioListDto>();

            using (var con = Open())
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = con;

                var sql = new StringBuilder(@"
                       SELECT 
                        aces.CodigoAcessorio,
                        aces.Nome,
                        aces.Valor,
                        aces.Modo,
                        aces.QuantidadeProduto,
                        aces.ACC_Sincor,
                        aces.Conf_Ativo,
                        aces.Conf_Exibir
                    FROM API_Venda_Acessorio aces
                    WHERE 1 = 1
                ");

                if (!string.IsNullOrWhiteSpace(filtro?.NomeAcessorio))
                {
                    sql.Append(" AND aces.Nome LIKE @NomeAcessorio");
                    cmd.Parameters.AddWithValue("@NomeAcessorio", "%" + filtro.NomeAcessorio.Trim() + "%");
                }


                sql.Append(" ORDER BY aces.CodigoAcessorio ASC");

                cmd.CommandText = sql.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var item = new AcessorioListDto
                        {
                            CodigoAcessorio = rd.GetInt32(rd.GetOrdinal("CodigoAcessorio")),
                            NomeAcessorio = rd.GetString(rd.GetOrdinal("NomeAcessorio")),
                            ValorAcessorio = rd.GetInt32(rd.GetOrdinal("ValorAcessorio")),
                            QuantidadeProduto = rd.GetInt32(rd.GetOrdinal("QuantidadeProduto")),
                            ACC_Sincor = rd.GetString(rd.GetOrdinal("CodigoAcessorio")),
                            Conf_Ativo = rd.GetBoolean(rd.GetOrdinal("Conf_Ativo")),
                            Conf_Exibir = rd.GetBoolean(rd.GetOrdinal("Conf_Exibir")),
                        };

                        lista.Add(item);
                    }
                }
            }

            return lista;
        }
    }
}
