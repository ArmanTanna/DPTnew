//
/*
Gestione dell'errore.
Nel caso di IE6sp1, quando la doc NON ・installata su un disco locale (cio・se ・in rete) si verificano errori JavaScript a causa delle nuove restrizioni di security inserite a tradimento da Microsoft.
Al posto di un errore JavaScript, visualizziamo un messaggio in cui si informa l'utente che questa situazione si verifica se l'installazione non ・locale.
*/
function errorHandler() {
if(this.location.href.match('english')) {
        alert("To view the desired page, install the documentation to your machine.\nIf the problem persists, contact think3 Customer Care (e-mail: thinkcare@think3.com).");
}
if(this.location.href.match('italian')) {
	    alert("Per visualizzare la pagina di guida desiderata, installare la documentazione sulla propria macchina.\nSe il problema persiste, contattare l'Assistenza clienti di think3 (e-mail: thinkcare@think3.com).");
}
if(this.location.href.match('french')) {
        alert("Pour visualiser la page d'Aide d駸ir馥, installer localement la documentation sur la machine.\nSi le probl鑪e persiste, contacter l'Assistance Clients de think3 (e-mail: thinkcare@think3.com).");
}
if(this.location.href.match('german')) {
        alert("Installieren Sie zur Anzeige der gew・schten Hilfe-Seite die Dokumentation auf Ihrem Rechner.\nWenn das Problem bestehen bleibt, wenden Sie sich an den think3-Kundendienst (e-mail: thinkcare@think3.com).");
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
strBrowserName = navigator.appName;
fltBrowserVersion = parseFloat(navigator.appVersion); 
var allSupport = document.all!=null
var intPopupWidth = 300;
var intPopupHeight = 300;
var X = 0;
var Y = 0; 
var ImgWndFlg = 0;
//var ImgWndFlg = 0;
//
function ImageWindow(X, Y) {
	var bitToolBar = 0;
	var bitLocation = 0
    var bitDirectories = 0;
    var bitStatus = 8;
	var bitMenubar = 0;
    var bitScrollBars = 0;
    var bitResizable = 64;
	var strWindowName = 'ImageWindow';
    var w=open('', strWindowName,
        "toolbar=" + bitToolBar +
        ",location=" + bitLocation +
        ",directories=" + bitDirectories +
        ",status=" + bitStatus +
	    ",menubar=" + bitMenubar +
        ",scrollbars=" + bitScrollBars +
        ",resizable=" + bitResizable +
        ",width=" + 100 +
        ",height=" + 100 +
        ",left=" + X +
        ",top=" + Y)
		ImgWndFlg = 1;
		w.focus();
}
//
//
//
    if (fltBrowserVersion >= 4.0) {
        intHeight = screen.availHeight;
        intWidth = screen.availWidth;
        } else {
        intHeight = 640;
        intWidth = 480;
    } // end if
	// Calculate popup location
	if (Y - intPopupHeight >= 20) {
		intTopPos = Y - intPopupHeight - 30;
		} else {
		intTopPos = 0;
	} // end if
	if (X + (intPopupWidth / 2) <= intWidth && X - (intPopupWidth / 2) > 0) {
		intLeftPos = X - (intPopupWidth / 2);
		} else if (X + (intPopupWidth / 2) > intWidth) {
			intLeftPos = intWidth - intPopupWidth - 7;
		} else {
			intLeftPos = 0;
	} // end if
//
//
//
function locateWindow(strDirection,intSize) {
    if (fltBrowserVersion >= 4.0) {
        intHeight = screen.availHeight;
        intWidth = screen.availWidth;
        } else {
        intHeight = 800;
        intWidth = 600;
    } // end if
    
    if (strDirection == 'height') {
    	intTop = Math.round((intHeight - intSize) / 2);
    	return intTop;
    
    } else {
    	intLeft = Math.round((((intWidth / 2) - intSize) / 2) + (intWidth / 2));
    	return intLeft;
    }
 }
