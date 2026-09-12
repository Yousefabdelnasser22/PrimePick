using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimePick.Core.DTOs.Orders;
using PrimePick.Core.Models.orders;
using PrimePick.Core.Services.Contract;
using PrimePick.Service.Common;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PrimePick.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController(IOrderService orderService , IMapper mapper) : ControllerBase
    {
        [HttpPost]
        [Authorize]

        public async Task<IActionResult> CreateOrder(OrderDto model)
        {
            var address = mapper.Map<Address>(model.ShipToAddress);

            var result = await orderService.CreateOrderAsync(
                model.CartId,
                model.DeliveryMethodId,
                address
            );

            if (result.IsFailure)
                return BadRequest(result.Error);

            var orderDto =
                mapper.Map<OrderToReturnDto>(result.Value);

            return Ok(orderDto);
        }

        [HttpGet("GetOrderByIdForSpecificUser")]
        [Authorize]

        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var result =
                await orderService.GetOrderByIdForSpecificUserAsync(orderId);

            if (result.IsFailure)
                return NotFound(result.Error);

            var orderDto =
                mapper.Map<OrderToReturnDto>(result.Value);

            return Ok(orderDto);
        }


        [HttpGet("GetOrdersForSpecificUser")]
        [Authorize]

        public async Task<IActionResult> GetOrdersForSpecificUser()
        {
            var result = await orderService.GetOrdersForSpecificUserAsync();

            if (result.IsFailure)
                return BadRequest(result);

            var ordersDto =
                mapper.Map<ICollection<OrderToReturnDto>>(result.Value);

            return Ok(Result<ICollection<OrderToReturnDto>>.Success(ordersDto));
        }



    }
}
