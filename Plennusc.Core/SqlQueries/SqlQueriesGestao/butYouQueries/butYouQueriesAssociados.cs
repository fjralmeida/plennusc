using Plennusc.Core.Models.ModelsGestao.modelsButYou;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.butYouQueries
{
    public class butYouQueriesAssociados
    {
        private string _connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;

        public butYouQueriesAssociados(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<DadosAssociadoCompleto> BuscarAssociadosCompletos()
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
                     PS1032.VALOR_PLANO

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
                     AND P0.CODIGO_MOTIVO_EXCLUSAO IS NULL
                     AND P0.NUMERO_CPF IN ('08854420409',
                     '13025299436',
                     '07729534495',
                     '03433303428',
                     '56336284468'
                     )
                     OR P0.CODIGO_TITULAR IN (SELECT CODIGO_ASSOCIADO FROM PS1000 WHERE PS1000.NUMERO_CPF IN ('08854420409',
                     '13025299436',
                     '07729534495',
                     '03433303428',
                     '56336284468'))
                 ORDER BY P0.CODIGO_ASSOCIADO;";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var associado = new DadosAssociadoCompleto
                        {
                            CodigoAssociado = reader["CODIGO_ASSOCIADO"]?.ToString() ?? "",
                            // Exemplo de mapeamento no leitor de dados
                            DiaVencimentoPadrao = reader["DIA_VENCIMENTO_PADRAO"] != DBNull.Value ? (int)reader["DIA_VENCIMENTO_PADRAO"] : (int?)null,
                            NomeCompleto = reader["NOME_COMPLETO"]?.ToString() ?? "",
                            CpfTitular = FormatarCpf(reader["NUMERO_CPF"]?.ToString() ?? ""),
                            DataNascimento = reader["DATA_NASCIMENTO"] is DateTime dataNasc ?
         dataNasc.ToString("dd/MM/yyyy") : "",
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
                            TelefoneCelular = reader["TELEFONE_CELULAR"]?.ToString() ?? ""
                        };

                        associados.Add(associado);
                    }
                }
            }

            return associados;
        }

        private string FormatarCpf(string cpf)
        {
            if (string.IsNullOrEmpty(cpf) || cpf.Length != 11)
                return cpf;

            return $"{cpf.Substring(0, 3)}.{cpf.Substring(3, 3)}.{cpf.Substring(6, 3)}-{cpf.Substring(9, 2)}";
        }

        private string ConverterEstadoCivil(string codigo)
        {
            switch (codigo)
            {
                case "2": return "SOLTEIRO";
                case "3": return "CASADO";
                case "5": return "DIVORCIADO";
                case "8": return "VIÚVO";
                case "4": return "DESQUITDO";
                case "6": return "MARITAL";
                case "7": return "NÃO INFORMADO";
                default: return "OUTROS";
            }
        }


        public void InserirRegistroProposta(
         DadosAssociadoCompleto associado,
         string tipoAssociado,
         string nomeArquivo)
        {
            const string sql = @"
        INSERT INTO TMP_Send_Proposta_ClickSign
            (Numero_CPF, Nome_Associado, Tipo_Associado, Data_Nascimento,
             Endereco_Email, Numero_Telefone, Cidade, Estado, Nome_Arquivo_Proposta)
        VALUES
            (@cpf, @nome, @tipo, @dataNasc, @email, @telefone, @cidade, @estado, @arquivo)";

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand(sql, conn))
            {
                // CPF
                cmd.Parameters.AddWithValue("@cpf", (object)associado.CpfTitular ?? DBNull.Value);

                // Nome
                cmd.Parameters.AddWithValue("@nome", (object)associado.NomeCompleto ?? DBNull.Value);

                // Tipo
                cmd.Parameters.AddWithValue("@tipo", (object)tipoAssociado ?? DBNull.Value);

                // Data de Nascimento
                DateTime? dataNasc = null;
                if (!string.IsNullOrWhiteSpace(associado.DataNascimento))
                {
                    if (DateTime.TryParse(associado.DataNascimento, out DateTime dn))
                        dataNasc = dn;
                }
                cmd.Parameters.AddWithValue("@dataNasc", (object)dataNasc ?? DBNull.Value);

                // Email
                cmd.Parameters.AddWithValue("@email", (object)associado.Email ?? DBNull.Value);

                // Telefone (pode ser concatenado, sem problema)
                cmd.Parameters.AddWithValue("@telefone", (object)associado.TelefoneCelular ?? DBNull.Value);

                // Cidade
                cmd.Parameters.AddWithValue("@cidade", (object)associado.Cidade ?? DBNull.Value);

                // Estado (UF)
                cmd.Parameters.AddWithValue("@estado", (object)associado.Uf ?? DBNull.Value);

                // Nome do arquivo
                cmd.Parameters.AddWithValue("@arquivo", (object)nomeArquivo ?? DBNull.Value);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}