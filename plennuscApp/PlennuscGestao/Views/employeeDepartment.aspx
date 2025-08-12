<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="employeeDepartment.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.employeeDepartment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Fonts / Ícones (mantidos) -->
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />

    <title>Departamento de Funcionários</title>

    <style>
        body { font-family: 'Poppins', sans-serif; background-color: #f9f9f9; }

       .titulo-pagina {
            font-weight: 500; /* mais leve que 600 */
            font-size: 1.5rem; /* tamanho ajustado */
            margin: 24px 0 20px;
            color: #2c3e50; /* tom mais neutro e moderno */
            display: flex;
            align-items: center;
            gap: 10px;
        }

        .titulo-pagina i {
            font-size: 1.6rem;
            background-color: #e8f5ee; /* fundo suave do ícone */
            color: #4CB07A;
            padding: 8px;
            border-radius: 8px;
        }
        .card-container { margin-top: 8px; }

        .table-responsive {
            border-radius: 8px;
            overflow: hidden;
            border: 1px solid #ddd;
            background: #fff;
        }

        .table {
            font-size: 0.95rem;
            margin-bottom: 0;
        }

        /* Cabeçalho mais clean */
        .table thead th {
            background-color: #f5f5f5 !important;
            color: #333 !important;
            font-weight: 600;
            border-bottom: 1px solid #ddd;
            padding: 12px 10px;
        }

        /* Linhas */
        .table tbody td {
            padding: 12px 10px;
            color: #444;
            border-bottom: 1px solid #eee;
        }

        /* Hover sutil */
        .table-hover > tbody > tr:hover > * {
            background: #fafafa;
        }

        /* Texto */
        .table > :not(caption) > * > * { color: #333; }

        @media (max-width: 900px) {
            .table thead th, .table tbody td {
                font-size: 13px; padding: 10px 8px;
            }
            .titulo-pagina { font-size: 1.1rem; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
       <h2 class="titulo-pagina">
            <i class="fa-solid fa-building"></i>
            Departamentos
        </h2>

        <div class="card-container">
            <div class="table-responsive">
                <asp:GridView ID="gvDepartments"
                    runat="server"
                    AutoGenerateColumns="false"
                    GridLines="None"
                    CssClass="table table-hover align-middle">

                    <Columns>
                        <asp:BoundField DataField="CodDepartamento" HeaderText="Código" />
                        <asp:BoundField DataField="Nome"             HeaderText="Nome" />
                        <asp:BoundField DataField="NumRamal"         HeaderText="Ramal" />
                        <asp:BoundField DataField="EmailGeral"       HeaderText="E-mail" />
                        <asp:BoundField DataField="Telefone"         HeaderText="Telefone" />
                        <asp:BoundField DataField="Informacoes_Log_I" HeaderText="Informações Log" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>