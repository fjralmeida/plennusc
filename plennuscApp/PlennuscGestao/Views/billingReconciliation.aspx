<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" AutoEventWireup="true" CodeBehind="billingReconciliation.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.billingReconciliation" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/Css/projects/gestao/structuresCss/billing/billingReconciliation.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<div class="form-row">
    <!-- OPERADORA -->
    <div class="form-group">
        <label for="ddlOperadora">Operadora *</label>
        <asp:DropDownList ID="ddlOperadora" runat="server" CssClass="form-control" onchange="mostrarImportacao(this)">
        </asp:DropDownList>
    </div>

    <!-- MÊS/ANO REFERÊNCIA -->
    <div class="form-group">
        <label for="txtMesAnoReferencia">Mês/Ano Referência *</label>
        <asp:TextBox ID="txtMesAnoReferencia" runat="server" CssClass="form-control"
            MaxLength="7" placeholder="MM/AAAA" onkeyup="mascararMesAno(this)" />
    </div>

    <!-- GRUPO DE FATURAMENTO -->
    <div class="form-group">
        <label class="field-label">Vigência (opcional)</label>
        <div class="custom-dropdown" id="dropdownGrupoFaturamento">
            <div class="dropdown-header" onclick="toggleDropdown(this)">
                <span class="resumo-texto" id="lblGrupoFaturamentoResumo">
                    <span class="placeholder">Selecione...</span>
                </span>
                <span class="arrow">▼</span>
            </div>
            <div class="dropdown-panel">
                <asp:CheckBoxList ID="cblGrupoFaturamento" runat="server" ClientIDMode="Static">
                </asp:CheckBoxList>
            </div>
        </div>
    </div>
</div>

 <!-- IMPORTAÇÃO (só aparece depois de escolher a operadora) -->
    <div class="form-group hidden" id="divImportar">
        <label class="field-label">Importar Relatório da Operadora (Excel) *</label>
        <asp:FileUpload ID="fileRelatorio" runat="server" CssClass="form-control" />
        <asp:Button ID="btnImportar" runat="server" Text="Importar" CssClass="btn btn-primary"
            OnClick="btnImportar_Click" style="margin-top:10px;" />
        <asp:Label ID="lblMensagemImportacao" runat="server" CssClass="msg-importacao"></asp:Label>
    </div>

 <div class="form-group hidden" id="divPreview" runat="server">
    <label class="field-label">Pré-visualização dos dados importados</label>
    <asp:Button ID="btnConferir" runat="server" Text="Conferir com a Operadora"
        CssClass="btn btn-primary" OnClick="btnConferir_Click" style="margin-bottom:12px;" />

     <asp:Button ID="btnExportarDivergentes" runat="server" Text="Exportar Divergentes (Excel)"
    CssClass="btn btn-secondary" OnClick="btnExportarDivergentes_Click" style="margin-left:8px;" />

    <asp:Label ID="lblMensagemConferencia" runat="server" CssClass="msg-importacao"></asp:Label>
    <div class="grid-container">
        <asp:GridView ID="gridPreview" runat="server" CssClass="custom-grid" AutoGenerateColumns="false"
            EmptyDataText="Nenhum registro encontrado." GridLines="None"
            OnRowDataBound="gridPreview_RowDataBound">
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

 <script>
     function mostrarImportacao(ddl) {
         var divImportar = document.getElementById('divImportar');
         if (ddl.value && ddl.value !== '') {
             divImportar.classList.remove('hidden');
         } else {
             divImportar.classList.add('hidden');
         }
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
             if (!wrapper.contains(e.target)) {
                 wrapper.classList.remove('open');
             }
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
                 if (e.target && e.target.type === 'checkbox') {
                     atualizarResumoGrupoFaturamento();
                 }
             });
         }

         // Garante que o placeholder "Selecione..." apareça já no carregamento da página
         atualizarResumoGrupoFaturamento();

         var ddl = document.getElementById('<%= ddlOperadora.ClientID %>');
        if (ddl) mostrarImportacao(ddl);
     });

     function mascararMesAno(input) {
         var valor = input.value.replace(/\D/g, ''); // remove tudo que não é número

         if (valor.length > 6) valor = valor.substring(0, 6);

         if (valor.length >= 3) {
             valor = valor.substring(0, 2) + '/' + valor.substring(2);
         }

         input.value = valor;
     }
    </script>
</asp:Content>