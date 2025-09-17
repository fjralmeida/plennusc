<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="myDemands.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.myDemands" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <style>
        .custom-grid {
            width: 100%;
            border-collapse: collapse;
            margin-top: 20px;
        }
        
        .custom-grid th {
            background-color: #f8f9fa;
            padding: 12px;
            text-align: left;
            font-weight: 600;
            border-bottom: 2px solid #dee2e6;
        }
        
        .custom-grid td {
            padding: 12px;
            border-bottom: 1px solid #dee2e6;
        }
        
        .prioridade-badge {
            padding: 4px 8px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 500;
        }
        
        .prioridade-33 { background-color: #ffcccc; color: #cc0000; } /* Crítica */
        .prioridade-32 { background-color: #ffe6cc; color: #cc5500; } /* Alta */
        .prioridade-31 { background-color: #ffffcc; color: #666600; } /* Média */
        .prioridade-30 { background-color: #e6ffcc; color: #006600; } /* Baixa */
        .prioridade-29 { background-color: #ccf2ff; color: #0066cc; } /* Muito Baixa */
        
        .status-badge {
            padding: 4px 8px;
            border-radius: 4px;
            font-size: 12px;
            font-weight: 500;
        }
        
        .status-aberta { background-color: #e6f4ea; color: #137333; }
        .status-fechada { background-color: #fce8e6; color: #c5221f; }
        .status-andamento { background-color: #e8f0fe; color: #1a73e8; }
        
        .btn-action {
            padding: 6px 12px;
            background-color: #1a73e8;
            color: white;
            border-radius: 4px;
            text-decoration: none;
            display: inline-block;
        }
        
        .btn-action:hover {
            background-color: #0d62c9;
            color: white;
        }
        
        .pagination-container {
            margin-top: 20px;
            text-align: center;
        }
        
        .grid-header {
            background-color: #f8f9fa;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <div class="header-card">
            <h1 class="demand-title">
                <span class="title-icon">
                    <i class="bi bi-list-check"></i>
                </span>
                Minhas Demandas Aceitas
            </h1>
            <p class="text-muted">Aqui você pode visualizar e gerenciar as demandas que aceitou</p>
        </div>
        
        <div class="form-container">
            <asp:GridView ID="gvMinhasDemandas" runat="server" CssClass="custom-grid"
                AutoGenerateColumns="False" AllowPaging="True" PageSize="10"
                OnPageIndexChanging="gvMinhasDemandas_PageIndexChanging"
                OnRowCommand="gvMinhasDemandas_RowCommand"
                EmptyDataText="Você não aceitou nenhuma demanda ainda.">

                <Columns>
                    <asp:BoundField DataField="CodDemanda" HeaderText="ID" />
                    <asp:BoundField DataField="Titulo" HeaderText="Título" />
                    <asp:BoundField DataField="Categoria" HeaderText="Categoria" />
                    <asp:BoundField DataField="Subtipo" HeaderText="Subtipo" />
                    
                    <asp:TemplateField HeaderText="Prioridade">
                        <ItemTemplate>
                            <span class='prioridade-badge prioridade-<%# Eval("CodPrioridade") %>'>
                                <%# Eval("Prioridade") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class="status-badge status-<%# Eval("Status").ToString().ToLower() %>">
                                <%# Eval("Status") %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:BoundField DataField="Solicitante" HeaderText="Solicitante" />
                    <asp:BoundField DataField="DataSolicitacao" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}" />
                    
                    <asp:TemplateField HeaderText="Data Aceite">
                        <ItemTemplate>
                            <%# Eval("DataAceite", "{0:dd/MM/yyyy HH:mm}") %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    
                    <asp:TemplateField HeaderText="Ações">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkVer" runat="server" CssClass="btn-action"
                                        CommandName="Ver" CommandArgument='<%# Eval("CodDemanda") %>'>
                                <i class="bi bi-eye"></i> Ver/Responder
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>

                <PagerStyle CssClass="pagination-container" />
                <HeaderStyle CssClass="grid-header" />
            </asp:GridView>
        </div>
    </div>

</asp:Content>
