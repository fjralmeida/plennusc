<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscMedic/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="medicalInterview.aspx.cs" Inherits="appWhatsapp.PlennuscMedic.Views.medicalInterview" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.2/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.10.5/font/bootstrap-icons.css" rel="stylesheet" />
    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet" />

    <style>
        body { font-family: 'Poppins', sans-serif; background-color: #f9f9f9; }
        .filters { background: white; padding: 15px; border-radius: 10px; margin-bottom: 20px; box-shadow: 0 1px 4px rgba(0, 0, 0, 0.05);}
        .table { font-size: 0.9rem; background: white; border-radius: 10px; overflow: hidden; border: none; }
        .table thead { background-color: #83ceee; color: #fff; font-weight: 500; font-size: 0.85rem; }
        .table thead th { border-bottom: 1px solid #83ceee; }
        .table tbody tr { transition: background 0.2s; }
        .table tbody tr:hover { background: #f1f9ff; }
        .table td, .table th { vertical-align: middle; border-top: none; }
        .status-badge { font-size: 0.75rem; padding: 4px 8px; border-radius: 6px; display: inline-block; border: 1px solid transparent; }
        .status-aguardando { background-color: #fbe9eb; color: #DC8689; border-color: #f5c4c6; }
        .status-em-espera { background-color: #f6e5fb; color: #c06ed4; border-color: #e0b4ed; }
        .status-aprovado { background-color: #e8f7f0; color: #4CB07A; border-color: #bde4cf; }
        .btn-primary { background-color: #4CB07A; border-color: #4CB07A; }
        .btn-primary:hover { background-color: #3d9465; border-color: #3d9465; }
        .btn-outline-primary { font-size: 0.8rem; padding: 4px 10px; border-radius: 6px; color: #4CB07A; border-color: #4CB07A; }
        .btn-outline-primary:hover { background-color: #4CB07A; color: white; }
        h3 { font-weight: 600; margin-bottom: 20px; color: #4CB07A; }
        .table-responsive { border-radius: 10px; overflow-x: auto; }
        .form-select:focus, .form-control:focus { border-color: #4CB07A; box-shadow: 0 0 0 0.2rem rgba(76, 176, 122, 0.25); }
        tbody, td, tfoot, th, thead, tr { border-color: inherit; border-style: inset; border-width: 0; }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div class="container-fluid p-3">
    <h3>Gestão de Associados</h3>

    <!-- Filtros -->
    <div class="filters row g-3">
        <div class="col-md-3 col-6">
            <label class="form-label">Status</label>
            <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
                <asp:ListItem Value="">Todos</asp:ListItem>
                <asp:ListItem Value="AGUARDANDO_AVALIACAO">Aguardando Avaliação</asp:ListItem>
                <asp:ListItem Value="EM_ESPERA">Em Espera</asp:ListItem>
                <asp:ListItem Value="APROVADO">Aprovado</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="col-md-2 col-6">
            <label class="form-label">Empresa</label>
            <asp:DropDownList ID="ddlEmpresa" runat="server" CssClass="form-select">
                <asp:ListItem Value="">Todas</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="col-md-2 col-6">
            <label class="form-label">Plano</label>
            <asp:DropDownList ID="ddlPlano" runat="server" CssClass="form-select">
                <asp:ListItem Value="">Todos</asp:ListItem>
            </asp:DropDownList>
        </div>
        <div class="col-md-3 col-6">
            <label class="form-label">Nome ou CPF</label>
            <asp:TextBox ID="txtBusca" runat="server" CssClass="form-control" placeholder="Buscar..." />
        </div>
        <div class="col-md-2 col-12 d-flex align-items-end">
            <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar" CssClass="btn btn-primary w-100" OnClick="btnFiltrar_Click" />
        </div>
    </div>

    <!-- Grid de Dados -->
    <div class="card mt-3">
        <div class="card-body p-0 table-responsive">
            <asp:GridView ID="gvAssociados" runat="server" CssClass="table table-hover mb-0" AutoGenerateColumns="False"
                          OnRowCommand="gvAssociados_RowCommand">
                <Columns>
                    <asp:BoundField DataField="CODIGO_ASSOCIADO" HeaderText="Cód. Associado" />
                    <asp:BoundField DataField="NOME" HeaderText="Nome" />
                    <asp:BoundField DataField="CPF" HeaderText="CPF" />
                    <asp:BoundField DataField="PLANO" HeaderText="Plano" />
                    <asp:BoundField DataField="EMPRESA" HeaderText="Empresa" />
                    <asp:BoundField DataField="DATA_NASC" HeaderText="Data Nasc." DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='status-badge <%# GetStatusClass(Eval("STATUS").ToString()) %>'>
                                <%# Eval("STATUS") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>   
                </Columns>
            </asp:GridView>
        </div>
    </div>
</div>
</asp:Content>