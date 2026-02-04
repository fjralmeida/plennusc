<%@ Page Title="Gerador de Propostas" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="Butyou.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.Butyou" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .container { padding: 20px; }
        .btn { padding: 10px 20px; margin: 10px; }
        .result { margin: 20px 0; padding: 20px; background: #f5f5f5; border: 1px solid #ddd; }
        .error { color: red; }
        .success { color: green; }
        .debug-panel { 
            margin: 20px 0; 
            padding: 15px; 
            background: #e8f4f8; 
            border: 2px solid #007bff; 
            border-radius: 5px; 
            max-height: 300px; 
            overflow-y: auto; 
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <h1>GERADOR DE PROPOSTAS - MIGRAÇÃO ANASERV</h1>
        
            
            <!-- BOTÃO DE TESTE NOVO CORRIGIDO -->
            <asp:Button ID="btnTestarPreenchimento" runat="server" Text="✅ TESTAR PREENCHIMENTO" 
                OnClick="btnTestarPreenchimento_Click" CssClass="btn btn-info" />
          
        </div>
        
        <!-- MENSAGENS -->
        <div style="margin: 15px 0;">
            <asp:Label ID="lblMensagem" runat="server" CssClass="success" Visible="false" Font-Bold="true"></asp:Label>
            <asp:Label ID="lblErro" runat="server" CssClass="error" Visible="false" Font-Bold="true"></asp:Label>
        </div>
        
        <!-- ÁREA DE RESULTADOS -->
        <div class="result">
            <!-- Lista de campos encontrados -->
            <asp:Panel ID="pnlCampos" runat="server" Visible="false">
                <h3>📋 CAMPOS IDENTIFICADOS NO DOCX:</h3>
                <asp:Literal ID="litCampos" runat="server"></asp:Literal>
            </asp:Panel>
            
            <!-- PANEL DE DEBUG (PARA VER O QUE ESTÁ SENDO ENCONTRADO) -->
            <asp:Panel ID="pnlDebug" runat="server" Visible="false" CssClass="debug-panel">
                <h4>🔍 DEBUG - Textos encontrados no documento:</h4>
                <asp:Literal ID="litDebug" runat="server"></asp:Literal>
            </asp:Panel>
            
            <!-- DADOS DE TESTE USADOS -->
            <asp:Panel ID="pnlDadosTeste" runat="server" Visible="false" style="margin-top: 20px;">
                <h4>📝 DADOS DE TESTE USADOS:</h4>
                <div style="background: white; padding: 10px; border: 1px solid #ccc;">
                    <strong>TITULAR:</strong><br />
                    NOME: MARCOS ANTONIO SILVEIRA<br />
                    CPF: 123.456.789-00<br />
                    NASCIMENTO: 15/03/1985<br />
                    RG: 12.345.678-9<br />
                    ENDEREÇO: AVENIDA BOA VIAGEM, 1001, APTO 502<br />
                    <br />
                    <strong>DEPENDENTE 1 (ESPOSA):</strong><br />
                    NOME: ANA CAROLINA SILVEIRA<br />
                    CPF: 111.222.333-44<br />
                    <br />
                    <strong>DEPENDENTE 2 (FILHO):</strong><br />
                    NOME: LUCAS SILVEIRA<br />
                    CPF: 555.666.777-88<br />
                </div>
            </asp:Panel>
        </div>
        
        <!-- INSTRUÇÕES DE TESTE -->
        <div style="margin-top: 20px; padding: 15px; background: #fff3cd; border: 1px solid #ffc107;">
            <h4>📋 COMO TESTAR:</h4>
            <ol>
                <li>Clique em <strong>"✅ TESTAR PREENCHIMENTO"</strong></li>
                <li>Será gerado um arquivo DOCX com dados de teste</li>
                <li>Baixe e abra o arquivo</li>
                <li>Verifique se todos os campos foram preenchidos corretamente</li>
                <li>No painel de DEBUG acima, veja quais textos foram encontrados</li>
            </ol>
        </div>
        
        <!-- Informações do projeto -->
        <div style="margin-top: 30px; padding: 15px; background: #e9ecef;">
            <h4>📊 INFORMAÇÕES DO PROJETO:</h4>
            <ul>
                <li><strong>Template:</strong> MAIS VOCE - PE - DOCX.docx</li>
                <li><strong>Tabela de origem:</strong> ps1000</li>
                <li><strong>Total de beneficiários:</strong> 1200 vidas</li>
                <li><strong>Saída:</strong> Arquivos DOCX individuais + ZIP</li>
                <li><strong>Status:</strong> <span style="color: orange;">⏳ EM TESTE</span></li>
            </ul>
        </div>
    </div>
    
    <!-- Script para reabilitar botão após erro -->
<%--    <script>
        function reabilitarBotoes() {
            var btnTeste = document.getElementById('<%= btnTestarPreenchimento.ClientID %>');
            if (btnTeste) {
                btnTeste.disabled = false;
                btnTeste.value = '✅ TESTAR PREENCHIMENTO';
            }
        }

        // Executar quando houver erro
        if (document.getElementById('<%= lblErro.ClientID %>') && 
            document.getElementById('<%= lblErro.ClientID %>').style.display != 'none') {
            reabilitarBotoes();
        }
    </script>--%>
</asp:Content>