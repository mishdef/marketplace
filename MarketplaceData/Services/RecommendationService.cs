using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketplaceData.Interfaces;
using MarketplaceData.Model.User;
using Microsoft.EntityFrameworkCore;
using VetClassLibrary.Model;
using VetClassLibrary.Services;

namespace MarketplaceData.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly AppDbContext _context;

        public RecommendationService(AppDbContext context)
        {
            _context = context;
        }

        private async Task<ClientInfo?> GetOrCreateClientInfoAsync(int userId)
        {
            var clientInfo = await _context.Clients
                .Include(c => c.ViewedItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (clientInfo == null)
            {
                clientInfo = new ClientInfo
                {
                    UserId = userId,
                    Address = string.Empty,
                    Cart = new MarketplaceData.Model.Cart.Cart()
                };
                _context.Clients.Add(clientInfo);
                await _context.SaveChangesAsync();
            }

            return clientInfo;
        }

        public async Task RecordItemViewAsync(int userId, int itemId)
        {
            var clientInfo = await GetOrCreateClientInfoAsync(userId);
            if (clientInfo == null) return;

            var existingView = clientInfo.ViewedItems.FirstOrDefault(v => v.ItemId == itemId);
            if (existingView != null)
            {
                existingView.ViewedAt = DateTime.Now;
            }
            else
            {
                var newView = new ClientViewedItem
                {
                    ClientInfoId = clientInfo.Id,
                    ItemId = itemId,
                    ViewedAt = DateTime.Now
                };
                _context.ClientViewedItems.Add(newView);
                clientInfo.ViewedItems.Add(newView);
            }

            // Keep only last 30
            if (clientInfo.ViewedItems.Count > 30)
            {
                var oldestItems = clientInfo.ViewedItems
                    .OrderByDescending(v => v.ViewedAt)
                    .Skip(30)
                    .ToList();
                
                _context.ClientViewedItems.RemoveRange(oldestItems);
                foreach (var item in oldestItems)
                {
                    clientInfo.ViewedItems.Remove(item);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<Item>> GetRecentlyViewedItemsAsync(int userId, int take = 6)
        {
            var clientInfo = await _context.Clients
                .Include(c => c.ViewedItems)
                .ThenInclude(v => v.Item)
                .ThenInclude(i => i.Company)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (clientInfo == null) return new List<Item>();

            return clientInfo.ViewedItems
                .OrderByDescending(v => v.ViewedAt)
                .Select(v => v.Item)
                .Take(take)
                .ToList();
        }

        public async Task<List<Item>> GetPersonalizedRecommendationsAsync(int userId, int take = 6)
        {
            var clientInfo = await _context.Clients
                .Include(c => c.ViewedItems)
                .ThenInclude(v => v.Item)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (clientInfo == null || !clientInfo.ViewedItems.Any())
            {
                // Fallback: newest items or random items
                return await _context.Products
                    .Include(i => i.Company)
                    .OrderByDescending(i => i.Id)
                    .Take(take)
                    .ToListAsync();
            }

            // Get top categories the user viewed
            var topCategoryIds = clientInfo.ViewedItems
                .GroupBy(v => v.Item.CategoryId)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .Take(3)
                .ToList();

            var viewedItemIds = clientInfo.ViewedItems.Select(v => v.ItemId).ToList();

            var recommendations = await _context.Products
                .Include(i => i.Company)
                .Where(i => topCategoryIds.Contains(i.CategoryId) && !viewedItemIds.Contains(i.Id))
                .OrderByDescending(i => i.Id)
                .Take(take)
                .ToListAsync();

            return recommendations;
        }

        public async Task<List<Item>> GetCompanyRecommendationsAsync(int companyId, int currentItemId, int take = 6)
        {
            return await _context.Products
                .Include(i => i.Company)
                .Where(i => i.CompanyId == companyId && i.Id != currentItemId)
                .Take(take)
                .ToListAsync();
        }

        public async Task<List<Item>> GetSimilarItemsAsync(int categoryId, int currentCompanyId, int take = 6)
        {
            return await _context.Products
                .Include(i => i.Company)
                .Where(i => i.CategoryId == categoryId && i.CompanyId != currentCompanyId)
                .Take(take)
                .ToListAsync();
        }
    }
}
