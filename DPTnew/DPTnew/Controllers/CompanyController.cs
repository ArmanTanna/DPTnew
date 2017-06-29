using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPTnew.Models;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using WebMatrix.WebData;
using Newtonsoft.Json;
using System.ServiceModel.Description;
using SafenetIntegration;
using DPTnew.Helper;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Net;
using Microsoft.JScript;
using System.IO;
using System.Net.Mail;
using System.Diagnostics;
using System.Net.Sockets;

namespace DPTnew.Controllers
{

    [Authorize(Roles = "VarExp,VarMed,Var,Internal,Admin")]
    public class CompanyController : BaseController
    {
        //
        // GET: /All Companies/

        public ActionResult Index(int pageSize = 10)
        {
            //LocalizationHelper.SetLocalization(Session["CurrentCulture"]);
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(_db.Companies.Select(x => x.AccountKind).Distinct().ToList()).ToString(Formatting.None));
            ViewBag.AccountKind = System.Convert.ToBase64String(plainTextBytes);
            var plainTextBytes2 = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(_db.Companies.Select(x => x.AccountStatus).Distinct().ToList()).ToString(Formatting.None));
            ViewBag.AccountStatus = System.Convert.ToBase64String(plainTextBytes2);
            var plainTextBytes3 = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(_db.Companies.Select(x => x.SalesRep).Distinct().ToList()).ToString(Formatting.None));
            ViewBag.SalesReps = System.Convert.ToBase64String(plainTextBytes3);
            var plainTextBytes4 = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(_db.Companies.Select(x => x.LastExp).Distinct().ToList()).ToString(Formatting.None));
            ViewBag.LastExp = System.Convert.ToBase64String(plainTextBytes4);
            ViewBag.IsButtonRole = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp")
                || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarMed") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal");
            ViewBag.UserRole = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal")
                || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp");
            ViewBag.AdminRole = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin");
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult SyncDB()
        {
            using (var db = new DptContext())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [dbo].[DPT_SafenetCompanies]");
                var query =
                from cmp in db.Companies
                where cmp.AccountStatus == "03 - Active Customer" || cmp.AccountStatus == "03 - Premium Customer" ||
                    cmp.AccountStatus == "06 - Partner" || cmp.AccountStatus == "04 - Not Active Customer" ||
                    cmp.AccountStatus == "04 - Not Active Customer OM" || cmp.AccountStatus == "01 - Prospect"
                select cmp;
                if (query.Count() > 0)
                {
                    foreach (var cmp in query.ToList())
                    {
                        SafenetComapny sfnc = new SafenetComapny();
                        sfnc.AccountNumber = cmp.AccountNumber;
                        sfnc.AccountName = cmp.AccountName;
                        sfnc.Email = cmp.Email;
                        sfnc.FirstName = "DPT";
                        sfnc.LastName = "User";
                        sfnc.Description = "Company";

                        var pc =
                        from pcl in db.Peoples
                        where pcl.AccountNumber == sfnc.AccountNumber && pcl.PrimaryContact == "yes"
                        select pcl;

                        if (pc.Any())
                            sfnc.Language = pc.FirstOrDefault().Language;
                        else
                            sfnc.Language = "English";

                        db.SafenetCompanies.Add(sfnc);
                    }
                    db.SaveChanges();
                }
            }

            string errormsg = SaveCompaniesToSafenetDB();
            ViewBag.Message = errormsg;
            using (var db = new DptContext())
            {
                db.Database.ExecuteSqlCommand("TRUNCATE TABLE [dbo].[DPT_SafenetCompanies]");
            }
            return View();
        }

        private string SaveCompaniesToSafenetDB()
        {
            SentinelEMSWrapper sew;
            var cc = new ClientCredentials();
            cc.UserName.UserName = System.Configuration.ConfigurationManager.AppSettings["safenetusername"];
            cc.UserName.Password = System.Configuration.ConfigurationManager.AppSettings["safenetpassword"];
            var safenetUri = new Uri(System.Configuration.ConfigurationManager.AppSettings["safeneturi"]);
            string errormsg = "DB Sync failed for: ";
            using (var db = new DptContext())
            {
                foreach (SafenetComapny company in db.SafenetCompanies.ToList())
                {
                    try
                    {
                        //if (string.IsNullOrEmpty(company.Email))
                        //    continue;

                        sew = new SentinelEMSWrapper(safenetUri, cc);
                        sew.Authentication();
                        var data = new JObject();
                        data["Email"] = company.Email;
                        data["FirstName"] = company.FirstName;
                        data["LastName"] = company.LastName;
                        data["Locale"] = company.Language;
                        data["CompanyName"] = company.AccountName;
                        data["CrmId"] = company.AccountNumber;
                        data["ActualBatchCode"] = company.ActualBatchCode;
                        data["UpdateBatchCode"] = company.UpdateBatchCode;
                        data["Description"] = company.Description;
                        if (sew.CheckExistCustomer(data))
                            sew.UpdateCustomer(data);
                        else
                            sew.CreateCustomer(data);
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteLog("CompanyController (SaveCompaniesToSafenetDB): " + e.Message);
                        errormsg += company.AccountNumber + "-" + company.AccountName + " (" + e.Message + "); </br>";
                    }
                }
            }
            return errormsg;
        }

        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UpdateProductList()
        {
            string uri = Url.Action("UpdateDptProducts", "Safenet", new { httproute = "" }, "http");
            HttpResponseMessage response = await SendJsonAsync(uri);
            ViewBag.Message = response.ReasonPhrase;
            return View();
        }

        private async Task<HttpResponseMessage> SendJsonAsync(string uri)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage responseMessage = new HttpResponseMessage();
                try
                {
                    responseMessage = await httpClient.GetAsync(uri);
                }
                catch (Exception ex)
                {
                    responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                }
                return responseMessage;
            }
        }

        [HttpPost]
        public JsonResult Search()
        {
            var sps = Request.GetSearchParams();
            var items = GetCompanies();
            foreach (var sp in sps)
            {
                items = _db.Search<CompanyView>(sp, items);
            }
            return Json(_db.ConvertToSearchResult<CompanyView>(sps.FirstOrDefault(), items), JsonRequestBehavior.AllowGet);
            //return Json(_db.Search<CompanyView>(Request.GetSearchParams().FirstOrDefault(), GetCompanies()), JsonRequestBehavior.AllowGet);
        }

        // GET: /Licenses/
        public ActionResult Licenses(string id)
        {
            var company = GetCompanies().FirstOrDefault(u => u.AccountNumber == id);

            if (company == null)
                throw new HttpException(404, "Company not found!");

            return View(company.Licenses);
        }

        [Authorize(Roles = "Admin,Internal,VarExp,VarMed")]
        [HttpPost]
        public ActionResult SingleCompanyRow(CompanyView cmpSingleRow)
        {
            List<CompanyView> rows = new List<CompanyView>();
            rows.Add(cmpSingleRow);
            using (var db = new DptContext())
            {
                byte[] plainTextBytes;
                if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp"))
                {
                    var userName = Membership.GetUser().UserName;
                    var user = db.Contacts.Where(u => u.Email == userName).ToList().FirstOrDefault();
                    var company = db.Companies.Where(u => u.AccountNumber == user.AccountNumber).ToList().FirstOrDefault();
                    plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(db.SalesR.Where(u => u.Invoicer == company.AccountName).Select(u => u.SalesRep).ToList()).ToString(Formatting.None));
                    ViewBag.SalesReps = System.Convert.ToBase64String(plainTextBytes);
                }
                else
                {
                    plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(db.Companies.Select(x => x.SalesRep).Distinct().ToList()).ToString(Formatting.None));
                    ViewBag.SalesReps = System.Convert.ToBase64String(plainTextBytes);
                }
            }
            ViewBag.UserRole = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal")
                || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp");
            return View(rows);
        }

        [Authorize(Roles = "Admin,Internal,VarExp,VarMed")]
        [HttpPost]
        public JsonResult Modify(CompanyView cmpSingleRow)
        {
            Regex email = new Regex(@"^(([0-9a-zA-Z_!#%'=`&\|~\-\*\+\?\^\$\{\}]+)(\.[0-9a-zA-Z0-9a-zA-Z_!#%'=`&\|~\-\*\+\?\^\$\{\}]+)*)@(([0-9a-zA-Z_!#%'=`&\|~\-\*\+\?\^\$\{\}]+)(\.[0-9a-zA-Z_!#%'=`&\|~\-\*\+\?\^\$\{\}]+)+)$");
            if (string.IsNullOrEmpty(cmpSingleRow.AccountName))
                return Json("Invalid AccountName", JsonRequestBehavior.AllowGet);
            if (string.IsNullOrEmpty(cmpSingleRow.Email) || (!email.IsMatch(cmpSingleRow.Email)))
                return Json("Invalid mail", JsonRequestBehavior.AllowGet);

            using (var db = new DptContext())
            {
                cmpSingleRow.AccountNameK = GlobalObject.unescape(cmpSingleRow.AccountNameK);
                cmpSingleRow.AddressK = GlobalObject.unescape(cmpSingleRow.AddressK);
                cmpSingleRow.CityK = GlobalObject.unescape(cmpSingleRow.CityK);
                cmpSingleRow.ProvinceK = GlobalObject.unescape(cmpSingleRow.ProvinceK);
                if (!string.IsNullOrEmpty(cmpSingleRow.Segment))
                    cmpSingleRow.Segment = GlobalObject.unescape(cmpSingleRow.Segment);
                if (!string.IsNullOrEmpty(cmpSingleRow.Industry))
                    cmpSingleRow.Industry = GlobalObject.unescape(cmpSingleRow.Industry);
                if (!string.IsNullOrEmpty(cmpSingleRow.AccountNumber))
                {
                    try
                    {
                        var query =
                    from cmp in db.Companies
                    where cmp.AccountNumber == cmpSingleRow.AccountNumber
                    select cmp;

                        if (query.Count() > 0)
                        {
                            query.FirstOrDefault().AccountName = cmpSingleRow.AccountName.ToUpper();
                            query.FirstOrDefault().Address = cmpSingleRow.Address;
                            query.FirstOrDefault().ZIP = cmpSingleRow.ZIP;
                            query.FirstOrDefault().City = cmpSingleRow.City;
                            query.FirstOrDefault().Province = cmpSingleRow.Province;
                            query.FirstOrDefault().Email = cmpSingleRow.Email;
                            query.FirstOrDefault().AccountNameK = cmpSingleRow.AccountNameK;
                            query.FirstOrDefault().ProvinceK = cmpSingleRow.ProvinceK;
                            query.FirstOrDefault().AddressK = cmpSingleRow.AddressK;
                            query.FirstOrDefault().CityK = cmpSingleRow.CityK;
                            query.FirstOrDefault().Phone1 = cmpSingleRow.Phone1;
                            query.FirstOrDefault().Phone2 = cmpSingleRow.Phone2;

                            if (string.IsNullOrEmpty(cmpSingleRow.Segment))
                                query.FirstOrDefault().Segment = "0 To be defined";
                            else
                                query.FirstOrDefault().Segment = cmpSingleRow.Segment.Replace("&amp;", "&");

                            if (string.IsNullOrEmpty(cmpSingleRow.Industry))
                                query.FirstOrDefault().Industry = "0.00 To be defined";
                            else
                                query.FirstOrDefault().Industry = cmpSingleRow.Industry.Replace("&amp;", "&");

                            query.FirstOrDefault().Fax = cmpSingleRow.Fax;
                            query.FirstOrDefault().Website = cmpSingleRow.Website;
                            query.FirstOrDefault().Production = cmpSingleRow.Production;
                            query.FirstOrDefault().Language = cmpSingleRow.Language;
                            if (string.IsNullOrEmpty(cmpSingleRow.Language))
                                query.FirstOrDefault().Language = "english";
                            query.FirstOrDefault().SalesRep = cmpSingleRow.SalesRep;
                            db.SaveChanges();
                        }
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteLog("CompanyController (Modify): " + e.InnerException.InnerException.Message);
                        return Json(e.InnerException.InnerException.Message, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    try
                    {
                        var maxq = db.Companies.Max(u => u.AccountNumber);
                        cmpSingleRow.AccountNumber = maxq.Split('-')[0] + "-" + (System.Convert.ToInt64(maxq.Split('-')[1]) + 1).ToString("D7");
                        cmpSingleRow.AccountName = cmpSingleRow.AccountName.ToUpper();
                        cmpSingleRow.AccountKind = "customer";
                        if (string.IsNullOrEmpty(cmpSingleRow.Segment))
                            cmpSingleRow.Segment = "0 To be defined";
                        if (string.IsNullOrEmpty(cmpSingleRow.Industry))
                            cmpSingleRow.Industry = "0.00 To be defined";
                        if (string.IsNullOrEmpty(cmpSingleRow.Language))
                            cmpSingleRow.Language = "english";
                        db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[DPT_Companies] (AccountNumber, AccountName, AccountKind," +
                        "AccountStatus, Address, ZIP, City, Province, Country, Phone1, Phone2, Email, Fax, Website, Segment, Industry," +
                        "Production, SalesRep, Language, AccountNameK, ProvinceK, AddressK, CityK) VALUES ('" + cmpSingleRow.AccountNumber + "','" +
                        cmpSingleRow.AccountName.ToUpper() + "','customer','" + cmpSingleRow.AccountStatus + "','" + cmpSingleRow.Address + "','" +
                        cmpSingleRow.ZIP + "','" + cmpSingleRow.City + "','" + cmpSingleRow.Province + "','" + cmpSingleRow.Country +
                        "','" + cmpSingleRow.Phone1 + "','" + cmpSingleRow.Phone2 + "','" + cmpSingleRow.Email + "','" + cmpSingleRow.Fax +
                        "','" + cmpSingleRow.Website + "',N'" + cmpSingleRow.Segment + "',N'" + cmpSingleRow.Industry + "','" +
                        cmpSingleRow.Production + "','" + cmpSingleRow.SalesRep + "','" + cmpSingleRow.Language + "',N'" + cmpSingleRow.AccountNameK
                        + "',N'" + cmpSingleRow.ProvinceK + "',N'" + cmpSingleRow.AddressK + "',N'" + cmpSingleRow.CityK + "');");
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteLog("CompanyController (Modify): " + e.Message);
                        return Json(e.Message, JsonRequestBehavior.AllowGet);
                    }
                }

                if (cmpSingleRow.AccountStatus == "03 - Active Customer" || cmpSingleRow.AccountStatus == "03 - Premium Customer" ||
                    cmpSingleRow.AccountStatus == "06 - Partner" || cmpSingleRow.AccountStatus == "04 - Not Active Customer" ||
                    cmpSingleRow.AccountStatus == "04 - Not Active Customer OM" || cmpSingleRow.AccountStatus == "01 - Prospect")
                {
                    var q =
                     from cmp in db.SafenetCompanies
                     where cmp.AccountNumber == cmpSingleRow.AccountNumber
                     select cmp;

                    if (q.Count() > 0)
                    {
                        q.FirstOrDefault().AccountName = cmpSingleRow.AccountName;
                        q.FirstOrDefault().Email = cmpSingleRow.Email;
                        db.SaveChanges();
                    }
                    else
                    {
                        SafenetComapny sfnc = new SafenetComapny();
                        sfnc.AccountNumber = cmpSingleRow.AccountNumber;
                        sfnc.AccountName = cmpSingleRow.AccountName.ToUpper();
                        sfnc.Email = cmpSingleRow.Email;
                        sfnc.FirstName = "DPT";
                        sfnc.LastName = "User";
                        sfnc.Description = "Company";
                        sfnc.Language = cmpSingleRow.Language;

                        db.SafenetCompanies.Add(sfnc);
                        db.SaveChanges();
                    }
                    SaveCompaniesToSafenetDB();
                }
            }
            return Json("Saved AccountNumber: " + cmpSingleRow.AccountNumber, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllSegments()
        {
            using (var db = new DptContext())
            {
                var segments = db.Segind.Select(x => x.Segment).Distinct().ToList();
                return Json(segments, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetIndustries(string segmentName)
        {
            if (string.IsNullOrEmpty(segmentName))
                return Json("Parameter Missing!", JsonRequestBehavior.AllowGet);
            using (var db = new DptContext())
            {
                var industries = db.Segind.Where(u => u.Segment == segmentName).Select(x => x.Industry).Distinct().ToList();
                return Json(industries, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SendMail()
        {
            return View();
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
                    LogHelper.WriteLog("CompanyController (Mail): accountnumber - " + line + " -- " + e.Message + "-" + e.InnerException);
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
                            if (comp.FirstOrDefault().Language.ToLower() == "italian")
                            {
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
                                    "<td align='center'>Tipo Licenza</td><td align='center'>Quantitá</td></tr>";
                            }
                            else
                            {
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
                                " until 30/06/2018; after this date, they will stop working unless we renew them.<br/>" +
                                "<br/><table border=1><tr><td align='center'>License ID</td><td align='center'>Product</td>" +
                                "<td align='center'>Machine ID</td><td align='center'>License type</td><td align='center'>Quantity</td></tr>";
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
                    LogHelper.WriteLog("CompanyController (Mail): accountnumber - " + line + " -- " + e.Message + "-" + e.InnerException);
                    errormsg += line + " (" + e.Message + "-" + e.InnerException + "); </br>";
                }
            }

            ViewBag.Message = errormsg;
            return View();
        }

        [NonAction]
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Session["CurrentCulture"] = LocalizationHelper.SetLocalization(Session["CurrentCulture"]);

            base.OnActionExecuting(filterContext);
        }

    }

}