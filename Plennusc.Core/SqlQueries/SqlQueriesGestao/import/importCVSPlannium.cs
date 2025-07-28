using appWhatsapp.Data_Bd;
using Plennusc.Core.Models.ModelsGestao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.import
{
    public class importCVSPlannium
    {
        public void InserirTMP1000Net(DadosMensagensCsvPlanium item)
        {
            string sql = @"
        INSERT INTO TMP1000_NET (
            NOME_ASSOCIADO, NUMERO_RG, ORGAO_EMISSOR_RG, NUMERO_CPF, CODIGO_CNS,
            DATA_NASCIMENTO, SEXO, CODIGO_ESTADO_CIVIL, CODIGO_PLANO, DATA_ADMISSAO,
            DATA_VALIDA_CARENCIA, CODIGO_EMPRESA, NOME_MAE, CODIGO_TITULAR,
            CODIGO_PARENTESCO, NUMERO_DECLARACAO_NASC_VIVO, CODIGO_VENDEDOR,
            PROFISSAO, CODIGO_CADASTRO_ANS
        )
        VALUES (
            @Nome, @RG, @OrgaoExp, @CPF, @CNS,
            @DataNascimento, @Sexo, @EstadoCivil, @CodPlano, @DtInclusao,
            @DtVigencia, @CodEmpresa, @NomeMae, @CodResp,
            @Parentesco, @NrDeclNascVivo, @CodVend,
            @Profissao, @CodANS
        )";

            var parametros = new Dictionary<string, object>
            {
                ["@Nome"] = item.NOME,
                ["@RG"] = item.ID,
                ["@OrgaoExp"] = item.ORG_EXP,
                ["@CPF"] = item.CPF,
                ["@CNS"] = item.CNS,
                ["@DataNascimento"] = item.DT_NASC,
                ["@Sexo"] = item.SEXO,
                ["@EstadoCivil"] = item.EST_CIVIL,
                ["@CodPlano"] = item.COD_PLANO,
                ["@DtInclusao"] = item.DT_INCL,
                ["@DtVigencia"] = item.DT_VIGENCIA,
                ["@CodEmpresa"] = item.COD_EMPRESA ?? "400",
                ["@NomeMae"] = item.NOME_MAE,
                ["@CodResp"] = item.COD_RESP,
                ["@Parentesco"] = item.PARENT,
                ["@NrDeclNascVivo"] = item.NR_DEC_NASC_VIVO,
                ["@CodVend"] = item.COD_VEND,
                ["@Profissao"] = item.COD_PROFISSAO,
                ["@CodANS"] = item.COD_ANS_OPERADORA
            };

            new Banco_Dados_SQLServer().ExecutarAlianca(sql, parametros);
        }
        public void InserirTMP1001(DadosMensagensCsvPlanium item)
        {
            string sql = @"
                INSERT INTO TMP1001 (
                    NUMERO_CPF, ENDERECO, BAIRRO, CIDADE,
                    ESTADO, CEP, ENDERECO_EMAIL
                )
                VALUES (
                    @CPF, @Endereco, @Bairro, @Cidade,
                    @Estado, @CEP, @Email
                )";

            var endereco = $"{(string.IsNullOrWhiteSpace(item.TIPO_LOGRADOURO) ? "Rua" : item.TIPO_LOGRADOURO.Trim())} {item.RUA?.Trim()}, {item.NUMERO_LOGRADOURO?.Trim()} - {item.COMPLEMENTO?.Trim()}".Trim();

            var parametros = new Dictionary<string, object>
            {
                ["@CPF"] = item.CPF,
                ["@Endereco"] = endereco,
                ["@Bairro"] = item.BAIRRO,
                ["@Cidade"] = item.CIDADE,
                ["@Estado"] = item.UF,
                ["@CEP"] = item.CEP,
                ["@Email"] = item.EMAIL
            };

            new Banco_Dados_SQLServer().ExecutarAlianca(sql, parametros);
        }
        public void InserirTMP1002(DadosMensagensCsvPlanium item)
        {
            string sql = @"
            INSERT INTO TMP1002 (
                NUMERO_CPF, DIA_VENCIMENTO
            )
            VALUES (
                @CPF, @DiaVenc
            )";

            var parametros = new Dictionary<string, object>
            {
                ["@CPF"] = item.CPF,
                ["@DiaVenc"] = item.DIA_VENC
            };

            new Banco_Dados_SQLServer().ExecutarAlianca(sql, parametros);
        }
        public void InserirTMP1006(DadosMensagensCsvPlanium item)
        {
            string telefone1 = $"{item.DDD_TEL?.Trim()}{item.TEL?.Trim()}";
            string telefone2 = $"{item.DDD_TEL_2?.Trim()}{item.TEL_2?.Trim()}";
            string celular = $"{item.DDD_CEL?.Trim()}{item.CELULAR?.Trim()}";

            string sql = @"
            INSERT INTO TMP1006 (
                NUMERO_CPF, NUMERO_TELEFONE_1, NUMERO_TELEFONE_2, NUMERO_TELEFONE_CEL
            )
            VALUES (
                @CPF, @Tel1, @Tel2, @Celular
            )";

            var parametros = new Dictionary<string, object>
            {
                ["@CPF"] = item.CPF,
                ["@Tel1"] = telefone1,
                ["@Tel2"] = telefone2,
                ["@Celular"] = celular
            };

            new Banco_Dados_SQLServer().ExecutarAlianca(sql, parametros);
        }
    }
}
