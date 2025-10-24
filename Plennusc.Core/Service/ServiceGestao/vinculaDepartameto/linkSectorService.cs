using Plennusc.Core.Models.ModelsGestao.modelsLinkSector;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.department;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao.vinculaDepartameto
{
    public class linkSectorService
    {
        private string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Plennus"].ConnectionString;

        private SqlConnection Open()
        {
            var con = new SqlConnection(_connectionString);
            con.Open();
            return con;
        }

        public List<TypeStructureModel> GetTodasViews()
        {
            var list = new List<TypeStructureModel>();
            using (var con = Open())
            using (var cmd = new SqlCommand(linkSetorQueries.BuscarTodasViews, con))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    list.Add(new TypeStructureModel
                    {
                        CodTipoEstrutura = reader.GetInt32(0),
                        DescTipoEstrutura = reader.GetString(1),
                        NomeView = reader.GetString(2)
                    });
                }
            }
            return list;
        }

        public List<StructureModel> GetEstruturasPorView(int codTipoEstrutura)
        {
            var list = new List<StructureModel>();
            using (var con = Open())
            using (var cmd = new SqlCommand(linkSetorQueries.BuscarEstruturasPorView, con))
            {
                cmd.Parameters.AddWithValue("@CodTipoEstrutura", codTipoEstrutura);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new StructureModel
                        {
                            CodEstrutura = reader.GetInt32(0),
                            DescEstrutura = reader.GetString(1),
                            ValorPadrao = reader.GetInt32(2),
                            CodTipoEstrutura = reader.GetInt32(3)
                        });
                    }
                }
            }
            return list;
        }

        public List<SectorTypeDemandModel> GetVinculosExistentes(int codEstrutura)
        {
            var list = new List<SectorTypeDemandModel>();
            using (var con = Open())
            using (var cmd = new SqlCommand(linkSetorQueries.BuscarVinculosExistentes, con))
            {
                cmd.Parameters.AddWithValue("@CodEstrutura", codEstrutura);

                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new SectorTypeDemandModel
                        {
                            CodSetorTipoDemanda = reader.GetInt32(0),
                            CodSetor = reader.GetInt32(1),
                            CodEstr_TipoDemanda = reader.GetInt32(2),
                            NomeSetor = reader.GetString(3)
                        });
                    }
                }
            }
            return list;
        }

        public bool VincularSetor(int codSetor, int codEstrutura)
        {
            // PRIMEIRO: Verificar se a estrutura existe MESMO
            bool estruturaExiste = VerificarEstruturaExiste(codEstrutura);
            if (!estruturaExiste)
            {
                throw new Exception($"Estrutura {codEstrutura} não existe na tabela Estrutura!");
            }

            // SEGUNDO: Verifica se já existe o vínculo
            if (VerificarVinculoExistente(codSetor, codEstrutura))
                return false;

            using (var con = Open())
            using (var cmd = new SqlCommand(linkSetorQueries.InserirVinculo, con))
            {
                cmd.Parameters.AddWithValue("@CodSetor", codSetor);
                cmd.Parameters.AddWithValue("@CodEstrutura", codEstrutura);

                try
                {
                    int affectedRows = cmd.ExecuteNonQuery();
                    return affectedRows > 0;
                }
                catch (SqlException ex)
                {
                    throw new Exception($"Erro ao inserir: Setor={codSetor}, Estrutura={codEstrutura}. Detalhes: {ex.Message}");
                }
            }
        }

        private bool VerificarEstruturaExiste(int codEstrutura)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM Estrutura 
                WHERE CodEstrutura = @CodEstrutura", con))
            {
                cmd.Parameters.AddWithValue("@CodEstrutura", codEstrutura);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        public bool DesvincularSetor(int codSetorTipoDemanda)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(linkSetorQueries.ExcluirVinculo, con))
            {
                cmd.Parameters.AddWithValue("@CodSetorTipoDemanda", codSetorTipoDemanda);
                int affectedRows = cmd.ExecuteNonQuery();
                return affectedRows > 0;
            }
        }

        private bool VerificarVinculoExistente(int codSetor, int codEstrutura)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(linkSetorQueries.VerificarVinculoExistente, con))
            {
                cmd.Parameters.AddWithValue("@CodSetor", codSetor);
                cmd.Parameters.AddWithValue("@CodEstrutura", codEstrutura);
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }
    }
}