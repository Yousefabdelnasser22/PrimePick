using Microsoft.EntityFrameworkCore;
using PrimePick.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using PrimePick.Core.Models.orders;

namespace PrimePick.Repository.Data.Context
{
    public class AppDBContext: IdentityDbContext<ApplicationUser>
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options) { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<Product> products { get; set; }
        public DbSet<ProductBrand> productBrands { get; set; }
        public DbSet<ProductType> productTypes { get; set; }

        public DbSet<Order> Orders { get; set; }


        public DbSet<OrderItem> OrderItems { get; set; }


        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }





    }
}
