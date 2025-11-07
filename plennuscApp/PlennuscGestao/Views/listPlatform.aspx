<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="listPlatform.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.listPlatform" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <div class="container-main">
     <!-- Header da Página -->
     <div class="page-header">
         <div class="page-title">
             <div class="title-icon">
                 <i class="bi bi-list-ul me-2"></i>
             </div>
             Lista de Sistemas
         </div>
     </div>

     <!-- Card Principal -->
     <div class="main-card">
         <div class="card-body">
             <!-- GridView -->
             <div class="table-container">
                 <asp:GridView ID="gvSistemas" runat="server" AutoGenerateColumns="False" CssClass="structure-grid"
                     AllowPaging="True" PageSize="15"
                     EmptyDataText="Nenhum tipo de estrutura encontrado." DataKeyNames="CodSistema">
                     <Columns>

                         <asp:BoundField DataField="CodSistema" HeaderText="Código" SortExpression="CodSistema">
                             <HeaderStyle Width="80px" />
                             <ItemStyle HorizontalAlign="Center" />
                         </asp:BoundField>

                         <asp:BoundField DataField="NomeDisplay" HeaderText="Nome Sistema" SortExpression="NomeDisplay">
                             <HeaderStyle Width="200px" />
                         </asp:BoundField>

                         <asp:BoundField DataField="Descricao" HeaderText="Descrição" SortExpression="Descricao">
                             <HeaderStyle Width="200px" />
                         </asp:BoundField>

                         <asp:BoundField DataField="Conf_Logo" HeaderText="Logo do Sistema" SortExpression="Conf_Logo">
                             <HeaderStyle Width="250px" />
                         </asp:BoundField>

                     </Columns>
                     <PagerStyle CssClass="grid-pager" />
                     <HeaderStyle CssClass="grid-header" />
                     <RowStyle CssClass="grid-row" />
                     <AlternatingRowStyle CssClass="grid-alternating-row" />
                 </asp:GridView>
             </div>
         </div>
     </div>
 </div>
</asp:Content>
