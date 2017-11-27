using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DPTnew.Models
{
    [Table("service_SegmentIndustry")]
    public class SegmentIndustry
    {
        [Key]
        public string ID { get; set; }
        public string Segment { get; set; }
        public string Industry { get; set; }

    }
}