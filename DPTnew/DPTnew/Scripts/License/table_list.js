/* Formatting function for row details - modify as you need */
function format(d) {
    // `d` is the original data object for the row
    return '<table cellpadding="5" cellspacing="0" border="0" width="100%" style="padding-left:50px;">' +
'<tr>' +
            '<td><b>Start Date:</b></td>' +
            '<td>' + d[8] + '</td>' +

            '<td><b>End Date:</b></td>' +
            '<td>' + d[9] + '</td>' +

            '<td><b>Maint Start Date:</b></td>' +
            '<td>' + d[10] + '</td>' +
            '<td><b>Maint End Date:</b></td>' +
            '<td>' + d[11] + '</td>' +
        '</tr>' +
         '<tr>' +
            '<td><b>Ancestor:</b></td>' +
            '<td>' + d[7] + '</td>' +
        //'<tr>' +
        //    '<td><b>License Kind:</b></td>' +
        //    '<td>' + d[13] + '</td>' +
        //'</tr>' +
            '<td><b>Note:</b></td>' +
            '<td>' + d[14] + '</td>' +
        '</tr>' +
             '</table>';
}


$(document).ready(function () {

    var myTable = $('#licenses').DataTable({
        "columns": [
            {
                "className": 'details-control',
                "orderable": false,
                "data": null,
                "defaultContent": ''
            },
            null,
            null,
            null,
            null,
            null,
            null,
            { "bVisible": false, },
            { "bVisible": false, },
            { "bVisible": false, },
            { "bVisible": false, },
            null,
            null,
            //{ "bVisible": false, },
            { "bVisible": false, }
        ],
        "order": [[1, 'asc']],
        "orderClasses": false,
        createdRow: function (row, data, index) {
            if (data[11] != null) {
                var now = new Date();
                var month = (now.getMonth() + 1);
                var day = now.getDate();
                if (month < 10)
                    month = "0" + month;
                if (day < 10)
                    day = "0" + day;
                var today = now.getFullYear() + '-' + month + '-' + day;
                if (data[11] < today)
                    $(row).addClass('highlight_red');
                if (data[11] >= today && data[6].indexOf("ABCDEFGH") != -1)
                    $(row).addClass('highlight_blue');
            }
        }
    });
    // Add event listener for opening and closing details
    $('#licenses tbody').on('click', 'td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = myTable.row(tr);

        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        }
        else {
            // Open this row
            row.child(format(row.data())).show();
            tr.addClass('shown');
        }
    });

    yadcf.init(myTable, [
{ column_number: 1, filter_default_label: "Select" },
{ column_number: 2, filter_default_label: "Select" },
{ column_number: 3, filter_default_label: "Select" },
{ column_number: 4, filter_default_label: "Select" },
{ column_number: 5, filter_default_label: "Select" },
{ column_number: 6, filter_default_label: "Select" },
{ column_number: 11, filter_default_label: "Select" },
{ column_number: 12, filter_default_label: "Select" }]);


});