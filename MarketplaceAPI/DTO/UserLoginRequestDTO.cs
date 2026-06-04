using System.ComponentModel.DataAnnotations;

namespace VetClassLibrary.DTO
{
    public class UserLoginRequestDTO
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
