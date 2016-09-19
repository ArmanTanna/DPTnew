using DPTnew.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPTnew.Controllers
{
    [Authorize(Roles = "VarExp,Var,Admin,Internal")]
    public class FaqController : BaseController
    {
        //
        // GET: /Faq/

        public ActionResult Index()
        {
            if (LocalizationHelper.SetLocalization(Session["CurrentCulture"]) == 2)
                return RedirectToAction("Index_jp");
            return View();
        }

        public ActionResult Index_jp()
        {
            if (LocalizationHelper.SetLocalization(Session["CurrentCulture"]) == 2)
                return View();
            return RedirectToAction("Index");
        }

        //
        // GET: /Faq/

        public ActionResult Index2()
        {
            if (LocalizationHelper.SetLocalization(Session["CurrentCulture"]) == 2)
                return RedirectToAction("Index2_jp");
            return View();
        }

        public ActionResult Index2_jp()
        {
            if (LocalizationHelper.SetLocalization(Session["CurrentCulture"]) == 2)
                return View();
            return RedirectToAction("Index2");
        }
    }
}
