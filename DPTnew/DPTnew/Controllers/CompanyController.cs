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

namespace DPTnew.Controllers
{

    [Authorize(Roles = "VarExp,Admin,Var,Internal")]
    public class CompanyController : BaseController
    {
        //
        // GET: /All Companies/

        public ActionResult Index(int pageSize = 10)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(_db.Companies.Select(x => x.SalesRep).Distinct().ToList()).ToString(Formatting.None));
            ViewBag.SalesReps = System.Convert.ToBase64String(plainTextBytes);
            ViewBag.IsAdmin = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin");
            ViewBag.IsVarExp = Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp");
            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult SyncDB()
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
            ViewBag.Message = errormsg;
            return View();
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

            }

            return Json(cmpSingleRow, JsonRequestBehavior.AllowGet);
        }

    }

}