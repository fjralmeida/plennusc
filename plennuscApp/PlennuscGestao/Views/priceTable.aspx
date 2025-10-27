<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="priceTable.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.priceTable" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

        <title>Inserir Tabela de Preço</title>

    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <link href="../../Content/Css/projects/gestao/structuresCss/priceTable.css" rel="stylesheet" />

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
                        GridLines="None"
                        OnPreRender="gridXsl_PreRender">
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
