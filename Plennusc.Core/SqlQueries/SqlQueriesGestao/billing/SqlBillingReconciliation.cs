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

        public decimal? BuscarValorOperadoraPorCpfECredencial(string cpf, string credencial, string mesAnoReferencia)
        {
            string connStr = ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;
            string sql = @"
                SELECT VALOR_OPERADORA
                FROM VW_RELATORIO_CONFERENCIA
                WHERE NUMERO_CPF = @Cpf
                  AND NUMERO_CARTEIRINHA = @Credencial
                  AND MES_ANO_REFERENCIA = @MesAnoReferencia
                  AND TIPO = @Tipo";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Cpf", cpf);
                cmd.Parameters.AddWithValue("@Credencial", credencial);
                cmd.Parameters.AddWithValue("@MesAnoReferencia", mesAnoReferencia);
                cmd.Parameters.AddWithValue("@Tipo", "CONVÊNIO");
                conn.Open();

                var resultado = cmd.ExecuteScalar();
                return resultado != null && resultado != DBNull.Value
                    ? Convert.ToDecimal(resultado)
                    : (decimal?)null;
            }
        }

        public decimal? BuscarValorOdontologicoPorCpf(string cpf, string mesAnoReferencia, int codigoGrupoContrato)
        {
            string connStr = ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;
            string sql = @"
                SELECT SUM(VALOR_OPERADORA)
                FROM VW_RELATORIO_CONFERENCIA
                WHERE NUMERO_CPF = @Cpf
                  AND MES_ANO_REFERENCIA = @MesAnoReferencia
                  AND CODIGO_GRUPO_CONTRATO = @CodigoGrupoContrato
                  AND TIPO = @Tipo";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Cpf", cpf);
                cmd.Parameters.AddWithValue("@MesAnoReferencia", mesAnoReferencia);
                cmd.Parameters.AddWithValue("@CodigoGrupoContrato", codigoGrupoContrato);
                cmd.Parameters.AddWithValue("@Tipo", "EVENTO ADICIONAL");
                conn.Open();

                var resultado = cmd.ExecuteScalar();
                return resultado != null && resultado != DBNull.Value
                    ? Convert.ToDecimal(resultado)
                    : (decimal?)null;
            }
        }
    }
}
