using Microsoft.EntityFrameworkCore.Query.Internal;
using PrimePick.Core.DTOs.Product;
using PrimePick.Core.Models;
using PrimePick.Core.Specifications.Products;
using PrimePick.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Services.Contract
{
    public interface IProductService
    {
        Task<Result<IEnumerable<ProductDto>>> GetAllProductAsync(ProductParams productParams);
        Task<IEnumerable<TypeBrandDto>> GetAllTypeAsync();

        Task<IEnumerable<TypeBrandDto>> GetAllBrandAsync();

         //Task<Result> GetProductByIdAsync(int id );

        Task AddProduct( Product product );

    }
}

