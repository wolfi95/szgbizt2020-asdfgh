using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerSecurityWeb.Bll.Dtos.Caff
{
    public class CommentDto
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; }

        public string Content { get; set; }

        public Guid CaffFileId { get; set; }
    }
}
