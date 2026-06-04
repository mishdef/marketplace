using MarketplaceData.Model.Cart;
using VetClassLibrary.Model;

namespace VetClassLibrary.Interfaces
{
    public interface IOrderService
    {
        Task ProcessCheckoutAsync(Order order, int userId);
        Task ProcessItemsAsync(IEnumerable<CartItem> items);
        Task<List<Order>> GetOrderHistoryAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}