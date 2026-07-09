using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.operatorRegistration
{
    /// <summary>
    /// Todas as queries SQL da tela de cadastro/sincronização de operadoras.
    /// ATENÇÃO: classe com O maiúsculo — OperatorRegistrationQueries.
    /// </summary>
    public static class OperatorRegistrationQueries
    {
        // ─────────────────────────────────────────────────────────────────────
        // BANCO 2 (Plennus) — listagem com filtros
        // Placeholders substituídos dinamicamente no service
        // ─────────────────────────────────────────────────────────────────────

        public const string ListarOperadoras = @"
            SELECT
                o.CodigoOperadora,
                o.RegistroANS,
                o.Numero_CNPJ,
                o.RazaoSocial,
                o.NomeComercial,
                o.CodPessoaCadastro,
                o.CodPessoaAlteracao,
                o.Informacoes_log_a,
                o.Informacoes_log_i
            FROM dbo.API_Venda_Operadora o
            WHERE 1 = 1
              {FILTRO_NOME}
              {FILTRO_ANS}
              {FILTRO_CNPJ}
            ORDER BY o.CodigoOperadora";

        // ─────────────────────────────────────────────────────────────────────
        // BANCO 1 (Alianca) — todas as operadoras para comparação em memória
        // ─────────────────────────────────────────────────────────────────────

        public const string BuscarTodasOperadorasAlianca = @"
             SELECT
                e.CODIGO_GRUPO_CONTRATO AS CodigoGrupo,
                e.CNPJ_OPERADORA        AS Numero_CNPJ,
                e.NUMERO_ANS_OPERADORA  AS RegistroANS,
                e.NOME_OPERADORA        AS RazaoSocial,
                e.NOME_GRUPO_CONTRATO   AS NomeComercial
            FROM ESP0002 e
            WHERE e.CNPJ_OPERADORA IS NOT NULL
              AND NUMERO_ANS_OPERADORA IS NOT NULL
              AND LTRIM(RTRIM(e.CNPJ_OPERADORA)) <> ''
            ORDER BY e.NOME_GRUPO_CONTRATO";

        // ─────────────────────────────────────────────────────────────────────
        // BANCO 2 (Plennus) — CNPJs já cadastrados
        // ─────────────────────────────────────────────────────────────────────

        public const string BuscarCNPJsCadastrados = @"
            SELECT LTRIM(RTRIM(Numero_CNPJ)) AS Numero_CNPJ
            FROM dbo.API_Venda_Operadora
            WHERE Numero_CNPJ IS NOT NULL
              AND LTRIM(RTRIM(Numero_CNPJ)) <> ''";

        // ─────────────────────────────────────────────────────────────────────
        // BANCO 2 (Plennus) — inserção de operadora nova
        // Parâmetros: @RegistroANS, @Numero_CNPJ, @RazaoSocial,
        //             @NomeComercial, @CodPessoaCadastro, @DataLog
        // ─────────────────────────────────────────────────────────────────────

        public const string InserirOperadora = @"
            INSERT INTO dbo.API_Venda_Operadora
                (RegistroANS, Numero_CNPJ, RazaoSocial, NomeComercial,
                 CodPessoaCadastro, Informacoes_log_i)
            VALUES
                (@RegistroANS, @Numero_CNPJ, @RazaoSocial, @NomeComercial,
                 @CodPessoaCadastro, @DataLog)";

        // ─────────────────────────────────────────────────────────────────────
        // BANCO 2 (Plennus) — atualização de operadora existente
        // Parâmetros: @RegistroANS, @RazaoSocial, @NomeComercial,
        //             @CodPessoaAlteracao, @DataLog, @Numero_CNPJ
        // ─────────────────────────────────────────────────────────────────────

        public const string AtualizarOperadora = @"
            UPDATE dbo.API_Venda_Operadora
            SET RegistroANS        = @RegistroANS,
                RazaoSocial        = @RazaoSocial,
                NomeComercial      = @NomeComercial,
                CodPessoaAlteracao = @CodPessoaAlteracao,
                Informacoes_log_a  = @DataLog
            WHERE LTRIM(RTRIM(Numero_CNPJ)) = LTRIM(RTRIM(@Numero_CNPJ))";

        // ─────────────────────────────────────────────────────────────────────
        // BANCO 2 (Plennus) — nome do usuário logado (tabela Pessoa)
        // Parâmetro: @CodPessoa
        // ─────────────────────────────────────────────────────────────────────

        public const string BuscarNomePessoaPorAutenticacao = @"
            SELECT
                p.CodPessoa,
                LTRIM(RTRIM(p.Nome)) + ' ' + LTRIM(RTRIM(p.Sobrenome)) AS NomeCompleto,
                LTRIM(RTRIM(ISNULL(p.Apelido, '')))                     AS Apelido
            FROM dbo.AutenticacaoAcesso a
            INNER JOIN dbo.Pessoa p ON p.CodPessoa = a.CodPessoa
            WHERE a.CodAutenticacaoAcesso = @CodAutenticacaoAcesso";

        // ─────────────────────────────────────────────────────────────────────
        // BANCO 2 (Plennus) — operadoras existentes para comparar alterações
        // ─────────────────────────────────────────────────────────────────────

        public const string BuscarOperadorasExistentesPlennus = @"
            SELECT
                o.CodigoOperadora,
                LTRIM(RTRIM(o.Numero_CNPJ)) AS Numero_CNPJ,
                o.RegistroANS,
                o.RazaoSocial,
                o.NomeComercial
            FROM dbo.API_Venda_Operadora o
            WHERE o.Numero_CNPJ IS NOT NULL
              AND LTRIM(RTRIM(o.Numero_CNPJ)) <> ''";

        // ─────────────────────────────────────────────────────────────────────
        // BANCO 2 (Plennus) — atualização dinâmica (usada na sincronização)
        // ─────────────────────────────────────────────────────────────────────

        public const string AtualizarOperadoraDinamica = @"
            UPDATE dbo.API_Venda_Operadora
            SET RazaoSocial        = @RazaoSocial,
                NomeComercial      = @NomeComercial,
                {SET_ANS}
                CodPessoaAlteracao = @CodPessoaAlteracao,
                Informacoes_log_a  = @DataLog
            WHERE LTRIM(RTRIM(Numero_CNPJ)) = LTRIM(RTRIM(@Numero_CNPJ))";

        // ─────────────────────────────────────────────────────────────────────-──────
        //  Banco 2 (Plennus) — consultas específicas para edição manual de operadora
        // ────────────────────────────────────────────────────────────────────────────

        // Obtém uma operadora específica pelo código
        public const string ObterOperadoraPorCodigo = @"
            SELECT
                o.CodigoOperadora,
                o.RegistroANS,
                o.Numero_CNPJ,
                o.RazaoSocial,
                o.NomeComercial,
                o.CodPessoaCadastro,
                o.CodPessoaAlteracao,
                o.Informacoes_log_i,
                o.Informacoes_log_a
            FROM dbo.API_Venda_Operadora o
            WHERE o.CodigoOperadora = @CodigoOperadora";

        // Verifica se uma operadora existe pelo código
        public const string VerificarOperadoraExiste = @"
            SELECT COUNT(1)
            FROM dbo.API_Venda_Operadora
            WHERE CodigoOperadora = @CodigoOperadora";

        // Atualiza ambos Razão Social e Nome Comercial (usado na edição manual)
        public const string AtualizarOperadoraEdit = @"
            UPDATE dbo.API_Venda_Operadora
            SET 
                RazaoSocial        = @RazaoSocial,
                NomeComercial      = @NomeComercial,
                CodPessoaAlteracao = @CodPessoaAlteracao,
                Informacoes_log_a  = @DataLog
            WHERE CodigoOperadora = @CodigoOperadora";

        // Atualiza apenas Razão Social (usado na edição manual)
        public const string AtualizarRazaoSocial = @"
            UPDATE dbo.API_Venda_Operadora
            SET 
                RazaoSocial        = @RazaoSocial,
                CodPessoaAlteracao = @CodPessoaAlteracao,
                Informacoes_log_a  = @DataLog
            WHERE CodigoOperadora = @CodigoOperadora";

        // Atualiza apenas Nome Comercial (usado na edição manual)
        public const string AtualizarNomeComercial = @"
            UPDATE dbo.API_Venda_Operadora
            SET 
                NomeComercial      = @NomeComercial,
                CodPessoaAlteracao = @CodPessoaAlteracao,
                Informacoes_log_a  = @DataLog
            WHERE CodigoOperadora = @CodigoOperadora";
    }
}