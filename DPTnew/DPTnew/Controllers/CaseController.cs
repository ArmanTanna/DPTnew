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
using System.Net.Mail;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Net;
using System.IO;
using DPTnew.Helper;
using Microsoft.JScript;

namespace DPTnew.Controllers
{
    [Authorize]
    public class CaseController : BaseController
    {
        public ActionResult Index(int pageSize = 10)
        {
            //LocalizationHelper.SetLocalization(Session["CurrentCulture"]);
            ViewBag.UserRole = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal")
                || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp");
            return View();
        }

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

        [HttpPost]
        public ActionResult NewCaseRow(UpdateCase caseRow)
        {
            try
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
                    if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Var") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarMed")
                        || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp"))
                    {
                        if (company.SalesRep == "t3kk" && (!company.AccountName.Contains("T3 JAPAN KK")))
                            companyList.Add(company.AccountName + " \"" + company.AccountNumber + "\"");
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
                            companyList.Sort();
                        }
                    }
                    else
                        companyList.Add(company.AccountName + " \"" + company.AccountNumber + "\"");

                    var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(companyList).ToString(Formatting.None));
                    ViewBag.Companies = System.Convert.ToBase64String(plainTextBytes);
                    ViewBag.UserRole = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal")
                        || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp");
                    ViewBag.CurrUser = Membership.GetUser().UserName;
                }
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("Case (NewCaseRow): " + e.Message);
            }
            return View(caseRow);
        }

        [HttpPost]
        public ActionResult Insert(UpdateCase caseRow, string submitButton)
        {
            if (submitButton == "Close")
            {
                ViewBag.ok1 = DPTnew.Localization.Resource.CaseInsertCloseMsg;
                return View("Success");
            }
            if (string.IsNullOrEmpty(caseRow.AccountName) || string.IsNullOrEmpty(caseRow.Contact)
                || string.IsNullOrEmpty(caseRow.Description) || string.IsNullOrEmpty(caseRow.Details))
            {
                ViewBag.ok1 = DPTnew.Localization.Resource.CaseInsertWrongMsg;
                return View("Success");
            }

            string caseId = "";
            using (var db = new DptContext())
            {
                var newCase = new DptCases();
                newCase.AccountName = caseRow.AccountName;
                newCase.AccountNumber = db.Companies.Where(c => c.AccountName == newCase.AccountName).FirstOrDefault().AccountNumber;
                newCase.Contact = caseRow.Contact;
                try
                {
                    var usr = db.Contacts.Where(c => c.Email == caseRow.Contact).FirstOrDefault();
                    newCase.ContactId = usr.UserId;
                    if (usr.Language.ToLower() == "japanese")
                        newCase.Language = "Japanese";
                    else
                        if (usr.Language.ToLower() == "korean")
                            newCase.Language = "Korean";
                        else
                            newCase.Language = "English";
                }
                catch (Exception e)
                {
                    LogHelper.WriteLog("CaseController (Insert): " + e.Message);
                    ViewBag.ok1 = DPTnew.Localization.Resource.CaseInsertContactMsg;
                    return View("Success");
                }
                newCase.CreatedOn = DateTime.Now;
                newCase.CreatedBy = Membership.GetUser().UserName;
                newCase.Description = GlobalObject.unescape(caseRow.Description);
                newCase.Details = GlobalObject.unescape(caseRow.Details);
                newCase.ModifiedOn = DateTime.Now;
                newCase.Platform = caseRow.Platform;
                newCase.PlatformVersion = caseRow.PlatformVersion;
                newCase.Product = caseRow.Product;
                newCase.ProductVersion = caseRow.ProductVersion;
                newCase.Severity = caseRow.Severity;
                newCase.Type = "General Request";
                newCase.Status = "Open";

                var maxq = db.Cases.Where(u => u.CaseId.StartsWith("D")).Max(x => x.CaseId);
                if (maxq == null)
                    newCase.CaseId = "D00000001";
                else
                    newCase.CaseId = "D" + (System.Convert.ToInt64(maxq.Split('D')[1]) + 1).ToString("D8");

                db.Cases.Add(newCase);
                db.SaveChanges();
                caseId = db.Cases.Local[0].CaseId;

                var chl = new DptCaseHistory();
                chl.CaseId = caseId;
                chl.CreatedOn = DateTime.Now;
                chl.Description = newCase.Description;
                chl.Details = newCase.Details;
                chl.CreatedBy = Membership.GetUser().UserName;
                if (caseRow.File != null)
                {
                    var basePath = "E:\\Case";
                    var folder = caseId;
                    var path = basePath + "\\" + folder;
                    try
                    {
                        System.IO.Directory.CreateDirectory(path);
                        chl.File = path + "\\" + GlobalObject.unescape(caseRow.fileName);
                        caseRow.File.SaveAs(chl.File);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteLog("CaseController (Insert): " + e.Message);
                        ViewBag.ok1 = DPTnew.Localization.Resource.CaseInsertFileMsg;
                        return View("Success");
                    }
                }
                db.CaseHistories.Add(chl);
                db.SaveChanges();

                MailMessage mail = new MailMessage("is@dptcorporate.com", "Caseinteractions@think3.eu");
                if (newCase.Language.ToLower() == "japanese")
                {
                    mail.Subject = "[このメールには返信しないでください] ご質問項目 #" + caseId + " が作成されました - " + newCase.Description;
                    mail.Body = "お客様。\n以下のご質問項目 #" + caseId + " が作成されたことをお知らせいたします。\n\n" +
                        "詳細：\n" + newCase.Details + "\n\nご質問項目の詳細はこちらでご覧ください： http://dpt3.dptcorporate.com/Case" +
                        "\n\nご質問項目に追記したり、ファイルを追加したりする場合は、Web サイトから直接お願いいたします。" +
                        "このメールには返信しないでください。このメールに返信すると無効なメールアドレス（is@dptcorporate.com）" +
                        "へ返信されます。\n\n以上、よろしく願いいたします。\nシンクスリー・カスタマーケア";
                }
                else
                {
                    mail.Subject = "[DO NOT REPLY] Case #" + caseId + " has been inserted - " + newCase.Description;
                    mail.Body = "Dear User, \n\nThe case #" + caseId + " has been inserted.\n\n" +
                        "Details: " + newCase.Details + "\n\nYou can browse your cases at http://dpt3.dptcorporate.com/Case" +
                        "\n\nBest Regards,\n\nCustomer Care team";
                }
                try
                {
                    MailHelper.SendMail(mail);
                }
                catch (Exception e)
                {
                    LogHelper.WriteLog("CaseController (Insert): " + e.Message + "-" + e.InnerException);
                }
            }
            ViewBag.ok1 = DPTnew.Localization.Resource.CaseInsertSaveMsg;
            return View("Success");
        }

        [Authorize(Roles = "Admin,Internal,VarExp")]
        [HttpPost]
        public ActionResult ModifyCaseRow(UpdateCase caseRow)
        {
            using (var db = new DptContext())
            {
                var res = db.Cases.Where(x => x.CaseId == caseRow.CaseId).FirstOrDefault();
                caseRow.Description = res.Description;
                caseRow.Details = res.Details;
            }
            return View(caseRow);
        }

        [Authorize(Roles = "Admin,Internal,VarExp")]
        [HttpPost]
        public ActionResult Modify(UpdateCase caseRow, string submitButton)
        {
            if (submitButton == "Close")
            {
                ViewBag.ok1 = DPTnew.Localization.Resource.CaseModifyCloseMsg;
                return View("Success");
            }
            caseRow.Status = GlobalObject.unescape(caseRow.Description);
            using (var db = new DptContext())
            {
                var query = from ucase in db.Cases
                            where ucase.CaseId == caseRow.CaseId
                            select ucase;
                if (query.Count() > 0)
                {
                    foreach (DptCases ncase in query.ToList())
                    {
                        ncase.ModifiedOn = DateTime.Now;
                        ncase.CCEngineer = caseRow.CCEngineer;
                        ncase.EDBnumber = caseRow.EDBnumber;
                        ncase.Type = caseRow.Type;
                        try
                        {
                            ncase.CCEngineerId = db.Contacts.Where(c => c.Email == caseRow.CCEngineer).FirstOrDefault().UserId;
                        }
                        catch (Exception e)
                        {
                            LogHelper.WriteLog("CaseController (Modify): " + e.Message);
                            ViewBag.ok1 = DPTnew.Localization.Resource.CaseModifyCCEngMsg;
                            return View("Success");
                        }
                        var oldstatus = caseRow.Status;
                        if (caseRow.Status == "Open")
                            caseRow.Status = "Working";

                        if (ncase.Status != caseRow.Status)
                        {
                            var dcl = new DptCaseLog();
                            dcl.CaseId = caseRow.CaseId;
                            dcl.CreatedBy = Membership.GetUser().UserName;
                            dcl.CreatedOn = DateTime.Now;
                            dcl.Status = caseRow.Status;
                            db.CaseLogs.Add(dcl);
                            db.SaveChanges();

                            if (oldstatus != "Open")
                            {
                                MailMessage mail = new MailMessage("is@dptcorporate.com", ncase.Contact);
                                if (ncase.Contact != ncase.CreatedBy)
                                    mail.Bcc.Add(new MailAddress(dcl.CreatedBy));
                                mail.Bcc.Add(new MailAddress("Caseinteractions@think3.eu"));
                                var cr = db.Cases.Where(c => c.CaseId == caseRow.CaseId).FirstOrDefault();
                                if (caseRow.Status != "Closed")
                                {
                                    var lchr = db.CaseHistories.Where(c => c.CaseId == caseRow.CaseId).OrderByDescending(x => x.CaseHistoryId).FirstOrDefault();
                                    if (cr.Language.ToLower() == "japanese")
                                    {
                                        mail.Subject = "[このメールには返信しないでください] ご質問項目 #" + caseRow.CaseId + " が更新されました - " + lchr.Description;
                                        mail.Body = "お客様。\nご質問項目 #" + caseRow.CaseId + " が更新されたことをお知らせいたします。\n\n" +
                                            "詳細：\n" + lchr.Details + "\n\nご質問項目の詳細はこちらでご覧ください： http://dpt3.dptcorporate.com/Case" +
                                            " \n\nご質問項目に追記したり、ファイルを追加したりする場合は、Web サイトから直接お願いいたします。" +
                                            "このメールには返信しないでください。このメールに返信すると無効なメールアドレス（is@dptcorporate.com）へ返信されます。" +
                                            "\n\n以上、よろしくお願いいたします。\nシンクスリー・カスタマーケア";
                                    }
                                    else
                                    {
                                        mail.Subject = "[DO NOT REPLY] Case #" + caseRow.CaseId + " has been updated - " + lchr.Description;
                                        mail.Body = "Dear User, \n\nThe case #" + caseRow.CaseId + " status has changed.\n\n" +
                                            "Details: " + lchr.Details + "\n\nYou can browse your cases at http://dpt3.dptcorporate.com/Case" +
                                            "\n\nBest Regards,\n\nCustomer Care team";
                                    }
                                }
                                else
                                {
                                    if (cr.Language.ToLower() == "japanese")
                                    {
                                        mail.Subject = "[このメールには返信しないでください] ご質問項目 #" + caseRow.CaseId + " をクローズいたしました";
                                        mail.Body = "お客様。\n" + "本件はクローズさせていただきます。\n" +
                                            "より詳しい情報が必要な場合は、お手数ですが再度ご連絡ください。\n\n" +
                                            "ご質問項目の詳細はこちらでご覧ください：http://dpt3.dptcorporate.com/Case " +
                                            "\n\nご質問項目に追記したり、ファイルを追加したりする場合は、Web サイトから直接お願いいたします。" +
                                            "このメールには返信しないでください。このメールに返信すると無効なメールアドレス" +
                                            "（is@dptcorporate.com）へ返信されます。" +
                                            "\n\n以上、よろしく願いいたします。\nシンクスリー・カスタマーケア";
                                    }
                                    else
                                    {
                                        mail.Subject = "[DO NOT REPLY] Case #" + caseRow.CaseId + " has been closed";
                                        mail.Body = "Dear User,\n" + "We have closed the case.\n" +
                                            "If you need more information please do not hesitate to contact us.\n" +
                                            "To add more information and upload files, we strongly recommend you to use the web site " +
                                            "and NOT to reply to this email. In the latter case you’ll get a fictitious non-existent " +
                                            "address (noreply_thinkcare@think3.eu)." +
                                            "\n\nYou can browse your cases at http://dpt3.dptcorporate.com/Case " +
                                            " \nThank you for your patience and cooperation." +
                                            "\n\nBest Regards,\n\nCustomer Care team";
                                    }
                                }
                                try
                                {
                                    MailHelper.SendMail(mail);
                                }
                                catch (Exception e)
                                {
                                    LogHelper.WriteLog("CaseController (Modify): " + e.Message + "-" + e.InnerException);
                                }
                            }
                        }
                        ncase.Status = caseRow.Status;
                        db.SaveChanges();
                    }
                }
            }
            ViewBag.ok1 = DPTnew.Localization.Resource.CaseModifySaveMsg;
            return View("Success");
        }

        [HttpPost]
        public ActionResult NewCaseHistoryRow(UpdateCase caseRow)
        {
            return View(caseRow);
        }

        [HttpPost]
        public ActionResult Update(UpdateCase caseHistoryRow, string submitButton)
        {
            if (submitButton == "Close")
            {
                ViewBag.ok1 = DPTnew.Localization.Resource.CaseUpdateCloseMsg;
                return View("Success");
            }
            if (string.IsNullOrEmpty(caseHistoryRow.Description) || string.IsNullOrEmpty(caseHistoryRow.Details))
            {
                ViewBag.ok1 = DPTnew.Localization.Resource.CaseUpdateWrongMsg;
                return View("Success");
            }

            var casehId = 0;
            using (var db = new DptContext())
            {
                var chl = new DptCaseHistory();
                chl.CaseId = caseHistoryRow.CaseId;
                chl.CreatedOn = DateTime.Now;
                chl.Description = GlobalObject.unescape(caseHistoryRow.Description);
                chl.Details = GlobalObject.unescape(caseHistoryRow.Details);
                chl.CreatedBy = Membership.GetUser().UserName;
                db.CaseHistories.Add(chl);
                db.SaveChanges();
                casehId = db.CaseHistories.Local[0].CaseHistoryId;

                if (caseHistoryRow.File != null)
                {
                    var basePath = "E:\\Case" + "\\" + caseHistoryRow.CaseId.ToString();
                    var folder = casehId;
                    var path = basePath + "\\" + folder;
                    try
                    {
                        System.IO.Directory.CreateDirectory(path);
                        chl.File = path + "\\" + GlobalObject.unescape(caseHistoryRow.fileName);
                        caseHistoryRow.File.SaveAs(chl.File);
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteLog("CaseController (Update): " + e.Message);
                        ViewBag.ok1 = DPTnew.Localization.Resource.CaseInsertFileMsg;
                        return View("Success");
                    }
                }

                var origCase = from oc in db.Cases
                               where oc.CaseId == caseHistoryRow.CaseId
                               select oc;
                string destmail = null;
                string varmail = null;
                if (origCase.Count() > 0)
                {
                    foreach (var oc in origCase)
                    {
                        if (!string.IsNullOrEmpty(oc.CCEngineer))
                        {
                            if (oc.CCEngineer.ToLower() != chl.CreatedBy.ToLower())
                            {
                                oc.Status = "Working";
                                destmail = oc.CCEngineer;
                            }
                            else
                            {
                                oc.Status = "Waiting on Customer";
                                destmail = oc.Contact;
                                if (oc.Contact != oc.CreatedBy)
                                    varmail = oc.CreatedBy;
                            }
                            if (!string.IsNullOrEmpty(destmail))
                            {
                                MailMessage mail = new MailMessage("is@dptcorporate.com", destmail);
                                if (!string.IsNullOrEmpty(varmail))
                                    mail.Bcc.Add(new MailAddress(varmail));
                                mail.Bcc.Add(new MailAddress("Caseinteractions@think3.eu"));
                                if (oc.Language.ToLower() == "japanese")
                                {
                                    mail.Subject = "[このメールには返信しないでください] ご質問項目 #" + chl.CaseId + " が更新されました - " + chl.Description;
                                    mail.Body = "お客様。\nご質問項目 #" + chl.CaseId + " が更新されたことをお知らせいたします。\n\n" +
                                        "詳細：\n" + chl.Details + "\n\nご質問項目の詳細はこちらでご覧ください： http://dpt3.dptcorporate.com/Case" +
                                        " \n\nご質問項目に追記したり、ファイルを追加したりする場合は、Web サイトから直接お願いいたします。" +
                                        "このメールには返信しないでください。このメールに返信すると無効なメールアドレス（is@dptcorporate.com）へ返信されます。" +
                                        "\n\n以上、よろしくお願いいたします。\nシンクスリー・カスタマーケア";
                                }
                                else
                                {
                                    mail.Subject = "[DO NOT REPLY] Case #" + chl.CaseId + " has been updated - " + chl.Description;
                                    mail.Body = "Dear User, \n\nThe case #" + chl.CaseId + " status has changed.\n\n" +
                                        "Details: " + chl.Details + "\n\nYou can browse your cases at http://dpt3.dptcorporate.com/Case" +
                                        "\n\nBest Regards,\n\nCustomer Care team";
                                }
                                try
                                {
                                    MailHelper.SendMail(mail);
                                }
                                catch (Exception e)
                                {
                                    LogHelper.WriteLog("CaseController (Update): " + e.Message + "-" + e.InnerException);
                                }
                            }
                            oc.ModifiedOn = DateTime.Now;
                        }
                    }
                    db.SaveChanges();
                }
            }
            ViewBag.ok1 = DPTnew.Localization.Resource.CaseUpdateSaveMsg;
            return View("Success");
        }

        [HttpPost]
        public ActionResult CaseHistories(string caseId)
        {
            ViewBag.CaseId = caseId;
            using (var db = new DptContext())
            {
                try
                {
                    var xx = db.CaseHistories.Where(c => c.CaseId.Equals(caseId)).OrderByDescending(x => x.CaseHistoryId).ToList();
                    return View(xx);
                    //return View(db.CaseHistories.Where(c => c.CaseId.Equals(caseId)).OrderByDescending(x => x.CaseHistoryId).ToList());
                }
                catch (Exception e)
                {
                    return View();
                }
            }
        }

        [HttpPost]
        public JsonResult DeleteFile(int historyId)
        {
            using (var db = new DptContext())
            {
                var hId = db.CaseHistories.Where(h => h.CaseHistoryId == historyId).FirstOrDefault();
                System.IO.File.Delete(hId.File);
                hId.File = null;
                db.SaveChanges();
            }

            return Json(DPTnew.Localization.Resource.CaseDelFileMsg, JsonRequestBehavior.AllowGet);
        }

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

        [NonAction]
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Session["CurrentCulture"] = LocalizationHelper.SetLocalization(Session["CurrentCulture"]);

            base.OnActionExecuting(filterContext);
        }

    }

}