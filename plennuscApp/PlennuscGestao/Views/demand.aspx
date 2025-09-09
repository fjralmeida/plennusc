<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="demand.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.demand" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
<link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css" rel="stylesheet">
<style>
:root {
  --brand-green:#4CB07A;
  --brand-green-600:#3B8B65;
  --ink:#2a2f3a;
  --muted:#6b7280;
  --bg:#f9fafb;
  --card:#fff;
  --radius:14px;
  --font:'Inter','Poppins',sans-serif;
  --shadow:0 4px 16px rgba(0,0,0,.05);
}
body {
  background:var(--bg);
  font-family:var(--font);
  font-size:14px;
  color:var(--ink);
}

/* Card geral */
.demand-card {
  background:var(--card);
  border-radius:var(--radius);
  box-shadow:var(--shadow);
  border:1px solid #eceff3;
  overflow:hidden;
  margin:auto;
  max-width:1100px;
}

/* Cabeçalho */
.demand-head {
  padding:16px 22px;
  border-bottom:1px solid #eef2f7;
  display:flex;
  align-items:center;
  justify-content:space-between;
  background:#fff;
}
.demand-title {
  font-size:1rem;
  font-weight:600;
  color:var(--ink);
  display:flex;
  gap:10px;
  align-items:center;
}
.demand-title .ico {
  background:var(--brand-green);
  color:#fff;
  border-radius:10px;
  width:34px;height:34px;
  display:grid;place-items:center;
  font-size:16px;
}
.btn-save {
  background:var(--brand-green);
  border:none;
  color:#fff;
  font-weight:600;
  padding:8px 18px;
  border-radius:8px;
  transition:.2s;
}
.btn-save:hover {
  background:var(--brand-green-600);
}

/* Body */
.demand-body { padding:22px; }

/* Seções */
.section-title {
  font-size:.8rem;
  font-weight:600;
  color:var(--muted);
  margin-bottom:10px;
  text-transform:uppercase;
  letter-spacing:.5px;
}

/* Inputs */
.form-label {
  font-size:.8rem;
  color:var(--muted);
  margin-bottom:4px;
  font-weight:500;
}
.form-control,.form-select {
  border-radius:10px;
  border:1px solid #e5e7eb;
  font-size:.9rem;
  padding:.55rem .75rem;
}
.form-control:focus,.form-select:focus {
  border-color:var(--brand-green);
  box-shadow:0 0 0 2px rgba(76,176,122,.25);
}
textarea.autosize { min-height:120px; resize:none; }

/* Hints */
.input-hint { font-size:.75rem;color:var(--muted); }
.counter { font-size:.75rem;color:var(--muted); }

/*AJUSTE PARA CHECK-BOX CORRIGIR SOMBRA QUE ESTA DANDO BUG */
.form-check-input {
    margin: 2 !important;
    height: 0px;
    display: contents;
}
.mt-4 { margin-top:0.5rem !important; }

</style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="demand-card">
  <div class="demand-head">
    <h4 class="demand-title">
      <span class="ico"><i class="bi bi-clipboard-plus"></i></span>
      Criar Demanda
    </h4>
    <asp:Button ID="btnSalvar" runat="server" Text="Salvar" CssClass="btn-save" OnClick="btnSalvar_Click" />
  </div>

  <div class="demand-body">

    <!-- Mensagens -->
    <asp:Label ID="lblMsg" runat="server" CssClass="msg-area d-block my-3" EnableViewState="false" />

    <!-- Objetivo -->
    <div class="mb-4">
      <h6 class="section-title">Objetivo</h6>
      <div class="row g-3">
        <div class="col-lg-6">
          <label class="form-label">Título *</label>
          <asp:TextBox ID="txtTitulo" runat="server" CssClass="form-control" MaxLength="100"
            placeholder="Um resumo claro do que precisa ser feito..." />
          <div class="d-flex justify-content-between mt-1">
            <small class="input-hint">Máx. 100 caracteres</small>
            <small class="counter" id="cntTitulo">0/100</small>
          </div>
        </div>
        <div class="col-lg-3">
          <label class="form-label">Prioridade *</label>
          <asp:DropDownList ID="ddlPrioridade" runat="server" CssClass="form-select" />
        </div>
        <div class="col-lg-3">
          <label class="form-label">Prazo</label>
          <input type="date" id="txtPrazo" runat="server" class="form-control" />
        </div>
      </div>
    </div>

    <!-- Roteamento -->
    <div class="mb-4">
      <h6 class="section-title">Roteamento</h6>
      <div class="row g-3">
        <div class="col-lg-6">
          <label class="form-label">Setor de Origem *</label>
          <asp:DropDownList ID="ddlOrigem" runat="server" CssClass="form-select" />
        </div>
        <div class="col-lg-6">
          <label class="form-label">Setor de Destino *</label>
          <asp:DropDownList ID="ddlDestino" runat="server" CssClass="form-select" 
            AutoPostBack="true" 
            OnSelectedIndexChanged="ddlDestino_SelectedIndexChanged" />
        </div>
      </div>
    </div>

   <!-- Categoria -->
