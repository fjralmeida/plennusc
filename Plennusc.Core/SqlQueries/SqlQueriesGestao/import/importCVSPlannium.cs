using appWhatsapp.Data_Bd;
using Plennusc.Core.Models.ModelsGestao;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.import
{
    public class importCVSPlannium
    {
        public int BuscarProximoCodigoAssociado()
        {
            string sql = "SELECT ISNULL(MAX(CODIGO_ASSOCIADO), 0) FROM VND1000_ON";

            Banco_Dados_SQLServer bd = new Banco_Dados_SQLServer();
            DataTable resultado = bd.LerAlianca(sql);

            if (resultado.Rows.Count > 0)
            {
                return Convert.ToInt32(resultado.Rows[0][0]); // retorna o MAIOR código
            }

            return 0;
        }



        public void InserirTMP1000Net(DadosMensagensCsvPlanium item)
        {
            string codigoAssociado = "1";

            string sql = @"
        INSERT INTO TMP1000_NET (
            CODIGO_ASSOCIADO, NOME_ASSOCIADO, NUMERO_RG, ORGAO_EMISSOR_RG, NUMERO_CPF, CODIGO_CNS,
            DATA_NASCIMENTO, SEXO, CODIGO_ESTADO_CIVIL, CODIGO_PLANO, DATA_ADMISSAO,
            DATA_VALIDA_CARENCIA, CODIGO_EMPRESA, NOME_MAE, CODIGO_TITULAR,
            CODIGO_PARENTESCO, NUMERO_DECLARACAO_NASC_VIVO, CODIGO_VENDEDOR,
            PROFISSAO, CODIGO_CADASTRO_ANS
        )
        VALUES (
              @CodAssociado, @Nome, @RG, @OrgaoExp, @CPF, @CNS,
            @DataNascimento, @Sexo, @EstadoCivil, @CodPlano, @DtInclusao,
            @DtVigencia, @CodEmpresa, @NomeMae, @CodResp,
            @Parentesco, @NrDeclNascVivo, @CodVend,
            @Profissao, @CodANS
        )";

            var parametros = new Dictionary<string, object>
            {
                ["@CodAssociado"] = codigoAssociado,
                ["@Nome"] = item.NOME,
                ["@RG"] = item.ID,
                ["@OrgaoExp"] = item.ORG_EXP,
                ["@CPF"] = item.CPF,
                ["@CNS"] = item.CNS,
                ["@DataNascimento"] = item.DT_NASC,
                ["@Sexo"] = item.SEXO,
                ["@EstadoCivil"] = item.RESPF_ESTADOCIVIL,
                ["@CodPlano"] = item.COD_PLANO,
                ["@DtInclusao"] = item.DT_INCL,
                ["@DtVigencia"] = item.DT_VIGENCIA,
                ["@CodEmpresa"] = item.COD_EMPRESA ?? "400",
                ["@NomeMae"] = item.NOME_MAE,
                ["@CodResp"] = item.COD_RESP,
                ["@Parentesco"] = item.PARENT,
                ["@NrDeclNascVivo"] = item.NR_DEC_NASC_VIVO,
                ["@CodVend"] = "1",
                ["@Profissao"] = item.COD_PROFISSAO,
                ["@CodANS"] = item.COD_ANS_OPERADORA
            };

            new Banco_Dados_SQLServer().ExecutarAlianca(sql, parametros);
        }
        public void InserirTMP1001(DadosMensagensCsvPlanium item)
        {
            string sql = @"
                INSERT INTO TMP1001 (
                    ENDERECO, BAIRRO, CIDADE,
                    ESTADO, CEP, ENDERECO_EMAIL
                )
                VALUES (
                  @Endereco, @Bairro, @Cidade,
                    @Estado, @CEP, @Email
                )";

            var endereco = $"{(string.IsNullOrWhiteSpace(item.TIPO_LOGRADOURO) ? "Rua" : item.TIPO_LOGRADOURO.Trim())} {item.RUA?.Trim()}, {item.NUMERO_LOGRADOURO?.Trim()} - {item.COMPLEMENTO?.Trim()}".Trim();

            var parametros = new Dictionary<string, object>
            {
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

        public void InserirVND1000_ON(DadosMensagensCsvPlanium item)
        {
            string sql = @"
            INSERT INTO VND1000_ON (
                CODIGO_ASSOCIADO, CODIGO_EMPRESA, CODIGO_CARENCIA, NOME_ASSOCIADO, CODIGO_TABELA_PRECO,
                CODIGO_PLANO, DATA_NASCIMENTO, DATA_ADMISSAO, DATA_DIGITACAO, SEXO, FLAG_PLANOFAMILIAR,
                NOME_MAE, CODIGO_PARENTESCO, VALOR_NOMINAL, CODIGO_ESTADO_CIVIL, CODIGO_TITULAR,
                FLAG_ISENTO_PAGTO, TIPO_ASSOCIADO, NUMERO_CPF, NUMERO_RG, ORGAO_EMISSOR_RG,
                CODIGO_CNS, NUMERO_DECLARACAO_NASC_VIVO, ULTIMO_STATUS, CODIGO_VENDEDOR,
                DATA_VALIDA_CARENCIA, DATA_VIGENCIA
            )
            VALUES (
                @CodAssociado, @CodEmpresa, 0, @Nome, 0,
                @CodPlano, @DataNascimento, @DtAdmissao, GETDATE(), @Sexo, 'S',
                @NomeMae, @Parentesco, NULL, @EstadoCivil, @CodTitular,
                NULL, 'T', @CPF, @RG, @OrgaoExp,
                @CNS, @NrDeclNascVivo, 'AGUARDANDO_AVALIACAO', @CodVend,
                @DtValidaCarencia, @DtVigencia
            )";

            var parametros = new Dictionary<string, object>
            {
                ["@CodAssociado"] = item.CODIGO_ASSOCIADO,
                ["@CodEmpresa"] = item.COD_EMPRESA ?? "400",
                ["@Nome"] = item.NOME,
                ["@CodPlano"] = item.COD_PLANO,
                ["@DataNascimento"] = item.DT_NASC,
                ["@DtAdmissao"] = item.DT_INCL,
                ["@Sexo"] = item.SEXO,
                ["@NomeMae"] = item.NOME_MAE,
                ["@Parentesco"] = item.PARENT,
                ["@EstadoCivil"] = item.RESPF_ESTADOCIVIL,
                ["@CodTitular"] = item.COD_RESP,
                ["@CPF"] = item.CPF,
                ["@RG"] = item.ID,
                ["@OrgaoExp"] = item.ORG_EXP,
                ["@CNS"] = item.CNS,
                ["@NrDeclNascVivo"] = item.NR_DEC_NASC_VIVO,
                ["@CodVend"] = "1",
                ["@DtValidaCarencia"] = item.DT_VIGENCIA,
                ["@DtVigencia"] = item.DT_VIGENCIA
            };

            new Banco_Dados_SQLServer().ExecutarAlianca(sql, parametros);
        }

    }
}
