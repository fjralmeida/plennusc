<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="registerStructureType.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.registerStructureType" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
    <link href="../../Content/Css/projects/gestao/structuresCss/register-structure-type.css" rel="stylesheet" />

    <script type="text/javascript">
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

        document.addEventListener('DOMContentLoaded', function () {
            document.getElementById('<%= txtDescricao.ClientID %>').addEventListener('input', atualizarViewNome);
            atualizarViewNome();
        });
    </script>
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
                <!-- TIPO ESTRUTURA PAI -->
                <div class="form-group">
                    <label class="form-label">Tipo Estrutura Pai</label>
                    <asp:DropDownList ID="ddlTipoEstruturaPai" runat="server" CssClass="form-control">
                        <asp:ListItem Text="-- Não tem --" Value=""></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <!-- TIPO ESTRUTURA -->
                <div class="form-group">
                    <label class="form-label">Tipo Estrutura *</label>
                    <asp:TextBox ID="txtTipoEstrutura" runat="server" CssClass="form-control"
                        placeholder="Ex: PERFIL PESSOA, TIPO EMPRESA, etc." MaxLength="100"></asp:TextBox>
                </div>

                <!-- UTILIZAÇÃO -->
                <div class="form-group">
                    <label class="form-label">Utilização</label>
                    <asp:TextBox ID="txtUtilizacao" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"
                        placeholder="Descreva a utilização deste tipo de estrutura..." MaxLength="500"></asp:TextBox>
                </div>


                <!-- DESCRIÇÃO -->
                <div class="form-group">
                    <label class="form-label">Descrição</label>
                    <asp:TextBox ID="txtDescricao" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="3"
                        placeholder="Descreva este tipo de estrutura..." MaxLength="500"></asp:TextBox>
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
</asp:Content>
