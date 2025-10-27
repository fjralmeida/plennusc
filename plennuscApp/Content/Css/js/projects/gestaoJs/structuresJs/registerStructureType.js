// Atualiza nome da view automaticamente
function atualizarViewNome() {
    var descricao = document.getElementById('<%= txtDescricao.ClientID %>').value;
    var lblView = document.getElementById('<%= lblViewNome.ClientID %>');

    if (descricao) {
        lblView.innerText = 'VW_' + descricao.toUpperCase().replace(/ /g, '_');
    } else {
        lblView.innerText = 'VW_';
    }
}

document.getElementById('<%= txtDescricao.ClientID %>').addEventListener('input', atualizarViewNome);

// Executa uma vez ao carregar a página
document.addEventListener('DOMContentLoaded', function () {
    atualizarViewNome();
});