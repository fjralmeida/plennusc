using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.registrationPlan
{
    public class PlanoQueries
    {
        // ============================================================
        //  PLANOS JÁ IMPORTADOS (grid principal - Plennus)
        // ============================================================
        public const string ListarPlanos = @"
            SELECT
                CodigoPlano, CodigoProduto, RegistroANS, Num_CNPJ_Operadora,
                TipoContratacao, Nome, NomePlanoComercial, Segmentacao,
                Abrangencia, Coparticipacao, Acomodacao, DecSau, Promocional,
                Conf_Ativo, CodPessoaCadastro, CodPessoaAlteracao,
                Informacoes_log_i, Informacoes_log_a, Informacoes_log_e
            FROM API_VENDA_PLANO
            WHERE 1=1
              AND (@NomePlanoComercial IS NULL OR NomePlanoComercial LIKE '%' + @NomePlanoComercial + '%')
              AND (@Segmentacao IS NULL OR Segmentacao LIKE '%' + @Segmentacao + '%')
              AND (@Abrangencia IS NULL OR Abrangencia LIKE '%' + @Abrangencia + '%')
              AND (@Coparticipacao IS NULL OR Coparticipacao LIKE '%' + @Coparticipacao + '%')
            ORDER BY NomePlanoComercial";

        // ============================================================
        //  PLANOS DO ALIANÇA COM FLAG_LIBERAVENDA = 'S' (FILTRADOS POR CNPJ)
        // ============================================================
        public const string ListarPlanosAliancaLiberados = @"
                  SELECT
                    p.CODIGO_PLANO                 AS CodigoPlano,
                    p.NOME_PLANO_FAMILIARES        AS NomePlanoFamiliar,
                    p.NOME_PLANO_EMPRESAS          AS NomePlanoEmpresa,
                    p.CODIGO_CADASTRO_ANS          AS RegistroANS,
                    p.TIPO_CONTRATACAO_ANS         AS TipoContratacao,
                    p.CODIGO_TIPO_ACOMODACAO       AS CodigoAcomodacao,
                    p.CODIGO_TIPO_ABRANGENCIA      AS CodigoAbrangencia,
                    p.FLAG_COPARTICIPACAO          AS Coparticipacao,
                    p.CODIGO_TIPO_COBERTURA        AS Segmentacao,
                    p.FLAG_LIBERAVENDA             AS Conf_Ativo,
                    p.CODIGO_GRUPO_CONTRATO        AS CodigoGrupoContrato,
                    e.CNPJ_OPERADORA               AS CnpjOperadora,
                    e.NOME_OPERADORA               AS NomeOperadora,
                    CASE p.TIPO_CONTRATACAO_ANS
                        WHEN 3 THEN 'PJ'
                        WHEN 4 THEN 'AD'
                        ELSE ''
                    END AS TipoContratacaoDescricao,
                    CASE p.CODIGO_TIPO_ACOMODACAO
                        WHEN 1 THEN 'I'
                        WHEN 2 THEN 'C'
                        ELSE ''
                    END AS AcomodacaoDescricao
                FROM PS1030 p
                LEFT JOIN ESP0002 e ON e.CODIGO_GRUPO_CONTRATO = p.CODIGO_GRUPO_CONTRATO
                WHERE p.FLAG_LIBERAVENDA = 'S'
                ORDER BY p.NOME_PLANO_FAMILIARES";

        // ============================================================
        //  CÓDIGOS DOS PLANOS JÁ EXISTENTES NO PLENNUS
        // ============================================================
        public const string ListarCodigosPlanosExistentes = @"
            SELECT CodigoPlano FROM API_VENDA_PLANO";

        // ============================================================
        //  INSERIR PLANO (Plennus)
        // ============================================================
        public const string InserirPlano = @"
            INSERT INTO API_VENDA_PLANO
            (
                CodigoPlano, CodigoProduto, RegistroANS, Num_CNPJ_Operadora,
                TipoContratacao, Nome, NomePlanoComercial, Segmentacao,
                Abrangencia, Coparticipacao, Acomodacao, DecSau, Promocional,
                Conf_Ativo, CodPessoaCadastro, CodPessoaAlteracao,
                Informacoes_log_i, Informacoes_log_a
            )
            VALUES
            (
                @CodigoPlano, @CodigoPlano, @RegistroANS, @Num_CNPJ_Operadora,
                @TipoContratacao, @Nome, @NomePlanoComercial, @Segmentacao,
                @Abrangencia, @Coparticipacao, @Acomodacao, @DecSau, @Promocional,
                @Conf_Ativo, @CodPessoaCadastro, @CodPessoaAlteracao,
                GETDATE(), NULL
            )";

        // ============================================================
        //  OPERADORAS (dropdown do modal)
        // ============================================================
        public const string ListarOperadoras = @"
            SELECT
                CodigoOperadora,
                NomeComercial  AS NomeOperadora,
                Numero_CNPJ    AS Num_CNPJ_Operadora,
                RegistroANS
            FROM API_Venda_Operadora
            ORDER BY NomeComercial";
    }
}
