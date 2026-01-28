<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="viewShippingHistory.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.viewShippingHistory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Consulta de Histórico de Envios</title>
    <link href="../../Content/Css/projects/gestao/structuresCss/sendMessage/ViewShippingHistory.css" rel="stylesheet" />

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid p-4">
        <div class="card-container">
            <div class="card-header">
                <h5 class="mb-0">
                    <i class="fa-solid fa-history me-2"></i>Histórico de Envios
                </h5>
            </div>
            
            <div class="card-body">
            <!-- FILTROS SIMPLIFICADOS -->
<div class="filter-panel row gx-2 gy-3 mb-4">
    <div class="col-md-3">
        <label class="form-label">Data Início:</label>
        <asp:TextBox ID="txtDataInicio" runat="server" 
            CssClass="form-control" TextMode="Date"></asp:TextBox>
    </div>
    
    <div class="col-md-3">
        <label class="form-label">Data Fim:</label>
        <asp:TextBox ID="txtDataFim" runat="server" 
            CssClass="form-control" TextMode="Date"></asp:TextBox>
    </div>
    
    <div class="col-md-3">
        <label class="form-label">Código Associado:</label>
        <asp:TextBox ID="txtCodigoAssociado" runat="server"
            CssClass="form-control" placeholder="Código"></asp:TextBox>
    </div>
    
    <div class="col-md-3">
        <label class="form-label">Telefone:</label>
        <asp:TextBox ID="txtTelefone" runat="server"
            CssClass="form-control" placeholder="(00) 00000-0000"></asp:TextBox>
    </div>
    
    <div class="col-md-3">
        <label class="form-label">Status:</label>
        <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-select">
            <asp:ListItem Text="Todos" Value="TODOS" Selected="True" />
            <asp:ListItem Text="Enviado" Value="ENVIADO" />
            <asp:ListItem Text="Falhou" Value="FALHOU" />
            <asp:ListItem Text="Sem documentos" Value="SEM_DOCUMENTOS" />
        </asp:DropDownList>
    </div>
    
    <div class="col-md-3">
        <label class="form-label">Template:</label>
        <asp:DropDownList ID="ddlTemplate" runat="server" CssClass="form-select">
            <asp:ListItem Text="Todos" Value="TODOS" Selected="True" />
            <asp:ListItem Text="Boleto" Value="Suspensao" />
            <asp:ListItem Text="Nota Fiscal" Value="Definitivo" />
            <asp:ListItem Text="Boleto + Nota" Value="aVencer" />
        </asp:DropDownList>
    </div>
    
    <!-- BOTÕES ALINHADOS NO FINAL DA LINHA -->
    <div class="col-md-6 d-flex align-items-end">
        <div class="d-flex gap-2 w-100">
            <div style="flex: 1;"></div> <!-- Espaço vazio para empurrar os botões para direita -->
            <div class="d-flex gap-2">
                   <asp:Button ID="btnFiltrar" runat="server"
       CssClass="btn btn-primary"
       Text="Filtrar"
       OnClick="btnFiltrar_Click"
       style="border-radius: 6px; padding: 8px 32px; font-weight: 500; min-width: 120px;"/>
   
   <asp:Button ID="btnLimpar" runat="server"
       CssClass="btn btn-outline-secondary"
       Text="Limpar"
       OnClick="btnLimpar_Click"
       style="border-radius: 6px; padding: 8px 32px; font-weight: 500; min-width: 120px;"/>
   
   <asp:Button ID="btnExportar" runat="server"
       CssClass="btn btn-success"
       Text="Exportar Excel"
       OnClick="btnExportar_Click"
       style="border-radius: 6px; padding: 8px 32px; font-weight: 500; min-width: 140px;"/>
            </div>
        </div>
    </div>
</div>
                <!-- CONTADOR DE REGISTROS -->
                <div class="alert alert-info d-flex justify-content-between align-items-center mb-3">
                    <span>
                        <i class="fa-solid fa-database me-2"></i>
                        Total de registros: <strong><asp:Literal ID="litTotalRegistros" runat="server">0</asp:Literal></strong>
                    </span>
                </div>
                
             <div class="table-responsive">
    <asp:GridView ID="GridHistorico" runat="server"
        AutoGenerateColumns="False"
        CssClass="table table-hover align-middle mb-0"
        EmptyDataText="Nenhum registro encontrado."
        AllowPaging="true"
        PageSize="30"
        OnPageIndexChanging="GridHistorico_PageIndexChanging">
        <Columns>
            <asp:TemplateField HeaderText="Data/Hora">
                <ItemTemplate>
                    <asp:Label ID="lblDataHora" runat="server"
                        Text='<%# Eval("DataEnvio", "{0:dd/MM/yyyy HH:mm}") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField HeaderText="Código">
                <ItemTemplate>
                    <asp:Label ID="lblCodigo" runat="server"
                        Text='<%# Eval("CodigoAssociado") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Associado">
                <ItemTemplate>
                    <asp:Label ID="lblNome" runat="server"
                        Text='<%# Eval("NomeAssociado") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>
            
            <asp:TemplateField HeaderText="Telefone">
                <ItemTemplate>
                    <asp:Label ID="lblTelefone" runat="server"
                        Text='<%# Eval("NumTelefoneDestino") %>'></asp:Label>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Template">
                <ItemTemplate>
                    <span class='<%# GetBadgeTemplate(Eval("Mensagem").ToString()) %>'>
                        <%# GetDescricaoTemplate(Eval("Mensagem").ToString()) %>
                    </span>
                </ItemTemplate>
            </asp:TemplateField>

            <asp:TemplateField HeaderText="Status">
                <ItemTemplate>
                    <span class='<%# GetBadgeStatus(Eval("StatusEnvio").ToString()) %>'>
                        <%# Eval("StatusEnvio") %>
                    </span>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <PagerSettings Mode="NumericFirstLast" Position="Bottom" />
        <PagerStyle CssClass="pagination justify-content-end" />
    </asp:GridView>
</div>
            </div>
        </div>
    </div>

    <script>
        function verDetalhes(codEnvio) {
            $('#modalDetalhesConteudo').html(`
            <div class="text-center p-5">
                <i class="fa-solid fa-spinner fa-spin fa-2x text-primary"></i>
                <p class="mt-2">Carregando detalhes...</p>
            </div>
        `);

            // CORREÇÃO: MUDAR DE 'ConsultaHistoricoEnvios.aspx' PARA 'viewShippingHistory.aspx'
            $.ajax({
                url: 'viewShippingHistory.aspx/ObterDetalhesEnvio',
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify({ codEnvio: codEnvio }),
                success: function (response) {
                    if (response.d) {
                        $('#modalDetalhesConteudo').html(response.d);
                    }
                },
                error: function () {
                    $('#modalDetalhesConteudo').html(`
                    <div class="alert alert-danger">
                        <i class="fa-solid fa-exclamation-triangle me-2"></i>
                        Erro ao carregar detalhes.
                    </div>
                `);
                }
            });

            new bootstrap.Modal(document.getElementById('modalDetalhes')).show();
        }
    </script>
</asp:Content>
