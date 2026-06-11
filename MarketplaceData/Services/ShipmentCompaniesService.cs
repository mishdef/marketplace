using Domain;
using MarketplaceData.Interfaces;
using MarketplaceData.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using VetClassLibrary.Services;

namespace MarketplaceData.Services
{
    public class ShipmentCompaniesService : Repository<ShipmentCompany>, IShipmentCompaniesService
    {
        private readonly ICompanyService _companyService;

        public ShipmentCompaniesService(AppDbContext context, ICompanyService companyService) : base(context)
        {
            _companyService = companyService;
        }

        public List<ShipmentCompany> GetShipmentCompaniesForCompany(int id)
        {
            var company = _companyService.GetById(id);

            if (company == null)
            {
                throw new KeyNotFoundException();
            }

            return company.ShippingCompanies;
        }
    }
}
