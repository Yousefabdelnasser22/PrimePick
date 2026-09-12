using PrimePick.Core.Models.orders;
using PrimePick.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Services.Contract
{
    public interface IOrderService
    {
        Task<Result<Order>> CreateOrderAsync(
            string cartId,
            int deliveryMethod,
            Address shippingAddress);

        Task<Result<Order>>
            GetOrderByIdForSpecificUserAsync(int orderId);

        Task<Result<IEnumerable<Order>>>
            GetOrdersForSpecificUserAsync();
    }
}
