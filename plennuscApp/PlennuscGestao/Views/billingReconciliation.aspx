<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" AutoEventWireup="true" CodeBehind="billingReconciliation.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.billingReconciliation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/Css/projects/gestao/structuresCss/billing/billingReconciliation.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div class="container-main">

    <div class="page-header">
        <h1 class="page-title">
            <span class="title-icon"><i class="bi bi-file-earmark-check"></i></span>
            Conferência de Faturamento
        </h1>
    </div>

    <!-- CARD 1: IMPORTAÇÃO -->
    <div class="filters-card" id="divImportacao" runat="server">
        <div class="filters-title"><i class="bi bi-cloud-upload"></i> Dados da Importação</div>

        <div class="form-row">
            <div class="form-group">
                <label for="ddlOperadora" class="form-label">Operadora *</label>
                <asp:DropDownList ID="ddlOperadora" runat="server" CssClass="form-control" onchange="mostrarImportacao(this)" />
            </div>
            <div class="form-group">
                <label for="txtMesAnoReferencia" class="form-label">Mês/Ano Referência *</label>
                <asp:TextBox ID="txtMesAnoReferencia" runat="server" CssClass="form-control"
                    MaxLength="7" placeholder="MM/AAAA" onkeyup="mascararMesAno(this)" />
            </div>
            <div class="form-group">
                <label class="form-label">Vigência (opcional)</label>
                <div class="custom-dropdown" id="dropdownGrupoFaturamento">
                    <div class="dropdown-header" onclick="toggleDropdown(this)">
                        <span class="resumo-texto" id="lblGrupoFaturamentoResumo">
                            <span class="placeholder">Selecione...</span>
                        </span>
                        <span class="arrow">▼</span>
                    </div>
                    <div class="dropdown-panel">
                        <asp:CheckBoxList ID="cblGrupoFaturamento" runat="server" ClientIDMode="Static" />
                    </div>
                </div>
            </div>
        </div>

        <div class="form-group hidden" id="divImportar" style="max-width:none; margin-top:20px; padding-top:20px; border-top:1px solid var(--gray-200);">
            <label class="form-label">Importar Relatório da Operadora (Excel) *</label>
            <div class="form-row" style="align-items:center;">
                <div class="form-group" style="max-width:500px;">
                    <asp:FileUpload ID="fileRelatorio" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group" style="max-width:none; flex:0;">
                    <asp:Button ID="btnImportar" runat="server" Text="Importar" CssClass="btn btn-primary" OnClick="btnImportar_Click" />
                </div>
            </div>
            <asp:Label ID="lblMensagemImportacao" runat="server" CssClass="msg-importacao" />
        </div>
    </div>

    <!-- CARD 2: CONFERÊNCIA -->
    <div class="filters-card hidden" id="divPreview" runat="server">
        <div class="filters-title"><i class="bi bi-search"></i> Conferência com a Operadora</div>

        <div class="banner-ajuda-conferencia">
            <i class="bi bi-info-circle"></i>
            <div>
                <strong>Convênio:</strong> confere o plano de saúde principal.
                &nbsp;&nbsp;
                <strong>Odontológico:</strong> confere valores de evento adicional/odontológico — use quando o arquivo importado for o relatório odontológico separado.
            </div>
        </div>

        <div class="secao-tipo-conferencia">
            <div>
                <label class="form-label" style="margin-bottom:8px;">Tipo de Conferência *</label>
                <asp:RadioButtonList ID="rblTipoConferencia" runat="server" RepeatDirection="Horizontal"
                    RepeatLayout="Flow" CssClass="radio-tipo-conferencia">
                    <asp:ListItem Text="Convênio" Value="CONVENIO" Selected="True" />
                    <asp:ListItem Text="Odontológico (Evento Adicional)" Value="EVENTO_ADICIONAL" />
                </asp:RadioButtonList>
            </div>
            <div class="acoes-conferencia">
                <asp:Button ID="btnConferir" runat="server" Text="Conferir com a Operadora" CssClass="btn btn-primary" OnClick="btnConferir_Click" />
                <asp:Button ID="btnExportarDivergentes" runat="server" Text="Exportar Divergentes (Excel)" CssClass="btn btn-secondary" OnClick="btnExportarDivergentes_Click" />
            </div>
        </div>

        <asp:Label ID="lblMensagemConferencia" runat="server" CssClass="msg-importacao" />

        <div class="grid-container" style="margin-top:16px;">
            <asp:GridView ID="gridPreview" runat="server" CssClass="custom-grid" AutoGenerateColumns="false"
                EmptyDataText="Nenhum registro encontrado." GridLines="None" OnRowDataBound="gridPreview_RowDataBound">
                <Columns>
                    <asp:BoundField DataField="Empresa" HeaderText="Empresa" />
                    <asp:BoundField DataField="Unidade" HeaderText="Unidade" />
                    <asp:BoundField DataField="NomeUnidade" HeaderText="Nome Unidade" />
                    <asp:BoundField DataField="Matricula" HeaderText="Matrícula" />
                    <asp:BoundField DataField="Cpf" HeaderText="CPF" />
                    <asp:BoundField DataField="Beneficiario" HeaderText="Beneficiário" />
                    <asp:BoundField DataField="Nascimento" HeaderText="Nascimento" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="Parentesco" HeaderText="Parentesco" />
                    <asp:BoundField DataField="Plano" HeaderText="Plano" />
                    <asp:BoundField DataField="Mensalidade" HeaderText="Mensalidade" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="MesAnoReferencia" HeaderText="Mês/Ano Usado" />
                    <asp:BoundField DataField="Cobrado" HeaderText="Cobrado" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="ValorOperadoraView" HeaderText="Valor Operadora" DataFormatString="{0:N2}" />
                    <asp:BoundField DataField="DiferencaValor" HeaderText="Diferença" DataFormatString="{0:N2}" />
                    <asp:TemplateField HeaderText="Status">
                        <ItemTemplate>
                            <span class='badge status-<%# ((string)Eval("StatusConferencia") ?? "").ToLower().Replace("_", "-") %>'>
                                <%# TraduzirStatus((string)Eval("StatusConferencia")) %>
                            </span>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </div>

