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
    public class eLearningController : BaseController
    {
        //
        // GET: /eLearning/

        public ActionResult Index()
        {
            using (var db = new DptContext())
            {
                if (LocalizationHelper.SetLocalization(Session["CurrentCulture"]) == 1)
                    return View("Index_it");
                if (LocalizationHelper.SetLocalization(Session["CurrentCulture"]) == 2)
                    return View("Index_jp");
                if (LocalizationHelper.SetLocalization(Session["CurrentCulture"]) == 3)
                    return View("Index_kr");
                return View();
            }
        }

    }
}
