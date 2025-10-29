<%@ Page
    Title=""
    Language="C#"
    MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master"
    AutoEventWireup="true"
    CodeBehind="homeManagement.aspx.cs"
    Inherits="PlennuscGestao.Views.HomeGestao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Dashboard</title>

    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">

    <link href="../../Content/Css/projects/gestao/structuresCss/homeManagement.css" rel="stylesheet" />
    <script src="../../Content/Css/js/projects/gestaoJs/structuresJs/homeManagement.js"></script>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-main">
        <header class="page-header">
            <h1 class="page-title">
                <span class="title-icon">
                    <i class="fas fa-chart-line"></i>
                </span>
                Dashboard da Gestão
            </h1>
            <p class="page-description">Veja o que está acontecendo agora no sistema</p>
        </header>

        <!-- Cards principais -->
        <div class="dashboard-grid">
            <div class="dashboard-card" onclick="window.location.href='employeeManagement.aspx?acao=consultar';">
                <div class="card card-colaboradores">
                    <div class="card-body">
                        <div class="card-content">
                            <h6 class="card-title">
                                <asp:Label ID="lblTotalColaboradores" runat="server"></asp:Label>
                                colaboradores
                            </h6>
                            <p class="card-value">
                                <asp:Label ID="lblTotalColaboradoresValue" runat="server"></asp:Label>
                            </p>
                            <p class="card-description">Ativos no sistema</p>
                        </div>
                        <div class="card-icon">
                            <i class="fas fa-users"></i>
                        </div>
                    </div>
                </div>
            </div>

            <div class="dashboard-card" onclick="window.location.href='employeeDepartment.aspx';">
                <div class="card card-departamentos">
                    <div class="card-body">
                        <div class="card-content">
                            <h6 class="card-title">
                                <asp:Label ID="lblTotalDepartamentos" runat="server"></asp:Label>
                                departamentos
                            </h6>
                            <p class="card-value">
                                <asp:Label ID="lblTotalDepartamentosValue" runat="server"></asp:Label>
                            </p>
                            <p class="card-description">Organizados</p>
                        </div>
                        <div class="card-icon">
                            <i class="fas fa-sitemap"></i>
                        </div>
                    </div>
                </div>
            </div>

            <div class="dashboard-card" onclick="window.location.href='employeePosition.aspx';">
                <div class="card card-cargos">
                    <div class="card-body">
                        <div class="card-content">
                            <h6 class="card-title">
                                <asp:Label ID="lblTotalCargos" runat="server"></asp:Label>
                                cargos
                            </h6>
                            <p class="card-value">
                                <asp:Label ID="lblTotalCargosValue" runat="server"></asp:Label>
                            </p>
                            <p class="card-description">Em uso</p>
                        </div>
                        <div class="card-icon">
                            <i class="fas fa-user-tie"></i>
                        </div>
                    </div>
                </div>
            </div>

            <div class="dashboard-card" onclick="window.location.href='demand.aspx';">
                <div class="card card-teste">
                    <div class="card-body">
                        <div class="card-content">
                            <h6 class="card-title">Demanda</h6>
                            <p class="card-description">Criar Demanda</p>
                        </div>
                        <div class="card-icon">
                            <i class="bi bi-plus-lg"></i>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <!-- Widgets de Demandas LADO A LADO -->
        <div class="demands-row">
            <!-- Hoje -->
           <div class="today-section">
                <div class="section-header-with-toggle">
                    <h2 class="today-header">
                        <i class="fas fa-calendar-day"></i>
                        Hoje
                    </h2>
                    <div class="view-toggle">
                        <button class="toggle-btn active" data-view="cards" title="Visualização em Cards">
                            <i class="fas fa-th"></i>
                        </button>
                        <button class="toggle-btn" data-view="chart" title="Visualização em Gráfico">
                            <i class="fas fa-chart-pie"></i>
                        </button>
                    </div>
                </div>

                <!-- Cards (visível por padrão) -->
                <div class="view-content active" data-view="cards">
                    <div class="today-cards">
                        <div class="today-grid">
                            <div class="today-item">
                                <span class="today-label">Novas demandas</span>
                                <span class="today-value novos"><%= DashboardData?.NovasDemandasHoje ?? 0 %></span>
                            </div>
                            <div class="today-item">
                                <span class="today-label">Demandas finalizadas</span>
                                <span class="today-value finalizadas"><%= DashboardData?.DemandasFinalizadasHoje ?? 0 %></span>
                            </div>
                            <div class="today-item">
                                <span class="today-label">Aprovações pendentes</span>
                                <span class="today-value pendentes"><%= DashboardData?.AprovacoesPendentes ?? 0 %></span>
                            </div>
                            <div class="today-item">
                                <span class="today-label">Atrasos críticos</span>
                                <span class="today-value atrasos"><%= DashboardData?.AtrasosCriticos ?? 0 %></span>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Gráfico (oculto por padrão) -->
                <div class="view-content" data-view="chart">
                    <div style="height: 300px; position: relative;">
                        <canvas id="chartHoje"></canvas>
                    </div>
                </div>
            </div>

             <!-- Status das Demandas -->
            <div class="status-section">
                <div class="section-header-with-toggle">
                    <h2 class="status-header">
                        <i class="fas fa-tasks"></i>
                        Status das Demandas
                    </h2>
                    <div class="view-toggle">
                        <button class="toggle-btn active" data-view="cards" title="Visualização em Cards">
                            <i class="fas fa-th"></i>
                        </button>
                        <button class="toggle-btn" data-view="chart" title="Visualização em Gráfico">
                            <i class="fas fa-chart-pie"></i>
                        </button>
                    </div>
                </div>

                <!-- Cards (visível por padrão) -->
                <div class="view-content active" data-view="cards">
                    <div class="status-cards">
                        <div class="status-grid">
                            <div class="status-item">
                                <span class="status-label">Abertas</span>
                                <span class="status-value abertas"><%= DashboardData?.DemandasAbertas ?? 0 %></span>
                            </div>
                            <div class="status-item">
                                <span class="status-label">Andamento</span>
                                <span class="status-value andamento"><%= DashboardData?.DemandasAndamento ?? 0 %></span>
                            </div>
                            <div class="status-item">
                                <span class="status-label">Aguardando</span>
                                <span class="status-value aguardando"><%= DashboardData?.DemandasAguardando ?? 0 %></span>
                            </div>
                            <div class="status-item">
                                <span class="status-label">Atrasadas</span>
                                <span class="status-value atrasadas"><%= DashboardData?.DemandasAtrasadas ?? 0 %></span>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Gráfico (oculto por padrão) -->
                <div class="view-content" data-view="chart">
                    <div style="height: 300px; position: relative;">
                        <canvas id="chartStatus"></canvas>
                    </div>
                </div>
            </div>
        </div>
      </div>

