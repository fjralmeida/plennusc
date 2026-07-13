<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" AutoEventWireup="true" CodeBehind="sendAnAutomatedEmail.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.sendAnAutomatedEmail" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <p>Selecione o arquivo .csv (separado por ; ou ,) com colunas: NOME, ELEGIVEL_SORTEIO, NUMERO_SORTEIO, EMAIL</p>
    <asp:FileUpload ID="fuPlanilha" runat="server" />
    <asp:Button ID="btnEnviar" runat="server" Text="Enviar E-mails" OnClick="btnEnviar_Click" />
    <asp:Label ID="lblStatus" runat="server" />
    <asp:GridView ID="gvResultado" runat="server" AutoGenerateColumns="true" />
</asp:Content>