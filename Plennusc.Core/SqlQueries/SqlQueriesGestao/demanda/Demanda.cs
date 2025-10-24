﻿using System;
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

        public const string DepartamentoUsuario = @"
            SELECT d.CodDepartamento AS Value, d.Nome AS Text
            FROM dbo.Departamento d
            INNER JOIN dbo.Pessoa p ON d.CodDepartamento = p.CodDepartamento
            WHERE p.CodPessoa = @UsuarioId AND p.Conf_Ativo = 1
            ORDER BY d.Nome";

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
               Conf_RequerAprovacao, CodPessoaAprovacao, DataPrazoMaximo, DataDemanda, CodEstr_NivelImportancia)
            OUTPUT INSERTED.CodDemanda
            VALUES
              (@CodPessoaSolicitacao, @CodSetorOrigem, @CodSetorDestino,
               @CodEstr_TipoDemanda, @CodEstr_SituacaoDemanda,
               @CodEstr_NivelPrioridade, @Titulo, @TextoDemanda,
               @Conf_RequerAprovacao, @CodPessoaAprovacao, @DataPrazoMaximo, GETDATE(), @CodEstr_NivelImportancia);";

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

        public const string PrioridadesDemanda = @"
            SELECT CodEstrutura AS Value, DescEstrutura AS Text
            FROM dbo.Estrutura
            WHERE CodTipoEstrutura = @Tipo
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
            SELECT 
                d.CodDemanda,
                d.Titulo,
                cat.DescEstrutura AS Categoria,
                sub.DescEstrutura AS Subtipo,
                s.DescEstrutura AS Status,
                p.Nome + ' ' + ISNULL(p.Sobrenome, '') AS Solicitante,
                d.DataDemanda AS DataSolicitacao,
                d.DataPrazoMaximo AS DataPrazo,
                pri.DescEstrutura AS Prioridade,
                d.CodEstr_NivelPrioridade AS CodPrioridade,
                imp.DescEstrutura AS Importancia,
                d.CodEstr_NivelImportancia AS CodImportancia,
                d.CodPessoaExecucao,
                d.DataAceitacao,
                pexec.Nome + ' ' + ISNULL(pexec.Sobrenome, '') AS NomePessoaExecucao,
                d.CodPessoaSolicitacao
            FROM dbo.Demanda d
            INNER JOIN dbo.Pessoa p ON d.CodPessoaSolicitacao = p.CodPessoa
            INNER JOIN dbo.Estrutura s ON d.CodEstr_SituacaoDemanda = s.CodEstrutura
            INNER JOIN dbo.Estrutura cat ON d.CodEstr_TipoDemanda = cat.CodEstrutura
            LEFT JOIN dbo.Estrutura sub ON d.CodEstr_TipoDemanda = sub.CodEstrutura
            LEFT JOIN dbo.Estrutura pri ON d.CodEstr_NivelPrioridade = pri.CodEstrutura
            LEFT JOIN dbo.Estrutura imp ON d.CodEstr_NivelImportancia = imp.CodEstrutura
            LEFT JOIN dbo.Pessoa pexec ON d.CodPessoaExecucao = pexec.CodPessoa
            WHERE 1=1
        ";

        public const string ObterDemandaDetalhesPorIdQuery = @"
            SELECT 
                d.CodDemanda,
                d.Titulo,
                d.TextoDemanda,
                d.CodEstr_SituacaoDemanda AS StatusCodigo,
                sit.DescEstrutura AS StatusNome,
                (p.Nome + ' ' + ISNULL(p.Sobrenome,'')) AS Solicitante,
                d.DataDemanda AS DataSolicitacao,
                d.CodPessoaSolicitacao,
                d.CodPessoaExecucao,
                d.DataAceitacao,
                pexec.Nome + ' ' + ISNULL(pexec.Sobrenome,'') AS NomePessoaExecucao,
                d.CodPessoaAprovacao,
                d.CodSetorDestino,
                tipo.DescEstrutura AS Categoria,
                prioridade.DescEstrutura AS Prioridade,
                importancia.DescEstrutura AS Importancia,
                CONVERT(VARCHAR, d.DataPrazoMaximo, 103) + ' ' + CONVERT(VARCHAR, d.DataPrazoMaximo, 108) AS DataPrazo
            FROM dbo.Demanda d
            INNER JOIN dbo.Estrutura sit ON sit.CodEstrutura = d.CodEstr_SituacaoDemanda
            INNER JOIN dbo.Pessoa p ON p.CodPessoa = d.CodPessoaSolicitacao
            LEFT JOIN dbo.Pessoa pexec ON pexec.CodPessoa = d.CodPessoaExecucao
            LEFT JOIN dbo.Estrutura tipo ON tipo.CodEstrutura = d.CodEstr_TipoDemanda
            LEFT JOIN dbo.Estrutura prioridade ON prioridade.CodEstrutura = d.CodEstr_NivelPrioridade
            LEFT JOIN dbo.Estrutura importancia ON importancia.CodEstrutura = d.CodEstr_NivelImportancia
            WHERE d.CodDemanda = @CodDemanda
        ";

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
                d.CodPessoaExecucao,
                d.DataAceitacao,
                pexec.Nome + ' ' + ISNULL(pexec.Sobrenome,'') AS NomePessoaExecucao,
                d.CodPessoaAprovacao,
                d.CodSetorDestino
            FROM dbo.Demanda d
            INNER JOIN dbo.Estrutura sit ON sit.CodEstrutura = d.CodEstr_SituacaoDemanda
            INNER JOIN dbo.Pessoa p ON p.CodPessoa = d.CodPessoaSolicitacao
            LEFT JOIN dbo.Pessoa pexec ON pexec.CodPessoa = d.CodPessoaExecucao
            WHERE d.CodDemanda = @CodDemanda
        ";

        //public const string ObterDemandaPorId = @"
        //    SELECT 
        //        d.CodDemanda,
        //        d.Titulo,
        //        d.TextoDemanda,
        //        d.CodEstr_SituacaoDemanda AS StatusCodigo,
        //        sit.DescEstrutura AS StatusNome,
        //        (p.Nome + ' ' + ISNULL(p.Sobrenome,'')) AS Solicitante,
        //        d.DataDemanda AS DataSolicitacao,
        //        d.CodPessoaSolicitacao,
        //        d.CodSetorDestino
        //    FROM dbo.Demanda d
        //    INNER JOIN dbo.Estrutura sit ON sit.CodEstrutura = d.CodEstr_SituacaoDemanda
        //    INNER JOIN dbo.Pessoa p ON p.CodPessoa = d.CodPessoaSolicitacao
        //    WHERE d.CodDemanda = @CodDemanda";

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

        public const string SelectDemandaExecutorStatus = @"
            SELECT CodPessoaExecucao, CodEstr_SituacaoDemanda
            FROM dbo.Demanda
            WHERE CodDemanda = @CodDemanda;";

        public const string SelectDemandaStatus = @"
            SELECT CodEstr_SituacaoDemanda 
            FROM dbo.Demanda 
            WHERE CodDemanda = @CodDemanda";

        public const string GetStatusDemanda = @"
            SELECT CodEstrutura AS Value, DescEstrutura AS Text
            FROM dbo.Estrutura
            WHERE CodTipoEstrutura = @TipoStatus
            ORDER BY DescEstrutura";

        public const string UpdateSituacaoDemanda = @"
            UPDATE dbo.Demanda
            SET CodEstr_SituacaoDemanda = @NovoStatus
            WHERE CodDemanda = @CodDemanda;";

        public const string InsertDemandaHistorico = @"
            INSERT INTO dbo.DemandaHistorico
            (CodDemanda, CodEstr_SituacaoDemandaAnterior, CodEstr_SituacaoDemandaAtual, CodPessoaAlteracao, DataAlteracao)
            VALUES (@CodDemanda, @CodEstr_SituacaoDemandaAnterior, @CodEstr_SituacaoDemandaAtual, @CodPessoaAlteracao, GETDATE());";

        public const string SelectStatusCodigo = @"
            SELECT CodEstr_SituacaoDemanda FROM dbo.Demanda WHERE CodDemanda = @CodDemanda";

        public const string AtualizarStatus = @"
            UPDATE d
            SET d.CodEstr_SituacaoDemanda = sit.CodEstrutura
            FROM dbo.Demanda d
            INNER JOIN dbo.Estrutura sit ON sit.DescEstrutura = @NovoStatus
            WHERE d.CodDemanda = @CodDemanda";

        public const string ContarDemandasPorPrioridade = @"
            SELECT COUNT(*) 
            FROM dbo.Demanda 
            WHERE CodPessoaSolicitacao = @CodPessoa 
            AND CodEstr_NivelPrioridade = @Prioridade 
            AND DataDemanda >= DATEADD(DAY, -30, GETDATE())";

        public const string PrioridadesComLimites = @"
            SELECT 
                e.CodEstrutura AS Value, 
                e.DescEstrutura AS Text,
                CASE 
                    WHEN e.CodEstrutura = 33 THEN 1 
                    WHEN e.CodEstrutura = 32 THEN 2 
                    ELSE 999 
                END AS Limite
            FROM dbo.Estrutura e
            WHERE e.CodTipoEstrutura = @Tipo
            ORDER BY ISNULL(e.ValorPadrao, 999), e.DescEstrutura";

        public const string DemandasCriticasAbertas = @"
    SELECT 
        d.CodDemanda,
        d.Titulo,
        d.DataDemanda,
        s.DescEstrutura AS Situacao,
        p.DescEstrutura AS Prioridade
    FROM dbo.Demanda d
    INNER JOIN dbo.Estrutura s ON d.CodEstr_SituacaoDemanda = s.CodEstrutura
    INNER JOIN dbo.Estrutura p ON d.CodEstr_NivelPrioridade = p.CodEstrutura
    WHERE d.CodPessoaSolicitacao = @CodPessoa
    AND d.CodEstr_NivelPrioridade IN (32, 33) 
    AND d.CodEstr_SituacaoDemanda NOT IN (22, 65)  
    ORDER BY d.DataDemanda DESC";


        public const string SituacoesParaFechamento = @"
            SELECT CodEstrutura AS Value, DescEstrutura AS Text
            FROM dbo.Estrutura 
            WHERE CodTipoEstrutura = @TipoSituacao
            AND CodEstrutura NOT IN (17, 18, 23)
            ORDER BY DescEstrutura";

        public const string AlterarPrioridadeDemanda = @"
            UPDATE Demanda 
            SET CodEstr_NivelPrioridade = @NovaPrioridade 
            WHERE CodDemanda = @CodDemanda";

        public const string SalvarAnexoDemanda = @"
            INSERT INTO DemandaAnexos 
            (CodDemanda, DescArquivo, DataEnvio, Informacoes_log_i)
            VALUES 
            (@CodDemanda, @DescArquivo, GETDATE(), GETDATE())";

        public const string GetAnexosDemanda = @"
            SELECT 
                da.CodDemandaAnexo,
                da.DescArquivo,
                da.DataEnvio,
                da.CodDemandaAcompanhamento,
                p.Nome as NomeUsuario,
                COALESCE(
                    (SELECT TOP 1 dh.CodPessoaAlteracao 
                     FROM DemandaHistorico dh 
                     WHERE dh.CodDemanda = da.CodDemanda 
                     AND dh.DataAlteracao <= da.DataEnvio 
                     ORDER BY dh.DataAlteracao DESC),
                    da.CodDemandaAcompanhamento
                ) as CodPessoaUpload
            FROM DemandaAnexos da
            LEFT JOIN DemandaAcompanhamento dac ON da.CodDemandaAcompanhamento = dac.CodDemandaAcompanhamento
            LEFT JOIN Pessoa p ON p.CodPessoa = COALESCE(dac.CodPessoaAcompanhamento, 
                (SELECT TOP 1 dh.CodPessoaAlteracao 
                 FROM DemandaHistorico dh 
                 WHERE dh.CodDemanda = da.CodDemanda 
                 AND dh.DataAlteracao <= da.DataEnvio 
                 ORDER BY dh.DataAlteracao DESC))
            WHERE da.CodDemanda = @CodDemanda
            ORDER BY da.DataEnvio DESC";

        public const string AceitarDemanda = @"
        UPDATE Demanda 
        SET CodPessoaExecucao = @CodPessoaExecucao, 
            DataAceitacao = GETDATE() 
        WHERE CodDemanda = @CodDemanda";

        public const string GetDemandasParaListagem = @"
        SELECT 
            d.CodDemanda,
            d.Titulo,
            tg.Descricao as Categoria,
            td.Descricao as Subtipo,
            d.CodEstr_NivelPrioridade as CodPrioridade,
            np.Descricao as Prioridade,
            sd.Descricao as Status,
            p.Nome + ' ' + p.Sobrenome as Solicitante,
            d.DataDemanda as DataSolicitacao,
            d.CodPessoaExecucao,
            d.DataAceitacao,
            pexec.Nome + ' ' + pexec.Sobrenome as NomePessoaExecucao
        FROM Demanda d
        INNER JOIN Estr_TipoDemandaGrupo tg ON d.CodEstr_TipoDemanda = tg.CodEstr_TipoDemandaGrupo
        INNER JOIN Estr_TipoDemandaDetalhe td ON d.CodEstr_TipoDemanda = td.CodEstr_TipoDemandaDetalhe
        INNER JOIN Estr_NivelPrioridade np ON d.CodEstr_NivelPrioridade = np.CodEstr_NivelPrioridade
        INNER JOIN Estr_SituacaoDemanda sd ON d.CodEstr_SituacaoDemanda = sd.CodEstr_SituacaoDemanda
        INNER JOIN Pessoa p ON d.CodPessoaSolicitacao = p.CodPessoa
        LEFT JOIN Pessoa pexec ON d.CodPessoaExecucao = pexec.CodPessoa
        WHERE (@CodPessoaFiltro IS NULL OR d.CodPessoaExecucao = @CodPessoaFiltro)
        ORDER BY d.DataDemanda DESC";
        public const string DemandasEmAbertoPorPessoa = @"
