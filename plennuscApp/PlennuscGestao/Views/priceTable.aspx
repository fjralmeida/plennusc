<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="priceTable.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.priceTable" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet">
<link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />

 <style>
     * {
         font-family: 'Poppins', sans-serif;
         box-sizing: border-box;
     }

     body {
         background-color: #f2f4f8;
         font-size: 13px;
         color: #333;
     }

     .container {
         max-width: 960px;
         margin: auto;
         padding: 32px 16px;
     }

     .titulo-pagina {
         font-size: 22px;
         font-weight: 600;
         color: #4CB07A;
         text-align: center;
         margin-bottom: 30px;
         position: relative;
     }

     .titulo-pagina::after {
         content: "";
         width: 60px;
         height: 3px;
         background-color: #83CEEE;
         display: block;
         margin: 0.5rem auto 0 auto;
         border-radius: 2px;
     }

     .card-container {
         background: white;
         padding: 13px;
         border-radius: 16px;
         box-shadow: 0 3px 10px rgba(0, 0, 0, 0.06);
     }

     .btn-pill {
         border-radius: 50px;
         padding: 6px 18px;
         font-weight: 600;
         transition: all 0.3s ease-in-out;
         box-shadow: 0 2px 6px rgba(0,0,0,0.1);
     }

     .btn-success {
         background-color: #4CB07A;
         border-color: #4CB07A;
         color: #fff;
     }

     .btn-success:hover {
         background-color: #3B8B65;
     }

     .btn-purple {
         background-color: #C06ED4;
         border-color: #C06ED4;
         color: #fff;
     }

     .btn-purple:hover {
         background-color: #a14db8;
     }

     .filter-panel {
         background: #f0f2f5;
         padding: 16px 20px;
         border-radius: 12px;
         margin-bottom: 24px;
     }

     .grid-wrapper {
         max-height: 530px;
         overflow: auto;
         border-radius: 12px;
         margin-bottom: 20px;
     }

     .grid-wrapper table {
         min-width: 1200px;
     }

     .table th, .table td {
         white-space: nowrap;
         padding: 10px 16px;
         font-size: 13px;
         text-align: center;
     }
    .grid-wrapper {
        max-height: 530px;
        overflow-y: auto;
        border-radius: 12px;
        margin-bottom: 20px;
        border: 1px  #ddd;
    }

    .grid-wrapper table {
        min-width: 1200px;
    }

    .table th {
        white-space: nowrap;
        padding: 10px 16px;
        font-size: 13px;
        text-align: center;
    }

    .table td {
        white-space: nowrap;
        padding: 8px 14px;
        font-size: 13px;
        text-align: center;
    }


 </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <div class="container">
    <h2 class="titulo-pagina">
        <i class="fa-solid fa-file-csv me-2" style="color:#83CEEE;"></i> Importação de Arquivo XLS
    </h2>

    <div class="card-container">
        <div class="filter-panel">
            <div class="row g-3 align-items-end">
                <div class="col-md-8">
                    <label class="form-label fw-semibold">Selecionar Arquivo XLS</label>
                    <asp:FileUpload ID="fileUploadXls" runat="server" CssClass="form-control" />
                </div>
                <div class="col-md-4 text-end">
                    <asp:Button ID="btnLerXls" runat="server" Text="Carregar XLS"
                        OnClick="btnLerXls_Click"
                        CssClass="btn btn-purple btn-pill" />
                </div>
            </div>
        </div>

           <div class="grid-wrapper">
                <asp:GridView ID="gridXsl" 
                              runat="server" 
                              AutoGenerateColumns="False" 
                              CssClass="table table-striped table-bordered table-hover"
                              HeaderStyle-BackColor="#4CB07A" 
                              HeaderStyle-ForeColor="White" 
                              HeaderStyle-Font-Bold="True"
                              RowStyle-VerticalAlign="Middle"
                              GridLines="None">
                    <Columns>
                        <asp:BoundField DataField="CODIGO_PLANO" HeaderText="CODIGO_PLANO" />
                        <asp:BoundField DataField="CODIGO_TABELA_PRECO" HeaderText="CODIGO_TABELA_PRECO" />
                        <asp:BoundField DataField="IDADE_MINIMA" HeaderText="IDADE_MINIMA" />
                        <asp:BoundField DataField="IDADE_MAXIMA" HeaderText="IDADE_MAXIMA" />
                        <asp:BoundField DataField="VALOR_PLANO" HeaderText="VALOR_PLANO"
                                        DataFormatString="{0:N2}" HtmlEncode="false" />
                        <asp:BoundField DataField="TIPO_RELACAO_DEPENDENCIA" HeaderText="TIPO_RELACAO_DEPENDENCIA" />
                        <asp:BoundField DataField="CODIGO_GRUPO_CONTRATO" HeaderText="CODIGO_GRUPO_CONTRATO" />
                        <asp:BoundField DataField="NOME_TABELA" HeaderText="NOME_TABELA" />
                        <asp:BoundField DataField="VALOR_NET" HeaderText="VALOR_NET"
                                        DataFormatString="{0:N2}" HtmlEncode="false" />
                        <asp:BoundField DataField="TIPO_CONTRATO_ESTIPULADO" HeaderText="TIPO_CONTRATO_ESTIPULADO" />
                    </Columns>
                </asp:GridView>
            </div>

        <div class="text-end">
            <asp:Button ID="btnEnviar" runat="server" Text="Upload Arquivo"
                CssClass="btn btn-success btn-pill"
                Enabled="false"
                OnClick="btnEnviar_Click" />
        </div>

        <asp:Literal ID="lblResultado" runat="server" Mode="PassThrough"></asp:Literal>
    </div>
</div>

</asp:Content>
