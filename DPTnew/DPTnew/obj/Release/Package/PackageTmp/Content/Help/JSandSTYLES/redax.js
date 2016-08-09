//
/*
Gestione dell'errore.
Nel caso di IE6sp1, quando la doc NON Einstallata su un disco locale (cioEse Ein rete) si verificano errori JavaScript a causa delle nuove restrizioni di security inserite a tradimento da Microsoft.
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
        alert("Pour visualiser la page d'Aide désirée, installer localement la documentation sur la machine.\nSi le problème persiste, contacter le Customer Care (Assistance Clients) de think3 (e-mail: thinkcare@think3.com).");
}
if(this.location.href.match('german')) {
        alert("Installieren Sie zur Anzeige der gewünschten Hilfe-Seite die Dokumentation auf Ihrem Rechner.\nWenn das Problem bestehen bleibt, wenden Sie sich an den think3 Customer Care (Kundendienst) (e-mail: thinkcare@think3.com).");
}
if(this.location.href.match('japanese')) {
        alert("–Ú“I‚Ìƒy[ƒW‚ð•\Ž¦‚·‚é‚É‚ÍA‚¨Žg‚¢‚ÌƒRƒ“ƒsƒ…[ƒ^‚ÉƒhƒLƒ…ƒƒ“ƒg‚ðƒCƒ“ƒXƒg[ƒ‹‚µ‚Ä‚­‚¾‚³‚¢B\n–â‘è‚ªÄ”­‚·‚éê‡‚ÍAthink3 Customer Careie-mail: thinkcare@think3.comj‚É‚¨–â‚¢‡‚í‚¹‚­‚¾‚³‚¢B");
}
if(this.location.href.match('chinese')) {
        alert("Òª²é¿´ÇëÇóµÄÒ³Ãæ£¬ÇëÔÚÄúµÄ»úÆ÷ÉÏ°²×°Ïà¹ØÎÄµµ¡£\nÈç¹û°²×°ºó£¬ÈÔÈ»´æÔÚÎÊÌâ£¬ÇëÁªÏµthink3¿Í»§¹Ø»³(e-mail: thinkcare@think3.com)¡£");
}
	 return true;
}
window.onerror = errorHandler;
//
function OpenHelp(sPage, sCHMorDir, WindowType, Product, CHMfile)

		
{
	var sFile = CHMfile;
	var sPrologue = "mk:@MSITStore:"; // Compliant with MS IE 3.0
	var ToBeOpened = '';	
    var RememberChm = '';
// adesso 27Mar	
	var ThisFile = this.location.href;
//	alert(ThisFile);
	
//========================================================================================
		if(Product == "thinkdesign") {
			if(sFile =="") {
				sFile = "Help.chm";
			}
		}
		else {
			if(Product == "thinkteam") {
				if(sFile =="") {
					sFile = "tt.chm";
				}
			}
			else {
				if(Product == "cladmin") {
					if(sFile =="") {
						sFile = "la.chm";
					}
				}
				if(Product == "thinkprint") {
					if(sFile =="") {
						sFile = "tp.chm";
					}
				}
				if(Product == "ttwfedit") {
					if(sFile =="") {
						sFile = "ttwfedit.chm";
					}
				}
				if(Product == "switch") {
					if(sFile =="") {
						sFile = "sw.chm";
					}
				}
				if(Product == "instdb") {
					if(sFile =="") {
						sFile = "instdb.chm";
					}
				}
				if(Product == "insthw") {
					if(sFile =="") {
						sFile = "insthw.chm";
					}
				}
				if(Product == "thinkparts") {
					if(sFile =="") {
						sFile = "thp.chm";
					}
				}	
				if(Product == "dimensio") {
					if(sFile =="") {
						sFile = "dimensio.chm";
					}
				}
				if(Product == "draft") {
					if(sFile =="") {
						sFile = "draft.chm";
					}
				}
				if(Product == "claspro") {
					if(sFile =="") {
						sFile = "claspro.chm";
					}
				}
				if(Product == "selsna") {
					if(sFile =="") {
						sFile = "selsna.chm";
					}
				}
				if(Product == "forcus") {
					if(sFile =="") {
						sFile = "forcus.chm";
					}
				}
				if(Product == "filew") {
					if(sFile =="") {
						sFile = "filew.chm";
					}
				}
				if(Product == "solids") {
					if(sFile =="") {
						sFile = "solids.chm";
					}
				}
				if(Product == "image") {
					if(sFile =="") {
						sFile = "image.chm";
					}
				}
				if(Product == "view") {
					if(sFile =="") {
						sFile = "view.chm";
					}
				}
				if(Product == "sobj") {
					if(sFile =="") {
						sFile = "sobj.chm";
					}
				}
				if(Product == "curves") {
					if(sFile =="") {
						sFile = "curves.chm";
					}
				}
				if(Product == "surfaces") {
					if(sFile =="") {
						sFile = "surfaces.chm";
					}
				}
/*				if(Product == "pb") {
					if(sFile =="") {
						sFile = "pb.chm";
					}
				}*/
				if(Product == "thinkcatia") {
					if(sFile =="") {
						sFile = "ce.chm";
					}
				}
//				
				if((Product == "pb") && (sPage.match('thinkteamoptions'))) {
					if(sFile =="") {
						sFile = "common.chm";
					}
				}
				if((Product == "pb") && (sPage.match('projbrow'))) {
					if(sFile =="") {
						sFile = "pb.chm";
					}
				}				
//							
				if(Product == "converters") {
					if(sFile =="") {
						sFile = "cnv.chm";
					}
				}
				if(Product == "thinklight") {
					if(sFile =="") {
						sFile = "thinklight.chm";
					}
				}		
				if(Product == "PARTsolutions") {
					if(sFile =="") {
						sFile = "cadenas.chm";
					}
				}	
				if(Product == "englishonly") {
						sFile = CHMfile;
				}														
			}		
	
		}
//========================================================================================	
//alert(sPage);
//alert("hello" + ThisFile + "==" + sFile);
var ChmPos = ThisFile.indexOf(".chm");
//alert("hello 2");
var ThisFolder = ThisFile.substr(0,ChmPos);
//alert("hello 3");
ChmPos = ThisFolder.lastIndexOf("\\");
//alert("hello 4");
ThisFolder=ThisFolder.substr(0,ChmPos+1);
//alert(ThisFolder);
ToBeOpened = ThisFolder + sFile + "::" + "\/language" + sPage;
//alert(ToBeOpened);
//-------------------------------------------------------------------------------
     if((sPage == '')&&(RememberChm == 'ReleaseNotes')) {
	   sPage = sFile.replace(/.chm/, "");
	   sPage = sPage + "\/whatsnew.htm";	   
	   ToBeOpened = sPrologue + sFile + "::" + "\/language" + sPage;
	 }

			if(WindowType == 'CommandWindow') {
					open(ToBeOpened,"_top")
			}
		      	if(WindowType == 'MiniJobWindow') {
					open(ToBeOpened,"_top")
			}
			if(WindowType == 'OverWindow') {
					open(ToBeOpened,"_top")
			}
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
				open(ToBeOpened,"_top")
			}

}
