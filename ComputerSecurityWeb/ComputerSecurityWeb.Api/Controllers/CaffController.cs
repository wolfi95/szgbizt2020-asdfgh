using ComputerSecurityWeb.Bll.Dtos.Caff;
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

        [HttpPost]
        [Route("upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> UploadCaffFile(CaffUploadDto d)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> DownloadCaffFileById(Guid caffFileId)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("getcafffile")]
        [ProducesResponseType(typeof(List<string>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetCaffFileById()
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("cafffiles")]
        [ProducesResponseType(typeof(List<CaffHeader>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetAllCaffFiles()
        {
            return new JsonResult( await this.caffService.GetAllCaffFiles());
        }

        [HttpPost]
        [Route("comment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> UploadCaffFile(string message, Guid caffFileId)
        {
            throw new NotImplementedException();
        }
    }
}
