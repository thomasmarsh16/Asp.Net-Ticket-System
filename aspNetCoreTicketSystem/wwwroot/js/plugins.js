window.onload = function () {

    // start data tables
    $('#dataTable').DataTable();
    $('#completeddataTable').DataTable();

    // completion chart code
    var ctx = document.getElementById("completionChart");

    var completionDates = $('#completionChart').data('completiondates');
    var completionNumbers = $('#completionChart').data('completionnum');

    var myLineChart = new Chart(ctx, {
        type: 'line',

        data: {
            datasets: [{
                label: 'task Completion',
                data: completionNumbers,
                borderColor: 'rgb(65,105,225)',
            }],
            labels: completionDates,
        },
        options: {}
    });

    // pie chart code
    var ctx = document.getElementById('categoriesPieChart');
    var x = $('#categoriesPieChart').data('numbers');

    var myDoughnutChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            datasets: [{
                data: x,
                backgroundColor: ['#4df500', '#ffe752', '#ff000e'],
                hoverBackgroundColor: ['#44d600', '#ffdd0e', '#dc143c'],
                hoverBorderColor: "rgba(234, 236, 244, 1)",
            }],
            labels: ['Low', 'Medium', 'High'],
        },
        options: {
            maintainAspectRatio: false,
            tooltips: {
                backgroundColor: "rgb(255,255,255)",
                bodyFontColor: "#858796",
                borderColor: '#dddfeb',
                borderWidth: 1,
                xPadding: 15,
                yPadding: 15,
                displayColors: false,
                caretPadding: 10,
            },
            cutoutPercentage: 70,
        },
    });
}