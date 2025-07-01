<%@ Page Title="Envio Fixo CSV" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="FixedMessageSending.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.FixedMessageSending" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

    <style>
        body {
            background: #f5f7fa;
            font-family: 'Poppins', sans-serif;
            font-size: 13px;
            color: #333;
        }

        .container {
            max-width: 900px;
            margin: 0 auto;
            padding: 32px 16px;
        }

        .card-container {
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 1px 6px rgba(0,0,0,0.05);
            padding: 24px;
            margin-top: 24px;
        }

        .card-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 24px;
        }

        .card-header h2 {
            font-weight: 600;
            font-size: 20px;
            margin: 0;
        }

        .btn-pill {
            border-radius: 50px;
            padding: 6px 18px;
            font-weight: 600;
            box-shadow: 0 2px 6px rgba(0,0,0,0.1);
            transition: background-color 0.3s, box-shadow 0.3s;
        }

        .btn-primary {
            background-color: #4285f4;
            border-color: #4285f4;
            color: #fff;
        }

        .btn-primary:hover {
            background-color: #2d6cdf;
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
            border-radius: 8px;
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

        #modalResultadoConteudo {
            font-family: 'Inter', sans-serif;
            font-size: 14px;
            color: #333;
            white-space: pre-wrap;
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
            font-family: 'Poppins', sans-serif;
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
        <div class="card-container">

            <div class="card-header">
                <h2 class="mb-0" style="font-size: 18px; font-weight: 500; color: #333;">
                   <i class="fa-solid fa-file-csv me-2" style="font-size: 18px; color: #83CEEE;"></i>
                    Envio de Mensagens Fixo via CSV
                </h2>

                <asp:Button ID="btnEnviar" runat="server" Text="Enviar Mensagens"
                    CssClass="btn btn-success btn-pill"
                    Enabled="false"
                    OnClick="btnEnviar_Click"
                    OnClientClick="mostrarLoading();" />
            </div>

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

            <asp:Literal ID="litResultado" runat="server" Mode="PassThrough"></asp:Literal>

        </div>
    </div>

    <!-- Modal Resultado -->
    <div class="modal fade" id="resultadoModal" tabindex="-1" aria-labelledby="resultadoModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content rounded-4 shadow">
                <div class="modal-header bg-success text-white rounded-top-4">
                    <h5 class="modal-title" id="resultadoModalLabel"><i class="fa-solid fa-paper-plane me-2"></i>Resultado do Envio</h5>
                    <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Fechar"></button>
                </div>
                <div class="modal-body" style="max-height: 400px; overflow-y: auto;">
                    <pre id="modalResultadoConteudo" class="mb-0"></pre>
                </div>
                <div class="modal-footer bg-light rounded-bottom-4">
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
