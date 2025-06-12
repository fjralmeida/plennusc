<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Index.Master" AutoEventWireup="true" CodeBehind="EnvioMensagemBeneficiario.aspx.cs" Inherits="appWhatsapp.Views.EnvioMensagemBeneficiario" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    <style>
        body {
            background-color: #f9f9f9;
        }
        .panel-body {
            background-color: #ffffff;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 10px;
        }
        .table th, .table td {
            vertical-align: middle;
            text-align: center;
        }
        .table thead {
            background-color: #003399;
            color: white;
        }
        .btn-primary {
            background-color: #007bff;
            border-color: #007bff;
            color: white;
            cursor: pointer;
            border-radius: 4px;
        }
        .btn-primary:hover {
            background-color: #0056b3;
            border-color: #004085;
        }
        tr.linha-selecionada td {
            background-color: #d0ebff !important;
        }
        #loadingOverlay {
            display: none;
            position: fixed;
            top: 0; left: 0;
            width: 100%; height: 100%;
            background-color: rgba(255, 255, 255, 0.95);
            z-index: 9999;
            text-align: center;
            padding-top: 200px;
        }
        .spinner {
            border: 8px solid #f3f3f3;
            border-top: 8px solid #007bff;
            border-radius: 50%;
            width: 60px;
            height: 60px;
            animation: spin 1s linear infinite;
            margin: 20px auto;
        }
        @keyframes spin {
            0% { transform: rotate(0deg); }
            100% { transform: rotate(360deg); }
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

        var dotCount = 1;
        setInterval(function () {
            var dots = '.'.repeat(dotCount);
            var dotsEl = document.getElementById("dots");
            if (dotsEl) dotsEl.textContent = dots;
            dotCount = (dotCount % 3) + 1;
        }, 500);

        function mostrarLoading() {
            var overlay = document.getElementById("loadingOverlay");
            if (overlay) overlay.style.display = "block";
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Tela de carregamento -->
    <div id="loadingOverlay">
        <div class="spinner"></div>
        <div style="font-size: 20px; font-weight: bold;">
            Enviando mensagens<span id="dots">.</span>
        </div>
    </div>

    <div class="container">
        <asp:Button ID="btnTestarApi" runat="server" Text="Testar API" 
            OnClick="btnTestarApi_Click" OnClientClick="mostrarLoading();" />

        <br /><br />
        <asp:Label ID="lblResultado" runat="server" Text=""></asp:Label>

        <asp:Panel CssClass="container" runat="server" ID="pnlGridAssociados" Style="max-width: 960px; margin: auto; padding: 30px;">
            <div class="d-flex justify-content-center mb-4">
                <div class="row align-items-end" style="max-width: 600px; width: 100%;">
                    <div class="col-md-4">
                        <label for="txtDataInicio" class="form-label fw-bold">De:</label>
                        <input type="date" id="txtDataInicio" runat="server" class="form-control" />
                    </div>
                    <div class="col-md-4">
                        <label for="txtDataFim" class="form-label fw-bold">Até:</label>
                        <input type="date" id="txtDataFim" runat="server" class="form-control" />
                    </div>
                    <div class="col-md-4">
                        <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar"
                            CssClass="btn btn-primary w-100 fw-bold mt-3 mt-md-0"
                            OnClick="btnFiltrar_Click" />
                    </div>
                </div>
            </div>
            <asp:Label runat="server" ID="LblMensagem" Visible="false" CssClass="text-danger fs-5 fw-bold d-block mt-3 text-center"></asp:Label>
        </asp:Panel>

        <asp:Literal ID="LiteralMensagem" runat="server"></asp:Literal>

        <div class="table-responsive">
            <asp:GridView runat="server" ID="GridAssociados"
                AutoGenerateColumns="False"
                CssClass="table table-bordered table-hover align-middle"
                EmptyDataText="Nenhum registro encontrado."
                ClientIDMode="Static">
                <RowStyle CssClass="linha-grid" />
                <Columns>
                    <asp:TemplateField HeaderText="Selecionar">
                        <HeaderTemplate>
                            <asp:CheckBox ID="chkSelecionarTodos" runat="server" onclick="selecionarTodos(this);" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkSelecionar" runat="server" onclick="alternarSelecao(this);" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Código">
                        <ItemTemplate>
                            <asp:Label ID="lblCodigo" runat="server" Text='<%# Eval("CODIGO_ASSOCIADO") %>' CssClass="fw-semibold"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Registro">
                        <ItemTemplate>
                            <asp:Label ID="lblRegistro" runat="server" Text='<%# Eval("NUMERO_REGISTRO") %>' CssClass="fw-semibold"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Associado">
                        <ItemTemplate>
                            <asp:Label ID="lblNome" runat="server" Text='<%# Eval("NOME_ASSOCIADO") %>' CssClass="fw-semibold"></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Plano">
                        <ItemTemplate>
                            <asp:Label ID="lblPlano" runat="server" Text='<%# Eval("NOME_PLANO_ABREVIADO") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Operadora">
                        <ItemTemplate>
                            <asp:Label ID="lblOperadora" runat="server" Text='<%# Eval("NOME_OPERADORA") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Vencimento">
                        <ItemTemplate>
                            <asp:Label ID="lblVencimento" runat="server" Text='<%# String.Format("{0:dd/MM/yyyy}", Eval("DATA_VENCIMENTO")) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Valor">
                        <ItemTemplate>
                            <asp:Label ID="lblValor" runat="server" Text='<%# String.Format("R$ {0:N2}", Eval("VALOR_FATURA")) %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Telefone">
                        <ItemTemplate>
                            <asp:Label ID="lblTelefone" runat="server" Text='<%# Eval("NUMERO_TELEFONE") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
