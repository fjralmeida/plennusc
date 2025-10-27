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