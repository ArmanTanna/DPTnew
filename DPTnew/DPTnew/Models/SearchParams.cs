using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DPTnew.Models
{
    public class SearchParams
    {
        public int Start { get; set; }
        public int Length { get; set; }
        public int Draw { get; set; }
        public string OrderDir { get; set; }
        public string OrderBy { get; set; }
        public string Value { get; set; }
        public string SearchOn { get; set; }
    }
}