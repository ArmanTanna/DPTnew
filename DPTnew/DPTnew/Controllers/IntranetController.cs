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

    [Authorize(Roles = "Admin,SuperUser")]
    public class IntranetController : BaseController
    {
        public ActionResult Index(int pageSize = 10)
        {
            return View();
        }

        [Authorize(Roles = "Admin,SuperUser")]
        [HttpPost]
        public JsonResult Search()
        {
            return Json(_db.Search<DptErp>(Request.GetSearchParams(), GetErpRows()), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,SuperUser")]
        [HttpPost]
        public ActionResult SingleErpRow(DptErp erpSingleRow)
        {
            List<DptErp> rows = new List<DptErp>();
            if (string.IsNullOrEmpty(erpSingleRow.Email))
                erpSingleRow.Email = WebSecurity.CurrentUserName;
            rows.Add(erpSingleRow);
            return View(rows);
        }
        [Authorize(Roles = "Admin,SuperUser")]
        [HttpPost]
        public ActionResult Modify(DptErp erpSingleRow)
        {
            using (var db = new DptContext())
            {
                var query =
                    from rows in db.ErpRows
                    where rows.Email == erpSingleRow.Email && rows.ActivityDate == erpSingleRow.ActivityDate
                    select rows;

                if (query.Count() == 0)
                {
                    erpSingleRow.Total = (erpSingleRow.TimeSpent1.HasValue ? erpSingleRow.TimeSpent1 : 0) + (erpSingleRow.TimeSpent2.HasValue ? erpSingleRow.TimeSpent2 : 0) +
                        (erpSingleRow.TimeSpent3.HasValue ? erpSingleRow.TimeSpent3 : 0) + (erpSingleRow.TimeSpent4.HasValue ? erpSingleRow.TimeSpent4 : 0) +
                        (erpSingleRow.TimeSpent5.HasValue ? erpSingleRow.TimeSpent5 : 0) + (erpSingleRow.TimeSpent6.HasValue ? erpSingleRow.TimeSpent6 : 0) +
                        (erpSingleRow.TimeSpent7.HasValue ? erpSingleRow.TimeSpent7 : 0) + (erpSingleRow.TimeSpent8.HasValue ? erpSingleRow.TimeSpent8 : 0) +
                        (erpSingleRow.TimeSpent9.HasValue ? erpSingleRow.TimeSpent9 : 0) + (erpSingleRow.TimeSpent10.HasValue ? erpSingleRow.TimeSpent10 : 0);

                    db.ErpRows.Add(erpSingleRow);
                }

                foreach (DptErp row in query)
                {
                    row.Title1 = erpSingleRow.Title1;
                    row.Title2 = erpSingleRow.Title2;
                    row.Title3 = erpSingleRow.Title3;
                    row.Title4 = erpSingleRow.Title4;
                    row.Title5 = erpSingleRow.Title5;
                    row.Title6 = erpSingleRow.Title6;
                    row.Title7 = erpSingleRow.Title7;
                    row.Title8 = erpSingleRow.Title8;
                    row.Title9 = erpSingleRow.Title9;
                    row.Title10 = erpSingleRow.Title10;
                    row.Description1 = erpSingleRow.Description1;
                    row.Description2 = erpSingleRow.Description2;
                    row.Description3 = erpSingleRow.Description3;
                    row.Description4 = erpSingleRow.Description4;
                    row.Description5 = erpSingleRow.Description5;
                    row.Description6 = erpSingleRow.Description6;
                    row.Description7 = erpSingleRow.Description7;
                    row.Description8 = erpSingleRow.Description8;
                    row.Description9 = erpSingleRow.Description9;
                    row.Description10 = erpSingleRow.Description10;
                    row.TimeSpent1 = erpSingleRow.TimeSpent1;
                    row.TimeSpent2 = erpSingleRow.TimeSpent2;
                    row.TimeSpent3 = erpSingleRow.TimeSpent3;
                    row.TimeSpent4 = erpSingleRow.TimeSpent4;
                    row.TimeSpent5 = erpSingleRow.TimeSpent5;
                    row.TimeSpent6 = erpSingleRow.TimeSpent6;
                    row.TimeSpent7 = erpSingleRow.TimeSpent7;
                    row.TimeSpent8 = erpSingleRow.TimeSpent8;
                    row.TimeSpent9 = erpSingleRow.TimeSpent9;
                    row.TimeSpent10 = erpSingleRow.TimeSpent10;
                    row.Total = (erpSingleRow.TimeSpent1.HasValue ? erpSingleRow.TimeSpent1 : 0) + (erpSingleRow.TimeSpent2.HasValue ? erpSingleRow.TimeSpent2 : 0) +
                        (erpSingleRow.TimeSpent3.HasValue ? erpSingleRow.TimeSpent3 : 0) + (erpSingleRow.TimeSpent4.HasValue ? erpSingleRow.TimeSpent4 : 0) +
                        (erpSingleRow.TimeSpent5.HasValue ? erpSingleRow.TimeSpent5 : 0) + (erpSingleRow.TimeSpent6.HasValue ? erpSingleRow.TimeSpent6 : 0) +
                        (erpSingleRow.TimeSpent7.HasValue ? erpSingleRow.TimeSpent7 : 0) + (erpSingleRow.TimeSpent8.HasValue ? erpSingleRow.TimeSpent8 : 0) +
                        (erpSingleRow.TimeSpent9.HasValue ? erpSingleRow.TimeSpent9 : 0) + (erpSingleRow.TimeSpent10.HasValue ? erpSingleRow.TimeSpent10 : 0);
                }
                db.SaveChanges();
            }
            return View();
        }

    }

}