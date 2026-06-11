using Plennusc.Core.Models.ModelsGestao;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace Plennusc.Core.Service.ServiceGestao.planiumApi
{
    public class OperadoraService
    {
        private readonly string _cs;

        public OperadoraService(string connectionStringName = "Plennus")
        {
            _cs = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        private SqlConnection Open()
        {
            var c = new SqlConnection(_cs);
            c.Open();
            return c;
        }

        public List<OperadoraListDto> ListarOperadoras(OperadoraFiltro filtro)
        {
            var lista = new List<OperadoraListDto>();

            using (var con = Open())
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = con;

                var sql = new StringBuilder(@"
                SELECT
                    oper.CodigoOperadora,
                    oper.NomeComercial,
                    oper.RegistroANS,
                    oper.Numero_CNPJ,
                    oper.RazaoSocial,
                    oper.NomeComercial
                FROM dbo.API_Venda_Operadora oper
                WHERE 1 = 1");

                if (!string.IsNullOrWhiteSpace(filtro?.NomeOperadora))
                {
                    sql.Append(" AND oper.NomeComercial LIKE @NomeOperadora");
                    cmd.Parameters.AddWithValue("@NomeOperadora", "%" + filtro.NomeOperadora.Trim() + "%");
                }

                if (!string.IsNullOrWhiteSpace(filtro?.RegistroANS))
                {
                    sql.Append(" AND CAST(oper.RegistroANS AS VARCHAR) LIKE @RegistroANS");
                    cmd.Parameters.AddWithValue("@RegistroANS", "%" + filtro.RegistroANS.Trim() + "%");
                }

                if (!string.IsNullOrWhiteSpace(filtro?.Numero_CNPJ))
                {
                    sql.Append(" AND oper.Numero_CNPJ LIKE @Numero_CNPJ");
                    cmd.Parameters.AddWithValue("@Numero_CNPJ", "%" + filtro.Numero_CNPJ.Trim() + "%");
                }

                sql.Append(" ORDER BY oper.CodigoOperadora");
                cmd.CommandText = sql.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new OperadoraListDto
                        {
                            CodigoOperadora = rd.GetInt32(rd.GetOrdinal("CodigoOperadora")),
                            RegistroAns = rd.IsDBNull(rd.GetOrdinal("RegistroANS")) ? null : rd.GetInt32(rd.GetOrdinal("RegistroANS")).ToString(),
                            Numero_CNPJ = rd.IsDBNull(rd.GetOrdinal("Numero_CNPJ")) ? null : rd.GetString(rd.GetOrdinal("Numero_CNPJ")),
                            RazaoSocial = rd.IsDBNull(rd.GetOrdinal("RazaoSocial")) ? null : rd.GetString(rd.GetOrdinal("RazaoSocial")),
                            NomeComercial = rd.IsDBNull(rd.GetOrdinal("NomeComercial")) ? null : rd.GetString(rd.GetOrdinal("NomeComercial"))
                        });
                    }
                }
            }

            return lista;
        }
    }
}