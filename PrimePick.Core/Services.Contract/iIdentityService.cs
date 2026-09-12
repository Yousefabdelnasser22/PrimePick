using Microsoft.AspNetCore.Identity;
using PrimePick.Core.DTOs.User;
using PrimePick.Core.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Services.Contract
{
        public interface iIdentityService
    {
        Task<IdentityResult> Register(ApplicationUserDTO userDTO);
        Task<AuthResponseDTO?> Login(LoginDTO userDTO);
        Task<AuthResponseDTO?> RefreshToken(RefreshTokenDTO refreshTokenDTO);
        Task<IdentityResult> ConfirmEmail(ConfirmEmailDTO confirmEmailDTO);
        Task<bool> ResendConfirmationEmail(string email);


    }
}
