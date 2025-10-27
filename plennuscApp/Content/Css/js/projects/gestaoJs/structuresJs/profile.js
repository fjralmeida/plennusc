document.addEventListener("DOMContentLoaded", function () {
    const img = document.getElementById("imgFotoPerfil");
    const file = document.getElementById("fuFoto");
    if (img && file) {
        img.addEventListener("click", function () {
            file.click();
        });
    }
});
function previewFoto() {
    const fileInput = document.getElementById("fuFoto");
    const imgPreview = document.getElementById("imgFotoPerfil");
    if (fileInput.files && fileInput.files[0]) {
        const reader = new FileReader();
        reader.onload = function (e) {
            imgPreview.src = e.target.result;
        };
        reader.readAsDataURL(fileInput.files[0]);
    }
}