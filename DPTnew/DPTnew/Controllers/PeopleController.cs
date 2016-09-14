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
using System.Net.Mail;

namespace DPTnew.Controllers
{

    [Authorize(Roles = "Admin,VarExp,Internal")]
    public class PeopleController : BaseController
    {
        public ActionResult Index(int pageSize = 10)
        {
            LocalizationHelper.SetLocalization(Session["CurrentCulture"]);
            ViewBag.UserRole = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal");
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
            using (var db = new DptContext())
            {
                //var userName = Membership.GetUser().UserName;
                //var user = db.Contacts.Where(u => u.Email == userName).ToList().FirstOrDefault();
                //var company = db.Companies.Where(u => u.AccountNumber == user.AccountNumber).ToList().FirstOrDefault();
                //var salesRep = db.SalesR.Where(u => u.Invoicer == company.AccountName).Select(u => u.SalesRep).ToList();
                //List<String> companyList = new List<string>();
                //if (salesRep.Count == 0)
                //{
                //    var sR = db.SalesR.Where(u => u.AccountNumber == company.AccountNumber).Select(u => u.SalesRep).FirstOrDefault();
                //    companyList.AddRange(db.Companies.Where(x => x.SalesRep == sR).OrderBy(k => k.AccountName).Select(u => u.AccountName + " \"" + u.AccountNumber + "\"").ToList());
                //    companyList.Add(company.AccountName + " \"" + company.AccountNumber + "\"");
                //}
                //else
                //{
                //    foreach (var sr in salesRep)
                //        companyList.AddRange(db.Companies.Where(x => x.SalesRep == sr).OrderBy(k => k.AccountName).Select(u => u.AccountName + " \"" + u.AccountNumber + "\"").ToList());
                //}
                var companyList = db.Companies.Select(u => u.AccountName + " \"" + u.AccountNumber + "\"").ToList();
                companyList.Sort();

                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(companyList).ToString(Formatting.None));
                ViewBag.Companies = System.Convert.ToBase64String(plainTextBytes);
            }
            ViewBag.UserRole = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal");
            List<People> rows = new List<People>();
            rows.Add(pplSingleRow);
            return View(rows);
        }

        [Authorize(Roles = "Admin,VarExp,Internal")]
        [HttpPost]
        public JsonResult Modify(People pplSingleRow)
        {
            Regex email = new Regex(@"^(([0-9a-zA-Z_!#%'=`&\|~\-\*\+\?\^\$\{\}]+)(\.[0-9a-zA-Z0-9a-zA-Z_!#%'=`&\|~\-\*\+\?\^\$\{\}]+)*)@(([0-9a-zA-Z_!#%'=`&\|~\-\*\+\?\^\$\{\}]+)(\.[0-9a-zA-Z_!#%'=`&\|~\-\*\+\?\^\$\{\}]+)+)$");
            if (string.IsNullOrEmpty(pplSingleRow.Email) || (!email.IsMatch(pplSingleRow.Email)))
                return Json("Invalid mail", JsonRequestBehavior.AllowGet);

            using (var db = new DptContext())
            {
                if (pplSingleRow.UserId == 0)
                {
                    db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[DPT_People] (Email, OriginalPassword, FirstName, LastName, " +
                    "Language, Status, EmailStatus, AccountNumber, PrimaryContact, RoleName, FirstNameK, LastNameK) VALUES ('" +
                    pplSingleRow.Email + "','test','" + pplSingleRow.FirstName + "','" + pplSingleRow.LastName + "','" +
                    pplSingleRow.Language + "','" + pplSingleRow.Status + "','" + pplSingleRow.EmailStatus + "','" + pplSingleRow.AccountNumber
                    + "','" + pplSingleRow.PrimaryContact + "','user','" + pplSingleRow.FirstNameK + "','" +
                    pplSingleRow.LastNameK + "');");

                    pplSingleRow.UserId = db.Peoples.Max(u => u.UserId);
                    MailMessage mail = new MailMessage("is@dptcorporate.com", "dpt@dptcorporate.com");
                    mail.Subject = "[DO NOT REPLY] Created new User";
                    mail.Body = "Dear " + pplSingleRow.FirstName + " " + pplSingleRow.LastName +
                        ",\n\na new user has been created with the following credentials:\n\n" +
                        "Username: " + pplSingleRow.Email + "\nPassword: test" +
                        "\n\nYou can now access DPT3Care website http://dpt3.dptcorporate.com/ " + "by using your credentials." +
                        "\n\nBest Regards,\n\nCustomer Care team";
                    SendMail(mail);
                }
                else
                {
                    try
                    {
                        var query =
                    from ppl in db.Peoples
                    where ppl.UserId == pplSingleRow.UserId
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

                PeopleRoles pplr = new PeopleRoles();
                pplr.UserId = pplSingleRow.UserId;
                pplr.RoleId = pplSingleRow.RoleId;
                var qry = from ppl in db.PeopleRoles
                          where ppl.UserId == pplr.UserId
                          select ppl;

                if (pplSingleRow.RoleId > 0)
                {
                    if (qry.Count() > 0)
                        db.PeopleRoles.Remove(qry.FirstOrDefault());
                    db.PeopleRoles.Add(pplr);
                }
                else
                {
                    if (qry.Count() > 0)
                        db.PeopleRoles.Remove(qry.FirstOrDefault());
                }
                db.SaveChanges();
            }
            return Json("Saved User: " + pplSingleRow.UserId, JsonRequestBehavior.AllowGet);
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

        [Authorize(Roles = "Admin,VarExp,Internal")]
        [HttpPost]
        public JsonResult GetPeopleRolesFromDB()
        {
            return Json(GetWebMainRoles(), JsonRequestBehavior.AllowGet);
        }

    }

}