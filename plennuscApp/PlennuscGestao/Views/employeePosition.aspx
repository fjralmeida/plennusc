<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="employeePosition.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.employeePosition" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
        <title>Cargos</title>

    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <link href="../../Content/Css/projects/gestao/structuresCss/employee-Position.css" rel="stylesheet" />

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <h2 class="titulo-pagina">
            <i class="fa-solid fa-briefcase"></i>
            Cargos
        </h2>

        <div class="grid-container">
            <asp:GridView ID="gvPositions"
                runat="server"
                AutoGenerateColumns="false"
                GridLines="None"
                CssClass="custom-grid align-middle">

                <Columns>
                    <asp:BoundField DataField="CodCargo" HeaderText="Código" 
                        ItemStyle-CssClass="col-codigo" HeaderStyle-CssClass="col-codigo" />
                    <asp:BoundField DataField="Nome" HeaderText="Nome" 
                        ItemStyle-CssClass="col-nome" HeaderStyle-CssClass="col-nome" />
                    <asp:BoundField DataField="Descricacao" HeaderText="Descrição" 
                        ItemStyle-CssClass="col-descricao" HeaderStyle-CssClass="col-descricao" />
                    <asp:BoundField DataField="CodCBO" HeaderText="Cod CBO" 
                        ItemStyle-CssClass="col-cbo" HeaderStyle-CssClass="col-cbo" />
                    <asp:BoundField DataField="Conf_TipoGestor" HeaderText="Tipo Cargo" 
                        ItemStyle-CssClass="col-tipo" HeaderStyle-CssClass="col-tipo" />
                    <asp:BoundField DataField="Informacoes_Log_I" HeaderText="Informações Log" 
                        ItemStyle-CssClass="col-informacoes" HeaderStyle-CssClass="col-informacoes" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>
