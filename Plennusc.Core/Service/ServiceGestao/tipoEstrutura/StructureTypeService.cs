using Plennusc.Core.SqlQueries.SqlQueriesGestao.estruturaTipo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.TipoEstrutura
{
    public class StructureTypeService
    {
        private readonly string _cs;

        public StructureTypeService(string connectionStringName = "Plennus")
        {
            _cs = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        public SqlConnection Open()
        {
            var c = new SqlConnection(_cs);
            c.Open();
            return c;
        }

        public int InserirEstruturaPai(string nomeEstrutura, string descricao)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(StructureTypeQueries.InserirEstruturaPai, con))
            {
                cmd.Parameters.AddWithValue("@DescEstrutura", nomeEstrutura);
                cmd.Parameters.AddWithValue("@MemoEstrutura", descricao ?? "");
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void InserirSubtipo(int codEstruturaPai, string nomeSubtipo)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(StructureTypeQueries.InserirSubtipo, con))
            {
                cmd.Parameters.AddWithValue("@CodEstruturaPai", codEstruturaPai);
                cmd.Parameters.AddWithValue("@DescEstrutura", nomeSubtipo);
                cmd.Parameters.AddWithValue("@MemoEstrutura", "");
                cmd.ExecuteNonQuery();
            }
        }

        public void VincularSetorEstrutura(int codSetor, int codEstrutura)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(StructureTypeQueries.VincularSetorEstrutura, con))
            {
                cmd.Parameters.AddWithValue("@CodSetor", codSetor);
                cmd.Parameters.AddWithValue("@CodEstr_TipoDemanda", codEstrutura);
                cmd.ExecuteNonQuery();
            }
        }
    }
}