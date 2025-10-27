<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="employeeDepartment.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.employeeDepartment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

        <title>Departamento de Funcionários</title>

    <!-- Fonts / Ícones (mantidos) -->
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />

    <link href="../../Content/Css/projects/gestao/structuresCss/employee-Department.css" rel="stylesheet" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <h2 class="titulo-pagina">
            <i class="bi bi-diagram-3 me-2"></i>
            Departamentos
        </h2>

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
                    <asp:BoundField DataField="Informacoes_Log_I" HeaderText="Informações Log" 
                        ItemStyle-CssClass="col-informacoes" HeaderStyle-CssClass="col-informacoes" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>