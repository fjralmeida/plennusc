function configurarControleBotoes() {
    const ddlStatus = document.getElementById('<%= ddlStatusAcompanhamento.ClientID %>');
    const hdnStatusOriginal = document.getElementById('<%= hdnStatusOriginal.ClientID %>');

    const btnSolicitarAprovacao = document.getElementById('<%= btnSolicitarAprovacao.ClientID %>');
    const btnRecusar = document.getElementById('<%= btnRecusar.ClientID %>');
    const btnEncerrar = document.getElementById('<%= btnEncerrar.ClientID %>');
    const btnAdicionarAcompanhamento = document.getElementById('<%= btnAdicionarAcompanhamento.ClientID %>'); // 🔥 NOVO

    if (ddlStatus && hdnStatusOriginal) {
        ddlStatus.addEventListener('change', function () {
            const statusAtual = ddlStatus.value;
            const statusOriginal = hdnStatusOriginal.value;
            const textoStatusAtual = ddlStatus.options[ddlStatus.selectedIndex].text;

            // Habilita cada botão apenas para seu status específico
            const habilitarSolicitar = (statusAtual !== statusOriginal) && textoStatusAtual === "Aguardando Aprovação";
            const habilitarRecusar = (statusAtual !== statusOriginal) && textoStatusAtual === "Recusada";
            const habilitarConcluir = (statusAtual !== statusOriginal) && textoStatusAtual === "Concluída";

            // 🔥 NOVO: Desabilita botão de acompanhamento para "Aguardando Aprovação"
            const desabilitarAcompanhamento = textoStatusAtual === "Aguardando Aprovação";

            if (btnSolicitarAprovacao) {
                btnSolicitarAprovacao.disabled = !habilitarSolicitar;
                btnSolicitarAprovacao.style.opacity = habilitarSolicitar ? '1' : '0.5';
                btnSolicitarAprovacao.style.pointerEvents = habilitarSolicitar ? 'auto' : 'none';
            }

            if (btnRecusar) {
                btnRecusar.disabled = !habilitarRecusar;
                btnRecusar.style.opacity = habilitarRecusar ? '1' : '0.5';
                btnRecusar.style.pointerEvents = habilitarRecusar ? 'auto' : 'none';
            }

            if (btnEncerrar) {
                btnEncerrar.disabled = !habilitarConcluir;
                btnEncerrar.style.opacity = habilitarConcluir ? '1' : '0.5';
                btnEncerrar.style.pointerEvents = habilitarConcluir ? 'auto' : 'none';
            }

            // 🔥 NOVO: Controla botão de acompanhamento
            if (btnAdicionarAcompanhamento) {
                btnAdicionarAcompanhamento.disabled = desabilitarAcompanhamento;
                btnAdicionarAcompanhamento.style.opacity = desabilitarAcompanhamento ? '0.5' : '1';
                btnAdicionarAcompanhamento.style.pointerEvents = desabilitarAcompanhamento ? 'none' : 'auto';
                btnAdicionarAcompanhamento.title = desabilitarAcompanhamento
                    ? "Status aguardando aprovação - acompanhamento bloqueado"
                    : "Enviar Acompanhamento";
            }
        });

        // Configuração inicial - desabilita todos os botões
        if (btnSolicitarAprovacao) {
            btnSolicitarAprovacao.disabled = true;
            btnSolicitarAprovacao.style.opacity = '0.5';
            btnSolicitarAprovacao.style.pointerEvents = 'none';
        }

        if (btnRecusar) {
            btnRecusar.disabled = true;
            btnRecusar.style.opacity = '0.5';
            btnRecusar.style.pointerEvents = 'none';
        }

        if (btnEncerrar) {
            btnEncerrar.disabled = true;
            btnEncerrar.style.opacity = '0.5';
            btnEncerrar.style.pointerEvents = 'none';
        }

        // 🔥 NOVO: Configuração inicial do botão de acompanhamento
        if (btnAdicionarAcompanhamento) {
            const textoStatusAtual = ddlStatus.options[ddlStatus.selectedIndex].text;
            const desabilitarAcompanhamento = textoStatusAtual === "Aguardando Aprovação";

            btnAdicionarAcompanhamento.disabled = desabilitarAcompanhamento;
            btnAdicionarAcompanhamento.style.opacity = desabilitarAcompanhamento ? '0.5' : '1';
            btnAdicionarAcompanhamento.style.pointerEvents = desabilitarAcompanhamento ? 'none' : 'auto';
            btnAdicionarAcompanhamento.title = desabilitarAcompanhamento
                ? "Status aguardando aprovação - acompanhamento bloqueado"
                : "Enviar Acompanhamento";
        }
    }
}

// Executa quando a página carrega
document.addEventListener('DOMContentLoaded', function () {
    configurarControleBotoes();
});


