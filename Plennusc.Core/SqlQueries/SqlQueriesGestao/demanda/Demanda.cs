using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.demanda
{
    public static class Demanda
    {
        public const string Departamentos = @"
            SELECT CodDepartamento AS Value, Nome AS Text
            FROM dbo.Departamento
            ORDER BY Nome";

        public const string PessoasAtivas = @"
            SELECT CodPessoa AS Value, (Nome + ' ' + Sobrenome) AS Text
            FROM dbo.Pessoa
            WHERE Conf_Ativo = 1
            ORDER BY Nome, Sobrenome";

        // Prioridades (CodTipoEstrutura = 7)
        public const string PrioridadesPorTipo = @"
            SELECT CodEstrutura AS Value, DescEstrutura AS Text
            FROM dbo.Estrutura
            WHERE CodTipoEstrutura = @Tipo
            ORDER BY ISNULL(ValorPadrao, 999), DescEstrutura";

        // Situação inicial = 'ABERTA' (CodTipoEstrutura = 5)
        public const string SituacaoAbertaPorTipo = @"
            SELECT TOP 1 e.CodEstrutura
            FROM dbo.Estrutura e
            WHERE e.CodTipoEstrutura = @Tipo
              AND UPPER(e.DescEstrutura) COLLATE Latin1_General_CI_AI = 'ABERTA'
            ORDER BY ISNULL(e.ValorPadrao, 999), e.CodEstrutura";

        // INSERT demanda
        public const string InsertDemanda = @"
            INSERT INTO dbo.Demanda
            (CodPessoaSolicitacao, CodSetorOrigem, CodSetorDestino,
             CodEstr_TipoDemanda, DataDemanda, CodEstr_SituacaoDemanda,
             CodEstr_NivelPrioridade, Titulo, TextoDemanda,
             Conf_RequerAprovacao, CodPessoaAprovacao, DataAprovacao, DataPrazoMaximo, DataFinalizacao)
            VALUES
            (@CodPessoaSolicitacao, @CodSetorOrigem, @CodSetorDestino,
             @CodEstr_TipoDemanda, GETDATE(), @CodEstr_SituacaoDemanda,
             @CodEstr_NivelPrioridade, @Titulo, @TextoDemanda,
             @Conf_RequerAprovacao, @CodPessoaAprovacao, NULL, @DataPrazoMaximo, NULL);
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

        // INSERT histórico inicial
        public const string InsertHistoricoInicial = @"
            INSERT INTO dbo.DemandaHistorico
            (CodDemanda, CodEstr_SituacaoDemandaAnterior, CodEstr_SituacaoDemandaAtual, CodPessoaAlteracao, DataAlteracao)
            VALUES
            (@CodDemanda, @CodEstr_SituacaoDemanda, @CodEstr_SituacaoDemanda, @CodPessoaAlteracao, GETDATE());";

        // Vincula tipo ao setor destino (Upsert)
        public const string UpsertSetorTipoDemanda = @"
        IF NOT EXISTS (
            SELECT 1 FROM dbo.SetorTipoDemanda
            WHERE CodSetor = @CodSetor
              AND CodEstr_TipoDemanda = @CodEstr_TipoDemanda
        )
        BEGIN
            INSERT INTO dbo.SetorTipoDemanda
                (CodSetor, CodEstr_TipoDemanda, Conf_Status, Informacoes_log_i)
            VALUES
                (@CodSetor, @CodEstr_TipoDemanda, 1, GETDATE());
        END
        ELSE
        BEGIN
            UPDATE dbo.SetorTipoDemanda
            SET Conf_Status = 1,
                Informacoes_log_a = GETDATE()
            WHERE CodSetor = @CodSetor
              AND CodEstr_TipoDemanda = @CodEstr_TipoDemanda;
        END";

        // ---- Dropdowns de Categoria/Subtipo (CodTipoEstrutura = 6) ----

        // Categorias (pais)
        public const string TiposDemandaGrupos = @"
            SELECT CodEstrutura AS Value, DescEstrutura AS Text
            FROM dbo.Estrutura
            WHERE CodTipoEstrutura = @Tipo   -- 6
              AND CodEstruturaPai IS NULL
            ORDER BY ISNULL(ValorPadrao, 999), DescEstrutura";

        // Subtipos (filhos do grupo)
        public const string SubtiposPorGrupo = @"
            SELECT CodEstrutura AS Value, DescEstrutura AS Text
            FROM dbo.Estrutura
            WHERE CodTipoEstrutura = @Tipo   -- 6
              AND CodEstruturaPai = @Pai
            ORDER BY ISNULL(ValorPadrao, 999), DescEstrutura";

        // (opcional) Tipos permitidos por setor
        public const string TiposPermitidosPorSetor = @"
            SELECT std.CodEstr_TipoDemanda
            FROM dbo.SetorTipoDemanda std
            WHERE std.Conf_Status = 1
              AND std.CodSetor = @Setor";
    }
}