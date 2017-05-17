using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DPTnew.Models
{
    [Table("service_SalesRegions")]
    public class SalesR
    {
        public SalesR()
        {
            this.Companies = new HashSet<Company>();

        }

        [Key]
        public string SalesRep { get; set; }
        public string SalesRegion { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string SalesProvince { get; set; }
        public string Invoicer { get; set; }
        public string Code { get; set; }

        public virtual ICollection<Company> Companies { get; set; }

    }
}