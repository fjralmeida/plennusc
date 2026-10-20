<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master"
    AutoEventWireup="true" CodeBehind="sendMessageFamiliarGroup.aspx.cs"
    Inherits="PlennuscApp.PlennuscGestao.Views.EnvioMensagemBeneficiario"
    Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Envio de Mensagens - Grupo Familiar</title>
    <link href="../../Content/Css/projects/gestao/structuresCss/sendMessageFamiliarGroup.css" rel="stylesheet" />
    
    <style>
        .texto-mensagem { white-space: pre-line; font-size: 13px; color: #444; line-height: 1.6; }
        .texto-mensagem strong { color: #2c3e50; font-weight: 700; }
        .input-data-card { max-width: 200px; margin: 15px 0; border-color: #4CB07A; }

        /* ESTILOS DO DROPDOWN COM FILTRO SIMPLES */
        .custom-dropdown { position: relative; width: 100%; }
        .dropdown-header { border: 1px solid #ced4da; border-radius: 8px; padding: 8px 12px; cursor: pointer; background: #fff; display: flex; justify-content: space-between; align-items: center; min-height: 36px; font-size: 13px; }
        .dropdown-panel { position: absolute; top: 100%; left: 0; right: 0; background: #fff; border: 1px solid #ced4da; border-radius: 8px; margin-top: 4px; z-index: 1000; display: none; box-shadow: 0 4px 12px rgba(0,0,0,0.1); }
        .custom-dropdown.open .dropdown-panel { display: block; }
        .dropdown-search { padding: 8px; border-bottom: 1px solid #e9ecef; }
        .dropdown-list { max-height: 250px; overflow-y: auto; padding: 4px 0; }
        .dropdown-list div { padding: 8px 12px; cursor: pointer; font-size: 13px; }
        .dropdown-list div:hover { background-color: #e6f5ee; color: #4CB07A; }
        .dropdown-list .placeholder { color: #999; font-size: 12px; font-style: italic; text-align: center; padding: 10px; }
    </style>

    <script>
        function abrirModal() { document.getElementById('modalEscolherMensagem').style.display = 'block'; }
        function fecharModal() { document.getElementById('modalEscolherMensagem').style.display = 'none'; }

        function selecionarTemplate(templateId, nomeAmigavel, inputId) {
            document.getElementById('<%= hfTemplateEscolhido.ClientID %>').value = templateId;
            var inputData = document.getElementById(inputId);
            if (inputData && inputData.value.trim() === "") {
                alert("Por favor, preencha a data para esta mensagem.");
                inputData.focus();
                return;
            }
            var lblMsg = document.getElementById('lblMsgEscolhida');
            lblMsg.innerText = "Mensagem selecionada: " + nomeAmigavel;
            lblMsg.style.display = 'inline-block';
            var btnEnviar = document.getElementById('<%= btnTestarApi.ClientID %>');
            if(btnEnviar) btnEnviar.disabled = false;
            fecharModal();
        }

        // LÓGICA DO DROPDOWN SIMPLES COM FILTRO INTERNO
        document.addEventListener("DOMContentLoaded", function () {
            var wrapper = document.getElementById('wrapperEntidades');
            var header = document.getElementById('dropdownHeaderEntidades');
            var panel = document.getElementById('dropdownPanelEntidades');
            var inputFiltro = document.getElementById('txtFiltroEntidades');
            var lista = document.getElementById('listaEntidades');
            var hiddenId = document.getElementById('<%= hfEntidadeId.ClientID %>');
            var labelResumo = document.getElementById('lblEntidadesResumo');

            // Abre e fecha o Dropdown
            header.addEventListener('click', function (e) {
                e.preventDefault();
                e.stopPropagation();
                var isOpen = wrapper.classList.contains('open');
                
                // Fecha outros dropdowns se houver
                document.querySelectorAll('.custom-dropdown').forEach(function(w) { w.classList.remove('open'); });
                
                if (!isOpen) {
                    wrapper.classList.add('open');
                    inputFiltro.value = ''; // limpa a busca ao abrir
                    filtrarLista(''); // reseta a lista
                    inputFiltro.focus();
                }
            });

            // Fecha ao clicar fora
            document.addEventListener('click', function (e) {
                if (!wrapper.contains(e.target)) {
                    wrapper.classList.remove('open');
                }
            });

            // Função de Filtro
            inputFiltro.addEventListener('input', function () {
                var termo = this.value.toLowerCase();
                
                // REGRA DOS 5 CARACTERES PARA MOSTRAR RESULTADOS
                if (termo.length > 0 && termo.length < 5) {
                    lista.innerHTML = '<div class="placeholder">Digite pelo menos 5 letras para buscar...</div>';
                    return;
                }
                
                filtrarLista(termo);
            });

            function filtrarLista(termo) {
                lista.innerHTML = '';
                var encontrou = false;

                // Pega todos os itens que foram renderizados pelo C# no início
                var todosItens = JSON.parse(document.getElementById('<%= hfTodosEntidades.ClientID %>').value || "[]");

                todosItens.forEach(function (item) {
                    var textoOriginal = item.Nome;
                    var textoMin = textoOriginal.toLowerCase();

                    // Se o termo estiver vazio, mostra tudo. Se tiver 5+ caracteres, filtra.
                    if (termo === "" || textoMin.includes(termo)) {
                        encontrou = true;
                        var div = document.createElement('div');
                        
                        // Destaca o texto pesquisado
                        if (termo !== "") {
                            var index = textoMin.indexOf(termo);
                            div.innerHTML = textoOriginal.substring(0, index) + 
                                            "<strong>" + textoOriginal.substring(index, index + termo.length) + "</strong>" + 
                                            textoOriginal.substring(index + termo.length);
                        } else {
                            div.innerText = textoOriginal;
                        }

                        // Ao clicar, seleciona o item
                        div.addEventListener('click', function () {
                            hiddenId.value = item.Codigo;
                            labelResumo.innerText = textoOriginal;
                            wrapper.classList.remove('open');
                        });
                        lista.appendChild(div);
                    }
                });

                if (!encontrou && termo !== "") {
                    lista.innerHTML = '<div class="placeholder">Nenhum grupo encontrado.</div>';
                }
            }
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid p-4">
        <div class="card-container">
            
            <div class="card-header d-flex justify-content-between align-items-center flex-wrap gap-3">
                <div>
                    <h5 class="mb-0 fw-bold text-primary">Envio de Mensagens - Grupo Familiar</h5>
                    <asp:Label ID="lblResultado" runat="server" CssClass="text-muted small d-block mt-1"></asp:Label>
                    <span id="lblMsgEscolhida" class="badge bg-purple mt-1" style="display:none;"></span>
                </div>
                <div class="d-flex gap-2">
                    <asp:Button ID="btnTestarApi" runat="server" CssClass="btn btn-success btn-pill" Text="Enviar Mensagem" OnClientClick="return validarQuantidadeMensagens();" OnClick="btnTestarApi_Click" Enabled="false" />
                </div>
            </div>

            <div class="filter-panel">
                <div class="filter-grid" style="grid-template-columns: repeat(auto-fit, minmax(180px, 1fr));">
                    
                    <div class="filter-group">
                        <label class="form-label fw-semibold text-primary">Tipo de Plano (Familiar) *</label>
                        <asp:DropDownList ID="ddlTipoPlano" runat="server" CssClass="form-select form-select-sm border-primary">
                            <asp:ListItem Text="Selecione..." Value="" />
                            <asp:ListItem Text="Adesão" Value="4" />
                            <asp:ListItem Text="Empresarial" Value="3" />
                        </asp:DropDownList>
                    </div>

                    <div class="filter-group">
                        <label class="form-label fw-semibold">Operadora:</label>
                        <asp:DropDownList ID="ddlOperadora" runat="server" CssClass="form-select form-select-sm"></asp:DropDownList>
                    </div>
                    
                    <!-- DROPDOWN COM FILTRO INTERNO -->
                    <div class="filter-group">
                        <label class="form-label fw-semibold">Grupo de Pessoas:</label>
                        <div class="custom-dropdown" id="wrapperEntidades">
                            <div class="dropdown-header" id="dropdownHeaderEntidades">
                                <span id="lblEntidadesResumo"><span class="text-muted">Todos</span></span>
                                <span class="arrow">▼</span>
                            </div>
                            <div class="dropdown-panel" id="dropdownPanelEntidades">
                                <div class="dropdown-search">
                                    <input type="text" id="txtFiltroEntidades" class="form-control form-control-sm" placeholder="Digite para filtrar..." autocomplete="off" />
                                </div>
                                <div class="dropdown-list" id="listaEntidades">
                                    <!-- Itens renderizados via JS -->
                                </div>
                            </div>
                        </div>
                        <!-- HiddenField para guardar o valor selecionado -->
                        <asp:HiddenField ID="hfEntidadeId" runat="server" />
                        <!-- HiddenField com TODAS as entidades para o JS ler rápido -->
                        <asp:HiddenField ID="hfTodosEntidades" runat="server" />
                    </div>
                    
                    <div class="filter-group">
                        <label class="form-label fw-semibold">Grupo de Faturamento:</label>
                        <asp:DropDownList ID="ddlFaturamento" runat="server" CssClass="form-select form-select-sm"></asp:DropDownList>
                    </div>

                    <div class="filter-group">
                        <label class="form-label fw-semibold">Vencimento De:</label>
                        <asp:TextBox ID="txtDataVencimentoInicio" runat="server" TextMode="Date" CssClass="form-control form-control-sm"></asp:TextBox>
                    </div>

                    <div class="filter-group">
                        <label class="form-label fw-semibold">Vencimento Até:</label>
                        <asp:TextBox ID="txtDataVencimentoFim" runat="server" TextMode="Date" CssClass="form-control form-control-sm"></asp:TextBox>
                    </div>

                    <div class="filter-group">
                        <label class="form-label fw-semibold">Envio Antecipado De:</label>
                        <asp:TextBox ID="txtEnvioInicio" runat="server" TextMode="Date" CssClass="form-control form-control-sm"></asp:TextBox>
                    </div>

                    <div class="filter-group">
                        <label class="form-label fw-semibold">Envio Antecipado Até:</label>
                        <asp:TextBox ID="txtEnvioFim" runat="server" TextMode="Date" CssClass="form-control form-control-sm"></asp:TextBox>
                    </div>
                </div>

                <div class="filter-actions mt-3">
                    <asp:Button ID="btnFiltrar" runat="server" CssClass="btn btn-info btn-pill" Text="Filtrar" OnClick="btnFiltrar_Click" />
                    <asp:Button ID="btnEscMens" runat="server" CssClass="btn btn-purple btn-pill" Text="Escolher Mensagem" OnClientClick="abrirModal(); return false;" />
                </div>
            </div>

            <asp:HiddenField ID="hfTemplateEscolhido" runat="server" />
            <asp:HiddenField ID="hfTodosCodigos" runat="server" />
            <asp:HiddenField ID="hfSelecionados" runat="server" />

            <!-- CONTADORES -->
            <div class="d-flex justify-content-between align-items-center mb-3 flex-wrap gap-2">
                <div class="d-flex align-items-center gap-3">
                    <div class="d-flex align-items-center gap-2 bg-light px-3 py-2 rounded">
                        <i class="fas fa-database text-primary"></i>
                        <div>
                            <div class="small text-muted">Total de registros</div>
                            <div class="fw-bold"><asp:Label ID="lblTotalRegistros" runat="server" Text="0"></asp:Label></div>
                        </div>
                    </div>
                </div>
                <div class="d-flex align-items-center gap-2">
                    <label class="form-label mb-0 small">Itens por página:</label>
                    <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" CssClass="form-select form-select-sm w-auto" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
                        <asp:ListItem Text="10" Value="10" Selected="True" />
                        <asp:ListItem Text="50" Value="50" />
                        <asp:ListItem Text="100" Value="100" />
                    </asp:DropDownList>
                </div>
            </div>

            <!-- GRID -->
            <div class="table-responsive">
                <asp:GridView ID="GridAssociados" runat="server" AutoGenerateColumns="False" CssClass="table table-hover align-middle mb-0" ClientIDMode="Static" EmptyDataText="Nenhum registro encontrado." ShowFooter="true" PagerStyle-CssClass="pager-footer" AllowPaging="true" PageSize="10" OnPageIndexChanging="GridAssociados_PageIndexChanging" OnPreRender="GridAssociados_PreRender" OnRowDataBound="GridAssociados_RowDataBound">
                    <Columns>
                        <asp:TemplateField HeaderText="">
                            <HeaderTemplate>
                                <div class="checkbox-header">
                                    <asp:CheckBox ID="chkSelecionarTodos" runat="server" CssClass="form-check-input" onclick="selecionarTodosGlobal(this);" />
                                </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <asp:CheckBox ID="chkSelecionar" runat="server" CssClass="form-check-input" onclick="alternarSelecaoGlobal(this);" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Código"><ItemTemplate><asp:Label ID="lblCodigo" runat="server" Text='<%# Eval("CodigoAssociado") %>' /></ItemTemplate></asp:TemplateField>
                        <asp:TemplateField HeaderText="Titular / Empresa"><ItemTemplate><asp:Label ID="lblNomeTitular" runat="server" Text='<%# Eval("NomeTitular") %>' /></ItemTemplate></asp:TemplateField>
                        <asp:TemplateField HeaderText="CPF / CNPJ"><ItemTemplate><asp:Label ID="lblCpfCnpj" runat="server" Text='<%# Eval("CpfCnpj") %>' /></ItemTemplate></asp:TemplateField>
                        <asp:TemplateField HeaderText="Operadora"><ItemTemplate><asp:Label ID="lblOperadora" runat="server" Text='<%# Eval("GrupoContrato") %>' /></ItemTemplate></asp:TemplateField>
                        <asp:TemplateField HeaderText="Faturamento"><ItemTemplate><asp:Label ID="lblFaturamento" runat="server" Text='<%# Eval("GrupoFaturamento") %>' /></ItemTemplate></asp:TemplateField>
                        <asp:TemplateField HeaderText="Grupo Pessoas"><ItemTemplate><asp:Label ID="lblGrupoPessoas" runat="server" Text='<%# Eval("NomeGrupoPessoas") %>' /></ItemTemplate></asp:TemplateField>
                        <asp:TemplateField HeaderText="Vencimento"><ItemTemplate><asp:Label ID="lblVencimento" runat="server" Text='<%# String.Format("{0:dd/MM/yyyy}", Eval("DataVencimento")) %>' /></ItemTemplate></asp:TemplateField>
                        <asp:TemplateField HeaderText="Valor"><ItemTemplate><asp:Label ID="lblValor" runat="server" Text='<%# String.Format("R$ {0:N2}", Eval("ValorBoleto")) %>' /></ItemTemplate></asp:TemplateField>
                        <asp:TemplateField HeaderText="Nº Boleto"><ItemTemplate><asp:Label ID="lblNumeroBoleto" runat="server" Text='<%# Eval("NumeroRegistroBoleto") %>' /></ItemTemplate></asp:TemplateField>
                        <asp:TemplateField HeaderText="E-mail"><ItemTemplate><asp:Label ID="lblEmail" runat="server" Text='<%# Eval("Email") %>' /></ItemTemplate></asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>

    <!-- MODAL -->
    <div id="modalEscolherMensagem" style="display:none; position:fixed; z-index:1055; left:0; top:0; width:100vw; height:100vh; background:rgba(0,0,0,0.5); padding:60px 20px;">
        <div class="modal-content" style="background:#fff; border-radius:16px; max-width:900px; margin:auto; padding:24px;">
            <div class="modal-header d-flex justify-content-between bg-success text-white rounded-top p-3">
                <h2>Escolher Mensagem</h2>
                <span class="close" style="cursor:pointer;" onclick="fecharModal()">&times;</span>
            </div>
            <div class="modal-body p-3" style="max-height: 70vh; overflow-y: auto;">
                
                <div class="template-opcao p-3 mb-3 border rounded" onclick="selecionarTemplate('MSG_1', 'MSG 1 - Envio Antecipado', 'txtDataMsg1')" style="cursor:pointer;">
                    <h4>[MSG 1] - Envio Antecipado</h4>
                    <div class="texto-mensagem">
Boa tarde!
Prezado(a) Sr.(a) <strong>{{NOME}}</strong>,
Tudo bem?
Esperamos que sim!
Para sua comodidade, segue em anexo o boleto referente à mensalidade de <strong>{{MES}}</strong> do seu plano de saúde da operadora <strong>{{OPERADORA}}</strong>, com vencimento em <strong>{{VENCIMENTO}}</strong>.
Atenciosamente,
                    </div>
                    <div class="mt-2">
                        <label class="small text-muted">Data de Vencimento:</label>
                        <asp:TextBox ID="txtDataMsg1" runat="server" TextMode="Date" CssClass="form-control input-data-card" onclick="event.stopPropagation();"></asp:TextBox>
                    </div>
                </div>
                
                <div class="template-opcao p-3 mb-3 border rounded" onclick="selecionarTemplate('MSG_2', 'MSG 2 - Atraso / Suspensão', 'txtDataMsg2')" style="cursor:pointer;">
                    <h4>[MSG 2] - Atraso / Suspensão</h4>
                    <div class="texto-mensagem">
Olá, <strong>{{NOME}}</strong>! Esperamos que esteja tudo bem.
Você é um dos nossos melhores clientes e identificamos que o pagamento no valor de R$ <strong>{{VALOR}}</strong>, referente à mensalidade do mês de <strong>{{MES}}</strong> do seu plano <strong>{{PLANO}}</strong>, ainda não foi identificado em nosso sistema.
Informamos que, a partir do dia <strong>{{DATA_SUSPENSAO}}</strong>, o serviço contratado poderá ser cancelado.
Atenciosamente,
                    </div>
                    <div class="mt-2">
                        <label class="small text-muted">Data de Suspensão:</label>
                        <asp:TextBox ID="txtDataMsg2" runat="server" TextMode="Date" CssClass="form-control input-data-card" onclick="event.stopPropagation();"></asp:TextBox>
                    </div>
                </div>

                <div class="template-opcao p-3 mb-3 border rounded" onclick="selecionarTemplate('MSG_4', 'MSG 4 - Definitivo (1 Boleto)', 'txtDataMsg4')" style="cursor:pointer;">
                    <h4>[MSG 4] - Definitivo (1 Boleto)</h4>
                    <div class="texto-mensagem">
Olá, <strong>{{NOME}}</strong>! Esperamos que esteja tudo bem.
Você é um dos nossos melhores clientes e identificamos que o pagamento no valor de R$ <strong>{{VALOR}}</strong>, referente à mensalidade do mês de <strong>{{MES}}</strong> do seu plano <strong>{{PLANO}}</strong>, ainda não foi identificado em nosso sistema.
Informamos que, a partir do dia <strong>{{DATA_SUSPENSAO}}</strong>, o serviço contratado poderá ser cancelado em definitivo.
Atenciosamente,
                    </div>
                    <div class="mt-2">
                        <label class="small text-muted">Data de Suspensão Definitiva:</label>
                        <asp:TextBox ID="txtDataMsg4" runat="server" TextMode="Date" CssClass="form-control input-data-card" onclick="event.stopPropagation();"></asp:TextBox>
                    </div>
                </div>

                <div class="template-opcao p-3 mb-3 border rounded" onclick="selecionarTemplate('MSG_5', 'MSG 5 - Definitivo (2 Boletos)', 'txtDataMsg5')" style="cursor:pointer;">
                    <h4>[MSG 5] - Definitivo (2 Boletos)</h4>
                    <div class="texto-mensagem">
Boa tarde!
Prezado(a) Sr.(a) <strong>{{NOME}}</strong>,
Para sua comodidade, seguem anexos os boletos referentes às mensalidades de <strong>{{MES1}}</strong> e <strong>{{MES2}}</strong>.
Informamos que ambos os boletos deverão ser pagos até o dia <strong>{{DATA_SUSPENSAO}}</strong>, a fim de evitar o cancelamento definitivo do seu plano de saúde.
Atenciosamente,
                    </div>
                    <div class="mt-2">
                        <label class="small text-muted">Data Limite para Pagamento:</label>
                        <asp:TextBox ID="txtDataMsg5" runat="server" TextMode="Date" CssClass="form-control input-data-card" onclick="event.stopPropagation();"></asp:TextBox>
                    </div>
                </div>

            </div>
        </div>
    </div>
</asp:Content>