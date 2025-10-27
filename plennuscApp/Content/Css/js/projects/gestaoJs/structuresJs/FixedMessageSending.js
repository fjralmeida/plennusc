function mostrarResultadoModal(texto) {
    document.getElementById("modalResultadoConteudo").textContent = texto;
    var modal = new bootstrap.Modal(document.getElementById('resultadoModal'));
    modal.show();
}

function mostrarLoading() {
    document.getElementById('loadingOverlay').style.display = 'block';
}