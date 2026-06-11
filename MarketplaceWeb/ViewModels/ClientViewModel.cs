using System.ComponentModel.DataAnnotations;

namespace MarketplaceWeb.ViewModels
{
    public class ClientViewModel
    {
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(50)]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string Address { get; set; }

        // Optional on Edit, Required on Create (can handle logic in controller or with view models)
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
