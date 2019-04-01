using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPTnew.Models;
using System.Web.Security;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using DPTnew.Helper;
using System.Net.Mail;
using WebMatrix.WebData;

namespace DPTnew.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private async Task<HttpResponseMessage> SendJsonAsync(string uri, string body)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                StringContent stringContent = new StringContent(body, UnicodeEncoding.UTF8, "application/json");

                HttpResponseMessage responseMessage = new HttpResponseMessage();
                try
                {
                    responseMessage = await httpClient.PostAsync(uri, stringContent);
                }
                catch (Exception ex)
                {
                    LogHelper.WriteLog("UserController (SendJsonAsync): " + ex.Message + "-" + ex.InnerException.Message);
                    responseMessage.StatusCode = HttpStatusCode.InternalServerError;
                    //    responseMessage.ReasonPhrase = string.Format("RestHttpClient.SendRequest failed: {0}", ex);
                }
                return responseMessage;
            }
        }

        //private Utility functions
        private string fileToString(HttpPostedFileBase file)
        {
            var totalfile = "";
            using (System.IO.StreamReader reader = new System.IO.StreamReader(file.InputStream))
            {
                totalfile = reader.ReadToEnd();
            }
            return totalfile;
        }

        private string encodeC2V(string c2v)
        {
            string[] separatingChars = { "?>\n" };
            string[] words = c2v.Split(separatingChars, System.StringSplitOptions.RemoveEmptyEntries);
            if (words.Length != 2)
            {
                string[] separatingChars2 = { "?>\r\n" };
                words = c2v.Split(separatingChars2, System.StringSplitOptions.RemoveEmptyEntries);
            }

            if (words.Length != 2)
            {
                throw new Exception("Incorrect .C2V");
            }
            string base64Decoded = words[1];
            base64Decoded = base64Decoded.TrimEnd('\n');
            base64Decoded = base64Decoded.TrimEnd('\r');
            string base64Encoded;
            byte[] data = System.Text.Encoding.UTF8.GetBytes(base64Decoded);
            base64Encoded = System.Convert.ToBase64String(data);
            return base64Encoded;
        }

        // GET: /Licenses/
        public ActionResult Licenses()
        {
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal") ||
                Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarMed") ||
                Roles.IsUserInRole(WebSecurity.CurrentUserName, "Var") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "UserNoLic"))
            {
                return View();
            }
            var licenses = GetLicenses();
            if (licenses.Count() > 0)
            {
                ViewBag.AccountNumber = licenses.FirstOrDefault().AccountNumber;
                ViewBag.AccountName = licenses.FirstOrDefault().AccountName;
                ViewBag.AccountStatus = licenses.FirstOrDefault().AccountStatus;
                //ViewBag.Licenses = Uri.EscapeDataString((new JavaScriptSerializer()).Serialize(licenses));
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes((new JavaScriptSerializer()).Serialize(licenses));
                ViewBag.Licenses = System.Convert.ToBase64String(plainTextBytes);
            }
            return View();
        }

        [HttpPost]
        public ActionResult Export(LicenseBase license)
        {
            return View(license);
        }

        [HttpPost]
        public ActionResult Create(Entitlement license)
        {
            if (ModelState.IsValid)
            { }

            return View(license);
        }

        [HttpPost]
        public ActionResult Validate(string licenseid)
        {
            ViewBag.LicenseId = licenseid;
            return View();
        }

        [HttpPost]
        public ActionResult V2CP(string licenseid)
        {
            ViewBag.LicenseId = licenseid;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ExportLicense(LicenseBase l)
        {
            LicenseView currentlicense = null;
            SerLicenseFlag lf = null;
            string dpt_Company;
            string var;
            int block;
            using (var context = new DptContext())
            {
                currentlicense = context.Licenses.SingleOrDefault(u => u.LicenseID == l.LicenseID);
                dpt_Company = currentlicense.AccountNumber;
                lf = context.LicFlag.SingleOrDefault(x => x.LicenseFlag == currentlicense.LicenseFlag);
                var company = context.Companies.SingleOrDefault(x => x.AccountNumber == dpt_Company);
                var salesRep = context.SalesR.SingleOrDefault(y => y.SalesRep == company.SalesRep);
                var = context.Companies.SingleOrDefault(z => z.AccountNumber == salesRep.AccountNumber).AccountNumber;
                block = context.SpecialCompanies.Where(c => c.Description == "BLOCKED").Select(c => c.AccountNumber).ToList().Contains(dpt_Company) ? 1:0;
            }

            if (currentlicense != null && currentlicense.MaxExport > 0 && currentlicense.ExportedNum >= currentlicense.MaxExport)
            {
                ModelState.AddModelError("EXPORT", "Maximum number of export is reached. It's impossible to export the license.");
                return View("Export", l);
            }

            if (currentlicense != null && Convert.ToInt64(currentlicense.Version) > 2014 && block == 0)
            {
                var now = System.DateTime.Now;
                Regex licensergx = new Regex(@"^KID[0-9]+$");
                Regex evalrgx = new Regex(@"^EVA");

                var isLocal = licensergx.IsMatch(currentlicense.MachineID);// blurgx.IsMatch(currentlicense.MachineID);
                var isTdVar = currentlicense.PwdCode.StartsWith("VA");
                var isTdirect = /*currentlicense.PwdCode.StartsWith("IX") || */currentlicense.PwdCode.StartsWith("IK") ||
                    currentlicense.PwdCode.StartsWith("XP") || currentlicense.PwdCode.StartsWith("IJ");

                //check for export
                if (currentlicense.Installed == 1 && currentlicense.MaintEndDate >= now)
                {
                    if (isLocal && (!isTdVar || currentlicense.LicenseID == "L00000387" || currentlicense.LicenseID == "L00000699")
                        && !isTdirect && lf.Export_Safenet == 1)
                    {
                        SafenetUpdateEntitlment ue = new SafenetUpdateEntitlment();

                        ue.CrmId = dpt_Company;
                        ue.EntType = "PROTECTIONKEY_UPDATE";
                        ue.ProtectionKeyId = currentlicense.MachineID.Remove(0, 3);
                        var rem = 4;
                        while (ue.ProtectionKeyId.StartsWith("0"))
                        {
                            ue.ProtectionKeyId = currentlicense.MachineID.Remove(0, rem);
                            rem++;
                        }
                        ue.refId1 = ue.ProtectionKeyId;
                        ue.refId2 = currentlicense.LicenseID;
                        var pname = GetProductName(currentlicense.ProductName);
                        var prodName = InitSafenetProduct(currentlicense.PwdCode, pname, "_20181CANCEL", var);
                        ue.ProductName = InitSafenetProduct(currentlicense.PwdCode, pname, "_20181CANCEL", var);
                        IList<string> verList = new List<string>();
                        verList.Add("_20171CANCEL");
                        verList.Add("_20161CANCEL");
                        verList.Add("_20152CANCEL");

                        if (currentlicense.ProductName.ToLower() != "tdprofessionaledu")
                        {
                            foreach (var ver in verList)
                            {
                                foreach (var it in prodName.ToList())
                                {
                                    var repName = it.ToString();
                                    repName = repName.Replace("_20181CANCEL", ver);
                                    ue.ProductName.Add(repName);
                                }
                            }
                        }

                        verList = new List<string>();
                        verList.Add("_20191CANCEL");
                        foreach (var ver in verList)
                        {
                            foreach (var it in prodName.ToList())
                            {
                                var repName = it.ToString();
                                repName = repName.Replace("_20181CANCEL", ver);
                                ue.ProductName.Add(repName);
                            }
                        }

                        ue.Encoded = true;
                        ue.C2V = "";

                        string input = JsonConvert.SerializeObject(ue);

                        string uri = Url.Action("CreateCompleteLicense", "Safenet", new { httproute = "" }, "https");

                        HttpResponseMessage response = await SendJsonAsync(uri, input);

                        if (response.IsSuccessStatusCode)
                        {
                            using (var context = new DptContext())
                            {
                                //update state in db
                                currentlicense.Installed = 0;
                                currentlicense.Exported = 1;
                                currentlicense.ExportedNum = currentlicense.ExportedNum + 1;
                                currentlicense.TotExported += 1;
                                context.Licenses.Attach(currentlicense);
                                var entry = context.Entry(currentlicense);
                                entry.Property(x => x.Installed).IsModified = true;
                                entry.Property(x => x.Exported).IsModified = true;
                                entry.Property(x => x.ExportedNum).IsModified = true;
                                entry.Property(x => x.TotExported).IsModified = true;
                                //context.Entry(currentlicense).State = EntityState.Modified;
                                context.SaveChanges();

                                DptLicenseLog log = new DptLicenseLog();
                                log.LicenseID = currentlicense.LicenseID;
                                log.MachineID = currentlicense.MachineID;
                                log.Action = "Export";
                                log.CreatedOn = DateTime.Now;
                                log.CreatedBy = Membership.GetUser().UserName;
                                log.VersionFrom = currentlicense.Version;
                                context.LicenseLogs.Add(log);
                                context.SaveChanges();
                            }
                            var company = from cmp in _db.Companies where cmp.AccountNumber == dpt_Company select cmp;
                            var salesRep = from salrep in _db.SalesR where salrep.SalesRep == company.FirstOrDefault().SalesRep select salrep;
                            var sr = from cmp in _db.Companies where cmp.AccountNumber == salesRep.FirstOrDefault().AccountNumber select cmp;
                            MailMessage mail = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["hostusername"], company.FirstOrDefault().Email);
                            mail.CC.Add(sr.FirstOrDefault().Email);
                            mail.Bcc.Add("Orders@dptcorporate.com");
                            if (company.FirstOrDefault().Language.ToLower() == "japanese")
                            {
                                mail.CC.Add(_db.Companies.Where(x => x.AccountName == "t3 japan kk").FirstOrDefault().Email);
                                mail.Subject = "[このメールには返信しないでください] " + company.FirstOrDefault().AccountName + " (" + company.FirstOrDefault().AccountNumber + ") 様、ライセンスが発行されました";
                                mail.Body = "<pre>DPT User 様。<br/>ライセンスのエクスポート操作が開始されました。<br/>以下の手順を実行して、エクスポート操作を完了してください。" +
                                    "<br/><br/>1. <b>Safenet Admin Control Center</b>（ http://localhost:1947 ）<b>のアップデート／アタッチ</b> セクションで、受け取った .v2c ファイルを適用 <br/>" +
                                    "2. エクスポート検証：<br/>  2a) <b>Admin Control Center</b>（ http://localhost:1947 ）→ <b>Sentinel キー</b> → C2V ボタン（アクション欄） → C2V ファイルのダウンロード<br/>" +
                                    "  2b) <b>DPT3Care</b>（ https://dpt3.dptcorporate.com ）サイトへログインして Licenses ページを表示 → エクスポートするライセンスの行を選択 → <b>エクスポート検証</b> ボタン → 前の手順で作成した .c2v ファイルをアップロード<br/>" +
                                    "3. これでエクスポート操作が完了し、ライセンスを別のＰＣにインストールすることができます。<br/><br/><br/>" +
                                    "以下にライセンスの詳細を記載いたします。<br/><br/>ライセンスID: " + currentlicense.LicenseID + " (" +
                                    currentlicense.LicenseFlag.Substring(0, 3).ToUpper() + ")<br/>マシンＩＤ: " +
                                    currentlicense.MachineID + "<br/>製品: " + currentlicense.ProductName + "<br/>バージョン: " + currentlicense.Version +
                                    "<br/>終了日: " + (currentlicense.ArticleDetail.ToLower() == "pl" ? "pl" : currentlicense.MED) +
                                    "<br/><br/>以上、よろしくお願いいたします。<br/>DPT Services</pre>";
                            }
                            else
                            {
                                mail.Subject = "[DO NOT REPLY] License issued for " + company.FirstOrDefault().AccountName + " (" + company.FirstOrDefault().AccountNumber + ") ";
                                mail.Body = "<pre>Dear User, <br/><br/>You are exporting your license.<br/><br/>Please follow these steps to complete the Export operation:" +
                                    "<br/><br/>1. Insert the .v2c file you received in the <b>Update/Attach</b> section of the <b>Safenet Admin Control Center</b> (http://localhost:1947)." +
                                    "<br/><br/>2. Validate Export:<br/><br/>  2a. <b>Admin Control Center</b> (http://localhost:1947) → <b>Sentinel keys</b> → C2V button (Action column) → download the .c2v file." +
                                    "<br/><br/>  2b. <b>DPT3Care</b> (https://dpt3.dptcorporate.com - refresh the Licenses page) → click on the license’s row → <b>Validate Export button</b> → upload the downloaded .c2v file." +
                                    "<br/><br/>3. You are now ready to reinstall your license on another PC." +
                                    "<br/><br/><br/>Here below you'll find more details:<br/><br/>LicenseID: " + currentlicense.LicenseID + " (" +
                                    currentlicense.LicenseFlag.Substring(0, 3).ToUpper() + ")<br/>MachineID: " + currentlicense.MachineID +
                                    "<br/>Product: " + currentlicense.ProductName + "<br/>Version: " + currentlicense.Version +
                                    //".\n\nYou can browse the licenses of the companies managed by you at https://dpt3.dptcorporate.com/License" +
                                    "<br/><br/>Best regards,<br/><br/>DPT Services</pre>";
                            }
                            mail.IsBodyHtml = true;
                            try
                            {
                                MailHelper.SendMail(mail);
                            }
                            catch (Exception e)
                            {
                                LogHelper.WriteLog("UserController (ExportLicense): " + e.Message + "-" + e.InnerException);
                            }

                            ViewBag.ok1 = DPTnew.Localization.Resource.LicenseExportMsg;
                            ViewBag.ok2 = DPTnew.Localization.Resource.LicenseMailMsg + ": " + company.FirstOrDefault().Email;
                            return View("Success");
                        }
                        else
                        {
                            ModelState.AddModelError("EXPORT", "Something went wrong. It's impossible to export the license.");
                            return View("Export", l);
                        }
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("EXPORT", "You can't export this license. Contact think3 Customer Care");
            return View("Export", l);
        }

        [HttpPost]
        public async Task<ActionResult> ValidateC2V(string Licenseid, HttpPostedFileBase file)
        {
            var success = false;
            LicenseView currentlicense = null;

            if (file != null)
            {
                using (var context = new DptContext())
                {
                    currentlicense = context.Licenses.SingleOrDefault(u => u.LicenseID == Licenseid);
                }

                if (currentlicense.Exported == 0)
                {
                    ModelState.AddModelError("VALIDATE", "You haven't exported this license.");
                    return View("Validate");
                }
                string filestring = fileToString(file);
                string c2v;
                try
                {
                    c2v = encodeC2V(filestring);
                }
                catch (Exception e)
                {
                    LogHelper.WriteLog("UserController (ValidateC2V): " + e.Message);
                    ModelState.AddModelError("VALIDATE", "The file you have uploaded is not correct");
                    return View("Validate");
                }
                //build of JSON
                JObject o = new JObject();
                o["Encoded"] = true;
                o["C2V"] = c2v;

                string uri = Url.Action("CheckInC2V", "Safenet", new { httproute = "" }, "https");

                HttpResponseMessage response = await SendJsonAsync(uri, o.ToString());

                if (response.IsSuccessStatusCode)
                {
                    string content = response.Content.ReadAsStringAsync().Result;
                    JObject contentresult = JObject.Parse(content);

                    var currentpkey = currentlicense.MachineID.Remove(0, 3);
                    var rem = 4;
                    while (currentpkey.StartsWith("0"))
                    {
                        currentpkey = currentlicense.MachineID.Remove(0, rem);
                        rem++;
                    }

                    string pkey = (string)contentresult["ProtectionKey"]["ProtectionKeyOutput"]["C2V"]["sentinel_ldk_info"]["key"]["id"];

                    //if c2v is related to the correct key
                    if (currentpkey == pkey)
                    {
                        JObject key = (JObject)contentresult["ProtectionKey"]["ProtectionKeyOutput"]["C2V"]["sentinel_ldk_info"]["key"];

                        var p = key.Property("product");
                        //check if there are products 
                        if (p == null) success = true;
                        else
                        {
                            JToken products = (JToken)contentresult["ProtectionKey"]["ProtectionKeyOutput"]["C2V"]["sentinel_ldk_info"]["key"]["product"];
                            //check if product is single
                            if (products is JObject)
                            {
                                JObject singleproduct = (JObject)products;
                                //product is the same and has no feature (the features have been deleted)
                                if ((singleproduct["name"].ToString().Trim().ToLower() == currentlicense.ProductName && singleproduct.Property("feature") == null) || singleproduct["name"].ToString().Trim().ToLower() != currentlicense.ProductName)
                                {
                                    //correct
                                    success = true;
                                }
                            }
                            //multiple products 
                            else if (products is JArray && !products.Any(child => child["feature"] != null && child["name"].ToString().Trim().ToLower() == currentlicense.ProductName))
                                success = true;
                        }
                    }
                }
            }

            if (success)
            {
                using (var context = new DptContext())
                {
                    DptLicenseLog log = new DptLicenseLog();
                    log.LicenseID = currentlicense.LicenseID;
                    log.MachineID = currentlicense.MachineID;
                    log.Action = "Validate";
                    log.CreatedOn = DateTime.Now;
                    log.CreatedBy = Membership.GetUser().UserName;
                    log.C2VFileName = file.FileName;
                    log.VersionFrom = currentlicense.Version;
                    context.LicenseLogs.Add(log);
                    context.SaveChanges();

                    //update state in db
                    currentlicense.Exported = 0;
                    currentlicense.Import = 1;
                    currentlicense.Ancestor = currentlicense.MachineID;
                    currentlicense.MachineID = "KIDABCDEFGH";

                    context.Licenses.Attach(currentlicense);
                    var entry = context.Entry(currentlicense);
                    entry.Property(x => x.Exported).IsModified = true;
                    entry.Property(x => x.Import).IsModified = true;
                    entry.Property(x => x.MachineID).IsModified = true;
                    entry.Property(x => x.Ancestor).IsModified = true;

                    context.SaveChanges();
                }

                ViewBag.ok1 = DPTnew.Localization.Resource.LicenseValidateMsg1;
                ViewBag.ok2 = DPTnew.Localization.Resource.LicenseValidateMsg2;
                return View("Success");
            }
            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("VALIDATE", "The file you have uploaded is not correct.");
            return View("Validate");
        }

        [HttpPost]
        public async Task<ActionResult> GetV2CP(string Licenseid, HttpPostedFileBase file)
        {
            var success = false;
            LicenseView currentlicense = null;
            int block;

            if (file != null)
            {
                using (var context = new DptContext())
                {
                    currentlicense = context.Licenses.SingleOrDefault(u => u.LicenseID == Licenseid);
                    block = context.SpecialCompanies.Where(c => c.Description == "BLOCKED").Select(c => c.AccountNumber).ToList().Contains(currentlicense.AccountNumber) ? 1 : 0;
                }

                if ((currentlicense.Installed == 0 && currentlicense.Exported == 0) || block == 1 ||
                    currentlicense.MaintEndDate < DateTime.Now)
                {
                    ModelState.AddModelError("V2CP", "You haven't installed this license.");
                    return View("V2CP");
                }

                string filestring = fileToString(file);
                string c2v;
                try
                {
                    c2v = encodeC2V(filestring);
                }
                catch (Exception e)
                {
                    LogHelper.WriteLog("UserController (GetV2CP): " + e.Message);
                    ModelState.AddModelError("V2CP", "The file you have uploaded is not correct");
                    return View("V2CP");
                }
                //build of JSON
                JObject o = new JObject();
                o["Encoded"] = true;
                o["C2V"] = c2v;

                string uri = Url.Action("RetrieveV2CP", "Safenet", new { httproute = "" }, "https");

                HttpResponseMessage response = await SendJsonAsync(uri, o.ToString());

                if (response.IsSuccessStatusCode)
                {
                    string content = response.Content.ReadAsStringAsync().Result;
                    JObject contentresult = JObject.Parse(content);
                    contentresult.Property("Headers").Remove();
                    contentresult.Property("Success").Remove();
                    var doc = JsonConvert.DeserializeXmlNode(contentresult.ToString());
                    var ms = new MemoryStream();
                    doc.Save(ms);
                    ms.Flush();
                    ms.Position = 0;

                    var company = from cmp in _db.Companies where cmp.AccountNumber == currentlicense.AccountNumber select cmp;
                    var salesRep = from salrep in _db.SalesR where salrep.SalesRep == company.FirstOrDefault().SalesRep select salrep;
                    var sr = from cmp in _db.Companies where cmp.AccountNumber == salesRep.FirstOrDefault().AccountNumber select cmp;
                    MailMessage mail = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["hostusername"], company.FirstOrDefault().Email);
                    mail.CC.Add(sr.FirstOrDefault().Email);
                    mail.Bcc.Add("licensing@dptcorporate.com");

                    if (company.FirstOrDefault().Language.ToLower() == "japanese")
                    {
                        mail.CC.Add(_db.Companies.Where(x => x.AccountName == "t3 japan kk").FirstOrDefault().Email);
                        mail.Subject = "[このメールには返信しないでください] " + company.FirstOrDefault().AccountName + " (" + company.FirstOrDefault().AccountNumber + ") 様、v2cp ファイルが発行されました";
                        mail.Body = "<pre>DPT User 様。<br/><br/>添付の .v2cp パスワードファイルをご確認ください。" +
                            "<br/>以下にライセンスの詳細を記載いたします。<br/><br/>ライセンスＩＤ: " + currentlicense.LicenseID + " (" +
                            currentlicense.LicenseFlag.Substring(0, 3).ToUpper() + ")<br/>マシンＩＤ: " + currentlicense.MachineID +
                            "<br/>製品: " + currentlicense.ProductName + "<br/>バージョン: " + currentlicense.Version +
                            "<br/>終了日: " + (currentlicense.ArticleDetail.ToLower() == "pl" ? "pl" : currentlicense.MED) +
                            //".\n\nYou can browse the licenses of the companies managed by you at https://dpt3.dptcorporate.com/License" +
                            "<br/><br/>以上、よろしくお願いいたします。<br/><br/>DPT Services</pre>";
                    }
                    else
                    {
                        mail.Subject = "[DO NOT REPLY] v2cp license issued for " + company.FirstOrDefault().AccountName + " (" + company.FirstOrDefault().AccountNumber + ") ";
                        mail.Body = "<pre>Dear User, <br/><br/>Please find attached the requested .v2cp password file." +
                            "<br/>Here below you'll find more details:<br/><br/>LicenseID: " + currentlicense.LicenseID + " (" +
                            currentlicense.LicenseFlag.Substring(0, 3).ToUpper() + ")<br/>MachineID: " + currentlicense.MachineID +
                            "<br/>Product: " + currentlicense.ProductName + "<br/>Version: " + currentlicense.Version +
                            "<br/>Expiration Date: " + (currentlicense.ArticleDetail.ToLower() == "pl" ? "pl" : currentlicense.MED) +
                            //".\n\nYou can browse the licenses of the companies managed by you at https://dpt3.dptcorporate.com/License" +
                            "<br/><br/>Best regards,<br/><br/>DPT Services</pre>";
                    }
                    mail.Attachments.Add(new Attachment(ms, currentlicense.LicenseID + "-" + currentlicense.MachineID + ".v2cp", "application/xml"));
                    mail.IsBodyHtml = true;
                    try
                    {
                        MailHelper.SendMail(mail);
                        success = true;
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteLog("UserController (GetV2CP): " + e.Message + "-" + e.InnerException);
                    }
                    if (success)
                    {
                        var res = DPTnew.Localization.Resource.LicenseMailMsg + ": " + company.FirstOrDefault().Email;
                        ViewBag.ok1 = res.Replace(".v2c", ".v2cp");
                        return View("Success");
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("V2CP", "The file you have uploaded is not correct.");
            return View("V2CP");
        }

        private JArray InitSafenetProduct(string pwdCode, string productName, string productPostfix, string accNumber)
        {
            var prodName = new JArray();
            if (pwdCode.StartsWith("VA"))//tdvar accNumber == "T3-0073628" (Enrico), TDVARFull
                prodName = new JArray(SafenetEntitlement.DPTVARBundle.Select(x => x + productPostfix).ToArray());
            else if (productName == "tdvarlight")
                prodName = new JArray(SafenetEntitlement.TDVARBundle.Select(x => x + productPostfix).ToArray());
            else
                if (pwdCode.StartsWith("IX"))//tdirectrw
                    prodName = new JArray(SafenetEntitlement.TDIRECTBundle.Select(x => x + productPostfix).ToArray());

            //else if (pwdCode.StartsWith("AH")) //tdxchangereader
                //{
                //    prodName = new JArray(SafenetEntitlement.TDIRECTBundle.Select(x => x + productPostfix).ToArray());
                //    prodName.Add(productName + productPostfix);
                //}
                else
                {
                    if (productName == "thinkapigsm")
                        prodName.Add("thinkapi_gsm" + productPostfix);
                    else
                        prodName.Add(productName + productPostfix);
                    if (SafenetEntitlement.AddTTeamDocTo.Contains(pwdCode.Substring(0, 2)))
                    {
                        prodName.Add("ThinkTeamDOC" + productPostfix);
                        prodName.Add("tdpartsolutions" + productPostfix);
                    }
                    //sener
                    if (accNumber == "T3-0032632" && (productName == "tdprofessional" || productName == "tdengineering"))
                        prodName.Add("thinkcore" + productPostfix);
                }

            return prodName;
        }

        [HttpGet]
        [ActionName("UpgradeLicense")]
        public async Task<JsonResult> UpgradeLicense(string licenseId, string version, int renew)//upgrade/renew
        {
            //common variables
            LicenseView currentlicense = null;
            SerLicenseFlag lf = null;
            string type = "";
            string dpt_Company = "";
            string input = "";
            string var = "";
            int block;

            using (var context = new DptContext())
            {
                currentlicense = context.Licenses.SingleOrDefault(u => u.LicenseID == licenseId);
                dpt_Company = currentlicense.AccountNumber;
                lf = context.LicFlag.SingleOrDefault(x => x.LicenseFlag == currentlicense.LicenseFlag);
                var company = context.Companies.SingleOrDefault(x => x.AccountNumber == dpt_Company);
                var salesRep = context.SalesR.SingleOrDefault(y => y.SalesRep == company.SalesRep);
                var = context.Companies.SingleOrDefault(z => z.AccountNumber == salesRep.AccountNumber).AccountNumber;
                block = context.SpecialCompanies.Where(c => c.Description == "BLOCKED").Select(c => c.AccountNumber).ToList().Contains(dpt_Company) ? 1 : 0;
            }

            if (currentlicense != null && Convert.ToInt64(currentlicense.Version) > 2014 && block == 0 &&
                ((Convert.ToInt64(version) != Convert.ToInt64(currentlicense.Version))
                || (renew > 0 && currentlicense.ArticleDetail.ToLower() != "pl")))
            {
                var now = System.DateTime.Now;
                Regex licensergx = new Regex(@"^KID[0-9]+$");
                Regex evalrgx = new Regex(@"^EVA");
                Regex blurgx = new Regex(@"^BLU[0-9]+$");
                Regex redrgx = new Regex(@"^RED[0-9]+$");
                Regex blkrgx = new Regex(@"^BLK[0-9]+$");

                var isLocal = licensergx.IsMatch(currentlicense.MachineID);// blurgx.IsMatch(currentlicense.MachineID);
                var isBlu = blurgx.IsMatch(currentlicense.MachineID);
                var isred = redrgx.IsMatch(currentlicense.MachineID);
                var isblk = blkrgx.IsMatch(currentlicense.MachineID);
                var isEval = evalrgx.IsMatch(currentlicense.LicenseFlag.ToUpper());

                if ((currentlicense.MaintEndDate >= now)
                    && ((lf.Renewal_Safenet == 1 && (isLocal || isred || isblk) && renew > 0 && currentlicense.ArticleDetail.ToLower() != "pl")
                    || ((currentlicense.Installed == 1 && (isBlu || (lf.ChangeVersion_Safenet == 1))) /*&& currentlicense.LicenseType.ToLower() != "floating"*/)))
                {
                    if (currentlicense.LicenseType == "local")
                    { //LOCAL
                        if (currentlicense.ArticleDetail == "pl")
                        {
                            type = "PL";
                        }
                        else
                        {
                            //if (isEval) { type = "EVAL"; }
                            //else
                            //{
                            if (currentlicense.MachineID.ToUpper().Contains("BLK"))
                                type = "PHYSIC_EXPIR";
                            else
                                type = "EXPIR";
                            //}
                        }
                    }
                    else
                    {//FLOATING
                        if (currentlicense.ArticleDetail == "pl")
                        {
                            type = "NET_PL";
                        }
                        else
                        {
                            type = "NET_EXPIR";
                        }
                    }

                    //building product
                    string productPostfix = "";
                    if (version == "2015")
                        productPostfix = "_" + version + "2" + type;
                    else
                        productPostfix = "_" + version + "1" + type;

                    var pname = GetProductName(currentlicense.ProductName);
                    //EVAL or LOCAL PL
                    if (type == "PL" /*|| type == "EVAL"*/)
                    {
                        SafenetEvalPlLocalEntitlment e1 = new SafenetEvalPlLocalEntitlment();
                        e1.CrmId = dpt_Company;
                        e1.EntType = "PROTECTIONKEY_UPDATE";
                        e1.ProtectionKeyId = currentlicense.MachineID.Remove(0, 3);
                        var rem = 4;
                        while (e1.ProtectionKeyId.StartsWith("0"))
                        {
                            e1.ProtectionKeyId = currentlicense.MachineID.Remove(0, rem);
                            rem++;
                        }
                        e1.refId1 = e1.ProtectionKeyId;
                        e1.refId2 = currentlicense.LicenseID;
                        //ADD PRODUCT
                        e1.ProductName = InitSafenetProduct(currentlicense.PwdCode, pname, productPostfix, var);
                        if (currentlicense.LicenseType.ToLower() == "floating" && renew == 0)
                        {
                            if (currentlicense.Version == "2015")
                                productPostfix = "_20152CANCEL";
                            else
                                productPostfix = "_" + currentlicense.Version + "1CANCEL";
                            var productName = InitSafenetProduct(currentlicense.PwdCode, pname, productPostfix, var);
                            foreach (var p in productName)
                                e1.ProductName.Add(p);
                        }

                        e1.Encoded = true;
                        e1.C2V = "";
                        input = JsonConvert.SerializeObject(e1);
                    }
                    else
                    {
                        SafenetNetAsfLocalEntitlment e2 = new SafenetNetAsfLocalEntitlment();

                        e2.CrmId = dpt_Company;
                        e2.EntType = "PROTECTIONKEY_UPDATE";
                        e2.ProtectionKeyId = currentlicense.MachineID.Remove(0, 3);
                        var rem = 4;
                        while (e2.ProtectionKeyId.StartsWith("0"))
                        {
                            e2.ProtectionKeyId = currentlicense.MachineID.Remove(0, rem);
                            rem++;
                        }
                        e2.refId1 = e2.ProtectionKeyId;
                        e2.refId2 = currentlicense.LicenseID;
                        //ADD PRODUCT
                        e2.ProductName = InitSafenetProduct(currentlicense.PwdCode, pname, productPostfix, var);
                        if (currentlicense.LicenseType.ToLower() == "floating" && renew == 0)
                        {
                            if (currentlicense.Version == "2015")
                                productPostfix = "_20152CANCEL";
                            else
                                productPostfix = "_" + currentlicense.Version + "1CANCEL";
                            var productName = InitSafenetProduct(currentlicense.PwdCode, pname, productPostfix, var);
                            foreach (var p in productName)
                                e2.ProductName.Add(p);
                        }

                        //ADDITIONAL PARAMETERS
                        e2.SaotParams = new JArray();

                        foreach (string pn in e2.ProductName)
                        {
                            JObject temp = new JObject();
                            temp["Product"] = pn;

                            //choose product
                            if (type == "EXPIR" || type == "NET_EXPIR" || type == "PHYSIC_EXPIR")
                            {
                                DateTime med = new DateTime();
                                med = (System.DateTime)currentlicense.MaintEndDate;
                                temp["EXPIRATION_DATE"] = med.Date.ToString("yyyy-MM-dd");
                            }
                            if (type == "NET_PL" || type == "NET_EXPIR")
                            {
                                temp["CONCURRENT_INSTANCES"] = currentlicense.Quantity.ToString();
                            }
                            e2.SaotParams.Add(temp);
                        }

                        e2.Encoded = true;
                        e2.C2V = "";

                        input = JsonConvert.SerializeObject(e2);
                    }

                    string uri = Url.Action("CreateCompleteLicense", "Safenet", new { httproute = "" }, "https");

                    HttpResponseMessage response = await SendJsonAsync(uri, input);

                    if (response.IsSuccessStatusCode)
                    {
                        using (var context = new DptContext())
                        {
                            DptLicenseLog log = new DptLicenseLog();
                            context.Licenses.Attach(currentlicense);
                            var entry = context.Entry(currentlicense);
                            log.VersionFrom = currentlicense.Version;
                            log.VersionTo = version;
                            log.Action = "ChangeVersion";
                            if ((Convert.ToInt64(version) > Convert.ToInt64(currentlicense.Version) && currentlicense.LicenseType.ToLower() == "local")
                                || currentlicense.LicenseType.ToLower() == "floating")
                            {
                                currentlicense.Version = version;
                                entry.Property(x => x.Version).IsModified = true;
                                context.SaveChanges();
                                log.Action = "Upgrade";
                            }
                            if (renew > 0)
                            {
                                currentlicense.Renew = 0;
                                entry.Property(x => x.Renew).IsModified = true;
                                context.SaveChanges();
                                log.Action = "Renew";
                            }

                            log.LicenseID = currentlicense.LicenseID;
                            log.MachineID = currentlicense.MachineID;
                            log.CreatedOn = DateTime.Now;
                            log.CreatedBy = Membership.GetUser().UserName;
                            context.LicenseLogs.Add(log);
                            context.SaveChanges();
                        }
                        var company = from cmp in _db.Companies where cmp.AccountNumber == dpt_Company select cmp;
                        var salesRep = from salrep in _db.SalesR where salrep.SalesRep == company.FirstOrDefault().SalesRep select salrep;
                        var sr = from cmp in _db.Companies where cmp.AccountNumber == salesRep.FirstOrDefault().AccountNumber select cmp;
                        MailMessage mail = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["hostusername"], company.FirstOrDefault().Email);
                        mail.CC.Add(sr.FirstOrDefault().Email);
                        mail.Bcc.Add("Orders@dptcorporate.com");
                        if (company.FirstOrDefault().Language.ToLower() == "japanese")
                        {
                            mail.CC.Add(_db.Companies.Where(x => x.AccountName == "t3 japan kk").FirstOrDefault().Email);
                            mail.Subject = "[このメールには返信しないでください] " + company.FirstOrDefault().AccountName + " (" + company.FirstOrDefault().AccountNumber + ") 様、ライセンスが発行されました";
                            mail.Body = "<pre>DPT User 様。<br/><br/>先ほど DPT ライセンスサービスよりライセンスパスワードを含む .v2c ファイルをお送りいたしました。\n" +
                                "以下にライセンスの詳細を記載いたします。<br/><br/>ライセンスID: " + currentlicense.LicenseID + " (" +
                                currentlicense.LicenseFlag.Substring(0, 3).ToUpper() + ")<br/>マシンＩＤ: " + currentlicense.MachineID +
                                "<br/>製品: " + currentlicense.ProductName + "<br/>バージョン: " + version +
                                "<br/>終了日: " + (currentlicense.ArticleDetail.ToLower() == "pl" ? "pl" : currentlicense.MED) +
                                "<br/><br/>以上、よろしくお願いいたします。<br/>DPT Services</pre>";
                        }
                        else
                        {
                            mail.Subject = "[DO NOT REPLY] License issued for " + company.FirstOrDefault().AccountName + " (" + company.FirstOrDefault().AccountNumber + ") ";
                            mail.Body = "<pre>Dear User, <br/><br/>You should have just received a message from DPT Licensing containing a .v2c password file." +
                                "<br/>Here below you'll find more details:<br/><br/>LicenseID: " + currentlicense.LicenseID + " (" +
                                currentlicense.LicenseFlag.Substring(0, 3).ToUpper() + ")<br/>MachineID: " + currentlicense.MachineID +
                                "<br/>Product: " + currentlicense.ProductName + "<br/>Version: " + version +
                                "<br/>Expiration Date: " + (currentlicense.ArticleDetail.ToLower() == "pl" ? "pl" : currentlicense.MED) +
                                //".\n\nYou can browse the licenses of the companies managed by you at https://dpt3.dptcorporate.com/License" +
                                "<br/><br/>Best regards,<br/><br/>DPT Services</pre>";
                        }
                        mail.IsBodyHtml = true;
                        try
                        {
                            MailHelper.SendMail(mail);
                        }
                        catch (Exception e)
                        {
                            LogHelper.WriteLog("UserController (UpgradeLicense): " + e.Message + "-" + e.InnerException);
                        }
                        //ViewBag.ok1 = "You have generated your license.";
                        return Json(DPTnew.Localization.Resource.LicenseMailMsg + ": " + company.FirstOrDefault().Email, JsonRequestBehavior.AllowGet);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            return Json("Something went wrong. It's impossible to upgrade the license.", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> CreateLicense(Entitlement l)
        {
            //common variables
            LicenseView currentlicense = null;
            SerLicenseFlag lf = null;
            string type = "";
            string dpt_Company = "";
            string var = "";
            string filestring = "";
            string c2v;
            string input = "";
            int block;

            if (l.file != null)
            {
                filestring = fileToString(l.file);
                int index1 = filestring.IndexOf("SL-AdminMode");
                int index2 = filestring.IndexOf("SL-UserMode");
                if (!l.MachineID.ToUpper().Contains("BLK") && (index1 == -1 || index2 == -1))
                {
                    ModelState.AddModelError("CREATE", "The file you have uploaded is not correct");
                    return View("Create");
                }
                try
                {
                    c2v = encodeC2V(filestring);
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("CREATE", "The file you have uploaded is not correct");
                    return View("Create");
                }

                using (var context = new DptContext())
                {
                    currentlicense = context.Licenses.SingleOrDefault(u => u.LicenseID == l.LicenseID);
                    dpt_Company = currentlicense.AccountNumber;
                    lf = context.LicFlag.SingleOrDefault(x => x.LicenseFlag == currentlicense.LicenseFlag);
                    var company = context.Companies.SingleOrDefault(x => x.AccountNumber == dpt_Company);
                    var salesRep = context.SalesR.SingleOrDefault(y => y.SalesRep == company.SalesRep);
                    var = context.Companies.SingleOrDefault(z => z.AccountNumber == salesRep.AccountNumber).AccountNumber;
                    block = context.SpecialCompanies.Where(c => c.Description == "BLOCKED").Select(c => c.AccountNumber).ToList().Contains(dpt_Company) ? 1 : 0;
                }

                if (currentlicense != null && Convert.ToInt64(currentlicense.Version) > 2014 && block == 0)
                {
                    var now = System.DateTime.Now;
                    Regex evalrgx = new Regex(@"^EVA");

                    //check for import/install
                    if (currentlicense.Import == 1 && currentlicense.MaintEndDate >= now && lf.Install_Safenet == 1)
                    {
                        //CHECK if is EVAL                        
                        var isEval = evalrgx.IsMatch(currentlicense.LicenseFlag.ToUpper());

                        if (currentlicense.LicenseType == "local")
                        { //LOCAL
                            if (currentlicense.ArticleDetail == "pl")
                            {
                                type = "PL";
                                SetDateToNow(currentlicense);
                            }
                            else
                            {
                                //if (isEval) { type = "EVAL"; }
                                //else
                                //{
                                if (currentlicense.MachineID.ToUpper().Contains("BLK"))
                                    type = "PHYSIC_EXPIR";
                                else
                                    type = "EXPIR";
                                CheckQMTsf(currentlicense);
                                //}
                            }
                        }
                        else
                        {//FLOATING
                            if (currentlicense.ArticleDetail == "pl")
                            {
                                type = "NET_PL";
                                SetDateToNow(currentlicense);
                            }
                            else
                            {
                                type = "NET_EXPIR";
                                CheckQMTsf(currentlicense);
                            }
                        }

                        //building product
                        string productPostfix = "_" + currentlicense.Version + currentlicense.Release + type;
                        var pname = GetProductName(l.ProductName);

                        //EVAL or LOCAL PL
                        if (type == "PL" /*|| type == "EVAL"*/)
                        {
                            SafenetEvalPlLocalEntitlment e1 = new SafenetEvalPlLocalEntitlment();
                            e1.CrmId = dpt_Company;
                            e1.EntType = "PRODUCT_KEY";
                            e1.refId1 = l.file.FileName;
                            e1.refId2 = currentlicense.LicenseID;
                            //ADD PRODUCT
                            e1.ProductName = InitSafenetProduct(currentlicense.PwdCode, pname, productPostfix, var);

                            e1.Encoded = true;
                            e1.C2V = c2v;
                            input = JsonConvert.SerializeObject(e1);
                        }
                        else
                        {
                            SafenetNetAsfLocalEntitlment e2 = new SafenetNetAsfLocalEntitlment();

                            e2.CrmId = dpt_Company;
                            e2.EntType = "PRODUCT_KEY";
                            e2.refId1 = l.file.FileName;
                            e2.refId2 = currentlicense.LicenseID;
                            //ADD PRODUCT
                            e2.ProductName = InitSafenetProduct(currentlicense.PwdCode, pname, productPostfix, var);

                            //ADDITIONAL PARAMETERS
                            e2.SaotParams = new JArray();

                            foreach (string pn in e2.ProductName)
                            {
                                JObject temp = new JObject();
                                temp["Product"] = pn;

                                //choose product
                                if (type == "EXPIR" || type == "NET_EXPIR" || type == "PHYSIC_EXPIR")
                                {
                                    DateTime med = new DateTime();
                                    med = (System.DateTime)currentlicense.MaintEndDate;
                                    temp["EXPIRATION_DATE"] = med.Date.ToString("yyyy-MM-dd");
                                }
                                if (type == "NET_PL" || type == "NET_EXPIR")
                                {
                                    temp["CONCURRENT_INSTANCES"] = currentlicense.Quantity.ToString();
                                }
                                e2.SaotParams.Add(temp);
                            }

                            e2.Encoded = true;
                            e2.C2V = c2v;

                            input = JsonConvert.SerializeObject(e2);
                        }

                        string uri = Url.Action("CreateCompleteLicense", "Safenet", new { httproute = "" }, "https");

                        HttpResponseMessage response = await SendJsonAsync(uri, input);

                        if (response.IsSuccessStatusCode)
                        {
                            string content = response.Content.ReadAsStringAsync().Result;
                            JObject contentresult = JObject.Parse(content);
                            string pkey = (string)contentresult["activation"]["activationOutput"]["protectionKeyId"];
                            if (pkey.Length > 10)
                            {
                                currentlicense.MachineID = "KID";
                                for (var i = 0; i < 18 - pkey.Length; i++)
                                    currentlicense.MachineID += "0";
                                currentlicense.MachineID += pkey;
                            }
                            else
                            {
                                currentlicense.MachineID = "BLK";

                                for (int i = 0; i < (10 - pkey.Length); i++)
                                {
                                    currentlicense.MachineID = currentlicense.MachineID + "0";
                                }
                                currentlicense.MachineID = currentlicense.MachineID + pkey;
                            }

                            using (var context = new DptContext())
                            {
                                //update state in db
                                currentlicense.Installed = 1;
                                currentlicense.Import = 0;
                                currentlicense.Renew = 0;

                                context.Licenses.Attach(currentlicense);
                                var entry = context.Entry(currentlicense);
                                entry.Property(x => x.Installed).IsModified = true;
                                entry.Property(x => x.Import).IsModified = true;
                                entry.Property(x => x.Renew).IsModified = true;
                                entry.Property(x => x.MachineID).IsModified = true;
                                if (currentlicense.ArticleDetail == "qsf" || currentlicense.ArticleDetail == "msf" ||
                                    currentlicense.ArticleDetail == "tsf" || currentlicense.ArticleDetail == "wsf")
                                {
                                    entry.Property(x => x.MaintEndDate).IsModified = true;
                                    entry.Property(x => x.MaintStartDate).IsModified = true;
                                    entry.Property(x => x.EndDate).IsModified = true;
                                    entry.Property(x => x.StartDate).IsModified = true;
                                }
                                context.SaveChanges();

                                DptLicenseLog log = new DptLicenseLog();
                                log.LicenseID = currentlicense.LicenseID;
                                log.MachineID = currentlicense.MachineID;
                                log.Action = "Install";
                                log.CreatedOn = DateTime.Now;
                                log.CreatedBy = Membership.GetUser().UserName;
                                log.C2VFileName = l.file.FileName;
                                log.VersionFrom = currentlicense.Version;
                                context.LicenseLogs.Add(log);
                                context.SaveChanges();
                            }

                            var company = from cmp in _db.Companies where cmp.AccountNumber == dpt_Company select cmp;
                            var salesRep = from salrep in _db.SalesR where salrep.SalesRep == company.FirstOrDefault().SalesRep select salrep;
                            var sr = from cmp in _db.Companies where cmp.AccountNumber == salesRep.FirstOrDefault().AccountNumber select cmp;
                            MailMessage mail = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["hostusername"], company.FirstOrDefault().Email);
                            mail.CC.Add(sr.FirstOrDefault().Email);
                            mail.Bcc.Add("Orders@dptcorporate.com");
                            if (company.FirstOrDefault().Language.ToLower() == "japanese")
                            {
                                mail.CC.Add(_db.Companies.Where(x => x.AccountName == "t3 japan kk").FirstOrDefault().Email);
                                mail.Subject = "[このメールには返信しないでください] " + company.FirstOrDefault().AccountName + " (" + company.FirstOrDefault().AccountNumber + ") 様、ライセンスが発行されました";
                                mail.Body = "<pre>DPT User 様。<br/><br/>先ほど DPT ライセンスサービスよりライセンスパスワードを含む .v2c ファイルをお送りいたしました。<br/>" +
                                    "以下にライセンスの詳細を記載いたします。<br/><br/>ライセンスID: " + currentlicense.LicenseID + " (" + currentlicense.LicenseFlag.Substring(0, 3).ToUpper() +
                                    ")<br/>マシンＩＤ: " + currentlicense.MachineID + "<br/>.c2v ファイル: " +
                                    l.file.FileName + "<br/>製品: " + currentlicense.ProductName + "<br/>バージョン: " + currentlicense.Version +
                                    "<br/>終了日: " + (currentlicense.ArticleDetail.ToLower() == "pl" ? "pl" : currentlicense.MED) +
                                    "<br/><br/>以上、よろしくお願いいたします。<br/>DPT Services</pre>";
                            }
                            else
                            {
                                mail.Subject = "[DO NOT REPLY] License issued for " + company.FirstOrDefault().AccountName + " (" + company.FirstOrDefault().AccountNumber + ") ";
                                mail.Body = "<pre>Dear User, <br/><br/>You should have just received a message from DPT Licensing containing a .v2c password file." +
                                    "<br/>Here below you'll find more details:<br/><br/>LicenseID: " + currentlicense.LicenseID + " (" + currentlicense.LicenseFlag.Substring(0, 3).ToUpper() +
                                    ")<br/>MachineID: " + currentlicense.MachineID + "<br/>.c2v file: " +
                                    l.file.FileName + "<br/>Product: " + currentlicense.ProductName + "<br/>Version: " + currentlicense.Version +
                                    "<br/>Expiration Date: " + (currentlicense.ArticleDetail.ToLower() == "pl" ? "pl" : currentlicense.MED) +
                                    //".\n\nYou can browse the licenses of the companies managed by you at https://dpt3.dptcorporate.com/License" +
                                    "<br/><br/>Best regards,<br/><br/>DPT Services</pre>";
                            }
                            mail.IsBodyHtml = true;
                            try
                            {
                                MailHelper.SendMail(mail);
                            }
                            catch (Exception e)
                            {
                                LogHelper.WriteLog("UserController (Create): " + e.Message + "-" + e.InnerException);
                            }
                            ViewBag.ok1 = DPTnew.Localization.Resource.LicenseCreateMsg;
                            ViewBag.ok2 = DPTnew.Localization.Resource.LicenseMailMsg + ": " + company.FirstOrDefault().Email;
                            return View("Success");
                        }
                        ModelState.AddModelError("CREATE", "Something went wrong. It's impossible to generate the license: " + response.ReasonPhrase);
                        return View("Create", l);
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            ModelState.AddModelError("CREATE", "Something went wrong. It's impossible to generate the license.");
            return View("Create", l);
        }

        private static string GetProductName(string prodName)
        {
            switch (prodName)
            {
                case "tteampdm": return "ThinkTeamPDM";
                case "tteamadd": return "ThinkTeamADD";
                case "tteamdev": return "ThinkTeamDEV";
                case "tteamdm": return "ThinkTeamMDM";
                case "tteamdoc": return "ThinkTeamDOC";
                case "tteampcb": return "ThinkTeamPCB";
                case "tteampcm": return "ThinkTeamPCM";
                case "tdprofessionaledu": return "TDEducation";
                default: return prodName;
            }
        }

        private static void SetDateToNow(LicenseView currentlicense)
        {
            currentlicense.StartDate = DateTime.Now;
            currentlicense.EndDate = DateTime.Now;
            currentlicense.MaintStartDate = DateTime.Now;
            currentlicense.MaintEndDate = DateTime.Now;
        }

        private static void CheckQMTsf(LicenseView currentlicense)
        {
            if (currentlicense.MED.Replace("-", "") == "20280101")
            {
                using (var db = new DptContext())
                {
                    //update maintenddate in db
                    if (currentlicense.ArticleDetail == "qsf")
                    {
                        currentlicense.MaintStartDate = DateTime.Now;
                        currentlicense.MaintEndDate = DateTime.Now.AddDays(90);
                        currentlicense.StartDate = currentlicense.MaintStartDate;
                        currentlicense.EndDate = currentlicense.MaintEndDate;
                    }
                    if (currentlicense.ArticleDetail == "msf")
                    {
                        currentlicense.MaintStartDate = DateTime.Now;
                        currentlicense.MaintEndDate = DateTime.Now.AddDays(30);
                        currentlicense.StartDate = currentlicense.MaintStartDate;
                        currentlicense.EndDate = currentlicense.MaintEndDate;
                    }
                    if (currentlicense.ArticleDetail == "tsf")
                    {
                        currentlicense.MaintStartDate = DateTime.Now;
                        currentlicense.MaintEndDate = DateTime.Now.AddDays(3);
                        currentlicense.StartDate = currentlicense.MaintStartDate;
                        currentlicense.EndDate = currentlicense.MaintEndDate;
                    }
                    if (currentlicense.ArticleDetail == "wsf")
                    {
                        currentlicense.MaintStartDate = DateTime.Now;
                        currentlicense.MaintEndDate = DateTime.Now.AddDays(7);
                        currentlicense.StartDate = currentlicense.MaintStartDate;
                        currentlicense.EndDate = currentlicense.MaintEndDate;
                    }
                }
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