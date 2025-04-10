using GenAiPoc.Core.Interfaces.IService;
using GenAiPoc.Core.Interfaces.IService.IVisionetClientService;
using GenAiPoc.Core.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAiPoc.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IVisionetClientService _visionetClientService;

        public AuthenticationService(IVisionetClientService visionetClientService)
        {
            _visionetClientService = visionetClientService;
        }
        public async Task<bool> IsAdminAsync(string userId)
        {
            try
            {

                var userGroupResponse = await _visionetClientService.GetUserGroupsAsync(new UserGroupRequest
                {
                    email = userId
                });

                // Check if response and permissions exist
                if (userGroupResponse?.response?.permissions != null)
                {
                    return userGroupResponse.response.permissions.Contains("GNAI Admin");
                }
            }
            catch (Exception ex)
            {
                return false;
            }
          
            return false;
        }
    }
}
