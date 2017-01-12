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
    [Authorize(Roles = "Admin,Internal")]
    public class InternosController : Controller
    {
        public ActionResult Index()
        {
            Session["CurrentCulture"] = LocalizationHelper.SetLocalization(Session["CurrentCulture"]);
            return View();
        }

    }
}
