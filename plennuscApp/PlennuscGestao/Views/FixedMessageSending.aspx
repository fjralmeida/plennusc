<%@ Page Title="Envio Fixo CSV" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="FixedMessageSending.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.FixedMessageSending" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

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
            max-width: 960px;
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
            padding: 30px;
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

        .btn-info {
            background-color: #83CEEE;
            border-color: #83CEEE;
            color: #fff;
        }

        .btn-info:hover {
            background-color: #6AB9E0;
        }

        .btn-secondary:hover {
            background-color: #b5b5b5;
        }

        .filter-panel {
            background: #f0f2f5;
            padding: 16px 20px;
            border-radius: 12px;
            margin-bottom: 24px;
        }

        .table th, .table td {
            text-align: center;
            padding: 12px;
            border-top: none;
            border-bottom: 1px solid #e9ecef;
        }

        .table-striped tbody tr:nth-child(odd) {
            background-color: #ffffff;
        }

        .table-striped tbody tr:nth-child(even) {
            background-color: #f4f8fb;
        }

        .modal-body {
            background-color: #f8f9fa;
        }

        .modal-header {
            background-color: #4CB07A !important;
            color: white;
        }

        .modal-footer {
            background-color: #f1f1f1;
        }

        #modalResultadoConteudo {
            white-space: pre-wrap;
            font-size: 14px;
            font-family: 'Poppins', sans-serif;
            color: #333;
        }

        #loadingOverlay {
            display: none;
            position: fixed;
            top: 0;
            left: 0;
            width: 100vw;
            height: 100vh;
            background: rgba(255,255,255,0.9);
            z-index: 1050;
            text-align: center;
            padding-top: 30vh;
        }
    </style>

    <script>
        function mostrarResultadoModal(texto) {
            document.getElementById("modalResultadoConteudo").textContent = texto;
            var modal = new bootstrap.Modal(document.getElementById('resultadoModal'));
            modal.show();
        }

        function mostrarLoading() {
            document.getElementById('loadingOverlay').style.display = 'block';
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <h2 class="titulo-pagina"><i class="fa-solid fa-file-csv me-2" style="color:#83CEEE;"></i> Envio de Mensagens Fixo via CSV</h2>

        <div class="card-container">
            <div class="filter-panel">
                <div class="row g-3 align-items-end">
                    <div class="col-md-8">
                        <label class="form-label fw-semibold">Selecionar Arquivo CSV</label>
                        <asp:FileUpload ID="fileUploadCsv" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-4 text-end">
                        <asp:Button ID="btnLerCsv" runat="server" Text="Carregar CSV"
                            OnClick="btnLerCsv_Click"
                            CssClass="btn btn-purple btn-pill" />
                    </div>
                </div>
            </div>

            <asp:GridView ID="gridCsv" runat="server" AutoGenerateColumns="false"
                CssClass="table table-striped table-bordered shadow-sm rounded mb-4">
                <Columns>
                    <asp:BoundField DataField="Field3" HeaderText="Nome" />
                    <asp:BoundField DataField="Field4" HeaderText="Data" />
                    <asp:BoundField DataField="Telefone" HeaderText="Telefone" />
                    <asp:BoundField DataField="Field5" HeaderText="CPF" />
                </Columns>
            </asp:GridView>

            <div class="text-end">
                <asp:Button ID="btnEnviar" runat="server" Text="Enviar Mensagens"
                    CssClass="btn btn-success btn-pill"
                    Enabled="false"
                    OnClick="btnEnviar_Click"
                    OnClientClick="mostrarLoading();" />
            </div>

            <asp:Literal ID="litResultado" runat="server" Mode="PassThrough"></asp:Literal>
        </div>
    </div>

    <!-- Modal Resultado -->
    <div class="modal fade" id="resultadoModal" tabindex="-1" aria-labelledby="resultadoModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content rounded-4 shadow">
                <div class="modal-header text-white rounded-top-4">
                    <h5 class="modal-title" id="resultadoModalLabel">
                        <i class="fa-solid fa-paper-plane me-2"></i>Resultado do Envio
                    </h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Fechar"></button>
                </div>
                <div class="modal-body" style="max-height: 400px; overflow-y: auto;">
                    <pre id="modalResultadoConteudo" class="mb-0"></pre>
                </div>
                <div class="modal-footer bg-light rounded-bottom-4">
                    <asp:Button ID="btnDownloadCsvStatus" runat="server"
                        Text="Baixar Csv com Status"
                        CssClass="btn btn-info btn-pill"
                        OnClick="btnDownloadCsvStatus_Click" />

                    <button type="button" class="btn btn-secondary btn-pill" data-bs-dismiss="modal">
                        <i class="fa-solid fa-xmark me-1"></i>Fechar
                    </button>
                </div>
            </div>
        </div>
    </div>

    <!-- Overlay -->
    <div id="loadingOverlay">
        <div class="spinner-border text-success" style="width: 50px; height: 50px;"></div>
        <div style="margin-top: 8px; font-size: 18px; color: #4CB07A;">Enviando mensagens...</div>
    </div>
</asp:Content>
