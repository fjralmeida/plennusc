using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao
{
    public class NotaFiscalUtil
    {
        private string connectionString;

        public NotaFiscalUtil()
        {
            // PEGA A CONNECTION STRING DO WEB.CONFIG
            connectionString = ConfigurationManager.ConnectionStrings["Alianca"]?.ConnectionString;
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new Exception("Connection string 'Alianca' não encontrada no Web.config");
            }
        }

        public (string NumeroNF, string CodigoVerificacao) BuscarNotaFiscal(int numeroRegistro)
        {
            string numeroNF = null;
            string codigoVerificacao = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // PRIMEIRO: Busca o NUMERO_NOTA_FISCAL na tabela PS1020
                    string query = @"
                    SELECT NUMERO_NOTA_FISCAL 
                    FROM PS1020 
                    WHERE NUMERO_REGISTRO = @NumeroRegistro";

                    string numeroNotaFiscal = null;

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NumeroRegistro", numeroRegistro);
                        conn.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                numeroNotaFiscal = reader["NUMERO_NOTA_FISCAL"]?.ToString();
                            }
                        }
                    }

                    // SEGUNDO: Se encontrou o número da nota fiscal, busca na view
                    if (!string.IsNullOrEmpty(numeroNotaFiscal))
                    {
                        // Fecha a primeira conexão para abrir uma nova
                        conn.Close();

                        string queryView = @"
                        SELECT TOP 1 NUMERO_NFSE, CODIGO_VERIFICACAO 
                        FROM VW_CABECALHO_NFSE 
                        WHERE NUMERO_NOTA_FISCAL_SISTEMA = @NumeroNotaFiscal 
                        AND STATUS_NOTA_FISCAL = 'NFSE-EMITIDA'
                        ORDER BY DATA_HORA_EMISSAO DESC";

                        using (SqlCommand cmd2 = new SqlCommand(queryView, conn))
                        {
                            cmd2.Parameters.AddWithValue("@NumeroNotaFiscal", numeroNotaFiscal);
                            conn.Open();

                            using (SqlDataReader reader2 = cmd2.ExecuteReader())
                            {
                                if (reader2.Read())
                                {
                                    numeroNF = reader2["NUMERO_NFSE"]?.ToString();
                                    codigoVerificacao = reader2["CODIGO_VERIFICACAO"]?.ToString();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Logar erro se necessário
                Console.WriteLine($"Erro ao buscar nota fiscal: {ex.Message}");
            }

            return (numeroNF, codigoVerificacao);
        }

        public string GerarUrlNotaFiscal(string numeroNF, string codigoVerificacao)
        {
            if (string.IsNullOrEmpty(numeroNF) || string.IsNullOrEmpty(codigoVerificacao))
                return null;

            return $"?NF={numeroNF}&CV={codigoVerificacao}";

            //return $"https://portaldocliente.vallorbeneficios.com.br/ServidorAl2/ProcessoDinamico/notaBeloHorizonte.php?NF={numeroNF}&CV={codigoVerificacao}";
        }

        public string BuscarUrlNotaFiscal(int numeroRegistro)
        {
            var (numeroNF, codigoVerificacao) = BuscarNotaFiscal(numeroRegistro);
            return GerarUrlNotaFiscal(numeroNF, codigoVerificacao);
        }
    }
}