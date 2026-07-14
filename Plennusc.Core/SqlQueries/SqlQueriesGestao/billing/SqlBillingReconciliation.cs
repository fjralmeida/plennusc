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

        //public decimal? BuscarValorOperadoraPorCpfECredencial(string cpf, string credencial, string mesAnoReferencia)
        //{
        //    string connStr = ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;
        //    string sql = @"
        //        SELECT VALOR_OPERADORA
        //        FROM VW_RELATORIO_CONFERENCIA
        //        WHERE NUMERO_CPF = @Cpf
        //          AND NUMERO_CARTEIRINHA = @Credencial
        //          AND MES_ANO_REFERENCIA = @MesAnoReferencia
        //          AND TIPO = @Tipo";

        //    using (SqlConnection conn = new SqlConnection(connStr))
        //    using (SqlCommand cmd = new SqlCommand(sql, conn))
        //    {
        //        cmd.Parameters.AddWithValue("@Cpf", cpf);
        //        cmd.Parameters.AddWithValue("@Credencial", credencial);
        //        cmd.Parameters.AddWithValue("@MesAnoReferencia", mesAnoReferencia);
        //        cmd.Parameters.AddWithValue("@Tipo", "CONVÊNIO");
        //        conn.Open();

        //        var resultado = cmd.ExecuteScalar();
        //        return resultado != null && resultado != DBNull.Value
        //            ? Convert.ToDecimal(resultado)
        //            : (decimal?)null;
        //    }
        //}

        //public decimal? BuscarValorOdontologicoPorCpf(string cpf, string mesAnoReferencia, int codigoGrupoContrato)
        //{
        //    string connStr = ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;
        //    string sql = @"
        //        SELECT SUM(VALOR_OPERADORA)
        //        FROM VW_RELATORIO_CONFERENCIA
        //        WHERE NUMERO_CPF = @Cpf
        //          AND MES_ANO_REFERENCIA = @MesAnoReferencia
        //          AND CODIGO_GRUPO_CONTRATO = @CodigoGrupoContrato
        //          AND TIPO = @Tipo";

        //    using (SqlConnection conn = new SqlConnection(connStr))
        //    using (SqlCommand cmd = new SqlCommand(sql, conn))
        //    {
        //        cmd.Parameters.AddWithValue("@Cpf", cpf);
        //        cmd.Parameters.AddWithValue("@MesAnoReferencia", mesAnoReferencia);
        //        cmd.Parameters.AddWithValue("@CodigoGrupoContrato", codigoGrupoContrato);
        //        cmd.Parameters.AddWithValue("@Tipo", "EVENTO ADICIONAL");
        //        conn.Open();

        //        var resultado = cmd.ExecuteScalar();
        //        return resultado != null && resultado != DBNull.Value
        //            ? Convert.ToDecimal(resultado)
        //            : (decimal?)null;
        //    }
        //}

        public ResultadoViewConferencia BuscarDadosConvenioPorCpfECredencial(string cpf, string credencial, string mesAnoReferencia)
        {
            string connStr = ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;
            string sql = @"
        SELECT TOP 1
            VALOR_OPERADORA,
            DATA_ADMISSAO,
            DATA_EXCLUSAO,
            NOME_MOTIVO_EXCLUSAO,
            NOME_TABELA_PRECO,
            NOME_GRUPO_DE_PESSOAS,
            DESCRICAO_GRUPO_FATURAMENTO
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

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapearResultado(reader);
                }
            }
            return null;
        }

        public ResultadoViewConferencia BuscarDadosOdontologicoPorCpf(string cpf, string mesAnoReferencia, int codigoGrupoContrato)
        {
            string connStr = ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;
            string sql = @"
        SELECT TOP 1
            VALOR_OPERADORA,
            DATA_ADMISSAO,
            DATA_EXCLUSAO,
            NOME_MOTIVO_EXCLUSAO,
            NOME_TABELA_PRECO,
            NOME_GRUPO_DE_PESSOAS,
            DESCRICAO_GRUPO_FATURAMENTO
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

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                        return MapearResultado(reader);
                }
            }
            return null;
        }

        private ResultadoViewConferencia MapearResultado(SqlDataReader reader)
        {
            return new ResultadoViewConferencia
            {
                ValorOperadora = reader["VALOR_OPERADORA"] != DBNull.Value ? Convert.ToDecimal(reader["VALOR_OPERADORA"]) : (decimal?)null,
                DataAdmissao = reader["DATA_ADMISSAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_ADMISSAO"]) : (DateTime?)null,
                DataExclusao = reader["DATA_EXCLUSAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_EXCLUSAO"]) : (DateTime?)null,
                NomeMotivoExclusao = reader["NOME_MOTIVO_EXCLUSAO"] != DBNull.Value ? reader["NOME_MOTIVO_EXCLUSAO"].ToString() : null,
                NomeTabelaPreco = reader["NOME_TABELA_PRECO"] != DBNull.Value ? reader["NOME_TABELA_PRECO"].ToString() : null,
                NomeGrupoPessoas = reader["NOME_GRUPO_DE_PESSOAS"] != DBNull.Value ? reader["NOME_GRUPO_DE_PESSOAS"].ToString() : null,
                DescricaoGrupoFaturamento = reader["DESCRICAO_GRUPO_FATURAMENTO"] != DBNull.Value ? reader["DESCRICAO_GRUPO_FATURAMENTO"].ToString() : null
            };
        }

        public List<ItemInconsistenciaFaturamento> BuscarInconsistenciasFaturamento(string mesAnoReferencia, int codigoGrupoContrato)
        {
            var lista = new List<ItemInconsistenciaFaturamento>();
            string connStr = ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;

            string sql = @"
        SELECT 
            ps.NUMERO_REGISTRO,
            ps.CODIGO_EMPRESA,
            ps.CODIGO_ASSOCIADO,
            ps.MES_ANO_VENCIMENTO AS MES_ANO_REFERENCIA,
            ps.VALOR_CONVENIO,
            ps.VALOR_ADICIONAL,
            ps.VALOR_FATURA,
            p1000.NOME_ASSOCIADO,
            p1000.NUMERO_CPF,
            p1000.DATA_ADMISSAO,
            p1000.DATA_EXCLUSAO,
            p1000.CODIGO_MOTIVO_EXCLUSAO
        FROM PS1021 ps
        INNER JOIN PS1000 p1000 ON ps.CODIGO_ASSOCIADO = p1000.CODIGO_ASSOCIADO
        WHERE ps.MES_ANO_VENCIMENTO = @MesAnoReferencia
          AND ps.DATA_CONFERENCIA_FATUR IS NULL
          AND p1000.CODIGO_GRUPO_CONTRATO = @CodigoGrupoContrato";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MesAnoReferencia", mesAnoReferencia);
                cmd.Parameters.AddWithValue("@CodigoGrupoContrato", codigoGrupoContrato);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new ItemInconsistenciaFaturamento
                        {
                            NumeroRegistro = Convert.ToInt32(reader["NUMERO_REGISTRO"]),
                            CodigoEmpresa = Convert.ToInt32(reader["CODIGO_EMPRESA"]),
                            CodigoAssociado = reader["CODIGO_ASSOCIADO"].ToString(),
                            MesAnoReferencia = reader["MES_ANO_REFERENCIA"].ToString(),
                            ValorConvenio = reader["VALOR_CONVENIO"] != DBNull.Value ? Convert.ToDecimal(reader["VALOR_CONVENIO"]) : 0,
                            ValorAdicional = reader["VALOR_ADICIONAL"] != DBNull.Value ? Convert.ToDecimal(reader["VALOR_ADICIONAL"]) : 0,
                            ValorFatura = reader["VALOR_FATURA"] != DBNull.Value ? Convert.ToDecimal(reader["VALOR_FATURA"]) : 0,
                            NomeDoAssociado = reader["NOME_ASSOCIADO"]?.ToString(),
                            NumeroCpf = reader["NUMERO_CPF"]?.ToString(),
                            DataAdmissao = reader["DATA_ADMISSAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_ADMISSAO"]) : (DateTime?)null,
                            DataExclusao = reader["DATA_EXCLUSAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_EXCLUSAO"]) : (DateTime?)null,
                        });
                    }
                }
            }
            return lista;
        }
    }
}
