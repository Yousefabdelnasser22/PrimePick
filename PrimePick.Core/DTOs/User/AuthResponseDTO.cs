using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.DTOs.User
{
    public class AuthResponseDTO
    {
        public string AccessToken { get; set; } = string.Empty;

        public DateTime AccessTokenExpiration { get; set; }

        public string RefreshToken { get; set; } = string.Empty;
    }
}
