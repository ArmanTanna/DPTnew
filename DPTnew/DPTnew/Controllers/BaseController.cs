using DPTnew.Helper;
using DPTnew.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace DPTnew.Controllers
{
    public class BaseController : Controller
    {
        protected DptContext _db = new DptContext();

        protected override void ExecuteCore()
        {
            int culture = 0;
            if (this.Session == null || this.Session["CurrentCulture"] == null)
            {

                int.TryParse(System.Configuration.ConfigurationManager.AppSettings["Culture"], out culture);
                this.Session["CurrentCulture"] = culture;
            }
            else
            {
                culture = (int)this.Session["CurrentCulture"];
            }
            // calling CultureHelper class properties for setting  
            CultureHelper.CurrentCulture = culture;

            base.ExecuteCore();
        }

        private IEnumerable<CompanyView> GetVarCompanies()
        {
            var user = _db.Contacts.Single(u => u.Email == WebSecurity.CurrentUserName);

            var salesRep = _db.SalesR.Single(x => x.AccountNumber.Trim() == user.AccountNumber);
            IEnumerable<CompanyView> filteredcompanies = new List<CompanyView>();
            if (salesRep.SalesRep.Trim() == "t3kk")
            {
                var salesReps = (_db.SalesR.Where(x => x.SalesRegion == "japan")).Select(x => x.SalesRep).ToList();

                filteredcompanies = _db.Companies.Where(x => salesReps.Contains(x.SalesRep.Trim()));
            }
            else
            {
                if (salesRep.SalesRep.Trim() == "t3korea")
                {
                    var salesReps = (_db.SalesR.Where(x => x.SalesProvince == "southkorea")).Select(x => x.SalesRep).ToList();

                    filteredcompanies = _db.Companies.Where(x => salesReps.Contains(x.SalesRep.Trim()));
                }
                else
                    filteredcompanies = _db.Companies.Where(x => x.SalesRep.Trim() == salesRep.SalesRep.Trim());
            }
            return filteredcompanies.ToList();
        }

        private IEnumerable<People> GetVarPeoples()
        {
            IEnumerable<CompanyView> filteredcompanies = GetVarCompanies();

            var accountIds = filteredcompanies.Select(p => p.AccountNumber).ToList();

            IEnumerable<People> filteredcontact = _db.Peoples.Where(item => accountIds.Contains(item.AccountNumber));

            //IEnumerable<Contact> filteredcontact = _db.Contacts.Where(p => p.AccountNumber.Any(x => accountIds.Contains(x.ToString())));

            return filteredcontact.ToList();
        }

        protected IEnumerable<string> GetCompanyAccountNumbers()
        {
            return GetCompanies().Select(x => x.AccountNumber);
        }

        protected IEnumerable<CompanyView> GetCompanies()
        {
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal"))
                return _db.Companies.ToList();
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Var") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp"))
                return GetVarCompanies();

            var user = _db.Contacts.Single(u => u.Email == WebSecurity.CurrentUserName);
            return _db.Companies.Where(x => x.AccountNumber == user.AccountNumber).ToList();
        }

        protected IEnumerable<Order> GetOrders()
        {
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal"))
                return _db.Orders.ToList();

            return null;
        }

        protected IEnumerable<People> GetPeoples()
        {
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal"))
                return _db.Peoples.ToList();
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp"))
                return GetVarPeoples();

            return null;
        }

        protected IEnumerable<DptErp> GetErpRows()
        {
            var user = WebSecurity.CurrentUserName;
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal"))
                return _db.ErpRows.Where(u => u.Email == user).ToList();
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin"))
                return _db.ErpRows.ToList();

            return null;
        }

        protected IEnumerable<DptCases> GetCases()
        {
            var user = WebSecurity.CurrentUserName;
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal"))
                return _db.Cases.ToList();

            var contact = _db.Contacts.Where(u => u.Email == user).ToList().FirstOrDefault();
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Var") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp"))
            {
                var company = _db.Companies.Where(u => u.AccountNumber == contact.AccountNumber).ToList().FirstOrDefault();
                var companies = _db.Companies.Where(x => x.SalesRep.Contains(company.SalesRep)).Select(u => u.AccountNumber).ToList();
                return _db.Cases.Where(c => companies.Contains(c.AccountNumber));
            }
            return _db.Cases.Where(c => c.AccountNumber == contact.AccountNumber);
        }

        protected IEnumerable<ActivityTitles> GetActivityTitles()
        {
            var user = WebSecurity.CurrentUserName;
            return _db.ActivityTitles.Where(u => u.Email == user).ToList();
        }

        protected IEnumerable<MainWebRoles> GetWebMainRoles()
        {
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin"))
                return _db.MainWebRoles.ToList();

            return _db.MainWebRoles.Where(u => u.RoleName.Contains("User")).ToList();
        }

        protected IEnumerable<PeopleRoles> GetPeopleRoles()
        {
            return _db.PeopleRoles.ToList();
        }

        protected IEnumerable<LicenseView> GetLicenses()
        {
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal"))
                return _db.Licenses.ToList();

            var companies = GetCompanyAccountNumbers();
            return _db.Licenses.Where(x => companies.Contains(x.AccountNumber)).ToList();
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
