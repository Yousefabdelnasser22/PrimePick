using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Repository
{
    public static class RoleSeeder
    {
        public static async Task RoleSeed(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin" , "Seller"  , "Customer" };

            foreach (var role in roles)
            {
                if ( !await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole (role));
                }
            }

        }
    }
}
