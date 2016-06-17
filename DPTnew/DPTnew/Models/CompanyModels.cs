using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace DPTnew.Models
{
    [Table("Companies_Site")]
    public class CompanyView
    {
        [Key]
        [Display(Name = "Account #")]
        public string AccountNumber { get; set; }
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }
        [Display(Name = "Kind")]
        public string AccountKind { get; set; }
        [Display(Name = "Status")]
        public string AccountStatus { get; set; }
        [Display(Name = "Status")]
        public string ShortStatus { get; set; }
        public string Address { get; set; }
        public string ZIP { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string Region { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string Email { get; set; }
        public string SalesRep { get; set; }
        public string SalesRegion { get; set; }
        public string LastExp { get; set; }
        public string ServCompany1 { get; set; }
        public string ServCompany2 { get; set; }
        public string ServCompany3 { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        //public int Blocked { get; set; }

        [ScriptIgnore(ApplyToOverrides = true)]
        public virtual ICollection<LicenseView> Licenses { get; set; }
    }

    [Table("DPT_SafenetCompanies")]
    public class SafenetComapny
    {
        [Key]
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Language { get; set; }
    }

    [Table("DPT_Companies")]
    public class Company
    {
        public Company()
        {
            //this.Contacts = new HashSet<Contact>();
            //this.Licenses = new HashSet<LicenseView>();
            //this.SalesR = new SalesR();
        }

        [Key]
        [Display(Name = "Account Num.")]
        public string AccountNumber { get; set; }
        [Display(Name = "Account Name")]
        public string AccountName { get; set; }
        //[Display(Name = "Kind")]
        //public string AccountKind { get; set; }
        [Display(Name = "Status")]
        public string AccountStatus { get; set; }
        //public string Address { get; set; }
        //public string ZIP { get; set; }
        //public string City { get; set; }
        //public string Province { get; set; }
        //public string Country { get; set; }
        public string SalesRep { get; set; }
        public int Blocked { get; set; }
        //[System.Web.Script.Serialization.ScriptIgnore]
        //public virtual ICollection<Contact> Contacts { get; set; }
        //[System.Web.Script.Serialization.ScriptIgnore]
        //public virtual ICollection<LicenseView> Licenses { get; set; }
        //[System.Web.Script.Serialization.ScriptIgnore]
        //public virtual SalesR SalesR { get; set; }

        //public object ToJson()
        //{
        //    return new
        //    {
        //        AccountName = AccountName,
        //        AccountNumber = AccountNumber,
        //        AccountKind = AccountKind,
        //        AccountStatus = AccountStatus,
        //        Address = Address,
        //        ZIP = ZIP,
        //        City = City,
        //        Province = Province,
        //        Country = Country,
        //        SalesRep = SalesRep,
        //        Blocked = Blocked
        //    };
        //}

    }
}