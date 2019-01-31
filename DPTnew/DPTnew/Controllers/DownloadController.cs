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
        {// description: ベータ版 -- view_download: ベータ版
            LocalizationHelper.SetLocalization(Session["CurrentCulture"]);
            ViewBag.Message = LocalizationHelper.GetSalesProv();
            ViewBag.UserNoLicCase = !(Roles.IsUserInRole(WebSecurity.CurrentUserName, "UserNoLicCase"));
            return View();
        }

        [HttpGet]
        [ActionName("downloadCounter")]
        public JsonResult downloadCounter(string prodName)
        {
            try
            {
                var pName = "";
                switch (prodName.Split('_')[1])
                {
                    case "2018":
                        if (prodName.Split('_')[0].Contains("2019"))
                            pName = "ThinkDesign 2019 64bit";
                        else
                            pName = "Utility tools 2019";
                        break;
                    default:
                        break;
                }

                using (var db = new DptContext())
                {
                    db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[downloadCounter] (ProdName, DownloadedOn, DownloadedBy, Cnt) VALUES ('" + pName + "',GETDATE(),'" + WebSecurity.CurrentUserName + "',1)");
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
