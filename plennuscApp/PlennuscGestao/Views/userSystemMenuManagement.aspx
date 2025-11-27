<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="userSystemMenuManagement.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.userSystemMenuManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/Css/projects/gestao/structuresCss/usuario/user-System-Menu-Management.css" rel="stylesheet" />

<script>
    // Realçar checkboxes quando desmarcados (feedback visual)
    document.addEventListener('DOMContentLoaded', function () {
        const checkboxes = document.querySelectorAll('#<%= chkSistemaEmpresas.ClientID %> input[type="checkbox"]');

        checkboxes.forEach(checkbox => {
            checkbox.addEventListener('change', function () {
                const label = this.closest('span');
                if (!this.checked) {
                    // Adiciona efeito visual quando desmarca
                    label.style.background = 'linear-gradient(135deg, #ffeaa7 0%, #ffd166 100%)';
                    label.style.borderColor = '#e74c3c';
                    label.style.boxShadow = '0 0 0 2px rgba(231, 76, 60, 0.2)';

                    // Remove o efeito após 2 segundos
                    setTimeout(() => {
                        label.style.background = '';
                        label.style.borderColor = '';
                        label.style.boxShadow = '';
                    }, 2000);
                }
            });
        });
    });

    // Efeito de ripple nos checkboxes (seu código existente)
    document.addEventListener('click', function (e) {
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

    // Animação ao marcar/desmarcar (seu código existente)
    document.querySelectorAll('.checkbox-item input[type="checkbox"]').forEach(checkbox => {
        checkbox.addEventListener('change', function () {
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
        <h4>Vincular  Sistemas a Usuário e Menus</h4>
        
        <!-- AVISO FIXO IMPORTANTE -->
        <div class="aviso-desvinculo">
            <div class="d-flex align-items-start">
                <div class="aviso-icon">⚠️</div>
                <div>
                    <div class="aviso-titulo">ATENÇÃO: Controle de Acesso</div>
                    <p class="aviso-texto">
                        <span class="aviso-destaque">Sistemas NÃO MARCADOS serão DESVINCULADOS automaticamente.</span><br/>
                        Certifique-se de marcar <strong>TODOS</strong> os sistemas que o usuário deve acessar. 
                        Ao desmarcar um sistema, todos os menus vinculados serão <strong>PERDIDOS</strong>.
                    </p>
                </div>
            </div>
        </div>

        <!-- 1. Selecionar Usuário -->
        <div class="row mb-3">
            <div class="col-md-6">
               <%-- <label class="form-label">Selecionar Usuário:</label>--%>
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
                    CssClass="btn-primary" OnClick="btnSalvarVinculos_Click" />
            </div>
        </div>
    </div>
</asp:Content>