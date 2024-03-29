
dataSetChapters = [
["1","Home","What is new!"],
["2","BI Tools","Some Business Intelligence tools to support DPT Vars, with some instructions about their use."],
["3","Pricelist","Everything concerning prices, discounts and sales policies."],
["4","Products","Most interesting information about TD & TT packages, with all products' correspondences."],
["5","Campaigns","Recap of currently running campaigns."],
["6","Licensing 2015","All you need to know about C2Vs, V2Cs and new licensing."],
["7","Licensing 2014","What is still important to remember about old licensing topics."],
["8","General","Some 'technicalities' to keep on hand."],
["9","ThinkDesign","About TD configuration."],
["10","GBG","Around our evergreen."],
["11","Sentinel","Notes about some typical issues."],
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
["1","whatisnew","1. What is new?","l1c1p1","Home","Sales & Mktg","160"],
["2","index","2. Index","l1c1p2","Home","Sales & Mktg","470"],
["3","varguidebook","1. VAR Guidebook","l1c2p1","BI Tools","Sales & Mktg","650"],
["4","onlinelicensesreport","2. Online Licenses Report","l1c2p2","BI Tools","Sales & Mktg","1220"],
["5","bitools","3. BI Tools","l1c2p3","BI Tools","Sales & Mktg","180"],
["6","prices","1. What about the prices?","l1c3p1","Pricelist","Sales & Mktg","800"],
["7","mydiscount","2. What is my discount?","l1c3p2","Pricelist","Sales & Mktg","780"],
["8","licenseid&accountstatus","3. How to distinguish License ID and Account Status kinds?","l1c3p3","Pricelist","Sales & Mktg","1000"],
["9","licenseagreement","4. What about the License Agreement?","l1c3p4","Pricelist","Sales & Mktg","850"],
["10","pcbrokenlost","5. What if my PC is broken or got lost/stolen?","l1c3p5","Pricelist","Sales & Mktg","1200"],
["11","chw","6. How does a CHW work?","l1c3p6","Pricelist","Sales & Mktg","440"],
["12","exportpolicy","7. Export policy","l1c3p7","Pricelist","Sales & Mktg","1000"],
["13","usbdongles","8. What about USB dongles?","l1c3p8","Pricelist","Sales & Mktg","570"],
["14","crack","9. How to deal with irregular licenses?","l1c3p9","Pricelist","Sales & Mktg","500"],
["15","tdsuite","1. TD Suite at a glance!","l1c4p1","Products","Product Mktg","440"],
["16","tddraftvstdbasevstdeng","2. TDDrafting vs TDBase vs TDEngineering","l1c4p2","Products","Product Mktg","1130"],
["17","tdengvsbigbrothers","3. TDEngineering vs Big Brothers","l1c4p3","Products","Product Mktg","1950"],
["18","tdengplusvstfree","4. TDEngineeringPlus vs ThinkFree (EoL) vs TDTooling","l1c4p4","Products","Product Mktg","1140"],
["19","tdstylingvstdprof","5. TDStyling vs TDProfessional","l1c4p5","Products","Product Mktg","2060"],
["20","tdproductsequivalence","6. TD Products Equivalence","l1c4p6","Products","Product Mktg","1180"],
["21","eolmodules","7. What about End-of-Life modules?","l1c4p7","Products","Product Mktg","1510"],
["22","all","8. TD Suite in detail","l1c4p8","Products","Product Mktg","1200"],
["23","ttsuite","9. TT Suite at a glance!","l1c4p9","Products","Product Mktg","1520"],
["24","releasenotes","10. Release Notes","l1c4p10","Products","Product Mktg","600"],
["25","trycampaign","1. Try & &hellip;","l1c5p1","Campaigns","Sales & Mktg","1160"],
["26","genginers","2. Genginers","l1c5p2","Campaigns","Sales & Mktg","1250"],
["27","thinkcompusers","3. ThinkCompusers","l1c5p3","Campaigns","Sales & Mktg","1610"],
["28","cadlabians","4. Cadlabians","l1c5p4","Campaigns","Sales & Mktg","400"],
["29","thinkstart","5. ThinkStart","l1c5p5","Campaigns","Sales & Mktg","200"],
["30","installtd2015","1. The first time I install TD on my PC","l1c6p1","Licensing 2015","License Manager","450"],
["31","generatec2v","2. How to generate a C2V file?","l1c6p2","Licensing 2015","License Manager","350"],
["32","generatec2vdongle","3. How to generate a C2V file using a dongle?","l1c6p3","Licensing 2015","License Manager","710"],
["33","wrongc2v","4. Why is my C2V file wrong?","l1c6p4","Licensing 2015","License Manager","350"],
["34","getv2c","5. How can I get my V2C file?","l1c6p5","Licensing 2015","License Manager","90"],
["35","usev2c","6. How can I use my V2C file?","l1c6p6","Licensing 2015","License Manager","1200"],
["36","wrongv2c","7. What if my V2C file is wrong?","l1c6p7","Licensing 2015","License Manager","600"],
["37","floatinglicenses2015","8. How can I manage floating licenses?","l1c6p8","Licensing 2015","License Manager","1460"],
["38","td2015nostart","9. TD2015 doesn&lsquo;t start, and neither does the previous version","l1c6p9","Licensing 2015","License Manager","200"],
["39","exportlicense2015","10. Can I export my licence 2015?","l1c6p10","Licensing 2015","License Manager","110"],
["40","selfserviceexport","11. How does the self-service export functionality work?","l1c6p11","Licensing 2015","License Manager","1000"],
["41","dongleondifferentpc2015","12. What if I want to use my dongle on a different PC?","l1c6p12","Licensing 2015","License Manager","120"],
["42","exportlicense2014","1. Can I export my licence 2014?","l1c7p1","Licensing 2014","License Manager","110"],
["43","howtoexportlicense2014","2. How can I export a license?","l1c7p2","Licensing 2014","License Manager","760"],
["44","dongleondifferentpc2014","3. What if I want to use my dongle on a different PC?","l1c7p3","Licensing 2014","License Manager","200"],
["45","invalidlicense","4. Why do I get an &lsquo;invalid license&lsquo; message?","l1c7p4","Licensing 2014","License Manager","120"],
["46","graybuttonsinla","5. Why buttons in License Administrator are grayed out?","l1c7p5","Licensing 2014","License Manager","90"],
["47","floatinglicenses2014","6. How can I manage floating licenses?","l1c7p6","Licensing 2014","License Manager","2890"],
["48","downloadsoftware","1. Where can I download the software from?","l2c1p1","General","Customer Care","100"],
["49","configurewindows","2. How can I configure Windows?","l2c1p2","General","Customer Care","800"],
["50","correctinstallation","3. What do I need for a correct installation?","l2c1p3","General","Customer Care","140"],
["51","systemrequirements","4. Which are the System Requirements?","l2c1p4","General","Customer Care","290"],
["52","graphiccards","5. Which graphics cards can I use?","l2c1p5","General","Customer Care","390"],
["53","tdrunningslow","1. Why is TD running slow on my PC?","l2c2p1","ThinkDesign","Customer Care","800"],
["54","whattosaveconfigurationmanager","2. What can I save with the Configuration Manager?","l2c2p2","ThinkDesign","Customer Care","560"],
["55","copyconfigurationnewpc","3. How can I copy TD configuration on a new PC?","l2c2p3","ThinkDesign","Customer Care","220"],
["56","copyconfigurationnewversion","4. How can I copy TD configuration on a new version?","l2c2p4","ThinkDesign","Customer Care","120"],
["57","shareconfiguration","5. How can I share TD configuration?","l2c2p5","ThinkDesign","Customer Care","250"],
["58","appinconflicttd","6. Which applications get in conflict with TD?","l2c2p6","ThinkDesign","Customer Care","390"],
["59","beforereinstalling","7. What should I do before reinstalling?","l2c2p7","ThinkDesign","Customer Care","470"],
["60","registertdll","8. How can I register the correct TD dll&lsquo;s?","l2c2p8","ThinkDesign","Customer Care","590"],
["61","installgbg","1. How do I install GBG?","l2c3p1","GBG","Customer Care","290"],
["62","howtoactivategbg","2. How do I activate GBG license?","l2c3p2","GBG","Customer Care","330"],
["63","gbgdongles","3. GBG and USB dongles","l2c3p3","GBG","Customer Care","190"],
["64","howtomovegbg6or7","4. How to move GBGDM 6.0/7.0 license if you lost your pwd?","l2c3p4","GBG","Customer Care","600"],
["65","productsenabledoldgbg","5. Which products are enabled in my old GBGDM installation?","l2c3p5","GBG","Customer Care","250"],
["66","installgbgamec","6. How do I install GBG Amec under Windows 7?","l2c3p6","GBG","Customer Care","300"],
["67","sentinelfails","1. What if Sentinel fails to start?","l2c4p1","Sentinel","License Manager","370"],
["68","uninstallsentinel","2. How to uninstall Sentinel?","l2c4p2","Sentinel","License Manager","630"],
["69","sentinelversion","3. Which version of Sentinel am I using?","l2c4p3","Sentinel","License Manager","540"],
["70","errorsentinelinstaller","4. Error installing Sentinel Installer","l2c4p4","Sentinel","License Manager","440"],
["71","notteam","1. I can't see TTeam inside TD!","l2c5p1","Miscellanea","Customer Care","210"],
["72","errortteam","2. Error starting TTeam","l2c5p2","Miscellanea","Customer Care","520"],
["73","molddesignrequirements","3. Which are the special requirements for Mold Design?","l2c5p3","Miscellanea","Customer Care","140"],
["74","partsolutionserror","4. PARTsolutions plug-in error","l2c5p4","Miscellanea","Customer Care","280"],
["75","onlinehelplooksbad","5. Why does the online help look so bad?","l2c5p5","Miscellanea","Customer Care","180"],
["76","onlinehelpnotshowup","6. Why does the online help not show up?","l2c5p6","Miscellanea","Customer Care","520"],
["77","howtoinsertnewcase","7. How do I open a New Case?","l2c5p7","Miscellanea","Customer Care","1000"],
["78","chartgeography","1. Where do they use TD?","l3c1p1","Stats","Sales & Mktg","500"]
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