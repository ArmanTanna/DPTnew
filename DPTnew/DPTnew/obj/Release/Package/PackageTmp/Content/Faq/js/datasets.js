var dataSet = [
["0", "name", "name", "TDDrafting", "TDBase", "TDEngineering", "TDEngineeringPlus", "TDTooling", "TDStyling", "TDProfessional", "TDViewerPlus", "thinkOM", "ThinkComputes", "TDModifier", "tdesign", "tfree", "tfreen", "thinkcreate", "thinkbasis", "thinkID", "thinkcompensator", "tdesignplus", "tdesignstudio"],
["5", "code", "name", "TDD", "TDB", "TDE", "TDE+", "TDT", "TDS", "TDP", "TDV", "TOM", "TBY", "TDO", "tds", "tfr", "tfn", "tcr", "tbs", "TID", "TCP", "tds+", "tdss"],
["10", "code", "code", "UD", "BO", "UE", "BY", "UT", "US", "UP", "UE", "BH", "BY", "UM", "BU", "BU+BR", "BS", "BD", "IL", "BI", "CO", "BJ", "BV"],
["100", "JK", "base", "JK", "JK", "JK", "JK", "JK", "JK", "JK", "JK", "JK", "JK", "JK", "JK", "JK", "JK", "JK", "JK", "JK", "JK", "JK", "JK"],
["105", "JD", "drafting+layout", "JD", "JD", "JD", "JD", "JD", "JD", "JD", "JD", "JD", "JD", "", "JD", "JD", "JD", "JD", "JD", "JD", "JD", "JD", "JD"],
["110", "JV", "save capability", "JV", "JV", "JV", "JV", "JV", "JV", "JV", "", "JV", "JV", "JV", "JV", "JV", "JV", "JV", "JV", "JV", "JV", "JV", "JV"],
["115", "JG", "surface base", "", "JG", "JG", "JG", "JG", "JG", "JG", "JG", "JG", "JG", "JG", "JG", "JG", "JG", "JG", "JG", "JG", "JG", "JG", "JG"],
["120", "JT", "surface intermediate", "", "JT", "JT", "JT", "JT", "JT", "JT", "JT", "JT", "JT", "JT", "JT", "JT", "JT", "JT", "JT", "JT", "JT", "JT", "JT"],
["125", "D2", "two entities distance map", "", "D2", "D2", "D2", "D2", "D2", "D2", "D2", "D2", "D2", "D2", "", "D2", "D2", "D2", "D2", "D2", "D2", "D2", "D2"],
["130", "JI", "iges+vda", "", "JI", "JI", "JI", "JI", "JI", "JI", "JI", "JI", "JI", "JI", "JI", "JI", "JI", "JI", "JI", "JI", "JI", "JI", "JI"],
["135", "JP", "thinkstep", "", "JP", "JP", "JP", "JP", "JP", "JP", "JP", "JP", "JP", "JP", "JP", "JP", "JP", "JP", "JP", "JP", "JP", "JP", "JP"],
["140", "JS", "solids+sketcher", "", "JS", "JS", "JS", "JS", "JS", "JS", "JS", "JS", "JS", "JS", "JS", "JS", "JS", "JS", "JS", "JS", "JS", "JS", "JS"],
["145", "AP", "pdf3d (hoops publish)", "", "", "AP", "AP", "AP", "AP", "AP", "AP", "", "AP", "AP", "AP", "AP", "AP", "AP", "AP", "AP", "AP", "AP", "AP"],
["150", "JQ", "hq rendering", "", "", "JQ", "JQ", "JQ", "JQ", "JQ", "JQ", "", "", "JQ", "JQ", "JQ", "JQ", "JQ", "JQ", "JQ", "JQ", "JQ", "JQ"],
["155", "JM", "sheet metal", "", "", "JM", "JM", "JM", "", "JM", "JM", "", "JM", "", "JM", "JM", "JM", "", "", "JM", "JM", "JM", "JM"],
["160", "JY", "assembly", "", "", "JY", "JY", "JY", "", "JY", "JY", "", "JY", "", "JY", "JY", "JY", "", "", "JY", "JY", "JY", "JY"],
["165", "J1", "break, create (assembly)", "", "", "J1", "J1", "J1", "", "J1", "J1", "", "J1", "", "J1", "J1", "J1", "", "", "J1", "J1", "J1", "J1"],
["170", "J2", "setcurrent (assembly)", "", "J2", "J2", "J2", "J2", "", "J2", "J2", "J2", "J2", "", "J2", "J2", "J2", "", "", "J2", "J2", "J2", "J2"],
["175", "JB", "symbolic ref & light reps", "", "", "JB", "JB", "JB", "", "JB", "JB", "JB", "JB", "", "JB", "JB", "JB", "", "", "JB", "JB", "JB", "JB"],
["180", "NT", "tubing", "", "", "NT", "NT", "NT", "", "NT", "NT", "", "NT", "", "NT", "NT", "NT", "", "", "NT", "NT", "NT", "NT"],
["185", "FR", "frame", "", "", "FR", "FR", "FR", "", "FR", "FR", "", "FR", "", "FR", "FR", "FR", "", "", "FR", "FR", "FR", "FR"],
["190", "MX", "edit part list", "", "", "MX", "MX", "MX", "", "MX", "MX", "", "", "", "", "", "", "", "", "", "", "", ""],
["195", "MY", "surface mold", "", "", "MY", "MY", "MY", "", "MY", "MY", "", "", "", "", "", "", "", "", "", "", "", ""],
["200", "JL", "static realistic rendering", "", "", "", "JL", "JL", "JL", "JL", "", "", "JL", "", "", "", "", "", "", "", "", "", ""],
["205", "JA", "surface advanced", "", "", "", "JA", "JA", "JA", "JA", "", "JA", "JA", "JA", "", "JA", "JA", "JA", "JA", "JA", "JA", "JA", "JA"],
["210", "JC", "advanced continuity", "", "", "", "", "JC", "JC", "JC", "", "", "", "JC", "", "", "JC", "JC", "JC", "JC", "JC", "JC", "JC"],
["215", "JZ", "gsm", "", "", "", "", "JZ", "JZ", "JZ", "", "JZ", "", "JZ", "", "JZ", "JZ", "JZ", "", "JZ", "JZ", "", "JZ"],
["220", "JW", "gsm plus", "", "", "", "", "JW", "JW", "JW", "", "", "", "JW", "", "JW", "JW", "JW", "", "JW", "JW", "", "JW"],
["225", "JH", "blending shape", "", "", "", "", "JH", "JH", "JH", "", "", "", "JH", "", "", "JH", "JH", "", "JH", "JH", "JH", "JH"],
["230", "JN", "zone draft", "", "", "", "", "JN", "JN", "JN", "", "", "", "JN", "", "", "JN", "", "", "JN", "JN", "JN", ""],
["235", "JO", "zone modeling", "", "", "", "", "JO", "JO", "JO", "", "", "", "JO", "", "", "JO", "", "", "JO", "JO", "", ""],
["240", "SR", "subdivision surfaces", "", "", "", "", "SR", "SR", "SR", "", "", "", "", "", "SR", "SR", "SR", "", "SR", "SR", "", "SR"],
["245", "ML", "mold design", "", "", "", "", "", "", "ML", "", "", "", "", "", "", "", "", "", "", "", "", ""],
["250", "JJ", "u-connect", "", "", "", "", "", "JJ", "JJ", "", "", "", "", "", "", "", "", "", "JJ", "JJ", "", ""],
["255", "JF", "i-interact", "", "", "", "", "", "JF", "JF", "", "", "", "", "", "", "", "", "", "JF", "JF", "", ""],
["260", "GT", "tdd target driven design", "", "", "", "", "", "GT", "GT", "", "", "", "", "", "", "", "", "", "GT", "GT", "", ""],
["265", "GR", "die design", "", "", "", "", "", "", "GR", "", "", "", "", "", "", "", "", "", "", "", "", ""],
["270", "EO", "reshape", "", "", "", "", "", "", "EO", "", "", "", "", "", "", "", "", "", "", "", "", ""],
["275", "EH", "super capping", "", "", "", "", "", "", "EH", "", "", "", "", "", "", "", "", "", "", "", "", ""],
["280", "EN", "mat", "", "", "", "", "", "", "EN", "", "", "", "", "", "", "", "", "", "", "EN", "", ""],
["285", "GN", "compensator", "", "", "", "", "", "", "GN", "", "", "", "", "", "", "", "", "", "", "GN", "", ""],
["290", "JX", "compensation+adaption", "", "", "", "", "", "", "J3", "", "", "", "", "", "", "", "", "", "", "", "", ""],
["300", "", "End-of-Life", "", "", "", "", "", "", "", "", "eol", "eol", "eol", "eol", "eol", "eol", "eol", "eol", "eol", "eol", "eol", "eol"]
];


