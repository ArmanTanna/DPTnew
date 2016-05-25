using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPTnew.Controllers
{
    [Authorize(Roles = "Var, Admin, SuperUser")]
    public class FaqController : Controller
    {
        //
        // GET: /Faq/

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Faq/

        public ActionResult Index2()
        {
            return View();
        }


    }
}
