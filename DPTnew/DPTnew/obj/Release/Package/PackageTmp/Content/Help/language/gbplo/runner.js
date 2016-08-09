/* Runner.js : This scripts is used together with the Redax.DLL to run 
an external program or open an external file from inside an HTML page. 

FOR INTERNAL TEST ONLY : the default directories and other parameters 
refer to Think3 Internal LAN !!

This script must be located in the directory given in the HTML script call
(see the calling HTML page for an example)

See the RunCommand Function for the expected parameters.

Alex Martelli (alma@cadlab.it) and Alessandro Bottoni (bottoni@cadlab.it) for Think3 Inc. March 1999.
Redax.DLL by Alex Martelli (alma@cadlab.it), Think3 Inc. January 1999.

Last Revision: April, the 8th, 1999.
*/

// Auxiliary function: it just create an Array object.
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
// "e:\" (CD-ROM) or "c:\data\avi" (hard disk).
RegistryKeys[1] = 'SOFTWARE/think3/thinkdesign/4.0/Options/LocalAVIDir';
// LANAVIDir is the location of the same files on LAN server, if any.
RegistryKeys[2] = 'SOFTWARE/think3/thinkdesign/4.0/Options/LANAVIDir';
// WebAVIDir is the location of the same files on a web server (URL).
RegistryKeys[3] = 'SOFTWARE/think3/thinkdesign/4.0/Options/WebAVIDir';
var TotalTries = 4;

// Default "Base" URL (Hard Encoded)
//var DefaultDir = 'http://pc6xa16.cadlab.it/Avi/';
var DefaultDir = 'http://thinkcare.thinkthree.com/Avi/';

var CurrentTry;
var BaseDir;
// Attempted command : mainly debug purpose...
var AttemptedCommand = "Unknown";
// bDLLOK keeps track of the availability of the requested DLLs:
// if just one of them is missing, bDLLOK is FALSE.
var bDLLOK = true; 

// Auxiliary Function: setting bDLLOK to false
function SignalNoDLL()
{
	bDLLOK = false;
	TotalTries = 1;
}

// try reading an entry from registry, or, last-chance, return the
// default directory; return false if even that last-chance has already
// been exhausted
function ScanRegistry()
{
	while(CurrentTry < TotalTries) {
		// try to read this entry from registry (NB: we are certain here that
		// the redirector DLL is present, else TotalTries would be 1)
		BaseDir = TheForm.Redirector.ReadRegistry(RegistryKeys[CurrentTry]);
		if(TheForm.Redirector.LastError != 0) {
			// unable to read this entry from registry, try the next one
			TheForm.Redirector.LastError = 0;
			++CurrentTry;
		} else {
			// we managed to read an entry from registry
			if(CurrentTry == TotalTries-1) {
				// For a URL call, the "Open" verb MUST be used
				TheForm.Redirector.Verb = 'Open';
			}
			return true;
		}
	}
    if(CurrentTry > TotalTries) return false;
	if(CurrentTry == TotalTries) {
		// Use "Last-Chance" URL
		BaseDir = DefaultDir;
		if(bDLLOK) {
			// For a URL call, the "Open" verb MUST be used
			TheForm.Redirector.Verb = 'Open';
		}
		return true;
	}
}

// make sure that a string ends with /  (or \)
function AddTrailingSlash(sString)
{
	var NameLength = sString.length;
	var LastChar = sString.charAt(NameLength-1);
	
	if(LastChar=="/" || LastChar=="\\") return sString;
	return sString+"/";
}

// try to execute the file, return true on success, false on failure
function ExecuteCommand(FileToBeOpened)
{
	AttemptedCommand = AddTrailingSlash(BaseDir) + FileToBeOpened;
	if (!bDLLOK) {
		open (AttemptedCommand, "_self");
		// "Open" does not return an error value that we could check, so...
		return true;
	} else {
		TheForm.Redirector.Exec(AttemptedCommand);
		if(TheForm.Redirector.LastError != 0) {
			TheForm.Redirector.LastError = 0;
			// failure case
			return false;
		} else {		
			// No error detected -- success case
			return true;
		}
	}		
}

/* Function used to retrieve and open the required file.
Expected Parameters:
1) sVerb (Mandatory, NO Default) = Action to be performed. 
	Use "Open" to open a data file or run a program and "Play" to play a movie.
2) sAviName (Mandatory, NO Default) = Name of the file to be processed. 
	It can be prefixed by its parent directory (NO initial slash in any case!).
	This parameter will be used as the final part of the whole UNC/URL to be invoked.
	valid Examples: avi\s451-87.avi , s451-87.avi.
Expected Behavior:
1) A whole URL or UNC path will be built up by joining the parts defined by
	the regsitry keys and the call and an object will be invoked.
2) It is up to Windows to open/Run/Play the object with the related application
*/
function RunCommand(sVerb, sAviName)
{	
	// Set the Verb (if the Redirector is present)
	if (bDLLOK) {
		TheForm.Redirector.Verb = sVerb;
	}

	// re-start trying, every time, from the 1st possibility
	CurrentTry=1;
	for(;;) {
	// one exit case is on failure of ScanRegistry (we can try nothing else...)
		if(!ScanRegistry()) {
			alert("Error running : " + AttemptedCommand + "\nPlease read the Installation notes");
			break;
		}
	// other exit case is on success of ExecuteCommand (everything presumably done OK)
		if(ExecuteCommand(sAviName)) break;
	// neither exit case applied, so, try again, on next possibility
		++CurrentTry;
	}
}

