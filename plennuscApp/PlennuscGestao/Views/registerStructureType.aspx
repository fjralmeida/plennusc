<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="registerStructureType.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.registerStructureType" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="../../Content/Css/projects/gestao/structuresCss/register-structure-type.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <!-- Header da Página -->
        <div class="page-header">
            <div class="page-title">
                <div class="title-icon">
                     <i class="bi bi-tags me-2"></i>
                </div>
                Cadastro de Tipo Estrutura
            </div>
        </div>

        <!-- Card Principal -->
        <div class="main-card">
            <div class="card-header">
                <h2 class="card-title">
                    <i class="bi bi-pencil-square"></i>
                    Dados do Tipo Estrutura
                </h2>
            </div>
            
            <div class="card-body">
                <!-- DESCRIÇÃO -->
                <div class="form-group">
                    <label class="form-label">Descrição *</label>
                    <asp:TextBox ID="txtDescricao" runat="server" CssClass="form-control" 
                        placeholder="Ex: PERFIL PESSOA, TIPO EMPRESA, etc." MaxLength="100"></asp:TextBox>
                </div>

               <!-- EDITÁVEL -->
                <div class="form-check">
                    <asp:CheckBox ID="chkEditavel" runat="server" CssClass="form-check-input-custom" />
                    <label class="form-check-label" for="<%= chkEditavel.ClientID %>">Editável</label>
                </div>
                <!-- INFO DA VIEW -->
                <div class="view-info">
                    <div class="view-label">View que será criada</div>
                    <div class="view-name" id="lblViewNome" runat="server">VW_</div>
                </div>

                <!-- BOTÃO SALVAR -->
                <div class="action-buttons">
                    <asp:Button ID="btnSalvar" runat="server" Text="Salvar e Criar View" 
                        CssClass="btn-save" OnClick="btnSalvar_Click" />
                </div>

                <!-- MENSAGENS -->
                <asp:Panel ID="pnlMensagem" runat="server" Visible="false">
                    <div class="message" id="divMensagem" runat="server">
                        <asp:Label ID="lblMensagem" runat="server"></asp:Label>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>

    <script>
        // Atualiza nome da view automaticamente
        function atualizarViewNome() {
            var descricao = document.getElementById('<%= txtDescricao.ClientID %>').value;
            var lblView = document.getElementById('<%= lblViewNome.ClientID %>');

            if (descricao) {
                lblView.innerText = 'VW_' + descricao.toUpperCase().replace(/ /g, '_');
            } else {
                lblView.innerText = 'VW_';
            }
        }

        document.getElementById('<%= txtDescricao.ClientID %>').addEventListener('input', atualizarViewNome);

        // Executa uma vez ao carregar a página
        document.addEventListener('DOMContentLoaded', function () {
            atualizarViewNome();
        });
    </script>
</asp:Content>