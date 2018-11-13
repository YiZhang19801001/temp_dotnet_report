using demoBusinessReport.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Helpers
{
    public class ShopDbContext: DbContext
    {
        public string _cn;
        public DbSet<Docket> Docket { get; set; }
        public DbSet<Return> Returns { get; set; }
        public DbSet<ReturnsLine> ReturnsLine { get; set; }
        public DbSet<DocketLine> DocketLine { get; set; }
        public DbSet<Stock> Stock { get; set; }
        public DbSet<Audit> Audit { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<SalesOrder> SalesOrder { get; set; }
        public DbSet<Payments> Payments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder option)
        {

            /** Only Change is here use static class to store connection string, 
             * that static variable MUST have validate default value, 
             * otherwise Error occurs when Inject this service or DB */
            option.UseSqlServer(DataHelper.con);
        }
    }
}
