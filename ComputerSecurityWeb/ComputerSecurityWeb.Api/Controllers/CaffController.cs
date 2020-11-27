using ComputerSecurityWeb.Api.Common;
using ComputerSecurityWeb.Bll.Dtos.Caff;
using ComputerSecurityWeb.Bll.ServiceInterfaces;
using ComputerSecurityWeb.Dal.Models;
using Microsoft.AspNetCore.Authorization;
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
    public class CaffController : ComputerSecurityControllerBase
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
        [Route("getImageById")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetImageForHeader(Guid caffId)
        {
            byte[] imageArray = await this.caffService.GetImageForCaff(caffId);
            return File(imageArray, "image/bmp");
        }

        [HttpPost]
        [Route("upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> UploadCaffFile([FromQuery]string fileName, [FromForm] IFormFile data)
        {
            if (data.Length > 0)
            {
                char[] InvalidFilenameChars = Path.GetInvalidFileNameChars();

                if (fileName.IndexOfAny(InvalidFilenameChars) >= 0)
                {
                    throw new Exception("invalid file name!");
                }

                var filePath = Path.Combine("CaffFiles",
                    $"{fileName}.caff");

                using (FileStream stream = System.IO.File.Create(filePath))
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

        public async Task<List<CaffHeader>> GetAllCaffFiles()
        {
            return await caffService.GetAllCaffFiles();
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

        [HttpPost]
        [Authorize(Roles ="Admin")]
        [Route("editcaff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> Comment(Guid caffId, string newName)
        {
            await this.caffService.EditCaffFile(caffId, newName);
            return new OkResult();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("deletecaff")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> DeleteCaffFile(Guid caffId)
        {
            await this.caffService.DeleteCaffFile(caffId);
            return new OkResult();
        }
    }
}
