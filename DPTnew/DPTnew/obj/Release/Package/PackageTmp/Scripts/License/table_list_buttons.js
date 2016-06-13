String.prototype.replaceAll = function (search, replacement) {
    var target = this;
    return target.replace(new RegExp(search, 'g'), replacement);
};

String.prototype.startsWith = function (pattern) {
    if (!String.prototype.startsWith) {
        String.prototype.startsWith = function (searchString, position) {
            position = position || 0;
            return this.indexOf(searchString, position) === position;
        };
    }
};

/* init drop down */

function initdropdown(table) {

    yadcf.init(table, [
  { column_number: 1, filter_default_label: "Select" },
  { column_number: 2, filter_default_label: "Select" },
  { column_number: 3, filter_default_label: "Select" },
  { column_number: 4, filter_default_label: "Select" },
  { column_number: 5, filter_default_label: "Select" },
  { column_number: 6, filter_default_label: "Select" },
  { column_number: 7, filter_default_label: "Select" },
  { column_number: 8, filter_default_label: "Select" }]);

}

/* Formatting function for row details - modify as you need */
function format(d) {
    // `d` is the original data object for the row
    return '<table cellpadding="5" cellspacing="0" border="0" width="100%" style="padding-left:50px;">' +
             '<tr>' +
            '<td><b>Start Date:</b></td>' +
            '<td>' + d.SD + '</td>' +

            '<td><b>End Date:</b></td>' +
            '<td>' + d.ED + '</td>' +

            '<td><b>Maintenance Start Date:</b></td>' +
            '<td>' + d.MSD + '</td>' +
            '<td><b>Maintenance End Date:</b></td>' +
            '<td>' + d.MED + '</td>'
        +
        '</tr>' +
         '<tr>' +
            '<td><b>Ancestor:</b></td>' +
            '<td>' + d.Ancestor + '</td>' +

            //'<td><b>License Kind:</b></td>' +
            //'<td>' + d.LicenseKind + '</td>' +

            '<td><b>Note:</b></td>' +
            '<td>' + d.Note + '</td>' +
        '</tr>' +
             '</table>';
}

function parseJsonDate(jsonDateString) {
    return new Date(parseInt(jsonDateString.replace('/Date(', '')));
}

