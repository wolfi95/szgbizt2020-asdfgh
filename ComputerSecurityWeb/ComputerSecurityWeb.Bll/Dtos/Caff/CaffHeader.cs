﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerSecurityWeb.Bll.Dtos.Caff
{
    public class CaffHeader
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public FileContentResult ImageData { get; set; }

        public List<CommentDto> Comments { get; set; }
    }
}
