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

        // VERIFICA SE A VIEW JÁ EXISTE
        public const string VerificarViewExiste = @"
            SELECT COUNT(*) 
            FROM INFORMATION_SCHEMA.VIEWS 
            WHERE TABLE_NAME = @NomeView";

        // INSERE NOVO TIPO ESTRUTURA
        public const string InserirTipoEstrutura = @"
            INSERT INTO TipoEstrutura (
                DescTipoEstrutura, 
                CodTipoEstruturaPai, 
                NomeView, 
                Editavel, 
                Definicao, 
                Utilizacao, 
                Informacoes_Log_I
            ) 
            VALUES (
                @DescTipoEstrutura, 
                @CodTipoEstruturaPai, 
                @NomeView, 
                @Editavel, 
                @Definicao, 
                @Utilizacao, 
                GETDATE()
            );
            SELECT SCOPE_IDENTITY();";

        // NOVAS QUERIES PARA ESTRUTURA
        public const string BuscarTodosTiposEstrutura = @"
            SELECT CodTipoEstrutura, DescTipoEstrutura, NomeView 
            FROM TipoEstrutura 
            ORDER BY DescTipoEstrutura";

        public const string BuscarEstruturasPai = @"
            SELECT CodEstrutura, DescEstrutura 
            FROM Estrutura 
            WHERE CodTipoEstrutura = @CodTipoEstrutura 
            AND CodEstruturaPai IS NULL
            ORDER BY DescEstrutura";

        public const string InserirEstrutura = @"
            INSERT INTO Estrutura (
                CodEstruturaPai, 
                CodTipoEstrutura, 
                DescEstrutura, 
                MemoEstrutura, 
                InfoEstrutura, 
                Conf_IsDefault, 
                ValorPadrao
            ) 
            VALUES (
                @CodEstruturaPai, 
                @CodTipoEstrutura, 
                @DescEstrutura, 
                @MemoEstrutura, 
                @InfoEstrutura, 
                @Conf_IsDefault, 
                @ValorPadrao
            );
            SELECT SCOPE_IDENTITY();";
    }
}