using Plennusc.Core.Models.ModelsGestao.modelsButYou;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.butYouQueries
{
    public class enterpriseButYouQueries
    {
        private readonly string _connectionString;

        public enterpriseButYouQueries(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<DadosAssociadoCompleto> BuscarAssociadosPorEmpresa(int codigoEmpresa)
        {
            var associados = new List<DadosAssociadoCompleto>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = @"

                       SELECT 
                 P0.CODIGO_ASSOCIADO, 
                 REPLACE(P0.NOME_ASSOCIADO, '#', '') AS NOME_COMPLETO, 
                 P0.DATA_ADMISSAO,
                 P0.NUMERO_CPF, 
                 P0.DATA_NASCIMENTO, 
                 P0.SEXO, 
                 P0.CODIGO_ESTADO_CIVIL,
                 P0.NUMERO_RG, 
                 P0.ORGAO_EMISSOR_RG, 
                 P0.CODIGO_CNS, 
                 P0.NUMERO_DECLARACAO_NASC_VIVO, 
                 P0.CODIGO_TITULAR,
                 P0.CODIGO_PLANO,
                 P0.TIPO_ASSOCIADO,
                 P0.DATA_EXCLUSAO,
                 P0.CODIGO_MOTIVO_EXCLUSAO,
                 FLOOR(DATEDIFF(DAY, P0.DATA_NASCIMENTO, GETDATE()) / 365.25) AS IDADE,
                 PS1051.DESCRICAO_GP_FATURAMENTO,
                 PS1051.DIA_VENCIMENTO_PADRAO,
    
                 PS1045.NOME_PARENTESCO,

                 P1.ENDERECO,
                 NULL AS NUMERO,
                 NULL AS COMPLEMENTO,
                 P1.CEP,
                 P1.BAIRRO,
                 P1.CIDADE,
                 P1.ESTADO AS UF,
    
                 ISNULL(P1.ENDERECO_EMAIL, P1.ENDERECO_ELETRONICO) AS EMAIL,

                 STUFF((
                     SELECT DISTINCT ', ' + T.NUMERO_TELEFONE
                     FROM PS1006 T
                     WHERE T.CODIGO_ASSOCIADO = P0.CODIGO_ASSOCIADO
                       AND T.NUMERO_TELEFONE IS NOT NULL
                     FOR XML PATH('')
                 ), 1, 2, '') AS TELEFONE_CELULAR,

                 ESP0002.NOME_OPERADORA,
                 ESP0002.NOME_GRUPO_CONTRATO,

                 PS1030.NOME_PLANO_ABREVIADO,
                 PS1032.NOME_TABELA, 
                 PS1032.VALOR_PLANO,
                 P0.CODIGO_EMPRESA

             FROM PS1000 P0

             LEFT JOIN PS1001 P1 
                 ON P1.CODIGO_ASSOCIADO = P0.CODIGO_ASSOCIADO

             LEFT JOIN PS1045 
                 ON PS1045.CODIGO_PARENTESCO = P0.CODIGO_PARENTESCO

             LEFT JOIN ESP0002 
                 ON ESP0002.CODIGO_GRUPO_CONTRATO = P0.CODIGO_GRUPO_CONTRATO

             LEFT JOIN PS1030 
                 ON PS1030.CODIGO_PLANO = P0.CODIGO_PLANO 

             LEFT JOIN VW_FILTRO_BENEFICIARIOS_IDADE V 
                 ON V.CODIGO_ASSOCIADO = P0.CODIGO_ASSOCIADO

             LEFT JOIN PS1032 
                 ON PS1032.CODIGO_TABELA_PRECO = P0.CODIGO_TABELA_PRECO
                 AND V.IDADE BETWEEN PS1032.IDADE_MINIMA AND PS1032.IDADE_MAXIMA

            LEFT JOIN PS1051 
                ON PS1051.CODIGO_GRUPO_FATURAMENTO = P0.CODIGO_GRUPO_FATURAMENTO

             WHERE 
                 P0.CODIGO_GRUPO_CONTRATO = 2 
                 AND P0.DATA_EXCLUSAO IS NULL 
                 AND P0.CODIGO_SITUACAO_ATENDIMENTO = 2
                 AND P0.CODIGO_MOTIVO_EXCLUSAO IS NULL
                 AND P0.NUMERO_CPF IN (
                '00035820438'


            )
            AND P0.CODIGO_EMPRESA = 3150

            OR P0.CODIGO_TITULAR IN (
                SELECT CODIGO_ASSOCIADO 
                FROM PS1000 
                WHERE 
                DATA_EXCLUSAO IS NULL 
                AND CODIGO_MOTIVO_EXCLUSAO IS NULL
                AND NUMERO_CPF IN (
            '00035820438'

            )
            )
             ORDER BY P0.CODIGO_ASSOCIADO;

";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@codigoEmpresa", codigoEmpresa);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var associado = new DadosAssociadoCompleto
                            {
                                CodigoAssociado = reader["CODIGO_ASSOCIADO"]?.ToString() ?? "",
                                DiaVencimentoPadrao = reader["DIA_VENCIMENTO_PADRAO"] != DBNull.Value ? (int)reader["DIA_VENCIMENTO_PADRAO"] : (int?)null,
                                NomeCompleto = reader["NOME_COMPLETO"]?.ToString() ?? "",
                                CpfTitular = Formatador.FormatarCpf(reader["NUMERO_CPF"]?.ToString() ?? ""),
                                DataNascimento = reader["DATA_NASCIMENTO"] is DateTime dataNasc ? dataNasc.ToString("dd/MM/yyyy") : "",
                                Sexo = reader["SEXO"]?.ToString() ?? "",
                                EstadoCivil = ConverterEstadoCivil(reader["CODIGO_ESTADO_CIVIL"]?.ToString() ?? ""),
                                Rg = reader["NUMERO_RG"]?.ToString() ?? "",
                                OrgaoExpedidor = reader["ORGAO_EMISSOR_RG"]?.ToString() ?? "",
                                CartaoSus = reader["CODIGO_CNS"]?.ToString() ?? "",
                                NumeroDeclaracaoNascidoVivo = reader["NUMERO_DECLARACAO_NASC_VIVO"]?.ToString() ?? "",
                                CodigoTitular = reader["CODIGO_TITULAR"]?.ToString() ?? "",
                                TipoAssociado = reader["TIPO_ASSOCIADO"]?.ToString() ?? "",
                                Idade = reader["IDADE"]?.ToString() ?? "",
                                Endereco = reader["ENDERECO"]?.ToString() ?? "",
                                Numero = reader["NUMERO"]?.ToString() ?? "",
                                Complemento = reader["COMPLEMENTO"]?.ToString() ?? "",
                                Cep = reader["CEP"]?.ToString() ?? "",
                                Bairro = reader["BAIRRO"]?.ToString() ?? "",
                                Cidade = reader["CIDADE"]?.ToString() ?? "",
                                Uf = reader["UF"]?.ToString() ?? "",
                                Email = reader["EMAIL"]?.ToString() ?? "",
                                ValorPlano = reader["VALOR_PLANO"] != DBNull.Value ? Convert.ToDecimal(reader["VALOR_PLANO"]) : (decimal?)null,
                                TelefoneCelular = reader["TELEFONE_CELULAR"]?.ToString() ?? "",
                                NomeParentesco = reader["NOME_PARENTESCO"]?.ToString() ?? ""
                            };
                            associados.Add(associado);
                        }
                    }
                }
            }

            return associados;
        }

        public List<DadosEmpresaCompleto> BuscarDadosEmpresa()
        {
            var empresas = new List<DadosEmpresaCompleto>();

            string query = @"
             
SELECT 
    e.CODIGO_EMPRESA,
    e.NOME_EMPRESA AS RAZAO_SOCIAL,
    COALESCE(e.NOME_USUAL_FANTASIA, e.NOME_EMPRESA) AS NOME_FANTASIA,
    e.NUMERO_CNPJ AS CNPJ,
    e.NUMERO_INSC_ESTADUAL AS INSCRICAO_ESTADUAL,
    e.NUMERO_INSC_MUNICIPAL AS INSCRICAO_MUNICIPAL,
    e.DATA_ADMISSAO AS DATA_INICIO_VIGENCIA,
    en.ENDERECO AS ENDERECO_EMPRESA,
    en.BAIRRO AS BAIRRO_EMPRESA,
    en.CIDADE AS CIDADE_EMPRESA,
    en.CEP AS CEP_EMPRESA,
    en.ESTADO AS ESTADO_EMPRESA,
    en.ENDERECO_EMAIL AS EMAIL_EMPRESA
FROM PS1010 e
LEFT JOIN PS1001 en 
    ON e.CODIGO_EMPRESA = en.CODIGO_EMPRESA 
    AND (en.FLAG_COBRANCA = 'S' OR en.FLAG_COBRANCA IS NULL)
WHERE e.CODIGO_EMPRESA IN (3150)
    AND e.CODIGO_SITUACAO_ATENDIMENTO = 2
    AND e.DATA_EXCLUSAO IS NULL;

";

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var empresa = new DadosEmpresaCompleto
                        {
                            CodigoEmpresa = reader["CODIGO_EMPRESA"]?.ToString() ?? "",
                            RazaoSocial = reader["RAZAO_SOCIAL"]?.ToString() ?? "",
                            NomeFantasia = reader["NOME_FANTASIA"]?.ToString() ?? "",
                            Cnpj = Formatador.FormatarCnpj(reader["CNPJ"]?.ToString() ?? ""),
                            InscricaoEstadual = reader["INSCRICAO_ESTADUAL"]?.ToString() ?? "",
                            InscricaoMunicipal = reader["INSCRICAO_MUNICIPAL"]?.ToString() ?? "",
                            DataInicioVigencia = reader["DATA_INICIO_VIGENCIA"] as DateTime?,
                            Endereco = reader["ENDERECO_EMPRESA"]?.ToString() ?? "",
                            Bairro = reader["BAIRRO_EMPRESA"]?.ToString() ?? "",
                            Cidade = reader["CIDADE_EMPRESA"]?.ToString() ?? "",
                            Cep = reader["CEP_EMPRESA"]?.ToString() ?? "",
                            Estado = reader["ESTADO_EMPRESA"]?.ToString() ?? "",
                            Email = reader["EMAIL_EMPRESA"]?.ToString() ?? ""
                        };
                        empresas.Add(empresa);
                    }
                }
            }

            return empresas;
        }

        private string ConverterEstadoCivil(string codigo)
        {
            switch (codigo)
            {
                case "2": return "SOLTEIRO";
                case "3": return "CASADO";
                case "5": return "DIVORCIADO";
                case "8": return "VIÚVO";
                case "4": return "DESQUITADO";
                case "6": return "MARITAL";
                case "7": return "NÃO INFORMADO";
                default: return "OUTROS";
            }
        }
    }

    public static class Formatador
    {
        public static string FormatarCnpj(string cnpj)
        {
            if (string.IsNullOrEmpty(cnpj) || cnpj.Length != 14)
                return cnpj;

            return $"{cnpj.Substring(0, 2)}.{cnpj.Substring(2, 3)}.{cnpj.Substring(5, 3)}/{cnpj.Substring(8, 4)}-{cnpj.Substring(12, 2)}";
        }

        public static string FormatarCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11)
                return cpf;
            return Convert.ToUInt64(cpf).ToString(@"000\.000\.000\-00");
        }
    }
}