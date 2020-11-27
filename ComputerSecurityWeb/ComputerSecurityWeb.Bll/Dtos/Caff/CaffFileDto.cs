using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerSecurityWeb.Bll.Dtos.Caff
{
    public class CaffFileDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string ImageData { get; set; }

        public List<CommentDto> Comments { get; set; }
    }
}
