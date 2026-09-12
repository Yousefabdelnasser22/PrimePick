using PrimePick.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrimePick.Core.Services.Contract
{
    public interface IGetCurrentUserService
    {
        Task<ApplicationUser> GetUser();
       
    }
}
