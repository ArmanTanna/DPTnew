using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Linq.Dynamic;

namespace DPTnew.Models
{
    public class DptContext : DbContext
    {
        public DptContext()
            : base("DefaultConnection")
        {
        }


        public DbSet<Contact> Contacts { get; set; }
        //public DbSet<Company> Companies { get; set; }
        public DbSet<SalesR> SalesR { get; set; }
        public DbSet<CompanyView> Companies { get; set; }
        public DbSet<SafenetComapny> SafenetCompanies { get; set; }
        public DbSet<LicenseView> Licenses { get; set; }
        public DbSet<People> Peoples { get; set; }
        public DbSet<DptErp> ErpRows { get; set; }
        public DbSet<ActivityTitles> ActivityTitles { get; set; }
        public DbSet<PeopleRoles> PeopleRoles { get; set; }
        public DbSet<MainWebRoles> MainWebRoles { get; set; }
        public DbSet<Order> Orders { get; set; }

        public IEnumerable<T> Search<T>(SearchParams sp, IEnumerable<T> datasource)
        {
            IEnumerable<T> allCompanies = new List<T>();
            if (!string.IsNullOrEmpty(sp.Value))
            {
                var columns = new List<string>();
                if (!string.IsNullOrEmpty(sp.SearchOn)) // search on fixed column
                    columns.Add(sp.SearchOn);
                else
                    //get all string properties of the CompanyView type
                    foreach (var prop in typeof(T).GetProperties())
                        if (prop.PropertyType == typeof(string))
                            columns.Add(prop.Name);

                foreach (T company in datasource)
                    foreach (string col in columns)
                    {
                        string propValue = (string)(company.GetType().GetProperty(col).GetValue(company, null));
                        if (!string.IsNullOrEmpty(propValue) && propValue.Contains(sp.Value, StringComparison.CurrentCultureIgnoreCase))
                        {
                            ((IList<T>)allCompanies).Add(company);
                            break;
                        }
                    }
            }
            else
                allCompanies = datasource;

            return allCompanies;
        }

        public SearchResult<T> ConvertToSearchResult<T>(SearchParams sp, IEnumerable<T> allCompanies)
        {
            var filtered = allCompanies.OrderBy(sp.OrderBy + " " + sp.OrderDir).ToList();
            var data = filtered.Skip(sp.Start).Take(sp.Length).ToList();
            var fCount = filtered.Count();
            var tCount = allCompanies.Count();
            return new SearchResult<T>
            {
                data = data,
                draw = ++sp.Draw,
                recordsFiltered = fCount,
                recordsTotal = tCount
            };
        }
    }
}