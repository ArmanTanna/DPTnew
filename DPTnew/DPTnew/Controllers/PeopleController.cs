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

    [Authorize(Roles = "Admin,VarExp,Internal")]
    public class PeopleController : BaseController
    {
        public ActionResult Index(int pageSize = 10)
        {
            LocalizationHelper.SetLocalization(Session["CurrentCulture"]);
            return View();
        }

        [Authorize(Roles = "Admin,VarExp,Internal")]
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

        [Authorize(Roles = "Admin,VarExp,Internal")]
        [HttpPost]
        public ActionResult SinglePeopleRow(People pplSingleRow)
        {
            List<People> rows = new List<People>();
            rows.Add(pplSingleRow);
            return View(rows);
        }

        [Authorize(Roles = "Admin,VarExp,Internal")]
        [HttpPost]
        public JsonResult Modify(People pplSingleRow)
        {
            using (var db = new DptContext())
            {
                PeopleRoles pplr = new PeopleRoles();
                pplr.UserId = pplSingleRow.UserId;
                pplr.RoleId = pplSingleRow.RoleId;

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
                try
                {
                    var query =
                from ppl in db.Peoples
                where ppl.UserId == pplr.UserId
                select ppl;
                    if (query.Count() > 0)
                    {
                        query.FirstOrDefault().Email = pplSingleRow.Email;
                        query.FirstOrDefault().FirstName = pplSingleRow.FirstName;
                        query.FirstOrDefault().LastName = pplSingleRow.LastName;
                        query.FirstOrDefault().Language = pplSingleRow.Language;
                        query.FirstOrDefault().EmailStatus = pplSingleRow.EmailStatus;
                        query.FirstOrDefault().PrimaryContact = pplSingleRow.PrimaryContact;
                        query.FirstOrDefault().FirstNameK = pplSingleRow.FirstNameK;
                        query.FirstOrDefault().LastNameK = pplSingleRow.LastNameK;
                        query.FirstOrDefault().Status = pplSingleRow.Status;
                        db.SaveChanges();
                    }
                }
                catch (Exception e)
                {
                    return Json(e.InnerException.InnerException.Message, JsonRequestBehavior.AllowGet);
                }
            }
            return Json("Saved!", JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,VarExp,Internal")]
        [HttpPost]
        public JsonResult GetPeopleRolesFromDB()
        {
            return Json(GetWebMainRoles(), JsonRequestBehavior.AllowGet);
        }

    }

}