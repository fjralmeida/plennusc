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
        public const string ObterDemandaPorId = @"
SELECT 
    d.CodDemanda,
    d.Titulo,
    d.TextoDemanda,
    d.CodEstr_SituacaoDemanda AS StatusCodigo,
    sit.DescEstrutura AS StatusNome,
    (p.Nome + ' ' + ISNULL(p.Sobrenome,'')) AS Solicitante,
    d.DataDemanda AS DataSolicitacao,
    d.CodPessoaSolicitacao,
    d.CodPessoaAprovacao -- ESSE CAMPO É CRUCIAL
FROM dbo.Demanda d
INNER JOIN dbo.Estrutura sit ON sit.CodEstrutura = d.CodEstr_SituacaoDemanda
INNER JOIN dbo.Pessoa p ON p.CodPessoa = d.CodPessoaSolicitacao
WHERE d.CodDemanda = @CodDemanda";
        public const string ObterHistorico = @"
SELECT dh.CodDemandaHistorico,
       dh.CodDemanda,
       dh.CodEstr_SituacaoDemandaAnterior,
       dh.CodEstr_SituacaoDemandaAtual,
       prev.DescEstrutura AS SituacaoAnterior,
       curr.DescEstrutura AS SituacaoAtual,
       dh.CodPessoaAlteracao,
       (p.Nome + ' ' + ISNULL(p.Sobrenome,'')) AS Usuario,
       dh.DataAlteracao
FROM dbo.DemandaHistorico dh
LEFT JOIN dbo.Estrutura prev ON prev.CodEstrutura = dh.CodEstr_SituacaoDemandaAnterior
LEFT JOIN dbo.Estrutura curr ON curr.CodEstrutura = dh.CodEstr_SituacaoDemandaAtual
LEFT JOIN dbo.Pessoa p ON p.CodPessoa = dh.CodPessoaAlteracao
WHERE dh.CodDemanda = @CodDemanda
ORDER BY dh.DataAlteracao ASC";

        public const string ObterAcompanhamentos = @"
SELECT da.CodDemandaAcompanhamento,
       da.CodDemanda,
       da.TextoAcompanhamento,
       da.DataAcompanhamento,
       da.CodPessoaAcompanhamento,
       (p.Nome + ' ' + ISNULL(p.Sobrenome,'')) AS Autor
FROM dbo.DemandaAcompanhamento da
LEFT JOIN dbo.Pessoa p ON p.CodPessoa = da.CodPessoaAcompanhamento
WHERE da.CodDemanda = @CodDemanda
ORDER BY da.DataAcompanhamento ASC";

        public const string InsertAcompanhamento = @"
INSERT INTO dbo.DemandaAcompanhamento (CodDemanda, TextoAcompanhamento, CodPessoaAcompanhamento, DataAcompanhamento)
VALUES (@CodDemanda, @TextoAcompanhamento, @CodPessoaAcompanhamento, GETDATE());";

        public const string SelectStatusCodigo = @"
SELECT CodEstr_SituacaoDemanda FROM dbo.Demanda WHERE CodDemanda = @CodDemanda";

        public const string AtualizarStatus = @"
UPDATE d
SET d.CodEstr_SituacaoDemanda = sit.CodEstrutura
FROM dbo.Demanda d
INNER JOIN dbo.Estrutura sit ON sit.DescEstrutura = @NovoStatus
WHERE d.CodDemanda = @CodDemanda";

        public const string InsertDemandaHistorico = @"
INSERT INTO dbo.DemandaHistorico
    (CodDemanda, CodEstr_SituacaoDemandaAnterior, CodEstr_SituacaoDemandaAtual, CodPessoaAlteracao, DataAlteracao)
VALUES
    (@CodDemanda, @CodAnterior, @CodAtual, @CodPessoaAlteracao, GETDATE());";

    }
}