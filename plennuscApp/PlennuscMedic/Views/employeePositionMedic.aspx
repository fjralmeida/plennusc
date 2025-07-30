<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscMedic/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="employeePositionMedic.aspx.cs" Inherits="appWhatsapp.PlennuscMedic.Views.employeePositionMedic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

        <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />

    <style>
        * {
            font-family: 'Poppins', sans-serif;
            box-sizing: border-box;
        }

        body {
            background-color: #f2f4f8;
            font-size: 13px;
            color: #333;
        }

        .container {
            max-width: 1100px;
            margin: 32px auto;
            padding: 0 16px;
        }

     .titulo-pagina {
    font-size: 22px;
    font-weight: 600;
    color: #333;
    text-align: center;
    margin-bottom: 20px;
    position: relative;
}

        .titulo-pagina::after {
            content: "";
            width: 60px;
            height: 3px;
            background-color: #c06ed4;
            display: block;
            margin: 0.5rem auto 0 auto;
            border-radius: 2px;
        }

        .card-container {
            background: white;
            padding: 30px;
            border-radius: 18px;
            box-shadow: 0 4px 14px rgba(0, 0, 0, 0.05);
        }

        .table-positions {
            width: 100%;
            border-collapse: separate;
            border-spacing: 0;
            margin-top: 16px;
        }

        .table-positions thead {
            background-color: #c06ed4;
            color: white;
        }

        .table-positions th, .table-positions td {
            padding: 12px 14px;
            text-align: left;
            vertical-align: middle;
            border-bottom: 1px solid #e0e0e0;
        }

        .table-positions tbody tr:nth-child(even) {
            background-color: #f9f9f9;
        }

        .table-positions tbody tr:hover {
            background-color: #f4e9f7;
        }

        .table-positions th:first-child,
        .table-positions td:first-child {
            border-top-left-radius: 10px;
        }

        .table-positions th:last-child,
        .table-positions td:last-child {
            border-top-right-radius: 10px;
        }

        @media (max-width: 768px) {
            .table-positions th, .table-positions td {
                font-size: 12px;
                padding: 10px;
            }
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

     <div class="container">
     <h2 class="titulo-pagina">
         <i class="fa-solid fa-briefcase me-2" style="color:#c06ed4;"></i>
         Cargos
     </h2>
     <div class="card-container">
         <asp:GridView ID="gvPositions" runat="server" CssClass="table-positions" AutoGenerateColumns="false" GridLines="None">
             <Columns>
                 <asp:BoundField DataField="CodCargo" HeaderText="Código" />
                 <asp:BoundField DataField="Nome" HeaderText="Nome" />
                 <asp:BoundField DataField="Descricacao" HeaderText="Descrição" />
                 <asp:BoundField DataField="CodCBO" HeaderText="Cod CBO" />
                 <asp:BoundField DataField="Conf_TipoGestor" HeaderText="Tipo Cargo" />
                 <asp:BoundField DataField="Informacoes_Log_I" HeaderText="Informações Log" />
             </Columns>
         </asp:GridView>
     </div>
 </div>

</asp:Content>