<div class="mb-4">
  <h6 class="section-title">Categoria</h6>
  <div class="row g-3">
    <div class="col-lg-6">
      <label class="form-label">Categoria *</label>
      <!-- ADICIONE AutoPostBack="true" E OnSelectedIndexChanged AQUI -->
      <asp:DropDownList ID="ddlTipoGrupo" runat="server" CssClass="form-select" 
          AutoPostBack="true" 
          OnSelectedIndexChanged="ddlTipoGrupo_SelectedIndexChanged" />
      <small class="input-hint">Ex.: “Tecnologia - Suporte”.</small>
    </div>
    <div class="col-lg-6">
      <label class="form-label">Subtipo</label>
      <asp:DropDownList ID="ddlTipoDetalhe" runat="server" CssClass="form-select" />
      <small class="input-hint">Ex.: “Criação Cx. Postal (email)”.</small>
    </div>
  </div>
</div>

    <!-- Aprovação -->
    <div class="mb-4">
      <h6 class="section-title">Aprovação</h6>
      <div class="d-flex align-items-center gap-3">
        <asp:CheckBox ID="chkAprova" runat="server" CssClass="form-check-input" />
        <label class="form-label mb-0">Requer aprovação</label>
        <asp:DropDownList ID="ddlAprovador" runat="server" CssClass="form-select w-50" Enabled="false" />
      </div>
    </div>

    <!-- Detalhes -->
    <div>
      <h6 class="section-title">Detalhes</h6>
      <label class="form-label">Descrição *</label>
      <asp:TextBox ID="txtDescricao" runat="server" CssClass="form-control autosize" TextMode="MultiLine"
        placeholder="Descreva o contexto, o que precisa ser feito e critérios de aceitação..." />
      <div class="d-flex justify-content-between mt-1">
        <small class="input-hint">Dica: detalhe o objetivo, anexos e responsáveis.</small>
        <small class="counter" id="cntDesc">0</small>
      </div>
    </div>

  </div>
</div>

<script>
    (function () {
        var chk = document.getElementById('<%= chkAprova.ClientID %>');
      var ddlApr = document.getElementById('<%= ddlAprovador.ClientID %>');
    var titulo = document.getElementById('<%= txtTitulo.ClientID %>');
      var desc = document.getElementById('<%= txtDescricao.ClientID %>');
      var cntTitulo = document.getElementById('cntTitulo');
      var cntDesc = document.getElementById('cntDesc');

      function toggleAprovador() {
          var on = chk.checked;
          ddlApr.disabled = !on;
          if (!on) ddlApr.selectedIndex = 0;

          ddlApr.style.display = on ? 'block' : 'none';
      }

      if (chk) {
          chk.addEventListener('change', toggleAprovador);
          toggleAprovador();
      }

      function updateCounters() {
          if (titulo && cntTitulo) cntTitulo.textContent = (titulo.value || '').length + "/100";
          if (desc && cntDesc) cntDesc.textContent = (desc.value || '').length + " caracteres";
      }

      function autoSize(el) {
          el.style.height = 'auto';
          el.style.height = (el.scrollHeight + 2) + 'px';
      }

      if (desc) {
          desc.addEventListener('input', function () { autoSize(desc); updateCounters(); });
          setTimeout(function () { autoSize(desc); updateCounters(); }, 0);
      }
      if (titulo) {
          titulo.addEventListener('input', updateCounters);
          setTimeout(updateCounters, 0);
      }
  })();
</script>
</asp:Content>