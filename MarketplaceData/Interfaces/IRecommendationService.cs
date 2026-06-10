using System.Collections.Generic;
using System.Threading.Tasks;
using VetClassLibrary.Model;

namespace MarketplaceData.Interfaces
{
    public interface IRecommendationService
    {
        Task RecordItemViewAsync(int userId, int itemId);
        Task<List<Item>> GetRecentlyViewedItemsAsync(int userId, int take = 6);
        Task<List<Item>> GetPersonalizedRecommendationsAsync(int userId, int take = 6);
        Task<List<Item>> GetCompanyRecommendationsAsync(int companyId, int currentItemId, int take = 6);
        Task<List<Item>> GetSimilarItemsAsync(int categoryId, int currentCompanyId, int take = 6);
    }
}
