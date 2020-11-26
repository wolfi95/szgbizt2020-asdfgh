using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerSecurityWeb.Dal.Models
{
    public class CaffFileModel
    {
        public Guid Id { get; set; }

        public string FileName { get; set; }

        public List<Comment> Comments { get; set; }
    }
}