SELECT 
    d.CodDemanda,
    d.Titulo,
    cat.DescEstrutura AS Categoria,
    s.DescEstrutura AS Status,
    p.Nome + ' ' + ISNULL(p.Sobrenome, '') AS Solicitante,
    d.DataDemanda AS DataSolicitacao,
    d.DataPrazoMaximo AS DataPrazo,
    pri.DescEstrutura AS Prioridade,
    d.CodEstr_NivelPrioridade AS CodPrioridade,
    imp.DescEstrutura AS Importancia,
    d.CodEstr_NivelImportancia AS CodImportancia,
    d.CodPessoaExecucao,
    d.DataAceitacao,
    pexec.Nome + ' ' + ISNULL(pexec.Sobrenome, '') AS NomePessoaExecucao
FROM dbo.Demanda d
INNER JOIN dbo.Pessoa p ON d.CodPessoaSolicitacao = p.CodPessoa
INNER JOIN dbo.Estrutura s ON d.CodEstr_SituacaoDemanda = s.CodEstrutura
INNER JOIN dbo.Estrutura cat ON d.CodEstr_TipoDemanda = cat.CodEstrutura
LEFT JOIN dbo.Estrutura pri ON d.CodEstr_NivelPrioridade = pri.CodEstrutura
LEFT JOIN dbo.Estrutura imp ON d.CodEstr_NivelImportancia = imp.CodEstrutura
LEFT JOIN dbo.Pessoa pexec ON d.CodPessoaExecucao = pexec.CodPessoa
WHERE d.CodEstr_SituacaoDemanda = 17  -- Apenas status Aberta
  AND (
      -- Demandas que você ACEITOU (é o executor)
      d.CodPessoaExecucao = @CodPessoa 
      -- OU demandas que você SOLICITOU (é o solicitante)
      OR d.CodPessoaSolicitacao = @CodPessoa
      -- OU demandas sem executor atribuído
      OR d.CodPessoaExecucao IS NULL
  )
