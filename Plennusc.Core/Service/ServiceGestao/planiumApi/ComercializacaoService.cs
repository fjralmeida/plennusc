using Plennusc.Core.Models.ModelsGestao;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace Plennusc.Core.Service.ServiceGestao.planiumApi
{
    public class ComercializacaoService
    {
        private readonly string _cs;

        public ComercializacaoService(string connectionStringName = "Plennus")
        {
            _cs = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        private SqlConnection Open()
        {
            var c = new SqlConnection(_cs);
            c.Open();
            return c;
        }

        public List<ComercializacaoListDto> ListarComercializacao(ComercializacaoFiltro filtro)
        {
            var lista = new List<ComercializacaoListDto>();

            using (var con = Open())
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = con;

                var sql = new StringBuilder(@"
                    SELECT
                        cm.CodigoComercializacaoMunicipio,
                        c.CodIBGE AS CodigoIBGE,                          
                        c.Descricao AS NomeCidade,                        
                        e.Sigla AS SiglaEstado,                           
                        p.NomePlanoComercial,
                        cm.Conf_Ativo,
                        cm.Conf_Exibir
                    FROM dbo.API_Venda_ComercializacaoMunicipio cm
                    INNER JOIN dbo.Cidade c ON c.CodigoCidade = cm.CodigoCidade
                    INNER JOIN dbo.Estado e ON e.CodigoEstado = c.CodigoEstado   
                    INNER JOIN dbo.API_Venda_Plano p ON p.CodigoPlano = cm.CodigoPlano
                    WHERE cm.Informacoes_log_e IS NULL   
                ");

                // Aplicar filtros corretamente
                if (!string.IsNullOrWhiteSpace(filtro?.SiglaEstado))
                {
                    sql.Append(" AND e.Sigla LIKE @SiglaEstado");
                    cmd.Parameters.AddWithValue("@SiglaEstado", "%" + filtro.SiglaEstado.Trim() + "%");
                }

                if (!string.IsNullOrWhiteSpace(filtro?.NomeCidade))
                {
                    sql.Append(" AND c.Descricao LIKE @NomeCidade");
                    cmd.Parameters.AddWithValue("@NomeCidade", "%" + filtro.NomeCidade.Trim() + "%");
                }

                if (!string.IsNullOrWhiteSpace(filtro?.NomePlanoComercial))
                {
                    sql.Append(" AND p.NomePlanoComercial LIKE @NomePlanoComercial");
                    cmd.Parameters.AddWithValue("@NomePlanoComercial", "%" + filtro.NomePlanoComercial.Trim() + "%");
                }

                // Ordenação final
                sql.Append(" ORDER BY cm.CodigoComercializacaoMunicipio DESC");

                cmd.CommandText = sql.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var item = new ComercializacaoListDto
                        {
                            CodigoComercializacaoMunicipio = rd.GetInt32(rd.GetOrdinal("CodigoComercializacaoMunicipio")),
                            CodigoIBGE = rd.IsDBNull(rd.GetOrdinal("CodigoIBGE")) ? 0 : rd.GetInt32(rd.GetOrdinal("CodigoIBGE")),
                            NomeCidade = rd.GetString(rd.GetOrdinal("NomeCidade")),
                            SiglaEstado = rd.GetString(rd.GetOrdinal("SiglaEstado")),
                            NomePlanoComercial = rd.GetString(rd.GetOrdinal("NomePlanoComercial")),
                            Conf_Ativo = rd.GetBoolean(rd.GetOrdinal("Conf_Ativo")),
                            Conf_Exibir = rd.GetBoolean(rd.GetOrdinal("Conf_Exibir"))
                        };

                        lista.Add(item);
                    }
                }
            }

            return lista;
        }

        // Método para inserir nova comercialização
        public int InserirComercializacao(int codigoCidade, int codigoPlano, int codigoUsuario)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = @"
                    INSERT INTO dbo.API_Venda_ComercializacaoMunicipio 
                    (CodigoCidade, CodigoPlano, Conf_Ativo, Conf_Exibir, CodPessoaCadastro, Informacoes_log_i)
                    VALUES 
                    (@CodigoCidade, @CodigoPlano, 1, 0, @CodigoUsuario, GETDATE());
                    
                    SELECT SCOPE_IDENTITY(); -- Retorna o ID gerado
                ";

                cmd.Parameters.AddWithValue("@CodigoCidade", codigoCidade);
                cmd.Parameters.AddWithValue("@CodigoPlano", codigoPlano);
                cmd.Parameters.AddWithValue("@CodigoUsuario", codigoUsuario);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        // Método para atualizar comercialização
        public bool AtualizarComercializacao(int codigoComercializacao, bool confAtivo, bool confExibir, int codigoUsuario)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = @"
                    UPDATE dbo.API_Venda_ComercializacaoMunicipio 
                    SET Conf_Ativo = @ConfAtivo,
                        Conf_Exibir = @ConfExibir,
                        CodPessoaAlteracao = @CodigoUsuario,
                        Informacoes_log_a = GETDATE()
                    WHERE CodigoComercializacaoMunicipio = @CodigoComercializacao
                ";

                cmd.Parameters.AddWithValue("@CodigoComercializacao", codigoComercializacao);
                cmd.Parameters.AddWithValue("@ConfAtivo", confAtivo);
                cmd.Parameters.AddWithValue("@ConfExibir", confExibir);
                cmd.Parameters.AddWithValue("@CodigoUsuario", codigoUsuario);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        // Método para excluir (soft delete)
        public bool ExcluirComercializacao(int codigoComercializacao, int codigoUsuario)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = con;
                cmd.CommandText = @"
                    UPDATE dbo.API_Venda_ComercializacaoMunicipio 
                    SET Informacoes_log_e = GETDATE(),
                        CodPessoaAlteracao = @CodigoUsuario,
                        Informacoes_log_a = GETDATE()
                    WHERE CodigoComercializacaoMunicipio = @CodigoComercializacao
                ";

                cmd.Parameters.AddWithValue("@CodigoComercializacao", codigoComercializacao);
                cmd.Parameters.AddWithValue("@CodigoUsuario", codigoUsuario);

                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}