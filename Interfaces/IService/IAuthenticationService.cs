using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Core.Interfaces.IService
{
    public interface IAuthenticationService
    {
        Task<bool> IsAdminAsync(string userId);
    }
}
