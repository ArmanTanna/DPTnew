using DPTnew.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace DPTnew.Models
{
    [Table("Orders_Site")]
    public class Order
    {
        [Key]
        [Display(Name = "Row number")]
        public int idxx { get; set; }
        public string Invoicer { get; set; }
        public string InvoicedName { get; set; }
        public string InvoicedNumber { get; set; }
        [Display(Name = "AccountName", ResourceType = typeof(Resource))]
        public string AccountName { get; set; }
        [Display(Name = "AccountNumber", ResourceType = typeof(Resource))]
        public string AccountNumber { get; set; }
        public string OrderNumber { get; set; }
        public DateTime OrderDate { get; set; }
        [Display(Name = "Order Date")]
        public string StrOrderDate { get { return OrderDate != null ? ((DateTime)OrderDate).ToString("yyyy-MM-dd") : ""; } }
        public string PO_Number { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime InvoiceDate { get; set; }
        [Display(Name = "Invoice Date")]
        public string StrInvoiceDate { get { return InvoiceDate != null ? ((DateTime)InvoiceDate).ToString("yyyy-MM-dd") : ""; } }
        public string NewOldAccount { get; set; }
        public string SalesRep { get; set; }
        public string Currency { get; set; }
        public string LineType { get; set; }
        public string ProductName { get; set; }
        public string ArticleDetail { get; set; }
        public DateTime StartDate { get; set; }
        [Display(Name = "Start Date")]
        public string StrStartDate { get { return StartDate != null ? ((DateTime)StartDate).ToString("yyyy-MM-dd") : ""; } }
        public DateTime EndDate { get; set; }
        [Display(Name = "End Date")]
        public string StrEndDate { get { return EndDate != null ? ((DateTime)EndDate).ToString("yyyy-MM-dd") : ""; } }
        public DateTime? RequestDate { get; set; }
        [Display(Name = "Request Date")]
        public string StrRequestDate { get { return RequestDate != null ? ((DateTime)RequestDate).ToString("yyyy-MM-dd") : ""; } }
        public double Ordered { get; set; }
        public double? Invoiced { get; set; }
        public double? Quantity { get; set; }
        public string LicenseType { get; set; }
        public string NewRenewal { get; set; }
        public double? EURO_PriceList { get; set; }
        public double? JPY_PriceList { get; set; }
        public string LeasingCompany { get; set; }
        public string LicenseID { get; set; }
        public string Status { get; set; }
    }

}