var titles = dataSet[1]

function carica(mycase) {

    switch (mycase) {
        case "base": var buttons = [3, 4, 5]; break;
        case "bigbrothers": var buttons = [5, 6, 7, 8, 9]; break;
        case "free": var buttons = [6, 7, 15]; break;
        case "style": var buttons = [8, 9]; break;
        case "gengine": var buttons = [4, 5, 7, 11]; break;
        case "computes": var buttons = [5, 6, 12]; break;
        case "all": var buttons = [3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 14, 15, 17, 18, 20, 21, 22]; break;
    }

    var mywidth = 100 / (buttons.length + 3)

    var mywidthstring = mywidth + "%"
    var mywidthstringtitle = 2.5 * mywidth + "%"
    var mywidthstringcode = 0.5 * mywidth + "%"

    var btns = [];
    for (var i = 0; i < buttons.length; i++)
        btns.push({ extend: 'columnToggle', columns: buttons[i] });

    var ttls = [];
    for (var i = 0; i < titles.length; i++)
        ttls.push({ title: titles[i] });

    var clmnsdfs = [];
    clmnsdfs[0] = { "targets": 0, "visible": false, "searchable": false, } //sortorder
    clmnsdfs[1] = { "targets": 1, "width": mywidthstringcode, className: "mycenter" } //code
    clmnsdfs[2] = { "targets": 2, "width": "100%", className: "myleft" }   //name


    for (var i = 3; i < titles.length; i++) {
        a = buttons.indexOf(i)
        if (a == -1) clmnsdfs.push({ targets: i, visible: false });
        else clmnsdfs.push({ targets: i, width: mywidthstring, className: "mycenter" })
    }

    $(document).ready(function () {
        $('#mytable').DataTable({

            dom: 'Bfrtip',

            createdRow: function (row, data, dataIndex) {
                $('td', row).css('color', '#ffffff');
                switch (data[1]) {
                    case "JK":
                    case "JD":
                    case "JV": $('td', row).css('background-color', '#fb6522'); break;
                    case "JG":
                    case "JT":
                    case "D2":
                    case "JI":
                    case "JP": $('td', row).css('background-color', '#b25323'); break;
                    case "JS":
                    case "JQ":
                    case "AP": $('td', row).css('background-color', '#de011f'); break;
                    case "JM":
                    case "JY":
                    case "J1":
                    case "J2":
                    case "JB":
                    case "NT":
                    case "FR":
                    case "MX":
                    case "MY": $('td', row).css('background-color', '#bc000f'); break;
                    case "JL": $('td', row).css('background-color', '#7d50a5');
                        if (mycase == "base") $('td', row).css('display', 'none');
                        break;
                    case "JA": $('td', row).css('background-color', '#57823c');
                        if (mycase == "base") $('td', row).css('display', 'none');
                        break;
                    case "JC":
                    case "JZ":
                    case "JW":
                    case "JH":
                    case "JN":
                    case "JO":
                    case "SR": $('td', row).css('background-color', '#166521');
                        if (mycase == "base" || mycase == "computes")
                            $('td', row).css('display', 'none');
                        break;
                    case "ML": $('td', row).css('background-color', '#95d05d');
                        if (mycase == "base" || mycase == "free" || mycase == "gengine" || mycase == "computes")
                            $('td', row).css('display', 'none');
                        break;
                    case "GR":
                    case "EO":
                    case "EH":
                    case "EN":
                    case "JJ":
                    case "JF":
                    case "GT":
                    case "GN":
                    case "JX": $('td', row).css('background-color', '#7d7d7d');
                        if (mycase == "base" || mycase == "free" || mycase == "gengine" || mycase == "computes")
                            $('td', row).css('display', 'none');
                        break;
                    case "": $('td', row).css('color', 'red'); break;
                    default: $('td', row).css('display', 'none');
                }



            },

            columnDefs: clmnsdfs,

            buttons: btns,

            order: [[0, "asc"]],
            ordering: false,
            info: false,

            colReorder: true,
            paging: false,
            data: dataSet,
            columns: ttls
        });



    });

}


