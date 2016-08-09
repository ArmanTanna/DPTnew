//Generic Display code
//Cleaned up
//Updated 9/12/05 - updated Outline3()to ExpandCollapse()

function ExpandCollapse()
{

	window.event.returnValue=0	

	//Expand or collapse if a list item is clicked.
	var open = event.srcElement;

	//Verify that the tag which was clicked was either the 
	//trigger tag or nested within a trigger tag.
	var el = checkParent(open,"A");
	if(null != el)
	{	
		var incr=0;
		var elmPos = 0;
		var parentSpan;
		var fBreak

		//Get the position of the element which was clicked
		elemPos = window.event.srcElement.sourceIndex;

		//Search for a SPAN tag
		for (parentSpan = window.event.srcElement.parentElement;
			parentSpan!=null;
			parentSpan = parentSpan.parentElement) 
		{
			//test if already at a span tag 
		    if (parentSpan.tagName=="DIV") 
			{
				//alert("Parent Element is a SPAN");
				//incr=1;
				//break;
			}
			
			//Test if the tag clicked was in a body tag or in any of the possible kinds of lists
			//we perform this test because nested lists require special handling
			if (parentSpan.tagName=="BODY" || parentSpan.tagName=="UL" || parentSpan.tagName=="OL"|| parentSpan.tagName=="P") 
			{
				//Determine where the span to be expanded is.  
				for (incr=1; (elemPos+incr) < document.all.length; incr++)
				{	
					//verify we are at an expandable Div tag
					if(document.all(elemPos+incr).tagName=="DIV" && 
					(document.all(elemPos+incr).className=="expanded" ||
					 document.all(elemPos+incr).className=="collapsed"))
					{
						fBreak=1;
						break;
					}
					//If the next tag following the list item (li) is another 
					//list item(li) return in order to prevent accidentally opening
					//the next span in the list
					else if(document.all(elemPos+incr).tagName=="LI")
					{
						return;
					}
				}
			}
			//determine if we need to break out of the while loop (kind of a kludge since theres no goto in javascript)
			if(fBreak==1)
			{
				break;
			}
		}

	}
	else
	{
		Alert("Return!");
		return;
	}

	//Now that we've identified the span, expand or collapse it
	//Now that we've identified the span, expand or collapse it
	if(document.all(elemPos+incr).className=="collapsed")
	{
		
		document.all(elemPos+incr).className="expanded"
		document.all(elemPos+1).src="../../../images/general/aperto.gif";
		if(open.tagName=="IMG"){open.src="../../../images/general/aperto.gif";}
		if(open.tagName=="B")
			{
			if(open.parentElement.all.tags("IMG").length != 0)
				{open.parentElement.all.tags("IMG").item(0).src="../../../images/general/aperto.gif";}
			}
	}
	else if(document.all(elemPos+incr).className=="expanded")
	{
		document.all(elemPos+incr).className="collapsed"
		document.all(elemPos+1).src="../../../images/general/chiuso.gif";
		if(open.tagName=="IMG"){open.src="../../../images/general/chiuso.gif";}
		if(open.tagName=="B")
			{
			if(open.parentElement.all.tags("IMG").length != 0)
				{open.parentElement.all.tags("IMG").item(0).src="../../../images/general/chiuso.gif";}
			}
	}
	else
	{
		return;
	}
	event.cancelBubble = true;
//	open.scrollIntoView(true);
}
function ExpandAll()
{
//diff values for elempos and my_pin_point !! watchout 2005/Sept 23 RAS

	//alert ("I am here"+ document.all.length);
	var counter
	counter=0
for (counter=0; counter < document.all.length; counter++)
{	
	if(document.all(counter).className=="collapsed")
	  {
	  	//alert("collapsed one found no =" +counter)
	  	document.all(counter).className="expanded"
		document.all(counter-1).src="../../../images/general/aperto.gif";
	  }
	
}
}

function CollapseAll()
{
	window.location.reload(true);
}
function checkParent(src,dest)
{
	//Search for a specific parent of the current element.
	while(src !=null)
	{
		if(src.tagName == dest)
		{
			return src;
		}
		src = src.parentElement;
	}
	return null;
}
