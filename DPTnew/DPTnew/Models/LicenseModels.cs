﻿using DPTnew.Localization;
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
    //[Table("DPT_Licenses")]
    //public class License
    //{
    //    [Key]
    //    [Display(Name = "Licence ID")]
    //    public string LicenseID { get; set; }

    //    public string AccountNumber { get; set; }

    //     [Display(Name = "Product")]
    //    public string ProductName { get; set; }

    //    [Display(Name = "Article")]
    //    public string ArticleDetail { get; set; }

    //    public int Quantity { get; set; }

    //     [Display(Name = "Type")]
    //    public string LicenseType { get; set; }
    //    public string MachineID { get; set; }
    //    public string Ancestor { get; set; }

    //    [Display(Name = "Start Date")]
    //    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
    //    public Nullable<System.DateTime> StartDate { get; set; }

    //    [Display(Name = "Expiry")]
    //    [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
    //    public Nullable<System.DateTime> EndDate { get; set; }
    //     [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
    //    public Nullable<System.DateTime> MaintStartDate { get; set; }
    //     [Display(Name = "MED")]
    //     [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
    //    public Nullable<System.DateTime> MaintEndDate { get; set; }
    //    public string Version { get; set; }
    //    public string OriginalProduct { get; set; }
    //    public string LicenseKind { get; set; }
    //    public string Note { get; set; }
    //    public int Installed { get; set; }
    //    public int Exported { get; set; }
    //    public int Import { get; set; }

    //    public virtual Company Company { get; set; }
    //}

    [Table("Licenses_Site")]
    public class LicenseView
    {
        [Key]
        [Display(Name = "LicenseId", ResourceType = typeof(Resource))]
        public string LicenseID { get; set; }

        [Display(Name = "AccountNumber", ResourceType = typeof(Resource))]
        public string AccountNumber { get; set; }

        [Display(Name = "AccountName", ResourceType = typeof(Resource))]
        public string AccountName { get; set; }

        [Display(Name = "Status", ResourceType = typeof(Resource))]
        public string AccountStatus { get; set; }

        [Display(Name = "Product", ResourceType = typeof(Resource))]
        public string ProductName { get; set; }

        [Display(Name = "Article", ResourceType = typeof(Resource))]
        public string ArticleDetail { get; set; }

        [Display(Name = "Quantity", ResourceType = typeof(Resource))]
        public int Quantity { get; set; }

        [Display(Name = "Type", ResourceType = typeof(Resource))]
        public string LicenseType { get; set; }

        public string LicenseFlag { get; set; }

        [Display(Name = "MachineID", ResourceType = typeof(Resource))]
        public string MachineID { get; set; }
        [Display(Name = "Ancestor", ResourceType = typeof(Resource))]
        public string Ancestor { get; set; }
        public string Ancestor2 { get; set; }
        public string Ancestor3 { get; set; }

        [Display(Name = "StartDate", ResourceType = typeof(Resource))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> StartDate { get; set; }

        [Display(Name = "EndDate", ResourceType = typeof(Resource))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> EndDate { get; set; }
        [Display(Name = "MaintStartDate", ResourceType = typeof(Resource))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> MaintStartDate { get; set; }

        [ScriptIgnore(ApplyToOverrides = true)]
        [Display(Name = "MED", ResourceType = typeof(Resource))]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<DateTime> MaintEndDate { get; set; }
        [Display(Name = "MED", ResourceType = typeof(Resource))]
        public string MED { get { return MaintEndDate != null ? ((DateTime)MaintEndDate).ToString("yyyy-MM-dd") : ""; } }
        [Display(Name = "MaintStartDate", ResourceType = typeof(Resource))]
        public string MSD { get { return MaintEndDate != null ? ((DateTime)MaintStartDate).ToString("yyyy-MM-dd") : ""; } }
        [Display(Name = "EndDate", ResourceType = typeof(Resource))]
        public string ED { get { return MaintEndDate != null ? ((DateTime)EndDate).ToString("yyyy-MM-dd") : ""; } }
        [Display(Name = "StartDate", ResourceType = typeof(Resource))]
        public string SD { get { return MaintEndDate != null ? ((DateTime)StartDate).ToString("yyyy-MM-dd") : ""; } }

        public Nullable<DateTime> MaintEndDateT { get; set; }

        public string PwdCode { get; set; }

        [Display(Name = "Version", ResourceType = typeof(Resource))]
        public string Version { get; set; }
        //public string LicenseKind { get; set; }
        [Display(Name = "Note", ResourceType = typeof(Resource))]
        public string Note { get; set; }
        public int Installed { get; set; }
        public int Exported { get; set; }
        public int Import { get; set; }
        public int Renew { get; set; }

        public string SalesRep { get; set; }
        public string SalesRegion { get; set; }

        [Display(Name = "LastExp", ResourceType = typeof(Resource))]
        public string LastExp { get; set; }

        [Display(Name = "Type", ResourceType = typeof(Resource))]
        public string ServLicense1 { get; set; }
        public string Vend_String { get; set; }
        public int? FlexType { get; set; }
        [Display(Name = "ExportedNum", ResourceType = typeof(Resource))]
        public int ExportedNum { get; set; }
        [Display(Name = "MaxExport", ResourceType = typeof(Resource))]
        public int MaxExport { get; set; }
        public int TotExported { get; set; }
        [Display(Name = "OriginalProduct", ResourceType = typeof(Resource))]
        public string OriginalProduct { get; set; }
        [Display(Name = "Action", ResourceType = typeof(Resource))]
        public string Action { get; set; }
        public int Release { get; set; }
        public bool Sas { get; set; }
        public bool physic { get; set; }
    }

    [Table("DPT_LicenseLog")]
    public class DptLicenseLog
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int LicenseLogId { get; set; }
        public string LicenseID { get; set; }
        public string MachineID { get; set; }
        public string Action { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string C2VFileName { get; set; }
        public string VersionFrom { get; set; }
        public string VersionTo { get; set; }
    }

    public class LicenseState
    {
        public string LicenseID { get; set; }
        public string Version { get; set; }
        public string LicenseType { get; set; }
        public string LicenseFlag { get; set; }
        public string MachineID { get; set; }
        public string ProductName { get; set; }
        public Nullable<System.DateTime> MaintEndDate { get; set; }
        public int Installed { get; set; }
        public int Exported { get; set; }
        public int Import { get; set; }
        public int Renew { get; set; }
        public string ArticleDetail { get; set; }
        public string PwdCode { get; set; }
        public string salesRep { get; set; }
        public int Export_Safenet { get; set; }
        public int Install_Safenet { get; set; }
        public int ChangeVersion_Safenet { get; set; }
        public int Renewal_Safenet { get; set; }
        public int Install_Legacy { get; set; }
        public int ChangeVersion_Legacy { get; set; }
        public int Renewal_Legacy { get; set; }
        public int Blocked { get; set; }
    }

    public class LicenseBase
    {
        [Display(Name = "LicenseId", ResourceType = typeof(Resource))]
        public string LicenseID { get; set; }

        [Display(Name = "Product", ResourceType = typeof(Resource))]
        public string ProductName { get; set; }

        [Display(Name = "Article", ResourceType = typeof(Resource))]
        public string ArticleDetail { get; set; }

        [Display(Name = "Quantity", ResourceType = typeof(Resource))]
        public int Quantity { get; set; }

        [Display(Name = "Type", ResourceType = typeof(Resource))]
        public string LicenseType { get; set; }

        public string LicenseFlag { get; set; }

        //[Display(Name = "Protection Key")]
        [Display(Name = "MachineID", ResourceType = typeof(Resource))]
        public string MachineID { get; set; }

        //[Display(Name = "Maintenance End Date")]
        [Display(Name = "MED", ResourceType = typeof(Resource))]
        public string MaintEndDate { get; set; }

        [Display(Name = "Version", ResourceType = typeof(Resource))]
        public string Version { get; set; }

    }

    public class Entitlement
    {
        [Display(Name = "LicenseId", ResourceType = typeof(Resource))]
        public string LicenseID { get; set; }

        [Display(Name = "Product", ResourceType = typeof(Resource))]
        public string ProductName { get; set; }

        [Display(Name = "Article", ResourceType = typeof(Resource))]
        public string ArticleDetail { get; set; }

        [Display(Name = "Quantity", ResourceType = typeof(Resource))]
        public int Quantity { get; set; }

        [Display(Name = "Type", ResourceType = typeof(Resource))]
        public string LicenseType { get; set; }

        public string LicenseFlag { get; set; }

        //[Display(Name = "Protection Key")]
        [Display(Name = "MachineID", ResourceType = typeof(Resource))]
        public string MachineID { get; set; }

        //[Display(Name = "Maintenance End Date")]
        [Display(Name = "MED", ResourceType = typeof(Resource))]
        public string MaintEndDate { get; set; }

        [Display(Name = "Version", ResourceType = typeof(Resource))]
        public string Version { get; set; }

        [Display(Name = ".C2V File")]
        [DataType(DataType.Upload)]
        public HttpPostedFileBase file { get; set; }

    }

    public class SafenetEntitlement
    {
        public string CrmId { get; set; }

        public string EntType { get; set; }

        public string refId1 { get; set; }

        public string refId2 { get; set; }

        public JArray ProductName { get; set; }

        public static IEnumerable<string> AddTTeamDocTo = new List<string> { "BO", "BY", "UD", "UE", "UP", "US", "UT", "UZ", "TL" };//tdbase,tdengineeringplus,tddrafting,tdengineering,tdprofessional,tdstyling,tdtooling,tdmolding,tdviewerplus
        public static IEnumerable<string> TDVARBundle = new List<string> { "TDEducation", "thinkteamdev", "tdxchangereader", "tdirectcatiarw", "tdirectparasolidrw", "tdirectproerw", "tdpartsolutions", "thinkapi_gsm", "thinkprint" }; //TDVARLight
        public static IEnumerable<string> DPTVARBundle = new List<string> { "tdprofessional", "thinkteamdev", "tdxchangereader", "tdirectcatiarw", "tdirectparasolidrw", "tdirectproerw", "tdpartsolutions", "thinkapi_gsm", "thinkprint" }; //per enrico, TDVARFull 
        public static IEnumerable<string> TDIRECTBundle = new List<string> { "tdirectcatiarw", "tdirectparasolidrw", "tdirectproerw" };
    }

    public class SafenetEvalPlLocalEntitlment : SafenetEntitlement
    {
        public bool Encoded { get; set; }

        public string C2V { get; set; }

        public string ProtectionKeyId { get; set; }
    }

    public class SafenetNetAsfLocalEntitlment : SafenetEntitlement
    {
        public JArray SaotParams { get; set; }

        public bool Encoded { get; set; }

        public string C2V { get; set; }

        public string ProtectionKeyId { get; set; }
    }

    public class SafenetUpdateEntitlment : SafenetEntitlement
    {
        public string ProtectionKeyId { get; set; }

        public bool Encoded { get; set; }

        public string C2V { get; set; }
    }

}