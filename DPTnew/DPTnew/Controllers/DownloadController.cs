using DPTnew.Helper;
using DPTnew.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DPTnew.Controllers
{
    [Authorize]
    public class DownloadController : Controller
    {
        //
        // GET: /Download/

        public ActionResult Index()
        {
            ViewBag.Message = new LocalizationHelper().SetLocalization();

            return View();
        }

    }
}
