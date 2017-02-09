
dataSetChapters = [
["1","Home","What is new!"],
["2","BI Tools","Some Business Intelligence tools to support DPT Vars, with some instructions about their use."],
["3","Pricelist","Everything concerning prices, discounts and sales policies."],
["4","Products","Most interesting information about TT & TT packagings, with all products' correspondences."],
["5","Campaigns","Recap of currently running campaigns."],
["6","Licensing 2015","All you need to know about C2Vs, V2Cs and new licensing."],
["7","Licensing 2014","What is still important to remember about old licensing topics."],
["8","General","Some 'technicalities' to keep on hand."],
["9","ThinkDesign","About TD configuration."],
["10","GBG","Around our evergreen."],
["11","Sentinel","Notes about some tipical issues."],
["12","Miscellanea","Everything else."]
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
["1", "whatisnew", "What is new?", "l1c1p1", "Home", "Sales & Mktg", "160"],
["2", "index", "Index", "l1c1p2", "Home", "Sales & Mktg", "470"],
["3", "varguidebook", "VAR Guidebook", "l1c2p1", "BI Tools", "Sales & Mktg", "650"],
["4", "onlinelicensesreport", "Online Licenses Report", "l1c2p2", "BI Tools", "Sales & Mktg", "1220"],
["5", "bitools", "BI Tools", "l1c2p3", "BI Tools", "Sales & Mktg", "180"],
["6", "prices", "What about the prices?", "l1c3p1", "Pricelist", "Sales & Mktg", "800"],
["7", "mydiscount", "What is my discount?", "l1c3p2", "Pricelist", "Sales & Mktg", "780"],
["8", "licenseagreement", "What about the License Agreement?", "l1c3p3", "Pricelist", "Sales & Mktg", "780"],
["9", "pcbrokenlost", "What if my PC is broken or got lost/stolen?", "l1c3p4", "Pricelist", "Sales & Mktg", "1030"],
["10", "chw", "How does a CHW work?", "l1c3p5", "Pricelist", "Sales & Mktg", "440"],
["11", "usbdongles", "What about USB dongles?", "l1c3p6", "Pricelist", "Sales & Mktg", "570"],
["12", "tdsuite", "TD Suite at a glance!", "l1c4p1", "Products", "Product Mktg", "440"],
["13", "tdviewer", "TDViewerPlus vs TDViewerLight", "l1c4p2", "Products", "Product Mktg", "400"],
["14", "tddraftvstdbasevstdeng", "TDDrafting vs TDBase vs TDEngineering", "l1c4p3", "Products", "Product Mktg", "1130"],
["15", "tdengvsbigbrothers", "TDEngineering vs Big Brothers", "l1c4p4", "Products", "Product Mktg", "1950"],
["16", "tdengplusvstfree", "TDEngineeringPlus vs ThinkFree (EoL) vs TDTooling", "l1c4p5", "Products", "Product Mktg", "1140"],
["17", "tdstylingvstdprof", "TDStyling vs TDProfessional", "l1c4p6", "Products", "Product Mktg", "2060"],
["18", "tdproductsequivalence", "TD Products Equivalence", "l1c4p7", "Products", "Product Mktg", "1180"],
["19", "eolmodules", "What about End-of-Life modules?", "l1c4p8", "Products", "Product Mktg", "1510"],
["20", "all", "TD Suite in detail", "l1c4p9", "Products", "Product Mktg", "1200"],
["21", "ttsuite", "TT Suite at a glance!", "l1c4p10", "Products", "Product Mktg", "1520"],
["22", "trycampaign", "Try  & &hellip;", "l1c5p1", "Campaigns", "Sales & Mktg", "1160"],
["23", "convpack", "ConvPack", "l1c5p2", "Campaigns", "Sales & Mktg", "540"],
["24", "genginers", "Genginers", "l1c5p3", "Campaigns", "Sales & Mktg", "1250"],
["25", "thinkcompusers", "ThinkCompusers", "l1c5p4", "Campaigns", "Sales & Mktg", "1610"],
["26", "cadlabians", "Cadlabians", "l1c5p5", "Campaigns", "Sales & Mktg", "400"],
["27", "thinkstart", "ThinkStart", "l1c5p6", "Campaigns", "Sales & Mktg", "200"],
["28", "installtd2015", "The first time I install TD2015 on my PC", "l1c6p1", "Licensing 2015", "License Manager", "450"],
["29", "generatec2v", "How to generate a C2V file?", "l1c6p2", "Licensing 2015", "License Manager", "250"],
["30", "generatec2vdongle", "How to generate a C2V file using a dongle?", "l1c6p3", "Licensing 2015", "License Manager", "710"],
["31", "wrongc2v", "Why is my C2V file wrong?", "l1c6p4", "Licensing 2015", "License Manager", "330"],
["32", "getv2c", "How can I get my V2C file?", "l1c6p5", "Licensing 2015", "License Manager", "90"],
["33", "usev2c", "How can I use my V2C file?", "l1c6p6", "Licensing 2015", "License Manager", "1660"],
["34", "wrongv2c", "What if my V2C file is wrong?", "l1c6p7", "Licensing 2015", "License Manager", "320"],
["35", "floatinglicenses2015", "How can I manage floating licenses?", "l1c6p8", "Licensing 2015", "License Manager", "1460"],
["36", "td2015nostart", "TD2015 doesn&lsquo;t start, and neither does the previous version", "l1c6p9", "Licensing 2015", "License Manager", "200"],
["37", "exportlicense2015", "Can I export my licence 2015?", "l1c6p10", "Licensing 2015", "License Manager", "110"],
["38", "howtoexportlicense2015", "How can I export a license?", "l1c6p11", "Licensing 2015", "License Manager", "300"],
["39", "selfserviceexport", "How does the self-service export functionality work?", "l1c6p12", "Licensing 2015", "License Manager", "1000"],
["40", "dongleondifferentpc2015", "What if I want to use my dongle on a different PC?", "l1c6p13", "Licensing 2015", "License Manager", "120"],
["41", "exportlicense2014", "Can I export my licence 2014?", "l1c7p1", "Licensing 2014", "License Manager", "110"],
["42", "howtoexportlicense2014", "How can I export a license?", "l1c7p2", "Licensing 2014", "License Manager", "760"],
["43", "dongleondifferentpc2014", "What if I want to use my dongle on a different PC?", "l1c7p3", "Licensing 2014", "License Manager", "180"],
["44", "invalidlicense", "Why do I get an &lsquo;invalid license&lsquo; message?", "l1c7p4", "Licensing 2014", "License Manager", "120"],
["45", "graybuttonsinla", "Why do I get &lsquo;gray&lsquo; buttons in License Administrator?", "l1c7p5", "Licensing 2014", "License Manager", "90"],
["46", "floatinglicenses2014", "How can I manage floating licenses?", "l1c7p6", "Licensing 2014", "License Manager", "2890"],
["47", "downloadsoftware", "Where can I download the software from?", "l2c1p1", "General", "Customer Care", "100"],
["48", "configurewindows", "How can I configure Windows?", "l2c1p2", "General", "Customer Care", "800"],
["49", "correctinstallation", "What do I need for a correct installation?", "l2c1p3", "General", "Customer Care", "140"],
["50", "systemrequirements", "Which are the System Requirements?", "l2c1p4", "General", "Customer Care", "290"],
["51", "graphiccards", "Which graphics cards can I use?", "l2c1p5", "General", "Customer Care", "390"],
["52", "tdrunningslow", "Why is TD running slow on my PC?", "l2c2p1", "ThinkDesign", "Customer Care", "800"],
["53", "whattosaveconfigurationmanager", "What can I save with the Configuration Manager?", "l2c2p2", "ThinkDesign", "Customer Care", "560"],
["54", "copyconfigurationnewpc", "How can I copy TD configuration on a new PC?", "l2c2p3", "ThinkDesign", "Customer Care", "220"],
["55", "copyconfigurationnewversion", "How can I copy TD configuration on a new version?", "l2c2p4", "ThinkDesign", "Customer Care", "120"],
["56", "shareconfiguration", "How can I share TD configuration?", "l2c2p5", "ThinkDesign", "Customer Care", "250"],
["57", "appinconflicttd", "Which applications get in conflict with TD?", "l2c2p6", "ThinkDesign", "Customer Care", "390"],
["58", "beforereinstalling", "What should I do before reinstalling?", "l2c2p7", "ThinkDesign", "Customer Care", "470"],
["59", "registertdll", "How can I register the correct TD dll&lsquo;s?", "l2c2p8", "ThinkDesign", "Customer Care", "590"],
["60", "installgbg", "How do I install GBG?", "l2c3p1", "GBG", "Customer Care", "290"],
["61", "howtoactivategbg", "How do I activate GBG license?", "l2c3p2", "GBG", "Customer Care", "330"],
["62", "gbgdongles", "GBG and USB dongles", "l2c3p3", "GBG", "Customer Care", "190"],
["63", "howtomovegbg6or7", "How to move GBGDM 6.0/7.0 license if you lost your pwd?", "l2c3p4", "GBG", "Customer Care", "600"],
["64", "productsenabledoldgbg", "Which products are enabled in my old GBGDM installation?", "l2c3p5", "GBG", "Customer Care", "250"],
["65", "installgbgamec", "How do I install GBG Amec under Windows 7?", "l2c3p6", "GBG", "Customer Care", "300"],
["66", "sentinelfails", "What if Sentinel fails to start?", "l2c4p1", "Sentinel", "License Manager", "370"],
["67", "uninstallsentinel", "How to uninstall Sentinel?", "l2c4p2", "Sentinel", "License Manager", "630"],
["68", "sentinelversion", "Which version of Sentinel am I using?", "l2c4p3", "Sentinel", "License Manager", "540"],
["69", "errorsentinelinstaller", "Error installing Sentinel Installer", "l2c4p4", "Sentinel", "License Manager", "440"],
["70", "notteam", "I can't see TTeam inside TD!", "l2c5p1", "Miscellanea", "Customer Care", "210"],
["71", "errortteam", "Error starting TTeam", "l2c5p2", "Miscellanea", "Customer Care", "520"],
["72", "molddesignrequirements", "Which are the special requirements for Mold Design?", "l2c5p3", "Miscellanea", "Customer Care", "140"],
["73", "partsolutionserror", "PARTsolutions plug-in error", "l2c5p4", "Miscellanea", "Customer Care", "280"],
["74", "onlinehelplooksbad", "Why does the online help look so bad?", "l2c5p5", "Miscellanea", "Customer Care", "180"],
["75", "onlinehelpnotshowup", "Why does the online help not show up?", "l2c5p6", "Miscellanea", "Customer Care", "520"],
["76", "howtoinsertnewcase", "How do I open a New Case?", "l2c5p7", "Miscellanea", "Customer Care", "1000"],
["77", "chartgeography", "Where do they use TD?", "l3c1p1", "Stats", "Sales & Mktg", "500"]
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
	    if (row[3].substring(0, 2) == "l1") return '<a href="../../../Faq/Index_jp#' + row[1] + '" target="_top">' + data + '</a>';
	    else return '<a href="../../../Faq/Index2_jp#q' + row[1] + '" target="_top">' + data + '</a>';
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