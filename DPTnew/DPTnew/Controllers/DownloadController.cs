using DPTnew.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DPTnew.Controllers
{
    [Authorize]
    public class DownloadController : Controller
    {
        //
        // GET: /Download/

        public ActionResult Index()
        {
            using (var db = new DptContext())
            {
                var user = Membership.GetUser().UserName;
                var contact = db.Contacts.Where(u => u.Email == user).ToList().First();
                var company = db.Companies.Where(u => u.AccountNumber == contact.AccountNumber).ToList().First();
                ViewBag.Message = company.SalesRegion.Trim().ToLower();

                var salesProv = db.SalesR.Where(x => x.SalesRegion.Trim().ToLower() == company.SalesRegion.Trim().ToLower()).Select(x => x.SalesProvince).FirstOrDefault();
                if (salesProv.Contains("italy"))
                    Localization.Resource.Culture = new CultureInfo("it-IT");
                else if (salesProv.Contains("japan"))
                    Localization.Resource.Culture = new CultureInfo("ja-JP");
                else
                    Localization.Resource.Culture = new CultureInfo("en-US");
                
                return View();
            }
        }

    }
}
