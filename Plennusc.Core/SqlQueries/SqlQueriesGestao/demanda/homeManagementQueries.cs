using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.demanda
{
    public class HomeManagementQueries
    {
        // 🔥 AGORA FILTRADO POR PESSOA LOGADA
        public const string WidgetEstatisticasHoje = @"
            -- Novas demandas HOJE do usuário logado
            SELECT COUNT(*) FROM Demanda 
            WHERE CAST(DataDemanda AS DATE) = CAST(GETDATE() AS DATE)
            AND CodPessoaSolicitacao = @CodPessoa;
            
            -- Demandas finalizadas HOJE do usuário logado  
            SELECT COUNT(*) FROM Demanda 
            WHERE CAST(DataFinalizacao AS DATE) = CAST(GETDATE() AS DATE) 
            AND CodEstr_SituacaoDemanda = 23
            AND (CodPessoaSolicitacao = @CodPessoa OR CodPessoaExecucao = @CodPessoa);
            
            -- Aprovações pendentes DO USUÁRIO LOGADO
            SELECT COUNT(*) FROM Demanda 
            WHERE CodEstr_SituacaoDemanda = 65
            AND CodPessoaAprovacao = @CodPessoa;
            
            -- Atrasos críticos DO USUÁRIO LOGADO
            SELECT COUNT(*) FROM Demanda 
            WHERE DataPrazoMaximo < GETDATE() 
            AND CodEstr_SituacaoDemanda NOT IN (22, 23)
            AND CodEstr_NivelPrioridade IN (31, 32)
            AND (CodPessoaSolicitacao = @CodPessoa OR CodPessoaExecucao = @CodPessoa)";

        public const string WidgetStatusDemandas = @"
            -- Abertas DO USUÁRIO LOGADO
            SELECT COUNT(*) FROM Demanda 
            WHERE CodEstr_SituacaoDemanda = 17
            AND (CodPessoaSolicitacao = @CodPessoa OR CodPessoaExecucao = @CodPessoa);
            
            -- Andamento DO USUÁRIO LOGADO  
            SELECT COUNT(*) FROM Demanda 
            WHERE CodEstr_SituacaoDemanda = 18
            AND (CodPessoaSolicitacao = @CodPessoa OR CodPessoaExecucao = @CodPessoa);
            
            -- Aguardando DO USUÁRIO LOGADO
            SELECT COUNT(*) FROM Demanda 
            WHERE CodEstr_SituacaoDemanda = 65
            AND (CodPessoaSolicitacao = @CodPessoa OR CodPessoaExecucao = @CodPessoa);
            
            -- Atrasadas DO USUÁRIO LOGADO
            SELECT COUNT(*) FROM Demanda 
            WHERE DataPrazoMaximo < GETDATE() 
            AND CodEstr_SituacaoDemanda NOT IN (22, 23)
            AND (CodPessoaSolicitacao = @CodPessoa OR CodPessoaExecucao = @CodPessoa)";
    }
}