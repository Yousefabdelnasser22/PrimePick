using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using PrimePick.Core.DTOs.User;
using PrimePick.Core.Models;
using PrimePick.Core.Services.Contract;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Service.Services.Identity
{
    public class IdentityService : iIdentityService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;
        private readonly IEmailSenderService emailSender;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
           IEmailSenderService emailSender)
        {
            this.userManager = userManager;
            this.configuration = configuration;
            this.emailSender = emailSender;
        }

        public async Task<IdentityResult> Register(ApplicationUserDTO userDTO)
        {
            var user = new ApplicationUser
            {
                UserName = userDTO.UserName,
                Email = userDTO.Email
            };

            var result = await userManager.CreateAsync(user, userDTO.Password);

            if (!result.Succeeded)
                return result;

            // توليد الـ confirmation token
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            var baseUrl = configuration["BaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException(
                    "BaseUrl is missing from appsettings.json.");

            var confirmationLink =
                $"{baseUrl.TrimEnd('/')}/api/identity/confirm-email" +
                $"?userId={Uri.EscapeDataString(user.Id)}" +
                $"&token={Uri.EscapeDataString(token)}";

            var emailBody = $@"
                <h2>Confirm Your Email</h2>
                <p>Please click the link below to confirm your account:</p>
                <a href='{confirmationLink}'>Confirm Email</a>";

            await emailSender.SendEmailAsync(user.Email!, "Confirm your email", emailBody);

            return result;
        }

        public async Task<IdentityResult> ConfirmEmail(ConfirmEmailDTO confirmEmailDTO)
        {
            var user = await userManager.FindByIdAsync(confirmEmailDTO.UserId);

            if (user is null)
                return IdentityResult.Failed(new IdentityError
                {
                    Description = "User not found."
                });

            if (user.EmailConfirmed)
                return IdentityResult.Success;

            return await userManager.ConfirmEmailAsync (user, confirmEmailDTO.Token);
        }

        public async Task<bool> ResendConfirmationEmail(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user is null || user.EmailConfirmed)
                return false;

            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);

            var baseUrl = configuration["BaseUrl"];

            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new InvalidOperationException(
                    "BaseUrl is missing from appsettings.json.");

            var confirmationLink =
                $"{baseUrl.TrimEnd('/')}/api/identity/confirm-email" +
                $"?userId={Uri.EscapeDataString(user.Id)}" +
                $"&token={Uri.EscapeDataString(token)}";

            var emailBody = $@"
                <h2>Confirm Your Email</h2>
                <a href='{confirmationLink}'>Confirm Email</a>";

            await emailSender.SendEmailAsync(user.Email!, "Confirm your email", emailBody);

            return true;
        }

        public async Task<AuthResponseDTO?> Login(LoginDTO userDTO)
        {
            
            var user = await userManager.FindByNameAsync(userDTO.UserName);

            if (user is null)
                return null;

            // التأكد من كلمة المرور
            var passwordIsCorrect = await userManager.CheckPasswordAsync(user, userDTO.Password);

            if (!passwordIsCorrect)
                return null;

            // التأكد من تفعيل الإيميل
            if (!user.EmailConfirmed)
                return null; // ممكن تتعامل معاها بشكل مختلف - هنشرح تحت

            // إنشاء Access Token
            var accessTokenResult = await CreateAccessToken(user);

            // إنشاء Refresh Token
            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            var updateResult = await userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return null;

            return new AuthResponseDTO
            {
                AccessToken = accessTokenResult.Token,
                AccessTokenExpiration = accessTokenResult.Expiration,
                RefreshToken = user.RefreshToken
            };
        }

        public async Task<AuthResponseDTO?> RefreshToken(RefreshTokenDTO refreshTokenDTO)
        {
            var user = await userManager.Users
                .SingleOrDefaultAsync(user => user.RefreshToken == refreshTokenDTO.RefreshToken);

            if (user is null)
                return null;

            if (user.RefreshTokenExpiryTime is null ||
                user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return null;
            }

            var accessTokenResult = await CreateAccessToken(user);

            user.RefreshToken = GenerateRefreshToken();
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            var updateResult = await userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                return null;

            return new AuthResponseDTO
            {
                AccessToken = accessTokenResult.Token,
                AccessTokenExpiration = accessTokenResult.Expiration,
                RefreshToken = user.RefreshToken
            };
        }

        private async Task<(string Token, DateTime Expiration)> CreateAccessToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await userManager.GetRolesAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddMinutes(30);

            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return (tokenString, expiration);
        }

        private static string GenerateRefreshToken()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(64);
            return Convert.ToBase64String(randomBytes);
            
        }
    }
}