ORDER BY 
    d.CodEstr_NivelPrioridade DESC,
    CASE WHEN d.DataPrazoMaximo IS NULL THEN 1 ELSE 0 END,
    d.DataPrazoMaximo ASC,
    d.CodEstr_NivelImportancia DESC,
    d.DataDemanda DESC";

        public const string DemandasEmAndamentoPorPessoa = @"
SELECT 
    d.CodDemanda,
    d.Titulo,
    cat.DescEstrutura AS Categoria,
    s.DescEstrutura AS Status,
    p.Nome + ' ' + ISNULL(p.Sobrenome, '') AS Solicitante,
    d.DataDemanda AS DataSolicitacao,
    d.DataPrazoMaximo AS DataPrazo, 
    pri.DescEstrutura AS Prioridade,
    d.CodEstr_NivelPrioridade AS CodPrioridade,
    imp.DescEstrutura AS Importancia,
    d.CodEstr_NivelImportancia AS CodImportancia,
    d.CodPessoaExecucao,
    d.DataAceitacao,
    pexec.Nome + ' ' + ISNULL(pexec.Sobrenome, '') AS NomePessoaExecucao,
    d.CodPessoaAprovacao,
    d.DataAprovacao,
    paprov.Nome + ' ' + ISNULL(paprov.Sobrenome, '') AS NomePessoaAprovacao,
    -- Novo campo para identificar o papel do usuário
    CASE 
        WHEN d.CodPessoaSolicitacao = @CodPessoa THEN 'Solicitante'
        WHEN d.CodPessoaExecucao = @CodPessoa THEN 'Executor' 
        ELSE 'Outro'
    END AS PapelUsuario
