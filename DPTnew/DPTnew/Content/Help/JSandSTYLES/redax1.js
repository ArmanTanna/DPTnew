//
/*
Gestione dell'errore.
Nel caso di IE6sp1, quando la doc NON ・installata su un disco locale (cio・se ・in rete) si verificano errori JavaScript a causa delle nuove restrizioni di security inserite a tradimento da Microsoft.
Al posto di un errore JavaScript, visualizziamo un messaggio in cui si informa l'utente che questa situazione si verifica se l'installazione non Elocale.
*/
function errorHandler() {
if(this.location.href.match('english')) {
        alert("To view the desired page, install the documentation to your machine.\nIf the problem persists, contact think3 Customer Care (e-mail: thinkcare@think3.com).");
}
if(this.location.href.match('italian')) {
	    alert("Per visualizzare la pagina di guida desiderata, installare la documentazione sulla propria macchina.\nSe il problema persiste, contattare il Customer Care (Assistenza clienti) di think3 (e-mail: thinkcare@think3.com).");
}
if(this.location.href.match('french')) {
        alert("Pour visualiser la page d'Aide d駸ir馥, installer localement la documentation sur la machine.\nSi le probl鑪e persiste, contacter le Customer Care (Assistance Clients) de think3 (e-mail: thinkcare@think3.com).");
}
if(this.location.href.match('german')) {
        alert("Installieren Sie zur Anzeige der gew�nschten Hilfe-Seite die Dokumentation auf Ihrem Rechner.\nWenn das Problem bestehen bleibt, wenden Sie sich an den think3 Customer Care (Kundendienst) (e-mail: thinkcare@think3.com).");
}
if(this.location.href.match('japanese')) {
        alert("目的のページを表示するには、お使いのコンピュータにドキュメントをインストールしてください。\n問題が再発する場合は、think3 Customer Care（e-mail: thinkcare@think3.com）にお問い合わせください。");
}
	 return true;
}
window.onerror = errorHandler;
//
//
//
	var bitToolBar = 0;
	var bitLocation = 0
	var bitDirectories = 0;
	var bitStatus = 8;
	var bitMenubar = 0;
	var bitScrollBars = 32;
	var bitResizable = 64;

//
//
/* Redax.js (formerly runner.js, runner_60RelNotes.js): This scripts is used 
along with the Redax.DLL to run an external program or open an external file 
from inside an HTML page. 
Redax.DLL by Alex Martelli (alma@cadlab.it), Think3 Inc. January 1999.
Script By Nicola Binaghi, 2001,2002, 24-Jan-03*/
// Auxiliary function: it just creates an Array object.
function CreateArray(n)
{
	this.length = n;
	for (var i = 1; i <= n; i++) {this[i]=0}

	return this;
}
// Global variables...

/* This is an array of Windows Registry Keys.
    These registry keys should contain either an UNC path like:
		\\server\directory\
	or an URL like:
		http://server.domain.com/
	This UNC/URL is a "base" path that is used to build up a complete and valid HTML link.
*/
var RegistryKeys = new CreateArray(3);
// LocalAVIDir is the local position of the requested file, like:
// "e:\" (CD-ROM) or "c:\data\avi" (hard disk). NOT IN USE
RegistryKeys[1] = 'SOFTWARE/think3/Documentation/8.2/LocalAVIDir'; // location  on LAN server. NOT IN USE
RegistryKeys[2] = 'SOFTWARE/think3/Documentation/8.2/LANAVIDir'; 
RegistryKeys[3] = 'SOFTWARE/think3/Documentation/8.2/WebAVIDir';// location on a web server (URL). NOT IN USE
var TotalTries = 4;
var DefaultDir = 'http://think3.com/Avi/'; //Default "Base" URL (Hard Encoded)NOT IN USE
var CurrentTry;
var BaseDir;
var AttemptedCommand = "Unknown";// Attempted command : mainly debug purpose...
//
//*********************************************************************************
// bDLLOK keeps track of the availability of the requested DLLs:
// if just one of them is missing, bDLLOK is FALSE.
//
function SignalNoDLL()
{
	bDLLOK = false;
	TotalTries = 1;
}
/* Re-written by Nicola Binaghi 1999. Revised and modified 2000, and April 2001, January 2003
*/
function OpenHelp(sPage, sCHMorDir, WindowType, Product, CHMfile)
		
