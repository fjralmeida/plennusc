<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="csvImportPlanium.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.csvImportPlanuim" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />
    <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/js/bootstrap.bundle.min.js"></script>

    <style>
        body {
            background: #f5f7fa;
            font-family: 'Poppins', sans-serif;
            font-size: 13px;
            color: #333;
        }

        .container {
            max-width: 900px;
            margin: 0 auto;
            padding: 32px 16px;
        }

        .card-container {
            background: #fff;
            border-radius: 12px;
            box-shadow: 0 1px 6px rgba(0,0,0,0.05);
            padding: 24px;
            margin-top: 24px;
        }

        .card-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 24px;
        }

            .card-header h2 {
                font-weight: 600;
                font-size: 20px;
                margin: 0;
            }

        .btn-pill {
            border-radius: 50px;
            padding: 6px 18px;
            font-weight: 600;
            box-shadow: 0 2px 6px rgba(0,0,0,0.1);
            transition: background-color 0.3s, box-shadow 0.3s;
        }

        .btn-primary {
            background-color: #4285f4;
            border-color: #4285f4;
            color: #fff;
        }

            .btn-primary:hover {
                background-color: #2d6cdf;
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
            border-radius: 8px;
            margin-bottom: 24px;
        }

        .grid-wrapper {
            max-height: 530px; /* permite rolar se passar dessa altura */
            overflow: auto;
            border: 0px solid #ddd;
            border-radius: 8px;
            margin-top: 20px;
            position: relative;
            background: white;
        }

            .grid-wrapper::-webkit-scrollbar {
                height: 10px;
                background: #f1f1f1;
            }

            .grid-wrapper::-webkit-scrollbar-thumb {
                background: #ccc;
                border-radius: 10px;
            }

            .grid-wrapper table {
                min-width: 1200px; /* controla largura mínima */
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
        <div class="card-container">
            <div class="card-header">
                <h2 class="mb-0" style="font-size: 18px; font-weight: 500; color: #333;">
                    <i class="fa-solid fa-file-csv me-2" style="font-size: 18px; color: #83CEEE;"></i>
                    Importação de aquivo CSV
                </h2>

                <asp:Button ID="btnEnviar" runat="server" Text="Upload Arquivo"
                    CssClass="btn btn-success btn-pill"
                    Enabled="false"
                    OnClick="btnEnviar_Click" />
            </div>

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
                    CssClass="table table-striped table-bordered shadow-sm mb-0" />
            </div>
            <asp:Literal ID="litResultado" runat="server" Mode="PassThrough"></asp:Literal>
        </div>
    </div>
</asp:Content>