var loadLicenseTable = function (dtConfig, superUser) {
    var dtDefaults = {
        "columns": [
                {
                    "className": 'details-control',
                    "orderable": false,
                    "data": null,
                    "defaultContent": ''
                },
                { data: "LicenseID" },
                { data: "ProductName" },
                { data: "ArticleDetail", "className": 'mycenter-text' },
                { data: "Quantity", "className": 'mycenter-text' },
                { data: "LicenseType", "className": 'mycenter-text' },
                { data: "MachineID" },
                { data: "MED", type: "date", "width": "10%", "className": 'mycenter-text' },
                { data: "Version", "className": 'mycenter-text' }
        ],
        "order": [[1, 'asc']],
        "orderClasses": false,
        select: {
            style: 'single'
        }
    };

    dtConfig = $.extend(dtDefaults, dtConfig);

    var myTable = $('#licenses').DataTable(dtConfig);
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

    initdropdown(myTable);

    var generate2014pwd = function (data, mid) {
        var machineId = mid || data.MachineID;
        if (!machineId || machineId.length !== 8) return;

        var postData = {
            tipo: "P",
            machineid: machineId,
            prodotto: data.PwdCode
        };
        if (data.ArticleDetail.toLocaleLowerCase() === "asf") {
            postData.tipo = "T";
            postData.expdata = data.MED.replaceAll("-", "");
        }

        var $machineIdPopup = $(this);
        var onSuccess = function (result) {

            if ($machineIdPopup && $machineIdPopup.is(":visible")) {
                $machineIdPopup.dialog("close");
                $machineIdPopup.find("input#machineid").val("");
            }
            var $psw = $("#password");
            $psw.text("");
            $psw.text(result.Password || result.PwdLine.replaceAll("\\\"", "\""));
            var pwdDialogConfig = {
                modal: true,
                width: 400,
                height: result.PwdLine ? 250 : "auto",
                buttons: {
                    OK: function () {
                        $("#pwd-dialog").prop('title', '');
                        $(this).dialog("close");
                        location.reload();
                    }
                }
            };
            $("#pwd-dialog").prop('title', 'Your requested password is');
            $("#pwd-dialog").dialog(pwdDialogConfig);
            myTable.rows('.selected').deselect();

        };

        var headers = {};
        if (mid)
            headers.LicenseID = data.LicenseID;


        if (data.LicenseType.toLowerCase() === "local")
            $.ajax({
                url: "../api/PasswordGenerator/NewLicense",
                type: 'POST',
                data: postData,
                headers: headers,
                dataType: "json",
                success: onSuccess
            });
        else {
            var prms = data.PwdCode.split(',');
            $.ajax({
                url: "../api/PasswordGenerator/FlexLicense?flexkind=8&feature=" + prms[0] + "&Version=" + prms[1] + "&numServ=1&machineid1=" + machineId + "&machineid2=null&machineid3=null&nlic=" + prms[3] + "&expdate=" + prms[4] + "&vend_string=" + prms[5] + "&typelic=" + prms[6] + "&expkind=" + prms[7] + "&param=test",
                type: 'GET',
                data: null,
                headers: headers,
                dataType: "json",
                success: onSuccess
            });
        }

    };

    //BUTTONS
    if (superUser)
        new $.fn.dataTable.Buttons(myTable, {
            buttons: [
         {
             text: 'Export',
             className: 'export',
             action: function () {

                 if (myTable.rows('.selected').count() != 0) {
                     var data = myTable.rows('.selected').data()[0];
                     //upload .c2v
                     $.post(yourApp.Urls.exportUrl, {
                         LicenseID: data.LicenseID,
                         ProductName: data.ProductName,
                         ArticleDetail: data.ArticleDetail,
                         Quantity: data.Quantity,
                         LicenseType: data.LicenseType,
                         MachineID: data.MachineID,
                         MaintEndDate: data.MED,
                         Version: data.Version
                     },
                         function (result) {
                             var url = yourApp.Urls.exportUrl;
                             WinId = window.open(url, '_blank');
                             WinId.document.open();
                             WinId.document.write(result);
                             WinId.document.close();
                             myTable.rows('.selected').deselect();

                         });
                 }
             },
             enabled: false
         },
          {
              text: 'Validate Export',
              className: 'validate_export',
              action: function () {

                  if (myTable.rows('.selected').count() != 0) {
                      var data = myTable.rows('.selected').data()[0];
                      //upload .c2v
                      $.post(yourApp.Urls.validateUrl, { licenseid: data.LicenseID }, function (result) {
                          WinId = window.open('' + yourApp.Urls.validateUrl, '_blank');
                          WinId.document.open();
                          WinId.document.write(result);
                          WinId.document.close();
                          myTable.rows('.selected').deselect();
                      });

                  }
              },
              enabled: false
          },
         {
             text: 'Install 2015',
             className: 'import',
             action: function () {
                 if (myTable.rows('.selected').count() != 0) {
                     var datarow = myTable.rows('.selected').data()[0];
                     //upload .c2v
                     $.post(yourApp.Urls.createUrl, {
                         LicenseID: datarow.LicenseID,
                         ProductName: datarow.ProductName,
                         ArticleDetail: datarow.ArticleDetail,
                         Quantity: datarow.Quantity,
                         LicenseType: datarow.LicenseType,
                         MachineID: datarow.MachineID,
                         MaintEndDate: datarow.MED,
                         Version: datarow.Version
                     },
                         function (result) {
                             WinId = window.open('' + yourApp.Urls.createUrl, '_blank');
                             WinId.document.open();
                             WinId.document.write(result);
                             WinId.document.close();
                             myTable.rows('.selected').deselect();

                         });
                 }
             },
             enabled: false
         },
         {
             text: 'Install < 2015',
             className: 'license2014',
             action: function () {

                 if (myTable.rows('.selected').count() != 0) {
                     $("#machineid-dialog").dialog({
                         resizable: false,
                         height: 140,
                         modal: true,
                         buttons: {
                             "Install": function () { generate2014pwd.call(this, myTable.rows('.selected').data()[0], $("#machineid").val()) },
                             Cancel: function () {
                                 $(this).dialog("close");
                             }
                         }
                     });
                 }
             },
             enabled: false
         },
         {
             text: 'Password < 2015',
             className: 'pssw2014',
             action: function () {
                 if (myTable.rows('.selected').count() != 0)
                     generate2014pwd.call(this, myTable.rows('.selected').data()[0]);
             },
             enabled: false
         },
         {
             text: 'Upgrade 2014',
             className: 'upg2014',
             action: function () {
                 if (myTable.rows('.selected').count() != 0) {
                     var headers = {};
                     headers.LicenseID = myTable.rows('.selected').data()[0].LicenseID;
                     
                     var $psw = $("#password");
                     $psw.text("");
                     $psw.text("Would you like to upgrade the version to 2014?");
                     var pwdDialogConfig = {
                         modal: true,
                         width: 400,
                         height: "auto",
                         buttons: {
                             OK: function () {
                                 $(this).dialog("close");
                                 $.ajax({
                                     url: "../api/PasswordGenerator/Upgrade?licenseId=" + headers.LicenseID,
                                     type: 'GET',
                                     data: null,
                                     headers: headers,
                                     dataType: "json",
                                     success: function (result) {
                                         $("#pwd-dialog").prop('title', '');
                                         location.reload();
                                     }
                                 });
                             }
                         }
                     };
                     $("#pwd-dialog").prop('title', 'Upgrade 2014');
                     $("#pwd-dialog").dialog(pwdDialogConfig);
                 }
             },
             enabled: false
         }
            ]
        });



    myTable.buttons().container().insertBefore('#licenses_filter');

    myTable.on('select', function () {

        var license = myTable.rows('.selected').data()[0];

        $.ajax({
            url: yourApp.Urls.licenseStateUrl,
            data: 'LicenseId=' + license.LicenseID, // Send value of LicenseId
            dataType: 'json', // Choosing a JSON datatype
        }).done(function (data) {

            //check if license is 2015
            if (data.Version >= '2015') {

                var now = new Date();
                var isLocal = /^KID[0-9]+$/.test(data.MachineID);
                var isEval = /^EVAL[0-9]+$/.test(data.LicenseID);
                var isTdVar = data.PwdCode.startsWith("VA");
                var isL = /^L/.test(data.LicenseID);
                if (data.MaintEndDate != null) {
                    var maintenddate = parseJsonDate(data.MaintEndDate);
                }
                //check for export
                if (data.Installed == 1 && maintenddate >= now) {
                    if (isLocal && !isEval && !isTdVar && isL) {
                        myTable.buttons(['.export']).enable(true);
                    }
                }

                //check for validate
                if (data.Exported == 1) {
                    myTable.buttons(['.validate_export']).enable(true);
                }

                //check for import
                if (data.Import == 1) {
                    myTable.buttons(['.import']).enable(true);
                }
            } else {
                if (data.MachineID == "ABCDEFGH") {
                    myTable.buttons(['.license2014']).enable(true);
                } else {
                    if (data.LicenseType.toLowerCase() == "local")
                        myTable.buttons(['.pssw2014']).enable(true);
                    if (data.LicenseType.toLowerCase() == "local" && parseJsonDate(data.MaintEndDate) > new Date() &&
                        data.Version < '2014')
                        myTable.buttons(['.upg2014']).enable(true);
                }
            }

        })

    });

    myTable.on('deselect', function () {
        myTable.buttons(['.export']).disable();
        myTable.buttons(['.validate_export']).disable();
        myTable.buttons(['.import']).disable();
        myTable.buttons(['.license2014']).disable();
        myTable.buttons(['.pssw2014']).disable();
        myTable.buttons(['.upg2014']).disable()
    });


    myTable.on("draw", function () {
        var $body = $(myTable.table().body());
        $body.unhighlight();
        $body.highlight(myTable.search());
    });

    return myTable;

};