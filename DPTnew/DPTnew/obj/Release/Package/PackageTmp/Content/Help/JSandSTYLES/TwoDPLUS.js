function HideThreeD(){
//
// NIBI - July-September 2006
//
	var TwoD = true;
	var iSpan = 0; 
	var iSpanSource;
	var oSpan;
	var nSpan;
//	var AdvCont = TheForm.Redirector.ReadRegistry('SOFTWARE/think3/Documentation/CurrentVersion');
	var AdvCont1 = this.document.location;
	var AdvCont = AdvCont1.toString()
	//alert(AdvCont);
//	if(AdvCont.match(/thinkdesign\\/)||AdvCont.match(/thinkdesign\\/)) {
	if(AdvCont.match(/thinkdesign\\/)||AdvCont.match(/THINKDESIGN\\/)||AdvCont.match(/THINKD~1\\/)||AdvCont.match(/thinkd~1\\/)||AdvCont.match(/thinkdesign\//)) {
		//alert('beccato');
		TwoD = false;
	}
	else {
		if(AdvCont.match(/thinkdesigndrafting\\/)||AdvCont.match(/THINKDESIGNDRAFTING\\/)) {
			//alert('ciccia');
			TwoD = true;
		}
	}
	/*if(TheForm.Redirector.LastError != 0) {
		TwoD = false;
	}*/
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
				if (nSpan.id=="E2"){
					if(TwoD == true) {
						nSpan.innerHTML = ".e2";
					}	
				}
//
// Show only in f2d, hide in td
//
				if (nSpan.id=="HIDETD"){
					if(TwoD == false) {
						nSpan.style.display = "none";
						nSpan.style.topmargin = "0";
						nSpan.style.bottommargin = "0";						
					}
				}
//================================================================================
			}
	}
}
//================================================= 19 October 2006 ==============
function HideMore() {
	var F2D = true;
	var oTable;
	var iTable=0;
	var iTableSource;
	var nTable;
	oTable= document.getElementsByTagName('TABLE');
	var HideInF2D1 = this.document.location
	var HideInF2D = HideInF2D1.toString()
	if(HideInF2D.match(/thinkdesign\\/)||HideInF2D.match(/THINKDESIGN\\/)||HideInF2D.match(/thinkdesign\//)) {
		F2D = false;
	}
	else {
		F2D = true;
	}	
	if(oTable) {
		for (iTable=0; iTable < oTable.length; iTable++)		
			{
				nTable=document.all.tags("TABLE").item(iTable);
				iTableSource=oTable.sourceIndex;
				if (nTable.id=="F2DNOTVISIBLE"){
					if(HideInF2D.match(/thinkdesign\\/)||HideInF2D.match(/THINKDESIGN\\/)||HideInF2D.match(/thinkdesign\//)) {
						F2D = false;
					}
					else {
						F2D = true;
						nTable.style.display = "none";
						nTable.style.topmargin = "0";
						nTable.style.bottommargin = "0";							
					}	
				}					
		}	
	}
}		
function HideLists() {
	var FRD = true;
	var oList;
	var iList=0;
	var iListSource;
	var nList;
	oList= document.getElementsByTagName('LI');
	var HideInFRD1 = this.document.location;
	var HideInFRD = HideInFRD1.toString();
	if(HideInFRD.match(/thinkdesign\\/)||HideInFRD.match(/THINKDESIGN\\/)||HideInFRD.match(/thinkdesign\//)) {
		FRD = false;
	}
	else {
		FRD = true;
	}	
	if(oList) {
		for (iList=0; iList < oList.length; iList++)		
			{
				nList=document.all.tags("LI").item(iList);
				iListSource=oList.sourceIndex;
				if (nList.id=="HIDELI"){
					if(HideInFRD.match(/thinkdesign\\/)||HideInFRD.match(/THINKDESIGN\\/)||HideInFRD.match(/thinkdesign\//)) {
						FRD = false;
					}
					else {
						FRD = true;
						nList.style.display = "none";
						nList.style.topmargin = "0";
						nList.style.bottommargin = "0";
					}	
				}					
		}	
	}
}
//=======================================================================================
function HideH4() {
	var FRD_H4 = true;
	var oH4;
	var iH4=0;
	var iH4Source;
	var nH4;
	oH4= document.getElementsByTagName('H4');
	var HideInFRD_H41 = this.document.location
	var HideInFRD_H4 = HideInFRD_H41.toString();
	if(HideInFRD_H4.match(/thinkdesign\\/)||HideInFRD_H4.match(/THINKDESIGN\\/)||HideInFRD_H4.match(/thinkdesign\//)) {
		FRD_H4 = false;
	}
	else {
		FRD_H4 = true;
	}	
	if(oH4) {
		for (iH4=0; iH4 < oH4.length; iH4++)		
			{
				nH4=document.all.tags("H4").item(iH4);
				iH4Source=oH4.sourceIndex;
				if (nH4.id=="HIDEH4"){
					if(HideInFRD_H4.match(/thinkdesign\\/)||HideInFRD_H4.match(/THINKDESIGN\\/)||HideInFRD_H4.match(/thinkdesign\//)) {
						FRD_H4 = false;
					}
					else {
						FRD_H4 = true;
						nH4.style.display = "none";
						nH4.style.topmargin = "0";
						nH4.style.bottommargin = "0";							
					}	
				}					
		}	
	}
}
//===================================================================================================
function HideH1() {
	var FRD_H1 = true;
	var oH1;
	var iH1=0;
	var iH1Source;
	var nH1;
	oH1= document.getElementsByTagName('H1');
	var HideInFRD_H11 = this.document.location
	var HideInFRD_H1 = HideInFRD_H11.toString();
	if(HideInFRD_H1.match(/thinkdesign\\/)||HideInFRD_H1.match(/THINKDESIGN\\/)||HideInFRD_H1.match(/thinkdesign\//)) {
		FRD_H1 = false;
	}
	else {
		FRD_H1 = true;
	}	
	if(oH1) {
		for (iH1=0; iH1 < oH1.length; iH1++)		
			{
				nH1=document.all.tags("H1").item(iH1);
				iH1Source=oH1.sourceIndex;
				if (nH1.id=="HIDEH1"){
					if(HideInFRD_H1.match(/thinkdesign\\/)||HideInFRD_H1.match(/THINKDESIGN\\/)||HideInFRD_H1.match(/thinkdesign\//)) {
						FRD_H1 = false;
					}
					else {
						FRD_H1 = true;
						nH1.style.display = "none";
						nH1.style.topmargin = "0";
						nH1.style.bottommargin = "0";							
					}	
				}					
		}	
	}
}
//===========================================================================================================
function HideH2() {
	var FRD_H2 = true;
	var oH2;
	var iH2=0;
	var iH2Source;
	var nH2;
	oH2= document.getElementsByTagName('H2');
	var HideInFRD_H21 = this.document.location;
	var HideInFRD_H2 = HideInFRD_H21.toString();
	if(HideInFRD_H2.match(/thinkdesign\\/)||HideInFRD_H2.match(/THINKDESIGN\\/)||HideInFRD_H2.match(/thinkdesign\//)) {
		FRD_H2 = false;
	}
	else {
		FRD_H2 = true;
	}	
	if(oH2) {
		for (iH2=0; iH2 < oH2.length; iH2++)		
			{
				nH2=document.all.tags("H2").item(iH2);
				iH2Source=oH2.sourceIndex;
				if (nH2.id=="HIDEH2"){
					if(HideInFRD_H2.match(/thinkdesign\\/)||HideInFRD_H2.match(/THINKDESIGN\\/)||HideInFRD_H2.match(/thinkdesign\//)) {
						FRD_H2 = false;
					}
					else {
						FRD_H2 = true;
						nH2.style.display = "none";
						nH2.style.topmargin = "0";
						nH2.style.bottommargin = "0";							
					}	
				}					
		}	
	}
}
//==============================================================================================================
function HideH3() {
	var FRD_H3 = true;
	var oH3;
	var iH3=0;
	var iH3Source;
	var nH3;
	oH3= document.getElementsByTagName('H3');
	var HideInFRD_H31 = this.document.location;
	var HideInFRD_H3 = HideInFRD_H31.toString();
	if(HideInFRD_H3.match(/thinkdesign\\/)||HideInFRD_H3.match(/THINKDESIGN\\/)||HideInFRD_H3.match(/thinkdesign\//)) {
		FRD_H3 = false;
	}
	else {
		FRD_H3 = true;
	}	
	if(oH3) {
		for (iH3=0; iH3 < oH3.length; iH3++)		
			{
				nH3=document.all.tags("H3").item(iH3);
				iH3Source=oH3.sourceIndex;
				if (nH3.id=="HIDEH3"){
					if(HideInFRD_H3.match(/thinkdesign\\/)||HideInFRD_H3.match(/THINKDESIGN\\/)||HideInFRD_H3.match(/thinkdesign\//)) {
						FRD_H3 = false;
					}
					else {
						FRD_H3 = true;
						nH3.style.display = "none";
						nH3.style.topmargin = "0";
						nH3.style.bottommargin = "0";							
					}	
				}					
		}	
	}
}
//==============================================================================================================
function HideH5() {
	var FRD_H5 = true;
	var oH5;
	var iH5=0;
	var iH5Source;
	var nH5;
	oH5= document.getElementsByTagName('H5');
	var HideInFRD_H51 = this.document.location;
	var HideInFRD_H5 = HideInFRD_H51.toString();
	if(HideInFRD_H5.match(/thinkdesign\\/)||HideInFRD_H5.match(/THINKDESIGN\\/)||HideInFRD_H5.match(/thinkdesign\//)) {
		FRD_H5 = false;
	}
	else {
		FRD_H5 = true;
	}	
	if(oH5) {
		for (iH5=0; iH5 < oH5.length; iH5++)		
			{
				nH5=document.all.tags("H5").item(iH5);
				iH5Source=oH5.sourceIndex;
				if (nH5.id=="HIDEH5"){
					if(HideInFRD_H5.match(/thinkdesign\\/)||HideInFRD_H5.match(/THINKDESIGN\\/)||HideInFRD_H5.match(/thinkdesign\//)) {
						FRD_H5 = false;
					}
					else {
						FRD_H5 = true;
						nH5.style.display = "none";
						nH5.style.topmargin = "0";
						nH5.style.bottommargin = "0";							
					}	
				}					
		}	
	}
}
//============================================================================================================
//=======================================================================================
function HideP() {
	var FRD_P = true;
	var oP;
	var iP=0;
	var iPSource;
	var nP;
	oP= document.getElementsByTagName('P');
	var HideInFRD_P1 = this.document.location;
	var HideInFRD_P = HideInFRD_P1.toString();
	if(HideInFRD_P.match(/thinkdesign\\/)||HideInFRD_P.match(/THINKDESIGN\\/)||HideInFRD_P.match(/thinkdesign\//)) {
		FRD_P = false;
	}
	else {
		FRD_P = true;
	}	
	if(oP) {
		for (iP=0; iP < oP.length; iP++)		
			{
				nP=document.all.tags("P").item(iP);
				iPSource=oP.sourceIndex;
				if (nP.id=="HIDEP"){
					if(HideInFRD_P.match(/thinkdesign\\/)||HideInFRD_P.match(/THINKDESIGN\\/)||HideInFRD_P.match(/thinkdesign\//)) {
						FRD_P = false;
					}
					else {
						FRD_P = true;
						nP.style.display = "none";
						nP.style.topmargin = "0";
						nP.style.bottommargin = "0";							
					}	
				}					
		}	
	}
}
//==============================================================================================================
function HideHR() {
	var FRD_HR = true;
	var oHR;
	var iHR=0;
	var iHRSource;
	var nHR;
	oHR= document.getElementsByTagName('HR');
	var HideInFRD_HR1 = this.document.location;
	var HideInFRD_HR = HideInFRD_HR1.toString();	
	if(HideInFRD_HR.match(/thinkdesign\\/)||HideInFRD_HR.match(/THINKDESIGN\\/)||HideInFRD_HR.match(/thinkdesign\//)) {
		FRD_HR = false;
	}
	else {
		FRD_HR = true;
	}	
	if(oHR) {
		for (iHR=0; iHR < oHR.length; iHR++)		
			{
				nHR=document.all.tags("HR").item(iHR);
				iHRSource=oHR.sourceIndex;
				if (nHR.id=="HIDEHR"){
					if(HideInFRD_HR.match(/thinkdesign\\/)||HideInFRD_HR.match(/THINKDESIGN\\/)||HideInFRD_HR.match(/thinkdesign\//)) {
						FRD_HR = false;
					}
					else {
						FRD_HR = true;
						nHR.style.display = "none";
						nHR.style.topmargin = "0";
						nHR.style.bottommargin = "0";							
					}	
				}					
		}	
	}
}
//==============================================================================================================
function HideTR() {
	var FRD_TR = true;
	var oTR;
	var iTR=0;
	var iTRSource;
	var nTR;
	oTR= document.getElementsByTagName('TR');
	var HideInFRD_TR1 = this.document.location;
	var HideInFRD_TR = HideInFRD_TR1.toString();
	if(HideInFRD_TR.match(/thinkdesign\\/)||HideInFRD_TR.match(/THINKDESIGN\\/)||HideInFRD_TR.match(/thinkdesign\//)) {
		FRD_TR = false;
	}
	else {
		FRD_TR = true;
	}	
	if(oTR) {
		for (iTR=0; iTR < oTR.length; iTR++)		
			{
				nTR=document.all.tags("TR").item(iTR);
				iTRSource=oTR.sourceIndex;
				if (nTR.id=="HIDETR"){
					if(HideInFRD_TR.match(/thinkdesign\\/)||HideInFRD_TR.match(/THINKDESIGN\\/)||HideInFRD_TR.match(/thinkdesign\//)) {
						FRD_TR = false;
					}
					else {
						FRD_TR = true;
						nTR.style.display = "none";
						nTR.style.topmargin = "0";
						nTR.style.bottommargin = "0";							
					}	
				}					
		}	
	}
}
//==============================================================================================================
function HideTDC() {
	var FRD_TDC = true;
	var oTDC;
	var iTDC=0;
	var iTDCSource;
	var nTDC;
	oTDC= document.getElementsByTagName('TD');
	var HideInFRD_TDC1 = this.document.location;
	var HideInFRD_TDC = HideInFRD_TDC1.toString();
	if(HideInFRD_TDC.match(/thinkdesign\\/)||HideInFRD_TDC.match(/THINKDESIGN\\/)||HideInFRD_TDC.match(/thinkdesign\//)) {
		FRD_TDC = false;
	}
	else {
		FRD_TDC = true;
	}	
	if(oTDC) {
		for (iTDC=0; iTDC < oTDC.length; iTDC++)		
			{
				nTDC=document.all.tags("TD").item(iTDC);
				iTDCSource=oTDC.sourceIndex;
				if (nTDC.id=="HIDETCELL"){
					if(HideInFRD_TDC.match(/thinkdesign\\/)||HideInFRD_TDC.match(/THINKDESIGN\\/)||HideInFRD_TDC.match(/thinkdesign\//)) {
						FRD_TDC = false;
					}
					else {
						FRD_TDC = true;
						nTDC.style.display = "none";
						nTDC.style.topmargin = "0";
						nTDC.style.bottommargin = "0";							
					}	
				}					
		}	
	}
}
