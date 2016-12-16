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

            '<td><b>Exported Num:</b></td>' +
            '<td>' + d.ExportedNum + '</td>' +

            '<td><b>Max Export:</b></td>' +
            '<td>' + d.MaxExport + '</td>' +

            '<td><b>Legacy Product:</b></td>' +
            '<td>' + d.OriginalProduct + '</td>' +
         '</tr>' +
         '<tr>' +
            '<td><b>Action:</b></td>' +
            '<td>' + d.Action + '</td>' +
         '</tr>' +
         '</table>';
}

function parseJsonDate(jsonDateString) {
    return new Date(parseInt(jsonDateString.replace('/Date(', '')));
}

var enmodifybutton;

var loadLicenseTable = function (dtConfig, superUser, enablemodify, enableadd, btnTextLocalization, DTmsgLocalization) {
    enmodifybutton = enablemodify;
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
        createdRow: function (row, data, index) {
            if (data.MED != null) {
                var now = new Date();
                var month = (now.getMonth() + 1);
                var day = now.getDate();
                if (month < 10)
                    month = "0" + month;
                if (day < 10)
                    day = "0" + day;
                var today = now.getFullYear() + '-' + month + '-' + day;
                if (data.MED < today)
                    $(row).addClass('highlight_red');
                if (data.MED >= today && data.MachineID.indexOf("ABCDEFGH") != -1)
                    $(row).addClass('highlight_blue');
            }
        },
        select: {
            style: 'single'
        },
        language: {
            search: DTmsgLocalization[2],
            lengthMenu: DTmsgLocalization[3],
            info: DTmsgLocalization[4],
            infoFiltered: DTmsgLocalization[5],
            paginate: {
                previous: DTmsgLocalization[0],
                next: DTmsgLocalization[1]
            },
            select: {
                rows: {
                    0: DTmsgLocalization[7],
                    1: DTmsgLocalization[6]
                }
            }
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
            prodotto: data.PwdCode,
            artDetail: data.ArticleDetail.toLocaleLowerCase(),
        };
        if (data.ArticleDetail.toLocaleLowerCase() == "pl" && data.MED)
            postData.expdata = data.MED.replaceAll("-", "") == "20280101" ? data.MED.replaceAll("-", "") : null;

        if (data.ArticleDetail.toLocaleLowerCase() === "asf" || data.ArticleDetail.toLocaleLowerCase() === "qsf"
            || data.ArticleDetail.toLocaleLowerCase() === "msf" || data.ArticleDetail.toLocaleLowerCase() === "tsf"
            || data.ArticleDetail.toLocaleLowerCase() === "wsf") {
            postData.tipo = "T";
            postData.expdata = data.MED ? data.MED.replaceAll("-", "") : "";
        }

        var $machineIdPopup = $(this);
        var onSuccess = function (result) {

            if ($machineIdPopup && $machineIdPopup.is(":visible")) {
                $machineIdPopup.dialog("close");
                $machineIdPopup.find("input#machineid").val("");
            }
            var $psw = $("#password");
            $psw.text("");
            $psw.html(result.Password ? result.Password.replaceAll(";", "\n") : result.PwdLine.replaceAll("\\\"", "\""));
            var pwdDialogConfig = {
                modal: true,
                width: 600,
                height: 250,
                buttons: {
                    OK: function () {
                        $(this).dialog("close");
                        location.reload();
                    }
                }
            };

            $("#pwd-dialog").dialog(pwdDialogConfig);
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
             text: btnTextLocalization[0],//'Export',
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
              text: btnTextLocalization[1],//'Validate Export',
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
             text: btnTextLocalization[2],//'Install 2015',
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
                      text: btnTextLocalization[8],//upgrade (> 2014) [Change Version]
                      className: 'upgrade',
                      action: function () {
                          if (myTable.rows('.selected').count() != 0) {
                              var headers = {};
                              headers = myTable.rows('.selected').data()[0];
                              var $psw = $("#upgver");
                              $psw.text("");
                              var pwdDialogConfig = {
                                  modal: true,
                                  width: 200,
                                  height: "auto",
                                  buttons: {
                                      OK: function () {
                                          $(this).dialog("close");
                                          if ($("#upgrade-choice").val() !== headers.Version) {
                                              $.ajax({
                                                  url: yourApp.Urls.Upgrade + "?licenseId=" + headers.LicenseID + "&version=" + $("#upgrade-choice").val(),
                                                  type: 'GET',
                                                  data: null,
                                                  headers: headers,
                                                  dataType: "json",
                                                  success: function (result) {
                                                      var $psw = $("#msg");
                                                      $psw.text("");
                                                      $psw.text(result);
                                                      var pwdDialogConfig = {
                                                          modal: true,
                                                          width: 400,
                                                          height: result ? 250 : "auto",
                                                          buttons: {
                                                              OK: function () {
                                                                  $(this).dialog("close");
                                                                  location.reload();
                                                              }
                                                          }
                                                      };
                                                      $("#sysmsg-dialog").dialog(pwdDialogConfig);
                                                  }
                                              });
                                          } else {
                                              var $psw = $("#msg");
                                              $psw.text("");
                                              $psw.text("You have already the selected version!");
                                              var pwdDialogConfig = {
                                                  modal: true,
                                                  width: 400,
                                                  height: 250,
                                                  buttons: {
                                                      OK: function () {
                                                          $(this).dialog("close");
                                                          location.reload();
                                                      }
                                                  }
                                              };
                                              $("#sysmsg-dialog").dialog(pwdDialogConfig);
                                          }
                                      },
                                      Cancel: function () {
                                          $(this).dialog("close");
                                      }
                                  }
                              };
                              $("#upgrade-dialog").dialog(pwdDialogConfig);
                              myTable.rows('.selected').deselect();
                          }
                      },
                      enabled: false
                  },
                  {
                      text: btnTextLocalization[10],//renew (> 2014) 
                      className: 'renew',
                      action: function () {
                          if (myTable.rows('.selected').count() != 0) {
                              var headers = {};
                              headers = myTable.rows('.selected').data()[0];
                              $.ajax({
                                  url: yourApp.Urls.Upgrade + "?licenseId=" + headers.LicenseID + "&version=" + headers.Version,
                                  type: 'GET',
                                  data: null,
                                  headers: headers,
                                  dataType: "json",
                                  success: function (result) {
                                      var $psw = $("#msg");
                                      $psw.text("");
                                      $psw.text(result);
                                      var pwdDialogConfig = {
                                          modal: true,
                                          width: 400,
                                          height: result ? 250 : "auto",
                                          buttons: {
                                              OK: function () {
                                                  $(this).dialog("close");
                                                  location.reload();
                                              }
                                          }
                                      };
                                      $("#sysmsg-dialog").dialog(pwdDialogConfig);
                                  }
                              });
                              myTable.rows('.selected').deselect();
                          }
                      },
                      enabled: false
                  },
         {
             text: btnTextLocalization[3],//'Install < 2015',
             className: 'license2014',
             action: function () {
                 if (myTable.rows('.selected').count() != 0) {
                     if (myTable.rows('.selected').data()[0].LicenseType == "local") {
                         $("#machineid-dialog").dialog({
                             resizable: false,
                             height: 140,
                             modal: true,
                             buttons: {
                                 "Install": function () {
                                     generate2014pwd.call(this, myTable.rows('.selected').data()[0], $("#machineid").val())
                                 },
                                 Cancel: function () {
                                     $(this).dialog("close");
                                 }
                             }
                         });
                     }
                     else {
                         $("#floating-dialog").dialog({
                             resizable: false,
                             height: 300,
                             modal: true,
                             buttons: {
                                 "Install": function () {
                                     var header = myTable.rows('.selected').data()[0];
                                     header.FlexType = $("#flexType-choice").val();
                                     header.MachineID = $("input[name=macid]").val();
                                     if ($("input[name=qty]").val() > 1)
                                         header.Quantity = $("input[name=qty]").val();
                                     if ($("input[name=vString]").val() !== "")
                                         header.Vend_String = $("input[name=vString]").val();
                                     header.PwdCode = header.PwdCode.split(",")[0] + "," + header.PwdCode.split(",")[1] + "," +
                                         header.MachineID + "," + header.Quantity + "," + (header.MED ? header.MED.replaceAll("-", "") : "")
                                     + "," + header.Vend_String + "," + header.FlexType + "," + (header.ArticleDetail == "pl" ? 2 : 0);
                                     generate2014pwd.call(this, header, header.MachineID)
                                 },
                                 Cancel: function () {
                                     $(this).dialog("close");
                                 }
                             }
                         });
                     }
                 }
             },
             enabled: false
         },
         {
             text: btnTextLocalization[4],//'Password < 2015',
             className: 'pssw2014',
             action: function () {
                 if (myTable.rows('.selected').count() != 0)
                     generate2014pwd.call(this, myTable.rows('.selected').data()[0]);
             },
             enabled: false
         },
         {
             text: btnTextLocalization[5],//'Change Version < 2015',
             className: 'changeversion',
             action: function () {
                 if (myTable.rows('.selected').count() != 0) {
                     var headers = {};
                     headers = myTable.rows('.selected').data()[0];
                     var $psw = $("#year");
                     $psw.text("");
                     var pwdDialogConfig = {
                         modal: true,
                         width: 200,
                         height: "auto",
                         buttons: {
                             OK: function () {
                                 $(this).dialog("close");
                                 if ($("#version-choice").val() > headers.Version) {
                                     $.ajax({
                                         url: "../api/PasswordGenerator/Upgrade?licenseId=" + headers.LicenseID + "&version=" + $("#version-choice").val(),
                                         type: 'GET',
                                         data: null,
                                         headers: headers,
                                         dataType: "json",
                                         success: function (result) {
                                             location.reload();
                                         }
                                     });
                                 } else {
                                     switch ($("#version-choice").val()) {
                                         case "2008":
                                             if (headers.LicenseType === "local")
                                                 headers.PwdCode = headers.PwdCode.substring(0, 2) + "D1";
                                             else
                                                 headers.PwdCode = headers.PwdCode.substring(0, 3) + "2008.1" + headers.PwdCode.substring(9, headers.PwdCode.length);
                                             break;
                                         case "2009":
                                             if (headers.LicenseType === "local")
                                                 headers.PwdCode = headers.PwdCode.substring(0, 2) + "E3";
                                             else
                                                 headers.PwdCode = headers.PwdCode.substring(0, 3) + "2009.1" + headers.PwdCode.substring(9, headers.PwdCode.length);
                                             break;
                                         case "2011":
                                             if (headers.LicenseType === "local")
                                                 headers.PwdCode = headers.PwdCode.substring(0, 2) + "G1";
                                             else
                                                 headers.PwdCode = headers.PwdCode.substring(0, 3) + "2011.1" + headers.PwdCode.substring(9, headers.PwdCode.length);
                                             break;
                                         case "2012":
                                             if (headers.LicenseType === "local")
                                                 headers.PwdCode = headers.PwdCode.substring(0, 2) + "H1";
                                             else
                                                 headers.PwdCode = headers.PwdCode.substring(0, 3) + "2012.1" + headers.PwdCode.substring(9, headers.PwdCode.length);
                                             break;
                                         case "2013":
                                             if (headers.LicenseType === "local")
                                                 headers.PwdCode = headers.PwdCode.substring(0, 2) + "I1";
                                             else
                                                 headers.PwdCode = headers.PwdCode.substring(0, 3) + "2013.1" + headers.PwdCode.substring(9, headers.PwdCode.length);
                                             break;
                                         case "2014":
                                             if (headers.LicenseType === "local")
                                                 headers.PwdCode = headers.PwdCode.substring(0, 2) + "J1";
                                             else
                                                 headers.PwdCode = headers.PwdCode.substring(0, 3) + "2014.1" + headers.PwdCode.substring(9, headers.PwdCode.length);
                                             break;
                                         default:
                                     }
                                     generate2014pwd.call(this, headers);
                                 }
                             }
                         }
                     };
                     $("#version-dialog").dialog(pwdDialogConfig);
                     myTable.rows('.selected').deselect();
                 }
             },
             enabled: false
         },
         {
             text: btnTextLocalization[6], //decrypt password
             className: 'decryptpwd',
             action: function () {
                 var $psw = $("#password-dialog");
                 var pwdDialogConfig = {
                     modal: true,
                     width: 400,
                     height: "auto",
                     buttons: {
                         OK: function () {
                             $(this).dialog("close");
                             if ($("#pwd").val() !== "") {
                                 $.ajax({
                                     url: "../api/PasswordGenerator/CheckMIDs?pwd=" + $("#pwd").val(),
                                     type: 'GET',
                                     data: null,
                                     headers: null,
                                     dataType: "json",
                                     success: function (result) {
                                         var $psw = $("#decrypt-dialog");
                                         $psw.find("#machineidp").val(result.Codice);
                                         $psw.find("#ancestor").val(result.Ancestor);
                                         $psw.find("#expdate").val(result.DataExp);
                                         $psw.find("#product").val(result.Prod);
                                         $psw.find("#resdays").val(result.Res_Days);
                                         $psw.find("#version").val(result.Vers);
                                         var pwdDialogConfig = {
                                             modal: true,
                                             width: 350,
                                             height: 450,
                                             buttons: {
                                                 OK: function () {
                                                     location.reload();
                                                 }
                                             }
                                         };
                                         $("#decrypt-dialog").dialog(pwdDialogConfig);
                                     }
                                 });
                             }
                         }
                     }
                 };
                 $("#password-dialog").dialog(pwdDialogConfig);
             },
             enabled: true
         },
         {
             text: btnTextLocalization[7],//'Modify',
             className: 'modify',
             action: function () {
                 if (myTable.rows('.selected').count() != 0) {
                     var data = myTable.rows('.selected').data()[0];
                     $.post(yourApp.Urls.DataUrl, {
                         AccountName: data.AccountName,
                         AccountNumber: data.AccountNumber,
                         Ancestor: data.Ancestor,
                         ArticleDetail: data.ArticleDetail,
                         ED: data.ED,
                         LicenseID: data.LicenseID,
                         LicenseType: data.LicenseType,
                         MSD: data.MSD,
                         MED: data.MED,
                         MachineID: data.MachineID,
                         ProductName: data.ProductName,
                         Quantity: data.Quantity,
                         SD: data.SD,
                         SalesRegion: data.SalesRegion,
                         SalesRep: data.SalesRep,
                         ServLicense1: data.ServLicense1,
                         Version: data.Version,
                         ExportedNum: data.ExportedNum,
                         MaxExport: data.MaxExport,
                         Note: data.Note,
                     },
                                     function (result) {
                                         var url = yourApp.Urls.DataUrl;
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
             text: btnTextLocalization[9],//'New',
             className: 'addnew',
             action: function () {
                 $.post(yourApp.Urls.AddLicenseUrl, {
                     LicenseID: null,
                     AccountNumber: null,
                     ProductName: null,
                     ArticleDetail: null,
                     Quantity: null,
                     LicenseType: null,
                     MachineID: null,
                     Ancestor: null,
                     StartDate: null,
                     EndDate: null,
                     MaintStartDate: null,
                     MaintEndDate: null,
                     Version: null,
                     OriginalProduct: null,
                     Installed: null,
                     Exported: null,
                     Import: null,
                     Vend_String: null,
                     FlexType: null,
                     ExportedNum: null,
                     MaxExport: null,
                     Note: null,
                 },
                                 function (result) {
                                     var url = yourApp.Urls.AddLicenseUrl;
                                     WinId = window.open(url, '_blank');
                                     WinId.document.open();
                                     WinId.document.write(result);
                                     WinId.document.close();
                                     myTable.rows('.selected').deselect();
                                 });
             },
             enabled: enableadd
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
                var isTdVar = /^VA/.test(data.PwdCode);//data.PwdCode.startsWith("VA");
                var isTdirect = /^IX/.test(data.PwdCode) || /^IK/.test(data.PwdCode) || /^XP/.test(data.PwdCode) || /^IJ/.test(data.PwdCode);
                var isL = /^L[0-9]+$/.test(data.LicenseID);
                var isTest = /^TEST[0-9]+$/.test(data.LicenseID);
                var isDem = /^DEM[0-9]+$/.test(data.LicenseID);
                var isPool = /^POOL[0-9]+$/.test(data.LicenseID);
                if (data.MaintEndDate != null) {
                    var maintenddate = parseJsonDate(data.MaintEndDate);
                }
                //check for export
                if (data.Installed == 1 && maintenddate >= now) {
                    if (isLocal && !isEval && !isTdVar && !isTdirect && !isPool && (isTest || isL)) {
                        myTable.buttons(['.export']).enable(true);
                        myTable.buttons(['.renew']).enable(true);
                        if (data.LicenseType.toLowerCase() !== "floating")
                            myTable.buttons(['.upgrade']).enable(true);
                    }
                }
                //check for DEMo licenses
                if (isDem && maintenddate >= now && isLocal) {
                    myTable.buttons(['.renew']).enable(true);
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
                    //if (data.LicenseType.toLowerCase() == "local")
                    myTable.buttons(['.pssw2014']).enable(true);
                    if (parseJsonDate(data.MaintEndDate) > new Date() && data.Version < '2015' /*&& data.LicenseType.toLowerCase() !== "floating"*/)
                        myTable.buttons(['.changeversion']).enable(true);
                }
            }
            if (parseJsonDate(data.MaintEndDate) > new Date() && enmodifybutton) {
                myTable.buttons(['.modify']).enable(true);
            }

        })

    });

    myTable.on('deselect', function () {
        myTable.buttons(['.export']).disable();
        myTable.buttons(['.validate_export']).disable();
        myTable.buttons(['.import']).disable();
        myTable.buttons(['.upgrade']).disable();
        myTable.buttons(['.renew']).disable();
        myTable.buttons(['.license2014']).disable();
        myTable.buttons(['.pssw2014']).disable();
        myTable.buttons(['.changeversion']).disable();
        myTable.buttons(['.modify']).disable();
    });


    myTable.on("draw", function () {
        var $body = $(myTable.table().body());
        $body.unhighlight();
        $body.highlight(myTable.search());
    });

    return myTable;

};