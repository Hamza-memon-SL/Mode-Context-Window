using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KE.Service.Controllers
{
    [Produces("application/json")]
    [Route("api/Test")]
    public class TestController : Controller
    {
        /// <summary>
        /// Test service for response checking
        /// </summary>
        /// <returns></returns>
        // GET: api/Test
        [HttpGet]
        public string Get()
        {
            return "Service is running!";
        }
    }
}