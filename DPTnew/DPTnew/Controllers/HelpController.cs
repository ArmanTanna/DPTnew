using DPTnew.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPTnew.Controllers
{
    [Authorize]
    public class HelpController : BaseController
    {
        public ActionResult Index()
        {
            LocalizationHelper.SetLocalization(Session["CurrentCulture"]);
            return View();
        }

    }
}