<script type="text/javascript">

    document.addEventListener("DOMContentLoaded", function () {
        let chartHoje = null;
        let chartStatus = null;

        // Dados para os gráficos
        const hojeData = {
            labels: ['Novas', 'Finalizadas', 'Pendentes', 'Atrasos'],
            datasets: [{
                data: [
                    <%= DashboardData?.NovasDemandasHoje ?? 0 %>,
                    <%= DashboardData?.DemandasFinalizadasHoje ?? 0 %>,
                    <%= DashboardData?.AprovacoesPendentes ?? 0 %>,
                    <%= DashboardData?.AtrasosCriticos ?? 0 %>
                ],
            backgroundColor: [
                '#83ceee', '#4cb07a', '#ffa726', '#f44336'
            ],
            borderWidth: 0
        }]
    };

    const statusData = {
        labels: ['Abertas', 'Andamento', 'Aguardando', 'Atrasadas'],
        datasets: [{
            data: [
                    <%= DashboardData?.DemandasAbertas ?? 0 %>,
                    <%= DashboardData?.DemandasAndamento ?? 0 %>,
                    <%= DashboardData?.DemandasAguardando ?? 0 %>,
                    <%= DashboardData?.DemandasAtrasadas ?? 0 %>
                ],
                backgroundColor: [
                    '#83ceee', '#ffa726', '#80868b', '#f44336'
                ],
                borderWidth: 0
            }]
        };

        // Função para criar gráficos APENAS quando visíveis
        function createCharts() {
            // Destrói gráficos existentes
            if (chartHoje) chartHoje.destroy();
            if (chartStatus) chartStatus.destroy();

            // Gráfico de Hoje (se visível)
            const ctxHoje = document.getElementById('chartHoje');
            const hojeSection = document.querySelector('.today-section .view-content[data-view="chart"]');

            if (ctxHoje && hojeSection.classList.contains('active')) {
                chartHoje = new Chart(ctxHoje, {
                    type: 'doughnut',
                    data: hojeData,
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: { position: 'bottom', labels: { boxWidth: 12, padding: 15 } }
                        }
                    }
                });
            }

            // Gráfico de Status (se visível)
            const ctxStatus = document.getElementById('chartStatus');
            const statusSection = document.querySelector('.status-section .view-content[data-view="chart"]');

            if (ctxStatus && statusSection.classList.contains('active')) {
                chartStatus = new Chart(ctxStatus, {
                    type: 'pie',
                    data: statusData,
                    options: {
                        responsive: true,
                        maintainAspectRatio: false,
                        plugins: {
                            legend: { position: 'bottom', labels: { boxWidth: 12, padding: 15 } }
                        }
                    }
                });
            }
        }

        // Sistema de Toggle
        function setupViewToggles() {
            const toggleButtons = document.querySelectorAll('.toggle-btn');

            toggleButtons.forEach(button => {
                button.addEventListener('click', function (e) {
                    e.preventDefault(); // 🔥 IMPEDE O POSTBACK
                    e.stopPropagation();

                    const section = this.closest('.today-section, .status-section');
                    const viewType = this.getAttribute('data-view');

                    // Atualiza botões
                    const groupButtons = section.querySelectorAll('.toggle-btn');
                    groupButtons.forEach(btn => btn.classList.remove('active'));
                    this.classList.add('active');

                    // Mostra/oculta conteúdo
                    const contents = section.querySelectorAll('.view-content');
                    contents.forEach(content => {
                        content.classList.remove('active');
                        if (content.getAttribute('data-view') === viewType) {
                            content.classList.add('active');
                        }
                    });

                    // Recria gráficos quando mudar para view de gráfico
                    if (viewType === 'chart') {
                        setTimeout(createCharts, 50);
                    }
                });
            });
        }
        // Inicializar
        setupViewToggles();

        // Criar gráficos iniciais se estiverem visíveis
        setTimeout(createCharts, 100);
    });


</script>

<!-- 2) Chart.js (obrigatório antes de criarmos gráficos) -->
<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

<!-- 3) SEU JS EXTERNO (apenas uma vez) -->
<script src="../../Content/Css/js/projects/gestaoJs/structuresJs/homeManagement.js"></script>

</asp:Content>
