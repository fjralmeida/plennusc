<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" AutoEventWireup="true" CodeBehind="activityReport.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.activityReport" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/Css/projects/gestao/structuresCss/CIDs/ReportActivity.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- CARD 1: IMPORTAÇÃO -->
    <div class="filters-card" id="divImportacao" runat="server">
        <div class="filters-title"><i class="bi bi-cloud-upload"></i>Dados da Importação</div>
        <div class="form-row form-row-inline">
            <div class="form-group">
                <label for="ddlOperadora" class="form-label">Operadora *</label>
                <asp:DropDownList ID="ddlOperadora" runat="server" CssClass="form-control" onchange="mostrarImportacao(this)" />
            </div>
            <div class="form-group">
                <label class="form-label">Vigência <span class="icd-required">*</span></label>
                <asp:TextBox ID="txtVigencia" runat="server" TextMode="Date" CssClass="form-control" />
                <asp:RequiredFieldValidator ID="rfvVigencia" runat="server"
                    ControlToValidate="txtVigencia"
                    ErrorMessage="Campo Vigência é obrigatório."
                    CssClass="msg-importacao erro" Display="Dynamic" />
            </div>
            <div class="form-group form-group-btn">
                <asp:Button ID="btnRelatorioMovimentacao" runat="server" Text="Gerar Relatório" CssClass="btn btn-primary" OnClick="btnRelatorioMovimentacao_Click" />
            </div>
        </div>
    </div>

    <!-- MENSAGEM QUANDO NÃO HÁ RESULTADOS -->
    <asp:Label ID="lblMensagem" runat="server" CssClass="msg-importacao erro" Visible="false" />

    <!-- CARD 2: RESULTADO DO RELATÓRIO -->
    <div class="filters-card" id="divResultado" runat="server" visible="false">
        <div class="filters-title"><i class="bi bi-table"></i>Resultado</div>

<div class="grid-resumo">
    <span class="grid-resumo-total">
        <strong><asp:Literal ID="litTotalRegistros" runat="server" /></strong> registro(s) encontrado(s)
    </span>
    <asp:Button ID="btnExportarExcel" runat="server" Text="Exportar Excel" 
        CssClass="btn btn-success" OnClick="btnExportarExcel_Click" />
</div>
        <div class="grid-container">
            <div class="grid-scroll">
                <asp:GridView ID="gvRelatorio" runat="server" CssClass="custom-grid"
                    AutoGenerateColumns="false" AllowPaging="false" PageSize="30"
                    OnPageIndexChanging="gvRelatorio_PageIndexChanging"
                    EmptyDataText="Nenhum registro encontrado."
                    PagerStyle-CssClass="grid-pager"
                    UseAccessibleHeader="true">
                    <Columns>
                        <asp:BoundField DataField="Modalidade" HeaderText="Modalidade" ItemStyle-CssClass="cel-curta" />
                        <asp:BoundField DataField="Plano" HeaderText="Plano" ItemStyle-CssClass="cel-curta" />
                        <asp:BoundField DataField="Entidade" HeaderText="Entidade" ItemStyle-CssClass="cel-normal" />
                        <asp:BoundField DataField="Titular" HeaderText="Titular" ItemStyle-CssClass="cel-nome" />
                        <asp:BoundField DataField="Nome" HeaderText="Nome" ItemStyle-CssClass="cel-nome" />

                        <asp:TemplateField HeaderText="Tipo">
                            <ItemStyle CssClass="cel-curta" />
                            <ItemTemplate>
                                <span class='badge-tipo <%# Eval("Tipo")?.ToString() == "T" ? "tipo-t" : "tipo-d" %>'>
                                    <%# Eval("Tipo") %>
                                </span>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:BoundField DataField="Parentesco" HeaderText="Parentesco" ItemStyle-CssClass="cel-normal" />
                        <asp:BoundField DataField="EstadoCivil" HeaderText="Estado Civil" ItemStyle-CssClass="cel-normal" />
                        <asp:BoundField DataField="Sexo" HeaderText="Sexo" ItemStyle-CssClass="cel-curta" />
                        <asp:BoundField DataField="DataNascimento" HeaderText="Data Nasc." DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="cel-curta" />
                        <asp:BoundField DataField="Cpf" HeaderText="CPF" ItemStyle-CssClass="cel-normal" />
                        <asp:BoundField DataField="Cns" HeaderText="CNS" ItemStyle-CssClass="cel-normal" />
                        <asp:BoundField DataField="Telefone" HeaderText="Telefone" ItemStyle-CssClass="cel-normal" />
                        <asp:BoundField DataField="Endereco" HeaderText="Endereço" ItemStyle-CssClass="cel-normal" />
                        <asp:BoundField DataField="Complemento" HeaderText="Complemento" ItemStyle-CssClass="cel-curta" />
                        <asp:BoundField DataField="Bairro" HeaderText="Bairro" ItemStyle-CssClass="cel-normal" />
                        <asp:BoundField DataField="Cep" HeaderText="CEP" ItemStyle-CssClass="cel-curta" />
                        <asp:BoundField DataField="Cidade" HeaderText="Cidade" ItemStyle-CssClass="cel-normal" />
                        <asp:BoundField DataField="Estado" HeaderText="Estado" ItemStyle-CssClass="cel-curta" />
                        <asp:BoundField DataField="DtVigencia" HeaderText="Dt. Vigência" DataFormatString="{0:dd/MM/yyyy}" ItemStyle-CssClass="cel-curta" />
                        <asp:BoundField DataField="Email" HeaderText="Email" ItemStyle-CssClass="cel-email" />
                        <asp:BoundField DataField="Filiacao1" HeaderText="Filiação 1" ItemStyle-CssClass="cel-normal" />
                        <asp:BoundField DataField="Filiacao2" HeaderText="Filiação 2" ItemStyle-CssClass="cel-normal" />
                        <asp:BoundField DataField="NomenclaturaCarencia" HeaderText="Nomenclatura da Carência" ItemStyle-CssClass="cel-normal" />

                        <asp:TemplateField HeaderText="CID">
                            <ItemStyle CssClass="cel-cid" />
                            <ItemTemplate>
                                <div class="cid-truncado" title='<%# Eval("Cid") %>'>
                                    <%# Eval("Cid") %>
                                </div>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
