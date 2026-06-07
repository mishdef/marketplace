using Domain;
using MarketplaceData.Interfaces;
using MarketplaceData.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VetClassLibrary.Model;
using VetClassLibrary.Services;
using Microsoft.Extensions.Caching.Memory;

namespace MarketplaceData.Services
{
    public class CategoryService : Repository<Category>, ICategoryService
    {
        private readonly IMemoryCache _cache;

        public CategoryService(AppDbContext context, IMemoryCache cache) : base(context)
        {
            _cache = cache;
        }

        public override Category? GetById(int id)
        {
            return _cache.GetOrCreate($"Category_{id}", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);

                var category = _dbSet.AsNoTracking()
                                     .Include(c => c.Items)
                                     .Include(c => c.SubCategories)
                                         .ThenInclude(sc => sc.Items)
                                     .Include(c => c.SubCategories)
                                         .ThenInclude(sc => sc.SubCategories) 
                                     .FirstOrDefault(c => c.Id == id);

                if (category == null) return null;

                var currentSubCategories = category.SubCategories?.ToList() ?? new List<Category>();

                while (currentSubCategories.Count > 0)
                {
                    var itemsFromSubs = currentSubCategories
                        .Where(c => c.Items != null)
                        .SelectMany(c => c.Items!)
                        .ToList();

                    if (itemsFromSubs.Count > 0)
                    {
                        category.Items ??= new List<Item>();
                        category.Items.AddRange(itemsFromSubs);
                    }

                    currentSubCategories = currentSubCategories
                        .Where(c => c.SubCategories != null)
                        .SelectMany(c => c.SubCategories!)
                        .ToList();
                }

                return category;
            });
        }

        public List<Category> GetCategories()
        {
            return _cache.GetOrCreate("AllRootCategories", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(15);
                return _dbSet.Where(c => c.ParentCategoryId == null).ToList();
            }) ?? new List<Category>();
        }
    }
}
