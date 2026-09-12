using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimePick.Core.DTOs.User;
using PrimePick.Core.Services.Contract;
using PrimePick.Service.Services.Identity;
using System.Threading.Tasks;

namespace PrimePick.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController(iIdentityService identity, IPasswordService passwordService, IGetCurrentUserService currentUserService ,RoleService roleService) : ControllerBase
    {
        [HttpPost("Register")]
        public async Task<IActionResult> Register(ApplicationUserDTO user)
        {
            var result = await identity.Register(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Registration successful. Please check your email." });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var result =
                await identity.Login(loginDTO);

            if (result is null)
            {
                return Unauthorized(new
                {
                    message = "Invalid username or password"
                });
            }

            return Ok(result);
        }


        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken( RefreshTokenDTO refreshTokenDTO)
        {
            var result =
                await identity.RefreshToken(
                    refreshTokenDTO);

            if (result is null)
            {
                return Unauthorized(new
                {
                    message = "Invalid or expired refresh token"
                });
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet("GetCurrentUser")]

        public async Task<IActionResult> GetCurrentUser()
        {
          var user =   await currentUserService.GetUser();

            return Ok(user);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailDTO dto)
        {
            var result = await identity.ConfirmEmail(dto);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Email confirmed successfully.");
        }

        [AllowAnonymous]
        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailDTO dto)
        {
            await identity.ResendConfirmationEmail(dto.Email);

            return Ok(new
            {
                message = "If the account exists and needs confirmation, an email has been sent."
            });
        }

        [AllowAnonymous]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(
            ForgotPasswordDto request)
        {
            await passwordService.ForgotPasswordAsync(request.Email);

            return Ok(new
            {
                message =
                    "If this email exists, a reset link has been sent."
            });
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto request)
        {
            var result =
                await passwordService.ResetPasswordAsync(request);

            if (!result.Succeeded)
            {
                return BadRequest(new
                {
                    message = "Password reset failed.",
                    errors = result.Errors
                        .Select(error => error.Description)
                });
            }

            return Ok(new
            {
                message = "Password reset successfully."
            });
        }


        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole(string userId , string roleName)
        {

         var result = await roleService.AssignRole(userId, roleName);
            if (!result)
            {
                return BadRequest();
            }

            return Ok("Role Assigned");

        }



        [HttpPost("UnAssignRole")]
        public async Task<IActionResult> UnAssignRole(string userId, string roleName)
        {

            var result = await roleService.UnAssignRole(userId, roleName);
            if (!result)
            {
                return BadRequest("Cannot remove role");
            }

            return Ok("Role UnAssigned");

        }

    }
}
