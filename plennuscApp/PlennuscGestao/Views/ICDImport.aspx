<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" AutoEventWireup="true" CodeBehind="ICDImport.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.ICDImport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="../../Content/Css/projects/gestao/structuresCss/CIDs/ICDImport.css" rel="stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="icd-import-container">
        <h3>Importação de CID</h3>

        <div class="form-row">
            <label>Vigência <span class="icd-required">*</span></label>
            <asp:TextBox ID="txtVigencia" runat="server" TextMode="Date" CssClass="form-control" />
            <asp:RequiredFieldValidator ID="rfvVigencia" runat="server"
                ControlToValidate="txtVigencia"
                ErrorMessage="Campo Vigência é obrigatório."
                CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="form-row">
            <label>Arquivo Excel <span class="icd-required">*</span></label>
            <asp:FileUpload ID="fileUploadExcel" runat="server" />
            <asp:RegularExpressionValidator ID="revArquivo" runat="server"
                ControlToValidate="fileUploadExcel"
                ValidationExpression="^.+\.(xlsx|xls)$"
                ErrorMessage="O arquivo deve ser .xlsx ou .xls."
                CssClass="text-danger" Display="Dynamic" />
            <asp:RequiredFieldValidator ID="rfvArquivo" runat="server"
                ControlToValidate="fileUploadExcel"
                ErrorMessage="Selecione um arquivo Excel."
                CssClass="text-danger" Display="Dynamic" />
        </div>

        <div class="form-row">
            <asp:Button ID="btnImportar" runat="server" Text="Importar" CssClass="btn btn-primary" OnClick="btnImportar_Click" />
        </div>

        <asp:Panel ID="pnlResultado" runat="server" Visible="false">

            <h4>Registros Importados com Sucesso</h4>
            <asp:GridView ID="gridImportados" runat="server" AutoGenerateColumns="true"
                CssClass="table table-striped" EmptyDataText="Nenhum registro importado." />

            <h4>Registros Não Importados</h4>
            <asp:GridView ID="gridNaoImportados" runat="server" AutoGenerateColumns="true"
                CssClass="table table-striped" EmptyDataText="Todos os registros foram importados." />

        </asp:Panel>
    </div>
</asp:Content>