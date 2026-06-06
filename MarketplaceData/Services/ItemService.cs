using Domain;
using MarketplaceData.Interfaces;
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
    }
}
