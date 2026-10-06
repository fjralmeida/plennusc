<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" AutoEventWireup="true" CodeBehind="demaisEntidadesTaxaAssociativa.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.demaisEntidadesTaxaAssociativa" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/Css/projects/gestao/structuresCss/taxasAssociativas/taxasAssociativas.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div id="overlayCarregando" class="overlay-carregando">
        <div class="overlay-carregando-card">
            <div class="overlay-spinner"></div>
            <p class="overlay-texto">Processando, aguarde...</p>
        </div>
    </div>

    <div class="container-main">

        <div class="page-header">
            <h1 class="page-title">
                <span class="title-icon"><i class="bi bi-people-fill"></i></span>
                Taxas Associativas - Demais Entidades
            </h1>
        </div>

        <!-- CARD 1: FILTROS -->
        <div class="filters-card">
            <div class="filters-title"><i class="bi bi-search"></i>Filtros de Pesquisa</div>

            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Entidade *</label>
                    <div class="custom-dropdown" id="dropdownEntidades">
                        <!-- Sem onclick: o listener é registrado no DOMContentLoaded -->
                        <div class="dropdown-header" id="dropdownHeaderEntidades">
                            <span class="resumo-texto" id="lblEntidadesResumo">
                                <span class="placeholder">Selecione...</span>
                            </span>
                            <span class="arrow">▼</span>
                        </div>
                        <div class="dropdown-panel">
                            <!-- Ações rápidas -->
                            <div class="dropdown-acoes">
                                <a href="javascript:void(0);"
                                   onclick="selecionarTodos(true); event.stopPropagation();">Selecionar todos</a>
                                <a href="javascript:void(0);"
                                   onclick="selecionarTodos(false); event.stopPropagation();">Limpar</a>
                            </div>

                            <asp:CheckBoxList ID="cblEntidades" runat="server" ClientIDMode="Static" />
                        </div>
                    </div>
                </div>

                <div class="form-group">
                    <label for="txtDataInicio" class="form-label">Data Pagamento - De *</label>
                    <asp:TextBox ID="txtDataInicio" runat="server" TextMode="Date" CssClass="form-control" />
                </div>

                <div class="form-group">
                    <label for="txtDataFim" class="form-label">Data Pagamento - Até *</label>
                    <asp:TextBox ID="txtDataFim" runat="server" TextMode="Date" CssClass="form-control" />
                </div>

                <div class="form-group" style="max-width: none; flex: 0;">
                    <asp:Button ID="btnProcessar" runat="server" Text="Processar"
                        CssClass="btn btn-primary"
                        OnClientClick="return mostrarOverlayCarregando();"
                        OnClick="btnProcessar_Click" />
                </div>
            </div>

            <asp:Label ID="lblMensagem" runat="server" CssClass="msg-importacao" />
        </div>

        <!-- CARD 2: RESULTADO -->
        <div class="filters-card" id="divResultado" runat="server" visible="false">
            <div class="filters-title"><i class="bi bi-table"></i>Resultado</div>

            <div class="grid-container">

                <div class="grid-toolbar">
                    <div class="grid-toolbar-left">
                        <label for="ddlPageSize">Registros por página:</label>
                        <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true"
                            OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged"
                            CssClass="form-control ddl-pagesize">
                            <asp:ListItem Text="5" Value="5" />
                            <asp:ListItem Text="10" Value="10" Selected="True" />
                            <asp:ListItem Text="20" Value="20" />
                            <asp:ListItem Text="50" Value="50" />
                            <asp:ListItem Text="100" Value="100" />
                        </asp:DropDownList>
                    </div>
                    <div class="grid-toolbar-right">
                        <span class="resultado-total">
                            <strong><asp:Literal ID="litTotalRegistros" runat="server" /></strong> registro(s) encontrado(s)
                            &nbsp;|&nbsp;
                            Valor Total: <strong><asp:Literal ID="litTotalValor" runat="server" /></strong>
                        </span>
                        <asp:Button ID="btnExportarExcel" runat="server" Text="Exportar Excel"
                            CssClass="btn btn-success" OnClick="btnExportarExcel_Click" />
                    </div>
                </div>

                <asp:GridView ID="gvResultado" runat="server" AutoGenerateColumns="false"
                    CssClass="custom-grid" GridLines="None"
                    EmptyDataText="Nenhum registro encontrado."
                    AllowPaging="True" PageSize="10"
                    OnPageIndexChanging="gvResultado_PageIndexChanging"
                    OnDataBound="gvResultado_DataBound">
                    <PagerSettings Mode="NumericFirstLast"
                        FirstPageText="«" LastPageText="»"
                        PageButtonCount="7"
                        Position="Bottom" />
                    <PagerStyle CssClass="pager-custom" />
                    <Columns>
                        <asp:BoundField DataField="NumeroRegistroPs1020" HeaderText="Nº Registro" ItemStyle-CssClass="col-curta" />
                        <asp:BoundField DataField="NomeBeneficiario" HeaderText="Beneficiário" ItemStyle-CssClass="col-nome" />
                        <asp:BoundField DataField="TipoContratacaoAns" HeaderText="Tipo" ItemStyle-CssClass="col-normal" />
                        <asp:BoundField DataField="DataPagamento" HeaderText="Data Pagamento"
                            DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="col-curta" />
                        <asp:BoundField DataField="ValorEvento" HeaderText="Valor Taxa"
                            DataFormatString="{0:N2}" ItemStyle-CssClass="col-curta" />
                    </Columns>
                </asp:GridView>

                <asp:Label ID="lblPagerInfo" runat="server" CssClass="pager-info" />
            </div>
        </div>

    </div>

    <script>
        // Overlay de carregamento
        function mostrarOverlayCarregando() {
            document.getElementById('overlayCarregando').style.display = 'flex';
            document.body.style.overflow = 'hidden';

            setTimeout(function () {
                var botoes = document.querySelectorAll('button, input[type="submit"]');
                botoes.forEach(function (b) { b.disabled = true; });
            }, 0);

            return true;
        }

        // Dropdown custom (multiselect com checkboxes)
        function toggleDropdown(header) {
            var wrapper = header.parentElement;
            var isOpen = wrapper.classList.contains('open');

            document.querySelectorAll('.custom-dropdown.open').forEach(function (w) {
                if (w !== wrapper) w.classList.remove('open');
            });

            wrapper.classList.toggle('open', !isOpen);
        }

        // Resumo das entidades selecionadas (tags no header)
        function atualizarResumoEntidades() {
            var checkboxes = document.querySelectorAll('#cblEntidades input[type="checkbox"]:checked');
            var label = document.getElementById('lblEntidadesResumo');
            label.innerHTML = '';

            if (checkboxes.length === 0) {
                label.innerHTML = '<span class="placeholder">Selecione...</span>';
                return;
            }

            checkboxes.forEach(function (chk) {
                var nome = chk.closest('td').textContent.trim();
                var tag = document.createElement('span');
                tag.className = 'tag-item';
                tag.textContent = nome;
                label.appendChild(tag);
            });
        }

        // Selecionar todos / Limpar
        function selecionarTodos(marcar) {
            var painel = document.getElementById('cblEntidades');
            if (!painel) return;

            var checkboxes = painel.querySelectorAll('input[type="checkbox"]');
            checkboxes.forEach(function (chk) {
                chk.checked = marcar;
            });

            atualizarResumoEntidades();
        }

        // Inicialização
        document.addEventListener('DOMContentLoaded', function () {

            // 1) Header do dropdown (sem onclick inline, previne o "carregando")
            var header = document.getElementById('dropdownHeaderEntidades');
            if (header) {
                header.addEventListener('click', function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    toggleDropdown(this);
                });
            }

            // 2) Fecha dropdown ao clicar fora
            document.addEventListener('click', function (e) {
                document.querySelectorAll('.custom-dropdown').forEach(function (wrapper) {
                    if (!wrapper.contains(e.target)) wrapper.classList.remove('open');
                });
            });

            // 3) Atualiza resumo quando marca/desmarca
            var painel = document.getElementById('cblEntidades');
            if (painel) {
                painel.addEventListener('change', function (e) {
                    if (e.target && e.target.type === 'checkbox')
                        atualizarResumoEntidades();
                });
            }

            // 4) Inicial
            atualizarResumoEntidades();
        });
    </script>

</asp:Content>