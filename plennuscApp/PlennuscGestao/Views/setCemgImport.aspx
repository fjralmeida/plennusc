<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" AutoEventWireup="true" CodeBehind="setCemgImport.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.setCemgImport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <h4>Importação de planilha de carga</h4>

    <div>
        <asp:FileUpload ID="fuPlanilha" runat="server" accept=".xlsm,.xlsx" />
        <asp:Button ID="btnImportar" runat="server" Text="Importar" CssClass="btn btn-primary" OnClick="btnImportar_Click" />
    </div>

    <br />
    <asp:Label ID="lblMensagem" runat="server" />

    <asp:Panel ID="pnlErros" runat="server" Visible="false">
        <h5>Inconsistências encontradas</h5>
        <asp:GridView ID="gvErros" runat="server" AutoGenerateColumns="false" CssClass="table table-sm table-bordered">
            <Columns>
                <asp:BoundField DataField="LinhaExcel" HeaderText="Linha" />
                <asp:BoundField DataField="Campo" HeaderText="Campo" />
                <asp:BoundField DataField="Mensagem" HeaderText="Mensagem" />
            </Columns>
        </asp:GridView>
    </asp:Panel>

   <asp:Panel ID="pnlLinhas" runat="server" Visible="false">
    <h5>Linhas lidas</h5>
    <asp:Button ID="btnExportar" runat="server" Text="Exportar layout" CssClass="btn btn-success" OnClick="btnExportar_Click" />
        <asp:GridView ID="gvLinhas" runat="server" AutoGenerateColumns="false" CssClass="table table-sm table-bordered">
            <Columns>
                <asp:BoundField DataField="LinhaExcel" HeaderText="Linha" />
                <asp:BoundField DataField="Seq" HeaderText="Seq" />
                <asp:BoundField DataField="TipoMovimentacao" HeaderText="Tp Mov" />
                <asp:BoundField DataField="DataVigencia" HeaderText="Vigência" />
                <asp:BoundField DataField="NomeCliente" HeaderText="Nome" />
                <asp:BoundField DataField="Parentesco" HeaderText="Parent" />
                <asp:BoundField DataField="DataNascimento" HeaderText="Nascimento" />
                <asp:BoundField DataField="Cpf" HeaderText="CPF" />
                <asp:BoundField DataField="Produto" HeaderText="Produto" />
            </Columns>
        </asp:GridView>
    </asp:Panel>

</asp:Content>
