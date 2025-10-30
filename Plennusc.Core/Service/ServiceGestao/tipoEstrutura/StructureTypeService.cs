using Plennusc.Core.Mappers.MappersGestao;
using Plennusc.Core.Models.ModelsGestao.modelsStructure;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.estruturaTipo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.TipoEstrutura
{
    public class StructureTypeService
    {
        private readonly string _cs;

        public StructureTypeService(string connectionStringName = "Plennus")
        {
            _cs = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        public SqlConnection Open()
        {
            var c = new SqlConnection(_cs);
            c.Open();
            return c;
        }

        // MÉTODO PARA VERIFICAR SE VIEW JÁ EXISTE
        public bool ViewExiste(string nomeView)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(StructureTypeQueries.VerificarViewExiste, con))
            {
                cmd.Parameters.AddWithValue("@NomeView", nomeView);
                var resultado = cmd.ExecuteScalar();
                return resultado != null && Convert.ToInt32(resultado) > 0;
            }
        }

        // MÉTODO PARA CRIAR A VIEW AUTOMATICAMENTE
        public bool CriarView(string nomeView, int codTipoEstrutura)
        {
            try
            {
                using (var con = Open())
                {
                    // APENAS CRIA A VIEW
                    var sqlView = $@"
                        CREATE VIEW {nomeView} AS
                        SELECT 
                            e.CodEstrutura,
                            e.CodEstruturaPai,
                            e.CodTipoEstrutura,
                            e.DescEstrutura,
                            e.MemoEstrutura,
                            e.InfoEstrutura,
                            e.Conf_IsDefault,
                            e.ValorPadrao,
                            te.DescTipoEstrutura
                        FROM Estrutura e
                        INNER JOIN TipoEstrutura te ON e.CodTipoEstrutura = te.CodTipoEstrutura
                        WHERE te.CodTipoEstrutura = {codTipoEstrutura}";

                    using (var cmdView = new SqlCommand(sqlView, con))
                    {
                        cmdView.ExecuteNonQuery();
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao criar view: {ex.Message}");
                return false;
            }
        }

        public bool CriarEstruturaPai(int codTipoEstrutura, string descEstrutura)
        {
            try
            {
                using (var con = Open())
                {
                    // APENAS INSERE O REGISTRO PAI NA ESTRUTURA
                    var sqlInserirEstruturaPai = @"
                        INSERT INTO Estrutura (
                            CodEstruturaPai,
                            CodTipoEstrutura, 
                            DescEstrutura,
                            Conf_IsDefault,
                            ValorPadrao
                        ) 
                        VALUES (
                            NULL,
                            @CodTipoEstrutura,
                            @DescEstrutura,
                            1,
                            0
                        )";

                    using (var cmdInserir = new SqlCommand(sqlInserirEstruturaPai, con))
                    {
                        cmdInserir.Parameters.AddWithValue("@CodTipoEstrutura", codTipoEstrutura);
                        cmdInserir.Parameters.AddWithValue("@DescEstrutura", descEstrutura);
                        int result = cmdInserir.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao criar estrutura pai: {ex.Message}");
                return false;
            }
        }

        public int BuscarPaiPorTipoEstrutura(int codTipoEstrutura)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(@"
                SELECT TOP 1 CodEstrutura 
                FROM Estrutura 
                WHERE CodTipoEstrutura = @CodTipoEstrutura 
                AND CodEstruturaPai IS NULL", con))
            {
                cmd.Parameters.AddWithValue("@CodTipoEstrutura", codTipoEstrutura);

                var result = cmd.ExecuteScalar();
                if (result == null)
                    throw new Exception($"Não foi encontrado o pai para o TipoEstrutura {codTipoEstrutura}");

                return Convert.ToInt32(result);
            }
        }

        public int SalvarTipoEstrutura(structureTypeModel model)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(StructureTypeQueries.InserirTipoEstrutura, con))
            {
                cmd.Parameters.AddWithValue("@DescTipoEstrutura", model.DescTipoEstrutura);
                cmd.Parameters.AddWithValue("@CodTipoEstruturaPai", (object)model.CodTipoEstruturaPai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@NomeView", model.NomeView ?? "");
                cmd.Parameters.AddWithValue("@Definicao", model.Definicao ?? "");
                cmd.Parameters.AddWithValue("@Utilizacao", model.Utilizacao ?? "");

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }



        // No StructureTypeService.cs - método já existente
        public List<structureTypeModel> GetTipoEstrutura(string filtro = "")
        {
            var lista = new List<structureTypeModel>();

            using (var con = Open())
            {
                var query = @"SELECT CodTipoEstrutura, DescTipoEstrutura, CodTipoEstruturaPai, 
                             NomeView, Editavel, Definicao, Utilizacao, 
                             Informacoes_Log_I, Informacoes_Log_A 
                      FROM TipoEstrutura 
                      WHERE 1=1";

                if (!string.IsNullOrEmpty(filtro))
                {
                    query += " AND (DescTipoEstrutura LIKE @filtro OR NomeView LIKE @filtro OR Definicao LIKE @filtro)";
                }

                query += " ORDER BY CodTipoEstrutura";

                using (var cmd = new SqlCommand(query, con))
                {
                    if (!string.IsNullOrEmpty(filtro))
                    {
                        cmd.Parameters.AddWithValue("@filtro", $"%{filtro}%");
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new structureTypeModel
                            {
                                CodTipoEstrutura = Convert.ToInt32(reader["CodTipoEstrutura"]),
                                DescTipoEstrutura = reader["DescTipoEstrutura"].ToString(),
                                CodTipoEstruturaPai = reader["CodTipoEstruturaPai"] == DBNull.Value ? null : (int?)Convert.ToInt32(reader["CodTipoEstruturaPai"]),
                                NomeView = reader["NomeView"].ToString(),
                                Editavel = Convert.ToBoolean(reader["Editavel"]),
                                Definicao = reader["Definicao"].ToString(),
                                Utilizacao = reader["Utilizacao"].ToString(),
                                Informacoes_Log_I = Convert.ToDateTime(reader["Informacoes_Log_I"]),
                                Informacoes_Log_A = reader["Informacoes_Log_A"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["Informacoes_Log_A"])
                            });
                        }
                    }
                }
            }

            return lista;
        }

        // Método para atualizar tipo estrutura
        public bool AtualizarTipoEstrutura(structureTypeModel model)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(@"
        UPDATE TipoEstrutura 
        SET DescTipoEstrutura = @DescTipoEstrutura,
            CodTipoEstruturaPai = @CodTipoEstruturaPai,
            Editavel = @Editavel,
            Definicao = @Definicao,
            Utilizacao = @Utilizacao,
            Informacoes_Log_A = GETDATE()
        WHERE CodTipoEstrutura = @CodTipoEstrutura", con))
            {
                cmd.Parameters.AddWithValue("@CodTipoEstrutura", model.CodTipoEstrutura);
                cmd.Parameters.AddWithValue("@DescTipoEstrutura", model.DescTipoEstrutura);
                cmd.Parameters.AddWithValue("@CodTipoEstruturaPai", (object)model.CodTipoEstruturaPai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Editavel", model.Editavel);
                cmd.Parameters.AddWithValue("@Definicao", model.Definicao ?? "");
                cmd.Parameters.AddWithValue("@Utilizacao", model.Utilizacao ?? "");

                int affectedRows = cmd.ExecuteNonQuery();
                return affectedRows > 0;
            }
        }

        // Verifica se existem estruturas vinculadas a este tipo
        public bool VerificarEstruturasVinculadas(int codTipoEstrutura)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(@"
        SELECT COUNT(*) 
        FROM Estrutura 
        WHERE CodTipoEstrutura = @CodTipoEstrutura", con))
            {
                cmd.Parameters.AddWithValue("@CodTipoEstrutura", codTipoEstrutura);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        // Verifica se existem tipos filhos (sub-tipos)
        public bool VerificarTiposFilhos(int codTipoEstrutura)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(@"
        SELECT COUNT(*) 
        FROM TipoEstrutura 
        WHERE CodTipoEstruturaPai = @CodTipoEstrutura", con))
            {
                cmd.Parameters.AddWithValue("@CodTipoEstrutura", codTipoEstrutura);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        // Exclui a view do banco
        public bool ExcluirView(string nomeView)
        {
            try
            {
                using (var con = Open())
                using (var cmd = new SqlCommand($@"DROP VIEW IF EXISTS {nomeView}", con))
                {
                    cmd.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao excluir view {nomeView}: {ex.Message}");
                return false;
            }
        }

        // Exclui o tipo de estrutura
        public bool ExcluirTipoEstrutura(int codTipoEstrutura)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(@"
        DELETE FROM TipoEstrutura 
        WHERE CodTipoEstrutura = @CodTipoEstrutura", con))
            {
                cmd.Parameters.AddWithValue("@CodTipoEstrutura", codTipoEstrutura);
                int affectedRows = cmd.ExecuteNonQuery();
                return affectedRows > 0;
            }
        }


        // MÉTODO PARA BUSCAR TODOS OS TIPOS ESTRUTURA PARA O COMBO
        public List<structureTypeModel> GetTodosTiposEstrutura()
        {
            var lista = new List<structureTypeModel>();

            using (var con = Open())
            using (var cmd = new SqlCommand(StructureTypeQueries.BuscarTodosTiposEstrutura, con))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    lista.Add(new structureTypeModel
                    {
                        CodTipoEstrutura = Convert.ToInt32(reader["CodTipoEstrutura"]),
                        DescTipoEstrutura = reader["DescTipoEstrutura"].ToString(),
                        NomeView = reader["NomeView"].ToString()
                    });
                }
            }

            return lista;
        }

        // MÉTODO PARA BUSCAR TODAS AS ESTRUTURAS DE UM TIPO (PAIS E FILHOS)
        public List<structureModel> GetTodasEstruturasPorTipo(int codTipoEstrutura)
        {
            var lista = new List<structureModel>();

            using (var con = Open())
            using (var cmd = new SqlCommand(StructureTypeQueries.BuscarTodasEstruturasPorTipo, con))
            {
                cmd.Parameters.AddWithValue("@CodTipoEstrutura", codTipoEstrutura);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new structureModel
                        {
                            CodEstrutura = Convert.ToInt32(reader["CodEstrutura"]),
                            DescEstrutura = reader["DescEstrutura"].ToString(),
                            CodEstruturaPai = reader["CodEstruturaPai"] != DBNull.Value ?
                                             Convert.ToInt32(reader["CodEstruturaPai"]) : (int?)null,
                            Conf_IsDefault = Convert.ToBoolean(reader["Conf_IsDefault"]),
                            ValorPadrao = Convert.ToInt32(reader["ValorPadrao"])
                        });
                    }
                }
            }

            return lista;
        }

        // MÉTODO PARA BUSCAR ESTRUTURAS PAI DE UM TIPO ESPECÍFICO
        public List<structureModel> GetEstruturasPai(int codTipoEstrutura)
        {
            var lista = new List<structureModel>();

            using (var con = Open())
            using (var cmd = new SqlCommand(StructureTypeQueries.BuscarEstruturasPai, con))
            {
                cmd.Parameters.AddWithValue("@CodTipoEstrutura", codTipoEstrutura);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new structureModel
                        {
                            CodEstrutura = Convert.ToInt32(reader["CodEstrutura"]),
                            DescEstrutura = reader["DescEstrutura"].ToString()
                        });
                    }
                }
            }

            return lista;
        }

        // MÉTODO PARA SALVAR ESTRUTURA (PAI OU SUBTIPO)
        public int SalvarEstrutura(structureModel model)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(StructureTypeQueries.InserirEstrutura, con))
            {
                cmd.Parameters.AddWithValue("@CodEstruturaPai", (object)model.CodEstruturaPai ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CodTipoEstrutura", model.CodTipoEstrutura);
                cmd.Parameters.AddWithValue("@DescEstrutura", model.DescEstrutura);
                cmd.Parameters.AddWithValue("@MemoEstrutura", model.MemoEstrutura ?? "");
                cmd.Parameters.AddWithValue("@InfoEstrutura", model.InfoEstrutura ?? "");
                cmd.Parameters.AddWithValue("@Conf_IsDefault", model.Conf_IsDefault);
                cmd.Parameters.AddWithValue("@ValorPadrao", model.ValorPadrao);

                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        //VERIFICAR ESTRUTURAS FILHAS
        public bool VerificarEstruturasFilhas(int codEstruturaPai)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(StructureTypeQueries.VerificarEstruturasFilhas, con))
            {
                cmd.Parameters.AddWithValue("@CodEstruturaPai", codEstruturaPai);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        //EXCLUIR ESTRUTURA
        public bool ExcluirEstrutura(int codEstrutura)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(StructureTypeQueries.ExcluirEstrutura, con))
            {
                cmd.Parameters.AddWithValue("@CodEstrutura", codEstrutura);
                int affectedRows = cmd.ExecuteNonQuery();
                return affectedRows > 0;
            }
        }

        public int InserirEstruturaPai(string nomeEstrutura, string descricao)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(StructureTypeQueries.InserirEstruturaPai, con))
            {
                cmd.Parameters.AddWithValue("@DescEstrutura", nomeEstrutura);
                cmd.Parameters.AddWithValue("@MemoEstrutura", descricao ?? "");
                return Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void InserirSubtipo(int codEstruturaPai, string nomeSubtipo)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(StructureTypeQueries.InserirSubtipo, con))
            {
                cmd.Parameters.AddWithValue("@CodEstruturaPai", codEstruturaPai);
                cmd.Parameters.AddWithValue("@DescEstrutura", nomeSubtipo);
                cmd.Parameters.AddWithValue("@MemoEstrutura", "");
                cmd.ExecuteNonQuery();
            }
        }

        public void VincularSetorEstrutura(int codSetor, int codEstrutura)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(StructureTypeQueries.VincularSetorEstrutura, con))
            {
                cmd.Parameters.AddWithValue("@CodSetor", codSetor);
                cmd.Parameters.AddWithValue("@CodEstr_TipoDemanda", codEstrutura);
                cmd.ExecuteNonQuery();
            }
        }
    }
}