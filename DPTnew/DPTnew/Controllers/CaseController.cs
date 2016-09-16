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

    public class CaseController : BaseController
    {
        public ActionResult Index(int pageSize = 10)
        {
            LocalizationHelper.SetLocalization(Session["CurrentCulture"]);
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
            using (var db = new DptContext())
            {
                var userName = Membership.GetUser().UserName;
                var user = db.Contacts.Where(u => u.Email == userName).ToList().FirstOrDefault();
                var company = db.Companies.Where(u => u.AccountNumber == user.AccountNumber).ToList().FirstOrDefault();
                var salesRep = db.SalesR.Where(u => u.Invoicer == company.AccountName).Select(u => u.SalesRep).ToList();
                List<String> companyList = new List<string>();
                if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal"))
                    companyList = db.Companies.Select(u => u.AccountName + " \"" + u.AccountNumber + "\"").ToList();
                if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Var") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp"))
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
            }
            List<UpdateCase> rows = new List<UpdateCase>();
            rows.Add(caseRow);
            return View(rows);
        }

        [HttpPost]
        public ActionResult Insert(UpdateCase caseRow)
        {
            if (string.IsNullOrEmpty(caseRow.AccountName) || string.IsNullOrEmpty(caseRow.Contact))
            {
                return Json("Cannot save the case!", JsonRequestBehavior.AllowGet);
            }

            int caseId = 0;
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
                    newCase.Language = usr.Language;
                }
                catch (Exception e)
                {
                    ViewBag.ok1 = "The Contact doesn't exist in the DB!";
                    return View("Success");
                }
                newCase.CreatedOn = DateTime.Now;
                newCase.CreatedBy = Membership.GetUser().UserName;
                newCase.Description = caseRow.Description;
                newCase.Details = caseRow.Details;
                newCase.MachineId = caseRow.MachineId;
                newCase.ModifiedOn = DateTime.Now;
                newCase.Platform = caseRow.Platform;
                newCase.PlatformVersion = caseRow.PlatformVersion;
                newCase.Product = caseRow.Product;
                newCase.ProductVersion = caseRow.ProductVersion;
                newCase.Severity = caseRow.Severity;
                newCase.Type = "Bug";
                newCase.Status = "Open";

                db.Cases.Add(newCase);
                db.SaveChanges();
                caseId = db.Cases.Local[0].CaseId;

                var chl = new DptCaseHistory();
                chl.CaseId = caseId;
                chl.CreatedOn = DateTime.Now;
                chl.Description = caseRow.Description;
                chl.Details = caseRow.Details;
                chl.CreatedBy = Membership.GetUser().UserName;
                db.CaseHistories.Add(chl);
                db.SaveChanges();

                MailMessage mail = new MailMessage("is@dptcorporate.com", "Caseinteractions@think3.eu");
                if (newCase.Language == "japanese")
                {
                    mail.Subject = "[このメールには返信しないでください] ご質問項目 #" + caseId + " が作成されました - " + caseRow.Description;
                    mail.Body = "お客様。\n以下のご質問項目 #" + caseId + " が作成されたことをお知らせいたします。\n\n" +
                        "詳細：\n" + caseRow.Details + "\n\nご質問項目の詳細はこちらでご覧ください： http://dpt3.dptcorporate.com/" +
                        "\n\nご質問項目に追記したり、ファイルを追加したりする場合は、Web サイトから直接お願いいたします。" +
                        "このメールには返信しないでください。このメールに返信すると無効なメールアドレス（is@dptcorporate.com）" +
                        "へ返信されます。\n\n以上、よろしく願いいたします。\nシンクスリー・カスタマーケア";
                }
                else
                {
                    mail.Subject = "[DO NOT REPLY] Case #" + caseId + " has been inserted - " + caseRow.Description;
                    mail.Body = "Dear User, \n\nThe case #" + caseId + " has been inserted.\n\n" +
                        "Details: " + caseRow.Details + "\n\nYou can browse your cases at http://dpt3.dptcorporate.com/" +
                        "\n\nBest Regards,\n\nCustomer Care team";
                }
                SendMail(mail);
            }
            return Json(caseId, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadFile(string caseRowID, string filename, HttpPostedFileBase file, string submitButton)
        {
            if (submitButton == "close")
            {
                ViewBag.ok1 = "The new case has been saved correctly!";
                return View("Success");
            }
            string decodedfl = GlobalObject.unescape(filename);

            //if (file != null && !(Path.GetExtension(file.FileName).Contains("zip")
            //    || Path.GetExtension(file.FileName).Contains("rar") /*||
            //    file.FileName.Split('.')[file.FileName.Split('.').Length - 1].Contains("7z")*/))
            //{
            //    ViewBag.ok1 = "Only the *.zip, and *.rar file are accepted!";
            //    return View("Success");
            //}

            if (file != null)
            {
                using (var db = new DptContext())
                {
                    var caseID = System.Convert.ToInt64(caseRowID);
                    var query = from ucase in db.CaseHistories
                                where ucase.CaseId == caseID
                                select ucase;
                    if (query.Count() > 0)
                    {
                        var basePath = "E:\\Case";
                        var folder = caseRowID;
                        var path = basePath + "\\" + folder;
                        foreach (DptCaseHistory ncase in query.ToList())
                        {
                            try
                            {
                                System.IO.Directory.CreateDirectory(path);
                                ncase.File = path + "\\" + decodedfl;
                                file.SaveAs(ncase.File);
                                db.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                ViewBag.ok1 = "Cannot save the uploaded file!";
                                return View("Success");
                            }
                        }
                    }
                }
                ViewBag.ok1 = "The uploaded file has been saved correctly!";
                return View("Success");
            }
            ViewBag.ok1 = "The new case has been saved correctly!";
            return View("Success");
        }

        [Authorize(Roles = "Admin,Internal,Var,VarExp")]
        [HttpPost]
        public ActionResult ModifyCaseRow(UpdateCase caseRow)
        {
            List<UpdateCase> rows = new List<UpdateCase>();
            rows.Add(caseRow);
            return View(rows);
        }

        [Authorize(Roles = "Admin,Internal,Var,VarExp")]
        [HttpPost]
        public ActionResult Modify(UpdateCase caseRow)
        {
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
                        ncase.MachineId = caseRow.MachineId;
                        try
                        {
                            ncase.CCEngineerId = db.Contacts.Where(c => c.Email == caseRow.CCEngineer).FirstOrDefault().UserId;
                        }
                        catch (Exception e)
                        {
                            return Json("The CCEngineer doesn't exist in the DB!", JsonRequestBehavior.AllowGet);
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
                                MailMessage mail = new MailMessage("is@dptcorporate.com", ncase.CreatedBy);
                                mail.Bcc.Add(new MailAddress("Caseinteractions@think3.eu"));
                                var cr = db.Cases.Where(c => c.CaseId == caseRow.CaseId).FirstOrDefault();
                                if (caseRow.Status != "Closed")
                                {
                                    var lchr = db.CaseHistories.Where(c => c.CaseId == caseRow.CaseId).OrderByDescending(x => x.CaseHistoryId).FirstOrDefault();
                                    if (cr.Language == "japanese")
                                    {
                                        mail.Subject = "[このメールには返信しないでください] ご質問項目 #" + caseRow.CaseId + " が更新されました - " + lchr.Description;
                                        mail.Body = "お客様。\nご質問項目 #" + caseRow.CaseId + " が更新されたことをお知らせいたします。\n\n" +
                                            "詳細：\n" + lchr.Details + "\n\nご質問項目の詳細はこちらでご覧ください： http://dpt3.dptcorporate.com/" +
                                            " \n\nご質問項目に追記したり、ファイルを追加したりする場合は、Web サイトから直接お願いいたします。" +
                                            "このメールには返信しないでください。このメールに返信すると無効なメールアドレス（is@dptcorporate.com）へ返信されます。" +
                                            "\n\n以上、よろしくお願いいたします。\nシンクスリー・カスタマーケア";
                                    }
                                    else
                                    {
                                        mail.Subject = "[DO NOT REPLY] Case #" + caseRow.CaseId + " has been updated - " + lchr.Description;
                                        mail.Body = "Dear User, \n\nThe case #" + caseRow.CaseId + " status has changed.\n\n" +
                                            "Details: " + lchr.Details + "\n\nYou can browse your cases at http://dpt3.dptcorporate.com/" +
                                            "\n\nBest Regards,\n\nCustomer Care team";
                                    }
                                }
                                else
                                {
                                    if (cr.Language == "japanese")
                                    {
                                        mail.Subject = "[このメールには返信しないでください] ご質問項目 #" + caseRow.CaseId + " をクローズいたしました";
                                        mail.Body = "お客様。\n" + "本件はクローズさせていただきます。\n" +
                                            "より詳しい情報が必要な場合は、お手数ですが再度ご連絡ください。\n\n" +
                                            "ご質問項目の詳細はこちらでご覧ください：http://dpt3.dptcorporate.com/ " +
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
                                            "\n\nYou can browse your cases at http://dpt3.dptcorporate.com/ " +
                                            " \nThank you for your patience and cooperation." +
                                            "\n\nBest Regards,\n\nCustomer Care team";
                                    }
                                }
                                SendMail(mail);
                            }
                        }
                        ncase.Status = caseRow.Status;
                        db.SaveChanges();
                    }
                }
            }
            return Json("The new case has been modified correctly!", JsonRequestBehavior.AllowGet);
        }

        private void SendMail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = true;
            client.Host = System.Configuration.ConfigurationManager.AppSettings["host"];
            client.Send(mail);
        }

        [HttpPost]
        public ActionResult NewCaseHistoryRow(UpdateCase caseRow)
        {
            List<UpdateCase> rows = new List<UpdateCase>();
            rows.Add(caseRow);
            return View(rows);
        }

        [HttpPost]
        public ActionResult Update(UpdateCase caseHistoryRow)
        {
            int casehId = 0;
            using (var db = new DptContext())
            {
                var chl = new DptCaseHistory();
                chl.CaseId = caseHistoryRow.CaseId;
                chl.CreatedOn = DateTime.Now;
                chl.Description = caseHistoryRow.Description;
                chl.Details = caseHistoryRow.Details;
                chl.CreatedBy = Membership.GetUser().UserName;
                db.CaseHistories.Add(chl);
                db.SaveChanges();
                casehId = db.CaseHistories.Local[0].CaseHistoryId;

                var origCase = from oc in db.Cases
                               where oc.CaseId == caseHistoryRow.CaseId
                               select oc;
                string destmail = null;
                if (origCase.Count() > 0)
                {
                    foreach (var oc in origCase)
                    {
                        if (!string.IsNullOrEmpty(oc.CCEngineer))
                        {
                            if (oc.CreatedBy == chl.CreatedBy)
                            {
                                destmail = oc.CCEngineer;
                                oc.Status = "Working";
                            }
                            else
                            {
                                oc.Status = "Waiting on Customer";
                                destmail = oc.CreatedBy;
                            }
                            if (!string.IsNullOrEmpty(destmail))
                            {
                                MailMessage mail = new MailMessage("is@dptcorporate.com", destmail);
                                mail.Bcc.Add(new MailAddress("Caseinteractions@think3.eu"));
                                if (oc.Language == "japanese")
                                {
                                    mail.Subject = "[このメールには返信しないでください] ご質問項目 #" + caseHistoryRow.CaseId + " が更新されました - " + caseHistoryRow.Description;
                                    mail.Body = "お客様。\nご質問項目 #" + caseHistoryRow.CaseId + " が更新されたことをお知らせいたします。\n\n" +
                                        "詳細：\n" + caseHistoryRow.Details + "\n\nご質問項目の詳細はこちらでご覧ください： http://dpt3.dptcorporate.com/" +
                                        " \n\nご質問項目に追記したり、ファイルを追加したりする場合は、Web サイトから直接お願いいたします。" +
                                        "このメールには返信しないでください。このメールに返信すると無効なメールアドレス（is@dptcorporate.com）へ返信されます。" +
                                        "\n\n以上、よろしくお願いいたします。\nシンクスリー・カスタマーケア";
                                }
                                else
                                {
                                    mail.Subject = "[DO NOT REPLY] Case #" + caseHistoryRow.CaseId + " has been updated - " + caseHistoryRow.Description;
                                    mail.Body = "Dear User, \n\nThe case #" + caseHistoryRow.CaseId + " status has changed.\n\n" +
                                        "Details: " + caseHistoryRow.Details + "\n\nYou can browse your cases at http://dpt3.dptcorporate.com/" +
                                        "\n\nBest Regards,\n\nCustomer Care team";
                                }
                                SendMail(mail);
                            }
                        }
                    }
                    db.SaveChanges();
                }
            }
            return Json(caseHistoryRow.CaseId + "_" + casehId, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult UploadHistoryFile(string caseRowID, string casehId, string filename, HttpPostedFileBase file, string submitButton)
        {
            if (submitButton == "close")
            {
                ViewBag.ok1 = "The new history case has been saved correctly!";
                return View("Success");
            }

            //string decodedfl = Encoding.Default.GetString(Convert.FromBase64String(filename));
            string decodedfl = GlobalObject.unescape(filename);

            //if (file != null && !(Path.GetExtension(file.FileName).Contains("zip")
            //    || Path.GetExtension(file.FileName).Contains("rar") /*||
            //    file.FileName.Split('.')[file.FileName.Split('.').Length - 1].Contains("7z")*/))
            //{
            //    ViewBag.ok1 = "Only the *.zip and *.rar file are accepted!";
            //    return View("Success");
            //}
            if (file != null)
            {
                using (var db = new DptContext())
                {
                    var casehID = System.Convert.ToInt64(casehId);
                    var query = from uhcase in db.CaseHistories
                                where uhcase.CaseHistoryId == casehID
                                select uhcase;
                    if (query.Count() > 0)
                    {

                        var basePath = "E:\\Case" + "\\" + caseRowID;
                        var folder = casehID;
                        var path = basePath + "\\" + folder;
                        foreach (DptCaseHistory ncase in query.ToList())
                        {
                            try
                            {
                                System.IO.Directory.CreateDirectory(path);
                                ncase.File = path + "\\" + decodedfl;
                                file.SaveAs(ncase.File);
                                db.SaveChanges();
                            }
                            catch (Exception e)
                            {
                                ViewBag.ok1 = "Cannot save the uploaded file!";
                                return View("Success");
                            }
                        }
                    }
                }
            }
            ViewBag.ok1 = "The new history case has been saved correctly!";
            return View("Success");
        }

        [HttpPost]
        public ActionResult CaseHistories(int caseId)
        {
            using (var db = new DptContext())
            {
                return View(db.CaseHistories.Where(c => c.CaseId == caseId).OrderByDescending(x => x.CaseHistoryId).ToList());
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

            return Json("The file is deleted correctly!", JsonRequestBehavior.AllowGet);
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

    }

}