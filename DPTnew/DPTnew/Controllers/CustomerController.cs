using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPTnew.Models;
using DPTnew.ViewModels;
using Newtonsoft.Json.Linq;

using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace DPTnew.Controllers
{
    public class CustomerController : Controller
    {
        private DptContext db = new DptContext();


        //
        // GET: /Customer/

        public ActionResult Index()
        {
            return View(db.Companies.ToList());
        }

        //
        // GET: /Customer/Details/5

        public ActionResult Details(string id)
        {
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            Contact contact = company.Contacts.Single(u => u.PrimaryContact == "yes");
            Customer customer = new Customer();
            customer.Company = company;
            customer.Contact = contact;
            return View(customer);
        }

        //
        // GET: /Customer/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Customer/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer model) {

            if (ModelState.IsValid)
            {
                
                db.Companies.Add(model.Company);
                db.SaveChanges();

                model.Contact.AccountNumber = model.Company.AccountNumber;
                model.Contact.PrimaryContact="yes";
                db.Contacts.Add(model.Contact);
                db.SaveChanges();

                using (var client = new HttpClient())
                {
                    StringContent stringContent = new StringContent(
                      "{ Email: \"" + model.Contact.Email + "\"," +
                      "FirstName: \"" + model.Contact.FirstName + "\"," +
                      "LastName: \"" + model.Contact.LastName + "\"," +
                      "CompanyName: \"" + model.Company.AccountName + "\"," +
                      "CrmId: \"" + model.Company.AccountNumber + "\"," +
                      "Locale: \" Italiano \"" +
                      "}",
                         UnicodeEncoding.UTF8,
                         "application/json");

                    var response = client.PostAsync("http://localhost:27107/api/Safenet/CreateCustomer",
                        stringContent)
                            .Result;

                    if (response.IsSuccessStatusCode)
                    {
                        dynamic content = JsonConvert.DeserializeObject(
                            response.Content.ReadAsStringAsync()
                            .Result);
                    }
                }



                return RedirectToAction("Index");
            }
            
            return View(model);
        }

        //
        // GET: /Customer/Edit/5

        public ActionResult Edit(string id)
        {
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            Contact contact = company.Contacts.Single(u => u.PrimaryContact == "yes");
            Customer customer = new Customer();
            customer.Company = company;
            customer.Contact = contact;
            return View(customer);
        }

        //
        // POST: /Customer/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.Contact.AccountNumber = customer.Company.AccountNumber;
                customer.Contact.PrimaryContact="yes";

                db.Entry(customer.Contact).State = EntityState.Modified; ;
                db.Entry(customer.Company).State = EntityState.Modified; ;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
           
            return View(customer);
        }

        //
        // GET: /Customer/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Contact contact = db.Contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }
            return View(contact);
        }

        //
        // POST: /Customer/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Contact contact = db.Contacts.Find(id);
            db.Contacts.Remove(contact);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    
}
}