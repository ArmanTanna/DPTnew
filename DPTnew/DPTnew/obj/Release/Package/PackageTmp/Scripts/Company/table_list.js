/* Formatting function for row details - modify as you need */
function format(d) {
    // `d` is the original data object for the row
    return '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
       '<tr>' +
            '<td><b>Address:</b></td>' +
            '<td>' + d[8] + '</td>' +

        '</tr>' +
         '<tr>' +

            '<td><b>ZIP:</b></td>' +
            '<td>' + d[9] + '</td>' +
        '</tr>' +
        '<tr>' +

            '<td><b>City:</b></td>' +
            '<td>' + d[10] + '</td>' +
        '</tr>' +
         '<tr>' +
            '<td><b>Province:</b></td>' +
            '<td>' + d[11] + '</td>'
        +
        '</tr>' +
         '<tr>' +
            '<td><b>Country:</b></td>' +
            '<td>' + d[12] + '</td>' +
        '</tr>' +
             '</table>';
}



$(document).ready(function () {


    var myTable = $('#companies').DataTable({
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
           { "orderable": false, },
            { "bVisible": false, },
            { "bVisible": false, },
            { "bVisible": false, },
            { "bVisible": false, },
            { "bVisible": false, },
            { "bVisible": false, },
        ],
        "order": [[1, 'asc']],
        "orderClasses": false,

    });


    // Add event listener for opening and closing details
    $('#companies tbody').on('click', 'td.details-control', function () {
        var tr = $(this).closest('tr');
        var row = myTable.row(tr);
        var index = row.index();
        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        }
        else {
            // Open this row
            var data = row.data();
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
{ column_number: 6, filter_default_label: "Select" }]);


});