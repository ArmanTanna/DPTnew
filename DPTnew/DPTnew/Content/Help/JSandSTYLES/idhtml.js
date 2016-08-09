  <!--
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
  /*  Your are permitted to reuse this code as long as the following copyright
      notice is not removed:
      This HTML tip handling is copyright 1998 by insideDHTML.com, LLC. More information about this
      code can be found at Inside Dynamic HTML: HTTP://www.insideDHTML.com
  */
  // Support for all collection
  var allSupport = document.all!=null
  function setupEventObject(e) {
    // Map NS event object to IEs
    if (e==null) return // IE returns
    window.event = e
    window.event.fromElement = e.target
    window.event.toElement = e.target
    window.event.srcElement = e.target
    window.event.x = e.x
    window.event.y = e.y
    // Route the event to the original element
    // Necessary to make sure _tip is set.
    window.event.srcElement.handleEvent(e);
  }
  function checkName(src) {
    // Look for tooltip in IE
    while ((src!=null) && (src._tip==null))
      src = src.parentElement
    return src
  }
  function getElement(elName) {
    // Get an element from its ID
    if (allSupport)
      return document.all[elName]
    else
      return document.layers[elName]
  }
  function writeContents(el, tip) {
    // Replace the contents of the tooltip
    if (allSupport)
      el.innerHTML = tip
    else {
      // In NS, insert a table to work around
      // stylesheet rendering bug.
      // NS fails to apply style sheets when writing
      // contents into a positioned element.
      el.document.open()
      el.document.write("<TABLE WIDTH=200 BORDER=1 bordercolor=black><TR><TD WIDTH=100% BGCOLOR=#ffffe1>")
      el.document.write(tip)
      el.document.write("</TD></TR></TABLE>")
      el.document.close()
    }
  }
  function getOffset(el, which) {
    // Function for IE to calculate position 
    // of an element.
    var amount = el["offset"+which] 
    if (which=="Top")
      amount+=el.offsetHeight
    el = el.offsetParent
    while (el!=null) {
      amount+=el["offset"+which]
      el = el.offsetParent
    }
    return amount
  }
  
  function setPosition(el) {
    // Set the position of an element
    src = window.event.srcElement
    if (allSupport) {
      el.style.pixelTop = getOffset(src, "Top")
      el.style.pixelLeft = getOffset(src, "Left")
    } else
    {
      el.top = src.y + 20 //window.event.y + 15
      el.left = src.x //window.event.x
    }
  }
      
  function setVisibility(el, bDisplay) {
    // Hide or show to tip
    if (bDisplay)
      if (allSupport)
        el.style.visibility = "visible" 
      else
        el.visibility = "show";
    else
      if (allSupport)
        el.style.visibility = "hidden"
      else
        el.visibility = "hidden"
  }
  function displayContents(tip) {
    // Display the tooltip. 
    var el = getElement("tipBox")
    writeContents(el, tip)
    setPosition(el)
    setVisibility(el, true)
  }
  function doMouseOver(e) {
    // Mouse moves over an element
    setupEventObject(e)
    var el, tip
    if ((el = checkName(window.event.srcElement))!=null)
      if  (!el._display) {
        displayContents(el._tip)
        el._display = true
      }
  }
  function doMouseOut(e) {
    // Mouse leaves an element
    setupEventObject(e)
    el = checkName(window.event.srcElement)
    var el, tip
    if ((el = checkName(window.event.srcElement))!=null)
      if (el._display)
        if ((el.contains==null) || (!el.contains(window.event.toElement))) {
          setVisibility(getElement("tipBox"), false)
          el._display = false
        }
  }
  function doLoad() {
    // Do Loading
    if ((window.document.captureEvents==null) && (!allSupport))
      return // Not IE4 or NS4
    if (window.document.captureEvents!=null)  // NS - capture events
      window.document.captureEvents(Event.MOUSEOVER | Event.MOUSEOUT)
    window.document.onmouseover = doMouseOver;
    window.document.onmouseout = doMouseOut;
  }
  window.onload = doLoad
  // -->
