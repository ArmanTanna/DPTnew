using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DPTnew.Models
{
    [Table("service_unknownIP")]
    public class UnknownIP
    {
        [Key]
        public int Id { get; set; }
        public string ipcustomer { get; set; }
        public string Country { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }
}