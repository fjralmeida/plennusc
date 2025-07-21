using appWhatsapp.Data_Bd;
using appWhatsapp.Models.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace appWhatsapp.SqlQueries
{
    public class ItensPedIntegradoUtil
    {
        public DataTable ConsultaAssociados(DateTime? dataIni = null, DateTime? dataFim = null, int? codigoOperadora = null)
        {
            if (!dataIni.HasValue || !dataFim.HasValue)
            {
                // ATENÇÃO: você precisa garantir que esse caminho não traga tudo
                return new DataTable(); // ← não retorna nada
            }

            string sql = @"
                            SELECT 
                                A.NUMERO_REGISTRO, 
                                A.CODIGO_ASSOCIADO, 
                                B.NOME_ASSOCIADO,
                                A.DATA_VENCIMENTO, 
                                A.VALOR_FATURA,
                                C.NOME_OPERADORA,
                                D.NOME_PLANO_ABREVIADO,
                                T.NUMERO_TELEFONE
                            FROM PS1020 A
                            LEFT JOIN PS1000 B ON A.CODIGO_ASSOCIADO = B.CODIGO_ASSOCIADO
                            LEFT JOIN ESP0002 C ON B.CODIGO_GRUPO_CONTRATO = C.CODIGO_GRUPO_CONTRATO
                            LEFT JOIN PS1030 D ON D.CODIGO_PLANO = B.CODIGO_PLANO
                            OUTER APPLY (
                                SELECT TOP 1 NUMERO_TELEFONE 
                                FROM PS1006 
                                WHERE PS1006.CODIGO_ASSOCIADO = A.CODIGO_ASSOCIADO
                            ) T
                            WHERE 
                                A.DATA_VENCIMENTO BETWEEN @DataIni AND @DataFim
                                AND A.DATA_PAGAMENTO IS NULL
                                AND (VALOR_PAGO <> '0.00' OR VALOR_PAGO IS NOT NULL)
                                AND A.DATA_CANCELAMENTO IS NULL
                                AND A.CODIGO_EMPRESA = 400
                                AND B.CODIGO_MOTIVO_EXCLUSAO IS NULL
                                AND B.DATA_EXCLUSAO IS NULL
                                AND (@CodigoOperadora IS NULL OR C.CODIGO_GRUPO_CONTRATO = @CodigoOperadora)
                        ";

            var parametros = new Dictionary<string, object>
            {
                ["@DataIni"] = dataIni.Value.Date,
                ["@DataFim"] = dataFim.Value.Date,
                ["@CodigoOperadora"] = (object)codigoOperadora ?? DBNull.Value
            };

            return new Banco_Dados_SQLServer().LerAlianca(sql, parametros);
        }


        public DataTable ConsultaLoginComEmpresa(string login, string senha, string codSistema)
        {
            string senhaHash = CriptografiaUtil.CalcularHashSHA512(senha);

            string sql = @"
                           SELECT 
            AA.CodAutenticacaoAcesso,
            AA.CodPessoa,
            AA.NomeUsuario,
            AA.UsrNomeLogin,
            AA.Conf_Ativo AS UsuarioAtivo,
            SE.CodEmpresa,
            E.RazaoSocial,
            E.NomeFantasia,
            E.Conf_Ativo AS EmpresaAtiva,
            E.Conf_LiberaAcesso,
            SE.Conf_LiberaAcesso AS LiberacaoVinculoSistemaEmpresa,
            SEU.Conf_LiberaAcesso AS LiberacaoUsuarioSistema,
            SEU.Conf_BloqueiaAcesso AS BloqueioUsuarioSistema, 
            SI.CodSistema,
            SI.Nome AS NomeSistema,
            SI.NomeDisplay AS NomeDisplaySistema
        FROM AutenticacaoAcesso AA
        INNER JOIN SistemaEmpresaUsuario SEU 
            ON SEU.CodAutenticacaoAcesso = AA.CodAutenticacaoAcesso
        INNER JOIN SistemaEmpresa SE 
            ON SE.CodSistemaEmpresa = SEU.CodSistemaEmpresa
        INNER JOIN Empresa E 
            ON E.CodEmpresa = SE.CodEmpresa
        INNER JOIN Sistema SI 
            ON SI.CodSistema = SE.CodSistema
        WHERE AA.UsrNomeLogin = @login
          AND AA.UsrPasswd = @senhaHash
    ";

            var parametros = new Dictionary<string, object>
                        {
                            { "@login", login },
                            { "@senhaHash", senhaHash },
                            { "@CodSistema", codSistema }
                        };

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(sql, parametros);
        }



        public DataTable ConsultaInfoPerfil()
        {
            string sql = @"
                            SELECT Nome, Conf_Simbolo
                            FROM Sistema
                            WHERE CodSistema = 1
                        ";

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(sql);
        }

        public DataTable ConsultaInfoEmpresa(int CodSistema)
        {
            string sql = @"
                            SELECT NomeDisplay, Conf_Logo
                            FROM Sistema
                            WHERE CodSistema = @CodSistema
                        ";

                                var parametros = new Dictionary<string, object>
                        {
                            { "@CodSistema", CodSistema }
                        };

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(sql, parametros);
        }
    }
}