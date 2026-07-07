using Plennusc.Core.Models.ModelsGestao.modelsBilling;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.billing
{
    public class SqlBillingReconciliation
    {
        public List<OperadoraModel> BuscarOperadoras()
        {
            var lista = new List<OperadoraModel>();

            string connStr = ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;

            string sql = @"
                SELECT DISTINCT
                    CODIGO_GRUPO_CONTRATO,
                    NOME_OPERADORA
                FROM ESP0002
                WHERE NOME_OPERADORA IS NOT NULL
                AND NUMERO_ANS_OPERADORA IS NOT NULL
                ORDER BY NOME_OPERADORA";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new OperadoraModel
                        {
                            CodigoGrupoContrato = Convert.ToInt32(reader["CODIGO_GRUPO_CONTRATO"]),
                            NomeOperadora = reader["NOME_OPERADORA"].ToString()
                        });
                    }
                }
            }

            return lista;
        }

        public List<GrupoFaturamentoModel> BuscarGruposFaturamento()
        {
            var lista = new List<GrupoFaturamentoModel>();

            string connStr = ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;

            string sql = @"
                SELECT
                    CODIGO_GRUPO_FATURAMENTO,
                    DESCRICAO_GP_FATURAMENTO
                FROM PS1051
                ORDER BY DESCRICAO_GP_FATURAMENTO";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new GrupoFaturamentoModel
                        {
                            CodigoGrupoFaturamento = Convert.ToInt32(reader["CODIGO_GRUPO_FATURAMENTO"]),
                            DescricaoGrupoFaturamento = reader["DESCRICAO_GP_FATURAMENTO"].ToString()
                        });
                    }
                }
            }

            return lista;
        }
    }
}
