using Plennusc.Core.Models.ModelsGestao.modelsBilling;
using Plennusc.Core.Models.ModelsGestao.modelsCIDs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.dataCIDs
{
    public class dataActivityReport
    {
        public List<OperadoraModel> BuscarOperadoras()
        {
            var lista = new List<OperadoraModel>();

            string connStr = ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;

            string sql = @"
                SELECT DISTINCT
                    CODIGO_GRUPO_CONTRATO,
                    NOME_OPERADORA
                FROM ESP0002
                WHERE NOME_OPERADORA IS NOT NULL
                AND NUMERO_ANS_OPERADORA IS NOT NULL
                AND CODIGO_GRUPO_CONTRATO IN (54)";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new OperadoraModel
                        {
                            CodigoGrupoContrato = Convert.ToInt32(reader["CODIGO_GRUPO_CONTRATO"]),
                            NomeOperadora = reader["NOME_OPERADORA"].ToString()
                        });
                    }
                }
            }

            return lista;
        }

        public List<ActivityReportModel> BuscarRelatorio(int codigoGrupoContrato, DateTime vigencia)
        {
            var lista = new List<ActivityReportModel>();
            string connStr = ConfigurationManager.ConnectionStrings["Alianca"].ConnectionString;

            string sql = @"
        SELECT
            CASE pl.TIPO_CONTRATO_ESTIPULADO
                WHEN 'E'  THEN 'ESTIPULADO'
                WHEN 'NE' THEN 'NÃO ESTIPULADO'
                ELSE pl.TIPO_CONTRATO_ESTIPULADO
            END                                                                                   AS MODALIDADE,
            p1030.NOME_PLANO_ABREVIADO                                                            AS PLANO,
            ent.NOME_GRUPO_PESSOAS                                                                AS ENTIDADE,
            tit.NOME_ASSOCIADO                                                                    AS TITULAR,
            a.NOME_ASSOCIADO                                                                      AS NOME,
            a.TIPO_ASSOCIADO                                                                      AS TIPO,
            par.NOME_PARENTESCO                                                                   AS PARENTESCO,
            ec.NOME_ESTADO_CIVIL                                                                  AS ESTADO_CIVIL,
            a.SEXO                                                                                AS SEXO,
            a.DATA_NASCIMENTO                                                                     AS DATA_NASC,
            a.NUMERO_CPF                                                                          AS CPF,
            a.CODIGO_CNS                                                                          AS CNS,
            (
                SELECT STRING_AGG(
                    CONVERT(VARCHAR(5), t.CODIGO_AREA)
                    + REPLACE(REPLACE(REPLACE(REPLACE(REPLACE(t.NUMERO_TELEFONE,
                          '-', ''), ' ', ''), '.', ''), '(', ''), ')', '')
                , '; ')
                FROM PS1006 t
                WHERE t.CODIGO_ASSOCIADO = eff.CodAssoc
            )                                                                                     AS TELEFONE,
            CASE WHEN eff.CodEmp = '400' THEN e01.ENDERECO       ELSE e15.ENDERECO       END     AS ENDERECO,
            NULL                                                                                  AS COMPLEMENTO,
            CASE WHEN eff.CodEmp = '400' THEN e01.BAIRRO         ELSE e15.BAIRRO         END     AS BAIRRO,
            CASE WHEN eff.CodEmp = '400' THEN e01.CEP            ELSE e15.CEP            END     AS CEP,
            CASE WHEN eff.CodEmp = '400' THEN e01.CIDADE         ELSE e15.CIDADE         END     AS CIDADE,
            CASE WHEN eff.CodEmp = '400' THEN e01.ESTADO         ELSE e15.ESTADO         END     AS ESTADO,
            a.DATA_ADMISSAO                                                                       AS DT_VIGENCIA,
            CASE WHEN eff.CodEmp = '400' THEN e01.ENDERECO_EMAIL ELSE e15.ENDERECO_EMAIL END     AS EMAIL,
            a.NOME_MAE                                                                            AS FILIACAO_1,
            a.NOME_PAI                                                                            AS FILIACAO_2,
            car.OBSERVACAO_CARENCIA                                                               AS NOMENCLATURA_CARENCIA,
            (
                SELECT STRING_AGG(c.CODIGO_CID + ' - ' + cat.NOME_PATOLOGIA, '; ')
                FROM PS1009 c
                LEFT JOIN PS5201 cat ON c.CODIGO_CID = cat.CODIGO_CID
                WHERE c.CODIGO_ASSOCIADO = a.CODIGO_ASSOCIADO
            )                                                                                     AS CID

        FROM PS1000 a
        LEFT JOIN PS1000 tit    ON a.CODIGO_TITULAR         = tit.CODIGO_ASSOCIADO
        LEFT JOIN PS1014 ent    ON a.CODIGO_GRUPO_PESSOAS   = ent.CODIGO_GRUPO_PESSOAS
        LEFT JOIN PS1045 par    ON a.CODIGO_PARENTESCO      = par.CODIGO_PARENTESCO
        LEFT JOIN PS1044 ec     ON a.CODIGO_ESTADO_CIVIL    = ec.CODIGO_ESTADO_CIVIL
        CROSS APPLY (
            SELECT
                CASE
                    WHEN a.TIPO_ASSOCIADO = 'T' THEN a.CODIGO_ASSOCIADO
                    ELSE COALESCE(
                            NULLIF(a.CODIGO_TITULAR, '0'),
                            NULLIF(a.CODIGO_TITULAR, ''),
                            a.CODIGO_ASSOCIADO)
                END AS CodAssoc,
                CASE
                    WHEN a.TIPO_ASSOCIADO = 'T' THEN a.CODIGO_EMPRESA
                    ELSE COALESCE(tit.CODIGO_EMPRESA, a.CODIGO_EMPRESA)
                END AS CodEmp
        ) eff
        LEFT JOIN PS1001 e01    ON e01.CODIGO_ASSOCIADO     = eff.CodAssoc
        LEFT JOIN PS1015 e15    ON e15.CODIGO_ASSOCIADO     = eff.CodAssoc
        LEFT JOIN PS1007 car    ON car.CODIGO_ASSOCIADO     = a.CODIGO_ASSOCIADO
        OUTER APPLY (
            SELECT TOP 1 p.*
            FROM PS1032 p
            WHERE a.CODIGO_PLANO           = p.CODIGO_PLANO
              AND a.CODIGO_TABELA_PRECO    = p.CODIGO_TABELA_PRECO
              AND a.CODIGO_GRUPO_CONTRATO  = p.CODIGO_GRUPO_CONTRATO
            ORDER BY
                CASE WHEN DATEDIFF(YEAR, a.DATA_NASCIMENTO, GETDATE())
                          BETWEEN p.IDADE_MINIMA AND p.IDADE_MAXIMA
                     THEN 0 ELSE 1 END,
                p.IDADE_MINIMA
        ) pl
        OUTER APPLY (
            SELECT TOP 1 p.NOME_PLANO_ABREVIADO
            FROM PS1030 p
            WHERE p.CODIGO_PLANO = a.CODIGO_PLANO
            ORDER BY p.DATA_CADASTRAMENTO DESC
        ) p1030

        WHERE a.CODIGO_GRUPO_CONTRATO = @CodigoGrupoContrato
          AND a.DATA_EXCLUSAO IS NULL
          AND CAST(a.DATA_ADMISSAO AS DATE) = @Vigencia

        ORDER BY
            a.CODIGO_TITULAR,
            CASE WHEN a.TIPO_ASSOCIADO = 'T' THEN 0 ELSE 1 END,
            a.NOME_ASSOCIADO
    ";

            using (SqlConnection conn = new SqlConnection(connStr))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add("@CodigoGrupoContrato", SqlDbType.Int).Value = codigoGrupoContrato;
                cmd.Parameters.Add("@Vigencia", SqlDbType.Date).Value = vigencia.Date;
                cmd.CommandTimeout = 120;

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new ActivityReportModel
                        {
                            Modalidade = reader["MODALIDADE"] as string,
                            Plano = reader["PLANO"]?.ToString(),
                            Entidade = reader["ENTIDADE"] as string,
                            Titular = reader["TITULAR"] as string,
                            Nome = reader["NOME"] as string,
                            Tipo = reader["TIPO"] as string,
                            Parentesco = reader["PARENTESCO"] as string,
                            EstadoCivil = reader["ESTADO_CIVIL"] as string,
                            Sexo = reader["SEXO"] as string,
                            DataNascimento = reader["DATA_NASC"] as DateTime?,
                            Cpf = reader["CPF"] as string,
                            Cns = reader["CNS"] as string,
                            Telefone = reader["TELEFONE"] as string,
                            Endereco = reader["ENDERECO"] as string,
                            Complemento = reader["COMPLEMENTO"] as string,
                            Bairro = reader["BAIRRO"] as string,
                            Cep = reader["CEP"] as string,
                            Cidade = reader["CIDADE"] as string,
                            Estado = reader["ESTADO"] as string,
                            DtVigencia = reader["DT_VIGENCIA"] as DateTime?,
                            Email = reader["EMAIL"] as string,
                            Filiacao1 = reader["FILIACAO_1"] as string,
                            Filiacao2 = reader["FILIACAO_2"] as string,
                            NomenclaturaCarencia = reader["NOMENCLATURA_CARENCIA"] as string,
                            Cid = reader["CID"] as string
                        });
                    }
                }
            }

            return lista;
        }
    }
}