using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Specifications.Products
{
    public class ProductParams
    {
   
    public string? sort { get; set; }

      public int? BrandId { get; set; }

        public int? typeId { get; set; }

        public string? Search { get; set; }

        public int pageIndex { get; set; } = 1;

        public int pageSize { get; set; } = 10;


    }
}
