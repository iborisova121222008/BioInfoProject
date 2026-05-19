window.bioInfoCharts = {
    charts: {},

    renderLineChart: function (canvasId, labels, casesData, deathsData) {
        const canvas = document.getElementById(canvasId);
        if (!canvas || !window.Chart) {
            return;
        }

        this.destroy(canvasId);

        this.charts[canvasId] = new Chart(canvas, {
            type: "line",
            data: {
                labels: labels,
                datasets: [
                    {
                        label: "New cases",
                        data: casesData,
                        borderColor: "#1f77b4",
                        backgroundColor: "rgba(31, 119, 180, 0.12)",
                        tension: 0.25,
                        pointRadius: 0,
                        borderWidth: 2
                    },
                    {
                        label: "New deaths",
                        data: deathsData,
                        borderColor: "#d62728",
                        backgroundColor: "rgba(214, 39, 40, 0.12)",
                        tension: 0.25,
                        pointRadius: 0,
                        borderWidth: 2
                    }
                ]
            },
            options: this.defaultOptions()
        });
    },

    renderBarChart: function (canvasId, labels, values, label, color) {
        const canvas = document.getElementById(canvasId);
        if (!canvas || !window.Chart) {
            return;
        }

        this.destroy(canvasId);

        this.charts[canvasId] = new Chart(canvas, {
            type: "bar",
            data: {
                labels: labels,
                datasets: [
                    {
                        label: label,
                        data: values,
                        backgroundColor: color
                    }
                ]
            },
            options: this.defaultOptions()
        });
    },

    destroy: function (canvasId) {
        if (this.charts[canvasId]) {
            this.charts[canvasId].destroy();
            delete this.charts[canvasId];
        }
    },

    defaultOptions: function () {
        return {
            responsive: true,
            maintainAspectRatio: false,
            interaction: {
                intersect: false,
                mode: "index"
            },
            plugins: {
                legend: {
                    position: "bottom"
                }
            },
            scales: {
                y: {
                    beginAtZero: true,
                    ticks: {
                        precision: 0
                    }
                }
            }
        };
    }
};
