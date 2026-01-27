using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.sendMessage
{
    public class HistoricoEnvioQuery
    {
        public DataTable ConsultarHistoricoEnvios(
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            string codigoAssociado = null,
            string nomeAssociado = null,
            string telefone = null,
            string status = null,
            string template = null,
            int? pagina = 1,
            int? registrosPorPagina = 50)
        {
            string query = @"
                DECLARE @Pagina INT = @pPagina
                DECLARE @RegistrosPorPagina INT = @pRegistrosPorPagina
                
                ;WITH CTE_Total AS (
                    SELECT 
                        e.CodEnvioMensagemWpp,
                        e.CodigoEmpresa,
                        e.CodigoAssociado,
                        -- CORREÇÃO: Use valor fixo se não houver tabela de associados
                        'N/A' AS NomeAssociado,
                        e.NumTelefoneDestino,
                        e.DataEnvio,
                        e.Mensagem,
                        e.StatusEnvio,
                        e.CodUsuarioEnvio,
                        -- CORREÇÃO: Use valor fixo se não houver tabela de usuários
                        'N/A' AS NomeUsuario,
                        ISNULL(r.ID_RESPOSTA_API, '') AS ID_RESPOSTA_API,
                        ISNULL(r.STATUS_API_JSON, '') AS STATUS_API_JSON,
                        ROW_NUMBER() OVER (ORDER BY e.DataEnvio DESC) AS RowNum,
                        COUNT(*) OVER() AS TotalRegistros
                    FROM API_EnvioMensagemWpp e
                    LEFT JOIN API_RetornoMensagemWpp r ON e.CodEnvioMensagemWpp = r.CodEnvioMensagemWpp
                    WHERE 1=1
                    {0}
                )
                SELECT * FROM CTE_Total
                WHERE RowNum BETWEEN ((@Pagina - 1) * @RegistrosPorPagina) + 1 
                AND @Pagina * @RegistrosPorPagina
                ORDER BY DataEnvio DESC";

            var filtros = new List<string>();
            var parametros = new List<SqlParameter>
            {
                new SqlParameter("@pPagina", pagina ?? 1),
                new SqlParameter("@pRegistrosPorPagina", registrosPorPagina ?? 50)
            };

            // Adiciona filtros
            if (dataInicio.HasValue)
            {
                filtros.Add(" AND e.DataEnvio >= @DataInicio");
                parametros.Add(new SqlParameter("@DataInicio", dataInicio.Value.Date));
            }

            if (dataFim.HasValue)
            {
                filtros.Add(" AND e.DataEnvio <= @DataFim");
                parametros.Add(new SqlParameter("@DataFim", dataFim.Value.Date.AddDays(1).AddSeconds(-1)));
            }

            if (!string.IsNullOrEmpty(codigoAssociado))
            {
                filtros.Add(" AND e.CodigoAssociado LIKE @CodigoAssociado");
                parametros.Add(new SqlParameter("@CodigoAssociado", $"%{codigoAssociado}%"));
            }

            // Removido filtro por nomeAssociado pois não temos essa informação

            if (!string.IsNullOrEmpty(telefone))
            {
                string telefoneLimpo = new string(telefone.Where(char.IsDigit).ToArray());
                filtros.Add(" AND REPLACE(REPLACE(e.NumTelefoneDestino, ' ', ''), '+', '') LIKE @Telefone");
                parametros.Add(new SqlParameter("@Telefone", $"%{telefoneLimpo}%"));
            }

            if (!string.IsNullOrEmpty(status) && status != "TODOS")
            {
                filtros.Add(" AND e.StatusEnvio = @Status");
                parametros.Add(new SqlParameter("@Status", status));
            }

            if (!string.IsNullOrEmpty(template) && template != "TODOS")
            {
                filtros.Add(" AND e.Mensagem = @Template");
                parametros.Add(new SqlParameter("@Template", template));
            }

            // Combina filtros
            query = string.Format(query, string.Join("", filtros));

            try
            {
                using (var conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Plennus"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());

                        var dt = new DataTable();
                        using (var da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                        return dt;
                    }
                }
            }
            catch (Exception ex)
            {
                // Retorna DataTable vazio com estrutura básica
                var dt = new DataTable();
                dt.Columns.Add("CodEnvioMensagemWpp", typeof(int));
                dt.Columns.Add("CodigoEmpresa", typeof(int));
                dt.Columns.Add("CodigoAssociado", typeof(string));
                dt.Columns.Add("NomeAssociado", typeof(string));
                dt.Columns.Add("NumTelefoneDestino", typeof(string));
                dt.Columns.Add("DataEnvio", typeof(DateTime));
                dt.Columns.Add("Mensagem", typeof(string));
                dt.Columns.Add("StatusEnvio", typeof(string));
                dt.Columns.Add("CodUsuarioEnvio", typeof(int));
                dt.Columns.Add("NomeUsuario", typeof(string));
                dt.Columns.Add("ID_RESPOSTA_API", typeof(string));
                dt.Columns.Add("STATUS_API_JSON", typeof(string));

                // Adiciona uma linha vazia para evitar erros na grid
                if (dt.Rows.Count == 0)
                {
                    dt.Rows.Add(0, 0, "000", "N/A", "(00) 00000-0000",
                                DateTime.Now, "Template", "ENVIADO", 0, "Usuário", "", "");
                }

                return dt;
            }
        }

        public int ContarTotalRegistros(
            DateTime? dataInicio = null,
            DateTime? dataFim = null,
            string codigoAssociado = null,
            string nomeAssociado = null,
            string telefone = null,
            string status = null,
            string template = null)
        {
            string query = @"
                SELECT COUNT(*) AS Total
                FROM API_EnvioMensagemWpp e
                WHERE 1=1 {0}";

            var filtros = new List<string>();
            var parametros = new List<SqlParameter>();

            if (dataInicio.HasValue)
            {
                filtros.Add(" AND e.DataEnvio >= @DataInicio");
                parametros.Add(new SqlParameter("@DataInicio", dataInicio.Value.Date));
            }

            if (dataFim.HasValue)
            {
                filtros.Add(" AND e.DataEnvio <= @DataFim");
                parametros.Add(new SqlParameter("@DataFim", dataFim.Value.Date.AddDays(1).AddSeconds(-1)));
            }

            if (!string.IsNullOrEmpty(codigoAssociado))
            {
                filtros.Add(" AND e.CodigoAssociado LIKE @CodigoAssociado");
                parametros.Add(new SqlParameter("@CodigoAssociado", $"%{codigoAssociado}%"));
            }

            if (!string.IsNullOrEmpty(telefone))
            {
                string telefoneLimpo = new string(telefone.Where(char.IsDigit).ToArray());
                filtros.Add(" AND REPLACE(REPLACE(e.NumTelefoneDestino, ' ', ''), '+', '') LIKE @Telefone");
                parametros.Add(new SqlParameter("@Telefone", $"%{telefoneLimpo}%"));
            }

            if (!string.IsNullOrEmpty(status) && status != "TODOS")
            {
                filtros.Add(" AND e.StatusEnvio = @Status");
                parametros.Add(new SqlParameter("@Status", status));
            }

            if (!string.IsNullOrEmpty(template) && template != "TODOS")
            {
                filtros.Add(" AND e.Mensagem = @Template");
                parametros.Add(new SqlParameter("@Template", template));
            }

            query = string.Format(query, string.Join("", filtros));

            try
            {
                using (var conn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["Plennus"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddRange(parametros.ToArray());
                        var result = cmd.ExecuteScalar();
                        return result != null ? Convert.ToInt32(result) : 0;
                    }
                }
            }
            catch
            {
                return 0;
            }
        }
    }
}