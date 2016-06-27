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

    [Authorize(Roles = "Admin,VarExp")]
    public class PeopleController : BaseController
    {
        public ActionResult Index(int pageSize = 10)
        {
            return View();
        }

        [Authorize(Roles = "Admin,VarExp")]
        [HttpPost]
        public JsonResult Search()
        {
            var sps = Request.GetSearchParams();
            var items = GetPeoples();
            foreach (var sp in sps)
            {
                items = _db.Search<People>(sp, items);
            }
            return Json(_db.ConvertToSearchResult<People>(sps.FirstOrDefault(), items), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,VarExp")]
        [HttpPost]
        public ActionResult SingleRow(People pplSingleRow)
        {
            List<People> rows = new List<People>();
            rows.Add(pplSingleRow);
            return View(rows);
        }

        [Authorize(Roles = "Admin,VarExp")]
        [HttpPost]
        public JsonResult Modify(People pplSingleRow)
        {
            using (var db = new DptContext())
            {
                PeopleRoles pplr = new PeopleRoles();
                pplr.UserId = pplSingleRow.UserId;
                pplr.RoleId = pplSingleRow.RoleId;
                try
                {
                    if (pplSingleRow.RoleId > 0)
                        db.PeopleRoles.Add(pplr);
                    else
                    {
                        var query =
                    from ppl in db.PeopleRoles
                    where ppl.UserId == pplr.UserId
                    select ppl;
                        if (query.Count() > 0)
                            db.PeopleRoles.Remove(query.FirstOrDefault());
                    }
                    db.SaveChanges();
                }
                catch (Exception e) { }
            }
            return Json(pplSingleRow, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,VarExp")]
        [HttpPost]
        public JsonResult GetPeopleRolesFromDB()
        {
            return Json(GetWebMainRoles(), JsonRequestBehavior.AllowGet);
        }

    }

}