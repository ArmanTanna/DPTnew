using DPTnew.Localization;
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
    [Table("Case_Site")]
    public class DptCases
    {
        [Key]
        [Display(Name = "CaseID", ResourceType = typeof(Resource))]
        public string CaseId { get; set; }
        public DateTime CreatedOn { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public string StrCreatedOn { get { return CreatedOn != null ? ((DateTime)CreatedOn).ToString("yyyy-MM-dd HH:mm") : ""; } }
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        [Display(Name = "ModifiedOn", ResourceType = typeof(Resource))]
        public string StrModifiedOn { get { return ModifiedOn != null ? ((DateTime)ModifiedOn).ToString("yyyy-MM-dd HH:mm") : ""; } }
        [Display(Name = "Description", ResourceType = typeof(Resource))]
        public string Description { get; set; }
        [Display(Name = "Details", ResourceType = typeof(Resource))]
        public string Details { get; set; }
        [Display(Name = "Product", ResourceType = typeof(Resource))]
        public string Product { get; set; }
        [Display(Name = "Version", ResourceType = typeof(Resource))]
        public string ProductVersion { get; set; }
        [Display(Name = "Severity", ResourceType = typeof(Resource))]
        public string Severity { get; set; }
        [Display(Name = "Platform", ResourceType = typeof(Resource))]
        public string Platform { get; set; }
        [Display(Name = "PlatformVersion", ResourceType = typeof(Resource))]
        public string PlatformVersion { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Resource))]
        public string Status { get; set; }
        [Display(Name = "Type", ResourceType = typeof(Resource))]
        public string Type { get; set; }
        public string AccountNumber { get; set; }
        [Display(Name = "AccountName", ResourceType = typeof(Resource))]
        public string AccountName { get; set; }
        [Display(Name = "Language", ResourceType = typeof(Resource))]
        public string Language { get; set; }
        [Display(Name = "AssignedTo", ResourceType = typeof(Resource))]
        public string CCEngineer { get; set; }
        public int? CCEngineerId { get; set; }
        [Display(Name = "Contact", ResourceType = typeof(Resource))]
        public string Contact { get; set; }
        public int? ContactId { get; set; }
        public string EDBnumber { get; set; }
    }

    public class UpdateCase
    {
        [Display(Name = "CaseID", ResourceType = typeof(Resource))]
        public string CaseId { get; set; }
        public DateTime CreatedOn { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public string StrCreatedOn { get { return CreatedOn != null ? ((DateTime)CreatedOn).ToString("yyyy-MM-dd") : ""; } }
        public DateTime ModifiedOn { get; set; }
        [Display(Name = "ModifiedOn", ResourceType = typeof(Resource))]
        public string StrModifiedOn { get { return ModifiedOn != null ? ((DateTime)ModifiedOn).ToString("yyyy-MM-dd") : ""; } }
        [Display(Name = "Description", ResourceType = typeof(Resource))]
        public string Description { get; set; }
        [Display(Name = "Details", ResourceType = typeof(Resource))]
        public string Details { get; set; }
        [Display(Name = "Product", ResourceType = typeof(Resource))]
        public string Product { get; set; }
        [Display(Name = "Version", ResourceType = typeof(Resource))]
        public string ProductVersion { get; set; }
        [Display(Name = "Severity", ResourceType = typeof(Resource))]
        public string Severity { get; set; }
        [Display(Name = "Platform", ResourceType = typeof(Resource))]
        public string Platform { get; set; }
        [Display(Name = "PlatformVersion", ResourceType = typeof(Resource))]
        public string PlatformVersion { get; set; }
        [Display(Name = "Status", ResourceType = typeof(Resource))]
        public string Status { get; set; }
        [Display(Name = "Type", ResourceType = typeof(Resource))]
        public string Type { get; set; }
        public string AccountNumber { get; set; }
        [Display(Name = "AccountName", ResourceType = typeof(Resource))]
        public string AccountName { get; set; }
        [Display(Name = "AssignedTo", ResourceType = typeof(Resource))]
        public string CCEngineer { get; set; }
        public int CCEngineerId { get; set; }
        [Display(Name = "Contact", ResourceType = typeof(Resource))]
        public string Contact { get; set; }
        public int ContactId { get; set; }
        public HttpPostedFileBase File { get; set; }
        public string fileName { get; set; }
        public string EDBnumber { get; set; }
    }

    [Table("DPT_CaseHistory")]
    public class DptCaseHistory
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "CaseHistoryID", ResourceType = typeof(Resource))]
        public int CaseHistoryId { get; set; }
        [Display(Name = "CaseID", ResourceType = typeof(Resource))]
        public string CaseId { get; set; }
        public DateTime CreatedOn { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public string StrCreatedOn { get { return CreatedOn != null ? ((DateTime)CreatedOn).ToString("yyyy-MM-dd HH:mm") : ""; } }
        [Display(Name = "CreatedBy", ResourceType = typeof(Resource))]
        public string CreatedBy { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Resource))]
        public string Description { get; set; }
        [Display(Name = "Details", ResourceType = typeof(Resource))]
        public string Details { get; set; }
        [Display(Name = "File", ResourceType = typeof(Resource))]
        public string File { get; set; }
    }

    [Table("DPT_CaseLog")]
    public class DptCaseLog
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CaseLogId { get; set; }
        public string CaseId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string Status { get; set; }
    }

    [Table("CaseArchive_Site")]
    public class DptCaseArchive
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        [Display(Name = "CaseHistoryID", ResourceType = typeof(Resource))]
        public int CaseHistoryId { get; set; }
        [Display(Name = "CaseID", ResourceType = typeof(Resource))]
        public string CaseId { get; set; }
        public DateTime CreatedOn { get; set; }
        [Display(Name = "CreatedOn", ResourceType = typeof(Resource))]
        public string StrCreatedOn { get { return CreatedOn != null ? ((DateTime)CreatedOn).ToString("yyyy-MM-dd HH:mm") : ""; } }
        [Display(Name = "CreatedBy", ResourceType = typeof(Resource))]
        public string CreatedBy { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Resource))]
        public string Description { get; set; }
        [Display(Name = "Details", ResourceType = typeof(Resource))]
        public string Details { get; set; }
        [Display(Name = "Product", ResourceType = typeof(Resource))]
        public string Product { get; set; }
        [Display(Name = "Version", ResourceType = typeof(Resource))]
        public string ProductVersion { get; set; }
        [Display(Name = "Platform", ResourceType = typeof(Resource))]
        public string Platform { get; set; }
        public string EDBnumber { get; set; }
    }

}