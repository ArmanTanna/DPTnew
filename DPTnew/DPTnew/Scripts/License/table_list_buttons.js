﻿String.prototype.replaceAll = function (search, replacement) {
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
  { column_number: 8, filter_default_label: "Select" },
  { column_number: 9, filter_default_label: "Select" }]);

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
            '<td>' + (d.MaxExport == -1 ? "unlimited" : d.MaxExport) + '</td>' +

            '<td><b>Export History:</b></td>' +
            '<td>' + d.TotExported + '</td>' +
         '</tr>' +
         '<tr>' +
            '<td><b>Legacy Product:</b></td>' +
            '<td>' + d.OriginalProduct + '</td>' +
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
                { data: "LicenseFlag" },
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
            licenseID: data.LicenseID
        };
        if (data.ArticleDetail.toLocaleLowerCase() == "pl" && data.MED)
            postData.expdata = data.MED.replaceAll("-", "") == "20280101" ? data.MED.replaceAll("-", "") : null;

        if (data.ArticleDetail.toLocaleLowerCase() === "asf" || data.ArticleDetail.toLocaleLowerCase() === "qsf"
            || data.ArticleDetail.toLocaleLowerCase() === "msf" || data.ArticleDetail.toLocaleLowerCase() === "tsf"
            || data.ArticleDetail.toLocaleLowerCase() === "wsf" || data.ArticleDetail.toLocaleLowerCase() === "asp") {
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
        //if (mid)
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
                         Licenseflag: datarow.LicenseFlag,
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
                              //if (headers.AccountNumber === "T3-0073628") {
                              //    $("#upgrade-choice").append($('<option></option>').val(2015).html(2015));
                              //    $("#upgrade-choice").append($('<option></option>').val(2016).html(2016));
                              //    $("#upgrade-choice").append($('<option></option>').val(2017).html(2017));
                              //}
                              var pwdDialogConfig = {
                                  modal: true,
                                  width: 200,
                                  height: "auto",
                                  buttons: {
                                      OK: function () {
                                          $(this).dialog("close");
                                          if ($("#upgrade-choice").val() !== headers.Version) {
                                              var $psw = $("#msg");
                                              $psw.text("");
                                              $psw.text("please wait...");
                                              var pwdDialogConfig = {
                                                  modal: true,
                                                  width: 400,
                                                  height: 250,
                                                  buttons: {
                                                      OK: function () {
                                                      }
                                                  }
                                              };
                                              $("#sysmsg-dialog").dialog(pwdDialogConfig);

                                              $.ajax({
                                                  url: yourApp.Urls.Upgrade + "?licenseId=" + headers.LicenseID + "&version=" + $("#upgrade-choice").val() + "&renew=" + 0,
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
                                              var text = "You have already the selected version! \n\n 選択したバージョンは取得済みです！";
                                              $psw.html(text.replace(/\n/g, '<br />'));
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
                                  url: yourApp.Urls.Upgrade + "?licenseId=" + headers.LicenseID + "&version=" + headers.Version + "&renew=" + 1,
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
                       text: btnTextLocalization[11],//V2CP,
                       className: 'v2cp',
                       action: function () {
                           if (myTable.rows('.selected').count() != 0) {
                               var data = myTable.rows('.selected').data()[0];
                               //upload .c2v
                               $.post(yourApp.Urls.V2CP, { licenseid: data.LicenseID }, function (result) {
                                   WinId = window.open('' + yourApp.Urls.V2CP, '_blank');
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
                         var llc = myTable.rows('.selected').data()[0];
                         if (llc.LicenseID === "L00052585") {
                             $("#machineidf").val("HUS12345");
                             $("#machineidf").prop("readonly", true);
                             $("#quantity").val(540);
                             $("#quantity").prop("readonly", true);
                         } else {
                             $("#machineidf").val("");
                             $("#machineidf").prop("readonly", false);
                             $("#quantity").val();
                             $("#quantity").prop("readonly", false);
                         }
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
                                         $psw.find("#counter").val(result.Counter);
                                         var pwdDialogConfig = {
                                             modal: true,
                                             width: 350,
                                             height: 500,
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
            myTable.buttons(['.export']).disable();
            myTable.buttons(['.validate_export']).disable();
            myTable.buttons(['.import']).disable();
            myTable.buttons(['.upgrade']).disable();
            myTable.buttons(['.renew']).disable();
            myTable.buttons(['.license2014']).disable();
            myTable.buttons(['.pssw2014']).disable();
            myTable.buttons(['.changeversion']).disable();
            myTable.buttons(['.modify']).disable();
            myTable.buttons(['.v2cp']).disable();
            if (data.Version >= '2015') {
                if (data.Blocked == 0) {
                    var now = new Date();
                    var isLocal = /^KID[0-9]+$/.test(data.MachineID);
                    var isBlu = /^BLU[0-9]+$/.test(data.MachineID);
                    var isRed = /^RED[0-9]+$/.test(data.MachineID);
                    var isBlk = /^BLK[0-9]+$/.test(data.MachineID);
                    var isTdVar = /^VA/.test(data.PwdCode);//data.PwdCode.startsWith("VA");
                    var isTdirect = /*/^IX/.test(data.PwdCode) || *//^IK/.test(data.PwdCode) || /^XP/.test(data.PwdCode) || /^IJ/.test(data.PwdCode);

                    if (data.MaintEndDate != null) {
                        var maintenddate = parseJsonDate(data.MaintEndDate);
                    }
                    //maintenance or not
                    if (data.Installed == 1 && maintenddate >= now) {
                        //check for export
                        if (isLocal && (!isTdVar || data.LicenseID == "L00000387" || data.LicenseID == "L00000699")
                            && !isTdirect && data.Export_Safenet == 1) {
                            myTable.buttons(['.export']).enable(true);
                        }
                        //upgrade-changeversion
                        if ((data.ChangeVersion_Safenet == 1 && data.ProductName.toLowerCase() !== "tdprofessionaledu")
                            || isBlu)
                            myTable.buttons(['.upgrade']).enable(true);

                        myTable.buttons(['.v2cp']).enable(true);
                    }
                    //renew
                    if (maintenddate >= now && data.Renewal_Safenet == 1 && data.Renew == 1 && data.Exported == 0 && (isLocal || isRed || isBlk)
                        && data.ArticleDetail.toLowerCase() != "pl"/*((isDemEduF || isLBZT) && (isLocal || isRed) 
                    && data.ArticleDetail.toLowerCase() != "pl" && data.Renew == 1) && !isGO && !isPPTE*/) {
                        myTable.buttons(['.renew']).enable(true);
                    }
                    //check for validate
                    if (data.Exported == 1 && maintenddate >= now) {
                        myTable.buttons(['.validate_export']).enable(true);
                        myTable.buttons(['.v2cp']).enable(true);
                    }
                    //check for install
                    if (data.Import == 1 && data.Install_Safenet == 1 && maintenddate >= now && data.MachineID.indexOf("ABCDEFGH") !== -1) {
                        myTable.buttons(['.import']).enable(true);
                    }
                }
            } else {
                if (data.Blocked == 0) {
                    //if (data.MaintEndDate != null) {
                    //    var maintenddate = parseJsonDate(data.MaintEndDate);
                    //}
                    if (parseJsonDate(data.MaintEndDate) > new Date() && data.ProductName.toLowerCase().indexOf("gbg") == -1) {
                        if (data.Install_Legacy == 1 && data.Import == 1 && data.MachineID == "ABCDEFGH") {
                            myTable.buttons(['.license2014']).enable(true);
                        } else {
                            //if (data.LicenseType.toLowerCase() == "local" || data.PwdCode.toLowerCase().indexOf("vs001") == -1)
                            myTable.buttons(['.pssw2014']).enable(true);
                            //maintenance or not
                            if (((data.ChangeVersion_Legacy == 1 && data.LicenseType.toLowerCase() !== "floating") || data.salesRep.toLowerCase() == "sener"))
                                myTable.buttons(['.changeversion']).enable(true);
                        }
                    }
                }
            }
            if (parseJsonDate(data.MaintEndDate) > new Date() && data.MachineID.indexOf("ABCDEFGH") !== -1
                && enmodifybutton && data.LicenseFlag.toLowerCase() == "pool") {
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
        myTable.buttons(['.v2cp']).disable();
    });


    myTable.on("draw", function () {
        var $body = $(myTable.table().body());
        $body.unhighlight();
        $body.highlight(myTable.search());
    });

    return myTable;

};