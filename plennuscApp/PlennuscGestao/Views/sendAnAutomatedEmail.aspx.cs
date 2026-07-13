using appWhatsapp.Data_Bd;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web.UI;

namespace appWhatsapp.PlennuscGestao.Views
{
    public partial class sendAnAutomatedEmail : System.Web.UI.Page
    {
        private const string CaminhoLogo = @"C:\inetpub\wwwroot\plennusc\plennuscApp\public\uploadgestao\images\logo-VALLOR-CMYK_13 - Simbolo.png";

        protected void Page_Load(object sender, EventArgs e) { }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            if (!fuPlanilha.HasFile)
            {
                lblStatus.Text = "Selecione um arquivo CSV.";
                return;
            }

            string logoBase64Tag = ObterLogoEmBase64();

            var resultados = new List<object>();

            using (var reader = new StreamReader(fuPlanilha.PostedFile.InputStream, Encoding.UTF8))
            {
                string linhaHeader = reader.ReadLine();
                char separador = linhaHeader != null && linhaHeader.Contains(";") ? ';' : ',';

                string linha;
                while ((linha = reader.ReadLine()) != null)
                {
                    if (string.IsNullOrWhiteSpace(linha)) continue;

                    var colunas = linha.Split(separador);
                    if (colunas.Length < 4) continue;

                    string nome = colunas[0].Trim().Trim('"');
                    string premio = colunas[1].Trim().Trim('"');
                    string numeroSorte = colunas[2].Trim().Trim('"');
                    string email = colunas[3].Trim().Trim('"');

                    if (string.IsNullOrWhiteSpace(email)) continue;

                    string assunto = $"🎉 Você já está participando do sorteio - {premio}";

                    string corpo = $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                            <meta charset=""UTF-8"">
                        </head>
                        <body style=""font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 20px;"">
                            <table align=""center"" cellpadding=""0"" cellspacing=""0"" style=""max-width: 600px; background-color: #ffffff; border-radius: 8px; padding: 30px; box-shadow: 0 2px 5px rgba(0,0,0,0.1);"">
                                <tr>
                                    <td style=""font-size: 16px; line-height: 1.6; color: #333333;"">
                                        <p style=""font-weight: bold; font-size: 18px;"">Parabéns, {nome}! 🎉</p>
                                        <p>Seu número da sorte para a campanha foi gerado e você já está participando do sorteio do prêmio <strong>{premio}</strong>.</p>
                                        <p><strong>Número da sorte:</strong> {numeroSorte}</p>
                                        <p>Lembramos que as suas chances são maiores conforme a quantidade de vidas vendidas durante a campanha!</p>
                                        <p>📅 <strong>Anote na agenda!</strong><br/>
                                        O sorteio será realizado no dia <strong>22 de julho</strong>, às <strong>16h</strong>, em uma live no Instagram <strong>@valloradmbeneficios</strong>. Não perca a oportunidade de acompanhar esse momento ao vivo!</p>
                                        <p>Agradecemos pela sua parceria e desejamos muito sucesso. Boa sorte!</p>
                                        <p style=""margin-top: 30px; text-align: center;"">Atenciosamente,<br/><strong>Equipe Vallor Administradora de Benefícios</strong></p>
                                        <div style=""text-align: center; margin-top: 15px;"">
                                            <img src=""data:image/png;base64,{logoBase64Tag}"" alt=""Vallor"" height=""80"" width=""auto"" style=""height: 80px; width: auto; display: block; margin: 0 auto;"" />
                                        </div>
                                    </td>
                                </tr>
                            </table>
                        </body>
                        </html>";

                    string status = "OK";
                    try
                    {
                        EnviarEmailSorteio(email, assunto, corpo);
                    }
                    catch (Exception ex)
                    {
                        status = "ERRO: " + ex.Message;
                    }

                    resultados.Add(new { Nome = nome, Email = email, Status = status });
                }
            }

            gvResultado.DataSource = resultados;
            gvResultado.DataBind();
            lblStatus.Text = $"{resultados.Count} processados.";
        }

        private string ObterLogoEmBase64()
        {
            try
            {
                if (!File.Exists(CaminhoLogo)) return "";

                using (var imagemOriginal = System.Drawing.Image.FromFile(CaminhoLogo))
                {
                    int novaAltura = 80;
                    int novaLargura = (int)((double)imagemOriginal.Width / imagemOriginal.Height * novaAltura);

                    using (var imagemRedimensionada = new Bitmap(novaLargura, novaAltura))
                    {
                        using (var g = Graphics.FromImage(imagemRedimensionada))
                        {
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.DrawImage(imagemOriginal, 0, 0, novaLargura, novaAltura);
                        }

                        using (var ms = new MemoryStream())
                        {
                            imagemRedimensionada.Save(ms, ImageFormat.Png);
                            byte[] bytes = ms.ToArray();
                            return Convert.ToBase64String(bytes);
                        }
                    }
                }
            }
            catch
            {
                return "";
            }
        }

        private void EnviarEmailSorteio(string destinatario, string assunto, string corpoHtml)
        {
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
    }
}