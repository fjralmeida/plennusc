<%@ Page 
    Title="" 
    Language="C#" 
    MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" 
    AutoEventWireup="true" 
    CodeBehind="homeManagement.aspx.cs" 
    Inherits="PlennuscGestao.Views.HomeGestao" 
%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <title>Dashboard</title>

     <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css">

    <!-- Chart.js CDN -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

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

        body {
            background: var(--gray-100);
            font-family: 'Roboto', sans-serif;
            color: var(--gray-800);
            line-height: 1.5;
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

        /* Cards mais compactos */
        .dashboard-grid {
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 16px;
            margin-bottom: 24px;
        }

        .dashboard-card {
            cursor: pointer;
            border-radius: var(--border-radius);
        }

        .dashboard-card:hover {
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
        }

        .dashboard-card:active {
            transform: scale(0.98);
        }

        .card {
            background: white;
            border-radius: var(--border-radius);
            box-shadow: var(--shadow);
            transition: var(--transition);
            border: none;
            height: auto;
            min-height: 120px; /* Altura reduzida */
        }   

        .card:hover {
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
        }

      .card-body {
            display: flex;
            align-items: center;
            justify-content: space-between;
            padding: 16px 20px; /* Padding reduzido */
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
            font-size: 28px; /* Tamanho reduzido */
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

        /* Ícones menores */
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

        .card-colaboradores .card-icon {
            background: rgba(76, 176, 122, 0.2);
            color: var(--success);
        }

        .card-departamentos .card-icon {
            background: rgba(255, 167, 38, 0.2);
            color: var(--warning);
        }

        .card-cargos .card-icon {
            background: rgba(244, 67, 54, 0.2);
            color: var(--danger);
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

/* Hoje Section */
.today-section {
    background: white;
    border-radius: var(--border-radius);
    box-shadow: var(--shadow);
    padding: 20px;
    height: fit-content;
}

.today-header {
    font-size: 18px;
    font-weight: 500;
    color: var(--gray-800);
    margin-bottom: 16px;
    display: flex;
    align-items: center;
    gap: 8px;
}

.today-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 12px;
}

.today-item {
    display: flex;
    flex-direction: column;
    padding: 16px;
    border-radius: var(--border-radius);
    background: var(--gray-50);
    text-align: center;
    transition: var(--transition);
}

.today-item:hover {
    background: var(--gray-100);
    transform: translateY(-1px);
}

.today-label {
    font-size: 13px;
    color: var(--gray-600);
    margin-bottom: 8px;
    font-weight: 500;
}

.today-value {
    font-size: 24px;
    font-weight: 700;
    line-height: 1;
}

.today-value.novos {
    color: var(--primary);
}

.today-value.finalizadas {
    color: var(--success);
}

.today-value.pendentes {
    color: var(--warning);
}

.today-value.atrasos {
    color: var(--danger);
}

/* Status Section */
.status-section {
    background: white;
    border-radius: var(--border-radius);
    box-shadow: var(--shadow);
    padding: 20px;
    height: fit-content;
}

.status-header {
    font-size: 18px;
    font-weight: 500;
    color: var(--gray-800);
    margin-bottom: 16px;
    display: flex;
    align-items: center;
    gap: 8px;
}

.status-grid {
    display: grid;
    grid-template-columns: 1fr 1fr;
    gap: 12px;
}

.status-item {
    display: flex;
    flex-direction: column;
    padding: 16px;
    border-radius: var(--border-radius);
    background: var(--gray-50);
    text-align: center;
    transition: var(--transition);
}

.status-item:hover {
    background: var(--gray-100);
    transform: translateY(-1px);
}

.status-label {
    font-size: 13px;
    color: var(--gray-600);
    margin-bottom: 8px;
    font-weight: 500;
}

.status-value {
    font-size: 24px;
    font-weight: 700;
    line-height: 1;
}

.status-value.abertas {
    color: var(--primary);
}

.status-value.andamento {
    color: var(--warning);
}

.status-value.aguardando {
    color: var(--gray-600);
}

.status-value.atrasadas {
    color: var(--danger);
}

/* Responsividade */
@media (max-width: 1024px) {
    .demands-row {
        grid-template-columns: 1fr;
        gap: 16px;
    }
    
    .today-grid,
    .status-grid {
        grid-template-columns: repeat(2, 1fr);
    }
}

@media (max-width: 768px) {
    .today-grid,
    .status-grid {
        grid-template-columns: 1fr;
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

        /* Efeito hover mais sutil */
        .dashboard-card:hover .card {
            transform: translateY(-2px);
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
        }

        /* Responsividade */
        @media (max-width: 768px) {
            .dashboard-grid {
                grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
                gap: 12px;
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
                                <asp:Label ID="lblTotalColaboradores" runat="server"></asp:Label> colaboradores
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
                                <asp:Label ID="lblTotalDepartamentos" runat="server"></asp:Label> departamentos
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
                                <asp:Label ID="lblTotalCargos" runat="server"></asp:Label> cargos
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
        <h2 class="today-header">
            <i class="fas fa-calendar-day"></i>
            Hoje
        </h2>
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

    <!-- Status das Demandas -->
    <div class="status-section">
        <h2 class="status-header">
            <i class="fas fa-tasks"></i>
            Status das Demandas
        </h2>
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
    <%--        <!-- Gráfico -->
        <div class="card shadow-sm border-0 mb-4">
            <div class="card-body">
                <h5 class="fw-semibold mb-3">Acessos nas últimas semanas</h5>
                <canvas id="graficoDashboard" height="100"></canvas>
            </div>
        </div>--%>

       <%-- <!-- Rodapé com resumos -->
        <div class="row g-4">
            <div class="col-md-6">
                <div class="card shadow-sm border-0">
                    <div class="card-body">
                        <h6 class="fw-semibold mb-2">Últimos acessos</h6>
                        <p class="text-muted">Em breve será possível ver os últimos acessos ao sistema aqui.</p>
                    </div>
                </div>
            </div>
            <div class="col-md-6">
                <div class="card shadow-sm border-0">
                    <div class="card-body">
                        <h6 class="fw-semibold mb-2">Estatísticas adicionais</h6>
                        <p class="text-muted">Espaço reservado para futuras funcionalidades.</p>
                    </div>
                </div>
            </div>
        </div>--%>

    <script>
        document.addEventListener("DOMContentLoaded", function () {
            // Se você quiser adicionar o gráfico futuramente, pode descomentar essa parte
            /*
            const ctx = document.getElementById('graficoDashboard').getContext('2d');
            new Chart(ctx, {
                type: 'line',
                data: {
                    labels: ['01 Mai', '08 Mai', '15 Mai', '22 Mai', '29 Mai'],
                    datasets: [{
                        label: 'Acessos',
                        data: [3, 5, 9, 6, 10],
                        borderColor: '#4CB07A',
                        backgroundColor: 'rgba(76, 176, 122, 0.1)',
                        fill: true,
                        tension: 0.4
                    }]
                },
                options: {
                    responsive: true,
                    plugins: {
                        legend: { display: false }
                    },
                    scales: {
                        y: { beginAtZero: true }
                    }
                }
            });
            */
        });
    </script>
</asp:Content>