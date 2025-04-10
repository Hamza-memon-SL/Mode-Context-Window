using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using KE.Service.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KE.Service.Controllers
{
    [Produces("application/json")]
    [Route("api/Token")]
    public class TokenController : Controller
    {
        //// POST: api/Token
        [HttpPost]
        public IActionResult GetSecurityTokenByUsernameNPassword([FromBody]Credential credential)
        {
            if (credential.userName.ToLowerInvariant() == "admin" && credential.password.ToLowerInvariant() == "admin")
            {
                return Ok(Guid.Parse("93AAC40E-172C-45D6-89AA-A3C15920AECF"));
            }
            else
            {
                return BadRequest("Invalid Credential");
            }
        }
        
    }
}
