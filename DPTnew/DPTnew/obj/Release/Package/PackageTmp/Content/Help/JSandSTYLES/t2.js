function CheckProd(){
	var TdId = false;
	var iSpan = 0; 
	var iSpanSource
	var oSpan
	var AdvCont = TheForm.Redirector.ReadRegistry('SOFTWARE/think3/Documentation/TID');
	if(AdvCont == "YES") {
		TdId = true;
	}
	if(TheForm.Redirector.LastError != 0) {
		TdId = true;
	}
	for (iSpan=0; iSpan < document.all.tags("SPAN").length; iSpan++)
		{
			oSpan=document.all.tags("SPAN").item(iSpan);
			iSpanSource=oSpan.sourceIndex;
			if (oSpan.id=="TID"){
				if(TdId == false) {
					oSpan.style.display = "none";
				}
			}
		}
}
		