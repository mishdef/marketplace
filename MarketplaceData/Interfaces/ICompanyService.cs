using Domain;
using MarketplaceData.Model.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketplaceData.Interfaces
{
    public interface ICompanyService : IRepository<Company>
    {
        Task UpdateCompanyShipmentsAsync(int companyId, List<int> shipmentCompanyIds);
    }
}
