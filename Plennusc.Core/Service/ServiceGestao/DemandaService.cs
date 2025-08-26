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
                        cmdVinc.Parameters.AddWithValue("@CodEstr_TipoDemanda", dto.CodEstr_TipoDemanda); // << corrigido
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
                        cmd.Parameters.AddWithValue("@CodPessoaAprovacao",
                            dto.CodPessoaAprovacao > 0 ? dto.CodPessoaAprovacao : dto.CodPessoaSolicitacao);

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
    }
}