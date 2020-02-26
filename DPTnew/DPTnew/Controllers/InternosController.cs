using DPTnew.Helper;
using DPTnew.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace DPTnew.Controllers
{
    [Authorize]
    [Authorize(Roles = "Admin,Internal")]
    public class InternosController : Controller
    {
        public ActionResult Index()
        {
            Session["CurrentCulture"] = LocalizationHelper.SetLocalization(Session["CurrentCulture"]);
            return View();
        }

        [Authorize(Roles = "Admin,Internal")]
        public ActionResult UpdateAccountStatus()
        {
            using (var db = new DptContext())
            {
                var errormsg = "";
                try
                {
                    db.Database.ExecuteSqlCommand("exec [dbo].[Update_AccountStatus_Every8Hours]");
                }
                catch (Exception e)
                {
                    LogHelper.WriteLog("InternosController (UpdateAccountStatus): " + e.Message);
                    errormsg = e.Message;
                }
                ViewBag.Message = errormsg;

                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UnknownCompanies()
        {
            using (var db = new DptContext())
            {
                var clist = db.uCust.ToList();
                var errormsg = "";
                try
                {
                    foreach (var ip in clist)
                    {
                        string url = "http://api.ipstack.com/" + ip.ipcustomer.ToString() +
                            "?access_key=18d12dc4a27d99b80244e0e67b2be52d";
                        WebClient client = new WebClient();
                        string jsonstring = client.DownloadString(url);
                        dynamic dynObj = JsonConvert.DeserializeObject(jsonstring);
                        ip.Country = dynObj["country_name"];
                        ip.Region = dynObj["region_name"];
                        ip.City = dynObj["city"];
                        ip.ZipCode = dynObj["zip"];
                        ip.Latitude = dynObj["latitude"];
                        ip.Longitude = dynObj["longitude"];
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    LogHelper.WriteLog("InternosController (UnknownCompanies): " + e.Message);
                    errormsg = e.Message;
                }
                ViewBag.Message = errormsg;

                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult VarCompanies()
        {
            using (var db = new DptContext())
            {
                var res = from c in db.Companies
                          orderby c.SalesRep
                          group c by c.SalesRep into grp
                          select new { key = grp.Key, cnt = grp.Count() };

                var data = new List<string>();
                foreach (var r in res)
                    data.Add(r.key + "-" + r.cnt);

                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(data).ToString(Formatting.None));
                ViewBag.Data = System.Convert.ToBase64String(plainTextBytes);

                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult UpdateMailStatus()
        {
            using (var db = new DptContext())
            {
                var errormsg = "";
                var apiKey = "ea9a8e767cc1d327e9f188430a3c270f-us5";
                var listIds = new List<string>();
                listIds.Add("245d406f69"); // think3 English Newsletter
                listIds.Add("6f255b0e31"); // think3 Italian Newsletter
                listIds.Add("c37be5baf4"); //think3 Japanese Newsletter
                listIds.Add("43824c52ba"); //Initiatives
                var wc = new WebClient();
                wc.Headers.Add("Authorization", "apikey " + apiKey);
                foreach (var listId in listIds)
                {
                    try
                    { // cleaned member list
                        var url = String.Format("https://{0}.api.mailchimp.com/3.0/{1}", "us5", "lists/" + listId + "/members?status=cleaned&count=1500");
                        string jsonstring = wc.DownloadString(url);
                        JObject contentresult = JObject.Parse(jsonstring);
                        JToken member = contentresult["members"];
                        var x = member.Count();
                        foreach (JObject mail in member)
                        {
                            string ema = mail["email_address"].ToString();
                            var query = from ppl in db.Peoples
                                        where ppl.Email == ema
                                        select ppl;
                            if (query.Count() > 0 && query.FirstOrDefault().EmailStatus != "ko: na")
                            {
                                db.Database.ExecuteSqlCommand("UPDATE [dbo].[DPT_People] Set EmailStatus = 'ko: nd' WHERE Email = '" + ema + "'");
                            }
                        }
                        // unsubscribed member list
                        url = String.Format("https://{0}.api.mailchimp.com/3.0/{1}", "us5", "lists/" + listId + "/members?status=unsubscribed&count=1500");
                        jsonstring = wc.DownloadString(url);
                        contentresult = JObject.Parse(jsonstring);
                        member = contentresult["members"];
                        x = member.Count();
                        foreach (JObject mail in member)
                        {
                            string ema = mail["email_address"].ToString();
                            var query = from ppl in db.Peoples
                                        where ppl.Email == ema
                                        select ppl;
                            if (query.Count() > 0 && query.FirstOrDefault().EmailStatus != "ko: na")
                            {
                                if (listId != "43824c52ba")
                                    db.Database.ExecuteSqlCommand("UPDATE [dbo].[DPT_People] Set EmailStatus = 'ko: nl' WHERE Email = '" + ema + "'");
                                else //Initiatives
                                    db.Database.ExecuteSqlCommand("UPDATE [dbo].[DPT_People] Set EmailStatus = 'ko: na' WHERE Email = '" + ema + "'");
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteLog("InternosController (UpdateMailStatus): " + e.Message);
                        errormsg = e.Message;
                    }
                }
                ViewBag.Message = errormsg;

                return View();
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult SendZeroCampaignMail()
        {
            //using (StreamReader reader = new StreamReader(file.InputStream))
            //{
            var errormsg = "";
            var line = "";
            var inClause = new string[] { "tdstyling", "tteampdm", "thinkprint", "tdmolding", "tdxchangereader",
                                "tdengineering", "teamdev", "tdtooling", "tdengineeringplus", "tdbase", "tdprofessional", "tddrafting" };
            //while ((line = reader.ReadLine()) != null)
            //{
            using (var db = new DptContext())
            {
                try
                {
                    var cmpcompanies = from cmp in db.CmpCompanies
                                       where cmp.Flag.ToLower() == "si" && cmp.Campaigns.ToLower() == "zero"
                                       select cmp;
                    if (cmpcompanies.Count() > 0 && cmpcompanies.Count() < 501)
                    {
                        foreach (var cp in cmpcompanies.ToList())
                        {
                            line = cp.AccountNumber;
                            var lics = from lic in db.Licenses
                                       where lic.AccountNumber == line.Trim() && lic.LicenseID.StartsWith("L") &&
                                       inClause.Contains(lic.ProductName.ToLower()) && lic.MaintEndDate < DateTime.Now
                                       select lic;
                            var comp = from cmp in db.Companies
                                       where cmp.AccountNumber == line.Trim()
                                       select cmp;
                            Dictionary<string, string> machines = new Dictionary<string, string>();
                            if (lics.Count() > 0)
                            {
                                foreach (var lic in lics.ToList())
                                {
                                    if (machines.ContainsKey(lic.MachineID))
                                    {
                                        string val = "";
                                        machines.TryGetValue(lic.MachineID, out val);
                                        val += "," + lic.LicenseID + "-" + lic.ProductName + "-" + lic.LicenseType + "-" + lic.Quantity;
                                        machines[lic.MachineID] = val;
                                    }
                                    else
                                        machines.Add(lic.MachineID, lic.LicenseID + "-" + lic.ProductName + "-" + lic.LicenseType + "-" + lic.Quantity);
                                }
                            }
                            MailMessage mail = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["hostusername"], "info@dptcorporate.com");
                            if (comp.FirstOrDefault().Language.ToLower() == "italian")
                            {
                                mail.Subject = "Iniziativa Zero - " + comp.FirstOrDefault().AccountName;
                                mail.Body = "Email: " + comp.FirstOrDefault().Email +
                                    "<br/><br/>Gentile Cliente, <br/><br/>Le ricordiamo che l’<b>Iniziativa Zero</b> (http://bit.ly/IniziativaZero)," +
                                    " che Le permetterà di ottenere <b>gratuitamente</b> delle licenze <b>think3 aggiornate</b>" +
                                    " all’ultima versione, scadrà il 21 giugno 2017.<br/>Dopo tale data non sarà più possibile usufruire" +
                                    " di questa – a nostro parere - irrinunciabile opportunità.<br/><br/>Per facilitare la Sua eventuale adesione," +
                                    " in calce troverà il testo di un documento che ha una doppia valenza. Rappresenta " +
                                    "una descrizione delle procedure di implementazione e, allo stesso tempo, il modulo di adesione." +
                                    "<br/><br/>La ringraziamo e Le auguriamo una buona giornata!<br/>Cordiali saluti,<br/><br/>" +
                                    "Team DPT/think3<br/>Viale Angelo Masini, n. 12<br/>c/o Regus - 40126 Bologna<br/>Tel. +39 051 092 3545<br/>" +
                                    "<br/>www.dptcorporate.com<br/>www.think3.eu <br/><br/><hr><br/><br/><u>TESTO DA " +
                                    "RIPORTARE SU CARTA INTESTATA DELL’AZIENDA, CON FIRMA E TIMBRO DEL CEO O DEL RESPONSABILE" +
                                    " DI DIPARTIMENTO</u>:<br/><br/><br/><i>Luogo, data<br/><br/><br/>" +
                                    "Spett. DPT,<br/><br/>Con la presente comunichiamo la nostra adesione all’Iniziativa " +
                                    "Zero da Voi proposta.<br/><br/>Siamo consapevoli che la suddetta Iniziativa:<br/><br/>" +
                                    "a) comporterà la conversione di tutte le nostre licenze permanenti (PL) fuori " +
                                    "manutenzione in licenze temporanee annuali (ASF), che potremo usare su PC nuovi e/o " +
                                    "in versioni più aggiornate. Tale conversione avverrà esportando le vecchie licenze – " +
                                    "se queste sono legate alle macchine – e/o restituendo le chiavi parallele/USB presso " +
                                    "gli uffici di DPT nel caso di licenze su dispositivi hardware.<br/><br/>" +
                                    "b) è completamente a costo zero e ci consentirà di utilizzare gratuitamente le nuove" +
                                    " licenze fino al 31/05/2018, data in cui le stesse smetteranno di funzionare se non " +
                                    "rinnovate.<br/><br/><table border=1><tr><td align='center'>ID Licenza</td>" +
                                    "<td align='center'>Prodotto</td><td align='center'>ID Macchina</td>" +
                                    "<td align='center'>Tipo Licenza</td><td align='center'>Quantitá</td></tr>";
                                mail.BodyEncoding = System.Text.Encoding.UTF8;
                            }
                            else
                            {
                                mail.Subject = "Initiative Zero - " + comp.FirstOrDefault().AccountName;
                                mail.Body = "Email: " + comp.FirstOrDefault().Email +
                                "<br/><br/>Dear Customer,<br/><br/>We would like to remind you that <b>Initiative Zero</b>" +
                                " (http://bit.ly/InitiativeZero), which allows you to receive up-to-date <b>think3 licenses at" +
                                " no cost</b>, will expire on June 21st, 2017.<br/>After this date, it will no longer " +
                                "be possible to subscribe this  – in our opinion - extraordinary opportunity.<br/><br/>" +
                                "To make it easier for you to join, at the bottom of this email you will find the text" +
                                " of a document that has a double value. It represents a description of the implementation" +
                                " procedures and, at the same time, the application form.<br/><br/>Thank you and have a " +
                                "good day.<br/>Best regards,<br/><br/>DPT/think3 Team<br/>Viale Angelo Masini, n. 12" +
                                "<br/>c/o Regus - 40126 Bologna<br/>Tel. +39 051 092 3545<br/><br/>www.dptcorporate.com" +
                                "<br/>www.think3.eu<br/><br/><hr><br/><br/><u>THE FOLLOWING TEXT HAS TO BE COPIED ON THE " +
                                "COMPANY’S HEADED PAPER, WITH THE STAMP AND THE SIGNATURE OF THE CEO OR OF THE PERSON IN " +
                                "CHARGE OF THE DEPARTMENT.</u><br/><br/><br/><i>Location, date<br/><br/><br/>" +
                                "Dear DPT,<br/><br/>we hereby confirm our enrollment in the Initiative Zero.<br/><br/>" +
                                "We are aware that this Initiative:<br/><br/>a) entails the conversion of all our " +
                                "out-of-maintenance permanent licenses (PL) into ASF (Annual Subscription Fee) " +
                                "licenses, which we could use on new PCs and/or in up-to-date versions. This conversion" +
                                " will take place by exporting our old licenses – if they are linked to workstations –" +
                                " and/or returning the USB/parallel dongles to DPT offices in case of licenses on hardware" +
                                " keys.<br/><br/>b) is completely free of charge and it will allow us to use the new licenses" +
                                " until 31/05/2018; after this date, they will stop working unless we renew them.<br/>" +
                                "<br/><table border=1><tr><td align='center'>License ID</td><td align='center'>Product</td>" +
                                "<td align='center'>Machine ID</td><td align='center'>License type</td><td align='center'>Quantity</td></tr>";
                            }
                            foreach (var mac in machines.Keys)
                            {
                                var vl = "";
                                machines.TryGetValue(mac, out vl);
                                var values = vl.Split(',');
                                var licids = "";
                                var prods = "";
                                var ltype = "";
                                var qty = 0;
                                var i = 0;
                                foreach (var v in values)
                                {
                                    if (i == 0)
                                    {
                                        licids += v.Split('-')[0];
                                        prods += v.Split('-')[1];
                                        ltype += v.Split('-')[2];
                                        i++;
                                        if (ltype.Contains("f"))
                                            qty += Int32.Parse(v.Split('-')[3]);
                                    }
                                    else
                                    {
                                        licids += " / " + v.Split('-')[0];
                                        prods += " / " + v.Split('-')[1];
                                        if (ltype.Contains("f"))
                                            qty += Int32.Parse(v.Split('-')[3]);
                                    }
                                }
                                mail.Body += "<tr><td>" + licids + "</td><td>" + prods + "</td><td align='center'>" + mac +
                                    "</td><td align='center'>" + ltype + "</td><td align='center'>" + (ltype.Contains("f") ? qty.ToString() : "") + "</td></tr>";
                            }
                            if (comp.FirstOrDefault().Language.ToLower() == "italian")
                            {
                                mail.Body += "</table><br/>Nota bene: il costo delle licenze in modalità floating corrisponde al 15% in più " +
                                    "rispetto ai prezzi segnalati sopra.<br/><br/>c) Ci dichiariamo altresì consapevoli che, alla scadenza del " +
                                    "periodo di 12 mesi ad uso gratuito, potremo decidere liberamente se e quante licenze " +
                                    "rinnovare – sempre in modalità ASF -, facendo riferimento ai prezzi speciali riportati " +
                                    "nella tabella sottostante.<br/>Il costo del rinnovo per il secondo anno (e quelli " +
                                    "successivi) corrisponde alla metà del prezzo di listino previsto per le licenze ASF, " +
                                    "come illustrato nella tabella sotto per alcuni dei principali prodotti DPT.<br/><br/>" +
                                    "<table border=1><tr><td>Prodotto</td><td>Prezzo rinnovo</td></tr>" +
                                "<tr><td>TDEngineering</td><td align='center'>1.300€</td></tr><tr><td>TDProfessional</td><td align='center'>2.400€</td></tr>" +
                                "<tr><td>TDTooling</td><td align='center'>1.900€</td></tr><tr><td>TTeamPDM</td><td align='center'>320€</td></tr></table><br/><br/>" +
                                "d) Una volta firmato questo documento e dopo aver esportato le vecchie licenze permanenti e/o" +
                                " restituito le chiavi USB/parallele, DPT provvederà a fornirci le licenze annuali che ci " +
                                "spettano.<br/><br/>Cordiali Saluti,<br/><br/><br/>Firma del CEO &<br/>Timbro dell’azienda</i>";
                            }
                            else
                            {
                                mail.Body += "</table><br/>Please, note that the price for floating licenses increases by 15%." +
                                    "<br/><br/>c) We also acknowledge that, after the free 12-months period," +
                                    " we can freely decide whether and how many licenses we want to renew – always as " +
                                    "ASF -, by referring to the special prices shown in the table below.<br/>The renewal" +
                                    " price for the 2nd year (and the years after) corresponds to half of the ASF price " +
                                    "established in DPT Price List, as shown in the table here below for some of the main" +
                                    " DPT products.<br/><br/><table border=1><tr><td>Product</td><td>Renewal price</td></tr>" +
                                    "<tr><td>TDEngineering</td><td align='center'>1.300€</td></tr><tr><td>TDProfessional</td>" +
                                    "<td align='center'>2.400€</td></tr><tr><td>TDTooling</td><td align='center'>1.900€</td></tr>" +
                                    "<tr><td>TTeamPDM</td><td align='center'>320€</td></tr></table><br/><br/>d) Once we send" +
                                    " back this signed document and the .tbu files/dongles, DPT will provide us with " +
                                    "the ASF (Annual Subscription Fee) licenses in the version we request.<br/><br/>Best regards," +
                                    "<br/><br/><br/>CEO Signature & <br/>Company Stamp</i>";
                            }
                            mail.IsBodyHtml = true;
                            MailHelper.SendMail(mail);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogHelper.WriteLog("InternosController (Mail): accountnumber - " + line + " -- " + e.Message + "-" + e.InnerException);
                    errormsg += line + " (" + e.Message + "-" + e.InnerException + "); </br>";
                }
            }
            //}
            //}

            ViewBag.Message = errormsg;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult SendZeroBisCampaignMail()
        {
            var errormsg = "";
            var line = "";
            var inClause = new string[] { "tdstyling", "tteampdm", "thinkprint", "tdmolding", "tdxchangereader",
                                "tdengineering", "teamdev", "tdtooling", "tdengineeringplus", "tdbase", "tdprofessional", "tddrafting" };
            using (var db = new DptContext())
            {
                try
                {
                    var cmpcompanies = from cmp in db.CmpCompanies
                                       where cmp.Flag.ToLower() == "si" && cmp.Campaigns.ToLower() == "zerobis"
                                       select cmp;
                    if (cmpcompanies.Count() > 0 && cmpcompanies.Count() < 501)
                    {
                        foreach (var cp in cmpcompanies.ToList())
                        {
                            line = cp.AccountNumber;
                            var lics = from lic in db.Licenses
                                       where lic.AccountNumber == line.Trim() && lic.LicenseID.StartsWith("L") &&
                                       inClause.Contains(lic.ProductName.ToLower()) && lic.MaintEndDate < DateTime.Now
                                       select lic;
                            var comp = from cmp in db.Companies
                                       where cmp.AccountNumber == line.Trim()
                                       select cmp;
                            Dictionary<string, string> machines = new Dictionary<string, string>();
                            if (lics.Count() > 0)
                            {
                                foreach (var lic in lics.ToList())
                                {
                                    if (machines.ContainsKey(lic.MachineID))
                                    {
                                        string val = "";
                                        machines.TryGetValue(lic.MachineID, out val);
                                        val += "," + lic.LicenseID + "-" + lic.ProductName + "-" + lic.LicenseType + "-" + lic.Quantity;
                                        machines[lic.MachineID] = val;
                                    }
                                    else
                                        machines.Add(lic.MachineID, lic.LicenseID + "-" + lic.ProductName + "-" + lic.LicenseType + "-" + lic.Quantity);
                                }
                            }
                            MailMessage mail = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["hostusername"], "info@dptcorporate.com");
                            var mails = from ppl in db.Peoples
                                        where ppl.AccountNumber == line.Trim()
                                        select ppl;
                            switch (comp.FirstOrDefault().Language.ToLower())
                            {
                                case "italian":
                                    mail.Subject = "Iniziativa Zero Bis - " + comp.FirstOrDefault().AccountName;
                                    if (mails.Count() > 0)
                                    {
                                        mail.Body = "Email: ";
                                        foreach (var ml in mails.ToList())
                                            mail.Body += ml.Email + "; ";
                                    }
                                    else
                                        mail.Body = "Email: " + comp.FirstOrDefault().Email;
                                    mail.Body +=
                                        "<br/><br/>Gentile Cliente, <br/><br/>La ringrazio per essere un Cliente Attivo e " +
                                        "per il suo costante sostegno alle attività di Ricerca e Sviluppo su ThinkDesign. " +
                                        "Per questa ragione sto per proporle quella che ritengo essere una interessante opportunità" +
                                        " per entrambe le nostre Aziende.<br/><br/>Mi risulta infatti che alcune delle sue licenze " +
                                        "(veda la lista qui sotto) non siano ‘coperte’ da un contratto di manutenzione.<br/>" +
                                        "Vorrei fare in modo che tutte le licenze in uso presso di lei siano allo stato dell’arte" +
                                        " della tecnologia, esportabili su altri PC ed in definitiva beneficino di tutto ciò " +
                                        "che deriva dall’essere in manutenzione. Pertanto, le offro la possibilità di sostituire" +
                                        " <b>gratuitamente</b> le sue vecchie licenze permanenti fuori manutenzione con altrettante licenze" +
                                        " annuali <b>aggiornate ed in manutenzione per 12 mesi</b>. Tutto questo, merita ribadirlo, " +
                                        "<b>a costo zero</b> e senza alcun impegno da parte vostra, in vista di eventuali rinnovi futuri " +
                                        "di tali licenze.<br/><br/>Troverà maggiori informazioni a questo link: http://bit.ly/IniziativaZeroBis" +
                                        " oppure rivolgendosi al VAR di riferimento, che legge in copia.<br/><br/>Non perda quest’opportunità! " +
                                        "L’Iniziativa sarà valida per tutta l’estate, ma non oltre il <b>30 settembre 2017</b>.<br/><br/>" +
                                        "Per facilitare la sua eventuale adesione, in calce troverà il testo di un documento che ha " +
                                        "una doppia valenza. Rappresenta una descrizione delle procedure di implementazione e, " +
                                        "allo stesso tempo, il modulo di adesione.<br/><br/>La ringraziamo e Le auguriamo una buona giornata!" +
                                        "<br/>Cordiali saluti,<br/><br/>Massimo Signani<br/>CEO DPT/think3<br/><br/>" +
                                        "<br/>www.dptcorporate.com<br/>www.think3.eu <br/><br/><hr><br/><br/><u>TESTO DA " +
                                        "RIPORTARE SU CARTA INTESTATA DELL’AZIENDA, CON FIRMA E TIMBRO DEL CEO O DEL RESPONSABILE" +
                                        " DI DIPARTIMENTO</u>:<br/><br/><br/><i>Luogo, data<br/><br/><br/>" +
                                        "Spett. DPT,<br/><br/>Con la presente comunichiamo la nostra adesione all’Iniziativa " +
                                        "Zero Bis da Voi proposta.<br/><br/>Siamo consapevoli che la suddetta Iniziativa:<br/><br/>" +
                                        "a) comporterà la conversione di tutte le nostre licenze permanenti (PL) fuori " +
                                        "manutenzione in licenze temporanee annuali (ASF), che potremo usare su PC nuovi e/o " +
                                        "in versioni più aggiornate. Tale conversione avverrà esportando le vecchie licenze – " +
                                        "se queste sono legate alle macchine – e/o restituendo le chiavi parallele/USB presso " +
                                        "gli uffici di DPT nel caso di licenze su dispositivi hardware.<br/><br/>" +
                                        "b) è completamente a costo zero e ci consentirà di utilizzare gratuitamente le nuove" +
                                        " licenze fino al 30/06/2018, data in cui le stesse smetteranno di funzionare se non " +
                                        "rinnovate.<br/><br/><table border=1><tr><td align='center'>ID Licenza</td>" +
                                        "<td align='center'>Prodotto</td><td align='center'>ID Macchina</td>" +
                                        "<td align='center'>Tipo Licenza</td><td align='center'>Quantitá</td></tr>"; break;
                                case "japanese":
                                    mail.Subject = "Initiative Zero Bis - " + comp.FirstOrDefault().AccountName;
                                    if (mails.Count() > 0)
                                    {
                                        foreach (var ml in mails.ToList())
                                            mail.CC.Add(ml.Email);
                                    }
                                    else
                                        mail.CC.Add(comp.FirstOrDefault().Email);
                                    mail.Body +=
                                    "お客様各位,<br/><br/>本メールは、弊社の顧客情報をもとに送信しております。<br/>" +
                                    "御社におかれましては、日頃弊社ThinkDesign製品をお使いいただき誠にありがとうございます。<br/>" +
                                    "今まで保守契約を有効な状態でお持ちのお客様に対し、興味深い提案をご用意いたしましたので、ご検討いただきたく以下にご説明いたします。<br/><br/>" +
                                    "弊社の顧客情報によりますと、有効な契約が続いている中、数本のライセンスの保守契約が切れた状態となっております。（下記に記載のリストをご参照ください）" +
                                    "私共は、お持ちの全てのライセンスがお客様の環境で有効に活用されることを願い、保守契約締結の上で、他のPCへのライセンス移行や、お客様に対してのサポートを行っております。<br/><br/>" +
                                    "PLライセンスを残したままで、保守に加入していない期間を遡ってお支払いいただくキャンペーンは終了いたしました。<br/><br/>" +
                                    "お客様の現状をご確認いただくことやお客様の不自由を解決する意味も含め、保守契約が切れた分のPLライセンスを下取りした上で、ASF（年間ライセンス）と交換することをご提案いたします。" +
                                    "ASFはこれからの12ヶ月間の保守料金も含まれた年間使用料金となっております。<br/>" +
                                    "もしも、過去に購入したPLライセンスを上位製品に切り替えたり、もう既にご使用になっていないなどの理由で、そのままになっている場合、弊社または販売代理店に一度ご相談ください。<br/>" +
                                    "保守契約が切れているPLライセンスと入れ替えで１２ヶ月間お使いいただけるASFライセンスを無料でお渡しいたします。（既存のPLライセンスをASFライセンスに切り替えることになりますので、１２ヶ月を経過後に使用を続ける場合は、別途通常のASF料金が発生いたします）<br/><br/>" +
                                    "今回のご提案は、２０１７年9月末日までの限定となりますので、ぜひこの機会に、お客様ご自身のライセンスの状態をご確認いただきご検討をいただきますようにお願いいたします。<br/>" +
                                    "本件お問い合わせにつきましては、ご遠慮なくお取引先の販売代理店あるいはT3Japan株式会社 （info.jp@t3-japan.co.jp） 宛にご連絡ください。<br/><br/>" +
                                    "よろしくお願いいたします。<br/><br/><b>Massimo Signani</b>（マッシモ　シグナニ）<br/><b>CEO DPT/think3</b><br/><br/>" +
                                    "www.dptcorporate.com<br/>www.think3.eu<br/><br/><br/>More details at http://bit.ly/InitiativeZeroBis <br/><br/>" +
                                    "To make it easier for you to join, at the bottom of this email you will find the text of a document that has a double value. It represents a description of the implementation" +
                                    " procedures and, at the same time, the application form.<br/><br/><br/><hr><br/><br/><br/>" +
                                    "<u>THE FOLLOWING TEXT HAS TO BE COPIED ON <b>" + comp.FirstOrDefault().AccountName + "</b> HEADED PAPER, " +
                                    "WITH THE STAMP AND THE SIGNATURE OF THE CEO OR OF THE PERSON IN CHARGE OF THE DEPARTMENT.</u><br/><br/>" +
                                    "<i>Location, date<br/><br/><br/>Dear DPT,<br/><br/>we hereby confirm our enrollment in the Initiative Zero.<br/><br/>" +
                                    "We are aware that:<br/><br/>a.	this Initiative entails the conversion of all our out-of-maintenance permanent licenses (PL)" +
                                    " into ASF (Annual Subscription Fee) licenses, which we could use on new PCs and/or in up-to-date versions. This conversion " +
                                    "will take place by exporting our old licenses – if they are linked to workstations – and/or returning the USB/parallel dongles" +
                                    " to DPT offices in case of licenses on hardware keys.<br/><br/>b.	this Initiative is completely free of charge and it will " +
                                    "allow us to use the new licenses until 30/06/2018; after this date, they will stop working unless we renew them.<br/>" +
                                    "<br/><table border=1><tr><td align='center'><b>License ID</b></td><td align='center'><b>Product</b></td>" +
                                    "<td align='center'><b>Machine ID</b></td><td align='center'><b>Type</b></td><td align='center'><b>Quantity</b></td></tr>"; ;
                                    break;
                                default:
                                    mail.Subject = "Initiative Zero Bis - " + comp.FirstOrDefault().AccountName;
                                    if (mails.Count() > 0)
                                    {
                                        mail.Body = "Email: ";
                                        foreach (var ml in mails.ToList())
                                            mail.Body += ml.Email + "; ";
                                    }
                                    else
                                        mail.Body = "Email: " + comp.FirstOrDefault().Email;
                                    mail.Body +=
                                    "<br/><br/>Dear Customer,<br/><br/>I’d like to thank you for being an Active Customer and for" +
                                    " your constant support to ThinkDesign’s R&D activities. For this reason, I’m about to offer " +
                                    "you a very interesting opportunity for both our Companies.<br/><br/>According to our records, " +
                                    "some of your licenses (please, see the list below) are not under a maintenance contract.<br/>" +
                                    "I would like to make sure that all the licenses currently in use at your company are the state " +
                                    "of art of our technology, that they can be moved to other PCs and that, ultimately, they can " +
                                    "benefit from everything that comes from having an active maintenance contract.<br/>" +
                                    "Therefore, I’m offering you the possibility to convert for free all your old out-of-maintenance" +
                                    " Permanent Licenses (PL) into <b>super new</b> and <b>up-to-date</b> Annual Subscription" +
                                    " Fee licenses (ASF), which will be under maintenance for 12 months. All this – it is worth repeating" +
                                    " - <b>at no cost</b>, completely <b>free of charge</b> and without any commitment on your part " +
                                    "considering possible future renewals of such licenses.<br/><br/>Further information are available" +
                                    " at this link: http://bit.ly/InitiativeZeroBis. Alternatively, please, contact your reseller, " +
                                    "whom I’ve copied in this email.<br/><br/>Don’t miss this opportunity! Zero Bis Initiative will be" +
                                    " valid all summer long and it will expire on <b>September 30, 2017</b>.<br/><br/>" +
                                    "To make it easier for you to join, at the bottom of this email you will find the text of a document " +
                                    "that has a double value. It represents a description of the implementation procedures and, " +
                                    "at the same time, the application form.<br/><br/>Thank you and have a good day.<br/>Best regards," +
                                    "<br/><br/>Massimo Signani<br/>CEO DPT/think3<br/><br/>www.dptcorporate.com" +
                                    "<br/>www.think3.eu<br/><br/><hr><br/><br/><u>THE FOLLOWING TEXT HAS TO BE COPIED ON THE " +
                                    "COMPANY’S HEADED PAPER, WITH THE STAMP AND THE SIGNATURE OF THE CEO OR OF THE PERSON IN " +
                                    "CHARGE OF THE DEPARTMENT.</u><br/><br/><br/><i>Location, date<br/><br/><br/>" +
                                    "Dear DPT,<br/><br/>we hereby confirm our enrollment in the Initiative Zero Bis.<br/><br/>" +
                                    "We are aware that this Initiative:<br/><br/>a) entails the conversion of all our " +
                                    "out-of-maintenance permanent licenses (PL) into ASF (Annual Subscription Fee) " +
                                    "licenses, which we could use on new PCs and/or in up-to-date versions. This conversion" +
                                    " will take place by exporting our old licenses – if they are linked to workstations –" +
                                    " and/or returning the USB/parallel dongles to DPT offices in case of licenses on hardware" +
                                    " keys.<br/><br/>b) is completely free of charge and it will allow us to use the new licenses" +
                                    " until 30/06/2018; after this date, they will stop working unless we renew them.</i><br/>" +
                                    "<br/><table border=1><tr><td align='center'>License ID</td><td align='center'>Product</td>" +
                                    "<td align='center'>Machine ID</td><td align='center'>License type</td><td align='center'>Quantity</td></tr>";
                                    break;
                            }
                            mail.BodyEncoding = System.Text.Encoding.UTF8;
                            foreach (var mac in machines.Keys)
                            {
                                var vl = "";
                                machines.TryGetValue(mac, out vl);
                                var values = vl.Split(',');
                                var licids = "";
                                var prods = "";
                                var ltype = "";
                                var qty = 0;
                                var i = 0;
                                foreach (var v in values)
                                {
                                    if (i == 0)
                                    {
                                        licids += v.Split('-')[0];
                                        prods += v.Split('-')[1];
                                        ltype += v.Split('-')[2];
                                        i++;
                                        if (ltype.Contains("f"))
                                            qty += Int32.Parse(v.Split('-')[3]);
                                    }
                                    else
                                    {
                                        licids += " / " + v.Split('-')[0];
                                        prods += " / " + v.Split('-')[1];
                                        if (ltype.Contains("f"))
                                            qty += Int32.Parse(v.Split('-')[3]);
                                    }
                                }
                                mail.Body += "<tr><td>" + licids + "</td><td>" + prods + "</td><td align='center'>" + mac +
                                    "</td><td align='center'>" + ltype + "</td><td align='center'>" + (ltype.Contains("f") ? qty.ToString() : "") + "</td></tr>";
                            }
                            switch (comp.FirstOrDefault().Language.ToLower())
                            {
                                case "italian":
                                    mail.Body += "</table><br/>Nota bene: il costo delle licenze in modalità floating corrisponde al 15% in più " +
                                    "rispetto ai prezzi segnalati sopra.<br/><br/>c) Ci dichiariamo altresì consapevoli che, alla scadenza del " +
                                    "periodo di 12 mesi ad uso gratuito, potremo decidere liberamente se e quante licenze " +
                                    "rinnovare – sempre in modalità ASF -, facendo riferimento ai prezzi speciali riportati " +
                                    "nella tabella sottostante.<br/>Il costo del rinnovo per il secondo anno (e quelli " +
                                    "successivi) corrisponde alla metà del prezzo di listino previsto per le licenze ASF, " +
                                    "come illustrato nella tabella sotto per alcuni dei principali prodotti DPT.<br/><br/>" +
                                    "<table border=1><tr><td>Prodotto</td><td>Prezzo rinnovo</td></tr>" +
                                    "<tr><td>TDEngineering</td><td align='center'>1.300€</td></tr><tr><td>TDProfessional</td><td align='center'>2.400€</td></tr>" +
                                    "<tr><td>TDTooling</td><td align='center'>1.900€</td></tr><tr><td>TTeamPDM</td><td align='center'>320€</td></tr></table><br/><br/>" +
                                    "d) Una volta firmato questo documento e dopo aver esportato le vecchie licenze permanenti e/o" +
                                    " restituito le chiavi USB/parallele, DPT provvederà a fornirci le licenze annuali che ci " +
                                    "spettano.<br/><br/>Cordiali Saluti,<br/><br/><br/>Firma del CEO &<br/>Timbro dell’azienda</i>";
                                    break;
                                case "japanese":
                                    mail.Body += "</table><br/><i>c.	when the free use period will be over (30/06/2018 ) " +
                                        "we can freely decide whether and how many licenses we want to renew – always as ASF -, " +
                                        "by referring to the special prices shown in the table below, which amount to 50% of the" +
                                        " standard ASF prices established in DPT Price List. Such prices will be valid for second" +
                                        " year of subscription and all following ones.</i><br/><br/><table border=1><tr><td><b>Product</b></td><td><b>Special Subscription Price</b></td></tr>" +
                                        "<tr><td>TDProfessional</td><td align='center'>267000¥</td></tr><tr><td>TDStyling</td>" +
                                        "<td align='center'>232000¥</td></tr><tr><td>TDTooling</td><td align='center'>217000¥</td></tr>" +
                                        "<tr><td>TDEngineering</td><td align='center'>132000¥</td></tr></table><br/><i>Please, note that the price for floating licenses increases by 15%." +
                                        "<br/><br/>d.	DPT Sarl will provide us with the ASF (Annual Subscription Fee) licenses  once we send back this signed document and we have " +
                                        "exported old licenses or returned old dongles.<br/><br/>Best regards,<br/><br/><br/>" +
                                        "CEO Signature &<br/>Company Stamp</i>";
                                    break;
                                default:
                                    mail.Body += "</table><br/>Please, note that the price for floating licenses increases by 15%." +
                                    "<br/><br/>c) We also acknowledge that, after the free 12-months period," +
                                    " we can freely decide whether and how many licenses we want to renew – always as " +
                                    "ASF -, by referring to the special prices shown in the table below.<br/>The renewal" +
                                    " price for the 2nd year (and the years after) corresponds to half of the ASF price " +
                                    "established in DPT Price List, as shown in the table here below for some of the main" +
                                    " DPT products.<br/><br/><table border=1><tr><td>Product</td><td>Renewal price</td></tr>" +
                                    "<tr><td>TDEngineering</td><td align='center'>1.300€</td></tr><tr><td>TDProfessional</td>" +
                                    "<td align='center'>2.400€</td></tr><tr><td>TDTooling</td><td align='center'>1.900€</td></tr>" +
                                    "<tr><td>TTeamPDM</td><td align='center'>320€</td></tr></table><br/><br/>d) Once we send" +
                                    " back this signed document and the .tbu files/dongles, DPT will provide us with " +
                                    "the ASF (Annual Subscription Fee) licenses in the version we request.<br/><br/>Best regards," +
                                    "<br/><br/><br/>CEO Signature & <br/>Company Stamp</i>";
                                    break;
                            }
                            mail.IsBodyHtml = true;
                            MailHelper.SendMail(mail);
                        }
                    }
                }
                catch (Exception e)
                {
                    LogHelper.WriteLog("InternosController (Mail): accountnumber - " + line + " -- " + e.Message + "-" + e.InnerException);
                    errormsg += line + " (" + e.Message + "-" + e.InnerException + "); </br>";
                }
            }

            ViewBag.Message = errormsg;
            return View();
        }

    }
}
