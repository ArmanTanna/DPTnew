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
            return View();
        }

        //
        // GET: /FaqUser/

        public ActionResult Index2()
        {
            return View();
        }


    }
}