{
	var bDLLOK = true; 
    if(Product == '') {
		Product = "thinkdesign";
	}
	//alert("Err = " + TheForm.Redirector.LastError)
	var SWCurrentVersion = TheForm.Redirector.ReadRegistry('SOFTWARE/think3/Documentation/CurrentVersion');
	var DefaultTutorialDir = 'http://thinkcare.think3.com/';
	var sDefaultParentDir = 'tutorial';
	var sBaseDir = '';
	var sParentDir = '';
	var sProduct = '';
	var sAuxProd = 'SOFTWARE/think3/Documentation/' + SWCurrentVersion;
	sAuxProd = sAuxProd + "\/" + Product + "\/";
	var sAuxFile = ''
	var sType = ''
	var sFile = CHMfile;
	var DefaultTutorialLanguage = 'English';
	var sLanguageDir = '';
	var sAuxTutorialFile = sAuxProd + "Help";
	var sTutorialFile = TheForm.Redirector.ReadRegistry(sAuxTutorialFile);
	var sAuxTutorialFile2 = sAuxProd + CHMfile;
	// var sPrologue = "ms-its:"; // Compliant with MS IE 6.0 or later (accordingly to MS...)
	var sPrologue = "mk:@MSITStore:"; // Compliant with MS IE 3.0
	var ToBeOpened = '';	
    var RememberChm = '';
//
// check Registry
//
        A_DocLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/" + "LanguageDir";
        DocLanguageDir = TheForm.Redirector.ReadRegistry(A_DocLanguageDir);
		A_sBaseDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/" + "HelpDirCD";
		sBaseDir = TheForm.Redirector.ReadRegistry(A_sBaseDir);
		if(TheForm.Redirector.LastError != 0) {
			bDLLOK = false;
			TheForm.Redirector.LastError = 0;
		}
//		
if(bDLLOK) {
	if (sCHMorDir) {
		alert('Not yet implemented');
	} else {
	    if(CHMfile == '') {
			sFile = sTutorialFile;
			sParentDir = sDefaultParentDir;
		}
		else {
		//02May01
		    if(CHMfile == 'ReleaseNotes') {
			   RememberChm = 'ReleaseNotes';
			   sAuxFile = sAuxProd + 'ReleaseNotes';
			   sFile = TheForm.Redirector.ReadRegistry(sAuxFile);
			}
			else {
				sFile = CHMfile;
				sParentDir = sDefaultParentDir;
			}
		}		
	}
//Determines the language depending on the chm file name
		if(Product == "thinkdesign") {
		    A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/thinkdesign\/" + "Language";
			sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
		}
		else {
			if(Product == "thinkteam") {
			   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/thinkteam\/" + "Language";
				sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
			}
			else {
				if(Product == "thinkreal") {
				A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/thinkreal\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				//else {
				if(Product == "cladmin") {
					A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/cladmin\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "thinkprint") {
				   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/thinkprint\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "ttwfedit") {
				   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/ttwfedit\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "switch") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/switch\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "instdb") {
					  A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/instdb\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "insthw") {
					  A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/insthw\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "thinkparts") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/thinkparts\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}	
				if(Product == "dimensio") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/dimensio\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "draft") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/draft\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "claspro") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/claspro\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "selsna") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/selsna\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "forcus") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/forcus\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "filew") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/filew\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "solids") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/solids\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "image") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/image\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "view") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/view\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "sobj") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/sobj\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "curves") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/curves\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "surfaces") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/surfaces\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "pb") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/pb\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "thinkcatia") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/thinkcatia\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "thinkparasolid") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/thinkparasolid\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "converters") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/converters\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}
				if(Product == "thinklight") {
					   A_sLanguageDir = "SOFTWARE\/think3\/Documentation\/" + SWCurrentVersion + "\/thinklight\/" + "Language";
					sLanguageDir = TheForm.Redirector.ReadRegistry(A_sLanguageDir);
				}		
			}		
	
		}
		ToBeOpened = sPrologue + sBaseDir + "\/" + DocLanguageDir + "\/" + sFile + "::" + "\/" + sLanguageDir + sPage;
//-------------------------------------------------------------------------------
     if((sPage == '')&&(RememberChm == 'ReleaseNotes')) {
	   sPage = sFile.replace(/.chm/, "");
	   sPage = sPage + "\/whatsnew.htm";
	   ToBeOpened = sPrologue + sBaseDir + "\/" + DocLanguageDir + "\/" + sFile + "::" + "\/" + sLanguageDir + "\/" +  sPage;
	 }
