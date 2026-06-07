using Domain;
using MarketplaceData.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketplaceData.Interfaces
{
    public interface ICategoryService : IRepository<Category>
    {
        List<Category> GetCategories();
    }
}
