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
    public class DownloadController : BaseController
    {
        //
        // GET: /Download/

        public ActionResult Index()
        {
            LocalizationHelper.SetLocalization(Session["CurrentCulture"]);
            ViewBag.Message = LocalizationHelper.GetSalesProv();
            ViewBag.UserNoCase = !(Roles.IsUserInRole(WebSecurity.CurrentUserName, "UserNoCase"));
            return View();
        }

    }
}
