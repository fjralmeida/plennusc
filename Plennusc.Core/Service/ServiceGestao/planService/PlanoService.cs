using Dapper;
using Plennusc.Core.Models.ModelsGestao.modelsPlan;
using Plennusc.Core.SqlQueries.SqlQueriesGestao.registrationPlan;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Plennusc.Core.Service.ServiceGestao.planService
{
    public class PlanoService
    {
        private readonly string _connPlennus;
        private readonly string _connAlianca;

        public PlanoService(string connPlennus, string connAlianca)
        {
            _connPlennus = connPlennus;
            _connAlianca = connAlianca;
        }

        // ============================================================
        //  LISTAR PLANOS IMPORTADOS (grid principal)
        // ============================================================
        public List<PlanoModel> ListarPlanos(PlanoFiltro filtro)
        {
            using (var conn = new SqlConnection(_connPlennus))
            {
                return conn.Query<PlanoModel>(
                    PlanoQueries.ListarPlanos,
                    new
                    {
                        NomePlanoComercial = filtro.NomePlanoComercial,
                        Segmentacao = filtro.Segmentacao,
                        Abrangencia = filtro.Abrangencia,
                        Coparticipacao = filtro.Coparticipacao
                    }
                ).ToList();
            }
        }

        // ============================================================
        //  LISTAR PENDENTES (modal)
        // ============================================================
        public List<PlanoPendenteAliancaModel> ListarPlanosPendentesAlianca()
        {
            // Busca todos os liberados no Aliança
            List<PlanoPendenteAliancaModel> planosAlianca;
            using (var conn = new SqlConnection(_connAlianca))
            {
                planosAlianca = conn.Query<PlanoPendenteAliancaModel>(
                    PlanoQueries.ListarPlanosAliancaLiberados
                ).ToList();
            }

            if (planosAlianca.Count == 0)
                return new List<PlanoPendenteAliancaModel>();

            // Busca códigos já existentes no Plennus
            HashSet<int> codigosExistentes;
            using (var conn = new SqlConnection(_connPlennus))
            {
                var existentes = conn.Query<int>(PlanoQueries.ListarCodigosPlanosExistentes).ToList();
                codigosExistentes = new HashSet<int>(existentes);
            }

            // Retorna apenas os que NÃO existem no Plennus
            return planosAlianca.Where(p => !codigosExistentes.Contains(p.CodigoPlano)).ToList();
        }

        public int ContarPlanosPendentesAlianca()
        {
            return ListarPlanosPendentesAlianca().Count;
        }

        public int ImportarPlanos(List<PlanoPendenteAliancaModel> selecionados, int codPessoaCadastro)
        {
            if (selecionados == null || selecionados.Count == 0)
                return 0;

            int importados = 0;
            using (var conn = new SqlConnection(_connPlennus))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    foreach (var p in selecionados)
                    {
                        var result = conn.Execute(
                            PlanoQueries.InserirPlano,
                            new
                            {
                                CodigoPlano = p.CodigoPlano,
                                RegistroANS = p.RegistroANS,
                                Num_CNPJ_Operadora = p.CnpjOperadora,
                                TipoContratacao = p.TipoContratacaoDescricao,
                                Nome = p.NomePlanoFamiliar,
                                NomePlanoComercial = p.NomePlanoFamiliar,
                                Segmentacao = p.Segmentacao,
                                Abrangencia = p.CodigoAbrangencia?.ToString(),
                                Coparticipacao = p.Coparticipacao,
                                Acomodacao = p.AcomodacaoDescricao,
                                DecSau = p.DecSau,
                                Promocional = p.Promocional,
                                Conf_Ativo = p.Conf_Ativo,
                                CodPessoaCadastro = codPessoaCadastro,
                                CodPessoaAlteracao = codPessoaCadastro
                            },
                            transaction: trans
                        );
                        if (result > 0) importados++;
                    }
                    trans.Commit();
                }
            }
            return importados;
        }
    }
}