using ComputerSecurityWeb.Bll.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComputerSecurityWeb.Api.Controllers
{
    [Route("caff")]
    [ApiController]
    public class CaffController : ControllerBase
    {
        private readonly ICaffService caffService;

        public CaffController(ICaffService caffService)
        {
            this.caffService = caffService;
        }

        [HttpPost]
        [Route("test")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> Test(int i)
        {
            return new JsonResult(await this.caffService.TestDll(i));
        }
    }
}
