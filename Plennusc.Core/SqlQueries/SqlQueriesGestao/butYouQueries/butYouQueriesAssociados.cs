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
                P0.NUMERO_CPF, 
                P0.DATA_NASCIMENTO, 
                P0.SEXO, 
                P0.CODIGO_ESTADO_CIVIL,
                P0.NUMERO_RG, 
                P0.ORGAO_EMISSOR_RG, 
                P0.CODIGO_CNS, 
                P0.NUMERO_DECLARACAO_NASC_VIVO, 
                FLOOR(DATEDIFF(DAY, P0.DATA_NASCIMENTO, GETDATE()) / 365.25) AS IDADE,
                P1.ENDERECO,
                NULL AS NUMERO,
                NULL AS COMPLEMENTO,
                P1.CEP,
                P1.BAIRRO,
                P1.CIDADE,
                P1.ESTADO AS UF,
                ISNULL(P1.ENDERECO_EMAIL, P1.ENDERECO_ELETRONICO) AS EMAIL,
                (SELECT TOP 1 NUMERO_TELEFONE 
                 FROM PS1006 
                 WHERE CODIGO_ASSOCIADO = P0.CODIGO_ASSOCIADO 
                 ORDER BY NUMERO_TELEFONE) AS TELEFONE_CELULAR
            FROM PS1000 P0
            LEFT JOIN PS1001 P1 ON P0.CODIGO_ASSOCIADO = P1.CODIGO_ASSOCIADO
            WHERE P0.CODIGO_GRUPO_CONTRATO = 2 
                AND P0.DATA_EXCLUSAO IS NULL 
                AND P0.CODIGO_MOTIVO_EXCLUSAO IS NULL
            ORDER BY P0.CODIGO_ASSOCIADO";

                using (var command = new SqlCommand(query, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var associado = new DadosAssociadoCompleto
                        {
                            CodigoAssociado = reader["CODIGO_ASSOCIADO"]?.ToString() ?? "",
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
                            Idade = reader["IDADE"]?.ToString() ?? "",
                            Endereco = reader["ENDERECO"]?.ToString() ?? "",
                            Numero = reader["NUMERO"]?.ToString() ?? "",
                            Complemento = reader["COMPLEMENTO"]?.ToString() ?? "",
                            Cep = reader["CEP"]?.ToString() ?? "",
                            Bairro = reader["BAIRRO"]?.ToString() ?? "",
                            Cidade = reader["CIDADE"]?.ToString() ?? "",
                            Uf = reader["UF"]?.ToString() ?? "",
                            Email = reader["EMAIL"]?.ToString() ?? "",
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
    }
}