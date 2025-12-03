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
                CssClass="custom-grid align-middle"
                OnRowDataBound="gvDepartments_RowDataBound">

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
                            <button type="button" class="btn btn-sm btn-outline-primary btn-editar me-2" 
                                data-id='<%# Eval("CodDepartamento") %>'
                                onclick='editarDepartamento(<%# Eval("CodDepartamento") %>)'>
                                <i class="fas fa-edit"></i>
                            </button>
                            <asp:Button ID="btnExcluir" runat="server" Text="Excluir" 
                                CssClass="btn btn-sm btn-outline-danger" 
                                CommandArgument='<%# Eval("CodDepartamento") %>'
                                OnClick="btnExcluir_Click"
                                OnClientClick="return confirmarExclusao();" />
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
                                MaxLength="100" placeholder="Ex: Recursos Humanos" required="required"></asp:TextBox>
                            <div class="invalid-feedback" id="nomeError">Por favor, insira o nome do departamento.</div>
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
                            <div class="invalid-feedback" id="emailError">Por favor, insira um e-mail válido.</div>
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
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <!-- SweetAlert2 JS -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    
    <script>
        // Validação do formulário
        function validarFormulario() {
            var nome = document.getElementById('<%= txtNomeDepartamento.ClientID %>').value;
            var email = document.getElementById('<%= txtEmail.ClientID %>').value;
            var isValid = true;

            // Limpar erros anteriores
            document.getElementById('nomeError').style.display = 'none';
            document.getElementById('emailError').style.display = 'none';
            document.getElementById('<%= txtNomeDepartamento.ClientID %>').classList.remove('is-invalid');
            document.getElementById('<%= txtEmail.ClientID %>').classList.remove('is-invalid');

            // Validar nome
            if (nome.trim() === '') {
                document.getElementById('nomeError').style.display = 'block';
                document.getElementById('<%= txtNomeDepartamento.ClientID %>').classList.add('is-invalid');
                isValid = false;
            }

            // Validar email se preenchido
            if (email.trim() !== '' && !isValidEmail(email)) {
                document.getElementById('emailError').style.display = 'block';
                document.getElementById('<%= txtEmail.ClientID %>').classList.add('is-invalid');
                isValid = false;
            }

            if (!isValid) {
                // Mostrar mensagem de erro usando SweetAlert
                Swal.fire({
                    icon: 'warning',
                    title: 'Validação',
                    text: 'Por favor, corrija os campos destacados.',
                    confirmButtonText: 'OK'
                });
            }

            return isValid;
        }

        function isValidEmail(email) {
            var re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            return re.test(email);
        }

        // Confirmação de exclusão
        function confirmarExclusao() {
            return Swal.fire({
                title: 'Tem certeza?',
                text: "Esta ação não poderá ser desfeita!",
                icon: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#d33',
                cancelButtonColor: '#3085d6',
                confirmButtonText: 'Sim, excluir!',
                cancelButtonText: 'Cancelar'
            }).then((result) => {
                return result.isConfirmed;
            });
        }

        // Função para editar departamento (exemplo)
        function editarDepartamento(id) {
            // Aqui você pode implementar a lógica de edição
            // Pode abrir um modal de edição ou redirecionar
            Swal.fire({
                title: 'Editar Departamento',
                text: 'Funcionalidade de edição será implementada em breve.',
                icon: 'info',
                confirmButtonText: 'OK'
            });
        }
    </script>
</asp:Content>