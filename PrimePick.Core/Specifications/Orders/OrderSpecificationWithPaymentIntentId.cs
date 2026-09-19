using PrimePick.Core.Models.orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Specifications.Orders
{
    public class OrderSpecificationWithPaymentIntentId : BaseSpecifications<Order, int>
    {
        public OrderSpecificationWithPaymentIntentId(string paymentIntentId) : base(o => o.PaymentIntentId == paymentIntentId)
        {
            Includes.Add(o => o.Items);
            Includes.Add(o => o.DeliveryMethod);
        }
    }
}
