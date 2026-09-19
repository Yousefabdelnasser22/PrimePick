using PrimePick.Core.DTOs.Cart;
using PrimePick.Core.Models;
using PrimePick.Core.Models.orders;
using PrimePick.Service.Common;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Services.Contract
{
    public interface IPaymentService
    {
        Task<Result<Cart>> CreateOrUpdatePaymentIntentIdAsync(string cartId);
        Task<Order> UpdatePaymentIntentForSucceededOrFailed(string paymentIntentId, bool flag);
    }
}
