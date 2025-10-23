using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.department
{
    public class linkSetorQueries
    {
        public const string BuscarTodasViews = @"
            SELECT CodTipoEstrutura, DescTipoEstrutura, NomeView 
            FROM TipoEstrutura 
            ORDER BY DescTipoEstrutura";

        public const string BuscarEstruturasPorView = @"
            SELECT 
                e.CodEstrutura,
                e.DescEstrutura,
                e.ValorPadrao,
                e.CodTipoEstrutura
            FROM Estrutura e
            WHERE e.CodTipoEstrutura = @CodTipoEstrutura
            ORDER BY e.ValorPadrao, e.DescEstrutura";

        public const string BuscarVinculosExistentes = @"
            SELECT 
                std.CodSetorTipoDemanda,
                std.CodSetor,
                std.CodEstr_TipoDemanda,
                d.Nome as NomeSetor
            FROM SetorTipoDemanda std
            INNER JOIN Departamento d ON std.CodSetor = d.CodDepartamento
            WHERE std.CodEstr_TipoDemanda = @CodEstrutura";

        public const string InserirVinculo = @"
            INSERT INTO SetorTipoDemanda (CodSetor, CodEstr_TipoDemanda, Conf_Status, Informacoes_log_i)
            VALUES (@CodSetor, @CodEstrutura, 1, GETDATE())";

        public const string ExcluirVinculo = @"
            DELETE FROM SetorTipoDemanda 
            WHERE CodSetorTipoDemanda = @CodSetorTipoDemanda";

        public const string VerificarVinculoExistente = @"
            SELECT COUNT(*) 
            FROM SetorTipoDemanda 
            WHERE CodSetor = @CodSetor 
            AND CodEstr_TipoDemanda = @CodEstrutura";
    }
}
