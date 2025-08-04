<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="employeeDepartment.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.employeeDepartment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />

  <style>
body { font-family: 'Poppins', sans-serif; background-color: #f9f9f9; }
h3 {
    font-weight: 600;
    margin-bottom: 20px;
    color: #4CB07A;
    display: flex;
    align-items: center;
    gap: 8px;
}
.card { border-radius: 10px; box-shadow: none; border: none; background: transparent; }
.table-responsive { border-radius: 10px; overflow-x: auto; background: transparent; }
.table {
    font-size: 1rem;
    background: #fff;
    border-radius: 10px;
    border: 1px solid #ececec;
    margin-bottom: 0;
    box-shadow: none;
}
.table thead { background: transparent; }
.table thead th {
    background-color: #83ceee !important;
    color: #fff !important;
    font-weight: 600;
    border: none;
    font-size: 1rem;
    padding: 14px 10px;
}
.table thead th:first-child { border-top-left-radius: 10px; }
.table thead th:last-child { border-top-right-radius: 10px; }
.table tbody td {
    padding: 13px 10px;
    color: #222;
    font-size: 0.97rem;
    border: none;
    border-bottom: 1px solid #f1f1f1;
    background: none;
}

    .table > :not(caption) > * > * {
        color: #3b3f5c;
    }

.table tbody tr:last-child td { border-bottom: none; }
.table tbody tr { background: none; }
.table tbody tr:hover { background: none !important; }
.table tbody tr:last-child td:first-child { border-bottom-left-radius: 10px; }
.table tbody tr:last-child td:last-child { border-bottom-right-radius: 10px; }
.table-striped > tbody > tr:nth-of-type(odd) { --bs-table-accent-bg: none; }
.table, .table * { box-shadow: none !important; }
@media (max-width: 900px) {
    .table thead th, .table tbody td { font-size: 13px; padding: 10px 5px; }
    h3 { font-size: 1.1rem; }
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