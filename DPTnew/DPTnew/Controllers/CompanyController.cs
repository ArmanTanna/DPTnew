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

        [NonAction]
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Session["CurrentCulture"] = LocalizationHelper.SetLocalization(Session["CurrentCulture"]);

            base.OnActionExecuting(filterContext);
        }

    }

}