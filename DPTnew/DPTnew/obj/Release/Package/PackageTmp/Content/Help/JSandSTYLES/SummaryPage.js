function WriteMe(ToBeOpened, CHMorDir, Product, CHMfile, ProgressiveNumber)
{
//%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
						var SumPag = this.location.toString();
//						alert("Proprio qui" + SumPag);
//
// Summary in properties
//						
						if(SumPag.match('language/format/properties')) {
							if(ToBeOpened.match('/format/properties/')) {					// il mio file è sotto properties
								ToBeOpened= ToBeOpened.replace('/format/properties/','../properties/');
							}
							if(ToBeOpened.match('/format/styles')){							// il mio file è sotto styles
								ToBeOpened= ToBeOpened.replace('/format','..');
							}
							if(ToBeOpened.match(/^\/options/)){								// il mio file è sotto options
								ToBeOpened= ToBeOpened.replace('/options', '../../../language/options');
							}
						}
//
// Summary in options
//						
						else if(SumPag.match('language/options')) {
							if(ToBeOpened.match('/options')) {						    	// il mio file è sotto options
								ToBeOpened= ToBeOpened.replace('/options/','');
							}		
							if(ToBeOpened.match('/format/properties')) {					// il mio file è sotto properties
								ToBeOpened= ToBeOpened.replace(/^/,'../../language/format/properties');
							}	
							if(ToBeOpened.match('format/styles')) {							// il mio file è sotto styles
								ToBeOpened= ToBeOpened.replace(/^/,'../../language/format/styles');
							}																			
						}
//
// Summary in thinkteamoptions
//						
						else if(SumPag.match('language/thinkteamoptions')) {	
							if(ToBeOpened.match('/thinkteamoptions')) {						// il mio file è sotto thinkteamoptions
								ToBeOpened= ToBeOpened.replace('/thinkteamoptions/','');
							}						
						}
//
// Summary in file
//					
						else if (SumPag.match('language/file')) {						
							if(ToBeOpened.match('/file/options/htmoptions/')) {						// il mio file è sotto file
								ToBeOpened= ToBeOpened.replace('/file/','');
							}	
							else if(ToBeOpened.match('/file/htmoptions/')) {						// il mio file è sotto file
								ToBeOpened= ToBeOpened.replace('/file/','');
							}													
						}							
//
// Summary in ttwizardoptions
//					
						/*else if (SumPag.match('language/ttwizardoptions')) {						
							if(ToBeOpened.match('/ttwizardoptions/')) {						// il mio file è sotto ttwizardoptions
								ToBeOpened= ToBeOpened.replace('/ttwizardoptions/','');
							}						
						}	*/	
//
// Summary in print
//					
						else if (SumPag.match('language/print')) {						
							if(ToBeOpened.match('/print/Options/')) {						// il mio file è sotto print
								ToBeOpened= ToBeOpened.replace('/print/','');
							}						
						}	
//
// Summary in Hole Table
//					
						else if (SumPag.match('language/format/OrganizeHoleTable')) {						
							if(ToBeOpened.match('/format/OrganizeHoleTable/')) {						// il mio file è sotto print
								ToBeOpened= ToBeOpened.replace('/format/OrganizeHoleTable/','../OrganizeHoleTable/');
							}						
						}							
																														
//
// Else
//
						else{
							ToBeOpened.replace(/^\//,'..');
						}
//%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

//
//*************************************************************************************
//
var ThisScriptLine00 = "<SCRIPT>";
var ThisScriptLine01 = "function ResizeIFrame" + ProgressiveNumber + "() {";
var ThisScriptLine02 = " var oIFrame" + ProgressiveNumber +" = document.getElementById('MyIframe" + ProgressiveNumber + "');";
var ThisScriptLine03 = " var altezza" + ProgressiveNumber + " = parseInt(oIFrame" + ProgressiveNumber + ".contentWindow.document.body.scrollHeight) - parseInt('30');";
//
//++++++++++++++++++++++++++++++++++++++++++++ caso file options: non toglie 30 in fondo
//
if(ToBeOpened.match('cnv.chm')) {
	var ThisScriptLine03 = "var altezza" + ProgressiveNumber + " = parseInt(oIFrame" + ProgressiveNumber + ".contentWindow.document.body.scrollHeight);";
}
var H1 = "oIFrame" + ProgressiveNumber + ".style.height = altezza" + ProgressiveNumber + ";";
var ThisScriptLine04 = "}";
var ThisScriptLine05 = "</SCRIPT>";
//
document.write(ThisScriptLine00);
document.write(ThisScriptLine01);
document.write(ThisScriptLine02);
document.write(ThisScriptLine03);
document.write(H1);
document.write(ThisScriptLine04);
document.write(ThisScriptLine05);
//
//+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++caso "current"
//
var ThisScriptLine00b = "<SCRIPT>";
var ThisScriptLine01b = "function ResizeIFrameUFFA" + ProgressiveNumber + "() {";
var ThisScriptLine02b = " var oIFrameUFFA = document.getElementById('MyIframeUFFA" + ProgressiveNumber + "');";
var ThisScriptLine03b = "oIFrameUFFA.style.height = oIFrameUFFA.contentWindow.document.body.scrollHeight +30 + 'px';";
var ThisScriptLine04b= "}";
var ThisScriptLine05b = "<\/SCRIPT>";
//
document.write(ThisScriptLine00b);
document.write(ThisScriptLine01b);
document.write(ThisScriptLine02b);
document.write(ThisScriptLine03b);
document.write(ThisScriptLine04b);
document.write(ThisScriptLine05b);
//
var ThisOutputLineUFFA = '<iframe id="MyIframeUFFA' + ProgressiveNumber + '" name="MyIframeUFFA' + ProgressiveNumber + '" frameborder="no" marginheight="0" marginwidth="0" style="height: 100%; width: 98%; border: none; margin: 0px;" SRC="' + ToBeOpened + '"' + ' onResize="ResizeIFrameUFFA' + ProgressiveNumber  + '()" scrolling="no"></iframe>';
var ThisOutputLine2 = '<iframe id="MyIframe' + ProgressiveNumber + '" name="MyIframe' + ProgressiveNumber + '" frameborder="no" marginheight="0" marginwidth="0" style="height: auto; width: 98%; border: none; margin: 0px;" SRC="' + ToBeOpened + '"' + ' onResize="ResizeIFrame' + ProgressiveNumber  + '()"' + ' scrolling="no" ></iframe>';
//
if((ToBeOpened.match('currentstyle.htm'))||(ToBeOpened.match('general/jsoptions/Description.htm'))||(ToBeOpened.match('Font.htm'))||(ToBeOpened.match('FrameGroup.htm'))||(ToBeOpened.match('chareffects.htm'))||(ToBeOpened.match('SM_Orientation.htm'))||(ToBeOpened.match('SM_FlipSightDirection.htm'))||(ToBeOpened.match('SM_RotationAngle.htm'))||(ToBeOpened.match('HOMEOPS.htm'))||(ToBeOpened.match('T_TOC_Customize.htm'))||(ToBeOpened.match('Op_MouseDragActions.htm'))||(ToBeOpened.match('htmoptions'))||(ToBeOpened.match('standard.htm'))||(ToBeOpened.match('current.htm'))||(ToBeOpened.match('large.htm'))||(ToBeOpened.match('file/htmoptions'))||(ToBeOpened.match('HT_Columns.htm'))||(ToBeOpened.match('HT_Columns.htm'))||(ToBeOpened.match('HT_Available.htm'))||(ToBeOpened.match('HT_Used.htm'))||(ToBeOpened.match('HT_RightLeft.htm'))||(ToBeOpened.match('HT_UpDown.htm'))||(ToBeOpened.match('HT_HeaderFooter.htm'))||(ToBeOpened.match('HT_Footer.htm'))||(ToBeOpened.match('HT_Header.htm'))||(ToBeOpened.match('chareffects.htm'))||(ToBeOpened.match('spacingscale.htm'))||(ToBeOpened.match('dm_19_OP_9.htm'))||(ToBeOpened.match('dm_19_OP_5.htm'))||(ToBeOpened.match('dm_stretch.htm'))||(ToBeOpened.match('dm_19_OP_4.htm'))||(ToBeOpened.match('dm_19_OP_3.htm'))||(ToBeOpened.match('HT_TextStyle.htm'))||(ToBeOpened.match('HT_Name.htm'))||(ToBeOpened.match('HT_TextFormat.htm'))||(ToBeOpened.match('HT_MeasureUnit.htm'))||(ToBeOpened.match('HT_ZeroDisplay.htm'))||(ToBeOpened.match('HT_Leading.htm'))||(ToBeOpened.match('HT_Trailing.htm'))||(ToBeOpened.match('AreaGeneral.htm'))||(ToBeOpened.match('TableOrigin.htm'))||(ToBeOpened.match('HierarchyLevel.htm'))||(ToBeOpened.match('Separator.htm'))||(ToBeOpened.match('SkipGroupsWithNoPN.htm'))||(ToBeOpened.match('IncludeGroupsWithNoBalloon.htm'))||(ToBeOpened.match('InsertHeaderInTable.htm'))||(ToBeOpened.match('AreaSorting.htm'))||(ToBeOpened.match('Order.htm'))||(ToBeOpened.match('EditColumn.htm'))||(ToBeOpened.match('OrderBy.htm'))){
//alert("A SSEN CHI");
	document.write(ThisOutputLineUFFA);
}
else if(ToBeOpened.match('ttwizardoptions')){
	if(ToBeOpened.match('OP_Icon.htm')||ToBeOpened.match('OP_IconBrowse.htm')||ToBeOpened.match('OP_AddRight.htm')||ToBeOpened.match('OP_RemoveRight.htm')||ToBeOpened.match('OP_NamingRule.htm')||ToBeOpened.match('OP_Prefix.htm')||ToBeOpened.match('OP_Postfix.htm')||ToBeOpened.match('OP_NumOfDigits.htm')||ToBeOpened.match('OP_Step.htm')||ToBeOpened.match('OP_Rule.htm')||ToBeOpened.match('OP_StartValue.htm')||ToBeOpened.match('OP_GroupQueryView.htm')||ToBeOpened.match('OP_CaseInsensitive.htm')||ToBeOpened.match('OP_DeleteAttribute.htm')||ToBeOpened.match('OP_NewAttribute.htm')||ToBeOpened.match('OP_NewAttribute.htm')||ToBeOpened.match('OP_Description.htm')||ToBeOpened.match('OP_Name.htm')){
	 ThisOutputLineUFFA  =  ThisOutputLineUFFA.replace('/ttwizardoptions/','');	
	//alert(ThisOutputLineUFFA);
	document.write(ThisOutputLineUFFA);}
	else{
		//alert(ThisOutputLine2);
		ThisOutputLine2  =  ThisOutputLine2.replace('/ttwizardoptions/','');			
		document.write(ThisOutputLine2);
	}
}
else {
	//alert(ThisOutputLine2);
	document.write(ThisOutputLine2);
}
//==========================================
}