function checkDemandStatus() {
    const statusBadge = document.getElementById('<%= lblStatusBadge.ClientID %>');
    const editorSection = document.getElementById('<%= editorSection.ClientID %>');

    if (statusBadge && statusBadge.textContent.includes('Fechada')) {
        if (editorSection) {
            editorSection.classList.add('demand-closed');
            const textarea = document.getElementById('<%= txtNovoAcompanhamento.ClientID %>');
            if (textarea) {
                textarea.disabled = true;
                textarea.placeholder = "Demanda fechada - não é possível adicionar acompanhamentos";
            }

            const button = document.getElementById('<%= btnAdicionarAcompanhamento.ClientID %>');
            if (button) {
                button.disabled = true;
                button.textContent = "Demanda Fechada";
                button.classList.add("btn-secondary");
            }

            // Desabilitar upload de arquivos também
            const fileUploadArea = document.querySelector('.file-upload-area');
            if (fileUploadArea) {
                fileUploadArea.style.opacity = '0.5';
                fileUploadArea.style.pointerEvents = 'none';
            }
        }
    }
}

// Toggle do histórico
function toggleHistory() {
    const section = document.getElementById('historySection');
    const icon = document.getElementById('historyIcon');

    section.classList.toggle('history-visible');
    icon.classList.toggle('bi-chevron-down');
    icon.classList.toggle('bi-chevron-up');
}

// Editor de texto simples
document.querySelectorAll('.toolbar-btn').forEach(button => {
    button.addEventListener('click', function () {
        if (this.tagName === 'BUTTON') {
            const command = this.dataset.command;
            document.execCommand(command, false, null);
            this.classList.toggle('active');
        }
    });
});

document.getElementById('fontFamily').addEventListener('change', function () {
    document.execCommand('fontName', false, this.value);
});

// Executar quando a página carregar
document.addEventListener('DOMContentLoaded', function () {
    checkDemandStatus();
    setupDragAndDrop();
});


function createToast(message, type, title) {
    var container = document.getElementById('globalToastContainer');
    if (!container) return;
    container.style.display = 'block';

    var toast = document.createElement('div');
    toast.className = 'toast ' + type;
    toast.innerHTML = (title ? '<div class=\"title\">' + title + '</div>' : '') +
        '<div class=\"msg\">' + message + '</div>';
    container.appendChild(toast);

    // show
    setTimeout(function () { toast.classList.add('show'); }, 10);

    // remove after 5s
    setTimeout(function () {
        toast.classList.remove('show');
        setTimeout(function () { try { container.removeChild(toast); } catch (e) { } }, 300);
    }, 5000);
}

function showToastSucesso(message) {
    createToast(message, 'success', 'Sucesso');
}
function showToastErro(message) {
    createToast(message, 'error', 'Erro');
}
function showToastAviso(message) {
    createToast(message, 'warning', 'Aviso');
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
    const counter = document.getElementById('fileCounter');

    if (selectedFiles.length === 0) {
        container.style.display = 'none';
        list.innerHTML = '';
        counter.textContent = '0 arquivos selecionados';
        return;
    }

    container.style.display = 'block';
    list.innerHTML = '';
    counter.textContent = selectedFiles.length + ' arquivo(s) selecionado(s)';

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

// Função para arrastar e soltar arquivos (atualizada)
function setupDragAndDrop() {
    const dropArea = document.getElementById('fileUploadArea');
    if (!dropArea) return;

    ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
        dropArea.addEventListener(eventName, preventDefaults, false);
    });

    function preventDefaults(e) {
        e.preventDefault();
        e.stopPropagation();
    }

    ['dragenter', 'dragover'].forEach(eventName => {
        dropArea.addEventListener(eventName, function () {
            dropArea.classList.add('dragover');
        }, false);
    });

    ['dragleave', 'drop'].forEach(eventName => {
        dropArea.addEventListener(eventName, function () {
            dropArea.classList.remove('dragover');
        }, false);
    });

    dropArea.addEventListener('drop', handleDrop, false);

    function handleDrop(e) {
        const dt = e.dataTransfer;
        const files = dt.files;

        if (files.length > 0) {
            const fileInput = document.getElementById('<%= fuAnexos.ClientID %>');

            // Combinar arquivos existentes com novos
            const allFiles = [...selectedFiles, ...Array.from(files)];
            selectedFiles = allFiles;

            // Atualizar o FileUpload ASP.NET
            const dataTransfer = new DataTransfer();
            selectedFiles.forEach(file => {
                dataTransfer.items.add(file);
            });

            fileInput.files = dataTransfer.files;
            updateFilePreview();

            showToastSucesso(files.length + ' arquivo(s) adicionado(s)!');
        }
    }
}
// Função para limpar os anexos (chamada após o envio)
function limparAnexos() {
    selectedFiles = [];
    updateFilePreview();

    const fileInput = document.getElementById('<%= fuAnexos.ClientID %>');
    fileInput.value = '';
}