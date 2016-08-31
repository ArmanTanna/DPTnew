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

    [Authorize(Roles = "Admin,Internal,VarExp")]
    public class OrderController : BaseController
    {

        public ActionResult Index(int pageSize = 10)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(_db.Orders.Select(x => x.SalesRep).Distinct().ToList()).ToString(Formatting.None));
            ViewBag.SalesReps = System.Convert.ToBase64String(plainTextBytes);
            return View();
        }

        [HttpPost]
        public JsonResult Search()
        {
            var sps = Request.GetSearchParams();
            var items = GetOrders();
            foreach (var sp in sps)
            {
                items = _db.Search<Order>(sp, items);
            }
            return Json(_db.ConvertToSearchResult<Order>(sps.FirstOrDefault(), items), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult NewOrderRow(Order orderRow)
        {
            using (var db = new DptContext())
            {
                var userName = Membership.GetUser().UserName;
                var user = db.Contacts.Where(u => u.Email == userName).ToList().FirstOrDefault();
                var company = db.Companies.Where(u => u.AccountNumber == user.AccountNumber).ToList().FirstOrDefault();
                var salesRep = db.SalesR.Where(u => u.Invoicer == company.AccountName).Select(u => u.SalesRep).ToList();
                List<String> companyList = new List<string>();
                if (salesRep.Count == 0)
                    companyList.AddRange(db.Companies.Where(x => x.SalesRep == company.SalesRep).OrderBy(k => k.AccountName).Select(u => u.AccountName + " \"" + u.AccountNumber + "\"").ToList());
                else
                {
                    foreach (var sr in salesRep)
                        companyList.AddRange(db.Companies.Where(x => x.SalesRep == sr).OrderBy(k => k.AccountName).Select(u => u.AccountName + " \"" + u.AccountNumber + "\"").ToList());
                }
                companyList.Sort();
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(companyList).ToString(Formatting.None));
                ViewBag.Companies = System.Convert.ToBase64String(plainTextBytes);
            }
            List<Order> rows = new List<Order>();
            rows.Add(orderRow);
            return View(rows);
        }

        [HttpPost]
        public ActionResult Insert(Order orderRow)
        {
            var company = _db.Companies.Where(c => c.AccountName == orderRow.AccountName).FirstOrDefault();
            orderRow.AccountNumber = company.AccountNumber;
            var sr = _db.SalesR.Where(u => u.SalesRep == company.SalesRep).FirstOrDefault();
            orderRow.Invoicer = sr.Invoicer;
            orderRow.InvoicedNumber = sr.AccountNumber;
            orderRow.InvoicedName = company.SalesRep;
            orderRow.SalesRep = company.SalesRep;
            orderRow.idxx = _db.Orders.Max(o => o.idxx) + 1;
            return Json("Saved!", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetProducts(string companyName)
        {
            using (var db = new DptContext())
            {
                var company = db.Companies.Where(c => c.AccountName == companyName).FirstOrDefault();
                var products = db.Licenses.Where(u => u.AccountNumber == company.AccountNumber).Select(x => x.ProductName).Distinct().ToList();
                return Json(products, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetArtDetails(string companyName, string productName)
        {
            using (var db = new DptContext())
            {
                var company = db.Companies.Where(c => c.AccountName == companyName).FirstOrDefault();
                var artDetails = db.Licenses.Where(u => u.AccountNumber == company.AccountNumber && u.ProductName == productName).Select(x => x.ArticleDetail).Distinct().ToList();
                return Json(artDetails, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetLicenseIds(string companyName, string productName, string artDetail)
        {
            using (var db = new DptContext())
            {
                var company = db.Companies.Where(c => c.AccountName == companyName).FirstOrDefault();
                var licIds = db.Licenses.Where(u => u.AccountNumber == company.AccountNumber && u.ProductName == productName && u.ArticleDetail == artDetail).Select(x => x.LicenseID).ToList();
                return Json(licIds, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetLicenseType(string licenseId)
        {
            using (var db = new DptContext())
            {
                var licenseType = db.Licenses.Where(u => u.LicenseID == licenseId).Select(x => x.LicenseType).ToList();
                return Json(licenseType, JsonRequestBehavior.AllowGet);
            }
        }

    }
}