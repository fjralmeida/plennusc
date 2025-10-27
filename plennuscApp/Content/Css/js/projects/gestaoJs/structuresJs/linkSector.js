// Funções para exibir toasts (iguais às da outra página)
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