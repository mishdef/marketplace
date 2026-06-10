using Domain;
using MarketplaceData.Interfaces;
using MarketplaceData.Model.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VetClassLibrary.Services;
using Microsoft.Extensions.Caching.Memory;

namespace MarketplaceData.Services
{
    public class CompanyService : Repository<Company>, ICompanyService
    {
        private readonly IMemoryCache _cache;

        public CompanyService(AppDbContext context, IMemoryCache cache) : base(context)
        {
            _cache = cache;
        }

        public override Company? GetById(int id)
        {
            return _cache.GetOrCreate($"Company_{id}", entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10);
                
                return _dbSet.AsNoTracking()
                             .Include(c => c.Owner)
                             .Include(c => c.Employees)
                             .Include(c => c.StoreItems)
                             .FirstOrDefault(c => c.Id == id);
            });
        }

        public override IEnumerable<Company> GetAll()
        {
            return _dbSet.AsNoTracking()
                         .Include(c => c.Owner)
                         .Include(c => c.Employees)
                         .Include(c => c.StoreItems)
                         .ToList();
        }
    }
}
