using CALCXLib;
using DPTnew.Helper;
using DPTnew.Models;
using LicenseObject;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Web.Http;
using System.Web.Security;

namespace DptLicensingServer.Controllers
{
    public class PasswordGeneratorController : ApiController
    {
        private const string token = "CalcX@Integration!";
        List<string> acceptedType = new List<string> { "P", "T", "E", "S", "V" };
        //NewLicense(int old, string tipo, string prodotto, string machineid, string expdata, out string outstr)
        [HttpPost]
        [ActionName("NewLicense")]
        public HttpResponseMessage NewLicense(NewLicenseProperties data)
        {
            string tipo = data.tipo;
            string prodotto = data.prodotto;
            string machineid = data.machineid;
            string expdata = data.expdata;
            int old = data.old == 0 ? 3 : data.old;
            IEnumerable<string> h_lid;

            if (!Authenticate())
                return CreateResponse(HttpStatusCode.Unauthorized);
            try
            {
                if (old > 3 || old < 2 || !acceptedType.Contains(tipo))
                    return CreateResponse(HttpStatusCode.BadRequest);
                LicenseView currentlicense = null;

                if (!string.IsNullOrEmpty(data.licenseID))
                    using (var db = new DptContext())
                    {
                        currentlicense = db.Licenses.Single(x => x.LicenseID == data.licenseID);
                    }
                else if (Request.Headers.TryGetValues("LicenseID", out h_lid))
                    using (var db = new DptContext())
                    {
                        currentlicense = db.Licenses.Single(x => x.LicenseID == h_lid.FirstOrDefault());
                    }

                if (currentlicense == null || (currentlicense.MachineID == "ABCDEFGH" && currentlicense.Import == 0))
                    return CreateResponse(HttpStatusCode.BadRequest);

                if (expdata == "20280101")
                    using (var db = new DptContext())
                    {
                        expdata = CheckQMTsf(data.artDetail, expdata, db, currentlicense);
                    }
                IEnumerable<string> TDIRECTBundle = new List<string> { "tdirectcatiarw", "tdirectparasolidrw", "tdirectproerw" };
                IEnumerable<string> TDIRECTCode = new List<string> { "IK", "XP", "IJ" };
                IEnumerable<string> TTeamAddBundle = new List<string> { "REPLICATEDVAULT", "TTEAMECR-ECO", "TTEAMPDMXCHNGXTD", "TTEAMPDMXCHANGES", "TTEAMPDMXCHNGXAC", "TTEAMPDMXCHNGXPE", "TTEAMPDMXCHNGXSW", "TTEAMMAINTAIN" };
                IEnumerable<string> TTeamAddCode = new List<string> { "RV", "TG", "YB", "YC", "YD", "YE", "YF", "TH" };
                JObject newLicenseResult = null;
                if (prodotto.Substring(0, 2) == "IX") //TDIRECTRW
                {
                    var pwd = "";
                    for (int i = 0; i < TDIRECTBundle.Count(); i++)
                    {
                        prodotto = TDIRECTCode.ElementAt(i) + prodotto.Substring(2);
                        string outstrres = CalculatePassword(tipo, prodotto, machineid, expdata, old);
                        pwd += TDIRECTBundle.ElementAt(i) + ":" + outstrres + ";";
                    }
                    newLicenseResult = JObject.FromObject(new
                    {
                        Password = pwd
                    });
                }
                else
                {
                    if (prodotto.Substring(0, 2) == "AD") // TTEAMADD
                    {
                        var pwd = "";
                        for (int i = 0; i < TTeamAddBundle.Count(); i++)
                        {
                            prodotto = TTeamAddCode.ElementAt(i) + prodotto.Substring(2);
                            string outstrres = CalculatePassword(tipo, prodotto, machineid, expdata, old);
                            pwd += TTeamAddBundle.ElementAt(i) + ":" + outstrres + ";";
                        }
                        newLicenseResult = JObject.FromObject(new
                        {
                            Password = pwd
                        });
                    }
                    else
                    {
                        string outstrres = CalculatePassword(tipo, prodotto, machineid, expdata, old);
                        newLicenseResult = JObject.FromObject(new
                        {
                            Password = outstrres
                        });
                    }
                }
                using (var db = new DptContext())
                {
                    if (currentlicense.MachineID != machineid)
                    {
                        //update machineid in db
                        currentlicense.MachineID = machineid;
                        currentlicense.Import = 0;
                        db.Licenses.Attach(currentlicense);
                        var entry = db.Entry(currentlicense);
                        entry.Property(x => x.MachineID).IsModified = true;
                        entry.Property(x => x.Import).IsModified = true;
                        db.SaveChanges();

                        SendMailWriteLog(currentlicense, db);
                    }
                }
                return CreateResponse(HttpStatusCode.OK, newLicenseResult);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("PasswordController (NewLicense): " + e.Message);
                var resp = JObject.FromObject(new
                {
                    Error = e.Message
                });
                return CreateResponse(HttpStatusCode.InternalServerError, resp);
            }
        }

