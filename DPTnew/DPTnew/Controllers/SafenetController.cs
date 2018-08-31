using DPTnew.Helper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SafenetIntegration;
using SafenetIntegration.Safenet;
using SafenetIntegration.Safenet.BusinessObject;
using SafenetIntegration.Safenet.Exception;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.ServiceModel.Description;
using System.Text;
using System.Web.Http;

namespace DptLicensingServer.Controllers
{
    public class SafenetController : ApiController
    {
        private const string token = "CalcX@Integration!";

        #region API

        [HttpGet]
        [ActionName("Login")]
        public HttpResponseMessage Login()
        {
            ClientCredentials cc;
            Uri uri;
            SentinelEMSWrapper sew;
            try
            {
                this.ValidateHeaders(out cc, out uri);
                sew = new SentinelEMSWrapper(uri, cc);
                sew.Authentication();
            }
            catch (SafenetException e)
            {
                LogHelper.WriteLog("SafenetController (Login): " + e.Message);
                return CreateResponse(HttpStatusCode.Unauthorized, e.Message);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("SafenetController (Login): " + e.Message);
                return CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return CreateResponse(HttpStatusCode.OK, "Logged in");
        }

        [HttpGet]
        [ActionName("GetCustomerById")]
        public HttpResponseMessage GetCustomerById(string id)
        {
            ClientCredentials cc;
            SentinelEMSWrapper sew;
            Uri uri;
            try
            {
                this.ValidateHeaders(out cc, out uri);
                sew = new SentinelEMSWrapper(uri, cc);
                sew.GetCustomerById(id);
            }
            catch (SafenetException e)
            {
                LogHelper.WriteLog("SafenetController (GetCustomerById): " + e.Message);
                return CreateResponse((HttpStatusCode)e.Data["StatusCode"], e.Message);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("SafenetController (GetCustomerById): " + e.Message);
                return CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return CreateResponse(HttpStatusCode.OK, sew.JsonResponse);
        }

        [HttpGet]
        [ActionName("GetAllVendors")]
        public HttpResponseMessage GetAllVendors()
        {
            ClientCredentials cc;
            SentinelEMSWrapper sew;
            Uri uri;
            try
            {
                this.ValidateHeaders(out cc, out uri);
                sew = new SentinelEMSWrapper(uri, cc);
                sew.GetAllVendors();
            }
            catch (SafenetException e)
            {
                LogHelper.WriteLog("SafenetController (GetAllVendors): " + e.Message);
                return CreateResponse((HttpStatusCode)e.Data["StatusCode"], e.Message);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("SafenetController (GetAllVendors): " + e.Message);
                return CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return CreateResponse(HttpStatusCode.OK, sew.JsonResponse);
        }

        [HttpGet]
        [ActionName("GetAllProducts")]
        public HttpResponseMessage GetAllProducts(string vendorId = "")
        {
            ClientCredentials cc;
            SentinelEMSWrapper sew;
            Uri uri;
            try
            {
                this.ValidateHeaders(out cc, out uri);
                sew = new SentinelEMSWrapper(uri, cc);
                sew.SearchProducts(new string[] { }, vendorId);
            }
            catch (SafenetException e)
            {
                LogHelper.WriteLog("SafenetController (GetAllProducts): " + e.Message);
                return CreateResponse((HttpStatusCode)e.Data["StatusCode"], e.Message);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("SafenetController (GetAllProducts): " + e.Message);
                return CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return CreateResponse(HttpStatusCode.OK, sew.JsonResponse);
        }

        [HttpGet]
        [ActionName("UpdateDptProducts")]
        public HttpResponseMessage UpdateDptProducts()
        {
            ClientCredentials cc;
            SentinelEMSWrapper sew;
            Uri uri;
            try
            {
                this.ValidateHeaders(out cc, out uri);
                sew = new SentinelEMSWrapper(uri, cc);
                sew.UpdateDptProducts();
            }
            catch (SafenetException e)
            {
                LogHelper.WriteLog("SafenetController (UpdateDptProducts): " + e.Message);
                return CreateResponse((HttpStatusCode)e.Data["StatusCode"], e.Message);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("SafenetController (UpdateDptProducts): " + e.Message);
                return CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return CreateResponse(HttpStatusCode.OK, sew.JsonResponse);
        }

        [HttpPost]
        [ActionName("UpdateCustomer")]
        public HttpResponseMessage UpdateCustomer(JObject data)
        {
            ClientCredentials cc;
            Uri uri;
            SentinelEMSWrapper sew;
            try
            {
                this.ValidateHeaders(out cc, out uri);
                sew = new SentinelEMSWrapper(uri, cc);
                sew.Authentication();
                sew.UpdateCustomer(data);
            }
            catch (SafenetException e)
            {
                LogHelper.WriteLog("SafenetController (UpdateCustomer): " + e.Message);
                return CreateResponse((HttpStatusCode)e.Data["StatusCode"], e.Message);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("SafenetController (UpdateCustomer): " + e.Message);
                return CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return CreateResponse(HttpStatusCode.OK, sew.JsonResponse);
        }

        [HttpPost]
        [ActionName("UpdateOrCreateCustomer")]
        public HttpResponseMessage UpdateOrCreateCustomer(JObject data)
        {
            ClientCredentials cc;
            Uri uri;
            SentinelEMSWrapper sew;
            try
            {
                this.ValidateHeaders(out cc, out uri);
                sew = new SentinelEMSWrapper(uri, cc);
                sew.Authentication();
                if (sew.CheckExistCustomer(data))
                    sew.UpdateCustomer(data);
                else
                    sew.CreateCustomer(data);
            }
            catch (SafenetException e)
            {
                LogHelper.WriteLog("SafenetController (UpdateOrCreateCustomer): " + e.Message);
                return CreateResponse((HttpStatusCode)e.Data["StatusCode"], e.Message);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("SafenetController (UpdateOrCreateCustomer): " + e.Message);
                return CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return CreateResponse(HttpStatusCode.OK, sew.JsonResponse);
        }

        [HttpPost]
        [ActionName("CreateCustomer")]
        public HttpResponseMessage CreateCustomer(JObject data)
        {
            ClientCredentials cc;
            Uri uri;
            SentinelEMSWrapper sew;
            try
            {
                this.ValidateHeaders(out cc, out uri);
                sew = new SentinelEMSWrapper(uri, cc);
                sew.Authentication();
                sew.CreateCustomer(data);
            }
            catch (SafenetException e)
            {
                LogHelper.WriteLog("SafenetController (CreateCustomer): " + e.Message);
                return CreateResponse((HttpStatusCode)e.Data["StatusCode"], e.Message);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("SafenetController (CreateCustomer): " + e.Message);
                return CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return CreateResponse(HttpStatusCode.OK, sew.JsonResponse);
        }

        [HttpPost]
        [ActionName("CreateCustomerBatch")]
        public HttpResponseMessage CreateCustomerBatch(JObject data)
        {
            ClientCredentials cc;
            Uri uri;
            SentinelEMSWrapper sew;
            try
            {
                this.ValidateHeaders(out cc, out uri);
                sew = new SentinelEMSWrapper(uri, cc);
                sew.Authentication();
                return CreateResponse(HttpStatusCode.OK, sew.CreateCustomerBatch(data));
            }
            catch (SafenetException e)
            {
                LogHelper.WriteLog("SafenetController (CreateCustomerBatch): " + e.Message);
                return CreateResponse((HttpStatusCode)e.Data["StatusCode"], e.Message);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("SafenetController (CreateCustomerBatch): " + e.Message);
                return CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }

        }

        [HttpPost]
        [ActionName("CreateEntitlement")]
        public HttpResponseMessage CreateEntitlement(JObject data)
        {
            ClientCredentials cc;
            Uri uri;
            SentinelEMSWrapper sew;
            try
            {
                this.ValidateHeaders(out cc, out uri);
                sew = new SentinelEMSWrapper(uri, cc);
                sew.Authentication();
                sew.CreateEntitlement(data);
            }
            catch (SafenetException e)
            {
                LogHelper.WriteLog("SafenetController (CreateEntitlement): " + e.Message);
                return CreateResponse((HttpStatusCode)e.Data["StatusCode"], e.Message);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("SafenetController (CreateEntitlement): " + e.Message);
                return CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return CreateResponse(HttpStatusCode.OK, sew.JsonResponse);
        }

        [HttpPost]
        [ActionName("CheckInC2V")]
        public HttpResponseMessage CheckInC2V(JObject data)
        {
            ClientCredentials cc;
            Uri uri;
            SentinelEMSWrapper sew;
            try
            {
                this.ValidateHeaders(out cc, out uri);
                sew = new SentinelEMSWrapper(uri, cc);
                sew.Authentication();
                sew.CheckInC2V(data);
            }
            catch (SafenetException e)
            {
                LogHelper.WriteLog("SafenetController (CheckInC2V): " + e.Message);
                return CreateResponse((HttpStatusCode)e.Data["StatusCode"], e.Message);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("SafenetController (CheckInC2V): " + e.Message);
                return CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return CreateResponse(HttpStatusCode.OK, sew.JsonResponse);
        }

        [HttpPost]
        [ActionName("RetrieveV2CP")]
        public HttpResponseMessage RetrieveV2CP(JObject data)
        {
            ClientCredentials cc;
            Uri uri;
            SentinelEMSWrapper sew;
            try
            {
                this.ValidateHeaders(out cc, out uri);
                sew = new SentinelEMSWrapper(uri, cc);
                sew.Authentication();
                sew.RetrieveV2CP(data);
            }
            catch (SafenetException e)
            {
                LogHelper.WriteLog("SafenetController (CheckInC2V): " + e.Message);
                return CreateResponse((HttpStatusCode)e.Data["StatusCode"], e.Message);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("SafenetController (CheckInC2V): " + e.Message);
                return CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return CreateResponse(HttpStatusCode.OK, sew.JsonResponse);
        }

        [HttpPost]
        [ActionName("GenerateLicense")]
        public HttpResponseMessage GenerateLicense(JObject data)
        {
            ClientCredentials cc;
            Uri uri;
            SentinelEMSWrapper sew;
            try
            {
                this.ValidateHeaders(out cc, out uri);
                sew = new SentinelEMSWrapper(uri, cc);
                sew.Authentication();
                sew.GenerateLicense(data);
            }
            catch (SafenetException e)
            {
                LogHelper.WriteLog("SafenetController (GenerateLicense): " + e.Message);
                return CreateResponse((HttpStatusCode)e.Data["StatusCode"], e.Message);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("SafenetController (GenerateLicense): " + e.Message);
                return CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return CreateResponse(HttpStatusCode.OK, sew.JsonResponse);
        }

        [HttpPost]
        [ActionName("CreateCompleteLicense")]
        public HttpResponseMessage CreateCompleteLicense(JObject data)
        {
            ClientCredentials cc;
            Uri uri;
            SentinelEMSWrapper sew;
            try
            {
                this.ValidateHeaders(out cc, out uri);
                SafenetUtilities.ValidateParameters(data);
                sew = new SentinelEMSWrapper(uri, cc);
                sew.Authentication();
                sew.CreateEntitlement(data);
                string entId = sew.JsonResponse["Headers"]["Location"][0].Value<string>();
                data.Add("Eid", entId);
                JToken c2v = "";
                // if (data.TryGetValue("C2V", out c2v) && !string.IsNullOrEmpty(c2v.Value<string>()))
                if (data.TryGetValue("C2V", out c2v))
                {
                    sew.GenerateLicense(data);
                    string protectionKeyId = sew.JsonResponse["activation"]["activationOutput"]["protectionKeyId"].Value<string>();
                }
            }
            catch (SafenetException e)
            {
                LogHelper.WriteLog("SafenetController (CreateCompleteLicense) - Safenet Exception: " + e.Message);
                return CreateResponse((HttpStatusCode)e.Data["StatusCode"], e.Message);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("SafenetController (CreateCompleteLicense) - Exception: " + e.Message);
                return CreateResponse(HttpStatusCode.BadRequest, e.Message);
            }
            return CreateResponse(HttpStatusCode.OK, sew.JsonResponse);
        }

        #endregion

        #region private methods

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

        private HttpResponseMessage CreateResponse(HttpStatusCode code, string result = null)
        {
            JObject resp = null;
            resp = JObject.FromObject(new
            {
                Success = code != HttpStatusCode.OK ? false : true
            });
            resp.Add("Result", result);
            var settings = new JsonSerializerSettings
            {
                DateTimeZoneHandling = DateTimeZoneHandling.Utc
            };
            var serialized = JsonConvert.SerializeObject(resp, Formatting.None, settings);
            HttpResponseMessage response = this.Request.CreateResponse(code, serialized);
            if (!string.IsNullOrEmpty(result))
                response.ReasonPhrase = result;
            response.Content = new StringContent(serialized, Encoding.UTF8);
            return response;
        }

        private void ValidateHeaders(out ClientCredentials safenetClientCredential, out Uri safenetUri)
        {
            safenetClientCredential = new ClientCredentials();
            safenetUri = null;

            //IEnumerable<string> authHeader = new List<string>();
            //IEnumerable<string> userName = new List<string>();
            //IEnumerable<string> password = new List<string>();
            //IEnumerable<string> uri = new List<string>();
            //if (!this.Request.Headers.TryGetValues("Token", out authHeader) ||
            //  !authHeader.FirstOrDefault<string>().Equals(token))
            //  throw new SafenetException("Wrong Token Header", HttpStatusCode.Unauthorized);
            //if (!this.Request.Headers.TryGetValues("SafenetUser", out userName) ||
            //    string.IsNullOrEmpty(userName.FirstOrDefault<string>()))
            //  throw new SafenetException("Missing or empty header: SafenetUser", HttpStatusCode.BadRequest);
            //if (!this.Request.Headers.TryGetValues("SafenetPassword", out password) ||
            //  string.IsNullOrEmpty(password.FirstOrDefault<string>()))
            //  throw new SafenetException("Missing or empty header: SafenetPassword", HttpStatusCode.BadRequest);
            //if (!this.Request.Headers.TryGetValues("SafenetUri", out uri) ||
            //    string.IsNullOrEmpty(uri.FirstOrDefault<string>()))
            //  throw new SafenetException("Missing or empty header: SafenetUri", HttpStatusCode.BadRequest);

            //safenetClientCredential.UserName.UserName = userName.FirstOrDefault<string>();
            //safenetClientCredential.UserName.Password = password.FirstOrDefault<string>();
            //safenetUri = new Uri(uri.FirstOrDefault<string>());

            safenetClientCredential.UserName.UserName = System.Configuration.ConfigurationManager.AppSettings["safenetusername"];
            safenetClientCredential.UserName.Password = System.Configuration.ConfigurationManager.AppSettings["safenetpassword"];
            safenetUri = new Uri(System.Configuration.ConfigurationManager.AppSettings["safeneturi"]);

        }

        #endregion
    }
}
