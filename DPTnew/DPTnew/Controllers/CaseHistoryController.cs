using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPTnew.Models;
using Newtonsoft.Json;
using DPTnew.Helper;

namespace DPTnew.Controllers
{
    [Authorize(Roles = "Admin,Internal,VarExp")]
    public class CaseHistoryController : BaseController
    {
        public ActionResult Index(int pageSize = 10)
        {
            return View();
        }

        [HttpPost]
        public JsonResult Search()
        {
            var sps = Request.GetSearchParams();
            var items = GetHistoryCases();
            foreach (var sp in sps)
            {
                items = _db.Search<DptCaseHistory>(sp, items);
            }
            return Json(_db.ConvertToSearchResult<DptCaseHistory>(sps.FirstOrDefault(), items), JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Session["CurrentCulture"] = LocalizationHelper.SetLocalization(Session["CurrentCulture"]);

            base.OnActionExecuting(filterContext);
        }

    }

}