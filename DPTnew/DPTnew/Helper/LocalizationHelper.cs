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

                return company.SalesRegion.Trim().ToLower();
            }
        }

        public static int SetLocalization(object session)
        {
            if (session == null)
            {
                using (var db = new DptContext())
                {
                    var user = Membership.GetUser().UserName;
                    var lang = db.Contacts.Where(u => u.Email == user).ToList().First();
                    if (lang.Language == "italian")
                    {
                        Localization.Resource.Culture = new CultureInfo("it-IT");
                        return CultureHelper.CurrentCulture = 1;
                    }
                    if (lang.Language == "japanese")
                    {
                        Localization.Resource.Culture = new CultureInfo("ja-JP");
                        return CultureHelper.CurrentCulture = 2;
                    }
                    if (lang.Language == "korean")
                    {
                        Localization.Resource.Culture = new CultureInfo("ko-KR");
                        return CultureHelper.CurrentCulture = 3;
                    }
                    if (lang.Language == "german")
                    {
                        Localization.Resource.Culture = new CultureInfo("de-DE");
                        return CultureHelper.CurrentCulture = 4;
                    }
                    if (lang.Language == "french")
                    {
                        Localization.Resource.Culture = new CultureInfo("fr-FR");
                        return CultureHelper.CurrentCulture = 5;
                    }
                    if (lang.Language == "chinese")
                    {
                        Localization.Resource.Culture = new CultureInfo("zh-CN");
                        return CultureHelper.CurrentCulture = 6;
                    }
                    Localization.Resource.Culture = new CultureInfo("en-US");
                    return CultureHelper.CurrentCulture = 0;
                }
            }
            else
            {
                if (((int)session) == 1)
                {
                    Localization.Resource.Culture = new CultureInfo("it-IT");
                    return CultureHelper.CurrentCulture = 1;
                }
                if (((int)session) == 2)
                {
                    Localization.Resource.Culture = new CultureInfo("ja-JP");
                    return CultureHelper.CurrentCulture = 2;
                }
                if (((int)session) == 3)
                {
                    Localization.Resource.Culture = new CultureInfo("ko-KR");
                    return CultureHelper.CurrentCulture = 3;
                }
                if (((int)session) == 4)
                {
                    Localization.Resource.Culture = new CultureInfo("de-DE");
                    return CultureHelper.CurrentCulture = 4;
                }
                if (((int)session) == 5)
                {
                    Localization.Resource.Culture = new CultureInfo("fr-FR");
                    return CultureHelper.CurrentCulture = 5;
                }
                if (((int)session) == 6)
                {
                    Localization.Resource.Culture = new CultureInfo("zh-CN");
                    return CultureHelper.CurrentCulture = 6;
                }
                Localization.Resource.Culture = new CultureInfo("en-US");
                return CultureHelper.CurrentCulture = 0;
            }
        }

    }
}