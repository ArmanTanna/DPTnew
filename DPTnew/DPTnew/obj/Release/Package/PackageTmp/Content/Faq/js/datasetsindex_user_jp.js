
dataSetChapters = [
["1","Home","What is new!"],
["2","Products","Most interesting information about TT & TT packagings, with all products' correspondences."],
["3","Licensing 2015","All you need to know about C2Vs, V2Cs and new licensing."],
["4","Licensing 2014","What is still important to remember about old licensing topics."],
["5","General","Some 'technicalities' to keep on hand."],
["6","ThinkDesign","About TD configuration."],
["7","GBG","Around our evergreen."],
["8","Sentinel","Notes about some tipical issues."],
["9","Miscellanea","Everything else."]
]

var chapters = ["sortorder","Title","Description"]

function caricaChapters() {
		
	var ttls = [];
	for (var i=0;i<chapters.length;i++)
	ttls.push({ title: chapters[i]});
	
	var clmnsdfs = [];
	clmnsdfs[0] = { targets: 0 , visible: false, searchable: false } //sortorder
	clmnsdfs[1] = { targets: 1 , width: "20%", className: "myleft", }  //title
	clmnsdfs[2] = { targets: 2 , width: "80%", className: "myleft" } //description
  
	$(document).ready(function() {
			$('#mytable').DataTable( {
				
				columnDefs: clmnsdfs,
				data: dataSetChapters,
				order: [[ 0, "asc" ]],
				ordering: true,
				info:     false,	
				paging: false,
				columns: ttls
			} );
	} );
	
}

var dataSetIndex = [
["1", "index_user", "Index", "l1c1p1", "Home", "Sales & Mktg", "470"],
["2", "tdsuite", "TD Suite at a glance!", "l1c2p1", "Products", "Product Mktg", "440"],
["3", "tdviewer", "TDViewerPlus vs TDViewerLight", "l1c2p2", "Products", "Product Mktg", "400"],
["4", "tdproductsequivalence", "TD Products Equivalence", "l1c2p3", "Products", "Product Mktg", "1180"],
["5", "ttsuite", "TT Suite at a glance!", "l1c2p4", "Products", "Product Mktg", "1520"],
["6", "installtd2015", "The first time I install TD2015 on my PC", "l1c3p1", "Licensing 2015", "License Manager", "450"],
["7", "generatec2v", "How to generate a C2V file?", "l1c3p2", "Licensing 2015", "License Manager", "250"],
["8", "generatec2vdongle", "How to generate a C2V file using a dongle?", "l1c3p3", "Licensing 2015", "License Manager", "710"],
["9", "wrongc2v", "Why is my C2V file wrong?", "l1c3p4", "Licensing 2015", "License Manager", "330"],
["10", "getv2c", "How can I get my V2C file?", "l1c3p5", "Licensing 2015", "License Manager", "90"],
["11", "usev2c", "How can I use my V2C file?", "l1c3p6", "Licensing 2015", "License Manager", "1660"],
["12", "wrongv2c", "What if my V2C file is wrong?", "l1c3p7", "Licensing 2015", "License Manager", "320"],
["13", "floatinglicenses2015", "How can I manage floating licenses?", "l1c3p8", "Licensing 2015", "License Manager", "1460"],
["14", "td2015nostart", "TD2015 doesn&lsquo;t start, and neither does the previous version", "l1c3p9", "Licensing 2015", "License Manager", "200"],
["15", "exportlicense2015", "Can I export my license 2015?", "l1c3p10", "Licensing 2015", "License Manager", "110"],
["16", "howtoexportlicense2015", "How can I export a license?", "l1c3p11", "Licensing 2015", "License Manager", "300"],
["17", "selfserviceexport", "How does the self-service export functionality work?", "l1c3p12", "Licensing 2015", "License Manager", "1000"],
["18", "dongleondifferentpc2015", "What if I want to use my dongle on a different PC?", "l1c3p13", "Licensing 2015", "License Manager", "120"],
["19", "exportlicense2014", "Can I export my license 2014?", "l1c4p1", "Licensing 2014", "License Manager", "110"],
["20", "howtoexportlicense2014", "How can I export a license?", "l1c4p2", "Licensing 2014", "License Manager", "760"],
["21", "dongleondifferentpc2014", "What if I want to use my dongle on a different PC?", "l1c4p3", "Licensing 2014", "License Manager", "180"],
["22", "invalidlicense", "Why do I get an &lsquo;invalid license&lsquo; message?", "l1c4p4", "Licensing 2014", "License Manager", "120"],
["23", "graybuttonsinla", "Why do I get &lsquo;gray&lsquo; buttons in License Administrator?", "l1c4p5", "Licensing 2014", "License Manager", "90"],
["24", "floatinglicenses2014", "How can I manage floating licenses?", "l1c4p6", "Licensing 2014", "License Manager", "2890"],
["25", "downloadsoftware", "Where can I download the software from?", "l2c1p1", "General", "Customer Care", "100"],
["26", "configurewindows", "How can I configure Windows?", "l2c1p2", "General", "Customer Care", "800"],
["27", "correctinstallation", "What do I need for a correct installation?", "l2c1p3", "General", "Customer Care", "140"],
["28", "systemrequirements", "Which are the System Requirements?", "l2c1p4", "General", "Customer Care", "290"],
["29", "graphiccards", "Which graphics cards can I use?", "l2c1p5", "General", "Customer Care", "390"],
["30", "tdrunningslow", "Why is TD running slow on my PC?", "l2c2p1", "ThinkDesign", "Customer Care", "800"],
["31", "whattosaveconfigurationmanager", "What can I save with the Configuration Manager?", "l2c2p2", "ThinkDesign", "Customer Care", "560"],
["32", "copyconfigurationnewpc", "How can I copy TD configuration on a new PC?", "l2c2p3", "ThinkDesign", "Customer Care", "220"],
["33", "copyconfigurationnewversion", "How can I copy TD configuration on a new version?", "l2c2p4", "ThinkDesign", "Customer Care", "120"],
["34", "shareconfiguration", "How can I share TD configuration?", "l2c2p5", "ThinkDesign", "Customer Care", "250"],
["35", "appinconflicttd", "Which applications get in conflict with TD?", "l2c2p6", "ThinkDesign", "Customer Care", "390"],
["36", "beforereinstalling", "What should I do before reinstalling?", "l2c2p7", "ThinkDesign", "Customer Care", "470"],
["37", "registertdll", "How can I register the correct TD dll&lsquo;s?", "l2c2p8", "ThinkDesign", "Customer Care", "590"],
["38", "installgbg", "How do I install GBG?", "l2c3p1", "GBG", "Customer Care", "290"],
["39", "howtoactivategbg", "How do I activate GBG license?", "l2c3p2", "GBG", "Customer Care", "330"],
["40", "gbgdongles", "GBG and USB dongles", "l2c3p3", "GBG", "Customer Care", "190"],
["41", "howtomovegbg6or7", "How to move GBGDM 6.0/7.0 license if you lost your pwd?", "l2c3p4", "GBG", "Customer Care", "600"],
["42", "productsenabledoldgbg", "Which products are enabled in my old GBGDM installation?", "l2c3p5", "GBG", "Customer Care", "250"],
["43", "installgbgamec", "How do I install GBG Amec under Windows 7?", "l2c3p6", "GBG", "Customer Care", "300"],
["44", "sentinelfails", "What if Sentinel fails to start?", "l2c4p1", "Sentinel", "License Manager", "370"],
["45", "uninstallsentinel", "How to uninstall Sentinel?", "l2c4p2", "Sentinel", "License Manager", "630"],
["46", "sentinelversion", "Which version of Sentinel am I using?", "l2c4p3", "Sentinel", "License Manager", "540"],
["47", "errorsentinelinstaller", "Error installing Sentinel Installer", "l2c4p4", "Sentinel", "License Manager", "440"],
["48", "notteam", "I can't see TTeam inside TD!", "l2c5p1", "Miscellanea", "Customer Care", "210"],
["49", "errortteam", "Error starting TTeam", "l2c5p2", "Miscellanea", "Customer Care", "520"],
["50", "molddesignrequirements", "Which are the special requirements for Mold Design?", "l2c5p3", "Miscellanea", "Customer Care", "140"],
["51", "partsolutionserror", "PARTsolutions plug-in error", "l2c5p4", "Miscellanea", "Customer Care", "280"],
["52", "onlinehelplooksbad", "Why does the online help look so bad?", "l2c5p5", "Miscellanea", "Customer Care", "180"],
["53", "onlinehelpnotshowup", "Why does the online help not show up?", "l2c5p6", "Miscellanea", "Customer Care", "520"],
["54", "howtoinsertnewcase", "How do I open a New Case?", "l2c5p7", "Miscellanea", "Customer Care", "1000"],
["55", "chartgeography", "Where do they use TD?", "l3c1p1", "Stats", "Sales & Mktg", "500"]
]