var dataSetPrices = [
["100", "UD", "TDDRAFTING", "CAD", "2500", "500", "1000", "300000", "50000", "90000"],
["102", "BO", "TDBASE", "CAD", "4000", "800", "1600", "600000", "90000", "180000"],
["104", "UE", "TDENGINEERING", "CAD", "6500", "1300", "2600", "1290000", "132000", "380000"],
["106", "BY", "TDENGINEERINGPLUS", "CAD", "8000", "1600", "3200", "na", "na", "na"],
["108", "UT", "TDTOOLING", "CAD", "9500", "1900", "3800", "1450000", "217000", "450000"],
["110", "ML+UT", "TDMOLDING", "CAD", "10000", "2000", "4000", "na", "na", "na"],
["112", "US", "TDSTYLING", "CAD", "10000", "2000", "4000", "1550000", "232000", "480000"],
["114", "UP", "TDPROFESSIONAL", "CAD", "12000", "2400", "4800", "1780000", "267000", "550000"],
["200", "BA", "TTEAMDOC <sup>(1)</sup>", "PLM", "500", "100", "200", "tbd", "tbd", "tbd"],
["210", "YT", "TTEAMPDM", "PLM", "1600", "320", "640", "tbd", "tbd", "tbd"],
["220", "TE", "TTEAMDEV", "PLM", "3500", "700", "1400", "tbd", "tbd", "tbd"],
["230", "AD", "TTEAMADD", "PLM", "2500", "500", "1000", "tbd", "tbd", "tbd"],
["240", "TW", "TTEAMPCB", "PLM", "500", "100", "200", "tbd", "tbd", "tbd"],
["250", "TM", "TTEAMPCM", "PLM", "7500", "1500", "3000", "tbd", "tbd", "tbd"],
["260", "WB", "WEBTEAM", "PLM", "10000", "2000", "4000", "tbd", "tbd", "tbd"],
["300", "AH", "TDXCHANGEREADER", "MORE", "2000", "500", "1000", "290000", "55000", "110000"],
["310", "IK+XP+IJ", "TDIRECTRW", "MORE", "4500", "0", "1800", "720000", "108000", "360000"],
["320", "TR", "THINKPRINT", "MORE", "5000", "1000", "2000", "na", "na", "na"],
["340", "TL", "TDVIEWERPLUS <sup>(2)</sup>", "MORE", "1000", "200", "400", "na", "na", "40000"],
["350", "DM", "GBGDM", "MORE", "1000", "200", "400", "na", "na", "na"]
];


