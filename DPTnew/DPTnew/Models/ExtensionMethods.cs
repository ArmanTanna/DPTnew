using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace DPTnew.Models
{
    public static class ExtensionMethods
    {
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source != null && toCheck != null && source.IndexOf(toCheck, comp) >= 0;
        }

        public static SearchParams GetSearchParams(this HttpRequestBase source) 
        {
            var sp = new SearchParams
            {
                Start = int.Parse(source.Form["start"]),
                Length = int.Parse(source.Form["length"]),
                Draw = int.Parse(source.Form["draw"]),
                Value = source.Form["search[value]"],
                OrderDir = source.Form["order[0][dir]"] ?? string.Empty,
                SearchOn = string.Empty,
                OrderBy = "AccountNumber"
            };

            int orderColumn = int.Parse(source.Form["order[0][column]"]);
            if (orderColumn > 0)
                sp.OrderBy = source.Form[string.Format("columns[{0}][data]", orderColumn)];

            string regex = @"columns\[(\d+)\]\[search\]\[value\]";
            if (string.IsNullOrEmpty(sp.Value))
            {
              var colIndex = (from k in source.Form.AllKeys where Regex.Match(k, regex).Success && !string.IsNullOrEmpty(source.Form[k]) select Regex.Match(k, regex).Groups[1].Value).FirstOrDefault();
              sp.Value = source.Form[string.Format("columns[{0}][search][value]", colIndex)];
              sp.SearchOn = source.Form[string.Format("columns[{0}][data]", colIndex)];
            }

            return sp;
        }
    }
}