using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPTnew.Models;
using System.Web.Security;

namespace DPTnew.Controllers
{

    [Authorize(Roles = "Var")]
    public class VarController : Controller
    {

        private DptContext db = new DptContext();
        
        //
        // GET: /Va/rCompanies/

        public ActionResult Companies()
        {

            var useronline = Membership.GetUser();
            var user = db.Contacts.Single(u => u.Email == useronline.UserName);
            var dpt_Company = user.Company;

            var salesRep = db.SalesR.Single(x => x.AccountNumber.Trim() == dpt_Company.AccountNumber);
            IEnumerable<Company> filteredcompanies = new List<Company>();
            if (salesRep.SalesRep.Trim() == "t3kk")
            {
                var salesReps = (db.SalesR.Where(x => x.SalesRegion == "japan")).ToList();
                foreach (SalesR s in salesReps)
                {
                    IEnumerable<Company> tempcomp = db.Companies.Where(x => x.SalesRep.Trim() == s.SalesRep.Trim());
                    if (tempcomp.Count() != 0)
                    {
                        filteredcompanies = filteredcompanies.Union(tempcomp);
                    }

                }
            }
            else
            {
                filteredcompanies = db.Companies.Where(x => x.SalesRep.Trim() == salesRep.SalesRep.Trim());
            }
            return View(filteredcompanies);


        }

        
        // POST: /Licenses/

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Licenses(Company model)
        {
            var company = db.Companies.Single(u => u.AccountNumber == model.AccountNumber);
            return View(company.Licenses);
        }

       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        
    }

}