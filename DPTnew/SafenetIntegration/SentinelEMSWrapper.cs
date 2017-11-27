#region Copyright Solair PLM.
//
// All rights are reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
// Filename:    SentinelEMSWrapper.cs
//
// Author:      Marco D'Alessandro
// Date:        June 2014
//
#endregion
using Microsoft.JScript;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using SafenetIntegration.Safenet;
using SafenetIntegration.Safenet.BusinessObject;
using SafenetIntegration.Safenet.Exception;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.ServiceModel.Description;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace SafenetIntegration
{
    public class SentinelEMSWrapper
    {
        #region public properties
        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>
        /// The response. This field is used to store, in a string format, each response of a specific request sent to the Sugar CRM web service.
        /// </value>
        public string Response { get; private set; }

        public WebHeaderCollection Headers { get; private set; }
        /// <summary>
        /// Gets the response.
        /// </summary>
        /// <value>
        /// The response. This field is used to store, in a string format, each response of a specific request sent to the Sugar CRM web service.
        /// </value>
        public JObject JsonResponse { get; private set; }

        /// <summary>
        /// Gets the status of the request.
        /// </summary>
        /// <value>
        /// The status of the HttpWebRequest request.
        /// </value>
        public HttpStatusCode Status { get; private set; }

        /// <summary>
        /// Gets the Sugar CRM URI.
        /// </summary>
        /// <value>
        /// The URI of Sugar CRM web service.
        /// </value>
        public Uri SentinelEMSUri { get; private set; }

        /// <summary>
        /// Gets the authentication token needed to send subsequent request to Solair PLM web service.
        /// </summary>
        /// <value>
        /// The authentication token.
        /// </value>
        public string AuthToken { get; private set; }

        /// <summary>
        /// Gets the credential.
        /// </summary>
        /// <value>
        /// The credential to connect to Solair PLM web service.
        /// </value>
        public ClientCredentials Credentials { get; private set; }

        /// <summary>
        /// Gets a value indicating whether [is authenticated].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is authenticated]; otherwise, <c>false</c>.
        /// </value>
        public bool IsAuth { get; private set; }
        public string CookPath { get; private set; }
        public string CookDomain { get; private set; }
        public CookieCollection CookieCont { get; private set; }
        public string ProductKeyId { get; private set; }
        private const string VENDOR = SafenetUtilities.MXBWI;
        //private const string VENDOR = SafenetUtilities.DEMOMA;
        #endregion

        #region public methods

        public SentinelEMSWrapper()
        {
            //localhost is for test purpose only. Fiddler must be running during API call (.fiddler in url has been added in order to capture packet from visual studio)
            SentinelEMSUri = new Uri("http://localhost.fiddler:8081/");
            Credentials = new ClientCredentials();
            Credentials.UserName.UserName = "admin";
            Credentials.UserName.Password = "admin";
        }

        public SentinelEMSWrapper(Uri uri, ClientCredentials credentials)
        {
            SentinelEMSUri = uri;
            Credentials = new ClientCredentials();
            Credentials.UserName.UserName = credentials.UserName.UserName;
            Credentials.UserName.Password = credentials.UserName.Password;
        }

        public void SendSentinelEMSRequest(string method, string api, string data)
        {
            string url = SentinelEMSUri + "ems/v71/ws/" + api;
            try
            {
                HttpWebRequest request = CreateRequest(method, url, data);
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    Response = reader.ReadToEnd();
                    Headers = response.Headers;
                    Status = response.StatusCode;
                    if (!IsAuth)
                    {
                        //if not authenticated set cookies from auth response, otherwise if already authenticated cookie
                        //are set in CreateRequest(...)
                        CookieCont = response.Cookies;
                        if (CookieCont.Count <= 0)
                            throw new Exception();
                        GetSessionId();
                    }
                    //SetSessionIdCookie(request);
                    CheckStatus(Status, response);
                }
            }
            catch (WebException wexc)
            {
                HttpWebResponse r = (HttpWebResponse)wexc.Response;
                StreamReader reader = new StreamReader(r.GetResponseStream());
                throw new Exception(r.StatusCode + ": " + reader.ReadToEnd());
            }
            catch (Exception exc)
            {
                throw new Exception(url, exc);
            }
        }
        #endregion

        #region IWrapper Interface Implementation

        public void Authentication()
        {
            try
            {
                if (!IsAuth)
                {
                    Login();
                    if (AuthToken != "null") IsAuth = true;
                }
            }
            catch (Exception e)
            {
                throw new SafenetException(e.Message, HttpStatusCode.Unauthorized);
            }
        }

        public object ReadEntity(object T)
        {
            throw new System.NotImplementedException();
        }

        public object WriteEntity(object T)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteEntity(object T)
        {
            throw new System.NotImplementedException();
        }

        public object ListEntities()
        {
            throw new System.NotImplementedException();
        }

        public object ListRelations()
        {
            throw new NotImplementedException();
        }

        public object UpdateEntity(object T)
        {
            throw new System.NotImplementedException();
        }

        public object ListEntityFields(object T)
        {
            throw new System.NotImplementedException();
        }

        public object ListEntityRecords(object T)
        {
            throw new System.NotImplementedException();
        }

        public object GetRecordIdList(object T)
        {
            throw new System.NotImplementedException();
        }

        public object GetRecordID(object T)
        {
            throw new System.NotImplementedException();
        }

        public object GetLastModified(object T)
        {
            throw new System.NotImplementedException();
        }

        public bool IsAuthenticated()
        {
            throw new System.NotImplementedException();
        }

        public object FinalOperations(object T)
        {
            throw new System.NotImplementedException();
        }

        public object ReadRelation(object T)
        {
            throw new NotImplementedException();
        }

        public object WriteRelation(object T)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Authentication API

        //Use this method to log in to the Sentinel EMS Server and start a client session. A client
        //application must log on and obtain a sessionId before making any web services calls.
        //To call the login method, the client application needs to provide a user name and password
        //in XML format. The Sentinel EMS Server authenticates the credentials and returns a session ID
        //in response header to be used in subsequent calls.
        public void Login()
        {
            //string auth = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
            //               "<AuthenticationDetail xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" >" +
            //               "<userName>" + Credentials.UserName.UserName + "</userName>" +
            //               "<password>" + Credentials.UserName.Password + "</password>" +
            //               "</AuthenticationDetail>";

            //<?xml version="1.0" encoding="utf-16"?><AuthenticationDetail xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"><userName>admin</userName><password>admin</password></AuthenticationDetail>
            authenticationDetail authModel = new authenticationDetail();
            authModel.userName = Credentials.UserName.UserName;
            authModel.password = Credentials.UserName.Password;
            string xml = Serialize(authModel);
            SendSentinelEMSRequest("POST", "login.ws", xml);
            FormatResponse();
        }

        public static string Serialize<T>(T value)
        {
            if (value == null)
            {
                return null;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
            settings.Indent = false;
            settings.OmitXmlDeclaration = false;
            using (StringWriter textWriter = new StringWriter())
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
                {
                    serializer.Serialize(xmlWriter, value);
                }
                return textWriter.ToString();
            }
        }

        public static T Deserialize<T>(string xml)
        {

            if (string.IsNullOrEmpty(xml))
            {
                return default(T);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            XmlReaderSettings settings = new XmlReaderSettings();
            // No settings need modifying here

            using (StringReader textReader = new StringReader(xml))
            {
                using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
                {
                    return (T)serializer.Deserialize(xmlReader);
                }
            }
        }
        public void LoginByProductKey(string productKey)
        {
            //98cca4ff-48c7-4b8a-8a74-6eb7f9213401
            //string pk = "98cca4ff-48c7-4b8a-8a74-6eb7f9213401";
            SendSentinelEMSRequest("POST", "loginByProductKey.ws?productKey=" + productKey, null);
            FormatResponse();
        }

        public void Logout()
        {
            SendSentinelEMSRequest("POST", "logout.ws", null);
            FormatResponse();
            IsAuth = false;
        }
        #endregion

        #region Vendor API

        //Returns the list of available Enforcements.
        public void GetAllEnforcement(string vendorId)
        {
            CheckAuth();
            SendSentinelEMSRequest("GET", "enforcement.ws?vendorId=" + vendorId, null);
            FormatResponse();
        }

        //Returns the list of available Vendor IDs.
        public void GetAllVendorsByEnforcementId(string enforcementId)
        {
            CheckAuth();
            //In this release of Sentinel EMS, the only Enforcement ID available is Sentinel LDK (with enforcementId = 1). In future releases, other Enforcements like Sentinel RMS will also be supported.
            SendSentinelEMSRequest("GET", "enforcement/" + enforcementId + "/vendor.ws", null);
            FormatResponse();
        }

        //Returns a list of all Vendor IDs available in Sentinel EMS.
        public void GetAllVendors()
        {
            CheckAuth();
            SendSentinelEMSRequest("GET", "vendor.ws", null);
            FormatResponse();
        }

        //Returns a list of Vendor IDs for a given User ID.
        public void GetAllVendorsByUserId(string userId)
        {
            CheckAuth();
            SendSentinelEMSRequest("GET", "vendor.ws?userId=" + userId, null);
            FormatResponse();
        }

        //Returns a file containing the Feature Name and Feature ID of all defined Features, together with the Product Names and Product IDs in which they are incorporated.
        //The output file type. The options are:
        //CSV — Returns Features in CSV format.
        //XML — Returns Features and Products in XML format.
        //C — Returns Features and Products in C-style header.
        //CPP — Returns Features and Products in CPP-style header.
        public void ExportDefinition(string vendorId, string fileType)
        {
            CheckAuth();
            SendSentinelEMSRequest("GET", "vendor/" + vendorId + "/exportDefinition.ws?fileType=" + fileType, null);
            FormatResponse();
        }

        //Bundle Provisional Products.
        public void BundleProvisional(string vendorId, string prodIds)
        {
            CheckAuth();
            SendSentinelEMSRequest("GET", "vendor/" + vendorId + "/bundleProvisional.ws?prodIds=" + prodIds, null);
            FormatResponse();
        }
        #endregion

        #region Viewing License Model Details API

        //Get the license model details for a given License Model ID.
        public void GetLicenseModelById(int licensModelId)
        {
            CheckAuth();
            SendSentinelEMSRequest("GET", "licenseModel/" + licensModelId + ".ws", null);
            FormatResponse();
        }

        //Get the license model details for a given License Model ID.
        public void GetAllLicenseModels(string enforcementId)
        {
            CheckAuth();
            SendSentinelEMSRequest("GET", "enforcement/" + enforcementId + "/licenseModel.ws", null);
            FormatResponse();
        }
        #endregion

        #region Feature API

        //Create a new Feature for specified Vendor.
        public void CreateFeature(string vendorId, string feature)//, string fName, string fId, string fVersion, string fDescription, string fRefId1, string fRefId2)
        {
            CheckAuth();
            //string feature = BuildFeature(fName, fId, fVersion, fDescription, fRefId1, fRefId2);
            string featureXml = BuildFeature(feature);
            SendSentinelEMSRequest("PUT", "vendor/" + vendorId + "/feature.ws", featureXml);
            FormatResponse();
        }

        //Edit the properties of a Feature. Consider the following points before editing a Feature:
        //If the Feature is not deployed (not included in a Product yet), you can edit the Feature properties like Name, Feature ID, Ref IDs, and Description.
        //If the Feature is deployed (included in one or more Products), you can change only the Ref IDs and Description.
        public void EditFeature(string featureId, string feature)
        {
            CheckAuth();
            string featureXml = BuildFeature(feature);
            //string feature = BuildFeature(fName, fId, fVersion, fDescription, fRefId1, fRefId2);
            SendSentinelEMSRequest("POST", "feature/" + featureId + ".ws", featureXml);
            FormatResponse();
        }

        //Delete a feature for a given Feature ID. It's an internal ID not the Identifier set in CreateFeature.
        public void DeleteFeature(string featureId)
        {
            CheckAuth();
            SendSentinelEMSRequest("DELETE", "feature/" + featureId + ".ws", null);
            FormatResponse();
        }

        //Get the details of a Feature for a given Feature ID.
        public void GetFeatureById(string featureId)
        {
            CheckAuth();
            SendSentinelEMSRequest("GET", "feature/" + featureId + ".ws", null);
            FormatResponse();
        }

        //Search Features in a vendorId for the given parameters.
        public void SearchFeatures(string vendorId, string featureName, string refId1, string refId2)
        {
            CheckAuth();
            SendSentinelEMSRequest("GET", "vendor/" + vendorId
                                        + "/feature.ws?featureName=" + featureName
                                        + "&refId1=" + refId1
                                        + "&refId2=" + refId2, null);
            FormatResponse();
        }
        #endregion

        #region Product API

        //Create a new Product for specified Vendor.
        public void CreateProduct(string vendorId, string product)
        {
            CheckAuth();
            string productXml = BuildProduct(product);
            SendSentinelEMSRequest("PUT", "vendor/" + vendorId + "/product.ws", productXml);
            FormatResponse();
        }

        //Edit the properties of a Product. Consider the following points before editing:
        //If the Product is not deployed (not included in a Entitlement yet), you can edit the Product properties like Name, Product ID, Ref IDs, and Description.
        //If the Product is deployed (included in one or more Entitlements), you can change only the Ref IDs and Description.
        public void EditProduct(string productId, string product)
        {
            CheckAuth();
            BuildProduct(product);
            SendSentinelEMSRequest("POST", "product/" + productId + "/.ws", product);
            FormatResponse();
        }

        //Delete a Product for a given Product ID.
        public void DeleteProduct(string productId)
        {
            CheckAuth();
            SendSentinelEMSRequest("DELETE", "product/" + productId + ".ws", null);
            FormatResponse();
        }

        //Get the details of a Product for a given Product ID.
        public void GetProductById(string productId)
        {
            CheckAuth();
            SendSentinelEMSRequest("GET", "product/" + productId + ".ws", null);
            FormatResponse();
        }

        //Search the list of Products in a vendorId by the provided parameters.
        public void SearchProducts(string[] searchParams, string vendorId = SafenetUtilities.DEMOMA)
        {
            //string vendorId,
            //string productName,
            //string lifeCycleStage,
            //string refId1,
            //string refId2

            CheckAuth();
            string queryString = String.Join("&", searchParams);

            //SendSentinelEMSRequest("GET", "vendor/" + vendorId
            //                        + "/product.ws?"
            //                        + "productName=" + productName
            //                        + "&lifeCycleStage=" + lifeCycleStage
            //                        + "&refId1=" + refId1
            //                        + "&refId2=" + refId2, null);
            SendSentinelEMSRequest("GET", "vendor/" + vendorId + "/product.ws?" + queryString, null);
            FormatResponse();
        }
        #endregion

        #region Managing Entitlement API

        //Create an Entitlement using the parameters provided.
        public void CreateEntitlement(JObject entitlement)
        {
            CheckAuth();
            string entitlementXml = BuildEntitlement(entitlement);
            SendSentinelEMSRequest("PUT", "entitlement.ws", entitlementXml);
            FormatResponse();
        }

        //Create an Entitlement using the parameters provided.
        public void CheckInC2V(JObject checkInC2V)
        {
            CheckAuth();
            string c2vCheckInXml = BuildC2VCheckIn(checkInC2V);
            SendSentinelEMSRequest("POST", "target.ws", c2vCheckInXml);
            FormatResponse();
        }

        //Update Entitlement details for a given entId. You can edit an Entitlement only if it is not yet produced.
        public void UpdateEntitlement(string entId, JObject entitlement)
        {
            CheckAuth();
            string entitlementXml = BuildEntitlement(entitlement);
            SendSentinelEMSRequest("POST", "entitlement/" + entId + ".ws", entitlementXml);
            FormatResponse();
        }

        //Delete an Entitlement for given entId.
        public void DeleteEntitlement(string entId)
        {
            CheckAuth();
            SendSentinelEMSRequest("DELETE", "entitlement/" + entId + ".ws", null);
            FormatResponse();
        }

        //Get the list of line items, with Product(s), Features(s), and License Model, for a given Entitlement ID.
        public void GetEntitlementById(string entId)
        {
            CheckAuth();
            SendSentinelEMSRequest("GET", "entitlement/" + entId + ".ws", null);
            FormatResponse();
        }

        //Search Entitlements by the given query parameters.
        public void SearchEntitlements(string[] searchParams)
        {
            //string entStatus,
            //string refId1, 
            //string refId2, 
            //string customerId, 
            //string cstmrname, 
            //string emailId, 
            //string vendorId, 
            //string enforcementId
            CheckAuth();
            string queryString = String.Join("&", searchParams);
            //SendSentinelEMSRequest("GET", "entitlement.ws?entStatus=" + entStatus
            //                                + "&refId1=" + refId1
            //                                + "&refId2=" + refId2
            //                                + "&customerId" + customerId
            //                                + "&cstmrname=" + cstmrname
            //                                + "&cstmrname=" + emailId
            //                                + "&vendorId=" + vendorId
            //                                + "&enforcementId=" + enforcementId, null);
            SendSentinelEMSRequest("GET", "entitlement.ws?" + queryString, null);
            FormatResponse();
        }

        #endregion

        #region ProductKey and Activation API

        //Get the details of a given Product key.
        public void GetProductKeyById(string productKeyId)
        {
            CheckAuth();
            SendSentinelEMSRequest("GET", "productKey/" + productKeyId + ".ws", null);
            FormatResponse();
            //maybe istantiate a productkey: productkey id of result is the real key to send to the user!
        }

        //Update Entitlement details for a given entId. You can edit an Entitlement only if it is not yet produced.
        public void UpdateProductKey(string productKeyId, string productKey)
        {
            CheckAuth();
            string productKeyXml = BuildProductKey(productKey);
            SendSentinelEMSRequest("POST", "productKey/" + productKeyId + ".ws", productKeyXml);
            FormatResponse();
        }

        //This web service returns the Activation (XML) for a given Product Key. The <activationString> section contains the V2C in XML-encoded format.
        public void GenerateLicense(JObject data)
        {
            CheckAuth();
            string c2v = data["C2V"].Value<string>();
            string eid = data["Eid"].Value<string>();

            bool encoded = true;
            JToken jt;
            if (data.TryGetValue("Encoded", out jt))
                encoded = data["Encoded"].Value<bool>();
            this.GetEntitlementById(eid);
            string productKeyId = JsonResponse["entitlement"]["entitlementItem"]["productKeyId"].Value<string>();
            string c2vXml = "";
            switch (data["EntType"].Value<string>())
            {
                case SafenetUtilities.PROTECTIONKEY_UPDATE:
                    string protectionKeyId = JsonResponse["entitlement"]["entitlementItem"]["protectionKeyId"].Value<string>();
                    c2vXml = BuildC2V("", true, protectionKeyId);
                    break;
                default:
                    string decodedC2v = encoded ? System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(c2v)) : c2v;
                    c2vXml = System.Uri.UnescapeDataString(BuildC2V(decodedC2v));
                    break;
            }

            SendSentinelEMSRequest("POST", "productKey/" + productKeyId + "/activation.ws", c2vXml);
            FormatResponse();
        }

        //Returns the License in XML format for given Activation ID (aid).
        public void GetLicenseByAid(string aid)
        {
            CheckAuth();
            SendSentinelEMSRequest("GET", "activation/" + aid + ".ws", null);
            FormatResponse();
        }

        #endregion

        #region Customer and Contact API

        //Create a new Customer.
        public void CreateCustomer(JObject customer)
        {
            CheckAuth();
            if (CustomerExist(new string[] { "crmId=" + customer["CrmId"].Value<string>(), "vendorId=" + ((customer["ActualBatchCode"].Value<string>() == "DEMOMA") ? SafenetUtilities.DEMOMA : VENDOR) }))
                throw new SafenetException("Customer with CRM-ID: " + customer["CrmId"].Value<string>() + "already exists", HttpStatusCode.BadRequest);
            string customerXml = BuildCustomer(customer);
            try
            {
                SendSentinelEMSRequest("PUT", "customer.ws", customerXml);
                FormatResponse();
            }
            catch (Exception e)
            {
                throw new SafenetException(e.Message, HttpStatusCode.BadRequest);
            }
        }

        public JObject CreateCustomerBatch(JObject customers)
        {
            CheckAuth();
            JArray customerList = customers["CustomerList"].Value<JArray>();
            JObject result = new JObject();
            result.Add("CustomerResults", new JArray());

            for (int i = 0; i < customerList.Count; i++)
            {
                this.CreateCustomer(customerList[i].Value<JObject>());
                ((JArray)result["CustomerResults"]).Add(this.JsonResponse);
            }
            return result;
        }

        private bool CustomerExist(string[] searchParams)
        {
            SearchCustomers(searchParams);
            return (JsonResponse["listResponse"]["@count"].Value<int>() > 0);
        }

        //Update the details of a Customer.
        public void UpdateCustomer(JObject customer)
        {
            CheckAuth();
            string[] searchCustParams = new string[] { "crmId=" + customer["CrmId"].Value<string>(), "vendorId=" +((customer["ActualBatchCode"].Value<string>() == "DEMOMA") ? SafenetUtilities.DEMOMA : VENDOR) };
            if (!CustomerExist(searchCustParams))
                throw new SafenetException("Customer with CRM-ID: " + customer["CrmId"].Value<string>() + " DOES NOT exist", HttpStatusCode.BadRequest);
            this.SearchCustomers(searchCustParams);

            string customerId = JsonResponse["listResponse"]["instance"]["@customerId"].Value<string>();

            string customerXml = BuildCustomer(customer);
            SendSentinelEMSRequest("POST", "customer/" + customerId + ".ws", customerXml);
            FormatResponse();
        }

        //Delete a Customer for a given customer ID.
        public void DeleteCustomer(string customerId)
        {
            CheckAuth();
            SendSentinelEMSRequest("DELETE", "customer/" + customerId + ".ws", null);
            FormatResponse();
        }

        //Returns the details of a Customer by Customer ID.
        public void GetCustomerById(string customerId)
        {
            CheckAuth();
            SendSentinelEMSRequest("GET", "customer/" + customerId + ".ws", null);
            FormatResponse();
        }

        //Search customers by the given query parameters.
        public void SearchCustomers(string[] searchParams)
        {
            //string customerName = "", 
            //                          string crmId ="",
            //                          string refId="", 
            //                          string pageIndex="",
            //                          string pageSize="",
            //                          string sortCol="",
            //                          string sortOrder="",
            //                          string vendorId=""
            CheckAuth();
            string queryString = String.Join("&", searchParams);
            //SendSentinelEMSRequest("GET", "customer.ws?"
            //                                + "customerName=" + customerName
            //                                + "&crmId=" + crmId
            //                                + "&refId=" + refId
            //                                + "&pageIndex=" + pageIndex
            //                                + "&pageSize=" + pageSize
            //                                + "&sortCol=" + sortCol
            //                                + "&sortOrder=" + sortOrder
            //                                + "&vendorId=" + vendorId, null);
            SendSentinelEMSRequest("GET", "customer.ws?" + queryString, null);
            FormatResponse();
        }

        //Returns a list of Contact for the given Customer ID.
        public void GetContactByCustomerId(string customerId)
        {
            CheckAuth();
            SendSentinelEMSRequest("GET", "customer/" + customerId + "contact.ws", null);
            FormatResponse();
        }

        //Create a new Contact.
        public void CreateContact(string customerId, string contact)
        {
            CheckAuth();
            string contactXml = BuildContact(contact);
            SendSentinelEMSRequest("PUT", "customer/" + customerId + "/contact.ws", contactXml);
            FormatResponse();
        }

        //Update the details of a Contact.
        public void UpdateContact(string contactId, string contact)
        {
            CheckAuth();
            string contactXml = BuildContact(contact);
            SendSentinelEMSRequest("POST", "contact/" + contactId + ".ws", contactXml);
            FormatResponse();
        }

        //Delete a Contact for a given Contact ID.
        public void DeleteContact(string contactId)
        {
            CheckAuth();
            SendSentinelEMSRequest("DELETE", "contact/" + contactId + ".ws", null);
            FormatResponse();
        }

        //Returns the details of a Contact by Contact ID.
        public void GetContactByContactId(string contactId)
        {
            CheckAuth();
            SendSentinelEMSRequest("GET", "contact/" + contactId + ".ws", null);
            FormatResponse();
        }

        //Returns the details of a Contact by E-mail ID (in e-mail format).
        public void GetContactByContactEmailId(string contactEmailId)
        {
            CheckAuth();
            SendSentinelEMSRequest("GET", "contact.ws?contactEmailId=" + contactEmailId, null);
            FormatResponse();
        }

        //Search Contacts by the given query parameters.
        public void SearchContacts(string firstName, string lastName, string pageIndex, string pageSize, string sortCol, string sortOrder)
        {
            CheckAuth();
            SendSentinelEMSRequest("GET", "contact.ws?firstName=" + firstName
                                            + "&lastName=" + lastName
                                            + "&pageIndex=" + pageIndex
                                            + "&pageSize=" + pageSize
                                            + "&sortCol=" + sortCol
                                            + "&sortOrder=" + sortOrder, null);
            FormatResponse();
        }

        #endregion

        #region private

        private void FormatResponse()
        {
            JsonResponse = new JObject();
            if (Response != "")
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(Response);
                JsonResponse = JObject.Parse(Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc));
                //string hhh = SafenetUtilities.GetHeadersSerialized(Headers);
            }
            if (Headers.Count != 0)
            {
                JsonResponse.Add("Headers", SafenetUtilities.GetHeaders(Headers));
            }
        }

        private void GetSessionId()
        {
            foreach (Cookie cook in CookieCont)
            {
                if (cook.Name == "JSESSIONID")
                {
                    AuthToken = cook.Value;
                    CookPath = cook.Path;
                    CookDomain = cook.Domain;
                }
            }
        }

        private void SetSessionIdCookie(HttpWebRequest request)
        {
            request.CookieContainer.Add(SentinelEMSUri, new Cookie("JSESSIONID", AuthToken, "/", CookDomain));
        }

        private void CheckAuth()
        {
            if (!IsAuth)
                Authentication();
        }

        private void CheckStatus(HttpStatusCode status, HttpWebResponse response)
        {
            switch (status)
            {
                case HttpStatusCode.NoContent:
                case HttpStatusCode.OK:
                case HttpStatusCode.Created:
                case HttpStatusCode.NonAuthoritativeInformation:
                    break;
                case HttpStatusCode.BadRequest:
                case HttpStatusCode.Unauthorized:
                case HttpStatusCode.Forbidden:
                case HttpStatusCode.NotFound:
                case HttpStatusCode.MethodNotAllowed:
                    throw new Exception(response.StatusDescription);
                default: throw new Exception(response.StatusDescription);
            }
        }

        //private string BuildFeature(string fName, string fId, string fVersion, string fDescription, string fRefId1, string fRefId2)
        private string BuildFeature(string feature)
        {
            //FOR TESTING PURPOSE. Info will be extracted from feature.
            feature featureModel = new feature();
            featureModel.featureName = "TestFeature";
            featureModel.featureDescription = "A test from API";
            featureModel.featureIdentifier = 10000;
            featureModel.refId1 = "ONE";
            featureModel.refId2 = "TWO";
            return Serialize(featureModel);
        }

        private string BuildProduct(string product)
        {
            //FOR TESTING PURPOSE. Info will be extracted from product.
            product productModel = new product();
            productModel.productName = "TEST PRODUCT FROM API";
            productModel.baseProductId = 0;
            productModel.productIdentifier = 1000;
            productModel.LifeCycleStage = productLifeCycleStage.DRAFT;
            productModel.EnforcementProductType = productEnforcementProductType.BASE;
            productModel.EnforcementProtectionType = productEnforcementProtectionType.SL_AdminMode;
            productModel.rehost = productRehost.DISABLE;
            productModel.productDescription = "a test from api";
            productFeatureRef _pfr = new productFeatureRef();
            _pfr.featureId = 9;
            _pfr.actionName = productFeatureRefActionName.NONE;
            _pfr.productFeatureLicenseModel.actionName = productFeatureLicenseModelActionName.NONE;
            _pfr.productFeatureLicenseModel.licenseModel.licenseModelId = 1;
            _pfr.productFeatureLicenseModel.licenseModel.licenseModelName = "HASP Execution Count";
            _pfr.productFeatureLicenseModel.licenseModel.licenseType = 0;
            lmAttribute _lma = new lmAttribute();
            _lma.attribute.attributeId = 1;
            _lma.attribute.attributeName = "LICENSE_TYPE";
            _lma.attribute.attributeType = 7;
            _lma.attribute.attributeValueChoice.Add("Time Period");
            _lma.attribute.attributeValueChoice.Add("Executions");
            _lma.attribute.attributeValueChoice.Add("Expiration Date");
            _lma.attribute.attributeValueChoice.Add("Perpetual");
            _lma.attribute.displayOrder = 1;
            _lma.attribute.attributeLevel = 1;
            _lma.attribute.endUserPermission = 0;
            _lma.attribute.parameterGroupName = "License Terms";
            _lma.attribute.isvPermission = attributeIsvPermission.WRITE;
            _lma.attribute.nullable = false;
            _lma.attribute.saotAllowed = true;
            _lma.attributeValue = "1";
            _lma.saot = false;
            _pfr.productFeatureLicenseModel.licenseModel.lmAttribute.Add(_lma);
            _pfr.lmStatus = productFeatureRefLmStatus.DEFINED;
            _pfr.excludable = false;
            productModel.productFeatureRef.Add(_pfr);
            return Serialize(productModel);
        }

        private string BuildC2VCheckIn(JObject data)
        {
            string c2v = data["C2V"].Value<string>();
            bool encoded = true;
            JToken jt;
            if (data.TryGetValue("Encoded", out jt))
                encoded = data["Encoded"].Value<bool>();
            string decodedC2v = encoded ? System.Text.Encoding.UTF8.GetString(System.Convert.FromBase64String(c2v)) : c2v;
            string c2vXml = System.Uri.UnescapeDataString(decodedC2v);
            string protectionkey = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><ProtectionKey><ProtectionKeyInput><action>Checkin</action><C2V>" + c2vXml + "</C2V></ProtectionKeyInput></ProtectionKey>";
            return protectionkey;
        }

        private string BuildEntitlement(JObject entitlement)
        {
            //FOR TESTING PURPOSE. Info will be extracted from entitlement.
            entitlement entitlementModel = new entitlement();
            entitlementModel.enabled = true;
            entitlementModel.startDate = DateTime.UtcNow;
            entitlementModel.refId1 = entitlement["refId1"].Value<string>();
            entitlementModel.refId2 = entitlement["refId2"].Value<string>();
            //entitlementModel.endDate = new DateTime(2500, 12, 31);
            //search for customer
            string[] searchCustParams = new string[] { "crmId=" + entitlement["CrmId"].Value<string>(), "vendorId=" + VENDOR };
            if (!CustomerExist(searchCustParams))
                throw new SafenetException("Customer with CRM-ID: " + entitlement["CrmId"].Value<string>() + " DOES NOT exist", HttpStatusCode.BadRequest);
            this.SearchCustomers(searchCustParams);

            string custId = JsonResponse["listResponse"]["instance"]["@customerId"].Value<string>();
            string custEmail = JsonResponse["listResponse"]["instance"]["@defaultEmail"].Value<string>();

            entitlementModel.customerId = custId;
            entitlementModel.customerEmail = custEmail;
            //
            entitlementModel.policy.registrationRequired = entitlementPolicyRegistrationRequired.NOT_REQUIRED;
            entitlementItem _eI = new entitlementItem();
            switch (entitlement["EntType"].Value<string>())
            {
                case SafenetUtilities.PRODUCT_KEY:
                    entitlementModel.action = entitlementAction.PRODUCE;
                    _eI.lineItemType = entitlementItemLineItemType.Product_Key;
                    break;
                case SafenetUtilities.PROTECTIONKEY_UPDATE:
                    entitlementModel.action = entitlementAction.COMMIT;
                    //          entitlementModel.action = entitlementAction.PRODUCE;
                    _eI.protectionKeyId.Add(entitlement["ProtectionKeyId"].Value<string>());
                    _eI.lineItemType = entitlementItemLineItemType.ProtectionKey_Update;
                    break;
                case SafenetUtilities.HARDWARE_KEY:
                    _eI.lineItemType = entitlementItemLineItemType.Hardware_Key;
                    break;
                default:
                    _eI.lineItemType = entitlementItemLineItemType.Product_Key;
                    break;
            }
            _eI.numActivationPerProductKey = "1";
            _eI.numProductKeys = "1";
            _eI.vendorId = VENDOR;
            _eI.enforcementId = SafenetUtilities.ENFORCEMENT_SENTINEL_LDK;
            //Search for product
            //TODO handle array of product
            JArray productList = entitlement["ProductName"].Value<JArray>();
            for (int i = 0; i < productList.Count; i++)
            {
                string dptProductAssociation = SafenetUtilities.Instance.PRODUCT_LIST_DPTASSOCIATION[productList[i].Value<string>().ToLower()];
                itemProduct _iP = new itemProduct();
                this.SearchProducts(new string[] { "productName=" + dptProductAssociation + "&lifeCycleStage=Commit" }, VENDOR);
                string pId = JsonResponse["listResponse"]["instance"]["@id"].Value<string>();
                this.GetProductById(pId);
                _iP.productId = pId; //tocheck
                product currentProd = Deserialize<product>(this.Response);
                //product aaa = JsonConvert.DeserializeObject<product>(this.JsonResponse.ToString());
                //product bbb = this.JsonResponse.ToObject<product>();
                for (int j = 0; j < currentProd.productFeatureRef.Count; j++)
                {

                    var saotAttribute = currentProd.productFeatureRef[j].productFeatureLicenseModel.licenseModel.lmAttribute.Where(x => x.saot);
                    foreach (lmAttribute attr in saotAttribute)
                    {
                        string value = entitlement["SaotParams"].Where(x => SafenetUtilities.Instance.PRODUCT_LIST_DPTASSOCIATION[x["Product"].Value<string>().ToLower()].Equals(currentProd.productName)).FirstOrDefault()[attr.attribute.attributeName].Value<string>();
                        if (attr.attribute.attributeName.Equals(SafenetUtilities.CONCURRENT_ISTANCES))
                        {
                            int concurrentValue = 0;
                            if (!Int32.TryParse(value, out concurrentValue))
                                value = SafenetUtilities.FormatConcurrentIstances(concurrentValue);

                        }
                        attr.attributeValue = value;
                    }
                }
                _iP.product = currentProd;
                _eI.itemProduct.Add(_iP);

            }
            activationAttribute _aA = new activationAttribute();
            _aA.attributeName = activationAttributeAttributeName.C2V;
            _aA.attributeValue = "false";
            _eI.activationAttribute.Add(_aA);
            entitlementModel.entitlementItem.Add(_eI);
            return Serialize(entitlementModel);
        }

        private string BuildCustomer(JObject customer)
        {
            //FOR TEST PURPOSE. Info will be extracted from customer.
            customer customerModel = new customer();
            customerModel.type = customerTypeEnum.org;
            customerModel.name = customer["CompanyName"].Value<string>();
            customerModel.defaultContact.emailId = customer["Email"].Value<string>();
            string name = customer["FirstName"].Value<string>();
            if (!string.IsNullOrEmpty(name))
                customerModel.defaultContact.firstName = name;
            string surname = customer["LastName"].Value<string>();
            if (!string.IsNullOrEmpty(surname))
                customerModel.defaultContact.lastName = surname;
            customerModel.defaultContact.locale = SafenetUtilities.GetLocale(customer["Locale"].Value<string>());
            customerModel.enabled = true;
            customerModel.crmId = customer["CrmId"].Value<string>();
            if (customer["UpdateBatchCode"].Value<string>() == "DEMOMA")
                customerModel.vendorId = SafenetUtilities.DEMOMA;
            else
                customerModel.vendorId = VENDOR;
            customerModel.description = customer["Description"].Value<string>();
            return Serialize(customerModel);
        }

        private string BuildContact(string contact)
        {
            //FOR TESTING PURPOSE. Info will be extracted from contact.
            contact contactModel = new contact();
            contactModel.emailId = "breakingbad@gmail.com";
            contactModel.firstName = "Bryan";
            contactModel.middleName = "Heisenberg";
            contactModel.lastName = "Cranston";
            contactModel.locale = "de";
            contactModel.enabled = true;
            contactModel.@default = true;
            return Serialize(contactModel);
        }

        private string BuildProductKey(string productKey)
        {
            //FOR TESTING PURPOSE. Info will be extracted from productKey.
            productKey productKeyModel = new productKey();
            productKeyModel.productKeyId = "92643a67-8fe9-49a2-b2b3-b21024de37e4";
            productKeyModel.totalEntitled = "1";
            productKeyModel.available = "1";
            productKeyModel.enabled = true;
            productKeyModel.type = "Product Key-based";
            productKeyProductInfo _pKpI = new productKeyProductInfo();
            _pKpI.id = "1";
            _pKpI.productName = "prd1";
            _pKpI.type = "BASE";
            productKeyModel.productInfo.Add(_pKpI);
            activationAttribute _aA1 = new activationAttribute();
            _aA1.attributeName = activationAttributeAttributeName.ACKNOWLEDGEMENT_REQUEST;
            _aA1.attributeValue = "true";
            _aA1.isvPermission = activationAttributeIsvPermission.WRITE;
            _aA1.endUserPermission = activationAttributeEndUserPermission.NONE;
            activationAttribute _aA2 = new activationAttribute();
            _aA2.attributeName = activationAttributeAttributeName.CLEAR_BEFORE_APPLYING_UPDATE;
            _aA2.attributeValue = "null";
            _aA2.isvPermission = activationAttributeIsvPermission.NONE;
            _aA2.endUserPermission = activationAttributeEndUserPermission.WRITE;
            activationAttribute _aA3 = new activationAttribute();
            _aA3.attributeName = activationAttributeAttributeName.C2V;
            _aA3.attributeValue = "null";
            _aA3.isvPermission = activationAttributeIsvPermission.NONE;
            _aA3.endUserPermission = activationAttributeEndUserPermission.WRITE;
            productKeyModel.activationAttribute.Add(_aA1);
            productKeyModel.activationAttribute.Add(_aA2);
            productKeyModel.activationAttribute.Add(_aA3);
            return Serialize(productKeyModel);
        }

        private string BuildC2V(string c2v, bool protectionUpdate = false, string protectionKeyId = "")
        {
            //FOR TESTING PURPOSE. Info will be extracted from c2v.
            activation activationModel = new activation();
            activationInput _aI = new activationInput();
            activationAttribute _aA = new activationAttribute();
            _aA.attributeValue = protectionUpdate ? protectionKeyId : GlobalObject.escape("<![CDATA[<?xml version=\"1.0\" encoding=\"UTF-8\"?>" + c2v + "]]>");
            _aA.attributeName = protectionUpdate ? activationAttributeAttributeName.ProtectionKeyId : activationAttributeAttributeName.C2V;
            _aI.activationAttribute.Add(_aA);
            _aI.sendMail = true;
            activationModel.activationInput = _aI;
            return Serialize(activationModel);
        }

        private HttpWebRequest CreateRequest(string method, string url, string data)
        {
            HttpWebRequest request = null;
            switch (method)
            {
                case "GET":
                case "POST":
                case "PUT":
                case "DELETE":
                    request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = method;
                    request.ContentType = "text/xml; charset=utf-8";
                    request.CookieContainer = new CookieContainer();
                    if (IsAuth)
                        SetSessionIdCookie(request);
                    if (data != null)
                    {
                        request.ContentLength = data.Length;
                        using (var requestWriter = new StreamWriter(request.GetRequestStream()))
                        {
                            requestWriter.Write(data);
                        }
                    }
                    break;
            }
            return request;
        }

        #endregion

        #region dptregion
        public void UpdateDptProducts()
        {
            this.SearchProducts(new string[] { }, SafenetUtilities.MXBWI);
            //this.SearchProducts(new string[] { }, SafenetUtilities.DEMOMA);
            SafenetUtilities.Instance.UpdateProducts(this.JsonResponse);
        }
        #endregion


        public bool CheckExistCustomer(JObject data)
        {
            CheckAuth();
            string[] searchCustParams = new string[] { "crmId=" + data["CrmId"].Value<string>(), "vendorId=" + ((data["ActualBatchCode"].Value<string>() == "DEMOMA") ? SafenetUtilities.DEMOMA : VENDOR) };
            return CustomerExist(searchCustParams);

        }
    }
}
