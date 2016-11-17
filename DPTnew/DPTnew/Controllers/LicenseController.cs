using DPTnew.Helper;
using DPTnew.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace DPTnew.Controllers
{
    [Authorize]
    public class LicenseController : BaseController
    {
        // GET: /All Companies/
        [Authorize(Roles = "Var,VarMed,VarExp,Internal,Admin")]
        public ActionResult Index(int pageSize = 10)
        {
            //LocalizationHelper.SetLocalization(Session["CurrentCulture"]);
            ViewBag.IsAdmin = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin");
            ViewBag.IsInternal = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal");
            ViewBag.IsVarExp = Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarMed");
            ViewBag.IsVar =  Roles.IsUserInRole(WebSecurity.CurrentUserName, "Var");
            return View();
        }

        [Authorize(Roles = "Var,VarMed,VarExp,Internal,Admin")]
        [HttpPost]
        public JsonResult Search()
        {
            var sps = Request.GetSearchParams();
            var items = GetLicenses();
            foreach (var sp in sps)
            {
                items = _db.Search<LicenseView>(sp, items);
            }
            return Json(_db.ConvertToSearchResult<LicenseView>(sps.FirstOrDefault(), items), JsonRequestBehavior.AllowGet);
        }

        public JsonResult StateByLicenceId(string LicenseId)
        {
            LicenseState licenseState = new LicenseState();
            using (var context = new DptContext())
            {
                var x = context.Licenses.Where(a => a.LicenseID == LicenseId).ToList().FirstOrDefault();
                licenseState = new LicenseState()
                            {
                                LicenseID = x.LicenseID,
                                Version = x.Version,
                                LicenseType = x.LicenseType,
                                MachineID = x.MachineID,
                                MaintEndDate = x.MaintEndDate,
                                Installed = x.Installed,
                                Exported = x.Exported,
                                Import = x.Import,
                                PwdCode = x.PwdCode
                            };
            }
            return Json(licenseState, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Internal,VarExp,VarMed")]
        [HttpPost]
        public ActionResult SingleLicenseRow(LicenseView licSingleRow)
        {
            List<LicenseView> rows = new List<LicenseView>();
            rows.Add(licSingleRow);
            return View(rows);
        }

        [Authorize(Roles = "Admin,Internal,VarExp,VarMed")]
        [HttpPost]
        public JsonResult Modify(LicenseView licSingleRow)
        {
            if (licSingleRow.MaxExport < 0)
                return Json("Cannot save negative value for MaxExport", JsonRequestBehavior.AllowGet);

            using (var db = new DptContext())
            {
                var query =
                    from lic in db.Licenses
                    where lic.LicenseID == licSingleRow.LicenseID
                    select lic;
                if (query.Count() > 0)
                {
                    query.FirstOrDefault().MaxExport = licSingleRow.MaxExport;
                    db.SaveChanges();
                }

            }

            return Json("Saved!", JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Internal,VarExp,VarMed")]
        [HttpPost]
        public ActionResult AddLicenseRow(LicenseView licSingleRow)
        {
            List<LicenseView> rows = new List<LicenseView>();
            rows.Add(licSingleRow);
            using (var db = new DptContext())
            {
                var userName = Membership.GetUser().UserName;
                var user = db.Contacts.Where(u => u.Email == userName).ToList().FirstOrDefault();
                var company = db.Companies.Where(u => u.AccountNumber == user.AccountNumber).ToList().FirstOrDefault();
                var salesRep = db.SalesR.Where(u => u.Invoicer == company.AccountName).Select(u => u.SalesRep).ToList();
                List<String> companyList = new List<string>();
                if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal"))
                    companyList = db.Companies.Select(u => u.AccountName + " \"" + u.AccountNumber + "\"").ToList();
                else
                {
                    if (salesRep.Count == 0)
                    {
                        var sR = db.SalesR.Where(u => u.AccountNumber == company.AccountNumber).Select(u => u.SalesRep).FirstOrDefault();
                        companyList.AddRange(db.Companies.Where(x => x.SalesRep == sR).OrderBy(k => k.AccountName).Select(u => u.AccountName + " \"" + u.AccountNumber + "\"").ToList());
                        companyList.Add(company.AccountName + " \"" + company.AccountNumber + "\"");
                    }
                    else
                        companyList.AddRange(db.Companies.Where(x => salesRep.Contains(x.SalesRep)).OrderBy(k => k.AccountName).Select(u => u.AccountName + " \"" + u.AccountNumber + "\"").ToList());
                }
                companyList.Sort();

                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(companyList).ToString(Formatting.None));
                ViewBag.Companies = System.Convert.ToBase64String(plainTextBytes);
            }
            ViewBag.IsAdmin = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin");
            ViewBag.IsInternal = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal");
            return View(rows);
        }

        [Authorize(Roles = "Admin,Internal,VarExp,VarMed")]
        [HttpPost]
        public JsonResult AddNew(LicenseView licSingleRow)
        {
            var version = Convert.ToInt64(licSingleRow.Version);
            if (!string.IsNullOrEmpty(licSingleRow.MachineID))
            {
                if (version < 2015 && licSingleRow.MachineID.Length != 8)
                    return Json("Wrong Machine ID! The length should be 8", JsonRequestBehavior.AllowGet);
                if (version > 2014 && licSingleRow.MachineID.Length != 21)
                    return Json("Wrong Machine ID! The length should be 21", JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(licSingleRow.AccountNumber))
                return Json("Wrong Account Number!", JsonRequestBehavior.AllowGet);

            using (var db = new DptContext())
            {
                licSingleRow.OriginalProduct = licSingleRow.ProductName;
                licSingleRow.Installed = 0;
                licSingleRow.Exported = 0;
                licSingleRow.Import = 1;
                licSingleRow.Vend_String = "vs001";
                licSingleRow.FlexType = 0;

                if (licSingleRow.LicenseType == "local" || (licSingleRow.LicenseType == "floating" && licSingleRow.Quantity < 1))
                    licSingleRow.Quantity = 1;

                if (licSingleRow.LicenseID != "NEW")
                {
                    if (licSingleRow.ArticleDetail == "qsf" || licSingleRow.ArticleDetail == "msf" || licSingleRow.ArticleDetail == "tsf"
                        || licSingleRow.ArticleDetail == "wsf")
                    {
                        licSingleRow.StartDate = DateTime.Now;
                        licSingleRow.MaintStartDate = DateTime.Now;
                        licSingleRow.EndDate = Convert.ToDateTime("01/01/2028");
                        licSingleRow.MaintEndDate = Convert.ToDateTime("01/01/2028");
                    }
                    if (licSingleRow.ArticleDetail == "pl")
                    {
                        licSingleRow.EndDate = licSingleRow.StartDate;
                        licSingleRow.MaintStartDate = licSingleRow.StartDate;
                        licSingleRow.MaintEndDate = licSingleRow.StartDate;
                    }
                }
                else
                {
                    licSingleRow.Import = 0;
                    licSingleRow.StartDate = DateTime.Now;
                    licSingleRow.MaintStartDate = DateTime.Now;
                    licSingleRow.EndDate = DateTime.Now;
                    licSingleRow.MaintEndDate = DateTime.Now;
                }
                if (string.IsNullOrEmpty(licSingleRow.MachineID))
                {
                    if (version < 2015)
                        licSingleRow.MachineID = "ABCDEFGH";
                    else
                        licSingleRow.MachineID = "KIDABCDEFGH";
                }
                var sr = licSingleRow.LicenseID.Length == 1 ? licSingleRow.LicenseID + "0" : licSingleRow.LicenseID;
                var maxq = db.Licenses.Where(u => u.LicenseID.StartsWith(sr)).Max(x => x.LicenseID);

                char lc = licSingleRow.LicenseID.Length == 1 ? Convert.ToChar(licSingleRow.LicenseID) : Convert.ToChar(licSingleRow.LicenseID.Substring(licSingleRow.LicenseID.Length - 1));
                switch (licSingleRow.LicenseID)
                {
                    case "POOL":
                    case "EVAL":
                        var lId = licSingleRow.LicenseID + (Convert.ToInt64(maxq.Split(lc)[1]) + 1).ToString("D5");
                        licSingleRow.LicenseID = lId;
                        break;
                    case "TEST":
                        var lid = licSingleRow.LicenseID + (Convert.ToInt64(maxq.Split(lc)[2]) + 1).ToString("D5");
                        licSingleRow.LicenseID = lid;
                        break;
                    case "L":
                        var LID = licSingleRow.LicenseID + (Convert.ToInt64(maxq.Split(lc)[1]) + 1).ToString("D8");
                        licSingleRow.LicenseID = LID;
                        break;
                    default:
                        var lID = licSingleRow.LicenseID + (Convert.ToInt64(maxq.Split(lc)[1]) + 1).ToString("D6");
                        licSingleRow.LicenseID = lID;
                        break;
                }
                try
                {
                    db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[DPT_Licenses] (LicenseID, AccountNumber, ProductName, " +
                        "ArticleDetail, Quantity, LicenseType, MachineID, Ancestor, StartDate, EndDate, MaintStartDate, MaintEndDate," +
                        " Version, OriginalProduct, Note, [2Bexp], [2Bval], [2Bins], ExportedNum, MaxExport, Vend_String, FlexType)" +
                        " VALUES ('" + licSingleRow.LicenseID + "','" + licSingleRow.AccountNumber + "','" + licSingleRow.ProductName
                        + "','" + licSingleRow.ArticleDetail + "','" + licSingleRow.Quantity + "','" + licSingleRow.LicenseType + "','" +
                        licSingleRow.MachineID + "','" + licSingleRow.Ancestor + "','" + licSingleRow.StartDate + "','" +
                        licSingleRow.EndDate + "','" + licSingleRow.MaintStartDate + "','" + licSingleRow.MaintEndDate + "','" +
                        licSingleRow.Version + "','" + licSingleRow.OriginalProduct + "','" + licSingleRow.Note + "','" + licSingleRow.Installed
                        + "','" + licSingleRow.Exported + "','" + licSingleRow.Import + "','" + licSingleRow.ExportedNum + "','" +
                        licSingleRow.MaxExport + "','" + licSingleRow.Vend_String + "','" + licSingleRow.FlexType + "');");
                }
                catch (Exception e)
                {
                    return Json(e.Message, JsonRequestBehavior.AllowGet);
                }
            }

            return Json("Saved LicenseID: " + licSingleRow.LicenseID, JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Session["CurrentCulture"] = LocalizationHelper.SetLocalization(Session["CurrentCulture"]);

            base.OnActionExecuting(filterContext);
        }

    }
}
