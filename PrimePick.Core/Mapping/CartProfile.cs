using AutoMapper;
using Microsoft.Extensions.Configuration;
using PrimePick.Core.DTOs.Cart;
using PrimePick.Core.DTOs.Product;
using PrimePick.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Mapping
{
    public class CartProfile:Profile
    {
        public CartProfile(IConfiguration configuration)
        {
           

            CreateMap<Cart, CartDto>().ReverseMap();
           

        }
    }
}
