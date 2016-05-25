using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPTnew.Controllers
{
    [Authorize]
    public class eLearningController : Controller
    {
        //
        // GET: /eLearning/

        public ActionResult Index()
        {
            return View();
        }

    }
}
