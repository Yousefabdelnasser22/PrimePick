using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Models
{
    public class Product:BaseEntity<int>
    {
 
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? PictureUrl { get; set; }

        public int? TypeId { get; set; }
        public ProductType Type { get; set; } = null!;

        public int? BrandId { get; set; }
        public ProductBrand Brand { get; set; } = null!;
    }
}
