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
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(_db.Licenses.Select(x => x.ProductName).Distinct().ToList()).ToString(Formatting.None));
            ViewBag.ProductName = System.Convert.ToBase64String(plainTextBytes);
            var plainTextBytes2 = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(_db.Licenses.Select(x => x.ArticleDetail).Distinct().ToList()).ToString(Formatting.None));
            ViewBag.ArticleDetail = System.Convert.ToBase64String(plainTextBytes2);
            var plainTextBytes3 = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(_db.Licenses.Select(x => x.Version).Distinct().ToList()).ToString(Formatting.None));
            ViewBag.Version = System.Convert.ToBase64String(plainTextBytes3);
            var plainTextBytes4 = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(_db.Licenses.Select(x => x.LastExp).Distinct().ToList()).ToString(Formatting.None));
            ViewBag.LastExp = System.Convert.ToBase64String(plainTextBytes4);
            var plainTextBytes5 = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(_db.Licenses.Select(x => x.LicenseFlag).Distinct().ToList()).ToString(Formatting.None));
            ViewBag.LicenseFlag = System.Convert.ToBase64String(plainTextBytes5);

            ViewBag.IsAdmin = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin");
            ViewBag.IsInternal = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal");
            ViewBag.IsVarExp = Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarMed");
            ViewBag.IsVar = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Var");
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
                                LicenseFlag = x.LicenseFlag,
                                MachineID = x.MachineID,
                                ProductName = x.ProductName,
                                MaintEndDate = x.MaintEndDate,
                                Installed = x.Installed,
                                Exported = x.Exported,
                                Import = x.Import,
                                Renew = x.Renew,
                                ArticleDetail = x.ArticleDetail,
                                PwdCode = x.PwdCode
                            };
                var y = context.Companies.Where(a => a.AccountNumber == x.AccountNumber).ToList().FirstOrDefault();
                licenseState.salesRep = y.SalesRep;
                var lf = context.LicFlag.Where(k => k.LicenseFlag == x.LicenseFlag).ToList().FirstOrDefault();
                licenseState.Export_Safenet = lf.Export_Safenet;
                licenseState.Install_Safenet = lf.Install_Safenet;
                licenseState.ChangeVersion_Safenet = lf.ChangeVersion_Safenet;
                licenseState.Renewal_Safenet = lf.Renewal_Safenet;
                licenseState.Install_Legacy = lf.Install_Legacy;
                licenseState.ChangeVersion_Legacy = lf.ChangeVersion_Legacy;
                licenseState.Renewal_Legacy = lf.Renewal_Legacy;
                licenseState.Blocked = context.SpecialCompanies.Where(c => c.Description == "BLOCKED").Select(c => c.AccountNumber).ToList().Contains(x.AccountNumber) ? 1 : 0;
            }
            return Json(licenseState, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Internal,VarExp,VarMed,Var")]
        [HttpPost]
        public ActionResult SingleLicenseRow(LicenseView licSingleRow)
        {
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
                        companyList.AddRange(db.Companies.Where(x => x.SalesRep.Contains(sR)).OrderBy(k => k.AccountName).Select(u => u.AccountName + " \"" + u.AccountNumber + "\"").ToList());
                        //companyList.Add(company.AccountName + " \"" + company.AccountNumber + "\"");
                    }
                    else
                        companyList.AddRange(db.Companies.Where(x => salesRep.Contains(x.SalesRep)).OrderBy(k => k.AccountName).Select(u => u.AccountName + " \"" + u.AccountNumber + "\"").ToList());
                }
                companyList.Sort();

                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(companyList).ToString(Formatting.None));
                ViewBag.Companies = System.Convert.ToBase64String(plainTextBytes);
            }
            List<LicenseView> rows = new List<LicenseView>();
            rows.Add(licSingleRow);
            return View(rows);
        }

        [Authorize(Roles = "Admin,Internal,VarExp,VarMed,Var")]
        [HttpPost]
        public JsonResult Modify(LicenseView licSingleRow)
        {
            using (var db = new DptContext())
            {
                var query =
                    from lic in db.Licenses
                    where lic.LicenseID == licSingleRow.LicenseID
                    select lic;
                if (query.Count() > 0)
                {
                    if (licSingleRow.AccountNumber.StartsWith("T3-"))
                    {
                        var cnt = db.Licenses.Where(x => x.AccountNumber == licSingleRow.AccountNumber && x.LicenseFlag == "premium"
                            && x.MachineID.Contains("ABCDEFGH")).Count();
                        if (cnt > 0)
                        {
                            return Json("The customer has already premium licenses to be installed!", JsonRequestBehavior.AllowGet);
                        }
                    }
                    if (query.FirstOrDefault().LicenseFlag.ToLower() == "pool" && licSingleRow.AccountNumber.StartsWith("T3-")
                        && query.FirstOrDefault().MachineID.Contains("ABCDEFGH"))
                    {
                        query.FirstOrDefault().AccountNumber = licSingleRow.AccountNumber;
                        if (query.FirstOrDefault().SalesRep.ToLower() != "sener")
                            query.FirstOrDefault().LicenseFlag = "evaluation";
                        query.FirstOrDefault().Import = 1;
                        db.SaveChanges();
                    }
                    if (query.FirstOrDefault().LicenseFlag.ToLower() == "pool" && licSingleRow.Quantity > 1
                        && query.FirstOrDefault().MachineID.Contains("ABCDEFGH")
                        && Int32.Parse(query.FirstOrDefault().Version) > 2014 && query.FirstOrDefault().ArticleDetail != "pl"
                        && query.FirstOrDefault().LicenseType.ToLower() == "floating"
                        && query.FirstOrDefault().SalesRep.ToLower() == "sener")
                    {
                        query.FirstOrDefault().Quantity = licSingleRow.Quantity;
                        db.SaveChanges();
                    }
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
                licSingleRow.Import = licSingleRow.LicenseFlag.ToLower() == "pool" ? 0 : 1;
                licSingleRow.Vend_String = "vs001";
                licSingleRow.FlexType = 0;
                licSingleRow.MaxExport = -1;
                licSingleRow.LicenseFlag = licSingleRow.ProductName.ToLower() == "tdviewerplus" ? "free" : licSingleRow.LicenseFlag;

                if (licSingleRow.LicenseType == "local" || (licSingleRow.LicenseType == "floating" && licSingleRow.Quantity < 1))
                    licSingleRow.Quantity = 1;

                if (licSingleRow.LicenseFlag.ToLower() != "new")
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
                        if (licSingleRow.LicenseFlag.ToLower() == "plasasp")
                            licSingleRow.MachineID = "BLKABCDEFGH";
                        else
                            licSingleRow.MachineID = "KIDABCDEFGH";
                }
                try
                {
                    var maxq = db.Licenses.Max(x => x.LicenseID);
                    if (!string.IsNullOrEmpty(maxq))
                        licSingleRow.LicenseID = "L" + (Convert.ToInt64(maxq.Substring(1, 8)) + 1).ToString("D8");
                    else
                        licSingleRow.LicenseID = "L00000001";

                    db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[DPT_Licenses] (LicenseID, AccountNumber, ProductName, " +
                        "ArticleDetail, Quantity, LicenseType, LicenseFlag, MachineID, Ancestor, StartDate, EndDate, MaintStartDate, MaintEndDate," +
                        " Version, OriginalProduct, Note, [2Bexp], [2Bval], [2Bins], ExportedNum, MaxExport, Vend_String, FlexType)" +
                        " VALUES ('" + licSingleRow.LicenseID + "','" + licSingleRow.AccountNumber + "','" + licSingleRow.ProductName
                        + "','" + licSingleRow.ArticleDetail + "','" + licSingleRow.Quantity + "','" + licSingleRow.LicenseType + "','" +
                        licSingleRow.LicenseFlag + "','" + licSingleRow.MachineID + "','" + licSingleRow.Ancestor + "','" + licSingleRow.StartDate + "','" +
                        licSingleRow.EndDate + "','" + licSingleRow.MaintStartDate + "','" + licSingleRow.MaintEndDate + "','" +
                        licSingleRow.Version + "','" + licSingleRow.OriginalProduct + "','" + licSingleRow.Note + "','" + licSingleRow.Installed
                        + "','" + licSingleRow.Exported + "','" + licSingleRow.Import + "','" + licSingleRow.ExportedNum + "','" +
                        licSingleRow.MaxExport + "','" + licSingleRow.Vend_String + "','" + licSingleRow.FlexType + "');");
                }
                catch (Exception e)
                {
                    LogHelper.WriteLog("LicenseController (AddNew): " + e.Message);
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
