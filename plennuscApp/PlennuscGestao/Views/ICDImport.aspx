<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" AutoEventWireup="true" CodeBehind="ICDImport.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.ICDImport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/Css/projects/gestao/structuresCss/CIDs/ICDImport.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">

        <div class="page-header">
            <h2 class="page-title">
                <span class="title-icon"><i class="fa fa-file-medical"></i></span>
                Importação de CID
            </h2>
        </div>

        <div class="filters-card">
            <div class="filters-title">
                <i class="fa fa-upload"></i> Importar Arquivo
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Vigência <span class="icd-required">*</span></label>
                    <asp:TextBox ID="txtVigencia" runat="server" TextMode="Date" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvVigencia" runat="server"
                        ControlToValidate="txtVigencia"
                        ErrorMessage="Campo Vigência é obrigatório."
                        CssClass="msg-importacao erro" Display="Dynamic" />
                </div>

                <div class="form-group">
                    <label class="form-label">Arquivo Excel <span class="icd-required">*</span></label>
                    <asp:FileUpload ID="fileUploadExcel" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvArquivo" runat="server"
                        ControlToValidate="fileUploadExcel"
                        ErrorMessage="Selecione um arquivo Excel."
                        CssClass="msg-importacao erro" Display="Dynamic" />
                </div>

                <div class="form-row" style="margin-top: 10px;">
    <asp:Button ID="btnImportar" runat="server" Text="Importar" CssClass="btn btn-primary" OnClick="btnImportar_Click" />
</div>
            </div>

            
        </div>

        <asp:Panel ID="pnlResultado" runat="server" Visible="false">
            <div class="tabs-status">
                <button type="button" class="tab-status-link active" data-tab-target="tab-todos">
                    Todos <span class="tab-status-count"><asp:Literal ID="litCountTodos" runat="server" /></span>
                </button>
                <button type="button" class="tab-status-link" data-tab-target="tab-importados">
                    Importados <span class="tab-status-count"><asp:Literal ID="litCountImportados" runat="server" /></span>
                </button>
                <button type="button" class="tab-status-link" data-tab-target="tab-ja-cadastrados">
                    Já Cadastrados <span class="tab-status-count"><asp:Literal ID="litCountJaCadastrados" runat="server" /></span>
                </button>
                <button type="button" class="tab-status-link" data-tab-target="tab-divergencia">
                    Vigência Divergente <span class="tab-status-count"><asp:Literal ID="litCountDivergencia" runat="server" /></span>
                </button>
                <button type="button" class="tab-status-link" data-tab-target="tab-cid-invalido">
                    CID Inválido <span class="tab-status-count"><asp:Literal ID="litCountCidInvalido" runat="server" /></span>
                </button>
                <button type="button" class="tab-status-link" data-tab-target="tab-nao-encontrado">
                    CPF Não Encontrado <span class="tab-status-count"><asp:Literal ID="litCountNaoEncontrado" runat="server" /></span>
                </button>
            </div>


      <div class="tab-status-panel active" id="tab-todos">
    <div class="grid-container">

        
            <div class="grid-toolbar">
    <div class="grid-toolbar-left">
        <label for="ddlPageSize">Registros por página:</label>
        <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged" CssClass="form-control ddl-pagesize">
            <asp:ListItem Text="5" Value="5" />
            <asp:ListItem Text="10" Value="10" Selected="True" />
            <asp:ListItem Text="20" Value="20" />
            <asp:ListItem Text="50" Value="50" />
            <asp:ListItem Text="100" Value="100" />
        </asp:DropDownList>
    </div>
</div>

        <asp:GridView ID="gridTodos" runat="server" AutoGenerateColumns="false" CssClass="custom-grid"
            EmptyDataText="Nenhum registro processado."
            OnRowDataBound="gridTodos_RowDataBound"
            OnDataBound="gridTodos_DataBound"
            AllowPaging="True" PageSize="10"
            OnPageIndexChanging="gridTodos_PageIndexChanging"
            EnableViewState="true">
            <PagerSettings Mode="NumericFirstLast"
                FirstPageText="«" LastPageText="»"
                PageButtonCount="7"
                Position="Bottom" />
            <PagerStyle CssClass="pager-custom" />
            <Columns>
                <asp:BoundField DataField="Cpf" HeaderText="CPF" />
                <asp:BoundField DataField="Titular" HeaderText="Titular" />
                <asp:BoundField DataField="Beneficiario" HeaderText="Beneficiário" />
                <asp:BoundField DataField="Cid" HeaderText="CID" />
                <asp:BoundField DataField="CodigoAssociado" HeaderText="Cód. Associado" />
                <asp:TemplateField HeaderText="Status">
                    <ItemTemplate>
                        <span class='badge <%# GetStatusCss(Eval("Sucesso"), Eval("Motivo")) %>'>
                            <%# Eval("Sucesso").Equals(true) ? "Importado" : "Não Importado" %>
                        </span>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Motivo" HeaderText="Motivo" />
            </Columns>
        </asp:GridView>
        <asp:Label ID="lblPagerInfo" runat="server" CssClass="pager-info" />
    </div>
