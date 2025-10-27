function alternarSelecao(chk) {
    var row = chk.closest('tr');
    if (row) {
        if (chk.checked) {
            row.classList.add('linha-selecionada');
        } else {
            row.classList.remove('linha-selecionada');
        }
    }
    atualizarSelecionarTodos();
}

function selecionarTodos(chkHeader) {
    var grid = document.getElementById("GridAssociados");
    var checkboxes = grid.querySelectorAll("input[type='checkbox'][id*='chkSelecionar']");
    checkboxes.forEach(function (chk) {
        chk.checked = chkHeader.checked;
        var row = chk.closest('tr');
        if (chk.checked) {
            row.classList.add('linha-selecionada');
        } else {
            row.classList.remove('linha-selecionada');
        }
    });
}

function atualizarSelecionarTodos() {
    var grid = document.getElementById("GridAssociados");
    var chkTodos = grid.querySelector("input[type='checkbox'][id*='chkSelecionarTodos']");
    var checkboxes = grid.querySelectorAll("input[type='checkbox'][id*='chkSelecionar']");
    var todosMarcados = true;
    checkboxes.forEach(function (chk) {
        if (!chk.checked) {
            todosMarcados = false;
        }
    });
    if (chkTodos) {
        chkTodos.checked = todosMarcados;
    }
}

// 🔁 FUNÇÃO  MOSTRAR OVERLAY DE CARREGAMENTO
function mostrarLoading() {
    document.getElementById('loadingOverlay').style.display = 'block';
}


function mostrarResultadoModal(texto) {
    document.getElementById("modalResultadoConteudo").textContent = texto;
    var modal = new bootstrap.Modal(document.getElementById('resultadoModal'));
    modal.show();
}

// 🔁 FUNÇÃO PARA MODAL MENSAGEM
function abrirModal() {
    document.getElementById('modalEscolherMensagem').style.display = 'block';
}

function fecharModal() {
    document.getElementById('modalEscolherMensagem').style.display = 'none';
}

function selecionarTemplate(template) {
    document.getElementById('<%= hfTemplateEscolhido.ClientID %>').value = template;
    fecharModal();
}


// mapeia template -> id do input date
const mapInputPorTemplate = {
    "Suspensao": "txtDataSuspensao",
    "Definitivo": "txtDataDefinitivo",
    "DoisBoletos": "txtDataNovaOpcao"
};

function selecionarTemplate(template) {
    const inputId = mapInputPorTemplate[template];
    const input = document.getElementById(inputId);

    if (!input) {
        // fallback raro: se o input não existir, não bloqueia a escolha
        document.getElementById('<%= hfTemplateEscolhido.ClientID %>').value = template;
        fecharModal();
        return;
    }

    // limpa estado de erro anterior
    input.classList.remove('is-invalid');

    const val = (input.value || "").trim();
    if (!val) {
        // marca como inválido e foca — NÃO fecha o modal
        input.classList.add('is-invalid');
        input.focus();
        input.scrollIntoView({ behavior: 'smooth', block: 'center' });
        return;
    }

    // data ok → grava e fecha
    document.getElementById('<%= hfTemplateEscolhido.ClientID %>').value = template;
    fecharModal();
}