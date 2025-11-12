<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="companyRegistration.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.companyRegistration" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/Css/projects/gestao/structuresCss/Company-Registration.css" rel="stylesheet" />
    
    <!-- jQuery Mask Plugin -->
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.16/jquery.mask.min.js"></script>
    
    <!-- SweetAlert2 -->
    <link href="https://cdn.jsdelivr.net/npm/sweetalert2@11.7.12/dist/sweetalert2.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11.7.12/dist/sweetalert2.all.min.js"></script>

    <script>
        $(document).ready(function () {
            // Máscara para CNPJ
            $('#<%= txtCNPJ.ClientID %>').mask('00.000.000/0000-00');

            // Validação customizada
            function validarFormulario() {
                var cnpj = $('#<%= txtCNPJ.ClientID %>').val().replace(/\D/g, '');
                if (cnpj.length !== 14) {
                    Swal.fire('Atenção', 'CNPJ deve ter 14 dígitos', 'warning');
                    return false;
                }
                return true;
            }
            
            // Anexar validação ao botão de salvar
            $('#<%= btnSalvar.ClientID %>').click(function (e) {
                if (!validarFormulario()) {
                    e.preventDefault();
                    e.stopPropagation();
                }
            });
        });
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="company-registration-container">
        
        <!-- Header -->
        <div class="page-header">
            <h2 class="page-title">
                <div class="title-icon">
                    <i class="bi bi-building-add"></i>
                </div>
                Cadastro de Empresa
            </h2>
        </div>

        <!-- Alertas -->
        <asp:Panel ID="pnlMensagem" runat="server" Visible="false" CssClass="alert alert-dismissible fade show">
            <asp:Literal ID="litMensagem" runat="server"></asp:Literal>
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </asp:Panel>

        <!-- Card Principal -->
        <div class="main-card">
            <div class="card-header">
                <h3 class="card-title">
                    <i class="bi bi-building"></i>
                    Dados da Empresa
                </h3>
            </div>
            
            <div class="card-body">
                <!-- Razão Social -->
                <div class="form-group">
                    <label class="form-label required-field">Razão Social</label>
                    <asp:TextBox ID="txtRazaoSocial" runat="server" CssClass="form-control" 
                        placeholder="Digite a razão social completa" MaxLength="200" />
                    <asp:RequiredFieldValidator ID="rfvRazaoSocial" runat="server" 
                        ControlToValidate="txtRazaoSocial" ErrorMessage="Razão social é obrigatória"
                        CssClass="text-danger" Display="Dynamic" ValidationGroup="CadastroEmpresa" />
                </div>

                <!-- Nome Fantasia -->
                <div class="form-group">
                    <label class="form-label required-field">Nome Fantasia</label>
                    <asp:TextBox ID="txtNomeFantasia" runat="server" CssClass="form-control" 
                        placeholder="Digite o nome fantasia" MaxLength="100" />
                    <asp:RequiredFieldValidator ID="rfvNomeFantasia" runat="server" 
                        ControlToValidate="txtNomeFantasia" ErrorMessage="Nome fantasia é obrigatório"
                        CssClass="text-danger" Display="Dynamic" ValidationGroup="CadastroEmpresa" />
                </div>

                <!-- CNPJ -->
                <div class="form-group">
                    <label class="form-label required-field">CNPJ</label>
                    <asp:TextBox ID="txtCNPJ" runat="server" CssClass="form-control" 
                        placeholder="00.000.000/0000-00" MaxLength="18" />
                    <asp:RequiredFieldValidator ID="rfvCNPJ" runat="server" 
                        ControlToValidate="txtCNPJ" ErrorMessage="CNPJ é obrigatório"
                        CssClass="text-danger" Display="Dynamic" ValidationGroup="CadastroEmpresa" />
                    <asp:CustomValidator ID="cvCNPJ" runat="server" 
                        ControlToValidate="txtCNPJ" ErrorMessage="CNPJ inválido"
                        CssClass="text-danger" Display="Dynamic" 
                        OnServerValidate="cvCNPJ_ServerValidate" ValidationGroup="CadastroEmpresa" />
                </div>

                <!-- Configurações -->
                <div class="config-section">
                    <label class="config-label">Configurações</label>
                    <div class="form-check">
                        <asp:CheckBox ID="chkLiberaAcesso" runat="server" CssClass="form-check-input" Checked="true" />
                        <label class="form-check-label" for="<%= chkLiberaAcesso.ClientID %>">
                            Liberar Acesso
                        </label>
                    </div>
                    <div class="form-check">
                        <asp:CheckBox ID="chkAtivo" runat="server" CssClass="form-check-input" Checked="true" />
                        <label class="form-check-label" for="<%= chkAtivo.ClientID %>">
                            Ativo
                        </label>
                    </div>
                </div>

                <!-- Botões de Ação -->
                <div class="action-buttons">
                    <asp:Button ID="btnSalvar" runat="server" Text="Salvar Empresa" 
                        CssClass="btn-save" ValidationGroup="CadastroEmpresa" 
                        OnClick="btnSalvar_Click" />
                </div>
            </div>
        </div>
    </div>
</asp:Content>