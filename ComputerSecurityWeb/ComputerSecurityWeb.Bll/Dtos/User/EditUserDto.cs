using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerSecurityWeb.Bll.Dtos.User
{
    public class EditUserDto
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
