
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using PrimePick.Core.Mapping;
using PrimePick.Core.Repository.Contract;
using PrimePick.Core.Services.Contract;
using PrimePick.Repository.Data;
using PrimePick.Repository.Data.Context;
using PrimePick.Repository.Repositories;
using PrimePick.Service.Services.Products;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.RateLimiting;
using PrimePick.Core.Models;
using Microsoft.AspNetCore.Identity;
using PrimePick.Service.Services.Identity;
using PrimePick.APIs.Helper;
namespace PrimePick.APIs
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDependencies(builder.Configuration);

            var app = builder.Build();

            await app.ConfigureMiddleware();

            app.Run();
        }
    }
}
