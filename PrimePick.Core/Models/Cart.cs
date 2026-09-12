using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Models
{
    public class Cart
    {
        public string Id { get; set; }

        public List<CartItem> Items { get; set; } = new();

        public int? DeliveryMethodId { get; set; }

        public string? PaymentIntentId { get; set; }

        public string? ClientSecret { get; set; }
    }
}
