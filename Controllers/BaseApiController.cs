using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace capstoneDotNet.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        private IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public BaseApiController(IConfiguration config, IWebHostEnvironment env)
        {
            _configuration = config;
            _env = env;
        }
    }
}

