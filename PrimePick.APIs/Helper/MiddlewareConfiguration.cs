using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrimePick.APIs.Middleware;
using PrimePick.Core.Models;
using PrimePick.Repository;
using PrimePick.Repository.Data;
using PrimePick.Repository.Data.Context;
using System.Threading.Tasks;

namespace PrimePick.APIs.Helper
{
    public static class MiddlewareConfiguration
    {
        public static async Task<WebApplication> ConfigureMiddleware(this WebApplication app) 
        {
            
            using var scope = app.Services.CreateScope();
            var service = scope.ServiceProvider;
            var context = service.GetRequiredService<AppDBContext>();
            var LoggerFactory = service.GetRequiredService<ILoggerFactory>();
            var roleManger = service.GetRequiredService<RoleManager<IdentityRole>>();
            var userManger = service.GetRequiredService<UserManager<ApplicationUser>>();

            try
            {
                await context.Database.MigrateAsync();
                await AppDBContextSeed.SeedAsync(context);
                await RoleSeeder.RoleSeed(roleManger);
                await AdminSeeder.AdminSeed(userManger ,roleManger);
            }
            catch (Exception ex)
            {

                var logger = LoggerFactory.CreateLogger<Program>();
                logger.LogError(ex, "there are problems during apply migrations");
            }


            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
      

            app.UseAuthorization();

            app.UseHangfireDashboard();

            app.UseStaticFiles();

            app.UseRateLimiter();

            app.MapControllers();

         
            return app;
        }
        
    }
}
