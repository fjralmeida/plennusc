<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="userSystemMenuManagement.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.userSystemMenuManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/Css/projects/gestao/structuresCss/usuario/user-System-Menu-Management.css" rel="stylesheet" />

    <script>
    // Efeito de ripple nos checkboxes
    document.addEventListener('click', function(e) {
        if (e.target.closest('.checkbox-item')) {
            const checkboxItem = e.target.closest('.checkbox-item');
            const ripple = document.createElement('span');
            const rect = checkboxItem.getBoundingClientRect();
            const size = Math.max(rect.width, rect.height);
            const x = e.clientX - rect.left - size / 2;
            const y = e.clientY - rect.top - size / 2;
            
            ripple.style.cssText = `
                width: ${size}px;
                height: ${size}px;
                left: ${x}px;
                top: ${y}px;
            `;
            ripple.classList.add('ripple');
            
            checkboxItem.appendChild(ripple);
            
            setTimeout(() => {
                ripple.remove();
            }, 600);
        }
    });

    // Animação ao marcar/desmarcar
    document.querySelectorAll('.checkbox-item input[type="checkbox"]').forEach(checkbox => {
        checkbox.addEventListener('change', function() {
            const item = this.closest('.checkbox-item');
            if (this.checked) {
                item.classList.add('checked');
            } else {
                item.classList.remove('checked');
            }
        });
    });
    </script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <h4>Vincular Usuário a Sistemas e Menus</h4>
        
        <!-- 1. Selecionar Usuário -->
        <div class="row mb-3">
            <div class="col-md-6">
                <asp:DropDownList ID="ddlUsuarios" runat="server" CssClass="form-control" AutoPostBack="true"
                    OnSelectedIndexChanged="ddlUsuarios_SelectedIndexChanged">
                </asp:DropDownList>
            </div>
        </div>

        <!-- 2. Sistemas×Empresas Disponíveis COM PAINEL -->
        <div class="row mb-3">
            <div class="col-12">
                <h5>Sistemas × Empresas Disponíveis</h5>
                <div class="checkbox-panel" ID="idCheck" runat="server" visible="false">
                    <div class="checkbox-container">
                        <asp:CheckBoxList ID="chkSistemaEmpresas" runat="server"
                            AutoPostBack="true" OnSelectedIndexChanged="chkSistemaEmpresas_SelectedIndexChanged">
                        </asp:CheckBoxList>
                    </div>
                </div>
            </div>
        </div>

        <!-- 3. Container para MÚLTIPLAS seções de menus -->
        <asp:Panel ID="pnlMultiplosMenus" runat="server" Visible="false">
            <!-- Esta Panel será preenchida dinamicamente com várias seções -->
        </asp:Panel>
        
        <!-- Botão Salvar -->
        <div class="row">
            <div class="col-12">
                <asp:Button ID="btnSalvarVinculos" runat="server" Text="Salvar Vínculos"
                    CssClass="btn btn-primary" OnClick="btnSalvarVinculos_Click" />
            </div>
        </div>
    </div>
</asp:Content>