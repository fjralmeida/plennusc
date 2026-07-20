using Plennusc.Core.Models.ModelsGestao.modelsBilling;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.billing
{
    public class SqlBillingReconciliation
    {
        #region BUSCA TODAS OPERADORAS E GRUPOS DE FATURAMENTO
        //public List<OperadoraModel> BuscarOperadoras()
        //{
        //    var lista = new List<OperadoraModel>();

        //    string connStr = ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;

        //    string sql = @"
        //        SELECT DISTINCT
        //            CODIGO_GRUPO_CONTRATO,
        //            NOME_OPERADORA
        //        FROM ESP0002
        //        WHERE NOME_OPERADORA IS NOT NULL
        //        AND NUMERO_ANS_OPERADORA IS NOT NULL
        //        ORDER BY NOME_OPERADORA";

        //    using (SqlConnection conn = new SqlConnection(connStr))
        //    using (SqlCommand cmd = new SqlCommand(sql, conn))
        //    {
        //        conn.Open();
        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            while (reader.Read())
        //            {
        //                lista.Add(new OperadoraModel
        //                {
        //                    CodigoGrupoContrato = Convert.ToInt32(reader["CODIGO_GRUPO_CONTRATO"]),
        //                    NomeOperadora = reader["NOME_OPERADORA"].ToString()
        //                });
        //            }
        //        }
        //    }

        //    return lista;
        //}
        #endregion

        public OperadoraModel BuscarOperadoras()
        {
            OperadoraModel operadora = null;
            string connStr = ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;
            string sql = @"
                SELECT DISTINCT
                    CODIGO_GRUPO_CONTRATO,
                    NOME_OPERADORA
                FROM ESP0002
                WHERE NOME_OPERADORA IS NOT NULL
                AND NUMERO_ANS_OPERADORA IS NOT NULL
                AND CODIGO_GRUPO_CONTRATO = 4";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        operadora = new OperadoraModel
                        {
                            CodigoGrupoContrato = Convert.ToInt32(reader["CODIGO_GRUPO_CONTRATO"]),
                            NomeOperadora = reader["NOME_OPERADORA"].ToString()
                        };
                    }
                }
            }
            return operadora;
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
            ps.VALOR_NET_CORRIGIDO,
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
                            NumeroRegistro = reader["NUMERO_REGISTRO"] != DBNull.Value ? Convert.ToInt32(reader["NUMERO_REGISTRO"]) : 0,
                            CodigoEmpresa = reader["CODIGO_EMPRESA"] != DBNull.Value ? Convert.ToInt32(reader["CODIGO_EMPRESA"]) : 0,
                            CodigoAssociado = reader["CODIGO_ASSOCIADO"] != DBNull.Value ? reader["CODIGO_ASSOCIADO"].ToString() : "",
                            MesAnoReferencia = reader["MES_ANO_REFERENCIA"] != DBNull.Value ? reader["MES_ANO_REFERENCIA"].ToString() : "",
                            ValorConvenio = reader["VALOR_NET_CORRIGIDO"] != DBNull.Value ? Convert.ToDecimal(reader["VALOR_NET_CORRIGIDO"]) : 0,
                            NomeDoAssociado = reader["NOME_ASSOCIADO"] != DBNull.Value ? reader["NOME_ASSOCIADO"].ToString() : "",
                            NumeroCpf = reader["NUMERO_CPF"] != DBNull.Value ? reader["NUMERO_CPF"].ToString() : "",
                            DataAdmissao = reader["DATA_ADMISSAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_ADMISSAO"]) : (DateTime?)null,
                            DataExclusao = reader["DATA_EXCLUSAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_EXCLUSAO"]) : (DateTime?)null,
                            NomeMotivoExclusao = "" // se quiser buscar o nome do motivo em outra tabela, faça um JOIN ou consulta adicional
                        });
                    }
                }
            }
            return lista;
        }
        public void ConferirInconsistencias(List<ItemInconsistenciaFaturamento> itens)
        {
            if (itens == null || itens.Count == 0)
                throw new ArgumentException("Nenhum item para conferir.");

            string connStr = ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;

            string sql = @"
        UPDATE PS1021
        SET DATA_CONFERENCIA_FATUR = GETDATE()
        WHERE NUMERO_REGISTRO = @NumeroRegistro
          AND CODIGO_EMPRESA = @CodigoEmpresa
          AND CODIGO_ASSOCIADO = @CodigoAssociado
          AND MES_ANO_VENCIMENTO = @MesAnoReferencia";

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@NumeroRegistro", SqlDbType.Int);
                    cmd.Parameters.Add("@CodigoEmpresa", SqlDbType.Int);
                    cmd.Parameters.Add("@CodigoAssociado", SqlDbType.VarChar, 20);
                    cmd.Parameters.Add("@MesAnoReferencia", SqlDbType.VarChar, 10);

                    foreach (var item in itens)
                    {
                        cmd.Parameters["@NumeroRegistro"].Value = item.NumeroRegistro;
                        cmd.Parameters["@CodigoEmpresa"].Value = item.CodigoEmpresa;
                        cmd.Parameters["@CodigoAssociado"].Value = item.CodigoAssociado;
                        cmd.Parameters["@MesAnoReferencia"].Value = item.MesAnoReferencia;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
