using Org.BouncyCastle.Bcpg;
using PrimePick.Core.Models;
using PrimePick.Core.Models.orders;
using PrimePick.Core.Repository.Contract;
using PrimePick.Core.Services.Contract;
using PrimePick.Core.Specifications.Orders;
using PrimePick.Repository.Repositories;
using PrimePick.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Service.Services.Orders
{
    public class OrderService(ICartService cartService , IUnitOfWork unitOfWork , IGetCurrentUserService getCurrentUserService , IPaymentService paymentService) : IOrderService
    {
        public async Task<Result<Order>> CreateOrderAsync(string cartId,int deliveryMethod,Address shippingAddress)
        {
            var user = await getCurrentUserService.GetUser();

            if (user is null)
                return Result<Order>.Failure("Current user was not found");

            var buyerEmail = user.Email;

            if (string.IsNullOrEmpty(buyerEmail))
                return Result<Order>.Failure("User email was not found");

            var dMethod = await unitOfWork
                .Repository<DeliveryMethod, int>()
                .GetByIdAsync(deliveryMethod);

            if (dMethod is null)
                return Result<Order>.Failure("Delivery method was not found");

            var cartResult = await cartService.GetCartAsync(cartId);

            if (cartResult is null ||
                cartResult.IsFailure ||
                cartResult.Value is null)
            {
                return Result<Order>.Failure("Cart was not found");
            }

            var cart = cartResult.Value;

            if (cart.Items is null || !cart.Items.Any())
                return Result<Order>.Failure("Cart is empty");

            var orderItems = new List<OrderItem>();

            foreach (var item in cart.Items)
            {
                var product = await unitOfWork
                    .Repository<Product, int>()
                    .GetByIdAsync(item.Id);

                if (product is null)
                    return Result<Order>.Failure(
                        $"Product with id {item.Id} was not found"
                    );

                var productOrderItem = new ProductItemOrder()
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    PictureUrl = product.PictureUrl
                };

                var orderItem = new OrderItem(
                    productOrderItem,
                    product.Price,
                    item.Quantity
                );

                orderItems.Add(orderItem);
            }

            var subTotal = orderItems.Sum(
                item => item.Price * item.Quantity
            );


            // todo

            if (!string.IsNullOrEmpty(cart.PaymentIntentId))
            {
                var spec = new OrderSpecificationWithPaymentIntentId(cart.PaymentIntentId);
                var ExOrder = await unitOfWork.Repository<Order, int>().GetByIdAsyncWithSpecs(spec);
                unitOfWork.Repository<Order, int>().Delete(ExOrder);
            }
            var cartt = await paymentService.CreateOrUpdatePaymentIntentIdAsync(cartId);


          
            var order = new Order()
            {
                BuyerEmail = buyerEmail,
                DeliveryMethod = dMethod,
                ShippingAddress = shippingAddress,
                Items = orderItems,
                SubTotal = subTotal,
                PaymentIntentId = cartt.Value.PaymentIntentId,
                
            };

            await unitOfWork
                .Repository<Order, int>()
                .AddAsync(order);

            await unitOfWork.CompleteAsync();

            return Result<Order>.Success(order);
        }

        public async Task<Result<Order>> GetOrderByIdForSpecificUserAsync(int orderId)
        {
            var user = await getCurrentUserService.GetUser();

            if (user is null)
                return Result<Order>.Failure("Current user was not found");

            var buyerEmail = user.Email;

            if (string.IsNullOrEmpty(buyerEmail))
                return Result<Order>.Failure("User email was not found");

            var specs =
                new GetOrderByIdForSpecificUserSpecification(
                    buyerEmail,
                    orderId
                );

            var order = await unitOfWork
                .Repository<Order, int>()
                .GetByIdAsyncWithSpecs(specs);

            if (order is null)
                return Result<Order>.Failure("Order was not found");

            return Result<Order>.Success(order);
        }
        public async Task<Result<IEnumerable<Order>>> GetOrdersForSpecificUserAsync()
        {
            var user = await getCurrentUserService.GetUser();

            if (user is null)
                return Result<IEnumerable<Order>>
                    .Failure("Current user was not found");

            var buyerEmail = user.Email;

            if (string.IsNullOrEmpty(buyerEmail))
                return Result<IEnumerable<Order>>
                    .Failure("User email was not found");

            var specs =
                new GetOrdersForSpecificUserSpecfication(
                    buyerEmail
                );

            var orders = await unitOfWork
                .Repository<Order, int>()
                .GetAllAsyncWithSpecs(specs);

            return Result<IEnumerable<Order>>.Success(orders);
        }
    }
}