var titlesPrices = ["sortorder", "code", "name", "family", "PL", "PLSS", "ASF", "PL", "PLSS", "ASF"]

function caricaPrices() {
    mycase = "euro"
    var ttls = [];
    for (var i = 0; i < titlesPrices.length; i++)
        ttls.push({ title: titlesPrices[i] });

    var clmnsdfs = [];
    clmnsdfs[0] = { targets: 0, visible: false, searchable: false } //sortorder
    clmnsdfs[1] = { targets: 1, visible: false, searchable: false } //code
    clmnsdfs[2] = { targets: 2, width: "61%", className: "myleft" }   //name
    clmnsdfs[3] = { targets: 3, visible: false, searchable: false } //family
    clmnsdfs[4] = {
        targets: 4, width: "13%", className: "myright",
        render: $.fn.dataTable.render.number(',', '.', 0, '', ' €')
    };
    clmnsdfs[5] = {
        targets: 5, width: "13%", className: "myright",
        render: $.fn.dataTable.render.number(',', '.', 0, '', ' €')
    };
    clmnsdfs[6] = {
        targets: 6, width: "13%", className: "myright",
        render: $.fn.dataTable.render.number(',', '.', 0, '', ' €')
    };
    clmnsdfs[7] = {
        targets: 7, width: "13%", className: "myright", visible: false,
        render: $.fn.dataTable.render.number(',', '.', 0, '', ' &yen;')
    };
    clmnsdfs[8] = {
        targets: 8, width: "13%", className: "myright", visible: false,
        render: $.fn.dataTable.render.number(',', '.', 0, '', ' &yen;')
    };
    clmnsdfs[9] = {
        targets: 9, width: "13%", className: "myright", visible: false,
        render: $.fn.dataTable.render.number(',', '.', 0, '', ' &yen;')
    };

    $(document).ready(function () {
        $('#mytable').DataTable({
            dom: 'Bfrtip',
            buttons: [
                //'pdf',
                'excel',
            {
                text: 'Euro',
                action: function showeuro() {
                    var table = $('#mytable').DataTable();
                    for (var i = 4 ; i < 7 ; i++) { table.column(i).visible(true, false); }
                    for (var i = 7 ; i < 10 ; i++) { table.column(i).visible(false, false); }
                    //	table.columns.adjust().draw( true ); // adjust column sizing and redraw
                }
            },
             {
                 text: 'Yen',
                 action: function showyen() {
                     var table = $('#mytable').DataTable();
                     for (var i = 7 ; i < 10 ; i++) { table.column(i).visible(true, false); }
                     for (var i = 4 ; i < 7 ; i++) { table.column(i).visible(false, false); }
                     //	table.columns.adjust().draw( true ); // adjust column sizing and redraw
                 }
             }
            ],
            columnDefs: clmnsdfs,
            data: dataSetPrices,
            order: [[0, "asc"]],
            ordering: false,
            info: false,
            paging: false,
            columns: ttls,
            drawCallback: function (settings) {
                var api = this.api();
                var rows = api.rows({ page: 'current' }).nodes();
                var last = null;
                api.column(3, { page: 'current' }).data().each(function (group, i) {
                    if (last !== group) {
                        $(rows).eq(i).before(
                               '<tr class="group" ><td>' + group + '</td><td class="myright">PL</td><td class="myright">PLSS</td><td class="myright">ASF</td></tr>'
                        );
                        last = group;
                    }
                });
            },

            fnInitComplete: function (oSettings, json) {
                // Find the wrapper and hide all thead
                $('#mytable').parents('.dataTables_wrapper').first().find('thead').hide();
            }

        });
    });

}