using Plennusc.Core.Models.ModelsGestao.ModelsHome;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.demanda;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.home
{
    public class HomeManagementService
    {
        private readonly string _connectionString;

        public HomeManagementService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public homeManagementModels CarregarWidgetsDemandas(int codPessoa)
        {
            var widgets = new homeManagementModels();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                con.Open();

                // WIDGET 1: ESTATÍSTICAS DO DIA
                using (SqlCommand cmd = new SqlCommand(HomeManagementQueries.WidgetEstatisticasHoje, con))
                {
                    cmd.Parameters.AddWithValue("@CodPessoa", codPessoa);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) widgets.NovasDemandasHoje = reader.GetInt32(0);
                        if (reader.NextResult() && reader.Read()) widgets.DemandasFinalizadasHoje = reader.GetInt32(0);
                        if (reader.NextResult() && reader.Read()) widgets.AprovacoesPendentes = reader.GetInt32(0);
                        if (reader.NextResult() && reader.Read()) widgets.AtrasosCriticos = reader.GetInt32(0);
                    }
                }

                // WIDGET 2: DEMANDAS POR STATUS
                using (SqlCommand cmd = new SqlCommand(HomeManagementQueries.WidgetStatusDemandas, con))
                {
                    cmd.Parameters.AddWithValue("@CodPessoa", codPessoa);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) widgets.DemandasAbertas = reader.GetInt32(0);
                        if (reader.NextResult() && reader.Read()) widgets.DemandasAndamento = reader.GetInt32(0);
                        if (reader.NextResult() && reader.Read()) widgets.DemandasAguardando = reader.GetInt32(0);
                        if (reader.NextResult() && reader.Read()) widgets.DemandasAtrasadas = reader.GetInt32(0);
                    }
                }
            }

            return widgets;
        }
    }
}