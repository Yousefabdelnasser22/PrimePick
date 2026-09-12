using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PrimePick.Core.DTOs.Product;
using PrimePick.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Mapping
{
    public class ProductProfile : Profile
    {
        public ProductProfile(IConfiguration configuration )
        {
            CreateMap<Product, ProductDto>()
                .ForMember(p=>p.BrandName,options=> options.MapFrom(s=>s.Brand.Name))
                .ForMember(p => p.TypeName, options => options.MapFrom(s => s.Type.Name))
                .ForMember(p => p.PictureUrl, options => options.MapFrom(s => $"{configuration["BaseUrl"]}{s.PictureUrl}"));


            CreateMap<ProductBrand, TypeBrandDto>();
            CreateMap<ProductType, TypeBrandDto>();

        }
    }
}
