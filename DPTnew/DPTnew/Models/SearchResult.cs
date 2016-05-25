using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DPTnew.Models
{
    public class SearchResult<T>
    {
        public long recordsFiltered { get; set; }
        public long recordsTotal { get; set; }
        public int draw { get; set; }
        public IEnumerable<T> data { get; set; }
    }
}