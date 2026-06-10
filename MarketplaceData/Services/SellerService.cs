using Domain;
using MarketplaceData.Interfaces;
using MarketplaceData.Model.User;
using System;
using System.Collections.Generic;
using System.Text;
using VetClassLibrary.Services;

namespace MarketplaceData.Services
{
    public class SellerService : Repository<SellerInfo>, ISellerService
    {
        public SellerService(AppDbContext context) : base(context)
        {
        }
    }
}
