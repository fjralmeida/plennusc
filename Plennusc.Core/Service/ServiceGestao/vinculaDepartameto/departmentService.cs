using Plennusc.Core.Mappers.MappersGestao;
using Plennusc.Core.Models.ModelsGestao.modelsLinkSector;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.department;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.vinculaDepartameto
{
    public class departmentService
    {
        private string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Plennus"].ConnectionString;

        private SqlConnection Open()
        {
            var con = new SqlConnection(_connectionString);
            con.Open();
            return con;
        }

        public List<departmentModel> GetTodosDepartamentos()
        {
            var list = new List<departmentModel>();
            using (var con = Open())
            using (var cmd = new SqlCommand(departmentQueries.BuscarTodosDepartamentos, con))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    // CORREÇÃO: Converter tudo para os tipos corretos
                    list.Add(new departmentModel
                    {
                        CodDepartamento = Convert.ToInt32(reader["CodDepartamento"]),
                        Nome = reader["Nome"]?.ToString(),
                        NumRamal = reader["NumRamal"]?.ToString(),
                        EmailGeral = reader["EmailGeral"]?.ToString(),
                        Telefone = reader["Telefone"]?.ToString()
                    });
                }
            }
            return list;
        }

        public departmentModel GetDepartamentoPorId(int codDepartamento)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(departmentQueries.BuscarDepartamentoPorId, con))
            {
                cmd.Parameters.AddWithValue("@CodDepartamento", codDepartamento);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new departmentModel
                        {
                            CodDepartamento = reader.GetInt32(0),
                            Nome = reader.GetString(1),
                            NumRamal = reader.IsDBNull(2) ? null : reader.GetString(2),
                            EmailGeral = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Telefone = reader.IsDBNull(4) ? null : reader.GetString(4)
                        };
                    }
                }
            }
            return null;
        }
    }
}