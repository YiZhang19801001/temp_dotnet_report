using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.IO;

namespace demoBusinessReport.Helpers
{
    public static class SeedHelper
    {
        public static async Task Seed(IServiceProvider provider)
        {
            var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = scopeFactory.CreateScope())
            {
                //create serveices objects to manage data
                UserManager<IdentityUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                RoleManager<IdentityRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                //add Customer role
                if (await roleManager.FindByNameAsync("Customer") == null)
                {
                    await roleManager.CreateAsync(new IdentityRole("Customer"));
                }
                //add Admin role
                if (await roleManager.FindByNameAsync("Admin") == null)
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                //add default Admin
                if (await userManager.FindByNameAsync("Admin") == null)
                {
                    IdentityUser admin = new IdentityUser("Admin");
                    await userManager.CreateAsync(admin, "admin233###");
                    await userManager.AddToRoleAsync(admin, "Admin");
                }

                //create sample customer and its profile
                if (await userManager.FindByNameAsync("Zhang") == null)
                {
                    //create sample customer
                    IdentityUser sampleCustomer = new IdentityUser("Zhang");
                    await userManager.CreateAsync(sampleCustomer, "aaa333");
                    await userManager.AddToRoleAsync(sampleCustomer, "Customer");
                }

            }
        }
    }
}
