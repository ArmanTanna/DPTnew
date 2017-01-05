using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPTnew.Models;
using System.Web.Security;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using WebMatrix.WebData;
using Newtonsoft.Json;
using System.ServiceModel.Description;
using SafenetIntegration;
using DPTnew.Helper;
using System.Net.Mail;

namespace DPTnew.Controllers
{
    [Authorize(Roles = "Admin,Internal,VarExp,VarMed")]
    public class OrderController : BaseController
    {
        public ActionResult Index(int pageSize = 10)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(_db.Orders.Select(x => x.SalesRep).Distinct().ToList()).ToString(Formatting.None));
            ViewBag.SalesReps = System.Convert.ToBase64String(plainTextBytes);
            ViewBag.UserRole = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin");
            return View();
        }

        [HttpPost]
        public JsonResult Search()
        {
            var sps = Request.GetSearchParams();
            var items = GetOrders();
            foreach (var sp in sps)
            {
                items = _db.Search<Order>(sp, items);
            }
            return Json(_db.ConvertToSearchResult<Order>(sps.FirstOrDefault(), items), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult NewOrderRow(Order orderRow)
        {
            using (var db = new DptContext())
            {
                var userName = Membership.GetUser().UserName;
                var user = db.Contacts.Where(u => u.Email == userName).ToList().FirstOrDefault();
                var company = db.Companies.Where(u => u.AccountNumber == user.AccountNumber).ToList().FirstOrDefault();
                var salesRep = db.SalesR.Where(u => u.Invoicer == company.AccountName).Select(u => u.SalesRep).ToList();
                List<String> companyList = new List<string>();
                if (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal"))
                    companyList = db.Companies.Select(u => u.AccountName + " \"" + u.AccountNumber + "\"").ToList();
                else
                {
                    if (salesRep.Count == 0)
                    {
                        var sR = db.SalesR.Where(u => u.AccountNumber == company.AccountNumber).Select(u => u.SalesRep).FirstOrDefault();
                        companyList.AddRange(db.Companies.Where(x => x.SalesRep.Contains(sR)).OrderBy(k => k.AccountName).Select(u => u.AccountName + " \"" + u.AccountNumber + "\"").ToList());
                        companyList.Add(company.AccountName + " \"" + company.AccountNumber + "\"");
                    }
                    else
                        companyList.AddRange(db.Companies.Where(x => salesRep.Contains(x.SalesRep)).OrderBy(k => k.AccountName).Select(u => u.AccountName + " \"" + u.AccountNumber + "\"").ToList());
                }
                companyList.Sort();

                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JArray.FromObject(companyList).ToString(Formatting.None));
                ViewBag.Companies = System.Convert.ToBase64String(plainTextBytes);

                if (!string.IsNullOrEmpty(orderRow.OrderNumber))
                {
                    var ord = db.Orders.Where(x => x.idxx == orderRow.idxx).FirstOrDefault();
                    orderRow.OrderDate = ord.OrderDate;
                    orderRow.InvoiceDate = ord.InvoiceDate;
                    orderRow.StartDate = ord.StartDate;
                    orderRow.EndDate = ord.EndDate;
                    orderRow.AccountName = orderRow.AccountName + " \"" + orderRow.AccountNumber + "\"";
                }
                else
                {
                    orderRow.OrderDate = DateTime.Now;
                    orderRow.InvoiceDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                    orderRow.StartDate = DateTime.Now;
                    orderRow.EndDate = DateTime.Now;
                }
            }
            List<Order> rows = new List<Order>();
            rows.Add(orderRow);
            return View(rows);
        }

        [HttpPost]
        public ActionResult Insert(Order orderRow)
        {
            if (string.IsNullOrEmpty(orderRow.AccountName) || string.IsNullOrEmpty(orderRow.ProductName) || string.IsNullOrEmpty(orderRow.LicenseID))
                return Json("Missing Info", JsonRequestBehavior.AllowGet);

            using (var db = new DptContext())
            {
                if (orderRow.idxx < 1)
                {
                    var company = db.Companies.Where(c => c.AccountName == orderRow.AccountName).FirstOrDefault();
                    orderRow.AccountNumber = company.AccountNumber;
                    var sr = db.SalesR.Where(u => u.SalesRep == company.SalesRep).FirstOrDefault();
                    orderRow.Invoicer = sr.Invoicer;
                    orderRow.InvoicedNumber = sr.AccountNumber;
                    orderRow.InvoicedName = sr.AccountName;
                    orderRow.SalesRep = company.SalesRep;
                    orderRow.Invoiced = orderRow.Ordered;
                    if (orderRow.LineType == "activation")
                    {
                        orderRow.InvoiceNumber = "ACT";
                        var act = db.Activations.Where(x => x.AccountNumber == orderRow.InvoicedNumber).FirstOrDefault();
                        if (act != null)
                        {
                            orderRow.OrderNumber = act.OrderNumber;
                            orderRow.OrderDate = act.OrderDate;
                            orderRow.PO_Number = act.PO_Number;
                        }
                    }
                    else
                    {
                        if (orderRow.Invoicer.ToLower().Trim() == "dpt srl")
                            orderRow.InvoiceNumber = "I16-XXX";
                        else
                        {
                            if (orderRow.Invoicer.ToLower().Trim() == "dpt sarl")
                                orderRow.InvoiceNumber = "F16-XXX";
                            else
                                orderRow.InvoiceNumber = "JJ";
                        }
                    }
                    orderRow.RequestDate = orderRow.OrderDate;
                    if (string.IsNullOrEmpty(orderRow.PO_Number))
                        orderRow.PO_Number = "automatic input " + DateTime.Now;
                    int res = 0;
                    orderRow.idxx = db.Database.SqlQuery<int>("SELECT MAX(idxx) FROM [dbo].[DPT_Orders]", res).First() + 1;

                    if (string.IsNullOrEmpty(orderRow.OrderNumber))
                    {
                        if (DateTime.Now.Year == 2016)
                        {
                            var maxq = db.Orders.Where(u => u.OrderNumber.StartsWith("S")).Max(x => x.OrderNumber);
                            if (maxq == null)
                                orderRow.OrderNumber = "S000001";
                            else
                                orderRow.OrderNumber = "S" + (Convert.ToInt64(maxq.Split('S')[1]) + 1).ToString("D6");
                        }
                        if (DateTime.Now.Year == 2017)
                        {
                            var maxq = db.Orders.Where(u => u.OrderNumber.StartsWith("T")).Max(x => x.OrderNumber);
                            if (maxq == null)
                                orderRow.OrderNumber = "T000001";
                            else
                                orderRow.OrderNumber = "T" + (Convert.ToInt64(maxq.Split('T')[1]) + 1).ToString("D6");
                        }
                    }
                    else
                    {
                        var nocheck = db.Activations.Select(x => x.OrderNumber).ToList();
                        if (!nocheck.Contains(orderRow.OrderNumber))
                        {
                            var old = db.Orders.Where(x => x.OrderNumber == orderRow.OrderNumber).FirstOrDefault();
                            if (old != null && old.AccountNumber != orderRow.AccountNumber)
                                return Json("rows belonging to the same order can't have different account number", JsonRequestBehavior.AllowGet);
                            if (orderRow.ArticleDetail == "plss")
                            {
                                var plss = db.Orders.Where(x => x.OrderNumber == orderRow.OrderNumber && x.LicenseID == orderRow.LicenseID && x.ArticleDetail == orderRow.ArticleDetail).FirstOrDefault();
                                if (plss != null)
                                    return Json("for a license, in an Order, cannot insert more than one plss", JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            if (orderRow.LineType != "activation")
                                return Json("the linetype of the row should be activation", JsonRequestBehavior.AllowGet);
                        }
                    }
                    db.Database.ExecuteSqlCommand("INSERT INTO [dbo].[DPT_Orders] (Invoicer, InvoicedName, InvoicedNumber, AccountName," +
                        "AccountNumber, OrderNumber, OrderDate, PO_Number, InvoiceNumber, InvoiceDate, NewOldAccount, SalesRep, Currency," +
                        " LineType, ProductName, ArticleDetail, StartDate, EndDate, RequestDate, Ordered, Invoiced, Quantity, LicenseType," +
                        " NewRenewal, EURO_PriceList, JPY_PriceList, LeasingCompany, LicenseID, idxx, Note, Probability) VALUES ('" + orderRow.Invoicer + "','" +
                        orderRow.InvoicedName + "','" + orderRow.InvoicedNumber + "','" + orderRow.AccountName + "','" + orderRow.AccountNumber
                        + "','" + orderRow.OrderNumber + "','" + orderRow.OrderDate + "','" + orderRow.PO_Number + "','" + orderRow.InvoiceNumber
                        + "','" + orderRow.InvoiceDate + "','" + orderRow.NewOldAccount + "','" + orderRow.SalesRep + "','" +
                        orderRow.Currency + "','" + orderRow.LineType + "','" + orderRow.ProductName + "','" + orderRow.ArticleDetail + "','" +
                        orderRow.StartDate + "','" + orderRow.EndDate + "','" + orderRow.RequestDate + "','" + orderRow.Ordered + "','" +
                        orderRow.Invoiced + "','" + orderRow.Quantity + "','" + orderRow.LicenseType + "','" + orderRow.NewRenewal + "','" +
                        orderRow.EURO_PriceList + "','" + orderRow.JPY_PriceList + "','" + orderRow.LeasingCompany + "','" +
                        orderRow.LicenseID + "','" + orderRow.idxx + "','" + orderRow.Note + "',25" + ");");
                }
                else
                {
                    var query = from ord in db.Orders
                                where ord.idxx == orderRow.idxx
                                select ord;
                    if (query.Count() > 0)
                    {
                        if ((DateTime.Now.Year == 2016 && !(orderRow.OrderNumber.StartsWith("S"))) ||
                            (DateTime.Now.Year == 2017 && !(orderRow.OrderNumber.StartsWith("T"))))
                            return Json("Error: wrong order number (2016 = S)", JsonRequestBehavior.AllowGet);
                        foreach (Order o in query.ToList())
                        {
                            o.OrderNumber = orderRow.OrderNumber;
                            o.OrderDate = orderRow.OrderDate;
                            o.PO_Number = orderRow.PO_Number;
                            if (o.LineType == "activation")
                            {
                                o.InvoiceNumber = "ACT";
                                var act = db.Activations.Where(x => x.AccountNumber == orderRow.InvoicedNumber).FirstOrDefault();
                                if (act != null)
                                {
                                    orderRow.OrderNumber = act.OrderNumber;
                                    orderRow.OrderDate = act.OrderDate;
                                    orderRow.PO_Number = act.PO_Number;
                                }
                            }
                            else
                            {
                                if (o.Invoicer.ToLower().Trim() == "dpt srl")
                                    o.InvoiceNumber = "I16-XXX";
                                else
                                {
                                    if (o.Invoicer.ToLower().Trim() == "dpt sarl")
                                        o.InvoiceNumber = "F16-XXX";
                                    else
                                        o.InvoiceNumber = "JJ";
                                }
                            }
                            if (string.IsNullOrEmpty(orderRow.PO_Number))
                                orderRow.PO_Number = "automatic input " + DateTime.Now;
                            o.InvoiceDate = orderRow.InvoiceDate;
                            o.Invoiced = orderRow.Ordered;
                            o.RequestDate = orderRow.OrderDate;
                            o.ProductName = orderRow.ProductName;
                            o.ArticleDetail = orderRow.ArticleDetail;
                            o.LicenseID = orderRow.LicenseID;
                            o.LineType = orderRow.LineType;
                            o.LicenseType = orderRow.LicenseType;
                            o.Quantity = orderRow.Quantity;
                            o.StartDate = orderRow.StartDate;
                            o.EndDate = orderRow.EndDate;
                            o.Ordered = orderRow.Ordered;
                            o.NewRenewal = orderRow.NewRenewal;
                            o.EURO_PriceList = orderRow.EURO_PriceList;
                            o.JPY_PriceList = orderRow.JPY_PriceList;
                            o.Note = orderRow.Note;
                            db.SaveChanges();
                        }
                    }
                }
            }
            return Json("Saved OrderNumber: " + orderRow.OrderNumber + " " + orderRow.idxx, JsonRequestBehavior.AllowGet);
        }

        public ActionResult OrderRows(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ViewBag.ok1 = "Something went wrong. Cannot open the order!";
                return View("Success");
            }
            using (var db = new DptContext())
            {
                var ord = db.Orders.Where(x => x.OrderNumber == id).OrderBy(y => y.idxx).ToList();
                //if (ord.FirstOrDefault().LineType != "activation")
                //{
                //    ViewBag.ButtonBook = ord.FirstOrDefault().Status == "Entered";
                //    ViewBag.ButtonCheck = (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal"))
                //        && ord.FirstOrDefault().Status == "Booked";
                //    ViewBag.ButtonReject = (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") && ord.FirstOrDefault().Status == "Checked")
                //        || (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal") && ord.FirstOrDefault().Status == "Booked");
                //    ViewBag.ButtonApprove = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") && ord.FirstOrDefault().Status == "Checked";
                //}
                ViewBag.ButtonBook = db.Orders.Where(x => x.OrderNumber == id && x.Status == "entered").ToList().Count > 0;
                ViewBag.ButtonCheck = (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal"))
                    && (db.Orders.Where(x => x.OrderNumber == id && x.Status == "booked").ToList().Count > 0);
                ViewBag.ButtonReject = (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") && (db.Orders.Where(x => x.OrderNumber == id && x.Status == "checked").ToList().Count > 0))
                || (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal") && (db.Orders.Where(x => x.OrderNumber == id && x.Status == "booked").ToList().Count > 0));
                ViewBag.ButtonApprove = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") && (db.Orders.Where(x => x.OrderNumber == id && x.Status == "checked").ToList().Count > 0);
                double tot = 0;
                foreach (var rw in ord)
                {
                    tot += rw.Ordered;
                }
                ViewBag.OrderNumber = id;
                ViewBag.Total = tot;
                return View(ord);
            }
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ApproveOrderRow(Order orderRow)
        {
            if (string.IsNullOrEmpty(orderRow.OrderNumber))
            {
                ViewBag.ok1 = "Something went wrong. Cannot open the order!";
                return View("Success");
            }
            using (var db = new DptContext())
            {
                var ord = db.Orders.Where(x => x.OrderNumber == orderRow.OrderNumber).OrderBy(y => y.idxx).ToList();
                return View(ord);
            }
        }

        public ActionResult Book(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                ViewBag.ok1 = "Something went wrong. Cannot save the order!";
                return View("Success");
            }
            using (var db = new DptContext())
            {
                var query = from ord in db.Orders
                            where ord.OrderNumber == orderNumber && ord.Status == "entered"
                            select ord;
                if (query.Count() > 0)
                {
                    var destmail = "";
                    MailMessage mail = null;
                    foreach (Order o in query.ToList())
                    {
                        o.Status = "booked";
                        o.Probability = 50;
                        db.SaveChanges();
                        var lic = db.Licenses.Where(l => l.LicenseID == o.LicenseID).FirstOrDefault();
                        if (string.IsNullOrEmpty(destmail))
                        {
                            if (o.Invoicer.Trim().ToLower() == "t3 japan kk")
                                destmail = db.Companies.Where(x => x.AccountName == "t3 japan kk").FirstOrDefault().Email;
                            else
                                destmail = db.Companies.Where(x => x.AccountNumber == o.InvoicedNumber).FirstOrDefault().Email;

                            mail = new MailMessage("is@dptcorporate.com", "Orders@dptcorporate.com");
                            if (o.LineType != "activation")
                                mail.CC.Add(destmail);
                            mail.Subject = "[DO NOT REPLY] Order booked for " + o.AccountName.Trim() + " (" + o.AccountNumber + ")";
                            mail.Body = "Dear Sir, <br/><br/>The Order #" + orderNumber + " has been booked.<br/><br/>" +
                                "Account Name: " + o.AccountName.Trim() + " (" + o.AccountNumber + ")" + "<br/>Order date: " + o.StrOrderDate +
                                "<br/>PO number: " + o.PO_Number + "<br/><br/>" +
                                "<table border=1><tr><td>LicenseID</td><td>MachineID</td><td>Item</td><td>LicenseType</td>" +
                                "<td>Quantity</td><td>StartDate</td><td>EndDate</td><td>Ordered</td></tr>" +
                                "<tr><td>" + o.LicenseID + "</td><td>" + lic.MachineID + "</td><td>" + o.Item + "</td><td>" +
                                o.LicenseType + "</td><td>" + o.Quantity + "</td><td>" + o.StrStartDate + "</td><td>" +
                                o.StrEndDate + "</td><td>" + o.Ordered + "</td></tr>";
                        }
                        else
                            mail.Body += "<tr><td>" + o.LicenseID + "</td><td>" + lic.MachineID + "</td><td>" + o.Item + "</td><td>" +
                                o.LicenseType + "</td><td>" + o.Quantity + "</td><td>" + o.StrStartDate + "</td><td>" +
                                o.StrEndDate + "</td><td>" + o.Ordered + "</td></tr>";
                    }
                    mail.Body += "</table><br/>" +
                        "You can browse the orders at http://dpt3.dptcorporate.com/Order" +
                        "<br/><br/>Best Regards,<br/>DPT Accounting";
                    mail.IsBodyHtml = true;
                    SendMail(mail);
                }
            }
            return Json("Booked OrderNumber: " + orderNumber, JsonRequestBehavior.AllowGet);
        }

        private void SendMail(MailMessage mail)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 25;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = true;
            client.Host = System.Configuration.ConfigurationManager.AppSettings["host"];
            client.Send(mail);
        }

        [Authorize(Roles = "Admin,Internal")]
        public ActionResult Check(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                ViewBag.ok1 = "Something went wrong. Cannot save the order!";
                return View("Success");
            }
            using (var db = new DptContext())
            {
                var query = from ord in db.Orders
                            where ord.OrderNumber == orderNumber && ord.Status == "booked"
                            select ord;
                if (query.Count() > 0)
                {
                    var destmail = "";
                    MailMessage mail = null;
                    foreach (Order o in query.ToList())
                    {
                        o.Status = "checked";
                        o.Probability = 75;
                        db.SaveChanges();
                        var lic = db.Licenses.Where(l => l.LicenseID == o.LicenseID).FirstOrDefault();
                        if (string.IsNullOrEmpty(destmail))
                        {
                            if (o.Invoicer.Trim().ToLower() == "t3 japan kk")
                                destmail = db.Companies.Where(x => x.AccountName == "t3 japan kk").FirstOrDefault().Email;
                            else
                                destmail = db.Companies.Where(x => x.AccountNumber == o.InvoicedNumber).FirstOrDefault().Email;

                            mail = new MailMessage("is@dptcorporate.com", "Orders@dptcorporate.com");
                            if (o.LineType != "activation")
                                mail.CC.Add(destmail);
                            mail.Subject = "[DO NOT REPLY] Order checked for " + o.AccountName.Trim() + " (" + o.AccountNumber + ")";
                            mail.Body = "Dear Sir, <br/><br/>The Order #" + orderNumber + " has been checked.<br/><br/>" +
                                "Account Name: " + o.AccountName.Trim() + " (" + o.AccountNumber + ")" + "<br/>Order date: " + o.StrOrderDate +
                                "<br/>PO number: " + o.PO_Number + "<br/><br/>" +
                                "<table border=1><tr><td>LicenseID</td><td>MachineID</td><td>Item</td><td>LicenseType</td>" +
                                "<td>Quantity</td><td>StartDate</td><td>EndDate</td><td>Ordered</td></tr>" +
                                "<tr><td>" + o.LicenseID + "</td><td>" + lic.MachineID + "</td><td>" + o.Item + "</td><td>" +
                                o.LicenseType + "</td><td>" + o.Quantity + "</td><td>" + o.StrStartDate + "</td><td>" +
                                o.StrEndDate + "</td><td>" + o.Ordered + "</td></tr>";
                        }
                        else
                            mail.Body += "<tr><td>" + o.LicenseID + "</td><td>" + lic.MachineID + "</td><td>" + o.Item + "</td><td>" +
                                o.LicenseType + "</td><td>" + o.Quantity + "</td><td>" + o.StrStartDate + "</td><td>" +
                                o.StrEndDate + "</td><td>" + o.Ordered + "</td></tr>";
                    }
                    mail.Body += "</table><br/>" +
                        "You can browse the orders at http://dpt3.dptcorporate.com/Order" +
                        "<br/><br/>Best Regards,<br/>DPT Accounting";
                    mail.IsBodyHtml = true;
                    SendMail(mail);
                }
            }
            return Json("Checked OrderNumber: " + orderNumber, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin,Internal")]
        public ActionResult Reject(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                ViewBag.ok1 = "Something went wrong. Cannot save the order!";
                return View("Success");
            }
            var destmail = "";
            using (var db = new DptContext())
            {
                var query = from ord in db.Orders
                            where ord.OrderNumber == orderNumber && (ord.Status == "Booked" || ord.Status == "Checked")
                            select ord;
                if (query.Count() > 0)
                {
                    foreach (Order o in query.ToList())
                    {
                        o.Status = "entered";
                        o.Probability = 0;
                        db.SaveChanges();
                        if (string.IsNullOrEmpty(destmail))
                        {
                            if (o.Invoicer.Trim().ToLower() == "t3 japan kk")
                                destmail = db.Companies.Where(x => x.AccountName == "t3 japan kk").FirstOrDefault().Email;
                            else
                                destmail = db.Companies.Where(x => x.AccountNumber == o.InvoicedNumber).FirstOrDefault().Email;
                            MailMessage mail = new MailMessage("is@dptcorporate.com", destmail);
                            mail.Subject = "[DO NOT REPLY] Order rejected for " + o.AccountName.Trim() + " (" + o.AccountNumber + ")";
                            mail.Body = "Dear Sir, \n\nThe Order #: " + orderNumber + " has been rejected.\n\n" +
                                "Account Name: " + o.AccountName.Trim() + "; PO number: " + o.PO_Number + "; Order date: " + o.OrderDate + "\n\n" +
                                "You can browse and check the orders at http://dpt3.dptcorporate.com/Order" +
                                "\n\nBest Regards,\nDPT orders";
                            SendMail(mail);
                        }
                    }
                }
            }
            return Json("Rejected OrderNumber: " + orderNumber + "\n\n an e-mail was sent to " + destmail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string orderNumber, int idxx)
        {
            if (string.IsNullOrEmpty(orderNumber) || idxx < 1)
            {
                ViewBag.ok1 = "Something went wrong. Cannot delete the order!";
                return View("Success");
            }
            using (var db = new DptContext())
            {
                db.Database.ExecuteSqlCommand("DELETE [dbo].[DPT_Orders] WHERE ordernumber='" + orderNumber + "' and idxx=" + idxx);
            }
            return Json("Deleted OrderNumber: " + orderNumber + ", idxx: " + idxx, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Approve(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                ViewBag.ok1 = "Something went wrong. Cannot save the order!";
                return View("Success");
            }
            var varmail = "";
            using (var db = new DptContext())
            {
                var query = from ord in db.Orders
                            where ord.OrderNumber == orderNumber && ord.Status == "checked"
                            select ord;
                if (query.Count() > 0)
                {
                    MailMessage mail = null;
                    var dic = new Dictionary<string, string>();
                    var inv = "";
                    var lang = "";
                    foreach (Order o in query.ToList())
                    {
                        if (o.LicenseID.StartsWith("NEW"))
                        {
                            var maxq = db.Licenses.Where(u => u.LicenseID.StartsWith("L")).Max(x => x.LicenseID);
                            var LID = "L" + (Convert.ToInt64(maxq.Split('L')[1]) + 1).ToString("D8");
                            if (!dic.ContainsKey(o.LicenseID))
                                dic.Add(o.LicenseID, LID);
                        }
                        o.Status = "approved";
                        o.Probability = 100;
                        var lic = db.Licenses.Where(l => l.LicenseID == o.LicenseID).FirstOrDefault();
                        if (lic == null)
                        {
                            var val = dic.FirstOrDefault(k => k.Key == o.LicenseID);
                            lic = db.Licenses.Where(l => l.LicenseID == val.Value).FirstOrDefault();
                        }

                        if (lic.ArticleDetail.ToLower() != "pl")
                            lic.Renew = 1;

                        if (o.ArticleDetail == "plss" || o.ArticleDetail == "cvu")
                            lic.MaintEndDate = o.EndDate;

                        if (o.ArticleDetail == "asf")
                        {
                            lic.MaintEndDate = o.EndDate;
                            lic.EndDate = o.EndDate;
                            lic.MaintStartDate = o.StartDate;
                        }

                        if (o.LicenseID.StartsWith("NEW"))
                        {
                            var val = dic.FirstOrDefault(k => k.Key == o.LicenseID);
                            o.LicenseID = val.Value;
                        }
                        db.SaveChanges();

                        if (lic.LicenseID.StartsWith("NEW"))
                        {
                            var val = dic.FirstOrDefault(k => k.Key == lic.LicenseID);
                            db.Database.ExecuteSqlCommand("UPDATE [dbo].[DPT_Licenses] Set licenseID = '" + val.Value + "', [2Bins] = 1 WHERE licenseID = '" + val.Key + "'");
                        }

                        if (string.IsNullOrEmpty(varmail))
                        {
                            var clmail = db.Companies.Where(x => x.AccountNumber == o.AccountNumber).FirstOrDefault().Email;
                            varmail = db.Companies.Where(x => x.AccountNumber == o.InvoicedNumber).FirstOrDefault().Email;

                            inv = o.Invoicer.Trim().ToLower();
                            lang = db.Companies.Where(x => x.AccountNumber == o.InvoicedNumber).FirstOrDefault().Language.ToLower();
                            mail = new MailMessage("is@dptcorporate.com", clmail);
                            mail.CC.Add(varmail);
                            if (inv == "t3 japan kk")
                                mail.CC.Add(db.Companies.Where(x => x.AccountName == "t3 japan kk").FirstOrDefault().Email);

                            mail.Bcc.Add("Orders@dptcorporate.com");

                            MailHeader(orderNumber, mail, lang, o, lic);
                        }
                        else
                            mail.Body += "<tr><td>" + o.LicenseID + "</td><td>" + lic.MachineID + "</td><td>" + o.Item + "</td><td>" +
                                o.LicenseType + "</td><td>" + o.Quantity + "</td><td>" + o.StrStartDate + "</td><td>" + o.StrEndDate + "</td></tr>";
                    }
                    MailFooter(mail, lang);
                    mail.IsBodyHtml = true;
                    SendMail(mail);
                }
                //db.Database.ExecuteSqlCommand("UPDATE [dbo].[DPT_Orders] SET [STATUS] = 'Approved' WHERE ordernumber='" + orderNumber + "'");
            }
            return Json("Approved OrderNumber: " + orderNumber + "\n\n an e-mail was sent to " + varmail, JsonRequestBehavior.AllowGet);
        }

        private static void MailFooter(MailMessage mail, string lang)
        {
            if (lang == "japanese")
            {
                mail.Body += "</table><br/><br/>ライセンス管理の詳細につきましては下記「インストールガイド」の「２．ライセンス」をご覧ください。<br/>" +
                    "(ftp://ftp.think3.jp/tdExtra/InstallGuide/InstallGuide.pdf)<br/><br/>" +
                    "think3 製品の最新のリリースは、下記カスタマーケアサイトのダウンロードエリアからダウンロードすることができます。<br/>" +
                    "(http://dpt3.dptcorporate.com/Download)<br/><br/>" +
                    "下記 URL からカスタマーケアへアクセスして、ライセンスを取得してください。<br/>" +
                    "(http://dpt3.dptcorporate.com/license)<br/><br/>" +
                    "なお、ご注文いただいたライセンスと登録されたライセンスに相違がないかどうかを必ずご確認ください。<br/>" +
                    "ライセンスの種類には、ASF（年間使用料）、PL（永久）、PLSS（永久ライセンスのメンテナンスライセンス）等の種類があります。<br/><br/>" +
                    "PLSS ライセンスはライセンス本体ではないため、カスタマーケアサイトに新しいライセンスが増えるわけではありません。<br/>" +
                    "詳しくは上記インストールガイドの「２－４．ライセンスの更新」をご確認ください。<br/><br/>" +
                    "弊社では評価用／貸出用のライセンスを納品することで、お客様からは費用を頂戴しておりません。<br/><br/>" +
                    "以上、ご不明な点はカスタマーケアサイトよりお問い合わせください。<br/>シンク・スリー カスタマーケアスタッフ";
            }
            else
            {
                if (lang == "korean")
                    mail.Body += "</table><br/><br/>라이선스 관리에 대한 자세한 내용은 아래 'think3 제품 설치 가이드'의 '2. 라이선스'를 참조하십시오.<br/>" +
                    "(ftp://ftp.t3-japan.co.jp/tdExtra/InstallGuide/kr/InstallGuide_Kr.pdf)<br/><br/>" +
                    " think3 제품의 최신 릴리스는 고객 지원 사이트의 다운로드 영역에서 다운로드 할 수 있습니다.<br/>" +
                    "(http://dpt3.dptcorporate.com/Download)<br/><br/>" +
                    "아래 URL에서 고객 센터에 접속하여 라이선스를 취득해야합니다.<br/>" +
                    "(http://dpt3.dptcorporate.com/license)<br/><br/>" +
                    "또한, 주문하신 라이선스와  등록된 라이선스가 차이가 있는지를반드시 확인하시기 바랍니다. <br/>" +
                    "라이선스 유형은 ASF (연간 라이선스), PL (영구라이선스), PLSS (영구 라이선스의 유지 보수 라이선스) 등의 종류가 있습니다.<br/><br/>" +
                    "PLSS 라이선스는 유지보수 라이선스이기 때문에, 고객 지원 사이트에서 새로운 라이선스가 늘어나는 것은 아닙니다. <br/>" +
                    "자세한 내용은 위 설치 안내서의 '2-4. 라이선스 갱신'을 확인하시기 바랍니다.<br/><br/>" +
                    " 당사는 평가 / 대여용 라이선스에 대해 비용을 부과하지 않습니다.<br/><br/>" +
                    "기타 문의 사항은 고객 지원 사이트에서 문의해 주세요.<br/>think3 고객 센터 직원";
                else
                    mail.Body += "</table><br/>(*) ASF or PL items are ready for self-installation<br/><br/>" +
                        "You can browse the orders at http://dpt3.dptcorporate.com/Order" +
                        "<br/><br/>Best Regards,<br/><br/>DPT Accounting";
            }
        }

        private static void MailHeader(string orderNumber, MailMessage mail, string lang, Order o, LicenseView lic)
        {
            if (lang == "japanese")
            {
                mail.Subject = o.AccountName.Trim() + " (" + o.AccountNumber + ") [このメールには返信しないでください] 様、think3製品が登録されました　(#S11209)";
                mail.Body = o.AccountName.Trim() + " (" + o.AccountNumber + ") 様<br/><br/>" +
                    "下記の通りライセンスが登録されておりますのでお知らせいたします。<br/><br/>" +
                    "アカウント名: " + o.AccountName.Trim() + " (" + o.AccountNumber + ")" +
                    "<br/>注文日: " + o.StrOrderDate + "<br/><br/>" +
                    "<table border=1><tr>" + "<td>ライセンスID</td>" + "<td>マシンID</td>" + "<td>製品</td>" +
                    "<td>タイプ</td>" + "<td>数</td>" + "<td>開始日</td>" + "<td>終了日</td></tr>" +
                    "<tr><td>" + o.LicenseID + "</td><td>" + lic.MachineID + "</td><td>" + o.Item + "</td><td>" +
                    o.LicenseType + "</td><td>" + o.Quantity + "</td><td>" + o.StrStartDate + "</td><td>" + o.StrEndDate + "</td></tr>";
            }
            else
            {
                if (lang == "korean")
                {
                    mail.Subject = o.AccountName.Trim() + " (" + o.AccountNumber + ") [이 이메일에 회신하지 마세요] 님, think3 제품이 등록되었습니다 (#" + orderNumber + ")";
                    mail.Body = o.AccountName.Trim() + " (" + o.AccountNumber + ") 님<br/><br/>" +
                        "아래와 같이 라이선스가 등록되었음을 알려드립니다.<br/><br/>" +
                        "계정명：" + o.AccountName.Trim() + " (" + o.AccountNumber + ")" + "<br/>주문일: " + o.StrOrderDate +
                        "<br/><br/>" +
                        "<table border=1><tr>" + "<td>라이선스ID</td>" + "<td>머신ID</td>" + "<td>제품 </td>" +
                        "<td>유형</td>" + "<td>수량</td>" + "<td>시작일</td>" + "<td>종료일</td></tr>" +
                        "<tr><td>" + o.LicenseID + "</td><td>" + lic.MachineID + "</td><td>" + o.Item + "</td><td>" +
                        o.LicenseType + "</td><td>" + o.Quantity + "</td><td>" + o.StrStartDate + "</td><td>" + o.StrEndDate + "</td></tr>";
                }
                else
                {
                    mail.Subject = "[DO NOT REPLY] Order approved for " + o.AccountName.Trim() + " (" + o.AccountNumber + ")";
                    mail.Body = "Dear Sir, <br/><br/>The Order #" + orderNumber + " has been approved.<br/><br/>" +
                        "Account Name: " + o.AccountName.Trim() + " (" + o.AccountNumber + ")" + "<br/>Order date: " + o.StrOrderDate +
                        "<br/>PO number: " + o.PO_Number + "<br/><br/>" +
                        "<table border=1><tr>" + "<td>LicenseID</td>" + "<td>MachineID</td>" + "<td>Item</td>" +
                        "<td>LicenseType</td>" + "<td>Quantity</td>" + "<td>StartDate</td>" + "<td>EndDate</td></tr>" +
                        "<tr><td>" + o.LicenseID + "</td><td>" + lic.MachineID + "</td><td>" + o.Item + "</td><td>" +
                        o.LicenseType + "</td><td>" + o.Quantity + "</td><td>" + o.StrStartDate + "</td><td>" + o.StrEndDate + "</td></tr>";
                }
            }
        }

        [HttpPost]
        public ActionResult GetInvoiceInfo(string companyName)
        {
            if (string.IsNullOrEmpty(companyName))
                return Json("Parameter Missing!", JsonRequestBehavior.AllowGet);
            using (var db = new DptContext())
            {
                var orderRow = new Order();
                var company = db.Companies.Where(c => c.AccountName == companyName).FirstOrDefault();
                orderRow.AccountNumber = company.AccountNumber;
                var sr = db.SalesR.Where(u => u.SalesRep == company.SalesRep).FirstOrDefault();
                orderRow.Invoicer = sr.Invoicer;
                orderRow.InvoicedNumber = sr.AccountNumber;
                orderRow.InvoicedName = sr.AccountName;
                orderRow.SalesRep = company.SalesRep;
                var query = "SELECT COUNT(*) as cnt FROM [dbo].[DPT_Orders] WHERE AccountNumber='" + company.AccountNumber + "'";
                var res = 0;
                var cnt = 0;
                cnt = db.Database.SqlQuery<Int32>(query, res).First();

                if (cnt == 0)
                    orderRow.NewOldAccount = "new";
                else
                    orderRow.NewOldAccount = "old";

                if (orderRow.Invoicer.ToLower().Trim() == "dpt srl" || orderRow.Invoicer.ToLower().Trim() == "dpt sarl")
                    orderRow.Currency = "eur";
                else
                    orderRow.Currency = "jpy";

                return Json(orderRow, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetProducts(string companyName)
        {
            if (string.IsNullOrEmpty(companyName))
                return Json("Parameter Missing!", JsonRequestBehavior.AllowGet);
            using (var db = new DptContext())
            {
                var company = db.Companies.Where(c => c.AccountName == companyName).FirstOrDefault();
                var products = db.Licenses.Where(u => u.AccountNumber == company.AccountNumber).Select(x => x.ProductName).Distinct().ToList();
                return Json(products, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetLicenseIds(string companyName, string productName)
        {
            if (string.IsNullOrEmpty(companyName) || string.IsNullOrEmpty(productName))
                return Json("Parameter Missing!", JsonRequestBehavior.AllowGet);
            using (var db = new DptContext())
            {
                var company = db.Companies.Where(c => c.AccountName == companyName).FirstOrDefault();
                var licIds = db.Licenses.Where(u => u.AccountNumber == company.AccountNumber && u.ProductName == productName &&
                    (u.LicenseID.StartsWith("NEW") || u.LicenseID.StartsWith("L") || u.LicenseID.StartsWith("EDU"))).ToList();
                return Json(licIds, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetLicenseInfo(string licenseId)
        {
            if (string.IsNullOrEmpty(licenseId))
                return Json("Parameter Missing!", JsonRequestBehavior.AllowGet);
            var lic = new LicenseView();
            using (var db = new DptContext())
            {
                lic.LicenseType = db.Licenses.Where(u => u.LicenseID == licenseId).Select(x => x.LicenseType).FirstOrDefault();
                lic.Quantity = db.Licenses.Where(u => u.LicenseID == licenseId).Select(x => x.Quantity).FirstOrDefault();
                var nd = db.Licenses.Where(u => u.LicenseID == licenseId).Select(x => x.MaintEndDate).FirstOrDefault();
                lic.PwdCode = ((DateTime)nd).AddDays(1).ToString("yyyy-MM-dd");
                lic.Ancestor = ((DateTime)nd).AddDays(365).ToString("yyyy-MM-dd");
                return Json(lic, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult GetPrice(string productName, string articleDetail, string licenseType, int quantity)
        {
            if (string.IsNullOrEmpty(productName) || string.IsNullOrEmpty(articleDetail))
                return Json("Parameter Missing!", JsonRequestBehavior.AllowGet);
            if (articleDetail == "upg")
                return Json("1500_200000", JsonRequestBehavior.AllowGet);
            using (var db = new DptContext())
            {
                string query = "SELECT ";
                switch (articleDetail)
                {
                    case "pl":
                    case "chw":
                        query += "PL_EURO";
                        break;
                    case "asf":
                    case "qsf":
                        query += "ASF_EURO";
                        break;
                    default:
                        query += "PLSS_EURO";
                        break;
                }
                query += " FROM [DPT].[dbo].[DPT_PriceList] WHERE ProductName = '" + productName + "' and [Status] = 'active'";
                var res = 0;
                var europrice = db.Database.SqlQuery<decimal>(query, res).First();

                query = query.Replace("PL_EURO", "PL_JPY");
                query = query.Replace("PLSS_EURO", "PLSS_JPY");
                query = query.Replace("ASF_EURO", "ASF_JPY");
                var jpyprice = db.Database.SqlQuery<decimal>(query, res).First();
                if (licenseType == "floating")
                {
                    europrice = europrice + ((europrice * 15) / 100);
                    jpyprice = jpyprice + ((jpyprice * 5) / 100);
                }
                switch (articleDetail)
                {
                    case "qsf":
                        europrice = europrice / 4;
                        jpyprice = jpyprice / 4;
                        break;
                    case "cvu":
                        europrice = europrice * 3;
                        jpyprice = jpyprice * 3;
                        break;
                    case "chw":
                        europrice = (europrice * 20) / 100;
                        jpyprice = (jpyprice * 20) / 100;
                        break;
                }
                if (quantity > 1)
                {
                    europrice = europrice * quantity;
                    jpyprice = jpyprice * quantity;
                }
                return Json(europrice + "_" + jpyprice, JsonRequestBehavior.AllowGet);
            }
        }

        [NonAction]
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Session["CurrentCulture"] = LocalizationHelper.SetLocalization(Session["CurrentCulture"]);

            base.OnActionExecuting(filterContext);
        }

    }
}