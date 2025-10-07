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

<style>
    :root {
        --primary: #83ceee;
        --primary-hover: #0d62c9;
        --success: #4cb07a;
        --success-hover: #3b8b65;
        --warning: #ffa726;
        --warning-hover: #f57c00;
        --danger: #f44336;
        --danger-hover: #d32f2f;
        --gray-50: #f8f9fa;
        --gray-100: #f1f3f4;
        --gray-200: #e8eaed;
        --gray-300: #dadce0;
        --gray-400: #bdc1c6;
        --gray-500: #9aa0a6;
        --gray-600: #80868b;
        --gray-700: #5f6368;
        --gray-800: #3c4043;
        --gray-900: #202124;
        --border-radius: 8px;
        --shadow: 0 1px 2px 0 rgba(60, 64, 67, 0.3), 0 1px 3px 1px rgba(60, 64, 67, 0.15);
        --transition: all 0.2s ease-in-out;
    }

    .container-main {
        max-width: 2206px;
        margin: 20px auto;
        padding: 0 16px;
    }

    /* Header */
    .page-header {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 24px;
        flex-wrap: wrap;
        gap: 16px;
    }

    .page-title {
        display: flex;
        align-items: center;
        gap: 12px;
        font-size: 24px;
        font-weight: 500;
        color: var(--gray-800);
        margin: 0;
    }

    .title-icon {
        background: var(--primary);
        color: white;
        width: 44px;
        height: 44px;
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .page-description {
        color: var(--gray-600);
        font-size: 16px;
        margin: 0;
        flex-basis: 100%;
    }

    /* Cards principais */
    .dashboard-grid {
        display: grid;
        grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
        gap: 16px;
        margin-bottom: 24px;
    }

    .dashboard-card {
        cursor: pointer;
        border-radius: var(--border-radius);
        transition: var(--transition);
    }

    .dashboard-card:hover {
        transform: translateY(-2px);
    }

    .card {
        background: white;
        border-radius: var(--border-radius);
        box-shadow: var(--shadow);
        transition: var(--transition);
        border: none;
        height: auto;
        min-height: 120px;
    }   

    .dashboard-card:hover .card {
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
    }

    .card-body {
        display: flex;
        align-items: center;
        justify-content: space-between;
        padding: 16px 20px;
        height: 100%;
    }

    .card-content {
        flex: 1;
    }

    .card-title {
        font-size: 16px;
        font-weight: 500;
        color: var(--gray-800);
        margin: 0 0 4px 0;
        line-height: 1.2;
    }

    .card-value {
        font-size: 28px;
        font-weight: 700;
        margin: 0 0 4px 0;
        line-height: 1;
    }

    .card-description {
        font-size: 13px;
        color: var(--gray-600);
        margin: 0;
        line-height: 1.2;
    }

    .card-icon {
        width: 50px;
        height: 50px;
        border-radius: 50%;
        display: flex;
        align-items: center;
        justify-content: center;
        margin-left: 12px;
        font-size: 1.25rem;
    }

    /* Cores dos cards */
    .card-colaboradores {
        background: #e1fbee !important;
    }

    .card-colaboradores .card-value,
    .card-colaboradores .card-title {
        color: var(--success) !important;
    }

    .card-colaboradores .card-icon {
        background: rgba(76, 176, 122, 0.2);
        color: var(--success);
    }

    .card-departamentos {
        background: #fff8e1 !important;
    }

    .card-departamentos .card-value,
    .card-departamentos .card-title {
        color: var(--warning) !important;
    }

    .card-departamentos .card-icon {
        background: rgba(255, 167, 38, 0.2);
        color: var(--warning);
    }

    .card-cargos {
        background: #fbe7e7 !important;
    }

    .card-cargos .card-value,
    .card-cargos .card-title {
        color: var(--danger) !important;
    }

    .card-cargos .card-icon {
        background: rgba(244, 67, 54, 0.2);
        color: var(--danger);
    }

    .card-teste {
        background: #eae7fb !important;
    }

    .card-teste .card-value,
    .card-teste .card-title {
        color: #c06ed4 !important;
    }

    .card-teste .card-icon {
        background: rgba(192, 110, 212, 0.2);
        color: #c06ed4;
    }

    /* Layout das demandas lado a lado */
    .demands-row {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 24px;
        margin-bottom: 24px;
    }

    /* Seções Hoje e Status */
    .today-section,
    .status-section {
        background: white;
        border-radius: var(--border-radius);
        box-shadow: var(--shadow);
        padding: 20px;
        height: fit-content;
    }

    .today-header,
    .status-header {
        font-size: 18px;
        font-weight: 500;
        color: var(--gray-800);
        margin-bottom: 16px;
        display: flex;
        align-items: center;
        gap: 8px;
    }

    /* Grids internos */
    .today-grid,
    .status-grid {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 12px;
    }

    .today-item,
    .status-item {
        display: flex;
        flex-direction: column;
        padding: 16px;
        border-radius: var(--border-radius);
        background: var(--gray-50);
        text-align: center;
        transition: var(--transition);
    }

    .today-item:hover,
    .status-item:hover {
        background: var(--gray-100);
        transform: translateY(-1px);
    }

    .today-label,
    .status-label {
        font-size: 13px;
        color: var(--gray-600);
        margin-bottom: 8px;
        font-weight: 500;
    }

    .today-value,
    .status-value {
        font-size: 24px;
        font-weight: 700;
        line-height: 1;
    }

    /* Cores dos valores */
    .today-value.novos,
    .status-value.abertas { color: var(--primary); }

    .today-value.finalizadas,
    .status-value.andamento { color: var(--success); }

    .today-value.pendentes { color: var(--warning); }

    .today-value.atrasos,
    .status-value.atrasadas { color: var(--danger); }

    .status-value.aguardando { color: var(--gray-600); }

    /* ESTILOS DE GRÁFICOS */
    .section-header-with-toggle {
        display: flex;
        justify-content: space-between;
        align-items: center;
        margin-bottom: 16px;
    }

    .view-toggle {
        display: flex;
        gap: 4px;
        background: var(--gray-100);
        border-radius: 6px;
        padding: 4px;
    }

    .toggle-btn {
        background: transparent;
        border: none;
        border-radius: 4px;
        padding: 8px 12px;
        cursor: pointer;
        color: var(--gray-500);
        transition: all 0.2s ease;
        display: flex;
        align-items: center;
        justify-content: center;
    }

    .toggle-btn:hover {
        background: var(--gray-200);
        color: var(--gray-700);
    }

    .toggle-btn.active {
        background: var(--primary);
        color: white;
        box-shadow: var(--shadow);
    }

    /* Conteúdo das Visualizações */
    .view-content {
        display: none;
    }

    .view-content.active {
        display: block;
    }

    /* ESTILOS MODERNOS PARA OS GRÁFICOS - APENAS CSS */
.view-content[data-view="chart"] {
    background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%) !important;
    border-radius: 16px !important;
    padding: 20px !important;
    border: 1px solid #e2e8f0 !important;
    box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06) !important;
    margin-top: 10px;
}