FROM dbo.Demanda d
INNER JOIN dbo.Pessoa p ON d.CodPessoaSolicitacao = p.CodPessoa
INNER JOIN dbo.Estrutura s ON d.CodEstr_SituacaoDemanda = s.CodEstrutura
INNER JOIN dbo.Estrutura cat ON d.CodEstr_TipoDemanda = cat.CodEstrutura
LEFT JOIN dbo.Estrutura pri ON d.CodEstr_NivelPrioridade = pri.CodEstrutura
LEFT JOIN dbo.Estrutura imp ON d.CodEstr_NivelImportancia = imp.CodEstrutura
LEFT JOIN dbo.Pessoa pexec ON d.CodPessoaExecucao = pexec.CodPessoa
LEFT JOIN dbo.Pessoa paprov ON d.CodPessoaAprovacao = paprov.CodPessoa
WHERE d.CodEstr_SituacaoDemanda = 18  -- Apenas status Em Andamento
  AND (d.CodPessoaExecucao = @CodPessoa OR d.CodPessoaSolicitacao = @CodPessoa)
ORDER BY 
    d.CodEstr_NivelPrioridade DESC,
    CASE WHEN d.DataPrazoMaximo IS NULL THEN 1 ELSE 0 END,
    d.DataPrazoMaximo ASC,
    d.CodEstr_NivelImportancia DESC,
    d.DataDemanda DESC";
        // busca o CODSETOR (destino) e status atual da demanda
        public const string SelectDemanda_Setor_Status = @"
            SELECT CodSetorDestino, CodEstr_SituacaoDemanda, CodPessoaAprovacao
            FROM dbo.Demanda
            WHERE CodDemanda = @CodDemanda;";

        // busca um gestor ativo do departamento (setor)
        public const string SelectGestorPorDepartamento = @"
            SELECT TOP 1 p.CodPessoa
            FROM dbo.Pessoa p
            INNER JOIN dbo.Cargo c ON p.CodCargo = c.CodCargo
            WHERE p.CodDepartamento = @CodDepartamento
              AND c.Conf_TipoGestor = 1
              AND p.Conf_Ativo = 1
            ORDER BY p.CodPessoa; -- ordenação simples, escolha a regra que preferir";

        // atualiza o aprovador da demanda
        public const string UpdateDemanda_SetAprovador = @"
            UPDATE dbo.Demanda
            SET CodPessoaAprovacao = @CodPessoaAprovacao
            WHERE CodDemanda = @CodDemanda;";

        public const string SelectCodSituacaoAguardando = @"
            SELECT TOP 1 CodEstrutura
            FROM dbo.Estrutura
            WHERE LOWER(DescEstrutura) = 'aguardando aprovação'
               OR (LOWER(DescEstrutura) LIKE '%aguardando%' AND LOWER(DescEstrutura) LIKE '%aprov%');";

        // Buscar níveis de importância (CodTipoEstrutura = 12)
        public const string SelectNiveisImportancia = @"
            SELECT CodEstrutura AS Value, DescEstrutura AS Text
            FROM dbo.Estrutura 
            WHERE CodTipoEstrutura = 12 
            ORDER BY ISNULL(ValorPadrao, 999), DescEstrutura";

        // Listar demandas que estão aguardando aprovação e que o gestor logado é o aprovador
        public const string DemandasAguardandoAprovacaoPorGestor = @"
            SELECT 
                d.CodDemanda,
                d.Titulo,
                cat.DescEstrutura AS Categoria,
                s.DescEstrutura AS Status,
                p.Nome + ' ' + ISNULL(p.Sobrenome, '') AS Solicitante,
                d.DataDemanda AS DataSolicitacao,
                d.DataPrazoMaximo AS DataPrazo,
                pri.DescEstrutura AS Prioridade,
                d.CodEstr_NivelPrioridade AS CodPrioridade,
                imp.DescEstrutura AS Importancia,
                d.CodEstr_NivelImportancia AS CodImportancia,
                d.CodPessoaExecucao,
                d.DataAceitacao,
                pexec.Nome + ' ' + ISNULL(pexec.Sobrenome, '') AS NomePessoaExecucao,
                d.CodPessoaAprovacao,
                d.DataAprovacao,
                psolic.Nome + ' ' + ISNULL(psolic.Sobrenome, '') AS NomeSolicitanteCompleto
            FROM dbo.Demanda d
            INNER JOIN dbo.Pessoa p ON d.CodPessoaSolicitacao = p.CodPessoa
            INNER JOIN dbo.Estrutura s ON d.CodEstr_SituacaoDemanda = s.CodEstrutura
            INNER JOIN dbo.Estrutura cat ON d.CodEstr_TipoDemanda = cat.CodEstrutura
            LEFT JOIN dbo.Estrutura pri ON d.CodEstr_NivelPrioridade = pri.CodEstrutura
            LEFT JOIN dbo.Estrutura imp ON d.CodEstr_NivelImportancia = imp.CodEstrutura
            LEFT JOIN dbo.Pessoa pexec ON d.CodPessoaExecucao = pexec.CodPessoa
            LEFT JOIN dbo.Pessoa psolic ON d.CodPessoaSolicitacao = psolic.CodPessoa
            WHERE d.CodEstr_SituacaoDemanda = 65  -- Aguardando Aprovação
              AND d.CodPessoaAprovacao = @CodPessoa  -- Gestor logado
              AND d.DataAprovacao IS NULL  -- Ainda não aprovada
            ORDER BY 
                d.CodEstr_NivelPrioridade DESC,
                CASE WHEN d.DataPrazoMaximo IS NULL THEN 1 ELSE 0 END,
                d.DataPrazoMaximo ASC,
                d.CodEstr_NivelImportancia DESC,
                d.DataDemanda DESC";

        //DEMANDAS CONCLUÍDAS
        public const string DemandasConcluidasPorPessoa = @"
        SELECT 
            d.CodDemanda,
            d.Titulo,
            cat.DescEstrutura AS Categoria,
            s.DescEstrutura AS Status,
            p.Nome + ' ' + ISNULL(p.Sobrenome, '') AS Solicitante,
            d.DataDemanda AS DataSolicitacao,
            d.DataPrazoMaximo AS DataPrazo, 
            pri.DescEstrutura AS Prioridade,
            d.CodEstr_NivelPrioridade AS CodPrioridade,
            imp.DescEstrutura AS Importancia,
            d.CodEstr_NivelImportancia AS CodImportancia,
            d.CodPessoaExecucao,
            d.DataAceitacao,
            pexec.Nome + ' ' + ISNULL(pexec.Sobrenome, '') AS NomePessoaExecucao
        FROM dbo.Demanda d
        INNER JOIN dbo.Pessoa p ON d.CodPessoaSolicitacao = p.CodPessoa
        INNER JOIN dbo.Estrutura s ON d.CodEstr_SituacaoDemanda = s.CodEstrutura
        INNER JOIN dbo.Estrutura cat ON d.CodEstr_TipoDemanda = cat.CodEstrutura
        LEFT JOIN dbo.Estrutura pri ON d.CodEstr_NivelPrioridade = pri.CodEstrutura
        LEFT JOIN dbo.Estrutura imp ON d.CodEstr_NivelImportancia = imp.CodEstrutura
        LEFT JOIN dbo.Pessoa pexec ON d.CodPessoaExecucao = pexec.CodPessoa
        WHERE d.CodEstr_SituacaoDemanda = 23  -- Status Concluída
          AND (d.CodPessoaSolicitacao = @CodPessoa OR d.CodPessoaExecucao = @CodPessoa)
        ORDER BY d.DataDemanda DESC";

        // DEMANDAS RECUSADAS - VERSÃO QUE FUNCIONA
        public const string DemandasRecusadasPorPessoa = @"
