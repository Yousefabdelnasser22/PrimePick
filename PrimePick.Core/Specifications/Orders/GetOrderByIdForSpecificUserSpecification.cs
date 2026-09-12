using PrimePick.Core.Models.orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Specifications.Orders
{
    public class GetOrderByIdForSpecificUserSpecification:BaseSpecifications<Order,int>
    {
        public GetOrderByIdForSpecificUserSpecification(string buyerEmail, int orderId):base(o=>o.BuyerEmail == buyerEmail && o.Id == orderId)
        {
            Includes.Add(o => o.Items);
            Includes.Add(o => o.DeliveryMethod);
        }
    }
}