/* Containers dos gráficos */
.view-content[data-view="chart"] > div {
    height: 320px !important;
    position: relative !important;
    margin: 0 auto !important;
}

/* Canvas dos gráficos */
.view-content[data-view="chart"] canvas {
    border-radius: 12px !important;
    background: white !important;
    padding: 15px !important;
    box-shadow: inset 0 2px 4px rgba(0, 0, 0, 0.05) !important;
}

/* Efeitos hover */
.view-content[data-view="chart"]:hover {
    transform: translateY(-2px) !important;
    box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04) !important;
    transition: all 0.3s ease !important;
}

/* Animações */
@keyframes chartSlideIn {
    from {
        opacity: 0;
        transform: translateY(10px);
    }
    to {
        opacity: 1;
        transform: translateY(0);
    }
}

.view-content[data-view="chart"].active {
    animation: chartSlideIn 0.5s ease-out !important;
}

/* Responsividade */
@media (max-width: 768px) {
    .view-content[data-view="chart"] > div {
        height: 280px !important;
    }
    
    .view-content[data-view="chart"] {
        padding: 16px !important;
    }
}

@media (max-width: 480px) {
    .view-content[data-view="chart"] > div {
        height: 250px !important;
    }
}

    /* Responsividade */
    @media (max-width: 1024px) {
        .demands-row {
            grid-template-columns: 1fr;
            gap: 16px;
        }
    }

    @media (max-width: 768px) {
        .dashboard-grid {
            grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
            gap: 12px;
        }

        .today-grid,
        .status-grid {
            grid-template-columns: 1fr;
        }

        .card-body {
            padding: 14px 16px;
        }

        .card-value {
            font-size: 24px;
        }

        .card-icon {
            width: 40px;
            height: 40px;
            font-size: 1rem;
            margin-left: 8px;
        }

        .today-item,
        .status-item {
            padding: 14px 12px;
        }

        .today-value,
        .status-value {
            font-size: 22px;
        }
    }

    @media (max-width: 480px) {
        .today-section,
        .status-section {
            padding: 16px;
        }

        .today-header,
        .status-header {
            font-size: 16px;
        }
    }
</style>
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

<script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
<script>
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
                button.addEventListener('click', function(e) {
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
</asp:Content>
