<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TelaEnvio.aspx.cs" Inherits="appWhatsapp.Controller.TelaEnvio" Async="true" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>Consulta Associados</title>

    <!-- Bootstrap (opcional, para melhor visual) -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />

    <style>
        body {
            padding: 20px;
            background-color: #f9f9f9;
        }

        .panel-body {
            background-color: #ffffff;
            padding: 20px;
            border: 1px solid #ccc;
            border-radius: 10px;
        }

        .table th, .table td {
            vertical-align: middle;
            text-align: center;
        }

        .table thead {
            background-color: #003399;
            color: white;
        }
        .btn-primary {
            background-color: #007bff;
            border-color: #007bff;
            color: white;
            cursor: pointer;
            border-radius: 4px;
        }

        .btn-primary:hover {
            background-color: #0056b3;
            border-color: #004085;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">

        <asp:Button ID="btnTestarApi" runat="server" Text="Testar API" OnClick="btnTestarApi_Click" />
        <br /><br />
        <asp:Label ID="lblResultado" runat="server" Text=""></asp:Label>

       <asp:Panel CssClass="container" runat="server" ID="pnlGridAssociados" Style="max-width: 960px; margin: auto; padding: 30px;">
    
       <div class="d-flex justify-content-center mb-4">
    <div class="row align-items-end" style="max-width: 600px; width: 100%;">
        <!-- Data inicial -->
        <div class="col-md-4">
            <label for="txtDataInicio" class="form-label fw-bold">De:</label>
            <input type="date" id="txtDataInicio" runat="server"
                   class="form-control" />
        </div>

        <!-- Data final -->
        <div class="col-md-4">
            <label for="txtDataFim" class="form-label fw-bold">Até:</label>
            <input type="date" id="txtDataFim" runat="server"
                   class="form-control" />
        </div>

        <!-- Botão -->
        <div class="col-md-4">
            <asp:Button ID="btnFiltrar" runat="server" Text="Filtrar"
                        CssClass="btn btn-primary w-100 fw-bold mt-3 mt-md-0"
                        OnClick="btnFiltrar_Click" />
        </div>
    </div>
</div>


    <asp:Literal ID="LiteralMensagem" runat="server"></asp:Literal>

    <div class="table-responsive">
        <asp:GridView runat="server" ID="GridAssociados"
                        AutoGenerateColumns="False"
                        CssClass="table table-bordered table-hover align-middle"
                        EmptyDataText="Nenhum registro encontrado.">
            
           <Columns>

               <asp:TemplateField HeaderText="Selecionar">
                   <ItemTemplate>
                       <asp:CheckBox ID="chkSelecionar" runat="server" />
                   </ItemTemplate>
                   <HeaderStyle HorizontalAlign="Center" />
                   <ItemStyle HorizontalAlign="Center" />
               </asp:TemplateField>

               <asp:TemplateField HeaderText="Código">
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("CODIGO_ASSOCIADO") %>' CssClass="fw-semibold"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Registro">
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("NUMERO_REGISTRO") %>' CssClass="fw-semibold"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Associado">
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("NOME_ASSOCIADO") %>' CssClass="fw-semibold"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Plano">
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("NOME_PLANO_ABREVIADO") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Vencimento">
                    <ItemTemplate>
                        <asp:Label runat="server" 
                                   Text='<%# String.Format("{0:dd/MM/yyyy}", Eval("DATA_VENCIMENTO")) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Valor">
                    <ItemTemplate>
                        <asp:Label runat="server" 
                                   Text='<%# String.Format("R$ {0:N2}", Eval("VALOR_FATURA")) %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Telefone">
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%# Eval("NUMERO_TELEFONE") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

           <asp:Button ID="btnSelecionar" runat="server" Text="Processar Selecionados"
                        CssClass="btn btn-success mt-3 fw-bold"
                        OnClick="btnSelecionar_Click" />


    <asp:Label runat="server" ID="LblMensagem" Visible="false" CssClass="text-danger fs-5 fw-bold d-block mt-3 text-center"></asp:Label>
</asp:Panel>

        </div>
    </form>
</body>
</html>
