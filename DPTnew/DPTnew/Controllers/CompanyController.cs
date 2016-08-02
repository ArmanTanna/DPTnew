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

namespace DPTnew.Controllers
{

    [Authorize(Roles = "VarExp,Admin,Var,Internal")]
    public class CompanyController : BaseController
    {
        //
        // GET: /All Companies/

        public ActionResult Index(int pageSize = 10)
        {
            LocalizationHelper.SetLocalization(Session["CurrentCulture"]);
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(_db.Companies.Select(x => x.SalesRep).Distinct().ToList()).ToString(Formatting.None));
            ViewBag.SalesReps = System.Convert.ToBase64String(plainTextBytes);
            ViewBag.IsAdmin = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin");
            ViewBag.IsVarExp = Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp");
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

            foreach (SafenetComapny company in _db.SafenetCompanies.ToList())
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
                    errormsg += company.AccountNumber + "-" + company.AccountName + " (" + e.Message + "); ";
                }
            }
            return errormsg;
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

        [Authorize(Roles = "Admin,VarExp")]
        [HttpPost]
        public ActionResult SingleCompanyRow(CompanyView cmpSingleRow)
        {
            List<CompanyView> rows = new List<CompanyView>();
            rows.Add(cmpSingleRow);
            return View(rows);
        }

        [Authorize(Roles = "Admin,VarExp")]
        [HttpPost]
        public JsonResult Modify(CompanyView cmpSingleRow)
        {
            using (var db = new DptContext())
            {
                try
                {
                    var query =
                from cmp in db.Companies
                where cmp.AccountNumber == cmpSingleRow.AccountNumber
                select cmp;

                    if (query.Count() > 0)
                    {
                        query.FirstOrDefault().AccountName = cmpSingleRow.AccountName;
                        query.FirstOrDefault().Address = cmpSingleRow.Address;
                        query.FirstOrDefault().ZIP = cmpSingleRow.ZIP;
                        query.FirstOrDefault().City = cmpSingleRow.City;
                        query.FirstOrDefault().Province = cmpSingleRow.Province;
                        query.FirstOrDefault().Email = cmpSingleRow.Email;
                        query.FirstOrDefault().AccountNameK = cmpSingleRow.AccountNameK;
                        query.FirstOrDefault().AddressK = cmpSingleRow.AddressK;
                        query.FirstOrDefault().CityK = cmpSingleRow.CityK;
                        query.FirstOrDefault().ProvinceK = cmpSingleRow.ProvinceK;
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    return Json(e.InnerException.InnerException.Message, JsonRequestBehavior.AllowGet);
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
                        sfnc.AccountName = cmpSingleRow.AccountName;
                        sfnc.Email = cmpSingleRow.Email;
                        sfnc.FirstName = "DPT";
                        sfnc.LastName = "User";
                        sfnc.Description = "Company";

                        var pc =
                        from pcl in db.Peoples
                        where pcl.AccountNumber == cmpSingleRow.AccountNumber && pcl.PrimaryContact == "yes"
                        select pcl;

                        if (pc.Count() > 0)
                            sfnc.Language = pc.FirstOrDefault().Language;
                        else
                            sfnc.Language = "English";

                        db.SafenetCompanies.Add(sfnc);
                        db.SaveChanges();
                    }
                    SaveCompaniesToSafenetDB();
                }
            }

            return Json("Saved!", JsonRequestBehavior.AllowGet);
        }

    }

}