//*************************************************************************************10-Dec-02
// Now check if the compiled file contianing the page to be displayed is installed.
// If it isn't, display a warning instead
//
		var A_ChmFileToBeChecked = 'SOFTWARE/think3/Documentation/' + SWCurrentVersion + "/List/" + sFile
		ChmFileToBeChecked = TheForm.Redirector.ReadRegistry(A_ChmFileToBeChecked);
		if(ChmFileToBeChecked == "true") {


//*************************************************************************************

//-------------------------------------------------------------------------------

			if(WindowType == 'CommandWindow') {
					open(ToBeOpened,"_self")
			}
		      	if(WindowType == 'MiniJobWindow') {
					open(ToBeOpened,"_self")
			}
			if(WindowType == 'OverWindow') {
					open(ToBeOpened,"_self")
			}
			if(WindowType == '') {
					open(ToBeOpened,"_top")
			}
			
//+++++++++++++++++++++++++++++++++++++++Option Window+++++++++++++++++++++++++++++++++++++++++06-October-2003
			if(WindowType == 'OptionWindow') {
				var strWindowName = 'OptionWindow';
				var MyX = event.screenX-100; 
				var MyY = event.screenY;
				var w=open(ToBeOpened, strWindowName,
					"toolbar=" + bitToolBar +
					",location=" + bitLocation +
					",directories=" + bitDirectories +
					",status=" + bitStatus +
					",menubar=" + bitMenubar +
					",scrollbars=" + bitScrollBars +
					",resizable=" + bitResizable +
					",width=" + 250 +
					",height=" + 150 +
					",left=" + MyX +
					",top=" + MyY);
					//w.moveTo(MyX,MyY); This line was removed as it caused an error ONLY on STFO's machine (????)
					w.close()
					var w=open(ToBeOpened, strWindowName,
					"toolbar=" + bitToolBar +
					",location=" + bitLocation +
					",directories=" + bitDirectories +
					",status=" + bitStatus +
					",menubar=" + bitMenubar +
					",scrollbars=" + bitScrollBars +
					",resizable=" + bitResizable +
					",width=" + 250 +
					",height=" + 150 +
					",left=" + MyX +
					",top=" + MyY);
					w.blur();
					w.focus();	
			}
//++++++++++++++++++++++++++++++++++++++End of Option Window+++++++++++++++++++++++++++++++++++			
			
			if(WindowType == '') {
				open(ToBeOpened,"_self")
			}
//**********************************************************10-Dec-02
	}
	else {
		if(this.location.href.match('english')) {
        	alert("To view this help page, install the software component/product associated with the requested documentation (" + sFile + ") and try again.");
		}
		if(this.location.href.match('italian')) {
	    	alert("Per visualizzare questa pagina di guida, installare il componente software/prodotto associato con la documentazione richiesta (" + sFile + ") e riprovare.");
		}
		if(this.location.href.match('french')) {
        	alert("Pour visualiser cette page d'Aide, installer le composant logiciel/produit associ・・la documentation demand馥 (" + sFile + ") et r馥ssayer.");
		}
		if(this.location.href.match('german')) {
        	alert("Installieren Sie zur Anzeige dieser Hilfe-Seite zun臘hst die/das zur aufgerufenen Dokumentation gehige Softwarekomponente/Softwareprodukt (" + sFile + ") und probieren es dann erneut.");
		}
		if(this.location.href.match('japanese')) {
        	alert("このヘルプページを表示するには、指定したドキュメント(" + sFile + ")に対応するソフトウェアコンポーネント/製品をインストールしてください。");
		}
		TheForm.Redirector.LastError = 0;
	}
//**********************************************************	
} else {
		// If the DLL or any of the Registry Keys are not available:
		if(this.location.href.match('english')) {
			alert('To view the desired page, install the think3 documentation and try again.')
		}
		if(this.location.href.match('italian')) {
			alert('Per visualizzare la pagina di guida desiderata, installare la documentazione di think3 e riprovare.')
		}
		if(this.location.href.match('french')) {
			alert("Pour visualiser la page d'Aide d駸ir馥, installer la documentation de think3 et r馥ssayer.")
		}
		if(this.location.href.match('german')) {
			alert('Installieren Sie zur Anzeige der gew�nschten Hilfe-Seite zun臘hst die think3-Dokumentation und probieren es dann erneut.')
		}
		if(this.location.href.match('japanese')) {
			alert('必要なページを表示するには、think3ドキュメントをインストールした後でもう一度実行してください。')
		}
		//TheForm.Redirector.LastError = 0;
	}
}
