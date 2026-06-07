using Domain;
using MarketplaceData.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VetClassLibrary.Model;
using VetClassLibrary.Services;
using Microsoft.Extensions.Caching.Memory;

namespace MarketplaceData.Services
{
    public class ItemService : Repository<Item>, IItemService
    {
        private readonly IMemoryCache _cache;

        public ItemService(AppDbContext context, IMemoryCache cache) : base(context)
        {
            _cache = cache;
        }

        public List<Item> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return GetAll().ToList();
            }

            var items = GetAll().Where(i =>
                (i.Name != null && i.Name.Contains(query.ToLower().Trim())) ||
                (i.Description != null && i.Description.Contains(query.ToLower().Trim()))
            ).ToList();
            return items;
        }

        public override IEnumerable<Item> GetAll()
        {
            return _cache.GetOrCreate("AllItems", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return _dbSet.AsNoTracking()
                             .Include(i => i.Category)
                             .Include(i => i.Company)
                             .Where(i => !i.IsDeleted)
                             .ToList();
            }) ?? new List<Item>();
        }

        public override Item? GetById(int id)
        {
            var item = _cache.GetOrCreate($"Item_{id}", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                return _dbSet.AsNoTracking()
                             .Include(i => i.Category)
                             .Include(i => i.Company)
                             .FirstOrDefault(i => i.Id == id && !i.IsDeleted);
            });

            if (item == null)
                throw new KeyNotFoundException("Item not found.");
            
            return item;
        }
    }
}
