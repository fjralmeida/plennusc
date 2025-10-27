let subtipoCount = 1;

function adicionarSubtipo() {
    subtipoCount++;
    const container = document.getElementById('containerSubtipos');

    // Pega a próxima ordem disponível (maior ordem atual + 1)
    const proximaOrdem = obterProximaOrdem();

    const subtipoItem = document.createElement('div');
    subtipoItem.className = 'subtipo-item';
    subtipoItem.innerHTML = `
                <div class="subtipo-input">
                    <input type="text" class="form-control" name="subtipo_${subtipoCount}" 
                        placeholder="Digite o nome da estrutura" maxlength="100" />
                </div>
                <div class="subtipo-ordem">
                    <input type="number" class="form-control campo-ordem" name="ordem_${subtipoCount}" 
                        placeholder="Ordem" value="${proximaOrdem}" min="1" 
                        onchange="reordenarAutomaticamente(this)" />
                </div>
                <div class="subtipo-actions">
                    <button type="button" class="btn-add-small" onclick="adicionarSubtipo()" title="Adicionar outra estrutura">
                        <i class="bi bi-plus-lg"></i>
                    </button>
                    <button type="button" class="btn-remove" onclick="removerSubtipo(this)" title="Remover estrutura">
                        <i class="bi bi-dash-lg"></i>
                    </button>
                </div>
            `;

    container.appendChild(subtipoItem);
}

// Função para obter a próxima ordem disponível
function obterProximaOrdem() {
    const camposOrdem = document.querySelectorAll('.campo-ordem');
    let maiorOrdem = 0;

    camposOrdem.forEach(campo => {
        const ordem = parseInt(campo.value) || 0;
        if (ordem > maiorOrdem) {
            maiorOrdem = ordem;
        }
    });

    return maiorOrdem + 1;
}

// Função principal de reordenação automática
function reordenarAutomaticamente(campoAlterado) {
    const novaOrdem = parseInt(campoAlterado.value);
    const ordemAntiga = parseInt(campoAlterado.getAttribute('data-ordem-anterior')) || parseInt(campoAlterado.value);

    if (isNaN(novaOrdem) || novaOrdem < 1) {
        campoAlterado.value = ordemAntiga;
        return;
    }

    const todosCamposOrdem = document.querySelectorAll('.campo-ordem');
    let campoParaTrocar = null;

    // Encontra o campo que tem a ordem para a qual estamos mudando
    todosCamposOrdem.forEach(campo => {
        if (campo !== campoAlterado && parseInt(campo.value) === novaOrdem) {
            campoParaTrocar = campo;
        }
    });

    // Se encontrou um campo com a mesma ordem, faz a troca
    if (campoParaTrocar) {
        campoParaTrocar.value = ordemAntiga;
        campoParaTrocar.setAttribute('data-ordem-anterior', ordemAntiga);
    }

    // Atualiza a ordem anterior para o novo valor
    campoAlterado.setAttribute('data-ordem-anterior', novaOrdem);
}

function removerSubtipo(button) {
    const subtipoItem = button.closest('.subtipo-item');
    const ordemRemovida = parseInt(subtipoItem.querySelector('.campo-ordem').value);

    subtipoItem.remove();

    // Reorganiza as ordens dos itens restantes
    reorganizarOrdensAposRemocao(ordemRemovida);
}

// Reorganiza as ordens quando um item é removido
function reorganizarOrdensAposRemocao(ordemRemovida) {
    const camposOrdem = document.querySelectorAll('.campo-ordem');
    const ordens = [];

    // Coleta todas as ordens atuais
    camposOrdem.forEach(campo => {
        ordens.push(parseInt(campo.value));
    });

    // Ordena as ordens
    ordens.sort((a, b) => a - b);

    // Reatribui as ordens sequencialmente
    camposOrdem.forEach((campo, index) => {
        campo.value = index + 1;
        campo.setAttribute('data-ordem-anterior', index + 1);
    });
}

function limparCamposSubtipos() {
    document.getElementById('containerSubtipos').innerHTML = '';
    subtipoCount = 0;
    adicionarSubtipo();
}

// Antes de enviar o formulário, coleta todos os dados dos subtipos
document.getElementById('<%= btnSalvarTudo.ClientID %>').addEventListener('click', function () {
    const subtipos = [];
    const subtipoItems = document.querySelectorAll('.subtipo-item');

    subtipoItems.forEach((item, index) => {
        const nome = item.querySelector('input[type="text"]').value.trim();
        const ordem = item.querySelector('.campo-ordem').value;

        if (nome !== '') {
            subtipos.push({
                nome: nome,
                ordem: parseInt(ordem) || 0
            });
        }
    });

    document.getElementById('<%= hdnSubtipos.ClientID %>').value = JSON.stringify(subtipos);
});

// Adiciona um campo inicial se não houver nenhum
document.addEventListener('DOMContentLoaded', function () {
    if (document.getElementById('containerSubtipos').children.length === 0) {
        adicionarSubtipo();
    }

    // Inicializa os data-atributes de ordem anterior
    const camposOrdem = document.querySelectorAll('.campo-ordem');
    camposOrdem.forEach(campo => {
        campo.setAttribute('data-ordem-anterior', campo.value);
    });
});