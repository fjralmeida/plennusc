using Plennusc.Core.Models.ModelsGestao.modelsSendMessage;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao
{
    public class GrupoFamiliarQuery
    {
        // ==========================================
        // 1. BUSCAR DADOS DO ALIANÇA
        // ==========================================
        public List<GrupoFamiliarEnvioModel> BuscarBeneficiarios(FiltroEnvioModel filtro, string tabelaBase)
        {
            var lista = new List<GrupoFamiliarEnvioModel>();

            string query = @"
                SELECT 
                    A.CODIGO_ASSOCIADO,
                    CAST(A.CODIGO_EMPRESA AS VARCHAR) AS CodigoEmpresa,
                    ISNULL(A.NOME_SOCIAL, A.NOME_ASSOCIADO) AS NomeTitular, 
                    A.NUMERO_CPF AS CpfCnpj,
                    C.NOME_PLANO_ABREVIADO AS GrupoContrato,
                    CAST(A.CODIGO_EMPRESA AS VARCHAR) AS GrupoFaturamento,
                    E.NOME_GRUPO_PESSOAS AS NomeGrupoPessoas,
                    B.NUMERO_REGISTRO AS NumeroRegistroBoleto,
                    TRY_CAST(B.DATA_VENCIMENTO AS DATETIME) AS DataVencimento,
                    B.VALOR_FATURA AS ValorBoleto,
                    '' AS Email,
                    '' AS Telefone
                FROM PS1000 A
                INNER JOIN PS1020 B ON A.CODIGO_ASSOCIADO = B.CODIGO_ASSOCIADO
                INNER JOIN PS1030 C ON A.CODIGO_PLANO = C.CODIGO_PLANO
                LEFT JOIN PS1014 E ON A.CODIGO_GRUPO_PESSOAS = E.CODIGO_GRUPO_PESSOAS
                WHERE 1=1
                    -- REGRA DO GRUPO FAMILIAR
                    AND A.CODIGO_EMPRESA = 400
                    AND B.DATA_PAGAMENTO IS NULL
                    
                    AND (@TipoContratacaoAns IS NULL OR C.TIPO_CONTRATACAO_ANS = @TipoContratacaoAns)
                    AND (@Operadora IS NULL OR A.CODIGO_GRUPO_CONTRATO = @Operadora)
                    AND (@GrupoPessoas IS NULL OR A.CODIGO_GRUPO_PESSOAS = @GrupoPessoas)";

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TipoContratacaoAns", string.IsNullOrEmpty(filtro.TipoPlano) ? (object)DBNull.Value : filtro.TipoPlano);
                    cmd.Parameters.AddWithValue("@Operadora", string.IsNullOrEmpty(filtro.Operadora) ? (object)DBNull.Value : filtro.Operadora);
                    cmd.Parameters.AddWithValue("@GrupoPessoas", string.IsNullOrEmpty(filtro.GrupoPessoas) ? (object)DBNull.Value : filtro.GrupoPessoas);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new GrupoFamiliarEnvioModel
                            {
                                CodigoAssociado = reader["CODIGO_ASSOCIADO"].ToString(),
                                CodigoEmpresa = reader["CodigoEmpresa"].ToString(),
                                NomeTitular = reader["NomeTitular"].ToString(),
                                CpfCnpj = reader["CpfCnpj"].ToString(),
                                GrupoContrato = reader["GrupoContrato"].ToString(),
                                GrupoFaturamento = reader["GrupoFaturamento"].ToString(),
                                NomeGrupoPessoas = reader["NomeGrupoPessoas"] != DBNull.Value ? reader["NomeGrupoPessoas"].ToString() : "N/A",
                                NumeroRegistroBoleto = Convert.ToInt32(reader["NumeroRegistroBoleto"]),
                                DataVencimento = reader["DataVencimento"] != DBNull.Value ? Convert.ToDateTime(reader["DataVencimento"]) : DateTime.MinValue,
                                ValorBoleto = Convert.ToDecimal(reader["ValorBoleto"]),
                                Email = reader["Email"].ToString(),
                                Telefone = reader["Telefone"].ToString()
                            });
                        }
                    }
                }
            }
            return lista;
        }

        // ==========================================
        // 2. BUSCAR ENVIOS NO PLENNUS (CORRIGIDO)
        // ==========================================
        public List<string> ObterCodigosEnviadosNoPeriodo(DateTime? inicio, DateTime? fim, string mensagem)
        {
            var lista = new List<string>();

            // Removido o ISNULL para não forçar conversão de texto para data
            string query = @"
                SELECT DISTINCT CodigoAssociado 
                FROM API_EnvioMensagemWpp 
                WHERE (@Inicio IS NULL OR DataEnvio >= @Inicio) 
                  AND (@Fim IS NULL OR DataEnvio <= @Fim) 
                  AND Mensagem = @Mensagem 
                  AND StatusEnvio = 'ENVIADO'";

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Plennus"].ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                {
                    // Enviando como SqlDbType.DateTime para evitar o erro de smalldatetime
                    var pInicio = cmd.Parameters.Add("@Inicio", SqlDbType.DateTime);
                    pInicio.Value = inicio.HasValue ? (object)inicio.Value : DBNull.Value;

                    var pFim = cmd.Parameters.Add("@Fim", SqlDbType.DateTime);
                    pFim.Value = fim.HasValue ? (object)fim.Value : DBNull.Value;

                    cmd.Parameters.AddWithValue("@Mensagem", mensagem ?? (object)DBNull.Value);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(reader["CodigoAssociado"].ToString());
                        }
                    }
                }
            }
            return lista;
        }

        // ==========================================
        // 3. MÉTODOS AUXILIARES (DROPDOWNS)
        // ==========================================
        public DataTable ObterOperadoras()
        {
            string query = "SELECT CODIGO_GRUPO_CONTRATO, DESCRICAO_GRUPO_CONTRATO FROM ESP0002 WHERE NUMERO_ANS_OPERADORA IS NOT NULL ORDER BY DESCRICAO_GRUPO_CONTRATO";
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                {
                    var dt = new DataTable();
                    using (var da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                    return dt;
                }
            }
        }

        public DataTable ObterTodasEntidades()
        {
            string query = "SELECT CODIGO_GRUPO_PESSOAS, NOME_GRUPO_PESSOAS FROM PS1014 WHERE NOME_GRUPO_PESSOAS IS NOT NULL ORDER BY NOME_GRUPO_PESSOAS";
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                {
                    var dt = new DataTable();
                    using (var da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                    return dt;
                }
            }
        }

        public DataTable ObterGruposFaturamento()
        {
            string query = "SELECT CODIGO_GRUPO_FATURAMENTO, DESCRICAO_GP_FATURAMENTO FROM PS1051 WHERE DESCRICAO_GP_FATURAMENTO IS NOT NULL ORDER BY DESCRICAO_GP_FATURAMENTO";
            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                {
                    var dt = new DataTable();
                    using (var da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
                    return dt;
                }
            }
        }
    }
}