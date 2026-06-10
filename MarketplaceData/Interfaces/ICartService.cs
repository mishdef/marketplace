using System.Threading.Tasks;
using MarketplaceData.Model.Cart;

namespace MarketplaceData.Interfaces
{
    public interface ICartService
    {
        Task AddToCartAsync(int userId, int productId, double qty);
        Task UpdateQuantityAsync(int userId, int cartItemId, double newQty);
        Task RemoveFromCartAsync(int userId, int cartItemId);
        Task<Cart> GetUserCartAsync(int userId);
    }
}
