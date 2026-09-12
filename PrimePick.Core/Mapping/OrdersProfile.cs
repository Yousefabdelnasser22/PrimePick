using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PrimePick.Core.DTOs.Orders;
using PrimePick.Core.Models.orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Mapping
{
    public class OrdersProfile : Profile
    {
        public OrdersProfile(IConfiguration configuration)
        {
            CreateMap<Order, OrderToReturnDto>()
      .ForMember(d => d.DeliveryMethod,
          options => options.MapFrom(s => s.DeliveryMethod.ShortName))
      .ForMember(d => d.DeliveryMethodCost,
          options => options.MapFrom(s => s.DeliveryMethod.Cost));

            CreateMap<Address, AddressDto>().ReverseMap();

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId,
                    options => options.MapFrom(s => s.Product.ProductId))
                .ForMember(d => d.ProductName,
                    options => options.MapFrom(s => s.Product.ProductName))
                .ForMember(d => d.PictureUrl,
                    options => options.MapFrom(s =>
                        $"{configuration["BaseUrl"]}{s.Product.PictureUrl}"));
        }
    }
}
