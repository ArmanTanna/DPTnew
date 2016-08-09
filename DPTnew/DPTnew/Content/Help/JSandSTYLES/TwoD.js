function HideThreeD(){
	var TwoD = true;
	var iSpan = 0; 
	var iSpanSource;
	var oSpan;
	var nSpan;
	var AdvCont = TheForm.Redirector.ReadRegistry('SOFTWARE/think3/Documentation/CurrentVersion');
	if(AdvCont.match(/^200/)) {
		TwoD = false;
	}
	else {
		TwoD = true;
	}
	if(TheForm.Redirector.LastError != 0) {
		TwoD = false;
	}
	var oSpan;
	if (document.all) {
		oSpan = document.all.tags('SPAN');
	}
	else if (document.getElementsByTagName) {
		oSpan= document.getElementsByTagName('SPAN');
	}
	if(oSpan) {
		for (iSpan=0; iSpan < oSpan.length; iSpan++)		
			{
				nSpan=document.all.tags("SPAN").item(iSpan);
				iSpanSource=oSpan.sourceIndex;
				if (nSpan.id=="TWOD"){
					if(TwoD == true) {
						nSpan.style.display = "none";
					}
				}
			}
	}
}
		