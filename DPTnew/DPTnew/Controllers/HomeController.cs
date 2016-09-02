using DPTnew.Helper;
using DPTnew.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace DPTnew.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Session["CurrentCulture"] = LocalizationHelper.SetLocalization(Session["CurrentCulture"]);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult ChangeCurrentCulture(int id)
        {
            //  
            // Change the current culture for this user.  
            //  
            CultureHelper.CurrentCulture = id;
            switch (id)
            {
                case 1: Localization.Resource.Culture = new CultureInfo("it-IT");
                    break;
                case 2: Localization.Resource.Culture = new CultureInfo("ja-JP");
                    break;
                case 3: Localization.Resource.Culture = new CultureInfo("ko-KR");
                    break;
                case 4: Localization.Resource.Culture = new CultureInfo("de-DE");
                    break;
                case 0:
                default: Localization.Resource.Culture = new CultureInfo("en-US");
                    break;
            }
            //  
            // Cache the new current culture into the user HTTP session.   
            //  
            Session["CurrentCulture"] = id;
            //  
            // Redirect to the same page from where the request was made!   
            //  
            return Redirect(Request.UrlReferrer.ToString());
        }

    }
}
