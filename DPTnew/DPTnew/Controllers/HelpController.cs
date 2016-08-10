using DPTnew.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPTnew.Controllers
{
    public class HelpController : Controller
    {
        public ActionResult Index()
        {
            LocalizationHelper.SetLocalization(Session["CurrentCulture"]);
            return View();
        }

    }
}
