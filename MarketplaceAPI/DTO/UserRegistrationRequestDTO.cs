using System.ComponentModel.DataAnnotations;

namespace VetClassLibrary.DTO
{
    public class UserRegistrationRequestDTO
    {
        [Required]
        public string FullName { get; set; } = default!;

        [Required]
        public string PhoneNumber { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;

        [Required]
        public string Role { get; set; } = default!;
    }
}
