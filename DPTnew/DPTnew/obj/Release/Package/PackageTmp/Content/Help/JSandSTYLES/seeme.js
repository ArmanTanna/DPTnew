//
/*
Gestione dell'errore.
Nel caso di IE6sp1, quando la doc NON �Einstallata su un disco locale (cio�Ese �Ein rete) si verificano errori JavaScript a causa delle nuove restrizioni di security inserite a tradimento da Microsoft.
Al posto di un errore JavaScript, visualizziamo un messaggio in cui si informa l'utente che questa situazione si verifica se l'installazione non �Elocale.
*/
function errorHandler() {
if(this.location.href.match('english')) {
        alert("To view the desired page, install the documentation to your machine.\nIf the problem persists, contact think3 Customer Care (e-mail: thinkcare@think3.com).");
}
if(this.location.href.match('italian')) {
	    alert("Per visualizzare la pagina di guida desiderata, installare la documentazione sulla propria macchina.\nSe il problema persiste, contattare l'Assistenza clienti di think3 (e-mail: thinkcare@think3.com).");
}
if(this.location.href.match('french')) {
        alert("Pour visualiser la page d'Aide d�sir�e, installer localement la documentation sur la machine.\nSi le probl�me persiste, contacter l'Assistance Clients de think3 (e-mail: thinkcare@think3.com).");
}
if(this.location.href.match('german')) {
        alert("Installieren Sie zur Anzeige der gew�Eschten Hilfe-Seite die Dokumentation auf Ihrem Rechner.\nWenn das Problem bestehen bleibt, wenden Sie sich an den think3-Kundendienst (e-mail: thinkcare@think3.com).");
}
if(this.location.href.match('japanese')) {
        alert("�ړI�̃y�[�W��\������ɂ́A���g���̃R���s���[�^�Ƀh�L�������g���C���X�g�[�����Ă��������B\n��肪�Ĕ�����ꍇ�́Athink3 Customer Care�ie-mail: thinkcare@think3.com�j�ɂ��₢���킹���������B");
}
	 return true;
}
window.onerror = errorHandler;
//
//
//
if(this.location.href.match('english')) {
   seeme="<span style=\"font-size: 9pt;\">Click here to see an image<\/span>";
}
if(this.location.href.match('italian')) {
	  seeme="<span style=\"font-size: 9pt;\">Fare clic qui per vedere un'immagine<\/span>";
}
if(this.location.href.match('french')) {
	  seeme="<span style=\"font-size: 9pt;\">Cliquez ici pour afficher l'image.<\/span>";
}
if(this.location.href.match('german')) {
	  seeme="<span style=\"font-size: 9pt;\">Klicken Sie hier, um das Bild anzuzeigen.<\/span>";
}
if(this.location.href.match('japanese')) {
	  seeme="<span style=\"font-size: 9pt;\">;\">�N���b�N����ƁA�֘A����C���[�W���\������܂�<\/span>";
}
 
