using DPTnew.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace DPTnew.Helper
{
    public class LocalizationHelper
    {
        public static string GetSalesProv()
        {
            using (var db = new DptContext())
            {
                var user = Membership.GetUser().UserName;
                var contact = db.Contacts.Where(u => u.Email == user).ToList().First();
                var company = db.Companies.Where(u => u.AccountNumber == contact.AccountNumber).ToList().First();

                var salesProv = db.SalesR.Where(x => x.SalesRegion.Trim().ToLower() == company.SalesRegion.Trim().ToLower()).Select(x => x.SalesProvince).FirstOrDefault();
                //var salesProv = db.SalesR.Where(x => x.SalesRep.Trim().ToLower() == company.SalesRep.Trim().ToLower()).Select(x => x.SalesProvince).FirstOrDefault();

                //if (salesProv.Contains("italy"))
                //{
                //    Localization.Resource.Culture = new CultureInfo("it-IT");
                //    CultureHelper.CurrentCulture = 1;
                //}
                //else if (salesProv.Contains("japan"))
                //{
                //    Localization.Resource.Culture = new CultureInfo("ja-JP");
                //    CultureHelper.CurrentCulture = 2;
                //}
                //else if (salesProv.Contains("japan"))
                //{
                //    Localization.Resource.Culture = new CultureInfo("ko-KR");
                //    CultureHelper.CurrentCulture = 3;
                //}
                //else
                //{
                //    Localization.Resource.Culture = new CultureInfo("en-US");
                //    CultureHelper.CurrentCulture = 0;
                //}

                return company.SalesRegion.Trim().ToLower();
            }
        }

        public static void SetLocalization(object session)
        {

            if (session != null && ((int)session) == 1)
            {
                Localization.Resource.Culture = new CultureInfo("it-IT");
                CultureHelper.CurrentCulture = 1;
            }
            else if (session != null && ((int)session) == 2)
            {
                Localization.Resource.Culture = new CultureInfo("ja-JP");
                CultureHelper.CurrentCulture = 2;
            }
            else if (session != null && ((int)session) == 3)
            {
                Localization.Resource.Culture = new CultureInfo("ko-KR");
                CultureHelper.CurrentCulture = 3;
            }
            else
            {
                Localization.Resource.Culture = new CultureInfo("en-US");
                CultureHelper.CurrentCulture = 0;
            }

            return;
        }

    }
}