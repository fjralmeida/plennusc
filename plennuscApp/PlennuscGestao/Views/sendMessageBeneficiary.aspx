<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master"
    AutoEventWireup="true" CodeBehind="sendMessageBeneficiary.aspx.cs"
    Inherits="PlennuscApp.PlennuscGestao.Views.EnvioMensagemBeneficiario"
    Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>


    <style>
        body {
            background: #f5f7fa;
            font-family: 'Poppins', sans-serif;
            font-size: 13px; /* Ajuste aqui o tamanho global da fonte */
            color: #333;
        }


        .card-container {
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 1px 6px rgba(0,0,0,0.05);
            padding: 16px;
            margin-bottom: 24px;
        }

        .card-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 16px
        }

        .btn-pill {
            border-radius: 50px;
            padding: 6px 18px;
            font-weight: 600;
            box-shadow: 0 2px 6px rgba(0,0,0,0.1);
            transition: background-color 0.3s, box-shadow 0.3s;
        }

            .btn-pill .fa {
                margin-right: 8px;
            }

        .btn-success {
            background-color: #4CB07A;
            border-color: #4CB07A;
            color: #fff;
        }

            .btn-success:hover {
                background-color: #3B8B65;
            }

        .btn-danger {
            background-color: #DC8689;
            border-color: #DC8689;
            color: #fff;
        }

            .btn-danger:hover {
                background-color: #b75963;
            }

        .btn-info {
            background-color: #83CEEE;
            border-color: #83CEEE;
            color: #fff;
        }

            .btn-info:hover {
                background-color: #6AB9E0;
                color: white;
            }

        .btn-purple {
            background-color: #C06ED4;
            border-color: #C06ED4;
            color: #fff;
        }

            .btn-purple:hover {
                background-color: #a14db8;
                border-color: #a14db8;
                color: #fff;
            }

        .botoes-acoes {
            display: flex;
            justify-content: space-evenly;
            gap: 16px;
            flex-wrap: wrap;
        }

        .btn-info,
        .btn-info:hover,
        .btn-info:focus,
        .btn-info:active,
        .btn-info:visited {
            color: #fff !important;
        }

        .filter-panel {
            background: #f0f2f5;
            padding: 12px;
            border-radius: 8px;
            margin-bottom: 16px;
        }

            .filter-panel .form-select {
                padding: 6px 12px;
                border-radius: 8px;
                border: 1px solid #ced4da;
                background-color: #fff;
                transition: border-color 0.3s;
            }

                .filter-panel .form-select:focus {
                    border-color: #4CB07A;
                    outline: none;
                    box-shadow: 0 0 0 2px rgba(76, 176, 122, 0.25);
                }


        .table thead {
            background-color: #fff;
        }

        .table th, .table td {
            text-align: center;
            padding: 10px;
            border-top: none;
            border-bottom: 1px solid #e9ecef;
        }

        /* LINHAS ALTERNADAS NO GRID PARA MELHOR VISUALIZAÇÃO */
        #GridAssociados tbody tr:nth-child(odd) {
            background-color: #ffffff;
        }

        #GridAssociados tbody tr:nth-child(even) {
            background-color: #f4f8fb;
        }

        /* LINHA SELECIONADA */
        .linha-selecionada td {
            background-color: rgba(76, 176, 122, 0.2) !important;
        }

        /* ESTILO DO CHECKBOX - APENAS APARÊNCIA PADRÃO, SEM FUNDO COLORIDO */
        input[type="checkbox"].form-check-input {
            width: 20px;
            height: 20px;
            cursor: pointer;
            accent-color: #4CB07A;
            border-radius: 4px;
            margin: 0;
            vertical-align: middle;
            box-shadow: none;
            background-color: transparent !important;
            background-image: none !important;
            border: 1px solid #ffffff !important;
            appearance: auto !important;
            -webkit-appearance: checkbox !important;
            -moz-appearance: checkbox !important;
        }

        .form-check-input {
            --bs-border-width: 0 !important;
            --bs-border-color: transparent !important;
            --bs-form-check-bg: transparent !important;
            --bs-form-check-bg-image: none !important;
        }
        /* PARA O CONTAINER DO CHECKBOX NO HEADER */
        .checkbox-header {
            display: flex;
            flex-direction: column;
            align-items: center;
            font-weight: 600;
            font-size: 13px;
            gap: 4px;
            padding: 4px 0;
        }

        /* CÉLULA DA COLUNA DO CHECKBOX */
        .col-selecao {
            text-align: center;
            vertical-align: middle;
            width: 60px;
        }

        /* REMOVE O NEGRITO DOS DADOS DO GRID */
        #GridAssociados td {
            font-weight: normal !important;
        }

        /* CHECKBOX ESTADO CHECKED */
        .form-check-input:checked {
            background-color: #4CB07A !important;
            border-color: #4CB07A !important;
        }

        .form-check-input:focus {
            outline: 2px solid #4CB07A;
            outline-offset: 2px;
        }
      /* PAGINAÇÃO DO GRID */
        /* 🔧 Estilo simples e elegante para a paginação do GridView */
        #GridAssociados .pager-footer {
            padding: 10px 16px;
            border-top: 1px solid #e6e6e6;
            background-color: #fff;
            font-size: 13px;
            text-align: right;
        }

        #GridAssociados .pager-footer a,
        #GridAssociados .pager-footer span {
            display: inline-block;
            padding: 4px 10px;
            margin: 0 2px;
            text-decoration: none;
            font-weight: 500;
            color: #4CB07A;
            border-radius: 4px;
            transition: all 0.2s ease-in-out;
        }

        #GridAssociados .pager-footer a:hover {
            background-color: #4CB07A;
            color: white;
        }

        #GridAssociados .pager-footer span {
            background-color: #4CB07A;
            color: white;
        }
        #modalResultadoConteudo {
            font-family: 'Inter', sans-serif;
            font-size: 14px;
            color: #333;
        }

        .modal-body {
            background-color: #f8f9fa;
        }

        .modal-header {
            background-color: #4CB07A !important;
            color: white;
        }

        /* 🔥 Modal Escolher Template */
        #modalEscolherMensagem {
            display: none;
            position: fixed;
            z-index: 1055;
            left: 0;
            top: 0;
            width: 100vw;
            height: 100vh;
            overflow: auto;
            background-color: rgba(0, 0, 0, 0.5);
            padding: 60px 20px;
        }

        #modalEscolherMensagem .modal-content {
            background-color: #fff;
            border-radius: 16px;
            max-width: 900px;
            margin: auto;
            padding: 24px;
            box-shadow: 0 8px 24px rgba(0,0,0,0.2);
            animation: fadeIn 0.3s;
        }

        #modalEscolherMensagem .modal-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            background-color: #4CB07A;
            color: white;
            border-radius: 12px 12px 0 0;
            padding: 12px 20px;
            margin: 3px;
        }

        #modalEscolherMensagem .modal-header h2 {
            margin: 0;
            font-size: 20px;
        }

        #modalEscolherMensagem .close {
            cursor: pointer;
            font-size: 24px;
            color: white;
        }

        .template-opcao {
            border: 2px solid #ccc;
            border-radius: 12px;
            padding: 16px;
            margin-bottom: 20px;
            cursor: pointer;
            transition: all 0.3s;
            background-color: #f9f9f9;
        }

        .template-opcao:hover {
            border-color: #4CB07A;
            background-color: #e6f5ee;
        }

        .template-opcao h4 {
            color: #4CB07A;
            margin-top: 0;
        }

        .data-pequena {
            max-width: 200px; /* ajustável, você pode testar com 180px ou 150px */
            width: 100%;
            padding: 6px 12px;
            box-sizing: border-box;
            cursor: pointer;
        }

        .campo-data {
            max-width: 240px;
            width: 100%;
            padding: 10px;
            font-size: 14px;
            text-transform: uppercase;
            cursor: pointer;
            border-radius: 6px;
            border: 1px solid #ccc;
            box-sizing: border-box;
            transition: border-color 0.3s ease;
        }

        .campo-data:focus {
            border-color: #4CB07A;
            outline: none;
            box-shadow: 0 0 0 2px rgba(76, 176, 122, 0.2);
        }

        @keyframes fadeIn {
            from {
                opacity: 0;
            }

            to {
                opacity: 1;
            }
        }
    </style>

    <script>
        function alternarSelecao(chk) {
            var row = chk.closest('tr');
            if (row) {
                if (chk.checked) {
                    row.classList.add('linha-selecionada');
                } else {
                    row.classList.remove('linha-selecionada');
                }
            }
            atualizarSelecionarTodos();
        }

        function selecionarTodos(chkHeader) {
            var grid = document.getElementById("GridAssociados");
            var checkboxes = grid.querySelectorAll("input[type='checkbox'][id*='chkSelecionar']");
            checkboxes.forEach(function (chk) {
                chk.checked = chkHeader.checked;
                var row = chk.closest('tr');
                if (chk.checked) {
                    row.classList.add('linha-selecionada');
                } else {
                    row.classList.remove('linha-selecionada');
                }
            });
        }

        function atualizarSelecionarTodos() {
            var grid = document.getElementById("GridAssociados");
            var chkTodos = grid.querySelector("input[type='checkbox'][id*='chkSelecionarTodos']");
            var checkboxes = grid.querySelectorAll("input[type='checkbox'][id*='chkSelecionar']");
            var todosMarcados = true;
            checkboxes.forEach(function (chk) {
                if (!chk.checked) {
                    todosMarcados = false;
                }
            });
            if (chkTodos) {
                chkTodos.checked = todosMarcados;
            }
        }

        // 🔁 FUNÇÃO  MOSTRAR OVERLAY DE CARREGAMENTO
        function mostrarLoading() {
            document.getElementById('loadingOverlay').style.display = 'block';
        }


        function mostrarResultadoModal(texto) {
            document.getElementById("modalResultadoConteudo").textContent = texto;
            var modal = new bootstrap.Modal(document.getElementById('resultadoModal'));
            modal.show();
        }

        // 🔁 FUNÇÃO PARA MODAL MENSAGEM
        function abrirModal() {
            document.getElementById('modalEscolherMensagem').style.display = 'block';
        }

        function fecharModal() {
            document.getElementById('modalEscolherMensagem').style.display = 'none';
        }

        function selecionarTemplate(template) {
            document.getElementById('<%= hfTemplateEscolhido.ClientID %>').value = template;
            fecharModal();
        }

    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Modal de Resultado -->
    <div class="modal fade" id="resultadoModal" tabindex="-1" aria-labelledby="resultadoModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content rounded-4 shadow">
                <div class="modal-header bg-success text-white rounded-top-4">
                    <h5 class="modal-title" id="resultadoModalLabel"><i class="fa-solid fa-paper-plane me-2"></i>Resultado do Envio</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Fechar"></button>
                </div>
                <div class="modal-body" style="max-height: 400px; overflow-y: auto;">
                    <pre id="modalResultadoConteudo" class="mb-0" style="white-space: pre-wrap; font-family: 'Inter', sans-serif;"></pre>
                </div>
                <div class="modal-footer bg-light rounded-bottom-4">
                    <button type="button" class="btn btn-secondary btn-pill" data-bs-dismiss="modal">
                        <i class="fa-solid fa-xmark me-1"></i>Fechar
                    </button>
                </div>
            </div>
        </div>
    </div>

    <%--MODAL TEMPLETE MENSAGEM--%>

    <asp:HiddenField ID="hfTemplateEscolhido" runat="server" />
    <div id="modalEscolherMensagem">
        <div class="modal-content">
            <div class="modal-header">
                <h2>Escolher Template de Mensagem</h2>
                <span class="close" onclick="fecharModal()">&times;</span>
            </div>

            <div class="modal-body">

                <div class="template-opcao" onclick="selecionarTemplate('Suspensao')">
                    <h4>Suspensão</h4>
                    <p>
                        EVITE MAIS ACÚMULO DE JUROS E MULTA<br>
                        <br>
                        Prezado(a) beneficiário(a) ****<br>
                        Segue abaixo o boleto para pagamento referente ao vencimento MAIO do seu plano de saúde UNIMED BH.<br>
                        Prazo para efetuar o pagamento e evitar o cancelamento COM PROBABILIDADE DE REATIVAÇÃO do seu plano é:
        <asp:TextBox ID="txtDataSuspensao" runat="server" TextMode="Date"
            CssClass="form-control campo-data" ClientIDMode="Static"
            onclick="event.stopPropagation();"></asp:TextBox><br>
                        <br>
                        Gentileza conferir os dados do boleto antes de realizar o pagamento.<br>
                        <br>
                        Dúvidas, seguimos à disposição.<br>
                        Departamento de cobrança da Vallor Benefícios.
                    </p>
                </div>

                <div class="template-opcao" onclick="selecionarTemplate('Definitivo')">
                    <h4>Definitivo</h4>
                    <p>
                        EVITE MAIS ACÚMULO DE JUROS E MULTA<br>
                        <br>
                        Prezado(a) beneficiário(a) *****<br>
                        Segue abaixo o boleto para pagamento referente ao vencimento ABRIL do seu plano de saúde VOCE TOTAL.<br>
                        Prazo para efetuar o pagamento e evitar o cancelamento DEFINITIVO do seu plano é:
        <asp:TextBox ID="txtDataDefinitivo" runat="server" TextMode="Date"
            CssClass="form-control campo-data" ClientIDMode="Static"
            onclick="event.stopPropagation();"></asp:TextBox><br>
                        <br>
                        O não pagamento poderá acarretar a inclusão do seu nome nos órgãos de proteção ao crédito SPC/SERASA.<br>
                        Após o pagamento, gentileza enviar o comprovante.<br>
                        <br>
                        Gentileza conferir os dados do boleto antes de realizar o pagamento.<br>
                        <br>
                        Dúvidas, seguimos à disposição.<br>
                        Departamento de cobrança da Vallor Benefícios.
                    </p>
                </div>

                <div class="template-opcao" onclick="selecionarTemplate('DoisBoletos')">
                    <h4>Dois Boletos</h4>
                    <p>
                        Boa tarde!<br>
                        <br>
                        Prezado(a) Sr.(a) ******, tudo bem? Esperamos que sim!<br>
                        <br>
                        Para sua comodidade, seguem em anexo os boletos referentes às mensalidades de ABRIL e MAIO em atraso do seu plano de saúde AURORA, cujo vencimento original foi 05/04/2025.<br>
                        <br>
                        Ambos os boletos devem ser pagos até o dia:
        <asp:TextBox ID="txtDataNovaOpcao" runat="server" TextMode="Date"
            CssClass="form-control campo-data" ClientIDMode="Static"
            onclick="event.stopPropagation();"></asp:TextBox><br>
                        <br>
                        Para evitar o cancelamento definitivo do seu plano.<br>
                        <br>
                        Gentileza conferir os dados dos boletos antes de realizar o pagamento.<br>
                        <br>
                        Caso tenha alguma dúvida, basta responder este e-mail ou entrar em contato pelos telefones abaixo.<br>
                        <br>
                        Atenciosamente.
                    </p>
                </div>



            </div>
        </div>
    </div>

    <div id="loadingOverlay" style="display: none; position: fixed; top: 0; left: 0; width: 100vw; height: 100vh; background: rgba(255,255,255,0.9); z-index: 1050; text-align: center; padding-top: 30vh;">
        <div class="spinner-border text-success" style="width: 50px; height: 50px;"></div>
        <div style="margin-top: 8px; font-size: 18px; color: #4CB07A;">Enviando mensagens...</div>
    </div>

    <div class="container-fluid p-4">
        <div class="card-container">
            <div class="card-header">
                <h5 class="mb-0">Envio de Mensagens</h5>
                <asp:Button ID="btnTestarApi" runat="server"
                    CssClass="btn btn-success btn-pill"
                    Text='Enviar mensagem'
                    OnClientClick="mostrarLoading();" OnClick="btnTestarApi_Click" Enabled="false" />
            </div>

            <asp:Label ID="lblResultado" runat="server" CssClass="text-muted d-block mb-3"></asp:Label>

            <div class="filter-panel row gx-2 gy-2">
                <div class="col-md-3">
                    <label class="form-label">Operadora:</label>
                    <asp:DropDownList ID="ddlOperadora" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Todas" Value="" />
                    </asp:DropDownList>
                </div>

                <div class="col-md-4">
                    <label class="form-label">De:</label>
                    <input type="date" id="txtDataInicio" runat="server" class="form-control" />
                </div>

                <div class="col-md-4">
                    <label class="form-label">Até:</label>
                    <input type="date" id="txtDataFim" runat="server" class="form-control" />
                </div>

                <div class="col-md-12">
                    <label class="form-label">&nbsp;</label>
                    <div class="botoes-acoes">
                        <asp:Button ID="btnFiltrar" runat="server"
                            CssClass="btn btn-info btn-pill"
                            Text="Filtrar" OnClick="btnFiltrar_Click" />

                        <asp:Button ID="btnEscMens" runat="server"
                            CssClass="btn btn-purple btn-pill"
                            Text="Escolher Mensagem"
                            OnClientClick="abrirModal(); return false;" />
                    </div>
                </div>
            </div>

            <asp:Literal ID="LiteralMensagem" runat="server"></asp:Literal>

            <div class="table-responsive">

                <div class="mb-2 d-flex justify-content-end align-items-center">
                    <label for="ddlPageSize" class="form-label me-2 mb-0">Itens por página:</label>
                    <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true"
                        CssClass="form-select w-auto" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
                        <asp:ListItem Text="15" Value="15" Selected="True" />
                        <asp:ListItem Text="30" Value="30" />
                        <asp:ListItem Text="50" Value="50" />
                        <asp:ListItem Text="100" Value="100" />
                        <asp:ListItem Text="200" Value="200" />
                        <asp:ListItem Text="300" Value="300" />
                    </asp:DropDownList>
                </div>

                <asp:GridView ID="GridAssociados" runat="server"
    AutoGenerateColumns="False"
    CssClass="table table-hover align-middle mb-0"
    ClientIDMode="Static"
    EmptyDataText="Nenhum registro encontrado."
     ShowFooter="true"
    PagerStyle-CssClass="pager-footer"
    PagerStyle-HorizontalAlign="Right"
    AllowPaging="true"
    PageSize="15"
    OnPageIndexChanging="GridAssociados_PageIndexChanging">
    <Columns>

        <asp:TemplateField HeaderText="">
            <HeaderTemplate>
                <div class="checkbox-header">
                    <asp:CheckBox ID="chkSelecionarTodos" runat="server" CssClass="form-check-input" onclick="selecionarTodos(this);" />
                    <span>Todos</span>
                </div>
            </HeaderTemplate>
            <ItemTemplate>
                <div>
                    <asp:CheckBox ID="chkSelecionar" runat="server" CssClass="form-check-input" onclick="alternarSelecao(this);" />
                </div>
            </ItemTemplate>
            <ItemStyle CssClass="col-selecao" />
            <HeaderStyle CssClass="col-selecao" />
        </asp:TemplateField>

        <asp:TemplateField HeaderText="Código">
            <ItemTemplate><asp:Label ID="lblCodigo" runat="server" Text='<%# Eval("CODIGO_ASSOCIADO") %>' /></ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Registro">
            <ItemTemplate><asp:Label ID="lblRegistro" runat="server" Text='<%# Eval("NUMERO_REGISTRO") %>' /></ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Associado">
            <ItemTemplate><asp:Label ID="lblNome" runat="server" Text='<%# Eval("NOME_ASSOCIADO") %>' /></ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Plano">
            <ItemTemplate><asp:Label ID="lblPlano" runat="server" Text='<%# Eval("NOME_PLANO_ABREVIADO") %>' /></ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Operadora">
            <ItemTemplate><asp:Label ID="lblOperadora" runat="server" Text='<%# Eval("NOME_OPERADORA") %>' /></ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Vencimento">
            <ItemTemplate><asp:Label ID="lblVencimento" runat="server" Text='<%# String.Format("{0:dd/MM/yyyy}", Eval("DATA_VENCIMENTO")) %>' /></ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Valor">
            <ItemTemplate><asp:Label ID="lblValor" runat="server" Text='<%# String.Format("R$ {0:N2}", Eval("VALOR_FATURA")) %>' /></ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Telefone">
            <ItemTemplate><asp:Label ID="lblTelefone" runat="server" Text='<%# Eval("NUMERO_TELEFONE") %>' /></ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

            </div>
        </div>
    </div>
</asp:Content>
