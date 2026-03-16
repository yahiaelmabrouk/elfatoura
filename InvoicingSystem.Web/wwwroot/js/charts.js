window.invoiceCharts = {
    renderBarChart: function (canvasId, labels, values, label) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        if (ctx._chart) {
            ctx._chart.destroy();
        }

        ctx._chart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: labels,
                datasets: [{
                    label: label,
                    data: values,
                    borderWidth: 1,
                    backgroundColor: [
                        'rgba(10, 92, 181, 0.75)',
                        'rgba(255, 127, 17, 0.75)',
                        'rgba(46, 196, 182, 0.75)',
                        'rgba(17, 138, 178, 0.75)',
                        'rgba(10, 147, 150, 0.75)'
                    ]
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    },

    renderPieChart: function (canvasId, labels, values) {
        const ctx = document.getElementById(canvasId);
        if (!ctx) return;

        if (ctx._chart) {
            ctx._chart.destroy();
        }

        ctx._chart = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: labels,
                datasets: [{
                    data: values,
                    backgroundColor: [
                        '#0a5cb5',
                        '#ff7f11',
                        '#2ec4b6',
                        '#118ab2',
                        '#0a9396'
                    ]
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false
            }
        });
    }
};

window.invoiceDownloads = {
    saveFileFromBytes: async function (fileName, contentType, dotNetStreamRef) {
        const arrayBuffer = await dotNetStreamRef.arrayBuffer();
        const blob = new Blob([arrayBuffer], { type: contentType });
        const url = URL.createObjectURL(blob);
        const anchor = document.createElement('a');
        anchor.href = url;
        anchor.download = fileName;
        anchor.click();
        anchor.remove();
        URL.revokeObjectURL(url);
    }
};
