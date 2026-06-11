using System.ComponentModel.DataAnnotations;

namespace MarketplaceWeb.Models
{
    public class EditCompanyViewModel
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = null!;
        
        [Required]
        public string Description { get; set; } = null!;
        
        [Required]
        public string Address { get; set; } = null!;
        
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = null!;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        
        [Display(Name = "Logo URL (or upload below)")]
        public string? LogoUrl { get; set; }

        [Display(Name = "Upload Logo")]
        public Microsoft.AspNetCore.Http.IFormFile? LogoFile { get; set; }

        public List<int> SelectedShipmentCompanyIds { get; set; } = new List<int>();
        public List<MarketplaceData.Model.ShipmentCompany>? AvailableShipmentCompanies { get; set; }
    }

    public class AddEmployeeViewModel
    {
        public int CompanyId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        [Display(Name = "Full Name")]
        public string FullName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } = null!;

        [Required]
        [StringLength(32, MinimumLength = 4)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