var titles = ["sortorder","ancora","Title","cxpy","Chapter","Owner"]

function caricaIndex() {
		
	var ttls = [];
	for (var i=0;i<titles.length;i++)
	ttls.push({ title: titles[i]});
	
	var clmnsdfs = [];
	clmnsdfs[0] = { targets: 0 , visible: false, searchable: false } //sortorder
	clmnsdfs[1] = { targets: 1 , visible: false, searchable: false } //ancora
	clmnsdfs[2] = { targets: 2 , width: "70%", className: "myleft", 
	render: function ( data, type, row ) { 
	    if (row[3].substring(0, 2) == "l1") return '<a href="../../../FaqUser/Index_jp#' + row[1] + '" target="_top">' + data + '</a>';
		else return '<a href="../../../FaqUser/Index2_jp#q' + row[1] + '" target="_top">' + data + '</a>';
		}
	}  //title
	clmnsdfs[3] = { targets: 3 , visible: false, searchable: false } //cxpy
	clmnsdfs[4] = { targets: 4 , width: "30%", className: "myright" } //chapter
	clmnsdfs[5] = { targets: 5 , visible: false, searchable: false } //owner
	// "render": function ( data, type, row ) {
        //            return data +' ('+ row[3]+')';
  
	$(document).ready(function() {
			$('#mytable').DataTable( {
				
				columnDefs: clmnsdfs,
				data: dataSetIndex,
				order: [[ 0, "asc" ]],
				ordering: true,
				info:     false,	
				paging: true,
				columns: ttls,
				lengthChange: false
			} );
	} );
	
}