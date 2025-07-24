using appWhatsapp.Data_Bd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesMedic.settingsMedic
{
    public class PrivacySettingsMedic
    {
        public DataTable GetSettingsLogin(int codPessoa)
        {
            string sql = $@"
                            SELECT UsrNomeLogin,CodAutenticacaoAcesso
                            FROM AutenticacaoAcesso
                            WHERE CodPessoa = {codPessoa}
                              AND Conf_Ativo = 1
                              AND Conf_PermiteAcesso = 1
                        ";

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(sql);
        }
        public void UpdateUserLogin(int codAutenticacaoAcesso, string novoLogin)
        {
            string sql = @"
                        UPDATE AutenticacaoAcesso
                        SET UsrNomeLogin = @novoLogin,
                            Informacoes_log_a = GETDATE()
                        WHERE CodAutenticacaoAcesso = @cod";

            var parametros = new Dictionary<string, object>
                    {
                        { "@novoLogin", novoLogin },
                        { "@cod", codAutenticacaoAcesso }
                    };

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            db.ExecutarPlennus(sql, parametros);
        }

        public DataTable GetSettingsPassword(int codPessoa)
        {
            string sql = $@"
                            SELECT UsrPasswd,CodAutenticacaoAcesso
                            FROM AutenticacaoAcesso
                            WHERE CodPessoa = {codPessoa}
                              AND Conf_Ativo = 1
                              AND Conf_PermiteAcesso = 1
                        ";

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            return db.LerPlennus(sql);
        }
        public void UpdateUserPassword(int authAccessId, string newPasswordHash)
        {
            string sql = @"
                            UPDATE AutenticacaoAcesso
                            SET 
                                UsrPasswd = @newPassword,
                                Informacoes_log_a = GETDATE()
                            WHERE 
                                CodAutenticacaoAcesso = @authId";

            var parameters = new Dictionary<string, object>
                        {
                            { "@newPassword", newPasswordHash },
                            { "@authId", authAccessId }
                        };

            Banco_Dados_SQLServer db = new Banco_Dados_SQLServer();
            db.ExecutarPlennus(sql, parameters);
        }
    }
}
