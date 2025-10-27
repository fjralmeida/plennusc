<%@ Page Title="Envio Fixo CSV" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="FixedMessageSending.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.FixedMessageSending" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
           <title>Enviar Mensagem Padrão</title>

    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />

    <link href="../../Content/Css/projects/gestao/structuresCss/FixedMessageSending.css" rel="stylesheet" />
    <script src="../../Content/Css/js/projects/gestaoJs/structuresJs/FixedMessageSending.js"></script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <!-- Bootstrap JS carregado no final para garantir que o DOM esteja pronto -->
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>


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

              <div class="text-end">
      <asp:Button ID="btnEnviar" runat="server" Text="Enviar Mensagens"
          CssClass="btn btn-success btn-pill"
          Enabled="false"
          OnClick="btnEnviar_Click"
          OnClientClick="mostrarLoading();" />
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
