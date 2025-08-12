<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="employeePosition.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.employeePosition" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />

    <title>Cargos</title>

    <style>
        body { font-family: 'Poppins', sans-serif; background-color: #f9f9f9; color:#333; }
        .container { max-width: 1100px; margin: 32px auto; padding: 0 16px; }

        /* Título clean, leve e moderno */
        .titulo-pagina{
            font-weight: 500;
            font-size: 1.5rem;
            margin: 24px 0 20px;
            color:#2c3e50;
            display:flex; align-items:center; gap:10px;
        }
        .titulo-pagina i{
            font-size:1.6rem;
            background:#eef2f6; /* fundo sutil */
            color:#2c3e50;
            padding:8px; border-radius:8px;
        }

        /* Card/Wrapper do grid */
        .card-container{ margin-top: 8px; }
        .table-responsive{
            border:1px solid #ddd;
            background:#fff;
            border-radius:8px;
            overflow:hidden;
        }

        /* Grid no padrão minimalista */
        .table{ font-size:.95rem; margin-bottom:0; }
        .table thead th{
            background:#f5f5f5 !important;
            color:#333 !important;
            font-weight:600;
            border-bottom:1px solid #ddd;
            padding:12px 10px;
        }
        .table tbody td{
            padding:12px 10px;
            color:#444;
            border-bottom:1px solid #eee;
            background:#fff;
        }
        .table-hover>tbody>tr:hover>*{ background:#fafafa; }
        .table> :not(caption)>*>*{ color:#333; }

        @media (max-width: 900px){
            .table thead th, .table tbody td{ font-size:13px; padding:10px 8px; }
            .titulo-pagina{ font-size:1.1rem; }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <h2 class="titulo-pagina">
            <i class="fa-solid fa-briefcase" style="color:#c06ed4;"></i>
            Cargos
        </h2>

        <div class="card-container">
            <div class="table-responsive">
                <asp:GridView ID="gvPositions"
                    runat="server"
                    AutoGenerateColumns="false"
                    GridLines="None"
                    CssClass="table table-hover align-middle">

                    <Columns>
                        <asp:BoundField DataField="CodCargo"          HeaderText="Código" />
                        <asp:BoundField DataField="Nome"              HeaderText="Nome" />
                        <asp:BoundField DataField="Descricacao"       HeaderText="Descrição" />
                        <asp:BoundField DataField="CodCBO"            HeaderText="Cod CBO" />
                        <asp:BoundField DataField="Conf_TipoGestor"   HeaderText="Tipo Cargo" />
                        <asp:BoundField DataField="Informacoes_Log_I" HeaderText="Informações Log" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
 </asp:Content>