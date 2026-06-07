using Domain;
using MarketplaceData.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using VetClassLibrary.Model;
using VetClassLibrary.Services;

namespace MarketplaceData.Services
{
    public class ItemService : Repository<Item>, IItemService
    {
        public ItemService(AppDbContext context) : base(context)
        {
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
            return _dbSet.Include(i => i.Category)
                         .Include(i => i.Company)
                         .Where(i => !i.IsDeleted)
                         .ToList();
        }

        public override Item? GetById(int id)
        {
            var item = _dbSet.Include(i => i.Category)
                             .Include(i => i.Company)
                             .FirstOrDefault(i => i.Id == id && !i.IsDeleted);
            if (item == null)
                throw new KeyNotFoundException("Item not found.");
            return item;
        }
    }
}
