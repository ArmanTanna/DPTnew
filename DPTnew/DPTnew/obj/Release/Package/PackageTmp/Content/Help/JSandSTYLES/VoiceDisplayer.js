function HideVoice(){
//
// NIBI - April 2007 AMDG + November 2008 AMDGetS
//
	var English = true;
	var PathString = this.location.toString();

	//alert(PathString);
		
	if(PathString.match(/DOCF2D/)){
		English = false;
	}
	else {
		if(PathString.match(/THINKDESIGN\\DOC\\/) || PathString.match(/english/) || PathString.match(/italian/) || PathString.match(/DOCENG/)|| PathString.match(/thinkdesign\/language/) || PathString.match(/thinkdesign/) || PathString.match(/THINKDESIGN/)){
			English = true;
		}
		else {
			English = false;
		}
	}
	var iTd = 0; 
	var iTdSource;
	var oTd;
	var nTd;
	var oTd;
	if (document.all) {	
		oTd = document.all.tags('TD');
	}
	if(oTd) {
		for (iTd=0; iTd < oTd.length; iTd++)		
			{
				nTd=document.all.tags("TD").item(iTd);			
				if (nTd.innerHTML.match(/Voice:/)){				
					if(English == false) {
						nTd.style.display = "none";
					}
				}
			}
	}
}
