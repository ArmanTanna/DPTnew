using DPTnew.Helper;
using DPTnew.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DPTnew.Controllers
{
    [Authorize]
    public class eLearningController : Controller
    {
        //
        // GET: /eLearning/

        public ActionResult Index()
        {
            using (var db = new DptContext())
            {
                var user = Membership.GetUser().UserName;
                //var lang = db.Contacts.Where(u => u.Email == user).ToList().First();
                //if (lang.Language == "italian")
                //    return View("Index_it");
                //if (lang.Language == "japanese")
                //    return View("Index_jp");
                if (Localization.Resource.Culture != null && Localization.Resource.Culture.Name == "it-IT")
                    return View("Index_it");
                if (Localization.Resource.Culture != null && Localization.Resource.Culture.Name == "ja-JP")
                    return View("Index_jp");
                return View();
            }
        }

    }
}
