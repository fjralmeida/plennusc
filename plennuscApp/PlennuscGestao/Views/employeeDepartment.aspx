<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="employeeDepartment.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.employeeDepartment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Departamento de Funcionários</title>
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
    
    <!-- SweetAlert2 CSS -->
    <link href="https://cdn.jsdelivr.net/npm/sweetalert2@11/dist/sweetalert2.min.css" rel="stylesheet">
    <link href="../../Content/Css/projects/gestao/structuresCss/employee-Department.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <div class="d-flex justify-content-between align-items-center mb-4">
            <h2 class="titulo-pagina">
                <i class="bi bi-diagram-3 me-2"></i>
                Departamentos
            </h2>
            <button type="button" class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#modalNovoDepartamento">
                <i class="fas fa-plus me-2"></i>Novo Departamento
            </button>
        </div>

        <!-- Grid de Departamentos -->
        <div class="grid-container">
            <asp:GridView ID="gvDepartments"
                runat="server"
                AutoGenerateColumns="false"
                GridLines="None"
                CssClass="custom-grid align-middle">
                <Columns>
                    <asp:BoundField DataField="CodDepartamento" HeaderText="Código" 
                        ItemStyle-CssClass="col-codigo" HeaderStyle-CssClass="col-codigo" />
                    <asp:BoundField DataField="Nome" HeaderText="Nome" 
                        ItemStyle-CssClass="col-nome" HeaderStyle-CssClass="col-nome" />
                    <asp:BoundField DataField="NumRamal" HeaderText="Ramal" 
                        ItemStyle-CssClass="col-ramal" HeaderStyle-CssClass="col-ramal" />
                    <asp:BoundField DataField="EmailGeral" HeaderText="E-mail" 
                        ItemStyle-CssClass="col-email" HeaderStyle-CssClass="col-email" />
                    <asp:BoundField DataField="Telefone" HeaderText="Telefone" 
                        ItemStyle-CssClass="col-telefone" HeaderStyle-CssClass="col-telefone" />
                    <asp:BoundField DataField="Informacoes_Log_I" HeaderText="Criado em" 
                        ItemStyle-CssClass="col-data" HeaderStyle-CssClass="col-data" 
                        DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:TemplateField HeaderText="Ações" ItemStyle-CssClass="col-acoes">
                        <ItemTemplate>
                            <asp:Button ID="btnEditar" runat="server" Text="Editar" 
                                CssClass="btn btn-sm btn-outline-primary btn-editar me-2" 
                                CommandArgument='<%# Eval("CodDepartamento") %>'
                                OnClick="btnEditar_Click" />
                            <asp:Button ID="btnExcluir" runat="server" Text="Excluir" 
                                CssClass="btn btn-sm btn-outline-danger btn-excluir-departamento" 
                                CommandArgument='<%# Eval("CodDepartamento") %>'
                                OnClick="btnExcluir_Click"
                                OnClientClick="return confirm('Tem certeza que deseja excluir este departamento?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

    <!-- Modal Novo Departamento -->
    <div class="modal fade" id="modalNovoDepartamento" tabindex="-1" aria-labelledby="modalNovoDepartamentoLabel" aria-hidden="true">
        <div class="modal-dialog modal-lg">
            <div class="modal-content">
                <div class="modal-header">
                    <h5 class="modal-title" id="modalNovoDepartamentoLabel">
                        <i class="fas fa-plus-circle me-2"></i>Novo Departamento
                    </h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-md-12 mb-3">
                            <label for="txtNomeDepartamento" class="form-label">Nome do Departamento *</label>
                            <asp:TextBox ID="txtNomeDepartamento" runat="server" CssClass="form-control" 
                                MaxLength="100" placeholder="Ex: Recursos Humanos"></asp:TextBox>
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="txtRamal" class="form-label">Ramal</label>
                            <asp:TextBox ID="txtRamal" runat="server" CssClass="form-control" 
                                MaxLength="10" placeholder="Ex: 9120"></asp:TextBox>
                        </div>
                        <div class="col-md-6 mb-3">
                            <label for="txtTelefone" class="form-label">Telefone</label>
                            <asp:TextBox ID="txtTelefone" runat="server" CssClass="form-control" 
                                MaxLength="20" placeholder="Ex: 3133119120"></asp:TextBox>
                        </div>
                        <div class="col-md-12 mb-3">
                            <label for="txtEmail" class="form-label">E-mail Geral</label>
                            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" 
                                TextMode="Email" MaxLength="100" placeholder="Ex: departamento@empresa.com.br"></asp:TextBox>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>
                    <asp:Button ID="btnSalvarDepartamento" runat="server" 
                            Text="Salvar Departamento" CssClass="btn btn-primary"
                            OnClick="btnSalvarDepartamento_Click" 
                            OnClientClick="return validarFormulario();" />
                </div>
            </div>
        </div>
    </div>

    <!-- Scripts -->
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    
    <script>
        // Validação do formulário
        function validarFormulario() {
            var nome = document.getElementById('<%= txtNomeDepartamento.ClientID %>');
            var email = document.getElementById('<%= txtEmail.ClientID %>');

            if (nome) nome.classList.remove('is-invalid');
            if (email) email.classList.remove('is-invalid');

            if (!nome || !nome.value.trim()) {
                if (nome) nome.classList.add('is-invalid');
                Swal.fire({
                    icon: 'warning',
                    title: 'Atenção',
                    text: 'Nome do departamento é obrigatório',
                    confirmButtonText: 'OK'
                }).then(() => {
                    if (nome) nome.focus();
                });
                return false;
            }

            if (email && email.value.trim() !== '' && !isValidEmail(email.value)) {
                email.classList.add('is-invalid');
                Swal.fire({
                    icon: 'warning',
                    title: 'Atenção',
                    text: 'Por favor, insira um e-mail válido',
                    confirmButtonText: 'OK'
                }).then(() => {
                    email.focus();
                });
                return false;
            }

            return true;
        }

        function isValidEmail(email) {
            var re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return re.test(email);
        }
    </script>
</asp:Content>