        private static string CheckQMTsf(string artDetail, string expdata, DptContext db, LicenseView currentlicense)
        {
            //update maintenddate in db
            if (artDetail == "qsf")
            {
                currentlicense.MaintStartDate = DateTime.Now;
                currentlicense.MaintEndDate = DateTime.Now.AddDays(90);
                currentlicense.StartDate = currentlicense.MaintStartDate;
                currentlicense.EndDate = currentlicense.MaintEndDate;
                expdata = DateTime.Now.AddDays(90).ToString("yyyyMMdd");
            }
            if (artDetail == "msf")
            {
                currentlicense.MaintStartDate = DateTime.Now;
                currentlicense.MaintEndDate = DateTime.Now.AddDays(30);
                currentlicense.StartDate = currentlicense.MaintStartDate;
                currentlicense.EndDate = currentlicense.MaintEndDate;
                expdata = DateTime.Now.AddDays(30).ToString("yyyyMMdd");
            }
            if (artDetail == "tsf")
            {
                currentlicense.MaintStartDate = DateTime.Now;
                currentlicense.MaintEndDate = DateTime.Now.AddDays(3);
                currentlicense.StartDate = currentlicense.MaintStartDate;
                currentlicense.EndDate = currentlicense.MaintEndDate;
                expdata = DateTime.Now.AddDays(3).ToString("yyyyMMdd");
            }
            if (artDetail == "wsf")
            {
                currentlicense.MaintStartDate = DateTime.Now;
                currentlicense.MaintEndDate = DateTime.Now.AddDays(7);
                currentlicense.StartDate = currentlicense.MaintStartDate;
                currentlicense.EndDate = currentlicense.MaintEndDate;
                expdata = DateTime.Now.AddDays(7).ToString("yyyyMMdd");
            }
            if (artDetail == "pl")
            {
                currentlicense.MaintStartDate = DateTime.Now;
                currentlicense.MaintEndDate = DateTime.Now;
                currentlicense.StartDate = currentlicense.MaintStartDate;
                currentlicense.EndDate = currentlicense.MaintEndDate;
                expdata = DateTime.Now.ToString("yyyyMMdd");
            }

            db.Licenses.Attach(currentlicense);
            var entry = db.Entry(currentlicense);
            entry.Property(x => x.MaintEndDate).IsModified = true;
            entry.Property(x => x.MaintStartDate).IsModified = true;
            entry.Property(x => x.EndDate).IsModified = true;
            entry.Property(x => x.StartDate).IsModified = true;
            db.SaveChanges();
            return expdata;
        }

        private static string CalculatePassword(string tipo, string prodotto, string machineid, string expdata, int old)
        {
            string outstr = String.Empty;
            string outstrres = String.Empty;
            ILicense licenseManager = new License();
            licenseManager.NewLicense(old, tipo, prodotto, machineid, expdata, out outstr);
            licenseManager.ChangeMID(outstr, machineid, "4", out outstrres);
            return outstrres;
        }

