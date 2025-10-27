// ❌ ABRE MODAL DE INATIVAR
function abrirModalInativarBtn(btn) {
    const codPessoa = btn.getAttribute('data-codpessoa');
    const nome = btn.getAttribute('data-nome');

    $('#<%= hfCodPessoaInativa.ClientID %>').val(codPessoa);
    document.getElementById("lblNomeUsuarioInativa").innerText = nome;

    const myModal = new bootstrap.Modal(document.getElementById('modalInativarUsuario'));
    myModal.show();
}


// ✅ FUNÇÃO ÚNICA DE MÁSCARAS
function aplicarMascaras() {
    const campoCPF = $('#<%= txtDocCPF.ClientID %>');
    const campoBuscaCPF = $('#<%= txtBuscaCPF.ClientID %>');

    if (campoCPF.length) campoCPF.unmask().mask('000.000.000-00', { reverse: true });
    if (campoBuscaCPF.length) campoBuscaCPF.unmask().mask('000.000.000-00', { reverse: true });

    const phoneBehavior = function (val) {
        const nums = val.replace(/\D/g, '').slice(0, 11);
        return nums.length === 11 ? '(00) 00000-0000' : '(00) 0000-00009';
    };
    const phoneOptions = {
        onKeyPress: function (val, e, field, options) {
            field.mask(phoneBehavior.apply({}, arguments), options);
        }
    };

    const tel1 = $('#<%= txtTelefone1.ClientID %>');
    const tel2 = $('#<%= txtTelefone2.ClientID %>');
    const tel3 = $('#<%= txtTelefone3.ClientID %>');

    if (tel1.length) tel1.attr('maxlength', 15).unmask().mask(phoneBehavior, phoneOptions);
    if (tel2.length) tel2.attr('maxlength', 15).unmask().mask(phoneBehavior, phoneOptions);
    if (tel3.length) tel3.attr('maxlength', 15).unmask().mask(phoneBehavior, phoneOptions);
}

// ✅ APLICA MÁSCARAS QUANDO A PÁGINA CARREGA
$(document).ready(function () {
    aplicarMascaras();
});

// ✅ (Opcional) Se usar UpdatePanel, re-aplica após postback parcial
if (typeof (Sys) !== "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager) {
    Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
        aplicarMascaras();
    });
}

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

Sys.Application.add_load(function () {
    if ($('#<%= PanelCadastro.ClientID %>').is(':visible')) {
        aplicarMascaras();
    }
});