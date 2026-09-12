using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PrimePick.Core.DTOs.Cart;
using PrimePick.Core.Models;
using PrimePick.Core.Services.Contract;
using System.Threading.Tasks;

namespace PrimePick.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController(ICartService cartService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<Cart>> GetCart(string id)
        {
         var cart =  await cartService.GetCartAsync(id);
         return Ok(cart);
        }

        [HttpPost]
        public async Task<ActionResult<Cart>> CreateCart(CartDto cartDto)
        {
            var cart = await cartService.UpdateCartAsync(cartDto);
            return Ok(cart);
        }

        [HttpDelete]

        public async Task<ActionResult<bool>> DeleteCart(string id)
        {
            var cart = await cartService.DeleteCartAsync(id);
            return Ok(cart);
        }
    }
}
