﻿using System;
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
using System.Net;

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
            ViewBag.UserIntRole = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal")
                || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin");
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
                        //companyList.Add(company.AccountName + " \"" + company.AccountNumber + "\"");
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
                if (string.IsNullOrEmpty(orderRow.idxx))
                {
                    var company = db.Companies.Where(c => c.AccountName == orderRow.AccountName).FirstOrDefault();
                    orderRow.AccountNumber = company.AccountNumber;
                    if (orderRow.AccountName.Contains("'"))
                        orderRow.AccountName = orderRow.AccountName.Replace("'", "''");
                    var sr = db.SalesR.Where(u => u.SalesRep == company.SalesRep).FirstOrDefault();
                    orderRow.Invoicer = sr.Invoicer.Trim();
                    orderRow.InvoicedNumber = sr.AccountNumber.Trim();
                    orderRow.InvoicedName = sr.AccountName.Trim();
                    orderRow.SalesRep = company.SalesRep.Trim();
                    if (orderRow.ProductName.ToLower() == "dongle" || orderRow.ProductName.ToLower() == "penalty"
                        || orderRow.ProductName.ToLower() == "service")
                        orderRow.LicenseID = "NA";
                    if (orderRow.ArticleDetail.ToLower() == "pl")
                        orderRow.EndDate = orderRow.StartDate;
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
                        if (orderRow.Invoicer.ToLower().Trim() == "dpt srl" || orderRow.Invoicer.ToLower().Trim() == "dpt sarl")
                        {
                            var code = from salesR in db.SalesR
                                       where salesR.SalesRep == orderRow.SalesRep
                                       select salesR;
                            orderRow.InvoiceNumber = "L" + DateTime.Now.Year.ToString().Substring(2) + "-" + code.FirstOrDefault().Code.Trim() + orderRow.InvoiceDate.ToString("MM");
                        }
                        else
                            orderRow.InvoiceNumber = "JJ";
                    }
                    if (string.IsNullOrEmpty(orderRow.PO_Number))
                        orderRow.PO_Number = "automatic input " + DateTime.Now;
                    int res = 0;
                    var idx = db.Database.SqlQuery<string>("SELECT MAX(idxx) FROM [dbo].[DPT_Orders]", res).First();
                    orderRow.idxx = "X" + (Convert.ToInt64(idx.Split('X')[1]) + 1).ToString("D6");

                    if (string.IsNullOrEmpty(orderRow.OrderNumber))
                    {
                        if (DateTime.Now.Year == 2021)
                        {
                            var maxq = db.Orders.Where(u => u.OrderNumber.StartsWith("X")).Max(x => x.OrderNumber);
                            if (maxq == null)
                                orderRow.OrderNumber = "X000001";
                            else
                                orderRow.OrderNumber = "X" + (Convert.ToInt64(maxq.Split('X')[1]) + 1).ToString("D6");
                        }
                        if (DateTime.Now.Year == 2020)
                        {
                            var maxq = db.Orders.Where(u => u.OrderNumber.StartsWith("W")).Max(x => x.OrderNumber);
                            if (maxq == null)
                                orderRow.OrderNumber = "W000001";
                            else
                                orderRow.OrderNumber = "W" + (Convert.ToInt64(maxq.Split('W')[1]) + 1).ToString("D6");
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
                        " LineType, ProductName, ArticleDetail, StartDate, EndDate, Ordered, Quantity, LicenseType," +
                        " NewRenewal, EURO_PriceList, JPY_PriceList, LicenseID, idxx, Note, [Status]) VALUES ('" + orderRow.Invoicer + "','" +
                        orderRow.InvoicedName + "','" + orderRow.InvoicedNumber + "','" + orderRow.AccountName + "','" + orderRow.AccountNumber
                        + "','" + orderRow.OrderNumber + "','" + orderRow.OrderDate + "','" + orderRow.PO_Number + "','" + orderRow.InvoiceNumber
                        + "','" + orderRow.InvoiceDate + "','" + orderRow.NewOldAccount + "','" + orderRow.SalesRep + "','" +
                        orderRow.Currency + "','" + orderRow.LineType + "','" + orderRow.ProductName + "','" + orderRow.ArticleDetail + "','" +
                        orderRow.StartDate + "','" + orderRow.EndDate + "','" + orderRow.Ordered + "','" +
                        orderRow.Quantity + "','" + orderRow.LicenseType + "','" + orderRow.NewRenewal + "','" +
                        orderRow.EURO_PriceList + "','" + orderRow.JPY_PriceList + "','" + orderRow.LicenseID + "','" + orderRow.idxx + "','"
                        + orderRow.Note + "','entered');");
                }
                else
                {
                    var query = from ord in db.Orders
                                where ord.idxx == orderRow.idxx && ord.Status.ToLower() != "lost"
                                select ord;
                    if (query.Count() > 0)
                    {
                        var nocheck = db.Activations.Select(x => x.OrderNumber).ToList();
                        if (!nocheck.Contains(orderRow.OrderNumber) && ((orderRow.OrderDate.Year == 2020 &&
                            !(orderRow.OrderNumber.StartsWith("W"))) ||
                            (orderRow.OrderDate.Year == 2019 && !(orderRow.OrderNumber.StartsWith("V")))))
                            return Json("Error: wrong order number (2019 = V)", JsonRequestBehavior.AllowGet);
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
                                {
                                    var code = from salesR in db.SalesR
                                               where salesR.SalesRep == orderRow.SalesRep
                                               select salesR;
                                    orderRow.InvoiceNumber = "I" + DateTime.Now.Year.ToString().Substring(2) + "-" + code.FirstOrDefault().Code.Trim() + orderRow.InvoiceDate.ToString("MM");
                                }
                                else
                                {
                                    if (o.Invoicer.ToLower().Trim() == "dpt sarl")
                                    {
                                        var code = from salesR in db.SalesR
                                                   where salesR.SalesRep == orderRow.SalesRep
                                                   select salesR;
                                        orderRow.InvoiceNumber = "F" + DateTime.Now.Year.ToString().Substring(2) + "-" + code.FirstOrDefault().Code.Trim() + orderRow.InvoiceDate.ToString("MM");
                                    }
                                    else
                                        o.InvoiceNumber = "JJ";
                                }
                            }
                            if (string.IsNullOrEmpty(orderRow.PO_Number))
                                orderRow.PO_Number = "automatic input " + DateTime.Now;
                            o.InvoiceDate = orderRow.InvoiceDate;
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
                            o.Status = "entered";
                            query = from ord in db.Orders
                                    where ord.OrderNumber == o.OrderNumber && ord.idxx != orderRow.idxx
                                    select ord;
                            if (query.Count() > 0)
                            {
                                foreach (Order od in query.ToList())
                                {
                                    od.PO_Number = o.PO_Number;
                                    od.OrderDate = o.OrderDate;
                                    od.InvoiceDate = o.InvoiceDate;
                                    od.InvoiceNumber = o.InvoiceNumber;
                                }
                            }
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
                var ord = db.Orders.Where(x => x.OrderNumber == id && x.Status != "lost").OrderBy(y => y.idxx).ToList();
                var accNum = ord.FirstOrDefault().AccountNumber;
                var cmp = db.Companies.Where(y => y.AccountNumber == accNum).FirstOrDefault();
                ViewBag.ButtonBook = db.Orders.Where(x => x.OrderNumber == id && (x.Status == "entered" || x.Status == "preloaded")).ToList().Count > 0;
                ViewBag.ButtonCheck = (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal"))
                    && (db.Orders.Where(x => x.OrderNumber == id && x.Status == "booked").ToList().Count > 0);
                ViewBag.ButtonReject = (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") && (db.Orders.Where(x => x.OrderNumber == id && x.Status == "checked").ToList().Count > 0))
                || (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal") && (db.Orders.Where(x => x.OrderNumber == id && x.Status == "booked").ToList().Count > 0));
                ViewBag.ButtonApprove = Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") && (db.Orders.Where(x => x.OrderNumber == id && x.Status == "checked").ToList().Count > 0);
                ViewBag.ButtonPrMail = (Roles.IsUserInRole(WebSecurity.CurrentUserName, "Admin") || Roles.IsUserInRole(WebSecurity.CurrentUserName, "Internal"))
                    && (db.Orders.Where(x => x.OrderNumber == id && (x.Status == "approved" || x.Status == "invoiced")).ToList().Count > 0
                    && cmp.AccountStatus.Contains("03 - Premium Customer"));

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
                            where ord.OrderNumber == orderNumber && (ord.Status == "entered" || ord.Status == "preloaded")
                            select ord;
                if (query.Count() > 0)
                {
                    var destmail = "";
                    MailMessage mail = null;
                    foreach (Order o in query.ToList())
                    {
                        o.Status = "booked";
                        db.SaveChanges();
                        var lic = db.Licenses.Where(l => l.LicenseID == o.LicenseID).FirstOrDefault();
                        if (o.ProductName == "dongle")
                            lic = db.Licenses.Where(l => l.LicenseID == "L00000001").FirstOrDefault();
                        if (o.ProductName == "penalty")
                            lic = db.Licenses.Where(l => l.LicenseID == "L00000002").FirstOrDefault();
                        if (o.ProductName == "service")
                            lic = db.Licenses.Where(l => l.LicenseID == "L00000042").FirstOrDefault();

                        if (string.IsNullOrEmpty(destmail))
                        {
                            if (o.Invoicer.Trim().ToLower() == "t3 japan kk")
                                destmail = db.Companies.Where(x => x.AccountName == "t3 japan kk").FirstOrDefault().Email;
                            else
                                destmail = db.Companies.Where(x => x.AccountNumber == o.InvoicedNumber).FirstOrDefault().Email;

                            mail = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["hostusername"], "Orders@dptcorporate.com");
                            //if (o.LineType != "activation")
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
                        "You can browse the orders at https://dpt3.dptcorporate.com/Order" +
                        "<br/><br/>Best regards,<br/>DPT Accounting";
                    mail.IsBodyHtml = true;
                    try
                    {
                        MailHelper.SendMail(mail);
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteLog("OrderController (Book): ordernumber - " + orderNumber + " -- " + e.Message + "-" + e.InnerException);
                    }
                }
            }
            return Json("Booked OrderNumber: " + orderNumber, JsonRequestBehavior.AllowGet);
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
                        db.SaveChanges();
                        var lic = db.Licenses.Where(l => l.LicenseID == o.LicenseID).FirstOrDefault();
                        if (o.ProductName == "dongle")
                            lic = db.Licenses.Where(l => l.LicenseID == "L00000001").FirstOrDefault();
                        if (o.ProductName == "penalty")
                            lic = db.Licenses.Where(l => l.LicenseID == "L00000002").FirstOrDefault();
                        if (o.ProductName == "service")
                            lic = db.Licenses.Where(l => l.LicenseID == "L00000042").FirstOrDefault();

                        if (string.IsNullOrEmpty(destmail))
                        {
                            if (o.Invoicer.Trim().ToLower() == "t3 japan kk")
                                destmail = db.Companies.Where(x => x.AccountName == "t3 japan kk").FirstOrDefault().Email;
                            else
                                destmail = db.Companies.Where(x => x.AccountNumber == o.InvoicedNumber).FirstOrDefault().Email;

                            mail = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["hostusername"], "Orders@dptcorporate.com");
                            //if (o.LineType != "activation")
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
                        "You can browse the orders at https://dpt3.dptcorporate.com/Order" +
                        "<br/><br/>Best regards,<br/>DPT Accounting";
                    mail.IsBodyHtml = true;
                    try
                    {
                        MailHelper.SendMail(mail);
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteLog("OrderController (check): ordernumber - " + orderNumber + " -- " + e.Message + "-" + e.InnerException);
                    }
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
                        db.SaveChanges();
                        if (string.IsNullOrEmpty(destmail))
                        {
                            if (o.Invoicer.Trim().ToLower() == "t3 japan kk")
                                destmail = db.Companies.Where(x => x.AccountName == "t3 japan kk").FirstOrDefault().Email;
                            else
                                destmail = db.Companies.Where(x => x.AccountNumber == o.InvoicedNumber).FirstOrDefault().Email;
                            MailMessage mail = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["hostusername"], destmail);
                            mail.Subject = "[DO NOT REPLY] Order rejected for " + o.AccountName.Trim() + " (" + o.AccountNumber + ")";
                            mail.Body = "Dear Sir, \n\nThe Order #: " + orderNumber + " has been rejected.\n\n" +
                                "Account Name: " + o.AccountName.Trim() + "; PO number: " + o.PO_Number + "; Order date: " + o.OrderDate + "\n\n" +
                                "You can browse and check the orders at https://dpt3.dptcorporate.com/Order" +
                                "\n\nBest regards,\nDPT orders";
                            try
                            {
                                MailHelper.SendMail(mail);
                            }
                            catch (Exception e)
                            {
                                LogHelper.WriteLog("OrderController (Reject): ordernumber - " + orderNumber + " -- " + e.Message + "-" + e.InnerException);
                            }
                        }
                    }
                }
            }
            return Json("Rejected OrderNumber: " + orderNumber + "\n\n an e-mail was sent to " + destmail, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Delete(string orderNumber, string idxx)
        {
            if (string.IsNullOrEmpty(orderNumber) || string.IsNullOrEmpty(idxx))
                return Json("Something went wrong. Cannot delete the order!", JsonRequestBehavior.AllowGet);

            using (var db = new DptContext())
            {
                var query = db.Orders.Where(x => x.idxx == idxx && x.OrderNumber == orderNumber).FirstOrDefault();
                if (query != null && query.Status.ToLower() == "lost")
                    return Json("Something went wrong. Cannot delete the order!", JsonRequestBehavior.AllowGet);

                db.Database.ExecuteSqlCommand("DELETE [dbo].[DPT_Orders] WHERE ordernumber='" + orderNumber + "' and idxx='" + idxx + "'");
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
            double free = 0;
            using (var db = new DptContext())
            {
                var query = from ord in db.Orders
                            where ord.OrderNumber == orderNumber && ord.Status == "checked"
                            select ord;
                if (query.Count() > 0)
                {
                    MailMessage mail = null;
                    var inv = "";
                    var lang = "";
                    var cmp = db.Companies.Where(x => x.AccountNumber == query.FirstOrDefault().AccountNumber).FirstOrDefault();
                    foreach (Order o in query.ToList())
                    {
                        o.Status = o.Invoicer == "t3 japan kk" ? "invoiced" : "approved";
                        free += o.Ordered;
                        var lic = db.Licenses.Where(l => l.LicenseID == o.LicenseID).FirstOrDefault();

                        if (lic != null && lic.ArticleDetail.ToLower() != "pl" && !lic.LicenseFlag.ToLower().StartsWith("new"))
                            lic.Renew = 1;

                        if (o.ArticleDetail == "plss" || o.ArticleDetail == "cvu" || o.ArticleDetail == "qsf")
                            lic.MaintEndDate = o.EndDate;

                        if (o.MachineID.Contains("BLU"))
                            lic.MaintEndDateT = o.EndDate;

                        if (o.ArticleDetail == "plss" && !o.MachineID.Contains("BLU"))
                            lic.MaintEndDateT = o.EndDate.AddDays(60);

                        if (o.ProductName.ToLowerInvariant() == "tdviewerplus" && o.Ordered > 0)
                            lic.LicenseFlag = "regular";

                        if (o.ArticleDetail == "asf" || o.ArticleDetail == "asp")
                        {
                            lic.MaintEndDate = o.EndDate;
                            lic.EndDate = o.EndDate;
                            lic.MaintEndDateT = o.EndDate;
                            lic.MaintStartDate = o.StartDate;
                        }

                        db.SaveChanges();

                        if (lic != null && lic.LicenseFlag.ToLower().StartsWith("new"))
                        {
                            if (o.LineType.ToLower() != "freeofcharge")
                                db.Database.ExecuteSqlCommand("UPDATE [dbo].[DPT_Licenses] Set licenseFlag = 'regular', [2Bins] = 1 WHERE licenseID = '" + o.LicenseID + "'");
                            else
                            {
                                if (cmp.AccountKind != "educational")
                                    db.Database.ExecuteSqlCommand("UPDATE [dbo].[DPT_Licenses] Set licenseFlag = 'free', [2Bins] = 1 WHERE licenseID = '" + o.LicenseID + "'");
                                else
                                    db.Database.ExecuteSqlCommand("UPDATE [dbo].[DPT_Licenses] Set licenseFlag = 'educational', [2Bins] = 1 WHERE licenseID = '" + o.LicenseID + "'");
                            }
                            //lic.MaintEndDateT = o.EndDate.AddDays(60);
                        }

                        db.Database.ExecuteSqlCommand("UPDATE [dbo].[DPT_Licenses] Set [ExportedNum] = 0 WHERE licenseID = '" + o.LicenseID + "'");

                        if (string.IsNullOrEmpty(varmail))
                        {
                            var clmail = db.Companies.Where(x => x.AccountNumber == o.AccountNumber).FirstOrDefault().Email;
                            varmail = db.Companies.Where(x => x.AccountNumber == o.InvoicedNumber).FirstOrDefault().Email;

                            inv = o.Invoicer.Trim().ToLower();
                            lang = db.Companies.Where(x => x.AccountNumber == o.InvoicedNumber).FirstOrDefault().Language.ToLower();
                            mail = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["hostusername"], clmail);
                            mail.CC.Add(varmail);
                            if (inv == "t3 japan kk")
                                mail.CC.Add(db.Companies.Where(x => x.AccountName == "t3 japan kk").FirstOrDefault().Email);

                            mail.Bcc.Add("Orders@dptcorporate.com");

                            MailHeader(orderNumber, mail, lang, o, lic);
                        }
                        else
                            mail.Body += "<tr><td>" + o.LicenseID + "</td><td>" + (lic != null ? lic.MachineID : "BLKABCDEFGH") + "</td><td>" + o.Item + "</td><td>" +
                                o.LicenseType + "</td><td>" + o.Quantity + "</td><td>" + o.StrStartDate + "</td><td>" + o.StrEndDate + "</td></tr>";
                    }
                    MailFooter(mail, lang);
                    mail.IsBodyHtml = true;
                    try
                    {
                        MailHelper.SendMail(mail);
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteLog("OrderController (Approve): ordernumber - " + orderNumber + " -- " + e.Message + "-" + e.InnerException);
                    }
                }
                query = from ord in db.Orders
                        where ord.OrderNumber == orderNumber && ord.Status == "approved" && ord.ArticleDetail != "sas"
                        select ord;
                if (query.Count() > 0)
                {
                    foreach (Order o in query.ToList())
                    {
                        var lic = db.Licenses.Where(x => x.LicenseID == o.LicenseID).FirstOrDefault();
                        lic.Sas = false;
                        db.SaveChanges();
                    }
                }
                query = from ord in db.Orders
                        where ord.OrderNumber == orderNumber && ord.Status == "approved" && ord.ArticleDetail == "sas"
                        select ord;
                if (query.Count() > 0)
                {
                    foreach (Order o in query.ToList())
                    {
                        var lic = db.Licenses.Where(x => x.LicenseID == o.LicenseID).FirstOrDefault();
                        lic.Sas = true;
                        db.SaveChanges();
                    }
                }
                var order = db.Orders.Where(x => x.OrderNumber == orderNumber).FirstOrDefault();
                db.Database.ExecuteSqlCommand("exec [dbo].[Update_AccountStatus] @AccountNumber = '" + order.AccountNumber + "'");
                var comp = db.Companies.Where(x => x.AccountNumber == order.AccountNumber).FirstOrDefault();
                var licprm = db.Licenses.Where(x => x.AccountNumber == comp.AccountNumber
                        && x.LicenseFlag == "premium" && x.MachineID.Contains("ABCDEFGH")).Count();

                if (comp.AccountStatus.Contains("03 - Premium Customer") && licprm > 0 && free > 0)
                {
                    var email = comp.Email;
                    MailMessage mail = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["hostusername"], email); ;
                    varmail = db.Companies.Where(x => x.AccountNumber == order.InvoicedNumber).FirstOrDefault().Email;
                    mail.CC.Add(varmail);
                    if (order.Invoicer.Trim().ToLower() == "t3 japan kk")
                        mail.CC.Add(db.Companies.Where(x => x.AccountName == "t3 japan kk").FirstOrDefault().Email);
                    mail.Bcc.Add("Orders@dptcorporate.com");

                    if (comp.Language == "italian")
                    {
                        mail.Subject = "[DO NOT REPLY] Licenze Premium " + comp.AccountName + " (" + comp.AccountNumber + ")";
                        mail.Body = "Gentile Cliente,<br/><br/>La ringraziamo per il Suo ordine.<br/><br/>" +
                        "La informiamo che nella sezione <b>Licenses</b> del sito DPT3Care (https://dpt3.dptcorporate.com) " +
                        "troverà, oltre alla/e licenza/e regolarmente acquistata/e, anche <b>" + licprm + " Licenza/e Premium</b>: " +
                        "si tratta di un omaggio che DPT fa ai suoi <b>Clienti Premium</b>, vale a dire le aziende " +
                        "che hanno tutte le licenze sotto contratto attivo di manutenzione.<br/><br/>Ogni Licenza Premium " +
                        "ha durata di 30 giorni a partire dal momento dell’attivazione e può essere utilizzata per " +
                        "gestire picchi di lavoro nell’ufficio tecnico, stagisti, ecc.<br/>Per installarla/e in " +
                        "modalità self-service, quando sarà necessario, troverà qui di seguito le istruzioni: " +
                        "http://www.dptcorporate.com/it/guida-auto-installazione/." +
                        "<br/><br/>Restiamo a Sua disposizione per ulteriori informazioni.<br/>Buon lavoro e buona giornata!" +
                        "<br/><br/>DPT Accounting";
                    }
                    else if (comp.Language == "japanese")
                    {
                        mail.Subject = "[このメールには返信しないでください] PREMIUM - プレミアムライセンスについて " + comp.AccountName +
                            " (" + comp.AccountNumber + ") 様";
                        mail.Body = "ThinkDesignユーザー様,<br/><br/>この度はThinkDesignのご注文を頂きまして誠にありがとうございました。<br/><br/>" +
                        "ご注文頂いたライセンスのほか、お客様のカスタマーサイト DPT3Care website (https://dpt3.dptcorporate.com) " +
                        "に、プレミアムライセンス " + licprm + " 本が表示されております。<br/>プレミアムライセンスは、弊社DPTがプレミアムカスタマー" +
                        "（すべてのライセンスが有効なお客様）の 皆様へギフトとして提供される、取得手続きから 30 " +
                        "日間有効なライセンスです。お客様の繁忙期、臨時 スタッフやインターンの受入れ時、社内トレーニング、ご出張時などに活用頂けます。" +
                        "<br/><br/>ライセンスの取得については次の「セルフインストールガイド」をご参照ください。（方法A）" +
                        "http://www.dptcorporate.com/ja/self-installation-guide/" +
                        "<br/><br/>また、「製品インストールガイド」の「２－２．ライセンスの取得」も合わせてご参照ください。" +
                        "ftp://ftp.t3-japan.co.jp/tdExtra/InstallGuide/InstallGuide.pdf" +
                        "<br/><br/>以上、今後ともどうぞ宜しくお願い致します。.<br/><br/>DPT アカウンティング";
                    }
                    else
                    {
                        mail.Subject = "[DO NOT REPLY] Premium Licenses " + comp.AccountName + " (" + comp.AccountNumber + ")";
                        mail.Body = "Dear User,<br/><br/>Thank you for your order.<br/><br/>" +
                        "Please note that, besides the purchased license/s, in the <b>Licenses</b> section of " +
                        "DPT3Care website (https://dpt3.dptcorporate.com) you’ll also find <b>" + licprm +
                        " Premium License/s</b>.<br/>Premium Licenses are free 1-month licenses that DPT " +
                        "gives as a gift only to its <b>Premium Customers</b> (= companies with all licenses under " +
                        "maintenance), and they can be used for managing peak period activities, temporary staff, interns " +
                        "training, travels or whatever the reason.<br/><br/>You’ll be able to activate them whenever " +
                        "you want by following the instructions at this link: http://www.dptcorporate.com/self-installation-guide/." +
                        "<br/><br/>Have a good day!<br/>Best regards,<br/><br/>DPT Accounting";

                    }
                    mail.IsBodyHtml = true;
                    try
                    {
                        MailHelper.SendMail(mail);
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteLog("OrderController (Approve): ordernumber - " + orderNumber + " -- " + e.Message + "-" + e.InnerException);
                    }
                }
            }
            return Json("Approved OrderNumber: " + orderNumber + "\n\n an e-mail was sent to " + varmail, JsonRequestBehavior.AllowGet);
        }

        private static void MailFooter(MailMessage mail, string lang)
        {
            if (lang == "japanese")
            {
                mail.Body += "</table><br/><br/>ライセンス管理の詳細につきましては下記「インストールガイド」の「２．ライセンス」をご覧ください。<br/>" +
                    "(ftp://ftp.t3-japan.co.jp/tdExtra/InstallGuide/InstallGuide.pdf)<br/><br/>" +
                    "think3 製品の最新のリリースは、下記カスタマーケアサイトのダウンロードエリアからダウンロードすることができます。<br/>" +
                    "(https://dpt3.dptcorporate.com/Download)<br/><br/>" +
                    "下記 URL からカスタマーケアへアクセスして、ライセンスを取得してください。<br/>" +
                    "(https://dpt3.dptcorporate.com/license)<br/><br/>" +
                    "なお、ご注文いただいたライセンスと登録されたライセンスに相違がないかどうかを必ずご確認ください。<br/>" +
                    "上記「製品」の項目で製品名に続くアルファベットは次の種別を表します。<br/>" +
                    "ASF/ASP (年間使用料）、PL（永久）、PLSS（永久ライセンスのメンテナンスライセンス）<br/>" +
                    "SAS（バンドルパッケージのサポートサービス部分）<br/><br/>" +
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
                    "(https://dpt3.dptcorporate.com/Download)<br/><br/>" +
                    "아래 URL에서 고객 센터에 접속하여 라이선스를 취득해야합니다.<br/>" +
                    "(https://dpt3.dptcorporate.com/license)<br/><br/>" +
                    "또한, 주문하신 라이선스와  등록된 라이선스가 차이가 있는지를반드시 확인하시기 바랍니다. <br/>" +
                    "라이선스 유형은 ASF (연간 라이선스), PL (영구라이선스), PLSS (영구 라이선스의 유지 보수 라이선스) 등의 종류가 있습니다.<br/><br/>" +
                    "PLSS 라이선스는 유지보수 라이선스이기 때문에, 고객 지원 사이트에서 새로운 라이선스가 늘어나는 것은 아닙니다. <br/>" +
                    "자세한 내용은 위 설치 안내서의 '2-4. 라이선스 갱신'을 확인하시기 바랍니다.<br/><br/>" +
                    " 당사는 평가 / 대여용 라이선스에 대해 비용을 부과하지 않습니다.<br/><br/>" +
                    "기타 문의 사항은 고객 지원 사이트에서 문의해 주세요.<br/>think3 고객 센터 직원";
                else
                {
                    if (lang == "italian")
                    {
                        mail.Body += "</table><br/>(*) I prodotti ASF o PL sono pronti per l’installazione in self-service.<br/><br/>" +
                            //"You can browse the orders at http://dpt3.dptcorporate.com/Order" +
                            "<br/><br/>Cordiali saluti,<br/><br/>DPT Accounting";
                    }
                    else
                        mail.Body += "</table><br/>(*) ASF or PL items are ready for self-installation<br/><br/>" +
                            //"You can browse the orders at http://dpt3.dptcorporate.com/Order" +
                            "<br/><br/>Best regards,<br/><br/>DPT Accounting";
                }
            }
        }

        private static void MailHeader(string orderNumber, MailMessage mail, string lang, Order o, LicenseView lic)
        {
            if (lang == "japanese")
            {
                mail.Subject = "[このメールには返信しないでください]" + o.AccountName.Trim() + " (" + o.AccountNumber + ") 様、think3製品が登録されました　(#" + orderNumber + ")";
                mail.Body = "<b>" + o.AccountName.Trim() + " (" + o.AccountNumber + ")</b> 様<br/><br/>" +
                    "下記の通りライセンスが登録されておりますのでお知らせいたします。<br/><br/>" +
                    "アカウント名: <b>" + o.AccountName.Trim() + " (" + o.AccountNumber + ")</b>" +
                    "<br/>注文日: <b>" + o.StrOrderDate + "</b><br/><br/>" +
                    "<table border=1><tr>" + "<td>ライセンスID</td>" + "<td>マシンID</td>" + "<td>製品</td>" +
                    "<td>タイプ</td>" + "<td>数</td>" + "<td>開始日</td>" + "<td>終了日</td></tr>" +
                    "<tr><td>" + o.LicenseID + "</td><td>" + (lic != null ? lic.MachineID : "BLKABCDEFGH") + "</td><td>" + o.Item + "</td><td>" +
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
                        "<tr><td>" + o.LicenseID + "</td><td>" + (lic != null ? lic.MachineID : "BLKABCDEFGH") + "</td><td>" + o.Item + "</td><td>" +
                        o.LicenseType + "</td><td>" + o.Quantity + "</td><td>" + o.StrStartDate + "</td><td>" + o.StrEndDate + "</td></tr>";
                }
                else
                {
                    if (lang == "italian")
                    {
                        mail.Subject = "[DO NOT REPLY] Ordine approvato per " + o.AccountName.Trim() + " (" + o.AccountNumber + ")";
                        mail.Body = "Gentile Cliente, <br/><br/>L’Ordine #" + orderNumber + " è stato approvato.<br/><br/>" +
                            "Nome Account: " + o.AccountName.Trim() + " (" + o.AccountNumber + ")" + "<br/>Data ordine: " + o.StrOrderDate +
                            "<br/>Numero PO: " + o.PO_Number + "<br/><br/>" +
                            "<table border=1><tr>" + "<td>ID Licenza</td>" + "<td>ID Macchina</td>" + "<td>Prodotto</td>" +
                            "<td>Tipo</td>" + "<td>Quantità</td>" + "<td>Data Inizio</td>" + "<td>Data Scadenza</td></tr>" +
                            "<tr><td>" + o.LicenseID + "</td><td>" + (lic != null ? lic.MachineID : "BLKABCDEFGH") + "</td><td>" + o.Item + "</td><td>" +
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
                            "<tr><td>" + o.LicenseID + "</td><td>" + (lic != null ? lic.MachineID : "BLKABCDEFGH") + "</td><td>" + o.Item + "</td><td>" +
                            o.LicenseType + "</td><td>" + o.Quantity + "</td><td>" + o.StrStartDate + "</td><td>" + o.StrEndDate + "</td></tr>";
                    }
                }
            }
        }

        [Authorize(Roles = "Admin,Internal")]
        public ActionResult PremiumMail(string orderNumber)
        {
            if (string.IsNullOrEmpty(orderNumber))
            {
                ViewBag.ok1 = "Something went wrong. Cannot sent the mail!";
                return View("Success");
            }
            var email = "";
            using (var db = new DptContext())
            {
                var query = from ord in db.Orders
                            where ord.OrderNumber == orderNumber && (ord.Status == "approved" || ord.Status == "invoiced")
                            select ord;
                if (query.Count() > 0)
                {
                    var cmp = db.Companies.Where(y => y.AccountNumber == query.FirstOrDefault().AccountNumber).FirstOrDefault();
                    email = cmp.Email;
                    var licprm = db.Licenses.Where(x => x.AccountNumber == cmp.AccountNumber
                        && x.LicenseFlag == "premium" && x.MachineID.Contains("ABCDEFGH")).Count();
                    if (licprm < 1)
                        return Json("There are no premium licenses. Cannot sent the mail!", JsonRequestBehavior.AllowGet);

                    MailMessage mail = new MailMessage(System.Configuration.ConfigurationManager.AppSettings["hostusername"], email); ;
                    var varmail = db.Companies.Where(x => x.AccountNumber == query.FirstOrDefault().InvoicedNumber).FirstOrDefault().Email;
                    mail.CC.Add(varmail);
                    if (query.FirstOrDefault().Invoicer.Trim().ToLower() == "t3 japan kk")
                        mail.CC.Add(db.Companies.Where(x => x.AccountName == "t3 japan kk").FirstOrDefault().Email);
                    mail.Bcc.Add("Orders@dptcorporate.com");

                    if (cmp.Language == "italian")
                    {
                        mail.Subject = "[DO NOT REPLY] Licenze Premium " + cmp.AccountName + " (" + cmp.AccountNumber + ")";
                        mail.Body = "Gentile Cliente,<br/><br/>La ringraziamo per il Suo ordine.<br/><br/>" +
                        "La informiamo che nella sezione <b>Licenses</b> del sito DPT3Care (https://dpt3.dptcorporate.com) " +
                        "troverà, oltre alla/e licenza/e regolarmente acquistata/e, anche <b>" + licprm + " Licenza/e Premium</b>: " +
                        "si tratta di un omaggio che DPT fa ai suoi <b>Clienti Premium</b>, vale a dire le aziende " +
                        "che hanno tutte le licenze sotto contratto attivo di manutenzione.<br/><br/>Ogni Licenza Premium " +
                        "ha durata di 30 giorni a partire dal momento dell’attivazione e può essere utilizzata per " +
                        "gestire picchi di lavoro nell’ufficio tecnico, stagisti, ecc.<br/>Per installarla/e in " +
                        "modalità self-service, quando sarà necessario, troverà qui di seguito le istruzioni: " +
                        "http://www.dptcorporate.com/it/guida-auto-installazione/." +
                        "<br/><br/>Restiamo a Sua disposizione per ulteriori informazioni.<br/>Buon lavoro e buona giornata!" +
                        "<br/><br/>DPT Accounting";
                    }
                    else
                    {
                        mail.Subject = "[DO NOT REPLY] Premium Licenses " + cmp.AccountName + " (" + cmp.AccountNumber + ")";
                        mail.Body = "Dear User,<br/><br/>Thank you for your order.<br/><br/>" +
                        "Please note that, besides the purchased license/s, in the <b>Licenses</b> section of " +
                        "DPT3Care website (https://dpt3.dptcorporate.com) you’ll also find <b>" + licprm +
                        " Premium License/s</b>.<br/>Premium Licenses are free 1-month licenses that DPT " +
                        "gives as a gift only to its <b>Premium Customers</b> (= companies with all licenses under " +
                        "maintenance), and they can be used for managing peak period activities, temporary staff, interns " +
                        "training, travels or whatever the reason.<br/><br/>You’ll be able to activate them whenever " +
                        "you want by following the instructions at this link: http://www.dptcorporate.com/self-installation-guide/." +
                        "<br/><br/>Have a good day!<br/>Best regards,<br/><br/>DPT Accounting";

                    }
                    mail.IsBodyHtml = true;
                    try
                    {
                        MailHelper.SendMail(mail);
                    }
                    catch (Exception e)
                    {
                        LogHelper.WriteLog("OrderController (PremiumMail): ordernumber - " + orderNumber + " -- " + e.Message + "-" + e.InnerException);
                    }
                }
            }
            return Json("Mail sent: " + email, JsonRequestBehavior.AllowGet);
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
                orderRow.Invoicer = sr.Invoicer.Contains("dpt") ? "dpt sarl" : sr.Invoicer;
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
                var lf = db.LicFlag.Where(x => x.Order == 1).Select(k => k.LicenseFlag).ToList();
                var products = db.Licenses.Where(u => u.AccountNumber == company.AccountNumber && lf.Contains(u.LicenseFlag)).Select(x => x.ProductName).Distinct().ToList();
                if (!products.Contains("dongle"))
                    products.Add("dongle");
                if (!products.Contains("penalty"))
                    products.Add("penalty");
                if (!products.Contains("service"))
                    products.Add("service");
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
                if (productName == "dongle")
                {
                    var dongle = db.Licenses.Where(l => l.LicenseID == "L00000001").ToList();
                    return Json(dongle, JsonRequestBehavior.AllowGet);
                }
                if (productName == "penalty")
                {
                    var penalty = db.Licenses.Where(l => l.LicenseID == "L00000002").ToList();
                    return Json(penalty, JsonRequestBehavior.AllowGet);
                }
                if (productName == "service")
                {
                    var service = db.Licenses.Where(l => l.LicenseID == "L00000042").ToList();
                    return Json(service, JsonRequestBehavior.AllowGet);
                }
                var company = db.Companies.Where(c => c.AccountName == companyName).FirstOrDefault();
                var lf = db.LicFlag.Where(x => x.Order == 1).Select(k => k.LicenseFlag).ToList();
                var licIds = db.Licenses.Where(u => u.AccountNumber == company.AccountNumber && u.ProductName == productName
                    && lf.Contains(u.LicenseFlag)).ToList();
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
                var lc = db.Licenses.Where(u => u.LicenseID == licenseId).FirstOrDefault();
                lic.LicenseType = lc.LicenseType;
                lic.Quantity = lc.Quantity;
                if (licenseId == "L00000001" || licenseId == "L00000002")
                {
                    lic.PwdCode = DateTime.Now.ToString("yyyy-MM-dd");
                    lic.Ancestor = DateTime.Now.ToString("yyyy-MM-dd");
                }
                else
                {
                    lic.PwdCode = ((DateTime)lc.MaintEndDate).AddDays(1).ToString("yyyy-MM-dd");
                    lic.Ancestor = ((DateTime)lc.MaintEndDate).AddDays(365).ToString("yyyy-MM-dd");
                    var chkYear = Convert.ToDateTime(lic.Ancestor);
                    if (DateTime.IsLeapYear(chkYear.Year) &&
                        Convert.ToDateTime(lic.PwdCode) < new DateTime(chkYear.Year, 02, 28) &&
                        Convert.ToDateTime(lic.Ancestor) > new DateTime(chkYear.Year, 02, 27))
                        lic.Ancestor = ((DateTime)lc.MaintEndDate).AddDays(366).ToString("yyyy-MM-dd");
                }
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
            if (articleDetail == "srv")
                return Json("1000_150000", JsonRequestBehavior.AllowGet);
            if (articleDetail == "sas")
            {
                switch (productName)
                {
                    case "tdengineering":
                        return Json("0_80000", JsonRequestBehavior.AllowGet);
                    case "tdbase":
                        return Json("0_70000", JsonRequestBehavior.AllowGet);
                    case "tddrafting":
                        return Json("0_50000", JsonRequestBehavior.AllowGet);
                    default:
                        return Json("0_100000", JsonRequestBehavior.AllowGet);
                }
            }

            if (productName == "tdprofessionaledu")
                productName = "tdprofessional";

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
                    case "msf":
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
                    europrice = europrice + ((europrice * 20) / 100);
                    jpyprice = jpyprice + ((jpyprice * 20) / 100);
                }
                switch (articleDetail)
                {
                    case "qsf":
                        europrice = europrice / 4;
                        jpyprice = jpyprice / 4;
                        break;
                    case "msf":
                        europrice = europrice / 12;
                        jpyprice = jpyprice / 12;
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