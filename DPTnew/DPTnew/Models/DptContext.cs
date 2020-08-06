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
        public DbSet<SafenetCompany> SafenetCompanies { get; set; }
        public DbSet<LicenseView> Licenses { get; set; }
        public DbSet<SerLicenseFlag> LicFlag { get; set; }
        public DbSet<People> Peoples { get; set; }
        public DbSet<DptErp> ErpRows { get; set; }
        public DbSet<ActivityTitles> ActivityTitles { get; set; }
        public DbSet<PeopleRoles> PeopleRoles { get; set; }
        public DbSet<MainWebRoles> MainWebRoles { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<DptCases> Cases { get; set; }
        public DbSet<DptCaseHistory> CaseHistories { get; set; }
        public DbSet<DptCaseLog> CaseLogs { get; set; }
        public DbSet<DptCaseArchive> CaseArchive { get; set; }
        public DbSet<DptLicenseLog> LicenseLogs { get; set; }
        public DbSet<Activation> Activations { get; set; }
        public DbSet<SegmentIndustry> Segind { get; set; }
        public DbSet<CampaignComapny> CmpCompanies { get; set; }
        public DbSet<UnknownIP> uCust { get; set; }
        public DbSet<SafenetProductList> SafenetProducts { get; set; }
        public DbSet<SpecialCompany> SpecialCompanies { get; set; }
        public DbSet<Chart> chart { get; set; }
        public DbSet<Chart2> chart2 { get; set; }
        public DbSet<Chart3> chart3 { get; set; }
        public DbSet<Chart4> chart4 { get; set; }
        public DbSet<Chart5> chart5 { get; set; }
        public DbSet<Chart6> chart6 { get; set; }
        public DbSet<Chart7> chart7 { get; set; }

        public IEnumerable<T> Search<T>(SearchParams sp, IEnumerable<T> datasource)
        {
            IEnumerable<T> allrecords = new List<T>();
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

                foreach (T record in datasource)
                    foreach (string col in columns)
                    {
                        string propValue = (string)(record.GetType().GetProperty(col).GetValue(record, null));
                        if (!string.IsNullOrEmpty(propValue) && propValue.Contains(sp.Value, StringComparison.CurrentCultureIgnoreCase))
                        {
                            ((IList<T>)allrecords).Add(record);
                            break;
                        }
                    }
            }
            else
                allrecords = datasource;

            return allrecords;
        }

        public SearchResult<T> ConvertToSearchResult<T>(SearchParams sp, IEnumerable<T> allRecords)
        {
            var filtered = allRecords.OrderBy(sp.OrderBy + " " + sp.OrderDir).ToList();
            var data = filtered.Skip(sp.Start).Take(sp.Length).ToList();
            var fCount = filtered.Count();
            var tCount = allRecords.Count();
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