using Plennusc.Core.Models.ModelsGestao.modelsTaxasAssociativas;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.taxasAssociativas
{
    /// <summary>
    /// Acesso a dados das Taxas Associativas (Comissões / Premiações).
    /// 
    /// Tabelas envolvidas:
    ///   PS1029 → taxas associativas (CODIGO_EVENTO, VALOR_EVENTO, VALOR_TOTAL, NUMERO_REGISTRO_PS1020)
    ///   PS1020 → boleto (NUMERO_REGISTRO, CODIGO_ASSOCIADO, DATA_PAGAMENTO)
    ///   PS1000 → beneficiário (CODIGO_ASSOCIADO, NUMERO_CPF, NOME_ASSOCIADO, DATA_NASCIMENTO, CODIGO_PLANO)
    ///   PS1024 → entidades/eventos (CODIGO_EVENTO, NOME_EVENTO)
    ///   PS1030 → planos (CODIGO_PLANO, TIPO_CONTRATACAO_ANS)
    /// </summary>
    public class SqlTaxasAssociativas
    {
        #region CONEXÃO

        private readonly string _connStr = ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;

        #endregion

        // 
        // ENTIDADES (dropdown "Demais Entidades")
        // 

        /// <summary>
        /// Retorna todas as entidades (eventos) cujo NOME_EVENTO começa com "TX ASSOC",
        /// exceto SINDNAPI (CODIGO_EVENTO = 17).
        /// Alimenta o dropdown da tela "Demais Entidades".
        /// </summary>
        public List<EntidadeTaxaAssociativaModel> BuscarEntidadesTaxaAssociativa()
        {
            var lista = new List<EntidadeTaxaAssociativaModel>();

            string sql = @"
                SELECT DISTINCT
                    CODIGO_EVENTO,
                    NOME_EVENTO
                FROM PS1024
                WHERE NOME_EVENTO LIKE 'TX ASSOC%'
                  AND CODIGO_EVENTO <> 17
                ORDER BY NOME_EVENTO";

            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new EntidadeTaxaAssociativaModel
                        {
                            CodigoEvento = reader["CODIGO_EVENTO"] != DBNull.Value
                                ? Convert.ToInt32(reader["CODIGO_EVENTO"]) : 0,
                            NomeEvento = reader["NOME_EVENTO"] != DBNull.Value
                                ? reader["NOME_EVENTO"].ToString() : ""
                        });
                    }
                }
            }

            return lista;
        }

        // 
        // SINDNAPI
        // 

        /// <summary>
        /// Retorna as taxas associativas do SINDNAPI (CODIGO_EVENTO = 17) no intervalo
        /// de data de pagamento informado (PS1020.DATA_PAGAMENTO).
        /// 
        /// Joins:
        ///   PS1029.NUMERO_REGISTRO_PS1020 = PS1020.NUMERO_REGISTRO
        ///   PS1020.CODIGO_ASSOCIADO       = PS1000.CODIGO_ASSOCIADO
        /// </summary>
        public List<TaxaAssociativaSindnapiModel> BuscarTaxasSindnapi(DateTime dataInicio, DateTime dataFim)
        {
            var lista = new List<TaxaAssociativaSindnapiModel>();

            string sql = @"
                SELECT
                    p1000.NUMERO_CPF          AS CPF,
                    p1000.NOME_ASSOCIADO      AS NOME_BENEFICIARIO,
                    p1000.DATA_NASCIMENTO     AS DATA_NASCIMENTO,
                    p1020.DATA_PAGAMENTO      AS DATA_PAGAMENTO,
                    p1029.VALOR_EVENTO        AS VALOR_EVENTO,
                    p1029.VALOR_TOTAL         AS VALOR_TOTAL
                FROM PS1029 p1029
                INNER JOIN PS1020 p1020 
                    ON p1029.NUMERO_REGISTRO_PS1020 = p1020.NUMERO_REGISTRO
                INNER JOIN PS1000 p1000 
                    ON p1020.CODIGO_ASSOCIADO = p1000.CODIGO_ASSOCIADO
                WHERE p1029.CODIGO_EVENTO = 17
                  AND p1020.DATA_PAGAMENTO >= @DataInicio
                  AND p1020.DATA_PAGAMENTO <  DATEADD(DAY, 1, @DataFim)
                ORDER BY p1020.DATA_PAGAMENTO, p1000.NOME_ASSOCIADO";

            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@DataInicio", dataInicio.Date);
                cmd.Parameters.AddWithValue("@DataFim", dataFim.Date);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new TaxaAssociativaSindnapiModel
                        {
                            Cpf = reader["CPF"] as string,
                            NomeBeneficiario = reader["NOME_BENEFICIARIO"] as string,
                            DataNascimento = reader["DATA_NASCIMENTO"] != DBNull.Value
                                ? Convert.ToDateTime(reader["DATA_NASCIMENTO"]) : (DateTime?)null,
                            DataPagamento = reader["DATA_PAGAMENTO"] != DBNull.Value
                                ? Convert.ToDateTime(reader["DATA_PAGAMENTO"]) : (DateTime?)null,
                            ValorEvento = reader["VALOR_EVENTO"] != DBNull.Value
                                ? Convert.ToDecimal(reader["VALOR_EVENTO"]) : 0,
                            ValorTotal = reader["VALOR_TOTAL"] != DBNull.Value
                                ? Convert.ToDecimal(reader["VALOR_TOTAL"]) : 0
                        });
                    }
                }
            }

            return lista;
        }

        // 
        // DEMAIS ENTIDADES
        // 

        /// <summary>
        /// Retorna as taxas associativas das demais entidades (exceto SINDNAPI) no intervalo
        /// de data de pagamento informado.
        /// 
        /// Filtro:
        ///   - PS1029.CODIGO_EVENTO = @CodigoEvento (o que o usuário escolheu no dropdown)
        ///   - PS1029.CODIGO_EVENTO <> 17 (garante exclusão do SINDNAPI)
        /// 
        /// Joins:
        ///   PS1029.NUMERO_REGISTRO_PS1020 = PS1020.NUMERO_REGISTRO
        ///   PS1020.CODIGO_ASSOCIADO       = PS1000.CODIGO_ASSOCIADO
        ///   PS1000.CODIGO_PLANO           = PS1030.CODIGO_PLANO (LEFT JOIN)
        /// </summary>
        public List<TaxaAssociativaDemaisEntidadesModel> BuscarTaxasDemaisEntidades(
            int codigoEvento, DateTime dataInicio, DateTime dataFim)
        {
            var lista = new List<TaxaAssociativaDemaisEntidadesModel>();

            string sql = @"
                SELECT
                    p1029.NUMERO_REGISTRO_PS1020    AS NUMERO_REGISTRO_PS1020,
                    p1000.NOME_ASSOCIADO            AS NOME_BENEFICIARIO,
                    p1030.TIPO_CONTRATACAO_ANS      AS TIPO_CONTRATACAO_ANS,
                    p1020.DATA_PAGAMENTO            AS DATA_PAGAMENTO,
                    p1029.VALOR_EVENTO              AS VALOR_EVENTO,
                    p1029.VALOR_TOTAL               AS VALOR_TOTAL
                FROM PS1029 p1029
                INNER JOIN PS1020 p1020 
                    ON p1029.NUMERO_REGISTRO_PS1020 = p1020.NUMERO_REGISTRO
                INNER JOIN PS1000 p1000 
                    ON p1020.CODIGO_ASSOCIADO = p1000.CODIGO_ASSOCIADO
                LEFT JOIN PS1030 p1030 
                    ON p1000.CODIGO_PLANO = p1030.CODIGO_PLANO
                WHERE p1029.CODIGO_EVENTO = @CodigoEvento
                  AND p1029.CODIGO_EVENTO <> 17
                  AND p1020.DATA_PAGAMENTO >= @DataInicio
                  AND p1020.DATA_PAGAMENTO <  DATEADD(DAY, 1, @DataFim)
                ORDER BY p1020.DATA_PAGAMENTO, p1000.NOME_ASSOCIADO";

            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@CodigoEvento", codigoEvento);
                cmd.Parameters.AddWithValue("@DataInicio", dataInicio.Date);
                cmd.Parameters.AddWithValue("@DataFim", dataFim.Date);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new TaxaAssociativaDemaisEntidadesModel
                        {
                            NumeroRegistroPs1020 = reader["NUMERO_REGISTRO_PS1020"] != DBNull.Value
                                ? Convert.ToInt32(reader["NUMERO_REGISTRO_PS1020"]) : 0,
                            NomeBeneficiario = reader["NOME_BENEFICIARIO"] as string,
                            TipoContratacaoAns = reader["TIPO_CONTRATACAO_ANS"] as string,
                            DataPagamento = reader["DATA_PAGAMENTO"] != DBNull.Value
                                ? Convert.ToDateTime(reader["DATA_PAGAMENTO"]) : (DateTime?)null,
                            ValorEvento = reader["VALOR_EVENTO"] != DBNull.Value
                                ? Convert.ToDecimal(reader["VALOR_EVENTO"]) : 0,
                            ValorTotal = reader["VALOR_TOTAL"] != DBNull.Value
                                ? Convert.ToDecimal(reader["VALOR_TOTAL"]) : 0
                        });
                    }
                }
            }

            return lista;
        }
    }
}