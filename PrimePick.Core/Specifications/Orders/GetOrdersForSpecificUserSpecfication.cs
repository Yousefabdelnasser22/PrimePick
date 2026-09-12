using PrimePick.Core.Models.orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Specifications.Orders
{
    public class GetOrdersForSpecificUserSpecfication:BaseSpecifications<Order,int>
    {
        public GetOrdersForSpecificUserSpecfication(string buyerEmail):base(o=>o.BuyerEmail == buyerEmail)
        {
            Includes.Add(o => o.Items);
            Includes.Add(o => o.DeliveryMethod);
        }
    }
}
