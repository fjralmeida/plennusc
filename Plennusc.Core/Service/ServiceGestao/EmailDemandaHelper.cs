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
            string detalhes = $@"
        <p style='margin:0 0 12px; font-size:14px; color:#202124;'><strong>Solicitante:</strong> {solicitante}</p>
        <p style='margin:0 0 3px; font-size:12px; color:#9aa0a6; text-transform:uppercase; letter-spacing:.4px; font-weight:600;'>Título</p>
        <p style='margin:0 0 12px; font-size:14px; color:#202124; font-weight:600;'>{titulo}</p>
        <p style='margin:0 0 3px; font-size:12px; color:#9aa0a6; text-transform:uppercase; letter-spacing:.4px; font-weight:600;'>Prazo</p>
        <p style='margin:0; font-size:14px; color:#5f6368; line-height:1.5;'>{prazoTexto}</p>";

            string corpo = MontarCorpo(
                "equipe",
                "Uma nova demanda foi aberta para o seu setor.",
                detalhes,
                linkDemanda,
                "Ver Demanda");

            EnviarParaVarios(destinatarios, assunto, corpo);
        }

        public void NotificarDemandaAceita(int codPessoaSolicitante, string titulo, string aceitoPor, string linkDemanda)
        {
            var email = ObterEmailPessoa(codPessoaSolicitante);
            var nomeSolicitante = ObterNomePessoa(codPessoaSolicitante);
            if (string.IsNullOrWhiteSpace(email)) return;

            string assunto = $"Sua demanda foi aceita: {titulo}";
            string detalhes = $@"
        <p style='margin:0 0 12px; font-size:14px; color:#202124;'><strong>Aceita por:</strong> {aceitoPor}</p>
        <p style='margin:0 0 3px; font-size:12px; color:#9aa0a6; text-transform:uppercase; letter-spacing:.4px; font-weight:600;'>Título</p>
        <p style='margin:0; font-size:14px; color:#202124; font-weight:600;'>{titulo}</p>";

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
        <p style='margin:0 0 12px; font-size:14px; color:#202124;'><strong>Por:</strong> {autorAcompanhamento}</p>
        <p style='margin:0 0 3px; font-size:12px; color:#9aa0a6; text-transform:uppercase; letter-spacing:.4px; font-weight:600;'>Título</p>
        <p style='margin:0 0 12px; font-size:14px; color:#202124; font-weight:600;'>{titulo}</p>
        <p style='margin:0 0 3px; font-size:12px; color:#9aa0a6; text-transform:uppercase; letter-spacing:.4px; font-weight:600;'>Mensagem</p>
        <p style='margin:0; font-size:14px; color:#5f6368; line-height:1.5;'>{trechoTexto}</p>";

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
            string logoUrl = "https://plennusc.vallorbeneficios.com.br/Uploads/logo_plennus_cortado.png";

            return $@"
        <!DOCTYPE html>
        <html>
        <head><meta charset='UTF-8'></head>
        <body style='font-family: Roboto, Arial, sans-serif; background-color:#eef1f4; margin:0; padding:24px 0;'>
            <table align='center' cellpadding='0' cellspacing='0' width='600' style='max-width:600px; width:100%; background:#ffffff; border-radius:10px; overflow:hidden; box-shadow:0 2px 8px rgba(60,64,67,.12);'>

                <!-- Logo CENTRALIZADA e maior -->
                <tr>
                    <td style='padding:26px 32px 18px; text-align:center;'>
                        <img src='{logoUrl}' alt='Plennus' height='40' style='height:40px; width:auto; display:inline-block; border:0;' />
                    </td>
                </tr>

                <!-- Barra colorida -->
                <tr>
                    <td style='height:4px; background-color:#4cb07a; line-height:0; font-size:0;'>&nbsp;</td>
                </tr>

                <!-- Conteúdo -->
                <tr>
                    <td style='padding:28px 32px 8px;'>
                        <p style='margin:0 0 4px; font-size:13px; color:#9aa0a6; text-transform:uppercase; letter-spacing:.6px; font-weight:600;'>Plennus Gestão</p>
                        <h2 style='margin:0 0 16px; font-size:21px; font-weight:700; color:#202124;'>
                            Olá, {nomeDestinatario}
                        </h2>
                        <p style='margin:0 0 22px; font-size:14px; line-height:1.6; color:#5f6368;'>
                            {introducao}
                        </p>
                    </td>
                </tr>

                <!-- Caixa de destaque -->
                <tr>
                    <td style='padding:0 32px 28px;'>
                        <table cellpadding='0' cellspacing='0' width='100%' style='background:#f8f9fa; border-radius:8px;'>
                            <tr>
                                <td style='width:4px; background-color:#4cb07a; border-radius:8px 0 0 8px;'>&nbsp;</td>
                                <td style='padding:16px 20px;'>
                                    {detalhesHtml}
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

                <!-- Botão -->
                <tr>
                    <td style='padding:0 32px 36px;'>
                        <table cellpadding='0' cellspacing='0'>
                            <tr>
                                <td bgcolor='#4cb07a' style='border-radius:24px;'>
                                    <a href='{linkDemanda}'
                                       style='display:inline-block; font-family:Roboto, Arial, sans-serif; font-size:14px; font-weight:700; color:#ffffff !important; text-decoration:none; padding:14px 32px; border-radius:24px; letter-spacing:.3px;'>
                                        {textoBotao} &nbsp;→
                                    </a>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>

                <!-- Rodapé -->
                <tr>
                    <td style='padding:16px 32px 26px; border-top:1px solid #e8eaed;'>
                        <p style='margin:0; font-size:11px; color:#9aa0a6;'>
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