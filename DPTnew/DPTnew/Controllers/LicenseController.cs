using DPTnew.Helper;
using DPTnew.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace DPTnew.Controllers
{
    public class LicenseController : BaseController
    {
        // GET: /All Companies/
        [Authorize(Roles = "Admin,Var,VarExp,Internal")]
        public ActionResult Index(int pageSize = 10)
        {
            LocalizationHelper.SetLocalization(Session["CurrentCulture"]);
            ViewBag.IsAdmin = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin");
            ViewBag.IsVarExpInt = Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal");
            return View();
        }

        [Authorize(Roles = "Admin,Var,VarExp,Internal")]
        [HttpPost]
        public JsonResult Search()
        {
            LocalizationHelper.SetLocalization(Session["CurrentCulture"]);
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

        [Authorize(Roles = "Admin,VarExp,Internal")]
        [HttpPost]
        public ActionResult SingleLicenseRow(LicenseView licSingleRow)
        {
            List<LicenseView> rows = new List<LicenseView>();
            rows.Add(licSingleRow);
            return View(rows);
        }

        [Authorize(Roles = "Admin,VarExp,Internal")]
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

    }
}
