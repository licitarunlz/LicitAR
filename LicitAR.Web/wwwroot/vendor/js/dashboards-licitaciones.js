document.addEventListener("DOMContentLoaded", function () {
  var options = {
    chart: {
      type: 'pie',
    },
    series: [15, 45, 8],
    labels: ['Ganadas', 'Enviadas', 'Perdidas'],
  };

  var chart = new ApexCharts(document.querySelector("#licitacionesChart"), options);
  chart.render();
});
