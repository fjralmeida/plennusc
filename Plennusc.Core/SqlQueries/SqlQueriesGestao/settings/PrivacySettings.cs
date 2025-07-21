using appWhatsapp.Data_Bd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.settings
{
    public class PrivacySettings
    {
        public DataTable GetSettingsLogin(int codPessoa)
        {
            string sql = $@"
                            SELECT UsrNomeLogin
                            FROM AutenticacaoAcesso
                            WHERE CodPessoa = {codPessoa}
                              AND Conf_Ativo = 1
                              AND Conf_PermiteAcesso = 1
                        ";

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(sql);
        }
        public DataTable GetSettingsPassword()
        {
            string sql = @"
                            
                        ";

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(sql);
        }
    }
}
