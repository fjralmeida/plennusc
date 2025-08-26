<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="demand.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.demand" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <style>
    :root{
      --brand-green:#4CB07A;
      --brand-green-600:#3B8B65;
      --brand-blue:#83CEEE;
      --ink:#2a2f3a;
      --muted:#6b7280;
      --bg:#f5f7fb;
      --card:#fff;
      --ring:rgba(76,176,122,.28);
      --shadow:0 10px 24px rgba(28,35,45,.06),0 2px 6px rgba(28,35,45,.04);
      --radius:16px;
    }
    body{background:var(--bg);}
    .demand-card{background:var(--card);border:1px solid #edf0f5;border-radius:var(--radius);box-shadow:var(--shadow);}
    .demand-head{padding:18px 20px;border-bottom:1px solid #eef2f7;display:flex;align-items:center;justify-content:space-between;gap:12px;}
    .demand-title{display:flex;align-items:center;gap:12px;color:var(--ink);margin:0;font-weight:600;letter-spacing:.2px;}
    .demand-title .ico{width:40px;height:40px;border-radius:12px;display:grid;place-items:center;background:linear-gradient(135deg,var(--brand-blue),var(--brand-green));color:#fff;font-size:20px;box-shadow:0 6px 18px rgba(131,206,238,.35);}
    .demand-head .actions .btn-save{background:var(--brand-green);border:none;color:#fff;font-weight:600;padding:8px 16px;border-radius:10px;box-shadow:0 8px 20px rgba(76,176,122,.25);}
    .demand-head .actions .btn-save:hover{background:var(--brand-green-600);}
    .demand-body{padding:20px;}
    .fieldset-title{font-size:.85rem;color:var(--muted);text-transform:uppercase;letter-spacing:.06em;margin:8px 0 12px;}
    .form-label{font-weight:600;color:#374151;}
    .form-control,.form-select{border-radius:10px;border:1px solid #e6e9f0;}
    .form-control:focus,.form-select:focus{border-color:var(--brand-green)!important;box-shadow:0 0 0 .2rem var(--ring)!important;}
    .input-hint{color:var(--muted);font-size:.8rem;}
    .autosize{min-height:160px;resize:vertical;}
    .counter{font-size:.78rem;color:var(--muted);}
    .msg-area{margin:0 20px;padding:10px 12px;border-radius:10px;}
    .msg-area.text-danger{background:#feecec;color:#b4232a;border:1px solid #ffd3d6;}
    .msg-area.text-success{background:#effaf4;color:#1e6b46;border:1px solid #cfeee0;}
    .steps{display:flex;gap:10px;flex-wrap:wrap;padding:10px 20px 0;}
    .chip{display:inline-flex;align-items:center;gap:8px;padding:6px 10px;background:#f6faff;color:#2f3b4a;border:1px solid #eaf2fb;border-radius:999px;font-size:.82rem;font-weight:600;}
    .chip i{color:var(--brand-blue);}
    @media (max-width:991px){.demand-head{flex-direction:column;align-items:stretch}.demand-head .actions{display:flex;justify-content:flex-end}}

    /* ESCOPADO PARA ESTA PÁGINA */
    #demanda-page input.form-check-input[type="checkbox"]{
      width:18px!important;height:18px!important;border:1px solid #cbd5e1!important;border-radius:4px!important;
      background-color:#fff!important;background-image:none!important;box-shadow:none!important;outline:none!important;
      appearance:auto!important;-webkit-appearance:checkbox!important;-moz-appearance:checkbox!important;accent-color:#4CB07A;
    }
    #demanda-page input.form-check-input[type="checkbox"]:focus{box-shadow:none!important;outline:none!important;}
    #demanda-page input.form-check-input[type="checkbox"]:checked{background-color:#4CB07A!important;border-color:#4CB07A!important;}
          .form-check .form-check-input {
          margin: 2 !important;
          height: 0px;
          display: contents;
      }
.mt-4 {
    margin-top: 0.5rem !important;
}

    /* Removido: NADA de .form-check .form-check-input global aqui pra não quebrar outras páginas */
  </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div id="demanda-page"><!-- escopo do CSS -->

  <!-- Etapas (visual) -->
  <div class="steps">
    <span class="chip"><i class="bi bi-pencil-square"></i> Dados</span>
    <span class="chip"><i class="bi bi-diagram-3"></i> Roteamento</span>
    <span class="chip"><i class="bi bi-shield-check"></i> Aprovação</span>
    <span class="chip"><i class="bi bi-card-text"></i> Descrição</span>
  </div>

  <!-- Mensagens -->
  <asp:Label ID="lblMsg" runat="server" CssClass="msg-area d-block my-3" EnableViewState="false" />

  <!-- Card -->
  <div class="demand-card">
    <div class="demand-head">
      <h4 class="demand-title">
        <span class="ico"><i class="bi bi-envelope-paper"></i></span>
        Criar Demanda
      </h4>
      <div class="actions">
        <asp:Button ID="btnSalvar" runat="server" Text="Salvar" CssClass="btn btn-save" OnClick="btnSalvar_Click" />
      </div>
    </div>

    <div class="demand-body">
      <!-- Linha 1 -->
      <div class="row g-3">
        <div class="col-lg-6">
          <label for="txtTitulo" class="form-label">Título *</label>
          <asp:TextBox ID="txtTitulo" runat="server" CssClass="form-control" MaxLength="100" placeholder="Um resumo claro do que precisa ser feito..." />
          <div class="d-flex justify-content-between mt-1">
            <small class="input-hint">Máx. 100 caracteres</small>
            <small class="counter" id="cntTitulo">0/100</small>
          </div>
        </div>

        <div class="col-lg-3">
          <label for="ddlPrioridade" class="form-label">Prioridade *</label>
          <asp:DropDownList ID="ddlPrioridade" runat="server" CssClass="form-select"></asp:DropDownList>
        </div>

        <div class="col-lg-3">
          <label for="txtPrazo" class="form-label">Prazo</label>
          <input type="date" id="txtPrazo" runat="server" class="form-control" />
        </div>
      </div>

      <!-- Linha 2 -->
      <div class="row g-3 mt-1">
        <div class="col-lg-6">
          <label for="ddlOrigem" class="form-label">Setor de Origem *</label>
          <asp:DropDownList ID="ddlOrigem" runat="server" CssClass="form-select"></asp:DropDownList>
        </div>
        <div class="col-lg-6">
          <label for="ddlDestino" class="form-label">Setor de Destino *</label>
          <asp:DropDownList ID="ddlDestino" runat="server" CssClass="form-select"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlDestino_SelectedIndexChanged"></asp:DropDownList>
        </div>
      </div>

      <!-- Linha 3 -->
      <div class="row g-3 mt-1 align-items-end">
        <!-- Grupo (categoria) -->
        <div class="col-lg-4">
          <label for="ddlTipoGrupo" class="form-label">Categoria *</label>
          <asp:DropDownList ID="ddlTipoGrupo" runat="server" CssClass="form-select"
                            AutoPostBack="true" OnSelectedIndexChanged="ddlTipoGrupo_SelectedIndexChanged"></asp:DropDownList>
          <small class="input-hint">Ex.: “Tecnologia - Suporte”.</small>
        </div>

        <!-- Subtipo (detalhe) -->
        <div class="col-lg-4">
          <label for="ddlTipoDetalhe" class="form-label">Subtipo</label>
          <asp:DropDownList ID="ddlTipoDetalhe" runat="server" CssClass="form-select" />
          <small class="input-hint">Ex.: “Criação Cx. Postal (email)”.</small>
        </div>

        <!-- Aprovação -->
        <div class="col-lg-4 d-flex align-items-center gap-3">
          <div class="form-check m-0">
            <asp:CheckBox ID="chkAprova" runat="server" CssClass="form-check-input demand-cb" />
            <label class="form-check-label ms-2" for="chkAprova">Requer aprovação</label>
          </div>
          <div class="flex-grow-1">
            <label for="ddlAprovador" class="form-label">Aprovador (opcional)</label>
            <asp:DropDownList ID="ddlAprovador" runat="server" CssClass="form-select" Enabled="false" />
          </div>
        </div>
      </div>

      <!-- Linha 4 -->
      <div class="mt-3">
        <label for="txtDescricao" class="form-label">Descrição *</label>
        <asp:TextBox ID="txtDescricao" runat="server" CssClass="form-control autosize" TextMode="MultiLine" placeholder="Descreva o contexto, o que precisa ser feito e critérios de aceitação..." />
        <div class="d-flex justify-content-between mt-1">
          <small class="input-hint">Dica: detalhe o objetivo, anexos e responsáveis.</small>
          <small class="counter" id="cntDesc">0</small>
        </div>
      </div>
    </div>
  </div>

  <!-- JS da página -->
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

</div><!-- /#demanda-page -->
</asp:Content>