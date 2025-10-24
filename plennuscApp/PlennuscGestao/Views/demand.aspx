<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="demand.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.demand" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>

<script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>
    <style>
        :root {
     --primary: #83ceee;
     --primary-hover: #6ebfe1;
     --success: #4cb07a;
     --success-hover: #3b8b65;
     --gray-50: #f8f9fa;
     --gray-100: #f1f3f4;
     --gray-200: #e8eaed;
     --gray-300: #dadce0;
     --gray-400: #bdc1c6;
     --gray-500: #9aa0a6;
     --gray-600: #80868b;
     --gray-700: #5f6368;
     --gray-800: #3c4043;
     --gray-900: #202124;
     --border-radius: 8px;
     --shadow: 0 1px 2px 0 rgba(60, 64, 67, 0.3), 0 1px 3px 1px rgba(60, 64, 67, 0.15);
     --transition: all 0.2s ease-in-out;
 }


        body {
            background: var(--gray-50);
            font-family: 'Inter', sans-serif;
            color: var(--gray-800);
            line-height: 1.5;
        }

        .container-main {
            max-width: 2200px;
            margin: 30px auto;
            padding: 0 16px;
        }

        /* Header */
        .header-card {
            background: white;
            border-radius: var(--border-radius);
            box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.05);
            padding: 20px 24px;
            margin-bottom: 24px;
            border: 1px solid var(--gray-200);
        }

        .demand-title {
            display: flex;
            align-items: center;
            gap: 12px;
            font-size: 20px;
            font-weight: 600;
            margin: 0;
            color: var(--gray-800);
        }

       .title-icon {
             background: #4cb07a;
             color: white;
             width: 40px;
             height: 40px;
             border-radius: 8px;
             display: flex;
             align-items: center;
             justify-content: center;
       }


        /* Form Container */
        .form-container {
            background-color: white;
            border-radius: var(--border-radius);
            box-shadow: 0 1px 3px 0 rgba(0, 0, 0, 0.05);
            padding: 24px;
            margin-bottom: 30px;
            border: 1px solid var(--gray-200);
        }

        .section-title {
            font-weight: 600;
            margin-bottom: 16px;
            color: var(--gray-800);
            font-size: 16px;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .form-card {
            background-color: var(--gray-50);
            border-radius: var(--border-radius);
            padding: 18px;
            margin-bottom: 20px;
            border: 1px solid var(--gray-200);
        }

        .form-label {
            font-weight: 500;
            margin-bottom: 6px;
            color: var(--gray-700);
            font-size: 14px;
        }

        .form-control, .form-select {
            border-radius: var(--border-radius);
            padding: 10px 12px;
            border: 1px solid var(--gray-300);
            transition: var(--transition);
            background: white;
            font-size: 14px;
        }

        .form-control:focus, .form-select:focus {
            outline: none;
            border-color: var(--primary);
            box-shadow: 0 0 0 3px rgba(37, 99, 235, 0.1);
        }

        textarea.form-control {
            min-height: 160px;
            resize: vertical;
        }

        .btn-save {
            background: var(--primary);
            border: none;
            color: white;
            font-weight: 500;
            padding: 10px 20px;
            border-radius: var(--border-radius);
            transition: var(--transition);
            cursor: pointer;
            display: flex;
            align-items: center;
            gap: 8px;
            font-size: 14px;
        }

        .btn-save:hover {
            background: var(--primary-hover);
        }

        .input-hint {
            font-size: 12px;
            color: var(--gray-500);
            margin-top: 6px;
        }

        .counter {
            font-size: 12px;
            color: var(--gray-500);
            text-align: right;
            margin-top: 4px;
        }

        .file-upload-area {
            border: 1px dashed var(--gray-300);
            border-radius: var(--border-radius);
            padding: 16px;
            text-align: center;
            background-color: var(--gray-50);
            transition: var(--transition);
            margin-top: 8px;
        }

        .file-upload-area:hover {
            border-color: var(--primary);
            background-color: rgba(37, 99, 235, 0.03);
        }

        .file-item {
            background-color: white;
            border-radius: var(--border-radius);
            padding: 10px 12px;
            margin-bottom: 8px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            border: 1px solid var(--gray-200);
            font-size: 14px;
        }

        .file-item:hover {
            background-color: var(--gray-50);
        }

         .file-preview-container {
            margin-top: 15px;
        }
        
        .file-preview-item {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 8px 12px;
            background-color: var(--gray-50);
            border: 1px solid var(--gray-200);
            border-radius: var(--border-radius);
            margin-bottom: 8px;
        }
        
        .file-preview-info {
            display: flex;
            align-items: center;
            gap: 8px;
        }
        
        .file-preview-name {
            font-size: 14px;
            color: var(--gray-700);
        }
        
        .file-preview-size {
            font-size: 12px;
            color: var(--gray-500);
        }
        
        .file-preview-remove {
            color: var(--danger);
            cursor: pointer;
            background: none;
            border: none;
            padding: 4px;
        }
        
        .file-preview-remove:hover {
            color: #c53030;
        }

        /* Modal styles */
        .demanda-critica-item {
            background-color: #fffbeb;
            border: 1px solid #fef3c7;
            transition: all 0.2s ease;
        }

        .demanda-critica-item:hover {
            background-color: #fef3c7;
        }

        .demanda-critica-item h6 {
            color: #92400e;
            margin-bottom: 0.5rem;
            font-weight: 600;
        }

        .modal-header {
            background: #fef3c7;
            border-bottom: 1px solid #fde68a;
        }

        /* Responsividade */
        @media (max-width: 992px) {
            .form-card {
                margin-bottom: 16px;
            }
        }

        @media (max-width: 768px) {
            .header-card {
                text-align: center;
            }
            
            .btn-save {
                width: 100%;
                justify-content: center;
            }
        }

        .required-field::after {
            content: " *";
            color: #dc2626;
        }
        
        /* Toast customization */
        .swal2-popup.swal2-toast {
            padding: 10px 15px;
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.15);
            border-radius: var(--border-radius);
        }

          /* ADICIONE ESTES ESTILOS APENAS PARA O MODAL */
    .modal {
        z-index: 1060; /* Acima dos toasts (1050) */
    }
    
    .modal-dialog {
        margin: 1.75rem auto;
        display: flex;
        align-items: center;
        min-height: calc(100% - 3.5rem);
    }
    
    .modal-content {
        border-radius: var(--border-radius);
        border: none;
        box-shadow: 0 10px 25px rgba(0, 0, 0, 0.15);
    }
    
    .modal-header {
        background: #fff3cd;
        border-bottom: 1px solid #ffeaa7;
        padding: 1rem 1.5rem;
    }
    
    .modal-title {
        color: #856404;
        font-weight: 600;
    }
    
    .modal-body {
        padding: 1.5rem;
        max-height: 60vh;
        overflow-y: auto;
    }
    
    .demanda-critica-item {
        background-color: #fff9db;
        border: 1px solid #ffe8a1;
        border-radius: var(--border-radius);
    }
    
    .demanda-critica-item:hover {
        background-color: #fff3cd;
    }
    
    .demanda-critica-item h6 {
        color: #5c3c00;
    }

 .btn-primary {
    --bs-btn-color: #fff;
    --bs-btn-bg: var(--primary);
    --bs-btn-border-color: var(--primary); 
    --bs-btn-hover-color: #fff;
    --bs-btn-hover-bg: var(--primary-hover); 
    --bs-btn-hover-border-color: var(--primary-hover);
    --bs-btn-focus-shadow-rgb: 49,132,253;
    --bs-btn-active-color: #fff;
    --bs-btn-active-bg: var(--primary-hover); 
    --bs-btn-active-border-color: var(--primary-hover); 
    --bs-btn-active-shadow: inset 0 3px 5px rgba(0, 0, 0, 0.125);
    --bs-btn-disabled-color: #fff;
    --bs-btn-disabled-bg: var(--primary);
    --bs-btn-disabled-border-color: var(--primary); 
}
.user-header {
    margin-bottom: 20px;
    padding: 12px 16px;
    background: #f8f9fa;
    border-radius: 8px;
    border: 1px solid #e9ecef;
}