</div>

<script>
    function mostrarImportacao(ddl) {
        var divImportar = document.getElementById('divImportar');
        if (ddl.value && ddl.value !== '') divImportar.classList.remove('hidden');
        else divImportar.classList.add('hidden');
    }
    function toggleDropdown(header) {
        var wrapper = header.parentElement;
        var isOpen = wrapper.classList.contains('open');
        document.querySelectorAll('.custom-dropdown.open').forEach(function (w) {
            if (w !== wrapper) w.classList.remove('open');
        });
        wrapper.classList.toggle('open', !isOpen);
    }
    document.addEventListener('click', function (e) {
        document.querySelectorAll('.custom-dropdown').forEach(function (wrapper) {
            if (!wrapper.contains(e.target)) wrapper.classList.remove('open');
        });
    });
    function atualizarResumoGrupoFaturamento() {
        var checkboxes = document.querySelectorAll('#cblGrupoFaturamento input[type="checkbox"]:checked');
        var label = document.getElementById('lblGrupoFaturamentoResumo');
        label.innerHTML = '';
        if (checkboxes.length === 0) {
            label.innerHTML = '<span class="placeholder">Selecione...</span>';
            return;
        }
        checkboxes.forEach(function (chk) {
            var nome = chk.closest('td').textContent.trim();
            var tag = document.createElement('span');
            tag.className = 'tag-item';
            tag.textContent = nome;
            label.appendChild(tag);
        });
    }
    document.addEventListener('DOMContentLoaded', function () {
        var painel = document.getElementById('cblGrupoFaturamento');
        if (painel) {
            painel.addEventListener('change', function (e) {
                if (e.target && e.target.type === 'checkbox') atualizarResumoGrupoFaturamento();
            });
        }
        atualizarResumoGrupoFaturamento();
        var ddl = document.getElementById('<%= ddlOperadora.ClientID %>');
        if (ddl) mostrarImportacao(ddl);
    });
    function mascararMesAno(input) {
        var valor = input.value.replace(/\D/g, '');
        if (valor.length > 6) valor = valor.substring(0, 6);
        if (valor.length >= 3) valor = valor.substring(0, 2) + '/' + valor.substring(2);
        input.value = valor;
    }
</script>
</asp:Content>