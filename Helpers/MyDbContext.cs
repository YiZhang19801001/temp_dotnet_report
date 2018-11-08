using demoBusinessReport.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace demoBusinessReport.Services
{
    public class MyDbContext : IdentityDbContext
    {

        public DbSet<Shop> Shop { get; set; }
        public DbSet<UserShop> TblUserShop { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder option)
        {
            option.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB ; Database=demoLoginControl; Trusted_Connection=True");
        }

        //public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }

    }
}
