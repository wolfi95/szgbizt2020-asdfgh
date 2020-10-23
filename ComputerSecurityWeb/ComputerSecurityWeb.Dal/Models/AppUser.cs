using Microsoft.AspNetCore.Identity;
using System;

namespace ComputerSecurityWeb.Dal.Models
{
    public class AppUser : IdentityUser<Guid>
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Role { get; set; }

        public AppUser()
        {
        }
    }
}
