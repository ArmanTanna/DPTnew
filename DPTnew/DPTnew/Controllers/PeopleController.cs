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
using System.Net;

namespace DPTnew.Controllers
{
    [Authorize(Roles = "Admin,Internal,VarExp,VarMed,Var")]
    public class PeopleController : BaseController
    {
        public ActionResult Index(int pageSize = 10)
        {
            //LocalizationHelper.SetLocalization(Session["CurrentCulture"]);
            ViewBag.UserRole = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal")
                || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp");
            return View();
        }

        [Authorize(Roles = "Admin,Internal,VarExp,VarMed,Var")]
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

        [Authorize(Roles = "Admin,Internal,VarExp,VarMed,Var")]
        [HttpPost]
        public ActionResult SinglePeopleRow(People pplSingleRow)
        {
            using (var db = new DptContext())
            {
                List<String> companyList = new List<string>();
                if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal"))
                    companyList = db.Companies.Select(u => u.AccountName + " \"" + u.AccountNumber + "\"").ToList();
                else
                {
                    var userName = Membership.GetUser().UserName;
                    var user = db.Contacts.Where(u => u.Email == userName).ToList().FirstOrDefault();
                    var company = db.Companies.Where(u => u.AccountNumber == user.AccountNumber).ToList().FirstOrDefault();
                    var salesRep = db.SalesR.Where(u => u.Invoicer == company.AccountName).Select(u => u.SalesRep).ToList();

                    if (salesRep.Count == 0)
                    {
                        var sR = db.SalesR.Where(u => u.AccountNumber == company.AccountNumber).Select(u => u.SalesRep).FirstOrDefault();
                        companyList.AddRange(db.Companies.Where(x => x.SalesRep == sR).OrderBy(k => k.AccountName).Select(u => u.AccountName + " \"" + u.AccountNumber + "\"").ToList());
                        companyList.Add(company.AccountName + " \"" + company.AccountNumber + "\"");
                    }
                    else
                        companyList.AddRange(db.Companies.Where(x => salesRep.Contains(x.SalesRep)).OrderBy(k => k.AccountName).Select(u => u.AccountName + " \"" + u.AccountNumber + "\"").ToList());
                }
                companyList.Sort();

                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(companyList).ToString(Formatting.None));
                ViewBag.Companies = System.Convert.ToBase64String(plainTextBytes);
            }
            ViewBag.UserRole = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal")
                || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp");
            List<People> rows = new List<People>();
            rows.Add(pplSingleRow);
            return View(rows);
        }

        [Authorize(Roles = "Admin,Internal,VarExp,VarMed,Var")]
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
                    try
                    {
                        MailHelper.SendMail(mail);
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteLog("PeopleController (Sendmail): " + e.Message + "-" + e.InnerException);
                    }
                    mail = SendMailInUserLang(pplSingleRow, db, mail);
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
                        LogHelper.WriteLog("PeopleController (Modify): " + e.Message);
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

        private MailMessage SendMailInUserLang(People pplSingleRow, DptContext db, MailMessage mail)
        {
            if (pplSingleRow.Language.ToLower() == "japanese")
            {
                var varmail = db.Companies.Where(x => x.AccountName == "t3 japan kk").FirstOrDefault().Email;
                var accName = db.Companies.Where(x => x.AccountNumber == pplSingleRow.AccountNumber).FirstOrDefault().AccountName;
                mail = new MailMessage("is@dptcorporate.com", pplSingleRow.Email);
                mail.Bcc.Add(new MailAddress(varmail));
                mail.Subject = "カスタマーケアへようこそ " + accName + " " + pplSingleRow.FirstName + " " +
                    pplSingleRow.LastName + " 様";
                mail.Body = "<b>" + accName + " " + pplSingleRow.FirstName + " " + pplSingleRow.LastName + "</b> 様<br/><br/>" +
                    "いつも大変お世話になっております。<br/><br/>" + "弊社カスタマーケアサイト：DPT3Care サイト（http://dpt3.dptcorporate.com/ ）へのユーザー登録が完了しましたので、お知らせいたします。"
                    + "<br/>ログイン情報は以下の通りです。" + "<br/><br/>ユーザー名：<b>" + pplSingleRow.Email +
                    "</b><br/><br/>パスワードは、ログインページの「Recover if you forgot password」ボタンより設定していただけます。" +
                    "<br/>詳細につきましては、下記「製品インストールガイド」の「２－６．よくあるお問い合わせ」をご参照ください。" +
                    "<br/><br/>製品インストールガイド：ftp://ftp.t3-japan.co.jp/tdExtra/InstallGuide/InstallGuide.pdf" +
                    "<br/><br/>技術的なご質問は、ログイン後「お問い合わせ」よりお送りいただくことができます。" +
                    "<br/><br/>以上、よろしくお願いいたします。<br/>シンクスリー・カスタマーケアチーム";
                mail.IsBodyHtml = true;
                try
                {
                    MailHelper.SendMail(mail);
                }
                catch (Exception e)
                {
                    LogHelper.WriteLog("PeopleController (Sendmail): " + e.Message + "-" + e.InnerException);
                }
            }
            if (pplSingleRow.Language.ToLower() == "korean")
            {
                var varmail = db.Companies.Where(x => x.AccountName == "t3 japan kk").FirstOrDefault().Email;
                var accName = db.Companies.Where(x => x.AccountNumber == pplSingleRow.AccountNumber).FirstOrDefault().AccountName;
                mail = new MailMessage("is@dptcorporate.com", pplSingleRow.Email);
                mail.Bcc.Add(new MailAddress(varmail));
                mail.Subject = "고객 센터에 오신 것을 환영 " + accName + " " + pplSingleRow.FirstName + " " +
                    pplSingleRow.LastName + " 님";
                mail.Body = "<b>" + accName + " " + pplSingleRow.FirstName + " " + pplSingleRow.LastName + "</b> 님<br/><br/>" +
                    "안녕하세요.<br/><br/>" + "당사 고객 지원 사이트 ：DPT3Care 사이트 （http://dpt3.dptcorporate.com/ ）에 사용자 등록이 완료되었으므로 알려드립니다."
                    + "<br/>로그인 정보는 다음과 같습니다." + "<br/><br/>사용자 이름：<b>" + pplSingleRow.Email +
                    "</b><br/><br/>비밀번호는 로그인 페이지의 'Recover if you forgot password'에서 " +
                    "<br/>이메일 주소 입력 후 'Recover Account'를 클릭하시면 비밀번호 설정 화면을 알리는 이메일이 전달됩니다." +
                    "<br/><br/>기술적인 질문은 로그인 후[사례관리]에서 보낼 수 있습니다." +
                    "<br/><br/>이상, 잘 부탁드립니다." +
                    "<br/>think3 고객 관리 팀";
                mail.IsBodyHtml = true;
                try
                {
                    MailHelper.SendMail(mail);
                }
                catch (Exception e)
                {
                    LogHelper.WriteLog("PeopleController (Sendmail): " + e.Message + "-" + e.InnerException);
                }
            }
            return mail;
        }

        [Authorize(Roles = "Admin,Internal,VarExp,VarMed,Var")]
        [HttpPost]
        public JsonResult GetPeopleRolesFromDB()
        {
            return Json(GetWebMainRoles(), JsonRequestBehavior.AllowGet);
        }

        [NonAction]
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Session["CurrentCulture"] = LocalizationHelper.SetLocalization(Session["CurrentCulture"]);

            base.OnActionExecuting(filterContext);
        }

    }

}