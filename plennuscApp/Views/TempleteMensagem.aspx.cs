using System;
using System.Web;
using System.Web.UI;

namespace appWhatsapp.Controller
{
    public partial class TempleteMensagem : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Simula dados que futuramente virão do banco
                string nomeCliente = ""; // virá do banco
                string dataVencimento = DateTime.Now.ToString("dd/MM/yyyy");

                // Calcula o mês anterior em caixa alta
                DateTime mesAnterior = DateTime.Now.AddMonths(-1);
                string mesReferencia = mesAnterior.ToString("MMMM", new System.Globalization.CultureInfo("pt-BR")).ToUpper();

                // Último dia do mês atual
                string dataLimite = new DateTime(DateTime.Now.Year, DateTime.Now.Month,
                                     DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))
                                     .ToString("dd/MM/yyyy");

                // Preenche os campos
                txtNomeCliente.Text = nomeCliente;
                txtDataVencimento.Text = dataVencimento;
                txtMesReferencia.Text = mesReferencia;
                txtDataLimite.Text = dataLimite;
            }
        }

        protected void btnEnviarWhatsapp_Click(object sender, EventArgs e)
        {
            string nome = txtNomeCliente.Text.Trim();
            string mes = txtMesReferencia.Text.Trim();
            string vencimento = txtDataVencimento.Text.Trim();
            string limite = txtDataLimite.Text.Trim();
            string numero = txtWhatsapp.Text.Trim();

            if (string.IsNullOrEmpty(numero))
                return;

            // Remove caracteres indesejados do número
            numero = numero.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");

            // Monta a mensagem sem espaços desnecessários
            string mensagem = $"Boa tarde, {nome}!\n\n" +
                              $"Esperamos que esteja tudo bem com você.\n\n" +
                              $"Segue o boleto referente à mensalidade de {mes} em atraso do seu plano de saúde da operadora AURORA, com vencimento original em {vencimento}.\n\n" +
                              $"Solicitamos que o pagamento seja realizado até o dia {limite} para evitar o CANCELAMENTO DEFINITIVO do seu plano.\n\n" +
                              $"Em caso de dúvidas, estamos à disposição.\n\n" +
                              $"Vallor Administradora de Benefícios\n" +
                              $"0800 311 9100 | sac@vallorbeneficios.com.br";

            // Codifica a mensagem para URL (usando HttpUtility para compatibilidade com Web Forms)
            string mensagemEncoded = HttpUtility.UrlEncode(mensagem);

            // Cria o link do WhatsApp
            string urlWhatsapp = $"https://wa.me/55{numero}?text={mensagemEncoded}";

            // Redireciona o usuário
            Response.Redirect(urlWhatsapp);
        }
    }
}
