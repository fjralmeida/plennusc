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
                o.Informacoes_log_i,
                o.Informacoes_log_a
            FROM dbo.API_Venda_Operadora o
            WHERE 1 = 1
              {FILTRO_NOME}
              {FILTRO_ANS}
              {FILTRO_CNPJ}
            ORDER BY o.NomeComercial";

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
    }
}