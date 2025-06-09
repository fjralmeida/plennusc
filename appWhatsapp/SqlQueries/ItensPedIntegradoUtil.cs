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
        public DataTable ConsultaAssociados()
        {
            string sql = @"
           SELECT TOP 10
                A.CODIGO_ASSOCIADO, 
                A.NOME_ASSOCIADO,   
                C.NOME_PLANO_ABREVIADO, 
                B.DATA_VENCIMENTO, 
                B.VALOR_CONVENIO 
            FROM PS1000 A
            JOIN PS1020 B ON A.CODIGO_ASSOCIADO = B.CODIGO_ASSOCIADO
            JOIN PS1030 C ON A.CODIGO_PLANO = C.CODIGO_PLANO;
        ";

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer(); // seu objeto de conexão com o banco
            return db.LerAlianca(sql);
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
                                E.Conf_LiberaAcesso
                            FROM AutenticacaoAcesso AA
                            INNER JOIN SistemaEmpresaUsuario SEU ON SEU.CodAutenticacaoAcesso = AA.CodAutenticacaoAcesso
                            INNER JOIN SistemaEmpresa SE ON SE.CodSistemaEmpresa = SEU.CodSistemaEmpresa
                            INNER JOIN Empresa E ON E.CodEmpresa = SE.CodEmpresa
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

        public DataTable ConsultaInfoEmpresa(int codEmpresa)
        {
            string sql = @"
                            SELECT NomeFantasia, Conf_Logo
                            FROM Empresa
                            WHERE CodEmpresa = @CodEmpresa
                        ";

                                var parametros = new Dictionary<string, object>
                        {
                            { "@CodEmpresa", codEmpresa }
                        };

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(sql, parametros);
        }
    }
}