        //void ChangeMID(string product, string newMID, string outFamily, out string outstr)
        [HttpGet]
        [ActionName("ChangeMID")]
        public HttpResponseMessage ChangeMID(string product, string newMID, string outFamily)
        {
            try
            {
                if (!Authenticate())
                    return CreateResponse(HttpStatusCode.Unauthorized);
                string outstr = String.Empty;
                ILicense licenseManager = new License();
                licenseManager.ChangeMID(product, newMID, outFamily, out outstr);
                JObject changeMIDResult = JObject.FromObject(new
                {
                    OutStr = outstr
                });
                return CreateResponse(HttpStatusCode.OK, changeMIDResult);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("PasswordController (ChangeMID): " + e.Message);
                return CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        //void CheckMIDs(string pwd, out string isvalid, out string tipo, out string res_days, out string counter, out string family, out string prod, out string vers, out string codice, out string dataexp, out string ancestor, out string descr)
        [HttpGet]
        [ActionName("CheckMIDs")]
        public HttpResponseMessage CheckMIDs(string pwd)
        {
            try
            {
                if (!Authenticate())
                    return CreateResponse(HttpStatusCode.Unauthorized);
                string isvalid = String.Empty;
                string tipo = String.Empty;
                string res_days = String.Empty;
                string counter = String.Empty;
                string family = String.Empty;
                string prod = String.Empty;
                string vers = String.Empty;
                string codice = String.Empty;
                string dataExp = String.Empty;
                string ancestor = String.Empty;
                string descr = String.Empty;
                ILicense licenseManager = new License();
                licenseManager.CheckMIDs(pwd, out isvalid, out tipo, out res_days, out counter, out family, out prod, out vers, out codice, out dataExp, out ancestor, out descr);
                JObject checkMIDsResult = JObject.FromObject(new
                {
                    IsValid = isvalid,
                    Tipo = tipo,
                    Res_Days = res_days,
                    Counter = counter,
                    Family = family,
                    Prod = prod,
                    Vers = vers,
                    Codice = codice,
                    DataExp = dataExp,
                    Ancestor = ancestor,
                    Descr = descr
                });
                return CreateResponse(HttpStatusCode.OK, checkMIDsResult);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("PasswordController (CheckMIDs): " + e.Message);
                return CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        //void OldLicense(int iopz, string machineid, string prod, string livab, string username, out string outpwd)
        [HttpGet]
        [ActionName("OldLicense")]
        public HttpResponseMessage OldLicense(int iopz, string machineid, string prod, string livab, string username)
        {
            try
            {
                if (!Authenticate())
                    return CreateResponse(HttpStatusCode.Unauthorized);
                string outpwd = String.Empty;
                ILicense licenseManager = new License();
                licenseManager.OldLicense(iopz, machineid, prod, livab, username, out outpwd);
                JObject oldLicenseResult = JObject.FromObject(new
                {
                    OutPwd = outpwd
                });
                return CreateResponse(HttpStatusCode.OK, oldLicenseResult);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("PasswordController (OldLicense): " + e.Message);
                return CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        //void DecPWDAnchor(string pwd, int ancora, out string prod, out string vers, out string ancestor, out string dataexp, out string counter)
        [HttpGet]
        [ActionName("DecPWDAnchor")]
        public HttpResponseMessage DecPWDAnchor(string pwd, int ancora)
        {
            try
            {
                if (!Authenticate())
                    return CreateResponse(HttpStatusCode.Unauthorized);
                string prod = String.Empty;
                string vers = String.Empty;
                string ancestor = String.Empty;
                string dataexp = String.Empty;
                string counter = String.Empty;
                ILicense licenseManager = new License();
                licenseManager.DecPWDAnchor(pwd, ancora, out prod, out vers, out ancestor, out dataexp, out counter);
                JObject decPWDAnchorResult = JObject.FromObject(new
                {
                    Prod = prod,
                    Vers = vers,
                    Ancestor = ancestor,
                    DataExp = dataexp,
                    Counter = counter
                });
                return CreateResponse(HttpStatusCode.OK, decPWDAnchorResult);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("PasswordController (DecPWDAnchor): " + e.Message);
                return CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        //void ChangeMID2(string product, string newMID, string outFamily, int nlcounter, out string outstr)
        [HttpGet]
        [ActionName("ChangeMID2")]
        public HttpResponseMessage ChangeMID2(string product, string newMID, string outFamily, int nlcounter)
        {
            try
            {
                if (!Authenticate())
                    return CreateResponse(HttpStatusCode.Unauthorized);
                string outstr = String.Empty;
                ILicense licenseManager = new License();
                licenseManager.ChangeMID2(product, newMID, outFamily, nlcounter, out outstr);
                JObject changeMID2Result = JObject.FromObject(new
                {
                    OutStr = outstr
                });
                return CreateResponse(HttpStatusCode.OK, changeMID2Result);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("PasswordController (ChangeMID2): " + e.Message);
                return CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        //void CheckMIDs2(string pwd, out string isvalid, out string tipo, out string res_days, out string counter, out string family, out string prod, out string vers, out string codice, out string dataexp, out string ancestor, out string descr, out string nlcounter)
        [HttpGet]
        [ActionName("CheckMIDs2")]
        public HttpResponseMessage CheckMIDs2(string pwd)
        {
            try
            {
                if (!Authenticate())
                    return CreateResponse(HttpStatusCode.Unauthorized);
                string isvalid = String.Empty;
                string tipo = String.Empty;
                string res_days = String.Empty;
                string counter = String.Empty;
                string family = String.Empty;
                string prod = String.Empty;
                string vers = String.Empty;
                string codice = String.Empty;
                string dataExp = String.Empty;
                string ancestor = String.Empty;
                string descr = String.Empty;
                string nlcounter = String.Empty;
                ILicense licenseManager = new License();
                licenseManager.CheckMIDs2(pwd, out isvalid, out tipo, out res_days, out counter, out family, out prod, out vers, out codice, out dataExp, out ancestor, out descr, out nlcounter);
                JObject checkMIDsResult = JObject.FromObject(new
                {
                    IsValid = isvalid,
                    Tipo = tipo,
                    Res_Days = res_days,
                    Counter = counter,
                    Family = family,
                    Prod = prod,
                    Vers = vers,
                    Codice = codice,
                    DataExp = dataExp,
                    Ancestor = ancestor,
                    Descr = descr
                });
                return CreateResponse(HttpStatusCode.OK, checkMIDsResult);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("PasswordController (CheckMIDs2): " + e.Message);
                return CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        //void FlexLicense(int flexkind, string feature, string Version, int numServ, string machineid1, string machineid2, string machineid3, int nlic, string expdate, string vend_string, int typelic, int expkind, string param, out string pwdline)
        [HttpGet]
        [ActionName("FlexLicense")]
        public HttpResponseMessage FlexLicense(int flexkind, string feature, string Version, int numServ, string machineid1, string machineid2, string machineid3, int nlic, string expdate, string vend_string, int typelic, int expkind, string param)
        {
            try
            {
                if (!Authenticate())
                    return CreateResponse(HttpStatusCode.Unauthorized);
                string pwdline = String.Empty;
                IEnumerable<string> h_lid;
                LicenseView currentlicense = null;

                if (Request.Headers.TryGetValues("LicenseID", out h_lid))
                {
                    using (var db = new DptContext())
                    {
                        currentlicense = db.Licenses.Single(x => x.LicenseID == h_lid.FirstOrDefault());
                    }
                    if (currentlicense == null || (currentlicense.MachineID == "ABCDEFGH" && currentlicense.Import == 0))
                        return CreateResponse(HttpStatusCode.BadRequest);

                    if (expdate == "20280101")
                        using (var db = new DptContext())
                        {
                            expdate = CheckQMTsf(currentlicense.ArticleDetail, expdate, db, currentlicense);
                        }
                }
                IFlexLicense licenseManager = (IFlexLicense)new License();
                licenseManager.FlexLicense(flexkind, feature, Version, numServ, machineid1, machineid2, machineid3, nlic, expdate, vend_string, typelic, expkind, param, out pwdline);
                JObject checkMIDsResult = JObject.FromObject(new
                {
                    PwdLine = pwdline
                });

                if (currentlicense != null && currentlicense.MachineID != machineid1)
                {
                    using (var db = new DptContext())
                    {
                        //update machineid in db
                        currentlicense.MachineID = machineid1;
                        currentlicense.Vend_String = vend_string;
                        currentlicense.FlexType = typelic;
                        currentlicense.Quantity = nlic;
                        currentlicense.Import = 0;
                        db.Licenses.Attach(currentlicense);
                        var entry = db.Entry(currentlicense);
                        entry.Property(x => x.MachineID).IsModified = true;
                        entry.Property(x => x.Vend_String).IsModified = true;
                        entry.Property(x => x.FlexType).IsModified = true;
                        entry.Property(x => x.Quantity).IsModified = true;
                        entry.Property(x => x.Import).IsModified = true;
                        db.SaveChanges();

                        SendMailWriteLog(currentlicense, db);
                    }
                }
                return CreateResponse(HttpStatusCode.OK, checkMIDsResult);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("PasswordController (FlexLicense): " + e.Message + "-" + e.InnerException);
                return CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        private static void SendMailWriteLog(LicenseView currentlicense, DptContext db)
        {
            var company = from cmp in db.Companies where cmp.AccountNumber == currentlicense.AccountNumber select cmp;
            var salesRep = from salrep in db.SalesR where salrep.SalesRep == company.FirstOrDefault().SalesRep select salrep;
            var sr = from cmp in db.Companies where cmp.AccountNumber == salesRep.FirstOrDefault().AccountNumber select cmp;
            MailMessage mail = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["hostusername"], sr.FirstOrDefault().Email);
            mail.Bcc.Add("Orders@dptcorporate.com");
            if (company.FirstOrDefault().Language.ToLower() == "japanese")
            {
                mail.Subject = "[DO NOT REPLY] New license issued (< 2015) for " + company.FirstOrDefault().AccountName + " (" + company.FirstOrDefault().AccountNumber + ") ";
                mail.Body = "代理店ご担当者様。\n\n以下のライセンスがお客様によって取得されたことをお知らせいたします。\n" +
                    "会社名: " + company.FirstOrDefault().AccountName + " (" + company.FirstOrDefault().AccountNumber + ") \n" +
                    "ライセンスID: " + currentlicense.LicenseID + "\nマシンＩＤ: " + currentlicense.MachineID +
                    "\n製品: " + currentlicense.ProductName + "\nバージョン: " + currentlicense.Version +
                    "\n終了日: " + (currentlicense.ArticleDetail.ToLower() == "pl" ? "pl" : currentlicense.MED) +
                    "\n\nお客様のライセンスの状況は、https://dpt3.dptcorporate.com/License" +
                    " からご確認いただけます。\n\n以上、よろしくお願いいたします。\n\nDPT Licensing";
            }
            else
            {
                mail.Subject = "[DO NOT REPLY] New license issued (< 2015) for " + company.FirstOrDefault().AccountName + " (" + company.FirstOrDefault().AccountNumber + ") ";
                mail.Body = "Dear User, \n\nThe company " + company.FirstOrDefault().AccountName + " (" + company.FirstOrDefault().AccountNumber + ") " +
                    "issued a new license.\n\nLicenseID: " + currentlicense.LicenseID + "\nMachineID: " + currentlicense.MachineID +
                    "\nProduct: " + currentlicense.ProductName + "\nVersion: " + currentlicense.Version +
                    "\nExpiration Date: " + (currentlicense.ArticleDetail.ToLower() == "pl" ? "pl" : currentlicense.MED) +
                    //".\n\nYou can browse the licenses of the companies managed by you at https://dpt3.dptcorporate.com/License" +
                    "\n\nBest regards,\n\nDPT Licensing";
            }
            try
            {
                MailHelper.SendMail(mail);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("PasswordGeneratorController (NewLicense): " + e.Message + "-" + e.InnerException);
            }
            DptLicenseLog log = new DptLicenseLog();
            log.LicenseID = currentlicense.LicenseID;
            log.MachineID = currentlicense.MachineID;
            log.Action = "Install";
            log.CreatedOn = DateTime.Now;
            log.CreatedBy = Membership.GetUser() == null ? "API" : Membership.GetUser().UserName;
            log.VersionFrom = currentlicense.Version;
            db.LicenseLogs.Add(log);
            db.SaveChanges();
        }

        [HttpGet]
        [ActionName("Upgrade")]
        public HttpResponseMessage Upgrade(string licenseId, string version)
        {
            try
            {
                if (!Authenticate())
                    return CreateResponse(HttpStatusCode.Unauthorized);

                using (var db = new DptContext())
                {
                    var query =
                    from license in db.Licenses
                    where license.LicenseID == licenseId
                    select license;

                    DptLicenseLog log = new DptLicenseLog();

                    foreach (LicenseView lic in query)
                    {
                        log.VersionFrom = lic.Version;
                        log.VersionTo = version;
                        log.LicenseID = lic.LicenseID;
                        log.MachineID = lic.MachineID;

                        lic.Version = version;
                    }
                    log.CreatedOn = DateTime.Now;
                    log.CreatedBy = Membership.GetUser().UserName;
                    log.Action = "Upgrade";
                    db.LicenseLogs.Add(log);

                    db.SaveChanges();
                }

                return CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("PasswordController (Upgrade): " + e.Message);
                return CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        //int xcsde(int key1, int key2, string instring, out string outstring)
        [HttpGet]
        [ActionName("xcsde")]
        public HttpResponseMessage xcsde(int keyOne, int keyTwo, string instring)
        {
            try
            {
                if (!Authenticate())
                    return CreateResponse(HttpStatusCode.Unauthorized);
                string outstring = String.Empty;
                ICrypter crypterManager = new Crypter();
                crypterManager.xcsde(keyOne, keyTwo, instring, out outstring);
                JObject xcsdeResult = JObject.FromObject(new
                {
                    OutString = outstring
                });
                return CreateResponse(HttpStatusCode.OK, xcsdeResult);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("PasswordController (xcsde): " + e.Message);
                return CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        //int xcsen(int key1, int key2, string instring, out string outstring)
        [HttpGet]
        [ActionName("xcsen")]
        public HttpResponseMessage xcsen(int keyOne, int keyTwo, string instring)
        {
            try
            {
                if (!Authenticate())
                    return CreateResponse(HttpStatusCode.Unauthorized);
                string outstring = String.Empty;
                ICrypter crypterManager = new Crypter();
                crypterManager.xcsen(keyOne, keyTwo, instring, out outstring);
                JObject xcsenResult = JObject.FromObject(new
                {
                    OutString = outstring
                });
                return CreateResponse(HttpStatusCode.OK, xcsenResult);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("PasswordController (xcsen): " + e.Message);
                return CreateResponse(HttpStatusCode.InternalServerError);
            }
        }


        private HttpResponseMessage CreateResponse(HttpStatusCode code, JObject resp = null)
        {
            if (resp == null)
                resp = JObject.FromObject(new
                {
                    Success = code != HttpStatusCode.OK ? false : true
                });
            else
                resp.Add("Success", code != HttpStatusCode.OK ? false : true);
            var settings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
            var serialized = JsonConvert.SerializeObject(resp, Formatting.None, settings);
            HttpResponseMessage response = this.Request.CreateResponse(code, serialized);
            response.Content = new StringContent(serialized, Encoding.UTF8);
            return response;
        }

        private bool Authenticate()
        {
            return true;
            //IEnumerable<string> headers = new List<string>();
            //if (!this.Request.Headers.TryGetValues("Token", out headers) || !headers.FirstOrDefault<string>().Equals(token))
            //  return false;
            //return true;
        }
    }

}
