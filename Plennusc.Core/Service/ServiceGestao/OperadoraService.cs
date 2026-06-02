using Plennusc.Core.Models.ModelsGestao;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace Plennusc.Core.Service.ServiceGestao
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
                        o.CodOperadora,
                        o.RegistroAns,
                        o.CNPJ,
                        o.RazaoSocial,
                        o.NomeComercial
                    FROM dbo.API_Venda_Operadora o
                    WHERE 1 = 1");

                if (!string.IsNullOrWhiteSpace(filtro?.NomeOperadora))
                {
                    sql.Append(" AND o.NomeComercial LIKE @NomeOperadora");
                    cmd.Parameters.AddWithValue("@NomeOperadora", "%" + filtro.NomeOperadora.Trim() + "%");
                }

                if (!string.IsNullOrWhiteSpace(filtro?.RegistroAns))
                {
                    sql.Append(" AND o.RegistroAns LIKE @RegistroAns");
                    cmd.Parameters.AddWithValue("@RegistroAns", "%" + filtro.RegistroAns.Trim() + "%");
                }

                if (!string.IsNullOrWhiteSpace(filtro?.Cnpj))
                {
                    sql.Append(" AND o.CNPJ LIKE @Cnpj");
                    cmd.Parameters.AddWithValue("@Cnpj", "%" + filtro.Cnpj.Trim() + "%");
                }

                sql.Append(" ORDER BY o.NomeComercial");
                cmd.CommandText = sql.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        lista.Add(new OperadoraListDto
                        {
                            CodOperadora = rd.GetInt32(rd.GetOrdinal("CodOperadora")),
                            RegistroAns = rd.IsDBNull(rd.GetOrdinal("RegistroAns")) ? null : rd.GetString(rd.GetOrdinal("RegistroAns")),
                            Cnpj = rd.IsDBNull(rd.GetOrdinal("CNPJ")) ? null : rd.GetString(rd.GetOrdinal("CNPJ")),
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