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
        // QUERY PRINCIPAL DA GRID
        public List<GrupoFamiliarEnvioModel> BuscarBeneficiarios(FiltroEnvioModel filtro, string tabelaBase)
        {
            var lista = new List<GrupoFamiliarEnvioModel>();

            string query = @"
                SELECT 
                    A.CODIGO_ASSOCIADO,
                    A.NOME_ASSOCIADO AS NomeTitular,
                    A.CPF_CNPJ AS CpfCnpj,
                    A.NOME_PLANO_ABREVIADO AS GrupoContrato,
                    A.CODIGO_GRUPO_FATURAMENTO AS GrupoFaturamento,
                    E.NOME_GRUPO_PESSOAS AS NomeGrupoPessoas,
                    B.NUMERO_REGISTRO AS NumeroRegistroBoleto,
                    B.DATA_VENCIMENTO AS DataVencimento,
                    B.VALOR_FATURA AS ValorBoleto,
                    A.EMAIL AS Email,
                    A.NUMERO_TELEFONE AS Telefone
                FROM PS1000 A
                INNER JOIN PS1020 B ON A.CODIGO_ASSOCIADO = B.CODIGO_ASSOCIADO
                LEFT JOIN PS1030 C ON A.CODIGO_ASSOCIADO = C.CODIGO_ASSOCIADO 
                LEFT JOIN PS1014 E ON A.CODIGO_GRUPO_PESSOAS = E.CODIGO_GRUPO_PESSOAS
                WHERE 1=1
                    AND A.CODIGO_EMPRESA = 400
                    AND (@TipoContratacaoAns IS NULL OR C.TIPO_CONTRATACAO_ANS = @TipoContratacaoAns)
                    AND (@Operadora IS NULL OR A.CODIGO_GRUPO_CONTRATO = @Operadora)
                    AND (@GrupoPessoas IS NULL OR A.CODIGO_GRUPO_PESSOAS = @GrupoPessoas)
                    AND (@GrupoFaturamento IS NULL OR A.CODIGO_GRUPO_FATURAMENTO = @GrupoFaturamento)
                    AND B.DATA_VENCIMENTO BETWEEN @VencimentoInicio AND @VencimentoFim
                    AND B.DATA_PAGAMENTO IS NULL
                    AND A.CODIGO_ASSOCIADO NOT IN (
                        SELECT CodigoAssociado FROM Plennus.dbo.API_EnvioMensagemWpp 
                        WHERE DataEnvio BETWEEN @EnvioInicio AND @EnvioFim AND Mensagem = @Mensagem AND StatusEnvio = 'ENVIADO'
                    )
                    AND (@Mensagem != 'MSG_1' OR (B.DATA_VENCIMENTO >= CAST(GETDATE() AS DATE)))
                    AND (@Mensagem NOT IN ('MSG_2', 'MSG_4') OR (B.DATA_VENCIMENTO < CAST(GETDATE() AS DATE) AND (SELECT COUNT(1) FROM PS1020 B2 WHERE B2.CODIGO_ASSOCIADO = A.CODIGO_ASSOCIADO AND B2.DATA_PAGAMENTO IS NULL AND B2.DATA_VENCIMENTO < CAST(GETDATE() AS DATE)) = 1))
                    AND (@Mensagem != 'MSG_5' OR (B.DATA_VENCIMENTO < CAST(GETDATE() AS DATE) AND (SELECT COUNT(1) FROM PS1020 B2 WHERE B2.CODIGO_ASSOCIADO = A.CODIGO_ASSOCIADO AND B2.DATA_PAGAMENTO IS NULL AND B2.DATA_VENCIMENTO < CAST(GETDATE() AS DATE)) >= 2))";

            using (var conn = new SqlConnection(ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TipoContratacaoAns", string.IsNullOrEmpty(filtro.TipoPlano) ? (object)DBNull.Value : filtro.TipoPlano);
                    cmd.Parameters.AddWithValue("@Operadora", string.IsNullOrEmpty(filtro.Operadora) ? (object)DBNull.Value : filtro.Operadora);
                    cmd.Parameters.AddWithValue("@GrupoPessoas", string.IsNullOrEmpty(filtro.GrupoPessoas) ? (object)DBNull.Value : filtro.GrupoPessoas);
                    cmd.Parameters.AddWithValue("@GrupoFaturamento", string.IsNullOrEmpty(filtro.GrupoFaturamento) ? (object)DBNull.Value : filtro.GrupoFaturamento);
                    cmd.Parameters.AddWithValue("@VencimentoInicio", filtro.VencimentoInicio ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@VencimentoFim", filtro.VencimentoFim ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EnvioInicio", filtro.EnvioInicio ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EnvioFim", filtro.EnvioFim ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Mensagem", filtro.Mensagem);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new GrupoFamiliarEnvioModel
                            {
                                CodigoAssociado = reader["CODIGO_ASSOCIADO"].ToString(),
                                NomeTitular = reader["NomeTitular"].ToString(),
                                CpfCnpj = reader["CpfCnpj"].ToString(),
                                GrupoContrato = reader["GrupoContrato"].ToString(),
                                GrupoFaturamento = reader["GrupoFaturamento"].ToString(),
                                NomeGrupoPessoas = reader["NomeGrupoPessoas"].ToString(),
                                NumeroRegistroBoleto = Convert.ToInt32(reader["NumeroRegistroBoleto"]),
                                DataVencimento = Convert.ToDateTime(reader["DataVencimento"]),
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

        // QUERIES PARA POPULAR OS FILTROS
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
            // Puxa todas as entidades da PS1014 para popular o Dropdown com filtro
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
    }
}