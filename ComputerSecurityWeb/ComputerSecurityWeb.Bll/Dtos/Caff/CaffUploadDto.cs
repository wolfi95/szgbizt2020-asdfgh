using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerSecurityWeb.Bll.Dtos.Caff
{
    public class CaffUploadDto
    {
        public string Name { get; set; }

        public IFormFile Data { get; set; } 
    }
}
