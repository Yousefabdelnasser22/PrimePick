using Microsoft.AspNetCore.Identity;
using PrimePick.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Service.Services.Identity
{
    public class RoleService(UserManager<ApplicationUser> userManager , RoleManager<IdentityRole> roleManager)
    {
        public async Task<bool> AssignRole(string userId , string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null) 
            { 
                return false;
            }

            var role = await roleManager.RoleExistsAsync(roleName);
            if (!role)
            {
                return false;
            }

            var result = await userManager.AddToRoleAsync(user , roleName);

            return result.Succeeded;
        }


        public async Task<bool> UnAssignRole(string userId, string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

           

            var result = await userManager.RemoveFromRoleAsync(user, roleName);

            return result.Succeeded;
        }
    }


    }