/* Function used to open the local copy of the Tutorial.chm at a given page.
A "last-chance" feature is used when no DLL and/or no registry keys are
available on the user PC 
The sPage argument starts with a slash ('/').
The sCHMorDir is the name of a CHM file without the extension and the trailing "::"
and is also used as a the Parent Directory name in the calls to HTML files (optional)
*/
function OpenTutorial(sPage, sCHMorDir)
{
	// Local variables
//	var DefaultTutorialDir = 'http://pc6xa16.cadlab.it/';
	var DefaultTutorialDir = 'http://thinkcare.thinkthree.com/';
	var sDefaultParentDir = 'tutorial';
	var sBaseDir = '';
	var sParentDir = '';
	var sFile = '';
	var DefaultTutorialLanguage = 'English';
	var sLanguageDir = '';
	var sTutorialFile = "Tu_400.chm::";	
	// var sPrologue = "ms-its:"; // Compliant with MS IE 4.0 or later (accordingly to MS...)
	var sPrologue = "mk:@MSITStore:"; // Compliant with MS IE 3.0
	var ToBeOpened = '';	
	
	if (sCHMorDir) {
		sFile = sCHMorDir + ".chm::";
		sParentDir = sCHMorDir;
	} else {
		sFile = sTutorialFile;
		sParentDir = sDefaultParentDir;
	}

	if(bDLLOK) {
		TheForm.Redirector.Verb = 'open';		
		sBaseDir = TheForm.Redirector.ReadRegistry('SOFTWARE/think3/thinkdesign/4.0/Options/HelpDirCD');
		if(TheForm.Redirector.LastError != 0) {
			bDLLOK = false;
			TheForm.Redirector.LastError = 0;
		}
	}
	
	if(bDLLOK) {
		sLanguageDir = TheForm.Redirector.ReadRegistry('SOFTWARE/think3/thinkdesign/4.0/Options/Language');
		if(TheForm.Redirector.LastError != 0) {
			bDLLOK = false;
			TheForm.Redirector.LastError = 0;
		}
	}
		
	if(bDLLOK) {
		ToBeOpened = sPrologue + sBaseDir + sLanguageDir + "\/" + sFile + "\/" + sLanguageDir + sPage;	

	} else {
		// If the DLL or any of the Registry Keys are not available, we open the uncompiled version 
		// of the Tutorial that resides on the ThinkThree Web Server.
		ToBeOpened = DefaultTutorialDir + DefaultTutorialLanguage + "\/" + sParentDir + sPage;
	}
	
	open (ToBeOpened, "_self");
}

function OpenSolution(sPage, sCHMorDir)
{
	// Local variables
	var DefaultTutorialDir = 'http://thinkcare.thinkthree.com/';
	var sDefaultParentDir = 'Solutions';
	var sBaseDir = '';
	var sParentDir = '';
	var DefaultTutorialLanguage = 'English';
	var sLanguageDir = '';
	var ToBeOpened = '';	
	
	if (sCHMorDir) {
		sParentDir = sCHMorDir;
	} else {
		sParentDir = sDefaultParentDir;
	}

	if(bDLLOK) {
		TheForm.Redirector.Verb = 'open';		
		sBaseDir = TheForm.Redirector.ReadRegistry('SOFTWARE/think3/thinkdesign/4.0/Options/SolutionsDir');
		if(TheForm.Redirector.LastError != 0) {
			bDLLOK = false;
			TheForm.Redirector.LastError = 0;
		}
	}
	
	if(bDLLOK) {
		sLanguageDir = TheForm.Redirector.ReadRegistry('SOFTWARE/think3/thinkdesign/4.0/Options/Language');
		if(TheForm.Redirector.LastError != 0) {
			bDLLOK = false;
			TheForm.Redirector.LastError = 0;
		}
	}
		
	if(bDLLOK) {
		ToBeOpened = sBaseDir + sLanguageDir + "\\" + sParentDir + sPage;	
	} else {
		// If the DLL or any of the Registry Keys are not available, we open the uncompiled version 
		// of the Tutorial that resides on the ThinkThree Web Server.
		ToBeOpened = DefaultTutorialDir + DefaultTutorialLanguage + "\/" + sParentDir + sPage;
	}
	
	open (ToBeOpened, "_self");
}

