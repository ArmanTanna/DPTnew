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

    [Authorize(Roles = "Admin,Internal")]
    public class IntranetController : BaseController
    {
        public ActionResult Index(int pageSize = 10)
        {
            return View();
        }

        [Authorize(Roles = "Admin,Internal")]
        [HttpPost]
        public JsonResult Search()
        {
            var sps = Request.GetSearchParams();
            var items = GetErpRows();
            foreach (var sp in sps)
            {
                items = _db.Search<DptErp>(sp, items);
            }
            return Json(_db.ConvertToSearchResult<DptErp>(sps.FirstOrDefault(), items), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Internal")]
        [HttpPost]
        public ActionResult SingleErpRow(DptErp erpSingleRow)
        {
            List<DptErp> rows = new List<DptErp>();
            if (string.IsNullOrEmpty(erpSingleRow.Email))
                erpSingleRow.Email = WebSecurity.CurrentUserName;
            rows.Add(erpSingleRow);
            return View(rows);
        }

        [Authorize(Roles = "Admin,Internal")]
        [HttpPost]
        public JsonResult Modify(DptErp erpSingleRow)
        {
            using (var db = new DptContext())
            {
                var query =
                    from rows in db.ErpRows
                    where rows.ActivityId == erpSingleRow.ActivityId
                    select rows;

                if (query.Count() == 0)
                {
                    db.ErpRows.Add(erpSingleRow);
                }

                foreach (DptErp row in query)
                {
                    row.Title = erpSingleRow.Title;
                    row.Description = erpSingleRow.Description;
                    row.TimeSpent = erpSingleRow.TimeSpent;
                }
                db.SaveChanges();
            }
            return Json(erpSingleRow, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Internal")]
        [HttpPost]
        public JsonResult GetActivities()
        {
            return Json(GetActivityTitles(), JsonRequestBehavior.AllowGet);
        }
    }

}