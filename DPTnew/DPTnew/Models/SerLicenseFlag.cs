using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DPTnew.Models
{
    [Table("service_LicenseFlag")]
    public class SerLicenseFlag
    {
        [Key]
        public string LicenseFlag { get; set; }
        public string Group { get; set; }
        public int Order { get; set; }
        public int Export_Safenet { get; set; }
        public int Install_Safenet { get; set; }
        public int ChangeVersion_Safenet { get; set; }
        public int Renewal_Safenet { get; set; }
        public int Install_Legacy { get; set; }
        public int ChangeVersion_Legacy { get; set; }
        public int Renewal_Legacy { get; set; }
    }
}