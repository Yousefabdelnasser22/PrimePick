using PrimePick.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Specifications.Products
{
    public class ProductSpecifications:BaseSpecifications<Product,int>
    {
        public ProductSpecifications(int id) :base(p=>p.Id == id)
        {
            AddIncludes();
            Sorting = p => p.Name;
        }


        public ProductSpecifications(ProductParams productParams) 
            :base( p=> ((string.IsNullOrEmpty(productParams.Search)|| p.Name.Contains(productParams.Search.ToLower()))&& !productParams.BrandId.HasValue||p.BrandId == productParams.BrandId) && (!productParams.typeId.HasValue||p.TypeId == productParams.typeId))
        {
            AddIncludes();
            if (!string.IsNullOrEmpty(productParams.sort))
            {
                switch (productParams.sort)
                {
                    case "name":
                        Sorting = p => p.Name
                        ; break;

                    case "price":
                        Sorting = p => p.Price;

                        break;

                    default:
                        Sorting = p => p.Name;
                        break;
                }
            }

          ApplyPagination((productParams.pageIndex -1)* productParams.pageSize, productParams.pageSize);
          
        }

        public void AddIncludes()
        {
            Includes.Add(p => p.Brand);
            Includes.Add(p => p.Type);
        }

       
    }
}
