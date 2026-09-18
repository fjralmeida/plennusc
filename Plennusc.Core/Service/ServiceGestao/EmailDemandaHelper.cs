using appWhatsapp.Data_Bd;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plennusc.Core.Service.ServiceGestao
{
    public class EmailDemandaHelper
    {
        private readonly string _connKey;

        public EmailDemandaHelper(string connKey = "Plennus")
        {
            _connKey = connKey;
        }

        // ---------- ENVIO BASE (mesmo padrão do sendAnAutomatedEmail) ----------
        private void EnviarEmail(string destinatario, string assunto, string corpoHtml)
        {
            if (string.IsNullOrWhiteSpace(destinatario)) return;

            string sqlMail = @"
                EXEC msdb.dbo.sp_send_dbmail 
                    @profile_name = 'Mail_Vallor',
                    @recipients   = @destinatario,
                    @subject      = @assunto,
                    @body         = @corpo,
                    @body_format  = 'HTML';";

            var pars = new Dictionary<string, object>
            {
                { "@destinatario", destinatario },
                { "@assunto", assunto },
                { "@corpo", corpoHtml }
            };

            var db = new Banco_Dados_SQLServer();
            db.ExecutarPlennus(sqlMail, pars);
        }

        // Envia pra vários destinatários de uma vez, sem derrubar o processo se um falhar
        private void EnviarParaVarios(IEnumerable<string> destinatarios, string assunto, string corpoHtml)
        {
            foreach (var email in destinatarios)
            {
                try { EnviarEmail(email, assunto, corpoHtml); }
                catch { /* log silencioso, não pode travar o fluxo da demanda */ }
            }
        }

        // ---------- BUSCA DE DESTINATÁRIOS ----------

        // E-mails de todo mundo do setor de destino
        public List<string> ObterEmailsDoSetor(int codSetor)
        {
            var emails = new List<string>();
            using (var con = new SqlConnection(
                System.Configuration.ConfigurationManager.ConnectionStrings[_connKey].ConnectionString))
            using (var cmd = new SqlCommand(@"
                SELECT p.Email
                FROM dbo.Pessoa p
                WHERE p.CodDepartamento = @CodSetor
                  AND p.Email IS NOT NULL AND p.Email <> ''", con))
            {
                cmd.Parameters.AddWithValue("@CodSetor", codSetor);
                con.Open();
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        emails.Add(rd.GetString(0));
                }
            }
            return emails;
        }

        // E-mail de uma pessoa específica (solicitante, executor, etc.)
        public string ObterEmailPessoa(int codPessoa)
        {
            using (var con = new SqlConnection(
                System.Configuration.ConfigurationManager.ConnectionStrings[_connKey].ConnectionString))
            using (var cmd = new SqlCommand(
                "SELECT Email FROM dbo.Pessoa WHERE CodPessoa = @CodPessoa", con))
            {
                cmd.Parameters.AddWithValue("@CodPessoa", codPessoa);
                con.Open();
                var result = cmd.ExecuteScalar();
                return result == null || result == DBNull.Value ? null : result.ToString();
            }
        }

        // ---------- TEMPLATES + DISPARO POR EVENTO ----------

        public void NotificarNovaDemanda(int codSetorDestino, string titulo, string solicitante, string prazoTexto, string linkDemanda)
        {
            var destinatarios = ObterEmailsDoSetor(codSetorDestino);
            if (destinatarios.Count == 0) return;

            string assunto = $"Nova demanda: {titulo}";
            string corpo = MontarCorpo(
                $"Uma nova demanda foi aberta para o seu setor.",
                $@"<p><strong>Título:</strong> {titulo}</p>
                   <p><strong>Solicitante:</strong> {solicitante}</p>
                   <p><strong>Prazo:</strong> {prazoTexto}</p>",
                linkDemanda);

            EnviarParaVarios(destinatarios, assunto, corpo);
        }

        public void NotificarDemandaAceita(int codPessoaSolicitante, string titulo, string aceitoPor, string linkDemanda)
        {
            var email = ObterEmailPessoa(codPessoaSolicitante);
            if (string.IsNullOrWhiteSpace(email)) return;

            string assunto = $"Sua demanda foi aceita: {titulo}";
            string corpo = MontarCorpo(
                "Sua demanda foi aceita e está sendo tratada.",
                $@"<p><strong>Título:</strong> {titulo}</p>
                   <p><strong>Aceita por:</strong> {aceitoPor}</p>",
                linkDemanda);

            EnviarEmail(email, assunto, corpo);
        }

        public void NotificarDemandaRespondida(int codPessoaDestino, string titulo, string autorAcompanhamento, string trechoTexto, string linkDemanda)
        {
            var email = ObterEmailPessoa(codPessoaDestino);
            if (string.IsNullOrWhiteSpace(email)) return;

            string assunto = $"Nova interação na demanda: {titulo}";
            string corpo = MontarCorpo(
                "Sua demanda recebeu uma nova resposta/atualização.",
                $@"<p><strong>Título:</strong> {titulo}</p>
                   <p><strong>Por:</strong> {autorAcompanhamento}</p>
                   <p><strong>Mensagem:</strong> {trechoTexto}</p>",
                linkDemanda);

            EnviarEmail(email, assunto, corpo);
        }

        private string MontarCorpo(string introducao, string detalhesHtml, string linkDemanda)
        {
            return $@"
                <!DOCTYPE html>
                <html><head><meta charset='UTF-8'></head>
                <body style='font-family: Arial, sans-serif; background:#f4f4f4; padding:20px;'>
                    <table align='center' cellpadding='0' cellspacing='0' style='max-width:600px; background:#fff; border-radius:8px; padding:30px;'>
                        <tr><td style='font-size:15px; color:#333;'>
                            <p>{introducao}</p>
                            {detalhesHtml}
                            <p style='margin-top:20px;'>
                                <a href='{linkDemanda}' style='background:#4cb07a;color:#fff;padding:10px 18px;text-decoration:none;border-radius:6px;'>Ver demanda</a>
                            </p>
                            <p style='margin-top:30px;'>Atenciosamente,<br/><strong>Plennus Gestão</strong></p>
                        </td></tr>
                    </table>
                </body></html>";
        }
    }
}