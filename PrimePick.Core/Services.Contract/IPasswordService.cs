using Microsoft.AspNetCore.Identity;
using PrimePick.Core.DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Services.Contract
{
    public interface IPasswordService
    {
        Task ForgotPasswordAsync(string email);

        Task<IdentityResult> ResetPasswordAsync(
            ResetPasswordDto request);
    }
}
