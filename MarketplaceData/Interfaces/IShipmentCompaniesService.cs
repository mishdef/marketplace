using Domain;
using MarketplaceData.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace MarketplaceData.Interfaces
{
    public interface IShipmentCompaniesService : IRepository<ShipmentCompany>
    {
        List<ShipmentCompany> GetShipmentCompaniesForCompany(int id);
    }
}