.user-name {
    font-size: 16px;
    font-weight: 600;
    margin: 0;
    padding: 0;
    color: #000;
}

.user-sector {
    font-size: 14px;
    font-weight: 400;
    margin: 0;
    padding: 0;
    color: #6c757d;
}

    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- SCRIPTMANAGER DEVE VOLTAR AQUI - PRIMEIRA LINHA -->
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    
   <asp:UpdatePanel ID="upModalDemandas" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="modal fade" id="modalDemandas" tabindex="-1" aria-hidden="true">
                <div class="modal-dialog modal-lg modal-dialog-centered">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">
                                <asp:Literal ID="litTituloModal" runat="server"></asp:Literal>
                            </h5>
                            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                        </div>

                        <div class="modal-body">
                            <asp:Literal ID="litTextoModal" runat="server"></asp:Literal>

                            <asp:Repeater ID="rptDemandas" runat="server" OnItemDataBound="rptDemandas_ItemDataBound">
                                <ItemTemplate>
                                    <div class="demanda-critica-item mb-3 p-3 border rounded">
                                        <div class="row">
                                            <div class="col-md-8">
                                                <h6><%# Eval("Titulo") %></h6>
                                                <div class="text-muted small">
                                                    Data: <%# Eval("DataDemanda", "{0:dd/MM/yyyy}") %> |
                                                    Prioridade: <span class="badge bg-info"><%# Eval("Prioridade") %></span>
                                                </div>
                                            </div>
                                            <div class="col-md-4">
                                                <asp:DropDownList ID="ddlNovaPrioridade" runat="server" CssClass="form-select form-select-sm">
                                                    <asp:ListItem Text="-- Selecionar nova prioridade --" Value="" />
                                                </asp:DropDownList>
                                                <asp:Button ID="btnAlterarPrioridade" runat="server" Text="Alterar Prioridade"
                                                    CssClass="btn btn-primary btn-sm mt-2"
                                                    CommandArgument='<%# Eval("CodDemanda") %>'
                                                    OnClick="btnAlterarPrioridade_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </ItemTemplate>
                            </asp:Repeater>
                        </div>

                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Fechar</button>
                        </div>
                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <!-- Botão oculto fallback (dispara modal via data-bs) -->
    <button id="btnAbrirModalHidden" type="button" class="d-none" data-bs-toggle="modal" data-bs-target="#modalDemandas"></button>

    <!-- RESTO DO SEU CÓDIGO ORIGINAL -->

    <div class="user-header">
         <asp:Label ID="lblNomeUser" runat="server" Text="" CssClass="user-name" /> - 
        <asp:Label ID="lblSetorUsuario" runat="server" Text="" CssClass="user-sector" />
    </div>

   <%-- Setor de Origem --%>
   <div class="form-field">
       <asp:DropDownList 
           Visible="false"
           ID="ddlOrigem" 
           runat="server" 
           CssClass="form-select" 
           Required="true">
           <asp:ListItem Value="">Selecione o setor...</asp:ListItem>
           <asp:ListItem Value="Tecnologia">Tecnologia</asp:ListItem>
       </asp:DropDownList>
   </div>

    <div class="container-main">
        <div class="header-card">
            <h1 class="demand-title">
                <span class="title-icon">
                    <i class="bi bi-clipboard-plus"></i>
                </span>
                Criar Nova Demanda
            </h1>
        </div>
        
        <div class="form-container">
            <div class="alert alert-light alert-dismissible fade show mb-4" role="alert" style="background-color: var(--gray-50); border: 1px solid var(--gray-200);">
                <i class="bi bi-info-circle me-2"></i> Todos os campos marcados com <span class="text-danger">*</span> são obrigatórios.
                <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
            </div>

            <div class="row">
                <!-- Coluna principal - Título e Descrição -->
                <div class="col-lg-8">
                    <div class="mb-4">
                        <h3 class="section-title"><i class="bi bi-pencil-square"></i>Detalhes Principais</h3>
                        <div class="form-card">
                            <div class="mb-3">
                                <label class="form-label required-field">Título</label>
                                <asp:TextBox ID="txtTitulo" runat="server" CssClass="form-control" MaxLength="100"
                                    placeholder="Ex: Correção de relatório financeiro" />
                                <div class="input-hint">
                                    <span class="counter" id="cntTitulo">0/100</span> caracteres
                                </div>
                            </div>
                            
                            <div class="mb-3">
                                <label class="form-label required-field">Descrição</label>
                                <asp:TextBox ID="txtDescricao" runat="server" CssClass="form-control" TextMode="MultiLine"
                                    placeholder="Descreva detalhadamente o que precisa ser feito, o contexto, os critérios de aceitação e qualquer informação relevante..." />
                                <div class="input-hint">
                                    <span class="counter" id="cntDesc">0</span> caracteres
                                </div>
                            </div>
                        </div>
                    </div>
                    
                    <!-- Anexos (FORA dos UpdatePanels) -->
                    <div class="mb-4">
                        <h3 class="section-title"><i class="bi bi-paperclip"></i>Anexos</h3>
                        <div class="form-card">
                            <label class="form-label">Adicionar arquivos (Máx. 10MB cada)</label>
                            
                            <!-- FileUpload ASP.NET -->
                            <div class="file-upload-area">
                                <i class="bi bi-cloud-arrow-up text-muted mb-2"></i>
                                <p class="mb-2 small text-muted">Arraste arquivos aqui ou clique para selecionar</p>
                                <asp:FileUpload ID="fuAnexos" runat="server" CssClass="d-none" AllowMultiple="true" 
                                    onchange="handleFileSelection(this);" />
                                <button type="button" class="btn btn-outline-secondary btn-sm" 
                                    onclick="document.getElementById('<%= fuAnexos.ClientID %>').click()">
                                    Selecionar Arquivos
                                </button>
                            </div>
                            
                            <div class="input-hint">
                                Formatos permitidos: PDF, Word, Excel, imagens (JPG, PNG, GIF)
                            </div>
                            
                            <!-- Container para preview dos arquivos selecionados -->
                            <div id="filePreviewContainer" class="file-preview-container" style="display: none;">
                                <h6 class="small text-muted">Arquivos selecionados:</h6>
                                <div id="filePreviewList"></div>
                            </div>
                        </div>
                    </div>
                </div>
                
                <!-- Coluna lateral - Categoria, Roteamento e Prioridade -->
                <div class="col-lg-4">
                    <!-- Prioridade em UpdatePanel -->
                 <asp:UpdatePanel ID="upPrioridade" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="mb-4">
                            <h3 class="section-title"><i class="bi bi-flag"></i>Prioridade</h3>
                            <div class="form-card">
                                <!-- Importância (sempre visível) -->
                                <div class="mb-3">
                                    <label class="form-label">Nível de Importância</label>
                                    <asp:DropDownList ID="ddlImportancia" runat="server" CssClass="form-select">
                                        <asp:ListItem Text="Selecione a importância (opcional)" Value="" />
                                    </asp:DropDownList>
                                </div>

                                <!-- Prioridade -->
                                <div class="mb-3">
                                    <label class="form-label required-field">Prioridade</label>
                                    <asp:DropDownList ID="ddlPrioridade" runat="server" CssClass="form-select" 
                                        AutoPostBack="true" OnSelectedIndexChanged="ddlPrioridade_SelectedIndexChanged">
                                        <asp:ListItem Text="Selecione a prioridade" Value="" />
                                    </asp:DropDownList>
                                </div>

                                <!-- Prazo (aparece apenas para prioridades específicas) -->
                                <div class="mb-3" id="divPrazo" runat="server" style="display: none;">
                                    <label class="form-label required-field">Prazo</label>
                                    <input type="date" id="txtPrazo" runat="server" class="form-control" />
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                 </asp:UpdatePanel>

                    <!-- Roteamento em UpdatePanel -->
                     <asp:UpdatePanel ID="upRoteamento" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="mb-4">
                            <h3 class="section-title"><i class="bi bi-signpost"></i>Roteamento</h3>
                            <div class="form-card">
                                <!-- REMOVIDO: Setor de Origem daqui -->
                                
                                <div class="mb-3">
                                    <label class="form-label required-field">Setor de Destino</label>
                                    <asp:DropDownList ID="ddlDestino" runat="server" CssClass="form-select" 
                                        AutoPostBack="true" 
                                        OnSelectedIndexChanged="ddlDestino_SelectedIndexChanged">
                                        <asp:ListItem Text="Selecione o setor de destino" Value="" />
                                        <asp:ListItem Text="Financeiro" Value="1" />
                                        <asp:ListItem Text="RH" Value="2" />
                                        <asp:ListItem Text="TI" Value="3" />
                                        <asp:ListItem Text="Operações" Value="4" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>

                    <!-- Categoria em UpdatePanel -->
                    <asp:UpdatePanel ID="upCategoria" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="mb-4">
                                <h3 class="section-title"><i class="bi bi-tag"></i>Categoria</h3>
                                <div class="form-card">
                                    <div class="mb-3">
                                        <label class="form-label required-field">Tipo de Categoria</label>
                                        <asp:DropDownList ID="ddlTipoGrupo" runat="server" CssClass="form-select" 
                                            AutoPostBack="true" 
                                            OnSelectedIndexChanged="ddlTipoGrupo_SelectedIndexChanged">
                                            <asp:ListItem Text="Selecione uma categoria" Value="" />
                                            <asp:ListItem Text="Tecnologia - Suporte" Value="1" />
                                            <asp:ListItem Text="Financeiro - Relatórios" Value="2" />
                                            <asp:ListItem Text="RH - Documentação" Value="3" />
                                            <asp:ListItem Text="Operações - Processos" Value="4" />
                                        </asp:DropDownList>
                                    </div>
                                    
                                    <div class="mb-3">
                                        <label class="form-label">Subtipo</label>
                                        <asp:DropDownList ID="ddlTipoDetalhe" runat="server" CssClass="form-select">
                                            <asp:ListItem Text="Selecione um subtipo" Value="" />
                                            <asp:ListItem Text="Criação Cx. Postal (email)" Value="1" />
                                            <asp:ListItem Text="Ajuste de permissões" Value="2" />
                                            <asp:ListItem Text="Correção de dados" Value="3" />
                                            <asp:ListItem Text="Atualização de sistema" Value="4" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
            
            <!-- Rodapé com botão alinhado à direita -->
            <div class="d-flex justify-content-end mt-4 pt-3 border-top">
                <asp:Button ID="btnSalvar" runat="server" Text="Criar Demanda" CssClass="btn-save" OnClick="btnSalvar_Click" />
            </div>
        </div>
    </div>

    <script>
        // Contador de caracteres
        function setupCharacterCounters() {
            const titulo = document.getElementById('<%= txtTitulo.ClientID %>');
            const desc = document.getElementById('<%= txtDescricao.ClientID %>');
            const cntTitulo = document.getElementById('cntTitulo');
            const cntDesc = document.getElementById('cntDesc');

            function updateCounters() {
                if (titulo && cntTitulo) {
                    cntTitulo.textContent = (titulo.value || '').length + '/100';
                }
                if (desc && cntDesc) {
                    cntDesc.textContent = (desc.value || '').length;
                }
            }

            if (titulo) {
                titulo.addEventListener('input', updateCounters);
            }
            if (desc) {
                desc.addEventListener('input', updateCounters);
            }

            // Inicializar contadores
            updateCounters();
        }

        // Auto-ajuste da altura da textarea
        function autoResizeTextarea() {
            const textarea = document.getElementById('<%= txtDescricao.ClientID %>');
            if (textarea) {
                textarea.addEventListener('input', function () {
                    this.style.height = 'auto';
                    this.style.height = (this.scrollHeight) + 'px';
                });

                // Inicializar altura
                setTimeout(() => {
                    textarea.style.height = 'auto';
                    textarea.style.height = (textarea.scrollHeight) + 'px';
                }, 0);
            }
        }

        // Funções para exibir toasts
        function showToastErroObrigatorio() {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'error',
                title: 'Preencha todos os campos obrigatórios.',
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true
            });
        }
        
        function showToastSucesso(mensagem) {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'success',
                title: mensagem,
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true
            });
        }
        
        function showToastErro(mensagem) {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'error',
                title: mensagem,
                showConfirmButton: false,
                timer: 4000,
                timerProgressBar: true
            });
        }
        
        function showToastAviso(mensagem) {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'warning',
                title: mensagem,
                showConfirmButton: false,
                timer: 4000,
                timerProgressBar: true
            });
        }

        function rolarParaPrimeiroCampoInvalido(validationGroup) {
            // pega todos os validators do grupo
            var validators = Page_Validators;
            for (var i = 0; i < validators.length; i++) {
                var v = validators[i];
                if (v.validationGroup === validationGroup && !v.isvalid) {
                    var campo = document.getElementById(v.controltovalidate);
                    if (campo) {
                        campo.scrollIntoView({ behavior: 'smooth', block: 'center' });
                        campo.focus();
                        break; // rola só para o primeiro
                    }
                }
            }
        }

        // Variável global para armazenar os arquivos selecionados
        let selectedFiles = [];
        
        // Função para lidar com a seleção de arquivos
        function handleFileSelection(fileInput) {
            if (fileInput.files.length > 0) {
                selectedFiles = Array.from(fileInput.files);
                updateFilePreview();
                
                // Mostrar toast de sucesso
                showToastSucesso(selectedFiles.length + ' arquivo(s) selecionado(s) com sucesso!');
            }
        }
        
        // Função para atualizar o preview dos arquivos
        function updateFilePreview() {
            const container = document.getElementById('filePreviewContainer');
            const list = document.getElementById('filePreviewList');
            
            if (selectedFiles.length === 0) {
                container.style.display = 'none';
                list.innerHTML = '';
                return;
            }
            
            container.style.display = 'block';
            list.innerHTML = '';
            
            selectedFiles.forEach((file, index) => {
                const fileItem = document.createElement('div');
                fileItem.className = 'file-preview-item';
                fileItem.innerHTML = `
                    <div class="file-preview-info">
                        <i class="bi bi-file-earmark"></i>
                        <span class="file-preview-name">${file.name}</span>
                        <span class="file-preview-size">(${formatFileSize(file.size)})</span>
                    </div>
                    <button type="button" class="file-preview-remove" onclick="removeFile(${index})">
                        <i class="bi bi-x-circle"></i>
                    </button>
                `;
                list.appendChild(fileItem);
            });
        }
        
        // Função para formatar o tamanho do arquivo
        function formatFileSize(bytes) {
            if (bytes === 0) return '0 Bytes';
            const k = 1024;
            const sizes = ['Bytes', 'KB', 'MB', 'GB'];
            const i = Math.floor(Math.log(bytes) / Math.log(k));
            return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
        }
        
        // Função para remover arquivo da seleção
        function removeFile(index) {
            selectedFiles.splice(index, 1);
            updateFilePreview();
            
            // Atualizar o FileUpload ASP.NET
            const fileInput = document.getElementById('<%= fuAnexos.ClientID %>');
            const dataTransfer = new DataTransfer();
            
            selectedFiles.forEach(file => {
                dataTransfer.items.add(file);
            });
            
            fileInput.files = dataTransfer.files;
        }
        
        // Função para arrastar e soltar arquivos
        function setupDragAndDrop() {
            const dropArea = document.querySelector('.file-upload-area');
            
            ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
                dropArea.addEventListener(eventName, preventDefaults, false);
            });
            
            function preventDefaults(e) {
                e.preventDefault();
                e.stopPropagation();
            }
            
            ['dragenter', 'dragover'].forEach(eventName => {
                dropArea.addEventListener(eventName, highlight, false);
            });
            
            ['dragleave', 'drop'].forEach(eventName => {
                dropArea.addEventListener(eventName, unhighlight, false);
            });
            
            function highlight() {
                dropArea.style.borderColor = 'var(--primary)';
                dropArea.style.backgroundColor = 'rgba(37, 99, 235, 0.1)';
            }
            
            function unhighlight() {
                dropArea.style.borderColor = 'var(--gray-300)';
                dropArea.style.backgroundColor = 'var(--gray-50)';
            }
            
            dropArea.addEventListener('drop', handleDrop, false);
            
            function handleDrop(e) {
                const dt = e.dataTransfer;
                const files = dt.files;
                
                if (files.length > 0) {
                    const fileInput = document.getElementById('<%= fuAnexos.ClientID %>');
                    fileInput.files = files;
                    handleFileSelection(fileInput);
                }
            }
        }

        // Inicializar quando a página carregar
        document.addEventListener('DOMContentLoaded', function () {
            setupCharacterCounters();
            autoResizeTextarea();
            setupDragAndDrop();
        });

        // Debug: Verificar se Bootstrap está carregado
        console.log('Bootstrap carregado:', typeof bootstrap !== 'undefined');
    </script>
</asp:Content>