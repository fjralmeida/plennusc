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