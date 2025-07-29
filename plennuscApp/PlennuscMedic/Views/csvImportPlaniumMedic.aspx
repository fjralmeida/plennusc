<%@ Page Title="" Language="C#" MasterPageFile="~/PlennuscMedic/Views/Masters/Index.Master" AutoEventWireup="true" CodeBehind="csvImportPlaniumMedic.aspx.cs" Inherits="appWhatsapp.PlennuscMedic.Views.csvImportPlaniumMedic"  %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

     <link href="https://fonts.googleapis.com/css2?family=Poppins:wght@300;400;500;600&display=swap" rel="stylesheet">
 <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />

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
             <asp:GridView ID="gridCsv" runat="server" AutoGenerateColumns="False"
    CssClass="table table-striped table-bordered shadow-sm rounded mb-0">
    <Columns>
        <asp:BoundField DataField="NOME" HeaderText="Nome" />
        <asp:BoundField DataField="ID" HeaderText="ID" />
        <asp:BoundField DataField="ORG_EXP" HeaderText="Orgão Exp." />
        <asp:BoundField DataField="CPF" HeaderText="CPF" />
        <asp:BoundField DataField="PIS" HeaderText="PIS" />
        <asp:BoundField DataField="CNS" HeaderText="CNS" />
        <asp:BoundField DataField="DT_NASC" HeaderText="Data de Nascimento" />
        <asp:BoundField DataField="NATURALIDADE" HeaderText="Naturalidade" />
        <asp:BoundField DataField="SEXO" HeaderText="Sexo" />
        <asp:BoundField DataField="EST_CIVIL" HeaderText="Estado Civil" />
        <asp:BoundField DataField="TIPO_LOGRADOURO" HeaderText="Tipo Logradouro" />
        <asp:BoundField DataField="RUA" HeaderText="Rua" />
        <asp:BoundField DataField="NUMERO_LOGRADOURO" HeaderText="Número" />
        <asp:BoundField DataField="COMPLEMENTO" HeaderText="Complemento" />
        <asp:BoundField DataField="BAIRRO" HeaderText="Bairro" />
        <asp:BoundField DataField="CIDADE" HeaderText="Cidade" />
        <asp:BoundField DataField="UF" HeaderText="UF" />
        <asp:BoundField DataField="CEP" HeaderText="CEP" />
        <asp:BoundField DataField="DDD_TEL" HeaderText="DDD Tel" />
        <asp:BoundField DataField="TEL" HeaderText="Telefone" />
        <asp:BoundField DataField="DDD_TEL_2" HeaderText="DDD Tel 2" />
        <asp:BoundField DataField="TEL_2" HeaderText="Telefone 2" />
        <asp:BoundField DataField="DDD_CEL" HeaderText="DDD Cel" />
        <asp:BoundField DataField="CELULAR" HeaderText="Celular" />
        <asp:BoundField DataField="COD_PLANO" HeaderText="Código Plano" />
        <asp:BoundField DataField="DT_INCL" HeaderText="Data Inclusão" />
        <asp:BoundField DataField="DT_VIGENCIA" HeaderText="Data Vigência" />
        <asp:BoundField DataField="COD_EMPRESA" HeaderText="Código Empresa" />
        <asp:BoundField DataField="COD_UNID" HeaderText="Código Unidade" />
        <asp:BoundField DataField="MAT" HeaderText="Matrícula" />
        <asp:BoundField DataField="ADMISSAO" HeaderText="Admissão" />
        <asp:BoundField DataField="NOME_MAE" HeaderText="Nome Mãe" />
        <asp:BoundField DataField="NOME_PAI" HeaderText="Nome Pai" />
        <asp:BoundField DataField="EMAIL" HeaderText="Email" />
        <asp:BoundField DataField="COD_RESP" HeaderText="Código Responsável" />
        <asp:BoundField DataField="PARENT" HeaderText="Parentesco" />
        <asp:BoundField DataField="UNIVER" HeaderText="Universitário" />
        <asp:BoundField DataField="NR_DEC_NASC_VIVO" HeaderText="Declaração Nascimento Vivo" />
        <asp:BoundField DataField="AGREGADO" HeaderText="Agregado" />
        <asp:BoundField DataField="DEF_INVALIDO" HeaderText="Deficiente" />
        <asp:BoundField DataField="COD_LOTACAO" HeaderText="Código Lotação" />
        <asp:BoundField DataField="TIPO_MOVIMENTACAO" HeaderText="Tipo Movimentação" />
        <asp:BoundField DataField="DATA_EXCLUSAO" HeaderText="Data Exclusão" />
        <asp:BoundField DataField="MOTIVO_EXCLUSAO" HeaderText="Motivo Exclusão" />
        <asp:BoundField DataField="COD_OUTRO" HeaderText="Código Outro" />
        <asp:BoundField DataField="COD_GSEG" HeaderText="Código GSEG" />
        <asp:BoundField DataField="COD_VEND" HeaderText="Código Vendedor" />
        <asp:BoundField DataField="COD_PROP" HeaderText="Código Proposta" />
        <asp:BoundField DataField="OBS" HeaderText="Observação" />
        <asp:BoundField DataField="OBS_TEC" HeaderText="Observação Técnica" />
        <asp:BoundField DataField="MOSTRA_LIB" HeaderText="Mostra Liberação" />
        <asp:BoundField DataField="COD_FORMA" HeaderText="Código Forma" />
        <asp:BoundField DataField="DIA_VENC" HeaderText="Dia Vencimento" />
        <asp:BoundField DataField="COD_TABCOM" HeaderText="Código Tabela Comissão" />
        <asp:BoundField DataField="CARGO" HeaderText="Cargo" />
        <asp:BoundField DataField="RESPONSAVEL" HeaderText="Responsável" />
        <asp:BoundField DataField="DT_CASAMENTO" HeaderText="Data Casamento" />
        <asp:BoundField DataField="UF_ORGAO" HeaderText="UF Órgão" />
        <asp:BoundField DataField="COD_GRP" HeaderText="Código Grupo" />
        <asp:BoundField DataField="COD_SUPERV" HeaderText="Código Supervisor" />
        <asp:BoundField DataField="COD_PROFISSAO" HeaderText="Código Profissão" />
        <asp:BoundField DataField="CNPJ_OPERADORA" HeaderText="CNPJ Operadora" />
        <asp:BoundField DataField="COD_ANS_OPERADORA" HeaderText="Código ANS Operadora" />
        <asp:BoundField DataField="NOME_OPERADORA" HeaderText="Nome Operadora" />
        <asp:BoundField DataField="IDADE" HeaderText="Idade" />
        <asp:BoundField DataField="RESPF_MAE" HeaderText="Resp. Mãe" />
        <asp:BoundField DataField="RESPF_NASCIMENTO" HeaderText="Resp. Nascimento" />
        <asp:BoundField DataField="RESPF_EMAIL" HeaderText="Resp. Email" />
        <asp:BoundField DataField="RESPF_IDADE" HeaderText="Resp. Idade" />
        <asp:BoundField DataField="RESPF_SEXO" HeaderText="Resp. Sexo" />
        <asp:BoundField DataField="RESPF_ESTADOCIVIL" HeaderText="Resp. Estado Civil" />
        <asp:BoundField DataField="RESPF_RG" HeaderText="Resp. RG" />
        <asp:BoundField DataField="RESPF_ORG_EXP" HeaderText="Resp. Org. Exp." />
        <asp:BoundField DataField="RESPF_CNS" HeaderText="Resp. CNS" />
        <asp:BoundField DataField="NOME_VENDEDOR" HeaderText="Nome Vendedor" />
        <asp:BoundField DataField="CPF_VENDEDOR" HeaderText="CPF Vendedor" />
        <asp:BoundField DataField="EMAIL_VENDEDOR" HeaderText="Email Vendedor" />
        <asp:BoundField DataField="TELEFONE_VENDEDOR" HeaderText="Telefone Vendedor" />
        <asp:BoundField DataField="MENSALIDADE_TIT" HeaderText="Mensalidade Titular" />
        <asp:BoundField DataField="MENSALIDADE_DEP" HeaderText="Mensalidade Dependente" />
        <asp:BoundField DataField="ACESSORIOS" HeaderText="Acessórios" />
    </Columns>
</asp:GridView>
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
