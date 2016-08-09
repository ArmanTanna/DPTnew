function clickHandler() {
  var targetId, srcElement, targetElement;
  srcElement = window.event.srcElement;
  if (srcElement.className == "Outline") {
     targetId = srcElement.id + "d";
     targetElement = document.all(targetId);
     if (targetElement.style.display == "none") {
        targetElement.style.display = "";
        
     } else {
        targetElement.style.display = "none";
        
     }
  }
}

document.onclick = clickHandler;