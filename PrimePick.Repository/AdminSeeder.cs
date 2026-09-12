using Microsoft.AspNetCore.Identity;
using PrimePick.Core.Models;
using PrimePick.Repository.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Repository
{
    public static class AdminSeeder
    {
        public static async Task AdminSeed(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
        {

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(
                    new IdentityRole("Admin")
                );
            }


            var admin = await userManager.FindByEmailAsync(
                "Admin@gmail.com"
            );


            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = "Admin@gmail.com",
                    Email = "Admin@gmail.com",
                    EmailConfirmed = true
                };


                await userManager.CreateAsync(
                    admin,
                    "Jo123456789@"
                );


                await userManager.AddToRoleAsync(
                    admin,
                    "Admin"
                );
            }
        }
    }

}
