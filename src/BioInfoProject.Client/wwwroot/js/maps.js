window.bioInfoMaps = {
    renderWorldMap: function (
        elementId,
        countries,
        totalNewCases,
        cumulativeCases,
        totalDeaths,
        cumulativeDeaths,
        mortalityRates) {
        const element = document.getElementById(elementId);
        if (!element || !window.Plotly) {
            return;
        }

        Plotly.purge(element);

        const customData = countries.map((country, index) => [
            totalNewCases[index] || 0,
            cumulativeCases[index] || 0,
            totalDeaths[index] || 0,
            cumulativeDeaths[index] || 0,
            mortalityRates[index] || 0
        ]);

        const trace = {
            type: "choropleth",
            locationmode: "country names",
            locations: countries,
            z: cumulativeCases,
            text: countries,
            customdata: customData,
            colorscale: [
                [0, "#d8f3f1"],
                [0.25, "#7fcfc8"],
                [0.5, "#2a9fd6"],
                [0.75, "#0b8f8a"],
                [1, "#0b2a3c"]
            ],
            marker: {
                line: {
                    color: "#ffffff",
                    width: 0.35
                }
            },
            colorbar: {
                title: "Cumulative cases",
                thickness: 14,
                len: 0.72
            },
            hovertemplate:
                "<b>%{text}</b><br>" +
                "Total new cases: %{customdata[0]:,.0f}<br>" +
                "Cumulative cases: %{customdata[1]:,.0f}<br>" +
                "Total deaths: %{customdata[2]:,.0f}<br>" +
                "Cumulative deaths: %{customdata[3]:,.0f}<br>" +
                "Mortality rate: %{customdata[4]:.2f}%<extra></extra>"
        };

        const layout = {
            autosize: true,
            margin: { t: 0, r: 0, b: 0, l: 0 },
            paper_bgcolor: "rgba(0,0,0,0)",
            plot_bgcolor: "rgba(0,0,0,0)",
            geo: {
                projection: {
                    type: "natural earth"
                },
                showframe: false,
                showcoastlines: true,
                coastlinecolor: "#b8ccd4",
                showcountries: true,
                countrycolor: "#ffffff",
                showland: true,
                landcolor: "#f8fcfd",
                showocean: true,
                oceancolor: "#eef5f8",
                bgcolor: "rgba(0,0,0,0)"
            }
        };

        const config = {
            responsive: true,
            displaylogo: false,
            modeBarButtonsToRemove: ["lasso2d", "select2d"]
        };

        Plotly.newPlot(element, [trace], layout, config);
    },

    destroy: function (elementId) {
        const element = document.getElementById(elementId);
        if (element && window.Plotly) {
            Plotly.purge(element);
        }
    }
};
