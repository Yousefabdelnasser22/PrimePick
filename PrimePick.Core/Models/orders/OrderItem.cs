using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Models.orders
{
    public class OrderItem : BaseEntity<int>
    {
        public OrderItem(ProductItemOrder product, decimal price, int quantity)
        {
            Product = product;
            Price = price;
            Quantity = quantity;
        }

        private OrderItem()
        {

        }

        public ProductItemOrder Product { get; set; }

        public decimal Price { get; set; }

        public int Quantity { get; set; }
    }
}
