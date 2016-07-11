using CALCXLib;
using DPTnew.Models;
using LicenseObject;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

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

                if (string.IsNullOrEmpty(expdata))
                {
                    if (Request.Headers.TryGetValues("LicenseID", out h_lid))
                        using (var db = new DptContext())
                        {
                            var currentlicense = db.Licenses.Single(x => x.LicenseID == h_lid.FirstOrDefault());
                            //update maintenddate in db
                            if (data.artDetail == "qsf")
                            {
                                currentlicense.MaintEndDate = DateTime.Now.AddDays(90);
                                expdata = DateTime.Now.AddDays(90).ToString("yyyyMMdd");
                            }
                            if (data.artDetail == "msf")
                            {
                                currentlicense.MaintEndDate = DateTime.Now.AddDays(30);
                                expdata = DateTime.Now.AddDays(30).ToString("yyyyMMdd");
                            }
                            if (data.artDetail == "tsf")
                            {
                                currentlicense.MaintEndDate = DateTime.Now.AddDays(15);
                                expdata = DateTime.Now.AddDays(15).ToString("yyyyMMdd");
                            }
                            db.Licenses.Attach(currentlicense);
                            var entry = db.Entry(currentlicense);
                            entry.Property(x => x.MaintEndDate).IsModified = true;
                            db.SaveChanges();
                        }
                }

                string outstr = String.Empty;
                string outstrres = String.Empty;
                ILicense licenseManager = new License();
                licenseManager.NewLicense(old, tipo, prodotto, machineid, expdata, out outstr);
                licenseManager.ChangeMID(outstr, machineid, "4", out outstrres);
                JObject newLicenseResult = JObject.FromObject(new
                {
                    Password = outstrres
                });

                if (Request.Headers.TryGetValues("LicenseID", out h_lid))
                    using (var db = new DptContext())
                    {
                        var currentlicense = db.Licenses.Single(x => x.LicenseID == h_lid.FirstOrDefault());
                        //update machineid in db
                        currentlicense.MachineID = machineid;
                        db.Licenses.Attach(currentlicense);
                        var entry = db.Entry(currentlicense);
                        entry.Property(x => x.MachineID).IsModified = true;
                        db.SaveChanges();
                    }
                return CreateResponse(HttpStatusCode.OK, newLicenseResult);

            }
            catch (Exception e)
            {
                var resp = JObject.FromObject(new
                {
                    Error = e.Message
                });
                return CreateResponse(HttpStatusCode.InternalServerError, resp);
            }
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
            catch (Exception)
            {
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
            catch (Exception)
            {
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
            catch (Exception)
            {
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
            catch (Exception)
            {
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
            catch (Exception)
            {
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
            catch (Exception)
            {
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
                IFlexLicense licenseManager = (IFlexLicense)new License();
                licenseManager.FlexLicense(flexkind, feature, Version, numServ, machineid1, machineid2, machineid3, nlic, expdate, vend_string, typelic, expkind, param, out pwdline);
                JObject checkMIDsResult = JObject.FromObject(new
                {
                    PwdLine = pwdline
                });

                IEnumerable<string> h_lid;
                if (Request.Headers.TryGetValues("LicenseID", out h_lid))
                    using (var db = new DptContext())
                    {
                        var currentlicense = db.Licenses.Single(x => x.LicenseID == h_lid.FirstOrDefault());
                        //update machineid in db
                        currentlicense.MachineID = machineid1;
                        db.Licenses.Attach(currentlicense);
                        var entry = db.Entry(currentlicense);
                        entry.Property(x => x.MachineID).IsModified = true;
                        db.SaveChanges();
                    }

                return CreateResponse(HttpStatusCode.OK, checkMIDsResult);
            }
            catch (Exception)
            {
                return CreateResponse(HttpStatusCode.InternalServerError);
            }
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

                    foreach (LicenseView lic in query)
                    {
                        lic.Version = version;
                    }

                    db.SaveChanges();
                }

                return CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception)
            {
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
            catch (Exception)
            {
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
            catch (Exception)
            {
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
