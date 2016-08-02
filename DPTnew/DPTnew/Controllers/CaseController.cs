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

    [Authorize(Roles = "Admin,Internal,Var,VarExp")]
    public class CaseController : BaseController
    {
        public ActionResult Index(int pageSize = 10)
        {
            ViewBag.UserRole = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal");
            return View();
        }

        [Authorize(Roles = "Admin,Internal,Var,VarExp")]
        [HttpPost]
        public JsonResult Search()
        {
            var sps = Request.GetSearchParams();
            var items = GetCases();
            foreach (var sp in sps)
            {
                items = _db.Search<DptCases>(sp, items);
            }
            return Json(_db.ConvertToSearchResult<DptCases>(sps.FirstOrDefault(), items), JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Internal,Var,VarExp")]
        [HttpPost]
        public ActionResult NewCaseRow(UpdateCase caseRow)
        {
            using (var db = new DptContext())
            {
                var userName = Membership.GetUser().UserName;
                var user = db.Contacts.Where(u => u.Email == userName).ToList().FirstOrDefault();
                var company = db.Companies.Where(u => u.AccountNumber == user.AccountNumber).ToList().FirstOrDefault();
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(db.Companies.Where(x => x.SalesRep.Contains(company.SalesRep)).Select(u => u.AccountName).ToList()).ToString(Formatting.None));
                ViewBag.Companies = System.Convert.ToBase64String(plainTextBytes);
                ViewBag.UserRole = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal");
            }
            return View(caseRow);
        }

        [Authorize(Roles = "Admin,Internal,Var,VarExp")]
        [HttpPost]
        public ActionResult Insert(UpdateCase caseRow, HttpPostedFileBase file)
        {
            using (var db = new DptContext())
            {
                var query = from ucase in db.Cases
                            where ucase.CaseId == caseRow.CaseId
                            select ucase;

                if (query.Count() == 0)
                {
                    var newCase = new DptCases();
                    newCase.AccountName = caseRow.AccountName;
                    newCase.AccountNumber = _db.Companies.Where(c => c.AccountName == newCase.AccountName).FirstOrDefault().AccountNumber;
                    newCase.Contact = caseRow.Contact;
                    try
                    {
                        newCase.ContactId = _db.Contacts.Where(c => c.Email == caseRow.Contact).FirstOrDefault().UserId;
                    }
                    catch (Exception e)
                    {
                        ViewBag.ok1 = "The Contact doesn't exist in the DB!";
                        return View("Success");
                    }
                    newCase.CreatedOn = DateTime.Now;
                    newCase.Description = caseRow.Description;
                    newCase.Details = caseRow.Details;
                    newCase.MachineId = caseRow.MachineId;
                    newCase.ModifiedOn = DateTime.Now;
                    newCase.Platform = caseRow.Platform;
                    newCase.PlatformVersion = caseRow.PlatformVersion;
                    newCase.Product = caseRow.Product;
                    newCase.ProductVersion = caseRow.ProductVersion;
                    newCase.Severity = caseRow.Severity;
                    newCase.Status = "Open";
                    newCase.Type = "Bug";
                    db.Cases.Add(newCase);
                    db.SaveChanges();

                    if (file != null)
                    {
                        var basePath = "C:\\inetpub\\wwwroot\\Case";//"C:\\AAIT\\Visual Studio 2013\\Projects\\DPTnew\\Case";
                        var folder = db.Cases.Local[0].CaseId.ToString();
                        var path = basePath + "\\" + folder;
                        try
                        {
                            System.IO.Directory.CreateDirectory(path);
                            newCase.File = path + "\\" + file.FileName;
                            file.SaveAs(newCase.File);
                            db.SaveChanges();
                        }
                        catch (Exception e)
                        {
                            ViewBag.ok1 = "Cannot save the uploaded file!";
                            return View("Success");
                        }
                    }
                }
                else
                {
                    foreach (DptCases ncase in query.ToList())
                    {
                        ncase.ModifiedOn = DateTime.Now;
                        ncase.CCEngineer = caseRow.CCEngineer;
                        try
                        {
                            ncase.CCEngineerId = _db.Contacts.Where(c => c.Email == caseRow.CCEngineer).FirstOrDefault().UserId;
                        }
                        catch (Exception e)
                        {
                            ViewBag.ok1 = "The CCEngineer doesn't exist in the DB!";
                            return View("Success");
                        }
                        if (ncase.Status != caseRow.Status)
                        {
                            var dcl = new DptCaseLog();
                            dcl.CaseId = caseRow.CaseId;
                            dcl.CreatedBy = Membership.GetUser().UserName;
                            dcl.CreatedOn = DateTime.Now;
                            dcl.Status = caseRow.Status;
                            db.CaseLogs.Add(dcl);
                            db.SaveChanges();
                        }
                        ncase.Status = caseRow.Status;
                        db.SaveChanges();
                    }
                }
            }
            ViewBag.ok1 = "The new case is saved correctly!";
            return View("Success");
        }

        [Authorize(Roles = "Admin,Internal,Var,VarExp")]
        [HttpPost]
        public ActionResult NewCaseHistoryRow(UpdateCase caseRow)
        {
            caseRow.Description = "";
            caseRow.Details = "";
            return View(caseRow);
        }

        [Authorize(Roles = "Admin,Internal,Var,VarExp")]
        [HttpPost]
        public ActionResult Update(UpdateCase caseHistoryRow, HttpPostedFileBase file)
        {
            var chl = new DptCaseHistory();
            chl.CaseId = caseHistoryRow.CaseId;
            chl.CreatedOn = DateTime.Now;
            chl.Description = caseHistoryRow.Description;
            chl.Details = caseHistoryRow.Details;
            chl.CreatedBy = Membership.GetUser().UserName;
            _db.CaseHistories.Add(chl);
            _db.SaveChanges();
            if (file != null)
            {
                var basePath = "C:\\inetpub\\wwwroot\\Case" + "\\" + caseHistoryRow.CaseId.ToString();
                //"C:\\AAIT\\Visual Studio 2013\\Projects\\DPTnew\\Case" + "\\" + caseHistoryRow.CaseId.ToString();
                var folder = _db.CaseHistories.Local[0].CaseHistoryId.ToString();
                var path = basePath + "\\" + folder;
                try
                {
                    System.IO.Directory.CreateDirectory(path);
                    chl.File = path + "\\" + file.FileName;
                    file.SaveAs(chl.File);
                    _db.SaveChanges();
                }
                catch (Exception e)
                {
                    ViewBag.ok1 = "Cannot save the uploaded file!";
                    return View("Success");
                }
            }
            ViewBag.ok1 = "The case update is saved correctly!";
            return View("Success");
        }

        [Authorize(Roles = "Admin,Internal,Var,VarExp")]
        [HttpPost]
        public ActionResult CaseHistories(int caseId)
        {
            return View(_db.CaseHistories.Where(c => c.CaseId == caseId));
        }


        [Authorize(Roles = "Admin,Internal,Var,VarExp")]
        [HttpPost]
        public JsonResult GetContacts(string companyName)
        {
            using (var db = new DptContext())
            {
                var company = db.Companies.Where(c => c.AccountName == companyName).FirstOrDefault();
                var contacts = db.Peoples.Where(u => u.AccountNumber == company.AccountNumber).ToList();
                return Json(contacts, JsonRequestBehavior.AllowGet);
            }
        }

    }

}