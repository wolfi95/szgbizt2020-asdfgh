using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ComputerSecurityWeb.Dal.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Role { get; set; }

        public List<Comment> Comments { get; set; }

        public AppUser()
        {
        }
    }
}
