<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscMedic/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="employeeDepartmentMedic.aspx.cs" Inherits="appWhatsapp.PlennuscMedic.Views.employeeDepartmentMedic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

  <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />
  <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />


<style>
    :root {
        --primary: #83ceee;
        --primary-hover: #0d62c9;
        --success: #4cb07a;
        --success-hover: #3b8b65;
        --warning: #ffa726;
        --warning-hover: #f57c00;
        --danger: #f44336;
        --danger-hover: #d32f2f;
        --gray-50: #f8f9fa;
        --gray-100: #f1f3f4;
        --gray-200: #e8eaed;
        --gray-300: #dadce0;
        --gray-400: #bdc1c6;
        --gray-500: #9aa0a6;
        --gray-600: #80868b;
        --gray-700: #5f6368;
        --gray-800: #3c4043;
        --gray-900: #202124;
        --border-radius: 8px;
        --shadow: 0 1px 2px 0 rgba(60, 64, 67, 0.3), 0 1px 3px 1px rgba(60, 64, 67, 0.15);
        --transition: all 0.2s ease-in-out;
    }

    body {
        background: var(--gray-100);
        font-family: 'Roboto', sans-serif;
        color: var(--gray-800);
        line-height: 1.5;
    }

    .container {
        max-width: 2206px;
        margin: 20px auto;
        padding: 0 16px;
    }

    /* Header igual ao exemplo */
    .titulo-pagina {
        display: flex;
        align-items: center;
        gap: 12px;
        font-size: 24px;
        font-weight: 500;
        color: var(--gray-800);
        margin: 0 0 24px 0;
    }

    .titulo-pagina i {
        background: var(--success);
        color: white;
        width: 44px;
        height: 44px;
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
        font-size: 1.2rem;
    }

    /* Grid container igual ao exemplo */
    .grid-container {
        background: white;
        border-radius: var(--border-radius);
        box-shadow: var(--shadow);
        overflow: hidden;
        overflow-x: auto;
    }

    /* Tabela no padrão do exemplo */
    .custom-grid {
        width: 100%;
        border-collapse: collapse;
        min-width: 1200px;
        table-layout: fixed;
    }

        .custom-grid th,
        .custom-grid td {
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
            vertical-align: middle;
        }

        .custom-grid th {
            background: var(--gray-50);
            padding: 16px 12px;
            text-align: left;
            font-weight: 600;
            color: var(--gray-700);
            border-bottom: 2px solid var(--gray-300);
            font-size: 13px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            white-space: nowrap;
        }

        .custom-grid td {
            padding: 14px 12px;
            border-bottom: 1px solid var(--gray-200);
            vertical-align: middle;
            font-size: 14px;
            line-height: 1.4;
        }

        .custom-grid tr:last-child td {
            border-bottom: none;
        }

        .custom-grid tr:hover {
            background: var(--gray-50);
            transition: var(--transition);
        }

    /* Striped rows igual ao exemplo */
    .custom-grid tbody tr:nth-child(even) {
        background-color: var(--gray-50);
    }

    .custom-grid tbody tr:nth-child(even):hover {
        background-color: var(--gray-100);
    }

    /* Larguras específicas para as colunas */
    .col-codigo {
        width: 100px;
        min-width: 80px;
        max-width: 100px;
    }

    .col-nome {
        width: 250px;
        min-width: 200px;
        max-width: 250px;
    }

    .col-ramal {
        width: 120px;
        min-width: 100px;
        max-width: 120px;
    }

    .col-email {
        width: 200px;
        min-width: 150px;
        max-width: 200px;
    }

    .col-telefone {
        width: 140px;
        min-width: 120px;
        max-width: 140px;
    }

    .col-informacoes {
        width: 180px;
        min-width: 150px;
        max-width: 180px;
    }

    /* Responsividade */
    @media (max-width: 1024px) {
        .container {
            padding: 0 12px;
        }
    }

    @media (max-width: 768px) {
        .titulo-pagina {
            font-size: 20px;
        }
        
        .titulo-pagina i {
            width: 40px;
            height: 40px;
            font-size: 1.1rem;
        }
        
        .custom-grid th,
        .custom-grid td {
            font-size: 0.85rem;
            padding: 12px 8px;
        }
    }

    @media (max-width: 480px) {
        .titulo-pagina {
            font-size: 18px;
            gap: 8px;
        }
        
        .custom-grid th,
        .custom-grid td {
            font-size: 0.8rem;
            padding: 10px 6px;
        }
    }
</style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container">
        <h2 class="titulo-pagina">
            <i class="fa-solid fa-building"></i>
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