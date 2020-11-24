using ComputerSecurityWeb.Bll.Dtos.Caff;
using ComputerSecurityWeb.Bll.ServiceInterfaces;
using ComputerSecurityWeb.Dal.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> userManager;

        public CaffController(ICaffService caffService,
            UserManager<AppUser> userManager)
        {
            this.caffService = caffService;
            this.userManager = userManager;
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
            var mimeType = "application/caff";

            return new FileContentResult(caffFile.Bytes, mimeType)
            {
                FileDownloadName = $"{caffFile.Filename}.caff"
            };
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

        public async Task<IActionResult> Comment(string message, Guid caffFileId)
        {
            var userId = this.userManager.GetUserId(User);
            await this.caffService.AddComment(new Guid(userId), caffFileId, message);
            return new OkResult();
        }
    }
}