SELECT 
    d.CodDemanda,
    d.Titulo,
    cat.DescEstrutura AS Categoria,
    s.DescEstrutura AS Status,
    p.Nome + ' ' + ISNULL(p.Sobrenome, '') AS Solicitante,
    d.DataDemanda AS DataSolicitacao,
    d.DataPrazoMaximo AS DataPrazo, 
    pri.DescEstrutura AS Prioridade,
    d.CodEstr_NivelPrioridade AS CodPrioridade,
    imp.DescEstrutura AS Importancia,
    d.CodEstr_NivelImportancia AS CodImportancia,
    ISNULL(d.CodPessoaExecucao, ph.CodPessoaAlteracao) AS CodPessoaExecucao,
    ISNULL(pexec.Nome + ' ' + ISNULL(pexec.Sobrenome,''), 'Sem executor') AS NomePessoaExecucao,
    d.DataAceitacao  -- ⚠️ Adicionei aqui
FROM dbo.Demanda d
INNER JOIN dbo.Pessoa p ON d.CodPessoaSolicitacao = p.CodPessoa
INNER JOIN dbo.Estrutura s ON d.CodEstr_SituacaoDemanda = s.CodEstrutura
INNER JOIN dbo.Estrutura cat ON d.CodEstr_TipoDemanda = cat.CodEstrutura
LEFT JOIN dbo.Estrutura pri ON d.CodEstr_NivelPrioridade = pri.CodEstrutura
LEFT JOIN dbo.Estrutura imp ON d.CodEstr_NivelImportancia = imp.CodEstrutura
LEFT JOIN (
    SELECT h1.CodDemanda, h1.CodPessoaAlteracao
    FROM dbo.DemandaHistorico h1
    WHERE h1.CodEstr_SituacaoDemandaAtual = 20
      AND h1.DataAlteracao = (
          SELECT MAX(h2.DataAlteracao)
          FROM dbo.DemandaHistorico h2
          WHERE h2.CodDemanda = h1.CodDemanda
            AND h2.CodEstr_SituacaoDemandaAtual = 20
      )
) ph ON ph.CodDemanda = d.CodDemanda
LEFT JOIN dbo.Pessoa pexec ON pexec.CodPessoa = ISNULL(d.CodPessoaExecucao, ph.CodPessoaAlteracao)
WHERE d.CodEstr_SituacaoDemanda = 20
  AND (
        d.CodPessoaSolicitacao = @CodPessoa
        OR EXISTS (
            SELECT 1 
            FROM dbo.DemandaHistorico h 
            WHERE h.CodDemanda = d.CodDemanda
              AND h.CodEstr_SituacaoDemandaAtual = 20
              AND h.CodPessoaAlteracao = @CodPessoa
        )
      )
