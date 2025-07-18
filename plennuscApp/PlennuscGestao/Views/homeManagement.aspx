<%@ Page 
    Title="" 
    Language="C#" 
    MasterPageFile="~/PlennuscGestao/Views/Masters/Index.Master" 
    AutoEventWireup="true" 
    CodeBehind="homeManagement.aspx.cs" 
    Inherits="PlennuscGestao.Views.HomeGestao" 
%>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <!-- Chart.js CDN -->
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid py-4">
        <!-- Título -->
        <h2 class="fw-bold">Dashboard da Gestão</h2>
        <p class="text-muted mb-4">Veja o que está acontecendo agora no sistema</p>

        <!-- Cards coloridos -->
        <div class="row row-cols-1 row-cols-sm-2 row-cols-md-2 row-cols-lg-4 g-4 mb-4 justify-content-center">
            <div>
                <div class="card shadow-sm border-0" style="background: #e1fbee;">
                    <div class="card-body d-flex align-items-center justify-content-between">
                        <div>
                            <h6 class="fw-semibold text-success">
                                <asp:Label ID="lblTotalColaboradores" runat="server" CssClass="text-success"></asp:Label> colaboradores
                            </h6>
                            <small class="text-muted">Ativos no sistema</small>
                        </div>
                        <i class="bi bi-person-check-fill fs-2 text-success"></i>
                    </div>
                </div>
            </div>
            <div>
                <div class="card shadow-sm border-0" style="background: #fff8e1;">
                    <div class="card-body d-flex align-items-center justify-content-between">
                        <div>
                            <h6 class="fw-semibold text-warning">
                                <asp:Label ID="lblTotalDepartamentos" runat="server" CssClass="text-warning"></asp:Label> departamentos
                            </h6>
                            <small class="text-muted">Organizados</small>
                        </div>
                        <i class="bi bi-diagram-3-fill fs-2 text-warning"></i>
                    </div>
                </div>
            </div>
            <div>
                <div class="card shadow-sm border-0" style="background: #fbe7e7;">
                    <div class="card-body d-flex align-items-center justify-content-between">
                        <div>
                            <h6 class="fw-semibold text-danger">
                                <asp:Label ID="lblTotalCargos" runat="server" CssClass="text-danger"></asp:Label> cargos
                            </h6>
                            <small class="text-muted">Em uso</small>
                        </div>
                        <i class="bi bi-briefcase-fill fs-2 text-danger"></i>
                    </div>
                </div>
            </div>
            <div>
                <div class="card shadow-sm border-0" style="background: #eae7fb;">
                    <div class="card-body d-flex align-items-center justify-content-between">
                        <div>
                            <h6 class="fw-semibold" style="color: #c06ed4;">teste</h6>
                            <small class="text-muted">teste</small>
                        </div>
                        <i class="bi bi-briefcase-fill fs-2" style="color: #c06ed4;"></i>
                    </div>
                </div>
            </div>
        </div>

        <!-- Gráfico -->
        <div class="card shadow-sm border-0 mb-4">
            <div class="card-body">
                <h5 class="fw-semibold mb-3">Acessos nas últimas semanas</h5>
                <canvas id="graficoDashboard" height="100"></canvas>
            </div>
        </div>

        <!-- Rodapé com resumos -->
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
        </div>
    </div>

    <script>
        document.addEventListener("DOMContentLoaded", function () {
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
        });
    </script>
</asp:Content>
