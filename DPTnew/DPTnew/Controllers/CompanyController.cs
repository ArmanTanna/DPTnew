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
                where cmp.AccountStatus.Contains("03") || cmp.AccountStatus.Contains("06")
                    || cmp.AccountStatus.Contains("04") || cmp.AccountStatus.Contains("01")
                    || cmp.AccountStatus.Contains("05")
                select cmp;
                if (query.Count() > 0)
                {
                    foreach (var cmp in query.ToList())
                    {
                        SafenetCompany sfnc = new SafenetCompany();
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
            string errormsg = "Safenet Sync failed for: ";
            using (var db = new DptContext())
            {
                foreach (SafenetCompany company in db.SafenetCompanies.ToList())
                {
                    try
                    {
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

        //[Authorize(Roles = "Admin")]
        //public ActionResult UpdateProductList()
        //{
        //    using (var db = new DptContext())
        //    {
        //        var pList = db.SafenetProducts.ToList();
        //        string root = HttpContext.Server.MapPath("~/App_Data/");
        //        string file = Path.Combine(root, "ProductList.txt");
        //        try
        //        {
        //            using (StreamWriter sw = new StreamWriter(file))
        //            {
        //                for (int i = 0; i < pList.Count; i++)
        //                    sw.WriteLine(pList[i].PRDName);
        //            }
        //        }
        //        catch (System.Exception e)
        //        {
        //            //throw new System.Exception(file, e);
        //            ViewBag.Message = e.Message;
        //        }
        //    }
        //    ViewBag.Message = "ok";
        //    return View();
        //}
        public async Task<ActionResult> UpdateProductList()
        {
            string uri = Url.Action("UpdateDptProducts", "Safenet", new { httproute = "" }, "https");
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
            var safmsg = "";
            var retmsg = "Saved AccountNumber: ";
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
                            query.FirstOrDefault().Country = cmpSingleRow.Country;
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
                            retmsg += cmpSingleRow.AccountNumber;
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
                        retmsg += cmpSingleRow.AccountNumber;
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteLog("CompanyController (Modify): " + e.Message);
                        return Json(e.Message, JsonRequestBehavior.AllowGet);
                    }
                }

                if (cmpSingleRow.AccountStatus.Contains("03") || cmpSingleRow.AccountStatus.Contains("06")
                    || cmpSingleRow.AccountStatus.Contains("04") || cmpSingleRow.AccountStatus.Contains("01")
                    || cmpSingleRow.AccountStatus.Contains("05"))
                {
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE [dbo].[DPT_SafenetCompanies]");
                    SafenetCompany sfnc = new SafenetCompany();
                    sfnc.AccountNumber = cmpSingleRow.AccountNumber;
                    sfnc.AccountName = cmpSingleRow.AccountName.ToUpper();
                    sfnc.Email = cmpSingleRow.Email;
                    sfnc.FirstName = "DPT";
                    sfnc.LastName = "User";
                    sfnc.Description = "Company";
                    sfnc.Language = cmpSingleRow.Language;

                    db.SafenetCompanies.Add(sfnc);
                    db.SaveChanges();

                    safmsg = SaveCompaniesToSafenetDB();
                    db.Database.ExecuteSqlCommand("TRUNCATE TABLE [dbo].[DPT_SafenetCompanies]");
                    if (safmsg.Any(c => char.IsDigit(c)))
                        retmsg += "; " + safmsg;
                }
            }
            return Json(retmsg, JsonRequestBehavior.AllowGet);
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
        public JsonResult GetCompanyInfo(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
                return Json("Wrong AccountNumber", JsonRequestBehavior.AllowGet);
            using (var db = new DptContext())
            {
                var exp = DateTime.Now.AddDays(30);
                var query = from lic in db.Licenses
                            where lic.AccountNumber == accountNumber && (lic.ArticleDetail.Contains("pl") || lic.ArticleDetail.Contains("plasasp"))
                            && lic.MaintEndDate > DateTime.Now && lic.MaintEndDate < exp
                            select lic;
                if (query.Count() > 0)
                    return Json("true", JsonRequestBehavior.AllowGet);
                else
                    return Json("false", JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize(Roles = "Admin,Internal")]
        public ActionResult FarewellMail(string accountNumber)
        {
            if (string.IsNullOrEmpty(accountNumber))
            {
                ViewBag.ok1 = "Something went wrong. Cannot send the mail!";
                return View("Success");
            }
            var email = "";
            using (var db = new DptContext())
            {
                var exp = DateTime.Now.AddDays(30);
                var query = from lic in db.Licenses
                            where lic.AccountNumber == accountNumber && (lic.ArticleDetail.Contains("pl") || lic.ArticleDetail.Contains("plasasp"))
                            && lic.MaintEndDate > DateTime.Now && lic.MaintEndDate < exp
                            select lic;
                if (query.Count() > 0)
                {
                    MailMessage mail = null;
                    var lang = "";
                    foreach (LicenseView lic in query.ToList())
                    {
                        if (string.IsNullOrEmpty(email))
                        {
                            var cmp = db.Companies.Where(y => y.AccountNumber == accountNumber).FirstOrDefault();
                            email = cmp.Email;
                            lang = cmp.Language;
                            mail = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["hostusername"], email); ;
                            var salesRep = from salrep in _db.SalesR where salrep.SalesRep == cmp.SalesRep select salrep;
                            var sr = from comp in _db.Companies where comp.AccountNumber == salesRep.FirstOrDefault().AccountNumber select comp;

                            var varmail = sr.FirstOrDefault().Email;
                            mail.CC.Add(varmail);
                            mail.Bcc.Add("Orders@dptcorporate.com");
                            if (cmp.Language == "italian")
                            {
                                mail.Subject = "[DO NOT REPLY] Rinnovo manutenzione - " + cmp.AccountName.Trim();
                                mail.Body = "Gentile Cliente,<br/><br/>il <b>contratto di manutenzione</b> delle " +
                                    "seguenti licenze <b>scadrà nei prossimi 30 giorni</b> e non ci risulta ancora rinnovato.<br/><br/>" +
                                    "<table border=1><tr><td>ID Licenza</td><td>Prodotto</td><td>Quantità</td>" +
                                    "<td>Tipo</td><td>ID Macchina</td><td>Scadenza</td></tr>" +
                                    "<tr><td>" + lic.LicenseID + "</td><td>" + lic.ProductName + "." + lic.ArticleDetail +
                                    "</td><td>" + lic.Quantity + "</td><td>" + lic.LicenseType + "</td><td>" + lic.MachineID + "</td><td>" +
                                    ((DateTime)lic.MaintEndDate).ToString("yyyy-MM-dd") + "</td></tr>";
                            }
                            else
                            {
                                mail.Subject = "[DO NOT REPLY] Maintenance contract renewal - " + cmp.AccountName.Trim();
                                mail.Body = "Dear Customer,<br/><br/>the <b>maintenance contract</b> of the following " +
                                    "license(s) will <b>expire in the next 30 days</b>, and it seems you have not renewed it yet.<br/><br/>" +
                                    "<table border=1><tr><td>LicenseID</td><td>Item</td><td>Quantity</td><td>LicenseType</td>" +
                                    "<td>MachineID</td><td>MaintEndDate</td></tr>" +
                                    "<tr><td>" + lic.LicenseID + "</td><td>" + lic.ProductName + "." + lic.ArticleDetail +
                                    "</td><td>" + lic.Quantity + "</td><td>" + lic.LicenseType + "</td><td>" + lic.MachineID + "</td><td>" +
                                    ((DateTime)lic.MaintEndDate).ToString("yyyy-MM-dd") + "</td></tr>";
                            }
                        }
                        else
                            mail.Body += "<tr><td>" + lic.LicenseID + "</td><td>" + lic.ProductName + "." + lic.ArticleDetail +
                                    "</td><td>" + lic.Quantity + "</td><td>" + lic.LicenseType + "</td><td>" + lic.MachineID + "</td><td>" +
                                    ((DateTime)lic.MaintEndDate).ToString("yyyy-MM-dd") + "</td></tr>";
                    }
                    mail.Body += "</table><br/>";
                    if (lang == "italian")
                    {
                        mail.Body += "Se si tratta solo di un disguido nella trasmissione dell’ordine di rinnovo, " +
                            "chiediamo scusa per questo che potrebbe sembrare un sollecito.<br/>In tal caso, vi invitiamo ad ignorare " +
                            "questo messaggio.<br/><br/>Se invece aveste deciso di non rinnovare il contratto di " +
                            "manutenzione, ce ne dispiace. In questo caso saremmo interessati a conoscerne le ragioni, " +
                            "in modo tale da poter eventualmente sopperire agli inconvenienti che vi hanno portato a " +
                            "tale decisione, peraltro alla vigilia del rilascio della v.2020.<br/><br/>Naturalmente tale " +
                            "decisione non vi impedirà di continuare ad utilizzare le vostre licenze permanenti.<br/>" +
                            "Tuttavia, non sarà più possibile usufruire di alcuni servizi quali " +
                            "l’aggiornamento, l’assistenza e l’esportazione delle licenze.<br/>" +
                            "Qualora la vostra azienda fosse un Cliente Premium, vi ricordiamo che anche le <b>licenze " +
                            "Premium</b>, mensili e gratuite, fino ad ora a vostra disposizione, non verranno rinnovate.<br/>" +
                            "<br/>È nostro compito peraltro fare presente che da tempo <b>la nostra policy aziendale non " +
                            "prevede più rientri in manutenzione in ritardo</b>, a meno che - contestualmente all’eventuale " +
                            "richiesta di manutenzione di una licenza - un cliente non accetti la trasformazione della " +
                            "stessa da permanente ad annuale, al costo del canone di manutenzione standard.<br/><br/>" +
                            "Restiamo in attesa di un vostro gentile riscontro e inviamo i nostri più cordiali saluti." +
                            "<br/><br/>DPT Accounting";
                    }
                    else
                    {
                        mail.Body += "If there has been a mistake in the transmission of the renewal order, we apologize " +
                            "for sending this reminder. So, in this case, please disregard this message.<br/><br/>" +
                            "If instead you decided not to renew your maintenance contract, we are very sorry for that.<br/>" +
                            "We would like to know the reasons why you made such a choice, in order to possibly overcome " +
                            "the problems that led you to this decision on the eve of the release of v.2020.<br/><br/>" +
                            "Of course, your decision will not prevent you from continuing to use your permanent license(s).<br/>" +
                            "However, it will no longer be possible to update and export your license(s) " +
                            "and receive assistance.<br/>" +
                            "We would also like to remind you that, if your company is a Premium Customer, the - monthly " +
                            "and free - <b>Premium licenses</b> available to you until now, will not be renewed.<br/>" +
                            "<br/>It is our duty to point out that <b>our company policy does not accept delayed maintenance " +
                            "renewals</b> any more, unless the customer accepts the conversion of his/her permanent licenses " +
                            "into annual subscriptions at the cost of the standard maintenance fee.<br/><br/>" +
                            "Looking forward to hearing from you.<br/>Best regards,<br/><br/>DPT Accounting";
                    }
                    mail.IsBodyHtml = true;
                    try
                    {
                        MailHelper.SendMail(mail);
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteLog("CompanyController (FarewellMail): accountnumber - " + accountNumber + " -- " + e.Message + "-" + e.InnerException);
                    }
                }
            }

            return Json("Mail sent: " + email, JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Session["CurrentCulture"] = LocalizationHelper.SetLocalization(Session["CurrentCulture"]);

            base.OnActionExecuting(filterContext);
        }

    }

}