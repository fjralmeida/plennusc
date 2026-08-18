using Plennusc.Core.Models.ModelsGestao.modelsCIDs;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.dataCIDs
{
    /// <summary>
    /// Acesso a dados (PS1000 / PS1009) para a importação de CID.
    /// </summary>
    public class CIDsData
    {
        private readonly string _connectionString;

        public CIDsData(string connectionString)
        {
            _connectionString = connectionString;
        }

        /// <summary>
        /// Busca CODIGO_ASSOCIADO e DATA_ADMISSAO na PS1000 a partir do CPF.
        /// </summary>
        public CIDsAssociadoModel BuscarAssociadoPorCpf(SqlConnection conn, string cpf)
        {
            const string sql = @"
                SELECT CODIGO_ASSOCIADO, DATA_ADMISSAO
                FROM PS1000
                WHERE CPF = @cpf";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@cpf", cpf);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new CIDsAssociadoModel
                        {
                            CodigoAssociado = reader["CODIGO_ASSOCIADO"].ToString(),
                            DataAdmissao = reader["DATA_ADMISSAO"] != DBNull.Value
                                ? Convert.ToDateTime(reader["DATA_ADMISSAO"])
                                : (DateTime?)null
                        };
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Verifica se já existe um registro para o associado + CID na PS1009.
        /// </summary>
        public bool JaExisteRegistro(SqlConnection conn, string codigoAssociado, string cid)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM PS1009
                WHERE CODIGO_ASSOCIADO = @codigoAssociado
                  AND CODIGO_CID = @cid";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@codigoAssociado", codigoAssociado);
                cmd.Parameters.AddWithValue("@cid", cid);

                var count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        /// <summary>
        /// Insere o registro de CID na PS1009.
        /// </summary>
        public void InserirRegistro(SqlConnection conn, CIDsRegistroInsertModel registro)
        {
            const string sql = @"
                INSERT INTO PS1009
                    (CODIGO_ASSOCIADO, CODIGO_CID, DATA_TERMINO, REFERENCIA_IMPORTACAO,
                     INFORMACOES_LOG_I, INFORMACOES_LOG_A, ID_INSTANCIA_PROCESSO)
                VALUES
                    (@codigoAssociado, @cid, @dataTermino, @referenciaImportacao,
                     @logI, @logA, @idInstancia)";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@codigoAssociado", registro.CodigoAssociado);
                cmd.Parameters.AddWithValue("@cid", registro.CodigoCid);
                cmd.Parameters.AddWithValue("@dataTermino", (object)registro.DataTermino ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@referenciaImportacao", (object)registro.ReferenciaImportacao ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@logI", (object)registro.InformacoesLogI ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@logA", (object)registro.InformacoesLogA ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@idInstancia", (object)registro.IdInstanciaProcesso ?? DBNull.Value);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Cria e abre uma conexão usando a connection string configurada.
        /// </summary>
        public SqlConnection AbrirConexao()
        {
            var conn = new SqlConnection(_connectionString);
            conn.Open();
            return conn;
        }
    }
}