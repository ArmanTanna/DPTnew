using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DPTnew.Models
{
    [Table("ProductList_Safenet")]
    public class SafenetProductList
    {
        [Key]
        public string PRDName { get; set; }
    }
}