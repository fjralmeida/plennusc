<%@ Page 
    Title="" 
    Language="C#" 
    MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" 
    AutoEventWireup="true" 
    CodeBehind="homeManagement.aspx.cs" 
    Inherits="PlennuscGestao.Views.HomeGestao" 
%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Conteúdo do head, se precisar -->
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Label ID="LabelErro" runat="server" ForeColor="Red" CssClass="error-message" Visible="false" />
    <!-- Conteúdo principal da página -->
</asp:Content>