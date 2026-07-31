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
    public class SqlRevenueVerified
    {
        public List<ItemFaturamentosConferidos> BuscarFaturamentosConferidos(string mesAnoReferencia, int codigoGrupoContrato, List<int> codigosGrupoFaturamento)
        {
            var lista = new List<ItemFaturamentosConferidos>();
            string connStr = ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;

            // CORRIGIDO: Removidas as colunas que não existem em PS1000
            string sql = @"
                SELECT 
                    ps.NUMERO_REGISTRO,
                    ps.CODIGO_EMPRESA,
                    ps.CODIGO_ASSOCIADO,
                    ps.MES_ANO_VENCIMENTO AS MES_ANO_REFERENCIA,
                    ps.VALOR_NET_CORRIGIDO AS VALOR_CONVENIO,
                    ps.VALOR_ADICIONAL,
                    ps.VALOR_FATURA,
                    ps.DATA_CONFERENCIA_FATUR,
                    p1000.NOME_ASSOCIADO,
                    p1000.NUMERO_CPF,
                    p1000.DATA_ADMISSAO,
                    p1000.DATA_EXCLUSAO,
                    p1000.CODIGO_MOTIVO_EXCLUSAO,
                    p1051.DESCRICAO_GP_FATURAMENTO AS DESCRICAO_GRUPO_FATURAMENTO
                FROM PS1021 ps
                INNER JOIN PS1000 p1000 ON ps.CODIGO_ASSOCIADO = p1000.CODIGO_ASSOCIADO
                LEFT JOIN PS1051 p1051 ON p1000.CODIGO_GRUPO_FATURAMENTO = p1051.CODIGO_GRUPO_FATURAMENTO
                WHERE ps.MES_ANO_VENCIMENTO = @MesAnoReferencia
                  AND ps.DATA_CONFERENCIA_FATUR IS NOT NULL
                  AND p1000.CODIGO_GRUPO_CONTRATO = @CodigoGrupoContrato";

            // Filtro opcional de Vigência (Grupo de Faturamento)
            if (codigosGrupoFaturamento != null && codigosGrupoFaturamento.Count > 0)
            {
                var nomesParametros = codigosGrupoFaturamento
                    .Select((codigo, indice) => $"@GrupoFat{indice}")
                    .ToList();

                sql += $" AND p1000.CODIGO_GRUPO_FATURAMENTO IN ({string.Join(",", nomesParametros)})";
            }

            sql += " ORDER BY ps.DATA_CONFERENCIA_FATUR DESC";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@MesAnoReferencia", mesAnoReferencia);
                cmd.Parameters.AddWithValue("@CodigoGrupoContrato", codigoGrupoContrato);

                if (codigosGrupoFaturamento != null && codigosGrupoFaturamento.Count > 0)
                {
                    for (int i = 0; i < codigosGrupoFaturamento.Count; i++)
                    {
                        cmd.Parameters.AddWithValue($"@GrupoFat{i}", codigosGrupoFaturamento[i]);
                    }
                }

                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new ItemFaturamentosConferidos
                        {
                            NumeroRegistro = reader["NUMERO_REGISTRO"] != DBNull.Value ? Convert.ToInt32(reader["NUMERO_REGISTRO"]) : 0,
                            CodigoEmpresa = reader["CODIGO_EMPRESA"] != DBNull.Value ? Convert.ToInt32(reader["CODIGO_EMPRESA"]) : 0,
                            CodigoAssociado = reader["CODIGO_ASSOCIADO"] != DBNull.Value ? reader["CODIGO_ASSOCIADO"].ToString() : "",
                            MesAnoReferencia = reader["MES_ANO_REFERENCIA"] != DBNull.Value ? reader["MES_ANO_REFERENCIA"].ToString() : "",
                            ValorConvenio = reader["VALOR_CONVENIO"] != DBNull.Value ? Convert.ToDecimal(reader["VALOR_CONVENIO"]) : 0,
                            ValorAdicional = reader["VALOR_ADICIONAL"] != DBNull.Value ? Convert.ToDecimal(reader["VALOR_ADICIONAL"]) : 0,
                            ValorFatura = reader["VALOR_FATURA"] != DBNull.Value ? Convert.ToDecimal(reader["VALOR_FATURA"]) : 0,
                            DataConferenciaFatur = reader["DATA_CONFERENCIA_FATUR"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_CONFERENCIA_FATUR"]) : (DateTime?)null,
                            NomeDoAssociado = reader["NOME_ASSOCIADO"] != DBNull.Value ? reader["NOME_ASSOCIADO"].ToString() : "",
                            NumeroCpf = reader["NUMERO_CPF"] != DBNull.Value ? reader["NUMERO_CPF"].ToString() : "",
                            // Removidos NomeTabelaPreco e NomeGrupoPessoas pois não existem em PS1000
                            NomeTabelaPreco = "",
                            NomeGrupoPessoas = "",
                            DescricaoGrupoFaturamento = reader["DESCRICAO_GRUPO_FATURAMENTO"] != DBNull.Value ? reader["DESCRICAO_GRUPO_FATURAMENTO"].ToString() : "",
                            DataAdmissao = reader["DATA_ADMISSAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_ADMISSAO"]) : (DateTime?)null,
                            DataExclusao = reader["DATA_EXCLUSAO"] != DBNull.Value ? Convert.ToDateTime(reader["DATA_EXCLUSAO"]) : (DateTime?)null,
                            NomeMotivoExclusao = reader["CODIGO_MOTIVO_EXCLUSAO"] != DBNull.Value ? reader["CODIGO_MOTIVO_EXCLUSAO"].ToString() : ""
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
                AND CODIGO_GRUPO_CONTRATO IN (4, 3)";

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
    }
}