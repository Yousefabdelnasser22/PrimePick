using PrimePick.Core.Models;
using PrimePick.Core.Models.orders;
using PrimePick.Repository.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PrimePick.Repository.Data
{
    public static class AppDBContextSeed
    {
        public async static  Task SeedAsync(AppDBContext context)
        {
            if (context.productBrands.Count() == 0 )
            {
                var BrandData = File.ReadAllText(@"..\PrimePick.Repository\Data\DataSeed\brands.json");

                var Brand = JsonSerializer.Deserialize<List<ProductBrand>>(BrandData);


                if (Brand != null)
                {
                    await context.productBrands.AddRangeAsync(Brand);
                    await context.SaveChangesAsync();
                }
            }

            if (context.productTypes.Count() == 0)
            {
                var TypeData = File.ReadAllText(@"..\PrimePick.Repository\Data\DataSeed\types.json");

                var Type = JsonSerializer.Deserialize<List<ProductType>>(TypeData);


                if (Type != null)
                {
                    await context.productTypes.AddRangeAsync(Type);
                    await context.SaveChangesAsync();
                }
            }

            if (context.products.Count() == 0)
            {
                var Productdata = File.ReadAllText(@"..\PrimePick.Repository\Data\DataSeed\products.json");

                var products = JsonSerializer.Deserialize<List<Product>>(Productdata);


                if (products != null)
                {
                    await context.products.AddRangeAsync(products);
                    await context.SaveChangesAsync();
                }
            }



            if (context.DeliveryMethods.Count() == 0)
            {
                var deliveryData = File.ReadAllText(@"..\PrimePick.Repository\Data\DataSeed\delivery.json");

                var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryData);


                if (deliveryMethods != null)
                {
                    await context.DeliveryMethods.AddRangeAsync(deliveryMethods);
                    await context.SaveChangesAsync();
                }
            }



        }
    }
}
