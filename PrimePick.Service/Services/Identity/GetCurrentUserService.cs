using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PrimePick.Core.Models;
using PrimePick.Core.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Service.Services.Identity
{
    public class GetCurrentUserService(IHttpContextAccessor _httpContextAccessor ,UserManager<ApplicationUser> userManager) : IGetCurrentUserService
    {
        Task<ApplicationUser> IGetCurrentUserService.GetUser()
        {
            var userId = _httpContextAccessor.HttpContext?.User
            .FindFirstValue(ClaimTypes.NameIdentifier);

            var user = userManager.FindByIdAsync(userId);

            return user;

        }
    }
}
