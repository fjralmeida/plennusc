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
    public class ProfissaoService
    {
        private readonly string _cs;

        public ProfissaoService(string connectionStringName = "Plennus")
        {
            _cs = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        private SqlConnection Open()
        {
            var c = new SqlConnection(_cs);
            c.Open();
            return c;
        }

        public List<ProfissaoListDto> ListarProfissao(ProfissaoFiltro filtro)
        {
            var lista = new List<ProfissaoListDto>();

            using (var con = Open())
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = con;

                var sql = new StringBuilder(@"
                   SELECT 
                        prof.CodigoProfissao,
                        prof.Nome,
                        prof.CodCBO
                   FROM API_Venda_Profissao prof
                   WHERE 1 = 1
                ");

                if (!string.IsNullOrWhiteSpace(filtro?.NomeProfissao))
                {
                    sql.Append(" AND prof.Nome LIKE @NomeProfissao");
                    cmd.Parameters.AddWithValue("@NomeProfissao", "%" + filtro.NomeProfissao.Trim() + "%");
                }


                sql.Append(" ORDER BY prof.CodigoProfissao ASC");

                cmd.CommandText = sql.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var item = new ProfissaoListDto
                        {
                            CodigoProfissao = rd.GetInt32(rd.GetOrdinal("CodigoProfissao")),
                            NomeProfissao = rd.GetString(rd.GetOrdinal("NomeProfissao")),
                        };

                        lista.Add(item);
                    }
                }
            }

            return lista;
        }
    }
}
