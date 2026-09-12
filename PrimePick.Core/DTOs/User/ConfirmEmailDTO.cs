using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.DTOs.User
{
    public class ConfirmEmailDTO
    {
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = null!;
    }
}
