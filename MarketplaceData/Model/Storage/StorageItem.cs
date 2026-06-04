using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VetClassLibrary.Model.Storage
{
    public class StorageItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ItemId { get; set; }

        [ForeignKey(nameof(ItemId))]
        [Required]
        public Item Item { get; set; } = null!; 

        [Required]
        public double Qty { get; set; }

        public StorageItem(Item item, double qty)
        {
            Item = item;
            ItemId = item.Id;
            Qty = qty;
        }

        public StorageItem() { }
    }
}