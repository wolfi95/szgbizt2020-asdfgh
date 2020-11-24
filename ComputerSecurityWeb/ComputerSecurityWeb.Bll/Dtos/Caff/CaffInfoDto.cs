using ComputerSecurityWeb.Dal.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ComputerSecurityWeb.Bll.Dtos.Caff
{
    public class CaffInfoDto
    {
        public Guid Id { get; set; }

        public string Filename { get; set; }

        public byte[] Bytes { get; set; }

        public CaffInfoDto()
        {

        }
        public CaffInfoDto(CaffFileModel model)
        {
            this.Id = model.Id;
            this.Filename = model.FileName;
            this.Bytes = File.ReadAllBytes($"CaffFiles/{model.FileName}");
        }
    }

}
