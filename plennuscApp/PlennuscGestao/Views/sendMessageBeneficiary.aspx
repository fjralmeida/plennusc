<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master"
    AutoEventWireup="true" CodeBehind="sendMessageBeneficiary.aspx.cs"
    Inherits="PlennuscApp.PlennuscGestao.Views.EnvioMensagemBeneficiario"
    Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Enviar Mensagem ao Beneficiário</title>

    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />

    <link href="../../Content/Css/projects/gestao/structuresCss/sendMessageBeneficiary.css" rel="stylesheet" />

<script>
    // ============================================
    // FUNÇÕES ORIGINAIS (MANTÉM TUDO FUNCIONANDO)
    // ============================================

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
        atualizarContadorSelecionados();
    }

    function selecionarTodos(chkHeader) {
        var grid = document.getElementById("GridAssociados");
        var checkboxes = grid.querySelectorAll(
            "input[type='checkbox'][id*='chkSelecionar']:not(:disabled)"
        );

        checkboxes.forEach(function (chk) {
            chk.checked = chkHeader.checked;
            var row = chk.closest('tr');
            if (chk.checked) {
                row.classList.add('linha-selecionada');
            } else {
                row.classList.remove('linha-selecionada');
            }
        });

        atualizarContadorSelecionados();
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

    function mostrarLoading() {
        document.getElementById('loadingOverlay').style.display = 'block';
    }

    function mostrarResultadoModal(texto) {
        document.getElementById("modalResultadoConteudo").textContent = texto;
        var modal = new bootstrap.Modal(document.getElementById('resultadoModal'));
        modal.show();
    }

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

    const mapInputPorTemplate = {
        "Suspensao": "txtDataSuspensao",
        "Definitivo": "txtDataDefinitivo",
        "aVencer": "txtDataVencer"
    };

    function selecionarTemplate(template) {
        const inputId = mapInputPorTemplate[template];
        const input = document.getElementById(inputId);

        if (!input) {
            document.getElementById('<%= hfTemplateEscolhido.ClientID %>').value = template;
            fecharModal();
            return;
        }

        input.classList.remove('is-invalid');
        const val = (input.value || "").trim();
        if (!val) {
            input.classList.add('is-invalid');
            input.focus();
            input.scrollIntoView({ behavior: 'smooth', block: 'center' });
            return;
        }

        document.getElementById('<%= hfTemplateEscolhido.ClientID %>').value = template;
        fecharModal();
    }

    function atualizarContadorSelecionados() {
        var grid = document.getElementById("GridAssociados");
        if (!grid) return;

        var checkboxes = grid.querySelectorAll(
            "input[type='checkbox'][id*='chkSelecionar']:not(:disabled)"
        );

        var totalSelecionados = 0;
        checkboxes.forEach(function (chk) {
            if (chk.checked) totalSelecionados++;
        });

        document.getElementById("lblSelecionados").innerText = totalSelecionados;
    }

    // ============================================
    // FUNÇÕES NOVAS PARA SELECIONAR TODAS AS PÁGINAS
    // ============================================

    // Variável global para armazenar TODOS os códigos da consulta
    var todosCodigosConsulta = [];

    // Função para coletar TODOS os códigos do grid
    function coletarTodosCodigos() {
        var grid = document.getElementById("GridAssociados");
        if (!grid) return [];

        var codigos = [];
        var codigosElements = grid.querySelectorAll('[id*="lblCodigo"]');

        for (var i = 0; i < codigosElements.length; i++) {
            var codigo = codigosElements[i].innerText.trim();
            if (codigo && codigo !== '') {
                codigos.push(codigo);
            }
        }

        return codigos;
    }

    // Função para SALVAR TODAS as seleções (usada quando clica em "Selecionar Todos")
    function salvarSelecoesGlobais(selecionar) {
        // Pega o HiddenField
        var hfSelecionados = document.getElementById('<%= hfSelecionados.ClientID %>');
        if (!hfSelecionados) return;
        
        // Se está selecionando TODOS
        if (selecionar) {
            // Coleta TODOS os códigos da página atual
            var codigosPagina = coletarTodosCodigos();
            
            // Se tem o HiddenField com todos os códigos, usa ele
            var hfTodosCodigos = document.getElementById('<%= hfTodosCodigos.ClientID %>');
            if (hfTodosCodigos && hfTodosCodigos.value) {
                // Usa TODOS os códigos da consulta completa
                hfSelecionados.value = hfTodosCodigos.value;
            } else {
                // Usa os códigos da página atual (fallback)
                hfSelecionados.value = codigosPagina.join(',');
            }
        } else {
            // Se está DESMARCADO, limpa tudo
            hfSelecionados.value = '';
        }
        
        // Atualiza contador
        atualizarContadorSelecionadosGlobais();
    }
    
    // Atualiza contador baseado no HiddenField (todas as páginas)
    function atualizarContadorSelecionadosGlobais() {
        var hfSelecionados = document.getElementById('<%= hfSelecionados.ClientID %>');
        if (!hfSelecionados) return;
        
        var total = 0;
        if (hfSelecionados.value && hfSelecionados.value.trim() !== '') {
            total = hfSelecionados.value.split(',').length;
        }
        
        document.getElementById("lblSelecionados").innerText = total;
    }
    
    // NOVA FUNÇÃO para "Selecionar Todos" que seleciona TODAS as páginas
    function selecionarTodosGlobal(chkHeader) {
        // Primeiro executa a função normal (para marcar na página atual)
        selecionarTodos(chkHeader);
        
        // Depois salva no HiddenField para todas as páginas
        salvarSelecoesGlobais(chkHeader.checked);
    }
    
    // NOVA FUNÇÃO para seleção individual que salva no HiddenField
    function alternarSelecaoGlobal(chk) {
        // Executa a função normal
        alternarSelecao(chk);
        
        // Pega o código do registro
        var row = chk.closest('tr');
        var codigoElement = row.querySelector('[id*="lblCodigo"]');
        var codigo = codigoElement ? codigoElement.innerText.trim() : '';
        
        if (!codigo) return;
        
        // Pega o HiddenField
        var hfSelecionados = document.getElementById('<%= hfSelecionados.ClientID %>');
        if (!hfSelecionados) return;
        
        // Pega as seleções atuais
        var selecionados = [];
        if (hfSelecionados.value && hfSelecionados.value.trim() !== '') {
            selecionados = hfSelecionados.value.split(',');
        }
        
        // Adiciona ou remove o código
        if (chk.checked) {
            if (!selecionados.includes(codigo)) {
                selecionados.push(codigo);
            }
        } else {
            var index = selecionados.indexOf(codigo);
            if (index > -1) {
                selecionados.splice(index, 1);
            }
        }
        
        // Atualiza o HiddenField
        hfSelecionados.value = selecionados.join(',');
        
        // Atualiza contador global
        atualizarContadorSelecionadosGlobais();
    }
    
    // Função para RESTAURAR seleções quando carrega a página
    function restaurarSelecoes() {
        var hfSelecionados = document.getElementById('<%= hfSelecionados.ClientID %>');
        if (!hfSelecionados || !hfSelecionados.value || hfSelecionados.value.trim() === '') {
            return;
        }

        var selecionados = hfSelecionados.value.split(',');
        var grid = document.getElementById("GridAssociados");
        if (!grid) return;

        var checkboxes = grid.querySelectorAll("input[type='checkbox'][id*='chkSelecionar']");

        for (var i = 0; i < checkboxes.length; i++) {
            var chk = checkboxes[i];
            var row = chk.closest('tr');
            var codigoElement = row.querySelector('[id*="lblCodigo"]');
            var codigo = codigoElement ? codigoElement.innerText.trim() : '';

            if (codigo && selecionados.includes(codigo)) {
                chk.checked = true;
                row.classList.add('linha-selecionada');
            } else {
                chk.checked = false;
                row.classList.remove('linha-selecionada');
            }
        }

        // Atualiza contador
        atualizarContadorSelecionadosGlobais();
    }

    // ============================================
    // INICIALIZAÇÃO
    // ============================================

    document.addEventListener("DOMContentLoaded", function () {
        // Inicializa contador
        atualizarContadorSelecionadosGlobais();

        // Restaura seleções salvas
        restaurarSelecoes();
    });
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Bootstrap JS carregado no final para garantir que o DOM esteja pronto -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>

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
                <!-- BOTÃO DE DOWNLOAD EXCEL (AGORA DENTRO DO MODAL) -->
                <asp:Button ID="btnDownloadExcel" runat="server"
                    CssClass="btn btn-success btn-pill me-2"
                    Text="Baixar Relatório Excel"
                    OnClick="btnDownloadExcel_Click" />
                
                <button type="button" class="btn btn-secondary btn-pill" data-bs-dismiss="modal">
                    <i class="fa-solid fa-xmark me-1"></i>Fechar
                </button>
            </div>
        </div>
    </div>
</div>

    <%--MODAL TEMPLETE MENSAGEM--%>

    <asp:HiddenField ID="hfTemplateEscolhido" runat="server" />

<asp:HiddenField ID="hfTodosCodigos" runat="server" />
<asp:HiddenField ID="hfSelecionados" runat="server" />
    <div id="modalEscolherMensagem">
        <div class="modal-content">
            <div class="modal-header">
                <h2>Escolher Template de Mensagem</h2>
                <span class="close" onclick="fecharModal()">&times;</span>
            </div>

            <div class="modal-body">

                <!-- APENAS BOLETO -->
               <div class="template-opcao" onclick="selecionarTemplate('Suspensao')">
                    <h4>[BOLETO]</h4>
                    <p>
                        Olá, {{1}}.<br>
                        <br>
                        Esperamos que esteja bem.<br>
                        <br>
                        Informamos que o boleto referente ao pagamento do seu plano de saúde já está disponível.<br>
                        <br>
                        Para sua comodidade, confira as informações do seu plano:
                    </p>

                    <div class="mb-2">
                        <!-- MUDEI O ID PARA txtDataVencer -->
                        <asp:TextBox ID="txtDataSuspensao" runat="server" TextMode="Date"
                            CssClass="form-control campo-data"
                            ClientIDMode="Static"
                            onclick="event.stopPropagation();"></asp:TextBox>
                        <div class="invalid-feedback">Informe a data antes de escolher o template.</div>
                    </div>

                    <p>
                        PLANO: {{2}}<br>
                        VENCIMENTO: {{3}}<br>
                        VALOR: {{4}}<br>
                        <br>
                        Em caso de dúvidas ou necessidade de apoio, nossa equipe permanece à disposição.
                    </p>
                </div>

                    <!-- APENAS NOTA FISCAL -->
                 <div class="template-opcao" onclick="selecionarTemplate('Definitivo')">
                    <h4>[NOTA FISCAL]</h4>
                    <p>
                        Olá, {{1}}.<br>
                        <br>
                        Esperamos que esteja bem.<br>
                        <br>
                        Informamos que a NOTA FISCAL referente ao pagamento do seu plano de saúde já está disponível.<br>
                        <br>
                        Para sua comodidade, confira as informações do seu plano:
                    </p>

                    <div class="mb-2">
                        <!-- MUDEI O ID PARA txtDataVencer -->
                        <asp:TextBox ID="txtDataDefinitivo" runat="server" TextMode="Date"
                            CssClass="form-control campo-data"
                            ClientIDMode="Static"
                            onclick="event.stopPropagation();"></asp:TextBox>
                        <div class="invalid-feedback">Informe a data antes de escolher o template.</div>
                    </div>

                    <p>
                        PLANO: {{2}}<br>
                        VENCIMENTO: {{3}}<br>
                        VALOR: {{4}}<br>
                        <br>
                        Em caso de dúvidas ou necessidade de apoio, nossa equipe permanece à disposição.
                    </p>
                </div>
 
             <%--   <!-- DOIS BOLETOS -->
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
                    </p>

                    <div class="mb-2">
                        <asp:TextBox ID="txtDataNovaOpcao" runat="server" TextMode="Date"
                            CssClass="form-control campo-data"
                            ClientIDMode="Static"
                            onclick="event.stopPropagation();"></asp:TextBox>
                        <div class="invalid-feedback">Informe a data antes de escolher o template.</div>
                    </div>

                    <p>
                        Para evitar o cancelamento definitivo do seu plano.<br>
                        <br>
                        Gentileza conferir os dados dos boletos antes de realizar o pagamento.<br>
                        <br>
                        Caso tenha alguma dúvida, basta responder este e-mail ou entrar em contato pelos telefones abaixo.<br>
                        <br>
                        Atenciosamente.
                    </p>
                </div>--%>

                <!-- TEMPLATE: PAGAMENTO A VENCER -->
                 <div class="template-opcao" onclick="selecionarTemplate('aVencer')">
                     <h4>[BOLETO E NOTA FISCAL]</h4>
                     <p>
                         Olá, {{1}}.<br>
                         <br>
                         Esperamos que esteja bem.<br>
                         <br>
                         Informamos que o boleto e nota fiscal referente ao pagamento do seu plano de saúde já está disponível.<br>
                         <br>
                         Para sua comodidade, confira as informações do seu plano:
                     </p>

                     <div class="mb-2">
                         <!-- MUDEI O ID PARA txtDataVencer -->
                         <asp:TextBox ID="txtDataVencer" runat="server" TextMode="Date"
                             CssClass="form-control campo-data"
                             ClientIDMode="Static"
                             onclick="event.stopPropagation();"></asp:TextBox>
                         <div class="invalid-feedback">Informe a data antes de escolher o template.</div>
                     </div>

                     <p>
                         PLANO: {{2}}<br>
                         VENCIMENTO: {{3}}<br>
                         VALOR: {{4}}<br>
                         <br>
                         Em caso de dúvidas ou necessidade de apoio, nossa equipe permanece à disposição.
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

<%--                    <a href="viewShippingHistory" class="btn btn-info btn-pill">
                        <i class="fa-solid fa-history me-2"></i>Ver Histórico
                    </a>--%>
            </div>

            <asp:Label ID="lblResultado" runat="server" CssClass="text-muted d-block mb-3"></asp:Label>

            <div class="filter-panel row gx-2 gy-2">
                <div class="col-md-3">
                    <label class="form-label">Operadora:</label>
                    <asp:DropDownList ID="ddlOperadora" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Todas" Value="" />
                    </asp:DropDownList>
                </div>

                <!-- ADICIONE ESTE NOVO FILTRO -->
                <div class="col-md-3">
                    <label class="form-label">Status de Envio:</label>
                    <asp:DropDownList ID="ddlFiltroEnvio" runat="server" CssClass="form-select">
                        <asp:ListItem Text="Disponíveis para envio" Value="disponiveis" Selected="True" />
                        <asp:ListItem Text="Já enviados (últimas 24h)" Value="enviados24h" />
                        <asp:ListItem Text="Todos" Value="todos" />
                    </asp:DropDownList>
                </div>

                <div class="col-md-3">
                    <label class="form-label">De:</label>
                    <input type="date" id="txtDataInicio" runat="server" class="form-control" />
                </div>

                <div class="col-md-3">
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

            <div class="d-flex justify-content-between align-items-center mb-2">
    <div>
        <strong>Total de registros:</strong>
        <asp:Label ID="lblTotalRegistros" runat="server" Text="0"></asp:Label>
    </div>

    <div>
        <strong>Selecionados:</strong>
        <span id="lblSelecionados">0</span>
    </div>
</div>

            <div class="table-responsive">

                <div class="mb-2 d-flex justify-content-end align-items-center">
                    <label for="ddlPageSize" class="form-label me-2 mb-0">Itens por página:</label>
                   <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true"
    CssClass="form-select w-auto" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged">
    <asp:ListItem Text="6" Value="6" Selected="True" />
    <asp:ListItem Text="30" Value="30" />
    <asp:ListItem Text="50" Value="50" />
    <asp:ListItem Text="100" Value="100" />
    <asp:ListItem Text="200" Value="200" />
    <asp:ListItem Text="300" Value="300" />
    <asp:ListItem Text="500" Value="500" />
    <asp:ListItem Text="1000" Value="1000" />
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
    PageSize="6"
    OnPageIndexChanging="GridAssociados_PageIndexChanging"
    OnPreRender="GridAssociados_PreRender"
    OnRowDataBound="GridAssociados_RowDataBound">
                    <Columns>

                        <asp:TemplateField HeaderText="">
                            <HeaderTemplate>
                                <div class="checkbox-header">
<asp:CheckBox ID="chkSelecionarTodos" runat="server"
    CssClass="form-check-input"
    onclick="selecionarTodosGlobal(this);" />
                                    <span>Todos</span>
                                </div>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <div>
                                    <asp:CheckBox ID="chkSelecionar" runat="server" 
                                        CssClass="form-check-input" 
                                       onclick="alternarSelecaoGlobal(this);"
                                        Enabled='<%# !Convert.ToBoolean(Eval("JA_ENVIADO_24H")) && ddlFiltroEnvio.SelectedValue != "enviados24h" %>' />
                                </div>
                            </ItemTemplate>
                            <ItemStyle CssClass="col-selecao" />
                            <HeaderStyle CssClass="col-selecao" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Código">
                            <ItemTemplate>
                                <asp:Label ID="lblCodigo" runat="server" Text='<%# Eval("CODIGO_ASSOCIADO") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
        
                        <asp:TemplateField HeaderText="Registro">
                            <ItemTemplate>
                                <asp:Label ID="lblRegistro" runat="server" Text='<%# Eval("NUMERO_REGISTRO") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
        
                        <asp:TemplateField HeaderText="Associado">
                            <ItemTemplate>
                                <asp:Label ID="lblNome" runat="server" Text='<%# Eval("NOME_ASSOCIADO") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
        
                        <asp:TemplateField HeaderText="Plano">
                            <ItemTemplate>
                                <asp:Label ID="lblPlano" runat="server" Text='<%# Eval("NOME_PLANO_ABREVIADO") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
        
                        <asp:TemplateField HeaderText="Operadora">
                            <ItemTemplate>
                                <asp:Label ID="lblOperadora" runat="server" Text='<%# Eval("DESCRICAO_GRUPO_CONTRATO") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
        
                        <asp:TemplateField HeaderText="Vencimento">
                            <ItemTemplate>
                                <asp:Label ID="lblVencimento" runat="server" 
                                    Text='<%# String.Format("{0:dd/MM/yyyy}", Eval("DATA_VENCIMENTO")) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
        
                        <asp:TemplateField HeaderText="Valor">
                            <ItemTemplate>
                                <asp:Label ID="lblValor" runat="server" 
                                    Text='<%# String.Format("R$ {0:N2}", Eval("VALOR_FATURA")) %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
        
                        <asp:TemplateField HeaderText="Telefone">
                            <ItemTemplate>
                                <asp:Label ID="lblTelefone" runat="server" Text='<%# Eval("NUMERO_TELEFONE") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
        
                   <asp:TemplateField HeaderText="Status Envio">
                    <ItemTemplate>
                        <span class='<%# Convert.ToBoolean(Eval("JA_ENVIADO_24H")) ? "status-enviado" : "status-disponivel" %>'>
                            <%# Convert.ToBoolean(Eval("JA_ENVIADO_24H")) ? "Enviado 24h" : "Disponível" %>
                        </span>
                    </ItemTemplate>
                    <ItemStyle Width="120px" />
                </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
