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

        public DataTable ConsultaLoginUsuario(string login, string senha)
        {
            // Calcular o hash SHA-512 da senha
            string senhaHash = CriptografiaUtil.CalcularHashSHA512(senha);


            string sql = $@"
                            SELECT *
                            FROM AutenticacaoAcesso
                            WHERE UsrNomeLogin = '{login}' 
                              AND UsrPasswd = '{senhaHash}'
                              AND Conf_Ativo = 1 
                              AND Conf_PermiteAcesso = 1";

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(sql);
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
    }
}