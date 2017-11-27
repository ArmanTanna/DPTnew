using DPTnew.Helper;
using DPTnew.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
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

        [HttpGet]
        [ActionName("downloadCounter")]
        public JsonResult downloadCounter(string prodName)
        {
            try
            {
                using (var db = new DptContext())
                {
                    db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[downloadCounter] (ProdName, DownloadedOn, DownloadedBy, Cnt) VALUES ('" + prodName + "',GETDATE(),'" + WebSecurity.CurrentUserName + "',1)");
                }
                return Json(HttpStatusCode.OK, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                LogHelper.WriteLog("DownloadController (downloadCounter): " + e.Message);
                return Json(HttpStatusCode.OK, JsonRequestBehavior.AllowGet);
            }
        }

    }
}
