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
    [Table("DPT_Erp")]
    public class DptErp
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int ActivityId { get; set; }
        [Display(Name = "User")]
        public string Email { get; set; }
        public DateTime ActivityDate { get; set; }
        [Display(Name = "Activity Date")]
        public string StrActivityDate { get { return ActivityDate != null ? ((DateTime)ActivityDate).ToString("yyyy-MM-dd") : ""; } }
        [Display(Name = "ReadOnly")]
        public bool Status { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal? TimeSpent { get; set; }
    }

    [Table("service_ActivityTitles")]
    public class ActivityTitles
    {
        [Key]
        public string Email { get; set; }
        public string Activities { get; set; }
    }

}