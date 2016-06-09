using DPTnew.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace DPTnew.Controllers
{
    public class BaseController : Controller
    {
        protected DptContext _db = new DptContext();

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
                filteredcompanies = _db.Companies.Where(x => x.SalesRep.Trim() == salesRep.SalesRep.Trim());

            return filteredcompanies.ToList();
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

        protected IEnumerable<DptErp> GetErpRows()
        {
            var user = WebSecurity.CurrentUserName;
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal"))
                return _db.ErpRows.Where(u => u.Email == user).ToList();
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin"))
                return _db.ErpRows.ToList();

            return null;
        }

        protected IEnumerable<ActivityTitles> GetActivityTitles()
        {
            var user = WebSecurity.CurrentUserName;
            return _db.ActivityTitles.Where(u => u.Email == user).ToList();
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
