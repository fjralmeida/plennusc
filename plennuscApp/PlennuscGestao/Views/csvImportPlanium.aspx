<%@ Page Title="Importar CSV Planium" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="csvImportPlanium.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.csvImportPlanuim" Async="true" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
   <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet">
   <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />

    <title>Importação de Arquivo CSV – Planium</title>

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
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <h2 class="titulo-pagina">
            <i class="fa-solid fa-file-csv me-2" style="color:#83CEEE;"></i> Importação de Arquivo CSV
        </h2>

        <div class="card-container">
            <div class="filter-panel">
                <div class="row g-3 align-items-end">
                    <div class="col-md-8">
                        <label class="form-label fw-semibold">Selecionar Arquivo CSV</label>
                        <asp:FileUpload ID="fileUploadCsv" runat="server" CssClass="form-control" />
                    </div>
                    <div class="col-md-4 text-end">
                        <asp:Button ID="btnLerCsv" runat="server" Text="Carregar CSV"
                            OnClick="btnLerCsv_Click"
                            CssClass="btn btn-purple btn-pill" />
                    </div>
                </div>
            </div>

            <div class="grid-wrapper">
                <asp:GridView ID="gridCsv" runat="server" AutoGenerateColumns="true"
                    CssClass="table table-striped table-bordered shadow-sm rounded mb-0" />
            </div>

            <div class="text-end">
                <asp:Button ID="btnEnviar" runat="server" Text="Upload Arquivo"
                    CssClass="btn btn-success btn-pill"
                    Enabled="false"
                    OnClick="btnEnviar_Click" />
            </div>

            <asp:Literal ID="litResultado" runat="server" Mode="PassThrough"></asp:Literal>
        </div>
    </div>
</asp:Content>
