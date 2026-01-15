<%@ Page Title="Envio Fixo CSV" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="FixedMessageSending.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.FixedMessageSending" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
           <title>Enviar Mensagem Padrão</title>

    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet">
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />

    <link href="../../Content/Css/projects/gestao/structuresCss/FixedMessageSending.css" rel="stylesheet" />
    <script src="../../Content/Css/js/projects/gestaoJs/structuresJs/FixedMessageSending.js"></script>


    <script type="text/javascript">

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

    <!-- Bootstrap JS carregado no final para garantir que o DOM esteja pronto -->
<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/js/bootstrap.bundle.min.js"></script>


    <div class="container">
        <h2 class="titulo-pagina"><i class="fa-solid fa-file-csv me-2" style="color:#83CEEE;"></i> Envio de Mensagens Fixo via CSV</h2>

        <!-- Seção compacta de modelos -->
        <div class="mb-4">
            <div class="d-flex align-items-center mb-3">
                <h6 class="fw-semibold mb-0 me-3" style="color: #4CB07A;">
                    <i class="fa-solid fa-download me-2"></i>Modelos Disponíveis
                </h6>
                <div class="flex-grow-1" style="height: 1px; background-color: #dee2e6;"></div>
            </div>
            
            <div class="row g-3">
                <div class="col-md-6">
                    <div class="card model-card border h-100">
                        <div class="card-body p-3">
                            <div class="d-flex align-items-center mb-2">
                                <div class="bg-primary-light p-2 rounded me-3">
                                    <i class="fa-solid fa-file-csv text-primary"></i>
                                </div>
                                <div class="flex-grow-1">
                                    <h6 class="card-title mb-0 fw-semibold" style="color: #4CB07A;">Modelo Completo</h6>
                                    <small class="text-muted">5 colunas</small>
                                </div>
                                <asp:LinkButton ID="btnModeloCompleto" runat="server" 
                                    CssClass="btn btn-primary btn-sm btn-pill"
                                    OnClick="btnModeloCompleto_Click">
                                    <i class="fa-solid fa-download me-1"></i> Baixar
                                </asp:LinkButton>
                            </div>
                            <div class="model-columns">
                                <span class="badge bg-light text-dark border me-1 mb-1">Sexo</span>
                                <span class="badge bg-light text-dark border me-1 mb-1">Nome</span>
                                <span class="badge bg-light text-dark border me-1 mb-1">Data</span>
                                <span class="badge bg-light text-dark border me-1 mb-1">Telefone</span>
                                <span class="badge bg-light text-dark border mb-1">CPF</span>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-md-6">
                    <div class="card model-card border h-100">
                        <div class="card-body p-3">
                            <div class="d-flex align-items-center mb-2">
                                <div class="bg-success-light p-2 rounded me-3">
                                    <i class="fa-solid fa-file-csv text-success"></i>
                                </div>
                                <div class="flex-grow-1">
                                    <h6 class="card-title mb-0 fw-semibold" style="color: #4CB07A;">Novo Plano</h6>
                                    <small class="text-muted">4 colunas</small>
                                </div>
                                <asp:LinkButton ID="btnModeloSimples" runat="server" 
                                    CssClass="btn btn-success btn-sm btn-pill"
                                    OnClick="btnModeloSimples_Click">
                                    <i class="fa-solid fa-download me-1"></i> Baixar
                                </asp:LinkButton>
                            </div>
                            <div class="model-columns">
                                <span class="badge bg-light text-dark border me-1 mb-1">Nome</span>
                                <span class="badge bg-light text-dark border mb-1">Telefone</span>
                                <span class="badge bg-light text-dark border mb-1">Nome Operador</span>
                                <span class="badge bg-light text-dark border mb-1">Antigo Cliente</span>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            
            <small class="text-muted mt-2 d-block">
                <i class="fa-solid fa-circle-info me-1"></i>
                Escolha um modelo, preencha com seus dados e faça o upload abaixo
            </small>
        </div>
        <!-- Fim seção compacta de modelos -->

        <div class="card-container">
           <div class="filter-panel">
                <div class="row g-3 align-items-end">
                    <div class="col-md-8">
                        <label class="form-label fw-semibold" style="color: #4CB07A;">
                            <i class="fa-solid fa-upload me-2" style="color:#83CEEE;"></i>Selecionar Arquivo CSV
                        </label>
                        <div class="input-group">
                            <asp:FileUpload ID="fileUploadCsv" runat="server" CssClass="form-control" />
                            <asp:Button ID="btnLerCsv" runat="server" Text="Carregar CSV"
                                OnClick="btnLerCsv_Click"
                                CssClass="btn btn-purple btn-pill" />
                        </div>
                        <small class="form-text text-muted mt-2 d-block">
                            <i class="fa-solid fa-circle-info me-1"></i>
                            Selecione um arquivo CSV no formato desejado e clique em "Carregar CSV"
                        </small>
                    </div>
                    <div class="col-md-4 text-end">
                        <div class="d-grid">
                            <asp:Button ID="btnEnviar" runat="server" Text="Enviar Mensagens"
                                CssClass="btn btn-success btn-pill"
                                Enabled="false"
                                OnClick="btnEnviar_Click"
                                OnClientClick="mostrarLoading();" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="mt-4">
                <div class="d-flex justify-content-between align-items-center mb-3">
                    <h6 class="fw-bold text-dark mb-0">
                        <i class="fa-solid fa-list me-2" style="color:#83CEEE;"></i>Contatos Carregados
                    </h6>
                    <span class="badge" style="background-color: #4CB07A;" id="contadorContatos">0 contatos</span>
                </div>
                
                <div class="table-responsive">
                    <asp:GridView ID="gridCsv" runat="server" AutoGenerateColumns="false"
                        CssClass="table table-hover table-bordered mb-0">
                        <Columns>
                            <asp:BoundField DataField="Field3" HeaderText="Nome" ItemStyle-CssClass="fw-semibold" />
                            <asp:BoundField DataField="Field4" HeaderText="Data" />
                            <asp:BoundField DataField="Telefone" HeaderText="Telefone" />
                            <asp:BoundField DataField="Field5" HeaderText="CPF" />
                        </Columns>
                        <EmptyDataTemplate>
                            <div class="text-center py-5">
                                <i class="fa-solid fa-inbox fa-3x text-muted mb-3"></i>
                                <p class="text-muted">Nenhum contato carregado ainda.</p>
                                <p class="small text-muted">Faça upload de um arquivo CSV para visualizar os contatos aqui.</p>
                            </div>
                        </EmptyDataTemplate>
                    </asp:GridView>
                </div>
            </div>

            <asp:Literal ID="litResultado" runat="server" Mode="PassThrough"></asp:Literal>
        </div>
    </div>

    <!-- Modal Resultado -->
    <div class="modal fade" id="resultadoModal" tabindex="-1" aria-labelledby="resultadoModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg modal-dialog-centered">
            <div class="modal-content rounded-4 shadow">
                <div class="modal-header text-white rounded-top-4" style="background-color: #4CB07A;">
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
        <div class="spinner-border" style="width: 50px; height: 50px; color: #4CB07A;"></div>
        <div style="margin-top: 8px; font-size: 18px; color: #4CB07A;">Enviando mensagens...</div>
    </div>

    <script>
        // Script para atualizar o contador de contatos quando a tabela for carregada
        function updateContactCount() {
            var grid = document.getElementById('<%= gridCsv.ClientID %>');
        if (!grid) return;
        
        // Pega todas as linhas da tabela
        var allRows = grid.getElementsByTagName('tr');
        var dataRowsCount = 0;
        
        for (var i = 0; i < allRows.length; i++) {
            var row = allRows[i];
            
            // Ignora o cabeçalho da tabela (THEAD)
            if (row.parentNode.tagName === 'THEAD') continue;
            
            // Verifica se é uma linha de dados (não tem colspan)
            var isDataRow = true;
            var cells = row.getElementsByTagName('td');
            
            // Se a linha não tem células ou tem célula com colspan (template vazio), ignora
            if (cells.length === 0) continue;
            
            for (var j = 0; j < cells.length; j++) {
                if (cells[j].colSpan > 1) {
                    isDataRow = false;
                    break;
                }
            }
            
            if (isDataRow) {
                dataRowsCount++;
            }
        }
        
        // Atualiza o contador
        var counterElement = document.getElementById('contadorContatos');
        if (counterElement) {
            counterElement.textContent = dataRowsCount + ' contato' + (dataRowsCount !== 1 ? 's' : '');
        }
        
        // Atualiza o botão de envio
            var btnEnviar = document.getElementById('<%= btnEnviar.ClientID %>');
            if (btnEnviar) {
                btnEnviar.disabled = dataRowsCount === 0;
            }
        }

        // Executar quando a página carregar
        document.addEventListener('DOMContentLoaded', updateContactCount);

        // Para atualizar após postbacks no ASP.NET
        if (typeof (Sys) !== 'undefined') {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                updateContactCount();
            });
        }
    </script>
</asp:Content>