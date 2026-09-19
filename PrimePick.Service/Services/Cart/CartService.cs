using AutoMapper;
using PrimePick.Core.DTOs.Cart;
using PrimePick.Core.Models;
using PrimePick.Core.Services.Contract;
using PrimePick.Repository.Repositories;
using PrimePick.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Service.Services.Cart
{
    public class CartService(ICartRepository cartRepository ,IMapper mapper) : ICartService
    {
        public async Task<Result<bool>> DeleteCartAsync(string cartId)
        {
            var deleted = await cartRepository.DeleteCartAsync(cartId);

            if (!deleted)
            {
                return Result<bool>.Failure("Cart was not found or could not be deleted");
            }

            return Result<bool>.Success(true);
        }

        public async Task<Result<Core.Models.Cart>> GetCartAsync(string id)
        {
            if (id is null)
            {
               return Result<Core.Models.Cart>.Failure("Invalid Id");
            }
            var cart = await cartRepository.GetCartAsync(id);

            if (cart is null)
            {
                Core.Models.Cart cart1 = new Core.Models.Cart();
            }

            return Result<Core.Models.Cart>.Success(cart);
        }

        public async Task<Result<Core.Models.Cart>> UpdateCartAsync(CartDto cart)
        {
          var createdCart =  await cartRepository.UpdateCartAsync(mapper.Map<Core.Models.Cart>(cart));

            return Result<Core.Models.Cart>.Success(createdCart);
        }
    }
}
