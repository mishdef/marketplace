using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MarketplaceWeb.Models
{
    public class ShopItemViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Description { get; set; } = null!;

        [Required]
        [Range(0.01, 1000000, ErrorMessage = "Price must be greater than zero.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Range(0, 1000000, ErrorMessage = "Discount Price cannot be negative.")]
        [Display(Name = "Discount Price")]
        [DataType(DataType.Currency)]
        public decimal DiscountPrice { get; set; }

        [Display(Name = "Category")]
        [Required]
        public int CategoryId { get; set; }

        public IEnumerable<SelectListItem>? Categories { get; set; }

        [Display(Name = "Image URL (or upload below)")]
        public string? ImageUrl { get; set; }

        [Display(Name = "Upload Image")]
        public Microsoft.AspNetCore.Http.IFormFile? ImageFile { get; set; }
    }
}
