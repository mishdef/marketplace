using MarketplaceData.Model;
using MarketplaceData.Model.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VetClassLibrary.Model
{
    public class Item
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        [MinLength(3)]
        public string Name { get; set; } = null!;

        [MaxLength(200)]
        public string Description { get; set; } = null!;

        public List<string> ImageUrls { get; set; } = new List<string>();



        private decimal _price;
        private decimal _discountPrice;

        public decimal Price
        {
            get { return _price; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Price cannot be negative");
                }
                if (value == 0)
                {
                    throw new ArgumentException("Price cannot be zero");
                }
                _price = value;
            }
        }
        public decimal DiscountPrice
        {
            get { return _discountPrice; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Discount price cannot be negative");
                }
                _discountPrice = value;
            }
        }




        public bool IsDeleted { get; set; } = false;




        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;


        public int CompanyId { get; set; }
        public Company Company { get; set; } = null!;
    }
}
