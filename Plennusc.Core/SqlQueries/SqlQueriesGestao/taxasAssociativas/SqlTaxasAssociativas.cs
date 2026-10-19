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

        // DEMAIS ENTIDADES (dropdown de múltipla seleção)

        /// <summary>
        /// Retorna as entidades (eventos) permitidas para a tela "Demais Entidades".
        /// </summary>
        public List<EntidadeTaxaAssociativaModel> BuscarEntidadesTaxaAssociativa()
        {
            var lista = new List<EntidadeTaxaAssociativaModel>();

            string sql = @"
                SELECT DISTINCT
                    CODIGO_EVENTO,
                    NOME_EVENTO
                FROM PS1024
                WHERE CODIGO_EVENTO IN (14, 18, 21, 22, 43, 44)
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

        // SINDNAPI (tela própria)

        /// <summary>
        /// Retorna as taxas associativas do SINDNAPI (CODIGO_EVENTO = 17) no intervalo
        /// de data de pagamento informado (PS1020.DATA_PAGAMENTO).
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
                    p1029.VALOR_EVENTO        AS VALOR_EVENTO
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
                                ? Convert.ToDecimal(reader["VALOR_EVENTO"]) : 0
                        });
                    }
                }
            }

            return lista;
        }

        // DEMAIS ENTIDADES (resultado)

        /// <summary>
        /// Retorna as taxas associativas das demais entidades no intervalo
        /// de data de pagamento informado.
        /// </summary>
        public List<TaxaAssociativaDemaisEntidadesModel> BuscarTaxasDemaisEntidades(
            List<int> codigosEvento, DateTime dataInicio, DateTime dataFim)
        {
            var lista = new List<TaxaAssociativaDemaisEntidadesModel>();

            if (codigosEvento == null || codigosEvento.Count == 0)
                return lista;

            var nomesParametros = codigosEvento
                .Select((codigo, indice) => $"@CodigoEvento{indice}")
                .ToList();

            string sql = $@"
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
                WHERE p1029.CODIGO_EVENTO IN ({string.Join(",", nomesParametros)})
                  AND p1029.CODIGO_EVENTO <> 17
                  AND p1020.DATA_PAGAMENTO >= @DataInicio
                  AND p1020.DATA_PAGAMENTO <  DATEADD(DAY, 1, @DataFim)
                ORDER BY p1020.DATA_PAGAMENTO, p1000.NOME_ASSOCIADO";

            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                for (int i = 0; i < codigosEvento.Count; i++)
                {
                    cmd.Parameters.AddWithValue($"@CodigoEvento{i}", codigosEvento[i]);
                }

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
                            TipoContratacaoAns = TraduzirTipoContratacaoAns(reader["TIPO_CONTRATACAO_ANS"]?.ToString()),
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

        // SINCOR (tela própria — PF e PJ)

        public List<OpcaoSincorModel> BuscarOpcoesSincor()
        {
            return new List<OpcaoSincorModel>
            {
                new OpcaoSincorModel { CodigoEvento = 13, Descricao = "SINCOR - Pessoa Física (PF)" },
                new OpcaoSincorModel { CodigoEvento = 45, Descricao = "SINCOR - Pessoa Jurídica (PJ)" }
            };
        }

        /// <summary>
        /// Retorna as taxas associativas do SINCOR (PF = 13 ou PJ = 45) no intervalo
        /// de data de pagamento informado (PS1020.DATA_PAGAMENTO).
        /// 
        /// Como os contatos estão espalhados (PS1006, PS1001, PS1015), usamos um OUTER APPLY
        /// com COALESCE para tentar pegar o dado de onde ele existir, priorizando PS1006.
        /// </summary>

        public List<TaxaAssociativaSincorModel> BuscarTaxasSincor(
         List<int> codigosEvento, DateTime dataInicio, DateTime dataFim)
            {
                var lista = new List<TaxaAssociativaSincorModel>();

                if (codigosEvento == null || codigosEvento.Count == 0)
                    return lista;

                var nomesParametros = codigosEvento
                    .Select((codigo, indice) => $"@CodigoEvento{indice}")
                    .ToList();

                string sql = $@"
            SELECT
                p1000.NUMERO_CPF          AS CPF_CNPJ,
                p1000.NOME_ASSOCIADO      AS NOME_BENEFICIARIO,
                p1000.DATA_NASCIMENTO     AS DATA_NASCIMENTO,
                p1020.DATA_PAGAMENTO      AS DATA_PAGAMENTO,
                p1029.VALOR_EVENTO        AS VALOR_EVENTO,
            
                contato_email.EMAIL       AS EMAIL,
                contato_tel.TELEFONE      AS TELEFONE
        
            FROM PS1029 p1029
            INNER JOIN PS1020 p1020 
                ON p1029.NUMERO_REGISTRO_PS1020 = p1020.NUMERO_REGISTRO
            INNER JOIN PS1000 p1000 
                ON p1020.CODIGO_ASSOCIADO = p1000.CODIGO_ASSOCIADO
        
            LEFT JOIN PS1015 p1015 
                ON p1000.CODIGO_ASSOCIADO = p1015.CODIGO_ASSOCIADO
        
            OUTER APPLY (
                SELECT TOP 1 
                    COALESCE(p1001.ENDERECO_EMAIL, p1015.ENDERECO_EMAIL) AS EMAIL
                FROM PS1001 p1001
                WHERE p1001.CODIGO_ASSOCIADO = p1000.CODIGO_ASSOCIADO 
                   OR (p1015.CODIGO_ASSOCIADO IS NOT NULL AND p1001.CODIGO_ASSOCIADO = p1015.CODIGO_ASSOCIADO)
                ORDER BY CASE WHEN p1001.CODIGO_ASSOCIADO = p1000.CODIGO_ASSOCIADO THEN 1 ELSE 2 END
            ) contato_email
        
            OUTER APPLY (
                SELECT TOP 1 
                    ISNULL(CAST(p1006.CODIGO_AREA AS VARCHAR(10)), '') + ISNULL(p1006.NUMERO_TELEFONE, '') AS TELEFONE
                FROM PS1006 p1006
                WHERE p1006.CODIGO_ASSOCIADO = p1000.CODIGO_ASSOCIADO 
                   OR (p1015.CODIGO_ASSOCIADO IS NOT NULL AND p1006.CODIGO_ASSOCIADO = p1015.CODIGO_ASSOCIADO)
                ORDER BY CASE WHEN p1006.CODIGO_ASSOCIADO = p1000.CODIGO_ASSOCIADO THEN 1 ELSE 2 END
            ) contato_tel
    
            WHERE p1029.CODIGO_EVENTO IN ({string.Join(",", nomesParametros)})
              AND p1020.DATA_PAGAMENTO >= @DataInicio
              AND p1020.DATA_PAGAMENTO <  DATEADD(DAY, 1, @DataFim)
            ORDER BY p1020.DATA_PAGAMENTO, p1000.NOME_ASSOCIADO";

            using (SqlConnection conn = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                for (int i = 0; i < codigosEvento.Count; i++)
                {
                    cmd.Parameters.AddWithValue($"@CodigoEvento{i}", codigosEvento[i]);
                }

                cmd.Parameters.AddWithValue("@DataInicio", dataInicio.Date);
                cmd.Parameters.AddWithValue("@DataFim", dataFim.Date);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new TaxaAssociativaSincorModel
                        {
                            CpfCnpj = reader["CPF_CNPJ"] as string,
                            NomeBeneficiario = reader["NOME_BENEFICIARIO"] as string,
                            Email = reader["EMAIL"] as string,
                            Telefone = reader["TELEFONE"] as string,
                            DataNascimento = reader["DATA_NASCIMENTO"] != DBNull.Value
                                ? Convert.ToDateTime(reader["DATA_NASCIMENTO"]) : (DateTime?)null,
                            DataPagamento = reader["DATA_PAGAMENTO"] != DBNull.Value
                                ? Convert.ToDateTime(reader["DATA_PAGAMENTO"]) : (DateTime?)null,
                            ValorEvento = reader["VALOR_EVENTO"] != DBNull.Value
                                ? Convert.ToDecimal(reader["VALOR_EVENTO"]) : 0,
                            TipoPessoa = codigosEvento.Contains(13) && codigosEvento.Count == 1 ? "PF" :
                                         codigosEvento.Contains(45) && codigosEvento.Count == 1 ? "PJ" : "MÚLTIPLO"
                        });
                    }
                }
            }

            return lista;
        }


        // HELPERS

        private static string TraduzirTipoContratacaoAns(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return string.Empty;

            switch (codigo.Trim())
            {
                case "3": return "Empresarial";
                case "4": return "Adesão";
                default: return null;
            }
        }
    }
}