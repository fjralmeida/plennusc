<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="demand.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.demand" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="https://fonts.googleapis.com/css2?family=Roboto:wght@300;400;500;700&display=swap" rel="stylesheet">
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css">
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.1.3/dist/js/bootstrap.bundle.min.js"></script>

    <link href="../../Content/Css/projects/gestao/structuresCss/demand-Gestao.css" rel="stylesheet" />

    <script src="../../Content/Css/js/projects/gestaoJs/structuresJs/demand.js"></script>


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
    </script>



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
       
<div class="mb-4">
    <h3 class="section-title"><i class="bi bi-paperclip"></i>Anexos</h3>
    <div class="form-card">
        <label class="form-label">Adicionar arquivos (Máx. 10MB cada)</label>

        <div class="file-upload-area">
            <i class="bi bi-cloud-arrow-up text-muted mb-2"></i>
            <p class="mb-2 small text-muted">Selecione os arquivos e clique em Adicionar</p>
            
            <!-- FILEUPLOAD VISÍVEL E SIMPLES -->
            <asp:FileUpload ID="fuAnexos" runat="server" CssClass="form-control mb-2" AllowMultiple="true" />
            
            <!-- BOTÃO para adicionar arquivos -->
            <asp:Button ID="btnAdicionarAnexos" runat="server" Text="Adicionar Arquivos à Lista" 
                CssClass="btn btn-primary btn-sm" OnClick="btnAdicionarAnexos_Click" />
        </div>

        <div class="input-hint">
            Formatos permitidos: PDF, Word, Excel, imagens (JPG, PNG, GIF)
        </div>
   
        <asp:Repeater ID="rptAnexos" runat="server" OnItemCommand="rptAnexos_ItemCommand">
            <HeaderTemplate>
                <div class="file-preview-container mt-3">
                    <h6 class="small text-muted">Arquivos selecionados:</h6>
            </HeaderTemplate>
            <ItemTemplate>
                <div class="file-preview-item">
                    <div class="file-preview-info">
                        <i class="bi bi-file-earmark"></i>
                        <span class="file-preview-name"><%# Eval("FileName") %></span>
                        <span class="file-preview-size">(<%# Eval("SizeFormatted") %>)</span>
                    </div>
                    <asp:LinkButton ID="btnRemoverAnexo" runat="server" 
                        CommandName="Remover" 
                        CommandArgument='<%# Eval("Index") %>'
                        CssClass="file-preview-remove"
                        OnClientClick="return confirm('Remover este arquivo?');">
                        <i class="bi bi-x-circle"></i>
                    </asp:LinkButton>
                </div>
            </ItemTemplate>
            <FooterTemplate>
                </div>
            </FooterTemplate>
        </asp:Repeater>
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
</asp:Content>