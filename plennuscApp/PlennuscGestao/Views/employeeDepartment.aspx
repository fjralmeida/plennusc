<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="employeeDepartment.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.employeeDepartment" %>

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
            width: 50px;
            height: 2px;
            background-color: #c06ed4;
            display: block;
            margin: 0.5rem auto 0 auto;
            border-radius: 2px;
        }


        .titulo-pagina::after {
            content: "";
            width: 60px;
            height: 3px;
            background-color: #4CB07A;
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

        .table-dept {
            width: 100%;
            border-collapse: separate;
            border-spacing: 0;
            margin-top: 16px;
        }

        .table-dept thead {
            background-color: #83ceee;
            color: white;
        }

        .table-dept th, .table-dept td {
            padding: 12px 14px;
            text-align: left;
            vertical-align: middle;
            border-bottom: 1px solid #e0e0e0;
        }

        .table-dept tbody tr:nth-child(even) {
            background-color: #f9f9f9;
        }

        .table-dept tbody tr:hover {
            background-color: #d6f3e4;
        }

        .table-dept th:first-child,
        .table-dept td:first-child {
            border-top-left-radius: 10px;
        }

        .table-dept th:last-child,
        .table-dept td:last-child {
            border-top-right-radius: 10px;
        }

        @media (max-width: 768px) {
            .table-dept th, .table-dept td {
                font-size: 12px;
                padding: 10px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <h2 class="titulo-pagina">
            <i class="fa-solid fa-building me-2" style="color:#4CB07A;"></i>
            Departamentos
        </h2>
        <div class="card-container">
            <asp:GridView ID="gvDepartments" runat="server" CssClass="table-dept" AutoGenerateColumns="false" GridLines="None">
                <Columns>
                    <asp:BoundField DataField="CodDepartamento" HeaderText="Código" />
                    <asp:BoundField DataField="Nome" HeaderText="Nome" />
                    <asp:BoundField DataField="NumRamal" HeaderText="Ramal" />
                    <asp:BoundField DataField="EmailGeral" HeaderText="Email" />
                    <asp:BoundField DataField="Telefone" HeaderText="Telefone" />
                    <asp:BoundField DataField="Informacoes_Log_I" HeaderText="Informações Log" />
                </Columns>
            </asp:GridView>
        </div>
    </div>
</asp:Content>