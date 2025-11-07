using Plennusc.Core.Models.ModelsGestao.modelsStructure;
using Plennusc.Core.Models.ModelsSysPlatform;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.estruturaTipo;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.SysPlatform;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.PlatformSys
{
    public class listPlatformService
    {
        private readonly string _cs;
        public listPlatformService(string connectionStringName = "Plennus")
        {
            _cs = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }
        public SqlConnection Open()
        {
            var c = new SqlConnection(_cs);
            c.Open();
            return c;
        }
        public List<SysPlatformModel> GetSistema()
        {
            var lista = new List<SysPlatformModel>();

            using (var con = Open())
            using (var cmd = new SqlCommand(listPlatformQueries.VerificaSistamaExistente, con))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lista.Add(new SysPlatformModel
                    {
                        CodSistema = Convert.ToInt32(reader["CodSistema"]),
                        NomeDisplay = reader["NomeDisplay"].ToString(),
                        Descricao = reader["Descricao"].ToString(),
                        Conf_Logo =  reader["Conf_Logo"].ToString(),
                    });
                }
            }
            return lista;
        }
    }
}
