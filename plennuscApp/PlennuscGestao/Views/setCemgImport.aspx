<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" AutoEventWireup="true" CodeBehind="setCemgImport.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.setCemgImport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/Css/projects/gestao/structuresCss/CIDs/SetCemgImport.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container-main">

        <!-- ================= CABEÇALHO ================= -->
        <div class="page-header">
            <h1 class="page-title">
                <span class="title-icon">
                    <i class="fa fa-file-excel-o" aria-hidden="true"></i>
                </span>
                Importação de planilha de carga
            </h1>
        </div>

        <!-- ================= CARD 1: PARÂMETROS ================= -->
        <div class="filters-card">
            <div class="filters-title">
                <i class="fa fa-sliders" aria-hidden="true"></i>
                <span>Parâmetros da importação</span>
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label class="form-label" for="<%= txtCodPlano.ClientID %>">Código do plano (COD_PLANO)</label>
                    <asp:TextBox ID="txtCodPlano" runat="server" CssClass="form-control" MaxLength="20" />
                </div>

                <div class="form-group">
                    <label class="form-label" for="<%= txtCodAnsPlano.ClientID %>">Código ANS do plano (COD_ANS_PLANO)</label>
                    <asp:TextBox ID="txtCodAnsPlano" runat="server" CssClass="form-control" MaxLength="20" />
                </div>
            </div>
        </div>

        <!-- ================= CARD 2: ARQUIVO ================= -->
        <div class="filters-card">
            <div class="filters-title">
                <i class="fa fa-upload" aria-hidden="true"></i>
                <span>Planilha de carga</span>
            </div>

            <div class="form-row">
                <div class="form-group" style="max-width: 520px;">
                    <label class="form-label" for="<%= fuPlanilha.ClientID %>">Arquivo (.xlsm, .xlsx)</label>
                    <asp:FileUpload ID="fuPlanilha" runat="server" CssClass="file-input" accept=".xlsm,.xlsx" />
                </div>

                <div class="form-group" style="flex: 0 0 auto; min-width: 0;">
                    <asp:Button ID="btnImportar" runat="server" Text="Importar"
                        CssClass="btn btn-primary" OnClick="btnImportar_Click" />
                </div>
            </div>

            <asp:Label ID="lblMensagem" runat="server" CssClass="msg-importacao" />
        </div>

        <!-- ================= INCONSISTÊNCIAS ================= -->
        <asp:Panel ID="pnlErros" runat="server" Visible="false">
            <h2 class="section-title">
                <i class="fa fa-exclamation-triangle" aria-hidden="true"></i>
                Inconsistências / avisos
            </h2>

            <div class="grid-container">
                <asp:GridView ID="gvErros" runat="server"
                    AutoGenerateColumns="false"
                    CssClass="custom-grid grid-erros"
                    GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="LinhaExcel" HeaderText="Linha" />
                        <asp:BoundField DataField="Campo"      HeaderText="Campo" />
                        <asp:BoundField DataField="Mensagem"   HeaderText="Mensagem" />
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>

        <!-- ================= LINHAS LIDAS ================= -->
        <asp:Panel ID="pnlLinhas" runat="server" Visible="false">
            <h2 class="section-title">
                <i class="fa fa-list-alt" aria-hidden="true"></i>
                Linhas lidas
            </h2>

            <div class="grid-container">

                <div class="grid-toolbar">
                    <asp:Button ID="btnExportar" runat="server" Text="Exportar layout"
                        CssClass="btn btn-success" OnClick="btnExportar_Click" />
                </div>

                <asp:GridView ID="gvLinhas" runat="server"
                    AutoGenerateColumns="false"
                    CssClass="custom-grid"
                    GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="LinhaExcel"      HeaderText="Linha" />
                        <asp:BoundField DataField="Seq"             HeaderText="Seq" />
                        <asp:BoundField DataField="TipoMovimentacao" HeaderText="Tp Mov" />
                        <asp:BoundField DataField="DataVigencia"    HeaderText="Vigência" />
                        <asp:BoundField DataField="NomeCliente"     HeaderText="Nome" />
                        <asp:BoundField DataField="Parentesco"      HeaderText="Parent" />
                        <asp:BoundField DataField="DataNascimento"  HeaderText="Nascimento" />
                        <asp:BoundField DataField="Cpf"             HeaderText="CPF" />
                        <asp:BoundField DataField="Produto"         HeaderText="Produto" />
                    </Columns>
                </asp:GridView>

            </div>
        </asp:Panel>

    </div>

</asp:Content>