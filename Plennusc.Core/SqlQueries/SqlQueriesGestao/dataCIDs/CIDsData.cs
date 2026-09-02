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
                WHERE NUMERO_CPF = @cpf";

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
                cmd.Parameters.AddWithValue("@codigoAssociado", Truncar(registro.CodigoAssociado, 15));
                cmd.Parameters.AddWithValue("@cid", Truncar(registro.CodigoCid, 10));
                cmd.Parameters.AddWithValue("@dataTermino", (object)registro.DataTermino ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@referenciaImportacao", Truncar(registro.ReferenciaImportacao, 50));
                cmd.Parameters.AddWithValue("@logI", Truncar(registro.InformacoesLogI, 30));
                cmd.Parameters.AddWithValue("@logA", Truncar(registro.InformacoesLogA, 93));
                cmd.Parameters.AddWithValue("@idInstancia", Truncar(registro.IdInstanciaProcesso, 10));

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Verifica se o código do CID existe na tabela de domínio PS5201.
        /// </summary>
        public bool CidExisteNoDominio(SqlConnection conn, string codigoCid)
        {
            const string sql = @"
        SELECT COUNT(1)
        FROM PS5201
        WHERE CODIGO_CID = @cid";

            using (var cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@cid", codigoCid);
                var count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private object Truncar(string valor, int tamanhoMaximo)
        {
            if (string.IsNullOrEmpty(valor))
                return DBNull.Value;

            return valor.Length > tamanhoMaximo ? valor.Substring(0, tamanhoMaximo) : valor;
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