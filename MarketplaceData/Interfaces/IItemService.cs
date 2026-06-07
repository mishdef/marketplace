using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using VetClassLibrary.Model;

namespace MarketplaceData.Interfaces
{
    public interface IItemService : IRepository<Item>
    {
        public List<Item> Search(string query);
    }
}
