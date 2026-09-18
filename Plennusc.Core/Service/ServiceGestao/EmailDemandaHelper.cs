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
            var destinatarios = ObterEmailsDoSetor(codSetorDestino); // se quiser, retorna nome+email junto pra personalizar por pessoa
            if (destinatarios.Count == 0) return;

            string assunto = $"Nova demanda: {titulo}";
            string detalhes = $@"
        <p style='margin:0 0 8px; font-size:14px; color:#202124;'><strong>Título:</strong> {titulo}</p>
        <p style='margin:0 0 8px; font-size:14px; color:#202124;'><strong>Solicitante:</strong> {solicitante}</p>
        <p style='margin:0; font-size:14px; color:#202124;'><strong>Prazo:</strong> {prazoTexto}</p>";

            string corpo = MontarCorpo(
                "equipe", // ou o nome do setor/pessoa se você buscar individualmente
                "Uma nova demanda foi aberta para o seu setor. Confira os detalhes abaixo:",
                detalhes,
                linkDemanda,
                "Ver Demanda");

            EnviarParaVarios(destinatarios, assunto, corpo);
        }
        public void NotificarDemandaAceita(int codPessoaSolicitante, string titulo, string aceitoPor, string linkDemanda)
        {
            var email = ObterEmailPessoa(codPessoaSolicitante);
            var nomeSolicitante = ObterNomePessoa(codPessoaSolicitante); // ver método novo abaixo
            if (string.IsNullOrWhiteSpace(email)) return;

            string assunto = $"Sua demanda foi aceita: {titulo}";
            string detalhes = $@"
        <p style='margin:0 0 8px; font-size:14px; color:#202124;'><strong>Título:</strong> {titulo}</p>
        <p style='margin:0; font-size:14px; color:#202124;'><strong>Aceita por:</strong> {aceitoPor}</p>";

            string corpo = MontarCorpo(
                nomeSolicitante ?? "usuário",
                "Sua demanda foi aceita e já está sendo tratada por nossa equipe.",
                detalhes,
                linkDemanda,
                "Ver Demanda");

            EnviarEmail(email, assunto, corpo);
        }

        public void NotificarDemandaRespondida(int codPessoaDestino, string titulo, string autorAcompanhamento, string trechoTexto, string linkDemanda)
        {
            var email = ObterEmailPessoa(codPessoaDestino);
            var nomeDestino = ObterNomePessoa(codPessoaDestino);
            if (string.IsNullOrWhiteSpace(email)) return;

            string assunto = $"Nova interação na demanda: {titulo}";
            string detalhes = $@"
        <p style='margin:0 0 8px; font-size:14px; color:#202124;'><strong>Título:</strong> {titulo}</p>
        <p style='margin:0 0 8px; font-size:14px; color:#202124;'><strong>Por:</strong> {autorAcompanhamento}</p>
        <p style='margin:0; font-size:14px; color:#202124;'><strong>Mensagem:</strong> {trechoTexto}</p>";

            string corpo = MontarCorpo(
                nomeDestino ?? "usuário",
                "Sua demanda recebeu uma nova resposta/atualização.",
                detalhes,
                linkDemanda,
                "Ver Atualização");

            EnviarEmail(email, assunto, corpo);
        }
        private string MontarCorpo(string nomeDestinatario, string introducao, string detalhesHtml, string linkDemanda, string textoBotao = "Ver Demanda")
        {
            // Ajuste o caminho absoluto da logo (precisa ser uma URL pública acessível por quem recebe o email)
            string logoUrl = "https://plennusc.vallorbeneficios.com.br/Uploads/logo_plennus_cortado.png";

            return $@"
        <!DOCTYPE html>
        <html>
        <head><meta charset='UTF-8'></head>
        <body style='font-family: Roboto, Arial, sans-serif; background-color:#f1f3f4; margin:0; padding:24px 0;'>
            <table align='center' cellpadding='0' cellspacing='0' style='max-width:600px; width:100%; background:#ffffff; border-radius:8px; overflow:hidden; box-shadow:0 1px 3px 1px rgba(60,64,67,.15);'>

                <!-- Logo -->
                <tr>
                    <td style='padding:28px 32px 20px; text-align:center;'>
                        <img src='{logoUrl}' alt='Plennus' style='height:48px; width:auto;' />
                    </td>
                </tr>

                <!-- Barra colorida -->
                <tr>
                    <td style='height:6px; background:linear-gradient(90deg, #83ceee 0%, #4cb07a 100%); line-height:0; font-size:0;'>&nbsp;</td>
                </tr>

                <!-- Conteúdo -->
                <tr>
                    <td style='padding:28px 32px 8px;'>
                        <h2 style='margin:0 0 16px; font-size:20px; font-weight:600; color:#202124;'>
                            Olá, {nomeDestinatario}
                        </h2>
                        <p style='margin:0 0 20px; font-size:14px; line-height:1.6; color:#5f6368;'>
                            {introducao}
                        </p>
                    </td>
                </tr>

                <!-- Caixa de destaque -->
                <tr>
                    <td style='padding:0 32px 24px;'>
                        <div style='background:#f8f9fa; border:1px solid #e8eaed; border-left:4px solid #4cb07a; border-radius:6px; padding:16px 20px;'>
                            {detalhesHtml}
                        </div>
                    </td>
                </tr>

                <!-- Botão -->
                <tr>
                    <td style='padding:0 32px 32px;'>
                        <a href='{linkDemanda}'
                           style='display:inline-block; background:#4cb07a; color:#ffffff; font-size:14px; font-weight:600; text-decoration:none; padding:12px 24px; border-radius:6px;'>
                            {textoBotao}
                        </a>
                    </td>
                </tr>

                <!-- Rodapé -->
                <tr>
                    <td style='padding:16px 32px 28px; border-top:1px solid #e8eaed;'>
                        <p style='margin:0; font-size:12px; color:#9aa0a6;'>
                            Este é um e-mail automático do sistema Plennus Gestão. Não é necessário responder.
                        </p>
                    </td>
                </tr>
            </table>
        </body>
        </html>";
        }

        public string ObterNomePessoa(int codPessoa)
        {
            string sql = "SELECT Nome FROM dbo.Pessoa WHERE CodPessoa = @CodPessoa";
            // usa o mesmo padrão de conexão que o ObterEmailPessoa já usa hoje
            using (var con = new SqlConnection(
                System.Configuration.ConfigurationManager.ConnectionStrings[_connKey].ConnectionString))
            using (var cmd = new SqlCommand(sql, con))
            {
                cmd.Parameters.AddWithValue("@CodPessoa", codPessoa);
                con.Open();
                var result = cmd.ExecuteScalar();
                return result == null || result == DBNull.Value ? null : result.ToString();
            }
        }
    }
}