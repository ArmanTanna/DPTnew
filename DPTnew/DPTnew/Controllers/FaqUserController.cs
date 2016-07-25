using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPTnew.Controllers
{
    [Authorize]
    public class FaqUserController : Controller
    {
        //
        // GET: /FaqUser/

        public ActionResult Index()
        {
            if (Session["CurrentCulture"] != null && ((int)Session["CurrentCulture"]) == 2)
                return RedirectToAction("Index_jp");
            return View();
        }

        public ActionResult Index_jp()
        {
            if (Session["CurrentCulture"] != null && ((int)Session["CurrentCulture"]) == 2)
                return View();
            return RedirectToAction("Index");
        }

        //
        // GET: /FaqUser/

        public ActionResult Index2()
        {
            if (Session["CurrentCulture"] != null && ((int)Session["CurrentCulture"]) == 2)
                return RedirectToAction("Index2_jp");
            return View();
        }

        public ActionResult Index2_jp()
        {
            if (Session["CurrentCulture"] != null && ((int)Session["CurrentCulture"]) == 2)
                return View();
            return RedirectToAction("Index2");
        }


    }
}
