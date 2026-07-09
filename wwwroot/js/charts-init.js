// charts-init.js - initialize Chart.js charts from canvas + adjacent JSON data
(function(){
    function parseData(el){
        const script = el.parentElement.querySelector('.chart-data');
        if(!script) return null;
        try{
            return JSON.parse(script.textContent);
        }catch(e){
            console.error('Invalid chart JSON for', el, e);
            return null;
        }
    }

    function initRadar(el, data){
        const ctx = el.getContext('2d');
        new Chart(ctx, {
            type: 'radar',
            data: {
                labels: data.labels,
                datasets: [{
                    label: data.label || 'Valores',
                    data: data.values,
                    backgroundColor: 'rgba(124,58,237,0.15)',
                    borderColor: 'rgba(124,58,237,0.9)',
                    borderWidth: 2,
                    pointBackgroundColor: 'rgba(124,58,237,0.9)'
                }]
            },
            options: {
                responsive: true,
                scales: {
                    r: { beginAtZero: true, grid: { color: 'rgba(255,255,255,0.04)' }, angleLines: { color: 'rgba(255,255,255,0.03)' }, ticks: { color: 'rgba(255,255,255,0.5)' } }
                },
                plugins: { legend: { display: false } }
            }
        });
    }

    function initScatter(el, data){
        const ctx = el.getContext('2d');
        new Chart(ctx, {
            type: 'bubble',
            data: {
                datasets: [{
                    label: data.label || 'Riesgos',
                    data: data.points.map(p=>({x:p.impact, y:p.probability, r: (p.size||8)})),
                    backgroundColor: 'rgba(220,53,69,0.6)'
                })]
            },
            options: { scales: { x: { title: { display: true, text: 'Impacto' } }, y: { title: { display: true, text: 'Probabilidad' } } }, plugins: { legend: { display: false } } }
        });
    }

    function initLine(el, data){
        const ctx = el.getContext('2d');
        new Chart(ctx, {
            type: 'line',
            data: { labels: data.labels, datasets: [{ label: data.label || 'Tendencia', data: data.values, borderColor: 'rgba(59,130,246,0.9)', backgroundColor: 'rgba(59,130,246,0.12)', fill: true }] },
            options: { responsive: true, plugins: { legend: { display: false } } }
        });
    }

    function initBar(el, data){
        const ctx = el.getContext('2d');
        new Chart(ctx, {
            type: 'bar',
            data: { labels: data.labels, datasets: [{ label: data.label || 'Cantidad', data: data.values, backgroundColor: data.colors || 'rgba(124,58,237,0.85)' }] },
            options: { responsive: true, plugins: { legend: { display: false } }, scales: { y: { beginAtZero: true } } }
        });
    }

    function initDoughnut(el, data){
        const ctx = el.getContext('2d');
        new Chart(ctx, {
            type: 'doughnut',
            data: { labels: data.labels, datasets: [{ label: data.label || '', data: data.values, backgroundColor: data.colors || ['rgba(124,58,237,0.9)','rgba(168,85,247,0.8)','rgba(59,130,246,0.8)'] }] },
            options: { responsive: true, plugins: { legend: { position: 'bottom' } } }
        });
    }

    document.addEventListener('DOMContentLoaded', ()=>{
        document.querySelectorAll('.chart-canvas').forEach(canvas=>{
            const json = parseData(canvas);
            if(!json || !json.type) return;
            switch(json.type){
                case 'radar': initRadar(canvas, json); break;
                case 'scatter': initScatter(canvas, json); break;
                case 'line': initLine(canvas, json); break;
                case 'bar': initBar(canvas, json); break;
                case 'doughnut': initDoughnut(canvas, json); break;
                default: console.warn('Unknown chart type', json.type);
            }
        });
    });
})();
