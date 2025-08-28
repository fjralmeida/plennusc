using appWhatsapp.Data_Bd;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.SqlQueries.SqlQueriesGestao.profile
{
    public class AutenticacaoDAO
    {
        /// <summary>
        /// 3.1) Retorna o login do usuário atrelado à pessoa (ou null se não existir)
        /// </summary>

        public DataRow ObterLoginPorPessoa(int codPessoa)
        {
            string sql = @"
                SELECT TOP 1 *
                FROM AutenticacaoAcesso
                WHERE CodPessoa = @CodPessoa
                ORDER BY CodAutenticacaoAcesso DESC;
            ";

            var pars = new Dictionary<string, object> { { "@CodPessoa", codPessoa } };
            var db = new Banco_Dados_SQLServer();
            var dt = db.LerPlennus(sql, pars);
            return (dt != null && dt.Rows.Count > 0) ? dt.Rows[0] : null;
        }

        /// <summary>
        /// 3.2) Verifica se já existe alguém usando este nome de login
        /// </summary>

        public bool LoginExiste(string usrNomeLogin)
        {
            string sql = @"SELECT 1 FROM AutenticacaoAcesso WHERE UsrNomeLogin = @usr";

            var pars = new Dictionary<string, object> { { "@usr", usrNomeLogin } };
            var db = new Banco_Dados_SQLServer();
            var dt = db.LerPlennus(sql, pars);
            return dt != null && dt.Rows.Count > 0;
        }

        /// <summary>
        /// 3.2) Sugere um login disponível a partir de uma base. 
        /// Ex.: "joao.silva", vira "joao.silva2" se já existir, e assim por diante.
        /// </summary>

        public string SugerirLoginDisponivel(string baseLogin)
        {
            baseLogin = (baseLogin ?? "").Trim().ToLowerInvariant();
            if (string.IsNullOrEmpty(baseLogin)) baseLogin = "usuario";

            // livre? usa direto
            if (!LoginExiste(baseLogin)) return baseLogin;

            // tenta baseLogin1..baseLogin999
            for (int i = 1; i <= 999; i++)
            {
                string tentativa = $"{baseLogin}{i}";
                if (!LoginExiste(tentativa))
                    return tentativa;
            }

            // fallback improvável
            return baseLogin + DateTime.Now.Ticks.ToString().Substring(10);
        }

        public int InserirLogin( int codPerfilUsuario, string nomeUsuario, string sobrenomeUsuario, string usrNomeLogin,string usrPasswdHash,int confPermiteAcesso, int confAtivo, int confRestrito, int confMaster, int codPessoa)
        {
            string sql = @"
                INSERT INTO AutenticacaoAcesso
                (
                    CodPerfilUsuario,
                    NomeUsuario,
                    SobrenomeUsuario,
                    UsrNomeLogin,
                    UsrPasswd,
                    Conf_PermiteAcesso,
                    Conf_Ativo,
                    Conf_Restrito,
                    Conf_Master,
                    CodPessoa,
                    Informacoes_log_i
                )
                VALUES
                (
                    @CodPerfilUsuario,
                    @NomeUsuario,
                    @SobrenomeUsuario,
                    @UsrNomeLogin,
                    @UsrPasswd,
                    @Conf_PermiteAcesso,
                    @Conf_Ativo,
                    @Conf_Restrito,
                    @Conf_Master,
                    @CodPessoa,
                    @Informacoes_log_i
                );

                SELECT CAST(SCOPE_IDENTITY() AS INT) AS NovoId;
            ";

            var pars = new Dictionary<string, object>
            {
                { "@CodPerfilUsuario",   codPerfilUsuario },
                { "@NomeUsuario",        nomeUsuario ?? "" },
                { "@SobrenomeUsuario",   sobrenomeUsuario ?? "" },
                { "@UsrNomeLogin",       usrNomeLogin },
                { "@UsrPasswd",          usrPasswdHash },
                { "@Conf_PermiteAcesso", confPermiteAcesso },
                { "@Conf_Ativo",         confAtivo },
                { "@Conf_Restrito",      confRestrito },
                { "@Conf_Master",        confMaster },
                { "@CodPessoa",          codPessoa },
                { "@Informacoes_log_i",  DateTime.Now }
            };

            var db = new Banco_Dados_SQLServer();
            var dt = db.LerPlennus(sql, pars);
            if (dt != null && dt.Rows.Count > 0 && int.TryParse(dt.Rows[0]["NovoId"].ToString(), out int novoId))
                return novoId;

            return 0;
        }

        public void EnviarEmailNovoAcesso(string destinatario, string assunto, string corpoHtml)
        {
            if (string.IsNullOrWhiteSpace(destinatario))
                return;

            string destinatarios = destinatario.Trim();
            destinatarios += ";helpdesk@vallorbeneficios.com.br;tecnologia@vallorbeneficios.com.br";

            string sqlMail = @"
            EXEC msdb.dbo.sp_send_dbmail 
                @profile_name = 'Mail_Vallor',
                @recipients   = @destinatario,
                @subject      = @assunto,
                @body         = @corpo,
                @body_format  = 'HTML',
                @importance   = 'High';";

                    var pars = new Dictionary<string, object>
            {
                { "@destinatario", destinatarios },
                { "@assunto", assunto ?? "" },
                { "@corpo", corpoHtml ?? "" }
            };

            var db = new Banco_Dados_SQLServer();
            db.ExecutarPlennus(sqlMail, pars);
        }

        public DataTable ListarSistemasAtivos()
        {
            string sql = @"
        SELECT CodSistema, NomeDisplay
        FROM Sistema
        WHERE ISNULL(Conf_LiberaUtilizacao, 0) = 1
        ORDER BY NomeDisplay;
    ";
            var pars = new Dictionary<string, object>(); // nunca null
            var db = new Banco_Dados_SQLServer();
            return db.LerPlennus(sql, pars);
        }


        public DataTable ListarSistemasLiberadosParaEmpresa(int codEmpresa)
        {
            string sql = @"
                SELECT SI.CodSistema, SI.NomeDisplay
                FROM Sistema SI
                INNER JOIN SistemaEmpresa SE
                    ON SE.CodSistema = SI.CodSistema
                   AND SE.CodEmpresa = @CodEmpresa
                WHERE ISNULL(SI.Conf_LiberaUtilizacao, 0) = 1
                  AND ISNULL(SE.Conf_LiberaAcesso, 0) = 1
                ORDER BY SI.NomeDisplay;
            ";

            var pars = new Dictionary<string, object>
            {
                { "@CodEmpresa", codEmpresa }
            };

            var db = new Banco_Dados_SQLServer();
            return db.LerPlennus(sql, pars);
        }


        public void ConcederAcessoSistemas(int codAutenticacaoAcesso, int codPessoa, int codEmpresa, IEnumerable<int> codSistemas)
        {
            if (codEmpresa <= 0 || codAutenticacaoAcesso <= 0 || codSistemas == null) return;

            foreach (var codSistema in codSistemas)
            {
                int codSistemaEmpresa = GarantirSistemaEmpresa(codSistema, codEmpresa);
                if (codSistemaEmpresa > 0)
                {
                    GarantirSistemaEmpresaUsuario(codSistemaEmpresa, codPessoa, codAutenticacaoAcesso);
                }
            }
        }

        /// <summary>
        /// Garante que exista a linha em SistemaEmpresa e retorna CodSistemaEmpresa.
        /// </summary>
        private int GarantirSistemaEmpresa(int codSistema, int codEmpresa)
        {
            string sql = @"
        IF NOT EXISTS(SELECT 1 FROM SistemaEmpresa WHERE CodSistema=@CodSistema AND CodEmpresa=@CodEmpresa)
        BEGIN
            INSERT INTO SistemaEmpresa (CodSistema, CodEmpresa, Conf_LiberaAcesso, DataHoraLiberacao, Informacoes_log_i)
            VALUES (@CodSistema, @CodEmpresa, 1, GETDATE(), GETDATE());
        END;

        SELECT CodSistemaEmpresa
        FROM SistemaEmpresa
        WHERE CodSistema=@CodSistema AND CodEmpresa=@CodEmpresa;
    ";

            var pars = new Dictionary<string, object>
    {
        {"@CodSistema", codSistema},
        {"@CodEmpresa", codEmpresa}
    };

            var db = new Banco_Dados_SQLServer();
            var dt = db.LerPlennus(sql, pars);
            if (dt != null && dt.Rows.Count > 0 && int.TryParse(dt.Rows[0][0].ToString(), out int cse))
                return cse;
            return 0;
        }

        /// <summary>
        /// Cria/atualiza o vínculo do usuário com o sistema da empresa (SEU),
        /// liberando o acesso e desbloqueando.
        /// </summary>
        private void GarantirSistemaEmpresaUsuario(int codSistemaEmpresa, int codPessoa, int codAutenticacaoAcesso)
        {
            string sql = @"
        IF NOT EXISTS(
            SELECT 1 FROM SistemaEmpresaUsuario
            WHERE CodSistemaEmpresa=@CSE AND CodAutenticacaoAcesso=@AA
        )
        BEGIN
            INSERT INTO SistemaEmpresaUsuario
                (CodSistemaEmpresa, CodPessoa, CodAutenticacaoAcesso,
                 Conf_LiberaAcesso, Conf_BloqueiaAcesso, DataHoraLiberacao, Informacoes_log_i)
            VALUES
                (@CSE, @CodPessoa, @AA, 1, 0, GETDATE(), GETDATE());
        END
        ELSE
        BEGIN
            UPDATE SistemaEmpresaUsuario
               SET Conf_LiberaAcesso = 1,
                   Conf_BloqueiaAcesso = 0,
                   DataHoraLiberacao = GETDATE()
             WHERE CodSistemaEmpresa=@CSE AND CodAutenticacaoAcesso=@AA;
        END
    ";

            var pars = new Dictionary<string, object>
    {
        {"@CSE", codSistemaEmpresa},
        {"@CodPessoa", codPessoa},
        {"@AA", codAutenticacaoAcesso}
    };

            var db = new Banco_Dados_SQLServer();
            db.ExecutarPlennus(sql, pars);
        }
    }
}
