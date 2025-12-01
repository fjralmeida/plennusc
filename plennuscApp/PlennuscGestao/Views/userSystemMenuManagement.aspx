<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="userSystemMenuManagement.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.userSystemMenuManagement" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/Css/projects/gestao/structuresCss/usuario/user-System-Menu-Management.css" rel="stylesheet" />

<script>
    // Estilização dinâmica para CheckBoxList do ASP.NET
    document.addEventListener('DOMContentLoaded', function () {
        const checkboxes = document.querySelectorAll('.checkbox-container input[type="checkbox"]');

        checkboxes.forEach(checkbox => {
            const td = checkbox.closest('td');
            if (!td) return;

            // Estado inicial
            if (checkbox.checked) {
                td.classList.add('checked');
            }

            // Evento de change
            checkbox.addEventListener('change', function () {
                if (this.checked) {
                    td.classList.add('checked');
                } else {
                    td.classList.remove('checked');

                    // Efeito visual quando desmarca
                    td.style.background = 'linear-gradient(135deg, #ffeaa7 0%, #ffd166 100%)';
                    td.style.borderColor = '#e74c3c';
                    td.style.boxShadow = '0 0 0 2px rgba(231, 76, 60, 0.2)';

                    setTimeout(() => {
                        td.style.background = '';
                        td.style.borderColor = '';
                        td.style.boxShadow = '';
                    }, 2000);
                }
            });

            // Efeito de ripple
            td.addEventListener('click', function (e) {
                const ripple = document.createElement('span');
                const rect = td.getBoundingClientRect();
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

                td.appendChild(ripple);

                setTimeout(() => {
                    ripple.remove();
                }, 600);
            });
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
            <!-- COLUNA ESQUERDA: Filtro -->
            <div class="col-md-6">
                <label class="form-label">Filtrar Usuário:</label>
        
                <div class="input-group">
                    <asp:TextBox ID="txtFiltroUsuario" runat="server" 
                        CssClass="form-control" 
                        placeholder="Digite parte do nome para filtrar..."
                        AutoPostBack="false" />
            
                    <asp:Button ID="btnFiltrar" runat="server" 
                        Text="Filtrar" 
                        CssClass="btn btn-outline-secondary"
                        OnClick="btnFiltrar_Click" />
            
                    <asp:Button ID="btnLimparFiltro" runat="server" 
                        Text="Limpar" 
                        CssClass="btn btn-outline-secondary"
                        OnClick="btnLimparFiltro_Click" />
                </div>
        
                <small class="text-muted">
                    <asp:Literal ID="litInfoUsuarios" runat="server" Text="Mostrando todos os usuários" />
                </small>
            </div>
    
            <!-- COLUNA DIREITA: Dropdown -->
            <div class="col-md-6 ddl-container">
                <label class="form-label">Sistemas × Empresas Disponíveis</label>
        
                <asp:DropDownList ID="ddlUsuarios" runat="server" 
                    CssClass="form-control" 
                    AutoPostBack="true"
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
        
      <div class="row">
            <div class="col-12" style="display: flex; justify-content: end;">
                <asp:Button ID="btnSalvarVinculos" runat="server" Text="Salvar Vínculos" 
                    CssClass="btn btn-primary" OnClick="btnSalvarVinculos_Click" 
                    Enabled="false" />
            </div>
        </div>
    </div>
</asp:Content>