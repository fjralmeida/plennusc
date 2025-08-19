<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="priceTable.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.priceTable" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />

    <title>Inserir Tabela de Preço</title>

    <style>
        * {
            font-family: 'Poppins', sans-serif;
            box-sizing: border-box;
        }

        body {
            background-color: #f2f4f8;
            font-size: 13px;
            color: #333;
        }

        .container {
            max-width: 1300px;
            margin: auto;
            padding: 32px 16px;
        }

        .titulo-pagina {
            font-size: 22px;
            font-weight: 600;
            color: #4CB07A;
            text-align: center;
            margin-bottom: 30px;
            position: relative;
        }

            .titulo-pagina::after {
                content: "";
                width: 60px;
                height: 3px;
                background-color: #83CEEE;
                display: block;
                margin: 0.5rem auto 0 auto;
                border-radius: 2px;
            }

        .card-container {
            background: white;
            padding: 13px;
            border-radius: 16px;
            box-shadow: 0 3px 10px rgba(0, 0, 0, 0.06);
        }

        .btn-pill {
            border-radius: 50px;
            padding: 6px 18px;
            font-weight: 600;
            transition: all 0.3s ease-in-out;
            box-shadow: 0 2px 6px rgba(0,0,0,0.1);
        }

        .btn-success {
            background-color: #4CB07A;
            border-color: #4CB07A;
            color: #fff;
        }

            .btn-success:hover {
                background-color: #3B8B65;
            }

        .btn-purple {
            background-color: #C06ED4;
            border-color: #C06ED4;
            color: #fff;
        }

            .btn-purple:hover {
                background-color: #a14db8;
            }

        .filter-panel {
            background: #f0f2f5;
            padding: 16px 20px;
            border-radius: 12px;
            margin-bottom: 24px;
        }

        .grid-wrapper {
            max-height: 530px;
            overflow: auto;
            border-radius: 12px;
            margin-bottom: 20px;
        }

            .grid-wrapper table {
                min-width: 1200px;
            }

        .table th, .table td {
            white-space: nowrap;
            padding: 10px 16px;
            font-size: 13px;
            text-align: center;
        }

        .grid-wrapper {
            max-height: 530px;
            overflow-y: auto;
            border-radius: 12px;
            margin-bottom: 20px;
            border: 1px #ddd;
        }

            .grid-wrapper table {
                min-width: 1200px;
            }

        .table th {
            white-space: nowrap;
            padding: 10px 16px;
            font-size: 13px;
            text-align: center;
        }

        .table td {
            white-space: nowrap;
            padding: 8px 14px;
            font-size: 13px;
            text-align: center;
        }

        .btn-outline-excel {
            border: 1px solid #dbe7e1;
            color: #107C41;
            background: #f5fbf8;
            box-shadow: 0 2px 6px rgba(0,0,0,0.06);
        }

            .btn-outline-excel:hover {
                background: #107C41;
                color: #fff;
                border-color: #107C41;
            }

        .hint-line {
            font-size: 12px;
        }

        .filter-panel .form-control {
            height: 38px; /* deixa a altura do input parecida com os botões */
        }

        .btn-pill {
            line-height: 1;
        }
    </style>

    <script>
        function abrirConfirmacaoInserir() {
            var modal = new bootstrap.Modal(document.getElementById('confirmUpdate'));
            modal.show();
            return false; // cancela o postback do clique original
        }

        function confirmarInsercao() {
            // evita duplo clique
            var btn = document.getElementById('<%= btnEnviar.ClientID %>');
            if (btn) btn.disabled = true;

            // fecha o modal e faz o postback do btnEnviar
            var modalEl = document.getElementById('confirmUpdate');
            var instance = bootstrap.Modal.getInstance(modalEl);
            if (instance) instance.hide();

            __doPostBack('<%= btnEnviar.UniqueID %>', '');
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container">
        <h2 class="titulo-pagina">
            <i class="fa-solid fa-file me-2" style="color: #83CEEE;"></i>Inserção de Tabela de Preço
        </h2>

        <div class="card-container">
            <div class="filter-panel">

                <!-- Faixa 1: download do modelo + dica -->
                <div class="d-flex align-items-center justify-content-between flex-wrap gap-2 mb-3">
                    <div class="d-flex align-items-center gap-2">
                        <asp:HyperLink ID="lnkModeloXls" runat="server"
                            CssClass="btn btn-outline-excel btn-pill"
                            ToolTip="Baixar modelo XLS (formato da tabela)">
      <i class="fa-solid fa-file-excel me-2"></i> Baixar modelo XLS
                        </asp:HyperLink>
                        <small class="text-muted hint-line">Use o modelo para garantir os cabeçalhos corretos.</small>
                    </div>
                </div>

                <!-- Faixa 2: upload + botão carregar -->
                <label class="form-label fw-semibold mb-1">Selecionar Arquivo XLS</label>
                <div class="d-flex align-items-center gap-2">
                    <asp:FileUpload ID="fileUploadXls" runat="server" CssClass="form-control flex-grow-1" />
                    <asp:Button ID="btnLerXls" runat="server" Text="Carregar XLS"
                        OnClick="btnLerXls_Click"
                        CssClass="btn btn-purple btn-pill" />
                </div>

            </div>

            <div class="d-flex align-items-center justify-content-between gap-3 mb-2">
                <div class="flex-grow-1">
                    <asp:Literal ID="lblResultado" runat="server" Mode="PassThrough"></asp:Literal>
                </div>

                <asp:Button ID="btnEnviar" runat="server" Text="Inserir Tabela de Preço"
                    CssClass="btn btn-success btn-pill"
                    Enabled="false"
                    OnClick="btnEnviar_Click"
                    OnClientClick="return abrirConfirmacaoInserir();"
                    UseSubmitBehavior="false" />
            </div>

            <div class="grid-wrapper">
                <asp:GridView ID="gridXsl"
                    runat="server"
                    AutoGenerateColumns="False"
                    CssClass="table table-striped table-bordered table-hover"
                    HeaderStyle-BackColor="#4CB07A"
                    HeaderStyle-ForeColor="White"
                    HeaderStyle-Font-Bold="True"
                    RowStyle-VerticalAlign="Middle"
                    GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="CODIGO_PLANO" HeaderText="CODIGO_PLANO" />
                        <asp:BoundField DataField="CODIGO_TABELA_PRECO" HeaderText="CODIGO_TABELA_PRECO" />
                        <asp:BoundField DataField="IDADE_MINIMA" HeaderText="IDADE_MINIMA" />
                        <asp:BoundField DataField="IDADE_MAXIMA" HeaderText="IDADE_MAXIMA" />
                        <asp:BoundField DataField="VALOR_PLANO" HeaderText="VALOR_PLANO"
                            DataFormatString="{0:N2}" HtmlEncode="false" />
                        <asp:BoundField DataField="TIPO_RELACAO_DEPENDENCIA" HeaderText="TIPO_RELACAO_DEPENDENCIA" />
                        <asp:BoundField DataField="CODIGO_GRUPO_CONTRATO" HeaderText="CODIGO_GRUPO_CONTRATO" />
                        <asp:BoundField DataField="NOME_TABELA" HeaderText="NOME_TABELA" />
                        <asp:BoundField DataField="VALOR_NET" HeaderText="VALOR_NET"
                            DataFormatString="{0:N2}" HtmlEncode="false" />
                        <asp:BoundField DataField="TIPO_CONTRATO_ESTIPULADO" HeaderText="TIPO_CONTRATO_ESTIPULADO" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>

    <!-- Modal de Confirmação -->
    <div class="modal fade" id="confirmUpdate" tabindex="-1" aria-labelledby="confirmUpdateLabel" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content" style="border-radius: 16px;">
                <div class="modal-header" style="border-bottom: 0;">
                    <h5 class="modal-title" id="confirmUpdateLabel">
                        <i class="fa-solid fa-circle-exclamation me-2" style="color: #83CEEE;"></i>
                        Confirmar inserção
                    </h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Fechar"></button>
                </div>

                <div class="modal-body">
                    Isso vai <b>inserir os registros</b> da planilha na tabela <code>PS1032</code>.<br />
                    Deseja continuar?
                </div>

                <div class="modal-footer" style="border-top: 0;">
                    <button type="button" class="btn btn-light btn-pill" data-bs-dismiss="modal">Cancelar</button>

                    <!-- Botão que confirma e dispara o mesmo postback do btnEnviar -->
                    <button type="button" class="btn btn-success btn-pill" onclick="confirmarInsercao()">
                        Sim, inserir
                    </button>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
