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
        public DataTable ConsultaAssociados(DateTime? dataIni = null, DateTime? dataFim = null)
        {
            string sql = @"
                            SELECT {0}
                                   A.CODIGO_ASSOCIADO,
                                   A.NOME_ASSOCIADO,
                                   C.NOME_PLANO_ABREVIADO,
                                   B.DATA_VENCIMENTO,
                                   B.VALOR_FATURA,
                                   B.NUMERO_REGISTRO,
                                   T.NUMERO_TELEFONE
                            FROM PS1000 A
                            JOIN PS1020 B ON A.CODIGO_ASSOCIADO = B.CODIGO_ASSOCIADO
                            JOIN PS1030 C ON A.CODIGO_PLANO = C.CODIGO_PLANO
                            LEFT JOIN PS1006 T ON A.CODIGO_ASSOCIADO = T.CODIGO_ASSOCIADO
                        ";

            var parametros = new Dictionary<string, object>();
            string topClause = "";

            if (dataIni.HasValue && dataFim.HasValue)
            {
                sql += @"
            WHERE B.DATA_VENCIMENTO >= @DataIni
              AND B.DATA_VENCIMENTO < @DataFimPlusOne";

                parametros.Add("@DataIni", dataIni.Value.Date);
                parametros.Add("@DataFimPlusOne", dataFim.Value.Date.AddDays(1));
            }
            else
            {
                topClause = "TOP 300"; // Limita só quando não há filtro de datas
            }

            // Insere o TOP na query
            sql = string.Format(sql, topClause);

            var db = new Banco_Dados_SQLServer();
            return db.LerAlianca(sql, parametros);
        }


        public DataTable ConsultaLoginComEmpresa(string login, string senha)
        {
            string senhaHash = CriptografiaUtil.CalcularHashSHA512(senha);

            string sql = @"
                            SELECT 
                                    AA.CodAutenticacaoAcesso,
                                    AA.NomeUsuario,
                                    AA.UsrNomeLogin,
                                    AA.Conf_Ativo AS UsuarioAtivo,
                                    SE.CodEmpresa,
                                    E.RazaoSocial,
                                    E.NomeFantasia,
                                    E.Conf_Ativo AS EmpresaAtiva,
                                    E.Conf_LiberaAcesso,
                                    SI.CodSistema
                                    FROM AutenticacaoAcesso AA
                                    INNER JOIN SistemaEmpresaUsuario SEU ON SEU.CodAutenticacaoAcesso = AA.CodAutenticacaoAcesso
                                    INNER JOIN SistemaEmpresa SE ON SE.CodSistemaEmpresa = SEU.CodSistemaEmpresa
                                    INNER JOIN Empresa E ON E.CodEmpresa = SE.CodEmpresa
                                    INNER JOIN Sistema SI ON SI.CodSistema = SE.CodSistema  -- JOIN com a tabela Sistema
                                    WHERE AA.UsrNomeLogin = @login
                                    AND AA.UsrPasswd = @senhaHash";

                                var parametros = new Dictionary<string, object>
                        {
                            { "@login", login },
                            { "@senhaHash", senhaHash }
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