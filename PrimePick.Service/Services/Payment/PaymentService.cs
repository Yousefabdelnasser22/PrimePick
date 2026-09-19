using AutoMapper;
using PrimePick.Core.DTOs.Cart;
using PrimePick.Core.Models;
using PrimePick.Core.Models.orders;
using PrimePick.Core.Repository.Contract;
using PrimePick.Core.Services.Contract;
using PrimePick.Service.Common;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Product = PrimePick.Core.Models.Product;
using Cartt = PrimePick.Core.Models.Cart;
using Microsoft.Extensions.Configuration;
using PrimePick.Core.Specifications.Orders;
using Microsoft.EntityFrameworkCore.Storage.Json;
namespace PrimePick.Service.Services.Payment
{
    public class PaymentService(ICartService cartService , IUnitOfWork unitOfWork ,IMapper mapper , IConfiguration configuration) : IPaymentService
    {
        public async Task<Result<Cartt>> CreateOrUpdatePaymentIntentIdAsync(string cartId)
        {
            StripeConfiguration.ApiKey = configuration["Strip:Secretkey"];
            var cart = await cartService.GetCartAsync(cartId);

            if (cart == null || cart.Value == null)
            {
                return null;
            }

            decimal shippingPrice = 0;
            if (cart.Value.DeliveryMethodId.HasValue)
            {
                var del = await unitOfWork.Repository<DeliveryMethod ,int>().GetByIdAsync(cart.Value.DeliveryMethodId.Value);
                shippingPrice = del.Cost;
            }

            
            foreach (var item in cart.Value.Items)
            {

                var product = await unitOfWork.Repository<Product, int>().GetByIdAsync(item.Id);
                if (item.Price != product.Price)
                {
                    item.Price = product.Price;
                }
            }

            var subtotal = cart.Value.Items.Sum(o => o.Price * o.Quantity);
            var service = new PaymentIntentService();

            PaymentIntent paymentIntent;
            if (string.IsNullOrEmpty(cart.Value.PaymentIntentId))
            {

                var option = new PaymentIntentCreateOptions()
                {
                    Amount = (long)(subtotal * 100 + shippingPrice * 100),
                    PaymentMethodTypes = new List<string>() { "card" },
                    Currency = "usd",

                };

                paymentIntent = await service.CreateAsync(option);
                cart.Value.PaymentIntentId = paymentIntent.Id;
                cart.Value.ClientSecret = paymentIntent.ClientSecret;
            }

            else
            {
                var option = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)(subtotal * 100 + shippingPrice * 100),
                  
                };

                paymentIntent = await service.UpdateAsync( cart.Value.PaymentIntentId ,option );
                cart.Value.PaymentIntentId = paymentIntent.Id;
                cart.Value.ClientSecret = paymentIntent.ClientSecret;
            }
            var cartDto = mapper.Map<CartDto>(cart.Value);

            var retCart = await cartService.UpdateCartAsync(cartDto);

            return retCart;


        }

        public async Task<Order> UpdatePaymentIntentForSucceededOrFailed(string paymentIntentId, bool flag)
        {
            var spec = new  OrderSpecificationWithPaymentIntentId(paymentIntentId);  

            var order = await unitOfWork.Repository<Order, int>().GetByIdAsyncWithSpecs(spec);

            if (flag)
            {
                order.Status = OrderStatus.PaymentReceived;
            }

            else 
            {
                order.Status = OrderStatus.PaymentFailed;
            }

            unitOfWork.Repository<Order, int>().Update(order);
            await unitOfWork.CompleteAsync();   

            return order;
        }
    }
}