ORDER BY d.DataDemanda DESC";


        public const string VerificarGestor = @"
            SELECT COUNT(1) 
            FROM dbo.Pessoa p
            WHERE p.CodPessoa = @CodPessoa 
              AND p.CodDepartamento = @CodSetor
              AND p.Conf_Ativo = 1
              AND p.CodCargo = 2";

        public const string VerificarAdministrador = @"
            SELECT COUNT(*) 
            FROM AutenticacaoAcesso 
            WHERE CodPessoa = @CodPessoa 
            AND CodPerfilUsuario = 1
            AND Conf_PermiteAcesso = 1 
            AND Conf_Ativo = 1";

        public const string GetDemandaPorCodigo = @"
            SELECT 
                d.CodDemanda,
                d.Titulo,
                d.TextoDemanda,
                d.CodPessoaSolicitacao,
                d.CodPessoaExecucao,
                d.CodEstr_SituacaoDemanda,
                d.DataDemanda,
                d.DataPrazoMaximo,
                d.CodEstr_NivelPrioridade,
                d.CodEstr_NivelImportancia,
                s.DescEstrutura AS Status,
                p.Nome + ' ' + ISNULL(p.Sobrenome, '') AS Solicitante,
                cat.DescEstrutura AS Categoria
            FROM dbo.Demanda d
            INNER JOIN dbo.Pessoa p ON d.CodPessoaSolicitacao = p.CodPessoa
            INNER JOIN dbo.Estrutura s ON d.CodEstr_SituacaoDemanda = s.CodEstrutura
            INNER JOIN dbo.Estrutura cat ON d.CodEstr_TipoDemanda = cat.CodEstrutura
            WHERE d.CodDemanda = @CodDemanda";

        // ATUALIZE A CONSTANTE PARA INCLUIR O HISTÓRICO
        public const string RecusarDemanda = @"
            BEGIN TRANSACTION;
            UPDATE dbo.Demanda 
            SET 
                CodEstr_SituacaoDemanda = @CodStatusRecusada,
                CodPessoaExecucao = NULL,
                DataAceitacao = NULL
            WHERE CodDemanda = @CodDemanda;
    
            INSERT INTO dbo.DemandaHistorico 
            (CodDemanda, CodEstr_SituacaoDemandaAnterior, CodEstr_SituacaoDemandaAtual, CodPessoaAlteracao, DataAlteracao)
            VALUES 
            (@CodDemanda, @CodStatusAnterior, @CodStatusRecusada, @CodPessoaAlteracao, GETDATE());
    
            COMMIT TRANSACTION;";

    }
}