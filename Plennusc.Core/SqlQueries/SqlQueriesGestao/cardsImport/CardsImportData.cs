using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.dataCardsImport
{
    public class CardsImportData
    {
        private readonly string _connectionString;

        public CardsImportData(string connectionString)
        {
            _connectionString = connectionString;
        }

        public SqlConnection AbrirConexao()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        public List<AssociadoPs1000> BuscarTodosPorCpf(SqlConnection conn, string cpf)
        {
            const string sql = @"
                SELECT 
                    CODIGO_ASSOCIADO, 
                    NOME_ASSOCIADO, 
                    DATA_ADMISSAO,
                    DATA_EXCLUSAO
                FROM PS1000 WITH (NOLOCK)
                WHERE REPLACE(REPLACE(REPLACE(NUMERO_CPF, '.', ''), '-', ''), ' ', '') = @Cpf";

            var lista = new List<AssociadoPs1000>();

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@Cpf", cpf);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new AssociadoPs1000
                        {
                            CodigoAssociado = reader["CODIGO_ASSOCIADO"].ToString(),
                            Nome = reader["NOME_ASSOCIADO"]?.ToString() ?? string.Empty,
                            DataAdmissao = reader["DATA_ADMISSAO"] != DBNull.Value
                                ? (DateTime?)Convert.ToDateTime(reader["DATA_ADMISSAO"])
                                : null,
                            DataExclusao = reader["DATA_EXCLUSAO"] != DBNull.Value
                                ? (DateTime?)Convert.ToDateTime(reader["DATA_EXCLUSAO"])
                                : null
                        });
                    }
                }
            }

            return lista;
        }

        public void AtualizarCarteirinhaPs1000(SqlConnection conn, string codigoAssociado, string numeroCarteirinha, DateTime dataEmissao)
        {
            const string sql = @"
                UPDATE PS1000
                SET NUMERO_CARTEIRINHA = @NumeroCarteirinha,
                    DATA_EMISSAO_CARTEIRINHA = @DataEmissao
                WHERE CODIGO_ASSOCIADO = @CodigoAssociado";

            try
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@NumeroCarteirinha", numeroCarteirinha);
                    cmd.Parameters.AddWithValue("@DataEmissao", dataEmissao);
                    cmd.Parameters.AddWithValue("@CodigoAssociado", codigoAssociado);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Erro no UPDATE da PS1000. Detalhe: {ex.Message}");
            }
        }

        public void AtualizarObservacaoPs1007(SqlConnection conn, string codigoAssociado, string observacao)
        {
            const string sqlCheck = "SELECT COUNT(1) FROM PS1007 WITH (NOLOCK) WHERE CODIGO_ASSOCIADO = @CodigoAssociado";
            bool existe;

            using (var cmdCheck = new SqlCommand(sqlCheck, conn))
            {
                cmdCheck.Parameters.AddWithValue("@CodigoAssociado", codigoAssociado);
                existe = Convert.ToInt32(cmdCheck.ExecuteScalar()) > 0;
            }

            string sql = existe
                ? @"UPDATE PS1007 SET OBSERVACAO_CADASTRO = @Observacao WHERE CODIGO_ASSOCIADO = @CodigoAssociado"
                : @"INSERT INTO PS1007 (CODIGO_ASSOCIADO, OBSERVACAO_CADASTRO) VALUES (@CodigoAssociado, @Observacao)";

            try
            {
                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Observacao", string.IsNullOrWhiteSpace(observacao) ? (object)DBNull.Value : observacao.Trim());
                    cmd.Parameters.AddWithValue("@CodigoAssociado", codigoAssociado);
                    cmd.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Erro ao gravar na PS1007. Detalhe: {ex.Message}");
            }
        }
    }

    public class AssociadoPs1000
    {
        public string CodigoAssociado { get; set; }
        public string Nome { get; set; }
        public DateTime? DataAdmissao { get; set; }
        public DateTime? DataExclusao { get; set; }
    }
}