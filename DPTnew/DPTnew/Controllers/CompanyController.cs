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
                where cmp.AccountStatus == "03 - Active Customer" || cmp.AccountStatus == "06 - Partner" ||
                    cmp.AccountStatus == "04 - Not Active Customer" || cmp.AccountStatus == "04b - Not Active Customer OM"
                    || cmp.AccountStatus == "01 - Prospect"
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
                cmpSingleRow.Address = GlobalObject.unescape(cmpSingleRow.Address);
                cmpSingleRow.AddressK = GlobalObject.unescape(cmpSingleRow.AddressK);
                cmpSingleRow.City = GlobalObject.unescape(cmpSingleRow.City);
                cmpSingleRow.CityK = GlobalObject.unescape(cmpSingleRow.CityK);
                cmpSingleRow.Province = GlobalObject.unescape(cmpSingleRow.Province);
                cmpSingleRow.ProvinceK = GlobalObject.unescape(cmpSingleRow.ProvinceK);
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
                            query.FirstOrDefault().Segment = cmpSingleRow.Segment;
                            if (cmpSingleRow.Industry != "0.00 To be defined")
                                query.FirstOrDefault().Industry = cmpSingleRow.Industry;
                            query.FirstOrDefault().Fax = cmpSingleRow.Fax;
                            query.FirstOrDefault().Website = cmpSingleRow.Website;
                            query.FirstOrDefault().Production = cmpSingleRow.Production;
                            query.FirstOrDefault().Language = cmpSingleRow.Language;
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
                        db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[DPT_Companies] (AccountNumber, AccountName, AccountKind," +
                        "AccountStatus, Address, ZIP, City, Province, Country, Phone1, Phone2, Email, Fax, Website, Segment, Industry," +
                        "Production, SalesRep, Language, AccountNameK, ProvinceK, AddressK, CityK) VALUES ('" + cmpSingleRow.AccountNumber + "','" +
                        cmpSingleRow.AccountName.ToUpper() + "','customer','" + cmpSingleRow.AccountStatus + "',N'" + cmpSingleRow.Address + "','" +
                        cmpSingleRow.ZIP + "',N'" + cmpSingleRow.City + "','" + cmpSingleRow.Province + "','" + cmpSingleRow.Country +
                        "','" + cmpSingleRow.Phone1 + "','" + cmpSingleRow.Phone2 + "','" + cmpSingleRow.Email + "','" + cmpSingleRow.Fax +
                        "','" + cmpSingleRow.Website + "','" + cmpSingleRow.Segment + "','" + cmpSingleRow.Industry + "','" +
                        cmpSingleRow.Production + "','" + cmpSingleRow.SalesRep + "','" + cmpSingleRow.Language + "',N'" + cmpSingleRow.AccountNameK
                        + "',N'" + cmpSingleRow.ProvinceK + "',N'" + cmpSingleRow.AddressK + "',N'" + cmpSingleRow.CityK + "');");
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteLog("CompanyController (Modify): " + e.Message);
                        return Json(e.Message, JsonRequestBehavior.AllowGet);
                    }
                }

                if (cmpSingleRow.AccountStatus == "03 - Active Customer" || cmpSingleRow.AccountStatus == "06 - Partner" ||
                        cmpSingleRow.AccountStatus == "04 - Not Active Customer" || cmpSingleRow.AccountStatus == "04b - Not Active Customer OM"
                        || cmpSingleRow.AccountStatus == "01 - Prospect")
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

        [HttpPost]
        public ActionResult SendAutomaticMail(HttpPostedFileBase file)
        {
            using (StreamReader reader = new StreamReader(file.InputStream))
            {
                string line = "";
                var inClause = new string[] { "tdstyling", "tteampdm", "thinkprint", "tdmolding", "tdxchangereader",
                                "tdengineering", "teamdev", "tdtooling", "tdengineeringplus", "tdbase", "tdprofessional", "tddrafting" };
                while ((line = reader.ReadLine()) != null)
                {
                    using (var db = new DptContext())
                    {
                        try
                        {
                            var lics = from lic in db.Licenses
                                       where lic.AccountNumber == line.Trim() && lic.LicenseID.StartsWith("L") &&
                                       inClause.Contains(lic.ProductName.ToLower()) && lic.MaintEndDate < DateTime.Now
                                       select lic;
                            Dictionary<string, string> machines = new Dictionary<string, string>();
                            if (lics.Count() > 0)
                            {
                                foreach (var lic in lics)
                                {
                                    if (machines.ContainsKey(lic.MachineID))
                                    {
                                        string val = "";
                                        machines.TryGetValue(lic.MachineID, out val);
                                        val += "," + lic.LicenseID + "-" + lic.ProductName;
                                        machines[lic.MachineID] = val;
                                    }
                                    else
                                        machines.Add(lic.MachineID, lic.LicenseID + "-" + lic.ProductName);
                                }
                            }
                            MailMessage mail = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["hostusername"], "dpt@dptcorporate.com");
                            mail.Subject = "Iniziativa Zero";
                            mail.Body = "Gentile Cliente, <br/><br/>Le ricordiamo che l’<b>Iniziativa Zero</b>, che " +
                                "Le permetterà di ottenere <b>gratuitamente</b> delle licenze <b>think3 aggiornate</b>" +
                                " all’ultima versione, sta per scadere.<br/>Per scoprire come funziona e tutti i vantaggi" +
                                " che comporta, La invitiamo a cliccare sul seguente link: http://bit.ly/InitiativeZero." +
                                "<br/><br/>Nel caso decidesse di sottoscrivere l’Iniziativa, in calce troverà il testo" +
                                " del modulo di adesione, che dovrà essere riportato su carta intestata della Sua" +
                                " Azienda, con firma e timbro del CEO o del Responsabile del Suo Dipartimento." +
                                "<br/><br/>La ringraziamo e Le auguriamo una buona giornata!<br/>" +
                                "Cordiali saluti,<br/><br/>Team DPT/think3<br/>Viale Angelo Masini, n. 12<br/>c/o " +
                                "Regus - 40126 Bologna<br/>Tel. +39 051 092 3545<br/><br/>www.dptcorporate.com<br/>" +
                                "www.think3.eu <br/><br/><hr><i><br/>Luogo, data<br/><br/><br/>" +
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
                                "<td align='center'>Prodotto</td><td align='center'>ID Macchina</td></tr>";
                            foreach (var mac in machines.Keys)
                            {
                                var vl = "";
                                machines.TryGetValue(mac, out vl);
                                var values = vl.Split(',');
                                var licids = "";
                                var prods = "";
                                var i = 0;
                                foreach (var v in values)
                                {
                                    if (i == 0)
                                    {
                                        licids += v.Split('-')[0];
                                        prods += v.Split('-')[1];
                                        i++;
                                    }
                                    else
                                    {
                                        licids += " / " + v.Split('-')[0];
                                        prods += " / " + v.Split('-')[1];
                                    }
                                }
                                mail.Body += "<tr><td>" + licids + "</td><td>" + prods + "</td><td align='center'>" + mac + "</td></tr>";
                            }
                            mail.Body += "</table><br/><br/>Ci dichiariamo altresì consapevoli che, alla scadenza del " +
                                "periodo di 12 mesi ad uso gratuito, potremo decidere liberamente se e quante licenze " +
                                "rinnovare – sempre in modalità ASF -, facendo riferimento ai prezzi speciali riportati " +
                                "nella tabella sottostante.<br/>Il costo del rinnovo per il secondo anno (e quelli " +
                                "successivi) corrisponde alla metà del prezzo di listino previsto per le licenze ASF.<br/><br/>" +
                                "<table border=1><tr><td>Prodotto</td><td>Prezzo rinnovo</td></tr>" +
                            "<tr><td>TDEngineering</td><td align='center'>1.300€</td></tr><tr><td>TDProfessional</td><td align='center'>2.400€</td></tr>" +
                            "<tr><td>TDTooling</td><td align='center'>1.900€</td></tr><tr><td>TTeamPDM</td><td align='center'>320€</td></tr></table><br/><br/>" +
                            "Una volta firmato questo documento e dopo aver esportato le vecchie licenze permanenti e/o" +
                            " restituito le chiavi USB/parallele, DPT provvederà a fornirci le licenze annuali che ci " +
                            "spettano.<br/><br/>Cordiali Saluti,<br/><br/><br/>Firma del CEO &<br/>Timbro dell’azienda</i>";
                            mail.IsBodyHtml = true;
                            MailHelper.SendMail(mail);
                        }
                        catch (Exception e)
                        {
                            LogHelper.WriteLog("CompanyController (Mail): accountnumber - " + line + " -- " + e.Message + "-" + e.InnerException);
                        }
                    }
                }
            }

            ViewBag.ok1 = "The mail was succesfully sent!";
            return View("Success");
        }

        [NonAction]
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Session["CurrentCulture"] = LocalizationHelper.SetLocalization(Session["CurrentCulture"]);

            base.OnActionExecuting(filterContext);
        }

    }

}