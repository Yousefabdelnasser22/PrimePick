using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualBasic;
using PrimePick.Core.DTOs.Product;
using PrimePick.Core.Models;
using PrimePick.Core.Repository.Contract;
using PrimePick.Core.Services.Contract;
using PrimePick.Core.Specifications.Products;
using PrimePick.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace PrimePick.Service.Services.Products
{
    public class ProductService : IProductService
    {
        string cacheKey = "Products";

     
        public ProductService(IUnitOfWork unitOfWork,IMapper mapper , IMemoryCache cache , IDistributedCache distributedCache , IBackgroundJobClient backgroundJobClient)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
            _MemoryCache = cache;
            _DistributedCache = distributedCache;
            _BackgroundJobClient = backgroundJobClient;

        }

        public IUnitOfWork UnitOfWork { get; }
        public IMapper Mapper { get; }

        public IMemoryCache _MemoryCache { get; }

        public IDistributedCache _DistributedCache { get; }
        public IBackgroundJobClient _BackgroundJobClient { get; }

        public async Task<IEnumerable<TypeBrandDto>> GetAllBrandAsync()
        {

            var options =
            new DistributedCacheEntryOptions()
           .SetAbsoluteExpiration(
           TimeSpan.FromMinutes(30))
          .SetSlidingExpiration(
            TimeSpan.FromMinutes(5));

            var cacheData = _DistributedCache.GetString(cacheKey);

            if (cacheData != null)
            {
                Console.WriteLine("FROM CACHE");
                return JsonSerializer.Deserialize<IEnumerable<TypeBrandDto>>(cacheData)!;
            }
            Console.WriteLine("FROM DATABASE");
            var resultt =  Mapper.Map<IEnumerable<TypeBrandDto>>(await UnitOfWork.Repository<ProductBrand, int>().GetAllAsync());
           
            var json =JsonSerializer.Serialize(resultt);

            await _DistributedCache.SetStringAsync(cacheKey, json , options);

         var jobId=  _BackgroundJobClient.Enqueue<PrintService>(service => service.Print());


            BackgroundJob.ContinueJobWith<EmailService>(jobId, service => service.SendEmail());
            return resultt;

            
        }

        public async Task<Result<IEnumerable<ProductDto>>> GetAllProductAsync(ProductParams productParams)
        {
                var cacheKey =
               $"Products_" +
               $"{productParams.pageIndex}_" +
               $"{productParams.pageSize}_" +
               $"{productParams.Search?.Trim().ToLowerInvariant()}_" +
               $"{productParams.typeId}_" +
               $"{productParams.BrandId}_" +
               $"{productParams.sort}";

            if (_MemoryCache.TryGetValue(
                cacheKey,
                out IEnumerable<ProductDto>? products))
            {
                Console.WriteLine("FROM CACHE");
                return Result < IEnumerable < ProductDto >>.Success(products)!;
            }

            Console.WriteLine("FROM DATABASE");

            var specs = new ProductSpecifications(productParams);
            var result = await UnitOfWork
                .Repository<Product, int>()
                .GetAllAsyncWithSpecs(specs);


            if (result is null)
            {
                return Result<IEnumerable<ProductDto>>.Failure("Product is empty (Not Found)");
            }

            var productDtos = Mapper.Map<IEnumerable<ProductDto>>(result);

            _MemoryCache.Set(
                cacheKey,
                productDtos,
                TimeSpan.FromMinutes(10));

            return Result<IEnumerable<ProductDto>>.Success(productDtos);
        }

        public async Task<IEnumerable<TypeBrandDto>> GetAllTypeAsync()
        {
            return Mapper.Map<IEnumerable<TypeBrandDto>>(await UnitOfWork.Repository<ProductType,int>().GetAllAsync());
        }

        //public async Task<Result<ProductDto>> GetProductByIdAsync(int id)

        //{
        //    var product = await UnitOfWork.Repository<Product, int>().GetByIdAsync(id);

        //    if (product == null)
        //    {
        //        return Result<ProductDto>.Failure("product is null");
        //    }
        //    ProductDto productDto = Mapper.Map<ProductDto>(product);

        //    return Result<ProductDto>.Success(productDto);

        //}

        public async Task AddProduct(Product product)
        {
            await UnitOfWork.Repository<Product,int>().AddAsync(product);
            await UnitOfWork.CompleteAsync();
            RecurringJob.AddOrUpdate<PrintService>("dailyemail", service => service.Print(), Cron.Daily);
            _MemoryCache.Remove(cacheKey);
        }

    }
}
