<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TempleteMensagem.aspx.cs" Inherits="appWhatsapp.Controller.TempleteMensagem" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <title>Comunicado de Pagamento | Vallor</title>
    <style>
        body {
            font-family: 'Segoe UI', Arial, sans-serif;
            background-color: #f5f5f5;
            margin: 0;
            padding: 40px 0;
        }

        .container {
            background-color: #ffffff;
            max-width: 700px;
            margin: auto;
            padding: 30px 40px;
            border-radius: 10px;
            box-shadow: 0 0 15px rgba(0,0,0,0.05);
            color: #333333;
        }

        h1 {
            color: #7a004b;
            font-size: 22px;
        }

        p {
            font-size: 16px;
            margin-bottom: 15px;
        }

        strong {
            color: #e17b00;
        }

        .inputText {
            border: none;
            border-bottom: 1px solid #ccc;
            font-size: 16px;
            padding: 2px 4px;
            width: auto;
            background-color: transparent;
        }

        footer {
            margin-top: 40px;
            font-size: 14px;
            color: #555555;
        }
    </style>
</head>
<body>
   <form id="form1" runat="server">
        <div class="container">
            <h1>Comunicado Importante</h1>

            <!-- Campo de WhatsApp -->
            <div style="text-align:center; margin-bottom: 20px;">
                <label for="txtWhatsapp" style="font-weight: bold;">Número do WhatsApp:</label><br />
                <asp:TextBox ID="txtWhatsapp" runat="server" CssClass="inputText" placeholder="Ex: 31999998888" />
            </div>

            <p>Boa tarde!</p>

            <p>
                Prezado(a) 
                <strong>
                    <asp:TextBox ID="txtNomeCliente" runat="server" CssClass="inputText" placeholder="Nome do Cliente" />
                </strong>,
            </p>

            <p>Esperamos que esteja tudo bem com você.</p>

            <p>
                Para sua comodidade, segue em anexo o boleto referente à mensalidade de 
                <strong>
                    <asp:TextBox ID="txtMesReferencia" runat="server" CssClass="inputText" placeholder="Mês de Vencimento" />
                </strong> 
                em atraso do seu plano de saúde da operadora <strong>AURORA</strong>, com vencimento original em 
                <strong>
                    <asp:TextBox ID="txtDataVencimento" runat="server" CssClass="inputText" placeholder="DD/MM/AAAA" oninput="formatarData(this)" />
                </strong>.
            </p>

            <p>
                Solicitamos que o pagamento seja realizado até o dia 
                <strong>
                    <asp:TextBox ID="txtDataLimite" runat="server" CssClass="inputText" placeholder="DD/MM/AAAA" oninput="formatarData(this)" />
                </strong> 
                para evitar o <strong>CANCELAMENTO DEFINITIVO</strong> do seu plano de saúde.
            </p>

            <p>Recomendamos confirmar os dados dos boletos antes de efetuar o pagamento.</p>

            <p>
                Em caso de dúvidas, por favor responda este e-mail ou entre em contato através dos telefones informados abaixo.
            </p>

            <!-- Botão de envio -->
            <div style="text-align:center; margin-top: 30px;">
                <asp:Button ID="btnEnviarWhatsapp" runat="server" Text="Enviar WhatsApp" OnClick="btnEnviarWhatsapp_Click" CssClass="inputText" />
            </div>
        </div>
    </form>

    <script>
        function formatarData(input) {
            var valor = input.value.replace(/\D/g, '');

            if (valor.length > 8)
                valor = valor.substring(0, 8);

            var novaValor = '';
            for (var i = 0; i < valor.length; i++) {
                if (i === 2 || i === 4) {
                    novaValor += '/';
                }
                novaValor += valor[i];
            }

            input.value = novaValor;
        }
    </script>
</body>
</html>
