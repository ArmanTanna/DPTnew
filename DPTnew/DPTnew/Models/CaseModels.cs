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
    [Table("DPT_Cases")]
    public class DptCases
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CaseId { get; set; }
        public DateTime CreatedOn { get; set; }
        [Display(Name = "Submitted On")]
        public string StrCreatedOn { get { return CreatedOn != null ? ((DateTime)CreatedOn).ToString("yyyy-MM-dd") : ""; } }
        public DateTime ModifiedOn { get; set; }
        [Display(Name = "Modified On")]
        public string StrModifiedOn { get { return ModifiedOn != null ? ((DateTime)ModifiedOn).ToString("yyyy-MM-dd") : ""; } }
        public string Description { get; set; }
        public string Details { get; set; }
        public string Product { get; set; }
        public string ProductVersion { get; set; }
        public string Severity { get; set; }
        public string Platform { get; set; }
        public string PlatformVersion { get; set; }
        public string MachineId { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        [Display(Name = "Assigned To")]
        public string CCEngineer { get; set; }
        public int CCEngineerId { get; set; }
        public string Contact { get; set; }
        public int ContactId { get; set; }
        public string File { get; set; }
    }

    public class UpdateCase
    {
        public int CaseId { get; set; }
        public DateTime CreatedOn { get; set; }
        [Display(Name = "Submitted On")]
        public string StrCreatedOn { get { return CreatedOn != null ? ((DateTime)CreatedOn).ToString("yyyy-MM-dd") : ""; } }
        public DateTime ModifiedOn { get; set; }
        [Display(Name = "Modified On")]
        public string StrModifiedOn { get { return ModifiedOn != null ? ((DateTime)ModifiedOn).ToString("yyyy-MM-dd") : ""; } }
        public string Description { get; set; }
        public string Details { get; set; }
        public string Product { get; set; }
        public string ProductVersion { get; set; }
        public string Severity { get; set; }
        public string Platform { get; set; }
        public string PlatformVersion { get; set; }
        public string MachineId { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string AccountNumber { get; set; }
        public string AccountName { get; set; }
        [Display(Name = "Assigned To")]
        public string CCEngineer { get; set; }
        public int CCEngineerId { get; set; }
        public string Contact { get; set; }
        public int ContactId { get; set; }
        public HttpPostedFileBase file { get; set; }
    }

    [Table("DPT_CaseLog")]
    public class DptCaseHistory
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int CaseLogId { get; set; }
        public int CaseId { get; set; }
        public DateTime CreatedOn { get; set; }
        public string StrCreatedOn { get { return CreatedOn != null ? ((DateTime)CreatedOn).ToString("yyyy-MM-dd") : ""; } }
        public string Description { get; set; }
        public string Details { get; set; }
        public string File { get; set; }
    }

}