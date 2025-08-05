<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscMedic/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="employeePositionMedic.aspx.cs" Inherits="appWhatsapp.PlennuscMedic.Views.employeePositionMedic" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

         <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />

<style>
body { font-family: 'Poppins', sans-serif; background-color: #f9f9f9; }
h3 {
    font-weight: 600;
    margin-bottom: 20px;
    color: #c06ed4;
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
    background-color: #c06ed4 !important;
    color: #fff !important;
    font-weight: 600;
    border: none;
    font-size: 1rem;
    padding: 14px 10px;
}
    .table > :not(caption) > * > * {
        color: #3b3f5c;
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

    
   <div class="container-fluid p-3">
    <h3>
        <i class="bi bi-briefcase me-2" style="color:#c06ed4;"></i>
        Cargos
    </h3>

    <div class="card mt-3">
        <div class="card-body p-0 table-responsive">
            <asp:GridView ID="gvPositions" runat="server" CssClass="table table-hover mb-0" AutoGenerateColumns="false" GridLines="None">
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
</div>
</asp:Content>
