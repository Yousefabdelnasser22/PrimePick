using PrimePick.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.DTOs.Product
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? PictureUrl { get; set; }

        public int? TypeId { get; set; }
        public string TypeName { get; set; } = null!;

        public int? BrandId { get; set; }
        public string BrandName { get; set; } = null!;

    }
}
