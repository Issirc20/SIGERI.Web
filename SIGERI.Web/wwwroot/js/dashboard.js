/* ---------------------------------------------------------
   SIGERI CYBERSECURITY DASHBOARD - FRONTEND CONTROLLER
   Initializes Chart.js charts and binds interactive behaviors
   --------------------------------------------------------- */

(function () {
    // Utility to parse JSON data attached to canvas elements
    function parseChartData(canvasId) {
        const script = document.getElementById(canvasId + '-data');
        if (!script) return null;
        try {
            return JSON.parse(script.textContent);
        } catch (e) {
            console.error('Invalid chart JSON for', canvasId, e);
            return null;
        }
    }

    // Chart 1: Risk Distribution (Bar Chart)
    function initDistributionChart() {
        const data = parseChartData('chartDistribution');
        if (!data) return;

        const ctx = document.getElementById('chartDistribution').getContext('2d');
        new Chart(ctx, {
            type: 'bar',
            data: {
                labels: data.labels,
                datasets: [{
                    label: 'Cantidad de Riesgos',
                    data: data.values,
                    backgroundColor: [
                        '#991b1b', // Crítico
                        '#c2410c', // Alto
                        '#854d0e', // Medio
                        '#166534'  // Bajo
                    ],
                    borderWidth: 0,
                    borderRadius: 4
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    tooltip: { padding: 12 }
                },
                scales: {
                    x: { grid: { display: false } },
                    y: { 
                        beginAtZero: true, 
                        ticks: { precision: 0 },
                        grid: { color: '#f1f5f9' }
                    }
                }
            }
        });
    }

    // Chart 2: Risk Trend (Line Chart)
    function initTrendChart() {
        const data = parseChartData('chartTrend');
        if (!data) return;

        const ctx = document.getElementById('chartTrend').getContext('2d');
        const gradient = ctx.createLinearGradient(0, 0, 0, 200);
        gradient.addColorStop(0, 'rgba(79, 70, 229, 0.15)');
        gradient.addColorStop(1, 'rgba(79, 70, 229, 0)');

        new Chart(ctx, {
            type: 'line',
            data: {
                labels: data.labels,
                datasets: [{
                    label: 'Costo Salvaguarda (S/)',
                    data: data.values,
                    borderColor: '#4f46e5',
                    borderWidth: 2,
                    pointBackgroundColor: '#4f46e5',
                    pointHoverRadius: 6,
                    fill: true,
                    backgroundColor: gradient,
                    tension: 0.3
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    tooltip: { padding: 12 }
                },
                scales: {
                    x: { grid: { display: false } },
                    y: { 
                        grid: { color: '#f1f5f9' },
                        ticks: {
                            callback: function(value) { return 'S/ ' + value.toLocaleString(); }
                        }
                    }
                }
            }
        });
    }

    // Chart 3: Asset Categories (Doughnut Chart)
    function initCategoriesChart() {
        const data = parseChartData('chartCategories');
        if (!data) return;

        const ctx = document.getElementById('chartCategories').getContext('2d');
        new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: data.labels,
                datasets: [{
                    data: data.values,
                    backgroundColor: [
                        '#4f46e5', // Indigo
                        '#06b6d4', // Cyan
                        '#10b981', // Emerald
                        '#f59e0b', // Amber
                        '#ec4899'  // Pink
                    ],
                    borderWidth: 2,
                    borderColor: '#ffffff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { 
                        position: 'bottom',
                        labels: { boxWidth: 12, padding: 16 }
                    },
                    tooltip: { padding: 12 }
                },
                cutout: '65%'
            }
        });
    }

    // Chart 4: NIST CSF Progress (Radar Chart)
    function initNistRadarChart() {
        const data = parseChartData('chartNistRadar');
        if (!data) return;

        const ctx = document.getElementById('chartNistRadar').getContext('2d');
        new Chart(ctx, {
            type: 'radar',
            data: {
                labels: data.labels,
                datasets: [{
                    label: 'Madurez Actual',
                    data: data.values,
                    backgroundColor: 'rgba(79, 70, 229, 0.15)',
                    borderColor: '#4f46e5',
                    borderWidth: 2,
                    pointBackgroundColor: '#4f46e5',
                    pointHoverRadius: 5
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    tooltip: { padding: 12 }
                },
                scales: {
                    r: {
                        beginAtZero: true,
                        max: 4.0,
                        ticks: { stepSize: 1, display: false },
                        grid: { color: '#e2e8f0' },
                        angleLines: { color: '#e2e8f0' },
                        pointLabels: { font: { weight: 'bold' } }
                    }
                }
            }
        });
    }

    // Chart 5: ISO 27001 Compliance (Doughnut Gauge)
    function initIsoComplianceChart() {
        const data = parseChartData('chartIsoCompliance');
        if (!data) return;

        const compliance = data.value;
        const remainder = 100 - compliance;

        const ctx = document.getElementById('chartIsoCompliance').getContext('2d');
        new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: ['Cumplimiento', 'Brecha'],
                datasets: [{
                    data: [compliance, remainder],
                    backgroundColor: ['#10b981', '#f1f5f9'],
                    borderWidth: 0
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { display: false },
                    tooltip: {
                        callbacks: {
                            label: function(context) { return ' ' + context.label + ': ' + context.raw + '%'; }
                        }
                    }
                },
                cutout: '75%',
                rotation: -90,
                circumference: 180
            }
        });
    }

    // Chart 6: Treatment Status (Doughnut Chart)
    function initTreatmentStatusChart() {
        const data = parseChartData('chartTreatmentStatus');
        if (!data) return;

        const ctx = document.getElementById('chartTreatmentStatus').getContext('2d');
        new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: data.labels,
                datasets: [{
                    data: data.values,
                    backgroundColor: ['#10b981', '#e2e8f0'],
                    borderWidth: 2,
                    borderColor: '#ffffff'
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                plugins: {
                    legend: { 
                        position: 'bottom',
                        labels: { boxWidth: 12, padding: 16 }
                    },
                    tooltip: { padding: 12 }
                },
                cutout: '65%'
            }
        });
    }

    // Interactive Risk Matrix cell click handlers
    function bindRiskMatrixInteractivity() {
        const cells = document.querySelectorAll('.matrix-cell[data-severity]');
        cells.forEach(cell => {
            cell.addEventListener('click', function () {
                const impact = this.getAttribute('data-impact');
                const probability = this.getAttribute('data-probability');
                const count = this.getAttribute('data-count');
                
                if (parseInt(count) === 0) return;

                // Simulate filtering in UI or log details
                console.log(`Filtering risks with Impacto: ${impact}, Probabilidad: ${probability}`);
                
                // Set the global search input to the severity level or filter rows
                const globalSearch = document.getElementById('globalSearch');
                if (globalSearch) {
                    const severity = this.getAttribute('data-severity');
                    globalSearch.value = severity;
                    globalSearch.dispatchEvent(new Event('input'));
                    globalSearch.focus();
                }
            });
        });
    }

    // Initialize all components on DOM loaded
    document.addEventListener('DOMContentLoaded', () => {
        initDistributionChart();
        initTrendChart();
        initCategoriesChart();
        initNistRadarChart();
        initIsoComplianceChart();
        initTreatmentStatusChart();
        bindRiskMatrixInteractivity();

        // Refresh lucide icons if loaded
        if (window.lucide) {
            lucide.replace();
        }
    });
})();
