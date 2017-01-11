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

        private IEnumerable<CompanyView> GetVarCompanies()
        {
            var user = _db.Contacts.Single(u => u.Email == WebSecurity.CurrentUserName);
            var salesRep = _db.SalesR.Where(x => x.AccountNumber.Trim() == user.AccountNumber).ToList();

            IEnumerable<CompanyView> filteredcompanies = new List<CompanyView>();
            List<CompanyView> cmp = new List<CompanyView>();
            if (salesRep[0].SalesRep.Trim() == "t3kk")
            {
                var salesReps = (_db.SalesR.Where(x => x.SalesRegion == "japan")).Select(x => x.SalesRep).ToList();

                filteredcompanies = _db.Companies.Where(x => salesReps.Contains(x.SalesRep.Trim()));
            }
            else
            {
                if (salesRep[0].SalesRep.Trim() == "t3korea")
                {
                    var salesReps = (_db.SalesR.Where(x => x.SalesProvince == "southkorea")).Select(x => x.SalesRep).ToList();

                    filteredcompanies = _db.Companies.Where(x => salesReps.Contains(x.SalesRep.Trim()));
                }
                else
                {
                    foreach (var sr in salesRep)
                        cmp.AddRange(_db.Companies.Where(x => x.SalesRep.Trim() == sr.SalesRep.Trim()));
                    filteredcompanies = cmp;
                }
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
            return GetCompanies().Select(x => x.AccountNumber).ToList();
        }

        protected IEnumerable<CompanyView> GetCompanies()
        {
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal"))
                return _db.Companies.ToList();
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Var") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarMed")
                || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp"))
                return GetVarCompanies();

            var user = _db.Contacts.Single(u => u.Email == WebSecurity.CurrentUserName);
            return _db.Companies.Where(x => x.AccountNumber == user.AccountNumber).ToList();
        }

        protected IEnumerable<Order> GetOrders()
        {
            var user = WebSecurity.CurrentUserName;
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal"))
                return _db.Orders.ToList();

            //if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Var"))
            //{
            //    var user = WebSecurity.CurrentUserName;
            //    var contact = _db.Contacts.Where(u => u.Email == user).ToList().FirstOrDefault();
            //    var company = _db.Companies.Where(u => u.AccountNumber == contact.AccountNumber).ToList().FirstOrDefault();
            //    var companies = _db.Companies.Where(x => x.SalesRep.Contains(company.SalesRep)).Select(u => u.AccountNumber).ToList();
            //    return _db.Orders.Where(c => companies.Contains(c.AccountNumber)).ToList();
            //}
            //return null;

            var contact = _db.Contacts.Where(u => u.Email == user).ToList().FirstOrDefault();
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarMed") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp"))
            {
                var company = _db.Companies.Where(u => u.AccountNumber == contact.AccountNumber).ToList().FirstOrDefault();
                if (company.SalesRep == "t3kk" && (!company.AccountName.Contains("T3 JAPAN KK")))
                    return _db.Orders.Where(c => c.AccountNumber == contact.AccountNumber).ToList();
                
                var salesRep = new List<String>();
                if (company.SalesRep.Trim() == "t3korea")
                    salesRep = (_db.SalesR.Where(x => x.SalesProvince == "southkorea")).Select(x => x.SalesRep).ToList();
                else
                    salesRep = _db.SalesR.Where(u => u.Invoicer == company.AccountName).Select(u => u.SalesRep).ToList();

                if (salesRep.Count == 0)
                {
                    var sR = _db.SalesR.Where(u => u.AccountNumber == company.AccountNumber).Select(u => u.SalesRep).FirstOrDefault();
                    var companies = _db.Companies.Where(x => x.SalesRep == sR).Select(u => u.AccountNumber).ToList();
                    companies.Add(company.AccountNumber);
                    return _db.Orders.Where(c => companies.Contains(c.AccountNumber)).ToList();
                }
                else
                {
                    var res = new List<Order>();
                    foreach (var sr in salesRep)
                    {
                        var companies = _db.Companies.Where(x => x.SalesRep == sr).Select(u => u.AccountNumber).ToList();
                        res.AddRange(_db.Orders.Where(c => companies.Contains(c.AccountNumber)).ToList());
                    }
                    return res;
                }
            }
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
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Var") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarMed")
                || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp"))
            {
                var company = _db.Companies.Where(u => u.AccountNumber == contact.AccountNumber).ToList().FirstOrDefault();
                if (company.SalesRep == "t3kk" && (!company.AccountName.Contains("T3 JAPAN KK")))
                    return _db.Cases.Where(c => c.AccountNumber == contact.AccountNumber).ToList();

                var salesRep = _db.SalesR.Where(u => u.Invoicer == company.AccountName).Select(u => u.SalesRep).ToList();
                if (salesRep.Count == 0)
                {
                    var sR = _db.SalesR.Where(u => u.AccountNumber == company.AccountNumber).Select(u => u.SalesRep).FirstOrDefault();
                    var companies = _db.Companies.Where(x => x.SalesRep == sR).Select(u => u.AccountNumber).ToList();
                    companies.Add(company.AccountNumber);
                    return _db.Cases.Where(c => companies.Contains(c.AccountNumber)).ToList();
                }
                else
                {
                    var res = new List<DptCases>();
                    foreach (var sr in salesRep)
                    {
                        var companies = _db.Companies.Where(x => x.SalesRep == sr).Select(u => u.AccountNumber).ToList();
                        res.AddRange(_db.Cases.Where(c => companies.Contains(c.AccountNumber)).ToList());
                    }
                    return res;
                }
            }
            return _db.Cases.Where(c => c.AccountNumber == contact.AccountNumber).ToList();
        }

        protected IEnumerable<DptCaseHistory> GetHistoryCases()
        {
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal")
                || Roles.IsUserInRole(WebSecurity.CurrentUserName, "VarExp"))
                return _db.CaseHistories.ToList();
            return null;
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
