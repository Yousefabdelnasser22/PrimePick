using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using PrimePick.Core.DTOs.User;
using PrimePick.Core.Models;
using PrimePick.Core.Services.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Service.Services.Identity
{
    public class PasswordService(
     UserManager<ApplicationUser> userManager,
     IEmailSenderService emailSender
    )
     : IPasswordService
    {
        public async Task ForgotPasswordAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);

            if (user is null)
                return;

            var token =
                await userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = WebEncoders.Base64UrlEncode(
                Encoding.UTF8.GetBytes(token));

            var body = $"""
        <h2>Reset Your Password</h2>

        <p>Use the following token to reset your password:</p>

        <div style="
            padding:15px;
            background-color:#f3f4f6;
            word-break:break-all;
            border-radius:6px;">
            {encodedToken}
        </div>

        <p>Your email:</p>

        <strong>{user.Email}</strong>

        <p>If you didn't request this, ignore this email.</p>
        """;

            await emailSender.SendEmailAsync(
                user.Email!,
                "Reset Your Password",
                body);
        }

        public async Task<IdentityResult> ResetPasswordAsync(
            ResetPasswordDto request)
        {
            var user =
                await userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description =
                            "Invalid password reset request."
                    });
            }

            string decodedToken;

            try
            {
                decodedToken = Encoding.UTF8.GetString(
                    WebEncoders.Base64UrlDecode(request.Token));
            }
            catch
            {
                return IdentityResult.Failed(
                    new IdentityError
                    {
                        Description =
                            "Invalid password reset token."
                    });
            }

            return await userManager.ResetPasswordAsync(
                user,
                decodedToken,
                request.NewPassword);
        }
    }
}
