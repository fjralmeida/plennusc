using appWhatsapp.Data_Bd;
using Plennusc.Core.Models.ModelsGestao;
using Plennusc.Core.Models.ModelsMedic;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesMedic.import
{
    public class importCVSPlanniumMedic
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

        public void InserirProposta(DadosMensagensCsvPlaniumMedic item)
        {
            string sql = @"
        INSERT INTO PropostaImportacao (
            Nome, IdDocumento, OrgaoExpedidor, Cpf, Pis, Cns, DataNascimento, Naturalidade, Sexo, EstadoCivil,
            TipoLogradouro, Rua, NumeroLogradouro, Complemento, Bairro, Cidade, Uf, Cep, DddTelefone, Telefone,
            DddTelefone2, Telefone2, DddCelular, Celular, CodigoPlano, DataInclusao, DataVigencia, CodigoEmpresa,
            CodigoUnidade, Matricula, Admissao, NomeMae, NomePai, Email, CodigoResponsavel, Parentesco, Universidade,
            NumeroDeclaracaoNascimento, Agregado, DeficienteInvalido, CodigoLotacao, TipoMovimentacao, DataExclusao,
            MotivoExclusao, CodigoOutro, CodigoGseg, CodigoVendedor, CodigoProposta, Observacao, ObservacaoTecnica,
            MostraLiberacao, CodigoForma, DiaVencimento, CodigoTabelaComissao, Cargo, Responsavel, DataCasamento,
            UfOrgao, CodigoGrupo, CodigoSupervisor, CodigoProfissao, CnpjOperadora, CodigoAnsOperadora, NomeOperadora,
            Idade, RespfMae, RespfNascimento, RespfEmail, RespfIdade, RespfSexo, RespfEstadoCivil, RespfRg,
            RespfOrgaoExp, RespfCns, NomeVendedor, CpfVendedor, EmailVendedor, TelefoneVendedor, MensalidadeTit,
            MensalidadeDep, Acessorios, Informacoes_Log_I,Informacoes_Log_A
        )
        VALUES (
            @Nome, @IdDocumento, @OrgaoExpedidor, @Cpf, @Pis, @Cns, @DataNascimento, @Naturalidade, @Sexo, @EstadoCivil,
            @TipoLogradouro, @Rua, @NumeroLogradouro, @Complemento, @Bairro, @Cidade, @Uf, @Cep, @DddTelefone, @Telefone,
            @DddTelefone2, @Telefone2, @DddCelular, @Celular, @CodigoPlano, @DataInclusao, @DataVigencia, @CodigoEmpresa,
            @CodigoUnidade, @Matricula, @Admissao, @NomeMae, @NomePai, @Email, @CodigoResponsavel, @Parentesco, @Universidade,
            @NumeroDeclaracaoNascimento, @Agregado, @DeficienteInvalido, @CodigoLotacao, @TipoMovimentacao, @DataExclusao,
            @MotivoExclusao, @CodigoOutro, @CodigoGseg, @CodigoVendedor, @CodigoProposta, @Observacao, @ObservacaoTecnica,
            @MostraLiberacao, @CodigoForma, @DiaVencimento, @CodigoTabelaComissao, @Cargo, @Responsavel, @DataCasamento,
            @UfOrgao, @CodigoGrupo, @CodigoSupervisor, @CodigoProfissao, @CnpjOperadora, @CodigoAnsOperadora, @NomeOperadora,
            @Idade, @RespfMae, @RespfNascimento, @RespfEmail, @RespfIdade, @RespfSexo, @RespfEstadoCivil, @RespfRg,
            @RespfOrgaoExp, @RespfCns, @NomeVendedor, @CpfVendedor, @EmailVendedor, @TelefoneVendedor, @MensalidadeTit,
            @MensalidadeDep, @Acessorios, @Informacoes_Log_I, @Informacoes_Log_A
        )";

            var parametros = new Dictionary<string, object>
            {
                ["@Nome"] = item.NOME ?? (object)DBNull.Value,
                ["@IdDocumento"] = ParseNullableLong(item.ID),
                ["@OrgaoExpedidor"] = item.ORG_EXP ?? (object)DBNull.Value,
                ["@Cpf"] = item.CPF ?? (object)DBNull.Value,
                ["@Pis"] = item.PIS ?? (object)DBNull.Value,
                ["@Cns"] = item.CNS ?? (object)DBNull.Value,
                ["@DataNascimento"] = ParseNullableDate(item.DT_NASC),
                ["@Naturalidade"] = item.NATURALIDADE ?? (object)DBNull.Value,
                ["@Sexo"] = item.SEXO ?? (object)DBNull.Value,
                ["@EstadoCivil"] = item.EST_CIVIL ?? (object)DBNull.Value,
                ["@TipoLogradouro"] = item.TIPO_LOGRADOURO ?? (object)DBNull.Value,
                ["@Rua"] = item.RUA ?? (object)DBNull.Value,
                ["@NumeroLogradouro"] = item.NUMERO_LOGRADOURO ?? (object)DBNull.Value,
                ["@Complemento"] = item.COMPLEMENTO ?? (object)DBNull.Value,
                ["@Bairro"] = item.BAIRRO ?? (object)DBNull.Value,
                ["@Cidade"] = item.CIDADE ?? (object)DBNull.Value,
                ["@Uf"] = item.UF ?? (object)DBNull.Value,
                ["@Cep"] = item.CEP ?? (object)DBNull.Value,
                ["@DddTelefone"] = item.DDD_TEL ?? (object)DBNull.Value,
                ["@Telefone"] = item.TEL ?? (object)DBNull.Value,
                ["@DddTelefone2"] = item.DDD_TEL_2 ?? (object)DBNull.Value,
                ["@Telefone2"] = item.TEL_2 ?? (object)DBNull.Value,
                ["@DddCelular"] = item.DDD_CEL ?? (object)DBNull.Value,
                ["@Celular"] = item.CELULAR ?? (object)DBNull.Value,
                ["@CodigoPlano"] = ParseNullableLong(item.COD_PLANO),
                ["@DataInclusao"] = ParseNullableDate(item.DT_INCL),
                ["@DataVigencia"] = ParseNullableDate(item.DT_VIGENCIA),
                ["@CodigoEmpresa"] = ParseNullableLong(item.COD_EMPRESA),
                ["@CodigoUnidade"] = ParseNullableLong(item.COD_UNID),
                ["@Matricula"] = item.MAT ?? (object)DBNull.Value,
                ["@Admissao"] = ParseNullableDate(item.ADMISSAO),
                ["@NomeMae"] = item.NOME_MAE ?? (object)DBNull.Value,
                ["@NomePai"] = item.NOME_PAI ?? (object)DBNull.Value,
                ["@Email"] = item.EMAIL ?? (object)DBNull.Value,
                ["@CodigoResponsavel"] = ParseNullableLong(item.COD_RESP),
                ["@Parentesco"] = item.PARENT ?? (object)DBNull.Value,
                ["@Universidade"] = item.UNIVER ?? (object)DBNull.Value,
                ["@NumeroDeclaracaoNascimento"] = item.NR_DEC_NASC_VIVO ?? (object)DBNull.Value,
                ["@Agregado"] = ParseNullableLong(item.AGREGADO),
                ["@DeficienteInvalido"] = ParseNullableLong(item.DEF_INVALIDO),
                ["@CodigoLotacao"] = ParseNullableLong(item.COD_LOTACAO),
                ["@TipoMovimentacao"] = item.TIPO_MOVIMENTACAO ?? (object)DBNull.Value,
                ["@DataExclusao"] = ParseNullableDate(item.DATA_EXCLUSAO),
                ["@MotivoExclusao"] = item.MOTIVO_EXCLUSAO ?? (object)DBNull.Value,
                ["@CodigoOutro"] = ParseNullableLong(item.COD_OUTRO),
                ["@CodigoGseg"] = ParseNullableLong(item.COD_GSEG),
                ["@CodigoVendedor"] = ParseNullableLong(item.COD_VEND),
                ["@CodigoProposta"] = ParseNullableLong(item.COD_PROP),
                ["@Observacao"] = item.OBS ?? (object)DBNull.Value,
                ["@ObservacaoTecnica"] = item.OBS_TEC ?? (object)DBNull.Value,
                ["@MostraLiberacao"] = ParseNullableLong(item.MOSTRA_LIB),
                ["@CodigoForma"] = ParseNullableLong(item.COD_FORMA),
                ["@DiaVencimento"] = ParseNullableLong(item.DIA_VENC),
                ["@CodigoTabelaComissao"] = ParseNullableLong(item.COD_TABCOM),
                ["@Cargo"] = item.CARGO ?? (object)DBNull.Value,
                ["@Responsavel"] = item.RESPONSAVEL ?? (object)DBNull.Value,
                ["@DataCasamento"] = ParseNullableDate(item.DT_CASAMENTO),
                ["@UfOrgao"] = item.UF_ORGAO ?? (object)DBNull.Value,
                ["@CodigoGrupo"] = ParseNullableLong(item.COD_GRP),
                ["@CodigoSupervisor"] = ParseNullableLong(item.COD_SUPERV),
                ["@CodigoProfissao"] = ParseNullableLong(item.COD_PROFISSAO),
                ["@CnpjOperadora"] = item.CNPJ_OPERADORA ?? (object)DBNull.Value,
                ["@CodigoAnsOperadora"] = ParseNullableLong(item.COD_ANS_OPERADORA),
                ["@NomeOperadora"] = item.NOME_OPERADORA ?? (object)DBNull.Value,
                ["@Idade"] = ParseNullableLong(item.IDADE),
                ["@RespfMae"] = item.RESPF_MAE ?? (object)DBNull.Value,
                ["@RespfNascimento"] = ParseNullableDate(item.RESPF_NASCIMENTO),
                ["@RespfEmail"] = item.RESPF_EMAIL ?? (object)DBNull.Value,
                ["@RespfIdade"] = ParseNullableLong(item.RESPF_IDADE),
                ["@RespfSexo"] = item.RESPF_SEXO ?? (object)DBNull.Value,
                ["@RespfEstadoCivil"] = item.RESPF_ESTADOCIVIL ?? (object)DBNull.Value,
                ["@RespfRg"] = item.RESPF_RG ?? (object)DBNull.Value,
                ["@RespfOrgaoExp"] = item.RESPF_ORG_EXP ?? (object)DBNull.Value,
                ["@RespfCns"] = item.RESPF_CNS ?? (object)DBNull.Value,
                ["@NomeVendedor"] = item.NOME_VENDEDOR ?? (object)DBNull.Value,
                ["@CpfVendedor"] = item.CPF_VENDEDOR ?? (object)DBNull.Value,
                ["@EmailVendedor"] = item.EMAIL_VENDEDOR ?? (object)DBNull.Value,
                ["@TelefoneVendedor"] = item.TELEFONE_VENDEDOR ?? (object)DBNull.Value,
                ["@MensalidadeTit"] = ParseNullableDecimal(item.MENSALIDADE_TIT),
                ["@MensalidadeDep"] = ParseNullableDecimal(item.MENSALIDADE_DEP),
                ["@Acessorios"] = item.ACESSORIOS ?? (object)DBNull.Value,
                ["@Informacoes_Log_I"] = DateTime.Now,
                ["@Informacoes_Log_A"] = DBNull.Value
            };

            new Banco_Dados_SQLServer().ExecutarPlennus(sql, parametros);
        }
        private object ParseNullableLong(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DBNull.Value;

            return long.TryParse(value, out var result) ? result : (object)DBNull.Value;
        }

        private object ParseNullableDecimal(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DBNull.Value;

            return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
                ? result
                : (object)DBNull.Value;
        }

        private object ParseNullableDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DBNull.Value;

            return DateTime.TryParse(value, out var result) ? result : (object)DBNull.Value;
        }
    }
}