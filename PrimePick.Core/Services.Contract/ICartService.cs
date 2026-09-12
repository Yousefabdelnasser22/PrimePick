using PrimePick.Core.DTOs.Cart;
using PrimePick.Core.Models;
using PrimePick.Service.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Services.Contract
{
    public interface ICartService
    {
        Task<Result<Cart>> GetCartAsync(string id);

        Task<Result<Cart>> UpdateCartAsync(CartDto cart);

        Task<Result<bool>> DeleteCartAsync(string cartId);
    }
}
