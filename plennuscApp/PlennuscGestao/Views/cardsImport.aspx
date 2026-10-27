<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" 
    AutoEventWireup="true" CodeBehind="cardsImport.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.CardsImport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/Css/projects/gestao/structuresCss/CIDs/ICDImport.css?v=2" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">

        <div class="page-header">
            <h2 class="page-title">
                <span class="title-icon"><i class="bi bi-card-checklist"></i></span>
                Importação de Carteirinhas
            </h2>
        </div>

        <div class="filters-card">
            <div class="filters-title">
                <i class="fa fa-upload"></i> Importar Arquivo
            </div>

            <div class="form-row">
                <div class="form-group">
                    <label class="form-label">Arquivo Excel <span class="icd-required">*</span></label>
                    <asp:FileUpload ID="fileUploadExcel" runat="server" CssClass="form-control" />
                    <asp:RequiredFieldValidator ID="rfvArquivo" runat="server"
                        ControlToValidate="fileUploadExcel"
                        ErrorMessage="Selecione um arquivo Excel."
                        CssClass="msg-importacao erro" Display="Dynamic" />
                </div>

                <div class="form-group form-group-btn">
                    <span id="wrapBtnImportar" style="position: relative; display: inline-block;">
                        <asp:Button ID="btnImportar" runat="server"
                            Text="Importar"
                            CssClass="btn btn-primary"
                            OnClick="btnImportar_Click"
                            OnClientClick="return mostrarLoadingImportacao();" />
                        <span id="spinnerImportar"
                              class="spinner-border spinner-border-sm"
                              role="status" aria-hidden="true"
                              style="display:none; position:absolute; top:50%; left:50%;
                                     width:1rem; height:1rem; border-width:.18em;
                                     margin:-0.5rem 0 0 -0.5rem; color:#fff;
                                     pointer-events:none; border-radius:50%;">
                        </span>
                    </span>
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
                <button type="button" class="tab-status-link" data-tab-target="tab-divergencia">
                    Divergência de Data <span class="tab-status-count"><asp:Literal ID="litCountDivergencia" runat="server" /></span>
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
                            <asp:DropDownList ID="ddlPageSize" runat="server" AutoPostBack="true"
                                OnSelectedIndexChanged="ddlPageSize_SelectedIndexChanged"
                                CssClass="form-control ddl-pagesize">
                                <asp:ListItem Text="5" Value="5" />
                                <asp:ListItem Text="10" Value="10" Selected="True" />
                                <asp:ListItem Text="20" Value="20" />
                                <asp:ListItem Text="50" Value="50" />
                                <asp:ListItem Text="100" Value="100" />
                            </asp:DropDownList>
                        </div>
                        <div class="grid-toolbar-right">
                            <asp:Button ID="btnExportarTodos" runat="server" Text="Exportar Excel"
                                CssClass="btn btn-success" OnClick="btnExportarTodos_Click"
                                CausesValidation="false" />
                        </div>
                    </div>

                    <div style="overflow-x: auto; width: 100%; margin-bottom: 15px;">
                        <asp:GridView ID="gridTodos" runat="server" AutoGenerateColumns="false" CssClass="custom-grid"
                            EmptyDataText="Nenhum registro processado."
                            OnRowDataBound="gridTodos_RowDataBound"
                            OnDataBound="gridTodos_DataBound"
                            AllowPaging="True" PageSize="10"
                            OnPageIndexChanging="gridTodos_PageIndexChanging"
                            EnableViewState="true" Style="min-width: 1200px;">
                            <PagerSettings Mode="NumericFirstLast" FirstPageText="«" LastPageText="»"
                                PageButtonCount="7" Position="Bottom" />
                            <PagerStyle CssClass="pager-custom" />
                            <Columns>
                                <asp:BoundField DataField="LinhaCsv" HeaderText="Linha" />
                                <asp:BoundField DataField="Cpf" HeaderText="CPF" />
                                <asp:BoundField DataField="NomeAssociado" HeaderText="Titular" />
                                <asp:BoundField DataField="NumeroCarteirinha" HeaderText="Nº Carteirinha" />
                                <asp:BoundField DataField="DataAdmissaoPlanilha" HeaderText="Admissão (Planilha)" />
                                <asp:BoundField DataField="DataAdmissaoBanco" HeaderText="Admissão (PS1000)" />
                                
                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate>
                                        <span class='badge <%# GetStatusCss(Eval("Sucesso"), Eval("Motivo")) %>'>
                                            <%# Eval("Sucesso").Equals(true) ? "Importado" : "Não Importado" %>
                                        </span>
                                    </ItemTemplate>
                                </asp:TemplateField>

                                <asp:BoundField DataField="Motivo" HeaderText="Motivo" ItemStyle-Wrap="false" />
                            </Columns>
                        </asp:GridView>
                    </div>

                    <asp:Label ID="lblPagerInfo" runat="server" CssClass="pager-info" />
                </div>
            </div>

            <div class="tab-status-panel" id="tab-importados">
                <div class="grid-container">
                    <div class="grid-toolbar">
                        <div class="grid-toolbar-right">
                            <asp:Button ID="btnExportarImportados" runat="server" Text="Exportar Excel"
                                CssClass="btn btn-success" OnClick="btnExportarImportados_Click"
                                CausesValidation="false" />
                        </div>
                    </div>

                    <div style="overflow-x: auto; width: 100%; margin-bottom: 15px;">
                        <asp:GridView ID="gridImportados" runat="server" AutoGenerateColumns="false" CssClass="custom-grid"
                            EmptyDataText="Nenhum registro importado." Style="min-width: 800px;">
                            <Columns>
                                <asp:BoundField DataField="LinhaCsv" HeaderText="Linha" />
                                <asp:BoundField DataField="Cpf" HeaderText="CPF" />
                                <asp:BoundField DataField="NomeAssociado" HeaderText="Titular" />
                                <asp:BoundField DataField="NumeroCarteirinha" HeaderText="Nº Carteirinha" />
                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate><span class="badge status-ok">Importado</span></ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>

            <div class="tab-status-panel" id="tab-divergencia">
                <div class="grid-container">
                    <div class="grid-toolbar">
                        <div class="grid-toolbar-right">
                            <asp:Button ID="btnExportarDivergencia" runat="server" Text="Exportar Excel"
                                CssClass="btn btn-success" OnClick="btnExportarDivergencia_Click"
                                CausesValidation="false" />
                        </div>
                    </div>

                    <div style="overflow-x: auto; width: 100%; margin-bottom: 15px;">
                        <asp:GridView ID="gridDivergencia" runat="server" AutoGenerateColumns="false" CssClass="custom-grid"
                            EmptyDataText="Nenhuma divergência de data encontrada." Style="min-width: 1000px;">
                            <Columns>
                                <asp:BoundField DataField="LinhaCsv" HeaderText="Linha" />
                                <asp:BoundField DataField="Cpf" HeaderText="CPF" />
                                <asp:BoundField DataField="NomeAssociado" HeaderText="Titular" />
                                <asp:BoundField DataField="DataAdmissaoPlanilha" HeaderText="Admissão (Planilha)" />
                                <asp:BoundField DataField="DataAdmissaoBanco" HeaderText="Admissão (PS1000)" />
                                <asp:BoundField DataField="Motivo" HeaderText="Motivo" ItemStyle-Wrap="false" />
                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate><span class="badge status-divergencia-tolerada">Divergência</span></ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>

            <div class="tab-status-panel" id="tab-nao-encontrado">
                <div class="grid-container">
                    <div class="grid-toolbar">
                        <div class="grid-toolbar-right">
                            <asp:Button ID="btnExportarNaoEncontrado" runat="server" Text="Exportar Excel"
                                CssClass="btn btn-success" OnClick="btnExportarNaoEncontrado_Click"
                                CausesValidation="false" />
                        </div>
                    </div>

                    <div style="overflow-x: auto; width: 100%; margin-bottom: 15px;">
                        <asp:GridView ID="gridNaoEncontrado" runat="server" AutoGenerateColumns="false" CssClass="custom-grid"
                            EmptyDataText="Nenhum CPF não encontrado." Style="min-width: 800px;">
                            <Columns>
                                <asp:BoundField DataField="LinhaCsv" HeaderText="Linha" />
                                <asp:BoundField DataField="Cpf" HeaderText="CPF" />
                                <asp:BoundField DataField="Motivo" HeaderText="Motivo" ItemStyle-Wrap="false" />
                                <asp:TemplateField HeaderText="Status">
                                    <ItemTemplate><span class="badge status-nao-encontrado">Não Encontrado</span></ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
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

    <script type="text/javascript">
        // @ts-nocheck
        function mostrarLoadingImportacao() {
            if (typeof Page_ClientValidate === 'function') {
                if (!Page_ClientValidate('')) {
                    return false;
                }
            }

            var fu = document.getElementById('<%= fileUploadExcel.ClientID %>');
            if (fu && (!fu.value || fu.value.length === 0)) {
                alert('Selecione um arquivo Excel.');
                return false;
            }

            var btn = document.getElementById('<%= btnImportar.ClientID %>');
            if (btn) {
                btn.style.setProperty('color', 'transparent', 'important');
                btn.style.setProperty('pointer-events', 'none', 'important');
            }

            var spinner = document.getElementById('spinnerImportar');
            if (spinner) {
                spinner.style.display = 'inline-block';
            }

            return true;
        }
    </script>
</asp:Content>