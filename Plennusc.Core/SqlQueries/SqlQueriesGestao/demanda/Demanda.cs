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

        public const string PrioridadesPorTipo = @"
            SELECT CodEstrutura AS Value, DescEstrutura AS Text
            FROM dbo.Estrutura
            WHERE CodTipoEstrutura = @Tipo
            ORDER BY ISNULL(ValorPadrao, 999), DescEstrutura";

        public const string SituacaoAbertaPorTipo = @"
            SELECT TOP 1 e.CodEstrutura
            FROM dbo.Estrutura e
            WHERE e.CodTipoEstrutura = @Tipo
              AND UPPER(e.DescEstrutura) = 'ABERTA'
            ORDER BY e.CodEstrutura";

        public const string UpsertSetorTipoDemanda = @"
            MERGE dbo.SetorTipoDemanda AS tgt
            USING (SELECT @CodSetor AS CodSetor, @CodEstr_TipoDemanda AS CodEstr_TipoDemanda) AS src
            ON (tgt.CodSetor = src.CodSetor AND tgt.CodEstr_TipoDemanda = src.CodEstr_TipoDemanda)
            WHEN NOT MATCHED THEN
                INSERT (CodSetor, CodEstr_TipoDemanda) VALUES (src.CodSetor, src.CodEstr_TipoDemanda);";

        public const string InsertDemanda = @"
            INSERT INTO dbo.Demanda
              (CodPessoaSolicitacao, CodSetorOrigem, CodSetorDestino,
               CodEstr_TipoDemanda, CodEstr_SituacaoDemanda,
               CodEstr_NivelPrioridade, Titulo, TextoDemanda,
               Conf_RequerAprovacao, CodPessoaAprovacao, DataPrazoMaximo, DataDemanda)
            OUTPUT INSERTED.CodDemanda
            VALUES
              (@CodPessoaSolicitacao, @CodSetorOrigem, @CodSetorDestino,
               @CodEstr_TipoDemanda, @CodEstr_SituacaoDemanda,
               @CodEstr_NivelPrioridade, @Titulo, @TextoDemanda,
               @Conf_RequerAprovacao, @CodPessoaAprovacao, @DataPrazoMaximo, GETDATE());";

        public const string InsertHistoricoInicial = @"
            INSERT INTO dbo.DemandaHistorico
              (CodDemanda, CodEstr_SituacaoDemandaAnterior, CodEstr_SituacaoDemandaAtual,
               CodPessoaAlteracao, DataAlteracao)
            VALUES
              (@CodDemanda, @CodEstr_SituacaoDemanda, @CodEstr_SituacaoDemanda,
               @CodPessoaAlteracao, GETDATE());";

        public const string TiposDemandaGrupos = @"
            SELECT CodEstrutura AS Value, DescEstrutura AS Text
            FROM dbo.Estrutura
            WHERE CodTipoEstrutura = @Tipo AND CodEstruturaPai IS NULL
            ORDER BY DescEstrutura";

        public const string SubtiposPorGrupo = @"
            SELECT CodEstrutura AS Value, DescEstrutura AS Text
            FROM dbo.Estrutura
            WHERE CodTipoEstrutura = @Tipo AND CodEstruturaPai = @Pai
            ORDER BY DescEstrutura";

        public const string CategoriasDemanda = @"
            SELECT CodEstrutura AS Value, DescEstrutura AS Text
            FROM dbo.Estrutura
            WHERE CodTipoEstrutura = @Tipo AND CodEstruturaPai IS NULL
            ORDER BY DescEstrutura";

        public const string SubtiposDemanda = @"
            SELECT CodEstrutura AS Value, DescEstrutura AS Text
            FROM dbo.Estrutura
            WHERE CodTipoEstrutura = @Tipo AND CodEstruturaPai = @Pai
            ORDER BY DescEstrutura";

        public const string SituacoesDemanda = @"
            SELECT CodEstrutura AS Value, DescEstrutura AS Text
            FROM dbo.Estrutura
            WHERE CodTipoEstrutura = @Tipo
            ORDER BY DescEstrutura";

        public const string TiposPermitidosPorSetor = @"
            SELECT CodEstr_TipoDemanda
            FROM dbo.SetorTipoDemanda
            WHERE CodSetor = @Setor";

        public const string ListarDemandasBase = @"
            SELECT d.CodDemanda,
                   d.Titulo,
                   cat.DescEstrutura AS Categoria,
                   sub.DescEstrutura AS Subtipo,
                   sit.DescEstrutura AS Status,
                   (p.Nome + ' ' + ISNULL(p.Sobrenome,'')) AS Solicitante,
                   d.DataDemanda
            FROM dbo.Demanda d
            INNER JOIN dbo.Estrutura sit ON sit.CodEstrutura = d.CodEstr_SituacaoDemanda
            INNER JOIN dbo.Pessoa p ON p.CodPessoa = d.CodPessoaSolicitacao
            LEFT JOIN dbo.Estrutura sub ON sub.CodEstrutura = d.CodEstr_TipoDemanda
            LEFT JOIN dbo.Estrutura cat ON cat.CodEstrutura = sub.CodEstruturaPai
            WHERE 1=1";
    }
}