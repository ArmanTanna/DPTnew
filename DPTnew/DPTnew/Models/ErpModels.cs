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
        [Column(Order = 0)]
        [Display(Name = "User")]
        public string Email { get; set; }
        [Key]
        [Column(Order = 1)]
        public DateTime ActivityDate { get; set; }
        [Display(Name = "Activity Date")]
        public string StrActivityDate { get { return ActivityDate != null ? ((DateTime)ActivityDate).ToString("yyyy-MM-dd") : ""; } }
        [Display(Name = "ReadOnly")]
        public bool Status { get; set; }
        public string Title1 { get; set; }
        public string Title2 { get; set; }
        public string Title3 { get; set; }
        public string Title4 { get; set; }
        public string Title5 { get; set; }
        public string Title6 { get; set; }
        public string Title7 { get; set; }
        public string Title8 { get; set; }
        public string Title9 { get; set; }
        public string Title10 { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
        public string Description3 { get; set; }
        public string Description4 { get; set; }
        public string Description5 { get; set; }
        public string Description6 { get; set; }
        public string Description7 { get; set; }
        public string Description8 { get; set; }
        public string Description9 { get; set; }
        public string Description10 { get; set; }
        public decimal? TimeSpent1 { get; set; }
        public decimal? TimeSpent2 { get; set; }
        public decimal? TimeSpent3 { get; set; }
        public decimal? TimeSpent4 { get; set; }
        public decimal? TimeSpent5 { get; set; }
        public decimal? TimeSpent6 { get; set; }
        public decimal? TimeSpent7 { get; set; }
        public decimal? TimeSpent8 { get; set; }
        public decimal? TimeSpent9 { get; set; }
        public decimal? TimeSpent10 { get; set; }
        public decimal? Total { get; set; }

    }

}