</div>
            <!-- Demais grids (Importados, JaCadastrados, etc.) sem paginação -->
            <div class="tab-status-panel" id="tab-importados">
                <div class="grid-container">
                    <asp:GridView ID="gridImportados" runat="server" AutoGenerateColumns="false" CssClass="custom-grid"
                        EmptyDataText="Nenhum registro importado.">
                        <Columns>
                            <asp:BoundField DataField="Cpf" HeaderText="CPF" />
                            <asp:BoundField DataField="Titular" HeaderText="Titular" />
                            <asp:BoundField DataField="Beneficiario" HeaderText="Beneficiário" />
                            <asp:BoundField DataField="Cid" HeaderText="CID" />
                            <asp:BoundField DataField="CodigoAssociado" HeaderText="Cód. Associado" />
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate><span class="badge status-ok">Importado</span></ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

            <div class="tab-status-panel" id="tab-ja-cadastrados">
                <div class="grid-container">
                    <asp:GridView ID="gridJaCadastrados" runat="server" AutoGenerateColumns="false" CssClass="custom-grid"
                        EmptyDataText="Nenhum registro já cadastrado encontrado.">
                        <Columns>
                            <asp:BoundField DataField="Cpf" HeaderText="CPF" />
                            <asp:BoundField DataField="Titular" HeaderText="Titular" />
                            <asp:BoundField DataField="Cid" HeaderText="CID" />
                            <asp:BoundField DataField="CodigoAssociado" HeaderText="Cód. Associado" />
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate><span class="badge status-nao-encontrado">Já Cadastrado</span></ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

            <div class="tab-status-panel" id="tab-divergencia">
                <div class="grid-container">
                    <asp:GridView ID="gridDivergencia" runat="server" AutoGenerateColumns="false" CssClass="custom-grid"
                        EmptyDataText="Nenhuma divergência de vigência encontrada.">
                        <Columns>
                            <asp:BoundField DataField="Cpf" HeaderText="CPF" />
                            <asp:BoundField DataField="Titular" HeaderText="Titular" />
                            <asp:BoundField DataField="Cid" HeaderText="CID" />
                            <asp:BoundField DataField="Motivo" HeaderText="Motivo" />
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate><span class="badge status-divergencia-tolerada">Divergência</span></ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

            <div class="tab-status-panel" id="tab-cid-invalido">
                <div class="grid-container">
                    <asp:GridView ID="gridCidInvalido" runat="server" AutoGenerateColumns="false" CssClass="custom-grid"
                        EmptyDataText="Nenhum CID inválido encontrado.">
                        <Columns>
                            <asp:BoundField DataField="Cpf" HeaderText="CPF" />
                            <asp:BoundField DataField="Titular" HeaderText="Titular" />
                            <asp:BoundField DataField="Cid" HeaderText="CID" />
                            <asp:BoundField DataField="Motivo" HeaderText="Motivo" />
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate><span class="badge status-divergente">CID Inválido</span></ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>

            <div class="tab-status-panel" id="tab-nao-encontrado">
                <div class="grid-container">
                    <asp:GridView ID="gridNaoEncontrado" runat="server" AutoGenerateColumns="false" CssClass="custom-grid"
                        EmptyDataText="Nenhum CPF não encontrado.">
                        <Columns>
                            <asp:BoundField DataField="Cpf" HeaderText="CPF" />
                            <asp:BoundField DataField="Titular" HeaderText="Titular" />
                            <asp:BoundField DataField="Motivo" HeaderText="Motivo" />
                            <asp:TemplateField HeaderText="Status">
                                <ItemTemplate><span class="badge status-nao-encontrado">Não Encontrado</span></ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </asp:Panel>
    </div>

    <script>
        document.addEventListener('DOMContentLoaded', function () {
            var tabs = document.querySelectorAll('.tab-status-link');
            var panels = document.querySelectorAll('.tab-status-panel');
            tabs.forEach(function (tab) {
                tab.addEventListener('click', function () {
                    tabs.forEach(function (t) { t.classList.remove('active'); });
                    panels.forEach(function (p) { p.classList.remove('active'); });
                    tab.classList.add('active');
                    var targetId = tab.getAttribute('data-tab-target');
                    document.getElementById(targetId).classList.add('active');
                });
            });
        });
    </script>
</asp:Content>