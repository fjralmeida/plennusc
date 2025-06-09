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
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">

        <asp:Button ID="btnTestarApi" runat="server" Text="Testar API" OnClick="btnTestarApi_Click" />
        <br /><br />
        <asp:Label ID="lblResultado" runat="server" Text=""></asp:Label>

            <asp:FileUpload ID="FileUpload1" runat="server" />
                <asp:Button ID="btnUpload" runat="server" Text="Upload PDF" OnClick="btnUpload_Click" />
                <asp:Label ID="lblStatus" runat="server" Text=""></asp:Label>

            <asp:Panel CssClass="text-center" runat="server" ID="pnlGridAssociados">
                <div class="panel-body text-center">
                    <asp:Literal ID="LiteralMensagem" runat="server"></asp:Literal>
                    <div id="gridAssociados" style="width: 100%; height: 550px;">
                        <div style="color:red; font-size:20px;">
                            <asp:Label runat="server" ID="LblMensagem" Visible="false">Nenhum associado encontrado.</asp:Label>
                        </div>

                        <asp:GridView runat="server" ID="GridAssociados"
                            AutoGenerateColumns="False"
                            CssClass="table table-bordered table-hover table-striped">

                            <Columns>
                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label runat="server" ToolTip="Código do Associado">Código</asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblCodigo" Text='<%# Eval("CODIGO_ASSOCIADO") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label runat="server" ToolTip="Nome do Associado">Associado</asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblNome" Text='<%# Eval("NOME_ASSOCIADO") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label runat="server" ToolTip="Plano">Plano</asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblPlano" Text='<%# Eval("NOME_PLANO_ABREVIADO") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label runat="server" ToolTip="Data de Vencimento">Vencimento</asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblVencimento" 
                                            Text='<%# String.Format("{0:dd/MM/yyyy}", Eval("DATA_VENCIMENTO")) %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:TemplateField>
                                    <HeaderTemplate>
                                        <asp:Label runat="server" ToolTip="Valor Convênio">Valor</asp:Label>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <asp:Label runat="server" ID="lblValor" 
                                            Text='<%# String.Format("R$ {0:N2}", Eval("VALOR_CONVENIO")) %>'>
                                        </asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </form>
</body>
</html>
