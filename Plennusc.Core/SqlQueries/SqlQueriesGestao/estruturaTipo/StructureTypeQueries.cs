using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.estruturaTipo
{
    public static class StructureTypeQueries
    {
        // INSERE A ESTRUTURA PAI (CATEGORIA) - CodTipoEstrutura = 6, CodEstruturaPai = NULL
        public const string InserirEstruturaPai = @"
            INSERT INTO dbo.Estrutura (CodEstruturaPai, CodTipoEstrutura, DescEstrutura, MemoEstrutura, Conf_IsDefault, ValorPadrao)
            VALUES (NULL, 6, @DescEstrutura, @MemoEstrutura, 0, 0);
            SELECT SCOPE_IDENTITY();";

        // INSERE OS SUBTIPOS (SERVIÇOS) - CodTipoEstrutura = 6, CodEstruturaPai = ID da categoria
        public const string InserirSubtipo = @"
            INSERT INTO dbo.Estrutura (CodEstruturaPai, CodTipoEstrutura, DescEstrutura, MemoEstrutura, Conf_IsDefault, ValorPadrao)
            VALUES (@CodEstruturaPai, 6, @DescEstrutura, @MemoEstrutura, 0, 0)";

        // VINCULA A CATEGORIA AO SETOR
        public const string VincularSetorEstrutura = @"
            INSERT INTO dbo.SetorTipoDemanda (CodSetor, CodEstr_TipoDemanda, Conf_Status, Informacoes_log_i)
            VALUES (@CodSetor, @CodEstr_TipoDemanda, 1, GETDATE())";
    }
}