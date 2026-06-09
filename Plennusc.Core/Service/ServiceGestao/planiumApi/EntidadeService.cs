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
    public class EntidadeService
    {
        private readonly string _cs;

        public EntidadeService(string connectionStringName = "Plennus")
        {
            _cs = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        private SqlConnection Open()
        {
            var c = new SqlConnection(_cs);
            c.Open();
            return c;
        }

        public List<EntidadeListDto> ListarEntidade(EntidadeFiltro filtro)
        {
            var lista = new List<EntidadeListDto>();

            using (var con = Open())
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = con;

                var sql = new StringBuilder(@"
                   SELECT
                        ent.CodigoEntidade,
                        ent.RazaoSocial,
                        ent.Numero_CNPJ,
                        ent.Conf_Ativo
                    FROM dbo.API_Venda_Entidade ent
                    WHERE
                        ent.Informacoes_log_e IS NULL
                ");

                // Aplicar filtros corretamente
                if (!string.IsNullOrWhiteSpace(filtro?.NomeEntidade))
                {
                    sql.Append(" AND ent.RazaoSocial LIKE @RazaoSocial");
                    cmd.Parameters.AddWithValue("@RazaoSocial", "%" + filtro.NomeEntidade.Trim() + "%");
                }

                if (!string.IsNullOrWhiteSpace(filtro?.Numero_CNPJ))
                {
                    sql.Append(" AND ent.Numero_CNPJ LIKE @Numero_CNPJ");
                    cmd.Parameters.AddWithValue("@Numero_CNPJ", "%" + filtro.Numero_CNPJ.Trim() + "%");
                }

                // Ordenação final
                sql.Append(" ORDER BY ent.CodigoEntidade ASC");

                cmd.CommandText = sql.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var item = new EntidadeListDto
                        {
                            CodigoEntidade = rd.GetInt32(rd.GetOrdinal("CodigoEntidade")),
                            RazaoSocial = rd.GetString(rd.GetOrdinal("RazaoSocial")),
                            Numero_CNPJ = rd.IsDBNull(rd.GetOrdinal("Numero_CNPJ")) ? null : rd.GetString(rd.GetOrdinal("Numero_CNPJ")),
                            Conf_Ativo = rd.GetBoolean(rd.GetOrdinal("Conf_Ativo")),
                        };

                        lista.Add(item);
                    }
                }
            }

            return lista;
        }
    }
}
