namespace ComputerSecurityWeb.Bll.DTOs.User
{
    public class RegistrationRequestDTO
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }
    }
}
