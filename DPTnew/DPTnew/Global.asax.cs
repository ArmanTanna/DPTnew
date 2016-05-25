using DPTnew.Models;
using SafenetIntegration.Safenet;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebMatrix.WebData;

namespace DPTnew
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        //To prevent view page on back browser
        protected void Application_BeginRequest()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
        }


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            if (!WebSecurity.Initialized)
            WebSecurity.InitializeDatabaseConnection("DefaultConnection", "DPT_People", "UserId", "Email", autoCreateTables: true);
            var utilitySingleton = SafenetUtilities.Instance;
            
        }

        void Application_BeginRequest(object sender, EventArgs args)
    {
      HttpContext.Current.Response.AddHeader("Access-Control-Allow-Credentials", "true");

      //using (StreamWriter sw = new StreamWriter("ciaoBefore.txt"))
      //{
      //  sw.WriteLine("INIZIO APPLICATION BEGIN");
      //  sw.WriteLine(HttpContext.Current.Request.HttpMethod);
      //}
      if (HttpContext.Current.Request.Headers["Origin"] != null)
      {
        var origin = new Uri(HttpContext.Current.Request.Headers["Origin"]);
        HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", origin.OriginalString);
        if (HttpContext.Current.Request.HttpMethod == "OPTIONS") // for FireFox
        {
          HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST");
          HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Token, Content-Type, SafenetUser, SafenetUri, SafenetPassword");
          HttpContext.Current.Response.End();
        }
      }


    }
  }

    }
