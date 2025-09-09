using Microsoft.Data.Edm.Validation;
using Microsoft.Data.OData.Query.SemanticAst;
using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Models.Utils;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.demanda;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao
{
    public class DemandaService
    {
        private readonly string _cs;

        public DemandaService(string connectionStringName = "Plennus")
        {
            _cs = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        }

        private SqlConnection Open()
        {
            var c = new SqlConnection(_cs);
            c.Open();
            return c;
        }

        public List<OptionItem> GetDepartamentos()
        {
            var list = new List<OptionItem>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.Departamentos, con))
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                    list.Add(new OptionItem { Value = rd.GetInt32(0), Text = rd.GetString(1) });
            }
            return list;
        }

        public List<OptionItem> GetPessoasAtivas()
        {
            var list = new List<OptionItem>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.PessoasAtivas, con))
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                    list.Add(new OptionItem { Value = rd.GetInt32(0), Text = rd.GetString(1) });
            }
            return list;
        }

        public List<OptionItem> GetPrioridades()
        {
            var list = new List<OptionItem>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.PrioridadesPorTipo, con))
            {
                cmd.Parameters.AddWithValue("@Tipo", EstruturaTipos.NivelPrioridade); // 7
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        list.Add(new OptionItem { Value = rd.GetInt32(0), Text = rd.GetString(1) });
                }
            }
            return list;
        }

        // ---------- Categoria/Subtipo ----------
        public List<OptionItem> GetTiposDemandaGrupos()
        {
            var list = new List<OptionItem>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.TiposDemandaGrupos, con))
            {
                cmd.Parameters.AddWithValue("@Tipo", EstruturaTipos.TipoDemanda); // 6
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        list.Add(new OptionItem { Value = rd.GetInt32(0), Text = rd.GetString(1) });
                }
            }
            return list;
        }

        public List<OptionItem> GetSubtiposDemanda(int codGrupo)
        {
            var list = new List<OptionItem>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.SubtiposPorGrupo, con))
            {
                cmd.Parameters.AddWithValue("@Tipo", EstruturaTipos.TipoDemanda); // 6
                cmd.Parameters.AddWithValue("@Pai", codGrupo);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        list.Add(new OptionItem { Value = rd.GetInt32(0), Text = rd.GetString(1) });
                }
            }
            return list;
        }


        // (opcional) checar se um tipo está permitido para um setor (tabela SetorTipoDemanda)
        public bool TipoPermitidoParaSetor(int codSetor, int codEstrTipoDemanda)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.TiposPermitidosPorSetor, con))
            {
                cmd.Parameters.AddWithValue("@Setor", codSetor);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        if (rd.GetInt32(0) == codEstrTipoDemanda)
                            return true;
                    }
                }
            }
            return false;
        }

        // ---------- Situação inicial ----------
        public int ResolverSituacaoAberta()
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.SituacaoAbertaPorTipo, con))
            {
                cmd.Parameters.AddWithValue("@Tipo", EstruturaTipos.SituacaoDemanda); // 5
                var o = cmd.ExecuteScalar();
                return (o == null || o == DBNull.Value) ? 0 : Convert.ToInt32(o);
            }
        }

        // ---------- Criar Demanda ----------
        public int CriarDemanda(DemandaCreate dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var codSituacaoInicial = ResolverSituacaoAberta();
            if (codSituacaoInicial == 0)
                throw new Exception("Não foi possível localizar a situação inicial 'ABERTA'.");

            using (var con = Open())
            using (var tx = con.BeginTransaction(IsolationLevel.ReadCommitted))
            {
                try
                {
                    // Upsert no vínculo Setor x Tipo (grava o tipo que veio do form — subtipo ou grupo)
                    using (var cmdVinc = new SqlCommand(Demanda.UpsertSetorTipoDemanda, con, tx))
                    {
                        cmdVinc.Parameters.AddWithValue("@CodSetor", dto.CodSetorDestino);
                        cmdVinc.Parameters.AddWithValue("@CodEstr_TipoDemanda", dto.CodEstr_TipoDemanda);
                        cmdVinc.ExecuteNonQuery();
                    }

                    int novoId;
                    using (var cmd = new SqlCommand(Demanda.InsertDemanda, con, tx))
                    {
                        cmd.Parameters.AddWithValue("@CodPessoaSolicitacao", dto.CodPessoaSolicitacao);
                        cmd.Parameters.AddWithValue("@CodSetorOrigem", dto.CodSetorOrigem);
                        cmd.Parameters.AddWithValue("@CodSetorDestino", dto.CodSetorDestino);
                        cmd.Parameters.AddWithValue("@CodEstr_TipoDemanda", dto.CodEstr_TipoDemanda);
                        cmd.Parameters.AddWithValue("@CodEstr_SituacaoDemanda", codSituacaoInicial);
                        cmd.Parameters.AddWithValue("@CodEstr_NivelPrioridade", dto.CodEstr_NivelPrioridade);
                        cmd.Parameters.AddWithValue("@Titulo", dto.Titulo ?? "");
                        cmd.Parameters.AddWithValue("@TextoDemanda", dto.TextoDemanda ?? "");
                        cmd.Parameters.AddWithValue("@Conf_RequerAprovacao", dto.Conf_RequerAprovacao);

                        // MODIFICAÇÃO: Se não tem aprovador específico, usa NULL
                        if (dto.CodPessoaAprovacao.HasValue && dto.CodPessoaAprovacao > 0)
                        {
                            cmd.Parameters.AddWithValue("@CodPessoaAprovacao", dto.CodPessoaAprovacao.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@CodPessoaAprovacao", DBNull.Value);
                        }

                        var pPrazo = cmd.Parameters.Add("@DataPrazoMaximo", SqlDbType.DateTime);
                        pPrazo.Value = (object)dto.DataPrazoMaximo ?? DBNull.Value;

                        novoId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    using (var cmdHist = new SqlCommand(Demanda.InsertHistoricoInicial, con, tx))
                    {
                        cmdHist.Parameters.AddWithValue("@CodDemanda", novoId);
                        cmdHist.Parameters.AddWithValue("@CodEstr_SituacaoDemanda", codSituacaoInicial);
                        cmdHist.Parameters.AddWithValue("@CodPessoaAlteracao", dto.CodPessoaSolicitacao);
                        cmdHist.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return novoId;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }
        public List<OptionItem> GetSituacoesDemanda()
        {
            var list = new List<OptionItem>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.SituacoesDemanda, con))
            {
                cmd.Parameters.AddWithValue("@Tipo", EstruturaTipos.SituacaoDemanda); // 5
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        list.Add(new OptionItem { Value = rd.GetInt32(0), Text = rd.GetString(1) });
                }
            }
            return list;
        }

        public List<OptionItem> GetCategoriasDemanda()
        {
            var list = new List<OptionItem>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.CategoriasDemanda, con))
            {
                cmd.Parameters.AddWithValue("@Tipo", EstruturaTipos.TipoDemanda); // 6
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        list.Add(new OptionItem { Value = rd.GetInt32(0), Text = rd.GetString(1) });
                }
            }
            return list;
        }

        public List<DemandaListDto> ListarDemandas(DemandaFiltro filtro)
        {
            var lista = new List<DemandaListDto>();
            using (var con = Open())
            using (var cmd = new SqlCommand())
            {
                cmd.Connection = con;
                var sql = new StringBuilder(Demanda.ListarDemandasBase);

                // Visibilidade: MINHAS (M) -> solicitante/aprovador/acompanhamento
                if (!string.IsNullOrWhiteSpace(filtro?.Visibilidade))
                {
                    var v = filtro.Visibilidade.Trim().ToUpperInvariant();
                    if ((v == "M" || v == "MINHAS" || v == "P") && filtro.CodPessoa.HasValue)
                    {
                        sql.Append(" AND (d.CodPessoaSolicitacao = @CodPessoa OR d.CodPessoaAprovacao = @CodPessoa OR EXISTS(SELECT 1 FROM dbo.DemandaAcompanhamento da WHERE da.CodDemanda = d.CodDemanda AND da.CodPessoaAcompanhamento = @CodPessoa))");
                        cmd.Parameters.AddWithValue("@CodPessoa", filtro.CodPessoa.Value);
                    }
                    else if ((v == "S" || v == "SETOR") && filtro.CodSetor.HasValue)
                    {
                        sql.Append(" AND (d.CodSetorOrigem = @CodSetor OR d.CodSetorDestino = @CodSetor)");
                        cmd.Parameters.AddWithValue("@CodSetor", filtro.CodSetor.Value);
                    }
                }

                if (filtro?.CodStatus != null)
                {
                    sql.Append(" AND d.CodEstr_SituacaoDemanda = @CodStatus");
                    cmd.Parameters.AddWithValue("@CodStatus", filtro.CodStatus.Value);
                }
                if (filtro?.CodCategoria != null)
                {
                    // categoria corresponde ao pai (ou tipo quando é pai)
                    sql.Append(" AND ( (EXISTS(SELECT 1 FROM dbo.Estrutura e WHERE e.CodEstrutura = d.CodEstr_TipoDemanda AND e.CodEstruturaPai = @CodCategoria)) OR (d.CodEstr_TipoDemanda = @CodCategoria) )");
                    cmd.Parameters.AddWithValue("@CodCategoria", filtro.CodCategoria.Value);
                }
                if (filtro?.CodSubtipo != null)
                {
                    sql.Append(" AND d.CodEstr_TipoDemanda = @CodSubtipo");
                    cmd.Parameters.AddWithValue("@CodSubtipo", filtro.CodSubtipo.Value);
                }
                if (!string.IsNullOrWhiteSpace(filtro?.NomeSolicitante))
                {
                    sql.Append(" AND (p.Nome + ' ' + ISNULL(p.Sobrenome,'')) LIKE @Solicitante");
                    cmd.Parameters.AddWithValue("@Solicitante", "%" + filtro.NomeSolicitante.Trim() + "%");
                }

                sql.Append(" ORDER BY d.DataDemanda DESC");
                cmd.CommandText = sql.ToString();

                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var dto = new DemandaListDto
                        {
                            CodDemanda = rd.GetInt32(0),
                            Titulo = rd.IsDBNull(1) ? "" : rd.GetString(1),
                            Categoria = rd.IsDBNull(2) ? "" : rd.GetString(2),
                            Subtipo = rd.IsDBNull(3) ? null : rd.GetString(3),
                            Status = rd.IsDBNull(4) ? "" : rd.GetString(4),
                            Solicitante = rd.IsDBNull(5) ? "" : rd.GetString(5),
                            DataSolicitacao = rd.IsDBNull(6) ? DateTime.MinValue : rd.GetDateTime(6)
                        };
                        lista.Add(dto);
                    }
                }
            }
            return lista;
        }
        // DTOs mínimos — adicione em Models se quiser organizar melhor

        public DemandaDto ObterDemandaPorId(int codDemanda)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.ObterDemandaPorId, con))
            {
                cmd.Parameters.AddWithValue("@CodDemanda", codDemanda);
                using (var rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        var dto = new DemandaDto
                        {
                            CodDemanda = rd.GetInt32(rd.GetOrdinal("CodDemanda")),
                            Titulo = rd.IsDBNull(rd.GetOrdinal("Titulo")) ? null : rd.GetString(rd.GetOrdinal("Titulo")),
                            TextoDemanda = rd.IsDBNull(rd.GetOrdinal("TextoDemanda")) ? null : rd.GetString(rd.GetOrdinal("TextoDemanda")),
                            StatusNome = rd.IsDBNull(rd.GetOrdinal("StatusNome")) ? null : rd.GetString(rd.GetOrdinal("StatusNome")),
                            StatusCodigo = rd.IsDBNull(rd.GetOrdinal("StatusCodigo")) ? (int?)null : rd.GetInt32(rd.GetOrdinal("StatusCodigo")),
                            Solicitante = rd.IsDBNull(rd.GetOrdinal("Solicitante")) ? null : rd.GetString(rd.GetOrdinal("Solicitante")),
                            DataSolicitacao = rd.IsDBNull(rd.GetOrdinal("DataSolicitacao")) ? (DateTime?)null : rd.GetDateTime(rd.GetOrdinal("DataSolicitacao")),
                            CodPessoaSolicitacao = rd.IsDBNull(rd.GetOrdinal("CodPessoaSolicitacao")) ? 0 : rd.GetInt32(rd.GetOrdinal("CodPessoaSolicitacao")),
                            CodPessoaAprovacao = rd.IsDBNull(rd.GetOrdinal("CodPessoaAprovacao")) ? 0 : rd.GetInt32(rd.GetOrdinal("CodPessoaAprovacao"))
                        };
                        return dto;
                    }
                }
            }
            return null;
        }

        public IEnumerable<HistoricoDto> ObterHistorico(int codDemanda)
        {
            var list = new List<HistoricoDto>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.ObterHistorico, con))
            {
                cmd.Parameters.AddWithValue("@CodDemanda", codDemanda);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var anterior = rd.IsDBNull(rd.GetOrdinal("CodEstr_SituacaoDemandaAnterior")) ? (int?)null : rd.GetInt32(rd.GetOrdinal("CodEstr_SituacaoDemandaAnterior"));
                        var atual = rd.IsDBNull(rd.GetOrdinal("CodEstr_SituacaoDemandaAtual")) ? (int?)null : rd.GetInt32(rd.GetOrdinal("CodEstr_SituacaoDemandaAtual"));

                        list.Add(new HistoricoDto
                        {
                            CodDemandaHistorico = rd.IsDBNull(rd.GetOrdinal("CodDemandaHistorico")) ? 0 : rd.GetInt32(rd.GetOrdinal("CodDemandaHistorico")),
                            CodDemanda = rd.IsDBNull(rd.GetOrdinal("CodDemanda")) ? 0 : rd.GetInt32(rd.GetOrdinal("CodDemanda")),
                            CodEstr_SituacaoDemandaAnterior = anterior,
                            CodEstr_SituacaoDemandaAtual = atual,
                            SituacaoAnterior = rd.IsDBNull(rd.GetOrdinal("SituacaoAnterior")) ? null : rd.GetString(rd.GetOrdinal("SituacaoAnterior")),
                            SituacaoAtual = rd.IsDBNull(rd.GetOrdinal("SituacaoAtual")) ? null : rd.GetString(rd.GetOrdinal("SituacaoAtual")),
                            CodPessoaAlteracao = rd.IsDBNull(rd.GetOrdinal("CodPessoaAlteracao")) ? 0 : rd.GetInt32(rd.GetOrdinal("CodPessoaAlteracao")),
                            Usuario = rd.IsDBNull(rd.GetOrdinal("Usuario")) ? null : rd.GetString(rd.GetOrdinal("Usuario")),
                            DataAlteracao = rd.IsDBNull(rd.GetOrdinal("DataAlteracao")) ? (DateTime?)null : rd.GetDateTime(rd.GetOrdinal("DataAlteracao"))
                        });
                    }
                }
            }
            return list;
        }

        public IEnumerable<AcompanhamentoDto> ObterAcompanhamentos(int codDemanda)
        {
            var list = new List<AcompanhamentoDto>();
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.ObterAcompanhamentos, con))
            {
                cmd.Parameters.AddWithValue("@CodDemanda", codDemanda);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        list.Add(new AcompanhamentoDto
                        {
                            CodDemandaAcompanhamento = rd.IsDBNull(rd.GetOrdinal("CodDemandaAcompanhamento")) ? 0 : rd.GetInt32(rd.GetOrdinal("CodDemandaAcompanhamento")),
                            CodDemanda = rd.IsDBNull(rd.GetOrdinal("CodDemanda")) ? 0 : rd.GetInt32(rd.GetOrdinal("CodDemanda")),
                            TextoAcompanhamento = rd.IsDBNull(rd.GetOrdinal("TextoAcompanhamento")) ? null : rd.GetString(rd.GetOrdinal("TextoAcompanhamento")),
                            DataAcompanhamento = rd.IsDBNull(rd.GetOrdinal("DataAcompanhamento")) ? (DateTime?)null : rd.GetDateTime(rd.GetOrdinal("DataAcompanhamento")),
                            CodPessoaAcompanhamento = rd.IsDBNull(rd.GetOrdinal("CodPessoaAcompanhamento")) ? 0 : rd.GetInt32(rd.GetOrdinal("CodPessoaAcompanhamento")),
                            Autor = rd.IsDBNull(rd.GetOrdinal("Autor")) ? null : rd.GetString(rd.GetOrdinal("Autor"))
                        });
                    }
                }
            }
            return list;
        }

        public void AdicionarAcompanhamento(int codDemanda, int codPessoa, string texto)
        {
            using (var con = Open())
            using (var cmd = new SqlCommand(Demanda.InsertAcompanhamento, con))
            {
                cmd.Parameters.AddWithValue("@CodDemanda", codDemanda);
                cmd.Parameters.AddWithValue("@TextoAcompanhamento", (object)(texto ?? string.Empty));
                cmd.Parameters.AddWithValue("@CodPessoaAcompanhamento", codPessoa);
                cmd.ExecuteNonQuery();
            }
        }

        public void AtualizarStatusComHistorico(int codDemanda, int novoStatusCodigo, int codPessoaAlteracao)
        {
            using (var con = Open())
            {
                // 1. PEGA O STATUS ATUAL
                int codStatusAtual = 0;
                using (var cmdStatus = new SqlCommand("SELECT CodEstr_SituacaoDemanda FROM Demanda WHERE CodDemanda = @CodDemanda", con))
                {
                    cmdStatus.Parameters.AddWithValue("@CodDemanda", codDemanda);
                    var resultado = cmdStatus.ExecuteScalar();
                    if (resultado != null && resultado != DBNull.Value)
                        codStatusAtual = Convert.ToInt32(resultado);
                }

                // 2. ATUALIZA O STATUS
                using (var cmdUpdate = new SqlCommand("UPDATE Demanda SET CodEstr_SituacaoDemanda = @NovoStatus WHERE CodDemanda = @CodDemanda", con))
                {
                    cmdUpdate.Parameters.AddWithValue("@NovoStatus", novoStatusCodigo);
                    cmdUpdate.Parameters.AddWithValue("@CodDemanda", codDemanda);
                    cmdUpdate.ExecuteNonQuery();
                }

                // 3. REGISTRA NO HISTÓRICO
                using (var cmdHist = new SqlCommand("INSERT INTO DemandaHistorico (CodDemanda, CodEstr_SituacaoDemandaAnterior, CodEstr_SituacaoDemandaAtual, CodPessoaAlteracao, DataAlteracao) VALUES (@CodDemanda, @CodAnterior, @CodAtual, @CodPessoa, GETDATE())", con))
                {
                    cmdHist.Parameters.AddWithValue("@CodDemanda", codDemanda);
                    cmdHist.Parameters.AddWithValue("@CodAnterior", codStatusAtual);
                    cmdHist.Parameters.AddWithValue("@CodAtual", novoStatusCodigo);
                    cmdHist.Parameters.AddWithValue("@CodPessoa", codPessoaAlteracao);
                    cmdHist.ExecuteNonQuery();
                }
            }
        }
    }
}