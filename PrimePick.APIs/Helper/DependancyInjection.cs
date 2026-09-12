using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PrimePick.Core.Mapping;
using PrimePick.Core.Models;
using PrimePick.Core.Repository.Contract;
using PrimePick.Core.Services.Contract;
using PrimePick.Repository.Data.Context;
using PrimePick.Repository.Repositories;
using PrimePick.Service.Services.Cart;
using PrimePick.Service.Services.Identity;
using PrimePick.Service.Services.Orders;
using PrimePick.Service.Services.Products;
using StackExchange.Redis;
using System.Text;

namespace PrimePick.APIs.Helper
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDependencies(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddControllers();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(
                    "Bearer",
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,

                        Description = "Enter JWT token only"
                    });

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
                    });
            });

            services.AddDbContext<AppDBContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddMemoryCache();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<iIdentityService, IdentityService>();
            services.AddScoped<IEmailSenderService, EmailSender>();
            services.AddScoped<IPasswordService,PasswordService>();
            services.AddScoped<RoleService, RoleService>(); 
            services.AddScoped<ICartRepository,CartRepository>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<IOrderService, OrderService>();


            services.AddSingleton<IConnectionMultiplexer>((serviceProvider) =>
            {
                
               var connection = configuration.GetConnectionString("Redis");

                return ConnectionMultiplexer.Connect(connection);
            });


            services.AddAutoMapper(options =>
                options.AddProfile(new ProductProfile(configuration)));

            services.AddAutoMapper(options =>
              options.AddProfile(new CartProfile(configuration)));

            services.AddAutoMapper(options =>
            options.AddProfile(new OrdersProfile(configuration)));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration =
                    configuration.GetConnectionString("Redis");

                options.InstanceName = "PrimePick:";
            });

            services.AddHangfire(configurationOptions =>
                configurationOptions.UseSqlServerStorage(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddHangfireServer();

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode =
                    StatusCodes.Status429TooManyRequests;

                options.AddFixedWindowLimiter("fixed", fixedOptions =>
                {
                    fixedOptions.PermitLimit = 5;
                    fixedOptions.Window = TimeSpan.FromSeconds(10);
                    fixedOptions.QueueLimit = 0;
                });
            });

            services
                .AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.SignIn.RequireConfirmedEmail = true;
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 6;

                })
                .AddEntityFrameworkStores<AppDBContext>()
                .AddDefaultTokenProviders();

            var jwtSettings = configuration.GetSection("JWT");

            var secretKey = jwtSettings["SecretKey"]
                ?? throw new InvalidOperationException("JWT SecretKey is missing");

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters =
                        new TokenValidationParameters
                        {
                         
                            ValidateIssuer = true,
                            ValidIssuer = jwtSettings["Issuer"],

                           
                            ValidateAudience = true,
                            ValidAudience = jwtSettings["Audience"],

                           
                            ValidateLifetime = true,

                          
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey =
                                new SymmetricSecurityKey(
                                    Encoding.UTF8.GetBytes(secretKey)),

                        
                            ClockSkew = TimeSpan.Zero
                        };
                });

            services.AddAuthorization();
            

            services.AddHttpContextAccessor();
            services.AddScoped<IGetCurrentUserService, GetCurrentUserService>();
            return services;
        }
    }

}
