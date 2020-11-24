using ComputerSecurityWeb.Bll.Dtos.Caff;
using ComputerSecurityWeb.Bll.ServiceInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
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
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> Test(int i)
        {
            return new JsonResult(await this.caffService.TestDll(i));
        }

        [HttpPost]
        [Route("upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> UploadCaffFile(string fileName, IFormFile data)
        {
            if (data.Length > 0)
            {
                if(fileName.Contains('.') || fileName.Contains("\\") || fileName.Contains("/") )
                {
                    throw new Exception("invalid file name!");
                }

                var filePath = Path.Combine("CaffFiles",
                    $"{fileName}.caff");

                using (var stream = System.IO.File.Create(filePath))
                {
                   await data.CopyToAsync(stream);
                }

                await this.caffService.AddCaffFile(fileName);
            }

            return new OkResult();

        }

        [HttpGet]
        [Route("download")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> DownloadCaffFileById(Guid caffFileId)
        {
            var caffFile = await this.caffService.GetCaffById(caffFileId);
            ////MOCK:
            //var caffFile = new CaffInfoDto
            //{
            //    Id = Guid.NewGuid(),
            //    Filename = "1.caff",
            //    Bytes = System.IO.File.ReadAllBytes($"CaffFiles/1.caff")
            //};
            var mimeType = "application/caff";

            return new FileContentResult(caffFile.Bytes, mimeType)
            {
                FileDownloadName = $"{caffFile.Filename}.caff"
            };
        }

        [HttpGet]
        [Route("getcafffile")]
        [ProducesResponseType(typeof(CaffHeader), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetCaffFileById(Guid Id)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("cafffiles")]
        [ProducesResponseType(typeof(List<CaffHeader>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetAllCaffFiles()
        {
            return new JsonResult(await this.caffService.GetAllCaffFiles());
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
