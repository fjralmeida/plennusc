﻿<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="linkSector.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.linkSector" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
          <title>Vincular Setores às Estruturas</title>
    <style>
        :root {
            --primary: #83ceee;
            --primary-hover: #0d62c9;
            --success: #4cb07a;
            --success-hover: #3b8b65;
            --warning: #ffa726;
            --warning-hover: #f57c00;
            --gray-50: #f8f9fa;
            --gray-100: #f1f3f4;
            --gray-200: #e8eaed;
            --gray-300: #dadce0;
            --gray-400: #bdc1c6;
            --gray-500: #9aa0a6;
            --gray-600: #80868b;
            --gray-700: #5f6368;
            --gray-800: #3c4043;
            --gray-900: #202124;
            --border-radius: 8px;
            --shadow: 0 1px 2px 0 rgba(60, 64, 67, 0.3), 0 1px 3px 1px rgba(60, 64, 67, 0.15);
            --transition: all 0.2s ease-in-out;
        }

        body {
            background: var(--gray-100);
            font-family: 'Roboto', sans-serif;
            color: var(--gray-800);
            line-height: 1.5;
        }

        .container-main {
            max-width: 2206px;
            margin: 20px auto;
            padding: 0 16px;
        }

        /* Header */
        .page-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 24px;
            flex-wrap: wrap;
            gap: 16px;
        }

        .page-title {
            display: flex;
            align-items: center;
            gap: 12px;
            font-size: 24px;
            font-weight: 500;
            color: var(--gray-800);
            margin: 0;
        }

        .title-icon {
            background: var(--primary);
            color: white;
            width: 44px;
            height: 44px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
        }

        .btn-primary {
            background: var(--success);
            border: none;
            color: white;
            font-weight: 500;
            padding: 10px 20px;
            border-radius: 4px;
            transition: var(--transition);
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .btn-primary:hover {
            background: var(--success-hover);
            box-shadow: 0 1px 3px rgba(0, 0, 0, 0.2);
        }

        /* Card Principal */
        .main-card {
            background: white;
            border-radius: var(--border-radius);
            box-shadow: var(--shadow);
            margin-bottom: 24px;
            overflow: hidden;
        }

        .card-header {
            background: var(--gray-50);
            padding: 20px 24px;
            border-bottom: 1px solid var(--gray-200);
        }

        .card-title {
            font-size: 18px;
            font-weight: 500;
            color: var(--gray-800);
            margin: 0;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .card-body {
            padding: 24px;
        }

        /* Formulário */
        .form-group {
            margin-bottom: 20px;
        }

        .form-label {
            display: block;
            font-size: 14px;
            font-weight: 500;
            color: var(--gray-700);
            margin-bottom: 8px;
            white-space: nowrap;
        }

        .form-control, .form-select {
            width: 100%;
            padding: 10px 12px;
            border: 1px solid var(--gray-300);
            border-radius: 4px;
            font-size: 14px;
            transition: var(--transition);
            background: white;
            height: 40px;
            box-sizing: border-box;
        }

        .form-control:focus, .form-select:focus {
            outline: none;
            border-color: var(--primary);
            box-shadow: 0 0 0 2px rgba(26, 115, 232, 0.2);
        }

        /* Alertas */
        .alert {
            padding: 16px;
            border-radius: 4px;
            margin-bottom: 20px;
            border-left: 4px solid;
        }

        .alert-info {
            background: #e3f2fd;
            border-color: #2196f3;
            color: #1565c0;
        }

        /* ESTILOS DA GRIDVIEW */
        .table {
            width: 100%;
            border-collapse: collapse;
            margin: 0;
            font-size: 14px;
            table-layout: fixed;
        }

        .table th {
            background: var(--gray-50);
            color: var(--gray-700);
            font-weight: 600;
            padding: 12px 16px;
            text-align: left;
            border-bottom: 2px solid var(--gray-200);
            white-space: nowrap;
            overflow: hidden;
            text-overflow: ellipsis;
        }

        .table td {
            padding: 12px 16px;
            border-bottom: 1px solid var(--gray-200);
            vertical-align: middle;
            color: var(--gray-800);
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
        }

        .table tbody tr {
            transition: var(--transition);
        }

        .table tbody tr:hover {
            background: var(--gray-50);
        }

        .table-striped tbody tr:nth-child(odd) {
            background: var(--gray-100);
        }

        .table-striped tbody tr:nth-child(odd):hover {
            background: var(--gray-200);
        }

        .table-bordered {
            border: 1px solid var(--gray-200);
        }

        .table-bordered th,
        .table-bordered td {
            border: 1px solid var(--gray-200);
        }

        /* BOTÕES DO GRID */
        .btn {
            display: inline-flex;
            align-items: center;
            gap: 4px;
            padding: 6px 12px;
            font-size: 12px;
            font-weight: 500;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            transition: var(--transition);
            text-decoration: none;
            white-space: nowrap;
        }

        .btn-sm {
            padding: 4px 8px;
            font-size: 11px;
        }

        .btn-success {
            background: var(--success);
            color: white;
        }

        .btn-success:hover {
            background: var(--success-hover);
            box-shadow: 0 1px 3px rgba(76, 176, 122, 0.3);
        }

        .btn-danger {
            background: #dc3545;
            color: white;
        }

        .btn-danger:hover {
            background: #c82333;
            box-shadow: 0 1px 3px rgba(220, 53, 69, 0.3);
        }

        /* ESTADOS DA GRID */
        .empty-data {
            text-align: center;
            padding: 40px 20px;
            color: var(--gray-500);
            font-style: italic;
        }

        .empty-data i {
            font-size: 24px;
            margin-bottom: 8px;
            display: block;
        }

        /* BADGES E STATUS */
        .badge {
            display: inline-block;
            padding: 4px 8px;
            font-size: 11px;
            font-weight: 600;
            border-radius: 12px;
            text-transform: uppercase;
        }

        .badge-success {
            background: #d4edda;
            color: #155724;
        }

        .badge-secondary {
            background: #e2e3e5;
            color: #383d41;
        }

        /* LARGURAS DAS COLUNAS PARA EVITAR SCROLL */
        .column-text {
            width: 30%;
            min-width: 200px;
        }

        .column-number {
            width: 10%;
            min-width: 80px;
            text-align: center;
        }

        .column-setores {
            width: 25%;
            min-width: 150px;
        }

        .column-vincular {
            width: 20%;
            min-width: 150px;
        }

        .column-actions {
            width: 15%;
            min-width: 100px;
            text-align: center;
        }

        /* CARDS INTERNOS */
        .card {
            background: white;
            border-radius: var(--border-radius);
            box-shadow: var(--shadow);
            margin-bottom: 16px;
            overflow: hidden;
        }

        .card .card-header {
            background: var(--gray-50);
            padding: 16px 20px;
            border-bottom: 1px solid var(--gray-200);
        }

        .card .card-title {
            font-size: 16px;
            font-weight: 500;
            color: var(--gray-800);
            margin: 0;
            display: flex;
            align-items: center;
            gap: 8px;
        }

        .card .card-body {
            padding: 20px;
        }

        /* MENSAGENS */
        .message {
            padding: 12px 16px;
            border-radius: 4px;
            margin-top: 16px;
            font-size: 14px;
        }

        .message-success {
            background: #e8f5e9;
            border: 1px solid #c8e6c9;
            color: #2e7d32;
        }

        .message-error {
            background: #ffebee;
            border: 1px solid #ffcdd2;
            color: #c62828;
        }

        .message-info {
            background: #e3f2fd;
            border: 1px solid #bbdefb;
            color: #1565c0;
        }

        /* HOVER EFFECTS MELHORADOS */
        .table tbody tr {
            border-left: 3px solid transparent;
            transition: all 0.2s ease;
        }

        .table tbody tr:hover {
            border-left: 3px solid var(--primary);
            background: #f8fbff;
        }

        /* RESPONSIVIDADE */
        @media (max-width: 768px) {
            .page-header {
                flex-direction: column;
                align-items: flex-start;
            }

            .card-body {
                padding: 16px;
            }

            .table th,
            .table td {
                padding: 8px 12px;
                font-size: 12px;
            }

            .btn {
                padding: 4px 6px;
                font-size: 10px;
            }

            /* EM MOBILE, PERMITE QUEBRAR TEXTO */
            .table td {
                white-space: normal;
                word-wrap: break-word;
            }

            /* AJUSTA LARGURAS DAS COLUNAS EM MOBILE */
            .column-text {
                width: 40%;
            }
            .column-setores {
                width: 30%;
            }
            .column-vincular {
                width: 30%;
            }
            .column-actions {
                width: 20%;
            }
        }

        /* ANIMAÇÕES */
        @keyframes fadeIn {
            from {
                opacity: 0;
                transform: translateY(10px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        .main-card {
            animation: fadeIn 0.3s ease-out;
        }

        /* ESTILOS ESPECÍFICOS PARA OS DROPDOWNS NA GRID */
        .table .form-select {
            height: 32px;
            font-size: 12px;
            padding: 4px 8px;
        }

        /* PEQUENOS AJUSTES VISUAIS */
        .mt-4 {
            margin-top: 24px;
        }

        small {
            font-size: 11px;
            color: var(--gray-600);
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <div class="page-header">
            <div class="page-title">
                <div class="title-icon">
                    <i class="bi bi-link-45deg"></i>
                </div>
                Vincular Setores às Estruturas
            </div>
        </div>

        <div class="main-card">
            <div class="card-header">
                <h2 class="card-title">
                    <i class="bi bi-diagram-3"></i>
                    Gerenciar Vínculos
                </h2>
            </div>
            
            <div class="card-body">
                <!-- COMBO COM AS VIEWS -->
                <div class="form-group">
                    <label class="form-label">Selecione uma View *</label>
                    <asp:DropDownList ID="ddlView" runat="server" CssClass="form-control form-select" AutoPostBack="true"
                        OnSelectedIndexChanged="ddlView_SelectedIndexChanged">
                        <asp:ListItem Text="Selecione uma View" Value=""></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <!-- GRID COM ESTRUTURAS DA VIEW SELECIONADA -->
                <asp:Panel ID="pnlEstruturas" runat="server" Visible="false" class="mt-4">
                    <div class="card">
                        <div class="card-header">
                            <h5 class="card-title">
                                <i class="bi bi-list-ul"></i>
                                Estruturas da View: <asp:Label ID="lblViewSelecionada" runat="server" Text=""></asp:Label>
                            </h5>
                        </div>
                        <div class="card-body">
                            <asp:GridView ID="gvEstruturas" runat="server" CssClass="table table-striped table-bordered" 
                                AutoGenerateColumns="false" EmptyDataText="Nenhuma estrutura encontrada para esta View"
                                OnRowDataBound="gvEstruturas_RowDataBound">
                                <Columns>
                                    <asp:TemplateField Visible="false">
                                        <ItemTemplate>
                                            <asp:Label ID="lblCodEstrutura" runat="server" Text='<%# Eval("CodEstrutura") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="DescEstrutura" HeaderText="Nome da Estrutura" 
                                        ItemStyle-CssClass="column-text" HeaderStyle-CssClass="column-text" />
                                    
                                    <asp:BoundField DataField="ValorPadrao" HeaderText="Ordem" 
                                        ItemStyle-CssClass="column-number" HeaderStyle-CssClass="column-number" />
                                   
                                    <asp:TemplateField HeaderText="Setores Vinculados" 
                                        ItemStyle-CssClass="column-setores" HeaderStyle-CssClass="column-setores">
                                        <ItemTemplate>
                                            <small>Vincule um setor ao lado →</small>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Vincular Novo Setor" 
                                        ItemStyle-CssClass="column-vincular" HeaderStyle-CssClass="column-vincular">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="ddlSetor" runat="server" CssClass="form-control form-select">
                                            </asp:DropDownList>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Ações" 
                                        ItemStyle-CssClass="column-actions" HeaderStyle-CssClass="column-actions">
                                        <ItemTemplate>
                                            <asp:Button ID="btnVincularSetor" runat="server" CssClass="btn btn-success btn-sm"
                                                Text="Vincular" OnClick="btnVincularSetor_Click" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    <div class="empty-data">
                                        <i class="bi bi-inbox"></i>
                                        Nenhuma estrutura encontrada para esta View
                                    </div>
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </div>
    </div>

    <script>
        // Funções para exibir toasts (iguais às da outra página)
        function showToastErroObrigatorio() {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'error',
                title: 'Preencha todos os campos obrigatórios.',
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true
            });
        }
        
        function showToastSucesso(mensagem) {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'success',
                title: mensagem,
                showConfirmButton: false,
                timer: 3000,
                timerProgressBar: true
            });
        }
        
        function showToastErro(mensagem) {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'error',
                title: mensagem,
                showConfirmButton: false,
                timer: 4000,
                timerProgressBar: true
            });
        }
        
        function showToastAviso(mensagem) {
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'warning',
                title: mensagem,
                showConfirmButton: false,
                timer: 4000,
                timerProgressBar: true
            });
        }
    </script>
</asp:Content>
