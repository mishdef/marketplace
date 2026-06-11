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
                             .Include(c => c.ShippingCompanies)
                             .FirstOrDefault(c => c.Id == id);
            });
        }

        public override IEnumerable<Company> GetAll()
        {
            return _dbSet.AsNoTracking()
                         .Include(c => c.Owner)
                         .Include(c => c.Employees)
                         .Include(c => c.StoreItems)
                         .Include(c => c.ShippingCompanies)
                         .ToList();
        }

        public async Task UpdateCompanyShipmentsAsync(int companyId, List<int> shipmentCompanyIds)
        {
            var company = await _dbSet.Include(c => c.ShippingCompanies)
                                      .FirstOrDefaultAsync(c => c.Id == companyId);
            if (company != null)
            {
                var newShipments = await _context.Set<Model.ShipmentCompany>()
                                                 .Where(sc => shipmentCompanyIds.Contains(sc.Id))
                                                 .ToListAsync();
                
                company.ShippingCompanies.Clear();
                foreach (var sc in newShipments)
                {
                    company.ShippingCompanies.Add(sc);
                }
                
                await _context.SaveChangesAsync();
                _cache.Remove($"Company_{companyId}");
            }
        }
    }
}
