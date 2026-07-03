<%@ Page Title="Gerador de Propostas" Language="C#" MasterPageFile="~/PlennuscGestao/Views/Masters/IndexFrame.Master" AutoEventWireup="true" CodeBehind="Butyou.aspx.cs" Inherits="appWhatsapp.PlennuscGestao.Views.Butyou" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="../../Content/Css/projects/gestao/structuresCss/Migracoes/butYou.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div class="mg-wrap">

    <%-- ── Cabeçalho ── --%>
    <div class="mg-header">
        <div class="mg-header-icon">&#128196;</div>
        <div>
            <h1>Gerador de Propostas</h1>
            <p>Migra&ccedil;&atilde;o ANASERV &mdash; gera&ccedil;&atilde;o de documentos DOCX em lote</p>
        </div>
    </div>

    <%-- ── Mensagens ── --%>
    <asp:Panel ID="pnlMensagem" runat="server" Visible="false">
        <div class="mg-alert sucesso">
            <span class="mg-alert-icon">&#10004;</span>
            <asp:Label ID="lblMensagem" runat="server"></asp:Label>
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlErro" runat="server" Visible="false">
        <div class="mg-alert erro">
            <span class="mg-alert-icon">&#9888;</span>
            <asp:Label ID="lblErro" runat="server"></asp:Label>
        </div>
    </asp:Panel>

    <%-- ── Cards de ação ── --%>
    <div class="mg-grid">

        <%-- Card 1 — Dados Reais --%>
        <div class="mg-card">
            <div class="mg-card-ico blue">&#128209;</div>
            <p class="mg-card-title">Preencher com Dados Reais</p>
            <p class="mg-card-desc">
                Busca associados diretamente no banco Alian&ccedil;a e gera os documentos DOCX
                com titular e dependentes agrupados.
            </p>
            <asp:Button ID="btnPreencherComDadosReais" runat="server"
                Text="&#9654; Executar"
                OnClick="btnPreencherComDadosReais_Click"
                CssClass="mg-btn azul" />
        </div>

        <%-- Card 2 — CSV --%>
        <div class="mg-card">
            <div class="mg-card-ico green">&#128196;</div>
            <p class="mg-card-title">Preencher via CSV</p>
            <p class="mg-card-desc">
                L&ecirc; o arquivo <strong>AASP_MIGRACAO_UNIAOMED.csv</strong> e gera os
                documentos DOCX para cada titular encontrado.
            </p>
            <asp:Button ID="btnPreencherComDadosCsv" runat="server"
                Text="&#9654; Executar"
                OnClick="btnPreencherComDadosCsv_Click"
                CssClass="mg-btn verde" />
        </div>

        <%-- Card 3 — Termo de Reajuste --%>
        <div class="mg-card">
            <div class="mg-card-ico orange">&#128203;</div>
            <p class="mg-card-title">Termo de Reajuste Vallor</p>
            <p class="mg-card-desc">
                Gera o termo de reajuste Hapvida / Vallor a partir do arquivo
                <strong>AASP.csv</strong>, um documento por benefici&aacute;rio.
            </p>
            <asp:Button ID="trmReajusteVallor" runat="server"
                Text="&#9654; Executar"
                OnClick="trmReajusteVallor_Click"
                CssClass="mg-btn laranja" />
        </div>

    </div>

    <%-- ── Informações do projeto ── --%>
    <div class="mg-info-card">
        <p class="mg-info-title">
            <span class="ico">&#9432;</span> Informa&ccedil;&otilde;es do Projeto
        </p>
        <div class="mg-info-grid">
            <div class="mg-info-item">
                <span class="lbl">Template principal</span>
                <span class="val">MAIS VOCE - PE - DOCX.docx</span>
            </div>
            <div class="mg-info-item">
                <span class="lbl">Tabela de origem</span>
                <span class="val">ps1000</span>
            </div>
            <div class="mg-info-item">
                <span class="lbl">Total de benefici&aacute;rios</span>
                <span class="val">1.200 vidas</span>
            </div>
            <div class="mg-info-item">
                <span class="lbl">Sa&iacute;da</span>
                <span class="val">Arquivos DOCX individuais</span>
            </div>
            <div class="mg-info-item">
                <span class="lbl">Status</span>
                <span class="val"><span class="mg-badge-teste">&#9203; Em teste</span></span>
            </div>
        </div>
    </div>

    <%-- ── Instruções ── --%>
    <div class="mg-instrucoes">
        <p class="mg-instrucoes-title">&#10067; Como usar</p>
        <ol>
            <li>Escolha o processo desejado nos cards acima e clique em <strong>Executar</strong>.</li>
            <li>Aguarde o processamento &mdash; os documentos ser&atilde;o gerados automaticamente.</li>
            <li>Ao concluir, uma mensagem de sucesso indicar&aacute; a pasta de destino.</li>
            <li>Clique no link gerado para abrir a pasta com os arquivos DOCX.</li>
            <li>Em caso de erro, verifique os detalhes na mensagem vermelha acima.</li>
        </ol>
    </div>

    <%-- ── Panels de resultado ocultos ── --%>
    <asp:Panel ID="pnlCampos" runat="server" Visible="false">
        <div class="mg-info-card">
            <p class="mg-info-title"><span class="ico">&#9989;</span> Campos identificados no DOCX</p>
            <asp:Literal ID="litCampos" runat="server"></asp:Literal>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlDebug" runat="server" Visible="false">
        <div class="mg-debug">
            <h4>&#128027; DEBUG &mdash; Textos encontrados no documento</h4>
            <asp:Literal ID="litDebug" runat="server"></asp:Literal>
        </div>
    </asp:Panel>

    <asp:Panel ID="pnlDadosTeste" runat="server" Visible="false">
        <div class="mg-dados-teste">
            <h4>&#128101; Dados de teste utilizados</h4>
            <strong>TITULAR:</strong><br />
            NOME: MARCOS ANTONIO SILVEIRA &mdash; CPF: 123.456.789-00<br />
            NASCIMENTO: 15/03/1985 &mdash; RG: 12.345.678-9<br />
            ENDERE&Ccedil;O: AVENIDA BOA VIAGEM, 1001, APTO 502<br /><br />
            <strong>DEPENDENTE 1 (ESPOSA):</strong><br />
            NOME: ANA CAROLINA SILVEIRA &mdash; CPF: 111.222.333-44<br /><br />
            <strong>DEPENDENTE 2 (FILHO):</strong><br />
            NOME: LUCAS SILVEIRA &mdash; CPF: 555.666.777-88
        </div>
    </asp:Panel>

</div>
</asp:Content>
