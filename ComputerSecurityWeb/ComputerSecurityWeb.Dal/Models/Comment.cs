using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerSecurityWeb.Dal.Models
{
    public class Comment
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public AppUser User { get; set; }

        public Guid CaffId { get; set; }

        public CaffFileModel CaffFile { get; set; }

        public string Content { get; set; }
    }
}
